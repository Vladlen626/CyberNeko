using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;


public class EnemyController : MonoBehaviour
{
    [SerializeField] private bool _isSpawnEnemies;
    [SerializeField] private GameObject _enemyPrefab;

    private readonly List<EnemySpawnPoint> _spawnPoints = new List<EnemySpawnPoint>();
    private readonly Dictionary<AIVisionSensor, EnemySpawnPoint> _enemyVision = new();

    public void RegisterSpawnPoint(EnemySpawnPoint point)
    {
        if (!_spawnPoints.Contains(point))
            _spawnPoints.Add(point);
    }

    public void UnregisterSpawnPoint(EnemySpawnPoint point)
    {
        _spawnPoints.Remove(point);
    }

    public void Initialize()
    {
        // Перед новым стартом очищаем старое
        foreach (var enemy in _enemyVision.Keys)
        {
            if (enemy != null)
                Destroy(enemy.gameObject);
        }
        _enemyVision.Clear();

        foreach (var spawnPoint in _spawnPoints)
        {
            var enemy = Instantiate(_enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
            var patrolAction = enemy.GetComponent<PatrolAction>();
            var visionSensor = enemy.GetComponent<AIVisionSensor>();
            visionSensor.Initialize();

            // Если нужно, ищи patrol-пойнты среди детей spawnPoint
            patrolAction.SetPatrolPoints(spawnPoint.GetComponentsInChildren<Transform>());
            _enemyVision.Add(visionSensor, spawnPoint);
            enemy.SetActive(false);
        }
    }

    public async UniTask SpawnEnemies()
    {
        if (!_isSpawnEnemies) return;
        foreach (var (sensor, point) in _enemyVision)
        {
            SpawnEnemy(sensor, point.transform);
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
