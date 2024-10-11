using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioController : MonoBehaviour
{
    [SerializeField] private AudioSource music;

    [SerializeField] private AudioSource click;
    [SerializeField] private AudioSource lose;
    [SerializeField] private AudioSource win;

    public static GameAudioController Instance { get; private set; }

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SwitchMusicOn(bool isOn)
    {
        music.mute = !isOn;
    }

    public void SwitchSoundOn(bool isOn)
    {
        click.mute = !isOn;
        lose.mute = !isOn;
        win.mute = !isOn;
    }

    public void Click()
    {
        click.Play();
    }

    public void Lose()
    {
        lose.Play();
    }

    public void Win()
    {
        win.Play();
    }
}
