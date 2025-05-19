using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(AlertSystem))]
public class AIVisionSensor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _scanRange = 15f;
    [SerializeField] private float _fovAngle = 120f;
    [SerializeField] private float _scanInterval = 0.2f;
    
    [Header("Layers")]
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask _obstacleMask;

    private AlertSystem _alertSystem;
    private AIKnowledge _aiKnowledge;

    public void Initialize()
    {
        _aiKnowledge = GetComponent<AIKnowledge>();
        _alertSystem = GetComponent<AlertSystem>();
        _alertSystem.Initialize(_aiKnowledge);
    }

    public void Reset()
    {
        _alertSystem.ResetAlert();
    }

    private void Start() => RunDetection().Forget();

    private async UniTaskVoid RunDetection()
    {
        while (true)
        {
            ScanForTarget();
            
            await UniTask.WaitForSeconds(_scanInterval);
        }
    }

    private void ScanForTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, _scanRange,_targetMask);

        Transform targetTransform = null;
        
        foreach (var target in targets)
        {
            if (!target.CompareTag("Player"))
                continue;

            targetTransform = target.transform;
        }
        
        _aiKnowledge.IsTargetOnVision = IsTargetVisible(targetTransform);
        if (_aiKnowledge.IsTargetOnVision)
        {
            _alertSystem.AddAlert();
        }
        else
        {
            _alertSystem.RemoveAlert();
        }
    }

    private bool IsTargetVisible(Transform targetTransform)
    {
        if (!targetTransform)
            return false;

        _aiKnowledge.Target = targetTransform;
        CalculateDistanceToTarget();
        
        Vector3 dir = (targetTransform.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, dir) > _fovAngle / 2) return false;

        return !Physics.Raycast(
            transform.position + Vector3.up, 
            dir, 
            _scanRange, 
            _obstacleMask
        );
    }
    
    private void CalculateDistanceToTarget()
    {
        _aiKnowledge.DistanceToTarget = Vector3.Distance(transform.position, _aiKnowledge.Target.position);
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _scanRange);
        
        DrawFieldOfView();
    }
    private void DrawFieldOfView()
    {
        Gizmos.color = Color.yellow;
        
        Vector3 forward = transform.forward * _scanRange;
        Quaternion leftRot = Quaternion.Euler(0, -_fovAngle/2, 0);
        Quaternion rightRot = Quaternion.Euler(0, _fovAngle/2, 0);
        
        Vector3 leftBound = leftRot * forward + transform.position;
        Vector3 rightBound = rightRot * forward + transform.position;

        Gizmos.DrawLine(transform.position, leftBound);
        Gizmos.DrawLine(transform.position, rightBound);
        
        DrawViewArc();
    }

    private void DrawViewArc()
    {
        Gizmos.color = new Color(1, 1, 0, 0.2f);
        Vector3 pos = transform.position;
        Vector3 forward = transform.forward;
        
        int segments = 20;
        float angleStep = _fovAngle / segments;
        Vector3 prevPoint = pos + forward * _scanRange;
        
        for (int i = 1; i <= segments; i++)
        {
            float angle = -_fovAngle/2 + angleStep*i;
            Quaternion rot = Quaternion.Euler(0, angle, 0);
            Vector3 dir = rot * forward * _scanRange;
            Vector3 newPoint = pos + dir;
            
            Gizmos.DrawLine(pos, newPoint);
            Gizmos.DrawLine(prevPoint, newPoint);
            
            prevPoint = newPoint;
        }
    }
#endif
}