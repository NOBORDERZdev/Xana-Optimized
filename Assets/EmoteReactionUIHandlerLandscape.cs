using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteReactionUIHandlerLandscape : EmoteReactionUIHandler
{
    public Transform EmotesSeeAllHolder;
    public Transform ReactionSeeAllHolder;
    public static Action DisplayActionDuplicateMessage;
    public static Action CloseDisplayDialogScrollView;
    public Transform DuplicateMessage;
   [SerializeField] public Transform ActionFavouritDialogObj;
    public List<Transform> ActionFavouritCircleBtn= new List<Transform> ();
    private void OnEnable()
    {
        DisplayActionDuplicateMessage += DuplicateActionMessage;
        CloseDisplayDialogScrollView += CloseActionDisplayDialogScroll;
    }
    private void OnDisable()
    {
        DisplayActionDuplicateMessage -= DuplicateActionMessage;
        CloseDisplayDialogScrollView -= CloseActionDisplayDialogScroll;
    }
    public void DuplicateActionMessage()
    {
        StartCoroutine(ActivateDuplicateActionMessage());
    }
    IEnumerator ActivateDuplicateActionMessage()
    {
        DuplicateMessage.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        DuplicateMessage.gameObject.SetActive(false);
    }
    public void ShowAllBtnClick()
    {
        if(SelectedAction == EmoteReactionItemBtnHandler.ItemType.Emote)
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
    public override void CloseActionDisplayDialogScroll()
    {
        base.CloseActionDisplayDialogScroll();
        ActionFavouritDialogObj.gameObject.SetActive(false);
        foreach(Transform item in ActionFavouritCircleBtn)
        {
            item.GetComponent<ActionFavouriteCircleBtn>().InitializeBtn();
        }
    }
}
