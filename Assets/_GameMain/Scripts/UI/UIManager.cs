using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Transform BlackScreen;
    
    [SerializeField] private Menu _menu;
    [SerializeField] private FmodSoundSettings _fmodSoundSettings;
    
    public async UniTask Initialize()
    {
        BlackScreen.localScale = Vector3.one;
        _fmodSoundSettings.Initialize();
        _menu.Initialize();
        
        await UniTask.Yield();
    }

    public Menu GetMenu()
    {
        return _menu;
    }
    
    public void ShowBlackScreen()
    {
        BlackScreen.DOScale(1f, 0.1f);
    }

    public void HideBlackScreen()
    {
        BlackScreen.DOScale(0f, 0.15f);
    }
}
