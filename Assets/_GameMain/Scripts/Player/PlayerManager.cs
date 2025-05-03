using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private GameObject playerPrefab;

    private List<PlayerCheckpoint> _playerCheckpoints;
    
    public async UniTask Initialize()
    {
        var player = Instantiate(playerPrefab);
        var playerScript = player.GetComponent<PlayerController>();
        playerScript.Initialize();
        await UniTask.Yield();
    }

    public void SpawnPlayer()
    {
        
    }

}
