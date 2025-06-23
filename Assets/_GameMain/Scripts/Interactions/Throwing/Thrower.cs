using Cysharp.Threading.Tasks;
using UnityEngine;

public class Thrower : MonoBehaviour
{
    [SerializeField] private float throwDuration = 0.35f;
    [SerializeField] private float arcHeight = 1.5f;

    public async UniTask Throw(IThrowable throwable, Vector3 targetPos)
    {
        if (throwable == null) return;
        throwable.Throw(targetPos, throwDuration, arcHeight);
        await UniTask.CompletedTask;
    }
}