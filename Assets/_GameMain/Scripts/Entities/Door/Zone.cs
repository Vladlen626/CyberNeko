using System;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public event Action OnOpened;

    [SerializeField] private int zoneOrder;
    public int ZoneOrder => zoneOrder;

    private Door[] _doors;
    private bool _isOpen;

    private int _goalPoints;

    public int GoalPoints => _goalPoints;

    public void Initialize()
    {
        _doors = GetComponentsInChildren<Door>(true);

        foreach (var door in _doors)
        {
            door.Initialize();
            door.OnOpened += OpenAllConnectedDoors;
        }

        var foods = GetComponentsInChildren<Food>(true);
        foreach (var food in foods)
        {
            food.MultiplyPoints(zoneOrder);
            _goalPoints += food.Points;
        }

        if (_isOpen) OpenAllConnectedDoors();
    }

    public void Terminate()
    {
        foreach (var door in _doors)
        {
            door.OnOpened -= OpenAllConnectedDoors;
        }
    }

    private void OpenAllConnectedDoors()
    {
        OnOpened?.Invoke();
        _isOpen = true;
        foreach (var door in _doors)
        {
            door.Open();
        }
    }
}