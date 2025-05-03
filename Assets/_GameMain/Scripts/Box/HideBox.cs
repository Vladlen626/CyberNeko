using UnityEngine;
using UnityEngine.Assertions;

public class HideBox : MonoBehaviour
{
    [Tooltip("Will be shown if the player hides in the box but is being pursued")]
    [SerializeField] private GameObject _marker;

    private GameObject _hiddenPlayer = null;
    private bool _isMarkerActive = false;

    void Start()
    {
        HideMarker();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject playerObj = other.gameObject;
            if (_hiddenPlayer != null)
            {
                Debug.Log("The player " + playerObj.ToString() + " tries to hide in the box, but it is already occupied"); // For  multiplayer
                return;
            }

            HidePlayerInside(playerObj);

            Debug.Log("Player " + playerObj.ToString() + " hide in Box");
            StealthStatus stealthStatus = playerObj.GetComponent<StealthStatus>();
            Assert.IsNotNull(stealthStatus, $"{playerObj.name} need StealthStatus");
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

                if (_isMarkerActive)
                    HideMarker();
            }
        }
    }

    // TODO: USE DOTWEEN to move player
    private void HidePlayerInside(GameObject playerObj)
    {
        playerObj.transform.position = transform.position;
    }

    private void ShowMarker()
    {
        _isMarkerActive = true;
        _marker.SetActive(true);
    }

    private void HideMarker()
    {
        _marker.SetActive(false);
        _isMarkerActive = false;
    }
}
