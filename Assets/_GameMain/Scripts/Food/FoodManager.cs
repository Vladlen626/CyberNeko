using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : MonoBehaviour
{
    private readonly HashSet<FoodSpawner> _spawners = new();

    public async UniTask Initialize()
    {
        FoodSpawner[] spawners = FindObjectsByType<FoodSpawner>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );
        foreach (var spawner in spawners)
        {
            RegisterSpawner(spawner);
        }

        //await UniTask.Yield();
    }

    public async UniTask SpawnFood()
    {
        foreach (FoodSpawner spawner in _spawners)
        {
            spawner.TrySpawn();
        }

        await UniTask.Yield();
    }

    private void RegisterSpawner(FoodSpawner spawner)
    {
        _spawners.Add(spawner);
        spawner.Init();
    }

    private void UnregisterSpawner(FoodSpawner spawner)
    {
        _spawners.Remove(spawner);
    }
}
