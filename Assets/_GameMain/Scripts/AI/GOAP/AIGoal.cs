using UnityEngine;

public abstract class AIGoal : MonoBehaviour
{
    [SerializeField, Tooltip("Базовый приоритет цели")]
    protected int priority = 1;
    
    public abstract float GetPriority();
    public abstract bool IsGoalAchieved();
}