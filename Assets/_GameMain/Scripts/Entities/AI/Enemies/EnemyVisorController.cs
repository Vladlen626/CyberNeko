using UniRx;
using UnityEngine;

[RequireComponent(typeof(VisorController), typeof(AIKnowledge))]
public class EnemyVisorController : MonoBehaviour
{
    private AIKnowledge _aiKnowledge;
    private VisorController _visorController;

    private void Awake()
    {
        _aiKnowledge = transform.GetComponent<AIKnowledge>();
        _visorController = transform.GetComponent<VisorController>();
        _aiKnowledge.AlertProgress
            .Subscribe(ChangeVisorSettings);
    }

    private void ChangeVisorSettings(float value)
    {
        var visorNum = EnemyVisorEnum.Default;
        if (_aiKnowledge.IsAlerted)
        {
            visorNum = EnemyVisorEnum.Chase;
        } else if (value > 0.01)
        {
            visorNum = EnemyVisorEnum.Search;
        }
        
        _visorController.ChooseVisor((int)visorNum);
    }
}