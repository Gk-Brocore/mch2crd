using Game.Card;
using Game.Core;
using Game.Events;
using Game.Factory;
using Game.Layout;
using Game.Layout.Grid;
using Game.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LayoutController : MonoBehaviour , IGameEventsObserver
{
  
    [Header("Game")]
    [SerializeField] 
    private float firstRevealedTime;

    [Header("UI Elements")]
    [SerializeField]
    private GridUiLayout GridUiLayout;
    [SerializeField]
    private AssetReference cardPrefab;



    private ILayoutCollection<CellData> layout;

    private List<ICard> spawnedCards = new List<ICard>();
    private int matchesFound;
    public Action LevelComplete;
    private CanvasGroup canvasGroup;

    private ICardFactory cardFactory;

    private ILayoutSettings layoutSettings;
    private ILayoutView layoutView;
    void Start()
    {
        canvasGroup = GridUiLayout.GetComponent<CanvasGroup>();
        cardFactory = new AddressableCardFactory(cardPrefab);

        layoutSettings = GameDataManager.Instance.Layout;
        layoutView = GridUiLayout;
    }

    private void OnEnable()
    {
        GameEventsHandler.RegisterObserver(this);
    }
    private void OnDisable()
    {
        GameEventsHandler.UnregisterObserver(this);
    }

    public void StartGame()
    {
        if (!ValidateSetup()) 
            return;
        matchesFound = 0;
        InitilizeLayout(MakeGridCollection());
        SetupUILayout();
        SpawnUICards(false);
        SaveCurrentLayout(true);
    }

    private void InitilizeLayout(ILayoutCollection<CellData> _collection)
    {
        layoutSettings.PrepareLayout();

        layout = _collection;

        for (int y = 0; y < layoutSettings.Height; y++)
        {
            for (int x = 0; x < layoutSettings.Width; x++)
            {
                var _position = new Vector2Int(x, y);
                layout.SetValue(x, y, layoutSettings.CreateCellData(_position));
            }
        }
    }

    private ILayoutCollection<CellData> MakeGridCollection()
    {
        return new GridCollection<CellData>(
                                layoutSettings.Width,
                                layoutSettings.Height,
                                layoutSettings.CellSize,
                                layoutSettings.CellSpacing,
                                layoutSettings.Origin,
                                CreateNewCell);
    }

    private CellData CreateNewCell() => new("", State.Hidden);

    private void SetupUILayout()
    {
        if (GridUiLayout == null)
        {
            GridUiLayout = gameObject.AddComponent<GridUiLayout>();
            layoutView = GridUiLayout;
        }

        GridUiLayout.fitType = GridUiLayout.FitType.FIXEDROWS;
        layoutView.Rows = layoutSettings.Height;
        layoutView.Columns = layoutSettings.Width;
        layoutView.Spacing = layoutSettings.CellSpacing;
        layoutView.CellSize = layoutSettings.CellSize;
        layoutView.Padding = layoutSettings.Padding;
    }

    private bool ValidateSetup()
    {
        if (layoutSettings == null || cardPrefab == null)
        {
            Debug.LogError("GridUIController setup missing references!", this);
            return false;
        }
        return true;
    }

    #region Save/Load
    public void LoadGame()
    {
        if (!ValidateSetup()) 
            return;
        List<CellData> savedGrid = null; //TODO: Load from file or player prefs
        matchesFound = GetMatchCount(savedGrid);
        InitilizeLayout(MakeGridCollection());
        layout.FromList(savedGrid);
        SetupUILayout();
        SpawnUICards(true);
    }
    public int GetMatchCount(List<CellData> saved)
    {
        int count = 0;
        foreach (var _cell in saved)
        {
            if (_cell.matched)
                count++;
        }
        return count / 2;
    }
    private void SaveCurrentLayout(bool newGame = false)
    {
        if (layout == null)
            return;
        var _fromGrid = layout.ToList();
        //TODO: Save to file or player prefs
    }

    #endregion

    #region Cards

    private async void SpawnUICards(bool _load)
    {
        canvasGroup.interactable = false;
        ClearOldCards();

        var _imagesIds = GameDataManager.Instance.GetShuffledImageIds(layoutSettings.TotalCombinations);
        var _deck = _load ? new List<string>() : MakeDeck(_imagesIds);

        int _index = 0;
        for (int y = 0; y < layoutSettings.Height; y++)
        {
            for (int x = 0; x < layoutSettings.Width; x++)
            {
                var _gridPos = new Vector2Int(x, y);
                var _data = layout.GetValue(x, y);

                ICard _card = await cardFactory.CreateAsync(layoutView.transform, $"Card_{x}_{y}");

                spawnedCards.Add(_card);

                if (_data.state != State.Hidden && !_data.matched)
                {
                    string _id = _load ? _data.id : _deck[_index];
                    if (!_load)
                        _data.id = _id;

                    var _sprite = await GameDataManager.Instance.GetImage(_id);
                    _card.Initialize(_id, _gridPos, _sprite, GameDataManager.Instance.ImageBank.AssetSize);
                    _index++;
                    continue;
                }

                _card.Initialize($"Card_{x}_{y}", _gridPos, null, Vector2.zero);
            }
        }

        foreach (var card in spawnedCards)
        {
            card.Reveal();
        }
        await Task.Delay(TimeSpan.FromSeconds(firstRevealedTime));
        foreach (var _card in spawnedCards)
        {
            _card.Hide();
        }
        canvasGroup.interactable = true;
    }

    private void ClearOldCards()
    {
        for (int _index = 0; _index < spawnedCards.Count; _index++)
        {
            ICard _card = spawnedCards[_index];
            if (_card != null)
                Destroy(_card.gameObject);
        }
        spawnedCards.Clear();
    }

    private List<string> MakeDeck(List<string> input)
    {
        List<string> all = new();
        all.AddRange(input);
        all.AddRange(input);
        all.Shuffle();
        return all;
    }

    public void SetCellState(Vector2Int gridPos, bool newState)
    {
        var _cell = layout.GetValue(gridPos.x, gridPos.y);
        _cell.matched = newState;
    }

    private IEnumerator WaitForAnim(ICard card)
    {
        while (!card.IsAnimDone)
        {
            yield return null;
        }
        card.Hide();
    }

    #endregion

    #region Events

    public void OnMatch(ICard first, ICard second)
    {
        SetCellState(first.Coordinates, true);
        SetCellState(second.Coordinates, true);

        StartCoroutine(WaitForAnim(first));
        StartCoroutine(WaitForAnim(second));
        matchesFound++;

        if (matchesFound == layoutSettings.TotalCombinations)
        {
            GameEventsHandler.Instance.EmitGameComplete();
        }
    }

    public void OnMismatch(ICard first, ICard second)
    {
    }
    public void OnQueueCleared()
    {
    }
    public void OnScoreUpdated(int score)
    {
    }
    public void OnComboUpdated(int combo)
    {
    }
    public void OnGameStart()
    {
        StartGame();
    }
    public void OnGameComplete()
    {
    }

    public void OnGameLoad()
    {
        LoadGame();
    }

    #endregion

}
