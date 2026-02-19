using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Layout.Grid
{
    [CreateAssetMenu(fileName = "GridLayoutSettings", menuName = "Layouts/Grid Layout Settings")]
    public class GridLayoutSettings : BaseLayoutSettingsSO
    {
        public string gridName = "New Grid";
        [Header("Grid Settings")]
        [SerializeField]
        private int width = 4;
        [SerializeField]
        private int height = 4;
        [SerializeField]
        private Vector2 cellSize = new Vector2(1f, 1.2f);
        [SerializeField]
        private Vector2 cellSpacing = new Vector2(0.1f, 0.1f);
        [SerializeField]
        private RectOffset padding;
        [SerializeField]
        private Vector2 origin = Vector2.zero;

        [Header("Editor Display")]
        public bool showDebug = true;
        public bool showEditorPreview = true;

        [Header("Cell States")]
        [Tooltip("Mark cells as active (playable) or static (non-playable)")]
        public List<CellState> cells = new List<CellState>();

        private int totalUseableCells;
        private int totalCombinations;

        public override string Name => gridName;
        public override int Width => width;
        public override int Height => height;
        public override Vector2 CellSize => cellSize;
        public override Vector2 CellSpacing => cellSpacing;
        public override RectOffset Padding => padding;
        public override Vector2 Origin => origin;
        public int TotalUseableCells { get => totalUseableCells; set => totalUseableCells = value; }
        public override int TotalCombinations => totalCombinations;

        private void OnValidate()
        {
            CalulateUseable();
        }

        public void Initialize()
        {
            cells.Clear();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    cells.Add(new CellState
                    {
                        position = new Vector2Int(x, y),
                        state = State.Static
                    });
                }
            }
            CalulateUseable();
        }

        public void CalulateUseable()
        {
            TotalUseableCells = 0;
            foreach (var cell in cells)
            {
                if (cell.state == State.Static)
                    TotalUseableCells++;
            }

            totalCombinations = TotalUseableCells / 2;
        }

        public State GetCellState(Vector2Int _pos)
        {
            var cell = cells.Find(c => c.position == _pos);
            return cell != null ? cell.state : State.Hidden;
        }

        public void SetCellState(Vector2Int _pos, State _newState)
        {
            var cell = cells.Find(c => c.position == _pos);
            if (cell != null)
                cell.state = _newState;
        }


        public override void PrepareLayout()
        {
            CalulateUseable();
        }

        public override CellData CreateCellData(Vector2Int position)
        {
            var state = GetCellState(position);
            return new CellData(string.Empty, state);
        }
    }
}
