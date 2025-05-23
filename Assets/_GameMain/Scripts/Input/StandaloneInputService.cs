using UnityEngine;

public class StandaloneInputService : IInputService
{
    public float GetHorizontal() => Input.GetAxisRaw("Horizontal");
    public float GetVertical() => Input.GetAxisRaw("Vertical");
}