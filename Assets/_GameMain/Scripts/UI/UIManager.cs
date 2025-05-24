using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Transform BlackScreen;
    [SerializeField] private Transform WinScreen;
    [SerializeField] private Menu _menu;
    [SerializeField] private FmodSoundSettings _fmodSoundSettings;
    
    public void Initialize()
    {
        BlackScreen.localScale = Vector3.one;
        _fmodSoundSettings.Initialize();
        _menu.Initialize();
    }

    public Menu GetMenu()
    {
        return _menu;
    }

    public void Win()
    {
        WinScreen.gameObject.SetActive(true);
        WinScreen.localScale = Vector3.zero;
        WinScreen.DOScale(1f, 0.15f);
    }
    
    public void ShowBlackScreen()
    {
        BlackScreen.localScale = Vector3.one;
    }

    public void HideBlackScreen()
    {
        BlackScreen.DOScale(0f, 0.12f);
        _menu.Restart();
    }
}
