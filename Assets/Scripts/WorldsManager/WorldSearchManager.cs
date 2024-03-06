using AdvancedInputFieldPlugin;
using System;
using UnityEngine;

public class WorldSearchManager : MonoBehaviour
{
    public AdvancedInputField searchWorldInput;
    public static Action<string> OpenSearchPanel;
    public static Action<string> SearchWorld;

    public static bool IsSearchBarActive = false;
    private void OnEnable()
    {
        searchWorldInput.OnValueChanged.AddListener(UserInput => UserInputUpdate(UserInput)) ;
        SearchWorld += OpenSearchPanelFromTag;
    }

    private void OnDisable()
    {
        SearchWorld -= OpenSearchPanelFromTag;
    }
    public void UserInputUpdate(string UserInput)
    {
        WorldManager.instance.SearchWorldCall(UserInput);
    }
    public void ClearInputField()
    {
        searchWorldInput.Clear();
        WorldManager.instance.AllWorldTabReference.BackToPreviousScreen();
        FlexibleRect.OnAdjustSize?.Invoke(false);
    }
    public void GetSearchBarStatus()
    {
        Debug.Log("GetSearchBarStatus => " + PremiumUsersDetails.Instance.CheckSpecificItem("WorldSearchFeature"));
        searchWorldInput.Select();
        searchWorldInput.Clear();
    }


    void OpenSearchPanelFromTag(string tagName)
    {
        searchWorldInput.Clear();
        searchWorldInput.SetText(tagName);
        searchWorldInput.ManualDeselect();
        WorldManager.instance.previousSearchKey = string.Empty;
        //searchWorldInput.ReadOnly = true;
        WorldManager.instance.SearchWorldCall(tagName,true);
    }
}