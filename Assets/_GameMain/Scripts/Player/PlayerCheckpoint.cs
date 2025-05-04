using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCheckpoint : MonoBehaviour
{
    public Action<PlayerCheckpoint> OnActivate;
    
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private GameObject pillar;

    public bool isActive;

    public void DeactivateCheckpoint()
    {
        isActive = false;
        pillar.SetActive(isActive);
    }

    public void ActivateCheckpoint()
    {
        isActive = true;
        pillar.SetActive(isActive);
    }

    public Vector3 GetSpawnPosition()
    {
        return spawnTransform.position;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnActivate.Invoke(this);
        }
    }
}
