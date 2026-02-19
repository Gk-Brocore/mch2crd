using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Utils
{
    public static class Utilities
    {
        private static System.Random rng = new System.Random();

        public static void Shuffle<T>(this List<T> _list)
        {
            int _next = _list.Count;
            while (_next > 1)
            {
                _next--;
                int _index = rng.Next(_next);
                _list.Swap(_index, _next);
            }
        }

        public static List<T> GetShuffled<T>(this List<T> _list)
        {
            var _outList = new List<T>();
            _outList.AddRange(_list);
            _outList.Shuffle();
            return _outList;
        }

        public static List<T> Swap<T>(this List<T> _list, int _indexA, int _indexB)
        {
            T tmp = _list[_indexA];
            _list[_indexA] = _list[_indexB];
            _list[_indexB] = tmp;
            return _list;
        }

        public static bool debug = true;

        public static void Log(string _tag, string _message)
        {
            if (debug)
            {
                Debug.Log($"[{_tag}] {_message}");
            }
        }
    }

        [System.Serializable]
        public class SerializableDictionary<Key, T>
        {

            public List<KeyValPair<Key, T>> List = new List<KeyValPair<Key, T>>();

            public int Count => List.Count;

            public Key LastKey => List[Count - 1].key;

            public void Clear() => List.Clear();

            public void New() => List = new List<KeyValPair<Key, T>>();

            public KeyValPair<Key, T> Add(Key _key, T _value)
            {
                KeyValPair<Key, T> newVal = new KeyValPair<Key, T>(_key, _value);
                List.Add(newVal);
                return newVal;
            }

            public void Add(KeyValPair<Key, T> _value)
            {
                List.Add(_value);
            }

            public void Remove(Key _key)
            {
                if (TryGet(_key, out KeyValPair<Key, T> obj))
                {
                    List.Remove(obj);
                }
            }

            public void MoveUp(int _index)
            {
                if (_index != 0)
                {
                    List.Swap(_index, _index - 1);
                }
            }

            public void MoveDown(int _index)
            {
                if (_index != List.Count - 1)
                {
                    List.Swap(_index, _index + 1);
                }
            }

            public List<T> ToList()
            {
                List<T> list = new List<T>();
                foreach (var item in List)
                {
                    list.Add(item.value);
                }
                return list;
            }

            public KeyValPair<Key, T> GetData(Key _key)
            {
                KeyValPair<Key, T> _data = null;
                try
                {
                    _data = List.SingleOrDefault(x => x.key.Equals(_key));
                    return _data;
                }
                catch
                {
                    return default;
                }
            }

            public T Get(Key _key)
            {
                KeyValPair<Key, T> _data = null;
                try
                {
                    _data = List.SingleOrDefault(x => x.key.Equals(_key));
                    return _data.value;
                }
                catch
                {
                    return default;
                }
            }

            public bool Contains(Key _key)
            {
                return TryGet(_key, out KeyValPair<Key, T> obj);
            }

            public bool TryGet(Key _key, out T _value)
            {
                foreach (KeyValPair<Key, T> _item in List)
                {
                    if (_item.key.Equals(_key))
                    {
                        _value = _item.value;
                        return true;
                    }
                }
                _value = default;
                return false;
            }

            public bool TryGet(Key _key, out KeyValPair<Key, T> _value)
            {
                foreach (KeyValPair<Key, T> _item in List)
                {
                    if (_item.key.Equals(_key))
                    {
                        _value = _item;
                        return true;
                    }
                }
                _value = default;
                return false;
            }

            public T GetRandom()
            {
                List.Shuffle();
                var idx = UnityEngine.Random.Range(0, List.Count);
                return List[idx].value;
            }

            public void Shuffle() => List.Shuffle();

            public void ForEach(Action<KeyValPair<Key, T>> action) => List.ForEach(action);
        }

        [Serializable]
        public class KeyValPair<Key, T>
        {
            public Key key;
            public T value;

            public KeyValPair(Key key, T value)
            {
                this.key = key;
                this.value = value;
            }
        }
    }

