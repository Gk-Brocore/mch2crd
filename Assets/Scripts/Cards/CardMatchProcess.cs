using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Card
{
    /// <summary>
    /// Processes the logic for matching two cards. 
    /// It checks if the IDs of the two cards are the same, indicating a match.
    /// </summary>
    public class CardMatchProcess : ICardProcessor
    {
        public bool Process(ICard _card1, ICard _card2)
        {
            return _card1 != null && _card2 != null && _card1.Id == _card2.Id;
        }
    }
}