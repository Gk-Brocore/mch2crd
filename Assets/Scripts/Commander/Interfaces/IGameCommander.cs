using Game.Card;
using System;
using UnityEngine;

namespace Game.Commander
{
    public interface IGameCommander
    {
        void HandleSelect(Vector2 _screenPos);
        void HandleUndo();

        void HandleClear();

        event Action<ICard, ICard> OnMatch;
        event Action<ICard, ICard> OnMismatch;
    }
}