using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _baseSpeed = 3.5f;
    [SerializeField] private float _chaseSpeedMultiplier = 1.5f;
    
    private NavMeshAgent _agent;
    private CancellationTokenSource _movementCTS;
    private float _originalStoppingDistance;
    
    public async UniTask MoveToPoint(Vector3 point, float customStoppingDistance = -1, CancellationToken externalToken = default)
    {
        CancelCurrentMovement();
        
        try
        {
            var combinedToken = CancellationTokenSource.CreateLinkedTokenSource(_movementCTS.Token, externalToken).Token;

            ConfigureStoppingDistance(customStoppingDistance);
            _agent.SetDestination(point);

            await UniTask.WaitUntil(() => 
                IsDestinationReached(point, customStoppingDistance),
                cancellationToken: combinedToken
            );
        }
        catch (System.OperationCanceledException)
        {
            HandleMovementCancel();
            throw;
        }
    }
    
    public void UpdateDestination(Vector3 newPoint)
    {
        if (!_agent.enabled) return;
        
        if (_agent.destination != newPoint)
        {
            _agent.SetDestination(newPoint);
        }
    }
    
    public void StopMovement()
    {
        CancelCurrentMovement();
        _agent.ResetPath();
    }

    public void LookAt(Transform target)
    {
        transform.DOLookAt(target.position, 0.25f);
    }
    
    public void SetSpeedMode(bool isChasing)
    {
        _agent.speed = isChasing ? _baseSpeed * _chaseSpeedMultiplier : _baseSpeed;
    }

    public CancellationToken GetMovementToken()
    {
        return _movementCTS?.Token ?? CancellationToken.None;
    }

    // _____________ Private _____________

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _originalStoppingDistance = _agent.stoppingDistance;
        
        ConfigureAgent();
    }
    
    private void ConfigureAgent()
    {
        SetSpeedMode(false);
        _agent.stoppingDistance = _originalStoppingDistance;
    }

    private void ConfigureStoppingDistance(float customDistance)
    {
        _agent.stoppingDistance = customDistance >= 0 ? 
            customDistance : 
            _originalStoppingDistance;
    }

    private bool IsDestinationReached(Vector3 point, float customDistance)
    {
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            return Vector3.Distance(point, transform.position) <= 
                (customDistance >= 0 ? customDistance : _originalStoppingDistance);
        }
        return false;
    }

    private void CancelCurrentMovement()
    {
        _movementCTS?.Cancel();
        _movementCTS?.Dispose();
        _movementCTS = new CancellationTokenSource();
    }

    private void HandleMovementCancel()
    {
        _agent?.ResetPath();
        ConfigureStoppingDistance(-1);
    }

    private void OnDestroy()
    {
        _movementCTS?.Cancel();
        _movementCTS?.Dispose();
    }
}