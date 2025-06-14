using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PointsVisual : MonoBehaviour
{
    [SerializeField] private Slider _foodGoalSlider;
    [SerializeField] private GameObject _key;
    [SerializeField] private TextMeshProUGUI _scoreTmp;

    private int previousScore;
    
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
        _foodGoalSlider.DOValue(currentValue, 0.15f);
        previousScore = currentValue;
    }

    private void UpdateGoalSliderMaxValue(int currentValue)
    {
        _foodGoalSlider.maxValue = currentValue;
    }

    private void UpdateKeyActiveSprite(bool isActive)
    {
        _key.SetActive(isActive);
    }

    private void UpdateUiScoreText(int newScore)
    {
        DOTween.To(() => previousScore, x => previousScore = x, newScore, 0.25f)
            .OnUpdate(() =>  _scoreTmp.text = previousScore.ToString())
            .SetEase(Ease.Linear);
    }
}
