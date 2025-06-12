using System;
using UnityEngine;
using Zenject;

public class TestZoneFood : MonoBehaviour
{
    private FoodSpawner _foodSpawner;

    [Inject]
    private void Construct(FoodSpawner foodSpawner)
    {
        _foodSpawner = foodSpawner;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ResetFood();
        }
    }

    private void ResetFood()
    {
        _foodSpawner.ResetAllFood();
    }
}
