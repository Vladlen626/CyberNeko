using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
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
        await pointsManager.Initialize();
        await doorConnectionManager.Initialize();
        await enemyController.Initialize();
        await playerManager.Initialize();
    }

    //main thread 
    async UniTask GameUpdate()
    {
        await UniTask.Yield(PlayerLoopTiming.Update);
        await enemyController.SpawnEnemies();

    }

    void QuitGame()
    {
        Application.Quit();
    }
}