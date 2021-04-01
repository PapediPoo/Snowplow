using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBlowerAnimation : MonoBehaviour
{

    private SnowBlowerController sbc;
    private Rigidbody rb;

    public ParticleSystem chuteParticles;
    public ParticleSystem augerParticles;
    private float chuteParticlesInitRate;
    private float augerParticlesInitRate;

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

        if (chuteParticles)
        {
            var em = chuteParticles.emission;
            chuteParticlesInitRate = em.rateOverTimeMultiplier;
        }

        if (augerParticles)
        {
            var em = augerParticles.emission;
            augerParticlesInitRate = em.rateOverTimeMultiplier;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (auger)
        {
            auger.transform.Rotate(Vector3.right * augerRotSpeed * sbc.GetAugerDrive() * Time.deltaTime, Space.Self);
        }

        if (augerParticles)
        {
            var em = augerParticles.emission;
            // em.rateOverTimeMultiplier = sbc.GetSnowResistance() * augerParticlesInitRate;
            var m = augerParticles.main;
            m.startSize = sbc.GetSnowResistance() * 2f;
        }

        if (chute)
        {
            Vector2 chutestate = sbc.GetChuteRot();
            chute.transform.localRotation = chuteInitRot;
            chute.transform.Rotate(0, chutestate.x * chutePichtAmp, chutestate.y * chuteYawAmp);
        }

        if (chuteParticles)
        {
            var em = chuteParticles.emission;
            //em.rateOverTimeMultiplier = sbc.GetSnowResistance() * chuteParticlesInitRate;
            var m = chuteParticles.main;
            m.startSize = sbc.GetSnowResistance() * 2f;
        }

        foreach(GameObject w in wheels)
        {
            float v = Vector3.Dot(transform.forward, rb.GetPointVelocity(w.transform.position));

            w.transform.Rotate(Vector3.right * wheelRotSpeed * v * Time.deltaTime, Space.Self);
        }
    }
}
