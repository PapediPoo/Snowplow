using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Robin Schmidiger
// May 2021
public class Exit : MonoBehaviour
{
    /// <summary>
    /// Makes the EXIT text of the exit area sway up and down and scales it over time
    /// </summary>
    
    // Reference to the EXIT text
    public GameObject exitText;
    private Vector3 exitTextInitPosition;

    void Start()
    {
        // Remember initial position of EXIT text
        // NOTE: initial size is assumed to be 1
        exitTextInitPosition = exitText.transform.position;
    }

    void Update()
    {
        // sway the text up and down
        exitText.transform.position = exitTextInitPosition + (Mathf.Sin(Time.time) * 0.5f * Vector3.up);
        // scale the text a bit
        exitText.transform.localScale = Vector3.one * (1f + 0.1f * Mathf.Sin(Time.time * 1.33f));
    }
}
