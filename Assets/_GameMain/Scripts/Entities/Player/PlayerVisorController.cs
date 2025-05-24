using System;
using UnityEngine;

[RequireComponent(typeof(VisorController), typeof(PlayerStealthStatus))]
public class PlayerVisorController : MonoBehaviour
{
    private VisorController _visorController;
    private PlayerStealthStatus _playerStealthStatus;

    private void Awake()
    {
        _visorController = transform.GetComponent<VisorController>();
        _playerStealthStatus = transform.GetComponent<PlayerStealthStatus>();
        _playerStealthStatus.OnChaseStateChanged += ChangeVisorToScared;
    }

    private void OnDestroy()
    {
        _playerStealthStatus.OnChaseStateChanged -= ChangeVisorToScared;
    }
    
    private void ChangeVisorToScared(bool isScared)
    {
        var visorNum = isScared ? PlayerVisorsEnum.Scared : PlayerVisorsEnum.Default;
        _visorController.ChooseVisor((int)visorNum);
    }
}