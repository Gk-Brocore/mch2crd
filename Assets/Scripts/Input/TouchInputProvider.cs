using UnityEngine;

namespace Game.Input
{
    public class TouchInputProvider : MonoBehaviour, IInputProvider
    {
        public string Name => "Touch Input Provider";
        public bool IsActive => enabled;
        public event System.Action<Vector2> OnClick;

        private void OnEnable()
        {
            Debug.Log("TouchInputProvider enabled.");
        }
        public void Tick()
        {
            if (UnityEngine.Input.touchCount > 0)
            {
                Touch _touch = UnityEngine.Input.GetTouch(0);
                if (_touch.phase == TouchPhase.Began)
                {
                    OnClick?.Invoke(_touch.position);
                }
            }              
        }
    }
}