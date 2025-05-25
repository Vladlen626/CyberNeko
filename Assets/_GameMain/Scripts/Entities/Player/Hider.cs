using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PlayerController), typeof(PlayerStealthStatus))]
public class Hider : MonoBehaviour
{
    [SerializeField] private Transform _alertMark;
    
    private bool _isHiding;
    private PlayerController _playerController;
    private PlayerStealthStatus _stealthStatus;

    public void SetHiding(bool state)
    {
        if (_isHiding == state) return;
        _isHiding = state;
        if (state)
        {
            _playerController.StateContainer.AddState(PlayerState.InHider);
            if (_stealthStatus.IsChased()) ShowAlertMark();
        }
        else
        {
            _playerController.StateContainer.RemoveState(PlayerState.InHider);
            HideAlertMark();
        }
    }
    

    // _____________ Private _____________
    
    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
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
