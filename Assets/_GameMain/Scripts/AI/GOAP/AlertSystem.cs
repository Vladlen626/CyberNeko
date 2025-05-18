
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AlertSystem : MonoBehaviour
{
    [System.Serializable]
    public class Settings
    {
        [Range(0, 1)] public float AlertEnableThreshold = 1f;
        [Range(0, 1)] public float AlertDisableThreshold = 0.01f;
        public float DetectSpeed = 3f;
        public float DecaySpeed = 0.01f;
        public float SpreadRadius = 0f;
    }

    [SerializeField] private Settings _settings;
    [SerializeField] private LayerMask _enemyLayer;

    private AIKnowledge _aiKnowledge;

    public void Initialize(AIKnowledge aiKnowledge)
    {
        _aiKnowledge = aiKnowledge;
    }
    
    public void ResetAlert()
    {
        _aiKnowledge.AlertProgress.Value = 0f;
    }

    // _____________ Private _____________
    
    public void RemoveAlert()
    {
        _aiKnowledge.AlertProgress.Value = Mathf.Clamp01(_aiKnowledge.AlertProgress.Value - 1 * _settings.DecaySpeed * Time.deltaTime);
        if (_aiKnowledge.AlertProgress.Value <= _settings.AlertDisableThreshold)
        {
            DisableFullAlert();
        }
    }

    public void AddAlert()
    {
        _aiKnowledge.AlertProgress.Value += 1 * _settings.DetectSpeed * Time.deltaTime;

        if (_aiKnowledge.AlertProgress.Value >= _settings.AlertEnableThreshold)
        {
            ActivateFullAlert();
            SpreadAlert();
        }
    }

    private void ActivateFullAlert()
    {
        _aiKnowledge.AlertProgress.Value = 1f;
        _aiKnowledge.IsAlerted = true;
    }

    private void DisableFullAlert()
    {
        _aiKnowledge.IsAlerted = false;
    }

    private void SpreadAlert()
    {
        Collider[] enemies = Physics.OverlapSphere(
            transform.position,
            _settings.SpreadRadius,
            _enemyLayer
        );

        foreach (var enemy in enemies)
        {
            if (!enemy.TryGetComponent<AlertSystem>(out var system)) continue;
            system.ActivateFullAlert();
            system._aiKnowledge.Target = _aiKnowledge.Target;
        }
    }
}