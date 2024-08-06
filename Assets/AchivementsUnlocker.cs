using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchivementsUnlocker : MonoBehaviour
{
    private GameObject saveObject;
    private PlayerProgressScript saveData;
    [SerializeField] private GameObject Achievement1;
    [SerializeField] private GameObject Achievement2;
    [SerializeField] private GameObject Achievement3;
    [SerializeField] private GameObject Achievement4;
    [SerializeField] private GameObject Achievement5;
    [SerializeField] private GameObject Achievement6;

    // Start is called before the first frame update
    void Start()
    {
        saveObject = GameObject.FindGameObjectWithTag("PlayerSaveData");

        if (saveObject != null)
        {
            saveData = saveObject.GetComponent<PlayerProgressScript>();

        }

        if (saveData.AchievementLearner)
        {
            GameObject currentIcon = Achievement1.transform.GetChild(0).transform.GetChild(2).gameObject;
            Achievement1.transform.GetChild(1).gameObject.SetActive(false);
            currentIcon.transform.GetChild(0).gameObject.SetActive(false);
            currentIcon.transform.GetChild(1).gameObject.SetActive(true);
        }
        if (saveData.AchievementOil)
        {
            GameObject currentIcon = Achievement2.transform.GetChild(0).transform.GetChild(2).gameObject;
            Achievement2.transform.GetChild(1).gameObject.SetActive(false);
            currentIcon.transform.GetChild(0).gameObject.SetActive(false);
            currentIcon.transform.GetChild(1).gameObject.SetActive(true);
        }
        if (saveData.AchievementDominator)
        {
            GameObject currentIcon = Achievement3.transform.GetChild(0).transform.GetChild(2).gameObject;
            Achievement3.transform.GetChild(1).gameObject.SetActive(false);
            currentIcon.transform.GetChild(0).gameObject.SetActive(false);
            currentIcon.transform.GetChild(1).gameObject.SetActive(true);
        }
        if (saveData.AchievementShield)
        {
            GameObject currentIcon = Achievement4.transform.GetChild(0).transform.GetChild(2).gameObject;
            Achievement4.transform.GetChild(1).gameObject.SetActive(false);
            currentIcon.transform.GetChild(0).gameObject.SetActive(false);
            currentIcon.transform.GetChild(1).gameObject.SetActive(true);
        }
        if (saveData.AchievementNOS)
        {
            GameObject currentIcon = Achievement5.transform.GetChild(0).transform.GetChild(2).gameObject;
            Achievement5.transform.GetChild(1).gameObject.SetActive(false);
            currentIcon.transform.GetChild(0).gameObject.SetActive(false);
            currentIcon.transform.GetChild(1).gameObject.SetActive(true);
        }
        if (saveData.AchievementPortal)
        {
            GameObject currentIcon = Achievement6.transform.GetChild(0).transform.GetChild(2).gameObject;
            Achievement6.transform.GetChild(1).gameObject.SetActive(false);
            currentIcon.transform.GetChild(0).gameObject.SetActive(false);
            currentIcon.transform.GetChild(1).gameObject.SetActive(true);
        }
    }
}
