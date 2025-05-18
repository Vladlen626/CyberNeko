using UnityEngine;
using UnityEngine.Serialization;

public class WorldState : MonoBehaviour
{
    [FormerlySerializedAs("TargetOnVision")] [Header("Target")]
    public bool IsTargetOnVision; 
    public bool IsTargetClose;
    public Transform Target;
    
    [Header("Alert")]
    [Range(0,1)] public float AlertProgress;
    public bool IsAlerted;
}
