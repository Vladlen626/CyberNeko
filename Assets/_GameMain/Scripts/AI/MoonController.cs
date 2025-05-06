using UnityEngine;
using UnityEngine.Serialization;

public class MoonController : MonoBehaviour
{
    [SerializeField] private GameObject _happyVisor;
    [SerializeField] private GameObject _sadVisor;

    public void EnableHappy()
    {
        _happyVisor.SetActive(true);
        _sadVisor.SetActive(false);
    }
    
    public void EnableSad()
    {
        _happyVisor.SetActive(false);
        _sadVisor.SetActive(true);
    }
}
