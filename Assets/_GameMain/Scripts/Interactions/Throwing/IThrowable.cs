using UnityEngine;

public interface IThrowable
{
    bool IsHeld { get; }
    Transform GetTransform();
    void OnPickupStart();
    void OnPickup(Transform holder);
    void OnThrow(Vector3 force);
}