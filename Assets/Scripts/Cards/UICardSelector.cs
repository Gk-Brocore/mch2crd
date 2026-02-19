using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Card
{
    /// <summary>
    /// Implements the ICardSelector interface to allow selection of cards based on user input 
    /// in a Unity UI context.
    /// </summary>
    public class UICardSelector : MonoBehaviour , ICardSelector
    {
        [SerializeField] private GraphicRaycaster raycaster;
        [SerializeField] private EventSystem eventSystem;
        public ICard Select(Vector2 _screenPos)
        {
            if (raycaster == null || eventSystem == null)
                return null;

            var data = new PointerEventData(eventSystem) { position = _screenPos };
            var results = new List<RaycastResult>();
            raycaster.Raycast(data, results);

            foreach (var result in results)
            {
                var card = result.gameObject.GetComponentInParent<ICard>();
                if (card != null)
                    return card;
            }

            return null;
        }
    }
}