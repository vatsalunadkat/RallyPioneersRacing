using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataLogger : MonoBehaviour
{
    private string dataPath;

    private GameObject player;
    private LapCounter playerLapCounter;
    private ModifiedCarController playerCarController;
    private bool isDataLogged = false;

    private Rigidbody playerRB;
    private int count = 0;
    private float moving_mean = 0.0f;
    private float moving_variance = 0.0f;

    void Awake()
    {
        dataPath = Application.persistentDataPath + "/log.txt";
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("DataLogger is running at path: " + dataPath);

        if (!File.Exists(dataPath))
        {
            File.Create(dataPath).Dispose();
        }
        string content = "Date: " + System.DateTime.Now.ToString() + "\n";
        File.AppendAllText(dataPath, content);
        player = this.gameObject;
        playerLapCounter = player.GetComponent<LapCounter>();
        playerCarController = player.GetComponent<ModifiedCarController>();
        playerRB = player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        LogDataToFile();
    }

    void FixedUpdate()
    {
        // formula source - https://www.johndcook.com/blog/standard_deviation/
        count++;
        float curr_speed = playerRB.velocity.magnitude;
        float delta = curr_speed - moving_mean;
        moving_mean += delta / count;
        moving_variance += delta * (curr_speed - moving_mean);
    }

    public void LogDataToFile()
    {

        if (playerLapCounter.completedAllLaps && !isDataLogged)
        {
            isDataLogged = true;
            // Lap time data
            string content = "Player Lap Times: \n";
            float sum = 0.0f;
            for (int i = 0; i < playerLapCounter.lapTimes.Length; i++)
            {
                content += "Lap " + (i + 1) + ": " + playerLapCounter.lapTimes[i] + " seconds\n";
                sum += playerLapCounter.lapTimes[i];
            }
            content += "Total Time: " + sum + " seconds\n";
            content += "Time on track: " + playerCarController.timeOnTrack + " seconds\n";
            content += "Time off track: " + playerCarController.timeOffTrack + " seconds\n";
            File.AppendAllText(dataPath, content);
            // player speed data
            content = string.Empty; // clear the string
            content = "Average Speed: " + moving_mean + " m/s\n";
            content += "Std of Speed: " + Mathf.Sqrt(moving_variance / count) + " m/s\n";
            File.AppendAllText(dataPath, content);
            // collectibles data
            content = string.Empty; // clear the string
            content = "NOSTriggered Count: " + playerCarController.NOSTriggered + "\n";
            content += "OilSlickTriggered Count: " + playerCarController.OilSlickTriggered + "\n";
            content += "Potal Used: " + playerCarController.PortalUsed + "\n";
            content += " Collisions With Opponents: " + playerCarController.collisionsWithOpponents + "\n";
            content += " HandBrake count: " + playerCarController.handBrakeCount + "\n";
            File.AppendAllText(dataPath, content);

            // reset values
            count = 0;
            moving_mean = 0.0f;
            moving_variance = 0.0f;
        }
    }
}
