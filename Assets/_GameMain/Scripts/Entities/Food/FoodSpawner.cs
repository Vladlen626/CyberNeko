using System.Collections.Generic;
using UnityEngine;
using Zenject;
using DG.Tweening;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private List<Food> _foodDropPrefab;
    private PointsManager _pointsManager;
    private Food[] _sceneFood;
    private readonly List<Food> _npcDroppedFood = new();

    public void Initialize()
    {
        _sceneFood = FindObjectsByType<Food>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var food in _sceneFood)
        {
            food.Initialize();
            food.OnDevoured += HandleOnDevoured;
        }
    }
    
    public void ResetAllFood()
    {
        foreach (var food in _sceneFood)
            food.Spawn();

        foreach (var dropped in _npcDroppedFood)
        {
            if (dropped != null)
                Destroy(dropped.gameObject);
        }
        _npcDroppedFood.Clear();
    }

    public void DropFood(Vector3 pos)
    {
        var prefab = _foodDropPrefab[Random.Range(0, _foodDropPrefab.Count)];
        var food = Instantiate(prefab, pos, Quaternion.identity);
        AnimateFoodDrop(food, pos);
        food.OnDevoured += HandleOnDevoured;
        _npcDroppedFood.Add(food);
    }

    // _____________ Private _____________

    [Inject]
    private void Construct(PointsManager pointsManager)
    {
        _pointsManager = pointsManager;
    }

    private void HandleOnDevoured(int points)
    {
        _pointsManager?.AddPoints(points);
    }

    private void AnimateFoodDrop(Food food, Vector3 dropOrigin)
    {
        float jumpForce = Random.Range(1.2f, 1.7f);
        float spread = 1f;
        Vector3 randomOffset = new Vector3(Random.Range(-spread, spread), 0f, Random.Range(-spread, spread));
        food.transform.position = dropOrigin + randomOffset * 0.3f;
        food.transform.localScale = Vector3.zero;
        food.SetInactive();

        DOTween.Sequence()
            .Append(food.transform.DOScale(1, 0.25f).SetEase(Ease.OutBack))
            .Join(food.transform.DOJump(dropOrigin + randomOffset, jumpForce, 1, 0.4f).SetEase(Ease.OutCubic))
            .OnComplete(food.SetActive);
    }
}
