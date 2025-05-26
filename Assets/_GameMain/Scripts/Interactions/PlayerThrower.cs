using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;
// ReSharper disable Unity.PerformanceCriticalCodeInvocation

public class PlayerThrower : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private float _throwForce = 8f;
    [SerializeField] private float _pickupAngle = 60f;
    [SerializeField] private LayerMask _throwables;

    public event Action<Transform> PickupCandidateChanged;

    private readonly List<IThrowable> _candidates = new();
    private IThrowable _held;
    private IThrowable _best;
    private IInputService _input;

    [Inject]
    public void Construct(IInputService input)
    {
        _input = input;
    }

    private void Start()
    {
        SearchBestLoop().Forget();
    }

    private void Update()
    {
        if (_playerController.StateContainer.HasState(PlayerState.InInteract))
            return;

        if (_input.IsInteractPressed())
        {
            if (_held == null)
                TryPickup(_best).Forget();
            else
                TryThrow(transform.forward).Forget();
        }
    }

    // _____________ Private _____________

    private async UniTaskVoid SearchBestLoop()
    {
        IThrowable prevBest = null;
        while (true)
        {
            _best = FindBestCandidate();
            if (_held == null && _best != prevBest)
            {
                PickupCandidateChanged?.Invoke(_best?.GetTransform());
                prevBest = _best;
            }
            else if (_held != null && prevBest != null)
            {
                PickupCandidateChanged?.Invoke(null);
                prevBest = null;
            }

            await UniTask.Delay(80);
        }
    }

    private IThrowable FindBestCandidate()
    {
        IThrowable best = null;
        float bestScore = float.MaxValue;
        Vector3 forward = transform.forward;

        foreach (var candidate in _candidates)
        {
            if (candidate == null || candidate.IsHeld) continue;
            Vector3 dir = (candidate.GetTransform().position - transform.position).normalized;
            float angle = Vector3.Angle(forward, dir);
            float dist = Vector3.Distance(transform.position, candidate.GetTransform().position);

            if (angle > _pickupAngle) continue;
            float score = angle * 0.7f + dist * 0.3f;
            if (score < bestScore)
            {
                bestScore = score;
                best = candidate;
            }
        }
        return best;
    }

    private async UniTask TryPickup(IThrowable best)
    {
        if (best == null) return;

        _playerController.StateContainer.AddState(PlayerState.InInteract);
        await UniTask.Delay(180);
        
        var target = transform.position + Vector3.up * 2.25f;
        await best.GetTransform().DOJump(target, 1, 1, 0.16f)
            .SetEase(Ease.OutCubic)
            .AsyncWaitForCompletion();

        best.OnPickup(transform);
        
        await UniTask.Delay(180);

        _playerController.StateContainer.RemoveState(PlayerState.InInteract);
        _held = best;
        PickupCandidateChanged?.Invoke(null);
    }

    private async UniTask TryThrow(Vector3 direction)
    {
        if (_held == null) return;

        _playerController.StateContainer.AddState(PlayerState.InInteract);
        
        await UniTask.Delay(160);
        
        Vector3 throwDir = direction.normalized;
        throwDir.y = 0.45f;
        throwDir.Normalize();

        _held.OnThrow(throwDir * _throwForce);
        _held = null;

        await UniTask.Delay(90);

        _playerController.StateContainer.RemoveState(PlayerState.InInteract);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _throwables) == 0) return;
        var throwable = other.GetComponent<IThrowable>();
        if (throwable != null && !_candidates.Contains(throwable))
            _candidates.Add(throwable);
    }

    private void OnTriggerExit(Collider other)
    {
        var throwable = other.GetComponent<IThrowable>();
        if (throwable != null)
            _candidates.Remove(throwable);
    }
}
