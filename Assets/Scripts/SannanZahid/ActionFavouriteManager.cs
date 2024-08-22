using UnityEngine;

public class ActionFavouriteManager : MonoBehaviour
{
    public Transform ActionFavouritDialogObj;
    public Transform ActionCircleDialog;
    public bool IsInActionSelection;
    public ActionFavouritSelectionHandler ActionFavouritSelectionHandler;

   public void ActivateCircleDialog(bool enableFlag)
   {
        ActionCircleDialog.gameObject.SetActive(enableFlag);
   }
   public void ActivateActionFavouritDialogObj(bool enableFlag)
   {
       IsInActionSelection = enableFlag;
       ActionFavouritDialogObj.gameObject.SetActive(enableFlag);

       if (enableFlag)
       { 
          ActionFavouritSelectionHandler.OpenEmoteDialog(); 
       }
   }
   public void SetFavouriteAction(ActionData dataObj)
   {
       if(ActionFavouritSelectionHandler.IsValidActionToSave(dataObj))
       {
           ActionFavouritSelectionHandler.SetActionToFavouritSelectedByPlayer(dataObj);
       }
       else
       {
           EmoteReactionUIHandlerLandscape.DisplayActionDuplicateMessage?.Invoke(dataObj.TypeOfAction);
       }
   }
}