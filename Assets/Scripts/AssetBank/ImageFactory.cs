using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Assets
{
    /// <summary>
    /// Factory for creating and managing images (sprites) in the game. 
    /// This is just a wrappe around IAssetBank that provides a convenient interface for accessing sprites by their unique identifiers.
    /// It uses IAssetBank to load and cache sprites based on their unique identifiers.
    /// ImageBank is a ScriptableObject that holds references to the sprites and implements the IAssetBank interface.
    /// </summary>
    public class ImageFactory : MonoBehaviour
    {
        /// <summary>
        /// Reference to the ImageBank ScriptableObject that holds the sprite references.
        /// Can be changed to other implementations of IAssetBank if needed,
        /// allowing for flexibility in how images are stored and accessed.
        /// </summary>
        [SerializeField]
        private ImageBank imageBank;

        /// <summary>
        /// Provides access to the IAssetBank interface for retrieving sprites by their unique identifiers.
        /// Can be changed to other implementations of IAssetBank if needed, 
        /// allowing for flexibility in how images are stored and accessed.
        /// </summary>
        public IAssetBank<AssetReferenceSprite, Sprite> AssetBank => imageBank;
        public async Task<Sprite> GetImage(string _id)
        {
            return await AssetBank.GetAsset(_id);
        }

        public List<string> GetShuffledImageIds(int _count)
        {
            return AssetBank.GetShuffled(_count);
        }
    }
}

