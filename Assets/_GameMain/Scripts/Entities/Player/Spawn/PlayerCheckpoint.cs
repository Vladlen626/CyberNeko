using System;
using UnityEngine;

public class PlayerCheckpoint : MonoBehaviour
{
    public Action<PlayerCheckpoint> OnActivate;
    
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private GameObject pillar;
    [SerializeField] private bool isActive;

    public void Initialize()
    {
        if (!isActive)
        {
            DeactivateCheckpoint();
        }
    }
    
    public void DeactivateCheckpoint()
    {
        isActive = false;
        pillar.SetActive(isActive);
    }

    public void ActivateCheckpoint()
    {
        if (isActive) return;
        AudioManager.inst.PlaySound(SoundNames.ScaredMeow_2);
        isActive = true;
        pillar.SetActive(isActive);
        OnActivate.Invoke(this);
    }

    public Vector3 GetSpawnPosition()
    {
        return spawnTransform.position;
    }

    public bool IsActive()
    {
        return isActive;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateCheckpoint();
        }
    }
}
