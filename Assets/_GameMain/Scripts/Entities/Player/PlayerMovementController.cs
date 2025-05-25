using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour
{
    public event Action<float> OnSpeedChanged;

    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _rotationSpeed = 12f;
    [SerializeField] private float _acceleration = 24f;
    [SerializeField] private float _inputSmooth = 10f;
    [SerializeField] private List<PlayerState> _blockMovementStates;
    
    private Rigidbody _rb;
    private Transform _cameraTransform;
    private Vector3 _input;
    private Vector3 _smoothedInput;
    private IInputService _inputService;
    private PlayerStateContainer _stateContainer; 

    [Inject]
    public void Construct(IInputService inputService)
    {
        _inputService = inputService;
    }

    public void Initialize(Transform cameraTransform, Vector3 spawnPos,PlayerStateContainer stateContainer)
    {
        _cameraTransform = cameraTransform;
        _stateContainer = stateContainer;
        
        _rb.position = spawnPos;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    public void ForceStop()
    {
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    // _____________ Private _____________

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!CanMove()) return;

        var h = _inputService?.GetHorizontal() ?? Input.GetAxisRaw("Horizontal");
        var v = _inputService?.GetVertical() ?? Input.GetAxisRaw("Vertical");

        if (_cameraTransform != null)
        {
            var forward = _cameraTransform.forward; forward.y = 0;
            var right = _cameraTransform.right; right.y = 0;
            _input = (forward.normalized * v + right.normalized * h).normalized;
        }
        else
        {
            _input = new Vector3(h, 0, v).normalized;
        }
        _smoothedInput = Vector3.Lerp(_smoothedInput, _input, _inputSmooth * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!CanMove()) return;
        
        _rb.linearVelocity = _smoothedInput * _moveSpeed;

        if (_smoothedInput.sqrMagnitude > 0.01f)
        {
            var rot = Quaternion.LookRotation(_smoothedInput);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, _rotationSpeed * Time.fixedDeltaTime);
        }

        OnSpeedChanged?.Invoke(_rb.linearVelocity.magnitude / _moveSpeed);
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
