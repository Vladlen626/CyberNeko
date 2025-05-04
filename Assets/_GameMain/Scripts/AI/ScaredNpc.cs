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

    private NavMeshAgent agent;
    private float originalSpeed;
    private Transform player;
    private bool playerInRange = false;
    private Coroutine behaviorCoroutine;
    
    private FoodDropper _foodDropper;
    private VisorController visorController;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        originalSpeed = agent.speed;
        
        _foodDropper = GetComponent<FoodDropper>();
        if (_foodDropper == null)
        {
            Debug.LogError("ScaredNPC must have FoodDroper script!");
        }

        visorController = GetComponent<VisorController>();
    }

    void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        behaviorCoroutine = StartCoroutine(BehaviorStateMachine());
    }

    void OnDisable()
    {
        if (behaviorCoroutine != null)
            StopCoroutine(behaviorCoroutine);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            _foodDropper.TryDropFood();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    IEnumerator BehaviorStateMachine()
    {
        while (true)
        {
            if (playerInRange)
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

        while (elapsedTime < wanderTime && !playerInRange)
        {
            if (agent.remainingDistance < 0.5f)
            {
                Vector3 newPos = GetRandomNavMeshPosition(transform.position, wanderRadius, minWanderDistance);
                if (agent.isOnNavMesh)
                {
                    agent.SetDestination(newPos);
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (!playerInRange)
        {
            yield return new WaitForSeconds(Random.Range(idleTimeMin, idleTimeMax));
        }
    }

    IEnumerator FleeBehavior()
    {
        visorController.ChooseVisor(1);
        agent.speed = originalSpeed * fleeSpeedMultiplier;
        float calmDownTimer = calmDownTime;

        while (playerInRange || calmDownTimer > 0)
        {
            if (playerInRange)
            {
                calmDownTimer = calmDownTime;
                Vector3 fleeDirection = (transform.position - player.position).normalized;
                Vector3 fleePosition = transform.position + fleeDirection * fleeDistance;

                if (NavMesh.SamplePosition(fleePosition, out NavMeshHit hit, fleeDistance, NavMesh.AllAreas))
                {
                    if (agent.isOnNavMesh)
                    {
                        agent.SetDestination(hit.position);
                    }
                }
            }
            else
            {
                calmDownTimer -= Time.deltaTime;
            }

            yield return null;
        }

        agent.speed = originalSpeed;

        // Гарантированный возврат на NavMesh
        if (!agent.isOnNavMesh)
        {
            NavMeshHit closestHit;
            if (NavMesh.SamplePosition(transform.position, out closestHit, 5f, NavMesh.AllAreas))
            {
                agent.Warp(closestHit.position);
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

        if (playerInRange)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + Vector3.up * 2, 0.3f);
        }
    }

}