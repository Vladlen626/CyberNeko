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

    [Header("Camera Reference")]
    [SerializeField] private CinemachineCamera playerFollowCamera;

    private NavMeshAgent _agent;
    private Transform _cameraTransform;
    private Vector3 _movementDirection;
    private float _currentSpeed;
    private bool _isRunning;
    private CancellationTokenSource _movementCTS;

    public void Initialize()
    {
        _agent = GetComponent<NavMeshAgent>();
        _movementCTS = new CancellationTokenSource();
        SetupAgent();
        CacheCamera();
    }

    private void Start()
    {
        HandleSpeedTransition().Forget();
    }

    private void OnDestroy()
    {
        _movementCTS?.Cancel();
        _movementCTS?.Dispose();
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
        HandleMovementInput();
        UpdateMovement();
        UpdateRotation();
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

    private async UniTaskVoid HandleSpeedTransition()
    {
        while (!_movementCTS.IsCancellationRequested)
        {
            float targetSpeed = runSpeed;
            _currentSpeed = Mathf.MoveTowards(
                _currentSpeed, 
                targetSpeed, 
                acceleration * Time.deltaTime
            );

            await UniTask.Yield(PlayerLoopTiming.Update, _movementCTS.Token);
        }
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

}
