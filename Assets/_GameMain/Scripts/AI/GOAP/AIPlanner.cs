using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AIPlanner : MonoBehaviour
{
    private AIAction[] _availableActions;
    private AIAction _currentAction;
    private CancellationTokenSource _plannerCTS;

    private void Awake()
    {
        _availableActions = GetComponents<AIAction>();
        _plannerCTS = new CancellationTokenSource();
    }

    private void Start()
    {
        RunPlanner(_plannerCTS.Token).Forget();
    }

    private async UniTask RunPlanner(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var bestAction = _availableActions
                .Where(a => a.IsApplicable())
                .OrderByDescending(a => a.Priority)
                .FirstOrDefault();

            if (bestAction != null && bestAction != _currentAction)
            {
                if (_currentAction != null)
                {
                    if (_currentAction.IsCancellable)
                    {
                        _currentAction.CancelAction();
                        await UniTask.Yield();
                        
                        RunAction(bestAction).Forget();
                    }
                }
                else
                {
                    RunAction(bestAction).Forget();
                }
            }

            await UniTask.Delay(100, DelayType.DeltaTime, cancellationToken: token);
        }
    }

    private async UniTaskVoid RunAction(AIAction action)
    {
        try
        {
            _currentAction = action;
            await action.PerformAction();
        }
        finally
        {
            if (_currentAction == action)
                _currentAction = null;
        }
    }

    private void OnDestroy()
    {
        _plannerCTS?.Cancel();
        _plannerCTS?.Dispose();
        foreach (var action in _availableActions)
            action.CancelAction();
    }
}