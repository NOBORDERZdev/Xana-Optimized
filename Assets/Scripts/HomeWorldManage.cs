using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HomeWorldManage : MonoBehaviour
{
    [Header("Home Page Scrollviews and Component")]
    public List<GameObject> HighlghterList = new List<GameObject>();
    public List<GameObject> HighlghterListText = new List<GameObject>();
    public List<TextMeshProUGUI> ScrollerText = new List<TextMeshProUGUI>();
    public GameObject FlexibleReact;
    [Header("World Page Scrollviews and Component")]
    public List<GameObject> WorldPagehighlighters = new List<GameObject>();
    public List<GameObject> WorldPagehighlightersText = new List<GameObject>();

    private void OnEnable()
    {
        WorldSearchManager.OpenSearchPanel += SearchScreenLoad;
    }

    private void OnDisable()
    {
        WorldSearchManager.OpenSearchPanel -= SearchScreenLoad;
    }

    public void ToggleLobbyOnHomeScreen(bool flag)
    {
        /*UIManager.Instance.LobbyTabHolder.gameObject.SetActive(flag);*/
    }
    public void SearchScreenLoad()
    {
        WorldSearchManager.IsSearchBarActive = true;
        UIManager.Instance.SwitchToScreen(2);
        RectModifire.OnAdjustSize?.Invoke(true);
        WorldsHandler.instance.WorldScrollReset();
        WorldsHandler.instance.SearchPageNumb = 1;
    }

    public void SearchScreenLoad(string searchKey)
    {
        WorldSearchManager.IsSearchBarActive = true;
        UIManager.Instance.SwitchToScreen(2);
        RectModifire.OnAdjustSize?.Invoke(true);
        WorldsHandler.instance.WorldScrollReset();
    }

    public void BackToPreviousScreen()
    {
        WorldsHandler.instance.WorldScrollReset();
        UIManager.Instance.SwitchToScreen(UIManager.Instance.PreviousScreen);
        //WorldsHandler.instance.ChangeWorld(APIURL.HotSpaces);
        //ScrollEnableDisable(0);
    }
    /*public void XanaWorldLoad()
    {
        ScrollEnableDisable(0);
        WorldsHandler.instance.ChangeWorld(APIURL.HotSpaces);
        if(UIManager.Instance.PreviousScreen==0)
        {
            UIManager.Instance.LobbyTabHolder.gameObject.SetActive(UIManager.Instance.LobbyTabHolder.GetComponent<LobbyWorldItemFlagHandler>().ActivityInApp());
        }
    }

    public void GameWorldLoad()
    {
        if (!UserPassManager.Instance.CheckSpecificItem("GameWorlds"))
        {
            return;
        }
        ScrollEnableDisable(1);
        WorldsHandler.instance.ChangeWorld(APIURL.GameWorld);
    }
    public void CustomWorldLoad()
    {
        if (!UserPassManager.Instance.CheckSpecificItem("NewBuilderWorlds"))
        {
            return;
        }
        ScrollEnableDisable(2);
        WorldsHandler.instance.ChangeWorld(APIURL.AllWorld);
    }
    public void EventWorldLoadNew()   //my worlds method name is also same so add new here for event category
    {
        if (!UserPassManager.Instance.CheckSpecificItem("EventWrolds"))
        {
            return;
        }
        ScrollEnableDisable(3);
        WorldsHandler.instance.ChangeWorld(APIURL.EventWorld);
    }
    public void EventWorldLoad()
    {
        if (!UserPassManager.Instance.CheckSpecificItem("MyBuilderWorlds"))
        {
            return;
        }
        ScrollEnableDisable(4);
        if (PlayerPrefs.GetInt("IsLoggedIn") == 1)
        {
            WorldsHandler.instance.ChangeWorld(APIURL.MyWorld);
        }
        else
        {
            SetTextForScroller("You have not created any world yet.", ScrollerText[2]);
        }
    }
    public void TestWorldLoad()
    {
        if (!UserPassManager.Instance.CheckSpecificItem("TestWorlds"))
        {
            return;
        }
        ScrollEnableDisable(5);
        WorldsHandler.instance.ChangeWorld(APIURL.TestWorld);
    }*/

    void SetTextForScroller(string textToChange, TextMeshProUGUI text)
    {
        text.text = textToChange;
        text.gameObject.SetActive(true);
    }
    public void ScrollEnableDisable(int index)
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
    public void LobbyInactiveCallBack()
    {
        transform.GetComponent<RectTransform>().offsetMin = new Vector2(
            transform.GetComponent<RectTransform>().offsetMin.x,
            transform.GetComponent<RectTransform>().offsetMin.y + 342);
    }


    public void HotSpacesLoadMore()
    {
        SearchScreenLoad();
        WorldsHandler.instance.hotSpacePN = 1;
        WorldsHandler.instance.ChangeWorldTab(APIURL.HotSpaces);
    }

    public void HotGamesLoadMore()
    {
        SearchScreenLoad();
        WorldsHandler.instance.hotGamesPN = 1;
        WorldsHandler.instance.ChangeWorldTab(APIURL.HotGames);
    }

    public void FollowingSpacesLoadMore()
    {
        SearchScreenLoad();
        WorldsHandler.instance.followingPN = 1;
        WorldsHandler.instance.ChangeWorldTab(APIURL.FolloingSpace);
    }

    public void MySpacesLoadMore()
    {
        SearchScreenLoad();
        WorldsHandler.instance.mySpacesPN = 1;
        WorldsHandler.instance.ChangeWorldTab(APIURL.MySpace);
    }

    public void CategorySpacesLoadMore(int tag)
    {
        WorldsHandler.instance.SearchKey = SetupHomeWorldsPreview.mostVisitedTagList[tag];
        SearchScreenLoad();
        WorldsHandler.instance.SearchTagPageNumb = 1;
        WorldsHandler.instance.ChangeWorldTab(APIURL.SearchWorldByTag);
    }
}