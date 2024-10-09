using UnityEngine;

public class ActionSeeAllTabHandler : MonoBehaviour
{
    public int TabIndex = 0;

    [SerializeField] private EmoteReactionItemBtnHandler.ItemType _type;

    public void OpenTabSelected()
    {
        EmoteReactionUIHandler.ActivateHeighlightOfPanelBtn?.Invoke("ResetTabSelected");
        ActionManager.OpenActionCategoryTab?.Invoke(_type, TabIndex);
        EmoteReactionUIHandler.SetSeeAllTabSelectedReactionAction?.Invoke(_type, transform.name, TabIndex);
    }
}