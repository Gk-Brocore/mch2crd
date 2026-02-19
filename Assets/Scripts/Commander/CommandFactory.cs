using Game.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Commander
{
    public interface ICommandFactory
    {
        ICommand CreateReveal(ICard card);
        ICommand CreateHide(ICard card);
        ICommand CreateSetMatched(ICard first, ICard second, bool matched);
    }
    public class CommandFactory : ICommandFactory
    {
        public ICommand CreateReveal(ICard card)
        {
            return new RevealCardCommand(card);
        }

        public  ICommand CreateHide(ICard card)
        {
            return new HideCardCommand(card);
        }

        public ICommand CreateSetMatched(ICard first, ICard second, bool matched)
        {
            return new SetMatchedCommand(first, second, matched);
        }
    }
}
