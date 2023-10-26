using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using AdvancedInputFieldPlugin;
using static Photon.Pun.UtilityScripts.TabViewManager;

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

    public void ToggleLobbyOnHomeScreen(bool flag)
    {
        UIManager.Instance.LobbyTabHolder.gameObject.SetActive(flag);
    }
    public void SearchScreenLoad()
    {
        UIManager.Instance.SwitchToScreen(2);
        WorldManager.instance.ClearWorldScrollWorlds();
    }
    public void BackToPreviousScreen()
    {
        WorldManager.instance.ClearWorldScrollWorlds();
        UIManager.Instance.SwitchToScreen(UIManager.Instance.PreviousScreen);
       // WorldManager.instance.ChangeWorld(APIURL.Hot);
        ScrollEnableDisable(0);
    }
    public void XanaWorldLoad()
    {
        ScrollEnableDisable(0);
        WorldManager.instance.ChangeWorld(APIURL.Hot);
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
    }

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
        transform.GetComponent<RectTransform>().offsetMin = 
            new Vector2(
            transform.GetComponent<RectTransform>().offsetMin.x,
            transform.GetComponent<RectTransform>().offsetMin.y + 342
            );
    }
    public void SetCategorySize(int type)
    {
        transform.GetComponent<RectTransform>().offsetMin = 
            new Vector2(
            transform.GetComponent<RectTransform>().offsetMin.x,
            transform.GetComponent<RectTransform>().offsetMin.y -(430f*type)
            );
    }
    int State = 0;
    bool previousFlag = true;
    public void AvatarWindowSizeControl(bool flag)
    {
        Debug.LogError("Flag set " + flag);
      switch(State)
        {
            case 0:
                SetConentSizeAfterScreenChnage(flag);
                previousFlag=flag;
                State = 1;
                break;
            case 1:
                if(previousFlag!=flag)
                {
                    SetConentSizeAfterScreenChnage(flag);
                    previousFlag = flag;
                }
                break;
        }
    }
    void SetConentSizeAfterScreenChnage(bool flag)
    {
        transform.GetComponent<RectTransform>().offsetMin =
          new Vector2(
          transform.GetComponent<RectTransform>().offsetMin.x,
          transform.GetComponent<RectTransform>().offsetMin.y + (flag ? 940f : -940f)
          );
    }
}