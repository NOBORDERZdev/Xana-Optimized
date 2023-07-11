using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StoreManager;

public class AddHairPanelToList : MonoBehaviour
{
    public EnumClass.CategoryEnum categoryEnum;

    public GameObject hairAvatarBtn;

    private void OnEnable()
    {
        StoreManager.instance.storeOpen += AddToList;
    }
    private void OnDisable()
    {
        StoreManager.instance.storeOpen -= AddToList;
    }

    private void AddToList()
    {
        Invoke("Delay", 0.1f);
    }
    void Delay()
    {
        //Debug.Log("<color=red> Add selected hair btn into list </color>");
        AR_UndoRedo.obj.ActionWithParametersAdd(hairAvatarBtn, -1, "BtnClicked", AR_UndoRedo.ActionType.ChangePanel, Color.white, categoryEnum);
    }

}
