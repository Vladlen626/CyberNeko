using UnityEngine;
using Zenject;

public class LocationInstaller : MonoInstaller
{
    [SerializeField] private PlayerSpawner _playerSpawner;
    [SerializeField] private EnemyController _enemyController;
    [SerializeField] private FoodManager _foodManager;
    [SerializeField] private PointsManager _pointsManager;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private DoorConnectionManager _doorConnectionManager;
    
    // ReSharper disable Unity.PerformanceAnalysis
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<Main>().AsSingle().NonLazy();
        Container.BindInstance(_playerSpawner).AsSingle();
        Container.BindInstance(_enemyController).AsSingle();
        Container.BindInstance(_foodManager).AsSingle();
        Container.BindInstance(_pointsManager).AsSingle();
        Container.BindInstance(_uiManager).AsSingle();
        Container.BindInstance(_doorConnectionManager).AsSingle();
    }
    
}

