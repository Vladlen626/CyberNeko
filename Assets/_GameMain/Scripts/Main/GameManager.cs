using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private EnemyController _enemyController;
    
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
       await _enemyController.Initialize();
    }

    //main thread 
    async UniTask GameUpdate()
    {
        await UniTask.Yield(PlayerLoopTiming.Update);
        await _enemyController.SpawnEnemies();
    }

    void QuitGame()
    {
        Application.Quit();
    }
}


