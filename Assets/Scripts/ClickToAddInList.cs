using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static StoreManager;

public class ClickToAddInList : MonoBehaviour
{
    public int panelIndex = 0;
    public GameObject messageReceiverObject;
    public GameObject selectedLine;

    private void OnEnable()
    {
        StoreManager.instance.storeOpen += AddToList;
    }
    private void OnDisable()
    {
        StoreManager.instance.storeOpen -= AddToList;
    }

    void AddToList()
    {
        Invoke("Delay", 0.05f);
    }
    void Delay()
    {
        if (panelIndex == 1)
        {
            AR_UndoRedo.obj.ActionWithParametersAdd(messageReceiverObject, panelIndex, "SelectPanel", AR_UndoRedo.ActionType.ChangeCategory, Color.white, EnumClass.CategoryEnum.Avatar);
            AR_UndoRedo.obj.panelType = AR_UndoRedo.PanelType.Avatar;
        }
    }

    void Start()
    {
        //this.gameObject.GetComponent<Button>().onClick.AddListener(AddIntoList);
    }

    public void AddIntoList()
    {
        if (selectedLine.activeSelf) return;

        if (!AR_UndoRedo.obj.addToList)
            AR_UndoRedo.obj.addToList = true;
        else
        {
            if (panelIndex == 1)
            {
                AR_UndoRedo.obj.ActionWithParametersAdd(messageReceiverObject, panelIndex, "SelectPanel", AR_UndoRedo.ActionType.ChangeCategory, Color.white, EnumClass.CategoryEnum.Avatar);
                AR_UndoRedo.obj.panelType = AR_UndoRedo.PanelType.Avatar;
            }
            else if (panelIndex == 0)
            {
                AR_UndoRedo.obj.ActionWithParametersAdd(messageReceiverObject, panelIndex, "SelectPanel", AR_UndoRedo.ActionType.ChangeCategory, Color.white, EnumClass.CategoryEnum.Wearable);
                AR_UndoRedo.obj.panelType = AR_UndoRedo.PanelType.Wearable;
            }
            //Debug.Log("<color=red> Added Into List: " + this.gameObject.name + "</color>");
        }
    }

}
