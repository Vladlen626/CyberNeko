using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class Final : MonoBehaviour
{
    public UnityEvent finalGame;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transform.DOScale(0, 0.5f);
            AudioManager.inst.PlaySound(SoundNames.PrincessMeow);
            AudioManager.inst.PlaySound(SoundNames.ScaredMeow_1);
            finalGame.Invoke();
        }
    }

}
