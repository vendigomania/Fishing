using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay
{
    public class SwimmingFish : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;

        public RectTransform RectTransform => rectTransform;
        public FishModel Model { get; private set; }
        public float Weight { get; private set; }

        private Vector2 moveTarget;

        private void Update()
        {
            if(Vector2.Distance(rectTransform.anchoredPosition, moveTarget) < 20f)
            {
                moveTarget = GetRandomPoint();
            }
            else
            {
                rectTransform.anchoredPosition = Vector2.MoveTowards(
                    rectTransform.anchoredPosition, moveTarget, Time.deltaTime * 20f * Weight);

                rectTransform.localScale = new Vector2(
                    -Mathf.Abs(rectTransform.localScale.x) * Mathf.Sign(moveTarget.x - rectTransform.anchoredPosition.x),
                    -Mathf.Abs(rectTransform.localScale.y) * Mathf.Sign(moveTarget.y - rectTransform.anchoredPosition.y));
            }
        }

        public void SetRandomFish()
        {
            var model = GameData.Instance.FishModels[Random.Range(0, GameData.Instance.FishModels.Count)];

            Initialize(model, model.MinWeight + Random.Range(0, 3) * (model.MaxWeight - model.MinWeight) / 2f);
        }

        public void Initialize(FishModel _model, float _weight)
        {
            Model = _model;

            Weight = _weight;
            rectTransform.localScale = Vector2.one * _weight;
            rectTransform.anchoredPosition = GetRandomPoint();

            moveTarget = GetRandomPoint();

            gameObject.SetActive(true);
        }

        public float CheckDistance(Vector2 _vec)
        {
            return Vector2.Distance(rectTransform.anchoredPosition, _vec) - Weight / 2 * 30f;
        }

        private Vector2 GetRandomPoint()
        {
            return new Vector2(
                Random.Range(0f, GameScene.FieldMax.x),
                Random.Range(0f, GameScene.FieldMax.y));
        }
    }
}
