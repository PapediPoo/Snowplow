using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Robin Schmidiger
// April 20201
// Version 1.09

public class SnowBlowerController : MonoBehaviour
{
    /// <summary>
    /// This class is responsible for controlling the snowblower, namely driving it around as well as collecting and throwing snow
    /// It also exposes a lot of parameters to the inspector that let you control the handling of the machine.
    /// </summary>

    [Header("Controls")]
    public bool useKeyboard = true;
    private GameMaster master;

    [Range(0, 1)] [SerializeField] private float driveClutch;  // Controls how strongly the engine should drive the driving

    [Range(0, 1)] [SerializeField] private float augerClutch;  // Controls how strongly the engine should drive the snow clearing

    // Holds the current driving speed of the machine. This is frequently updated by the script at runtime. It is only exposed to the inspector for debugging purposes
    [SerializeField] private float currentSpeed;

    // Controls orientation of the chute
    // Note: We had plans for letting the player control the orientation of the chute. Although the functionality for this is still in the code, we don't expose it to the player
    [Range(-1, 1)] [SerializeField] private float chuteYaw;
    [Range(-1 ,1)] [SerializeField] private float chutePitch;

    // Controls the control levers on the snowblower that are responsible for steering left and right
    [Range(0, 1)] [SerializeField] private float controlLeverL;
    [Range(0,1)] [SerializeField] private float controlLeverR;

    [Header("Components")]
    [SerializeField]  private Vector3 centerOfMass = Vector3.zero;  // Sets the offset for the center of mass of the underlying rigidbody
    [HideInInspector] public Rigidbody rb;                          // Reference to the snowblower's rigidbody

    [Header("Driving Characteristics")]
    [SerializeField] private float track = 1f;          // Controls how far apart the tracks should be. Affects the turning radius
    [SerializeField] public float maxspeed = 3;        // Controls the maximum forward speed
    [SerializeField] public float reversespeed = -2f;  // Controls the maximum reverse speed
    [Range(0, 1)]
    public float lateralStiffness = 0.5f;               // Controls sideways stiffness/slipping
    [Range(0,1)]
    public float longitudinalStiffness = 0.5f;          // Controls forward stiffness/slipping
    [Range(0,1)]
    public float turnStiffness = 0.5f;                  // Controls rotational stiffness

    [SerializeField]
    private float groundedThreshold = 0.5f;             // max distance to ground so that it still counts as grounded

    [Header("Clearing and Throwing")]
    [SerializeField] private float clearingCapacity = 14;   // Indicates how much snow the machine can clear [u^3/s]
    private float volume;
    private float snowResistance;                       // Clearing snow makes you smaller. 0 = no resistance, 1 = full resistance. This is updated frequently by the script and not exposed to the designer
    [Range(.1f, 1f)]
    [SerializeField]
    private float maxSnowResistance = .5f;              // Controls how much the snow should slow the snowblower down at max
    [Range(0f, .5f)] [SerializeField] private float snowResistanceSmoother = .03f;
    [Range(0, 1)]  public float chuteLoss = .5f;        // Controls how much of the collected snow leaves the chute
    public SnowArea snowArea;                           // Specifies the area to be cleared
    public Transform augerTransform;                    // Sets the position where the clearing takes place
    public int augerKernelSize = 5;                     // Sets clearing size (gaussian kernel)
    public float augerKernelWeight = 2;                 // Sets clearing weight (gaussian kernel)
    private float[,] augerKernel;

    public int chuteKernelSize = 5;                     // Sets chute throwing area size
    public float chuteKernelWeight = 3f;                // Sets chute throwing area weight
    private float[,] chuteKernel;

    public Transform chuteTransform;                    // Sets the chute to throw from
    public Vector3 chuteTarget;                         // Sets the offset from the chute, where the snow lands

    [Header("Inputs")]
    public float controlSmoothing = 0.2f;
    public float shiftTime = 1f;
    private float speedRef;

    // Start is called before the first frame update
    public void Start()
    {
        // Initialize the snowblower's state
        rb = GetComponent<Rigidbody>();     // Initialize reference to rigidbody
        master = FindObjectOfType<GameMaster>();    // Get reference to gamemaster

        rb.velocity = Vector3.zero;         // set initial RB speed to 0
        rb.angularVelocity = Vector3.zero;  // set initial RB angular speed to 0
        rb.centerOfMass = centerOfMass;     // Define CoM according to variable exposed in inspector

        currentSpeed = 0f;
        speedRef = 0f;

        augerKernel = Utils.GaussianKernel(augerKernelSize, augerKernelWeight); // Generate gaussian kernel for clearing snow
        chuteKernel = Utils.GaussianKernel(chuteKernelSize, chuteKernelWeight); // Generate gaussian kernel for throwing snow
    }

    private void Update()
    {

        // Reads player input
        //currentSpeed += Input.GetKey("w") ? Time.deltaTime * shiftSpeed : 0;
        //currentSpeed -= Input.GetKey("s") ? Time.deltaTime * shiftSpeed : 0;
        //currentSpeed = Mathf.Clamp(currentSpeed, reversespeed, maxspeed);

        // decide if keyboard shold be used according to setting in gamemaster
        useKeyboard = !master.VRMode;


        // Maps control variables to keyboard inputs if keyboard is used
        if (useKeyboard)
        {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, (Input.GetKey("w") ? maxspeed : 0f) + (Input.GetKey("s") ? reversespeed : 0f), ref speedRef, shiftTime);

            // The use of lerping makes the controls a bit less jittery
            controlLeverL = Mathf.Lerp(controlLeverL, Input.GetKey("a") ? 1 : 0, controlSmoothing);
            controlLeverR = Mathf.Lerp(controlLeverR, Input.GetKey("d") ? 1 : 0, controlSmoothing);

            if (Input.GetKeyDown("q")) ToggleDriveClutch();
            if (Input.GetKeyDown("e")) ToggleAugerClutch();
        }
    }

    // Game Logic Loop
    void FixedUpdate()
    {
        // Note: Actual updates to the rigidbody should not be done inside Update() as it does not match with the update frequency of the underlying physics engine


        // moves snowblower around
        Drive(driveClutch, controlLeverL, controlLeverR);

        // Collects and throws snow
        if (currentSpeed >= 0f)
        {
            // First clear snow at position of auger
            volume = Collect(augerKernel, clearingCapacity * augerClutch);

            // Then add snow to target position of auger
            Throw(chuteKernel, volume);
        }

    }

    void Drive(float shaftSpeed, float controlL, float controlR)
    {
        ///<summary>
        /// Moved the snowblower according the the speed of the shaft and the steering input
        /// </summary>
        /// <params>
        /// shaftSpeed: Controls the amount of throttle. between 0 and 1
        /// controlL:   Controls the amount of left-steering. between 0 and 1
        /// controlR:   Controls the amount of right-steering. between 0 and 1
        /// </params>

        //float desiredSpeed = currentSpeed * shaftSpeed * (2f - controlL - controlR) * .5f;
        float desiredSpeed = currentSpeed * shaftSpeed * (2f - controlL - controlR) * .5f * (1f - Mathf.Clamp(snowResistance, 0f, maxSnowResistance));
        float actualSpeed = Vector3.Dot(transform.forward, rb.velocity);

        float desiredASpeed = Mathf.Atan((controlR - controlL) / track) * shaftSpeed * currentSpeed;
        float actualASpeed = Vector3.Dot(transform.up, rb.angularVelocity);

        // Check if the snowblower is grounded. The origin of the ray is slightly moved upwards to avoid casting the ray from inside a collider
        if (Physics.Raycast(transform.position + (.1f * transform.up), -transform.up, groundedThreshold + .1f))
        {
            // basic principle here is: add_velocity = desired_velocity - current_velocity
            // since I use forces and not velocities, I multiply by mass and divide by timestep
            // I dont use ForceMode.VelocityChange because a) I want the "strength" of the vehicle to depend on its mass and b) because it produces more jitter.
            // Add forward & backward forces
            rb.AddForce(transform.forward * Mathf.Clamp(desiredSpeed - actualSpeed, reversespeed, maxspeed) * longitudinalStiffness * rb.mass / Time.fixedDeltaTime, ForceMode.Force);

            // Add sidewise forces
            rb.AddForce(transform.right * (-Vector3.Dot(transform.right, rb.velocity)) * lateralStiffness * rb.mass / Time.fixedDeltaTime, ForceMode.Force);

            // Add torque for steering
            rb.AddTorque(transform.up * Mathf.Clamp(desiredASpeed - actualASpeed, 2 * reversespeed, 2 * maxspeed) * turnStiffness / Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    float Collect(float[,] kernel, float capacity)
    {
        /// <summary>
        /// Instructs SnowArea to clear snow at the auger's position. The clearing amound depends on the kernel as well as the clearing capacity.
        /// also updates the snowResistance
        /// </summary>
        /// <params>
        /// kernel:     the shape of the area to be cleared
        /// capacity:   the amount of snow to be cleared
        /// </params>
        /// <returns>
        /// the amount of snow actually cleared. There might be less snow at the auger than the clearing capacity would allow to clear.
        /// </returns>

        // return collected snow and adjust current snow resistance
        float v = snowArea.ClearArea(augerTransform.position, kernel, capacity * Time.deltaTime, ClearingMode.Subtract);
        snowResistance = Mathf.Lerp(snowResistance, v / Time.deltaTime / clearingCapacity, snowResistanceSmoother);
        return v;
    }

    public void Throw(float[,] kernel, float amount)
    {
        ///<summary>
        /// Add snow amount to chute target position
        /// </summary>
        /// <params>
        /// kernel:     the shape of the target throwing area
        /// amount:     the amount of snow that's to be added to the target area
        /// </params>
        snowArea.ClearArea(chuteTransform.position + chuteTransform.rotation * chuteTarget, kernel, amount * (1f - chuteLoss), ClearingMode.Add);
    }

    public void ToggleDriveClutch()
    {
        driveClutch = 1 - driveClutch;
    }

    public void ToggleAugerClutch()
    {
        augerClutch = 1f - augerClutch;
    }

    public void SetControlL(float value)
    {
        controlLeverL = value;
    }

    public void SetControlR(float value)
    {
        controlLeverR = value;
    }

    public float GetSpeed()
    {
        // returns the hypothetical speed of the vehicle. This varies depending on snow resistance and current steering angle
        return currentSpeed;
    }

    public float GetTrueSpeed()
    {
        // Returns the physical speed of the vehicle
        return currentSpeed * driveClutch * (2f - controlLeverL - controlLeverR) * .5f * (1f - Mathf.Clamp(snowResistance, 0f, maxSnowResistance));
    }

    public void SetSpeed(float value)
    {
        currentSpeed = Mathf.Clamp(value, reversespeed, maxspeed);
    }

    public void ShiftSpeed(bool up)
    {
        /// <summary>
        /// Shifts the current speed up or down by the shiftTime
        /// Especially useful for the speedUp and speedDown buttons in VR-mode
        /// </summary>
        /// <params>
        /// up:     Controls the direction of the shift: true = shift up, false = shift down
        /// </params>
        if (up)
        {
            currentSpeed = Mathf.Clamp(currentSpeed + shiftTime, reversespeed, maxspeed);
        }
        else
        {
            currentSpeed = Mathf.Clamp(currentSpeed - shiftTime, reversespeed, maxspeed);
        }
    }

    public float GetAugerDrive()
    {
        return augerClutch;
    }

    public float GetWheelDrive()
    {
        return driveClutch * maxspeed;
    }

    public Vector2 GetChuteRot()
    {
        return new Vector2(chutePitch, chuteYaw);
    }

    public float GetVolume()
    {
        return volume;
    }

    public float GetSnowResistance()
    {
        return snowResistance;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position + transform.rotation * centerOfMass, .1f);

        if(chuteTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(chuteTransform.position + chuteTransform.rotation * chuteTarget, .1f);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position - (groundedThreshold * transform.up));


    }

    public void OnTriggerEnter(Collider other)
    {
        ///<summary>
        /// If the snowblower enters the EXIT area, change the game's state to "Score"
        /// </summary>
        if(other.tag == "Exit")
        {
            print("entered exit area");
            master.ChangeGameState("Score");
        }
    }
}
