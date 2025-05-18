using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class AIAction : MonoBehaviour
{
    protected CancellationTokenSource _actionCTS;
    
    public int Priority = 1;
    public abstract UniTask PerformAction();
    public abstract bool IsApplicable();
    
    public virtual void CancelAction()
    {
        _actionCTS?.Cancel();
        _actionCTS?.Dispose();
        _actionCTS = null;
    }
}