using System;
using FMODUnity;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    [SerializeField] private EventReference MainMenuTheme;
    [SerializeField] private EventReference GameplayTheme;

    private void Start()
    {
        PlayGameplayTheme();
    }

    private void PlayMainMenuTheme()
    {
        AudioManager.inst.PlayMusic(MainMenuTheme);
    }

    private void PlayGameplayTheme()
    {
        AudioManager.inst.PlayMusic(GameplayTheme);
    }
}
