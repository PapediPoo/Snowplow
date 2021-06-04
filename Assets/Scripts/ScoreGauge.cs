using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Author: Robin Schmidiger
// May 2021
public class ScoreGauge : MonoBehaviour
{
    /// <summary>
    /// Updates the UI elements that are responsible for communicating the current score, clearing progress and game time to the player
    /// </summary>
    /// 
    // References to the components that hold clearing progress and game time.
    public SnowArea sa;
    public GameMaster master;

    // References to the different UI elements

    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI ProgressText;

    void Start()
    {
        // Since there should only be one GameMaster, this doesn't have to be set in the inspector
        master = FindObjectOfType<GameMaster>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get current snow status from SnowArea
        float curSnow = sa.GetRemainingSnow();
        float initSnow = sa.GetInitSnow();

        // Update the different texts of the UI
        ProgressText.text = ((1f - curSnow / initSnow) * 100f).ToString("00") + "%";
        TimeText.text = Mathf.FloorToInt(master.currentTime / 60).ToString("00") + ":" + Mathf.FloorToInt(master.currentTime % 60).ToString("00");
        ScoreText.text = Mathf.FloorToInt(master.currentScore).ToString() + " Pts";
    }
}
