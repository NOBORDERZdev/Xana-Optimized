using AdvancedInputFieldPlugin;
using UnityEngine;

public class WorldSearchManager : MonoBehaviour
{
    public AdvancedInputField searchWorldInput;
    private void OnEnable()
    {
        searchWorldInput.OnValueChanged.AddListener(UserInput => UserInputUpdate(UserInput)) ;
    }
    public void UserInputUpdate(string UserInput)
    {
       // Debug.LogError("Search  === " + UserInput);
        WorldManager.instance.SearchWorldCall(UserInput);
    }
    public void ClearInputField()
    {
        searchWorldInput.Clear();
        WorldManager.instance.AllWorldTabReference.BackToPreviousScreen();
    }
    public void GetSearchBarStatus()
    {
        Debug.Log("GetSearchBarStatus => " + PremiumUsersDetails.Instance.CheckSpecificItem("WorldSearchFeature"));
        searchWorldInput.Select();
        searchWorldInput.Clear();
    }
}