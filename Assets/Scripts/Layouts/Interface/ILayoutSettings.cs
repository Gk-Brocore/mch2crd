using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Layout
{
    /// <summary>
    /// Interface for layout settings, which defines the properties and methods required for configuring a layout.
    /// </summary>
    public interface ILayoutSettings
    {
        string Name { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        int TotalCombinations { get; }
        Vector2 CellSize { get; set; }
        Vector2 CellSpacing { get; set; }

        RectOffset Padding { get; set; }

        Vector2 Origin { get; set; }
        void PrepareLayout();

        CellData CreateCellData(Vector2Int _position);

    }
}