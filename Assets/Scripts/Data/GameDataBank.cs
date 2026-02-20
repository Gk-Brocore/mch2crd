using Game.Assets;
using Game.Layout;
using Game.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TextCore.Text;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "GameBank", menuName = "ScriptableObjects/Game Bank") ]
    public class GameDataBank : ScriptableObject , IAssetBank<BaseLayoutSettingsSO, BaseLayoutSettingsSO> , IAssetBank<ImageBank, ImageBank>
    {
        public SerializableDictionary<string,BaseLayoutSettingsSO> Settings;
        public SerializableDictionary<string, ImageBank> Images;

        SerializableDictionary<string, BaseLayoutSettingsSO> IAssetBank<BaseLayoutSettingsSO, BaseLayoutSettingsSO>.Collection 
        { 
            get => Settings; 
            set => Settings = value;
        }
        SerializableDictionary<string, ImageBank> IAssetBank<ImageBank, ImageBank>.Collection 
        { 
            get => Images;
            set => Images = value;
        }

        Task<BaseLayoutSettingsSO> IAssetBank<BaseLayoutSettingsSO, BaseLayoutSettingsSO>.GetAsset(string _id)
        {
            return Task.FromResult<BaseLayoutSettingsSO>(Settings.Get(_id));
        }

        Task<ImageBank> IAssetBank<ImageBank, ImageBank>.GetAsset(string _id)
        {
            return Task.FromResult(Images.Get(_id));
        }
        public Vector2 AssetSize => throw new System.NotImplementedException();

        public List<string> GetShuffled(int _count)
        {
            return new List<string>(_count);
        }

    }
}
