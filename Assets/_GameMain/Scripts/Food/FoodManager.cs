using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FoodSpawner))]
public class FoodManager : MonoBehaviour
{
    private FoodSpawner _foodSpawner;
    private FoodDropper[] _foodDroppers;
    
    public async UniTask Initialize(PointsManager pointsManager)
    {
        _foodSpawner = GetComponent<FoodSpawner>();
        _foodSpawner.Initialize(pointsManager);

        _foodDroppers = FindObjectsByType<FoodDropper>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        await UniTask.Yield();
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
