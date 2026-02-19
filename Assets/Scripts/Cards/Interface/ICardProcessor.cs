using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Card
{
    /// <summary>
    /// Interface for processing 2 card interactions in the memory game.
    /// </summary>
    public interface ICardProcessor
    {
        bool Process(ICard _card1, ICard _card2);
    }
}