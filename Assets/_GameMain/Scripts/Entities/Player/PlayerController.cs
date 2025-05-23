using System;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(PlayerMovementController), typeof(PlayerAnimationController))]
public class PlayerController : MonoBehaviour
{
    public event Action OnGrabbed;
    
    public PlayerMovementController Movement { get; private set; }
    public PlayerAnimationController Animation { get; private set; }

    [Inject]
    public void Construct()
    {
        Movement = GetComponent<PlayerMovementController>();
        Animation = GetComponent<PlayerAnimationController>();
    }

    public void Initialize(Transform cameraTransform, Vector3 spawnPos)
    {
        transform.position = spawnPos;
        transform.rotation = Quaternion.identity;
        Movement.Initialize(cameraTransform, spawnPos);
        Animation.Initialize(Movement);
    }

    public void Grabbed()
    {
        if (Movement.IsGrabbed())
        {
            return;
        }
        AudioManager.inst.PlaySound(SoundNames.ScaredMeow_1);
        Movement.Grabbed();
        OnGrabbed?.Invoke();
    }

    // _____________ Private _____________

    public class Factory : PlaceholderFactory<PlayerController>
    {
    }
}