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
    
    public void Initialize()
    {
        pauseButton.onClick.AddListener(PauseGame);
        resumeButton.onClick.AddListener(ResumeGame);
        playAgainButton.onClick.AddListener(PlayAgain);
        quitButton.onClick.AddListener(QuitGame);
    }
    
    public void GameOver(int points)
    {
        gameOverMenuPanel.SetActive(true);
        UpdateUiScoreText(_pointsTextMeshProUGUI, 0, points);
        OpenMenu();
    }
    
    private void UpdateUiScoreText(TextMeshProUGUI scoreTmp, int oldScore, int newScore)
    {
        DOTween.To(() => oldScore, x => oldScore = x, newScore, 0.25f)
            .OnUpdate(() =>  scoreTmp.text = oldScore.ToString())
            .SetEase(Ease.Linear);
    }
    
    public void CloseMenu()
    {
        gameOverMenuPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        mainPanel.SetActive(false);
        background.SetActive(false);
    }

    // _____________ Private _____________

    private void Update()
    {
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

    private void OpenMenu()
    {
        background.SetActive(true);
        mainPanel.SetActive(true);
    }
    
    private void PauseGame()
    {
        Time.timeScale = 0;
        isPaused = true;
        pauseMenuPanel.SetActive(true);
        OpenMenu();
    }

    private void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;
        CloseMenu();
    }

    private void PlayAgain()
    {
        Time.timeScale = 1;
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
