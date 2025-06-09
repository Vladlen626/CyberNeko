using DG.Tweening;
using UnityEngine;

public class Throwable : MonoBehaviour, IThrowable
{
    [SerializeField] private float _rotationPower = 4;
    
    public bool IsHeld => _isHeld;

    private Collider _collider;
    private Rigidbody _rb;
    private bool _isHeld;
    private Vector3 _initialScale;

    public void OnPickupStart()
    {
        _rb.isKinematic = true;
        _collider.enabled = false;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }
    
    public void OnPickup(Transform holder)
    {
        if (_isHeld) return;
        _isHeld = true;
        AnimateSquash(0.8f, 0.1f);
    }

    public void OnThrow(Vector3 force)
    {
        if (!_isHeld) return;
        _isHeld = false;
        transform.SetParent(null);
        _rb.isKinematic = false;
        _rb.angularVelocity = Vector3.zero;
        _rb.linearVelocity = Vector3.zero;
        _collider.enabled = true;
        _rb.AddForce(force, ForceMode.Impulse);
        _rb.AddTorque(Random.onUnitSphere * _rotationPower, ForceMode.Impulse);
        
        AnimateStretch(1.15f, 0.075f);
    }
    
    public Transform GetTransform()
    {
        return transform;
    }

    // _____________ Private _____________

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _initialScale = transform.localScale;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (_isHeld) return;
        
        if (collision.relativeVelocity.magnitude > 1f)
        {
            AnimateSquash(0.85f, 0.08f);
        }
    }
    
    private void AnimateSquash(float amount, float duration)
    {
        transform.DOComplete();
        transform.DOScale(new Vector3(_initialScale.x * amount, _initialScale.y / amount, _initialScale.z * amount), duration)
            .OnComplete(() => transform.DOScale(_initialScale, duration));
    }

    private void AnimateStretch(float amount, float duration)
    {
        transform.DOComplete();
        transform.DOScale(new Vector3(_initialScale.x / amount, _initialScale.y * amount, _initialScale.z / amount), duration)
            .OnComplete(() => transform.DOScale(_initialScale, duration));
    }
}