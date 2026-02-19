using Game.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Commander
{
    public static class CommandFactory
    {
        public static ICommand CreateReveal(ICard card)
        {
            return new RevealCardCommand(card);
        }

        public static ICommand CreateHide(ICard card)
        {
            return new HideCardCommand(card);
        }

        public static ICommand CreateSetMatched(ICard first, ICard second, bool matched)
        {
            return new SetMatchedCommand(first, second, matched);
        }
    }
}
