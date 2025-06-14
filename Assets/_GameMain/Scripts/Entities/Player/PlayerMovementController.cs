using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour
{
    public event Action<float> OnSpeedChanged;
    [SerializeField] private Transform playerModel;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float squashAmount = 0.9f;
    [SerializeField] private float squashDuration = 0.08f;
    [SerializeField] private List<PlayerState> blockMovementStates;
    [SerializeField] private float overEatenSpeedMultiplier = 0.5f;

    private Rigidbody _rb;
    private Transform _cameraTransform;
    private IInputService _inputService;
    private PlayerStateContainer _stateContainer;

    private Vector3 _input;
    private Vector3 _velocity;
    private Vector3 _initialScale;
    private bool _wasMoving;

    [Inject]
    public void Construct(IInputService inputService)
    {
        _inputService = inputService;
    }

    public void Initialize(Transform cameraTransform, Vector3 spawnPos, PlayerStateContainer stateContainer)
    {
        _cameraTransform = cameraTransform;
        _stateContainer = stateContainer;

        _rb.position = spawnPos;
        _initialScale = transform.localScale;
        ForceStop();
    }

    public void ForceStop()
    {
        _velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        ApplyVelocity();
    }

    // _____________ Private _____________

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!CanMove())
        {
            _input = Vector3.zero;
            return;
        }

        var h = _inputService.GetHorizontal();
        var v = _inputService.GetVertical();

        if (_cameraTransform)
        {
            var forward = _cameraTransform.forward; forward.y = 0;
            var right = _cameraTransform.right; right.y = 0;
            _input = (forward.normalized * v + right.normalized * h).normalized;
        }
        else
        {
            _input = new Vector3(h, 0, v).normalized;
        }
    }

    private void FixedUpdate()
    {
        if (!CanMove())
        {
            ForceStop();
            AnimateMotion();
            return;
        }
        
        _velocity = _input * GetCurrentSpeed();
        
        if (_input.sqrMagnitude < 0.01f)
        {
            _velocity = Vector3.zero;
        }

        ApplyVelocity();
        AnimateMotion();

        if (_velocity.sqrMagnitude > 0.01f)
        {
            var rot = Quaternion.LookRotation(_velocity);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, rotationSpeed * Time.fixedDeltaTime);
        }
    }
    
    private float GetCurrentSpeed()
    {
        float speed = moveSpeed;
        
        if (_stateContainer != null && _stateContainer.HasState(PlayerState.OverEaten))
            speed *= overEatenSpeedMultiplier;

        return speed;
    }

    private void ApplyVelocity()
    {
        var vel = _rb.linearVelocity;
        vel.x = _velocity.x;
        vel.z = _velocity.z;
        _rb.linearVelocity = vel;
        OnSpeedChanged?.Invoke(_velocity.magnitude / GetCurrentSpeed());
    }

    private void AnimateMotion()
    {
        bool isMoving = _velocity.sqrMagnitude > 0.05f;
        if (isMoving && !_wasMoving)
        {
            PlaySquash(squashAmount, squashDuration);
        }
        if (!isMoving && _wasMoving)
        {
            PlaySquash(1.1f, squashDuration * 1.2f);
        }
        _wasMoving = isMoving;
    }

    private void PlaySquash(float squash, float duration)
    {
        playerModel.DOComplete();
        playerModel.DOScale(new Vector3(_initialScale.x * squash, _initialScale.y / squash, _initialScale.z * squash), duration)
            .OnComplete(() => playerModel.DOScale(_initialScale, duration));
    }

    private bool CanMove()
    {
        foreach (var state in blockMovementStates)
        {
            if (_stateContainer.HasState(state))
            {
                return false;
            }
        }

        return true;
    }
}
