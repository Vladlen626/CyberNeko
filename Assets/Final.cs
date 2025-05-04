using System;
using UnityEngine;
using UnityEngine.Events;

public class Final : MonoBehaviour
{
    public UnityEvent finalGame;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            finalGame.Invoke();
        }
    }

}
