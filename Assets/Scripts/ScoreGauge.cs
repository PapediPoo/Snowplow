using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreGauge : MonoBehaviour
{
    public SnowArea sa;
    public GameMaster master;
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI ProgressText;

    // Start is called before the first frame update
    void Start()
    {
        master = FindObjectOfType<GameMaster>();
    }

    // Update is called once per frame
    void Update()
    {
        float curSnow = sa.GetRemainingSnow();
        float initSnow = sa.GetInitSnow();

        ProgressText.text = ((1f - curSnow / initSnow) * 100f).ToString("00") + "%";
        TimeText.text = Mathf.FloorToInt(master.currentTime / 60).ToString("00") + ":" + Mathf.FloorToInt(master.currentTime % 60).ToString("00");
        ScoreText.text = Mathf.FloorToInt(master.currentScore).ToString() + " Pts";
    }
}
