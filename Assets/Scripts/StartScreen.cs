using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
    [SerializeField] private Button classicModeBtn;
    [SerializeField] private Button duelModeBtn;
    [SerializeField] private Button raceModeBtn;
    [SerializeField] private TMP_Text racePLaceLable;

    private void Start()
    {
        classicModeBtn.onClick.AddListener(Classic);
        duelModeBtn.onClick.AddListener(Duel);
        raceModeBtn.onClick.AddListener(Race);
        racePLaceLable.text = $"Place in leaderboard {Game.Race.RaceController.GetPlayerRacePlace()}";
    }

    private void Classic()
    {
        GameAudioController.Instance.Click();
        GameData.Instance.ModeName = "Classic";
        SceneManager.LoadScene(2);
    }

    private void Duel()
    {
        GameAudioController.Instance.Click();
        GameData.Instance.ModeName = "Duel";
        SceneManager.LoadScene(2);
    }

    private void Race()
    {
        GameAudioController.Instance.Click();
        GameData.Instance.ModeName = "Race";
        SceneManager.LoadScene(2);
    }
}
