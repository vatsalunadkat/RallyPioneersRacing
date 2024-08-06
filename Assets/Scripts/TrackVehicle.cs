using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackVehicle : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject trackedVehicle;
    private bool enableTracking = true;
    void Start()
    {
        if (trackedVehicle == null)
        {
            enableTracking = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enableTracking)
        {
            transform.position = new Vector3(trackedVehicle.transform.position.x, transform.position.y, trackedVehicle.transform.position.z);
        }
    }
}
