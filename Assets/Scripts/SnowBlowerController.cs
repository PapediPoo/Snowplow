using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBlowerController : MonoBehaviour
{
    [Header("Controls")]
    public bool useKeyboard = true;
    private GameMaster master;

    [Range(0, 1)] [SerializeField] private float driveClutch;  // Controls how strongly the engine should drive the driving

    [Range(0, 1)] [SerializeField] private float augerClutch;  // Controls how strongly the engine should drive the snow clearing

    [SerializeField] private float currentSpeed;

    [Range(-1, 1)] [SerializeField] private float chuteYaw;
    [Range(-1 ,1)] [SerializeField] private float chutePitch;

    [Range(0, 1)] [SerializeField] private float controlLeverL;
    [Range(0,1)] [SerializeField] private float controlLeverR;

    [Header("Components")]
    [SerializeField]  private Vector3 centerOfMass = Vector3.zero;
    [HideInInspector] public Rigidbody rb;

    [Header("Driving Characteristics")]
    [SerializeField] private float track = 1f;          // Controls how far apart the tracks should be. Affects the turning radius
    [SerializeField] public float maxspeed = 3;        // Controls the maximum forward speed
    [SerializeField] public float reversespeed = -2f;  // Controls the maximum reverse speed
    [Range(0, 1)]
    public float lateralStiffness = 0.5f;               // Controls sideways stiffness
    [Range(0,1)]
    public float longitudinalStiffness = 0.5f;          // Controls forward stiffness
    [Range(0,1)]
    public float turnStiffness = 0.5f;                  // Controls rotational stiffness

    [SerializeField]
    private float groundedThreshold = 0.5f;             // max distance to ground so that it still counts as grounded

    [Header("Clearing and Throwing")]
    [SerializeField] private float clearingCapacity = 14;   // Indicates how much snow the machine can clear [u^3/s]
    private float volume;
    private float snowResistance;                       // Clearing snow makes you smaller. 0 = no resistance, 1 = full resistance
    [Range(.1f, 1f)]
    [SerializeField]
    private float maxSnowResistance = .5f;
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
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.centerOfMass = centerOfMass;
        currentSpeed = 0f;
        augerKernel = Utils.GaussianKernel(augerKernelSize, augerKernelWeight);
        chuteKernel = Utils.GaussianKernel(chuteKernelSize, chuteKernelWeight);
        master = FindObjectOfType<GameMaster>();
        speedRef = 0f;
    }

    private void Update()
    {

        // Reads player input
        //currentSpeed += Input.GetKey("w") ? Time.deltaTime * shiftSpeed : 0;
        //currentSpeed -= Input.GetKey("s") ? Time.deltaTime * shiftSpeed : 0;
        //currentSpeed = Mathf.Clamp(currentSpeed, reversespeed, maxspeed);

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

        // moves snowblower around
        Drive(driveClutch, controlLeverL, controlLeverR);

        // Collects and throws snow
        if (currentSpeed >= 0f)
        {
            volume = Collect(augerTransform.position, augerKernel, clearingCapacity * augerClutch);
            Throw(chuteTransform.position + chuteTransform.rotation * chuteTarget, chuteKernel, volume);
        }

    }

    void Drive(float shaftSpeed, float controlL, float controlR)
    {

        //float desiredSpeed = currentSpeed * shaftSpeed * (2f - controlL - controlR) * .5f;
        float desiredSpeed = currentSpeed * shaftSpeed * (2f - controlL - controlR) * .5f * (1f - Mathf.Clamp(snowResistance, 0f, maxSnowResistance));
        float actualSpeed = Vector3.Dot(transform.forward, rb.velocity);

        float desiredASpeed = Mathf.Atan((controlR - controlL) / track) * shaftSpeed * currentSpeed;
        float actualASpeed = Vector3.Dot(transform.up, rb.angularVelocity);

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

    float Collect(Vector3 position, float[,] kernel, float capacity)
    {
        // return collected snow and adjust current snow resistance
        float v = snowArea.ClearArea(augerTransform.position, kernel, capacity * Time.deltaTime, ClearingMode.Subtract);
        snowResistance = Mathf.Lerp(snowResistance, v / Time.deltaTime / clearingCapacity, snowResistanceSmoother
            );
        return v;
    }

    public void Throw(Vector3 position, float[,] kernel, float amount)
    {
        // add amount of snow to certain offset position
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
        if(other.tag == "Exit")
        {
            print("entered exit area");
            master.ChangeGameState("Score");
        }
    }
}
