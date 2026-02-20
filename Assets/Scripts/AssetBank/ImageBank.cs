using Game.Addressable;
using Game.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Assets
{
    /// <summary>
    /// ScriptableObject that stores a collection of images that can be accessed by id.
    /// Assets are stored as AssetReferences, which allows for asynchronous loading and unloading of assets.
    /// Sprites are loaded asynchronously using the AddressableManager.
    /// </summary>
    [CreateAssetMenu(fileName = "ImageBank", menuName = "ScriptableObjects/ImageBank")]
    public class ImageBank : ScriptableObject , IAssetBank<AssetReference, Sprite>
    {

        public string BankName;
        public Vector2 imageSize;

        public SerializableDictionary<string, AssetReference> entries;

        public Vector2 AssetSize => imageSize;

        public string Name => BankName;

        public SerializableDictionary<string, AssetReference> Collection 
        { 
            get => entries; 
            set => entries = value; 
        }

        public AssetReference GetAssetById(string _id)
        {
            if(entries.TryGet(_id, out AssetReference _entry))
            {
                return _entry;
            }
            return null;
        }

        public async Task<Sprite> GetAsset(string _id)
        {
            var _assetRef = GetAssetById(_id);
            return await AddressableManager.Instance.LoadImageAsync<Sprite>(_assetRef);
        }

        [ContextMenu("Get Names")]
        public void GetNames()
        {
            foreach (var _entry in entries.List)
            {
                var _name = _entry.value.SubObjectName;
                if (!string.IsNullOrEmpty(_name))
                {
                    _entry.key = _name;
                }
            }
        }


        public List<string> GetShuffled(int _count)
        {
            var _output = new List<string>();
            foreach (var _entry in entries.List)
            {
                _output.Add(_entry.key);
            }

            _output.Shuffle();

            if (_count > _output.Count)
            {
                Debug.Log("Invalid Count");
                return _output;
            }

            var _toRemove = _output.Count - _count;


            for (int _index = _toRemove - 1; _index >= 0; _index--)
            {
                _output.RemoveAt(_index);
            }


            return _output;
        }

    }
}