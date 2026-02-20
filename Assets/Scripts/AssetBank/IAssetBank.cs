
using Game.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Assets
{

    /// <summary>
    /// interface for an asset bank, which is a collection of assets that can be accessed by id. 
    /// The asset bank can be used to store and retrieve assets, such as images, audio clips, etc. 
    /// The asset bank can also be used to shuffle the assets and return a random selection of assets.
    /// </summary>
    /// <typeparam name="T">Storeable type</typeparam>
    /// <typeparam name="K">Useable Type</typeparam>
    public interface IAssetBank<T,K>
    {
        string Name { get; }
        SerializableDictionary<string, T> Collection { get; set; }
        public Vector2 AssetSize { get; }
        public Task<K> GetAsset(string _id);
        List<string> GetShuffled(int _count);
    }
}
