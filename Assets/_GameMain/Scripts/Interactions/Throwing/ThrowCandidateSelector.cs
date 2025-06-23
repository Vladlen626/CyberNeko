using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ThrowCandidateSelector : MonoBehaviour
{
    [SerializeField] private float _pickupAngle = 60f;
    [SerializeField] private LayerMask _throwables;
    public event Action<Transform> PickupCandidateChanged;
    
    private readonly List<IThrowable> _candidates = new();
    private IThrowable _best;
    private IThrowable _held;

    public void SetHeld(IThrowable held)
    {
        _held = held;
    }
    public IThrowable GetBestCandidate()
    {
        return _best;
    }

    private void Start()
    {
        SearchBestLoop().Forget();
    }

    public void OnEnterCandidate(Collider other)
    {
        if (((1 << other.gameObject.layer) & _throwables) == 0) return;
        var throwable = other.GetComponent<IThrowable>();
        if (throwable != null && !_candidates.Contains(throwable))
            _candidates.Add(throwable);
    }

    public void OnExitCandidate(Collider other)
    {
        var throwable = other.GetComponent<IThrowable>();
        if (throwable != null)
            _candidates.Remove(throwable);
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
            if (!candidate.GetTransform().gameObject.activeInHierarchy) continue;
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
}
