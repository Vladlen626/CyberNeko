using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using System.Threading;
using Zenject;

[RequireComponent(typeof(NavMeshAgent), typeof(VisorController))]
public class ScaredNPC : MonoBehaviour
{
    [Header("Wander Settings")]
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float minWanderTime = 3f;
    [SerializeField] private float maxWanderTime = 7f;
    [SerializeField] private float minWanderDistance = 3f;
    [SerializeField] private float idleTimeMin = 1f;
    [SerializeField] private float idleTimeMax = 3f;

    [Header("Fear Settings")]
    [SerializeField] private float fleeDistance = 8f;
    [SerializeField] private float fleeSpeedMultiplier = 1.5f;
    [SerializeField] private float calmDownTime = 2f;
    
    private NavMeshAgent _agent;
    private VisorController _visor;
    private float _baseSpeed;
    private Transform _player;
    private CancellationTokenSource _cts;
    private bool _isScared;

    private FoodSpawner _foodSpawner;

    public void Initialize(FoodSpawner foodSpawner)
    {
        _foodSpawner = foodSpawner;
        RestartLoop();
    }

    // _____________ Private _____________

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _visor = GetComponent<VisorController>();
        _baseSpeed = _agent.speed;
    }

    private void OnEnable()
    {
        RestartLoop();
    }

    private void OnDisable()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
    
    private void RestartLoop()
    {
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
        NpcLoop(_cts.Token).Forget();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _player = other.transform;
        _isScared = true;
        _cts?.Cancel();
        RestartLoop();
        _foodSpawner.DropFood(transform.position + Vector3.up * 0.5f);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _isScared = false;
    }

    private async UniTaskVoid NpcLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            if (_isScared && _player != null)
                await FearRoutine(ct);
            else
                await WanderRoutine(ct);

            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }
    }

    private async UniTask FearRoutine(CancellationToken ct)
    {
        _visor.ChooseVisor(1);
        _agent.speed = _baseSpeed * fleeSpeedMultiplier;
        float calm = calmDownTime;

        while ((_isScared || calm > 0f) && !ct.IsCancellationRequested)
        {
            if (_isScared && _player != null)
            {
                calm = calmDownTime;
                Vector3 fleeDir = (transform.position - _player.position).normalized;
                Vector3 fleeTarget = transform.position + fleeDir * fleeDistance;
                SetAgentDestinationSafe(fleeTarget, fleeDistance);
            }
            else
                calm -= Time.deltaTime;

            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }
        _agent.speed = _baseSpeed;
    }

    private async UniTask WanderRoutine(CancellationToken ct)
    {
        _visor.ChooseVisor(0);
        float wanderTime = Random.Range(minWanderTime, maxWanderTime);
        float elapsed = 0f;

        while (elapsed < wanderTime && !_isScared && !ct.IsCancellationRequested)
        {
            if (_agent.remainingDistance < 0.5f)
                SetAgentDestinationSafe(GetRandomNavMeshPosition(transform.position, wanderRadius, minWanderDistance), wanderRadius);

            elapsed += Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }

        if (!_isScared)
        {
            float idle = Random.Range(idleTimeMin, idleTimeMax);
            await UniTask.Delay(System.TimeSpan.FromSeconds(idle), cancellationToken: ct);
        }
    }

    private void SetAgentDestinationSafe(Vector3 pos, float maxDist)
    {
        if (_agent.isOnNavMesh
            && NavMesh.SamplePosition(pos, out NavMeshHit hit, maxDist, NavMesh.AllAreas))
            _agent.SetDestination(hit.position);
    }

    private Vector3 GetRandomNavMeshPosition(Vector3 origin, float radius, float minDistance)
    {
        for (int i = 0; i < 15; i++)
        {
            Vector3 randomDir = Random.insideUnitSphere * radius + origin;
            randomDir.y = origin.y;
            if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, radius, NavMesh.AllAreas)
                && Vector3.Distance(origin, hit.position) > minDistance)
                return hit.position;
        }
        return origin;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);

        if (_isScared)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + Vector3.up * 2, 0.3f);
        }
    }
}
