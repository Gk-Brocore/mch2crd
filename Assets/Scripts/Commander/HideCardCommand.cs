using Game.Card;

namespace Game.Commander
{
    public class HideCardCommand : ICommand
    {
        private readonly ICard card;
        private bool previousRevealed;

        public HideCardCommand(ICard _card)
        {
            card = _card;
        }

        public void Execute()
        {
            previousRevealed = card.IsRevealed;
            card.Hide();
        }

        public void Undo()
        {
            if (previousRevealed)
                card.Reveal();
        }
    }
}
