using System;
using DG.Tweening;
using UnityEngine;

public class Food : MonoBehaviour
{
    public event Action<int> OnDevoured;

    [SerializeField] private int _points = 1;

    private bool _isActive;
    private Vector3 _originalPosition;

    public void Initialize()
    {
        _originalPosition = transform.position;
    }

    public void Spawn()
    {
        transform.position = _originalPosition;
        transform.localScale = Vector3.one;
        _isActive = true;
    }

    public void Hide()
    {
        _isActive = false;
        transform.DOScale(0, 0.25f);
    }

    public void Devoured()
    {
        Hide();
        OnDevoured?.Invoke(_points);
    }

    public void SetInactive()
    {
        _isActive = false;
    }

    public void SetActive()
    {
        _isActive = true;
    }

    // _____________ Private _____________

    private void OnTriggerEnter(Collider other)
    {
        if (!_isActive) return;
        var devourer = other.GetComponent<Devourer>();
        if (devourer)
            devourer.Eat(this);
    }
}