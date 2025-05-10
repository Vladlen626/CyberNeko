using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TestZoneDoor : MonoBehaviour
{
    [SerializeField] private Door testDoor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ResetDoor();
        }
    }

    private void ResetDoor()
    {
        testDoor.Reset();
    }
}