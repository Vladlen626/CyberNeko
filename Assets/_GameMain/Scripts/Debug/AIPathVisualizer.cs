#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[ExecuteInEditMode]
public class AIPathVisualizer : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] private bool showPath = true;
    [SerializeField] private bool showVelocity = true;
    [SerializeField] private bool realtimeUpdate = true;
    
    [Space]
    [SerializeField] private Color pathColor = new Color(0, 1, 0, 0.5f);
    [SerializeField] private Color cornersColor = Color.red;
    [SerializeField] private Color velocityColor = Color.cyan;
    
    [Header("Display Settings")]
    [SerializeField] [Range(0.1f, 1f)] private float cornerSphereRadius = 0.3f;
    [SerializeField] private float pathHeightOffset = 0.1f;
    [SerializeField] private float velocityArrowLength = 2f;

    private NavMeshAgent _agent;
    private Vector3[] _pathCorners = new Vector3[0];

    private void Awake()
    {
        TryGetComponent(out _agent);
    }

    private void Update()
    {
        if (!Application.isPlaying) return;
        
        if (realtimeUpdate && showPath && _agent != null && _agent.hasPath)
        {
            _pathCorners = _agent.path.corners;
        }
    }

    private void OnDrawGizmos()
    {
        if (!showPath || !enabled) return;

        DrawPath();
        DrawAdditionalInfo();
    }

    private void DrawPath()
    {
        if (_pathCorners == null || _pathCorners.Length < 2) return;

        Gizmos.color = pathColor;
        Vector3 prevPoint = Vector3.zero;

        for (int i = 0; i < _pathCorners.Length; i++)
        {
            Vector3 currentPoint = _pathCorners[i] + Vector3.up * pathHeightOffset;
            
            // Рисуем угловые точки
            Gizmos.color = cornersColor;
            Gizmos.DrawSphere(currentPoint, cornerSphereRadius);

            // Рисуем линии пути
            if (i > 0)
            {
                Gizmos.color = pathColor;
                Gizmos.DrawLine(prevPoint, currentPoint);
            }

            prevPoint = currentPoint;
        }
    }

    private void DrawAdditionalInfo()
    {
        if (!Application.isPlaying) return;

        // Рисуем направление движения
        if (showVelocity && _agent != null && _agent.velocity != Vector3.zero)
        {
            Gizmos.color = velocityColor;
            Vector3 start = transform.position + Vector3.up * pathHeightOffset;
            Vector3 direction = _agent.velocity.normalized * velocityArrowLength;
            Gizmos.DrawRay(start, direction);
            
            // Стрелка
            DrawArrow(start, direction);
        }

        // Рисуем зону остановки
        if (_agent != null && _agent.hasPath)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_agent.destination, _agent.stoppingDistance);
        }
    }

    private void DrawArrow(Vector3 position, Vector3 direction)
    {
        float arrowHeadAngle = 20f;
        float arrowHeadLength = 0.5f;
        
        Vector3 right = Quaternion.LookRotation(direction) * 
                        Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * 
                        Vector3.forward;
        
        Vector3 left = Quaternion.LookRotation(direction) * 
                       Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * 
                       Vector3.forward;
        
        Gizmos.DrawRay(position + direction, right * arrowHeadLength);
        Gizmos.DrawRay(position + direction, left * arrowHeadLength);
    }

    // Автоматическое удаление в билде
#if !UNITY_EDITOR
    private void Start()
    {
        Destroy(this);
    }
#endif
}
#endif