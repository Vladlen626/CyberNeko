using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class PointsManager : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private GameObject _key;
    //[SerializeField] private Text _text;

    [SerializeField] private int _targetPoints = 10;

    private int _curPoints = 0;

    public async UniTask Initialize()
    {
        _key.SetActive(false);
        SetCurPoints(0);
        _slider.maxValue = _targetPoints;
        await UniTask.Yield();
    }

    public void AddPoints(int points)
    {
        Assert.IsTrue(points > 0, "Че умный такой, да?");

        SetCurPoints(_curPoints += points);

        if (_curPoints == _targetPoints)
        {
            _key.SetActive(true);
        }
    }

    public void ResetPoints()
    {
        SetCurPoints(0);
        _key.SetActive(false);
    }

    public bool IsKeyActive()
    {
        return _key.activeInHierarchy;
    }

    private void SetCurPoints(int points)
    {
        _curPoints = points;
        _slider.value = _curPoints;
        //_text.text = _curPoints;
    }
}
