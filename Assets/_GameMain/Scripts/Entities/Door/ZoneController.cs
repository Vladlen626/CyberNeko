using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZoneController : MonoBehaviour
{
    public event Action OnCurrentZoneChanged;
    
    private List<Zone> _zones;
    private int _currentZoneIndex = 1;
    public Zone CurrentZone => _zones[_currentZoneIndex - 1];
    public void Initialize()
    {
        _zones = FindObjectsByType<Zone>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OrderBy(z => z.ZoneOrder)
            .ToList();
        
        foreach (var zone in _zones)
        {
            zone.Initialize();
            zone.OnOpened += GoToNextZone;
        }
    }

    void OnDestroy()
    {
        foreach (var zone in _zones)
        {
            zone.OnOpened -= GoToNextZone;
            zone.Terminate();
        }
    }
    
    private void GoToNextZone()
    {
        if (_currentZoneIndex < _zones.Count - 1)
        {
            SetCurrentZone(_currentZoneIndex + 1);
        }
    }

    private void SetCurrentZone(int zoneIndex)
    {
        _currentZoneIndex = zoneIndex;
        OnCurrentZoneChanged?.Invoke();
    }
}
