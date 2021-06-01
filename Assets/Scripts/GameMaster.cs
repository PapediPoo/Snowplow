using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction;
using System;

public enum GameState
{
    Menu,   // State indicates that game is in main menu
    Game,   // State indicates that game is being played
    Score   // State indicates that game is over and score is being displayed
}

public class GameMaster : MonoBehaviour
{
    // General note: Although it would be really nice if after the score screen the game would return to the main menu, it currently just quits the game after the score screen
    // This is due to the fact that Unity does some reparenting shenanigans with interactibles. Since for the current version of the XR Plugin I can't force "ungrabbing" via code, this means that I can't actually fully reset the level via code which can result in some nasty bugs.
    // I don't have the time to find hacky workarounds for this, so just quitting the game is a cheap but bug-free solution.

    [Header("States")]
    // Current state and game objects for state switching
    public GameState currentState;
    public GameObject gameState;
    public GameObject menuState;
    public GameObject ScoreState;

    // current game settings and camera socket gameobjects
    [Header("VR / Normal Mode")]
    public bool VRMode = false;
    public GameObject VRCam;
    public GameObject NonVRCam;
    public float masterVolume;

    [Header("GameState")]
    // Variables and settings when ingame
    public float currentScore = 0f;
    public SnowBlowerController sbc;
    public float currentTime;
    public SnowArea snowArea;
    private Vector3 snowBlowerInitPos;
    private Quaternion snowBlowerInitRot;

    [Header("Score Screen")]
    public TextMeshProUGUI scoreText;

    // Start is called before the first frame update
    void Start()
    {
        ChangeGameState(GameState.Menu);
        snowBlowerInitPos = sbc.transform.position;
        snowBlowerInitRot = sbc.transform.rotation.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        // Depending on if the game is in VR mode or not, switch the camera system
        if (VRMode)
        {
            VRCam.SetActive(true);
            NonVRCam.SetActive(false);
        }
        else
        {
            VRCam.SetActive(false);
            NonVRCam.SetActive(true);
        }

        // If esc-key is pressed, return to menu
        if (Input.GetKeyDown(KeyCode.Escape) && currentState == GameState.Game)
        {
            ChangeGameState(GameState.Menu);
        }

        float curSnow = snowArea.GetRemainingSnow();
        float initSnow = snowArea.GetInitSnow();

        // calculate score from elapsed time and percentage of cleared snow
        currentTime += Time.deltaTime;
        currentScore = ((1f - curSnow / initSnow) * 100) + (Mathf.Clamp01((300 - currentTime) / 500) * 50);
    }

    public void ChangeGameState(string newState)
    {
        switch (newState)
        {
            case "Menu":
                ChangeGameState(GameState.Menu);
                break;
            case "Game":
                ChangeGameState(GameState.Game);
                break;
            case "Score":
                ChangeGameState(GameState.Score);
                break;
        }
    }

    public void ChangeGameState(GameState newState)
    {
        switch (currentState)
        {
            case GameState.Game:
                gameState.SetActive(false);
                break;
            case GameState.Menu:
                menuState.SetActive(false);
                break;
            case GameState.Score:
                ScoreState.SetActive(false);
                break;
            default: break;
        }

        currentState = newState;

        switch (currentState)
        {
            case GameState.Game:
                // Reset snowblower and respawn snow
                gameState.SetActive(true);
                sbc.GetComponent<Rigidbody>().MovePosition(snowBlowerInitPos);
                sbc.GetComponent<Rigidbody>().MoveRotation(snowBlowerInitRot);

                sbc.Start();
                currentTime = 0f;
                currentScore = 0f;
                FindObjectOfType<SnowArea>().Start();

                break;
            case GameState.Menu:
                menuState.SetActive(true);
                break;
            case GameState.Score:
                ScoreState.SetActive(true);
                scoreText.text = "SCORE: " + Mathf.FloorToInt(currentScore) + " pts";

                break;
        }
    }

    public void ToggleVRMode()
    {
        VRMode = !VRMode;
    }

    public static void Quit()
    {
        print("quitting");
        Application.Quit();
    }
}
