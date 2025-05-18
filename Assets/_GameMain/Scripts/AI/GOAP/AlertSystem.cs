
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

    private WorldState _worldState;

    public void Initialize(WorldState worldState)
    {
        _worldState = worldState;
    }
    
    public void ResetAlert()
    {
        _worldState.AlertProgress = 0f;
    }

    // _____________ Private _____________
    
    public void RemoveAlert(float amount)
    {
        _worldState.AlertProgress = Mathf.Clamp01(_worldState.AlertProgress - amount * _settings.DecaySpeed * Time.deltaTime);
        if (_worldState.AlertProgress <= _settings.AlertDisableThreshold)
        {
            DisableFullAlert();
        }
    }

    public void AddAlert(float amount)
    {
        _worldState.AlertProgress += amount * _settings.DetectSpeed * Time.deltaTime;

        if (_worldState.AlertProgress >= _settings.AlertEnableThreshold)
        {
            ActivateFullAlert();
            SpreadAlert();
        }
    }

    private void ActivateFullAlert()
    {
        _worldState.AlertProgress = 1f;
        _worldState.IsAlerted = true;
    }

    private void DisableFullAlert()
    {
        _worldState.IsAlerted = false;
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
            system._worldState.Target = _worldState.Target;
        }
    }
}