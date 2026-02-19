using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Card
{
    /// <summary>
    /// Interface for a card in the memory game. 
    /// Defines properties and methods for card behavior, such as revealing, hiding, and matching.
    /// </summary>
    public interface ICard
    {
        string Id { get; }
        bool IsMatched { get; }
        bool IsRevealed { get; }
        Vector2Int GridPosition { get; }

        void Reveal();
        void Hide();
        void SetMatched(bool matched);
        void Mismatch();

        void Initialize(string _id, Vector2Int _gridPos, Sprite _sprite, Vector2 _imgSize, bool _showDebug = false);
    }
}