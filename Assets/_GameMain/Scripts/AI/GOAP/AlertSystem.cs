using Cysharp.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class AlertSettings
{
    public float maxAlert = 100f;
    public float fillSpeed = 30f;
    public float decreaseSpeed = 15f;
    public float chaseThreshold = 0.9f;
    public float alertSpreadRadius = 10f;
}

public class AlertSystem : MonoBehaviour
{
    [SerializeField] private AlertSettings settings;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private AIWorldState worldState;
    
    private float _currentAlert;
    private bool _alertTriggered;
    
    public void TriggerAlert(Vector3 playerPosition)
    {
        _alertTriggered = true;
        worldState.LastKnownPlayerPosition = playerPosition;
        SpreadAlert();
    }

    public void ResetAlert()
    { 
        _alertTriggered = false;
    } 

    // _____________ Private _____________
    
    private void Start() => AlertUpdate().Forget();

    private async UniTaskVoid AlertUpdate()
    {
        while (true)
        {
            UpdateAlertState();
            await UniTask.Yield();
        }
    }

    private void UpdateAlertState()
    {
        worldState.AlertLevel = Mathf.Clamp01(_currentAlert / settings.maxAlert);
        worldState.IsInFullAlert = worldState.AlertLevel >= settings.chaseThreshold;

        if (_alertTriggered)
        {
            _currentAlert += settings.fillSpeed * Time.deltaTime;
            if (_currentAlert >= settings.maxAlert) 
                _currentAlert = settings.maxAlert;
        }
        else
        {
            _currentAlert -= settings.decreaseSpeed * Time.deltaTime;
            if (_currentAlert < 0) 
                _currentAlert = 0;
        }
    }

    private void SpreadAlert()
    {
        Collider[] enemies = Physics.OverlapSphere(
            transform.position, 
            settings.alertSpreadRadius, 
            enemyMask
        );

        foreach (var enemy in enemies)
        {
            if (enemy.TryGetComponent<AlertSystem>(out var system) && enemy != gameObject)
            {
                system.TriggerAlert(worldState.LastKnownPlayerPosition);
            }
        }
    }
}
