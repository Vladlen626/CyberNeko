using DG.Tweening;
using UnityEngine;

public class Devourer : MonoBehaviour
{
    [SerializeField] private Transform mouthTransform;
    [SerializeField] private float eatDuration;

    private Sequence eatSequence;

    public void Eat(Food food)
    {
        if (eatSequence != null && eatSequence.IsActive())
        {
            eatSequence.Complete();
        }

        var foodTransform = food.transform;
        eatSequence = DOTween.Sequence();
        foodTransform.parent = mouthTransform;
        AudioManager.inst.PlaySound(SoundNames.Smacking);
        eatSequence.Append(foodTransform.DOLocalJump(Vector3.zero, 1f, 1, 0.15f));
        eatSequence.Append(foodTransform.DOShakeScale(
            duration: eatDuration,
            strength: 0.8f,
            vibrato: 30,
            randomness: 45,
            fadeOut: true
        ));

        eatSequence.Append(foodTransform.DOScale(0, 0.05f));
        eatSequence.OnComplete( () => Devour(food));

    }

    private void Devour(Food food)
    {
        food.transform.parent = null;
        food.Devoured();
    }
    


}
