using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class AIAction : MonoBehaviour
{
    protected readonly float cost = 1f;
    
    public abstract UniTask PerformAction();
    public abstract bool PreCondition();
    public abstract void PostEffect();
    public virtual float GetCost() => cost;
}