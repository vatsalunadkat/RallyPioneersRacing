using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class PauseMenu : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public GameObject RaceCountdown;
    private RaceCountdown raceCountdownScript;
    private Gamepad gamepad;
    public bool PauseMenuShown = false;
    [SerializeField] private Button mainButton;
    void Awake()
    {
        gamepad = Gamepad.current;
        Time.timeScale = 1f;
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            //Comment Debug.LogError("CanvasGroup component not found on the GameObject.");
        }
        raceCountdownScript = RaceCountdown.GetComponent<RaceCountdown>();

    }

    void Start()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0f;
        PauseMenuShown = false;
    }

    void Update()
    {
        if (PauseMenuShown)
        {
            if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
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
                    FindObjectOfType<AudioManager>().Play("Select");
                }

                if (gamepad.dpad.down.wasPressedThisFrame || gamepad.dpad.up.wasPressedThisFrame)
                {
                    FindObjectOfType<AudioManager>().Play("Click");
                }

                if (gamepad.buttonEast.wasPressedThisFrame)
                {
                    Cursor.visible = false;
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                    canvasGroup.alpha = 0f;
                    if (raceCountdownScript.currentTime < 0)
                    {
                        Time.timeScale = 1f;
                    }
                    PauseMenuShown = false;
                }

            }
        }

        if (Input.GetKeyUp(KeyCode.Escape) || (gamepad != null && gamepad.startButton.wasPressedThisFrame))
        {
            if (canvasGroup.interactable)
            {
                // Hide pause menu and resume game
                Cursor.visible = false;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.alpha = 0f;
                if (raceCountdownScript.currentTime < 0)
                {
                    Time.timeScale = 1f;
                }
                PauseMenuShown = false;
                FindObjectOfType<AudioManager>().Play("Cancel");
            }
            else
            {
                // Show pause menu and pause game
                FindObjectOfType<AudioManager>().Play("Cancel");
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.alpha = 1f;
                Time.timeScale = 0f;
                PauseMenuShown = true;
                if (mainButton != null)
                {
                    mainButton.Select();
                }
            }
        }
    }
}
