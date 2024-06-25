using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UFE3D;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.tvOS;

public class ActionManager : MonoBehaviour
{
    public EmoteManager EmoteManager;
    public ReactionManager ReactionManager;
    public ActionFavouriteManager ActionFavouriteManager;
    public static Action<ActionData> ActionBtnClick;
    public static Action<bool> OpenActionFavouritPanel;
    public static Action<EmoteReactionItemBtnHandler.ItemType, int> OpenActionCategoryTab;
    public static Action StopActionAnimation;
    public static bool IsAnimRunning = default;


    private void OnEnable()
    {
        ActionBtnClick += ProcessAction;
        OpenActionFavouritPanel += OpenActionFavouritSelectionPanel;
        OpenActionCategoryTab += SetActionCategoryTab;
        StopActionAnimation += StopAnimation;
    }
    private void OnDisable()
    {
        ActionBtnClick -= ProcessAction;
        OpenActionFavouritPanel -= OpenActionFavouritSelectionPanel;
        OpenActionCategoryTab -= SetActionCategoryTab;
        StopActionAnimation -= StopAnimation;
    }
    public void Start()
    {
        EmoteManager.GetServerData();
        ReactionManager.GetServerData();
    }
    public void OpenEmoteDialogUI()
    {
        EmoteManager.OpenEmoteDialogUI();
    }
    public void OpenReactionDialogUI()
    {
        ReactionManager.OpenReactionDialogUI();
    }
    public void OpenActionCircleDialog()
    {
        ActionFavouriteManager.ActivateCircleDialog(true);
    }
    public void OpenActionFavouritSelectionPanel(bool flag)
    {
        ActionFavouriteManager.ActivateActionFavouritDialogObj(flag);
    }
    public void ProcessAction(ActionData dataObj)
    {
        Debug.LogError("----- ProcessAction --- >  " + ActionFavouriteManager.IsInActionSelection);
        if(ActionFavouriteManager.IsInActionSelection)
        {
            /// Save Action
            ActionFavouriteManager.SetFavouriteAction(dataObj);
        }
        else
        {
            if(dataObj.TypeOfAction == EmoteReactionItemBtnHandler.ItemType.Emote)
            {
                this.transform.GetComponent<ActionAnimationApplyToPlayer>().LoadAnimationAccrossInstance(dataObj.AnimationName);
                IsAnimRunning = true;
            }
            else
            {
                PlayerPrefs.SetString(ConstantsGod.ReactionThumb, dataObj.ThumbnailURL);
                ArrowManager.OnInvokeReactionButtonClickEvent(PlayerPrefs.GetString(ConstantsGod.ReactionThumb));
            }
            Debug.LogError("----- Play Animation ----");
            /// Run action on player
        }
    }
    public void SetActionCategoryTab(EmoteReactionItemBtnHandler.ItemType itemType, int categoryIndex)
    {
        if(itemType == EmoteReactionItemBtnHandler.ItemType.Emote)
        {
            EmoteManager.OpenEmoteDialogUITabClick(categoryIndex);
        }
        else
        {
            ReactionManager.OpenReactionDialogUITabClick(categoryIndex);
        }
    }
    public void StopAnimation()
    {
        Debug.LogError("----- STOPPPPP Animation ----");
        IsAnimRunning = false;
        this.transform.GetComponent<ActionAnimationApplyToPlayer>().StopAnimation();
    }
}
[Serializable]
public class ActionData
{
    public string AnimationName;
    public string ThumbnailURL;
    public EmoteReactionItemBtnHandler.ItemType TypeOfAction;
}