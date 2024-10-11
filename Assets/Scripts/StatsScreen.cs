using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text catched;
    [SerializeField] private TMP_Text uncatched;
    [SerializeField] private TMP_Text earned;
    [SerializeField] private TMP_Text commonCatched;
    [SerializeField] private TMP_Text uncommonCatched;
    [SerializeField] private TMP_Text rareCatched;
    [SerializeField] private TMP_Text epicCatched;
    [SerializeField] private TMP_Text largest;

    public void Show()
    {
        GameAudioController.Instance.Click();
        gameObject.SetActive(true);

        catched.text = GameData.Instance.Catched.ToString();
        uncatched.text = GameData.Instance.Escaped.ToString();
        earned.text = GameData.Instance.MoneyEarned.ToString();
        commonCatched.text = GameData.Instance.Common.ToString();
        uncommonCatched.text = GameData.Instance.Uncommon.ToString();
        rareCatched.text = GameData.Instance.Rare.ToString();
        epicCatched.text = GameData.Instance.Epic.ToString();
        largest.text = GameData.Instance.LargestFish.ToString();
    }

    public void Hide()
    {
        GameAudioController.Instance.Click();
        gameObject.SetActive(false);
    }
}
