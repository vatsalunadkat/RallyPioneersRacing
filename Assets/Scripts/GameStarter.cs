using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStarter : MonoBehaviour
{
    [SerializeField] private GameObject creditsViewPort;
    [SerializeField] private GameObject achievementsViewPort;
    [SerializeField] private GameObject creditsButton;
    [SerializeField] private GameObject achivementsButton;
    [SerializeField] private GameStarter creditsData;
    [SerializeField] private GameStarter achivementsData;
    [SerializeField] private GameObject scrollBar;
    public bool creditsShown = false;
    public bool achievementsShown = false;
    void Start()
    {
        Time.timeScale = 0f;
        if (creditsViewPort != null)
        {
            creditsViewPort.SetActive(false);
            creditsShown = false;
        }

        if (achievementsViewPort != null)
        {
            achievementsViewPort.SetActive(false);
            achievementsShown = false;
        }

        if (creditsButton != null)
        {
            creditsData = creditsButton.GetComponent<GameStarter>();
        }

        if (achivementsButton != null)
        {
            achivementsData = achivementsButton.GetComponent<GameStarter>();
        }

        if (scrollBar != null)
        {
            scrollBar.SetActive(false);
        }
    }

    public void StartGameForest()
    {
        SceneManager.LoadScene("Forest");
        Time.timeScale = 1f;
    }

    public void StartGameDesert()
    {
        SceneManager.LoadScene("Desert");
        Time.timeScale = 1f;
    }

    public void StartGameTraining()
    {
        SceneManager.LoadScene("Training_Desert");
        Time.timeScale = 1f;
    }

    public void RestartLevel()
    {
        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 0f;
    }

    public void ShowCredits()
    {
        if (creditsViewPort != null)
        {
            if (!creditsShown)
            {
                if (achievementsViewPort != null)
                {
                    if (achievementsShown || achivementsData.achievementsShown)
                    {
                        achievementsViewPort.SetActive(false);
                        achievementsShown = false;
                        achivementsData.achievementsShown = false;
                    }
                }

                creditsViewPort.SetActive(true);
                creditsShown = true;
                achivementsData.creditsShown = true;
            }
            else
            {
                creditsViewPort.SetActive(false);
                creditsShown = false;
                achivementsData.creditsShown = false;
            }
        }
        CheckScrollBar();
    }

    public void ShowAchievements()
    {
        if (achievementsViewPort != null)
        {
            if (!achievementsShown)
            {
                if (creditsViewPort != null)
                {
                    if (creditsShown || creditsData.creditsShown)
                    {
                        creditsViewPort.SetActive(false);
                        creditsShown = false;
                        creditsData.creditsShown = false;
                    }
                }
                achievementsViewPort.SetActive(true);
                achievementsShown = true;
                creditsData.achievementsShown = true;
            }
            else
            {
                achievementsViewPort.SetActive(false);
                achievementsShown = false;
                creditsData.achievementsShown = false;
            }
        }
        CheckScrollBar();
    }

    private void CheckScrollBar()
    {
        if (achievementsShown || creditsShown)
        {
            scrollBar.SetActive(true);
        }
        else
        {
            scrollBar.SetActive(false);
        }
    }
}
