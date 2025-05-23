using UnityEngine;
using Zenject;

[RequireComponent(typeof(PlayerMovementController), typeof(PlayerAnimationController))]
public class PlayerController : MonoBehaviour
{
    public PlayerMovementController Movement { get; private set; }
    public PlayerAnimationController Animation { get; private set; }

    [Inject]
    public void Construct()
    {
        Movement = GetComponent<PlayerMovementController>();
        Animation = GetComponent<PlayerAnimationController>();
    }

    public void Initialize(Transform cameraTransform)
    {
        Movement.Initialize(cameraTransform);
        Animation.Initialize(Movement);
    }

    public void Grabbed()
    {
        AudioManager.inst.PlaySound(SoundNames.ScaredMeow_1);
        Movement.Grabbed();
    }

    // _____________ Private _____________

    public class Factory : PlaceholderFactory<PlayerController>
    {
    }
}