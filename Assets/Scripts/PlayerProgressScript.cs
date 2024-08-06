using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgressScript : MonoBehaviour
{
    private static PlayerProgressScript instance;
    // Start is called before the first frame update
    // [SerializeField] private GameObject scoreboard;
    // [SerializeField] private ScoreboardManager scoreboardData;
    // [HideInInspector]
    public bool ForestLevelLocked = true;
    public bool AchievementOil = false;
    public bool AchievementNOS = false;
    public bool AchievementPortal = false;
    public bool AchievementShield = false;
    public bool AchievementDominator = false;
    public bool RankOneForest = false;
    public bool RankOneDesert = false;
    public bool AchievementLearner = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // private void CheckForUnlock()
    // {
    //     string playerRank = scoreboardData.PlayerRank.text;
    //     if (playerRank == "1")
    //     {
    //         ForestLevelLocked = false;
    //     }
    // }

    // void Start()
    // {
    //     scoreboard = GameObject.FindGameObjectWithTag("Scoreboard");

    //     if (scoreboard != null)
    //     {
    //         scoreboardData = scoreboard.GetComponent<ScoreboardManager>();
    //     }
    // }
}
