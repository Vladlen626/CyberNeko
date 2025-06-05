using System.Collections.Generic;
using UnityEngine;

public class VisorController : MonoBehaviour
{
    [SerializeField] private List<GameObject> visors;

    public void ChooseVisor(int visorNum)
    {
        if (visors.Count == 0) return;
        foreach (var visor in visors)
        {
            
            visor.SetActive(false);
        }
        
        visors[visorNum].SetActive(true);
    }
}
