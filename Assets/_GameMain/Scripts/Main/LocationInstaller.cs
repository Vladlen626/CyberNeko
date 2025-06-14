using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class LocationInstaller : MonoInstaller
{
    [SerializeField] private EnemyController enemyControllerPrefab;
    [SerializeField] private NpcController npcControllerPrefab;
    [SerializeField] private FoodSpawner foodSpawnerPrefab;
    [SerializeField] private ZoneController zoneControllerPrefab;
    [SerializeField] private UIManager uiManager;

    // ReSharper disable Unity.PerformanceAnalysis
    public override void InstallBindings()
    {
        BindMain();
        BindUIManager();
        BindPointsManager();
        
        BindEnemyController();
        BindFoodSpawner();
        BindNpcController();
        
        BindZoneController();
        BindBreakablesManager();
    }

    // _____________ Private _____________

    private void BindMain()
    {
        Container.BindInterfacesAndSelfTo<Main>().AsSingle().NonLazy();
    }

    private void BindEnemyController()
    {
        Container.Bind<EnemyController>()
            .FromComponentInNewPrefab(enemyControllerPrefab)
            .AsSingle()
            .NonLazy();
    }

    private void BindNpcController()
    {
        Container.Bind<NpcController>()
            .FromComponentInNewPrefab(npcControllerPrefab)
            .AsSingle()
            .NonLazy();
    }

    private void BindFoodSpawner()
    {
        Container.Bind<FoodSpawner>()
            .FromComponentInNewPrefab(foodSpawnerPrefab)
            .AsSingle()
            .NonLazy();
    }

    private void BindZoneController()
    {
        Container.Bind<ZoneController>()
            .FromComponentInNewPrefab(zoneControllerPrefab)
            .AsSingle()
            .NonLazy();
    }
    
    private void BindPointsManager()
    {
        Container.Bind<PointsManager>().AsSingle();
    }

    private void BindUIManager()
    {
        Container.BindInstance(uiManager).AsSingle();
    }
    
    private void BindBreakablesManager()
    {
        Container.BindInterfacesAndSelfTo<BreakablesManager>().AsSingle();
    }
}