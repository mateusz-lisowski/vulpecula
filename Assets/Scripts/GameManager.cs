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
    public TMP_Text timerText;
    public TMP_Text enemiesText;

    public Image[] keysTab;
    public Image[] livesTab;


    private float timer = 0.0f;

    private int score = 0;
    private int keysFound = 0;
    private int lives = 3;
    private int enemiesKilled = 0;

    private const int maxKeysNumber = 3;
    private const int maxLivesNumber = 4;


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
        int mod = keysFound % 3;
        if (mod == 0)
        {
            keysTab[keysFound].color = Color.red;
        }
        else if (mod == 1) 
        {
            keysTab[keysFound].color = Color.green;
        }
        else if (mod == 2) 
        {
            keysTab[keysFound].color = Color.blue;
        }
        
        keysFound++;
    }

    public bool IsEnoughKeys()
    {
        return keysFound == maxKeysNumber;
    }

    public bool IsDead()
    {
        return lives <= 0;
    }


    public void AddLive()
    {
        if (!IsDead())
        {
            lives++;
            livesTab[lives - 1].enabled = true;
        }
    }

    public void RemoveLive()
    {
        if (!IsDead())
        {
            livesTab[lives - 1].enabled = false;
            lives--;
        }
    }

    public void KilledEnemy()
    {
        enemiesKilled++;
        enemiesText.text = enemiesKilled.ToString();
    }

    void Awake() 
    {
        instance = this;

        for (int i = 0; i < maxKeysNumber; i++)
        {
            keysTab[i].color = Color.gray;
        }

        for (int i = lives; i < maxLivesNumber; i++)
        {
            livesTab[i].enabled = false;
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

        // Increase timer
        timer += Time.deltaTime;
        int minutes = (int)(timer / 60);
        int seconds = (int)(timer % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
