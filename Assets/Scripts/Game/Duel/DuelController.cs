using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Duel
{
    public class DuelController : MonoBehaviour
    {
        [SerializeField] private GameScene main;

        [SerializeField] private TMP_Text timerLable;
        [SerializeField] private TMP_Text playerEarnedLable;
        [SerializeField] private TMP_Text botEarnedLable;

        [Space, SerializeField] private GameObject resultScreen;
        [SerializeField] private GameObject winTitle;
        [SerializeField] private GameObject loseTitle;
        [SerializeField] private TMP_Text resultDesc;

        [Space, SerializeField] private RectTransform hitRectTransform;

        int botEarned = 0;

        // Start is called before the first frame update
        void Start()
        {
            if (GameData.Instance.ModeName != "Duel")
            {
                gameObject.SetActive(false);
                return;    
            }
        }

        public void StartDuel()
        {
            StartCoroutine(Timer(120));
            StartCoroutine(BotWork());
        }

        IEnumerator BotWork()
        {
            while(!resultScreen.activeSelf)
            {
                yield return new WaitForSeconds(Random.Range(4, 14));

                var targetFish = Random.Range(0, main.fishes.Count);
                if (!main.fishes[targetFish].gameObject.activeSelf) targetFish = (targetFish + 1) / main.fishes.Count;

                hitRectTransform.anchoredPosition = main.fishes[targetFish].RectTransform.anchoredPosition;
                hitRectTransform.gameObject.SetActive(true);
                main.fishes[targetFish].gameObject.SetActive(false);

                yield return new WaitForSeconds(Random.Range(8, 10));

                if(Random.Range(0, 100) < 50)
                {
                    botEarned += GameData.GetPrice(main.fishes[targetFish].Weight, main.fishes[targetFish].Model);
                }

                main.fishes[targetFish].SetRandomFish();
                main.fishes[targetFish].gameObject.SetActive(true);
                hitRectTransform.gameObject.SetActive(false);
            }
        }

        IEnumerator Timer(int _time)
        {
            while(_time > 0)
            {
                _time--;

                playerEarnedLable.text = main.EarnedInGame.ToString();
                botEarnedLable.text = botEarned.ToString();

                timerLable.text = System.TimeSpan.FromSeconds(_time).ToString(@"mm\:ss");

                yield return new WaitForSeconds(1f);
            }

            resultScreen.SetActive(true);
            winTitle.SetActive(main.EarnedInGame > botEarned);
            loseTitle.SetActive(main.EarnedInGame < botEarned);
            resultDesc.text = string.Format("YOU earn {0}$\nENEMY earn {1}$", main.EarnedInGame, botEarned);

            if(main.EarnedInGame > botEarned)
            {
                GameData.Instance.Cash += botEarned;
            }
            else if(main.EarnedInGame < botEarned)
            {
                GameData.Instance.Cash -= main.EarnedInGame;
            }

            if (main.EarnedInGame > botEarned) GameAudioController.Instance.Win();
            else GameAudioController.Instance.Lose();
        }
    }
}
