

using Game.Card;

namespace Game.Commander
{
    public class RevealCardCommand : ICommand
    {
        private readonly ICard card;
        private bool previousRevealed;

        public RevealCardCommand(ICard _card)
        {
            card = _card;
        }

        public void Execute()
        {
            previousRevealed = card.IsRevealed;
            card.Reveal();
        }

        public void Undo()
        {
            if (!previousRevealed)
                card.Hide();
        }
    }
}
