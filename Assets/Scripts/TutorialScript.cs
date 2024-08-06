using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialScript : MonoBehaviour
{
    [SerializeField] private GameObject AllControls;
    [SerializeField] private GameObject[] ControlChilds;
    [SerializeField] private GameObject HelpItems;
    [SerializeField] private GameObject[] HelpItemsChilds;
    [SerializeField] public enum ControlSchemes { Keyboard, Controller }
    [SerializeField] public ControlSchemes CurrentControls;
    [Range(-1, 5)]
    [SerializeField] public int CurrentHelp = -1;
    [SerializeField] public int PreviousHelp = -1;
    [SerializeField] private String hintSound = "Select";

    private Gamepad gamepad;
    // Start is called before the first frame update
    void Start()
    {
        int childCount = AllControls.transform.childCount;
        ControlChilds = new GameObject[childCount];

        for (int i = 0; i < childCount; i++)
        {
            ControlChilds[i] = AllControls.transform.GetChild(i).gameObject;
            ControlChilds[i].gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

        childCount = HelpItems.transform.childCount;
        HelpItemsChilds = new GameObject[childCount];

        for (int i = 0; i < childCount; i++)
        {
            HelpItemsChilds[i] = HelpItems.transform.GetChild(i).gameObject;
            HelpItemsChilds[i].gameObject.SetActive(false);
        }

        SetHints(-1);
        gamepad = Gamepad.current;
    }

    // Update is called once per frame
    void Update()
    {
        // if (gamepad != null && CurrentControls != ControlSchemes.Keyboard && (gamepad.rightTrigger.ReadValue() != 0 || gamepad.leftTrigger.ReadValue() != 0 || gamepad.leftStick.x.magnitude != 0 || gamepad.leftStick.y.magnitude != 0))
        // {
        //     print("Setting controller");
        //     CurrentControls = ControlSchemes.Controller;
        // }
        // if ((gamepad == null || CurrentControls != ControlSchemes.Keyboard) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)))
        // {
        //     CurrentControls = ControlSchemes.Keyboard;
        //     print("Setting keyboard");
        // }

        switch (CurrentControls)
        {
            case ControlSchemes.Keyboard:
                ShowKeyboardControls();
                break;
            case ControlSchemes.Controller:
                ShowGamepadControls();
                break;
            default:
                ShowKeyboardControls();
                break;
        }

        if (CurrentHelp != PreviousHelp)
        {
            SetHints(CurrentHelp);
            PreviousHelp = CurrentHelp;
        }
    }

    public void ShowKeyboardControls()
    {
        if (CurrentControls != ControlSchemes.Keyboard)
        {
            int childCount = AllControls.transform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                ControlChilds[i].gameObject.transform.GetChild(0).gameObject.SetActive(false);
                ControlChilds[i].gameObject.transform.GetChild(1).gameObject.SetActive(true);
                CurrentControls = ControlSchemes.Keyboard;
            }
        }
    }

    public void ShowGamepadControls()
    {
        if (CurrentControls != ControlSchemes.Controller)
        {
            int childCount = AllControls.transform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                ControlChilds[i].gameObject.transform.GetChild(0).gameObject.SetActive(true);
                ControlChilds[i].gameObject.transform.GetChild(1).gameObject.SetActive(false);
                CurrentControls = ControlSchemes.Controller;
            }
        }
    }

    private void SetHints(int HintIndex)
    {
        int childCount = HelpItems.transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            HelpItemsChilds[i].gameObject.SetActive(false);
        }

        switch (HintIndex)
        {
            case 0:
                HelpItemsChilds[0].gameObject.SetActive(true);
                FindObjectOfType<AudioManager>().Play(hintSound);
                break;
            case 1:
                HelpItemsChilds[1].gameObject.SetActive(true);
                FindObjectOfType<AudioManager>().Play(hintSound);
                break;
            case 2:
                HelpItemsChilds[2].gameObject.SetActive(true);
                FindObjectOfType<AudioManager>().Play(hintSound);
                break;
            case 3:
                HelpItemsChilds[3].gameObject.SetActive(true);
                FindObjectOfType<AudioManager>().Play(hintSound);
                break;
            case 4:
                HelpItemsChilds[4].gameObject.SetActive(true);
                FindObjectOfType<AudioManager>().Play(hintSound);
                break;
            case 5:
                HelpItemsChilds[5].gameObject.SetActive(true);
                FindObjectOfType<AudioManager>().Play(hintSound);
                break;
            default:
                break;
        }
    }
}
