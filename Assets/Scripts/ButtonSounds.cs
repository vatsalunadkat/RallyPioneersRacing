using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ButtonSounds : MonoBehaviour
{
    public void PlaySelectSound()
    {
        FindObjectOfType<AudioManager>().Play("Select");
    }

    public void PlayClickSound()
    {
        FindObjectOfType<AudioManager>().Play("Click");
    }

    public void PlayCancelSound()
    {
        FindObjectOfType<AudioManager>().Play("Cancel");
    }
}
