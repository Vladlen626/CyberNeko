using UniRx;
using UnityEngine;

[RequireComponent(typeof(Devourer))]
public class FatnessObserver : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _minScale = 1f;
    [SerializeField] private float _maxScale = 2f;
    
    private Devourer _devourer;
    private void Start()
    {
        _devourer = GetComponent<Devourer>();
        
        _devourer.CurrentFatness
            .Subscribe(OnFatnessChanged)
            .AddTo(this);
    }

    private void OnFatnessChanged(float fatness)
    {
        float normalized = Mathf.Clamp01(fatness / _devourer.OverEatenThreshold);
        float scale = Mathf.Lerp(_minScale, _maxScale, normalized);
        _playerTransform.localScale = new Vector3(scale, scale, scale);
    }
}