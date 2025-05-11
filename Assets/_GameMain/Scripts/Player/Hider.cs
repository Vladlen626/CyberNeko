using System;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
public class Hider : MonoBehaviour
{
    [SerializeField] private GameObject _alertMarker;
    public bool IsHiding { get; private set; }

    private NavMeshAgent _navMeshAgent;

    public void SetHiding(bool state)
    {
        if (IsHiding == state) return;
        
        IsHiding = state;
        _navMeshAgent.enabled = !state;
    }

    // _____________ Private _____________
    
    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }
    
    private void ShowMarker()
    {
        AudioManager.inst.PlaySound(SoundNames.Alert);
        _alertMarker.SetActive(true);
    }

    private void HideMarker()
    {
        _alertMarker.SetActive(false);
    }

}
