using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class ModifiedCarController : MonoBehaviour
{
    // [Header("Wheels")]
    // [SerializeField] private GameObject frontLeftWheelTrans;
    // [SerializeField] private GameObject frontRightWheelTrans;
    // [SerializeField] private GameObject rearLeftWheelTrans;
    // [SerializeField] private GameObject rearRightWheelTrans;
    // [SerializeField] private WheelCollider frontLeftWheelCol;
    // [SerializeField] private WheelCollider frontRightWheelCol;
    // [SerializeField] private WheelCollider rearLeftWheelCol;
    // [SerializeField] private WheelCollider rearRightWheelCol;

    // [SerializeField] private GameObject frontWeights;
    // [SerializeField] private GameObject rearWeights;
    // public TextMeshProUGUI myDebugText;
    // public TextMeshProUGUI bounceText;
    public LayerMask whatIsGround;
    public CinemachineFreeLook cameraDefault;
    public CinemachineFreeLook cameraController;

    [Header("MovementValues")]
    public float accelerationVal = 30.0f;
    public float bakeVal = 50.0f;

    public float turnSensitivity = 10.0f;
    public float maxSteer = 30.0f;
    public Vector3 centerOfMass = new Vector3(0.0f, 0.0f, 0.0f);
    public float playerHeight = 2.0f;
    public float maxSpeed = 55.0f;
    public Vector3 controlOnGround = new Vector3(0f, 0f, 200f);
    public Vector3 controlInAir = new Vector3(0f, 0f, 100f);
    public AudioSource horn;
    public AudioSource brake;
    // private float maxBounceAfterTouch = 0.0f;
    // private bool haveTouchedGround;
    public float motorTorque = 2000;
    public float brakeTorque = 2000;
    public float centerOfGravityOffset = -1.0f;
    public float steeringRange = 30f;
    public float steeringRangeAtMaxSpeed = 10f;
    public float turnSpeed = 0.6f;
    public float turnModifier;
    WheelControl[] wheels;

    [SerializeField] public Transform playerShaderPos;
    private const String HORIZONTAL = "Horizontal";
    private const String VERTICAL = "Vertical";
    private const String LJS = "LeftJoystickX";
    private const String RJS = "RightJoystickX";
    // [SerializeField] private bool controllerConnected = false;
    private Gamepad gamepad;
    public float currentSpeed;

    public float rightingTorqueStrength = 10.0f;
    private bool brakeSoundPlayed = false;

    float inputY;
    float inputX;
    bool grounded;

    private Rigidbody playerRB;

    [SerializeField] public VolumeProfile cameraProfile;
    private ChromaticAberration chromatic;
    public bool usingNitros = false;

    private float nitrosTimer;
    private float currentChromaticIntensity;
    [SerializeField] private CinemachineFreeLook playerCamera;
    private float cameraDefaultFOV;
    private float cameraCurrentFOV;

    private bool completedAllLaps = false;
    private bool forceApplyBrakes = false;
    private bool frictionSet = false;
    [SerializeField] GameObject path;
    [SerializeField] private ParticleSystem exhaustEmitter;
    [SerializeField] private ParticleSystem dustEmitter_L;
    [SerializeField] private ParticleSystem dustEmitter_R;
    [SerializeField] private ParticleSystem mudEmitter_L;
    [SerializeField] private ParticleSystem mudEmitter_R;
    [SerializeField] private bool mudEnabled = false;
    private int exhaustStopRequestSpeed = 0;
    private int exhaustStopRequestCompletion = 0;
    private int exhaustStopRequestReversal = 0;
    private int exhaustStopRequestNitros = 0;
    private int exhaustStopRequests;

    public float timeOnTrack = 0.0f;
    public float timeOffTrack = 0.0f;
    public int NOSTriggered = 0;
    public int OilSlickTriggered = 0;
    public bool PortalUsed = false;
    public float averageSpeed = 0.0f;
    private float startTime;
    public float finishTime = 0.0f;
    public bool dataCollected = false;
    public float totalSpeed = 0.0f;
    public int speedCounts = 0;
    public int collisionsWithOpponents = 0;
    private bool nitroSoundPlayed = false;
    private float collisionCooldown = 0.0f;

    public GameObject PauseMenuUI;
    private PauseMenu pauseScript;

    public GameObject RaceCountdown;
    private RaceCountdown raceCountdownScript;
    private float oilSlickCooldown = 0.0f;
    private bool canTriggerOilSick = true;
    public bool rightOffTrack;
    public bool leftOffTrack;
    public int handBrakeCount = 0;

    private float timeSnap01A;
    private float timeSnap01B;
    private float timeSnap02A;
    private float timeSnap02B;
    private bool usingController = false;
    private float handBrakeCooldown = 0.0f;
    [SerializeField] private GameObject tutorialSystem;
    [SerializeField] private TutorialScript tutorialScript;

    private bool tutorialGamepad = false;
    private GameObject shieldIcon;
    public bool isShieldActive { get; private set; }

    private void Awake()
    {
        gamepad = Gamepad.current;
        shieldIcon = transform.Find("car icon shield").gameObject;
        if (shieldIcon != null)
        {
            shieldIcon.SetActive(false);  // Ensure shield is initially invisible
        }
        else
        {
            Debug.LogError("Shield icon object not found!");
        }
    }

    private void Start()
    {
        cameraDefault.Priority = 10;
        cameraController.Priority = -1;
        pauseScript = PauseMenuUI.GetComponent<PauseMenu>();
        raceCountdownScript = RaceCountdown.GetComponent<RaceCountdown>();

        timeSnap01A = Mathf.Infinity;
        timeSnap01B = Mathf.Infinity;
        timeSnap02A = Mathf.Infinity;
        timeSnap02B = Mathf.Infinity;

        playerRB = GetComponent<Rigidbody>();

        dustEmitter_L.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        dustEmitter_R.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (mudEnabled)
        {
            mudEmitter_L.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            mudEmitter_R.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        playerRB.centerOfMass += Vector3.up * centerOfGravityOffset;
        wheels = GetComponentsInChildren<WheelControl>();
        grounded = true;
        turnModifier = turnSpeed;
        completedAllLaps = false;
        forceApplyBrakes = false;
        frictionSet = false;

        totalSpeed = playerRB.velocity.magnitude;
        startTime = Time.time;
        timeOnTrack = 0.0f;
        timeOffTrack = 0.0f;

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            Cursor.visible = false;
            // Cursor.lockState = CursorLockMode.Locked;
        }

        nitrosTimer = 0f;
        currentChromaticIntensity = 0f;
        cameraProfile.TryGet(out chromatic);
        chromatic.intensity.Override(currentChromaticIntensity);
        cameraDefaultFOV = playerCamera.m_Lens.FieldOfView;

        if (tutorialSystem != null)
        {
            tutorialScript = tutorialSystem.GetComponent<TutorialScript>();
        }
    }

    private void SetDefaultCamera()
    {
        if (usingController)
        {
            cameraController.Priority = 10;
            cameraDefault.Priority = -1;
            playerCamera = cameraController;
        }
        else
        {
            cameraController.Priority = -1;
            cameraDefault.Priority = 10;
            playerCamera = cameraDefault;
        }
    }

    private void UpdateShaderPosition()
    {
        if (playerShaderPos == null)
        {
            Shader.SetGlobalVector("_Player", transform.position);
        }
        else
        {
            Shader.SetGlobalVector("_Player", playerShaderPos.position);
        }
    }

    private void Update()
    {
        if (usingController && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) | Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow)))
        {
            if (usingController && tutorialScript != null)
            {
                tutorialScript.ShowKeyboardControls();
            }
            usingController = false;
        }
        if (!usingController && gamepad != null && (gamepad.rightTrigger.ReadValue() != 0 || gamepad.leftTrigger.ReadValue() != 0))
        {
            if (!usingController && tutorialScript != null)
            {
                tutorialScript.ShowGamepadControls();
            }
            usingController = true;
        }

        SetDefaultCamera();
        UpdateShaderPosition();


        if (gamepad != null && (gamepad.rightTrigger.ReadValue() != 0 || gamepad.leftTrigger.ReadValue() != 0))
        {
            inputY = gamepad.rightTrigger.ReadValue() - gamepad.leftTrigger.ReadValue();
        }
        else
        {
            inputY = Input.GetAxis(VERTICAL);
        }

        inputX = Input.GetAxis(HORIZONTAL);

        if (currentSpeed < 1)
        {
            exhaustStopRequestCompletion = 1;
        }
        else
        {
            exhaustStopRequestCompletion = 0;
        }

        completedAllLaps = GetComponent<LapCounter>().completedAllLaps;

        if (completedAllLaps && !dataCollected)
        {
            // GetRaceStats();
            finishTime = Time.time + startTime;
            dataCollected = true;
        }

        if (usingNitros)
        {
            if (!nitroSoundPlayed)
            {
                PlayOneShot("Nitros", 1f);
            }
            ApplyNitros();
            exhaustStopRequestNitros = 1;
        }
        else if (!usingNitros && exhaustStopRequestNitros == 1)
        {
            exhaustStopRequestNitros = 0;
        }
        // if (gamepad != null)
        // {
        //     print(gamepad.buttonEast.wasPressedThisFrame + " " + gamepad.buttonEast.wasReleasedThisFrame);
        // }

        if (Input.GetKeyDown(KeyCode.E) || (gamepad != null && gamepad.leftStickButton.wasPressedThisFrame))
        {
            PlayClip("Horn");
        }
        if (Input.GetKeyUp(KeyCode.E) || (gamepad != null && gamepad.leftStickButton.wasReleasedThisFrame))
        {
            StopClip("Horn");
        }

        if (Input.GetKeyDown(KeyCode.B) || (gamepad != null && gamepad.leftStickButton.isPressed && gamepad.rightStickButton.isPressed))
        {
            RespawnPlayer();
        }

        // if (Input.GetKey(KeyCode.R))
        // {
        //     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // }

        // if (Input.GetKeyDown(KeyCode.Q))
        // {
        //     // ApplyNitros();
        //     if (usingNitros)
        //     {
        //         usingNitros = false;
        //     }
        //     else
        //     {
        //         usingNitros = true;
        //     }
        // }

        if (usingNitros)
        {
            if (nitrosTimer < 1f)
            {
                currentChromaticIntensity = Mathf.Lerp(0f, 1f, nitrosTimer);
                cameraCurrentFOV = Mathf.Lerp(cameraDefaultFOV, 105f, nitrosTimer);
                // cameraCurrentFOV = Mathf.Lerp(default, 75f, nitrosTimer);
            }
            else if (nitrosTimer < 6f)
            {
                currentChromaticIntensity = 1f;
            }
            else if (nitrosTimer < 7f)
            {
                currentChromaticIntensity = Mathf.Lerp(1f, 0f, nitrosTimer - 6f);
                // cameraCurrentFOV = Mathf.Lerp(75f, cameraDefaultFOV, nitrosTimer - 6f);
                cameraCurrentFOV = Mathf.Lerp(105f, cameraDefaultFOV, nitrosTimer - 6f);
            }
            else
            {
                nitrosTimer = 0f;
                usingNitros = false;
            }

            playerCamera.m_Lens.FieldOfView = cameraCurrentFOV;

            if (cameraProfile != null)
            {
                cameraProfile.TryGet(out ChromaticAberration chromatic);
                chromatic.intensity.Override(currentChromaticIntensity);
            }
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
        Debug.DrawLine(sensorLeft, Vector3.down, Color.blue);
        Debug.DrawLine(sensorRight, Vector3.down, Color.red);

        if (rightOffTrack && leftOffTrack && timeSnap01A == Mathf.Infinity)
        {
            timeSnap01A = Time.time;
        }

        if ((!rightOffTrack || !leftOffTrack) && timeSnap01A != Mathf.Infinity)
        {
            timeSnap01B = Time.time;
            timeOffTrack += timeSnap01B - timeSnap01A;
            timeSnap01A = Mathf.Infinity;
            timeSnap01B = Mathf.Infinity;
        }

        if (!rightOffTrack && !leftOffTrack && timeSnap02A == Mathf.Infinity)
        {
            timeSnap02A = Time.time;
        }

        if ((rightOffTrack || leftOffTrack) && timeSnap02A != Mathf.Infinity)
        {
            timeSnap02B = Time.time;
            timeOnTrack += timeSnap02B - timeSnap02A;
            timeSnap02A = Mathf.Infinity;
            timeSnap02B = Mathf.Infinity;
        }

        if (!dataCollected && !pauseScript.PauseMenuShown && raceCountdownScript.countdownOver)
        {
            if (Physics.Raycast(sensorRight, Vector3.down, out hit, playerHeight + 0.2f))
            {
                if (hit.collider.CompareTag("Road"))
                {
                    rightOffTrack = false;
                }
                else
                {
                    rightOffTrack = true;
                }
            }

            if (Physics.Raycast(sensorLeft, Vector3.down, out hit, playerHeight + 0.2f))
            {
                if (hit.collider.CompareTag("Road"))
                {
                    // timeOnTrack += Time.deltaTime;
                    leftOffTrack = false;
                }
                else
                {
                    // timeOffTrack += Time.deltaTime;
                    leftOffTrack = true;
                }
            }
        }

        if (currentSpeed > 4 || currentSpeed < -4)
        {
            if (Physics.Raycast(sensorRight, Vector3.down, out hit, playerHeight + 0.2f))
            {

                if (hit.collider.CompareTag("Terrain"))
                {
                    dustEmitter_R.Play(true);
                    if (mudEnabled)
                    {
                        mudEmitter_R.Play();
                    }
                    rightOffTrack = true;
                    // print("Right");
                }
                else
                {
                    dustEmitter_R.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    mudEmitter_R.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    rightOffTrack = false;
                }
            }
            if (Physics.Raycast(sensorLeft, Vector3.down, out hit, playerHeight + 0.2f))
            {
                if (hit.collider.CompareTag("Terrain"))
                {
                    dustEmitter_L.Play(true);
                    mudEmitter_L.Play();
                    leftOffTrack = true;
                }
                else
                {
                    dustEmitter_L.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    if (mudEnabled)
                    {
                        mudEmitter_L.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    }
                    leftOffTrack = false;
                }
            }
        }
        else
        {
            dustEmitter_R.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            dustEmitter_L.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            mudEmitter_L.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            mudEmitter_R.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        }

        if (rightOffTrack && leftOffTrack)
        {
            playerRB.drag = 0.25f;
            playerRB.angularDrag = 0.20f;
        }
        else if (rightOffTrack || leftOffTrack)
        {

            playerRB.drag = 0.17f;
            playerRB.angularDrag = 0.17f;
        }
        else
        {
            playerRB.drag = 0.07f;
            playerRB.angularDrag = 0.05f;
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

    private void OnTriggerEnter(Collider c)
    {
        if (c.GetComponent<PortalScript>())
        {
            usingNitros = true;
            PortalUsed = true;
        }
    }

    public void ActivateShield()
    {
        if (shieldIcon != null)
        {
            FindObjectOfType<AudioManager>().Play("ShieldUp");
            shieldIcon.SetActive(true);
            isShieldActive = true;
            // Debug.Log("Shield activated");
        }
    }

    public void DeactivateShield()
    {
        if (shieldIcon != null)
        {
            FindObjectOfType<AudioManager>().Play("ShieldDown");
            shieldIcon.SetActive(false);
            isShieldActive = false;
            // Debug.Log("Shield deactivated");
        }
    }


    private void RespawnPlayer()
    {
        float minDist = Mathf.Infinity;
        Transform respawnPosition = null;
        Transform nextNode;
        Vector3 direction;
        int respawnPointIndex = 0;
        if (path != null)
        {
            Transform[] childs = path.transform.GetComponentsInChildren<Transform>();
            for (int i = 0; i < childs.Length; i++)
            {
                float distanceFromNode = Vector3.Distance(childs[i].position, transform.position);
                if (distanceFromNode < minDist)
                {
                    minDist = distanceFromNode;
                    nextNode = respawnPosition;
                    respawnPosition = childs[i];
                    respawnPointIndex = i;
                }
            }
            if (respawnPointIndex != childs.Length - 1)
            {
                direction = (childs[respawnPointIndex + 1].position - childs[respawnPointIndex].position).normalized;
            }
            else
            {
                direction = (childs[0].position - childs[respawnPointIndex].position).normalized;
            }
            transform.position = respawnPosition.position;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            playerRB.velocity = Vector3.zero;
        }
    }
    // bool CarIsGrounded = IsGrounded();


    // myDebugText.text = "Velocity: " + playerRB.velocity.magnitude.ToSafeString();// + "\n" + IsGrounded().ToString();

    // if (!haveTouchedGround && playerRB.transform.position.y < 0.2)
    // {
    //     haveTouchedGround = true;
    // }

    // if (haveTouchedGround)
    // {
    //     if (playerRB.transform.position.y > maxBounceAfterTouch)
    //     {
    //         maxBounceAfterTouch = playerRB.transform.position.y;
    //         bounceText.text = maxBounceAfterTouch.ToString();
    //     }
    // }

    // private void FixedUpdate()
    // {
    //     Debug.DrawLine(playerRB.transform.position, playerRB.transform.position + playerRB.transform.forward * 1.5f, Color.red, whatIsGround);
    //     Debug.DrawLine(playerRB.transform.position, playerRB.transform.position - playerRB.transform.forward * 1.5f, Color.green, whatIsGround);
    //     //     // Debug.DrawLine(transform.position, new Vector3(transform.position.x, playerHeight, transform.position.z), Color.red, whatIsGround);

    // }

    // private void OnHorn()
    // {

    // }

    private void ExhaustLogic()
    {
        bool startup = currentSpeed > 5 && currentSpeed < 15;
        bool gear1 = currentSpeed > 20 && currentSpeed < 27;
        bool gear2 = currentSpeed > 35 && currentSpeed < 39;
        bool gear3 = currentSpeed > 46 && currentSpeed < 50;

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
        exhaustStopRequests = exhaustStopRequestReversal + exhaustStopRequestCompletion + exhaustStopRequestSpeed + exhaustStopRequestNitros;
        if (exhaustStopRequests > 0)
        {
            exhaustEmitter.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        else
        {
            exhaustEmitter.Play(true);
        }
    }

    private void ApplyNitros()
    {
        nitrosTimer += Time.deltaTime;
        nitroSoundPlayed = true;
        if (nitrosTimer < 1f)
        {
            currentChromaticIntensity = Mathf.Lerp(0f, 1f, nitrosTimer);
            cameraCurrentFOV = Mathf.Lerp(cameraDefaultFOV, 105f, nitrosTimer);
        }
        else if (nitrosTimer < 3f)
        {
            currentChromaticIntensity = 1f;
        }
        else if (nitrosTimer < 4f)
        {
            currentChromaticIntensity = Mathf.Lerp(1f, 0f, nitrosTimer - 3f);
            cameraCurrentFOV = Mathf.Lerp(105f, cameraDefaultFOV, nitrosTimer - 3f);
        }
        else
        {
            nitrosTimer = 0f;
            usingNitros = false;
            currentChromaticIntensity = 0f;
            cameraCurrentFOV = cameraDefaultFOV;
            nitroSoundPlayed = false;
        }

        if (cameraProfile != null)
        {
            chromatic.intensity.Override(currentChromaticIntensity);
        }

        playerCamera.m_Lens.FieldOfView = cameraCurrentFOV;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Opponent" && collisionCooldown <= 0)
        {
            collisionsWithOpponents++;
            collisionCooldown = 0.3f;
        }
    }
    private void FixedUpdate()
    {
        ExhaustLogic();
        DustEffects();

        float forwardSpeed = Vector3.Dot(transform.forward, playerRB.velocity);
        float speedFactor;
        speedFactor = Mathf.InverseLerp(0, maxSpeed, forwardSpeed);

        float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);

        currentSpeed = forwardSpeed;

        float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

        if (handBrakeCooldown > 0)
        {
            handBrakeCooldown -= Time.deltaTime;
        }
        if (collisionCooldown > 0)
        {
            collisionCooldown -= Time.deltaTime;
        }

        if (oilSlickCooldown > 0)
        {
            collisionCooldown -= Time.deltaTime;
        }

        // print(raceCountdownScript.countdownOver);
        if (!dataCollected && !pauseScript.PauseMenuShown && raceCountdownScript.countdownOver)
        {
            totalSpeed += playerRB.velocity.magnitude;
            speedCounts++;
            averageSpeed = totalSpeed / speedCounts;
        }

        // bool isAccelerating = Mathf.Sign(inputY) == Mathf.Sign(forwardSpeed);
        if (!completedAllLaps)
        {
            if (exhaustStopRequestCompletion == 1)
            {
                exhaustStopRequestCompletion = 0;
            }

            foreach (var thisWheel in wheels)
            {
                if (thisWheel.steerable)
                {
                    // if (forwardSpeed < 25f)
                    // {
                    //     turnModifier = turnSpeed * 1.2f;
                    // }
                    // else if (forwardSpeed > 25f && forwardSpeed < 29.0f)
                    // {
                    //     turnModifier = turnSpeed * 0.75f;
                    // }
                    // else if (forwardSpeed > 40.0f)
                    // {
                    //     turnModifier = turnSpeed * 0.45f;
                    // }
                    // else if (forwardSpeed > 45.0f)
                    // {
                    //     turnModifier = turnSpeed * 0.2f;
                    // }
                    // else if (forwardSpeed > 55.0f)
                    // {
                    //     turnModifier = turnSpeed * 0.1f;
                    // }
                    turnModifier = turnSpeed * SetTurnModifier();
                    thisWheel.wheelCollider.steerAngle = currentSteerRange * inputX * turnModifier;
                }

                if (thisWheel.motorized)
                {
                    thisWheel.wheelCollider.motorTorque = currentMotorTorque * inputY * Time.deltaTime * 1800;
                    if (!frictionSet && usingNitros)
                    {
                        WheelFrictionCurve fwdCurve = thisWheel.wheelCollider.forwardFriction;
                        WheelFrictionCurve swdCurve = thisWheel.wheelCollider.sidewaysFriction;
                        fwdCurve.stiffness *= 1.5f;
                        swdCurve.stiffness *= 1.5f;
                        thisWheel.wheelCollider.forwardFriction = fwdCurve;
                        thisWheel.wheelCollider.sidewaysFriction = swdCurve;
                        frictionSet = true;
                    }
                    else if (frictionSet && !usingNitros)
                    {
                        WheelFrictionCurve fwdCurve = thisWheel.wheelCollider.forwardFriction;
                        WheelFrictionCurve swdCurve = thisWheel.wheelCollider.sidewaysFriction;
                        fwdCurve.stiffness /= 1.5f;
                        swdCurve.stiffness /= 1.5f;
                        thisWheel.wheelCollider.forwardFriction = fwdCurve;
                        thisWheel.wheelCollider.sidewaysFriction = swdCurve;
                        frictionSet = false;
                    }
                }

                if (forwardSpeed < -15f)
                {
                    ReverseSpeedControl();
                }
            }

            // if (isAccelerating)
            // {
            // if (thisWheel.motorized)
            // {
            // thisWheel.wheelCollider.motorTorque = currentMotorTorque * inputY * Time.deltaTime * 600;
            // }
            // thisWheel.wheelCollider.brakeTorque = 0;
            // }
            // else
            // {
            //     thisWheel.wheelCollider.brakeTorque = Mathf.Abs(inputY) * brakeTorque * Time.deltaTime * 300;
            //     thisWheel.wheelCollider.motorTorque = 0;
            // }
        }
        else
        {
            forceApplyBrakes = true;
            playerRB.drag = 2;
            playerRB.angularDrag = 2;
            exhaustStopRequestCompletion = 1;
        }

        // grounded = IsGrounded();

        // AnimateWheels();
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        // brake.PlayOneShot(brake.clip, 0.5f); 
        // }
        // Move();
        // Steer();
        ApplyBrakes();
        if (!usingNitros)
        {
            SpeedControl();
        }
        // adjustWeights();
        // checkInAir();
        // speedControl();
        // grounded = IsGrounded();
        // clampVehicleRotation();
    }

    public float SetTurnModifier()
    {
        float currentSpeed = playerRB.velocity.magnitude;
        if (currentSpeed <= 0)
        {
            return 2f;
        }
        else if (currentSpeed >= maxSpeed)
        {
            return 0.1f;
        }
        else
        {
            float normalizedCurrentSpeed = currentSpeed / maxSpeed;
            float curveAdjustment = Mathf.Pow(normalizedCurrentSpeed - 0.5f, 2) * 2;
            return 0.85f - (0.85f - 0.1f) * (normalizedCurrentSpeed + curveAdjustment);
        }
    }

    private void ApplyBrakes()
    {
        if (Input.GetKey(KeyCode.Space) || forceApplyBrakes || (gamepad != null && gamepad.buttonSouth.value == 1))
        {
            if (Input.GetKeyDown(KeyCode.Space) && handBrakeCooldown <= 0)
            {
                handBrakeCount++;
                handBrakeCooldown = 0.5f;
            }
            if (!brakeSoundPlayed)
            {
                // brake.PlayOneShot(brake.clip, 0.5f);
                PlayOneShot("Brake", 0.5f);
                brakeSoundPlayed = true;
            }
            foreach (var thisWheel in wheels)
            {
                if (playerRB.velocity.magnitude > 15)
                {
                    thisWheel.wheelCollider.brakeTorque = brakeTorque * Time.deltaTime * 300;
                }
                else
                {
                    thisWheel.wheelCollider.brakeTorque = brakeTorque * Time.deltaTime * 600;
                }
                thisWheel.wheelCollider.motorTorque = 0;
            }
        }
        else
        {
            foreach (var thisWheel in wheels)
            {
                thisWheel.wheelCollider.brakeTorque = 0;
            }
            brakeSoundPlayed = false;
        }

    }

    // private void Move()
    // {
    //     frontLeftWheelCol.motorTorque = inputY * accelerationVal * 600 * Time.deltaTime;
    //     frontRightWheelCol.motorTorque = inputY * accelerationVal * 600 * Time.deltaTime;
    //     rearLeftWheelCol.motorTorque = inputY * accelerationVal * 600 * Time.deltaTime;
    //     rearRightWheelCol.motorTorque = inputY * accelerationVal * 600 * Time.deltaTime;
    // }

    // private void Steer()
    // {
    //     float steerAngle = inputX * maxSteer * turnSensitivity * 0.10f;
    //     frontLeftWheelCol.steerAngle = Mathf.Lerp(frontLeftWheelCol.steerAngle, steerAngle, Time.deltaTime * turnSpeed);
    //     frontRightWheelCol.steerAngle = Mathf.Lerp(frontRightWheelCol.steerAngle, steerAngle, Time.deltaTime * turnSpeed);
    // }

    // private void Brake()
    // {
    //     if (Input.GetKey(KeyCode.Space))
    //     {
    //         // brake.PlayOneShot(brake.clip, 0.5f);

    //         frontLeftWheelCol.brakeTorque = bakeVal * 300;
    //         frontRightWheelCol.brakeTorque = bakeVal * 300;
    //         rearLeftWheelCol.brakeTorque = bakeVal * 300;
    //         rearRightWheelCol.brakeTorque = bakeVal * 300;
    //     }
    //     else
    //     {
    //         frontLeftWheelCol.brakeTorque = 0.0f;
    //         frontRightWheelCol.brakeTorque = 0.0f; ;
    //         rearLeftWheelCol.brakeTorque = 0.0f; ;
    //         rearRightWheelCol.brakeTorque = 0.0f; ;
    //     }

    // }

    // private void FixedUpdate()
    // {

    // }

    // void FixedUpdate()
    // {

    // }

    private bool IsGrounded()
    {
        // return Physics.Raycast(transform.position, Vector3.down, GetComponent<Collider>().bounds.extents.y + 0.1f);
        // Debug.DrawRay(transform.position, Vector3.down, Color.green, playerHeight + 0.2f);
        return Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z), Vector3.down, playerHeight + 0.2f, whatIsGround);

    }

    // public void adjustWeights()
    // {
    //     grounded = IsGrounded();

    //     if (Input.GetKey(KeyCode.LeftControl))
    //     {
    //         // playerRB.centerOfMass = rearMass;
    //         if (grounded)
    //         {
    //             playerRB.AddForceAtPosition(controlOnGround, new Vector3(playerRB.transform.position.x, playerRB.transform.position.y, playerRB.transform.position.z - 1), ForceMode.Force);
    //         }
    //         else
    //         {
    //             playerRB.AddForceAtPosition(controlInAir, new Vector3(playerRB.transform.position.x, playerRB.transform.position.y, playerRB.transform.position.z - 1), ForceMode.Force);
    //         }
    //     }
    //     else if (Input.GetKey(KeyCode.LeftShift))
    //     {
    //         // playerRB.centerOfMass = forwardMass;
    //         if (grounded)
    //         {
    //             playerRB.AddForceAtPosition(controlOnGround, new Vector3(playerRB.transform.position.x, playerRB.transform.position.y, playerRB.transform.position.z + 1), ForceMode.Force);
    //         }
    //         else
    //         {
    //             playerRB.AddForceAtPosition(controlInAir, new Vector3(playerRB.transform.position.x, playerRB.transform.position.y, playerRB.transform.position.z + 1), ForceMode.Force);
    //         }
    //     }
    // }

    public void MoveInput(float input)
    {
        inputY = input;
    }

    public void SteerInput(float input)
    {
        inputX = input;
    }

    private void GetInputs()
    {
        inputY = Input.GetAxis(VERTICAL);
        inputX = Input.GetAxis(HORIZONTAL);
    }


    // private void AnimateWheels()
    // {
    //     turnWheel(frontLeftWheelCol, frontLeftWheelTrans.transform);
    //     turnWheel(frontRightWheelCol, frontRightWheelTrans.transform);
    //     turnWheel(rearLeftWheelCol, rearLeftWheelTrans.transform);
    //     turnWheel(rearRightWheelCol, rearRightWheelTrans.transform);
    // }

    // private void turnWheel(WheelCollider thisWheel, Transform thisObject)
    // {
    //     Vector3 position;
    //     Quaternion rotation;
    //     thisWheel.GetWorldPose(out position, out rotation);
    //     thisObject.position = position;
    //     thisObject.rotation = rotation;
    // }

    private void ReverseSpeedControl()
    {
        Vector3 flatVel = new Vector3(playerRB.velocity.x, 0f, playerRB.velocity.z);

        if (flatVel.magnitude > 15)
        {
            Vector3 limitedVel = flatVel.normalized * 15;
            playerRB.velocity = new Vector3(limitedVel.x, 0f, limitedVel.z);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(playerRB.velocity.x, 0f, playerRB.velocity.z);

        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            playerRB.velocity = new Vector3(limitedVel.x, 0f, limitedVel.z);
        }
    }

    //     if (playerRB.velocity.magnitude < 10.0f)
    //     {
    //         // playerRB.drag = 0.7f;
    //         // playerRB.angularDrag = 0.7f;
    //         turnSensitivity = 10f;
    //         maxSteer = 30f;
    //     }
    //     else if (playerRB.velocity.magnitude > 10.0f && playerRB.velocity.magnitude < 12.5f)
    //     {
    //         // playerRB.drag = 0.5f;
    //         // playerRB.angularDrag = 0.6f;
    //         // turnSensitivity = 5f;
    //         maxSteer = 20f;
    //     }
    //     else
    //     {
    //         // playerRB.drag = 0.4f;
    //         // playerRB.angularDrag = 0.5f;
    //         maxSteer = 15f;
    //     }
    // }

    public IEnumerator IncreaseSpeed(float resetTime)
    {
        //Comment Debug.Log("Increasing speed");
        float originalAcceleration = accelerationVal;
        float originalMaxSpeed = maxSpeed;
        //Comment Debug.Log($"Original Acceleration: {originalAcceleration}, Original Max Speed: {originalMaxSpeed}");

        accelerationVal *= 1.5f;
        maxSpeed *= 1.2f;
        usingNitros = true; // set the boolean value true till the duration of the speedboost

        //Comment Debug.Log($"Increased Acceleration: {accelerationVal}, Increased Max Speed: {maxSpeed}");

        //Comment Debug.Log($"Waiting for {resetTime} seconds...");

        yield return new WaitForSeconds(resetTime);

        // Reset the car's speed to its original value
        accelerationVal = originalAcceleration;
        maxSpeed = originalMaxSpeed;
        NOSTriggered++;
        //Comment Debug.Log($"Reset Acceleration: {accelerationVal}, Reset Max Speed: {maxSpeed}");
    }

    public IEnumerator Perform360Rotation()
    {
        if (isShieldActive)
        {
            Debug.Log("Rotation skipped because shield is active or already rotating");
            yield break; // Do nothing if the shield is active or already rotating
        }

        if (canTriggerOilSick && currentSpeed > 15)
        {
            FindObjectOfType<AudioManager>().Play("Screech");
            float rotationAmount = 360;  // Rotate degrees
            float rotationTime = 1f;     // Duration of rotation
            float elapsedTime = 0f;      // Track time passed for rotation
            float delayAfterRotation = 1f;  // Delay time after rotation

            // Randomly select clockwise or counterclockwise rotation
            int direction = UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;
            Quaternion startRotation = transform.rotation;
            Quaternion endRotation = startRotation * Quaternion.Euler(Vector3.up * rotationAmount * direction);

            while (elapsedTime < rotationTime)
            {
                float angle = rotationAmount * direction * (Time.deltaTime / rotationTime);
                transform.Rotate(Vector3.up, angle);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.rotation = endRotation;
            Debug.Log("Rotation completed. Final rotation: " + transform.rotation.eulerAngles);

            yield return new WaitForSeconds(delayAfterRotation);
            OilSlickTriggered++;
            oilSlickCooldown = 2f;
        }
    }

}
