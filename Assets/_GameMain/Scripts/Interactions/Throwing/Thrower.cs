using Cysharp.Threading.Tasks;
using UnityEngine;

public class Thrower : MonoBehaviour
{
    [SerializeField] private float _throwDuration = 0.35f;
    [SerializeField] private float _arcHeight = 1.5f;

    public async UniTask Throw(IThrowable throwable, Vector3 targetPos)
    {
        if (throwable == null) return;
        throwable.TweenThrow(targetPos, _throwDuration, _arcHeight);
        await UniTask.CompletedTask;
    }
}