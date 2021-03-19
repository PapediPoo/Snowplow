using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleControl : MonoBehaviour
{
    public Rigidbody rb;
    public float moveSpeed = 1f;
    public float rotSpeed = .3f;

    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * moveSpeed * Input.GetAxis("Vertical");
        transform.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Horizontal"));
    }
}
