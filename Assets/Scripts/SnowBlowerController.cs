using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBlowerController : MonoBehaviour
{
    [Header("Controls")]
    [Range(0, 1)]
    public float driveClutch;

    [Range(0, 1)]
    public float augerClutch;

    public float currentSpeed;

    [Range(0, 1)]
    public float engineThrottle = 1;

    [Range(-1, 1)]
    public float chuteYaw;
    [Range(-1 ,1)]
    public float chutePitch;

    [Range(0, 1)]
    public float controlLeverL;
    [Range(0,1)]
    public float controlLeverR;

    [Header("Components")]
    public Vector3 centerOfMass = Vector3.zero;
    private Rigidbody rb;

    [Header("Driving Characteristics")]
    public float track = 1f;
    public float maxspeed = 3;
    public float reversespeed = -2f;
    [Range(0, 1)]
    public float lateralStiffness = 0.5f;
    [Range(0,1)]
    public float longitudinalStiffness = 0.5f;
    [Range(0,1)]
    public float turnStiffness = 0.5f;

    [Header("Clearing and Throwing")]
    public float clearingCapacity = 14;
    public SnowArea snowArea;
    public Transform augerTransform;
    public int augerKernelSize = 5;
    [Range(0, 1)]
    public float chuteLoss = .5f;
    public float augerKernelWeight = 2;
    private float[,] augerKernel;

    public int chuteKernelSize = 5;
    public float chuteKernelWeight = 3f;
    private float[,] chuteKernel;

    public Transform chuteTransform;
    public Vector3 chuteTarget;

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
        currentSpeed += Input.GetKey("w") ? Time.deltaTime * shiftSpeed : 0;
        currentSpeed -= Input.GetKey("s") ? Time.deltaTime * shiftSpeed : 0;
        currentSpeed = Mathf.Clamp(currentSpeed, reversespeed, maxspeed);

        controlLeverL = Mathf.Lerp(controlLeverL, Input.GetKey("a") ? 1 : 0, controlSmoothing);
        controlLeverR = Mathf.Lerp(controlLeverR, Input.GetKey("d") ? 1 : 0, controlSmoothing);

        if (Input.GetKeyDown("f")) ToggleDriveClutch();
        if (Input.GetKeyDown("g")) ToggleAugerClutch();
    }

    // Update is called once per frame
    void FixedUpdate()
    {


        Drive(engineThrottle * driveClutch, controlLeverL, controlLeverR);
        // snowArea.ClearArea(augerCollider, clearingCapacity * Time.fixedDeltaTime);
        float volume = snowArea.ClearArea(augerTransform.position, augerKernel, clearingCapacity * Time.fixedDeltaTime, ClearingMode.Subtract);
        snowArea.ClearArea(chuteTransform.position + chuteTransform.rotation * chuteTarget, chuteKernel, volume * (1f - chuteLoss), ClearingMode.Add);
        // Collect(engineThrottle * augerClutch);
    }

    void Drive(float shaftSpeed, float controlL, float controlR)
    {

        float desiredSpeed = currentSpeed * shaftSpeed * (1.6f - controlL - controlR) * .5f;
        float actualSpeed = Vector3.Dot(transform.forward, rb.velocity);

        float desiredASpeed = Mathf.Atan((controlR - controlL) / track) * shaftSpeed * currentSpeed;
        float actualASpeed = Vector3.Dot(transform.up, rb.angularVelocity);

        //transform.position += transform.forward * desiredSpeed * Time.fixedDeltaTime;
        //transform.Rotate(transform.up * Mathf.Rad2Deg * (Mathf.Atan((controlR - controlL) / width)) * driveSpeed * relativeSpeed * Time.fixedDeltaTime);
        rb.AddForce(transform.forward * Mathf.Clamp(desiredSpeed - actualSpeed , reversespeed, maxspeed) * longitudinalStiffness / Time.fixedDeltaTime, ForceMode.Acceleration);
        rb.AddForce(transform.right * (-Vector3.Dot(transform.right, rb.velocity)) * lateralStiffness / Time.fixedDeltaTime, ForceMode.Acceleration);
        rb.AddTorque(transform.up * Mathf.Clamp(desiredASpeed - actualASpeed, 2 * reversespeed, 2 * maxspeed) * turnStiffness / Time.fixedDeltaTime, ForceMode.Acceleration);

    }

    float Collect(float shaftSpeed)
    {
        return 0f;
    }

    public void Throw(Vector3 position, float amount)
    {

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
        return augerClutch * engineThrottle;
    }

    public float GetWheelDrive()
    {
        return driveClutch * engineThrottle * maxspeed;
    }

    public Vector2 GetChuteRot()
    {
        return new Vector2(chutePitch, chuteYaw);
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
