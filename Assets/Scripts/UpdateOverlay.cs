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
                if (Input.GetKeyDown("space")) {
                    current_state = states.starting;
                    Overlay.overrideSprite = start;
                }
                break;

            case states.starting:
                if (Input.GetKeyDown("return")) {
                    current_state = states.straight_normal;
                    Overlay.overrideSprite = straight_normal;
                }
                break;

            case states.straight_normal:
                if (Input.GetKeyDown("left") || Input.GetKeyDown("a")) {
                    current_state = states.left_normal;
                    Overlay.overrideSprite = left_normal;
                }
                else if (Input.GetKeyDown("right") || Input.GetKeyDown("d")) {
                    current_state = states.right_normal;
                    Overlay.overrideSprite = right_normal;
                }
                else if (Input.GetKeyDown("p")) {
                    current_state = states.straight_turbo;
                    Overlay.overrideSprite = straight_turbo;
                }
                break;

            case states.left_normal:
                if (Input.GetKeyDown("right") || Input.GetKeyDown("d")) {
                    current_state = states.right_normal;
                    Overlay.overrideSprite = right_normal;
                }
                else if (Input.GetKeyDown("up") || Input.GetKeyDown("w")) {
                    current_state = states.straight_normal;
                    Overlay.overrideSprite = straight_normal;
                }
                else if (Input.GetKeyDown("p")) {
                    current_state = states.left_turbo;
                    Overlay.overrideSprite = left_turbo;
                }
                break;

            case states.right_normal:
                if (Input.GetKeyDown("left") || Input.GetKeyDown("a")) {
                    current_state = states.left_normal;
                    Overlay.overrideSprite = left_normal;
                }
                else if (Input.GetKeyDown("up") || Input.GetKeyDown("w")) {
                    current_state = states.straight_normal;
                    Overlay.overrideSprite = straight_normal;
                }
                else if (Input.GetKeyDown("p")) {
                    current_state = states.right_turbo;
                    Overlay.overrideSprite = right_turbo;
                }
                break;

            case states.straight_turbo:
                if (Input.GetKeyDown("left") || Input.GetKeyDown("a"))
                {
                    current_state = states.left_turbo;
                    Overlay.overrideSprite = left_turbo;
                }
                else if (Input.GetKeyDown("right") || Input.GetKeyDown("d"))
                {
                    current_state = states.right_turbo;
                    Overlay.overrideSprite = right_turbo;
                }
                else if (Input.GetKeyDown("p"))
                {
                    current_state = states.straight_normal;
                    Overlay.overrideSprite = straight_normal;
                }
                break;

            case states.left_turbo:
                if (Input.GetKeyDown("right") || Input.GetKeyDown("d"))
                {
                    current_state = states.right_turbo;
                    Overlay.overrideSprite = right_turbo;
                }
                else if (Input.GetKeyDown("up") || Input.GetKeyDown("w"))
                {
                    current_state = states.straight_turbo;
                    Overlay.overrideSprite = straight_turbo;
                }
                else if (Input.GetKeyDown("p"))
                {
                    current_state = states.left_normal;
                    Overlay.overrideSprite = left_normal;
                }
                break;

            case states.right_turbo:
                if (Input.GetKeyDown("left") || Input.GetKeyDown("a"))
                {
                    current_state = states.left_turbo;
                    Overlay.overrideSprite = left_turbo;
                }
                else if (Input.GetKeyDown("up") || Input.GetKeyDown("w"))
                {
                    current_state = states.straight_turbo;
                    Overlay.overrideSprite = straight_turbo;
                }
                else if (Input.GetKeyDown("p"))
                {
                    current_state = states.right_normal;
                    Overlay.overrideSprite = right_normal;
                }
                break;

            default:
                break;
        }
    }
}
