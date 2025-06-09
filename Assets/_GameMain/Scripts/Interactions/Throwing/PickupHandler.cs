using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class PickupHandler : MonoBehaviour
{
    public async UniTask<IThrowable> PickupAsync(IThrowable target, Transform holder)
    {
        if (target == null) return null;

        target.OnPickupStart();
        await UniTask.Delay(180);

        await target.GetTransform().DOJump(holder.position,1, 1, 0.16f)
            .SetEase(Ease.OutCubic)
            .Join(target.GetTransform().DORotateQuaternion(Quaternion.identity, 0.1f))
            .AsyncWaitForCompletion();
        
        target.OnPickup(holder);

        await UniTask.Delay(180);

        return target;
    }
}
