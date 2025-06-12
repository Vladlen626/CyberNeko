using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;
// ReSharper disable Unity.PerformanceCriticalCodeInvocation

[RequireComponent(typeof(Thrower),typeof(ThrowCandidateSelector), typeof(PickupHandler))]
public class PlayerThrower : MonoBehaviour
{
    private static readonly int PickUp = Animator.StringToHash("PickUp");
    private static readonly int Throw = Animator.StringToHash("Throw");
    private static readonly int Hold = Animator.StringToHash("Hold");

    [SerializeField] private Transform hands;
    [SerializeField] private Transform throwTarget;
    [SerializeField] private PlayerController playerController;

    private ThrowCandidateSelector _selector;
    private Thrower _thrower;
    private PickupHandler _pickupHandler;
    private IInputService _input;
    private IThrowable _held;

    [Inject]
    public void Construct(IInputService input)
    {
        _input = input;
    }
    
    private void Start()
    {
        _selector = GetComponent<ThrowCandidateSelector>();
        _thrower = GetComponent<Thrower>();
        _pickupHandler = GetComponent<PickupHandler>();
        
        _selector.SetHeld(_held);
    }

    private void Update()
    {
        if (playerController.StateContainer.HasState(PlayerState.InInteract))
            return;

        if (_input.IsInteractPressed())
        {
            if (_held == null)
                TryPickup(_selector.GetBestCandidate()).Forget();
            else
                TryThrow(throwTarget.position).Forget();
        }
    }

    // _____________ Private _____________

    
    private void LateUpdate()
    {
        if (_held != null)
        {
            var tr = _held.GetTransform();
            tr.position = hands.position;
            tr.rotation = hands.rotation;
        }
    }

    private async UniTask TryPickup(IThrowable best)
    {
        if (best == null) return;
        playerController.StateContainer.AddState(PlayerState.InInteract);

        _held = await _pickupHandler.PickupAsync(best, hands);

        playerController.StateContainer.RemoveState(PlayerState.InInteract);
        _selector.SetHeld(_held);
    }


    private async UniTask TryThrow(Vector3 direction)
    {
        if (_held == null) return;
        playerController.StateContainer.AddState(PlayerState.InInteract);
        await UniTask.Delay(160);

        await _thrower.Throw(_held, direction);
        _held = null;
        _selector.SetHeld(null);

        await UniTask.Delay(200);
        playerController.StateContainer.RemoveState(PlayerState.InInteract);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        _selector.OnEnterCandidate(other);
    }

    private void OnTriggerExit(Collider other)
    {
        _selector.OnExitCandidate(other);
    }
}