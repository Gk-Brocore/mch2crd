using Game.Assets;
using Game.Card;
using Game.Core;
using Game.Events;
using Game.Layout;
using Game.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameUIController : MonoBehaviour , IGameEventsObserver
{

    public BaseUI pnl_Grids;
    public BaseUI pnl_Images;
    public BaseUI pnl_Save;
    public HUDUi pnl_Hud;
    public LevelCompleteUi pnl_LevelComplete;


    private void Awake()
    {
        pnl_Grids.Init(this);
        pnl_Images.Init(this);
        pnl_Save.Init(this);
        pnl_Hud.Init(this);
        pnl_LevelComplete.Init(this);
    }

    private void OnEnable()
    {
        GameEventsHandler.RegisterObserver(this);
    }
    private void OnDisable()
    {
        GameEventsHandler.UnregisterObserver(this);
    }

    public void OnLayoutChange(ILayoutSettings layoutSettings)
    {
        GameDataManager.Instance.ChangeLayout(layoutSettings);
    }


    public void OnImage(IAssetBank<AssetReference, Sprite> value)
    {
        GameDataManager.Instance.ChangeImages(value);
        if(GameDataManager.Instance.HasContinue())
        {
            pnl_Save.Show();
        }
        else
        {
            NewGame();
        }
    }

    public void NewGame()
    {
        pnl_Hud.Show();
        pnl_Save.Hide();
        pnl_LevelComplete.Hide();
        GameEventsHandler.Instance.EmitGameStart();
    }

    public void LoadGame()
    {
        pnl_Hud.Show();
        pnl_Save.Hide();
        pnl_LevelComplete.Hide();
        GameEventsHandler.Instance.EmitLoadGame();
    }
    public void Menu()
    {
        pnl_Hud.Hide();
        pnl_LevelComplete.Hide();
        pnl_Grids.Show();
    }

    public void OnGameStart()
    {
    }

    public void OnGameComplete()
    {
        pnl_Hud.Hide();
        pnl_LevelComplete.Show();
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
        pnl_Hud.UpdateScore(score);
        pnl_LevelComplete.UpdateScore(score);
    }

    public void OnComboUpdated(int combo)
    {
        pnl_Hud.UpdateCombo(combo);
    }
}
