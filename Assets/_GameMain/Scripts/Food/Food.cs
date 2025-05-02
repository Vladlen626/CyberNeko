using System;
using UnityEngine;
using UnityEngine.AI;

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
            // do dotween logic - player eat this shit
            AddPoints();
            // after some time
            OnDevoured?.Invoke();
        }
    }

    private void AddPoints()
    {
        _pointsManager?.AddPoints(_points);
    }
}
