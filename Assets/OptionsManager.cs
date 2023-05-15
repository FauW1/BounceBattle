using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsManager : MonoBehaviour
{
    public GameObject optionsMenu;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle screenT, musicT, sfxT;

    public static float musicVol = 1;
    public static float sfxVol = 1;

    public static bool musicUnmute = true;
    public static bool sfxUnmute = true;
    public static bool fullScreen = true;

    private void Start()
    {
        optionsMenu.SetActive(false);

        SetInitialState(screenT, fullScreen);
        SetInitialState(musicT, musicUnmute);
        SetInitialState(sfxT, sfxUnmute);
    }

    private void SetInitialState(Toggle toggle, bool val)
    {
        toggle.SetIsOnWithoutNotify(val);
    }
    public void OptionsOn()
    {
        Time.timeScale = 0;
        optionsMenu.SetActive(true);
    }

    public void OptionsOff()
    {
        Time.timeScale = 1;
        optionsMenu.SetActive(false);
    }

    public void FullScreen(bool on)
    {
        fullScreen = on;
        if (fullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }

    public void MusicVolumeSlider(float newValue)
    {
        musicVol = newValue;
    }

    public void SFXVolumeSlider(float newValue)
    {
        sfxVol = newValue;
    }

    public void MuteMusic(bool unmute)
    {
        musicSlider.interactable = unmute;
        musicUnmute = unmute;
    }

    public void MuteSFX(bool unmute)
    {
        sfxSlider.interactable = unmute;
        sfxUnmute = unmute;
    }

    public void Quit()
    {
        Application.Quit();
    }

    //see https://www.youtube.com/watch?v=0ewSSlTG2xo, dynamic floats and bools
}
