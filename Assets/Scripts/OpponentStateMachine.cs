using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentStateMachine : MonoBehaviour
{
    public Transform path;
    public Transform pathAlt;
    private bool useAlternatePath = false;
    public WheelCollider wheelsFL;
    public WheelCollider wheelsFR;
    public WheelCollider wheelsBL;
    public WheelCollider wheelsBR;
    private List<Transform> nodes;
    private List<Transform> nodesAlt;
    private Rigidbody vehicleRB;
    public int currNode = 0;
    public float centerOfGravityOffset = -1.0f;

    [Header("Vehicle Modifiers")]
    public float maxSteerAngle = 30f;
    public float baseTorque = 2000.0f;
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
    public float turnSpeed = 10f;

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
    public enum AllStates { Idle, FollowPath, AvoidObstacles, ControlSpeed, ReverseCar };
    public AllStates currentState;
    public AllStates previousState;
    public int brakeResourceSensors = 0;
    public int forceBrakeResource = 0;
    public int turningBrakeResource = 0;
    public int lapBrakeResource = 0;
    private bool completedAllLaps = false;
    private int speedControlBrakeResource;
    private LayerMask obstacleMask;

    [SerializeField] private GameObject smokeSystem;
    [SerializeField] private ParticleSystem exhaustEmitter;
    [SerializeField] private ParticleSystem dustEmitter_R;
    [SerializeField] private ParticleSystem dustEmitter_L;
    [SerializeField] private ParticleSystem mudEmitter_R;
    [SerializeField] private ParticleSystem mudEmitter_L;
    [SerializeField] private bool mudEnabled = false;

    private int exhaustStopRequestSpeed = 0;
    private int exhaustStopRequestCompletion = 0;
    private int exhaustStopRequestReversal = 0;
    private int exhaustStopRequestAvoidance = 0;
    private int exhaustStopRequests;
    public bool increasedSpeed = false;
    [SerializeField] private float boostSpeed;
    private bool boostApplied = false;
    public bool triggerOilSlick = true;

    // Start is called before the first frame update
    void Start()
    { 
        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();

        dustEmitter_L.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        dustEmitter_R.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (mudEnabled)
        {
            mudEmitter_L.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            mudEmitter_R.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        vehicleRB = GetComponent<Rigidbody>();
        vehicleRB.centerOfMass += Vector3.up * centerOfGravityOffset;
        exhaustEmitter = smokeSystem.GetComponent<ParticleSystem>();

        nodes = new List<Transform>();
        previousPos = Vector3.zero;

        reverseTorque = baseTorque * -1f;
        isTryingToReverse = false;

        IsInsideSpeedPoint = false;
        boostSpeed = maxSpeed + 15f;
        forceApplybrakes = false;
        completedAllLaps = false;
        obstacleMask = LayerMask.NameToLayer("CollidableObjects");


        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }

        // if (pathAlt != null)
        // {
        //     Transform[] altPathTransforms = pathAlt.GetComponentsInChildren<Transform>();
        // }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        SetWaypoints();
        ExhaustLogic();
        switch (currentState)
        {
            case AllStates.Idle:
                if (path != null && !completedAllLaps && !testingMode)// && !GetComponent<OpponentLapCounter>().completedAllLaps)
                {
                    currentState = AllStates.FollowPath;
                    previousState = AllStates.FollowPath;
                }
                if (completedAllLaps)
                {
                    // ApplyBrakes(maxBrakeTorque);
                    MoveCar(baseTorque / 4);
                    SteerCar();
                    vehicleRB.drag = 2;
                    vehicleRB.angularDrag = 2;
                    exhaustEmitter.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    exhaustStopRequestCompletion = 1;
                }
                break;

            case AllStates.FollowPath:
                MoveCar(baseTorque);
                SteerCar();
                SenseObstacles();
                LerpToAvoidObstacles(targetSteerAngle);
                DeadlockAvoidance();
                BrakeLogic();
                DustEffects();
                break;

            case AllStates.AvoidObstacles:
                MoveCar(baseTorque / 2);
                SteerCar();
                SenseObstacles();
                LerpToAvoidObstacles(targetSteerAngle);
                DeadlockAvoidance();
                DustEffects();
                break;

            case AllStates.ReverseCar:

                reverseTime += Time.deltaTime;
                exhaustStopRequestReversal = 1;

                if (reverseTime > reverseDuration)
                {
                    currentState = AllStates.FollowPath;
                    exhaustStopRequestReversal = 0;
                    reverseTime = 0.0f;
                }

                MoveCar(reverseTorque); // Move car with reverse (negative) torque.
                SteerCar();
                SenseObstacles();
                LerpToAvoidObstacles(-1 * targetSteerAngle); // Better results in most of the cases.
                DeadlockAvoidance();
                DustEffects();
                break;

            default:
                break;
        }
    }

    private void DustEffects()
    {
        RaycastHit hit;
        Vector3 position = transform.position;
        Vector3 sensorLeft = position - transform.right * 2.5f;
        Vector3 sensorRight = position + transform.right * 2.5f;
        sensorRight.y += 0.5f;
        sensorLeft.y += 0.5f;
        // Debug.DrawLine(sensorLeft, Vector3.down, Color.blue);
        // Debug.DrawLine(sensorRight, Vector3.down, Color.red);

        if (vehicleSpeed > 4 || vehicleSpeed < -4)
        {
            if (Physics.Raycast(sensorRight, Vector3.down, out hit, vehicleHeight + 0.2f))
            {

                if (hit.collider.CompareTag("Terrain"))
                {
                    dustEmitter_R.Play(true);
                    if (mudEnabled)
                    {
                        mudEmitter_R.Play();
                    }
                }
                else
                {
                    dustEmitter_R.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    if (mudEnabled)
                    {
                        mudEmitter_R.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    }
                }
            }
            if (Physics.Raycast(sensorLeft, Vector3.down, out hit, vehicleHeight + 0.2f))
            {
                if (hit.collider.CompareTag("Terrain"))
                {
                    dustEmitter_L.Play(true);
                    if (mudEnabled)
                    {
                        mudEmitter_L.Play();
                    }
                }
                else
                {
                    dustEmitter_L.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    if (mudEnabled)
                    {
                        mudEmitter_L.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    }
                }
            }
        }
        else
        {
            dustEmitter_L.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            dustEmitter_R.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            mudEmitter_L.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            mudEmitter_R.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    private void PlayClip(String clipName)
    {
        FindObjectOfType<AudioManager>().Play(clipName);
    }

    private void StopClip(String clipName)
    {
        FindObjectOfType<AudioManager>().Stop(clipName);
    }

    private void PlayOneShot(String clipName, float volumeScale)
    {
        FindObjectOfType<AudioManager>().PlayOneShot(clipName, volumeScale);
    }

    private void ExhaustLogic()
    {
        bool startup = vehicleSpeed > 5 && vehicleSpeed < 15;
        bool gear1 = vehicleSpeed > 20 && vehicleSpeed < 27;
        bool gear2 = vehicleSpeed > 35 && vehicleSpeed < 39;
        bool gear3 = vehicleSpeed > 44 && vehicleSpeed < 49;
        if (startup || gear1 || gear2 || gear3)
        {
            exhaustStopRequestSpeed = 0;
        }
        else
        {
            exhaustStopRequestSpeed = 1;
        }

        ExhaustEmmiter();
    }

    private void ExhaustEmmiter()
    {
        exhaustStopRequests = exhaustStopRequestAvoidance + exhaustStopRequestReversal + exhaustStopRequestCompletion + exhaustStopRequestSpeed;
        if (exhaustStopRequests > 0)
        {
            exhaustEmitter.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        else
        {
            exhaustEmitter.Play(true);
        }
    }

    private void RespawnCar()
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
        currentState = AllStates.FollowPath;
    }

    private void DeadlockAvoidance()
    {
        Vector3 currentPosition = transform.position;
        float distanceFromPrevious = Vector3.Distance(previousPos, currentPosition);

        if (distanceFromPrevious < stuckThres)
        {
            stuckTimer += Time.deltaTime;
            if (stuckDuration < stuckTimer)
            {
                currentState = AllStates.ReverseCar;
            }

            if (stuckTimer > maxStuckTime)
            {
                RespawnCar();
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
                RespawnCar();
                LoopStuckTimer = 0.00f;
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
        if (GetComponent<OpponentLapCounter>() != null)
        {
            if (GetComponent<OpponentLapCounter>().completedAllLaps)
            {
                completedAllLaps = true;
                currentState = AllStates.Idle;
            }
            else
            {
                completedAllLaps = false;
            }
        }


        int currentLap = GetComponent<OpponentLapCounter>().currLap;

        if (testingMode)
        {
            currentState = AllStates.Idle;
        }

        Vector3 vehiclePosition = transform.position;
        Vector3 currNodePosition;

        if (pathAlt != null && currentLap % 2 == 0)
        {
            if (!useAlternatePath)
            {
                Transform[] altPathTransforms = pathAlt.GetComponentsInChildren<Transform>();
                nodes.Clear();
                nodes = new List<Transform>();
                for (int i = 0; i < altPathTransforms.Length; i++)
                {
                    if (altPathTransforms[i] != pathAlt.transform)
                    {
                        nodes.Add(altPathTransforms[i]);
                    }
                }
                useAlternatePath = true;
            }
        }
        else
        {
            if (useAlternatePath)
            {
                Transform[] altPathTransforms = path.GetComponentsInChildren<Transform>();
                nodes.Clear();
                nodes = new List<Transform>();
                for (int i = 0; i < altPathTransforms.Length; i++)
                {
                    if (altPathTransforms[i] != path.transform)
                    {
                        nodes.Add(altPathTransforms[i]);
                    }
                }
                useAlternatePath = false;
            }
        }


        currNodePosition = nodes[currNode].position;


        Debug.DrawLine(vehiclePosition, currNodePosition, Color.magenta);

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

    private void LerpToAvoidObstacles(float targetSteerAngle)
    {
        // targetSteerAngle = Mathf.Clamp(targetSteerAngle, -maxSteerAngle, maxSteerAngle);
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
        if (Physics.Raycast(sensorPosition, transform.forward, out hit, sensingDistance, obstacleMask))
        {
            if (!hit.collider.CompareTag("Road") && !hit.collider.CompareTag("IgnoredBySensors") && !hit.collider.CompareTag("Terrain")) //  || Terrain.activeTerrain.SampleHeight(hit.point) > 0.6f)
            {
                Debug.DrawLine(sensorPosition, hit.point, Color.blue);
                currentlyAvoiding = true;

                if (hit.collider.CompareTag("Opponent") || hit.collider.CompareTag("Player"))
                {
                    float distanceFromOpponent = Vector3.Distance(sensorPosition, hit.point);
                    avoidingSensitivity += Mathf.Lerp(0.025f, 0.05f, Mathf.InverseLerp(2f, 0.5f, distanceFromOpponent));
                }
                else
                {
                    float distanceFromObstacle = Vector3.Distance(sensorPosition, hit.point);
                    avoidingSensitivity += Mathf.Lerp(0.35f, 0.75f, Mathf.InverseLerp(25f, 1f, distanceFromObstacle));
                }
            }
        }
        else if (Physics.Raycast(sensorPosition, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensingDistance))
        {
            if (!hit.collider.CompareTag("Road") && !hit.collider.CompareTag("IgnoredBySensors") && !hit.collider.CompareTag("Terrain")) //  || Terrain.activeTerrain.SampleHeight(hit.point) > 0.6f)
            {
                Debug.DrawLine(sensorPosition, hit.point, Color.red);
                currentlyAvoiding = true;
                if (hit.collider.CompareTag("Opponent") || hit.collider.CompareTag("Player"))
                {
                    float distanceFromOpponent = Vector3.Distance(sensorPosition, hit.point);
                    avoidingSensitivity += Mathf.Lerp(0.005f, 0.05f, Mathf.InverseLerp(3f, 0f, distanceFromOpponent));
                }
                else
                {
                    float distanceFromObstacle = Vector3.Distance(sensorPosition, hit.point);
                    avoidingSensitivity += Mathf.Lerp(0.25f, 0.45f, Mathf.InverseLerp(15f, 0f, distanceFromObstacle));
                }
            }
        }

        // Front Right Sensor
        sensorPosition += 2 * transform.right * frontSideSensorPos;
        if (Physics.Raycast(sensorPosition, transform.forward, out hit, sensingDistance))
        {
            if (!hit.collider.CompareTag("Road") && !hit.collider.CompareTag("IgnoredBySensors") && !hit.collider.CompareTag("Terrain")) //  || Terrain.activeTerrain.SampleHeight(hit.point) > 0.6f)
            {
                Debug.DrawLine(sensorPosition, hit.point, Color.blue);
                currentlyAvoiding = true;

                if (hit.collider.CompareTag("Opponent") || hit.collider.CompareTag("Player"))
                {
                    float distanceFromOpponent = Vector3.Distance(sensorPosition, hit.point);
                    avoidingSensitivity += -1 * Mathf.Lerp(0.025f, 0.05f, Mathf.InverseLerp(3f, 0f, distanceFromOpponent));
                }
                else
                {
                    float distanceFromObstacle = Vector3.Distance(sensorPosition, hit.point);
                    avoidingSensitivity += -1 * Mathf.Lerp(0.35f, 0.75f, Mathf.InverseLerp(25f, 0f, distanceFromObstacle));
                }
            }
        }
        else if (Physics.Raycast(sensorPosition, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensingDistance))
        {
            if (!hit.collider.CompareTag("Road") && !hit.collider.CompareTag("IgnoredBySensors") && !hit.collider.CompareTag("Terrain")) //  || Terrain.activeTerrain.SampleHeight(hit.point) > 0.6f)
            {
                Debug.DrawLine(sensorPosition, hit.point, Color.red);
                currentlyAvoiding = true;
                // print("Right: " + Vector3.Distance(sensorPosition, hit.point));
                if (hit.collider.CompareTag("Opponent") || hit.collider.CompareTag("Player"))
                {
                    float distanceFromOpponent = Vector3.Distance(sensorPosition, hit.point);
                    avoidingSensitivity += -1 * Mathf.Lerp(0.005f, 0.05f, Mathf.InverseLerp(3f, 0f, distanceFromOpponent));
                }
                else
                {
                    float distanceFromObstacle = Vector3.Distance(sensorPosition, hit.point);
                    avoidingSensitivity += -1 * Mathf.Lerp(0.25f, 0.45f, Mathf.InverseLerp(15f, 0f, distanceFromObstacle));
                }
            }
        }

        // Side Sensors
        // Side Right
        sensorPosition = position + transform.right * 0.5f;
        sensorPosition.y += vehicleHeight;

        if (Physics.Raycast(sensorPosition, transform.right, out hit, sensingDistance * 0.6f))
        {
            if (!hit.collider.CompareTag("Road") && !hit.collider.CompareTag("IgnoredBySensors") && !hit.collider.CompareTag("Terrain"))
            {
                currentlyAvoiding = true;
                float distanceFromObstacle = Vector3.Distance(sensorPosition, hit.point);
                if (distanceFromObstacle < 5f)
                {
                    Debug.DrawLine(sensorPosition, hit.point, Color.green);
                    avoidingSensitivity += -1 * Mathf.Lerp(0.1f, 0.2f, Mathf.InverseLerp(2.8f, 0f, distanceFromObstacle));
                }
            }
        }
        else if (Physics.Raycast(sensorPosition - transform.forward * 2.3f, transform.right, out hit, sensingDistance * 0.6f))
        {
            if (!hit.collider.CompareTag("Road") && !hit.collider.CompareTag("IgnoredBySensors") && !hit.collider.CompareTag("Terrain"))
            {
                currentlyAvoiding = true;
                float distanceFromObstacle = Vector3.Distance(sensorPosition - transform.forward * 2.3f, hit.point);
                if (distanceFromObstacle < 5f)
                {
                    Debug.DrawLine(sensorPosition - transform.forward * 2.3f, hit.point, Color.green);
                    avoidingSensitivity += -1 * Mathf.Lerp(0.05f, 0.15f, Mathf.InverseLerp(2.8f, 0f, distanceFromObstacle));
                }
            }
        }
        else if (Physics.Raycast(sensorPosition + transform.forward * 2.3f, transform.right, out hit, sensingDistance * 0.6f))
        {
            if (!hit.collider.CompareTag("Road") && !hit.collider.CompareTag("IgnoredBySensors") && !hit.collider.CompareTag("Terrain"))
            {
                currentlyAvoiding = true;
                float distanceFromObstacle = Vector3.Distance(sensorPosition + transform.forward * 2.3f, hit.point);
                if (distanceFromObstacle < 5f)
                {
                    Debug.DrawLine(sensorPosition + transform.forward * 2.3f, hit.point, Color.green);
                    avoidingSensitivity += -1 * Mathf.Lerp(0.2f, 0.35f, Mathf.InverseLerp(2.8f, 0f, distanceFromObstacle));
                }
            }
        }

        // Side Left
        sensorPosition -= 2 * transform.right * 0.5f;
        if (Physics.Raycast(sensorPosition, -transform.right, out hit, sensingDistance * 0.6f))
        {
            if (!hit.collider.CompareTag("Road") && !hit.collider.CompareTag("IgnoredBySensors") && !hit.collider.CompareTag("Terrain"))
            {
                currentlyAvoiding = true;
                float distanceFromObstacle = Vector3.Distance(sensorPosition, hit.point);
                if (distanceFromObstacle < 5f)
                {
                    Debug.DrawLine(sensorPosition, hit.point, Color.red);
                    avoidingSensitivity += Mathf.Lerp(0.1f, 0.2f, Mathf.InverseLerp(2.8f, 0.5f, distanceFromObstacle));
                }
            }
        }
        else if (Physics.Raycast(sensorPosition - transform.forward * 2.3f, -transform.right, out hit, sensingDistance * 0.6f))
        {
            if (!hit.collider.CompareTag("Road") && !hit.collider.CompareTag("IgnoredBySensors") && !hit.collider.CompareTag("Terrain"))
            {
                currentlyAvoiding = true;
                float distanceFromObstacle = Vector3.Distance(sensorPosition - transform.forward * 2.3f, hit.point);
                if (distanceFromObstacle < 5f)
                {
                    Debug.DrawLine(sensorPosition - transform.forward * 2.3f, hit.point, Color.red);
                    avoidingSensitivity += Mathf.Lerp(0.05f, 0.15f, Mathf.InverseLerp(2.8f, 0.5f, distanceFromObstacle));
                }
            }
        }
        else if (Physics.Raycast(sensorPosition + transform.forward * 2.3f, -transform.right, out hit, sensingDistance * 0.6f))
        {
            if (!hit.collider.CompareTag("Road") && !hit.collider.CompareTag("IgnoredBySensors") && !hit.collider.CompareTag("Terrain"))
            {
                currentlyAvoiding = true;
                float distanceFromObstacle = Vector3.Distance(sensorPosition + transform.forward * 2.3f, hit.point);
                if (distanceFromObstacle < 5f)
                {
                    Debug.DrawLine(sensorPosition + transform.forward * 2.3f, hit.point, Color.red);
                    avoidingSensitivity += Mathf.Lerp(0.2f, 0.35f, Mathf.InverseLerp(2.8f, 0.5f, distanceFromObstacle));
                }
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
                if (!hit.collider.CompareTag("Road") && !hit.collider.CompareTag("IgnoredBySensors") && !hit.collider.CompareTag("Terrain")) //  || Terrain.activeTerrain.SampleHeight(hit.point) > 0.6f)
                {
                    Debug.DrawLine(sensorPosition, hit.point, Color.cyan);
                    currentlyAvoiding = true;


                    float distanceFromObstacle = Vector3.Distance(sensorPosition, hit.point);
                    if (distanceFromObstacle < 10f && currentState != AllStates.ReverseCar)
                    {
                        if (currentState != AllStates.AvoidObstacles && previousState != AllStates.Idle)
                        {
                            previousState = currentState;
                        }

                        currentState = AllStates.AvoidObstacles;
                        exhaustStopRequestAvoidance = 1;
                    }
                    else
                    {
                        currentState = previousState;
                        exhaustStopRequestAvoidance = 0;
                    }

                    if (hit.normal.x < 0 && !(hit.collider.CompareTag("Opponent") || hit.collider.CompareTag("Player")))
                    {
                        distanceFromObstacle = Vector3.Distance(sensorPosition, hit.point);
                        avoidingSensitivity = -1 * Mathf.Lerp(0.8f, 1.2f, Mathf.InverseLerp(7f, 0.5f, distanceFromObstacle));
                    }
                    else
                    {
                        distanceFromObstacle = Vector3.Distance(sensorPosition, hit.point);
                        avoidingSensitivity = Mathf.Lerp(0.8f, 1.2f, Mathf.InverseLerp(7f, 0.5f, distanceFromObstacle));
                        if (vehicleSpeed > 20f)
                        {
                            brakeResourceSensors = 1;
                        }
                        else
                        {
                            brakeResourceSensors = 0;
                        }
                    }

                }
            }
            else
            {
                currentState = previousState;
                brakeResourceSensors = 0;
                exhaustStopRequestAvoidance = 0;
            }
        }

        if (currentlyAvoiding)
        {
            targetSteerAngle = maxSteerAngle * avoidingSensitivity * avoidanceModifier;
        }
    }

    private void MoveCar(float targetSpeed)
    {
        // NOTE: USING FORMULA FROM - https://discussions.unity.com/t/how-to-find-out-speed-in-mph-with-wheelcollider-rpm/170279

        currentSpeed = 2 * Mathf.PI * wheelsBL.radius * wheelsFL.rpm * 60 / 1000;

        if (currentSpeed < maxSpeed || (increasedSpeed && currentSpeed < boostSpeed))
        {
            wheelsFL.motorTorque = targetSpeed * Time.deltaTime * 600;
            wheelsFR.motorTorque = targetSpeed * Time.deltaTime * 600;
            wheelsBL.motorTorque = targetSpeed * Time.deltaTime * 600;
            wheelsBR.motorTorque = targetSpeed * Time.deltaTime * 600;
        }
        else
        {
            wheelsFL.motorTorque = 0.0f;
            wheelsFR.motorTorque = 0.0f;
            wheelsBL.motorTorque = 0.0f;
            wheelsBR.motorTorque = 0.0f;
        }
    }

    private void SteerCar()
    {
        currentSteerAngle = wheelsFL.steerAngle;
        vehicleSpeed = vehicleRB.velocity.magnitude;

        if (Mathf.Abs(currentSteerAngle) > 25 && vehicleSpeed > 38 || vehicleSpeed > 55 && Mathf.Abs(currentSteerAngle) > 10)// || vehicleSpeed > 40 && Mathf.Abs(currentSteerAngle) > 28)
        {
            turningBrakeResource = 1;
        }
        else
        {
            turningBrakeResource = 0;
        }

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

                if (currentState != AllStates.AvoidObstacles)
                {
                    targetSteerAngle = newSteer;
                }
            }
            else
            {
                Vector3 relativeVector = transform.InverseTransformPoint(nodes[currNode].position);
                relativeVector = relativeVector / relativeVector.magnitude;
                float newSteer = maxSteerAngle * (relativeVector.x / relativeVector.magnitude);

                if (currentState != AllStates.AvoidObstacles)
                {
                    targetSteerAngle = newSteer;
                }
            }
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

    private void BrakeLogic()
    {
        if (IsInsideSpeedPoint && vehicleSpeed > 40)
        {
            speedControlBrakeResource = 1;
        }
        else
        {
            speedControlBrakeResource = 0;
        }

        int brakeResource = lapBrakeResource + brakeResourceSensors + forceBrakeResource + turningBrakeResource + speedControlBrakeResource;

        if (brakeResource > 0)// && brakeResource != turningBrakeResource)
        {
            ApplyBrakes(maxBrakeTorque);
        }
        else
        {
            wheelsFL.brakeTorque = 0.0f;
            wheelsFR.brakeTorque = 0.0f;
            wheelsBL.brakeTorque = 0.0f;
            wheelsBR.brakeTorque = 0.0f;
        }
    }

    private void ApplyBrakes(float brakeTorque)
    {
        wheelsFL.brakeTorque = brakeTorque * Time.deltaTime * 600;
        wheelsFR.brakeTorque = brakeTorque * Time.deltaTime * 600;
        wheelsBL.brakeTorque = brakeTorque * Time.deltaTime * 600;
        wheelsBR.brakeTorque = brakeTorque * Time.deltaTime * 600;
        wheelsFL.motorTorque = 0.0f;
        wheelsFR.motorTorque = 0.0f;
        wheelsBL.motorTorque = 0.0f;
        wheelsBR.motorTorque = 0.0f;
    }

    public IEnumerator Perform360Rotation()
    {
        if (triggerOilSlick)
        {
            float rotationAmount = 360;  // Rotate degrees
            float rotationTime = 1f;     // Duration of rotation
            float elapsedTime = 0f;      // Track time passed for rotation
            float delayAfterRotation = 1f;  // Delay time after rotation

            // Randomly select clockwise or counterclockwise rotation
            int direction = UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;
            Quaternion startRotation = transform.rotation;
            Quaternion endRotation = startRotation * Quaternion.Euler(Vector3.up * rotationAmount * direction);

            Debug.Log($"Starting rotation. Start rotation: {startRotation.eulerAngles}, End rotation: {endRotation.eulerAngles}, Direction: {(direction == 1 ? "Clockwise" : "Counterclockwise")}");

            while (elapsedTime < rotationTime)
            {
                float angle = rotationAmount * direction * (Time.deltaTime / rotationTime);
                transform.Rotate(Vector3.up, angle);
                Debug.Log("Current rotation: " + transform.rotation.eulerAngles);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.rotation = endRotation;

            Debug.Log("Rotation completed. Final rotation: " + transform.rotation.eulerAngles);

            yield return new WaitForSeconds(delayAfterRotation);
        }
    }
}