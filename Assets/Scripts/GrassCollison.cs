using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassCollison : MonoBehaviour
{
    public enum BendMode { Normal, Downward, Unbendeable }
    public BendMode bendMode;
    public float bendFactor;
    public float striffness;
    public float resetDuration;
    public float resetDistance;
    private float sensorOffset = 6f;

    [SerializeField] private Mesh foliageMesh;
    private Vector3[] originalVertices;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        if (foliageMesh == null)
        {
            foliageMesh = GetComponent<Mesh>();
        }
        originalVertices = foliageMesh.vertices;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector3 sensorPosition = transform.position;
        sensorPosition.y += sensorOffset;
        if (Physics.Raycast(sensorPosition, Vector3.down, out hit, Mathf.Infinity))
        {
            Debug.DrawLine(sensorPosition, Vector3.down, Color.green);
            Vector3 vehiclePosition = hit.point;
            BendGrass(vehiclePosition);
        }
        ResetGrass();
    }

    private void BendGrass(Vector3 vehiclePosition)
    {
        Vector3 direction = transform.position - vehiclePosition;
        direction.Normalize();

        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 newVertex = originalVertices[i] + direction * bendFactor;
            foliageMesh.vertices[i] = newVertex;
        }
        foliageMesh.RecalculateNormals();
        timer = resetDuration;
    }

    private void ResetGrass()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;

            for (int i = 0; i < originalVertices.Length; i++)
            {
                Vector3 newVertex = Vector3.Lerp(foliageMesh.vertices[i], originalVertices[i], timer / resetDuration);
                foliageMesh.vertices[i] = newVertex;
            }
            foliageMesh.RecalculateNormals();
        }
    }
}
