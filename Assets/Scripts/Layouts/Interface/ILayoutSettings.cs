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
        string Name { get;  }
        int Width { get; }
        int Height { get; }
        int TotalCombinations { get; }
        Vector2 CellSize { get;  }
        Vector2 CellSpacing { get; }

        RectOffset Padding { get;  }

        Vector2 Origin { get; }
        void PrepareLayout();

        CellData CreateCellData(Vector2Int _position);

    }
}