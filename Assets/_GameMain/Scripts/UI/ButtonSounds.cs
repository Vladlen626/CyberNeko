using UnityEngine;
using UnityEngine.UI;

public class ButtonSounds : MonoBehaviour
{
    void Awake()
    {
        var buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var button in buttons)
        {
            button.onClick.AddListener( () => AudioManager.inst.PlaySound(SoundNames.Button));
        }
    }

}
