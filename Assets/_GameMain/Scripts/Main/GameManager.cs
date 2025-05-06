using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public event Action OnGameReady;
    
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private FoodManager foodManager;
    [SerializeField] private PointsManager pointsManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private DoorConnectionManager doorConnectionManager;

    private static GameManager _instance;
    private bool _gameActive;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            StartGame().Forget();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async UniTaskVoid StartGame()
    {
        await InitializeAll();
        await GameStart();
        _gameActive = true;
        while (_gameActive)
        {
            await GameUpdate();
        }

        QuitGame();
    }

    private void HandlerOnGameOver()
    {
        GameOverAsync().Forget();
    }

    private void HandlerOnRestart()
    {
        Respawn().Forget();
    }

    private async UniTask InitializeAll()
    {
        uiManager.Initialize();
        uiManager.GetMenu().OnRestart += HandlerOnRestart;
        
        pointsManager.Initialize();
        foodManager.Initialize(pointsManager);
        doorConnectionManager.Initialize(pointsManager);

        enemyController.Initialize();

        playerManager.Initialize();
        playerManager.GetPlayerController().OnGrabbed += HandlerOnGameOver;
        
        await UniTask.Yield();
        OnGameReady?.Invoke();
    }

    private async UniTask GameStart()
    {
        foodManager.RespawnFood();
        playerManager.SpawnPlayer();
        await enemyController.SpawnEnemies();
        await UniTask.WaitForSeconds(1f, true);
        uiManager.HideBlackScreen();
        await UniTask.WaitForSeconds(0.15f, true);
    }

    private async UniTask GameOverAsync()
    {
        playerManager.ShakePlayerCamera();
        await UniTask.WaitForSeconds(1f, true);
        AudioManager.inst.PlaySound(SoundNames.GameOver);
        uiManager.GetMenu().GameOver(pointsManager.GetCurrentPoints());
    }

    private async UniTask Respawn()
    {
        uiManager.ShowBlackScreen();
        pointsManager.ResetAllPoints();
        await UniTask.WaitForSeconds(0.5f, true);
        playerManager.SetupCamera();
        await GameStart();
    }

    //main thread 
    private async UniTask GameUpdate()
    {
        await UniTask.Yield(PlayerLoopTiming.Update);
    }

    private async UniTask Final()
    {
        await UniTask.WaitForSeconds(2f, true);
        AudioManager.inst.PlaySound(SoundNames.Win);
        uiManager.Win();
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}