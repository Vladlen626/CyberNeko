// PatrolAction.cs

using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PatrolAction : AIAction
{
    [Header("Settings")]
    public float WaypointPause = 2f;
    public float ReachDistance = 0.5f;
    
    private Transform[] _patrolPoints;
    private int _currentIndex;
    private AIMovementController _movement;

    public void SetPatrolPoints(Transform[] patrolPoints)
    {
        _patrolPoints = patrolPoints;
    }
    
    public override async UniTask PerformAction()
    {
        _actionCTS = new CancellationTokenSource();
        
        while (!_actionCTS.Token.IsCancellationRequested)
        {
            await MoveToNextPoint();
            await PauseAtPoint();
            SelectNextPoint();
        }
    }
    
    public override bool IsApplicable()
    {
        return !GetComponent<WorldState>().IsAlerted && 
               _patrolPoints.Length > 0;
    }
    
    // _____________ Private _____________
    
    private void Awake()
    {
        _movement = GetComponent<AIMovementController>();
    }

    private async UniTask MoveToNextPoint()
    {
        await _movement.MoveToPoint(_patrolPoints[_currentIndex].position, ReachDistance, _actionCTS.Token);
    }

    private async UniTask PauseAtPoint()
    {
        await UniTask.Delay((int)(WaypointPause * 1000), 
            cancellationToken: _actionCTS.Token);
    }

    private void SelectNextPoint()
    {
        _currentIndex = (_currentIndex + 1) % _patrolPoints.Length;
    }

}