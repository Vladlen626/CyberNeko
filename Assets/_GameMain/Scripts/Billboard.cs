using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera _mainCamera;

    void Awake()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {
        transform.LookAt(_mainCamera.transform);
        transform.rotation = _mainCamera.transform.rotation;
    }
}
