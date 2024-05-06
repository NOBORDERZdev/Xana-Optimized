using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InventoryManager;

public class AddHairPanelToList : MonoBehaviour
{
    public EnumClass.CategoryEnum categoryEnum;

    public GameObject hairAvatarBtn;

    private void OnEnable()
    {
        InventoryManager.instance.storeOpen += AddToList;
    }
    private void OnDisable()
    {
        InventoryManager.instance.storeOpen -= AddToList;
    }

    private void AddToList()
    {
        Invoke("Delay", 0.1f);
    }
    void Delay()
    {
        //Debug.Log("<color=red> Add selected hair btn into list </color>");
        StoreUndoRedo.obj.ActionWithParametersAdd(hairAvatarBtn, -1, "BtnClicked", StoreUndoRedo.ActionType.ChangePanel, Color.white, categoryEnum);
    }

}
