using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game
{
    public class GameTouchScreen : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public UnityAction<Vector2> OnBegin;
        public UnityAction<Vector2> OnEnd;

        public void OnPointerDown(PointerEventData eventData)
        {
            OnBegin?.Invoke(eventData.position);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnEnd?.Invoke(eventData.position);
        }
    }
}
