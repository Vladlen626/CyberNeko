using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ChaseAction : AIAction
{
    [Header("Settings")]
    public float UpdateInterval = 0.1f;
    public float StopDistance = 0.1f;
    
    private AIMovementController _movement;
    private AIKnowledge _aiKnowledge;

    private void Awake()
    {
        _movement = GetComponent<AIMovementController>();
        _aiKnowledge = GetComponent<AIKnowledge>();
    }

    public override async UniTask PerformAction()
    {
        _actionCTS = new CancellationTokenSource();
        
        try
        {
            while (!_actionCTS.Token.IsCancellationRequested)
            {
                _movement.UpdateDestination(_aiKnowledge.Target.transform.position);
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
        return _aiKnowledge.IsAlerted &&
               Vector3.Distance(transform.position, _aiKnowledge.Target.transform.position) > StopDistance;
    }
}