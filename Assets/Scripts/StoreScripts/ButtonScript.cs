using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static InventoryManager;

public class ButtonScript : MonoBehaviour
{
    public EnumClass.CategoryEnum categoryEnum;

    public int Index;
    public Text BtnTxt;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Button>().onClick.AddListener(BtnClicked);
        this.gameObject.GetComponent<Button>().onClick.AddListener(ButtonPressed);
        InventoryManager.instance.UpdateXanaConstants();
    }

    public void BtnClicked()
    {
        //// AR changes start
        if (BtnTxt.color != GetComponentInParent<SubBottons>().HighlightedColor)
        {
            if (!StoreUndoRedo.obj.addToList)
                StoreUndoRedo.obj.addToList = true;
            else
            {
                StoreUndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "BtnClicked", StoreUndoRedo.ActionType.ChangePanel, Color.white, categoryEnum);
            }
        }
        //// AR changes end


        if (ConstantsHolder.xanaConstants.currentButtonIndex == Index)
        {
            //    if (Index == 0)                                                  // AR Changes
            //        InventoryManager.instance.ForcellySetLastClickedBtnOfHair();     // AR Changes

            if (InventoryManager.instance.CloseColorPanel(Index) == true)
            {
                if (InventoryManager.instance.ParentOfBtnsAvatarEyeBrows.transform.childCount != 0)
                {
                    for (int i = 0; i < InventoryManager.instance.ParentOfBtnsAvatarEyeBrows.transform.childCount; i++)
                    {
                        InventoryManager.instance.ParentOfBtnsAvatarEyeBrows.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
                if (InventoryManager.instance.ParentOfBtnsAvatarHairs.transform.childCount != 0)
                {
                    for (int i = 0; i < InventoryManager.instance.ParentOfBtnsAvatarHairs.transform.childCount; i++)
                    {
                        InventoryManager.instance.ParentOfBtnsAvatarHairs.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
                //InventoryManager.instance.SubmitAllItemswithSpecificSubCategory(InventoryManager.instance.SubCategoriesList[Index + 8].id, true);      // AR changes
                
            }
            InventoryManager.instance.UpdateStoreSelection(Index);
            // If click on the same panel Do Nothing & return
            return;
        }
        // Items which are not downloaded stop them to download
        // because new category is opened
        InventoryManager.instance.StopAllCoroutines();
        InventoryManager.instance.eyeBrowTapButton.SetActive(false);

        // InventoryManager.instance.DeletePreviousItems();            // AR changes
        
        if (gameObject.transform.parent.name != "Accesary")
        {
            if (PlayerPrefs.GetInt("presetPanel") == 1)
            {
                PlayerPrefs.SetInt("presetPanel", 0);  // was loggedin as account 
            }
        }


        ConstantsHolder.xanaConstants.currentButtonIndex = Index;
        InventoryManager.instance.UpdateXanaConstants();
        InventoryManager.instance.DisableColorPanels();

        if (Index == 7 && InventoryManager.instance.panelIndex == 1)
            InventoryManager.instance.OnColorButtonClicked(ConstantsHolder.xanaConstants.currentButtonIndex);
        else
            InventoryManager.instance.UpdateStoreSelection(Index);

        if (LoadingHandler.Instance)
            LoadingHandler.Instance.storeLoadingScreen.SetActive(false);
        //if (this.gameObject.activeInHierarchy)
        GetComponentInParent<SubBottons>().ClickBtnFtn(Index);
    }

    private void ButtonPressed()
    {
        StoreStackHandler.obj.UpdatePanelStatus(Index, true);    // AR changes
    }

}
