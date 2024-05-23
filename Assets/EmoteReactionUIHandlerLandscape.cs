using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteReactionUIHandlerLandscape : EmoteReactionUIHandler
{
    public Transform EmotesSeeAllHolder;
    public Transform ReactionSeeAllHolder;
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
}
