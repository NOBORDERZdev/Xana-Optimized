using AdvancedInputFieldPlugin;
using UnityEngine;

public class WorldSearchManager : MonoBehaviour
{
    public AdvancedInputField searchWorldInput;
    public GameObject FindWorldScreen;

    private void OnEnable()
    {
        searchWorldInput.OnValueChanged.AddListener(UserInput => UserInputUpdate(UserInput)) ;
    }
    public void UserInputUpdate(string UserInput)
    {
        WorldManager.instance.SearchWorldCall(UserInput);
    }
    public void GetSearchBarStatus()
    {
        Debug.Log("GetSearchBarStatus => " + PremiumUsersDetails.Instance.CheckSpecificItem("WorldSearchFeature"));
        FindWorldScreen.SetActive(true);
        searchWorldInput.Select();
        searchWorldInput.Clear();
    }
}