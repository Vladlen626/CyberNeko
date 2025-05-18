using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public class AIKnowledge : MonoBehaviour
{
    [FormerlySerializedAs("TargetOnVision")] [Header("Target")]
    public bool IsTargetOnVision; 
    public bool IsTargetClose;
    public Transform Target;
    
    [Header("Alert")]
    [Range(0,1)]
    public ReactiveProperty<float> AlertProgress = new ReactiveProperty<float>(0);
    
    public bool IsAlerted;
}
