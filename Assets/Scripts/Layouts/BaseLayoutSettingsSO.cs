using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Layout
{
    public abstract class BaseLayoutSettingsSO : ScriptableObject , ILayoutSettings
    {
        public abstract string Name { get; }
        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract int TotalCombinations { get; }
        public abstract Vector2 CellSize { get; }
        public abstract Vector2 CellSpacing { get; }
        public abstract RectOffset Padding { get; }
        public abstract Vector2 Origin { get; }

        public abstract void PrepareLayout();
        public abstract CellData CreateCellData(Vector2Int position);

    }
}
