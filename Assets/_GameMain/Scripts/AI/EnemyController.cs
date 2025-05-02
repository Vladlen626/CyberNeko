using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform[] enemyPositions;
    private GameObject enemyPrefab;
    private Dictionary<GameObject, PatrolAndChaseAI> enemyScriptDict = new Dictionary<GameObject, PatrolAndChaseAI>();
    private Dictionary<GameObject, Transform> enemyPosDict = new Dictionary<GameObject, Transform>();

    public async UniTask Initialize()
    {
        foreach (var enemyPosition in enemyPositions)
        {
            var enemy = Instantiate(enemyPrefab, transform);
            enemy.SetActive(false);
            var enemyScript = enemy.GetComponent<PatrolAndChaseAI>();
            
            Assert.IsNotNull(enemyScript, $"{enemy.name} need PatrolAndChaseAI");
            
            enemyScriptDict.Add(enemy, enemyScript);
            enemyPosDict.Add(enemy, enemyPosition);
            enemyScript.Initialize(enemyPosition.GetComponentsInChildren<Transform>());
            await UniTask.Yield();
        }
    }

    public async UniTask SpawnEnemies()
    {
        foreach (var (enemy, posTransform) in enemyPosDict)
        {
            SpawnEnemy(enemy, posTransform);
        }
        
        await UniTask.Yield();
    }

    private void SpawnEnemy(GameObject enemy, Transform spawnPos)
    {
        enemy.transform.position = spawnPos.position;
        enemy.transform.rotation = spawnPos.rotation;
        enemy.SetActive(true);
    }

}
