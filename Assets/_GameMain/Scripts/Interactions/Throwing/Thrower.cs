using Cysharp.Threading.Tasks;
using UnityEngine;

public class Thrower : MonoBehaviour
{
    [SerializeField] private float _throwForce = 8f;

    public async UniTask Throw(IThrowable throwable, Vector3 direction)
    {
        if (throwable == null) return;
        Vector3 throwDir = direction.normalized;
        throwDir.y = 0.45f;
        throwDir.Normalize();
        throwable.OnThrow(throwDir * _throwForce);
        await UniTask.Yield();
    }
}
