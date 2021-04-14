using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateOverlay : MonoBehaviour {

    public Image Overlay;

    public Sprite idle;
    public Sprite start;
    public Sprite straight_normal;
    public Sprite right_normal;
    public Sprite left_normal;
    public Sprite straight_turbo;
    public Sprite right_turbo;
    public Sprite left_turbo;

    private enum states { off, starting, straight_normal, straight_turbo, left_normal, left_turbo, right_normal, right_turbo };
    private states current_state;

    // Start is called before the first frame update
    void Start() {
        Overlay.overrideSprite = idle;
        current_state = states.off;
    }

    // Update is called once per frame
    void Update() {
        
        switch (current_state) {
            case states.off:
                if (Input.GetButton("Space")) {
                    current_state = states.starting;
                    Overlay.overrideSprite = start;
                }
                break;

            case states.starting:
                if (Input.GetButton("Return")) {
                    current_state = states.straight_normal;
                    Overlay.overrideSprite = straight_normal;
                }
                break;

            case states.straight_normal:
                if (Input.GetButton("Left")) {
                    current_state = states.left_normal;
                    Overlay.overrideSprite = left_normal;
                }
                else if (Input.GetButton("Right")) {
                    current_state = states.right_normal;
                    Overlay.overrideSprite = right_normal;
                }
                else if (Input.GetButton("Power")) {
                    current_state = states.straight_turbo;
                    Overlay.overrideSprite = straight_turbo;
                }
                break;

            case states.left_normal:
                if (Input.GetButton("Right")) {
                    current_state = states.right_normal;
                    Overlay.overrideSprite = right_normal;
                }
                else if (Input.GetButton("Power")) {
                    current_state = states.left_turbo;
                    Overlay.overrideSprite = left_turbo;
                }
                else if (!(Input.GetButton("Left"))) {
                    current_state = states.straight_normal;
                    Overlay.overrideSprite = straight_normal;
                }
                break;

            case states.right_normal:
                if (Input.GetButton("Left")) {
                    current_state = states.left_normal;
                    Overlay.overrideSprite = left_normal;
                }
                else if (Input.GetButton("Power")) {
                    current_state = states.right_turbo;
                    Overlay.overrideSprite = right_turbo;
                }
                else if (!(Input.GetButton("Right"))) {
                    current_state = states.straight_normal;
                    Overlay.overrideSprite = straight_normal;
                }
                break;

            case states.straight_turbo:
                if (Input.GetButton("Left")) {
                    current_state = states.left_turbo;
                    Overlay.overrideSprite = left_turbo;
                }
                else if (Input.GetButton("Right")) {
                    current_state = states.right_turbo;
                    Overlay.overrideSprite = right_turbo;
                }
                else if (Input.GetButton("Power")) {
                    current_state = states.straight_normal;
                    Overlay.overrideSprite = straight_normal;
                }
                break;

            case states.left_turbo:
                if (Input.GetButton("Right")) {
                    current_state = states.right_turbo;
                    Overlay.overrideSprite = right_turbo;
                }
                else if (!(Input.GetButton("Left"))) {
                    current_state = states.straight_turbo;
                    Overlay.overrideSprite = straight_turbo;
                }
                else if (Input.GetButton("Power")) {
                    current_state = states.left_normal;
                    Overlay.overrideSprite = left_normal;
                }
                break;

            case states.right_turbo:
                if (Input.GetButton("Left")) {
                    current_state = states.left_turbo;
                    Overlay.overrideSprite = left_turbo;
                }
                else if (!(Input.GetButton("Right"))) {
                    current_state = states.straight_turbo;
                    Overlay.overrideSprite = straight_turbo;
                }
                else if (Input.GetButton("Power")) {
                    current_state = states.right_normal;
                    Overlay.overrideSprite = right_normal;
                }
                break;

            default:
                break;
        }
    }
}
