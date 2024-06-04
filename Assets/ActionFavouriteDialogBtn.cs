using SuperStar.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CustomNavigation;

public class ActionFavouriteDialogBtn : MonoBehaviour
{
    [SerializeField] Transform _highLightObj;
    [SerializeField] Transform _CrossBtnObj;
    [SerializeField] private Image _ActionImg;
    public string AnimationName;
    public string ThumbnailURL;
    public EmoteReactionItemBtnHandler.ItemType TypeOfAction;
    public int IndexOfBtn = 0;
    private void OnEnable()
    {
        InitializeBtn();
    }
    private void InitializeBtn()
    {
        if (PlayerPrefsUtility.GetEncryptedString(ConstantsGod.ANIMATION_DATA + IndexOfBtn) != "")
        {
            ActionData actionData = JsonUtility.FromJson<ActionData>(PlayerPrefsUtility.GetEncryptedString(ConstantsGod.ANIMATION_DATA + IndexOfBtn));
            AnimationName = actionData.AnimationName;
            TypeOfAction = actionData.TypeOfAction;
            ThumbnailURL = actionData.ThumbnailURL;
            LoadImageFromURL();
        }
    }
    private void LoadImageFromURL()
    {
        if (ThumbnailURL != "")
        {
            AssetCache.Instance.EnqueueOneResAndWait(ThumbnailURL, ThumbnailURL, (success) =>
            {
                if (success)
                {
                    AssetCache.Instance.LoadSpriteIntoImage(_ActionImg, ThumbnailURL, changeAspectRatio: true);
                    _ActionImg.gameObject.SetActive(true);
                    _CrossBtnObj.gameObject.SetActive(true);
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
        if (ThumbnailURL != "")
        {
           ///>--MemoryClean Stoped AssetCache.Instance.RemoveFromMemoryDelayCoroutine(ThumbnailURL, true);
            _ActionImg.gameObject.SetActive(false);
            _CrossBtnObj.gameObject.SetActive(false);
        }
    }
    public void EnableHighLightObj(bool flag)
    {
        _highLightObj.gameObject.SetActive(flag);
    }
    public bool ValidateSimilarData(string animationName, EmoteReactionItemBtnHandler.ItemType typeOfAction)
    {
        if(animationName == AnimationName && typeOfAction == TypeOfAction)
        {
            return true;
        }
        return false;
    }
    public bool ValidateSelectionOfFavouriteAction(bool flag)
    {
        if (AnimationName == "" && flag)
        {
            EnableHighLightObj(true);
            return true;
        }
        EnableHighLightObj(false);
        return false;
    }
    public void SetupActionSelected(ActionData dataObj)
    {
        AnimationName = dataObj.AnimationName;
        TypeOfAction = dataObj.TypeOfAction;
        ThumbnailURL = dataObj.ThumbnailURL;
        LoadImageFromURL();
    }
    public void CancelSelectedAction()
    {
        _ActionImg.sprite = default;
        AnimationName = default;
        ThumbnailURL = default;
        _ActionImg.gameObject.SetActive(false);
        _CrossBtnObj.gameObject.SetActive(false);
    }
    public void SaveActionSelected()
    {
        if (AnimationName != "")
        {
            ActionData actionData = new ActionData();
            actionData.AnimationName = AnimationName;
            actionData.TypeOfAction = TypeOfAction;
            actionData.ThumbnailURL = ThumbnailURL;
            PlayerPrefsUtility.SetEncryptedString(ConstantsGod.ANIMATION_DATA + IndexOfBtn, JsonUtility.ToJson(actionData).ToString());
        }
        else
        {
            PlayerPrefsUtility.SetEncryptedString(ConstantsGod.ANIMATION_DATA + IndexOfBtn, "");
        }
    }
}
