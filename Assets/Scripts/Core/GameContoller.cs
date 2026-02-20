using Game.Card;
using Game.Commander;
using Game.Events;
using Game.Input;
using Game.Utils;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Game.Core
{
    /// <summary>
    /// The GameController class serves as the central hub for managing the core game logic of the memory game. 
    /// It integrates the card selection and matching processes with the game's scoring and combo systems. 
    /// The GameController listens for card match and mismatch events from the GameCommander, 
    /// updates the player's score and combo count accordingly, 
    /// and emits relevant game events to notify other parts of the system about changes in game state, 
    /// such as score updates, combo updates, matches, mismatches, and queue clearing.
    /// </summary>
    public class GameContoller : MonoBehaviour , IGameEventsObserver
    {
        [Header("Debug Settings")]
        public bool debugMode = true;
        [Header("Dependencies")]
        [SerializeField] private InputHandler inputProviderBehaviour;
        [SerializeField] private MonoBehaviour selectionBehaviour;

        [Header("Scoring Settings")]
        public int baseMatchScore = 100;
        public int comboBonus = 25;
        public float comboDecayTime = 3f;
        [Header("Audio")]
        public string mismatchSfx = "Mismatch";
        public string matchSfx = "Match";
        [Range(0f, 10f)] public float cheerChance = 8f;

        [SerializeField] private int score;
        [SerializeField] private int comboCount;

        private IGameEventsEmitter eventEmitter;
        private IInputProvider inputProvider;
        private ICardSelector cardSelector;
        private IGameCommander commander;
        private float lastMatchTime;


        private void OnValidate()
        {
            Utilities.debug = debugMode;
        }

        private void Awake()
        {

            inputProvider = inputProviderBehaviour.InputProvider;
            cardSelector = selectionBehaviour as ICardSelector;

            if (inputProvider == null || cardSelector == null)
            {
                Debug.LogError("Missing IInputProvider or ISelectionResolver references.", this);
                enabled = false;
                return;
            }

            commander = new GameCommander(cardSelector, new CardMatchProcess(),new CommandInvoker(),new CommandFactory());

            eventEmitter = GameEventsHandler.Instance;

            commander.OnMatch += HandleMatch;
            commander.OnMismatch += HandleMismatch;
        }

        private void OnEnable()
        {
            if (inputProvider == null)
                return;

            inputProvider.OnClick += commander.HandleSelect;
            GameEventsHandler.RegisterObserver(this);
        }
        private void OnDisable()
        {
            if (inputProvider == null)
                return;

            inputProvider.OnClick -= commander.HandleSelect;
            GameEventsHandler.UnregisterObserver(this);
        }



        void Start()
        {
            ResetScore();
        }

        private void HandleMatch(ICard _first, ICard _second)
        {
            eventEmitter.EmitMatch(_first, _second);
            AudioConductor.PlaySfx(matchSfx);
            if (UnityEngine.Random.Range(0, 10) <= cheerChance)
                AudioConductor.PlayCheer();

            UpdateCombo();
            int points = baseMatchScore + (comboBonus * (comboCount - 1));
            score += points;
            eventEmitter.EmitScoreUpdated(score);
            eventEmitter.EmitComboUpdated(comboCount);
            eventEmitter.EmitQueueCleared();
        }

        private void HandleMismatch(ICard _first, ICard _second)
        {
            eventEmitter.EmitMismatch(_first, _second);
            comboCount = 0;
            eventEmitter.EmitComboUpdated(comboCount);
            _first.Mismatch();
            _second.Mismatch();
            eventEmitter.EmitQueueCleared();

            AudioConductor.PlaySfx(mismatchSfx);
        }

        private void UpdateCombo()
        {
            if (Time.time - lastMatchTime <= comboDecayTime)
                comboCount += 1;
            else
                comboCount = 1;

            lastMatchTime = Time.time;
        }

        public void ResetScore()
        {
            score = 0;
            comboCount = 0;
            lastMatchTime = 0;
            eventEmitter.EmitScoreUpdated(score);
            eventEmitter.EmitComboUpdated(comboCount);
            commander.HandleClear();
           
        }

        public void OnGameStart()
        {
            ResetScore();
        }

        public void OnGameComplete()
        {
            AudioConductor.PlaySfx("Win");
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
        }

        public void OnComboUpdated(int combo)
        {
        }

        public void OnGameLoad()
        {
            ResetScore() ;
        }
    }
}