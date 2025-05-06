using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using static UnityEngine.EventSystems.EventTrigger;

[RequireComponent(typeof(VisorController), typeof(NavMeshAgent))]
public class PatrolAndChaseAI : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float fieldOfView = 120f;
    [SerializeField] private float checkInterval = 0.3f;

    [SerializeField] private GameObject exclamationMarker;

    // AI will lose sight of the target if the distance between them is this value
    [SerializeField] private float chaseStopDistance = 20f;

    [SerializeField] private LayerMask detectionMask;

    [Header("Patrol")]
    [SerializeField] private float timeWaitOnPatrolPoint = 4f;
    [SerializeField] private float timeBeforeReturnToPatrol = 3f;
    
    // Distance at which the AI considers it has reached a patrol point
    [SerializeField] private float reachThreshold = 1.0f;

    private NavMeshAgent agent;
    private int curPatrolPointIndex;

    private bool _isChasing;
    private bool _isWaiting;
    private float _waitTimer;
    private float _chaseTimer;

    private Transform _targetTransform;
    private Transform[] _patrolPoints;

    private StealthStatus _lastChasedPlayerStealthStatus;
    private VisorController _visorController;

    public void Initialize(Transform[] inPatrolPoints)
    {
        exclamationMarker.SetActive(false);
        agent = GetComponent<NavMeshAgent>();
        _visorController = GetComponent<VisorController>();
        _patrolPoints = inPatrolPoints;
        _targetTransform = null;
        _isChasing = false;
        if (_patrolPoints.Length > 0)
        {
            agent.destination = _patrolPoints[curPatrolPointIndex].position;
        }
    }

    public void StartPlayerDetection()
    {
        StartCoroutine(PlayerDetectionRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    void Update()
    {
        if (_isChasing && _targetTransform)
        {
            // Continue chasing even if player is not in line of sight
            agent.destination = _targetTransform.position;

            // Check distance to stop chasing
            float distanceToTarget = Vector3.Distance(transform.position, _targetTransform.position);
            if (distanceToTarget > chaseStopDistance)
            {
                _chaseTimer += Time.deltaTime;
                if (_chaseTimer >= timeBeforeReturnToPatrol)
                {
                    StopChasing();
                }
            }
            else
            {
                _chaseTimer = 0f; // Reset timer if player is close enough
            }
            return;
        }

        if (_patrolPoints == null)
            return;

        // Patrolling: move to the next point if it had reached the current one
        if (!_isWaiting && _patrolPoints.Length > 0 && !agent.pathPending && agent.remainingDistance <= reachThreshold)
        {
            _isWaiting = true;
            _waitTimer = timeWaitOnPatrolPoint;
        }

        if (_isWaiting)
        {
            _waitTimer -= Time.deltaTime;
            if (_waitTimer <= 0f)
            {
                _isWaiting = false;
                curPatrolPointIndex = (curPatrolPointIndex + 1) % _patrolPoints.Length;
                agent.destination = _patrolPoints[curPatrolPointIndex].position;
            }
        }
    }
    
    private void StartChasing(GameObject playerObj)
    {
        Alert();
        StealthStatus stealthStatus = playerObj.GetComponent<StealthStatus>();
        //Assert.IsNotNull(stealthStatus, $"{playerObj.name} need StealthStatus");
        _lastChasedPlayerStealthStatus = stealthStatus;
        stealthStatus.AddToPursuer(gameObject);
        _isChasing = true;
        _isWaiting = false;
        _chaseTimer = 0f;
    }

    private void Alert()
    {
        AudioManager.inst.PlaySound(SoundNames.Alert);
        exclamationMarker.SetActive(true);
        _visorController.ChooseVisor(1);
    }

    private void StopChasing()
    {
        exclamationMarker.SetActive(false);
        _visorController.ChooseVisor(0);
        _lastChasedPlayerStealthStatus?.RemoveFromPursuer(gameObject);

        _isChasing = false;
        _targetTransform = null;
        _chaseTimer = 0f;

        // Return to patrol
        if (_patrolPoints.Length > 0)
        {
            agent.destination = _patrolPoints[curPatrolPointIndex].position;
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator PlayerDetectionRoutine()
    {
        while (true)
        {
            if (!_isChasing) // Only check for player when not already chasing
            {
                SearchForPlayerUsingOverlapSphere();
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }

    private void SearchForPlayerUsingOverlapSphere()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange, detectionMask);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Player"))
                continue;

            // Before checking the angle, subtract the AI position to avoid height differences
            Vector3 directionToTarget = (hit.transform.position - transform.position).normalized;

            // Checking the angle between the direction of view and the direction towards the player
            if (Vector3.Angle(transform.forward, directionToTarget) >= fieldOfView * 0.5f)
                continue;

            if (HasLineOfSight(hit.transform))
            {
                _targetTransform = hit.transform;
                StartChasing(hit.gameObject);
                break;
            }
        }
    }

    private bool HasLineOfSight(Transform target)
    {
        Vector3 origin = transform.position + Vector3.up; // Raise the beam so as not to "cut" the ground
        Vector3 direction = (target.position - origin).normalized;
        Ray ray = new Ray(origin, direction);
        RaycastHit hit;

        return Physics.Raycast(ray, out hit, detectionRange) && (hit.transform == target);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView * 0.5f, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView * 0.5f, 0) * transform.forward;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * detectionRange);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * detectionRange);
    }
}
