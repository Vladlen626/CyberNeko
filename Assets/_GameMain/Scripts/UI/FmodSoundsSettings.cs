using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;

public class FmodSoundSettings : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    
    [Header("FMOD Settings")]
    [SerializeField] private string musicBusPath = "bus:/Music";
    [SerializeField] private string sfxBusPath = "bus:/SFX";

    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;

    public void Initialize()
    {
        musicBus = RuntimeManager.GetBus(musicBusPath);
        sfxBus = RuntimeManager.GetBus(sfxBusPath);
        
        InitializeSlider(musicSlider, "MusicVolume", 0.7f);
        InitializeSlider(sfxSlider, "SFXVolume", 0.9f);
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
        if (slider == musicSlider) return musicBus;
        if (slider == sfxSlider) return sfxBus;
        return masterBus;
    }

    private void SetBusVolume(Bus bus, float volume)
    {
        bus.setVolume(volume);
    }
}