using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ChaseAction : AIAction
{
    [Header("Settings")]
    public float UpdateInterval = 0.1f;
    public float StopDistance = 0.1f;
    
    private AIMovementController _movement;
    private WorldState _worldState;

    private void Awake()
    {
        _movement = GetComponent<AIMovementController>();
        _worldState = GetComponent<WorldState>();
    }

    public override async UniTask PerformAction()
    {
        _actionCTS = new CancellationTokenSource();
        
        try
        {
            while (!_actionCTS.Token.IsCancellationRequested)
            {
                _movement.UpdateDestination(_worldState.Target.transform.position);
                await UniTask.Delay((int)(UpdateInterval * 1000), 
                    cancellationToken: _actionCTS.Token);
            }
        }
        finally
        {
            _movement.StopMovement();
        }
    }

    public override bool IsApplicable()
    {
        return _worldState.IsAlerted &&
               Vector3.Distance(transform.position, _worldState.Target.transform.position) > StopDistance;
    }
}