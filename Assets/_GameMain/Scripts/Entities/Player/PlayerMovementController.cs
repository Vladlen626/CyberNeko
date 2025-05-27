using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour
{
    public event Action<float> OnSpeedChanged;

    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _rotationSpeed = 12f;
    [SerializeField] private float _acceleration = 100f;    
    [SerializeField] private float _deceleration = 200f;      
    [SerializeField] private float _slideFactor = 0.05f;      
    [SerializeField] private float _squashAmount = 0.9f;
    [SerializeField] private float _squashDuration = 0.08f;
    [SerializeField] private List<PlayerState> _blockMovementStates;

    private Rigidbody _rb;
    private Transform _cameraTransform;
    private IInputService _inputService;
    private PlayerStateContainer _stateContainer;

    private Vector3 _input;
    private Vector3 _velocity;
    private Vector3 _desiredVelocity;
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
        _desiredVelocity = Vector3.zero;
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
        
        _velocity = _input * _moveSpeed;
        
        if (_input.sqrMagnitude < 0.01f)
        {
            _velocity = Vector3.zero;
        }

        ApplyVelocity();
        AnimateMotion();

        if (_velocity.sqrMagnitude > 0.01f)
        {
            var rot = Quaternion.LookRotation(_velocity);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, _rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void ApplyVelocity()
    {
        _rb.linearVelocity = _velocity;
        OnSpeedChanged?.Invoke(_velocity.magnitude / _moveSpeed);
    }

    private void AnimateMotion()
    {
        bool isMoving = _velocity.sqrMagnitude > 0.05f;
        if (isMoving && !_wasMoving)
        {
            PlaySquash(_squashAmount, _squashDuration);
        }
        if (!isMoving && _wasMoving)
        {
            PlaySquash(1.1f, _squashDuration * 1.2f);
        }
        _wasMoving = isMoving;
    }

    private void PlaySquash(float squash, float duration)
    {
        transform.DOComplete();
        transform.DOScale(new Vector3(_initialScale.x * squash, _initialScale.y / squash, _initialScale.z * squash), duration)
            .OnComplete(() => transform.DOScale(_initialScale, duration));
    }

    private bool CanMove()
    {
        foreach (var state in _blockMovementStates)
        {
            if (_stateContainer.HasState(state))
            {
                return false;
            }
        }

        return true;
    }
}
