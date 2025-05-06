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

        var foodTransform = food.transform;
        _eatSequence = DOTween.Sequence();
        foodTransform.parent = mouthTransform;
        AudioManager.inst.PlaySound(SoundNames.Smacking);
        _eatSequence.Append(foodTransform.DOLocalJump(Vector3.zero, 1f, 1, 0.15f));
        _eatSequence.Append(foodTransform.DOShakeScale(
            duration: eatDuration,
            strength: 0.8f,
            vibrato: 30,
            randomness: 45,
            fadeOut: true
        ));

        _eatSequence.Append(foodTransform.DOScale(0, 0.05f));
        _eatSequence.OnComplete( () => Devour(food));

    }

    private void Devour(Food food)
    {
        food.transform.parent = null;
        food.Devoured();
    }
    


}
