// GOAPPlanner.cs
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AIPlanner : MonoBehaviour
{
    private List<AIAction> _actions = new();
    private List<AIGoal> _goals = new();
    private AIWorldState _worldState;

    private void Awake()
    {
        GetComponents(_actions);
        GetComponents(_goals);
        _worldState = GetComponent<AIWorldState>();
    }

    private async void Start()
    {
        await PlanningLoop();
    }
    
    private async UniTask PlanningLoop()
    {
        while (true)
        {
            var goal = SelectGoal();
            var action = SelectAction(goal);
            
            if (action != null)
            {
                await ExecuteAction(action);
            }
            
            await UniTask.Yield();
        }
    }
    
    private AIGoal SelectGoal()
    {
        return _goals
            .Where(g => !g.IsGoalAchieved())
            .OrderByDescending(g => g.GetPriority())
            .FirstOrDefault();
    }
    
    private AIAction SelectAction(AIGoal goal)
    {
        return _actions
            .Where(a => a.PreCondition())
            .OrderBy(a => a.GetCost())
            .FirstOrDefault();
    }
    
    private async UniTask ExecuteAction(AIAction action)
    {
        await action.PerformAction();
        action.PostEffect();
    }
}