using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private bool isSpawnEnemies;
    [SerializeField] private Transform[] enemyPositions;
    [SerializeField] private GameObject enemyPrefab;
    private Dictionary<GameObject, PatrolAndChaseAI> enemyScriptDict = new Dictionary<GameObject, PatrolAndChaseAI>();
    private Dictionary<GameObject, Transform> enemyPosDict = new Dictionary<GameObject, Transform>();

    public async UniTask Initialize()
    {
        foreach (var enemyPosition in enemyPositions)
        {
            var enemy = Instantiate(enemyPrefab, enemyPosition.position, quaternion.identity);
            var enemyScript = enemy.GetComponent<PatrolAndChaseAI>();
            
            //Assert.IsNotNull(enemyScript, $"{enemy.name} need PatrolAndChaseAI");
            
            enemyScriptDict.Add(enemy, enemyScript);
            enemyPosDict.Add(enemy, enemyPosition);
            enemy.SetActive(false);
            await UniTask.Yield();
        }
    }

    public async UniTask SpawnEnemies()
    {
        if (!isSpawnEnemies) return;
        foreach (var (enemy, posTransform) in enemyPosDict)
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
        var enemyScript = enemyScriptDict[enemy];
        enemyScript.Initialize(spawnPos.GetComponentsInChildren<Transform>());
        enemyScript.StartPlayerDetection();
    }

}
