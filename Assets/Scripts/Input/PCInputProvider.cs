using UnityEngine;

namespace Game.Input
{
    public class PCInputProvider  : MonoBehaviour, IInputProvider 
    {

        public string Name => "PC Input Provider";

        public bool IsActive => enabled;

        public event System.Action<Vector2> OnClick;

        private void OnEnable()
        {
            Debug.Log("PCInputProvider enabled.");
        }

        public void Tick()
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
                OnClick?.Invoke(UnityEngine.Input.mousePosition);
        }
    }
}