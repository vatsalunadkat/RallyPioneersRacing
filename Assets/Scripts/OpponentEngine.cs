using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OpponentEngine : MonoBehaviour
{
    public Transform path;
    public WheelCollider wheelsFL;
    public WheelCollider wheelsFR;
    public WheelCollider wheelsBL;
    public WheelCollider wheelsBR;
    private List<Transform> nodes;
    private Rigidbody vehicleRB;
    public int currNode = 0;
    public float centerOfGravityOffset = -1.0f;
    // public GameObject tracker;
    // public GameObject trackerParent;
    // public float trackingDistance;
    // public Vector3 lastTrackerPos;

    [Header("Vehicle Modifiers")]
    public float maxSteerAngle = 30f;
    public float baseSpeed = 2000.0f;
    public float brakeTorqueValue = 0;
    public float maxBrakeTorque = 2500.0f;
    public float currentSpeed = 0;
    public float maxSpeed = 45;

    [Header("For testing/debug purposes")]
    public float rangeOffset = 2f;

    [Header("Sensors")]
    public float sensingDistance = 30f;
    public float sensorFrontPos = 1.5f;
    public float frontSideSensorPos = 1.7f;
    public float frontSensorAngle = 30f;
    public float vehicleHeight = 1f;
    private bool currentlyAvoiding = false;
    private float targetSteerAngle = 0f;
    private float turnSpeed = 10f;

    [Header("AntiStuck")]
    public float stuckThres = 1f;
    public float stuckDuration = 6f;
    public float reverseTorque = 0.0f;
    public float reverseDuration = 1.6f;
    private Vector3 previousPos;
    public float stuckTimer = 0.00f;
    public float LoopStuckTimer = 0.00f;
    public float LoopStuckDuration = 15.00f;
    public float maxStuckTime = 10f;
    public bool isTryingToReverse = false;

    public float curveOffset = 1.5f;
    public float currentSteerAngle;
    public float vehicleSpeed;

    public bool isBraking = false;
    public float nodeDistanceThres = 18f;
    public bool IsInsideSpeedPoint = false;
    public bool forceApplybrakes = false;

    public bool doNotReverse = false;
    [SerializeField] private bool testingMode = false;

    public float avoidanceModifier = 1.0f;
    public float reverseTime = 0.0f;
    [SerializeField] private float centerSensorThres = 0.08f;

    // Start is called before the first frame update
    void Start()
    {
        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();

        vehicleRB = GetComponent<Rigidbody>();
        vehicleRB.centerOfMass += Vector3.up * centerOfGravityOffset;
        nodes = new List<Transform>();
        previousPos = Vector3.zero;
        // lastTrackerPos = transform.position;

        // maxSteerAngle = 45f;
        // rangeOffset = 2f;
        // sensingDistance = 7.0f;
        // sensorFrontPos = 1.5f;
        // vehicleHeight = 1f;
        // frontSideSensorPos = 1.6f;

        reverseTorque = baseSpeed * -1f;
        isTryingToReverse = false;
        // reverseDuration = 1.5f;
        // stuckDuration = 5f;
        IsInsideSpeedPoint = false;
        forceApplybrakes = false;

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        SetWaypoints();
        if (!testingMode)
        {
            MoveCar();
        }


        if (isTryingToReverse)
        {
            reverseTime += Time.deltaTime;
            if (reverseTime > reverseDuration)
            {
                isTryingToReverse = false;
                reverseTime = 0.0f;
            }
        }
        // oldSteeringAlgo();

        SteerCar();
        ApplyBrakes();
        SenseObstacles();
        LerpToAvoidObstacles();

        if (IsInsideSpeedPoint)
        {
            if (vehicleRB.velocity.magnitude > 42f)
            {
                // isBraking = true;
                forceApplybrakes = true;
            }
            else if (vehicleRB.velocity.magnitude < 38f)
            {
                forceApplybrakes = false;
            }
        }
        else if (vehicleRB.velocity.magnitude < 38f)
        {
            forceApplybrakes = false;
        }

        if (!testingMode && !doNotReverse)
        {
            UnstuckVehicle();
        }

        if (GetComponent<OpponentLapCounter>() != null)
        {
            if (GetComponent<OpponentLapCounter>().completedAllLaps)
            {
                testingMode = true;
                // isBraking = true;
                forceApplybrakes = true;
                vehicleRB.drag = 2;
                vehicleRB.angularDrag = 2;
            }
        }

        // if (Vector3.Distance(transform.position, lastTrackerPos) > 5f)
        // {
        //     Instantiate(tracker, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);
        //     lastTrackerPos = transform.position;
        // }
    }

    // IEnumerator WaitXSeconds(float time)
    // {
    //     yield return new WaitForSeconds(time);
    //     isTryingToReverse = false;
    // }

    private void UnstuckVehicle()
    {
        Vector3 currentPosition = transform.position;
        float distanceFromPrevious = Vector3.Distance(previousPos, currentPosition);

        if (distanceFromPrevious < stuckThres)
        {
            stuckTimer += Time.deltaTime;
            if (stuckDuration < stuckTimer)
            {
                isTryingToReverse = true;
            }

            if (stuckTimer > maxStuckTime)
            {
                if (currNode != 0)
                {
                    transform.position = new Vector3(nodes[currNode - 1].position.x, nodes[currNode - 1].position.y + 3f, nodes[currNode - 1].position.z);
                    Vector3 directionVector = nodes[currNode].position - nodes[currNode - 1].position;
                    directionVector = directionVector.normalized;
                    transform.rotation = Quaternion.LookRotation(directionVector);
                    forceApplybrakes = false;
                }
                else
                {
                    transform.position = new Vector3(nodes[nodes.Count - 1].position.x, nodes[nodes.Count - 1].position.y + 3f, nodes[nodes.Count - 1].position.z);
                    Vector3 directionVector = nodes[currNode].position - nodes[nodes.Count - 1].position;
                    directionVector = directionVector.normalized;
                    transform.rotation = Quaternion.LookRotation(directionVector);
                    forceApplybrakes = false;
                }
                stuckTimer = 0.00f;
            }
        }
        else
        {
            stuckTimer = 0.00f;
        }

        if (distanceFromPrevious > 5f)
        {
            LoopStuckTimer += Time.deltaTime;
            if (LoopStuckDuration < LoopStuckTimer)
            {
                if (currNode != 0)
                {
                    transform.position = new Vector3(nodes[currNode - 1].position.x, nodes[currNode - 1].position.y + 3f, nodes[currNode - 1].position.z);
                    Vector3 directionVector = nodes[currNode].position - nodes[currNode - 1].position;
                    directionVector = directionVector.normalized;
                    transform.rotation = Quaternion.LookRotation(directionVector);
                    forceApplybrakes = false;
                }
                else
                {
                    transform.position = new Vector3(nodes[nodes.Count - 1].position.x, nodes[nodes.Count - 1].position.y + 3f, nodes[nodes.Count - 1].position.z);
                    Vector3 directionVector = nodes[currNode].position - nodes[nodes.Count - 1].position;
                    directionVector = directionVector.normalized;
                    transform.rotation = Quaternion.LookRotation(directionVector);
                    forceApplybrakes = false;
                }
                LoopStuckTimer = 0.00f;
                isTryingToReverse = false;
            }
        }
        else
        {
            LoopStuckTimer = 0.00f;
        }
        previousPos = transform.position;
    }

    private void SetWaypoints()
    {
        Vector3 vehiclePosition = transform.position;

        // float xOffset = UnityEngine.Random.Range(-rangeOffset, rangeOffset);
        // float zOffset = UnityEngine.Random.Range(-rangeOffset, rangeOffset);

        Vector3 currNodePosition = nodes[currNode].position;
        // Vector3 currNodePosition = new Vector3(nodes[currNode].position.x + xOffset, nodes[currNode].position.y, nodes[currNode].position.z + zOffset);
        ;
        Debug.DrawLine(vehiclePosition, currNodePosition, Color.magenta);
        // print(Vector3.Distance(vehiclePosition, currNodePosition));
        if (Vector3.Distance(vehiclePosition, currNodePosition) < nodeDistanceThres)
        {
            if (currNode == nodes.Count - 1)
            {
                currNode = 0;
            }
            else
            {
                currNode++;
            }
        }
    }

    private void LerpToAvoidObstacles()
    {
        wheelsFL.steerAngle = Mathf.Lerp(wheelsFL.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        wheelsFR.steerAngle = Mathf.Lerp(wheelsFL.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
    }

    private void SenseObstacles()
    {
        RaycastHit hit;
        Vector3 direction = transform.forward;
        Vector3 position = transform.position;
        Vector3 sensorPosition = (position + direction * sensorFrontPos) + direction;
        // Vector3 sensorPosition = position;
        sensorPosition.y += vehicleHeight;
        Vector3 sensorHitPos = position + (direction * sensingDistance);
        sensorHitPos.y += sensorPosition.y;

        float avoidingSensitivity = 0f;
        currentlyAvoiding = false;

        Vector3 FrontLeftSensorPosition = sensorPosition - (transform.right) * frontSideSensorPos;
        Vector3 FrontSensorsTarget = transform.forward;
        Vector3 FrontLeftAngle = Quaternion.AngleAxis(-frontSensorAngle, transform.up) * FrontSensorsTarget;
        Vector3 FrontRightAngle = Quaternion.AngleAxis(frontSensorAngle, transform.up) * FrontSensorsTarget;
        Vector3 FrontRightSensorPosition = sensorPosition + (transform.right) * frontSideSensorPos;

        Vector3 LeftSensorPositionCenter = position + transform.right * 0.5f;
        Vector3 RightSensorPositionCenter = position - transform.right * 0.5f;
        Vector3 SideSensorPositionOffset = transform.forward * 2.3f;
        Vector3 LeftSensorTarget = transform.right;
        Vector3 RightSensorTarget = -transform.right;

        // Front Left Sensors
        sensorPosition -= transform.right * frontSideSensorPos;
        if (Physics.Raycast(sensorPosition, transform.forward, out hit, sensingDistance))
        {
            if (!hit.collider.CompareTag("Terrain")) //  || Terrain.activeTerrain.SampleHeight(hit.point) > 0.6f)
            {
                Debug.DrawLine(sensorPosition, hit.point, Color.blue);
                currentlyAvoiding = true;
                if (hit.collider.CompareTag("Opponent") || hit.collider.CompareTag("Player"))
                {
                    float distanceFromOpponent = Vector3.Distance(sensorPosition, hit.point);
                    avoidingSensitivity += Mathf.Lerp(0.05f, 0.1f, Mathf.InverseLerp(2f, 0.5f, distanceFromOpponent));
                    // if (distanceFromOpponent < 1f)
                    // {
                    //     avoidingSensitivity += 0.1f;
                    // }
                    // else
                    // {
                    //     avoidingSensitivity += 0.05f;
                    // }
                }
                else
                {
                    float distanceFromObstacle = Vector3.Distance(sensorPosition, hit.point);
                    avoidingSensitivity += Mathf.Lerp(0.35f, 0.75f, Mathf.InverseLerp(25f, 1f, distanceFromObstacle));
                    // if (distanceFromObstacle > 3f)
                    // {
                    //     avoidingSensitivity += 0.6f;
                    // }
                    // else
                    // {
                    //     avoidingSensitivity += 0.3f;
                    // }
                }
            }
        }
        else if (Physics.Raycast(sensorPosition, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensingDistance))
        {
            if (!hit.collider.CompareTag("Terrain")) //  || Terrain.activeTerrain.SampleHeight(hit.point) > 0.6f)
            {
                Debug.DrawLine(sensorPosition, hit.point, Color.red);
                currentlyAvoiding = true;
                // print("Left: " + Vector3.Distance(sensorPosition, hit.point));
                if (hit.collider.CompareTag("Opponent") || hit.collider.CompareTag("Player"))
                {
                    float distanceFromOpponent = Vector3.Distance(sensorPosition, hit.point);
                    avoidingSensitivity += Mathf.Lerp(0.01f, 0.1f, Mathf.InverseLerp(3f, 0.5f, distanceFromOpponent));
                    // if (distanceFromOpponent < 1f)
                    // {
                    //     avoidingSensitivity += 0.1f;
                    // }
                    // else
                    // {
                    //     avoidingSensitivity += 0.05f;
                    // }
                }
                else
                {
                    float distanceFromObstacle = Vector3.Distance(sensorPosition, hit.point);
                    avoidingSensitivity += Mathf.Lerp(0.25f, 0.45f, Mathf.InverseLerp(15f, 1f, distanceFromObstacle));
                    // if (distanceFromObstacle < 3f)
                    // {
                    //     // print("Too close!");
                    //     avoidingSensitivity += 0.3f;
                    // }
                    // else
                    // {
                    //     // print("Don't worry.");
                    //     avoidingSensitivity += 0.2f;
                    // }
                }
            }
        }

        // Front Right Sensor
        sensorPosition += 2 * transform.right * frontSideSensorPos;
        if (Physics.Raycast(sensorPosition, transform.forward, out hit, sensingDistance))
        {
            if (!hit.collider.CompareTag("Terrain")) //  || Terrain.activeTerrain.SampleHeight(hit.point) > 0.6f)
            {
                Debug.DrawLine(sensorPosition, hit.point, Color.blue);
                currentlyAvoiding = true;
                if (hit.collider.CompareTag("Opponent") || hit.collider.CompareTag("Player"))
                {
                    float distanceFromOpponent = Vector3.Distance(sensorPosition, hit.point);
                    avoidingSensitivity += -1 * Mathf.Lerp(0.05f, 0.1f, Mathf.InverseLerp(3f, 0.5f, distanceFromOpponent));
                    // if (distanceFromOpponent < 1f)
                    // {
                    //     avoidingSensitivity -= 0.1f;
                    // }
                    // else
                    // {
                    //     avoidingSensitivity -= 0.05f;
                    // }
                }
                else
                {
                    float distanceFromObstacle = Vector3.Distance(sensorPosition, hit.point);
                    avoidingSensitivity += -1 * Mathf.Lerp(0.35f, 0.75f, Mathf.InverseLerp(25f, 1f, distanceFromObstacle));
                    // if (distanceFromObstacle < 3f)
                    // {
                    //     avoidingSensitivity -= 0.6f;
                    // }
                    // else
                    // {
                    //     avoidingSensitivity -= 0.3f;
                    // }
                }
            }
        }
        else if (Physics.Raycast(sensorPosition, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensingDistance))
        {
            if (!hit.collider.CompareTag("Terrain")) //  || Terrain.activeTerrain.SampleHeight(hit.point) > 0.6f)
            {
                Debug.DrawLine(sensorPosition, hit.point, Color.red);
                currentlyAvoiding = true;
                // print("Right: " + Vector3.Distance(sensorPosition, hit.point));
                if (hit.collider.CompareTag("Opponent") || hit.collider.CompareTag("Player"))
                {
                    float distanceFromOpponent = Vector3.Distance(sensorPosition, hit.point);
                    avoidingSensitivity += -1 * Mathf.Lerp(0.01f, 0.1f, Mathf.InverseLerp(3f, 0.5f, distanceFromOpponent));
                    // if (distanceFromOpponent < 1f)
                    // {
                    //     avoidingSensitivity -= 0.1f;
                    // }
                    // else
                    // {
                    //     avoidingSensitivity -= 0.05f;
                    // }
                }
                else
                {
                    float distanceFromObstacle = Vector3.Distance(sensorPosition, hit.point);
                    avoidingSensitivity += -1 * Mathf.Lerp(0.25f, 0.45f, Mathf.InverseLerp(15f, 1f, distanceFromObstacle));
                    // if (distanceFromObstacle < 3f)
                    // {
                    //     // print("Too close!");
                    //     avoidingSensitivity -= 0.3f;
                    // }
                    // else
                    // {
                    //     // print("Don't worry.");
                    //     avoidingSensitivity -= 0.2f;
                    // }
                }
            }
        }

        // Side Sensors
        // Side Right
        sensorPosition = position + transform.right * 0.5f;
        sensorPosition.y += vehicleHeight;
        // Debug.DrawLine(sensorPosition, position + Quaternion.AngleAxis(90, transform.up) * transform.forward, Color.red);
        if (Physics.Raycast(sensorPosition, transform.right, out hit, sensingDistance * 0.6f))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                currentlyAvoiding = true;
                float distanceFromObstacle = Vector3.Distance(sensorPosition, hit.point);
                if (distanceFromObstacle < 5f)
                {
                    Debug.DrawLine(sensorPosition, hit.point, Color.green);
                    avoidingSensitivity += -1 * Mathf.Lerp(0.1f, 0.2f, Mathf.InverseLerp(2.8f, 0.5f, distanceFromObstacle));
                }
                // if (distanceFromObstacle < 1.8f)
                // {
                //     avoidingSensitivity -= 0.05f;
                // }
                // else
                // {
                //     avoidingSensitivity -= 0.01f;
                // }
            }
        }
        else if (Physics.Raycast(sensorPosition - transform.forward * 2.3f, transform.right, out hit, sensingDistance * 0.6f))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                currentlyAvoiding = true;
                float distanceFromObstacle = Vector3.Distance(sensorPosition - transform.forward * 2.3f, hit.point);
                if (distanceFromObstacle < 5f)
                {
                    Debug.DrawLine(sensorPosition - transform.forward * 2.3f, hit.point, Color.green);
                    avoidingSensitivity += -1 * Mathf.Lerp(0.05f, 0.15f, Mathf.InverseLerp(2.8f, 0.5f, distanceFromObstacle));
                }
                // if (distanceFromObstacle < 1.8f)
                // {
                //     avoidingSensitivity -= 0.025f;
                // }
                // else
                // {
                //     avoidingSensitivity -= 0.005f;
                // }
            }
        }
        else if (Physics.Raycast(sensorPosition + transform.forward * 2.3f, transform.right, out hit, sensingDistance * 0.6f))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                currentlyAvoiding = true;
                float distanceFromObstacle = Vector3.Distance(sensorPosition + transform.forward * 2.3f, hit.point);
                if (distanceFromObstacle < 5f)
                {
                    Debug.DrawLine(sensorPosition + transform.forward * 2.3f, hit.point, Color.green);
                    avoidingSensitivity += -1 * Mathf.Lerp(0.2f, 0.35f, Mathf.InverseLerp(2.8f, 0.5f, distanceFromObstacle));
                }
                // if (distanceFromObstacle < 1.8f)
                // {
                //     avoidingSensitivity -= 0.025f;
                // }
                // else
                // {
                //     avoidingSensitivity -= 0.005f;
                // }
            }
        }

        // Side Left
        sensorPosition -= 2 * transform.right * 0.5f;
        if (Physics.Raycast(sensorPosition, -transform.right, out hit, sensingDistance * 0.6f))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                currentlyAvoiding = true;
                float distanceFromObstacle = Vector3.Distance(sensorPosition, hit.point);
                if (distanceFromObstacle < 5f)
                {
                    Debug.DrawLine(sensorPosition, hit.point, Color.red);
                    avoidingSensitivity += Mathf.Lerp(0.1f, 0.2f, Mathf.InverseLerp(2.8f, 0.5f, distanceFromObstacle));
                }
                // print(distanceFromObstacle);
                // if (distanceFromObstacle < 1.8f)
                // {
                //     avoidingSensitivity += 0.05f;
                // }
                // else
                // {
                //     avoidingSensitivity += 0.01f;
                // }
            }
        }
        else if (Physics.Raycast(sensorPosition - transform.forward * 2.3f, -transform.right, out hit, sensingDistance * 0.6f))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                currentlyAvoiding = true;
                float distanceFromObstacle = Vector3.Distance(sensorPosition - transform.forward * 2.3f, hit.point);
                if (distanceFromObstacle < 5f)
                {
                    Debug.DrawLine(sensorPosition - transform.forward * 2.3f, hit.point, Color.red);
                    avoidingSensitivity += Mathf.Lerp(0.05f, 0.15f, Mathf.InverseLerp(2.8f, 0.5f, distanceFromObstacle));
                }
                // print(distanceFromObstacle);
                // if (distanceFromObstacle < 1.8f)
                // {
                //     avoidingSensitivity += 0.025f;
                // }
                // else
                // {
                //     avoidingSensitivity += 0.005f;
                // }
            }
        }
        else if (Physics.Raycast(sensorPosition + transform.forward * 2.3f, -transform.right, out hit, sensingDistance * 0.6f))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                currentlyAvoiding = true;
                float distanceFromObstacle = Vector3.Distance(sensorPosition + transform.forward * 2.3f, hit.point);
                if (distanceFromObstacle < 5f)
                {
                    Debug.DrawLine(sensorPosition + transform.forward * 2.3f, hit.point, Color.red);
                    avoidingSensitivity += Mathf.Lerp(0.2f, 0.35f, Mathf.InverseLerp(2.8f, 0.5f, distanceFromObstacle));
                }
                // print(distanceFromObstacle);
                // if (distanceFromObstacle < 1.8f)
                // {
                //     avoidingSensitivity += 0.025f;
                // }
                // else
                // {
                //     avoidingSensitivity += 0.005f;
                // }
            }
        }

        // Front Center Sensor
        sensorPosition = (position + direction * sensorFrontPos) + direction;
        sensorPosition.y += vehicleHeight;

        // Debug.DrawLine(sensorPosition, sensorHitPos, Color.yellow);
        if (Mathf.Abs(avoidingSensitivity) < centerSensorThres)
        {
            if (Physics.Raycast(sensorPosition, transform.forward, out hit, sensingDistance))
            {
                if (!hit.collider.CompareTag("Terrain")) //  || Terrain.activeTerrain.SampleHeight(hit.point) > 0.6f)
                {
                    Debug.DrawLine(sensorPosition, hit.point, Color.cyan);
                    currentlyAvoiding = true;


                    if (hit.normal.x < 0 && !(hit.collider.CompareTag("Opponent") || hit.collider.CompareTag("Player")))
                    {
                        float distanceFromObstacle = Vector3.Distance(sensorPosition, hit.point);
                        avoidingSensitivity = -1 * Mathf.Lerp(0.8f, 1.2f, Mathf.InverseLerp(7f, 0.5f, distanceFromObstacle));
                        // if (distanceFromObstacle < 3f)
                        // {
                        //     avoidingSensitivity = -0.8f;
                        // }
                        // else
                        // {
                        //     avoidingSensitivity = -0.3f;
                        // }
                    }
                    else
                    {
                        float distanceFromObstacle = Vector3.Distance(sensorPosition, hit.point);
                        avoidingSensitivity = Mathf.Lerp(0.8f, 1.2f, Mathf.InverseLerp(7f, 0.5f, distanceFromObstacle));
                        // if (distanceFromObstacle < 3f)
                        // {
                        //     avoidingSensitivity = 0.8f;
                        // }
                        // else
                        // {
                        //     avoidingSensitivity = 0.3f;
                        // }
                    }
                }
            }
        }

        if (currentlyAvoiding)
        {
            targetSteerAngle = maxSteerAngle * avoidingSensitivity * avoidanceModifier;

            if (isTryingToReverse)
            {
                targetSteerAngle *= -1f;
            }

        }
    }

    private void MoveCar()
    {
        // NOTE: USING FORMULA FROM - https://discussions.unity.com/t/how-to-find-out-speed-in-mph-with-wheelcollider-rpm/170279

        currentSpeed = 2 * Mathf.PI * wheelsBL.radius * wheelsFL.rpm * 60 / 1000;

        if (!isTryingToReverse)
        {
            if (currentSpeed < maxSpeed)
            {
                wheelsFL.motorTorque = baseSpeed * Time.deltaTime * 600;
                wheelsFR.motorTorque = baseSpeed * Time.deltaTime * 600;
                wheelsBL.motorTorque = baseSpeed * Time.deltaTime * 600;
                wheelsBR.motorTorque = baseSpeed * Time.deltaTime * 600;
            }
            else
            {
                wheelsFL.motorTorque = 0.0f;
                wheelsFR.motorTorque = 0.0f;
                wheelsBL.motorTorque = 0.0f;
                wheelsBR.motorTorque = 0.0f;
            }
        }
        else
        {
            wheelsBL.motorTorque = reverseTorque * Time.deltaTime * 600;
            wheelsBR.motorTorque = reverseTorque * Time.deltaTime * 600;
            wheelsFL.motorTorque = reverseTorque * Time.deltaTime * 600;
            wheelsFR.motorTorque = reverseTorque * Time.deltaTime * 600;
        }
    }

    private void oldSteeringAlgo()
    {
        if (!currentlyAvoiding)
        {
            Vector3 relativeVector = transform.InverseTransformPoint(nodes[currNode].position);
            relativeVector = relativeVector / relativeVector.magnitude;
            float newSteer = maxSteerAngle * (relativeVector.x / relativeVector.magnitude);

            targetSteerAngle = newSteer;
        }
    }

    private void SteerCar()
    {
        if (!currentlyAvoiding)
        {
            if (nodes.Count > 3)
            {
                Vector3 currentWaypoint = nodes[currNode].position;
                Vector3 nextWaypopint = nodes[(currNode + 1) % nodes.Count].position;

                Vector3 controlPointB = currentWaypoint + Vector3.right * curveOffset;
                Vector3 controlPointC = currentWaypoint - Vector3.right * curveOffset;

                List<Vector3> pathPoints = new List<Vector3>();
                for (float t = 0; t <= 1; t += 1f / nodes.Count)
                {
                    pathPoints.Add(QuadraticBezierCurve(currentWaypoint, controlPointB, controlPointC, nextWaypopint, t));
                }

                Vector3 destinationPoint = pathPoints[2];

                Vector3 relativeVector = transform.InverseTransformPoint(destinationPoint);
                relativeVector = relativeVector / relativeVector.magnitude;
                float newSteer = maxSteerAngle * (relativeVector.x / relativeVector.magnitude);

                targetSteerAngle = newSteer;
            }
            else
            {
                oldSteeringAlgo();
            }

            // Vector3 relativeVector = transform.InverseTransformPoint(nodes[currNode].position);
            // relativeVector = relativeVector / relativeVector.magnitude;
            // float newSteer = maxSteerAngle * (relativeVector.x / relativeVector.magnitude);

            // targetSteerAngle = newSteer;
        }
    }

    private Vector3 QuadraticBezierCurve(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        // (1—t)^3*A + 3(1—t)^2*t*B + 3*(1—t)*t^2*C + t^3*D
        float oneMinusT = 1f - t;
        Vector3 part1 = Mathf.Pow(oneMinusT, 3) * a;
        Vector3 part2 = 3 * Mathf.Pow(oneMinusT, 2) * t * b;
        Vector3 part3 = 3 * oneMinusT * Mathf.Pow(t, 2) * c;
        Vector3 part4 = Mathf.Pow(t, 3) * d;

        Vector3 curvePoint = part1 + part2 + part3 + part4;

        return curvePoint;
    }

    private void ApplyBrakes()
    {
        currentSteerAngle = wheelsFL.steerAngle;
        vehicleSpeed = vehicleRB.velocity.magnitude;

        if (Mathf.Abs(currentSteerAngle) > 25 && vehicleSpeed > 38 || vehicleSpeed > 55 && Mathf.Abs(currentSteerAngle) > 10)// || vehicleSpeed > 40 && Mathf.Abs(currentSteerAngle) > 28)
        {
            isBraking = true;
        }
        else
        {
            isBraking = false;
        }

        if (isBraking || forceApplybrakes)
        {
            wheelsFL.brakeTorque = maxBrakeTorque * Time.deltaTime * 600;
            wheelsFR.brakeTorque = maxBrakeTorque * Time.deltaTime * 600;
            wheelsBL.brakeTorque = maxBrakeTorque * Time.deltaTime * 600;
            wheelsBR.brakeTorque = maxBrakeTorque * Time.deltaTime * 600;
            wheelsFL.motorTorque = 0.0f;
            wheelsFR.motorTorque = 0.0f;
            wheelsBL.motorTorque = 0.0f;
            wheelsBR.motorTorque = 0.0f;
        }
        else
        {
            wheelsFL.brakeTorque = 0.0f;
            wheelsFR.brakeTorque = 0.0f;
            wheelsBL.brakeTorque = 0.0f;
            wheelsBR.brakeTorque = 0.0f;
        }
    }
}
