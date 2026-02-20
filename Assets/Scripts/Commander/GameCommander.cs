using Game.Card;
using Game.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Commander
{

    /// <summary>
    /// The GameCommander class orchestrates the interactions between card selection, processing, and command execution in the memory game. It listens for card selection events, processes the selected cards to determine matches or mismatches, and uses a command pattern to execute actions such as revealing, hiding, and setting matched states. 
    /// The GameCommander also provides an undo functionality to revert the last action taken.
    /// </summary>
    public class GameCommander : IGameCommander
    {
        private readonly ICardSelector cardSelector;
        private readonly ICardProcessor cardProcessor;
        private readonly ICommandInvoker invoker;
        private readonly ICommandFactory commandFactory;


        private ICard first;

        public event Action<ICard, ICard> OnMatch;
        public event Action<ICard, ICard> OnMismatch;

        public GameCommander(ICardSelector _cardSelector, ICardProcessor _cardProcessor, ICommandInvoker _invoker, ICommandFactory _commandFactory)
        {
            Utilities.Log("GameCommander", "Initializing GameCommander with provided dependencies.");
            Utilities.Log("GameCommander", $"CardSelector: {_cardSelector.GetType().Name}, CardProcessor: {_cardProcessor.GetType().Name}, CommandInvoker: {_invoker.GetType().Name}, CommandFactory: {_commandFactory.GetType().Name}");
            cardSelector = _cardSelector;
            cardProcessor = _cardProcessor;
            invoker = _invoker;
            commandFactory = _commandFactory;
        }

        public void HandleSelect(Vector2 _screenPos)
        {
            var _card = cardSelector.Select(_screenPos);
            if (_card == null || _card.IsRevealed || _card.IsMatched)
            {
                Utilities.Log("GameCommander", $"Invalid card selection at position {_screenPos}. Card is null, already revealed, or already matched.");
                return;
            }

            invoker.Enqueue(commandFactory.CreateReveal(_card));
            invoker.ExecuteAll();
            if (first == null)
            {
                first = _card;
                return;
            }
            if (cardProcessor.Process(first, _card))
            {
                invoker.Enqueue(commandFactory.CreateSetMatched(first, _card, true));
                invoker.ExecuteAll();
                OnMatch?.Invoke(first, _card);
            }
            else 
            {
                invoker.Enqueue(commandFactory.CreateHide(first));
                invoker.Enqueue(commandFactory.CreateHide(_card));
                invoker.ExecuteAll();
                OnMismatch?.Invoke(first, _card);
            }

            first = null;

        }

        public void HandleUndo()
        {
            invoker.UndoLast();
        }

        public void HandleClear()
        {
            invoker.ClearHistory();
            first = null;
        }
    }
}