using UnityEngine;
using DG.Tweening;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private string _speedParam = "Speed";
    [SerializeField] private string _grabbedParam = "IsGrabbed";
    [SerializeField] private float _animSmooth = 0.1f;
    
    private float _curSpeed;
    private Tween _speedTween;

    public void Initialize(PlayerMovementController movement)
    {
        movement.OnSpeedChanged += SetSpeed;
        movement.OnGrabbedChanged += SetGrabbed;
    }

    // _____________ Private _____________

    private void SetSpeed(float speed)
    {
        _speedTween?.Kill();
        _speedTween = DOTween.To(() => _curSpeed, x => {
            _curSpeed = x;
            _animator.SetFloat(_speedParam, _curSpeed);
        }, speed, _animSmooth);
    }

    private void SetGrabbed(bool grabbed)
    {
        _animator.SetBool(_grabbedParam, grabbed);
    }

    private void OnDestroy()
    {
        _speedTween?.Kill();
    }
}