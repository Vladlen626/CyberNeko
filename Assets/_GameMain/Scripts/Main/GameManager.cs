using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private FoodManager foodManager;
    [SerializeField] private PointsManager pointsManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private DoorConnectionManager doorConnectionManager;

    [SerializeField] private MoonController _moonController;

    private static GameManager _instance;
    private bool _gameActive;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
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

    //single point of entry
    private async UniTask InitializeAll()
    {
        await uiManager.Initialize();
        uiManager.GetMenu().OnRestart += HandlerOnRestart;
        
        await pointsManager.Initialize();
        await foodManager.Initialize(pointsManager);
        await doorConnectionManager.Initialize(pointsManager);
        
        await enemyController.Initialize();
        
        await playerManager.Initialize();
        playerManager.GetPlayerController().OnGrabbed += HandlerOnGameOver;
    }
    
    private async UniTask GameStart()
    {     
        foodManager.RespawnFood();
        playerManager.SpawnPlayer();
        _moonController.EnableSad();
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

    public void WinLogic()
    {
        _moonController.EnableHappy();
    }

    private async UniTask Final()
    {
        _moonController.EnableHappy();
        await UniTask.WaitForSeconds(2f, true);
        uiManager.Win();
    }

    private void QuitGame()
    {
        Application.Quit();
    }


}