using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIAnimatorController : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private string speedParamName = "Speed";
    [SerializeField] private float smoothTime = 0.1f;
    
    [Header("Speed Settings")]
    [SerializeField] private float maxAnimationSpeed = 1f;

    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;
    private float _currentSpeed;
    private float _smoothVelocity;

    private void Update()
    {
        if (!_agent || !_animator) return;
        UpdateMovementAnimation();
    }

    private void UpdateMovementAnimation()
    {
        float normalizedSpeed = Mathf.Clamp01(_agent.velocity.magnitude / _agent.speed);
        
        _currentSpeed = Mathf.SmoothDamp(
            _currentSpeed, 
            normalizedSpeed, 
            ref _smoothVelocity, 
            smoothTime
        );
        
        _animator.SetFloat(speedParamName, _currentSpeed * maxAnimationSpeed);
        
        _animator.speed = Mathf.Lerp(
            1f, 
            maxAnimationSpeed, 
            normalizedSpeed
        );
    }
}
