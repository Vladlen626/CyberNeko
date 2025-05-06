using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private bool _isSpawnEnemies;
    [SerializeField] private Transform[] _enemyPositions;
    [SerializeField] private GameObject _enemyPrefab;
    
    private Dictionary<GameObject, PatrolAndChaseAI> _enemyScriptDict = new Dictionary<GameObject, PatrolAndChaseAI>();
    private Dictionary<GameObject, Transform> _enemyPosDict = new Dictionary<GameObject, Transform>();

    public void Initialize()
    {
        foreach (var enemyPosition in _enemyPositions)
        {
            var enemy = Instantiate(_enemyPrefab, enemyPosition.position, quaternion.identity);
            var enemyScript = enemy.GetComponent<PatrolAndChaseAI>();
            
            //Assert.IsNotNull(enemyScript, $"{enemy.name} need PatrolAndChaseAI");
            
            _enemyScriptDict.Add(enemy, enemyScript);
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
        var enemyScript = _enemyScriptDict[enemy];
        enemyScript.Initialize(spawnPos.GetComponentsInChildren<Transform>());
        enemyScript.StartPlayerDetection();
    }

}
