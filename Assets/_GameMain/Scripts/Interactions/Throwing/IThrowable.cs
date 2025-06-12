using UnityEngine;

public interface IThrowable
{
    bool IsHeld { get; }
    Transform GetTransform();

    void OnPickupStart();
    void OnPickup(Transform holder); 

    void TweenThrow(Vector3 targetPos, float duration, float arc);
}