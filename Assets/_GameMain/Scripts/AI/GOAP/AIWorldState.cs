using UnityEngine;

public class AIWorldState : MonoBehaviour
{
    [Header("Detection State")]
    public bool PlayerVisible;
    public bool AtPatrolPoint;
    public Vector3 LastKnownPlayerPosition;
    
    [Header("Alert State")]
    public float AlertLevel;
    public bool IsInFullAlert;
}
