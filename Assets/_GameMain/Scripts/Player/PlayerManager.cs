using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private GameObject playerPrefab;

    private PlayerCheckpoint[] _playerCheckpoints;
    private PlayerController _playerController;
    
    public async UniTask Initialize()
    {
        var player = Instantiate(playerPrefab);
        _playerController = player.GetComponent<PlayerController>();
        _playerController.Initialize();

        playerCamera.Target = new CameraTarget
        {
            TrackingTarget = player.transform
        };
        
        _playerController.SetupCamera(playerCamera.transform);
        _playerCheckpoints = FindObjectsByType<PlayerCheckpoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        await UniTask.Yield();
    }

    public void SpawnPlayer()
    {
        foreach (var playerCheckpoint in _playerCheckpoints)
        {
            if (playerCheckpoint.isActive)
            {
                _playerController.transform.position = playerCheckpoint.GetSpawnPosition();
            }
        }
    }

}
