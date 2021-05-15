using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreGauge : MonoBehaviour
{
    public SnowArea sa;
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI ProgressText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float curSnow = sa.GetRemainingSnow();
        float initSnow = sa.GetInitSnow();

        ProgressText.text = ((1f - curSnow / initSnow) * 100f).ToString("00") + "%";
        TimeText.text = (Time.time / 60).ToString("00") + ":" + (Time.time % 60).ToString("00");
    }
}
