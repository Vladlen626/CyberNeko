using System;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class Food : MonoBehaviour, IDevourable
{
    [SerializeField] private int _points = 1;

    public event Action OnDevoured;
    private bool _isActive = false;

    private PointsManager _pointsManager;

    void Start()
    {
        _pointsManager = FindFirstObjectByType<PointsManager>();
        if (_pointsManager == null)
        {
            Debug.LogError("No PointsManager on the scene");
        }
    }

    public int GetPoints()
    {
        return _points;
    }
    
    // use dotweeen
    public void Show()
    {
        _isActive = true;
    }

    // use dotweeen
    public void Hide()
    {
        // Add dotween logic - player eat this shit
        // after some time call OnDevoured so FoodSpawner will set new position for this food

        _isActive = false;
    }

    public bool IsActive()
    {
        return _isActive;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Collected Item");
            AddPoints();
            
            // TODO: Change this logic after GAM
            Hide(); // Im stupid. Show() call from FoodSpawner, Hide() from this point so when player eat this it can be hidden by himself

            // TODO: ADD DELAY BEFORE OnDevoured?.Invoke() depends on dotweeen time logic BECAUSE IT WILL CHANGE FOOD POSITION!!!
            OnDevoured?.Invoke();
        }
    }

    private void AddPoints()
    {
        _pointsManager?.AddPoints(_points);
    }
}
