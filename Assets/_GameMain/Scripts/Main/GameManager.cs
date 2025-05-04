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

    //single point of entry
    private async UniTask InitializeAll()
    {
        await uiManager.Initialize();
        await pointsManager.Initialize();
        await foodManager.Initialize(pointsManager);
        await doorConnectionManager.Initialize(pointsManager);
        
        await enemyController.Initialize();
        
        await playerManager.Initialize();
        playerManager.GetPlayerController().OnGrabbed += GameOver;
    }
    
    private async UniTask GameStart()
    {     
        foodManager.RespawnFood();
        await enemyController.SpawnEnemies();
        playerManager.SpawnPlayer();
        await UniTask.WaitForSeconds(1f, true);
        uiManager.HideBlackScreen();
        await UniTask.WaitForSeconds(0.15f, true);
    }

    private async UniTask GameEnd()
    {
        await UniTask.WaitForSeconds(0.25f, true);
        uiManager.ShowBlackScreen();
        pointsManager.ResetPoints();
        await UniTask.WaitForSeconds(0.15f, true);
        await GameStart();
    }
    
    
    //main thread 
    private async UniTask GameUpdate()
    {
        await UniTask.Yield(PlayerLoopTiming.Update);
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    private void GameOver()
    {
        GameEnd().Forget();
    }
}