using UniRx;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    private int _targetPoints = 100;
    public IReadOnlyReactiveProperty<int> CurrentPoints => _currentPoints;
    private readonly ReactiveProperty<int> _currentPoints = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> GoalPoints => _goalPoints;
    private readonly ReactiveProperty<int> _goalPoints = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<bool> IsKeyActive => _isKeyActive;
    private readonly ReactiveProperty<bool> _isKeyActive = new ReactiveProperty<bool>(false);

    public void Initialize()
    {
        DeactivateKey();
        ResetAllPoints();
    }

    public void AddPoints(int points)
    {
        SetCurPoints(_currentPoints.Value + points);
        SetGoalPoints(_goalPoints.Value + points);
    }

    public void ResetAllPoints()
    {
        ResetGoal();
        SetCurPoints(0);
    }

    public int GetTargetPoints()
    {
        return _targetPoints;
    }
    
    public void ResetGoal()
    {
        DeactivateKey();
        SetGoalPoints(0);
    }
    
    // _____________ Private _____________

    private void SetCurPoints(int points)
    {
        _currentPoints.Value = points;
    }

    private void SetGoalPoints(int points)
    {
        _goalPoints.Value = points;
        if (_goalPoints.Value > _targetPoints)
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
