using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class PointsManager : MonoBehaviour
{
    [Header("Main Fields")]
    [SerializeField] private Slider _slider;
    [SerializeField] private GameObject _key;
    [SerializeField] private TextMeshProUGUI _text;
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

    public void AddPoints(int points)
    {
        Assert.IsTrue(points > 0, "Че умный такой, да?");

        SetCurPoints(_curPoints += points);

        if (goalPoints == _targetPoints)
        {
            _key.SetActive(true);
        }
    }

    public int GetCurrentPoints()
    {
        return _curPoints;
    }

    public void ResetGoalPoints()
    {
        SetGoalPoints(0);
        _key.SetActive(false);
    }

    public void ResetAllPoints()
    {
        ResetGoalPoints();
        SetCurPoints(0);
    }

    public bool IsKeyActive()
    {
        return _key.activeInHierarchy;
    }

    private void SetCurPoints(int points)
    {
        
    }

    private void SetGoalPoints(int points)
    {
        goalPoints = points;
        _slider.DOValue(goalPoints, 0.15f);
    }
    
    private void UpdateUiScoreText(TextMeshProUGUI scoreTmp, int oldScore, int newScore)
    {
        DOTween.To(() => oldScore, x => oldScore = x, newScore, 0.25f)
            .OnUpdate(() =>  scoreTmp.text = oldScore.ToString())
            .SetEase(Ease.Linear);
    }
}
