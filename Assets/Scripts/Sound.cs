using UnityEngine.Audio;
using UnityEngine;
using System;
[System.Serializable]
public class Sound
{
    public String name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;

    [Range(0f, 1f)]
    public float blend = 1;

    [Range(0.1f, 3f)]
    public float pitch = 1;

    [Range(0f, 360f)]
    public float spread = 1;
    [Range(0f, 499f)]
    public float minDist = 1;
    [Range(0f, 1000f)]
    public float maxDist = 500;


    public bool loop;

    [HideInInspector]
    public AudioSource source;

}
