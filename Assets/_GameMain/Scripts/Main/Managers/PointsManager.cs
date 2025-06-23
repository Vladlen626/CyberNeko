using System;
using UniRx;
using UnityEngine;
using Zenject;

public class PointsManager : IDisposable
{
    public IReadOnlyReactiveProperty<int> GoalPoints => _goalPoints;
    private readonly ReactiveProperty<int> _goalPoints = new ReactiveProperty<int>(999);
    public IReadOnlyReactiveProperty<int> CurrentPoints => _currentPoints;
    private readonly ReactiveProperty<int> _currentPoints = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> CurrentGoalPoints => _currentGoalPoints;
    private readonly ReactiveProperty<int> _currentGoalPoints = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<bool> IsKeyActive => _isKeyActive;
    private readonly ReactiveProperty<bool> _isKeyActive = new ReactiveProperty<bool>(false);

    private ZoneController _zoneController;
    
    [Inject]
    private void Construct(ZoneController zoneController)
    {
        _zoneController = zoneController;
        _zoneController.OnCurrentZoneChanged += UpdateTargetGoalPoints;
    }
    
    public void Initialize()
    {
        DeactivateKey();
        ResetAllPoints();
    }

    public void UpdateCurrentGoalPoints()
    {
        UpdateTargetGoalPoints();
    }

    public void AddPoints(int points)
    {
        SetCurPoints(_currentPoints.Value + points);
        SetCurGoalPoints(_currentGoalPoints.Value + points);
    }

    public void ResetAllPoints()
    {
        ResetGoal();
        SetCurPoints(0);
    }
    
    public void ResetGoal()
    {
        DeactivateKey();
        SetCurGoalPoints(0);
    }
    
    public void Dispose()
    {
        if (_zoneController)
            _zoneController.OnCurrentZoneChanged -= UpdateTargetGoalPoints;
    }
    
    // _____________ Private _____________

    private void UpdateTargetGoalPoints()
    {
        _goalPoints.Value = _zoneController.CurrentZone.GoalPoints;
    }
    
    private void SetCurPoints(int points)
    {
        _currentPoints.Value = points;
    }

    private void SetCurGoalPoints(int points)
    {
        _currentGoalPoints.Value = points;
        if (_currentGoalPoints.Value >= _goalPoints.Value)
        {
            GoalReach();
        }
    }

    private void GoalReach()
    {
        if (!_isKeyActive.Value)
        {
            AudioManager.inst.PlaySound(SoundNames.GoalComplete);
        }
        ActivateKey();
    }

    private void ActivateKey() => _isKeyActive.Value = true;
    private void DeactivateKey() => _isKeyActive.Value = false;
    
}
