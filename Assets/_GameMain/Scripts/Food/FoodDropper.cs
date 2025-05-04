using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FoodDropper : MonoBehaviour
{
    [SerializeField] private GameObject[] _foodsPrefabs;
    [SerializeField] private int _foodDropCount = 2;
    [SerializeField] private float _countdown = 5f;

    [SerializeField] private float maxOffsetX = 2f;
    [SerializeField] private float maxOffsetY = 0f;
    [SerializeField] private float maxOffsetZ = 2f;

    private float _lastDropTime = -Mathf.Infinity;
    private List<GameObject> _spawnedFoods = new List<GameObject>();

    private GameManager _gameManager;

    public void CleanUp()
    {
        foreach (var food in _spawnedFoods)
        {
            Destroy(food);
        }
        _spawnedFoods.Clear();
    }

    public void TryDropFood()
    {
        if (_foodsPrefabs.Length == 0 )
        {
            Debug.LogWarning("Список еды НПС пуст!");
            return;
        }

        if (Time.time - _lastDropTime < _countdown)
        {
            Debug.Log("Спавн еды НПС на кулдауне, скип");
            return;
        }

        DropFood();
    }

    private void DropFood()
    {
        _lastDropTime = Time.time;
        for (int i = 0; i < _foodDropCount; i++)
        {
            Debug.Log("Spawn food by droper");
            var randomFoodInx = Random.Range(0, _foodDropCount - 1);
            var randomFoodPrefab = _foodsPrefabs[randomFoodInx];
            var food = Instantiate(randomFoodPrefab, transform.position, Quaternion.identity);
            
            DropAnimation(food.transform);
            
            _spawnedFoods.Add(food);
        }
    }

    private void DropAnimation(Transform foodTransform)
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(0f, maxOffsetX),
            Random.Range(0f, maxOffsetY),
            Random.Range(0f, maxOffsetZ)
        );
        foodTransform.localScale = Vector3.one;

        DOTween.Sequence()
            .Append(foodTransform.DOJump(foodTransform.position + randomOffset, 1, 1, 0.25f).SetEase(Ease.Linear))
            .Join(foodTransform.DOScale(1, 0.15f).SetEase(Ease.Linear));
    }
    
}
