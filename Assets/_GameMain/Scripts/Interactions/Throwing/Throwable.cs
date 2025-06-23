using DG.Tweening;
using UnityEngine;

public class Throwable : MonoBehaviour, IThrowable
{
    [SerializeField] private float collideImpulse = 4f;
    [SerializeField] private string obstacleTag = "Obstacle";

    public bool IsHeld => _isHeld;
    public Transform GetTransform() => transform;

    private Collider _collider;
    private Rigidbody _rb;
    private bool _isHeld;
    private Vector3 _initialScale;
    private Tween _flyTween;
    private bool _isFlyingTween;
    private Vector3 _lastTweenVelocity;

    public void OnPickupStart()
    {
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.isKinematic = true;
        _collider.enabled = false;
    }

    public void OnPickup(Transform holder)
    {
        if (_isHeld) return;
        _isHeld = true;
        AnimateSquash(0.8f, 0.1f);
    }

    public void Throw(Vector3 targetPos, float duration, float arc)
    {
        _isHeld = false;
        _isFlyingTween = true;
        _collider.enabled = true;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.isKinematic = true;

        var start = transform.position;
        var prevPos = start;
        var startY = start.y;
        var endY = targetPos.y;
        _lastTweenVelocity = Vector3.zero;

        _flyTween = DOTween.To(
            () => 0f, t =>
            {
                Vector3 pos = Vector3.Lerp(start, targetPos, t);
                pos.y = Mathf.Lerp(startY, endY, t) + Mathf.Sin(Mathf.PI * t) * arc;
                _lastTweenVelocity = (pos - prevPos) / Time.deltaTime;
                prevPos = pos;
                transform.position = pos;
            }, 1f, duration)
        .SetEase(Ease.Linear)
        .OnComplete(EndTween);

        AnimateSquash(0.85f, 0.13f);
    }
    
    // _____________ Private _____________
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _initialScale = transform.localScale;
        
        TriggerColliderUtility.CreateTriggerCollider(gameObject, _collider);
    }
    
    private void EndTween()
    {
        if (!_isFlyingTween) return;
        SwitchToPhysics(_lastTweenVelocity);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!_isFlyingTween) return;

        if (other.CompareTag(obstacleTag))
        {
            SwitchToPhysics(_lastTweenVelocity);
        }
    }

    private void SwitchToPhysics(Vector3 velocity)
    {
        _isFlyingTween = false;
        if (_flyTween != null && _flyTween.IsActive())
            _flyTween.Kill();
        _rb.isKinematic = false;
        _rb.linearVelocity = velocity.sqrMagnitude < 0.1f ? transform.forward * collideImpulse : velocity;
        _collider.enabled = true;
        AnimateSquash(1.1f, 0.1f);
    }

    private void AnimateSquash(float amount, float duration)
    {
        transform.DOComplete();
        transform.DOScale(
            new Vector3(_initialScale.x * amount, _initialScale.y / amount, _initialScale.z * amount),
            duration
        ).OnComplete(() => transform.DOScale(_initialScale, duration));
    }
}
