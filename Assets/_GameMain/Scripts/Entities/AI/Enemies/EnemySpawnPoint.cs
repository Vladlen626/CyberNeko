using UnityEngine;
using Zenject;

public class EnemySpawnPoint : MonoBehaviour
{
    private EnemyController _enemyController;

    [Inject]
    public void Construct(EnemyController enemyController)
    {
        _enemyController = enemyController;
        _enemyController.RegisterSpawnPoint(this);
    }

    private void OnDestroy()
    {
        _enemyController?.UnregisterSpawnPoint(this);
    }
}