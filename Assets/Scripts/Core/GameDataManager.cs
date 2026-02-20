using Game.Assets;
using Game.Data;
using Game.Layout;
using Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Core
{
    public class GameDataManager : MonoBehaviour
    {
        public static GameDataManager Instance { get; private set; }

        [SerializeField]
        private GameDataBank gameDataBank;

        [SerializeField]
        private string currentLayout;
        [SerializeField]
        private string currentImages;

        public static IAssetBank<BaseLayoutSettingsSO, BaseLayoutSettingsSO> Settings => Instance.gameDataBank;
        public static IAssetBank<ImageBank, ImageBank> Images => Instance.gameDataBank;

        public ILayoutSettings Layout;
        public IAssetBank<AssetReferenceSprite, Sprite> ImageBank;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }

            if(gameDataBank == null)
            {
                Utilities.Log("Game Data Manager", "Missig Game Data");
                return;
            }

            Layout = Settings.GetAsset(currentLayout).Result;

            ImageBank = Images.GetAsset(currentImages).Result;
        }


        public async Task<Sprite> GetImage(string _id)
        {
            return await ImageBank.GetAsset(_id);
        }

        public List<string> GetShuffledImageIds(int _count)
        {
            return ImageBank.GetShuffled(_count);
        }

        public void ChangeLayout(string _layout)
        {
            currentLayout = _layout;
            Layout = Settings.GetAsset(_layout).Result;
        }
        public void ChangeLayout(ILayoutSettings _layout)
        {
            currentLayout = _layout.Name;
            Layout = _layout;
        }

        public void ChangeImages(IAssetBank<AssetReferenceSprite, Sprite> value)
        {
            currentImages = value.Name;
            ImageBank = value;
        }

        public bool HasContinue()
        {
            //TODO Add save and load
            return false;
        }
    }
}
