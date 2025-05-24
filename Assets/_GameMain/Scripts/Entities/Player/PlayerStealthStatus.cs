using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStealthStatus : MonoBehaviour
{
    private readonly HashSet<AIVisionSensor> _chasingEnemies = new();

    public event Action<bool> OnChaseStateChanged;

    public bool IsChased()
    {
        return _chasingEnemies.Count > 0;
    }

    public void RegisterChaser(AIVisionSensor sensor)
    {
        var wasChased = IsChased();
        _chasingEnemies.Add(sensor);
        if (!wasChased && IsChased())
            OnChaseStateChanged?.Invoke(true);
    }
    public void UnregisterChaser(AIVisionSensor sensor)
    {
        var wasChased = IsChased();
        _chasingEnemies.Remove(sensor);
        if (wasChased && !IsChased())
            OnChaseStateChanged?.Invoke(false);
    }
}
