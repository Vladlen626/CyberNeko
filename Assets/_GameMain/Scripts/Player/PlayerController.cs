using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float runSpeed = 8f;        
    [SerializeField] private float rotationSpeed = 25f; 
    [SerializeField] private float acceleration = 30f;  
    [SerializeField] private float inputResponse = 10f;
    
    [Header("Animation Settings")]
    [SerializeField] private string speedParamName = "Speed";
    [SerializeField] private float animationSmoothTime = 0.1f;
    [SerializeField] private Animator animator;
    
    [Header("Cinemachine")]
    [SerializeField] private CinemachineCamera playerFollowCamera;

    private NavMeshAgent _agent;
    private Transform _cameraTransform;
    private Vector3 _movementDirection;
    private float _currentSpeed;
    private float _currentAnimSpeed;
    private float _animSpeedVelocity;
    private bool Initialized;

    public void Initialize()
    {
        _agent = GetComponent<NavMeshAgent>();
        SetupAgent();
        CacheCamera();
        Initialized = true;
    }

    private void CacheCamera()
    {
        if (playerFollowCamera != null)
        {
            _cameraTransform = playerFollowCamera.transform;
        }
        else
        {
            Debug.LogError("Cinemachine Virtual Camera not assigned!");
        }
    }

    private void SetupAgent()
    {
        _agent.updatePosition = true;
        _agent.updateRotation = false;
        _agent.acceleration = acceleration;
        _agent.speed = runSpeed;
        _agent.autoBraking = false;  // Отключаем автоторможение
    }

    private void Update()
    {
        if (!Initialized) return;
        HandleMovementInput();
        UpdateRotation();
        UpdateAnimations();
    }
    
    private void FixedUpdate()
    {
        if (!Initialized) return;
        UpdateMovement();
    }
    
    private void UpdateAnimations()
    {
        if (animator == null) return;

        // Рассчитываем фактическую скорость движения
        float targetSpeed = _agent.velocity.magnitude / runSpeed;
        
        // Плавное изменение параметра скорости
        _currentAnimSpeed = Mathf.SmoothDamp(
            _currentAnimSpeed,
            targetSpeed,
            ref _animSpeedVelocity,
            animationSmoothTime
        );

        animator.SetFloat(speedParamName, _currentAnimSpeed);
    }

    private void HandleMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); // Используем Raw для мгновенного отклика
        float vertical = Input.GetAxisRaw("Vertical");

        if (_cameraTransform != null)
        {
            Vector3 forward = _cameraTransform.forward;
            Vector3 right = _cameraTransform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            // Плавная интерполяция направления ввода
            _movementDirection = Vector3.Lerp(
                _movementDirection,
                (forward * vertical + right * horizontal).normalized,
                inputResponse * Time.deltaTime
            );
        }
    }

    private void UpdateMovement()
    {
        if (_movementDirection.magnitude > 0.1f)
        {
            // Непосредственное управление скоростью
            _agent.velocity = _movementDirection * runSpeed;
        }
        else
        {
            _agent.velocity = Vector3.zero;
        }
    }

    private void UpdateRotation()
    {
        if (_movementDirection.sqrMagnitude > 0.01f)
        {
            // Более резкий поворот с использованием Lerp вместо Slerp
            Quaternion targetRotation = Quaternion.LookRotation(_movementDirection);
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
    

}
