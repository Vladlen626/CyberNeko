using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private FoodManager foodSpawner;
    [SerializeField] private PointsManager pointsManager;

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
        await foodSpawner.Initialize();
        await pointsManager.Initialize();
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