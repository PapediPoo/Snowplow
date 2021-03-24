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

    [Range(0, 1)]
    public float engineThrottle;

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

    public float driveSpeed = 3;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = transform.position + (transform.rotation * centerOfMass);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        engineThrottle = Input.GetKey("w") ? 1 : 0;
        controlLeverL = Input.GetKey("a") ? 1 : 0;
        controlLeverR = Input.GetKey("d") ? 1 : 0;

        if (Input.GetKeyDown("f")) ToggleDriveClutch();
        if (Input.GetKeyDown("g")) ToggleAugerClutch();

        Drive(engineThrottle * driveClutch, controlLeverL, controlLeverR);
    }

    void Drive(float relativeSpeed, float controlL, float controlR)
    {
        float width = 1f;
        Vector3 posL = transform.position - (transform.right * width);
        Vector3 posR = transform.position + (transform.right * width);

        float desiredSpeedL = relativeSpeed * (1f - controlL);
        float desiredSpeedR = relativeSpeed * (1f - controlR);

        float actualSpeedL = Vector3.Dot(transform.forward, rb.GetPointVelocity(posL)) / driveSpeed;
        float actualSpeedR = Vector3.Dot(transform.forward, rb.GetPointVelocity(posR)) / driveSpeed;

        float desiredSpeed = driveSpeed * relativeSpeed * (2f - controlL - controlR) * .5f;
        float actualSpeed = Vector3.Dot(transform.forward, rb.velocity);

        float LRSlideCoeff = .5f;
        float steerAmp = 1f;
        // TODO: Use Dot product instead of vector projection to deal with backwards-freefalls
        // rb.AddForce(transform.forward * (desiredSpeed - actualSpeed), ForceMode.VelocityChange);
        // rb.AddForce(-LRSlideCoeff * Vector3.Project(rb.velocity, transform.right), ForceMode.VelocityChange);
        transform.position += transform.forward * desiredSpeed * Time.fixedDeltaTime;
        transform.Rotate(transform.up * Mathf.Rad2Deg * (Mathf.Atan((controlR - controlL) / width)) * driveSpeed * relativeSpeed * Time.fixedDeltaTime);
        // rb.AddTorque(transform.up * Mathf.Rad2Deg / Time.fixedDeltaTime * (Mathf.Atan2((controlR - controlL), width) * relativeSpeed * driveSpeed - Vector3.Dot(transform.up, rb.angularVelocity)), ForceMode.Acceleration);
        //rb.AddForceAtPosition(transform.forward * driveSpeed * (desiredSpeedL - actualSpeedL), posL, ForceMode.Acceleration);
        //rb.AddForceAtPosition(transform.forward * driveSpeed * (desiredSpeedR - actualSpeedR), posR, ForceMode.Acceleration);
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
        return driveClutch * engineThrottle * driveSpeed;
    }

    public Vector2 GetChuteRot()
    {
        return new Vector2(chutePitch, chuteYaw);
    }
}
