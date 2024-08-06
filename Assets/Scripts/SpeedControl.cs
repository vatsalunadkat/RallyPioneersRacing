using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedControl : MonoBehaviour
{
    // public float controlledSpeed = 0.5f;
    // public bool oppInRange = false;
    [SerializeField] private bool slowDown = false;
    [SerializeField] private bool catchSpeed = false;
    private void OnTriggerEnter(Collider other)
    {
        // print("Checking...");

        if (slowDown)
        {
            OpponentEngine engine = other.attachedRigidbody.gameObject.GetComponent<OpponentEngine>();
            if (engine != null)
            {
                engine.IsInsideSpeedPoint = true;
            }

            OpponentStateMachine engineFSM = other.attachedRigidbody.gameObject.GetComponent<OpponentStateMachine>();
            if (engineFSM != null)
            {
                engineFSM.IsInsideSpeedPoint = true;
            }
        }

        if (catchSpeed)
        {
            OpponentStateMachine engineFSM = other.attachedRigidbody.gameObject.GetComponent<OpponentStateMachine>();
            if (engineFSM != null)
            {
                engineFSM.increasedSpeed = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        OpponentEngine engine = other.attachedRigidbody.gameObject.GetComponent<OpponentEngine>();
        if (engine != null)
        {
            engine.IsInsideSpeedPoint = false;
        }

        OpponentStateMachine engineFSM = other.attachedRigidbody.gameObject.GetComponent<OpponentStateMachine>();
        if (engineFSM != null)
        {
            engineFSM.IsInsideSpeedPoint = false;
            engineFSM.increasedSpeed = false;
        }

    }

}
