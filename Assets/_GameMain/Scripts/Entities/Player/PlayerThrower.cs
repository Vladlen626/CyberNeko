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
    [SerializeField] private PlayerController _playerController;

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
        if (_playerController.StateContainer.HasState(PlayerState.InInteract))
            return;

        if (_input.IsInteractPressed())
        {
            if (_held == null)
                TryPickup(_selector.GetBestCandidate()).Forget();
            else
                TryThrow(transform.forward).Forget();
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
        _playerController.StateContainer.AddState(PlayerState.InInteract);

        _held = await _pickupHandler.PickupAsync(best, hands);

        _playerController.StateContainer.RemoveState(PlayerState.InInteract);
        _selector.SetHeld(_held);
    }


    private async UniTask TryThrow(Vector3 direction)
    {
        if (_held == null) return;
        _playerController.StateContainer.AddState(PlayerState.InInteract);
        await UniTask.Delay(160);

        await _thrower.Throw(_held, direction);
        _held = null;
        _selector.SetHeld(null);

        await UniTask.Delay(90);
        _playerController.StateContainer.RemoveState(PlayerState.InInteract);
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