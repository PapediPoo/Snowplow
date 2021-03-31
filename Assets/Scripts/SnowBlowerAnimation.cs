using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBlowerAnimation : MonoBehaviour
{

    private SnowBlowerController sbc;
    private Rigidbody rb;

    [Header("Chute")]
    public GameObject chute;
    public float chutePichtAmp = 100f;
    public float chuteYawAmp = 50f;
    private Quaternion chuteInitRot;

    [Header("Auger")]
    public GameObject auger;
    public float augerRotSpeed = 720f;


    [Header("Wheels")]
    public List<GameObject> wheels = new List<GameObject>();
    public float wheelRotSpeed = 720f;

    // Start is called before the first frame update
    void Start()
    {
        sbc = GetComponent<SnowBlowerController>();
        rb = GetComponent<Rigidbody>();
        if (chute) chuteInitRot = chute.transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (auger)
        {
            auger.transform.Rotate(Vector3.right * augerRotSpeed * sbc.GetAugerDrive() * Time.deltaTime, Space.Self);
        }

        if (chute)
        {
            Vector2 chutestate = sbc.GetChuteRot();
            chute.transform.localRotation = chuteInitRot;
            chute.transform.Rotate(0, chutestate.x * chutePichtAmp, chutestate.y * chuteYawAmp);
        }

        foreach(GameObject w in wheels)
        {
            float v = Vector3.Dot(transform.forward, rb.GetPointVelocity(w.transform.position));

            w.transform.Rotate(Vector3.right * wheelRotSpeed * v * Time.deltaTime, Space.Self);
        }
    }
}
