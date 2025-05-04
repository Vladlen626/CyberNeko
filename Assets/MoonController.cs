using UnityEngine;

public class MoonController : MonoBehaviour
{
    [SerializeField] private GameObject happyVisor;
    [SerializeField] private GameObject sadVisor;

    public void EnableHappy()
    {
        happyVisor.SetActive(true);
        sadVisor.SetActive(false);
    }
    
    public void EnableSad()
    {
        happyVisor.SetActive(false);
        sadVisor.SetActive(true);
    }
}
