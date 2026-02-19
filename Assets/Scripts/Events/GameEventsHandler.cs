using Game.Utils;
using System.Linq;
using UnityEngine;

namespace Game.Events
{
    public class GameEventsHandler : MonoBehaviour
    {
        private static IGameEventsEmitter _instance;

        /// <summary>
        /// Singleton instance of the GameEventsEmitter. 
        /// This will be initialized on the first access 
        /// or when the GameEventsHandler is awakened in the scene.
        /// </summary>
        public static IGameEventsEmitter Instance
        {
            get
            {
                if (_instance == null)
                {
                    Utilities.Log("GameEventsHandler", "Initializing New GameEventsEmitter instance.");
                    _instance = new GameEventsEmitter();
                }
                return _instance;
            }
            private set
            {
                if(_instance == null)
                    _instance = value;
            }
        }


        private void Awake()
        {
            if(_instance == null)
            {
                _instance = new GameEventsEmitter();
                Utilities.Log("GameEventsHandler", "GameEventsEmitter instance created on Awake.");
            }
        }

        public static void RegisterObserver(IGameEventsObserver observer)
        {
            if (Instance == null)
            {
                Debug.LogError("GameEventsHandler instance is not initialized.");
                return;
            }
            Utilities.Log("GameEventsHandler", $"Registering observer: {observer.GetType().Name}");
            Instance.RegisterObserver(observer);
        }

        public static void UnregisterObserver(IGameEventsObserver observer)
        {
            if (Instance == null)
            {
                Debug.LogError("GameEventsHandler instance is not initialized.");
                return;
            }
            Utilities.Log("GameEventsHandler", $"Unregistering observer: {observer.GetType().Name}");
            Instance.UnregisterObserver(observer);
        }
    }
}