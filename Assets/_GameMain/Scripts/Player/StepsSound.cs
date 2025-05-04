using UnityEngine;

public class StepsSound : MonoBehaviour
{
    [SerializeField] private string SoundName;
    
    public void StepSound()
    {
        AudioManager.inst.PlaySound(SoundName);
    }
}
