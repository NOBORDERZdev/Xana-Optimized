using SuperStar.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmoteReactionItemBtnHandler : MonoBehaviour
{
    public enum ItemType {Emote, Reaction}
    public ItemType TypeOfAction = ItemType.Reaction;
    public int Id;
    public string ActionName;
    public string ActionThumbnail_Url;
    public string ActionGroupType;
    public Image BtnImg;

    public void InitializeItem(ItemType _type, int _id, string _actionName, string _actionThumbnail_Ur, string _actionGroupType)
    {
        TypeOfAction = ItemType.Reaction;
        Id = _id;
        ActionName = _actionName;
        ActionThumbnail_Url = _actionThumbnail_Ur;
        ActionGroupType= _actionGroupType;
    }
    private void OnEnable()
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
    private void OnDisable()
    {
        AssetCache.Instance.RemoveFromMemoryDelayCoroutine(ActionThumbnail_Url, true);
    }
    public void ApplyAction()
    {

    }
}
