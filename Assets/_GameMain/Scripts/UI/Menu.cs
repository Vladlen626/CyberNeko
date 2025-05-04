using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
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
    
    private bool isPaused;
    
    public void Initialize()
    {
        pauseButton.onClick.AddListener(PauseGame);
        resumeButton.onClick.AddListener(ResumeGame);
        quitButton.onClick.AddListener(QuitGame);
    }

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

    private void CloseMenu()
    {
        gameOverMenuPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        mainPanel.SetActive(false);
        background.SetActive(false);
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

    private void GameOver()
    {
        Time.timeScale = 1;
        gameOverMenuPanel.SetActive(true);
        OpenMenu();
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
