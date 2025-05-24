using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(AIKnowledge), typeof(AIMovementController))]
public abstract class AIAction : MonoBehaviour
{
    [Header("Action Settings")] 
    public int Priority = 1;
    public bool IsCancellable = true;
    
    public abstract UniTask PerformAction();
    public abstract bool IsApplicable();

    protected AIMovementController _movement;
    protected AIKnowledge _aiKnowledge;

    protected CancellationTokenSource _actionCTS;

    protected virtual void Awake()
    {
        _movement = GetComponent<AIMovementController>();
        _aiKnowledge = GetComponent<AIKnowledge>();
    }
    
    public virtual void CancelAction()
    {
        _actionCTS?.Cancel();
        _actionCTS?.Dispose();
        _actionCTS = null;
    }
}