using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BreakablesManager : IInitializable
{
    private readonly List<BreakableObject> _staticBreakables = new();
    private readonly List<BreakableObject> _dynamicBreakables = new();

    public void Initialize()
    {
        _staticBreakables.Clear();
        _staticBreakables.AddRange(Object.FindObjectsByType<BreakableObject>(FindObjectsInactive.Include, FindObjectsSortMode.None));
        _dynamicBreakables.Clear();
    }
    
    public void Register(BreakableObject breakable)
    {
        if(!_dynamicBreakables.Contains(breakable))
            _dynamicBreakables.Add(breakable);
    }
    
    public void ResetAllBreakables()
    {
        foreach (var b in _staticBreakables)
        {
            if (b != null)
                b.ResetState();
        }
        foreach (var b in _dynamicBreakables)
        {
            if (b != null)
                Object.Destroy(b.gameObject);
        }
        _dynamicBreakables.Clear();
    }
}