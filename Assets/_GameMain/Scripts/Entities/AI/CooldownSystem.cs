using System.Threading;
using Cysharp.Threading.Tasks;
public class CooldownSystem
{
    private bool _isOnCooldown;
    private CancellationTokenSource _globalCts;

    public bool IsOnCooldown => _isOnCooldown;
    public float RemainingTime { get; private set; }

    public async void StartCooldown(float duration)
    {
        if (_isOnCooldown) return;

        _isOnCooldown = true;
        RemainingTime = duration;
        _globalCts?.Cancel();
        _globalCts = new CancellationTokenSource();

        try
        {
            while (RemainingTime > 0)
            {
                await UniTask.Delay(100, cancellationToken: _globalCts.Token);
                RemainingTime -= 0.1f;
            }
        }
        finally
        {
            _isOnCooldown = false;
            RemainingTime = 0;
            _globalCts?.Dispose();
            _globalCts = null;
        }
    }

    public void StopCooldown()
    {
        _globalCts?.Cancel();
        _isOnCooldown = false;
        RemainingTime = 0;
    }

    public void Dispose()
    {
        StopCooldown();
    }
}
