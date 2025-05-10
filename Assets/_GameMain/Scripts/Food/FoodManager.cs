using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(FoodSpawner))]
public class FoodManager : MonoBehaviour
{
    private FoodSpawner _foodSpawner;
    private FoodDropper[] _foodDroppers;

    public void Initialize()
    {
        _foodSpawner = GetComponent<FoodSpawner>();
        _foodDroppers = FindObjectsByType<FoodDropper>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    }

    public void RespawnFood()
    {
        foreach (var foodDropper in _foodDroppers)
        {
            foodDropper.CleanUp();
        }
        
        _foodSpawner.ResetAllFood();
    }
    
}
