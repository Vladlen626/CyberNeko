using System;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PlayerMovementController))]
public class Hider : MonoBehaviour
{
    public bool IsHiding { get; private set; }

    private PlayerMovementController _movement;

    public void SetHiding(bool state)
    {
        if (IsHiding == state) return;
        IsHiding = state;
        _movement.SetMovementBlocked(state);
    }

    // _____________ Private _____________
    
    private void Awake()
    {
        _movement = GetComponent<PlayerMovementController>();
    }
    

}
