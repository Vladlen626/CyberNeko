using UnityEngine;

public interface IBreakable
{
    void Register();
    void Break();
    void Reset();
    Transform GetTransform();
}