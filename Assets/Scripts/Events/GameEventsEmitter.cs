using Game.Card;
using System;

namespace Game.Events
{
    public class GameEventsEmitter : IGameEventsEmitter
    {
        public event Action OnGameStart;
        public event Action OnGameComplete;
        public event Action OnGameLoad;
        public event Action<ICard, ICard> OnMatch;
        public event Action<ICard, ICard> OnMismatch;
        public event Action OnQueueCleared;
        public event Action<int> OnScoreUpdated;
        public event Action<int> OnComboUpdated;

        public void RegisterObserver(IGameEventsObserver observer)
        {
            OnGameStart += observer.OnGameStart;
            OnGameComplete += observer.OnGameComplete;
            OnGameLoad += observer.OnGameLoad;
            OnMatch += observer.OnMatch;
            OnMismatch += observer.OnMismatch;
            OnQueueCleared += observer.OnQueueCleared;
            OnScoreUpdated += observer.OnScoreUpdated;
            OnComboUpdated += observer.OnComboUpdated;
        }

        public void UnregisterObserver(IGameEventsObserver observer)
        {
            OnGameStart -= observer.OnGameStart;
            OnGameComplete -= observer.OnGameComplete;
            OnGameLoad -= observer.OnGameLoad;
            OnMatch -= observer.OnMatch;
            OnMismatch -= observer.OnMismatch;
            OnQueueCleared -= observer.OnQueueCleared;
            OnScoreUpdated -= observer.OnScoreUpdated;
            OnComboUpdated -= observer.OnComboUpdated;
        }

        public void EmitGameStart() => OnGameStart?.Invoke();
        public void EmitGameComplete() => OnGameComplete?.Invoke();

        public void EmitLoadGame() => OnGameLoad?.Invoke();

        public void EmitMatch(ICard first, ICard second) => OnMatch?.Invoke(first, second);
        public void EmitMismatch(ICard first, ICard second) => OnMismatch?.Invoke(first, second);
        public void EmitQueueCleared() => OnQueueCleared?.Invoke();
        public void EmitScoreUpdated(int score) => OnScoreUpdated?.Invoke(score);
        public void EmitComboUpdated(int combo) => OnComboUpdated?.Invoke(combo);
    }
}
