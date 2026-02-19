using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Layout.Grid
{
    public class GridCollection<T> : ILayoutCollection<T>
    {
        private int width, height;
        private Vector2 cellSize;     // Width and height of each cell
        private Vector2 cellSpacing;  // Horizontal and vertical space between cells
        private Vector2 origin;       // Grid origin in world or local space
        private T[,] gridArray;

        public bool inGameDebug;
        public enum Anchor { BottomLeft, Center }
        private Anchor anchorMode;

        // ----------------------------------------------------------
        // CONSTRUCTOR
        // ----------------------------------------------------------
        public GridCollection(int _width, int _height, Vector2 _cellSize, Vector2 _cellSpacing, Vector2 _origin, Func<T> _defaultValue, Anchor _anchor = Anchor.Center)
        {
            width = _width;
            height = _height;
            cellSize = _cellSize;
            cellSpacing = _cellSpacing;
            origin = _origin;
            anchorMode = _anchor;

            gridArray = new T[_width, _height];

            for (int _x = 0; _x < _width; _x++)
                for (int _y = 0; _y < _height; _y++)
                    gridArray[_x, _y] = _defaultValue.Invoke();
        }

        // ----------------------------------------------------------
        // GRID HELPERS
        // ----------------------------------------------------------
        public bool IsInsideGrid(int _x, int _y)
        {
            return _x >= 0 && _y >= 0 && _x < width && _y < height;
        }

        public Vector2 GetCellSize() => cellSize;
        public Vector2 GetCellSpacing() => cellSpacing;
        public Vector2 GetOrigin() => origin;
        public Vector2Int GetSize() => new Vector2Int(width, height);

        // ----------------------------------------------------------
        // VALUE ACCESSORS
        // ----------------------------------------------------------
        public void SetValue(int _x, int _y, T _value)
        {
            if (IsInsideGrid(_x, _y))
                gridArray[_x, _y] = _value;
        }

        public void SetValue(Vector2Int _cell, T _value)
        {
            SetValue(_cell.x, _cell.y, _value);
        }

        public void SetValue(Vector2 _worldPos, T _value)
        {
            GetXY(_worldPos, out int x, out int y);
            SetValue(x, y, _value);
        }

        public T GetValue(int _x, int _y)
        {
            return IsInsideGrid(_x, _y) ? gridArray[_x, _y] : default;
        }

        public T GetValue(Vector2Int _cell) => GetValue(_cell.x, _cell.y);

        public T GetValue(Vector2 _worldPos)
        {
            GetXY(_worldPos, out int x, out int y);
            return GetValue(x, y);
        }

        // ----------------------------------------------------------
        // POSITION CONVERSIONS
        // ----------------------------------------------------------

        /// <summary>
        /// Converts grid coordinates (x, y) to world position (bottom-left corner of cell).
        /// </summary>
        public Vector2 GetWorldPosition(int _x, int _y)
        {
            float _px = origin.x + _x * (cellSize.x + cellSpacing.x);
            float _py = origin.y + _y * (cellSize.y + cellSpacing.y);
            Vector2 _world = new Vector2(_px, _py);

            if (anchorMode == Anchor.Center)
                _world += cellSize * 0.5f;

            return _world;
        }

        /// <summary>
        /// Converts world position to nearest grid indices.
        /// </summary>
        public void GetXY(Vector2 _worldPos, out int _x, out int _y)
        {
            Vector2 _offset = _worldPos - origin;
            _x = Mathf.FloorToInt(_offset.x / (cellSize.x + cellSpacing.x));
            _y = Mathf.FloorToInt(_offset.y / (cellSize.y + cellSpacing.y));
        }

        /// <summary>
        /// Returns world position of a cellfs **center**.
        /// </summary>
        public Vector2 GetWorldCenter(int _x, int _y)
        {
            Vector2 _bottomLeft = GetWorldPosition(_x, _y);
            return _bottomLeft + cellSize * 0.5f;
        }

        /// <summary>
        /// Returns local position relative to origin (without world translation).
        /// </summary>
        public Vector2 GetLocalPosition(int _x, int _y)
        {
            float _lx = _x * (cellSize.x + cellSpacing.x);
            float _ly = _y * (cellSize.y + cellSpacing.y);
            if (anchorMode == Anchor.Center)
                return new Vector2(_lx, _ly) + cellSize * 0.5f;
            return new Vector2(_lx, _ly);
        }

        /// <summary>
        /// Returns the bounding rect (in world coordinates) of a given cell.
        /// </summary>
        public Rect GetCellRect(int _x, int _y)
        {
            Vector2 _pos = GetWorldPosition(_x, _y);
            return new Rect(_pos, cellSize);
        }

        /// <summary>
        /// Returns all four corners (bottom-left, bottom-right, top-right, top-left) in world space.
        /// </summary>
        public Vector2[] GetCellCorners(int _x, int _y)
        {
            Vector2 _bl = GetWorldPosition(_x, _y);
            return new Vector2[]
            {
            _bl,
            _bl + new Vector2(cellSize.x, 0),
            _bl + cellSize,
            _bl + new Vector2(0, cellSize.y)
            };
        }

        // ----------------------------------------------------------
        // UTILITY
        // ----------------------------------------------------------
        public Vector2 GetTotalGridSize()
        {
            return new Vector2(
                width * cellSize.x + (width - 1) * cellSpacing.x,
                height * cellSize.y + (height - 1) * cellSpacing.y
            );
        }

        public Vector2 GetGridCenter()
        {
            Vector2 total = GetTotalGridSize();
            return origin + total * 0.5f;
        }

        public void FromList(List<T> _list)
        {
            int _index = 0;
            for (int _x = 0; _x < width; _x++)
            {
                for (int _y = 0; _y < height; _y++)
                {
                    if (_index < _list.Count)
                    {
                        gridArray[_x, _y] = _list[_index];
                        _index++;
                    }
                }
            }
        }

        public List<T> ToList()
        {
            var _list = new List<T>();
            for (int _x = 0; _x < width; _x++)
            {
                for (int _y = 0; _y < height; _y++)
                {
                    _list.Add(gridArray[_x, _y]);
                }
            }

            return _list;
        }

        // ----------------------------------------------------------
        // DEBUG DRAW
        // ----------------------------------------------------------
        public void DrawDebug(Color? _color = null)
        {
            if (!inGameDebug) return;
            Color c = _color ?? Color.gray;

            for (int _x = 0; _x < width; _x++)
                for (int _y = 0; _y < height; _y++)
                {
                    Vector2 _bl = GetWorldPosition(_x, _y);
                    Vector2 _tr = _bl + cellSize;
                    Debug.DrawLine(_bl, _bl + Vector2.right * cellSize.x, c);
                    Debug.DrawLine(_bl, _bl + Vector2.up * cellSize.y, c);
                    Debug.DrawLine(_bl + Vector2.right * cellSize.x, _tr, c);
                    Debug.DrawLine(_bl + Vector2.up * cellSize.y, _tr, c);
                }
        }
    }
}