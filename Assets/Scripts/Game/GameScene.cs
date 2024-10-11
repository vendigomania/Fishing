using Game.Gameplay;
using Game.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameScene : MonoBehaviour
    {
        public static Vector2 FieldMax { get; private set; } = new Vector2(1375f, 800f);

        [SerializeField] private TMP_Text cashLable;
        [SerializeField] private TMP_Text fishCountLable;

        [SerializeField] private GameObject tip;
        [SerializeField] private Animator fishEscapeAnim;
        
        [SerializeField] private GameTarget target;
        [SerializeField] private GameTouchScreen touchScreen;
        [SerializeField] private CatchPanel catchPanel;
        [SerializeField] private SuccessCatchScreen successCatchScreen;
        [SerializeField] private GameObject blockScreen;

        [SerializeField] private RectTransform hitAnimation;
        public List<SwimmingFish> fishes = new List<SwimmingFish>();

        public UnityAction<bool> OnThrowSuccess;
        public int EarnedInGame;

        int CatchedFishCount = 0;

        // Start is called before the first frame update
        void Start()
        {
            touchScreen.OnBegin += (vec) => SetTarget(vec);
            touchScreen.OnEnd += (vec) => Throw();

            target.OnBorderAchieve += () => Throw();

            catchPanel.OnSuccess += OnCatchResult;

            var swimCount = Random.Range(3, 7);
            fishes[0].SetRandomFish();
            fishes[0].gameObject.SetActive(true);

            for (var i = 1; i < swimCount; i++)
            {
                fishes.Add(Instantiate(fishes[0], fishes[0].transform.parent));
                fishes[i].SetRandomFish();
                fishes[i].gameObject.SetActive(true);
            }

            ShowStats();
        }

        public void BackToMenu()
        {
            GameAudioController.Instance.Click();
            SceneManager.LoadScene(1);
        }

        private void Throw()
        {
            target.gameObject.SetActive(false);

            StartCoroutine(CheckThrow());
        }

        private void SetTarget(Vector2 _vec)
        {
            Vector2 dir = Vector3.Normalize(_vec - target.StartPosition);

            target.SetDirecttion(dir);
        }

        private void OnCatchResult(bool _res)
        {
            for (int i = 0; i < fishes.Count; i++)
                if (!fishes[i].gameObject.activeSelf)
                {
                    if (_res)
                    {
                        CatchedFishCount++;
                        int price = GameData.GetPrice(fishes[i].Weight, fishes[i].Model);
                        GameData.Instance.Cash += price;
                        GameData.Instance.AddToTrophies(fishes[i].Model, fishes[i].Weight);

                        if (fishes[i].Weight > GameData.Instance.LargestFish)
                            GameData.Instance.LargestFish = fishes[i].Weight;

                        successCatchScreen.Show(
                            fishes[i].Model.Icon,
                            fishes[i].Model.Rarity,
                            fishes[i].Weight == fishes[i].Model.MaxWeight ? "big" : (fishes[i].Weight == fishes[i].Model.MinWeight ? "small" : "medium"),
                            fishes[i].Weight.ToString(),
                            price.ToString());

                        //Duel
                        EarnedInGame += price;
                        GameAudioController.Instance.Win();
                    }
                    else
                    {
                        GameAudioController.Instance.Lose();
                        GameData.Instance.Escaped++;
                        fishEscapeAnim.Play("Show");
                    }

                    fishes[i].SetRandomFish();
                    fishes[i].gameObject.SetActive(true);

                    OnThrowSuccess?.Invoke(_res);
                }

            blockScreen.SetActive(false);
            tip.SetActive(true);
            hitAnimation.gameObject.SetActive(false);
        }

        private void ShowStats()
        {
            cashLable.text = GameData.Instance.Cash.ToString();
            fishCountLable.text = CatchedFishCount.ToString();

            Invoke("ShowStats", 1f);
        }

        IEnumerator CheckThrow()
        {
            tip.SetActive(false);

            blockScreen.gameObject.SetActive(true);

            var point = target.RectTransform.anchoredPosition;

            hitAnimation.anchoredPosition = point;
            hitAnimation.gameObject.SetActive(true);

            yield return new WaitForSeconds(1f);

            SwimmingFish nearFish = null;
            float minDistance = 1000f;

            for (var i = 0; i < fishes.Count; i++)
            {
                if (!fishes[i].gameObject.activeSelf) continue;

                var dist = fishes[i].CheckDistance(point);
                if (dist > (1f + GameData.Instance.CatchDistance) * 10f)
                    continue;

                if (dist < minDistance)
                {
                    nearFish = fishes[i];
                    minDistance = dist;
                }
            }

            if(nearFish != null)
            {
                nearFish.gameObject.SetActive(false);

                catchPanel.Launch(nearFish.Weight);

                GameAudioController.Instance.Click();
            }
            else
            {
                OnThrowSuccess?.Invoke(false);
                hitAnimation.gameObject.SetActive(false);
                blockScreen.gameObject.SetActive(false);
                tip.SetActive(true);

                GameAudioController.Instance.Lose();
            }
        }
    }
}
