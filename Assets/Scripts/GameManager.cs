using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { GS_PAUSEMENU, GS_GAME, GS_LEVELCOMPLETED, GS_GAME_OVER }


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameState currentGameState = GameState.GS_PAUSEMENU;

    void SetGameState(GameState newGameState) 
    {
        currentGameState = newGameState;
    }

    public void PauseMenu()
    {
        SetGameState(GameState.GS_PAUSEMENU);
    }

    public void InGame()
    {
        SetGameState(GameState.GS_GAME);
    }

    public void LevelComplited()
    {
        SetGameState(GameState.GS_LEVELCOMPLETED);
    }

    public void GameOver()
    {
        SetGameState(GameState.GS_GAME_OVER);
    }


    void Awake() 
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            InGame();
        }

        if (Input.GetKey(KeyCode.P))
        {
            PauseMenu();    
        }

    }
}
