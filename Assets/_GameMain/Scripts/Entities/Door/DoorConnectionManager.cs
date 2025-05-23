using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Zenject;

public class DoorConnectionManager : MonoBehaviour
{
    [SerializeField] private List<DoorsConnection> _doorsByLvls = new List<DoorsConnection>();

    [Inject]
    private void Construct(PointsManager pointsManager)
    {
        foreach (var doorsByLvl in _doorsByLvls)
        {
            doorsByLvl.Init(pointsManager);
        }
    }

    void OnDestroy()
    {
        foreach (var doorsByLvl in _doorsByLvls)
        {
            doorsByLvl.Term();
        }
    }
}
