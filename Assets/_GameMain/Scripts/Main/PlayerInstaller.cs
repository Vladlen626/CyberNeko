using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    [SerializeField] private PlayerSpawner playerSpawnerPrefab;
    [SerializeField] private PlayerController playerPrefab;
    [SerializeField] private CinemachineCamera cameraPrefab;
    
    private UIManager _uiManager;

    [Inject]
    private void Construct(UIManager uiManager)
    {
        _uiManager = uiManager;
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public override void InstallBindings()
    {
        BindPlayerBundle();
        BindInputService();
        BindPlayerSpawner();
    }

    private void BindPlayerBundle()
    {
        Container.BindFactory<PlayerBundle, PlayerBundle.Factory>()
            .FromMethod(_ => CreatePlayerBundle(Container));
    }
    
    private void BindPlayerSpawner()
    {
        var spawner = Container.InstantiatePrefabForComponent<PlayerSpawner>(playerSpawnerPrefab);
        Container.BindInstance(spawner).AsSingle().NonLazy();
    }
   
    private void BindInputService()
    {
        Container.Bind<IInputService>().To<StandaloneInputService>().AsSingle();
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private PlayerBundle CreatePlayerBundle(DiContainer container)
    {
        var player = container.InstantiatePrefabForComponent<PlayerController>(playerPrefab);
        var playerCamera = container.InstantiatePrefabForComponent<CinemachineCamera>(cameraPrefab);
        var pickupMarker = _uiManager.CreatePickupMarker();
        return new PlayerBundle(player, playerCamera, pickupMarker);
    }
}