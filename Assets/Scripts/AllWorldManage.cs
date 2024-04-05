using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AllWorldManage : MonoBehaviour
{
    [Header("Home Page Scrollviews and Component")]
    public List<GameObject> HighlghterList = new List<GameObject>();
    public List<GameObject> HighlghterListText = new List<GameObject>();
    public List<TextMeshProUGUI> ScrollerText = new List<TextMeshProUGUI>();
    public GameObject FlexibleReact;
    [Header("World Page Scrollviews and Component")]
    public List<GameObject> WorldPagehighlighters = new List<GameObject>();
    public List<GameObject> WorldPagehighlightersText = new List<GameObject>();
    GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    public delegate void SeeAllBtndelegate(string _categType);
    public SeeAllBtndelegate _seeAllBtnDelegate;

    private void OnEnable()
    {
        WorldSearchManager.OpenSearchPanel += SearchScreenLoad;
        _seeAllBtnDelegate += CategoryLoadMore;
    }

    private void OnDisable()
    {
        WorldSearchManager.OpenSearchPanel -= SearchScreenLoad;
        _seeAllBtnDelegate -= CategoryLoadMore;
    }

    public void ToggleLobbyOnHomeScreen(bool flag)
    {
        /*gameManager.UiManager.LobbyTabHolder.gameObject.SetActive(flag);*/
    }
    public void SearchScreenLoad()
    {
        WorldSearchManager.IsSearchBarActive = true;
        gameManager.UiManager.SwitchToScreen(2);
        //FlexibleRect.OnAdjustSize?.Invoke(true);
        WorldManager.instance.WorldScrollReset();
        WorldManager.instance.SearchPageNumb = 1;
    }

    public void SearchScreenLoad(string searchKey)
    {
        WorldSearchManager.IsSearchBarActive = true;
        gameManager.UiManager.SwitchToScreen(2);
        //FlexibleRect.OnAdjustSize?.Invoke(true);
        WorldManager.instance.WorldScrollReset();
    }

    public void BackToPreviousScreen()
    {
        WorldManager.instance.WorldScrollReset();
        gameManager.UiManager.SwitchToScreen(gameManager.UiManager.PreviousScreen);
        //WorldManager.instance.ChangeWorld(APIURL.HotSpaces);
        //ScrollEnableDisable(0);
    }
    /*public void XanaWorldLoad()
    {
        ScrollEnableDisable(0);
        WorldManager.instance.ChangeWorld(APIURL.HotSpaces);
        if(GameManager.Instance.UiManager.PreviousScreen==0)
        {
            GameManager.Instance.UiManager.LobbyTabHolder.gameObject.SetActive(GameManager.Instance.UiManager.LobbyTabHolder.GetComponent<LobbyWorldViewFlagHandler>().ActivityInApp());
        }
    }

    public void GameWorldLoad()
    {
        if (!PremiumUsersDetails.Instance.CheckSpecificItem("GameWorlds"))
        {
            return;
        }
        ScrollEnableDisable(1);
        WorldManager.instance.ChangeWorld(APIURL.GameWorld);
    }
    public void CustomWorldLoad()
    {
        if (!PremiumUsersDetails.Instance.CheckSpecificItem("NewBuilderWorlds"))
        {
            return;
        }
        ScrollEnableDisable(2);
        WorldManager.instance.ChangeWorld(APIURL.AllWorld);
    }
    public void EventWorldLoadNew()   //my worlds method name is also same so add new here for event category
    {
        if (!PremiumUsersDetails.Instance.CheckSpecificItem("EventWrolds"))
        {
            return;
        }
        ScrollEnableDisable(3);
        WorldManager.instance.ChangeWorld(APIURL.EventWorld);
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
            WorldManager.instance.ChangeWorld(APIURL.MyWorld);
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
        WorldManager.instance.ChangeWorld(APIURL.TestWorld);
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


    public void CategoryLoadMore(string _categType)
    {
        Debug.Log("Selected Category Type: " + _categType);
        if (_categType.Contains("Featured Spaces"))
        {
            FeaturedSpacesLoadMore();
        }else if (_categType.Contains("Hot Spaces"))
        {
            HotSpacesLoadMore();
        }
        else if (_categType.Contains("Hot Games"))
        {
            HotGamesLoadMore();
        }
        else if (_categType.Contains("Following Spaces"))
        {
            FollowingSpacesLoadMore();
        }
        else if (_categType.Contains("My Spaces"))
        {
            MySpacesLoadMore();
        }
        else
        {
            CategorySpacesLoadMore(_categType);
        }
    }

    public void FeaturedSpacesLoadMore()
    {
        SearchScreenLoad();
        WorldManager.instance.hotFeatSpacePN = 1;
        WorldManager.instance.ChangeWorldTab(APIURL.FeaturedSpaces);
    }

    public void HotSpacesLoadMore()
    {
        SearchScreenLoad();
        WorldManager.instance.hotSpacePN = 1;
        WorldManager.instance.ChangeWorldTab(APIURL.HotSpaces);
    }

    public void HotGamesLoadMore()
    {
        SearchScreenLoad();
        WorldManager.instance.hotGamesPN = 1;
        WorldManager.instance.ChangeWorldTab(APIURL.HotGames);
    }

    public void FollowingSpacesLoadMore()
    {
        SearchScreenLoad();
        WorldManager.instance.followingPN = 1;
        WorldManager.instance.ChangeWorldTab(APIURL.FolloingSpace);
    }

    public void MySpacesLoadMore()
    {
        SearchScreenLoad();
        WorldManager.instance.mySpacesPN = 1;
        WorldManager.instance.ChangeWorldTab(APIURL.MySpace);
    }

    public void CategorySpacesLoadMore(string tag)
    {
        WorldManager.instance.SearchKey = tag;
        SearchScreenLoad();
        WorldManager.instance.SearchTagPageNumb = 1;
        WorldManager.instance.ChangeWorldTab(APIURL.SearchWorldByTag);
    }
}