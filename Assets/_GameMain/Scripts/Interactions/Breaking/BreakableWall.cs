using UnityEngine;
using Zenject;

public class BreakableWall : MonoBehaviour, IBreakable
{
    [Header("Breaking")]
    [SerializeField] private float minBreakVelocity = 6f;
    
    private Collider _collider;
    private bool _isBroken;
    private BreakablesManager _breakablesManager;
    private Vector3 _startPosition;
    private Quaternion _startRotation;

    [Inject]
    private void Construct(BreakablesManager breakablesManager)
    {
        _breakablesManager = breakablesManager;
        Register();
    }

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _startPosition = transform.position;
        _startRotation = transform.rotation;
    }

    public void Register()
    {
        _breakablesManager.Register(this);
    }

    public void Break()
    {
        if (_isBroken) return;
        _isBroken = true;
        _collider.enabled = false;
        gameObject.SetActive(false);
    }

    public void Reset()
    {
        _isBroken = false;
        transform.position = _startPosition;
        transform.rotation = _startRotation;
        _collider.enabled = true;
        gameObject.SetActive(true);
    }

    public Transform GetTransform() => transform;

    private void OnCollisionEnter(Collision collision)
    {
        if (_isBroken) return;
        if (collision.gameObject.CompareTag("Player"))
            return;
        if (collision.relativeVelocity.magnitude >= minBreakVelocity)
            Break();
    }
}
