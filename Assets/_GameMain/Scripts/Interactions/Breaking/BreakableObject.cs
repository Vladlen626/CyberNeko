using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class BreakableObject : MonoBehaviour, IBreakable
{
    [Header("Breaking")]
    [SerializeField] private float _minBreakVelocity = 6f;
    
    [Header("Loot")]
    [SerializeField] private int _foodCount = 0;
    [SerializeField] private float _lootScatter = 1.8f;

    private Rigidbody _rb;
    private Collider _collider;
    private bool _isBroken;
    private FoodSpawner _foodSpawner;
    private BreakablesManager _breakablesManager;

    private Vector3 _startPosition;
    private Quaternion _startRotation;

    [Inject]
    private void Construct(FoodSpawner foodSpawner, BreakablesManager breakablesManager)
    {
        _foodSpawner = foodSpawner;
        _breakablesManager = breakablesManager;
        Register();
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
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
        
        for (int i = 0; i < _foodCount; i++)
        {
            Vector3 scatter = Random.onUnitSphere; scatter.y = Mathf.Abs(scatter.y);
            Vector3 spawnPos = transform.position + scatter * Random.Range(0.3f, _lootScatter);
            _foodSpawner.DropFood(spawnPos);
        }

        _collider.enabled = false;
        _rb.isKinematic = true;
        gameObject.SetActive(false);
    }

    public void ResetState()
    {
        _isBroken = false;
        transform.position = _startPosition;
        transform.rotation = _startRotation;
        _collider.enabled = true;
        _rb.isKinematic = false;
        gameObject.SetActive(true);
    }

    public Transform GetTransform() => transform;

    private void OnCollisionEnter(Collision collision)
    {
        if (_isBroken) return;
        if (collision.gameObject.CompareTag("Player"))
            return;
        if (collision.relativeVelocity.magnitude >= _minBreakVelocity)
            Break();
    }
}
