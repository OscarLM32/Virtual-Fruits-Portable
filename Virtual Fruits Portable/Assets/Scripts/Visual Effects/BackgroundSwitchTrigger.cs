using System;
using DefaultNamespace;
using UnityEngine;

public class BackgroundSwitchTrigger : MonoBehaviour
{
    public BackgroundThemes Theme;

    public delegate void OnBackgroundSwitch(BackgroundThemes theme);
    public static event OnBackgroundSwitch BackgroundSwitchTriggered;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != (int)LayerValues.PlayerLayer)
            return;
        
        BackgroundSwitchTriggered?.Invoke(Theme);
    }
}
