using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateOverlay : MonoBehaviour {

    // Update Overlay
    public Image Overlay;

    public Sprite idle;
    public Sprite straight_normal_normal;
    public Sprite right_normal_normal;
    public Sprite left_normal_normal;
    public Sprite straight_turbo_normal;
    public Sprite right_turbo_normal;
    public Sprite left_turbo_normal;
    public Sprite straight_normal_up;
    public Sprite right_normal_up;
    public Sprite left_normal_up;
    public Sprite straight_turbo_up;
    public Sprite right_turbo_up;
    public Sprite left_turbo_up;
    public Sprite straight_normal_down;
    public Sprite right_normal_down;
    public Sprite left_normal_down;
    public Sprite straight_turbo_down;
    public Sprite right_turbo_down;
    public Sprite left_turbo_down;

    private enum running_states { off, running };
    private running_states current_running;

    private enum direction_states { straight, left, right };
    private direction_states current_direction;

    private enum speed_states { normal, fast };
    private speed_states current_speed;

    private enum leverage_states { down, normal, up }
    private leverage_states current_leverage;

    private float power_delay;
    private float leverage_delay;

    // Handle retarded users
    public TextMeshProUGUI Start_Indicator;
    private float update_alpha;
    private float fail_delay;

    // Show time
    private System.DateTime startTime;
    public Text timeText;

    // Start is called before the first frame update
    void Start() {
        // Initialize Overlay
        Overlay.overrideSprite = idle;
        current_running = running_states.off;
        current_direction = direction_states.straight;
        current_speed = speed_states.normal;
        current_leverage = leverage_states.normal;

        // Handle retarded users
        Start_Indicator.text = "";
        Color color = Start_Indicator.color;
        color.a = 0.3f;
        Start_Indicator.color = color;
        update_alpha = -0.001f;
        fail_delay = 5f;

        // Show time
        startTime = System.DateTime.UtcNow;
        timeText.text = "06:00";
    }

    // Update is called once per frame
    private void Update() {

        // Handle retarded users
        fail_delay -= Time.deltaTime;
        if (current_running == running_states.off && fail_delay < 0) {
            Handle_Retarded_Users();
        }
        else {
            Start_Indicator.text = "";
        }

        // Update time
        Update_Time();

        // Only relevant for starting the snowblower
        if (Input.GetButton("Space")) {
            current_running = running_states.running;
        }

        // Set direction
        if (Input.GetButton("Left")) {
            current_direction = direction_states.left;
        }
        else if (Input.GetButton("Right")) {
            current_direction = direction_states.right;
        }
        else {
            current_direction = direction_states.straight;
        }

        // Set speed
        if (Input.GetKeyDown("p")) {
            // We explicitely use Input.GetKeyDown here to prevent going into power mode and right back
            if(current_speed == speed_states.normal) {
                current_speed = speed_states.fast;
            }
            else {
                current_speed = speed_states.normal;
            }
        }

        // Set leverage
        if (Input.GetKeyDown("e")) {
            // We explicitely use Input.GetKeyDown here to prevent lifting up twice at the "same" time
            if (current_leverage == leverage_states.normal) {
                current_leverage = leverage_states.up;
            }
            else if (current_leverage == leverage_states.down) {
                current_leverage = leverage_states.normal;
            }
        }
        if (Input.GetKeyDown("q")) {
            // We explicitely use Input.GetKeyDown here to prevent lifting down twice at the "same" time
            if (current_leverage == leverage_states.normal) {
                current_leverage = leverage_states.down;
            }
            else if (current_leverage == leverage_states.up) {
                current_leverage = leverage_states.normal;
            }
        }

        Update_Sprite();
    }

    private void Handle_Retarded_Users() {
        Start_Indicator.text = "Start your snowblower by hitting the space button!";
        Color color = Start_Indicator.color;
        if (color.a < 0.3 || color.a > 1) {
            update_alpha = -update_alpha;
        }
        color.a += update_alpha;
        Start_Indicator.color = color;
    }

    private void Update_Time() {
        // The idea of this thing is to show the time that passes 60x as fast.
        // This means that 1 minute in real life corresponds to 1 hour in the game.

        System.TimeSpan ts = System.DateTime.UtcNow - startTime;
        int total_seconds = ts.Seconds;
        int minutes = total_seconds / 60;
        int seconds = total_seconds - minutes * 60;

        // Since our starting point is 06:00, we add 6 here.
        minutes = minutes + 6;

        // Set minutes
        if (minutes < 10) {
            timeText.text = "0" + minutes + ":";
        }
        else {
            timeText.text = minutes + ":";
        }

        // Set seconds
        if (seconds < 10) {
            timeText.text += "0" + seconds;
        }
        else {
            timeText.text += seconds;
        }
    }

    private void Update_Sprite() {

        Debug.Log(current_running + " " + current_direction + " " + current_speed + " " + current_leverage);
        
        switch (current_running, current_direction, current_speed, current_leverage) {

            case (running_states.running, direction_states.straight, speed_states.normal, leverage_states.normal):
                Overlay.overrideSprite = straight_normal_normal;
                break;

            case (running_states.running, direction_states.left, speed_states.normal, leverage_states.normal):
                Overlay.overrideSprite = left_normal_normal;
                break;

            case (running_states.running, direction_states.right, speed_states.normal, leverage_states.normal):
                Overlay.overrideSprite = right_normal_normal;
                break;

            case (running_states.running, direction_states.straight, speed_states.fast, leverage_states.normal):
                Overlay.overrideSprite = straight_turbo_normal;
                break;

            case (running_states.running, direction_states.left, speed_states.fast, leverage_states.normal):
                Overlay.overrideSprite = left_turbo_normal;
                break;

            case (running_states.running, direction_states.right, speed_states.fast, leverage_states.normal):
                Overlay.overrideSprite = right_turbo_normal;
                break;

            case (running_states.running, direction_states.straight, speed_states.normal, leverage_states.down):
                Overlay.overrideSprite = straight_normal_down;
                break;

            case (running_states.running, direction_states.left, speed_states.normal, leverage_states.down):
                Overlay.overrideSprite = left_normal_down;
                break;

            case (running_states.running, direction_states.right, speed_states.normal, leverage_states.down):
                Overlay.overrideSprite = right_normal_down;
                break;

            case (running_states.running, direction_states.straight, speed_states.fast, leverage_states.down):
                Overlay.overrideSprite = straight_turbo_down;
                break;

            case (running_states.running, direction_states.left, speed_states.fast, leverage_states.down):
                Overlay.overrideSprite = left_turbo_down;
                break;

            case (running_states.running, direction_states.right, speed_states.fast, leverage_states.down):
                Overlay.overrideSprite = right_turbo_down;
                break;

            case (running_states.running, direction_states.straight, speed_states.normal, leverage_states.up):
                Overlay.overrideSprite = straight_normal_up;
                break;

            case (running_states.running, direction_states.left, speed_states.normal, leverage_states.up):
                Overlay.overrideSprite = left_normal_up;
                break;

            case (running_states.running, direction_states.right, speed_states.normal, leverage_states.up):
                Overlay.overrideSprite = right_normal_up;
                break;

            case (running_states.running, direction_states.straight, speed_states.fast, leverage_states.up):
                Overlay.overrideSprite = straight_turbo_up;
                break;

            case (running_states.running, direction_states.left, speed_states.fast, leverage_states.up):
                Overlay.overrideSprite = left_turbo_up;
                break;

            case (running_states.running, direction_states.right, speed_states.fast, leverage_states.up):
                Overlay.overrideSprite = right_turbo_up;
                break;

            default:
                break;
        }
    }
}
