using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class Food : MonoBehaviour
{
    [SerializeField] private int _points = 1;
    public event Action<int> OnDevoured;
    private bool _isActive;
    private Vector3 _originalPosition;

    public void Initialize()
    {
        _originalPosition = transform.position;
    }

    public int GetPoints()
    {
        return _points;
    }
    
    public void Spawn()
    {
        transform.localScale = Vector3.zero;
        transform.position = _originalPosition;
        transform.DOScale(1, 0.15f)
            .OnComplete(() =>
            {
                _isActive = true;
            });
    }

    public void Hide()
    {
        _isActive = false;
        transform.DOScale(0, 0.15f);
    }
    
    public void Devoured()
    {
        OnDevoured?.Invoke(_points);
        Hide();
    }

    public bool IsActive()
    {
        return _isActive;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isActive) return;
        var devourer = other.GetComponent<Devourer>();
        if (devourer)
        {
            devourer.Eat(this);
        }
    }
    
}
