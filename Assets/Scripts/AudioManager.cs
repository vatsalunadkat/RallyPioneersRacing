using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    // Start is called before the first frame update
    void Awake()
    {
        foreach (Sound thisSound in sounds)
        {
            thisSound.source = gameObject.AddComponent<AudioSource>();
            thisSound.source.clip = thisSound.clip;
            thisSound.source.volume = thisSound.volume;
            thisSound.source.pitch = thisSound.pitch;
            thisSound.source.loop = thisSound.loop;
            thisSound.source.spread = thisSound.spread;
            thisSound.source.spatialBlend = thisSound.blend;
            thisSound.source.minDistance = thisSound.minDist;
            thisSound.source.maxDistance = thisSound.maxDist;
        }
    }

    public void PlaySelectSound()
    {
        Play("Select");
    }

    public void PlayClickSound()
    {
        Play("Click");
    }

    public void PlayCancelSound()
    {
        Play("Cancel");
    }

    private void Start()
    {
        // Play("Screech");
    }

    public void Play(String name)
    {
        Sound thisSound = Array.Find(sounds, sound => sound.name == name);
        if (thisSound == null)
        {
            // Debug.LogWarning("Can't play. Sound: " + name + " not found!");
            return;
        }
        thisSound.source.Play();
        // Debug.LogWarning("Playing " + name + " clip!");
    }

    public void PlayOneShot(String name, float volumeScale)
    {
        Sound thisSound = Array.Find(sounds, sound => sound.name == name);
        if (thisSound == null)
        {
            // Debug.LogWarning("Can't play. Sound: " + name + " not found!");
            return;
        }
        thisSound.source.PlayOneShot(thisSound.source.clip, volumeScale);
        // Debug.LogWarning("Playing " + name + " clip!");
    }

    public void Stop(String name)
    {
        Sound thisSound = Array.Find(sounds, sound => sound.name == name);
        if (thisSound == null)
        {
            // Debug.LogWarning("Can't stop. Sound: " + name + " not found!");
            return;
        }
        thisSound.source.Stop();
        // Debug.LogWarning("Stopping " + name + " clip!");
    }
}
