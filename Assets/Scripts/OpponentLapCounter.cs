using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class OpponentLapCounter : MonoBehaviour
{
    public int currLap = 1;
    public int totalLap = 2;
    public float[] lapTimes;
    public String[] lapTimesResults;
    public String lapTimesText;
    public String lapCountText;

    public GameObject checkpointsParent;
    public GameObject[] checkpoints;
    public int nextCheckpoint = 1;
    public bool completedAllLaps = false;

    public TextMeshProUGUI totalTimeText;

    private float totalTime;
    private Color darkGray;

    void Start()
    {

        lapTimes = new float[totalLap];
        lapTimesResults = new string[totalLap];
        darkGray = new Color(0.45f, 0.45f, 0.45f, 1f);

        int childCount = checkpointsParent.transform.childCount;
        checkpoints = new GameObject[childCount];

        if (checkpoints.Length < 2)
        {
            //Comment Debug.LogError("There should be at least 2 checkpoints");
        }

        for (int i = 0; i < childCount; i++)
        {
            if (i == 0)
            {
                checkpoints[i] = checkpointsParent.transform.GetChild(i).gameObject;
            }
            else
            {
                checkpoints[i] = checkpointsParent.transform.GetChild(i).gameObject.transform.GetChild(1).gameObject;
            }
        }

        for (int i = 0; i < totalLap; i++)
        {
            lapTimes[i] = 0;
        }

        completedAllLaps = false;
        lapTimesText = "Initializing";
        lapCountText = "Lap: " + currLap + " / " + totalLap;
        totalTime = 0f;
    }

    void Update()
    {
        if (!completedAllLaps)
        {
            lapTimes[currLap - 1] += Time.deltaTime;
            totalTime += Time.deltaTime;
            if (totalTimeText.color != darkGray)
            {
                totalTimeText.color = darkGray;
            }
        }

        if (completedAllLaps && totalTimeText.text == "NA")
        {
            totalTimeText.text = ConvertlapTimes(totalTime);
            if (totalTimeText.color == darkGray)
            {
                totalTimeText.color = Color.black;
            }
        }

        if (totalTimeText != null)
        {
            TimeSpan totalTimeSpan = TimeSpan.FromSeconds(totalTime);
            totalTimeText.text = string.Format("{0:D2}:{1:D2}:{2:D3}", totalTimeSpan.Minutes, totalTimeSpan.Seconds, totalTimeSpan.Milliseconds);
        }

        if (currLap > 1 && lapCountText != null)
        {
            lapCountText = "Lap: " + currLap.ToString() + " / " + totalLap.ToString();
        }
    }

    private String ConvertlapTimes(float lapTimes)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(lapTimes);
        return string.Format("{0:D2}:{1:D2}:{2:D3}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == checkpoints[nextCheckpoint])
        {
            if (nextCheckpoint == 0)
            {
                lapTimesResults[currLap - 1] = ConvertlapTimes(lapTimes[currLap - 1]);
                if (currLap >= totalLap)
                {
                    completedAllLaps = true;
                }
                else
                {
                    currLap++;
                }
            }
            nextCheckpoint = (nextCheckpoint + 1) % checkpoints.Length;
        }
    }
}
