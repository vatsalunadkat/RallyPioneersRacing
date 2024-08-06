using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarSounds : MonoBehaviour
{
    public float minSpeed;
    public float maxSpeed;
    private float currentSpeed;

    private Rigidbody playerRB;
    private AudioSource audioSrc;
    // public AudioClip horn;
    // public AudioSource hornSource;

    public float minPitch;
    public float maxPitch;
    private float pitchFromCar;
    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        audioSrc = GetComponent<AudioSource>();
    }

    void EngineSound()
    {
        currentSpeed = playerRB.velocity.magnitude;
        pitchFromCar = currentSpeed / 50.0f;

        if (currentSpeed < minSpeed)
        {
            audioSrc.pitch = minPitch;
        }

        if (currentSpeed > minSpeed && currentSpeed < maxSpeed)
        {
            audioSrc.pitch = minPitch + pitchFromCar;
        }

        if (currentSpeed > maxSpeed)
        {
            audioSrc.pitch = maxPitch;
        }
    }

    // private void OnHorn()
    // {
    //     hornSource.PlayOneShot(horn, 0.7f);
    // }

    // Update is called once per frame
    void Update()
    {
        EngineSound();
    }
}
