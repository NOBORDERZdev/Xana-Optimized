using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Mozilla;
using SuperStar.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionFavouriteCircleBtn : MonoBehaviour
{
    public bool IsActionSelected = false;
    public int IndexOfBtn = 0;
    private bool _actionSelected = false;
    [SerializeField] private Image _ActionImg;
    public string AnimationName;
    public string ThumbnailURL;

    public EmoteReactionItemBtnHandler.ItemType TypeOfAction;

    private void OnEnable()
    {
        InitializeBtn();
    }
    private void OnDisable()
    {
        if(ThumbnailURL != "")
        {
            ///>--MemoryClean Stoped AssetCache.Instance.RemoveFromMemoryDelayCoroutine(ThumbnailURL, true);
        }
    }
    public void InitializeBtn()
    {
        Debug.LogError("InitializeBtn -----> ");
        if(PlayerPrefsUtility.GetEncryptedString(ConstantsGod.ANIMATION_DATA + IndexOfBtn) != "")
        {
            ActionData actionData = JsonUtility.FromJson<ActionData>(PlayerPrefsUtility.GetEncryptedString(ConstantsGod.ANIMATION_DATA + IndexOfBtn));
            AnimationName = actionData.AnimationName;
            TypeOfAction = actionData.TypeOfAction;
            ThumbnailURL = actionData.ThumbnailURL;
            LoadImageFromURL();
        }
        else
        {
            AnimationName = default;
            ThumbnailURL = default;
            _ActionImg.sprite = default;
            _ActionImg.gameObject.SetActive(false);
        }
    }
    public void SelectedAction(ActionData dataObj)
    {
        PlayerPrefsUtility.SetEncryptedString(ConstantsGod.ANIMATION_DATA + IndexOfBtn, 
            JsonUtility.ToJson(dataObj).ToString());
    }
    public void ClearSelectedActionData()
    {
        PlayerPrefsUtility.SetEncryptedString(ConstantsGod.ANIMATION_DATA + IndexOfBtn,"");
    }
    public void PlayerActionInteraction()
    {
        if(_actionSelected)
        {
            // Run action on player
            Debug.LogError("PlayerActionInteraction ----> Play Action");
        }
        else
        {
            ActionManager.OpenActionFavouritPanel.Invoke(true);
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
                }
                else
                {
                    Debug.Log("Download Failed");
                }
            });
        }
    }
}


