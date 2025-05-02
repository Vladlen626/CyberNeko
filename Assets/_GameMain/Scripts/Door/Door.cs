using System;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Action OnCanBeOpened;

    private PointsManager _pointsManager;

    // Call from Door Connection Manager
    public void Open()
    {
        Debug.Log("Door opened");
        //dotween
    }

    // When game Reset
    public void Close()
    {
        Debug.Log("Door closed");
        //dotween
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_pointsManager.IsKeyActive())
            {
                Debug.Log("Player try open door with key");
                OnCanBeOpened?.Invoke();
                _pointsManager.ResetPoints(); // To call it once
            }
        }
    }

    void Start()
    {
        _pointsManager = FindFirstObjectByType<PointsManager>();
    }
}
