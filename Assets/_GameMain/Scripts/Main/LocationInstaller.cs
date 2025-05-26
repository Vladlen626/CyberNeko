using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class LocationInstaller : MonoInstaller
{
    [SerializeField] private PlayerSpawner _playerSpawner;
    [SerializeField] private EnemyController _enemyController;
    [SerializeField] private NpcController _npcController;
    [SerializeField] private FoodSpawner _foodSpawner;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private DoorConnectionManager _doorConnectionManager;
    [SerializeField] private PlayerController _playerPrefab;

    // ReSharper disable Unity.PerformanceAnalysis
    public override void InstallBindings()
    {
        BindMain();
        BindPlayerSpawner();
        BindEnemyController();
        BindNpcController();
        BindFoodManager();
        BindPointsManager();
        BindUIManager();
        BindDoorConnectionManager();
        BindPlayerControllerFactory();
        BindInputService();
        BindBreakablesManager();
    }

    // _____________ Private _____________

    private void BindMain()
    {
        Container.BindInterfacesAndSelfTo<Main>().AsSingle().NonLazy();
    }

    private void BindPlayerSpawner()
    {
        Container.BindInstance(_playerSpawner).AsSingle();
    }

    private void BindEnemyController()
    {
        Container.BindInstance(_enemyController).AsSingle();
    }
    
    private void BindNpcController()
    {
        Container.BindInstance(_npcController).AsSingle();
    }


    private void BindFoodManager()
    {
        Container.BindInstance(_foodSpawner).AsSingle();
    }

    private void BindPointsManager()
    {
        Container.Bind<PointsManager>().AsSingle();
    }

    private void BindUIManager()
    {
        Container.BindInstance(_uiManager).AsSingle();
    }

    private void BindDoorConnectionManager()
    {
        Container.BindInstance(_doorConnectionManager).AsSingle();
    }

    private void BindPlayerControllerFactory()
    {
        Container.BindFactory<PlayerController, PlayerController.Factory>()
            .FromComponentInNewPrefab(_playerPrefab);
    }
    
    private void BindBreakablesManager()
    {
        Container.BindInterfacesAndSelfTo<BreakablesManager>().AsSingle();
    }

    private void BindInputService()
    {
        Container.Bind<IInputService>().To<StandaloneInputService>().AsSingle();
    }
}

