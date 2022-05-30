using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]private bool start = false;
    public bool GetStart { get { return start; } }
    [SerializeField] private bool pause = false;
    public bool GetPause { get { return pause; } }

    public enum GameStatus
    {
        Start,
        Clear,
        Over,
        End
    }

    public GameStatus gameStatus = GameStatus.Start;

    public float time = 300;
    public float maxTime = 300;

    private int score = 0;

    const int timeScore = 300;

    const int lifeScore = 2000;

    [SerializeField] Player player;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject gameOver;
    [SerializeField] Selectable gameOverSelectUI;

    [SerializeField] GameObject gameClear;
    [SerializeField] Selectable gameClearSelectUI;
    [SerializeField] Text clearTimerText;
    [SerializeField] Text clearScoreText;

    [SerializeField] Text timerText;
    [SerializeField] Text scoreText;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        instance = this;
        start = false;
        pause = false;
    }

    void Start()
    {
        
    }

    void Update()
    {

        if (!start || gameStatus == GameStatus.End)
        {
            return;
        }

        if (!pause && Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
        if (gameStatus == GameStatus.Start) {
            timerText.text = Mathf.Ceil(time).ToString() + "秒";
            time -= Time.deltaTime;
        }

        if (canvasGroup)
        {
            canvasGroup.alpha += 1.0f / 2.0f * Time.deltaTime;
            if (canvasGroup.alpha >= 1)
            {
                switch (gameStatus)
                {
                    case GameStatus.Clear:
                        gameClearSelectUI.Select();
                        break;
                    case GameStatus.Over:
                        gameOverSelectUI.Select();
                        break;
                    default:
                        break;
                }
                gameStatus = GameStatus.End;
            }
        }
    }

    public void SceneLoad()
    {
        Time.timeScale = 1;
        if (Fade.instance)
        {
            Fade.instance.SceneLoad("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
        instance = null;
    }

    public void Pause()
    {
        pause = !pause;
        pauseMenu.SetActive(pause);
        if (pause)
        {
            pauseMenu.transform.Find("Button").GetChild(0).GetChild(0).GetComponent<Button>().Select();
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void GamaStart()
    {
        start = true;
    }

    public void AddScore(int num)
    {
        score += num;
        scoreText.text = score.ToString() + "点";
    }

    public void GameOver()
    {
        gameOver.SetActive(true);
        canvasGroup = gameOver.GetComponent<CanvasGroup>();
        gameStatus = GameStatus.Over;
    }

    public void GameClear()
    {
        gameClear.SetActive(true);
        canvasGroup = gameClear.GetComponent<CanvasGroup>();
        gameStatus = GameStatus.Clear;
        score += (int)((int)(maxTime - time) * timeScore) + lifeScore * player.CurrentHealth;
        scoreText.text = score.ToString() + "点";
        clearScoreText.text = score.ToString() + " 点";
        clearTimerText.text = new TimeSpan(0, 0, (int)(maxTime - time)).ToString(@"mm\:ss");
    }

    public void RankComparison()
    {

    }
}
