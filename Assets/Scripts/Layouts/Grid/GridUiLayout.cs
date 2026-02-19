using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Layout.Grid
{
    public class GridUiLayout : LayoutGroup, ILayoutView
    {
        public enum FitType
        {
            UNIFORM,
            WIDTH,
            HEIGHT,
            FIXEDROWS,
            FIXEDCOLUMNS
        }

        [Header("Flexible Grid")]
        public FitType fitType = FitType.UNIFORM;

        [Min(1)]
        public int rows = 1;
        public int Rows { get => rows; set => rows = value; }
        [Min(1)]
        public int columns = 1;
        public int Columns { get => columns; set => columns = value; }

        public Vector2 cellSize = new Vector2(100f, 100f);
        public Vector2 CellSize { get => cellSize; set => cellSize = value; }
        public Vector2 spacing = Vector2.zero;
        public Vector2 Spacing { get => spacing; set => spacing = value; }
        public RectOffset Padding { get => padding; set => padding = value; }

        public bool posFitX = false;
        public bool postFitY = false;


        // internal flags
        private bool fitX;
        private bool fitY;

        // cache of calculated cell size (used while laying out)
        private Vector2 calculatedCellSize;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            int _childCount = rectChildren.Count;

            // early fallback
            if (_childCount == 0)
            {
                calculatedCellSize = Vector2.zero;
                SetLayoutInputForAxis(0, 0, -1, 0);
                return;
            }

            // determine rows/columns based on fitType
            if (fitType == FitType.UNIFORM || fitType == FitType.WIDTH || fitType == FitType.HEIGHT)
            {
                float _sq = Mathf.Sqrt(_childCount);
                int _ceil = Mathf.CeilToInt(_sq);
                rows = columns = _ceil;

                fitX = (fitType == FitType.WIDTH) || (fitType == FitType.UNIFORM);
                fitY = (fitType == FitType.HEIGHT) || (fitType == FitType.UNIFORM);

                if (fitType == FitType.WIDTH)
                {
                    fitX = true; fitY = false;
                }
                else if (fitType == FitType.HEIGHT)
                {
                    fitX = false; fitY = true;
                }
            }

            if (fitType == FitType.WIDTH || fitType == FitType.FIXEDCOLUMNS)
            {
                // columns must be >= 1
                columns = Mathf.Max(1, columns);
                rows = Mathf.CeilToInt(_childCount / (float)columns);
            }

            if (fitType == FitType.HEIGHT || fitType == FitType.FIXEDROWS)
            {
                rows = Mathf.Max(1, rows);
                columns = Mathf.CeilToInt(_childCount / (float)rows);
            }

            rows = Mathf.Max(1, rows);
            columns = Mathf.Max(1, columns);

            float _parentWidth = rectTransform.rect.width - padding.horizontal;
            float _parentHeight = rectTransform.rect.height - padding.vertical;

            float _totalSpacingX = spacing.x * (columns - 1);
            float _totalSpacingY = spacing.y * (rows - 1);

            float _cellWidth = columns > 0 ? (_parentWidth - _totalSpacingX) / columns : 0;
            float _cellHeight = rows > 0 ? (_parentHeight - _totalSpacingY) / rows : 0;

            float _finalCellWidth = fitX ? _cellWidth : cellSize.x;
            float _finalCellHeight = fitY ? _cellHeight : cellSize.y;

            calculatedCellSize = new Vector2(_finalCellWidth, _finalCellHeight);

            float _totalMinWidth = padding.horizontal + (calculatedCellSize.x * columns) + _totalSpacingX;
            float _totalMinHeight = padding.vertical + (calculatedCellSize.y * rows) + _totalSpacingY;

            SetLayoutInputForAxis(_totalMinWidth, _totalMinWidth, -1, 0);
            SetLayoutInputForAxis(_totalMinHeight, _totalMinHeight, -1, 1);
        }

        public override void CalculateLayoutInputVertical()
        {
        }

        public override void SetLayoutHorizontal()
        {
            if (!posFitX)
                return;

            // position children on X axis
            int _columnCount = 0;
            int _rowCount = 0;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                _rowCount = i / columns;
                _columnCount = i % columns;

                RectTransform _child = rectChildren[i];

                float xPos = padding.left + _columnCount * (calculatedCellSize.x + spacing.x);
                SetChildAlongAxis(_child, 0, xPos, calculatedCellSize.x);
            }
        }

        public override void SetLayoutVertical()
        {
            if (!postFitY)
                return;
            // position children on Y axis
            int _columnCount = 0;
            int _rowCount = 0;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                _rowCount = i / columns;
                _columnCount = i % columns;

                RectTransform _child = rectChildren[i];

                float _yPosFromTop = padding.top + _rowCount * (calculatedCellSize.y + spacing.y);

                float _parentHeight = rectTransform.rect.height;

                float _totalGridHeight = rows * calculatedCellSize.y + (rows - 1) * spacing.y;

                float _yPos = padding.top + _rowCount * (calculatedCellSize.y + spacing.y);

                SetChildAlongAxis(_child, 1, _yPos, calculatedCellSize.y);
            }
        }
    }
}
