using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Race
{
    public class RaceController : MonoBehaviour
    {
        [SerializeField] private GameScene main;
        [SerializeField] private TMP_Text timerLable;
        [SerializeField] private TMP_Text playerEarnedLable;

        [SerializeField] private TMP_Text progressLable;
        [SerializeField] private List<RaceTableRow> tableRows = new List<RaceTableRow>();

        [Space, SerializeField] private GameObject resultScreen;
        [SerializeField] private TMP_Text resultDesc;

        public static int PlayerRaceRecord
        {
            get => PlayerPrefs.GetInt("RaceBest", 0);
            set => PlayerPrefs.SetInt("RaceBest", value);
        }

        private static (string name, int result)[] table =
        {
            ("Alex", 425),
            ("Daniil", 365),
            ("Joel", 306),
            ("Riply", 294),
            ("Smith", 288),
            ("Butcher", 274),
            ("Brickman", 214),
            ("Arnold", 168),
        };


        public static int GetPlayerRacePlace()
        {
            for(int i = 0; i < table.Length; i++)
            {
                if (table[i].result < PlayerRaceRecord) return i + 1;
            }
            return 9;
        }

        void Start()
        {
            if (GameData.Instance.ModeName != "Race")
            {
                gameObject.SetActive(false);
                return;
            }

            lastPlace = GetPlayerRacePlace();

            for(int i = 0; i < table.Length; i++)
            {
                tableRows.Add(Instantiate(tableRows[0], tableRows[0].transform.parent));
            }
            UpdateTable();

            main.OnThrowSuccess += OnThrowEnd;
        }

        public void StartRace()
        {
            StartCoroutine(Timer());
        }

        private void OnThrowEnd(bool _success)
        {
            if(_success)
            {
                timer += 10;

                PlayerRaceRecord = Mathf.Max(PlayerRaceRecord, main.EarnedInGame);
                UpdateTable();
            }
        }

        int timer = 120;

        IEnumerator Timer()
        {
            while (timer > 0)
            {
                timer--;

                playerEarnedLable.text = main.EarnedInGame.ToString();

                timerLable.text = System.TimeSpan.FromSeconds(timer).ToString(@"mm\:ss");

                yield return new WaitForSeconds(1f);
            }

            resultScreen.SetActive(true);

            int progressProfit = progress * 100;
            GameData.Instance.Cash += progressProfit;
            resultDesc.text = string.Format("YOU earn {0}$\nProgress {1} positions = +{2}$", main.EarnedInGame, progress, progressProfit);
        }

        int progress = 0;
        int lastPlace;

        private void UpdateTable()
        {
            int curPlace = GetPlayerRacePlace();
            progress += lastPlace - curPlace;
            lastPlace = curPlace;
            if (progress > 0) progressLable.text = $"+{progress}";


            int tableIndex = 0;
            for(int i = 0; i < tableRows.Count; i++)
            {
                if(i + 1 == curPlace)
                {
                    tableRows[i].SetData((i + 1).ToString(), "Player", PlayerRaceRecord.ToString(), true);
                }
                else
                {
                    tableRows[i].SetData((i + 1).ToString(), table[tableIndex].name, table[tableIndex].result.ToString());
                    tableIndex++;
                }
            }
        }
    }
}
