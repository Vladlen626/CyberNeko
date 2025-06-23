using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PointsVisual : MonoBehaviour
{
    [SerializeField] private Slider foodGoalSlider;
    [SerializeField] private GameObject key;
    [SerializeField] private TextMeshProUGUI scoreTmp;

    private int _previousScore;
    
    [Inject]
    public void Construct(PointsManager pointsManager)
    {
        pointsManager.CurrentPoints
            .Subscribe(UpdateUiScoreText)
            .AddTo(this);

        pointsManager.CurrentGoalPoints
            .Subscribe(UpdateGoalSliderValue)
            .AddTo(this);

        pointsManager.IsKeyActive
            .Subscribe(UpdateKeyActiveSprite)
            .AddTo(this);
        
        pointsManager.GoalPoints
            .Subscribe(UpdateGoalSliderMaxValue)
            .AddTo(this);
    }

    private void UpdateGoalSliderValue(int currentValue)
    {
        foodGoalSlider.DOValue(currentValue, 0.15f);
        _previousScore = currentValue;
    }

    private void UpdateGoalSliderMaxValue(int currentValue)
    {
        foodGoalSlider.maxValue = currentValue;
    }

    private void UpdateKeyActiveSprite(bool isActive)
    {
        key.SetActive(isActive);
    }

    private void UpdateUiScoreText(int newScore)
    {
        DOTween.To(() => _previousScore, x => _previousScore = x, newScore, 0.25f)
            .OnUpdate(() =>  scoreTmp.text = _previousScore.ToString())
            .SetEase(Ease.Linear);
    }
}
