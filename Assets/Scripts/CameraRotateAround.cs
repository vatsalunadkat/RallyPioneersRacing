using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotateAround : MonoBehaviour
{
    public Transform target;
    public float speed = 10f;

    private void Start()
    {
        if (target != null)
        {
            // transform.position = target.position + new Vector3(0, 5, -10);
            transform.LookAt(target);
        }
    }

    void Update()
    {
        if (target != null)
        {
            transform.RotateAround(target.position, Vector3.up, speed * Time.deltaTime);
            transform.LookAt(target);
        }
        else
        {
            //Comment Debug.LogWarning("Target is not assigned");
        }
    }
}


