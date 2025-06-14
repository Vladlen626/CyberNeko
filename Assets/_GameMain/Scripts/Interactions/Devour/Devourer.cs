using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using UniRx;


public class Devourer : MonoBehaviour
{
    public IReadOnlyReactiveProperty<float> CurrentFatness => _currentFatness;
    public int OverEatenThreshold => overEatenThreshold;
    
    [Header("Fat System")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] public int overEatenThreshold = 5;
    [SerializeField] private float fatDecreaseRate = 1f;
    [SerializeField] private float fatPerFood = 1f;

    [Header("Eat Animation")]
    [SerializeField] private Transform mouthTransform;
    [SerializeField] private float eatDuration = 1f;
    [SerializeField] private float fastEatDuration = 0.2f;

    private readonly ReactiveProperty<float> _currentFatness = new ReactiveProperty<float>(0f);

    private Sequence _eatSequence;
    private bool _isEating;
    private bool _isFastSwallowing;
    private readonly HashSet<Food> _foodsInTrigger = new HashSet<Food>();

    private void Start()
    {
        FatnessDecreaseLoop().Forget();
        EatLoop().Forget();
    }

    private void OnTriggerEnter(Collider other)
    {
        var food = other.GetComponent<Food>();
        if (food && food.IsActive())
        {
            _foodsInTrigger.Add(food);

            // Если уже едим, не переели и ускорение ещё не применялось — ускоряем поедание
            if (_isEating
                && !playerController.StateContainer.HasState(PlayerState.OverEaten)
                && !_isFastSwallowing)
            {
                SwallowFast();
                _isFastSwallowing = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var food = other.GetComponent<Food>();
        if (food)
            _foodsInTrigger.Remove(food);
    }

    private async UniTaskVoid EatLoop()
    {
        while (true)
        {
            if (!_isEating && playerController && _foodsInTrigger.Count > 0)
            {
                var food = GetAnyFood();
                await TryEat(food);
            }
            await UniTask.Yield();
        }
    }

    private Food GetAnyFood()
    {
        foreach (var food in _foodsInTrigger)
            return food;
        return null;
    }

    private async UniTask TryEat(Food food)
    {
        if (!food) return;

        bool isOvereaten = playerController.StateContainer.HasState(PlayerState.OverEaten);
        _isEating = true;

        if (isOvereaten)
        {
            // В переедании — едим медленно, жирность не растёт
            await PlayEatSequence(food, eatDuration);
        }
        else
        {
            // Если ускоряли поедание — едим быстро и толстеем
            if (_isFastSwallowing)
            {
                await PlayEatSequence(food, fastEatDuration);
                _currentFatness.Value += fatPerFood;
                _isFastSwallowing = false;
                if (_currentFatness.Value >= overEatenThreshold)
                    playerController.StateContainer.AddState(PlayerState.OverEaten);
            }
            else
            {
                // Если не ускоряли — едим медленно, жирность не растёт
                await PlayEatSequence(food, eatDuration);
            }
        }

        _isEating = false;
    }

    private async UniTask PlayEatSequence(Food food, float duration)
    {
        food.DisablePhysics();
        var tf = food.transform;
        tf.DOKill(true);
        tf.parent = mouthTransform;
        AudioManager.inst.PlaySound(SoundNames.Smacking);

        _eatSequence = DOTween.Sequence();
        _eatSequence.Append(tf.DOLocalJump(Vector3.zero, 1f, 1, 0.12f));
        _eatSequence.Append(tf.DOShakeScale(duration, 0.8f, 30, 45, true).OnUpdate(() =>
        {
            tf.localRotation = Quaternion.Euler(0, 180, 0);
            tf.localPosition = Vector3.zero;
        }));
        _eatSequence.Append(tf.DOScale(Vector3.zero, 0.05f));
        _eatSequence.OnComplete(() => Devour(food));

        await UniTask.WaitUntil(() => !_eatSequence.IsActive() || _eatSequence.IsComplete());
    }

    private void SwallowFast()
    {
        if (_eatSequence != null && _eatSequence.IsActive())
        {
            _eatSequence.timeScale = (_eatSequence.Duration() - _eatSequence.Elapsed()) > 0.01f
                ? (_eatSequence.Duration() - _eatSequence.Elapsed()) / fastEatDuration
                : 1f;
            _eatSequence.Complete(true);
        }
    }

    private void Devour(Food food)
    {
        food.transform.parent = null;
        food.Devoured();
        _foodsInTrigger.Remove(food);
    }

    private async UniTask FatnessDecreaseLoop()
    {
        while (true)
        {
            if (!_isEating && _currentFatness.Value > 0f)
            {
                _currentFatness.Value -= fatDecreaseRate * Time.deltaTime;
                _currentFatness.Value = Mathf.Max(_currentFatness.Value, 0f);
            }

            if (playerController.StateContainer.HasState(PlayerState.OverEaten) &&
                _currentFatness.Value < overEatenThreshold * 0.2f)
            {
                playerController.StateContainer.RemoveState(PlayerState.OverEaten);
            }

            await UniTask.Yield();
        }
    }
}
