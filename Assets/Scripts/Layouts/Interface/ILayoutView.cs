using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Layout
{
    /// <summary>
    /// Interface for game layout, this is used for the display of the game, 
    /// it defines the properties and methods required for configuring a game layout.
    /// </summary>
    public interface ILayoutView
    {
        int Rows { get; set; }
        int Columns { get; set; }
        Vector2 CellSize { get; set; }
        Vector2 Spacing { get; set; }
        RectOffset Padding { get; set; }

    }
}