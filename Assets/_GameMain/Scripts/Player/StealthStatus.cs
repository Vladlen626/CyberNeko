using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

// Local for every player
public class StealthStatus : MonoBehaviour
{

    private List<GameObject> _pursuers = new List<GameObject>();

    public bool IsStealthActive()
    {
        return _pursuers.Count == 0;
    }

    public void AddToPursuer(GameObject pursuer)
    {
        if (_pursuers.Contains(pursuer))
        {
            Debug.LogError("Pursuer " + pursuer.ToString() + " already added to pursuers for " + gameObject.ToString());
            return;
        }
        _pursuers.Add(pursuer);
    }

    public void RemoveFromPursuer(GameObject pursuer)
    {
        _pursuers.Remove(pursuer);
    }
}
