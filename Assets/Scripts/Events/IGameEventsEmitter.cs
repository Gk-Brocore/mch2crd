using Game.Card;
using System;

namespace Game.Events
{

    public interface IGameEventInjector
    {
        IGameEventsEmitter GameEventsEmitter { get; set; }
    }

    public interface IGameEventsEmitter
    {
        event Action<ICard, ICard> OnMatch;
        event Action<ICard, ICard> OnMismatch;
        event Action OnQueueCleared;
        event Action<int> OnScoreUpdated;
        event Action<int> OnComboUpdated;

        void RegisterObserver(IGameEventsObserver observer);
        void UnregisterObserver(IGameEventsObserver observer);

        void EmitMatch(ICard first, ICard second);
        void EmitMismatch(ICard first, ICard second);
        void EmitQueueCleared();
        void EmitScoreUpdated(int score);
        void EmitComboUpdated(int combo);
    }
}