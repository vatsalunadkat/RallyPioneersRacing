using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Threading;
using System;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ScoreboardManager : MonoBehaviour
{
    public GameObject scoreboardPanel;
    public GameObject pauseMenu;
    public LapCounter playerLapCounter;

    public TextMeshProUGUI PlayerRank;
    public TextMeshProUGUI RankOpp1;
    public TextMeshProUGUI RankOpp2;
    public TextMeshProUGUI RankOpp3;

    public TextMeshProUGUI PlayerTime;
    public TextMeshProUGUI TimeOpp1;
    public TextMeshProUGUI TimeOpp2;
    public TextMeshProUGUI TimeOpp3;

    public float waitForOpponents;
    public float waitingDuration = 10.00f;
    [SerializeField] private Button MainMenuButton;

    private Gamepad gamepad;

    private Color darkGray;
    [SerializeField] GameObject saveObject;
    [SerializeField] GameObject player;
    [SerializeField] ModifiedCarController playerData;
    [SerializeField] PlayerProgressScript saveData;
    // private bool menuSelected = false;

    void Start()
    {
        scoreboardPanel.SetActive(false);
        waitForOpponents = 0.00f;
        darkGray = new Color(0.45f, 0.45f, 0.45f, 1f);
        gamepad = Gamepad.current;

        saveObject = GameObject.FindGameObjectWithTag("PlayerSaveData");
        player = GameObject.FindGameObjectWithTag("Player").gameObject;
        playerData = player.GetComponent<ModifiedCarController>();
        saveData = saveObject.GetComponent<PlayerProgressScript>();
    }

    void Update()
    {

        if (playerLapCounter.completedAllLaps)
        {
            // print("Here");
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            ShowScoreboard();
            pauseMenu.SetActive(false);
            if (waitForOpponents < waitingDuration)
            {
                waitForOpponents += Time.deltaTime;
            }
            else
            {
                Time.timeScale = 0f;
            }

            if (Input.GetKeyDown(KeyCode.Return) || (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame))
            {
                MainMenuButton.onClick.Invoke();
            }
        }
    }

    void ShowScoreboard()
    {
        scoreboardPanel.SetActive(true);

        List<PlayerTimeInfo> players = new List<PlayerTimeInfo>
        {
            new PlayerTimeInfo("Player", ParseTime(PlayerTime.text), PlayerTime, PlayerRank)
        };

        if (!string.IsNullOrEmpty(TimeOpp1.text))
        {
            players.Add(new PlayerTimeInfo("Opp1", ParseTime(TimeOpp1.text), TimeOpp1, RankOpp1));
        }

        if (!string.IsNullOrEmpty(TimeOpp2.text))
        {
            players.Add(new PlayerTimeInfo("Opp2", ParseTime(TimeOpp2.text), TimeOpp2, RankOpp2));
        }
        if (!string.IsNullOrEmpty(TimeOpp3.text))
        {
            players.Add(new PlayerTimeInfo("Opp3", ParseTime(TimeOpp3.text), TimeOpp3, RankOpp3));
        }

        if (SceneManager.GetActiveScene().name == "Training_Desert")
        {
            if (saveObject != null)
            {
                saveData.AchievementLearner = true;
            }
        }


        players.Sort((x, y) => x.TotalTime.CompareTo(y.TotalTime));

        if (players[0].Name == "Player")
        {
            if (SceneManager.GetActiveScene().name == "Desert")
            {
                if (saveObject != null)
                {
                    // PlayerProgressScript saveData = saveObject.GetComponent<PlayerProgressScript>();
                    saveData.ForestLevelLocked = false;

                    if (player != null)
                    {
                        GameObject shield = player.transform.Find("car icon shield").gameObject;
                        if (shield.activeSelf)
                        {
                            saveData.AchievementShield = true;
                        }

                        if (playerData.OilSlickTriggered >= 6)
                        {
                            saveData.AchievementOil = true;
                        }

                        if (playerData.NOSTriggered >= 5)
                        {
                            saveData.AchievementNOS = true;
                        }
                        if (playerData.PortalUsed)
                        {
                            saveData.AchievementPortal = true;
                        }
                    }
                }
            }

            if (SceneManager.GetActiveScene().name == "Forest")
            {
                if (saveObject != null)
                {
                    // PlayerProgressScript saveData = saveObject.GetComponent<PlayerProgressScript>();

                    saveData.AchievementDominator = true;

                    if (player != null)
                    {
                        GameObject shield = player.transform.Find("car icon shield").gameObject;
                        if (shield.activeSelf)
                        {
                            saveData.AchievementShield = true;
                        }

                        if (playerData.OilSlickTriggered >= 5)
                        {
                            saveData.AchievementOil = true;
                        }

                        if (playerData.NOSTriggered >= 4)
                        {
                            saveData.AchievementNOS = true;
                        }
                    }

                }
            }



            foreach (var player in players)
            {
                if (playerLapCounter.completedAllLaps)
                {
                    player.TimeText.color = Color.black;
                }
                else
                {
                    player.TimeText.color = darkGray;
                }

                if (player.Name == "Opp1")
                {
                    player.TimeText.text = FormatTime(player.TotalTime);
                }
                else if (player.Name == "Opp2")
                {
                    player.TimeText.text = FormatTime(player.TotalTime);
                }
                else if (player.Name == "Opp3")
                {
                    player.TimeText.text = FormatTime(player.TotalTime);
                }
            }
            players.Sort((x, y) => x.TotalTime.CompareTo(y.TotalTime));
        }

        for (int i = 0; i < players.Count; i++)
        {

            if (SceneManager.GetActiveScene().name == "Training_Desert")
            {
                players[0].RankText.text = 1.ToString();
            }
            else
            {
                players[i].RankText.text = (i + 1).ToString();
            }
        }
    }

    string FormatTime(float totalTime)
    {
        int minutes = Mathf.FloorToInt(totalTime / 60F);
        int seconds = Mathf.FloorToInt(totalTime - minutes * 60);
        int milliseconds = Mathf.FloorToInt((totalTime - minutes * 60 - seconds) * 1000);

        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    float ParseTime(string timeText)
    {
        string[] timeParts = timeText.Split(':');
        if (timeParts.Length == 3)
        {
            int minutes = int.Parse(timeParts[0]);
            int seconds = int.Parse(timeParts[1]);
            int milliseconds = int.Parse(timeParts[2]);

            return minutes * 60f + seconds + milliseconds / 1000f;
        }
        return 0f;
    }

    private class PlayerTimeInfo
    {
        public string Name;
        public float TotalTime;
        public TextMeshProUGUI TimeText;
        public TextMeshProUGUI RankText;

        public PlayerTimeInfo(string name, float totalTime, TextMeshProUGUI timeText, TextMeshProUGUI rankText)
        {
            Name = name;
            TotalTime = totalTime;
            TimeText = timeText;
            RankText = rankText;
        }
    }
}
