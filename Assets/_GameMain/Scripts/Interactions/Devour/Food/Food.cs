using System;
using DG.Tweening;
using UnityEngine;

public class Food : MonoBehaviour
{
    public event Action<int> OnDevoured;

    [SerializeField] private int points = 1;
    public int Points => points;

    [SerializeField] private Collider col;
    [SerializeField] private Rigidbody rb;

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
        SetActive();
        
        rb.isKinematic = false;
        col.enabled = true;
    }

    public void MultiplyPoints(int multiplier)
    {
        points *= multiplier;
    }

    public void Devoured()
    {
        Hide();
        OnDevoured?.Invoke(Points);
    }
    
    public void DisablePhysics()
    {
        rb.angularVelocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
        col.enabled = false;
    }

    public void SetInactive()
    {
        _isActive = false;
    }

    public void SetActive()
    {
        _isActive = true;
    }

    public bool IsActive()
    {
        return _isActive;
    }

    // _____________ Private _____________
    
    private void Hide()
    {
        SetInactive();
        transform.DOScale(0, 0.25f);
    }
}