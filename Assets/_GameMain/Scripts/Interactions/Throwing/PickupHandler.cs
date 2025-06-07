using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class PickupHandler : MonoBehaviour
{
    
    [SerializeField] private Transform _hands;
    
    public async UniTask<IThrowable> PickupAsync(IThrowable target)
    {
        if (target == null) return null;

        target.OnPickupStart();
        await UniTask.Delay(180);

        await target.GetTransform().DOJump(_hands.position, 1, 1, 0.16f)
            .SetEase(Ease.OutCubic)
            .Join(target.GetTransform().DORotateQuaternion(Quaternion.identity, 0.1f))
            .AsyncWaitForCompletion();

        target.OnPickup(_hands);

        await UniTask.Delay(180);

        return target;
    }
}
