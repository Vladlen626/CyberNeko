using System;
using UnityEngine;
using Zenject;


[RequireComponent(typeof(PlayerMovementController), typeof(PlayerAnimationController))]
public class PlayerController : MonoBehaviour
{
    public event Action OnGrabbed;
    
    public PlayerUIController UIController { get; private set; }
    
    public PlayerMovementController Movement { get; private set; }
    public PlayerAnimationController Animation { get; private set; }
    public PlayerStateContainer StateContainer { get; private set; }

    [Inject]
    public void Construct()
    {
        UIController = GetComponent<PlayerUIController>();
        Movement = GetComponent<PlayerMovementController>();
        Animation = GetComponent<PlayerAnimationController>();
    }

    public void Initialize(Transform cameraTransform, Vector3 spawnPos, PickupMarker pickupMarker)
    {
        StateContainer = new PlayerStateContainer();
        transform.position = spawnPos;
        transform.rotation = Quaternion.identity;
        Movement.Initialize(cameraTransform, spawnPos, StateContainer);
        Animation.Initialize(Movement);
        UIController.Initialize(pickupMarker);
    }

    public void Grabbed()
    {
        if (StateContainer.HasState(PlayerState.Grabbed))
        {
            return;
        }
        AudioManager.inst.PlaySound(SoundNames.ScaredMeow_1);
        Movement.ForceStop();
        StateContainer.AddState(PlayerState.Grabbed);
        OnGrabbed?.Invoke();
    }

    // _____________ Private _____________

    public class Factory : PlaceholderFactory<PlayerController>
    {
    }
}