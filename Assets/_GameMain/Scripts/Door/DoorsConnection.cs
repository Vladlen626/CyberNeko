using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DoorsConnection
{
    [SerializeField] private List<GameObject> _doorsGameObj = new List<GameObject>();

    private List<Door> _doors = new List<Door>();

    public void Init(PointsManager pointsManager)
    {
        foreach (var doorObj in _doorsGameObj)
        {
            var door = doorObj.GetComponent<Door>();
            door.Initialize(pointsManager);
            _doors.Add(door);
            door.OnOpened += OpenAllConnectedDoors;
        }
    }

    public void Term()
    {
        foreach (var door in _doors)
        {
            door.OnOpened -= OpenAllConnectedDoors;
        }
    }

    private void OpenAllConnectedDoors()
    {
        foreach (var door in _doors)
        {
            door.Open();
        }
    }
}