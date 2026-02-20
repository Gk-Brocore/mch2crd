using Game.Layout;
using Game.Layout.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(GridLayoutSettings))]
public class GridLayoutSettingsEditor : Editor
{
    private GridLayoutSettings settings;
    private float previewCellSize = 2f;

    private void OnEnable()
    {
        settings = (GridLayoutSettings)target;
        if (settings.cells == null || settings.cells.Count == 0)
            settings.Initialize();
    }


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(10);

        if (GUILayout.Button("Regenerate Grid"))
        {
            Undo.RecordObject(settings, "Regenerate Memory Grid");
            settings.Initialize();
            EditorUtility.SetDirty(settings);
        }

        GUILayout.Space(10);

        if (settings.TotalUseableCells % 2 == 0)
        {
            EditorGUILayout.HelpBox($"Total Useable Cells: {settings.TotalUseableCells}\nTotal Possible Pairs: {settings.TotalCombinations}", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox($"Total Useable Cells: {settings.TotalUseableCells}\nTotal Possible Pairs: {settings.TotalCombinations}\n\nWarning: Odd number of useable cells may lead to unmatched cards!", MessageType.Warning);
        }

        if (settings.showEditorPreview)
        {
            previewCellSize = EditorGUILayout.FloatField("Preview Scale : Cell Size /", previewCellSize);
            DrawGridPreview();
        }
    }

    private void DrawGridPreview()
    {
        GUILayout.Label("Grid Preview", EditorStyles.boldLabel);

        float _cellSizeX = (settings.CellSize.x / previewCellSize);
        float _cellSizey = (settings.CellSize.y / previewCellSize);

        float _totalWidth = settings.Width * (_cellSizeX + settings.CellSpacing.x);
        float _totalHeight = settings.Height * (_cellSizey + settings.CellSpacing.y);
        Rect _gridRect = GUILayoutUtility.GetRect(_totalWidth, _totalHeight);

        Handles.BeginGUI();
        for (int _y = 0; _y < settings.Height; _y++)
        {
            for (int _x = 0; _x < settings.Width; _x++)
            {
                var _pos = new Vector2Int(_x, _y);
                var _cell = settings.cells.Find(c => c.position == _pos);
                var _state = _cell != null ? _cell.state : State.Static;

                float _px = _gridRect.x + _x * (_cellSizeX + settings.CellSpacing.x);
                float _py = _gridRect.y + _y * (_cellSizey + settings.CellSpacing.y);

                Rect _cellRect = new Rect(_px, _py, _cellSizeX, _cellSizey);

                Color _fill = _state == State.Static ? Color.green : Color.gray;
                EditorGUI.DrawRect(_cellRect, _fill);
                Handles.color = Color.black;
                Handles.DrawLine(new Vector3(_px, _py), new Vector3(_px + _cellSizeX, _py));
                Handles.DrawLine(new Vector3(_px, _py), new Vector3(_px, _py + _cellSizey));

                if (Event.current.type == EventType.MouseDown && _cellRect.Contains(Event.current.mousePosition))
                {
                    Undo.RecordObject(settings, "Toggle Cell State");
                    _cell.state = _cell.state == State.Static
                        ? State.Hidden
                        : State.Static;
                    EditorUtility.SetDirty(settings);
                    settings.CalulateUseable();
                    Event.current.Use();
                }
            }
        }
        Handles.EndGUI();
    }

}
