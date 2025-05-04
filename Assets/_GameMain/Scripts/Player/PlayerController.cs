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

    private NavMeshAgent agent;
    private Transform cameraTransform;
    private Vector3 movementDirection;
    private float currentSpeed;
    private float currentAnimSpeed;
    private float animSpeedVelocity;
    private bool Initialized;

    public void Initialize()
    {
        agent = GetComponent<NavMeshAgent>();
        SetupAgent();
        Initialized = true;
    }

    public void SetupCamera(Transform inCameraTransform)
    {
        cameraTransform = inCameraTransform;
    }

    // _____________ Private _____________
    private void SetupAgent()
    {
        agent.updatePosition = true;
        agent.updateRotation = false;
        agent.acceleration = acceleration;
        agent.speed = runSpeed;
        agent.autoBraking = false;  // Отключаем автоторможение
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
        float targetSpeed = agent.velocity.magnitude / runSpeed;
        
        // Плавное изменение параметра скорости
        currentAnimSpeed = Mathf.SmoothDamp(
            currentAnimSpeed,
            targetSpeed,
            ref animSpeedVelocity,
            animationSmoothTime
        );

        animator.SetFloat(speedParamName, currentAnimSpeed);
    }

    private void HandleMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); // Используем Raw для мгновенного отклика
        float vertical = Input.GetAxisRaw("Vertical");

        if (cameraTransform != null)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            // Плавная интерполяция направления ввода
            movementDirection = Vector3.Lerp(
                movementDirection,
                (forward * vertical + right * horizontal).normalized,
                inputResponse * Time.deltaTime
            );
        }
    }

    private void UpdateMovement()
    {
        if (movementDirection.magnitude > 0.1f)
        {
            // Непосредственное управление скоростью
            agent.velocity = movementDirection * runSpeed;
        }
        else
        {
            agent.velocity = Vector3.zero;
        }
    }

    private void UpdateRotation()
    {
        if (movementDirection.sqrMagnitude > 0.01f)
        {
            // Более резкий поворот с использованием Lerp вместо Slerp
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
    

}
