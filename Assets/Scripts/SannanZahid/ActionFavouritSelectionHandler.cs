using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionFavouritSelectionHandler : MonoBehaviour
{
    public List<Transform> ActionFavouritBtn = new List<Transform>();

    [SerializeField] private ActionManager _actionManager;
    [SerializeField] private Transform _reationBtn;
    [SerializeField] private Transform _emoteBtn;
    private int _selectActionFavouritBtnIndex = 0;

    private void OnEnable()
    {
        SetSelectionOfFavouriteBtn();
    }

    public void OpenReactionDialog()
    {
        SetReactionSelection(false);
        SetEmoteSelection(true);
        _actionManager.OpenReactionDialogUI();
    }

    public void OpenEmoteDialog()
    {
        SetReactionSelection(true);
        SetEmoteSelection(false);
        _actionManager.OpenEmoteDialogUI();
    }

    public void SetActionToFavouritSelectedByPlayer(ActionData dataObj)
    {
        ActionFavouritBtn[_selectActionFavouritBtnIndex].GetComponent<ActionFavouriteDialogBtn>().SetupActionSelected(dataObj);
    }

    public void SetActionToFavouritHighlight(int indexOfActionSelected)
    {
        _selectActionFavouritBtnIndex = indexOfActionSelected;
        for (int i = 0; i < ActionFavouritBtn.Count; i++)
        {
            if (i == _selectActionFavouritBtnIndex)
            {
                ActionFavouritBtn[i].GetComponent<ActionFavouriteDialogBtn>().EnableHighLightObj(true);
            }
            else
            {
                ActionFavouritBtn[i].GetComponent<ActionFavouriteDialogBtn>().EnableHighLightObj(false);
            }
        }
    }

    public void ResetActionBtnCall()
    {
        foreach (Transform item in ActionFavouritBtn)
        {
            item.GetComponent<ActionFavouriteDialogBtn>().CancelSelectedAction();
        }
        EmoteReactionUIHandlerLandscape.ResetActionFavouritBtn?.Invoke();
    }

    public void SaveActionBtnCall()
    {
        foreach (Transform item in ActionFavouritBtn)
        {
            item.GetComponent<ActionFavouriteDialogBtn>().SaveActionSelected();
        }
        EmoteReactionUIHandlerLandscape.CloseDisplayDialogScrollView?.Invoke();
        ActionManager.OpenActionFavouritPanel.Invoke(false);

    }

    public bool IsValidActionToSave(ActionData dataObj)
    {
        foreach (Transform item in ActionFavouritBtn)
        {
            if (item.GetComponent<ActionFavouriteDialogBtn>().ValidateSimilarData(dataObj.AnimationName, dataObj.TypeOfAction))
            {
                return false;
            }
        }
        return true;
    }

    private void CloseAllActionDialog()
    {
        SetReactionSelection(false);
        SetEmoteSelection(false);
        _actionManager.OpenEmoteDialogUI();
    }

    private void SetReactionSelection(bool flag)
    {
        _reationBtn.GetComponent<Image>().enabled = flag;
        _reationBtn.GetChild(0).GetComponent<Image>().enabled = !flag;
    }

    private void SetEmoteSelection(bool flag)
    {
        _emoteBtn.GetComponent<Image>().enabled = flag;
        _emoteBtn.GetChild(0).GetComponent<Image>().enabled = !flag;
    }

    private void SetSelectionOfFavouriteBtn()
    {
        bool flag = false;
        for(int i = 0; i < ActionFavouritBtn.Count; i++)
        {
            if (ActionFavouritBtn[i].GetComponent<ActionFavouriteDialogBtn>().ValidateSelectionOfFavouriteAction(flag))
            {
                flag = true;
                _selectActionFavouritBtnIndex = i;
            }
        }
        if(!flag)
        {
            ActionFavouritBtn[0].GetComponent<ActionFavouriteDialogBtn>().EnableHighLightObj(true);
            _selectActionFavouritBtnIndex = 0;
        }
    }
}