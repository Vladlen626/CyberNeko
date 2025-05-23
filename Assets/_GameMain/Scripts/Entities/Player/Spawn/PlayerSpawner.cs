using System;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private CinemachineCamera playerCamera;

    private PlayerCheckpoint[] _playerCheckpoints;
    private PlayerController _playerController;

    [Inject] private PlayerController.Factory _playerFactory;

    public void Initialize()
    {
        InitializeCheckpoints();
    }

    public void RespawnPlayer()
    {
        if (_playerController != null)
        {
            Destroy(_playerController.gameObject);
            _playerController = null;
        }
        
        var spawnPos = Vector3.zero;
        foreach (var checkpoint in _playerCheckpoints)
        {
            if (checkpoint.IsActive())
            {
                spawnPos = checkpoint.GetSpawnPosition();
                break;
            }
        }

        _playerController = _playerFactory.Create();
        _playerController.Initialize(playerCamera.transform, spawnPos);

        SetupCamera();
    }

    public PlayerController GetPlayerController()
    {
        return _playerController;
    }

    // _____________ Private _____________

    private void InitializeCheckpoints()
    {
        _playerCheckpoints = FindObjectsByType<PlayerCheckpoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var checkpoint in _playerCheckpoints)
        {
            checkpoint.OnActivate += HandleOnCheckpointActivate;
            checkpoint.Initialize();
        }
    }

    private void SetupCamera()
    {
        playerCamera.Target = new CameraTarget
        {
            TrackingTarget = _playerController.transform
        };
    }

    private void HandleOnCheckpointActivate(PlayerCheckpoint activatedCheckpoint)
    {
        foreach (var checkpoint in _playerCheckpoints)
        {
            if (checkpoint == activatedCheckpoint) continue;
            checkpoint.DeactivateCheckpoint();
        }
    }

    private void OnDestroy()
    {
        if (_playerCheckpoints == null) return;
        foreach (var checkpoint in _playerCheckpoints)
        {
            checkpoint.OnActivate -= HandleOnCheckpointActivate;
        }
    }
}
