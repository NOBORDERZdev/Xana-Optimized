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

    public delegate void SeeAllBtndelegate(string _categType);
    public SeeAllBtndelegate _seeAllBtnDelegate;

    private void OnEnable()
    {
        SearchWorldUIController.OpenSearchPanel += SearchScreenLoad;
        _seeAllBtnDelegate += CategoryLoadMore;
    }

    private void OnDisable()
    {
        SearchWorldUIController.OpenSearchPanel -= SearchScreenLoad;
        _seeAllBtnDelegate -= CategoryLoadMore;
    }

    public void ToggleLobbyOnHomeScreen(bool flag)
    {
        /*gameManager.UiManager.LobbyTabHolder.gameObject.SetActive(flag);*/
    }
    public void SearchScreenLoad(bool _state = true)
    {
        SearchWorldUIController.IsSearchBarActive = true;
        GameManager.Instance.UiManager.SwitchToScreen(2);
        //FlexibleRect.OnAdjustSize?.Invoke(true);
        WorldManager.instance.WorldScrollReset();
        WorldManager.instance.SearchPageNumb = 1;
        if (_state)
        {
            SearchWorldUIController.AutoSelectInputField?.Invoke();
        }
    }

    public void SearchScreenLoad(string searchKey)
    {
        SearchWorldUIController.IsSearchBarActive = true;
        GameManager.Instance.UiManager.SwitchToScreen(2);
        //FlexibleRect.OnAdjustSize?.Invoke(true);
        WorldManager.instance.WorldScrollReset();
    }

    public void BackToPreviousScreen()
    {
        WorldManager.instance.WorldScrollReset();
        GameManager.Instance.UiManager.SwitchToScreen(GameManager.Instance.UiManager.PreviousScreen);
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
        if (!UserPassManager.Instance.CheckSpecificItem("GameWorlds"))
        {
            return;
        }
        ScrollEnableDisable(1);
        WorldManager.instance.ChangeWorld(APIURL.GameWorld);
    }
    public void CustomWorldLoad()
    {
        if (!UserPassManager.Instance.CheckSpecificItem("NewBuilderWorlds"))
        {
            return;
        }
        ScrollEnableDisable(2);
        WorldManager.instance.ChangeWorld(APIURL.AllWorld);
    }
    public void EventWorldLoadNew()   //my worlds method name is also same so add new here for event category
    {
        if (!UserPassManager.Instance.CheckSpecificItem("EventWrolds"))
        {
            return;
        }
        ScrollEnableDisable(3);
        WorldManager.instance.ChangeWorld(APIURL.EventWorld);
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
            WorldManager.instance.ChangeWorld(APIURL.MyWorld);
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
        WorldManager.instance.seeAllPN = 1;
        SearchScreenLoad(false);
        WorldManager.instance.ChangeWorldTab(ApiUrlSelect(_categType), _categType);
    }

    APIURL ApiUrlSelect(string _categType)
    {
        if (_categType.Contains("Featured Spaces"))
        {
            return APIURL.FeaturedSpaces;
        }
        else if (_categType.Contains("Hot Spaces"))
        {
            return APIURL.HotSpaces;
        }
        else if (_categType.Contains("Hot Games"))
        {
            return APIURL.HotGames;
        }
        else if (_categType.Contains("Following Spaces"))
        {
            return APIURL.FolloingSpace;
        }
        else if (_categType.Contains("My Spaces"))
        {
            return APIURL.MySpace;
        }
        else
        {
            return APIURL.SearchWorldByTag;
        }
    }
}