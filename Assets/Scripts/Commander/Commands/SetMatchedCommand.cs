

using Game.Card;

namespace Game.Commander
{
    public class SetMatchedCommand : ICommand
    {
        private readonly ICard first;
        private readonly ICard second;
        private readonly bool matched;

        private bool prevFirst;
        private bool prevSecond;

        public SetMatchedCommand(ICard _first, ICard _second, bool _matched)
        {
            first = _first;
            second = _second;
            matched = _matched;
        }

        public void Execute()
        {
            prevFirst = first.IsMatched;
            prevSecond = second.IsMatched;
            first.SetMatched(matched);
            second.SetMatched(matched);
        }

        public void Undo()
        {
            first.SetMatched(prevFirst);
            second.SetMatched(prevSecond);
        }
    }
}
