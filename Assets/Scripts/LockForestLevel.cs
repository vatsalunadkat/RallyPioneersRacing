using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LockForestLevel : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Button ForestLevelButton;
    [SerializeField] private TextMeshProUGUI ForestLevelText;
    private String forestEnabledText = "(Level 2)\nTimber Trails";
    private String forestDisableText = "Reach Rank 1 in\nSahara to unlock";
    private GameObject saveObject;
    private PlayerProgressScript saveData;
    void Start()
    {
        saveObject = GameObject.FindGameObjectWithTag("PlayerSaveData");

        if (saveObject != null)
        {
            saveData = saveObject.GetComponent<PlayerProgressScript>();

            if (saveData.ForestLevelLocked && ForestLevelButton != null && ForestLevelButton.interactable)
            {
                // print("Here!");
                ForestLevelButton.interactable = false;
                ForestLevelText.text = forestDisableText;
            }
            else
            {
                // print("Not here!");
                ForestLevelButton.interactable = true;
                ForestLevelText.text = forestEnabledText;
            }
        }
    }
}
