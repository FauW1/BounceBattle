using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    [Header("Music")]
    public AudioSource menuMusic;
    public AudioSource battleMusic;

    [Header("Sound FX")]
    public AudioSource jump;
    public AudioSource land;
    public AudioSource hit;
    public AudioSource die;
    public AudioSource button;

    private static GameObject instance;
    private static bool menuPlaying = false;
    void Start()
    {
        //don't destroy or duplicate code
        if (instance == null)
        {
            DontDestroyOnLoad(this);
            instance = gameObject;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (OptionsManager.musicUnmute)
        {
            SetMusicVolume(OptionsManager.musicVol);
        }
        else
        {
            SetMusicVolume(0);
        }

        if (OptionsManager.sfxUnmute)
        {
            SetSFXVolume(OptionsManager.sfxVol);
        }
        else
        {
            SetSFXVolume(0);
        }

        //play music on menus
        if (!menuPlaying)
        {
            if (SceneManager.GetActiveScene().name == "Title Screen" || SceneManager.GetActiveScene().name == "Character Select")
            {
                menuMusic.Play();
                battleMusic.Stop();
                menuPlaying = true;
            }
        }

        //play battle music
        if (menuPlaying)
        {
            if (SceneManager.GetActiveScene().name == "Forest")
            {
                menuMusic.Stop();
                battleMusic.Play();
                menuPlaying = false;
            }
        }
    }

    private void SetMusicVolume(float newValue)
    {
        menuMusic.volume = newValue;
        battleMusic.volume = newValue;
    }

    private void SetSFXVolume(float newValue)
    {
        jump.volume = newValue;
        land.volume = newValue;
        hit.volume = newValue;
        die.volume = newValue;
        button.volume = newValue;
    }
}
