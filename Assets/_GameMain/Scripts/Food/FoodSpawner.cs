using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    //now not needed
    //[SerializeField] private List<GameObject> FoodPrefabs;
    
    private PointsManager _pointsManager;
    private Food[] _sceneFood;

    public void ResetAllFood()
    {
        foreach (var food in _sceneFood)
        {
            food.Spawn();
        }
    }
    
    public void Initialize(PointsManager pointsManager)
    {
        _pointsManager = pointsManager;
        _sceneFood = FindObjectsByType<Food>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        foreach (var food in _sceneFood)
        {
            food.Initialize();
            food.OnDevoured += HandleOnDevoured;
        }
    }

    private void HandleOnDevoured(int points)
    {
        _pointsManager?.AddPoints(points);
    }
}
