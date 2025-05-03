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
        // Turn the object towards the camera
        transform.LookAt(_mainCamera.transform);

        // So the picture doesn't turn upside down
        transform.rotation = _mainCamera.transform.rotation;
    }
}
