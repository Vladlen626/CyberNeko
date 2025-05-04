using System;
using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    [SerializeField] private PatrolAndChaseAI _patrolAndChaseAI;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var playerController = other.GetComponent<PlayerController>();
            playerController.Grabbed();
        }
    }
}
