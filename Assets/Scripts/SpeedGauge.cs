using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Author: Robin Schmidiger
// May 2021

public class SpeedGauge : MonoBehaviour
{
    /// <summary>
    /// Responsible for controlling and updating the UI elements that display the snowblower's speed.
    /// </summary>
    public SnowBlowerController sbc;
    public Image forwardGauge;          // The circle around the speed display responsible for the forward speed.  
    public Image backwardGauge;         // The circle around the speed display responsible for the backward speed.
    public TextMeshProUGUI speedText;   // The text that displays the current speed.

    // Update is called once per frame
    void LateUpdate()
    {
        // Get values to be displayed
        float curSpeed = sbc.GetTrueSpeed();
        float reversespeed = sbc.reversespeed;
        float maxSpeed = sbc.maxspeed;

        // Update UI elements according to the current speed
        forwardGauge.fillAmount = Mathf.InverseLerp(0f, maxSpeed, curSpeed);
        backwardGauge.fillAmount = Mathf.InverseLerp(0f, reversespeed, curSpeed);
        speedText.text = curSpeed.ToString("0.0") + " m/s";
    }
}
