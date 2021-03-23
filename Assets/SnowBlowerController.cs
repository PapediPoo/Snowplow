using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Engine
{
    public AnimationCurve torqueCurve;
    public int idleRpm;
}

public class SnowBlowerController : MonoBehaviour
{
    [Header("Controls")]
    [Range(0, 1)]
    public float driveClutch;

    [Range(0, 1)]
    public float augerClutch;

    //[Range(0, 1)]
    public float engineThrottle;

    public float chuteYaw;
    public float chutePitch;

    [Range(0, 1)]
    public float controlLeverL;
    public float controlLeverR;

    [Header("Components")]
    public Vector3 centerOfMass = Vector3.zero;
    public Engine engine;
    public List<WheelCollider> wheels = new List<WheelCollider>();

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = transform.position + (transform.rotation * centerOfMass);
    }

    // Update is called once per frame
    void Update()
    {
        foreach(WheelCollider w in wheels)
        {
            w.motorTorque = engineThrottle;
        }
    }
}
