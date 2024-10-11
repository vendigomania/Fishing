using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.UI
{
    public class GameTarget : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;

        public UnityAction OnBorderAchieve;
        public RectTransform RectTransform => rectTransform;

        public Vector2 StartPosition { get; private set; }
        private Vector2 direction;

        private void Start()
        {
            StartPosition = rectTransform.anchoredPosition;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            var temp = rectTransform.anchoredPosition + direction * Time.deltaTime * (80 + GameData.Instance.CastSpeed * 6);
            if (temp.x > GameScene.FieldMax.x || temp.y > GameScene.FieldMax.y)
            {
                OnBorderAchieve?.Invoke();
                gameObject.SetActive(false);
                return;
            }

            rectTransform.anchoredPosition = temp;
        }

        public void SetDirecttion(Vector2 _direction)
        {
            rectTransform.anchoredPosition = StartPosition;

            direction = _direction;

            gameObject.SetActive(true);
        }
    }
}
