using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;

public class HideBox : MonoBehaviour
{
    [Tooltip("Will be shown if the player hides in the box but is being pursued")]
    [SerializeField] private GameObject _marker;

    private Vector3 _playerHidePos;
    private GameObject _hiddenPlayer = null;

    void Start()
    {
        _playerHidePos = transform.position;
        HideMarker();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject playerObj = other.gameObject;
            if (_hiddenPlayer != null)
            {
                Debug.Log("The player " + playerObj + " tries to hide in the box, but it is already occupied"); // For  multiplayer
                return;
            }

            HidePlayerInside(playerObj.transform);

            Debug.Log("Player " + playerObj + " hide in Box");
            StealthStatus stealthStatus = playerObj.GetComponent<StealthStatus>();
            //Assert.IsNotNull(stealthStatus, $"{playerObj.name} need StealthStatus");
            _hiddenPlayer = playerObj;
            if (!stealthStatus.IsStealthActive())
            {
                ShowMarker();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left Box");
            GameObject playerObj = other.gameObject;

            if (_hiddenPlayer == playerObj)
            {
                _hiddenPlayer = null;
                DropPlayer();
            }
        }
    }
    
    private void HidePlayerInside(Transform player)
    {
        AudioManager.inst.PlaySound(SoundNames.InBox);
        player.DOMove(_playerHidePos + Vector3.up * 0.5f, 0.15f);
        transform.DOJump(_playerHidePos, 1.5f, 1, 0.18f);
    }

    private void DropPlayer()
    {
        AudioManager.inst.PlaySound(SoundNames.OutBox);
        transform.DOJump(_playerHidePos, 1f, 1, 0.1f);
        HideMarker();
    }

    private void ShowMarker()
    {
        AudioManager.inst.PlaySound(SoundNames.Alert);
        _marker.SetActive(true);
    }

    private void HideMarker()
    {
        _marker.SetActive(false);
    }
}
