using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;using UnityEngine;


public class EnemyController : MonoBehaviour
{
    [SerializeField] private bool _isSpawnEnemies;
    [SerializeField] private Transform[] _enemyPositions;
    [SerializeField] private GameObject _enemyPrefab;
    
    private readonly Dictionary<AIVisionSensor, Transform> _enemyVision = new Dictionary<AIVisionSensor, Transform>();

    public void Initialize()
    {
        foreach (var enemyPosition in _enemyPositions)
        {
            var enemy = Instantiate(_enemyPrefab, enemyPosition.position, quaternion.identity);
            var patrolAction = enemy.GetComponent<PatrolAction>();
            var visionSensor = enemy.GetComponent<AIVisionSensor>();
            visionSensor.Initialize();
            
            patrolAction.SetPatrolPoints(enemyPosition.GetComponentsInChildren<Transform>());
            _enemyVision.Add(visionSensor, enemyPosition);
            enemy.SetActive(false);
        }
    }

    public async UniTask SpawnEnemies()
    {
        if (!_isSpawnEnemies) return;
        foreach (var (enemy, posTransform) in _enemyVision)
        {
            SpawnEnemy(enemy, posTransform);
        }
        
        await UniTask.Yield();
    }

    // _____________ Private _____________

    private void SpawnEnemy(AIVisionSensor enemy, Transform spawnPos)
    {
        enemy.transform.position = spawnPos.position + Vector3.up;
        enemy.transform.rotation = spawnPos.rotation;
        enemy.gameObject.SetActive(true);
        enemy.Reset();
    }

}
