using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SearchAction : AIAction
{
    public override async UniTask PerformAction()
    {
        _actionCTS = new CancellationTokenSource();
        _movement.LookAt(_aiKnowledge.Target);
        _movement.StopMovement();

        await UniTask.Yield();
    }

    public override bool IsApplicable()
    {
        return _aiKnowledge.IsTargetOnVision && !_aiKnowledge.IsAlerted;
    }
}
