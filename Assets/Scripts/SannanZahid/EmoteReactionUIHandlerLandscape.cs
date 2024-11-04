using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmoteReactionUIHandlerLandscape : EmoteReactionUIHandler
{
    public Transform EmotesSeeAllHolder;
    public Transform ReactionSeeAllHolder;
    public static Action<EmoteReactionItemBtnHandler.ItemType> DisplayActionDuplicateMessage;
    public static Action CloseDisplayDialogScrollView, ResetActionFavouritBtn;
    public Transform DuplicateMessage;
    public Transform ActionFavouritDialogObj;
    public List<Transform> SeeAllEmoteTabBtn = new List<Transform>();
    public List<Transform> SeeAllReactionTabBtn = new List<Transform>();
    public List<Transform> ActionFavouritCircleBtn = new List<Transform>();

    protected override void Awake()
    {
        base.Awake();
        DisplayActionDuplicateMessage += DuplicateActionMessage;
        CloseDisplayDialogScrollView += CloseActionDisplayDialogScroll;
        ResetActionFavouritBtn += ResetActionFavouriteBtnState;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        DisplayActionDuplicateMessage -= DuplicateActionMessage;
        CloseDisplayDialogScrollView -= CloseActionDisplayDialogScroll;
        ResetActionFavouritBtn -= ResetActionFavouriteBtnState;

    }

    public void DuplicateActionMessage(EmoteReactionItemBtnHandler.ItemType type)
    {
        StartCoroutine(ActivateDuplicateActionMessage(type));
    }

    public void ShowAllBtnClick()
    {
        if (SelectedAction == EmoteReactionItemBtnHandler.ItemType.Emote)
        {
            EmotesSeeAllHolder.gameObject.SetActive(true);
            ReactionSeeAllHolder.gameObject.SetActive(false);
        }
        else
        {
            EmotesSeeAllHolder.gameObject.SetActive(false);
            ReactionSeeAllHolder.gameObject.SetActive(true);
        }
    }
    public void ResetActionFavouriteBtnState()
    {
        foreach (Transform item in ActionFavouritCircleBtn)
        {
            item.GetComponent<ActionFavouriteCircleBtn>().ClearActionButtonData();
        }
    }
    public override void CloseActionDisplayDialogScroll()
    {
        base.CloseActionDisplayDialogScroll();
        ActionFavouritDialogObj.gameObject.SetActive(false);
        foreach (Transform item in ActionFavouritCircleBtn)
        {
            item.GetComponent<ActionFavouriteCircleBtn>().InitializeBtn();
        }
    }

    public void SetHeighlightSeeAllEmote(int index)
    {
        for (int i = 0; i < SeeAllEmoteTabBtn.Count; i++)
        {
            if (i == index)
            {
                SeeAllEmoteTabBtn[i].GetComponentInChildren<Text>().color = SelectedColorTab;
            }
            else
            {
                SeeAllEmoteTabBtn[i].GetComponentInChildren<Text>().color = UnSelectedColorTab;
            }
        }
    }

    public void SetHeighlightSeeAllReaction(int index)
    {
        for (int i = 0; i < SeeAllReactionTabBtn.Count; i++)
        {
            if (i == index)
            {
                SeeAllReactionTabBtn[i].GetComponentInChildren<Text>().color = SelectedColorTab;
            }
            else
            {
                SeeAllReactionTabBtn[i].GetComponentInChildren<Text>().color = UnSelectedColorTab;
            }
        }
    }

    private IEnumerator ActivateDuplicateActionMessage(EmoteReactionItemBtnHandler.ItemType type)
    {
        if(type== EmoteReactionItemBtnHandler.ItemType.Emote)
        {
            DuplicateMessage.GetComponent<TMP_Text>().text = "Duplicate Animation";
            DuplicateMessage.GetComponent<TextLocalization>().LocalizeTextText(DuplicateMessage.GetComponent<TMP_Text>().text);
        }
        else
        {
            DuplicateMessage.GetComponent<TMP_Text>().text = "Duplicate Reaction";
            DuplicateMessage.GetComponent<TextLocalization>().LocalizeTextText(DuplicateMessage.GetComponent<TMP_Text>().text);
        }
        DuplicateMessage.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        DuplicateMessage.gameObject.SetActive(false);
    }
 
}