using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using AdvancedInputFieldPlugin;

public class AllWorldManage : MonoBehaviour
{
    [Header("Home Page Scrollviews and Component")]
    public List<GameObject> HighlghterList = new List<GameObject>();
    public List<GameObject> HighlghterListText = new List<GameObject>();
    public List<GameObject> ScrollObjectsList = new List<GameObject>();
    public List<TextMeshProUGUI> ScrollerText = new List<TextMeshProUGUI>();
    public GameObject FlexibleReact;
    [Header("World Page Scrollviews and Component")]
    public Button searchButton;
    public TMP_InputField searchBar;
    public List<GameObject> worldPageScrollviews = new List<GameObject>();
    public List<GameObject> WorldPagehighlighters = new List<GameObject>();
    public List<GameObject> WorldPagehighlightersText = new List<GameObject>();
    public List<TextMeshProUGUI> WorldPageText = new List<TextMeshProUGUI>();

    public void SearchScreenLoad()
    {
        UIManager.Instance.SwitchToScreen(2);
        WorldManager.instance.ClearWorldScrollWorlds();
       // WorldManager.instance.ChangeWorldTab(APIURL.SearchWorld);
    }
    public void BackToPreviousScreen()
    {
        UIManager.Instance.SwitchToScreen(UIManager.Instance.PreviousScreen);
        WorldManager.instance.ClearWorldScrollWorlds();
        WorldManager.instance.WorldItemManager.DisplayWorlds(WorldManager.instance.previousURL.ToString());
       // WorldManager.instance.ChangeWorldTab(WorldManager.instance.previousURL);
        // WorldManager.instance.ChangeWorldTab(APIURL.SearchWorld);
    }
    public void XanaWorldLoad()
    {
        ScrollEnableDisable(0);
        WorldManager.instance.WorldItemManager.DisplayWorlds(APIURL.Hot.ToString());
       // WorldManager.instance.ChangeWorldTab(APIURL.Hot);
    }

    public void GameWorldLoad()
    {
        if (!PremiumUsersDetails.Instance.CheckSpecificItem("GameWorlds"))
        {
            return;
        }
        ScrollEnableDisable(1);
        WorldManager.instance.ChangeWorldTab(APIURL.GameWorld);
    }
    public void CustomWorldLoad()
    {
        if (!PremiumUsersDetails.Instance.CheckSpecificItem("NewBuilderWorlds"))
        {
            return;
        }
        ScrollEnableDisable(2);
        WorldManager.instance.ChangeWorldTab(APIURL.AllWorld);

    }
    public void EventWorldLoadNew()   //my worlds method name is also same so add new here for event category
    {
        if (!PremiumUsersDetails.Instance.CheckSpecificItem("EventWrolds"))
        {
            return;
        }
        ScrollEnableDisable(3);
        WorldManager.instance.ChangeWorldTab(APIURL.EventWorld);

    }
    public void EventWorldLoad()
    {
        if (!PremiumUsersDetails.Instance.CheckSpecificItem("MyBuilderWorlds"))
        {
            return;
        }
        ScrollEnableDisable(4);
        if (PlayerPrefs.GetInt("IsLoggedIn") == 1)
        {
            WorldManager.instance.ChangeWorldTab(APIURL.MyWorld);
        }
        else
        {
            SetTextForScroller("You have not created any world yet.", ScrollerText[2]);
        }
    }
    public void TestWorldLoad()
    {
        if (!PremiumUsersDetails.Instance.CheckSpecificItem("TestWorlds"))
        {
            return;
        }
        ScrollEnableDisable(5);
        WorldManager.instance.ChangeWorldTab(APIURL.TestWorld);
        // WorldManager.instance.ChangeWorldTab(APIURL.Hot);
    }

    void SetTextForScroller(string textToChange, TextMeshProUGUI text)
    {
        text.text = textToChange;
        text.gameObject.SetActive(true);
    }
    void ScrollEnableDisable(int index)
    {
        for (int i = 0; i < HighlghterList.Count; i++)
        {
            WorldPagehighlighters[i].SetActive(false);
            WorldPagehighlightersText[i].SetActive(false);
            HighlghterList[i].SetActive(false);
            HighlghterListText[i].SetActive(false);
        }
        HighlghterList[index].SetActive(true);
        WorldPagehighlighters[index].SetActive(true);
        WorldPagehighlightersText[index].SetActive(true);
        HighlghterListText[index].SetActive(true);
    }
}