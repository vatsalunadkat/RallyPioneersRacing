using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelControl : MonoBehaviour
{
    public Transform wheelModel;
    [HideInInspector] public WheelCollider wheelCollider;
    // Start is called before the first frame update

    public bool steerable;
    public bool motorized;

    Vector3 position;
    Quaternion rotation;

    void Start()
    {
        wheelCollider = GetComponent<WheelCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        wheelCollider.GetWorldPose(out position, out rotation);
        wheelModel.position = position;
        wheelModel.rotation = rotation;
    }
}
