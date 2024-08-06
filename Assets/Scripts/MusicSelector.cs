using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSelector : MonoBehaviour
{
    public enum backgroundMusicList { Forest, Desert, MainMenu };
    public backgroundMusicList backgroundMusic;
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<AudioManager>().Play(backgroundMusic.ToString());
    }
}
