using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum GameState
{
    Menu,
    Game,
    Score
}

public class GameMaster : MonoBehaviour
{
    [Header("States")]
    public GameState currentState;
    public GameObject gameState;
    public GameObject menuState;
    public GameObject ScoreState;

    [Header("VR / Normal Mode")]
    public bool VRMode = false;
    public GameObject VRCam;
    public GameObject NonVRCam;

    [Header("GameState")]
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

        if (Input.GetKeyDown(KeyCode.Escape) && currentState == GameState.Game)
        {
            ChangeGameState(GameState.Menu);
        }

        float curSnow = snowArea.GetRemainingSnow();
        float initSnow = snowArea.GetInitSnow();

        currentTime += Time.deltaTime;
        currentScore = ((1f - curSnow / initSnow) * 100) + (Mathf.Clamp01((300 - currentTime) / 300) * 50);
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
}
