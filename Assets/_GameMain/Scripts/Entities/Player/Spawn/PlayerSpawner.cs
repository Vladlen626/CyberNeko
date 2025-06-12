using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class PlayerSpawner : MonoBehaviour
{
    private PlayerCheckpoint[] _playerCheckpoints;
    private PlayerBundle _playerBundle;
    private PlayerBundle.Factory _bundleFactory;

    [Inject]
    private void Construct(PlayerBundle.Factory bundleFactory)
    {
        _bundleFactory = bundleFactory;
    }
    
    public void Initialize()
    {
        InitializeCheckpoints();
    }

    public void RespawnPlayer()
    {
        if (_playerBundle != null)
        {
            if (_playerBundle.Player != null)
                Destroy(_playerBundle.Player.gameObject);
            if (_playerBundle.Camera != null)
                Destroy(_playerBundle.Camera.gameObject);
            if (_playerBundle.PickupMarker != null)
                Destroy(_playerBundle.PickupMarker.gameObject);
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

        _playerBundle = _bundleFactory.Create();
        _playerBundle.Player.Initialize(_playerBundle.Camera.transform, spawnPos, _playerBundle.PickupMarker);

        SetupCamera(_playerBundle.Camera, _playerBundle.Player.transform);
    }

    public PlayerController GetPlayerController()
    {
        return _playerBundle?.Player;
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

    private void SetupCamera(CinemachineCamera playerCamera, Transform target)
    {
        playerCamera.Target = new CameraTarget { TrackingTarget = target };
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