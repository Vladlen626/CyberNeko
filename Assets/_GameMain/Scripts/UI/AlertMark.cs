using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

public class AlertMark : MonoBehaviour
{
    [SerializeField] private AIKnowledge _aiKnowledge;
    [SerializeField] private Transform fillObject;
    [SerializeField] private Transform backGround;

    private void Start()
    {
        if (!_aiKnowledge)
        {
            Debug.LogWarning("Mark without ai knowledge");
            return;
        }
        
        _aiKnowledge.AlertProgress
            .Subscribe(FillMark);
    }

    private void FillMark(float value)
    {
        backGround.gameObject.SetActive(!(value < 0.01));
        fillObject.DOScaleY(value, 0.15f);
    }
}
