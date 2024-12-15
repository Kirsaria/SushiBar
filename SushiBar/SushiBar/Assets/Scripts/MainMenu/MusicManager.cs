using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    public AudioSource audioMusic;
    public AudioSource audioSFX;
    private void Awake()
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
    public void SetMusicVolume(float volume)
    {
        audioMusic.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        audioSFX.volume = volume;
    }
}
