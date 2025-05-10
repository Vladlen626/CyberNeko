using System;
using UnityEngine;

public class TestZoneFood : MonoBehaviour
{
    [SerializeField] private FoodSpawner _foodSpawner;

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
