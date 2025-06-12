using UnityEngine;
using Zenject;

public class TestZonePhysics : MonoBehaviour
{  
    private BreakablesManager _breakablesManager;

    [Inject]
    private void Construct(BreakablesManager breakablesManager)
    {
        _breakablesManager = breakablesManager;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ResetPhysObjects();
        }
    }

    private void ResetPhysObjects()
    {
        _breakablesManager.ResetAllBreakables();
    }
}
