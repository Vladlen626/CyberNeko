using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PointsManager : MonoBehaviour
{
    [Header("Main Fields")]
    [SerializeField] private Slider _slider;
    [SerializeField] private GameObject _key;
    [FormerlySerializedAs("_text")] [SerializeField] private TextMeshProUGUI _scoreTmp;
    [SerializeField] private int _targetPoints = 10;

    [Header("Additional settings")] [SerializeField]
    private int pointsMultiplier = 10;

    private int _curPoints = 0;
    private int goalPoints;

    public async UniTask Initialize()
    {
        _key.SetActive(false);
        ResetAllPoints();
        _slider.maxValue = _targetPoints;
        await UniTask.Yield();
    }

    public int GetCurrentPoints()
    {
        return _curPoints;
    }

    public void AddPoints(int points)
    {
        AddToGoalPoints(points);
        AddToCurrentPoints(points);
    }

    public void ResetGoalPoints()
    {
        SetGoalPoints(0);
        _key.SetActive(false);
    }

    public void ResetAllPoints()
    {
        ResetGoalPoints();
        AddToCurrentPoints(0);
    }

    public bool IsKeyActive()
    {
        return _key.activeInHierarchy;
    }

    private void AddToCurrentPoints(int points)
    {
        var newPoints = points * pointsMultiplier;
        SetCurPoints(_curPoints + newPoints);
    }

    private void SetCurPoints(int points)
    {
        UpdateUiScoreText(_scoreTmp, _curPoints, points);
        _curPoints = points;
    }
    
    private void AddToGoalPoints(int points)
    {
        SetGoalPoints(goalPoints + points);
    }
    
    private void SetGoalPoints(int points)
    {
        goalPoints = points;
        if (goalPoints == _targetPoints)
        {
            GoalReach();
        }
        _slider.DOValue(goalPoints, 0.15f);
    }
    
    private void UpdateUiScoreText(TextMeshProUGUI scoreTmp, int oldScore, int newScore)
    {
        DOTween.To(() => oldScore, x => oldScore = x, newScore, 0.25f)
            .OnUpdate(() =>  scoreTmp.text = oldScore.ToString())
            .SetEase(Ease.Linear);
    }

    private void GoalReach()
    {
        AudioManager.inst.PlaySound(SoundNames.GoalComplete);
        _key.SetActive(true);
    }
}
