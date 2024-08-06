using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LapCounter : MonoBehaviour
{
    private float lapTime;
    public int currLap = 0;
    public int totalLap = 2;
    public TextMeshProUGUI lapTimeText;
    public TextMeshProUGUI lapCountText;
    public TextMeshProUGUI totalTimeText;
    public GameObject[] checkpoints;
    public float[] lapTimes { get; private set; }
    public int nextCheckpoint = 1;
    public bool completedAllLaps = false;
    private float totalTime;

    void Start()
    {
        if (checkpoints.Length < 2)
        {
            //Comment Debug.LogError("There should be at least 2 checkpoints");
        }
        lapTimes = new float[totalLap];
        totalTime = 0f;
    }

    void Update()
    {
        if (!completedAllLaps)
        {
            lapTime += Time.deltaTime;
            lapTimes[currLap] = lapTime;
            totalTime += Time.deltaTime;

            if (lapTimeText != null)
            {
                TimeSpan timeSpan = TimeSpan.FromSeconds(lapTime);
                lapTimeText.text = string.Format("{0:D2}:{1:D2}:{2:D3}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
            }

            if (lapCountText != null)
            {
                lapCountText.text = "Lap: " + (currLap + 1).ToString() + " / " + totalLap.ToString();
            }


            if (totalTimeText != null)
            {
                TimeSpan totalTimeSpan = TimeSpan.FromSeconds(totalTime);
                totalTimeText.text = string.Format("{0:D2}:{1:D2}:{2:D3}", totalTimeSpan.Minutes, totalTimeSpan.Seconds, totalTimeSpan.Milliseconds);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == checkpoints[nextCheckpoint])
        {
            if (nextCheckpoint == 0 && currLap < totalLap)
            {
                lapTimes[currLap] = lapTime;
                //Comment Debug.Log("Lap " + (currLap + 1) + " time: " + lapTime.ToString("F2") + " seconds");

                if (currLap + 1 >= totalLap)
                {
                    completedAllLaps = true;
                    //Comment Debug.Log("Game Over");

                    if (totalTimeText != null)
                    {
                        TimeSpan totalTimeSpan = TimeSpan.FromSeconds(totalTime);
                        totalTimeText.text = string.Format("{0:D2}:{1:D2}:{2:D3}", totalTimeSpan.Minutes, totalTimeSpan.Seconds, totalTimeSpan.Milliseconds);
                    }
                }
                else
                {
                    currLap++;
                    lapTime = 0;
                }
            }

            nextCheckpoint = (nextCheckpoint + 1) % checkpoints.Length;
            //Comment Debug.Log("Next checkpoint: " + nextCheckpoint.ToString());
        }
    }
}
