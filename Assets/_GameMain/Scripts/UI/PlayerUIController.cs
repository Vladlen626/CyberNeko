using System;
using UnityEngine;

[RequireComponent(typeof(PlayerThrower))]
public class PlayerUIController : MonoBehaviour
{
    private PlayerThrower _playerThrower;
    private PickupMarker _pickupMarker;

    public void Initialize(PickupMarker pickupMarker)
    {
        _pickupMarker = pickupMarker;
    }

    // _____________ Private _____________

    private void Awake()
    {
        _playerThrower = transform.GetComponent<PlayerThrower>();
    }

    private void OnEnable()
    {
        _playerThrower.PickupCandidateChanged += OnPickupCandidateChanged;
    }

    private void OnDisable()
    {
        _playerThrower.PickupCandidateChanged -= OnPickupCandidateChanged;
    }

    private void OnPickupCandidateChanged(Transform candidate)
    {
        if (candidate != null)
            _pickupMarker.Show(candidate);
        else
            _pickupMarker.Hide();
    }
}
