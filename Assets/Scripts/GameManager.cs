using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public enum GameState { GS_PAUSEMENU, GS_GAME, GS_LEVELCOMPLETED, GS_GAME_OVER, GS_OPTIONS }


public class GameManager : MonoBehaviour
{
    const string keyHighScore = "HighScoreLevel1";

    public static GameManager instance;
    public Canvas pauseMenuCanvas;
    public Canvas levelCompletedCanvas;
    public Canvas optionsCanvas;

    public GameState currentGameState = GameState.GS_PAUSEMENU;
    public Canvas inGameCanvas;

    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text enemiesText;

    public TMP_Text LCscoreText;
    public TMP_Text LChighScoreText;

    public Image[] keysTab;
    public Image[] livesTab;


    private float timer = 0.0f;

    public int score = 0;
    private int keysFound = 0;
    public int lives = 3;
    private int enemiesKilled = 0;

    private const int maxKeysNumber = 3;
    private const int maxLivesNumber = 4;


    public void OnResumeButtonClicked()
    {
        InGame();
    }
    public void OnRestartButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OnOptionsMenuButtonClicked()
    {
        Options();
    }
    public void OnReturnToMainMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }


    void SetGameState(GameState newGameState)
    {
        if(newGameState == GameState.GS_LEVELCOMPLETED)
        {
            Scene currentScene = SceneManager.GetActiveScene();

            if(currentScene.name == "Level1")
            {
                int highScore = PlayerPrefs.GetInt(keyHighScore);

                if (highScore < score)
                {
                    highScore = score;
                    PlayerPrefs.SetInt(keyHighScore, highScore);
                }

                LCscoreText.SetText("Your score = " + score);
                LChighScoreText.SetText("Your best score = " + highScore);
            }
        }

        currentGameState = newGameState;
        inGameCanvas.enabled = true;
        pauseMenuCanvas.enabled = currentGameState == GameState.GS_PAUSEMENU;
        optionsCanvas.enabled = currentGameState == GameState.GS_OPTIONS;
        levelCompletedCanvas.enabled = currentGameState == GameState.GS_LEVELCOMPLETED;
    }

    public void PauseMenu()
    {
        Time.timeScale = 0.0f;
        SetGameState(GameState.GS_PAUSEMENU);
    }

    public void Options()
    {
        Time.timeScale = 0.0f;
        SetGameState(GameState.GS_OPTIONS);
    }

    public void InGame()
    {
		Time.timeScale = 1.0f;
		SetGameState(GameState.GS_GAME);
    }

    public void LevelCompleted()
    {
		Debug.Log("Level finished in " + timer + " seconds!");
        SetGameState(GameState.GS_LEVELCOMPLETED);
    }

    public void GameOver()
    {
		Debug.Log("Game Over");
		SetGameState(GameState.GS_GAME_OVER);
    }


    public void IncreaseQuality()
    {
        QualitySettings.IncreaseLevel();
    }
    public void DecreaseQuality()
    {
        QualitySettings.DecreaseLevel();
    }
    public void SetVolume(float val)
    {
        AudioListener.volume = val;
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
        if (!PlayerPrefs.HasKey(keyHighScore))
            PlayerPrefs.SetInt(keyHighScore, 0);

        instance = this;
        InGame();

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
        if (Input.GetKeyDown(KeyCode.Escape) && currentGameState == GameState.GS_PAUSEMENU)
        {
            InGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && currentGameState == GameState.GS_GAME)
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
