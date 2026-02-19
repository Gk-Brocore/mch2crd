using Game.Card;

namespace Game.Events
{
    public interface IGameEventsObserver
    {
        void OnMatch(ICard first, ICard second);
        void OnMismatch(ICard first, ICard second);
        void OnQueueCleared();
        void OnScoreUpdated(int score);
        void OnComboUpdated(int combo);
    }
}