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

    // Start is called before the first frame update
    void Start()
    {
        currentState = GameState.Menu;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeGameState(GameState newState)
    {
        currentState = newState;
    }
}
