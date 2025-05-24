using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PlayerMovementController), typeof(PlayerStealthStatus))]
public class Hider : MonoBehaviour
{
    [SerializeField] private Transform _alertMark;
    
    private bool _isHiding;
    private PlayerMovementController _movement;
    private PlayerStealthStatus _stealthStatus;

    public void SetHiding(bool state)
    {
        if (_isHiding == state) return;
        _isHiding = state;
        _movement.SetMovementBlocked(state);
        if (state && _stealthStatus.IsChased())
        {
            ShowAlertMark();
        }
        else
        {
            HideAlertMark();
        }
    }
    

    // _____________ Private _____________
    
    private void Awake()
    {
        _movement = GetComponent<PlayerMovementController>();
        _stealthStatus = GetComponent<PlayerStealthStatus>();
        _alertMark.gameObject.SetActive(true);
        _alertMark.localScale = Vector3.zero;
    }

    private void ShowAlertMark()
    {
        _alertMark.DOScale(1, 0.15f);
    }

    private void HideAlertMark()
    {
        _alertMark.DOScale(0, 0.15f);
    }
    

}
