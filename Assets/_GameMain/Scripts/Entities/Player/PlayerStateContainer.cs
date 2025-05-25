using System;
using System.Collections.Generic;

[Serializable]
public class PlayerStateContainer
{
    private readonly HashSet<PlayerState> _activeStates = new();
    public IReadOnlyCollection<PlayerState> ActiveStates => _activeStates;
    
    public event Action<PlayerState> OnStateAdded;
    public event Action<PlayerState> OnStateRemoved;

    public bool HasState(PlayerState state) => _activeStates.Contains(state);

    public void AddState(PlayerState state)
    {
        if (_activeStates.Add(state))
            OnStateAdded?.Invoke(state);
    }

    public void RemoveState(PlayerState state)
    {
        if (_activeStates.Remove(state))
            OnStateRemoved?.Invoke(state);
    }

    public void ClearAllStates()
    {
        foreach (var state in _activeStates)
            OnStateRemoved?.Invoke(state);
        _activeStates.Clear();
    }
}