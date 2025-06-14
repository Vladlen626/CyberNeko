using System;
using DG.Tweening;
using FMODUnity;
using UnityEngine;
using Zenject;

public class Door : MonoBehaviour
{
    public event Action OnOpened;
    
    [SerializeField] private Transform[] borders;

    private Vector3 originalPos;
    private PointsManager _pointsManager;

    [Inject]
    private void Construct(PointsManager pointsManager)
    {
        _pointsManager = pointsManager;
    }
    
    public void Initialize()
    {
        originalPos = transform.position;
    }
    
    public void Open()
    {
        foreach (var border in borders)
        {
            HideObject(border);
        }

        HideObject(transform);
    }

    public void Reset()
    {
        foreach (var border in borders)
        {
            ShowObject(border);
        }

        ShowObject(transform);
    }

    // _____________ Private _____________

    private void HideObject(Transform objectToHide)
    {
        AudioManager.inst.PlaySound(SoundNames.DoorOpen);
        objectToHide.DOJump(objectToHide.position += Vector3.up * 3, 1,1, 0.75f)
            .OnComplete(() =>
            {
                objectToHide.DOScale(0, 0.25f);
            });
    }

    private void ShowObject(Transform objectToShow)
    {
        objectToShow.localScale = Vector3.one;
        objectToShow.position = originalPos;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_pointsManager.IsKeyActive.Value)
            {
                OnOpened?.Invoke();
                _pointsManager.ResetGoal();
            }
        }
    }
    
}
