using UnityEngine;
using Zenject;

public class NPCSpawnPoint : MonoBehaviour
{
    private NpcController _npcController;

    [Inject]
    public void Construct(NpcController npcController)
    {
        _npcController = npcController;
        _npcController.RegisterSpawnPoint(this);
    }

    private void OnDestroy()
    {
        _npcController?.UnregisterSpawnPoint(this);
    }
}