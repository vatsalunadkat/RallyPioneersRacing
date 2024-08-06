using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckpointDirectionOverlay : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject checkpointArrow;
    [SerializeField] public float angleOffset = 1.0f;
    [SerializeField] private GameObject checkpointsParent;
    [SerializeField] private GameObject[] checkpoints;
    [SerializeField] private Transform[] markerLocations;
    [SerializeField] private LapCounter playerLapCounter;

    // Start is called before the first frame update
    void Start()
    {
        if (player != null)
        {
            playerLapCounter = player.GetComponent<LapCounter>();
            int childCount = checkpointsParent.transform.childCount;
            checkpoints = new GameObject[childCount];
            markerLocations = new Transform[childCount];

            for (int i = 0; i < childCount; i++)
            {
                checkpoints[i] = checkpointsParent.transform.GetChild(i).gameObject;
                // print(checkpoints[i].name);
            }

            // print("Checking Child's Childs");
            for (int i = 0; i < childCount; i++)
            {
                int childsChildsCount = checkpoints[i].transform.childCount;
                // GameObject[] childChilds = new GameObject[childsChildsCount];
                // print("\nParent: " + checkpoi/nts[i].name);
                if (childsChildsCount > 0)
                {
                    if (childsChildsCount > 2)
                    {
                        // print("Child: " + checkpoints[i].transform.GetChild(2).name);
                        markerLocations[i] = checkpoints[i].transform.GetChild(2).transform;
                    }
                    else if (childCount > 0)
                    {
                        markerLocations[i] = checkpoints[i].transform.GetChild(0).transform;
                        // print("Child: " + checkpoints[i].transform.GetChild(0).name);
                        // markerLocations.AddRange(childChilds[0].transform);
                    }
                    // else
                    // {
                    //     markerLocations[i] = checkpoints[i].transform;
                    // }
                }
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // transform.position = player.transform.position;

        int currentPlayerCheckpoint = playerLapCounter.nextCheckpoint;
        Vector3 currentPosition = player.transform.position;
        Vector3 destination = markerLocations[currentPlayerCheckpoint].position;
        // print(currentPlayerCheckpoint);
        Vector3 direction = (destination - currentPosition).normalized;
        Vector3 playerForward = player.transform.forward;
        playerForward.y = 0f;
        direction.y = 0f;
        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
        // angle += angleOffset;
        // float playerAngle = Mathf.Atan2(playerForward.z, playerForward.x) * Mathf.Rad2Deg;
        // angle += ;
        checkpointArrow.transform.rotation = Quaternion.Euler(0, angle, 0);
        // checkpointArrow.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(destination - currentPosition), 0.5f);
    }
}
