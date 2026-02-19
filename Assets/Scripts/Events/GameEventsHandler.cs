using UnityEngine;

namespace Game.Events
{
    public class GameEventsHandler : MonoBehaviour
    {
        public static IGameEventsEmitter Instance { get; private set; }

        private GameEventsEmitter emitter;

        private void Awake()
        {
            emitter = new GameEventsEmitter();
            Instance = emitter;
        }

        public static void RegisterObserver(IGameEventsObserver observer)
        {
            if (Instance == null)
            {
                Debug.LogError("GameEventsHandler instance is not initialized.");
                return;
            }
            Instance.RegisterObserver(observer);
        }

        public static void UnregisterObserver(IGameEventsObserver observer)
        {
            if (Instance == null)
            {
                Debug.LogError("GameEventsHandler instance is not initialized.");
                return;
            }
            Instance.UnregisterObserver(observer);
        }
    }
}