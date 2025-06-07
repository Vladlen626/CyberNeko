using System;
using DG.Tweening;
using UnityEngine;

public class Food : MonoBehaviour
{
    public event Action<int> OnDevoured;

    [SerializeField] private int points = 1;

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

    public void Hide()
    {
        _isActive = false;
        transform.DOScale(0, 0.25f);
    }

    public void Devoured()
    {
        Hide();
        OnDevoured?.Invoke(points);
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

    // _____________ Private _____________

    private void OnTriggerEnter(Collider other)
    {
        if (!_isActive) return;
        var devourer = other.GetComponent<Devourer>();
        if (devourer)
        {
            SetInactive();
            devourer.Eat(this);
        }
    }
}