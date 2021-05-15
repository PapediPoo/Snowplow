using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeedGauge : MonoBehaviour
{
    // Start is called before the first frame update
    public SnowBlowerController sbc;
    public Image forwardGauge;
    public Image backwardGauge;
    public TextMeshProUGUI speedText;

    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float curSpeed = sbc.GetTrueSpeed();
        float reversespeed = sbc.reversespeed;
        float maxSpeed = sbc.maxspeed;

        forwardGauge.fillAmount = Mathf.InverseLerp(0f, maxSpeed, curSpeed);
        backwardGauge.fillAmount = Mathf.InverseLerp(0f, reversespeed, curSpeed);
        speedText.text = curSpeed.ToString("0.0") + " m/s";
    }
}
