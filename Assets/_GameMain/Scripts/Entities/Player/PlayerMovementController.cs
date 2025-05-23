using System;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour
{
    public event Action<float> OnSpeedChanged;
    public event Action<bool> OnGrabbedChanged;

    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _rotationSpeed = 12f;
    [SerializeField] private float _acceleration = 24f;
    [SerializeField] private float _inputSmooth = 10f;

    private Rigidbody _rb;
    private Transform _cameraTransform;
    private Vector3 _input;
    private Vector3 _smoothedInput;
    private bool _grabbed;
    private IInputService _inputService;

    [Inject]
    public void Construct(IInputService inputService)
    {
        _inputService = inputService;
    }

    public void Initialize(Transform cameraTransform)
    {
        _cameraTransform = cameraTransform;
    }

    public void Respawn()
    {
        _grabbed = false;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        OnGrabbedChanged?.Invoke(false);
    }

    public void Grabbed()
    {
        _grabbed = true;
        _rb.linearVelocity = Vector3.zero;
        OnGrabbedChanged?.Invoke(true);
    }

    // _____________ Private _____________

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        _rb.useGravity = false;
    }

    private void Update()
    {
        if (_grabbed) return;

        float h = _inputService?.GetHorizontal() ?? Input.GetAxisRaw("Horizontal");
        float v = _inputService?.GetVertical() ?? Input.GetAxisRaw("Vertical");

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
        if (_grabbed) return;

        var targetVel = _smoothedInput * _moveSpeed;
        _rb.linearVelocity = Vector3.Lerp(_rb.linearVelocity, targetVel, _acceleration * Time.fixedDeltaTime);

        if (_smoothedInput.sqrMagnitude > 0.01f)
        {
            var rot = Quaternion.LookRotation(_smoothedInput);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, _rotationSpeed * Time.fixedDeltaTime);
        }

        OnSpeedChanged?.Invoke(_rb.linearVelocity.magnitude / _moveSpeed);
    }
}
