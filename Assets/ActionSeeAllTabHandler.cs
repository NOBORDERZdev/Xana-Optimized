using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSeeAllTabHandler : MonoBehaviour
{
    public int TabIndex = 0;
    [SerializeField] EmoteReactionItemBtnHandler.ItemType _type;
    public void OpenTabSelected()
    {
        Debug.LogError("OpenTabSelected Seeall ----> " + _type.ToString());
        ActionManager.OpenActionCategoryTab?.Invoke(_type, TabIndex);
        EmoteReactionUIHandler.SetSeeAllTabSelectedReactionAction?.Invoke(_type, transform.name, TabIndex);
        //if (_type == EmoteReactionItemBtnHandler.ItemType.Emote)
        //{
        //    EmoteReactionUIHandler.SetTabSelectedEmoteAction?.Invoke(TabIndex);
        //}
        //else
        //{
        //    EmoteReactionUIHandler.SetTabSelectedReactionAction?.Invoke(TabIndex);
        //}
    }
}