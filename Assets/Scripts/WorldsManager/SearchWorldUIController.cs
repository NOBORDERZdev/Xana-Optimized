using AdvancedInputFieldPlugin;
using Photon.Pun.Demo.PunBasics;
using System;
using TMPro;
using UnityEngine;

public class SearchWorldUIController : MonoBehaviour
{
    public AdvancedInputField searchWorldInput;
    public static Action<string> OpenSearchPanel;
    public static Action<string> SearchWorld;
    public static Action AutoSelectInputField;

    public static bool IsSearchBarActive = false;
    private void OnEnable()
    {
        searchWorldInput.OnValueChanged.AddListener(UserInput => UserInputUpdate(UserInput)) ;
        SearchWorld += OpenSearchPanelFromTag;
        AutoSelectInputField += ManualSelectInputField;
        MainSceneEventHandler.BackHomeSucessfully += ReAssignActions;
    }

    private void OnDisable()
    {
        SearchWorld -= OpenSearchPanelFromTag;
        AutoSelectInputField -= ManualSelectInputField;
        MainSceneEventHandler.BackHomeSucessfully -= ReAssignActions;
    }

    void ReAssignActions()
    {
        for (int i = 0; i < searchWorldInput.OnValueChanged.GetPersistentEventCount(); i++)
        {
            if (searchWorldInput.OnValueChanged.GetPersistentMethodName(i) == nameof(UserInputUpdate))
                return;
        }
        searchWorldInput.OnValueChanged.AddListener(UserInput => UserInputUpdate(UserInput));
    }
    public void UserInputUpdate(string UserInput)
    {
        WorldManager.instance.SearchWorldCall(UserInput);
    }
    public void ClearInputField()
    {
        searchWorldInput.Clear();
        WorldManager.instance.WorldScrollReset();
        //FlexibleRect.OnAdjustSize?.Invoke(false);
    }
    public void GetSearchBarStatus()
    {
        Debug.Log("GetSearchBarStatus => " + UserPassManager.Instance.CheckSpecificItem("WorldSearchFeature"));
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

    public void ManualSelectInputField()
    {
        searchWorldInput.ManualSelect();
    }

}