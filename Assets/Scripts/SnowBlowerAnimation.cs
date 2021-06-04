using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Author: Robin Schmidiger
// April 2021
public class SnowBlowerAnimation : MonoBehaviour
{
    /// <summary>
    /// Responsible for updating the VISUALS of the snowblower.
    /// It reads the state of the SnowBlowerController as well as the RigidBody to control the animations.
    /// </summary>

    // Pointers to components that contain the snowblower's state
    private SnowBlowerController sbc;
    private Rigidbody rb;

    // References to the particle systems. Note that the particle systems are assumed to already be instantiated.
    public ParticleSystem chuteParticles;
    public ParticleSystem augerParticles;

    // Unity behaves somewhat weirdly when adjusting ParticleSystem.emission.rateOverTimeMultiplier. So I just leave this code commented in case I get it working.
    //private float chuteParticlesInitRate;
    //private float augerParticlesInitRate;

    // Controls the orientation of the chute
    // We initially had plans for the player being able to control the chute.
    [Header("Chute")]
    public GameObject chute;
    public float chutePichtAmp = 100f;
    public float chuteYawAmp = 50f;
    private Quaternion chuteInitRot;

    // Controls the rotation of the auger
    [Header("Auger")]
    public GameObject auger;
    public float augerRotSpeed = 720f;

    // Controls the rotation of the wheels
    [Header("Wheels")]
    public List<GameObject> wheels = new List<GameObject>();
    public float wheelRotSpeed = 720f;

    void Start()
    {
        sbc = GetComponent<SnowBlowerController>();
        rb = GetComponent<Rigidbody>();
        if (chute) chuteInitRot = chute.transform.localRotation;

        //if (chuteParticles)
        //{
        //    var em = chuteParticles.emission;
        //    chuteParticlesInitRate = em.rateOverTimeMultiplier;
        //}

        //if (augerParticles)
        //{
        //    var em = augerParticles.emission;
        //    augerParticlesInitRate = em.rateOverTimeMultiplier;
        //}
    }

    void Update()
    {
        if (auger)
        {
            // Rotates the auger
            auger.transform.Rotate(Vector3.right * augerRotSpeed * sbc.GetAugerDrive() * Time.deltaTime, Space.Self);
        }

        if (augerParticles)
        {
            // Emits snow particles from auger. Particle size depends on amount of snow cleared 

            // var em = augerParticles.emission;
            // em.rateOverTimeMultiplier = sbc.GetSnowResistance() * augerParticlesInitRate;
            var m = augerParticles.main;
            m.startSize = sbc.GetSnowResistance() * 2f;
        }

        if (chute)
        {
            // Rotates the chute
            Vector2 chutestate = sbc.GetChuteRot();
            chute.transform.localRotation = chuteInitRot;
            chute.transform.Rotate(0, chutestate.x * chutePichtAmp, chutestate.y * chuteYawAmp);
        }

        if (chuteParticles)
        {
            // Throws snow particles out of the chute
            // Emission amount depends on amount of collected snow

            // var em = chuteParticles.emission;
            // em.rateOverTimeMultiplier = sbc.GetSnowResistance() * chuteParticlesInitRate;
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
