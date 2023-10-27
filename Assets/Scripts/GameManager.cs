using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public enum GameState { GS_PAUSEMENU, GS_GAME, GS_LEVELCOMPLETED, GS_GAME_OVER }


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameState currentGameState = GameState.GS_PAUSEMENU;
    public Canvas inGameCanvas;
    public TMP_Text scoreText;
    public Image[] keysTab;

    private int score = 0;
    private int keysFound = 0;

    private const int keysNumber = 3;


    void SetGameState(GameState newGameState)
    {
        currentGameState = newGameState;
        inGameCanvas.enabled = true;
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

    public void AddPoints(int points)
    {
        score += points;
        scoreText.text = score.ToString();
    }

    public void AddKeys()
    {
        keysTab[keysFound].color = Color.red;
        keysFound++;
    }

    public bool IsEnoughKeys()
    {
        return keysFound == keysNumber;
    }


    void Awake() 
    {
        instance = this;

        for (int i = 0; i < keysNumber; i++)
        {
            keysTab[i].color = Color.gray;
        }

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
