using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(VisorController), typeof(NavMeshAgent))]
public class ScaredNPC : MonoBehaviour
{
    [Header("Wander Settings")]
    [Tooltip("Maximum random walk radius (in meters)")]
    [SerializeField] private float wanderRadius = 10f;

    [SerializeField] private float minWanderTime = 3f;
    [SerializeField] private float maxWanderTime = 7f;
    [SerializeField] private float minWanderDistance = 3f;
    [SerializeField] private float idleTimeMin = 1f;
    [SerializeField] private float idleTimeMax = 3f;

    [Header("Fear Settings")]
    [Tooltip("How far does an NPC run from the player")]
    [SerializeField] private float fleeDistance = 8f;

    [Tooltip("How many times does the speed increase when escaping")]
    [SerializeField] private float fleeSpeedMultiplier = 1.5f;
    [SerializeField] private float calmDownTime = 2f;

    private NavMeshAgent _agent;
    private float _originalSpeed;
    private Transform _player;
    private bool _playerInRange;
    private Coroutine _behaviorCoroutine;
    
    private FoodDropper _foodDropper;
    private VisorController visorController;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _originalSpeed = _agent.speed;
        
        _foodDropper = GetComponent<FoodDropper>();
        if (_foodDropper == null)
        {
            Debug.LogError("ScaredNPC must have FoodDroper script!");
        }

        visorController = GetComponent<VisorController>();
    }

    public void Initialize(Transform playerTransform)
    {
        
    }
    
    void OnEnable()
    {
        _behaviorCoroutine = StartCoroutine(BehaviorStateMachine());
    }

    void OnDisable()
    {
        if (_behaviorCoroutine != null)
            StopCoroutine(_behaviorCoroutine);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _player = other.transform;
            _playerInRange = true;
            _foodDropper.TryDropFood();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
        }
    }

    IEnumerator BehaviorStateMachine()
    {
        while (true)
        {
            if (_playerInRange)
            {
                yield return StartCoroutine(FleeBehavior());
            }
            else
            {
                yield return StartCoroutine(WanderBehavior());
            }
        }
    }

    IEnumerator WanderBehavior()
    {
        visorController.ChooseVisor(0);
        float wanderTime = Random.Range(minWanderTime, maxWanderTime);
        float elapsedTime = 0f;

        while (elapsedTime < wanderTime && !_playerInRange)
        {
            if (_agent.remainingDistance < 0.5f)
            {
                Vector3 newPos = GetRandomNavMeshPosition(transform.position, wanderRadius, minWanderDistance);
                if (_agent.isOnNavMesh)
                {
                    _agent.SetDestination(newPos);
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (!_playerInRange)
        {
            yield return new WaitForSeconds(Random.Range(idleTimeMin, idleTimeMax));
        }
    }

    IEnumerator FleeBehavior()
    {
        visorController.ChooseVisor(1);
        _agent.speed = _originalSpeed * fleeSpeedMultiplier;
        float calmDownTimer = calmDownTime;

        while (_playerInRange || calmDownTimer > 0)
        {
            if (_playerInRange)
            {
                calmDownTimer = calmDownTime;
                Vector3 fleeDirection = (transform.position - _player.position).normalized;
                Vector3 fleePosition = transform.position + fleeDirection * fleeDistance;

                if (NavMesh.SamplePosition(fleePosition, out NavMeshHit hit, fleeDistance, NavMesh.AllAreas))
                {
                    if (_agent.isOnNavMesh)
                    {
                        _agent.SetDestination(hit.position);
                    }
                }
            }
            else
            {
                calmDownTimer -= Time.deltaTime;
            }

            yield return null;
        }

        _agent.speed = _originalSpeed;

        // Гарантированный возврат на NavMesh
        if (!_agent.isOnNavMesh)
        {
            NavMeshHit closestHit;
            if (NavMesh.SamplePosition(transform.position, out closestHit, 5f, NavMesh.AllAreas))
            {
                _agent.Warp(closestHit.position);
            }
        }
    }

    Vector3 GetRandomNavMeshPosition(Vector3 origin, float radius, float minDistance)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += origin;
            randomDirection.y = origin.y;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, NavMesh.AllAreas))
            {
                if (Vector3.Distance(origin, hit.position) > minDistance)
                {
                    return hit.position;
                }
            }
        }
        return origin;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);

        if (_playerInRange)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + Vector3.up * 2, 0.3f);
        }
    }

}