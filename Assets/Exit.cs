using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    public GameObject exitText;
    private Vector3 exitTextInitPosition;

    // Start is called before the first frame update
    void Start()
    {
        exitTextInitPosition = exitText.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        exitText.transform.position = exitTextInitPosition + (Mathf.Sin(Time.time) * 0.5f * Vector3.up);
        exitText.transform.localScale = Vector3.one * (1f + 0.1f * Mathf.Sin(Time.time * 1.33f));
    }

    private void OnTriggerEnter(Collider other)
    {
        print("sadfasdfasdf");
    }
}
