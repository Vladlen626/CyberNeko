using System;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PlayerMovementController))]
public class Hider : MonoBehaviour
{
    private bool _isHiding;

    private PlayerMovementController _movement;

    public void SetHiding(bool state)
    {
        if (_isHiding == state) return;
        _isHiding = state;
        _movement.SetMovementBlocked(state);
    }

    // _____________ Private _____________
    
    private void Awake()
    {
        _movement = GetComponent<PlayerMovementController>();
    }
    

}
