using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private CinemachineCamera playerCamera;
    private PlayerCheckpoint[] _playerCheckpoints;
    private PlayerController _playerController;

    public void Initialize()
    {
        InitializePlayer();
        InitializeCheckpoints();
        SetupCamera();
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
            if (playerCheckpoint.IsActive())
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
        foreach (var checkpoint in _playerCheckpoints)
        {
            checkpoint.OnActivate += HandleOnCheckpointActivate;
            checkpoint.Initialize();
        }
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
        foreach (var checkpoint in _playerCheckpoints)
        {
            checkpoint.OnActivate -= HandleOnCheckpointActivate;
        }
    }

}