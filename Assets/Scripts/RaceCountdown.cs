using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RaceCountdown : MonoBehaviour
{
    public TMP_Text countdownText;
    public TMP_Text instructionText;
    public int countdownTime = 3;
    public int currentTime;
    public GameObject PauseMenuUI;
    private PauseMenu pauseScript;

    [SerializeField] private AudioSource readySound;
    [SerializeField] private AudioSource goSound;

    public bool countdownOver = false;


    void Start()
    {
        if (countdownText == null)
        {
            //Comment Debug.LogError("CountdownText is not assigned in the inspector.");
            return;
        }
        pauseScript = PauseMenuUI.GetComponent<PauseMenu>();
        currentTime = countdownTime;
        countdownOver = false;
        StartCoroutine(CountdownCoroutine());
    }

    IEnumerator CountdownCoroutine()
    {
        Time.timeScale = 0f;

        // int currentTime = countdownTime;
        yield return new WaitForSecondsRealtime(2f);
        // clear the instruction text
        if (instructionText != null)
            instructionText.gameObject.SetActive(false);

        while (currentTime >= 0)
        {
            if (currentTime == 0)
            {
                countdownText.text = "GO!";
                countdownOver = true;
                goSound.PlayOneShot(goSound.clip, 1f);
                if (!pauseScript.PauseMenuShown)
                {
                    Time.timeScale = 1f;
                }
            }
            else
            {
                countdownText.text = currentTime.ToString();
            }

            yield return new WaitForSecondsRealtime(1f);
            if (!pauseScript.PauseMenuShown)
            {
                if (currentTime != 0)
                {
                    readySound.PlayOneShot(readySound.clip, 0.5f);
                }
                currentTime--;
            }
        }

        // countdownText.text = "GO!";
        // yield return new WaitForSecondsRealtime(1f);

        if (!pauseScript.PauseMenuShown)
        {
            countdownText.gameObject.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}

