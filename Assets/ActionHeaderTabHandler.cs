using UnityEngine;
using UnityEngine.UI;

public class ActionHeaderTabHandler : MonoBehaviour
{
    public int TabIndex = 0;
    public int TabSelectedIndex = 0;

    [SerializeField] private Text _tabNameTxt;
    [SerializeField] private EmoteReactionItemBtnHandler.ItemType _type;

    public void SetTabDetails(int index, string nameOfActionCategory)
    {
        TabIndex = index;
        _tabNameTxt.text = nameOfActionCategory;
    }

    public void OpenTabSelected()
    {
        ActionManager.OpenActionCategoryTab?.Invoke(_type, TabIndex);

        if(_type == EmoteReactionItemBtnHandler.ItemType.Emote)
        {
            EmoteReactionUIHandler.SetTabSelectedEmoteAction?.Invoke(TabIndex, TabSelectedIndex);
        }
        else
        {
            EmoteReactionUIHandler.SetTabSelectedReactionAction?.Invoke(TabIndex, TabSelectedIndex);
        }

        EmoteReactionUIHandler.ActivateHeighlightOfPanelBtn?.Invoke("ResetTabSelected");
    }
}