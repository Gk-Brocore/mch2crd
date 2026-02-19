

using UnityEngine;

namespace Game.Layout
{
    public enum State { Active, Static, Hidden }

    [System.Serializable]
    public class CellData
    {
        public string id;
        public State state;
        public bool revealed;
        public bool matched;

        public CellData() 
        { 
            id = ""; 
            state = State.Hidden; 
            revealed = false; 
            matched = false; 
        }
        public CellData(string _id, State _state)
        {
            id = _id;
            state = _state;
            revealed = false;
            matched = false;
        }

        public override string ToString()
        {
            return $"id:{id} state:{state} revealed:{revealed} matched:{matched}";
        }
    }

    [System.Serializable]
    public class CellState
    {
        public Vector2Int position;
        public State state;
    }
}
