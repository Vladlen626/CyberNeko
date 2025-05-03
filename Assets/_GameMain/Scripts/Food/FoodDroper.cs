using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Random = System.Random;

public class FoodDroper : MonoBehaviour
{
    [SerializeField] private GameObject[] _foodsPrefabs;
    [SerializeField] private int _foodDropCount = 2;
    [SerializeField] private float _countdown = 5f;

    [SerializeField] private float maxOffsetX = 2f;
    [SerializeField] private float maxOffsetY = 0f;
    [SerializeField] private float maxOffsetZ = 2f;

    private float _lastDropTime = -Mathf.Infinity;
    private Dictionary<GameObject, IDevourable> _spawnedFoods = new Dictionary<GameObject, IDevourable>();

    private GameManager _gameManager;

    private float _timeBeforeDestroy = 2f;

    void Awake()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
        _gameManager.OnGameStarted += HandleOnGameStarted;
    }

    public void TryDropFood()
    {
        if (_foodsPrefabs.Length == 0 )
        {
            Debug.LogWarning("Список еды НПС пуст!");
            return;
        }

        if (Time.time - _lastDropTime < _countdown)
        {
            Debug.Log("Спавн еды НПС на кулдауне, скип");
            return;
        }

        _lastDropTime = Time.time;
        for (int i = 0; i < _foodDropCount; i++)
        {
            Debug.Log("Spawn food by droper");
            int randomFoodInx = UnityEngine.Random.Range(0, _foodDropCount - 1);
            GameObject randomFoodPrefab = _foodsPrefabs[randomFoodInx];

            Vector3 randomOffset = new Vector3(
                UnityEngine.Random.Range(0f, maxOffsetX), // X
                UnityEngine.Random.Range(0f, maxOffsetY), // Y 
                UnityEngine.Random.Range(0f, maxOffsetZ)  // Z
            );

            GameObject food = Instantiate(randomFoodPrefab, transform.position + randomOffset, Quaternion.identity);
            IDevourable script = food.GetComponent<IDevourable>();

            Assert.IsNotNull(script, $"{food.name} need IDevourable");

            script.OnDevoured += () => DestroyFood(food, script);

            _spawnedFoods.Add(food, script);
        }
    }

    // TODO: Can be optimized later
    void DestroyFood(GameObject food, IDevourable script)
    {
        script.OnDevoured -= () => DestroyFood(food, script);
        _spawnedFoods.Remove(food);
        Destroy(food, _timeBeforeDestroy);
    }

    private void HandleOnGameStarted()
    {
        foreach (var food in _spawnedFoods)
        {
            DestroyFood(food.Key, food.Value);
        }
    }

    private void OnDestroy()
    {
        _gameManager.OnGameStarted -= HandleOnGameStarted;
        foreach (var food in _spawnedFoods)
        {
            food.Value.OnDevoured -= () => DestroyFood(food.Key, food.Value);
        }
    }
}
