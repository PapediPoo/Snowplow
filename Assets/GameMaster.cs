using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Menu,
    Game,
    Score
}

public class GameMaster : MonoBehaviour
{
    public GameState currentState;
    public bool VRMode = false;
    public GameObject VRCam;
    public GameObject NonVRCam;

    // Start is called before the first frame update
    void Start()
    {
        currentState = GameState.Menu;
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
    }

    public void ChangeGameState(GameState newState)
    {
        currentState = newState;
    }
}
