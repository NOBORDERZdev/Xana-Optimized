using SuperStar.Helpers;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EmoteReactionItemBtnHandler : MonoBehaviour
{
    public enum ItemType {Emote, Reaction}
    public ItemType TypeOfAction = ItemType.Reaction;
    public int Id;
    public string ActionName;
    public string ActionThumbnail_Url;
    public string ActionGroupType;
    public Image BtnImg;
    public TMP_Text NameTxt;
    public Transform HeighlightObj;

    private void OnDisable()
    {
        ///>--MemoryClean Stoped  AssetCache.Instance.RemoveFromMemoryDelayCoroutine(ActionThumbnail_Url, true);
        if(HeighlightObj != null)
            HeighlightObj.gameObject.SetActive(false);
    }

    public void InitializeItem(ItemType _type, int _id, string _actionName, string _actionThumbnail_Ur, string _actionGroupType)
    {
        TypeOfAction = _type ;
        Id = _id;
        ActionName = _actionName;
        ActionThumbnail_Url = _actionThumbnail_Ur;
        ActionGroupType= _actionGroupType;

        if(TypeOfAction == ItemType.Emote)
        {
            //NameTxt.text = ""+ ActionName;
            NameTxt.text = ItemNameLocalization(ActionName);

        }
        else
        {
            NameTxt.text = string.Empty;
        }
        GetImageFromServer();
    }
    string ItemNameLocalization(string _itemName)
    {
        string itemName = _itemName;

        // Check if the last character is a digit
        if (char.IsDigit(itemName[itemName.Length - 1]))
        {
            // Find the position where the digits start from the end
            int digitStartIndex = itemName.Length - 1;

            // Loop backwards until a non-digit character is found
            while (digitStartIndex >= 0 && char.IsDigit(itemName[digitStartIndex]))
            {
                digitStartIndex--;
            }

            // Separate the name and the number
            string namePart = itemName.Substring(0, digitStartIndex + 1);
            string digitPart = itemName.Substring(digitStartIndex + 1);

            // Output the result
            //Debug.Log($"Name: {namePart}, Digit: {digitPart}");
            return TextLocalization.GetLocaliseTextByKey(namePart) + digitPart;
        }
        else
        {
            //Debug.Log("The last character is not a digit.");
            return TextLocalization.GetLocaliseTextByKey(itemName);
        }
    }
    public void ApplyAction()
    {
        ActionData dataObj = new ActionData();
        dataObj.AnimationName = ActionName;
        dataObj.ThumbnailURL = ActionThumbnail_Url;
        dataObj.TypeOfAction = TypeOfAction;
        ActionManager.ActionBtnClick?.Invoke(dataObj);
        EmoteReactionUIHandler.ActivateHeighlightOfPanelBtn?.Invoke(ActionName);
    }

    private void GetImageFromServer()
    {
        if (ActionThumbnail_Url != "")
        {
            AssetCache.Instance.EnqueueOneResAndWait(ActionThumbnail_Url, ActionThumbnail_Url, (success) =>
            {
                if (success)
                {
                    AssetCache.Instance.LoadSpriteIntoImage(BtnImg, ActionThumbnail_Url, changeAspectRatio: true);
                }
                else
                {
                    Debug.Log("Download Failed");
                }
            });
        }
    }
}
