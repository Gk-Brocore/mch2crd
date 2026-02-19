using Game.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Input
{
    /// <summary>
    /// Handler for input providers. 
    /// It is responsible for ticking the input provider and exposing it to other classes.
    /// </summary>
    public class InputHandler : MonoBehaviour 
    {
        private IInputProvider inputProvider;

        public IInputProvider InputProvider
        {
            get
            {
                if (inputProvider == null)
                {
                    Init();
                }

                return inputProvider;
            }
        }

        private void Awake()
        {
            if (inputProvider == null)
                Init();
        }

        private void Init()
        {

            var _allProviders = GetComponents<IInputProvider>();

            foreach (var _provider in _allProviders)
            {
                if (_provider.IsActive)
                {
                    inputProvider = _provider;
                    break;
                }
            }

            if (inputProvider == null)
            {
                Debug.LogError("Input provider is not set on InputHandler.");
                return;   
            }

            Utilities.Log("Input Handler", $"Input provider {inputProvider.Name} is set on InputHandler.");
        }

        private void Update()
        {
            if (inputProvider == null)
            {
                return;
            }
            inputProvider.Tick();
        }
    }
}