using UnityEngine;

public interface IBreakable
{
    void Register();
    void Break();
    Transform GetTransform();
}