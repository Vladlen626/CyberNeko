using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BreakablesManager : IInitializable
{
    private readonly List<IBreakable> _staticBreakables = new();
    private readonly List<IBreakable> _dynamicBreakables = new();

    public void Initialize()
    {
        _staticBreakables.Clear();
        _staticBreakables.AddRange(Object.FindObjectsByType<FoodContainer>(FindObjectsInactive.Include, FindObjectsSortMode.None));
        _dynamicBreakables.Clear();
    }
    
    public void Register(IBreakable breakable)
    {
        if(!_dynamicBreakables.Contains(breakable))
            _dynamicBreakables.Add(breakable);
    }
    
    public void ResetAllBreakables()
    {
        foreach (var b in _staticBreakables)
        {
            if (b != null)
                b.Reset();
        }
        foreach (var b in _dynamicBreakables)
        {
            if (b != null)
                Object.Destroy(b.GetTransform().gameObject);
        }
        _dynamicBreakables.Clear();
    }
}