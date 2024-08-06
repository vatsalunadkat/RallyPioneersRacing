using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuSounds : MonoBehaviour
{
    private Gamepad gamepad;
    private float soundCooldown = 0.0f;

    // Update is called once per frame
    private void Start()
    {
        gamepad = Gamepad.current;
    }

    void Update()
    {
        if (soundCooldown > 0)
        {
            soundCooldown -= Time.unscaledDeltaTime;
        }

        if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            FindObjectOfType<AudioManager>().Play("Click");
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            FindObjectOfType<AudioManager>().Play("Select");
        }

        if (gamepad != null)
        {
            if (gamepad.buttonSouth.wasPressedThisFrame)
            {

            }

            if (gamepad.dpad.down.wasPressedThisFrame || gamepad.dpad.up.wasPressedThisFrame || gamepad.dpad.left.wasPressedThisFrame || gamepad.dpad.right.wasPressedThisFrame)
            {
                FindObjectOfType<AudioManager>().Play("Click");
            }
            else if (gamepad.leftStick.ReadValue() != Vector2.zero && soundCooldown <= 0f)
            {
                FindObjectOfType<AudioManager>().Play("Click");
                soundCooldown = 0.1f;
            }
        }
    }
}
