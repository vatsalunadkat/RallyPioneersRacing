using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnDirectionAdjustments : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Transform[] nodes = transform.GetComponentsInChildren<Transform>();
        for (int i = 0; i < nodes.Length; i++)
        {

            if (i != nodes.Length - 1)
            {
                Transform currentNode = nodes[i];
                Transform nextNode = nodes[i + 1];

                Vector3 direction = (nextNode.position - currentNode.position).normalized;
                currentNode.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
            else
            {
                Transform currentNode = nodes[i];
                Transform nextNode = nodes[0];

                Vector3 direction = (nextNode.position - currentNode.position).normalized;
                currentNode.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }

        }
    }
}
