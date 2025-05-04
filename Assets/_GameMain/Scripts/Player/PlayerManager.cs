using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
        InitializePlayer();
        SetupCamera();
        InitializeCheckpoints();
        
        await UniTask.Yield();
    }

    public void SetupCamera()
    {
        playerCamera.Target = new CameraTarget
        {
            TrackingTarget = _playerController.transform
        };
        
        _playerController.SetupCamera(playerCamera.transform);
    }

    public void SpawnPlayer()
    {
        foreach (var playerCheckpoint in _playerCheckpoints)
        {
            if (playerCheckpoint.isActive)
            {
                _playerController.Respawn();
                _playerController.transform.position = playerCheckpoint.GetSpawnPosition();
            }
        }
    }
    public PlayerController GetPlayerController()
    {
        return _playerController;
    }

    public void ShakePlayerCamera()
    {
        var cameraRig = playerCamera.transform.parent;
        playerCamera.Target.TrackingTarget = null;
        var originalPos = cameraRig.position;
        cameraRig.DOShakePosition(0.5f, 1f, 30, 180f)
            .OnComplete(() =>
            {
                cameraRig.position = originalPos;
            });
    }

    // _____________ Private _____________

    private void InitializePlayer()
    {
        var player = Instantiate(playerPrefab);
        _playerController = player.GetComponent<PlayerController>();
        _playerController.Initialize();
    }

    private void InitializeCheckpoints()
    {
        _playerCheckpoints = FindObjectsByType<PlayerCheckpoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var playerCheckpoint in _playerCheckpoints)
        {
            playerCheckpoint.OnActivate += HandleOnCheckpointActivate;
        }
    }
    
    private void HandleOnCheckpointActivate(PlayerCheckpoint activatedCheckpoint)
    {
        foreach (var playerCheckpoint in _playerCheckpoints)
        {
            playerCheckpoint.DeactivateCheckpoint();
        }
        
        activatedCheckpoint.ActivateCheckpoint();
    }

    private void OnDestroy()
    {
        foreach (var playerCheckpoint in _playerCheckpoints)
        {
            playerCheckpoint.OnActivate -= HandleOnCheckpointActivate;
        }
    }

}
