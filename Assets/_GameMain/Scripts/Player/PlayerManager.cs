using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private PlayerController player;

    public async UniTask Initialize()
    {
        player.Initialize();
        await UniTask.Yield();
    }

}
