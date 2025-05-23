using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;using UnityEngine;


public class EnemyController : MonoBehaviour
{
    [SerializeField] private bool _isSpawnEnemies;
    [SerializeField] private Transform[] _enemyPositions;
    [SerializeField] private GameObject _enemyPrefab;
    
    private readonly Dictionary<GameObject, PatrolAction> _enemyPatrolPoints = new Dictionary<GameObject, PatrolAction>();
    private readonly Dictionary<GameObject, AIVisionSensor> _enemyVision = new Dictionary<GameObject, AIVisionSensor>();
    private readonly Dictionary<GameObject, Transform> _enemyPosDict = new Dictionary<GameObject, Transform>();

    public void Initialize()
    {
        foreach (var enemyPosition in _enemyPositions)
        {
            var enemy = Instantiate(_enemyPrefab, enemyPosition.position, quaternion.identity);
            var patrolAction = enemy.GetComponent<PatrolAction>();
            var visionSensor = enemy.GetComponent<AIVisionSensor>();
            visionSensor.Initialize();
            
            _enemyPatrolPoints.Add(enemy, patrolAction);
            _enemyVision.Add(enemy, visionSensor);
            _enemyPosDict.Add(enemy, enemyPosition);
            enemy.SetActive(false);
        }
    }

    public async UniTask SpawnEnemies()
    {
        if (!_isSpawnEnemies) return;
        foreach (var (enemy, posTransform) in _enemyPosDict)
        {
            SpawnEnemy(enemy, posTransform);
        }
        
        await UniTask.Yield();
    }

    private void SpawnEnemy(GameObject enemy, Transform spawnPos)
    {
        enemy.transform.position = spawnPos.position + Vector3.up;
        enemy.transform.rotation = spawnPos.rotation;
        enemy.SetActive(true);
        _enemyPatrolPoints[enemy].SetPatrolPoints(spawnPos.GetComponentsInChildren<Transform>());
        _enemyVision[enemy].Reset();
    }

}
