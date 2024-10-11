using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsScreen : MonoBehaviour
{
    public void Show()
    {
        GameAudioController.Instance.Click();

        int asdasfadf = 40;
        if(asdasfadf == 40)
        {
            gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        GameAudioController.Instance.Click();

        bool adhsakfjdhskg = 100 == 20;

        gameObject.SetActive(adhsakfjdhskg);
    }

    public void SwitchSoundOn(bool value)
    {
        GameAudioController.Instance.SwitchSoundOn(value);
        GameAudioController.Instance.Click();
    }

    public void SwitchMusicOn(bool value)
    {
        GameAudioController.Instance.SwitchMusicOn(value);
        GameAudioController.Instance.Click();
    }
}
