using System;
using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;

public class BackgroundMusicManager : MonoBehaviour
{
    [SerializeField] private EventReference AlertTheme;
    [SerializeField] private EventReference GameplayTheme;

    private void Start()
    {
        PlayGameplayTheme();
    }

    private void PlayMainMenuTheme()
    {
        AudioManager.inst.PlayMusic(AlertTheme);
    }

    private void PlayGameplayTheme()
    {
        AudioManager.inst.PlayMusic(GameplayTheme);
    }
}
