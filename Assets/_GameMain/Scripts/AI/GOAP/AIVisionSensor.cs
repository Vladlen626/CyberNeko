using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(AlertSystem))]
public class AIVisionSensor : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float fovAngle = 120f;
    [SerializeField] private float checkInterval = 0.2f;
    [SerializeField] private LayerMask detectionMask;
    
    private AlertSystem _alertSystem;
    private AIWorldState _worldState;

    private void Awake()
    {
        _alertSystem = GetComponent<AlertSystem>();
        _worldState = GetComponent<AIWorldState>();
    }

    private void Start()
    {
       VisionCheckLoop().Forget();
    }

    private async UniTask VisionCheckLoop()
    {
        while (true)
        {
            if (!_worldState.IsInFullAlert) 
                CheckForPlayer();
            
            await UniTask.Delay((int)(checkInterval * 1000));
        }
    }

    private void CheckForPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position, 
            detectionRange, 
            detectionMask
        );

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;
            
            Vector3 dirToPlayer = (hit.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToPlayer) < fovAngle * 0.5f)
            {
                if (HasLineOfSight(hit.transform))
                {
                    _alertSystem.TriggerAlert(hit.transform.position);
                    return;
                }
            }
        }
        
        _alertSystem.ResetAlert();
    }

    private bool HasLineOfSight(Transform target)
    {
        Vector3 origin = transform.position + Vector3.up;
        Vector3 direction = (target.position - origin).normalized;
        return Physics.Raycast(origin, direction, out RaycastHit hit, detectionRange)
               && hit.transform == target;
    }
}