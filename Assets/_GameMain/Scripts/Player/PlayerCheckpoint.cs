using UnityEngine;

public class PlayerCheckpoint : MonoBehaviour
{
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private GameObject pillar;

    private bool isActive;

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
}
