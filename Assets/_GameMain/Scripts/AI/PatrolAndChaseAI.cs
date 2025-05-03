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
    private int curPatrolPointIndex = 0;

    private bool isChasing   = false;
    private bool isWaiting   = false;
    private float waitTimer  = 0f;
    private float chaseTimer = 0f;

    private Transform targetTransform = null;
    private Transform[] patrolPoints;

    private StealthStatus lastChasedPlayerStealthStatus = null;
    private VisorController visorController;

    public void Initialize(Transform[] inPatrolPoints)
    {
        exclamationMarker.SetActive(false);
        agent = GetComponent<NavMeshAgent>();
        visorController = GetComponent<VisorController>();
        patrolPoints = inPatrolPoints;
        targetTransform = null;
        if (patrolPoints.Length > 0)
        {
            agent.destination = patrolPoints[curPatrolPointIndex].position;
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
        if (isChasing && targetTransform)
        {
            // Continue chasing even if player is not in line of sight
            agent.destination = targetTransform.position;

            // Check distance to stop chasing
            float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);
            if (distanceToTarget > chaseStopDistance)
            {
                chaseTimer += Time.deltaTime;
                if (chaseTimer >= timeBeforeReturnToPatrol)
                {
                    StopChasing();
                }
            }
            else
            {
                chaseTimer = 0f; // Reset timer if player is close enough
            }
            return;
        }

        if (patrolPoints == null)
            return;

        // Patrolling: move to the next point if it had reached the current one
        if (!isWaiting && patrolPoints.Length > 0 && !agent.pathPending && agent.remainingDistance <= reachThreshold)
        {
            isWaiting = true;
            waitTimer = timeWaitOnPatrolPoint;
        }

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                curPatrolPointIndex = (curPatrolPointIndex + 1) % patrolPoints.Length;
                agent.destination = patrolPoints[curPatrolPointIndex].position;
            }
        }
    }
    
    private void StartChasing(GameObject playerObj)
    {
        exclamationMarker.SetActive(true);
        visorController.ChooseVisor(1);
        StealthStatus stealthStatus = playerObj.GetComponent<StealthStatus>();
        Assert.IsNotNull(stealthStatus, $"{playerObj.name} need StealthStatus");
        lastChasedPlayerStealthStatus = stealthStatus;
        stealthStatus.AddToPursuer(gameObject);
        isChasing = true;
        isWaiting = false;
        chaseTimer = 0f;
    }

    private void StopChasing()
    {
        exclamationMarker.SetActive(false);
        visorController.ChooseVisor(0);
        lastChasedPlayerStealthStatus?.RemoveFromPursuer(gameObject);

        isChasing = false;
        targetTransform = null;
        chaseTimer = 0f;

        // Return to patrol
        if (patrolPoints.Length > 0)
        {
            agent.destination = patrolPoints[curPatrolPointIndex].position;
        }
    }

    IEnumerator PlayerDetectionRoutine()
    {
        while (true)
        {
            if (!isChasing) // Only check for player when not already chasing
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
                targetTransform = hit.transform;
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
