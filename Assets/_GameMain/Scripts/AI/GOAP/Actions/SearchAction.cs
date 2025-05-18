using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SearchAction : AIAction
{
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
        _movement.LookAt(_aiKnowledge.Target);
        _movement.StopMovement();

        await UniTask.Yield();
    }

    public override bool IsApplicable()
    {
        return _aiKnowledge.IsTargetOnVision;
    }
}
