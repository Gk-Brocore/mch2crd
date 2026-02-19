using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Game.Addressable
{

    /// <summary>
    /// Singleton AddressableManager
    /// Features:
    /// - Generic Load/Unload with reference counting.
    /// - LRU cache with max entries (evicts least-recently-used assets when over limit).
    /// - Instantiate/Release instances (Addressables.InstantiateAsync) with optional pooling.
    /// - Preload and batch operations, cancellation and timeouts support.
    /// - Safe Addressables.Release and Addressables.ReleaseInstance usage.
    /// </summary>
    public class AddressableManager : MonoBehaviour
    {
        #region Singleton
        public static AddressableManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
           // DontDestroyOnLoad(gameObject);
            Addressables.InitializeAsync();
            Application.lowMemory += OnLowMemory;
        }

        private void OnDestroy()
        {
            Application.lowMemory -= OnLowMemory;
        }
        #endregion

        #region Config
        [Header("Cache / Memory")]
        [Tooltip("Maximum number of loaded asset entries kept in cache (not instances). Exceeding this triggers LRU eviction.")]
        public int maxCachedAssets = 50;

        [Tooltip("Whether to use prefab pooling for instantiated prefabs.")]
        public bool enablePrefabPooling = true;

        [Tooltip("Maximum pooled instances per prefab key.")]
        public int maxPoolPerKey = 10;
        #endregion

        #region Internal structures

        // Stores loaded asset handle and ref count + last used tick for LRU
        private class CacheEntry
        {
            public AsyncOperationHandle handle;
            public int refCount;
            public long lastUsedTick;
        }

        // Pool entry for prefab instances
        private class PoolEntry
        {
            public Queue<GameObject> pooled = new Queue<GameObject>();
        }

        // key -> CacheEntry
        private readonly Dictionary<object, CacheEntry> _cache = new Dictionary<object, CacheEntry>();

        // LRU: sorted by lastUsedTick; we maintain ticks manually via counters
        private long _tick = 0;

        // prefab key -> pool
        private readonly Dictionary<object, PoolEntry> _pools = new Dictionary<object, PoolEntry>();

        private readonly object threadLock = new object();

        #endregion

        #region Public API (Loads/Unloads)

        /// <summary>
        /// Load an asset of type T by addressable key.
        /// Multiple calls increase ref-count; call Unload when done.
        /// </summary>
        public async Task<T> LoadAsync<T>(object _key, int _timeoutMs = 0, CancellationToken _cancellationToken = default)
        {
            // if already loaded
            CacheEntry _entry;
            lock (threadLock)
            {
                if (_cache.TryGetValue(_key, out _entry))
                {
                    _entry.refCount++;
                    _entry.lastUsedTick = ++_tick;
                    if (_entry.handle.IsDone && _entry.handle.Status == AsyncOperationStatus.Succeeded)
                        return (T)_entry.handle.Result;
                    // If handle is still running, we'll await it below
                }
            }

            // Start load
            AsyncOperationHandle<T> _handle = Addressables.LoadAssetAsync<T>(_key);

            // Wrap cancellation and timeout manually
            Task<T> _waitTask = WaitForHandle(_handle, _cancellationToken, _timeoutMs);

            T _result = await _waitTask.ConfigureAwait(false);

            lock (threadLock)
            {
                if (!_cache.TryGetValue(_key, out _entry))
                {
                    _entry = new CacheEntry
                    {
                        handle = _handle,
                        refCount = 1,
                        lastUsedTick = ++_tick
                    };
                    _cache[_key] = _entry;
                }
                else
                {
                    // entry existed but maybe it was still loading
                    // ensure the handle stored is the completed one (Addressables returns same handle for same key)
                    _entry.handle = _handle;
                    _entry.refCount++; // account for this call
                    _entry.lastUsedTick = ++_tick;
                }
                TryEvictIfNeeded();
            }

            return _result;
        }

        /// <summary>
        /// Shortcut for loading Sprite or Texture2D specifically.
        /// Key can be addressable string, label, or AssetReference.
        /// </summary>
        public Task<T> LoadImageAsync<T>(object _key, int _timeoutMs = 0, CancellationToken _cancellationToken = default) where T : UnityEngine.Object
        {
            // images are just typed loads; this method exists to make intent clear
            return LoadAsync<T>(_key, _timeoutMs, _cancellationToken);
        }

        /// <summary>
        /// Decrement ref-count and release asset if count reaches zero.
        /// Safe to call multiple times; passing a key that isn't loaded logs a warning.
        /// </summary>
        public void Unload(object _key)
        {
            lock (threadLock)
            {
                if (!_cache.TryGetValue(_key, out CacheEntry _entry))
                {
                    Debug.LogWarning($"AddressableManager.Unload: key not found: {_key}");
                    return;
                }

                _entry.refCount = Mathf.Max(0, _entry.refCount - 1);
                _entry.lastUsedTick = ++_tick;

                if (_entry.refCount == 0)
                {
                    // Release immediately to free memory
                    ReleaseEntry(_entry);
                    _cache.Remove(_key);
                }
            }
        }

        #endregion

        #region Instantiate / Release Instances with optional pooling

        /// <summary>
        /// Instantiate a prefab addressably. If pooling is enabled, will reuse instances when available.
        /// Returns the instantiated GameObject (active by default).
        /// Caller is responsible for calling ReleaseInstance to return the instance (or AddressableManager.ReleaseInstance).
        /// </summary>
        public async Task<GameObject> InstantiateAsync(object prefabKey, Vector3 position, Quaternion rotation, Transform parent = null, int timeoutMs = 0, CancellationToken cancellationToken = default)
        {
            // Pool check
            if (enablePrefabPooling)
            {
                lock (threadLock)
                {
                    if (_pools.TryGetValue(prefabKey, out PoolEntry pool) && pool.pooled.Count > 0)
                    {
                        var go = pool.pooled.Dequeue();
                        if (go != null)
                        {
                            go.transform.SetParent(parent, false);
                            go.transform.localPosition = position;
                            go.transform.localRotation = rotation;
                            go.SetActive(true);
                            return go;
                        }
                    }
                }
            }

            var _handle = Addressables.InstantiateAsync(prefabKey, position, rotation, parent);
            GameObject _instance = await WaitForHandle(_handle, cancellationToken, timeoutMs);
            CacheEntry _entry = null;
            lock (threadLock)
            {
                if (!_cache.TryGetValue(prefabKey, out _entry))
                {
                    _entry = new CacheEntry 
                    { 
                        refCount = 1, 
                        lastUsedTick = ++_tick ,
                        handle = _handle
                    };
                    _cache[prefabKey] = _entry;
                }
                else
                {
                    _entry.handle = _handle;
                    _entry.refCount++;
                    _entry.lastUsedTick = ++_tick;
                }
                TryEvictIfNeeded();
            }


            return _instance;
        }


        /// <summary>
        /// Release an instantiated GameObject previously created with InstantiateAsync.
        /// If pooling is enabled and pool has space, the instance is returned to pool (deactivated).
        /// Otherwise uses Addressables.ReleaseInstance.
        /// </summary>
        public void ReleaseInstance(object _prefabKey, GameObject _instance)
        {
            if (_instance == null)
                return;

            if (enablePrefabPooling)
            {
                lock (threadLock)
                {
                    if (!_pools.TryGetValue(_prefabKey, out PoolEntry pool))
                    {
                        pool = new PoolEntry();
                        _pools[_prefabKey] = pool;
                    }

                    if (pool.pooled.Count < maxPoolPerKey)
                    {
                        // deactivate and enqueue
                        _instance.SetActive(false);
                            _instance.transform.SetParent(transform, false); // keep out of scene root
                        pool.pooled.Enqueue(_instance);

                        // Decrement ref-count for prefab key (because we still hold base prefab ref until pooled instance is destroyed)
                        if (_cache.TryGetValue(_prefabKey, out CacheEntry cacheEntry))
                        {
                            cacheEntry.refCount = Mathf.Max(0, cacheEntry.refCount - 1);
                            cacheEntry.lastUsedTick = ++_tick;
                            if (cacheEntry.refCount == 0)
                            {
                                ReleaseEntry(cacheEntry);
                                _cache.Remove(_prefabKey);
                            }
                        }
                        return;
                    }
                }
            }

            // Not pooled or pool full -> actually release instance
            Addressables.ReleaseInstance(_instance);

            // Also decrement ref-count for base prefab because instance is gone
            lock (threadLock)
            {
                if (_cache.TryGetValue(_prefabKey, out CacheEntry _cacheEntry))
                {
                    _cacheEntry.refCount = Mathf.Max(0, _cacheEntry.refCount - 1);
                    _cacheEntry.lastUsedTick = ++_tick;
                    if (_cacheEntry.refCount == 0)
                    {
                        ReleaseEntry(_cacheEntry);
                        _cache.Remove(_prefabKey);
                    }
                }
            }
        }

        #endregion

        #region Preload / Batch

        /// <summary>
        /// Preload multiple keys (assets) concurrently. Returns when all loaded or when canceled/timeout.
        /// Each asset will have its refCount incremented by 1.
        /// </summary>
        public async Task PreloadAsync(IEnumerable<object> _keys, int _timeoutMsPerKey = 0, CancellationToken _cancellationToken = default)
        {
            List<Task> _tasks = new List<Task>();
            foreach (var _key in _keys)
            {
                _tasks.Add(LoadAsync<UnityEngine.Object>(_key, _timeoutMsPerKey, _cancellationToken));
            }

            await Task.WhenAll(_tasks).ConfigureAwait(false);
        }

        #endregion

        #region Utilities & Memory management

        /// <summary>
        /// Force release and clear all cached assets and pooled instances.
        /// Use with caution.
        /// </summary>
        public void ReleaseAll()
        {
            lock (threadLock)
            {
                // release cached assets
                foreach (var _kv in _cache)
                {
                    try
                    {
                        ReleaseEntry(_kv.Value);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
                _cache.Clear();

                // destroy pooled instances
                foreach (var _kv in _pools)
                {
                    var _queue = _kv.Value.pooled;
                    while (_queue.Count > 0)
                    {
                        var go = _queue.Dequeue();
                        if (go != null)
                            Addressables.ReleaseInstance(go);
                    }
                }
                _pools.Clear();
            }
            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// Called on low memory signal - try to aggressively free caches.
        /// </summary>
        private void OnLowMemory()
        {
            Debug.LogWarning("[AddressableManager] Low memory signal - clearing non-referenced caches and shrinking pools.");
            lock (threadLock)
            {
                // release entries with refCount == 0
                var _toRemove = new List<object>();
                foreach (var _kv in _cache)
                {
                    if (_kv.Value.refCount == 0)
                    {
                        ReleaseEntry(_kv.Value);
                        _toRemove.Add(_kv.Key);
                    }
                }
                foreach (var _key in _toRemove)
                    _cache.Remove(_key);

                // trim pools to 1 per key
                foreach (var _kv in _pools)
                {
                    var _queue = _kv.Value.pooled;
                    while (_queue.Count > 1)
                    {
                        var _go = _queue.Dequeue();
                        if (_go != null)
                            Addressables.ReleaseInstance(_go);
                    }
                }
            }
            GC.Collect();
            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// Evict least-recently-used entries if we are above maxCachedAssets.
        /// Only evicts entries with refCount == 0 (safe to free).
        /// </summary>
        private void TryEvictIfNeeded()
        {
            if (_cache.Count <= maxCachedAssets)
                return;

            // build list of evictable entries (refCount == 0)
            var _evictable = new List<KeyValuePair<object, CacheEntry>>();
            foreach (var _kv in _cache)
                if (_kv.Value.refCount == 0)
                    _evictable.Add(_kv);

            // sort by lastUsedTick ascending (oldest first)
            _evictable.Sort((a, b) => a.Value.lastUsedTick.CompareTo(b.Value.lastUsedTick));

            int _target = Math.Max(0, maxCachedAssets);
            foreach (var _kv in _evictable)
            {
                if (_cache.Count <= _target)
                    break;
                ReleaseEntry(_kv.Value);
                _cache.Remove(_kv.Key);
            }
        }

        /// <summary>
        /// Release a CacheEntry's handle properly.
        /// </summary>
        private void ReleaseEntry(CacheEntry _entry)
        {
            try
            {
                if (_entry == null)
                    return;
                if (_entry.handle.IsValid())
                {
                    // handle might be an instantiated handle;
                    try
                    {
                        Addressables.Release(_entry.handle);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"AddressableManager.ReleaseEntry: Addressables.Release threw: {e.Message}");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        #endregion

        #region Helper: WaitForHandle with cancellation / timeout

        /// <summary>
        /// Waits for AsyncOperationHandle<T> to complete, supports cancellation and timeout.
        /// Throws OperationCanceledException on cancel or TimeoutException on timeout.
        /// </summary>
        private async Task<T> WaitForHandle<T>(AsyncOperationHandle<T> _handle, CancellationToken _cancellationToken, int _timeoutMs)
        {
            // Fast path
            if (_handle.IsDone)
            {
                if (_handle.Status == AsyncOperationStatus.Succeeded)
                    return _handle.Result;
                throw new InvalidOperationException($"Addressables load failed: {_handle.OperationException}");
            }

            var _tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);

            void Completed(AsyncOperationHandle<T> _h)
            {
                if (_h.Status == AsyncOperationStatus.Succeeded)
                    _tcs.TrySetResult(_h.Result);
                else
                    _tcs.TrySetException(new InvalidOperationException($"Addressables load failed: {_h.OperationException}"));
            }

            _handle.Completed += Completed;

            using (var _cts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken))
            {
                if (_timeoutMs > 0)
                    _cts.CancelAfter(_timeoutMs);

                try
                {
                    using (_cts.Token.Register(() =>
                    {
                        var ex = new OperationCanceledException("Load cancelled or timed out.");
                        _tcs.TrySetException(ex);
                    }))
                    {
                        var _result = await _tcs.Task.ConfigureAwait(false);
                        return _result;
                    }
                }
                finally
                {
                    _handle.Completed -= Completed;
                }
            }
        }

        #endregion

        #region Debug 

        /// <summary>
        /// Simple debug dump to show cached assets and pool sizes.
        /// </summary>
        [ContextMenu("Debug")]
        public void PrintDebug()
        {
            Debug.Log(DebugDump());
        }
        public string DebugDump()
        {
            System.Text.StringBuilder _sb = new System.Text.StringBuilder();
            _sb.AppendLine("AddressableManager Debug Dump:");
            _sb.AppendLine($"Cached assets: {_cache.Count}");
            foreach (var _kv in _cache)
            {
                _sb.AppendLine($" key={_kv.Key} ref={_kv.Value.refCount} last={_kv.Value.lastUsedTick}");
            }
            _sb.AppendLine($"Pools: {_pools.Count}");
            foreach (var _kv in _pools)
            {
                _sb.AppendLine($" key={_kv.Key} poolSize={_kv.Value.pooled.Count}");
            }
            return _sb.ToString();
        }

        #endregion
    }
}