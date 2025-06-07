using DG.Tweening;
using UnityEngine;

public class Devourer : MonoBehaviour
{
    [SerializeField] private Transform mouthTransform;
    [SerializeField] private float eatDuration;

    private Sequence _eatSequence;

    public void Eat(Food food)
    {
        if (_eatSequence != null && _eatSequence.IsActive())
        {
            _eatSequence.Complete();
        }
        
        food.DisablePhysics();
        
        var foodTransform = food.transform;
        foodTransform.DOKill(true);
        foodTransform.parent = mouthTransform;
        AudioManager.inst.PlaySound(SoundNames.Smacking);
        _eatSequence = DOTween.Sequence();
        _eatSequence.Append(foodTransform.DOLocalJump(Vector3.zero, 1f, 1, 0.12f));
        _eatSequence.Append(foodTransform.DOShakeScale(
            duration: eatDuration,
            strength: 0.8f,
            vibrato: 30,
            randomness: 45,
            fadeOut: true
        ).OnUpdate(() =>
        {
            foodTransform.localRotation = Quaternion.Euler(0, 180, 0);
            foodTransform.localPosition = Vector3.zero;
        }));

        _eatSequence.Append(foodTransform.DOScale(0, 0.05f));
        _eatSequence.OnComplete( () => Devour(food));

    }

    private void Devour(Food food)
    {
        food.transform.parent = null;
        food.Devoured();
    }
    
}
