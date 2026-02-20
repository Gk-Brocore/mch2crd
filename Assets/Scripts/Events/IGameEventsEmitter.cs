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

        void RegisterObserver(IGameEventsObserver observer);
        void UnregisterObserver(IGameEventsObserver observer);

        void EmitGameStart();
        void EmitGameComplete();

        void EmitLoadGame();
        void EmitMatch(ICard first, ICard second);
        void EmitMismatch(ICard first, ICard second);
        void EmitQueueCleared();
        void EmitScoreUpdated(int score);
        void EmitComboUpdated(int combo);
    }
}