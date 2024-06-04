using System;
using System.Collections;
using System.Collections.Generic;
using UFE3D;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public EmoteManager EmoteManager;
    public ReactionManager ReactionManager;
    public ActionFavouriteManager ActionFavouriteManager;
    public static Action<ActionData> ActionBtnClick;
    public static Action<bool> OpenActionFavouritPanel;

    private void OnEnable()
    {
        ActionBtnClick += ProcessAction;
        OpenActionFavouritPanel += OpenActionFavouritSelectionPanel;
    }
    private void OnDisable()
    {
        ActionBtnClick -= ProcessAction;
        OpenActionFavouritPanel -= OpenActionFavouritSelectionPanel;
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
        if(ActionFavouriteManager.isActiveAndEnabled)
        {
            /// Save Action
            ActionFavouriteManager.SetFavouriteAction(dataObj);
        }
        else
        {
            /// Run action on player
        }
    }
}
[Serializable]
public class ActionData
{
    public string AnimationName;
    public string ThumbnailURL;
    public EmoteReactionItemBtnHandler.ItemType TypeOfAction;
}