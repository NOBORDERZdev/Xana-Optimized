using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static StoreManager;

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
        StoreManager.instance.UpdateXanaConstants();
    }

    public void BtnClicked()
    {
        //// AR changes start
        if (BtnTxt.color != GetComponentInParent<SubBottons>().HighlightedColor)
        {
            if (!AR_UndoRedo.obj.addToList)
                AR_UndoRedo.obj.addToList = true;
            else
            {
                AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "BtnClicked", AR_UndoRedo.ActionType.ChangePanel, Color.white, categoryEnum);
            }
        }
        //// AR changes end


        if (XanaConstants.xanaConstants.currentButtonIndex == Index)
        {
            //    if (Index == 0)                                                  // AR Changes
            //        StoreManager.instance.ForcellySetLastClickedBtnOfHair();     // AR Changes

            if (StoreManager.instance.CloseColorPanel(Index) == true)
            {
                if (StoreManager.instance.ParentOfBtnsAvatarEyeBrows.transform.childCount != 0)
                {
                    for (int i = 0; i < StoreManager.instance.ParentOfBtnsAvatarEyeBrows.transform.childCount; i++)
                    {
                        StoreManager.instance.ParentOfBtnsAvatarEyeBrows.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
                if (StoreManager.instance.ParentOfBtnsAvatarHairs.transform.childCount != 0)
                {
                    for (int i = 0; i < StoreManager.instance.ParentOfBtnsAvatarHairs.transform.childCount; i++)
                    {
                        StoreManager.instance.ParentOfBtnsAvatarHairs.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
                //StoreManager.instance.SubmitAllItemswithSpecificSubCategory(StoreManager.instance.SubCategoriesList[Index + 8].id, true);      // AR changes
                StoreManager.instance.UpdateStoreSelection(Index);
            }
            // If click on the same panel Do Nothing & return
            return;
        }
        // Items which are not downloaded stop them to download
        // because new category is opened
        StoreManager.instance.StopAllCoroutines();
        StoreManager.instance.eyeBrowTapButton.SetActive(false);

        // StoreManager.instance.DeletePreviousItems();            // AR changes
        
        if (gameObject.transform.parent.name != "Accesary")
        {
            if (PlayerPrefs.GetInt("presetPanel") == 1)
            {
                PlayerPrefs.SetInt("presetPanel", 0);  // was loggedin as account 
            }
        }


        XanaConstants.xanaConstants.currentButtonIndex = Index;
        StoreManager.instance.UpdateXanaConstants();
        StoreManager.instance.DisableColorPanels();

        if (Index == 7 && StoreManager.instance.panelIndex == 1)
            StoreManager.instance.OnColorButtonClicked(XanaConstants.xanaConstants.currentButtonIndex);
        else
            StoreManager.instance.UpdateStoreSelection(Index);

        //if (this.gameObject.activeInHierarchy)
            GetComponentInParent<SubBottons>().ClickBtnFtn(Index);
    }

    private void ButtonPressed()
    {
        ActivePanelCallStack.obj.UpdatePanelStatus(Index, true);    // AR changes
    }

}
