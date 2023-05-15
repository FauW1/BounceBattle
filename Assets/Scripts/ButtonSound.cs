using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    private SoundManager sound;
    private AudioSource button;
    void Start()
    {
        sound = GameObject.FindGameObjectWithTag("Sound Manager").GetComponent<SoundManager>();
        button = sound.button;
    }

    public void ButtonPlay()
    {
        button.Play();
    }
}
