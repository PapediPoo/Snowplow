using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction;
using System;

// Author: Robin Schmidiger
// May 2021
public enum GameState
{
    Menu,   // State indicates that game is in main menu
    Game,   // State indicates that game is being played
    Score   // State indicates that game is over and score is being displayed
}

public class GameMaster : MonoBehaviour
{
    /// <summary>
    /// This class is responsible for controlling the state of the overall program. It functions like a statemachine with 3 states: Menu, Game and Score
    /// Menu: Game is currently in main menu
    /// Game: Game is currently being played
    /// Score: Game is over and score is being displayed. Game exits after this
    /// It is also responsible for enablind/disabling certain gameobjects needed for running the states. It also initializes some values needed for the different states
    /// </summary>
    // General note: Although it would be really nice if after the score screen the game would return to the main menu, it currently just quits the game after the score screen
    // This is due to the fact that Unity does some reparenting shenanigans with interactibles. Since for the current version of the XR Plugin I can't force "ungrabbing" via code, this means that I can't actually fully reset the level via code which can result in some nasty bugs.
    // I couldn't find any solutions that work for this program, so just quitting the game is a sub-optimal but bug-free solution.

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
    // Variables, references and settings when ingame
    public float currentScore = 0f;
    public SnowBlowerController sbc;
    public float currentTime;
    public SnowArea snowArea;
    private Vector3 snowBlowerInitPos;
    private Quaternion snowBlowerInitRot;

    // Variables and references for score screen
    [Header("Score Screen")]
    public TextMeshProUGUI scoreText;

    void Start()
    {
        // Initialize game
        ChangeGameState(GameState.Menu);
        snowBlowerInitPos = sbc.transform.position;
        snowBlowerInitRot = sbc.transform.rotation.normalized;
    }

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
        /// <summary>
        /// Changes the internal game-statemachine to another state
        /// </summary>
        /// <param>
        /// newState:   The name of the state to be transitioned to. The string has to correspond to a GameState
        /// </param>
        /// Note: This method is overloaded because strings tend to be more inspector-friendly than enums.
        /// Since I sometimes call this method from an event triggered by an UI element, strings are easier to work with.
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
        /// <summary>
        /// Changes the internal game-statemachine to another state
        /// </summary>
        /// <param>
        /// newState:   The Gamestate that the statemachine switches to
        /// </param>
        /// 

        // Disable the gameObject that is responsible for the previous state
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

        // Enable the gameobject that is responsible for the new state.
        // Also initialize some state-specific variables
        switch (currentState)
        {
            case GameState.Game:
                // Reset snowblower, respawn snow, reset score and reset clock
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
                // change the text of the UI element that displays the score
                scoreText.text = "SCORE: " + Mathf.FloorToInt(currentScore) + " pts";

                break;
        }
    }

    public void ToggleVRMode()
    {
        /// <summary>
        /// Toggles VR mode on and off. This mainly changes the camera to be used
        /// </summary>
        VRMode = !VRMode;
    }


    public static void Quit()
    {
        /// <summary>
        /// quits the game
        /// </summary>
        print("quitting");
        Application.Quit();
    }
}
