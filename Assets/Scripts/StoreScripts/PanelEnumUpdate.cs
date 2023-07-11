using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelEnumUpdate : MonoBehaviour
{
    public enum LocalPanelType { AvatarPanel, WearablePanel};
    public LocalPanelType localPanelType;

    private void OnEnable()
    {
        if (localPanelType.Equals(LocalPanelType.AvatarPanel))
            AR_UndoRedo.obj.panelType = AR_UndoRedo.PanelType.Avatar;
        else if (localPanelType.Equals(LocalPanelType.WearablePanel))
            AR_UndoRedo.obj.panelType = AR_UndoRedo.PanelType.Wearable;
    }

    void Start()
    {
        
    }


}
