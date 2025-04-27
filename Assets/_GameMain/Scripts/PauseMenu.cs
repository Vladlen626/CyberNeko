using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;

public class FMODPauseMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;

    [Header("FMOD Settings")]
    [SerializeField] private string masterBusPath = "bus:/";
    [SerializeField] private string musicBusPath = "bus:/Music";
    [SerializeField] private string sfxBusPath = "bus:/SFX";

    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;
    private bool isPaused;

    private void Awake()
    {
        masterBus = RuntimeManager.GetBus(masterBusPath);
        musicBus = RuntimeManager.GetBus(musicBusPath);
        sfxBus = RuntimeManager.GetBus(sfxBusPath);
        
        InitializeSlider(masterSlider, "MasterVolume", 0.8f);
        InitializeSlider(musicSlider, "MusicVolume", 0.7f);
        InitializeSlider(sfxSlider, "SFXVolume", 0.9f);
        
        resumeButton.onClick.AddListener(ResumeGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void InitializeSlider(Slider slider, string prefsKey, float defaultValue)
    {
        float savedValue = PlayerPrefs.GetFloat(prefsKey, defaultValue);
        slider.value = savedValue;
        
        SetBusVolume(GetBusFromSlider(slider), savedValue);
        
        slider.onValueChanged.AddListener((value) => {
            SetBusVolume(GetBusFromSlider(slider), value);
            PlayerPrefs.SetFloat(prefsKey, value);
        });
    }

    private Bus GetBusFromSlider(Slider slider)
    {
        if (slider == masterSlider) return masterBus;
        if (slider == musicSlider) return musicBus;
        if (slider == sfxSlider) return sfxBus;
        return masterBus;
    }

    private void SetBusVolume(Bus bus, float volume)
    {
        bus.setVolume(volume);
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

    private void PauseGame()
    {
        isPaused = true;
        pauseMenuPanel.SetActive(true);
    }

    private void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        PlayerPrefs.Save();
        
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