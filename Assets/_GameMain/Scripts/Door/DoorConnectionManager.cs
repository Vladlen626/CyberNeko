using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using NUnit.Framework;
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
            //Assert.IsNotNull(door, "Door must have Door script!");
            door.Initialize(pointsManager);
            _doors.Add(door);
            door.OnCanBeOpened += OpenAllConnectedDoors;
        }
    }

    public void Term()
    {
        foreach (var door in _doors)
        {
            door.OnCanBeOpened -= OpenAllConnectedDoors;
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

public class DoorConnectionManager : MonoBehaviour
{
    [SerializeField] private List<DoorsConnection> _doorsByLvls = new List<DoorsConnection>();

    public async UniTask Initialize(PointsManager pointsManager)
    {
        foreach (var doorsByLvl in _doorsByLvls)
        {
            doorsByLvl.Init(pointsManager);
        }

        await UniTask.Yield();
    }

    void OnDestroy()
    {
        foreach (var doorsByLvl in _doorsByLvls)
        {
            doorsByLvl.Term();
        }
    }
}
