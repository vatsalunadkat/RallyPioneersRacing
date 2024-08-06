using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerPosition : MonoBehaviour
{
    // [SerializeField] GameObject player;
    [SerializeField] GameObject path;
    [SerializeField] GameObject[] players;
    private Transform[] pathPoints;
    private int[] playerNodeIndexes;
    private int[] playerLaps;
    public TextMeshProUGUI playerPosText;

    // Start is called before the first frame update
    void Start()
    {
        if (path == null)
        {
            Debug.LogError("Path is not assigned");
            return;
        }

        pathPoints = path.transform.GetComponentsInChildren<Transform>();
        if (pathPoints == null || pathPoints.Length == 0)
        {
            Debug.LogError("Path is empty");
            return;
        }
        playerNodeIndexes = new int[players.Length];
        playerLaps = new int[players.Length];
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < players.Length; i++)
        {
            float minDist = Mathf.Infinity;
            int minIndex = 0;
            // restrict search to the next 2 nodes ahead of the player
            int start = playerNodeIndexes[i] + 2 >= pathPoints.Length ? 0 : playerNodeIndexes[i];
            int end = playerNodeIndexes[i] + 2 >= pathPoints.Length ? pathPoints.Length : playerNodeIndexes[i] + 2;
            for (int j = start; j < end; j++)
            {
                float distanceFromNode = Vector3.Distance(pathPoints[j].position, players[i].transform.position);
                if (distanceFromNode < minDist)
                {
                    minDist = distanceFromNode;
                    // playerNodeIndexes[i] = j + playerLaps[i] * pathPoints.Length;
                    minIndex = j;
                }
            }
            if (playerNodeIndexes[i] == (playerLaps[i] * pathPoints.Length) + pathPoints.Length - 1 && minIndex == 0)
            {
                // Debug.Log("Player " + i + " has completed a lap");
                playerLaps[i]++;
                playerNodeIndexes[i] = pathPoints.Length * playerLaps[i];
            }
            playerNodeIndexes[i] = minIndex + playerLaps[i] * pathPoints.Length;
        }

        // rank the players based on their current node index
        // player with the highest node index is the leader
        int playerCurrIndex = playerNodeIndexes[0];
        int playerRank = 1;
        for (int i = 1; i < players.Length; i++)
        {
            if (playerNodeIndexes[i] > playerCurrIndex)
            {
                playerRank++;
            }
            else if (playerNodeIndexes[i] == playerCurrIndex)
            {
                // if two players are at the same node index, the player that is closer to the next node is ranked higher
                float dist1 = Vector3.Distance(pathPoints[(playerCurrIndex + 1) % pathPoints.Length].position, players[0].transform.position);
                float dist2 = Vector3.Distance(pathPoints[(playerCurrIndex + 1) % pathPoints.Length].position, players[i].transform.position);
                if (dist1 > dist2)
                {
                    playerRank++;
                }
            }

        }

        playerPosText.text = "Pos: " + playerRank.ToString() + " / " + players.Length.ToString();
    }
}
