using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// The idea of this thing is to show the time that passes 60x as fast.
// This means that 1 minute in real life corresponds to 1 hour in the game.

public class MeasureTime : MonoBehaviour {

    private System.DateTime startTime;
    public Text timeText;

    // Start is called before the first frame update
    void Start() {
        startTime = System.DateTime.UtcNow;
        timeText.text = "06:00";
    }

    // Update is called once per frame
    void Update() {
        
        System.TimeSpan ts = System.DateTime.UtcNow - startTime;
        int total_seconds = ts.Seconds;
        int minutes = total_seconds / 60;
        int seconds = total_seconds - minutes * 60;

        // Since our starting point is 06:00, we add 6 here.
        minutes = minutes + 6;
        
        // Set minutes
        if(minutes < 10) {
            timeText.text = "0" + minutes + ":";
        }
        else {
            timeText.text = minutes + ":";
        }

        // Set seconds
        if(seconds < 10) {
            timeText.text += "0" + seconds;
        }
        else {
            timeText.text += seconds;
        }
    }
}
