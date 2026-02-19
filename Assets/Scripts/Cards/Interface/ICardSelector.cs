using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Card
{
    /// <summary>
    /// Interface for selecting a card based on a screen position.
    /// </summary>
    public interface ICardSelector 
    {
        ICard Select(Vector2 _screenPos);
    }
}