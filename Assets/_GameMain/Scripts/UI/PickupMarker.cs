using UnityEngine;

public class PickupMarker : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Vector3 _worldOffset = Vector3.up;
    [SerializeField] private bool _hideWhenOffscreen = true;

    private Transform _targetTransform;
    private Camera _mainCamera;

    public class Factory : Zenject.PlaceholderFactory<PickupMarker> { }
    
    public void Show(Transform target)
    {
        _targetTransform = target;
        SetVisible(true);
    }

    public void Hide()
    {
        _targetTransform = null;
        SetVisible(false);
    }

    // _____________ Private _____________

    private void Awake()
    {
        _mainCamera = Camera.main;
        SetVisible(false);
    }

    private void LateUpdate()
    {
        if (_targetTransform == null || _mainCamera == null) return;

        Vector3 worldPosition = _targetTransform.position + Vector3.up + _worldOffset;
        Vector3 screenPosition = _mainCamera.WorldToScreenPoint(worldPosition);

        if (_hideWhenOffscreen && (screenPosition.z <= 0 ||
                                   screenPosition.x < 0 || screenPosition.x > Screen.width ||
                                   screenPosition.y < 0 || screenPosition.y > Screen.height))
        {
            SetVisible(false);
            return;
        }

        SetVisible(true);
        transform.position = screenPosition;
    }

    private void SetVisible(bool visible)
    {
        _canvasGroup.alpha = visible ? 1f : 0f;
        _canvasGroup.blocksRaycasts = visible;
        gameObject.SetActive(visible);
    }
}