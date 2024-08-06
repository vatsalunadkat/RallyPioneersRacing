using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointMapMarkers : MonoBehaviour
{
    public GameObject checkpointsParent;
    public GameObject[] checkpoints;
    public int nextPlayerCheckpoint = 1;
    public int storedCheckpoint = -1;
    [SerializeField] private LapCounter player;
    [SerializeField] public enum CheckpointsSelector { Start, One, Two, Three };
    public CheckpointsSelector currentCheckpoint;
    private Animator[] animators;
    // Start is called before the first frame update
    void Start()
    {
        checkpointsParent = this.gameObject;
        currentCheckpoint = CheckpointsSelector.One;
        int childCount = checkpointsParent.transform.childCount;
        checkpoints = new GameObject[childCount];
        storedCheckpoint = -1;

        for (int i = 0; i < childCount; i++)
        {
            checkpoints[i] = checkpointsParent.transform.GetChild(i).gameObject;
            checkpoints[i].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        nextPlayerCheckpoint = player.nextCheckpoint;
        if (player != null && storedCheckpoint != nextPlayerCheckpoint)
        {
            foreach (GameObject thisCheckpoint in checkpoints)
            {
                thisCheckpoint.GetComponent<Animator>().SetBool("CurrentCheckpoint", false);
                thisCheckpoint.gameObject.SetActive(false);
            }
            if (!player.completedAllLaps)
            {
                checkpoints[nextPlayerCheckpoint].gameObject.SetActive(true);
                checkpoints[nextPlayerCheckpoint].GetComponent<Animator>().SetBool("CurrentCheckpoint", true);
                storedCheckpoint = nextPlayerCheckpoint;
            }
        }
    }
}
