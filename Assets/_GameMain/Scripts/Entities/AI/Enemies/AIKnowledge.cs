using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public class AIKnowledge : MonoBehaviour
{
    [Header("Target")]
    public bool IsTargetOnVision; 
    public float DistanceToTarget = 999;
    public Transform Target;
    
    [Header("Alert")]
    [Range(0,1)]
    public ReactiveProperty<float> AlertProgress = new ReactiveProperty<float>(0);
    public bool IsAlerted;
}
