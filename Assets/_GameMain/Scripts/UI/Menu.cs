using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Action OnRestart;
    [Header("UI References")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject gameOverMenuPanel;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject background;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button quitButton;
    
    [Header("Animation positions")]
    [SerializeField] private Transform upPosition;
    [SerializeField] private Transform bottomPosition;
    [SerializeField] private Transform CenterPosition;

    [Header("Text")] [SerializeField] private TextMeshProUGUI _pointsTextMeshProUGUI;
    
    private bool isPaused;
    private bool isGameOver;
    private bool isMenuOpened;
    
    public void Initialize()
    {
        pauseButton.onClick.AddListener(PauseGame);
        resumeButton.onClick.AddListener(ResumeGame);
        playAgainButton.onClick.AddListener(PlayAgain);
        quitButton.onClick.AddListener(QuitGame);
        CloseMenu();
    }
    
    public void GameOver(int points)
    {
        isGameOver = true;
        UpdateUiScoreText(_pointsTextMeshProUGUI, 0, points);
        OpenMenu(gameOverMenuPanel);
    }

    public void Restart()
    {
        isGameOver = false;
    }
    
    private void UpdateUiScoreText(TextMeshProUGUI scoreTmp, int oldScore, int newScore)
    {
        DOTween.To(() => oldScore, x => oldScore = x, newScore, 0.25f)
            .OnUpdate(() =>  scoreTmp.text = oldScore.ToString())
            .SetEase(Ease.Linear);
    }

    // _____________ Private _____________

    private void Update()
    {
        if (isGameOver) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void OpenMenu(GameObject menuType)
    {
        if (isMenuOpened) return;
        isMenuOpened = true;
        Time.timeScale = 0;
        background.SetActive(true);
        mainPanel.SetActive(true);
        menuType.SetActive(true);
    }
    
    private void CloseMenu()
    {
        if (!isMenuOpened) return;
        isMenuOpened = false;
        Time.timeScale = 1;
        gameOverMenuPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        mainPanel.SetActive(false);
        background.SetActive(false);
    }
    
    private void PauseGame()
    {
        isPaused = true;
        OpenMenu(pauseMenuPanel);
    }

    public void ResumeGame()
    {
        isPaused = false;
        CloseMenu();
    }

    private void PlayAgain()
    {
        isPaused = false;
        CloseMenu();
        OnRestart.Invoke();
    }

    private void QuitGame()
    {
        PlayerPrefs.Save();
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}
