using System.Collections.Generic;
using UnityEngine;
using Zenject;
using DG.Tweening;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private List<Food> foodDropPrefab;
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
        var prefab = foodDropPrefab[Random.Range(0, foodDropPrefab.Count)];
        var food = Instantiate(prefab, pos, Quaternion.identity);
        
        food.transform.localScale = Vector3.zero;
        food.SetInactive();

        _npcDroppedFood.Add(food);
        food.OnDevoured += HandleOnDevoured;
        
        var rb = food.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            
            float force = Random.Range(2.0f, 3.2f);
            Vector3 dir = (Vector3.up * 0.7f) + 
                          (Vector3.forward * Random.Range(-0.85f, 0.85f)) + 
                          (Vector3.right * Random.Range(-0.85f, 0.85f));
            rb.AddForce(dir.normalized * force, ForceMode.Impulse);
            rb.AddTorque(Random.onUnitSphere * Random.Range(2, 8), ForceMode.Impulse);
        }
        
        DOTween.Sequence()
            .Append(food.transform.DOScale(1, 0.18f).SetEase(Ease.OutBack))
            .AppendCallback(() => food.SetActive());
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
    
}
