using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PathViz : MonoBehaviour
{
    public Color lineColor;
    private List<Transform> nodes = new List<Transform>();

    public bool drawOnlyOnSelected = false;

    private void OnDrawGizmos()
    {
        if (!drawOnlyOnSelected)
        {
            Gizmos.color = lineColor;

            Transform[] pathTransforms = GetComponentsInChildren<Transform>();
            nodes = new List<Transform>();

            for (int i = 0; i < pathTransforms.Length; i++)
            {
                nodes.Add(pathTransforms[i]);
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                Vector3 currentNote = nodes[i].position;
                Vector3 previousNode = Vector3.zero;
                if (i > 0)
                {
                    previousNode = nodes[i - 1].position;
                }
                else if (i == 0 && nodes.Count > 1)
                {
                    previousNode = nodes[nodes.Count - 1].position;
                }

                Gizmos.DrawLine(previousNode, currentNote);
                Gizmos.DrawSphere(currentNote, 0.3f);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (drawOnlyOnSelected)
        {
            Gizmos.color = lineColor;

            Transform[] pathTransforms = GetComponentsInChildren<Transform>();
            nodes = new List<Transform>();

            for (int i = 0; i < pathTransforms.Length; i++)
            {
                nodes.Add(pathTransforms[i]);
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                Vector3 currentNote = nodes[i].position;
                Vector3 previousNode = Vector3.zero;
                if (i > 0)
                {
                    previousNode = nodes[i - 1].position;
                }
                else if (i == 0 && nodes.Count > 1)
                {
                    previousNode = nodes[nodes.Count - 1].position;
                }

                Gizmos.DrawLine(previousNode, currentNote);
                Gizmos.DrawSphere(currentNote, 0.3f);
            }
        }
    }
}
