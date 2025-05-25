using UnityEngine;

public class Throwable : MonoBehaviour, IThrowable
{
    public bool IsHeld => _isHeld;
    
    private Rigidbody _rb;
    private bool _isHeld;

    public void OnPickup(Transform holder)
    {
        if (_isHeld) return;
        _isHeld = true;
        _rb.isKinematic = true;
        transform.SetParent(holder, true);
    }

    public void OnThrow(Vector3 force)
    {
        if (!_isHeld) return;
        _isHeld = false;
        transform.SetParent(null);
        _rb.isKinematic = false;
        _rb.linearVelocity = Vector3.zero;
        _rb.AddForce(force, ForceMode.Impulse);
    }
    
    public Transform GetTransform()
    {
        return transform;
    }

    // _____________ Private _____________

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
}