using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioMusic;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(audioMusic);
    }
}
