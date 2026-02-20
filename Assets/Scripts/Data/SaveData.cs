using Game.Layout;
using Game.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData 
{
    public SerializableDictionary<string, GameLevelData> levels;
    public SaveData()
    {
        levels = new SerializableDictionary<string, GameLevelData>();
        levels.New();
    }

    public void AddScore(string _gridId, string _imagesId, int _score)
    {
        var _id = $"{_gridId}_{_imagesId}";
        if (levels.Contains(_id))
        {
            levels.Get(_id).score = _score;
        }
        else
        {
            GameLevelData _newLevel = new GameLevelData
            {
                gridId = _gridId,
                imagesId = _imagesId,
                score = _score,
                cells = new List<CellData>()
            };
            levels.Add(_id, _newLevel);
        }
    }

    public void Add(string _gridId, string _imagesId, List<CellData> _cells)
    {

        var _id = $"{_gridId}_{_imagesId}";

        if (levels.Contains(_id))
        {
            levels.Get(_id).cells = _cells;
        }
        else
        {
            GameLevelData _newLevel = new GameLevelData
            {
                gridId = _gridId,
                imagesId = _imagesId,
                cells = _cells
            };
            levels.Add(_id, _newLevel);
        }
    }

    public void SetCompleted(string _gridId, string _imgId, bool _completed)
    {
        var _id = $"{_gridId}_{_imgId}";
        if (levels.Contains(_id))
        {
            levels.Get(_id).completed = _completed;
        }
    }

    public GameLevelData Get(string _gridId, string _imgId)
    {
        var _id = $"{_gridId}_{_imgId}";
        return levels.Get(_id);
    }

    public bool Contains(string _gridId, string _imgId)
    {
        var _id = $"{_gridId}_{_imgId}";
        return levels.Contains(_id);
    }
}

[Serializable]
public class GameLevelData
{
    public string gridId;
    public string imagesId;
    public List<CellData> cells;


    public int score;
    public int combo;

    public bool completed;

    public int GetMatchedCount()
    {
        int _count = 0;
        foreach (var cell in cells)
        {
            if (cell.matched)
                _count++;
        }
        return _count;
    }


}