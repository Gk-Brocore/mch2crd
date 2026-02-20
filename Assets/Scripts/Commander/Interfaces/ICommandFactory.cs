using Game.Card;

namespace Game.Commander
{
    public interface ICommandFactory
    {
        ICommand CreateReveal(ICard card);
        ICommand CreateHide(ICard card);
        ICommand CreateSetMatched(ICard first, ICard second, bool matched);
    }
}
