using Game.Assets;
using Game.Card;
using Game.Data;
using Game.Events;
using Game.Layout;
using Game.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SocialPlatforms.Impl;

namespace Game.Core
{
    public class GameDataManager : MonoBehaviour , IGameEventsObserver
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

        private SaveData saveData;

        private int score;
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
            Load();
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
            return HasSave(currentLayout, currentImages) && !IsLevelCompleted(currentLayout, currentImages);
        }
        public void Save()
        {

            string _jsonData = JsonUtility.ToJson(saveData);

            string _path = Path.Combine(Application.persistentDataPath, "gamesave.json");
            File.WriteAllText(_path, _jsonData);

            Debug.Log($"Game saved to {_path}");


        }


        public bool Load()
        {
            var _path = Path.Combine(Application.persistentDataPath, "gamesave.json");
            if (File.Exists(_path))
            {
                string _jsonData = File.ReadAllText(_path);
                saveData = JsonUtility.FromJson<SaveData>(_jsonData);
                return true;
            }
            else
            {
                saveData = new SaveData();
                Save();
            }
            return false;
        }

        public List<CellData> GetGridData()
        {
            return GetGridData(currentLayout, currentImages);
        }

        public List<CellData> GetGridData(string gridName, string bankName)
        {

            GameLevelData _saveData = saveData.Get(gridName, bankName);
            Events.GameEventsHandler.Instance.EmitScoreUpdated(_saveData.score);
            return _saveData?.cells;
        }

        public void SaveGridData(List<CellData> cells)
        {
            SaveGridData(currentLayout,currentImages, cells);
        }

        public void SaveGridData(string gridName, string bankName, List<CellData> fromGrid)
        {
            saveData.Add(gridName, bankName, fromGrid);
            saveData.AddScore(gridName, bankName, score);
            Save();
        }

        public void SetLevelCompleted(bool completed)
        {
            SetLevelCompleted(currentLayout,currentImages,completed);
        }

        public void SetLevelCompleted(string gridName, string bankName, bool completed)
        {
            saveData.SetCompleted(gridName, bankName, completed);
            Save();
        }

        public bool HasSave(string gridName, string bankName)
        {
            return saveData.Contains(gridName, bankName);
        }

        public bool IsLevelCompleted(string gridName, string bankName)
        {
            var _levelData = saveData.Get(gridName, bankName);
            if (_levelData != null)
            {
                return _levelData.completed;
            }
            return false;
        }

        public void OnGameStart()
        {
        }

        public void OnGameComplete()
        {
        }

        public void OnGameLoad()
        {
        }

        public void OnMatch(ICard first, ICard second)
        {
        }

        public void OnMismatch(ICard first, ICard second)
        {
        }

        public void OnQueueCleared()
        {
        }

        public void OnScoreUpdated(int score)
        {
            this.score = score;
        }

        public void OnComboUpdated(int combo)
        {
        }
    }
}
