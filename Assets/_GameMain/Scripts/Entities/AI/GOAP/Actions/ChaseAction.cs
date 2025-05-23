using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ChaseAction : AIAction
{
    [Header("Chase Settings")]
    public float UpdateInterval = 0.1f;

    public override async UniTask PerformAction()
    {
        _actionCTS = new CancellationTokenSource();
        
        try
        {
            while (!_actionCTS.Token.IsCancellationRequested)
            {
                _movement.UpdateDestination(_aiKnowledge.Target.position);
                await UniTask.Delay((int)(UpdateInterval * 1000), cancellationToken: _actionCTS.Token);
            }
        }
        finally
        {
            _movement.StopMovement();
        }
    }

    public override bool IsApplicable()
    {
        return _aiKnowledge.IsAlerted;
    }
}