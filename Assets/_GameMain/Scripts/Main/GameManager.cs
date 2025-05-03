using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public Action OnGameStarted;

    [SerializeField] private EnemyController enemyController;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private FoodManager foodManager;
    [SerializeField] private PointsManager pointsManager;
    [SerializeField] private DoorConnectionManager doorConnectionManager;

    private static GameManager _instance;
    private bool _gameActive;

    void Awake()
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

    async UniTaskVoid StartGame()
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
    async UniTask InitializeAll()
    {
        await foodManager.Initialize();
        Debug.Log(foodManager + " initialize complete");
        await pointsManager.Initialize();
        Debug.Log(pointsManager + " initialize complete");
        await doorConnectionManager.Initialize(pointsManager);
        Debug.Log(doorConnectionManager + " initialize complete");
        await enemyController.Initialize();
        Debug.Log(enemyController + " initialize complete");
        await playerManager.Initialize();
        Debug.Log(playerManager + " initialize complete");
    }
    
    async UniTask GameStart()
    {     
        await enemyController.SpawnEnemies();
        OnGameStarted?.Invoke();
    }
    
    
    //main thread 
    async UniTask GameUpdate()
    {
        await UniTask.Yield(PlayerLoopTiming.Update);
        
    }

    void QuitGame()
    {
        Application.Quit();
    }
}