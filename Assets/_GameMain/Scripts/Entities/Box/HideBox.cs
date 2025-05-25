using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;


public class HideBox : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float holdTimeToEscape = 1f;
    [SerializeField] private float moveDistance = 3f;
    [SerializeField] private float moveDuration = 0.4f;
    [SerializeField] private Transform boxModel;

    private Vector3 _hideCenter;
    private Hider _currentHider;
    private bool _isHoldingInput;

    private CancellationTokenSource _cts;
    
    private void Start() => _hideCenter = transform.position;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (_currentHider != null) return;
        if (!other.TryGetComponent(out Hider hider)) return;

        StartHidingProcess(hider);
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!other.TryGetComponent(out Hider hider)) return;
        _currentHider = null;
        hider.SetHiding(false);
    }

    private void StartHidingProcess(Hider hider)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        
        AudioManager.inst.PlaySound(SoundNames.InBox);
        _currentHider = hider;
        hider.SetHiding(true);
       

        DOTween.Sequence()
            .Append(boxModel.DOJump(_hideCenter, 1.5f, 1, 0.15f))
            .Join(hider.transform.DOMove(_hideCenter, 0.15f))
            .OnComplete(() => HandleEscapeLogic(_cts.Token).Forget());
    }

    private async UniTask HandleEscapeLogic(CancellationToken ct)
    {
        float holdTimer = 0;
        Vector2 lastDirection = Vector2.zero;
        const float directionThreshold = 0.1f;

        while (!ct.IsCancellationRequested)
        {
            var input = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            ).normalized;

            bool isHolding = input.sqrMagnitude > 0.1f;
            bool directionChanged = Vector2.Distance(input, lastDirection) > directionThreshold;

            if (isHolding)
            {
                if (directionChanged)
                {
                    holdTimer = 0;
                    lastDirection = input;
                }

                holdTimer += Time.deltaTime;

                if (holdTimer >= holdTimeToEscape)
                {
                    ReleasePlayer(input);
                    return;
                }
            }
            else
            {
                holdTimer = 0;
                lastDirection = Vector2.zero;
            }

            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }
    }

    private void ReleasePlayer(Vector2 input)
    {
        AudioManager.inst.PlaySound(SoundNames.OutBox);
        var hider = _currentHider;
        _currentHider = null;

        var direction = new Vector3(input.x, 0, input.y);
        var targetPos = _hideCenter + direction * moveDistance;
        
        hider.SetHiding(false);
        
        boxModel.DOJump(_hideCenter, 3f, 1, 0.15f);
        hider.transform.DOJump(targetPos, 0.5f, 1, moveDuration);

        _cts?.Cancel();
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        if (_currentHider != null) _currentHider.SetHiding(false);
    }
}
