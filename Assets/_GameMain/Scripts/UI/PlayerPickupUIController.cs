using System;
using UnityEngine;

[RequireComponent(typeof(PlayerThrower))]
public class PlayerPickupUIController : MonoBehaviour
{
    private ThrowCandidateSelector _candidateSelector;
    private PickupMarker _pickupMarker;

    public void Initialize(PickupMarker pickupMarker)
    {
        _pickupMarker = pickupMarker;
    }

    // _____________ Private _____________

    private void Awake()
    {
        _candidateSelector = transform.GetComponent<ThrowCandidateSelector>();
    }

    private void OnEnable()
    {
        _candidateSelector.PickupCandidateChanged += OnPickupCandidateChanged;
    }

    private void OnDisable()
    {
        _candidateSelector.PickupCandidateChanged -= OnPickupCandidateChanged;
    }

    private void OnPickupCandidateChanged(Transform candidate)
    {
        if (candidate != null)
            _pickupMarker.Show(candidate);
        else
            _pickupMarker.Hide();
    }
}
