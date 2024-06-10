using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionHeaderTabHandler : MonoBehaviour
{
    public int TabIndex = 0;
    public int TabSelectedIndex = 0;
    [SerializeField] Text _tabNameTxt;
    [SerializeField] EmoteReactionItemBtnHandler.ItemType _type;
    public void SetTabDetails(int index, string nameOfActionCategory)
    {
        TabIndex = index;
        _tabNameTxt.text = nameOfActionCategory;
    }
    public void OpenTabSelected()
    {
        Debug.LogError("OpenTabSelected ----> " + _type.ToString());
        ActionManager.OpenActionCategoryTab?.Invoke(_type, TabIndex);
        if(_type == EmoteReactionItemBtnHandler.ItemType.Emote)
        {
            EmoteReactionUIHandler.SetTabSelectedEmoteAction?.Invoke(TabIndex, TabSelectedIndex);
        }
        else
        {
            EmoteReactionUIHandler.SetTabSelectedReactionAction?.Invoke(TabIndex, TabSelectedIndex);
        }
    }
}