using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBlowerController : MonoBehaviour
{
    [Header("Controls")]
    [Range(0, 1)] [SerializeField] private float driveClutch;  // Controls how strongly the engine should drive the driving

    [Range(0, 1)] [SerializeField] private float augerClutch;  // Controls how strongly the engine should drive the snow clearing

    [SerializeField] private float currentSpeed;

    [Range(-1, 1)] [SerializeField] private float chuteYaw;
    [Range(-1 ,1)] [SerializeField] private float chutePitch;

    [Range(0, 1)] [SerializeField] private float controlLeverL;
    [Range(0,1)] [SerializeField] private float controlLeverR;

    [Header("Components")]
    [SerializeField]  private Vector3 centerOfMass = Vector3.zero;
    private Rigidbody rb;

    [Header("Driving Characteristics")]
    [SerializeField] private float track = 1f;          // Controls how far apart the tracks should be. Affects the turning radius
    [SerializeField] private float maxspeed = 3;        // Controls the maximum forward speed
    [SerializeField] private float reversespeed = -2f;  // Controls the maximum reverse speed
    [Range(0, 1)]
    public float lateralStiffness = 0.5f;               // Controls sideways stiffness
    [Range(0,1)]
    public float longitudinalStiffness = 0.5f;          // Controls forward stiffness
    [Range(0,1)]
    public float turnStiffness = 0.5f;                  // Controls rotational stiffness

    [Header("Clearing and Throwing")]
    [SerializeField] private float clearingCapacity = 14;   // Indicates how much snow the machine can clear [u^3/s]
    private float volume;
    private float snowResistance;                       // Clearing snow makes you smaller. 0 = no resistance, 1 = full resistance
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
    public float shiftSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = transform.position + (transform.rotation * centerOfMass);
        currentSpeed = 0f;
        augerKernel = Utils.GaussianKernel(augerKernelSize, augerKernelWeight);
        chuteKernel = Utils.GaussianKernel(chuteKernelSize, chuteKernelWeight);
    }

    private void Update()
    {

        // Reads player input
        currentSpeed += Input.GetKey("w") ? Time.deltaTime * shiftSpeed : 0;
        currentSpeed -= Input.GetKey("s") ? Time.deltaTime * shiftSpeed : 0;
        currentSpeed = Mathf.Clamp(currentSpeed, reversespeed, maxspeed);

        controlLeverL = Mathf.Lerp(controlLeverL, Input.GetKey("a") ? 1 : 0, controlSmoothing);
        controlLeverR = Mathf.Lerp(controlLeverR, Input.GetKey("d") ? 1 : 0, controlSmoothing);

        if (Input.GetKeyDown("f")) ToggleDriveClutch();
        if (Input.GetKeyDown("g")) ToggleAugerClutch();
    }

    // Game Logic Loop
    void FixedUpdate()
    {


        Drive(driveClutch, controlLeverL, controlLeverR);

        volume = Collect(augerTransform.position, augerKernel, clearingCapacity * augerClutch);
        Throw(chuteTransform.position + chuteTransform.rotation * chuteTarget, chuteKernel, volume);

    }

    void Drive(float shaftSpeed, float controlL, float controlR)
    {

        float desiredSpeed = currentSpeed * shaftSpeed * (2f - controlL - controlR) * .5f * (1f - snowResistance);
        float actualSpeed = Vector3.Dot(transform.forward, rb.velocity);

        float desiredASpeed = Mathf.Atan((controlR - controlL) / track) * shaftSpeed * currentSpeed;
        float actualASpeed = Vector3.Dot(transform.up, rb.angularVelocity);

        rb.AddForce(transform.forward * Mathf.Clamp(desiredSpeed - actualSpeed , reversespeed, maxspeed) * longitudinalStiffness / Time.fixedDeltaTime, ForceMode.Acceleration);
        rb.AddForce(transform.right * (-Vector3.Dot(transform.right, rb.velocity)) * lateralStiffness / Time.fixedDeltaTime, ForceMode.Acceleration);
        rb.AddTorque(transform.up * Mathf.Clamp(desiredASpeed - actualASpeed, 2 * reversespeed, 2 * maxspeed) * turnStiffness / Time.fixedDeltaTime, ForceMode.Acceleration);

    }

    float Collect(Vector3 position, float[,] kernel, float capacity)
    {
        float v = snowArea.ClearArea(augerTransform.position, kernel, capacity * Time.deltaTime, ClearingMode.Subtract);
        snowResistance = Mathf.Lerp(snowResistance, v / Time.deltaTime / clearingCapacity, snowResistanceSmoother
            );
        return v;
    }

    public void Throw(Vector3 position, float[,] kernel, float amount)
    {
        snowArea.ClearArea(chuteTransform.position + chuteTransform.rotation * chuteTarget, kernel, amount * (1f - chuteLoss), ClearingMode.Add);
    }

    void ToggleDriveClutch()
    {
        driveClutch = 1 - driveClutch;
    }

    void ToggleAugerClutch()
    {
        augerClutch = 1f - augerClutch;
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
    }
}
