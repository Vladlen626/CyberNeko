using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AttackAction : AIAction
{
    [Header("Attack Settings")]
    [SerializeField] private float _attackDelaySec = 0.2f;
    [SerializeField] private float _attackRange = 1;
    [SerializeField] private float _dashForce = 1;
    [SerializeField] private float _attackTime = 1;
    [SerializeField] private float _attackCooldown = 3;
    
    private readonly CooldownSystem _cooldownSystem = new CooldownSystem();
    private CancellationTokenSource _cooldownCTS;
    private Rigidbody _rigidbody;
    private Vector3 attackDirection;

    public override async UniTask PerformAction()
    {
        await PrepareAttack();
        await Attack();
        AfterAttack();
    }

    public override bool IsApplicable()
    {
        return !_cooldownSystem.IsOnCooldown && _aiKnowledge.IsTargetOnVision && IsTargetInAttackRange();
    }

    // _____________ Private _____________

    protected override void Awake()
    {
        base.Awake();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private async UniTask PrepareAttack()
    {
        _rigidbody.isKinematic = false;
        
        _movement.DisableMovement();
        _movement.LookAt(_aiKnowledge.Target);
        
        attackDirection = (_aiKnowledge.Target.position - transform.position).normalized;
        
        await UniTask.WaitForSeconds(_attackDelaySec);
    }

    private async UniTask  Attack()
    {
        _rigidbody.AddForce(attackDirection * _dashForce, ForceMode.VelocityChange);
        
        await UniTask.WaitForSeconds(_attackTime);
    }

    private void AfterAttack()
    {
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.isKinematic = true;
        
        _movement.EnableMovement();
        
        _cooldownSystem.StartCooldown(_attackCooldown);
    }

    private bool IsTargetInAttackRange()
    {
        return _aiKnowledge.DistanceToTarget < _attackRange;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            var playerController = other.gameObject.GetComponent<PlayerController>();
            playerController.Grabbed();
        }
    }
    
}
