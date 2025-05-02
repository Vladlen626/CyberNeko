using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class SpawnableFoodSettings
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _count = 1;

    public GameObject Prefab
    {
        get => _prefab;
    }

    public int Count
    {
        get => _count;
    }
}

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private List<SpawnableFoodSettings> _foodsSettings = new List<SpawnableFoodSettings>();
    [SerializeField] private float _overallSpawnRadius = 5f;

    private Dictionary<GameObject, IDevourable> _spawnedFoods = new Dictionary<GameObject, IDevourable>();

    public void Init()
    {
        foreach (var foodSettings in _foodsSettings)
        {
            for (var i = 0; i < foodSettings.Count; i++)
            {
                var food = CreateFood(foodSettings.Prefab);
                var devourableSctript = food.GetComponent<IDevourable>();

                Assert.IsNotNull(devourableSctript, $"{food.name} need IDevourable");

                devourableSctript.Hide();

                devourableSctript.OnDevoured += () => HandleOnDevoured(food, devourableSctript);
                _spawnedFoods.Add(food, devourableSctript);
                Debug.Log("Spawn food");
            }
        }
    }

    public void TrySpawn()
    {
        foreach (var food in _spawnedFoods)
        {
            if (!food.Value.IsActive())
                food.Value.Show();
        }
    }

    private void OnDestroy()
    {
        foreach (var food in _spawnedFoods)
        {
            food.Value.OnDevoured -= () => HandleOnDevoured(food.Key, food.Value);
        }
    }

    private Vector3 GetSpawnPos()
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized *
                               Random.Range(0, _overallSpawnRadius);

        Vector3 spawnPosition = transform.position + new Vector3(randomCircle.x, transform.position.y, randomCircle.y);
        return spawnPosition;
    }

    private GameObject CreateFood(GameObject prefab)
    {
        GameObject spawnedObject = Instantiate(prefab, GetSpawnPos(), Quaternion.identity);
        spawnedObject.transform.parent = transform;
        return spawnedObject;
    }

    private void HandleOnDevoured(GameObject food, IDevourable devourable)
    {
        devourable.Hide();
        food.transform.position = GetSpawnPos();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _overallSpawnRadius);
    }
}
