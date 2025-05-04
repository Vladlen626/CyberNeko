using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCheckpoint : MonoBehaviour
{
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private GameObject pillar;

    public bool isActive;

    public void DeactivateCheckpoint()
    {
        isActive = false;
        pillar.SetActive(isActive);
    }

    private void ActivateCheckpoint()
    {
        isActive = true;
        pillar.SetActive(isActive);
    }

    public Vector3 GetSpawnPosition()
    {
        return spawnTransform.position;
    }
}
