using System;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public EmoteManager EmoteManager;
    public ReactionManager ReactionManager;
    public ActionFavouriteManager ActionFavouriteManager;
    public static Action<ActionData> ActionBtnClick;
    public static Action<bool> OpenActionFavouritPanel;
    public static Action DisableCircleDialog;
    public static Action DisableFavoriteCircleHighlighter;
    public static Action<EmoteReactionItemBtnHandler.ItemType, int> OpenActionCategoryTab;
    public static Action StopActionAnimation;
    public static bool IsAnimRunning = default;
    private bool _actionBtnActive = false;

    private void OnEnable()
    {
        ActionBtnClick += ProcessAction;
        OpenActionFavouritPanel += OpenActionFavouritSelectionPanel;
        OpenActionCategoryTab += SetActionCategoryTab;
        StopActionAnimation += StopAnimation;
        DisableCircleDialog += CloseActionCircleDialog;
    }

    private void OnDisable()
    {
        ActionBtnClick -= ProcessAction;
        OpenActionFavouritPanel -= OpenActionFavouritSelectionPanel;
        OpenActionCategoryTab -= SetActionCategoryTab;
        StopActionAnimation -= StopAnimation;
        DisableCircleDialog -= CloseActionCircleDialog;
    }

    private void Start()
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
    public void CloseActionCircleDialog()
    {
        _actionBtnActive = false;
        ActionFavouriteManager.ActivateCircleDialog(false);
    }
    public void OpenActionCircleDialog()
    {
        _actionBtnActive = !_actionBtnActive;

        if (_actionBtnActive)
        {
            ActionFavouriteManager.ActivateCircleDialog(true);
        }
        else
        {
            ActionFavouriteManager.ActivateCircleDialog(false);
        }
    }

    public void OpenActionFavouritSelectionPanel(bool flag)
    {
        ActionFavouriteManager.ActivateActionFavouritDialogObj(flag);
    }

    public void ProcessAction(ActionData dataObj)
    {
        if(ActionFavouriteManager.IsInActionSelection)
        {
            ActionFavouriteManager.SetFavouriteAction(dataObj);
        }
        else
        {
            if(dataObj.TypeOfAction == EmoteReactionItemBtnHandler.ItemType.Emote)
            {
                this.transform.GetComponent<ActionAnimationApplyToPlayer>().LoadAnimationAccrossInstance(dataObj.AnimationName);
                if(!GamePlayUIHandler.inst.AnimationBtnClose.activeInHierarchy)
                    GamePlayUIHandler.inst.AnimationBtnClose.SetActive(true);
                IsAnimRunning = true;
            }
            else
            {
                PlayerPrefs.SetString(ConstantsGod.ReactionThumb, dataObj.ThumbnailURL);
                ArrowManager.OnInvokeReactionButtonClickEvent(PlayerPrefs.GetString(ConstantsGod.ReactionThumb));
            }
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
        IsAnimRunning = false;
        EmoteReactionUIHandler.lastEmotePlayed = null;
        this.transform.GetComponent<ActionAnimationApplyToPlayer>().StopAnimation();

        //Remove emote highlighter when animation stop
        EmoteReactionUIHandler.ActivateHeighlightOfPanelBtn?.Invoke("");
        ActionManager.DisableFavoriteCircleHighlighter?.Invoke();
    }
}

[Serializable]
public class ActionData
{
    public string AnimationName;
    public string ThumbnailURL;
    public EmoteReactionItemBtnHandler.ItemType TypeOfAction;
}