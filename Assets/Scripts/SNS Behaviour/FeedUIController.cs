using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.IO;
using RenderHeads.Media.AVProVideo;
using UnityEngine.UI.Extensions;
using UnityEngine.Video;
using System;
using UnityEngine.Networking;
using AdvancedInputFieldPlugin;
using SuperStar.Helpers;
//using Amazon.S3.Model;

public class FeedUIController : MonoBehaviour
{
    public static FeedUIController Instance;

    [Header("-------FooterCan-------")]
    public GameObject footerCan;
    public BottomTabManager bottomTabManager;

    [Space]
    [Header("-------API Controller Feed References-------")]
    public Transform followingFeedTabLeftContainer;
    public Transform followingFeedTabRightContainer;
    public Transform followingFeedTabContainer;
    public Transform forYouFeedTabContainer;
    public Transform hotTabContainer;
    public Transform videofeedParent;
    public GameObject followingFeedMainContainer;

    [Space]
    [Header("-------All Screens-------")]
    public GameObject feedUiScreen;
    public GameObject otherPlayerProfileScreen;
    public GameObject giftItemScreens;
    public GameObject feedVideoScreen;
    public SNSAPILoaderController apiLoaderController;

    [Space]
    [Header("Top Story Panel")]
    public GameObject TopPanelMainObj;
    public GameObject TopPanelMainStoryObj;
    public Transform TopPanelMainContainerObj;

    [Header("GiftScreen")]
    //  public Transform fashionCetegoryContent;
    public Sprite selectedSprite, unSelectedSprite;

    [Space]
    [Header("HorizontalScrollSnap")]
    public HorizontalScrollSnap feedUiHorizontalSnap;

    public GameObject feedUiSelectionLine;
    public Transform[] feedUiSelectionTab;
    public TextMeshProUGUI[] feedUiTabTitleText;
    public Color selectedColor, unSelectedColor;

    [Space]
    [Header("ScrollRectFaster")]
    public ScrollRectFasterEx[] allFeedScrollRectFasterEx;
    public ScrollRectFasterEx feedUiScrollRectFasterEx;

    [Space]
    [Header("AllFeedScreen")]
    public GameObject[] allFeedPanel;
    public GameObject videoFeedRect;
    public List<TextMeshProUGUI> allFeedMessageTextList = new List<TextMeshProUGUI>();

    public int allFeedCurrentpage, followingUserCurrentpage, myPostCurrentPage;
    public bool isDataLoad = false;

    public bool isAnyUserFollow = false;

    [Space]
    [Header("Feed All Tabs Loaded checking Variable")]
    public int followingFeedInitiateTotalCount;
    //public int followingFeedImageLoadedCount;
    public int hotFeedInitiateTotalCount;
    //public int HotFeedImageLoadedCount;
    public int hotForYouFeedInitiateTotalCount;
    //public int hotForYouFeedImageLoadedCount;

    [Space]
    //this list is used to unfollowed user feed removed from following tab.......
    public List<int> unFollowedUserListForFollowingTab = new List<int>();

    //public GameObject fingerTouch;
    [Space]
    [Header("FeedVideo Screen")]
    public RectTransform feedVideoButtonPanelImage;
    public string feedFullViewScreenCallingFrom = "";

    [Space]
    [Header("Find Friend screen References")]
    public GameObject findFriendScreen;
    public Transform findFriendContainer;
    public TMP_InputField findFriendInputField;
    public AdvancedInputField findFriendInputFieldAdvanced;

    [Space]
    [Header("Create Feed Screen References")]
    public GameObject createFeedScreen;
    public TMP_InputField createFeedTitle;
    public AdvancedInputField createFeedTitleAdvanced;
    public TMP_InputField createFeedDescription;
    public AdvancedInputField createFeedDescriptionAdvanced;
    public Image createFeedImage;
    public GameObject createFeedVideoObj;
    public MediaPlayer createFeedMediaPlayer;

    [Space]
    [Header("Edit Delete Feed Screen Reference")]
    public FeedEditOrDeleteData feedEditOrDeleteData;
    public GameObject editDeleteFeedScreen;
    public TextMeshProUGUI editDeleteFeedUserNameText;
    public TextMeshProUGUI editDeleteFeedDateTimeText;
    public Image editDeleteCurrentFeedImage;
    public GameObject editDeleteVideoDisplay;
    //public MediaPlayer editDeleteMideaPlayer;
    public PostFeedVideoItem editDeleteCurrentPostFeedVideoItem;

    [Space]
    [Header("Edit Feed Screen Reference")]
    public GameObject editFeedScreen;
    public AdvancedInputField editFeedDescriptionInputField;
    public Image editFeedCurrentFeedImage;
    public GameObject editFeedCurrentVideoDisplay;

    [Space]
    [Header("Delete Feed Confirmation Screen")]
    public GameObject deleteFeedConfirmationScreen;

    [Space]
    [Header("Feed Comment Screen Reference")]
    public GameObject commentPanel;
    public AdvancedInputField commentInputFieldAdvanced;
    public Text commentFitertextDropdown;
    public ScrollRect commentScrollPosition;

    public GameObject commentContentPanel;
    public GameObject commentListItemPrefab;
    public Text CommentCount;

    [Space]
    public string attechmentArraystr;

    [Space]
    [Header("FadeInOut Screen Reference")]
    public GameObject fadeInOutScreen;
    public Color fadeInOutColor;

    [Space]
    [Header("Profile Follower and Following list Reference")]
    public GameObject profileFollowerFollowingListScreen;
    public Transform profileFollowerListContainer;
    public Transform profileFollowingListContainer;
    public GameObject followerPrefab;
    public GameObject followingPrefab;
    public TextMeshProUGUI profileFFScreenTitleText;
    public Transform profileFFLineSelection;
    public Transform[] profileFFSelectionTab;
    public TextMeshProUGUI[] profileFFSelectionTabText;
    public HorizontalScrollSnap profileFollowerFollowingHorizontalScroll;
    public ScrollRectFasterEx[] profileFFScreenScrollrectFasterEXList;
    public int profileFollowerPaginationPageNo = 1;
    public int profileFollowingPaginationPageNo = 1;
    public bool isProfileFollowerDataLoaded = false;
    public bool isProfileFollowingDataLoaded = false;

    private List<int> profileFollowerLoadedItemIDList = new List<int>();
    private List<int> profileFollowingLoadedItemIDList = new List<int>();
    public List<FollowerItemController> profileFollowerItemControllersList = new List<FollowerItemController>();
    public List<FollowingItemController> profileFollowingItemControllersList = new List<FollowingItemController>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    [Space]
    public int callCount = 0;
    private void OnEnable()
    {
        if (callCount > 0)
        {
            StartCoroutine(WaitToStartCallForFeedScene());
            return;
        }
        callCount += 1;
    }

    IEnumerator WaitToStartCallForFeedScene()
    {
        yield return new WaitForSeconds(0.2f);
       Debug.Log("FeedUIController isLoginFromDifferentId:" + APIManager.Instance.isLoginFromDifferentId);
        if (APIManager.Instance.isLoginFromDifferentId)
        {
           Debug.Log("FeedUI Controller new user login and calling feed start function");
            ResetAllFeedScreen(false);
            StartMethodCalling();//Start Function Calling.......
        }
    }

    private void Start()
    {
        StartMethodCalling();//Start Function Calling.......
    }

    //this method calling start of the scene.......
    public void StartMethodCalling()
    {
        //Debug.Log("FeedController Start UserToken:" + ConstantsGod.AUTH_TOKEN + "    :userID:" + PlayerPrefs.GetString("UserName"));
        //Debug.Log("ApiBaseUrl:" + ConstantsGod.API_BASEURL);

        if (GlobalVeriableClass.callingScreen == "Feed")
        {
            StartCoroutine(WaitToSceneLoad());
        }

        SetupFollowerAndFeedScreen(false);
    }

    IEnumerator WaitToSceneLoad()
    {
        yield return new WaitForSeconds(0);

        SetupLineSelectionPosition();//move Selection Line

        //APIManager.Instance.RequestGetAllFollowers(1, 10, "FeedStart");//Get All Follower

        for (int i = 0; i < allFeedMessageTextList.Count; i++)
        {
            AllFeedScreenMessageTextActive(true, i, TextLocalization.GetLocaliseTextByKey("please wait"));
        }

        //Debug.Log("FeedUIController Start:" + Application.internetReachability);
        //rik for start of the feed scene load data and default api calling....... 
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (File.Exists(Application.persistentDataPath + "/FeedData.json"))
            {
                APIManager.Instance.LoadJson();
            }

            if (File.Exists(Application.persistentDataPath + "/FeedFollowingData.json"))
            {
                APIManager.Instance.LoadJsonFollowingFeed();
            }
        }
        else
        {
            //Debug.Log("dfdfsd");
            OnFeedButtonTabBtnClick();
        }
    }


    //this method are used to Message button click.......
    public void OnClickMessageButton()
    {
        Initiate.Fade("SNSMessageModuleScene", Color.black, 1.0f, true);
    }

    public void ShowLoader(bool isActive)
    {
        apiLoaderController.ShowApiLoader(isActive);
    }

    private void Update()
    {
        //APiPagination();
    }

    public void OnFeedButtonTabBtnClick()
    {
       Debug.Log("OnFeedButtonTabBtnClick.......");
        APIManager.Instance.OnFeedAPiCalling();
        feedUiScreen.SetActive(true);
    }

    public void AllFeedScreenMessageTextActive(bool isActive, int index, string message)
    {
        allFeedMessageTextList[index].text = message;
        allFeedMessageTextList[index].gameObject.SetActive(isActive);
    }

    public void OnClickFollowingTabBtnClick()
    {
        RemoveUnFollowedUserFromFollowingTab();

        //if (isAnyUserFollow)
        //{
        //isAnyUserFollow = false;
        //APIManager.Instance.RequestGetFeedsByFollowingUser(1, 10);
        //}
    }

    public void OnClickHotAndDiscoverTabBtnClick()
    {
        //if (isAnyUserFollow)
        //{
        //isAnyUserFollow = false;
        //APIManager.Instance.RequestGetAllUsersWithFeeds(1, 5);
        //}
    }

    public void ResetAllFeedScreen(bool isFeedScreen)
    {
        if (isFeedScreen && APIManager.Instance.allUserRootList.Count == 0)
        {
           Debug.Log("Feed Data Load");
            StartCoroutine(WaitToSceneLoad());
        }

        feedUiScreen.SetActive(isFeedScreen);
        otherPlayerProfileScreen.SetActive(false);
        giftItemScreens.SetActive(false);
        feedVideoScreen.SetActive(false);
        findFriendScreen.SetActive(false);
        createFeedScreen.SetActive(false);

        profileFollowerFollowingListScreen.SetActive(false);
        OtherPlayerProfileData.Instance.backKeyManageList.Clear();

        SNSSettingController.Instance.myAccountScreen.SetActive(false);
        SNSSettingController.Instance.myAccountPersonalInfoScreen.SetActive(false);
        //StartCoroutine(WaitToResetAllFeedScreen(isFeedScreen));        
    }

    /*public IEnumerator WaitToResetAllFeedScreen(bool isFeedScreen)
    {
        yield return new WaitForSeconds(0f);

        if(isFeedScreen && APIManager.Instance.allUserRootList.Count == 0)
        {
           Debug.Log("Feed Data Load");
            StartCoroutine(WaitToSceneLoad());
        }

        feedUiScreen.SetActive(isFeedScreen);
        otherPlayerProfileScreen.SetActive(false);
        giftItemScreens.SetActive(false);
        feedVideoScreen.SetActive(false);
        findFriendScreen.SetActive(false);
        createFeedScreen.SetActive(false);

        SNSSettingController.Instance.myAccountScreen.SetActive(false);                
    }*/

    public void ClearAllFeedDataAfterLogOut()
    {
        APIManager.Instance.ClearAllFeedDataForLogout();

        APIController.Instance.feedFollowingIdList.Clear();
        APIController.Instance.feedForYouIdList.Clear();
        APIController.Instance.feedHotIdList.Clear();
        APIController.Instance.feedHotIdList.Clear();
        MyProfileDataManager.Instance.myProfileData = new GetUserDetailData();
        MyProfileDataManager.Instance.allMyFeedImageRootDataList.Clear();
        MyProfileDataManager.Instance.allMyFeedVideoRootDataList.Clear();
        OtherPlayerProfileData.Instance.allMyFeedImageRootDataList.Clear();
        OtherPlayerProfileData.Instance.allMyFeedVideoRootDataList.Clear();
        OtherPlayerProfileData.Instance.backKeyManageList.Clear();
        foreach (Transform item in followingFeedTabContainer)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in followingFeedTabLeftContainer)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in followingFeedTabRightContainer)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in forYouFeedTabContainer)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in hotTabContainer)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in videofeedParent)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in MyProfileDataManager.Instance.allPhotoContainer)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in MyProfileDataManager.Instance.allMovieContainer)
        {
            Destroy(item.gameObject);
        }
    }

    #region fade In Out screen methods.......
    public void FadeInOutScreenShow()
    {
        fadeInOutScreen.GetComponent<Image>().color = fadeInOutColor;
        fadeInOutScreen.SetActive(true);

        fadeInOutScreen.GetComponent<Image>().DOFade(1.0f, 1f).SetEase(Ease.Linear).OnComplete(() =>
           FadeOutStart()
        );
    }

    Coroutine waitToFadeOutCo;
    void FadeOutStart()
    {
        if (waitToFadeOutCo != null)
        {
            StopCoroutine(waitToFadeOutCo);
        }
        waitToFadeOutCo = StartCoroutine(WaitToFadeOut());
    }

    IEnumerator WaitToFadeOut()
    {
        yield return new WaitForSeconds(0.5f);
        fadeInOutScreen.GetComponent<Image>().DOFade(0.0f, 1f).SetEase(Ease.Linear).OnComplete(() =>
                fadeInOutScreen.SetActive(false)
        );
    }
    #endregion

    #region Setup position and size Feed and Top Follower Panel.......
    public void SetupFollowerAndFeedScreen(bool isStoryAvailable)
    {
        if (isStoryAvailable)
        {
            if (TopPanelMainStoryObj.activeSelf)
            {
                return;
            }
            TopPanelMainStoryObj.SetActive(true);

            TopPanelMainObj.GetComponent<RectTransform>().sizeDelta = new Vector2(TopPanelMainObj.GetComponent<RectTransform>().sizeDelta.x, TopPanelMainObj.GetComponent<RectTransform>().sizeDelta.y + TopPanelMainStoryObj.GetComponent<RectTransform>().sizeDelta.y);
            TopPanelMainObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (float)(TopPanelMainObj.GetComponent<RectTransform>().anchoredPosition.y - (TopPanelMainStoryObj.GetComponent<RectTransform>().sizeDelta.y / 2)));
            for (int i = 0; i < allFeedPanel.Length; i++)
            {
                allFeedPanel[i].GetComponent<RectTransform>().offsetMax = new Vector2(0, allFeedPanel[i].GetComponent<RectTransform>().offsetMax.y - TopPanelMainStoryObj.GetComponent<RectTransform>().sizeDelta.y);
            }
        }
        else
        {
            if (!TopPanelMainStoryObj.activeSelf)
            {
                return;
            }
            TopPanelMainStoryObj.SetActive(false);

            TopPanelMainObj.GetComponent<RectTransform>().sizeDelta = new Vector2(TopPanelMainObj.GetComponent<RectTransform>().sizeDelta.x, TopPanelMainObj.GetComponent<RectTransform>().sizeDelta.y - TopPanelMainStoryObj.GetComponent<RectTransform>().sizeDelta.y);
            TopPanelMainObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (float)(TopPanelMainObj.GetComponent<RectTransform>().anchoredPosition.y + (TopPanelMainStoryObj.GetComponent<RectTransform>().sizeDelta.y / 2)));

            for (int i = 0; i < allFeedPanel.Length; i++)
            {
                allFeedPanel[i].GetComponent<RectTransform>().offsetMax = new Vector2(0, allFeedPanel[i].GetComponent<RectTransform>().offsetMax.y + TopPanelMainStoryObj.GetComponent<RectTransform>().sizeDelta.y);
            }
        }
    }
    #endregion

    public void OnBackToMainXanaBtnClick()
    {
        Initiate.Fade("Main", Color.black, 1.0f);
    }

    public void OnClickCheckOtherPlayerProfile()
    {
        otherPlayerProfileScreen.SetActive(true);

        if (OtherPlayerProfileData.Instance.backKeyManageList.Count > 0)
        {
            switch (OtherPlayerProfileData.Instance.backKeyManageList[OtherPlayerProfileData.Instance.backKeyManageList.Count - 1])
            {
                case "FollowerFollowingListScreen":
                    MyProfileDataManager.Instance.myProfileScreen.SetActive(false);
                    profileFollowerFollowingListScreen.SetActive(false);
                    footerCan.GetComponent<BottomTabManager>().SetDefaultButtonSelection(3);
                    break;
                case "HotTabScreen":
                    //Debug.Log("Comes from Hot or Discover tab full feed screen");
                    //disable feed full screen after click on profile button and open other user profile.......
                    OnClickVideoItemBackButton();
                    break;
                case "FollowingTabScreen":
                    //Debug.Log("Comes from Following tab full feed screen");
                    //disable feed full screen after click on profile button and open other user profile.......
                    OnClickVideoItemBackButton();
                    break;
                default:
                    feedUiScreen.SetActive(false);
                    break;
            }
        }
        else
        {
            feedUiScreen.SetActive(false);
        }
    }

    public void OnClickProfileGiftBOxButton()
    {
        otherPlayerProfileScreen.SetActive(false);
        giftItemScreens.SetActive(true);
    }

    public void OnClickFashionCetegoryItem(int index)
    {
        /* for (int i = 0; i < fashionCetegoryContent.childCount; i++)
         {
             if (i == index)
             {
                 fashionCetegoryContent.GetChild(i).gameObject.GetComponent<Image>().sprite = selectedSprite;
                 fashionCetegoryContent.GetChild(i).gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
                 fashionCetegoryContent.GetChild(i).gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = selectedColor;
             }
             else
             {
                 fashionCetegoryContent.GetChild(i).gameObject.GetComponent<Image>().sprite = unSelectedSprite;
                 fashionCetegoryContent.GetChild(i).gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
                 fashionCetegoryContent.GetChild(i).gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = unSelectedColor;
             }
         }*/
    }

    public void Swipe(Vector2 value)
    {
        //Debug.Log(" value " + value.y);
    }

    public void OnSetSelectionLine()
    {
        if (feedUiScreen.activeSelf)
        {
            SetupLineSelectionPosition();//move Selection Line

            //feedUiSelectionLine.transform.DOMove(new Vector3((feedUiSelectionTab[feedUiHorizontalSnap.CurrentPage].position.x), feedUiSelectionLine.transform.position.y, feedUiSelectionLine.transform.position.z), .2f);
            /*for (int i = 0; i < feedUiTabTitleText.Length; i++)
            {
                if (i == feedUiHorizontalSnap.CurrentPage)
                {
                    feedUiTabTitleText[i].color = selectedColor;
                    StartCoroutine(ActiveFeedUi(i));
                }
                else
                {
                    feedUiTabTitleText[i].color = unSelectedColor;
                }
            }*/
            //  if (feedUiHorizontalSnap.CurrentPage != 0)
            // {
            isChangeMainScrollRect = true;
            feedUiScrollRectFasterEx = allFeedScrollRectFasterEx[feedUiHorizontalSnap.CurrentPage];

            StartCoroutine(WaitChangeScrollRectFasterOnMain());
            // }
        }
    }

    void SetupLineSelectionPosition()
    {
        float xPos;
        if (feedUiHorizontalSnap.CurrentPage == 0)
        {
            xPos = feedUiSelectionTab[feedUiHorizontalSnap.CurrentPage].position.x - 25f;
        }
        else if (feedUiHorizontalSnap.CurrentPage == 1)
        {
            xPos = feedUiSelectionTab[feedUiHorizontalSnap.CurrentPage].position.x - 10f;
        }
        else if (feedUiHorizontalSnap.CurrentPage == 2)
        {
            xPos = feedUiSelectionTab[feedUiHorizontalSnap.CurrentPage].position.x + 11f;
        }
        else
        {
            xPos = feedUiSelectionTab[feedUiHorizontalSnap.CurrentPage].position.x;
        }

        //feedUiSelectionLine.transform.DOMove(new Vector3(xPos, feedUiSelectionLine.transform.position.y, feedUiSelectionLine.transform.position.z), .2f);
        feedUiSelectionLine.transform.DOMoveX(xPos, .2f);

        //SetColor Feed Tab Title Text
        for (int i = 0; i < feedUiTabTitleText.Length; i++)
        {
            if (i == feedUiHorizontalSnap.CurrentPage)
            {
                feedUiTabTitleText[i].color = selectedColor;
                feedUiTabTitleText[i].fontStyle = FontStyles.Bold;
            }
            else
            {
                feedUiTabTitleText[i].color = unSelectedColor;
                feedUiTabTitleText[i].fontStyle = FontStyles.Normal;
            }
            /*if (i == 1)//new cmnt
            {
                StartCoroutine(SetContentOnFollowingItemScreen());
            }*/
        }
    }

    public IEnumerator SetContentOnFollowingItemScreen()
    {
        yield return new WaitForSeconds(0.01f);
        followingFeedMainContainer.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        yield return new WaitForSeconds(0.05f);
        followingFeedMainContainer.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    public void SetUpFeedTabDefaultTop()
    {
        feedUiScrollRectFasterEx.verticalNormalizedPosition = 1;
    }

    public bool isChangeMainScrollRect = false;
    IEnumerator WaitChangeScrollRectFasterOnMain()
    {
        yield return new WaitForSeconds(2f);
        isChangeMainScrollRect = false;
    }

    public IEnumerator ActiveFeedUi(int index, int callingIndex)
    {
        if (feedUiScreen.activeSelf)
        {
            for (int i = 0; i < allFeedPanel.Length; i++)
            {
                if (i == index)
                {
                    allFeedPanel[i].transform.gameObject.SetActive(true);
                    if (callingIndex == 1)//set default scroll top.......
                    {
                        SetUpFeedTabDefaultTop();
                    }
                }
            }
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < allFeedPanel.Length; i++)
            {
                if (i != feedUiHorizontalSnap.CurrentPage)
                {
                    allFeedPanel[i].transform.gameObject.SetActive(false);
                }
            }

            if (index == 1)
            {
                StartCoroutine(SetContentOnFollowingItemScreen());
            }
        }
    }

    public void CloseAllFeed(bool isActive)
    {
        if (feedUiScreen.activeSelf)
        {
            for (int i = 0; i < allFeedPanel.Length; i++)
            {
                allFeedPanel[i].transform.gameObject.SetActive(isActive);
            }
        }
    }

    public float verticalNormalizedPosition;
    public void APiPagination()
    {
        //Debug.Log("y pos:" + feedUiScrollRectFasterEx.verticalEndPos);

        if (isChangeMainScrollRect)
        {
            return;
        }
        verticalNormalizedPosition = feedUiScrollRectFasterEx.verticalNormalizedPosition;
        //Debug.Log("verticalNormalizedPosition : " + feedUiScrollRectFasterEx.verticalNormalizedPosition + "    :verticalEndPos:" + feedUiScrollRectFasterEx.verticalEndPos + "    :isDataLoad:" + isDataLoad);
        //if (feedUiScrollRectFasterEx.verticalEndPos <= 1 && isDataLoad)
        if (feedUiScrollRectFasterEx.verticalNormalizedPosition <= 0.01f && isDataLoad)
        {
            if (feedUiScrollRectFasterEx == allFeedScrollRectFasterEx[2])
            {
                APIManager.Instance.RequestGetAllUsersWithFeeds((allFeedCurrentpage + 1), 10);
            }
            //Debug.Log("scrollRect pos :" + feedUiScrollRectFasterEx.verticalNormalizedPosition);
            if (feedUiHorizontalSnap.CurrentPage == 1)
            {
                //Debug.Log("Feed Following scrollRect pos :" + feedUiScrollRectFasterEx.verticalNormalizedPosition + " rows count:"+ APIManager.Instance.followingUserRoot.Data.Rows.Count);
                //if (APIManager.Instance.followingUserRoot.Data.Rows.Count > 0 && followingFeedImageLoadedCount >= (followingFeedInitiateTotalCount - 1))
                if (APIManager.Instance.followingUserRoot.Data.Rows.Count > 0 /*&& followingFeedInitiateTotalCount < 2*/)
                {
                    isDataLoad = false;
                   Debug.Log("isDataLoad False");
                    APIManager.Instance.RequestGetFeedsByFollowingUser((followingUserCurrentpage + 1), 10);
                }
            }
            else
            {
                //Debug.Log("Feed scrollRect pos :" + feedUiScrollRectFasterEx.verticalNormalizedPosition + " rows count:"+ APIManager.Instance.root.data.rows.Count + " :current screen:" + feedUiHorizontalSnap.CurrentPage);
                //Riken
                //if (APIManager.Instance.root.data.rows.Count > 0)
                if (APIManager.Instance.allhotFeedRoot.data.rows.Count > 0)
                {
                    bool isCallAPi = false;
                    if (feedUiHorizontalSnap.CurrentPage == 0 && hotFeedInitiateTotalCount < 2)
                    {
                        isCallAPi = true;
                    }
                    else if (feedUiHorizontalSnap.CurrentPage == 2 && hotForYouFeedInitiateTotalCount < 2)
                    {
                        isCallAPi = true;
                    }
                   Debug.Log("isCalling:" + isCallAPi);
                    if (isCallAPi)
                    {
                       Debug.Log("isDataLoad False APiPagination currentPage :" + allFeedCurrentpage);
                        isDataLoad = false;
                        APIManager.Instance.RequestGetAllUsersWithFeeds((allFeedCurrentpage + 1), 10);
                    }
                }
            }
            //Debug.Log("isDataLoad False");
            //isDataLoad = false;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            APIManager.Instance.RequestGetAllUsersWithFeeds((allFeedCurrentpage + 1), 10);
        }
    }

    public string GetConvertedTimeString(DateTime dateTime)
    {
        DateTime timeUtc = dateTime;
        DateTime today = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, TimeZoneInfo.Local);

        TimeSpan timeDiff = (DateTime.Now - today);

        //Debug.Log("minuts : " + timeDiff.TotalMinutes + "  :days: " + timeDiff.Days + "    :Date:"+dateTime);
        string timestr = "";

        if (timeDiff.TotalMinutes < 1)
        {
            timestr = TextLocalization.GetLocaliseTextByKey("Just Now");
        }
        else if (timeDiff.TotalMinutes > 1 && timeDiff.TotalMinutes <= 60)
        {
            timestr = Mathf.Round((float)(timeDiff.TotalMinutes)) + " " + TextLocalization.GetLocaliseTextByKey("minutes ago");
        }
        else if (timeDiff.TotalMinutes > 60 && timeDiff.TotalMinutes <= 1440)
        {
            timestr = Mathf.Round((float)(timeDiff.TotalMinutes / 60)) + " " + TextLocalization.GetLocaliseTextByKey("hours ago");
        }
        else if (timeDiff.TotalDays > 1 && timeDiff.TotalDays <= 30)
        {
            timestr = timeDiff.Days.ToString() + " " + TextLocalization.GetLocaliseTextByKey("days ago");
        }
        else if (timeDiff.TotalDays > 30 && timeDiff.TotalDays <= 365)
        {
            timestr = Mathf.Round((float)(timeDiff.Days / 30)) + " " + TextLocalization.GetLocaliseTextByKey("months ago");
        }
        else
        {
            timestr = Mathf.Round((float)(timeDiff.Days / 365)) + " " + TextLocalization.GetLocaliseTextByKey("years ago");
        }
        return timestr;
    }

    public string GetConvertedTimeStringSpecifyKind(DateTime dateTime)
    {
        DateTime timeUtc = dateTime;
        //DateTime today = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, TimeZoneInfo.Local);
        DateTime today = DateTime.SpecifyKind(timeUtc, DateTimeKind.Local); ;

        TimeSpan timeDiff = (DateTime.Now - today);

        //Debug.Log("minuts : " + timeDiff.TotalMinutes + "  :days: " + timeDiff.Days + "    :Date:"+dateTime);
        string timestr = "";

        if (timeDiff.TotalMinutes < 1)
        {
            timestr = TextLocalization.GetLocaliseTextByKey("Just Now");
        }
        else if (timeDiff.TotalMinutes > 1 && timeDiff.TotalMinutes <= 60)
        {
            timestr = Mathf.Round((float)(timeDiff.TotalMinutes)) + " " + TextLocalization.GetLocaliseTextByKey("minutes ago");
        }
        else if (timeDiff.TotalMinutes > 60 && timeDiff.TotalMinutes <= 1440)
        {
            timestr = Mathf.Round((float)(timeDiff.TotalMinutes / 60)) + " " + TextLocalization.GetLocaliseTextByKey("hours ago");
        }
        else if (timeDiff.TotalDays > 1 && timeDiff.TotalDays <= 30)
        {
            timestr = timeDiff.Days.ToString() + " " + TextLocalization.GetLocaliseTextByKey("days ago");
        }
        else if (timeDiff.TotalDays > 30 && timeDiff.TotalDays <= 365)
        {
            timestr = Mathf.Round((float)(timeDiff.Days / 30)) + " " + TextLocalization.GetLocaliseTextByKey("months ago");
        }
        else
        {
            timestr = Mathf.Round((float)(timeDiff.Days / 365)) + " " + TextLocalization.GetLocaliseTextByKey("years ago");
        }
        return timestr;
    }

    public void OnClickVideoItemBackButton()
    {
        feedVideoScreen.SetActive(false);
        switch (feedFullViewScreenCallingFrom)
        {
            case "MyProfile":
               Debug.Log("Full Video Screen back calling from MyProfile");
                MyProfileDataManager.Instance.mainPostContainer.gameObject.SetActive(true);
                MyProfileDataManager.Instance.ResetMainScrollDefaultTopPos();
                MyProfileDataManager.Instance.SetupEmptyMsgForPhotoTab(false);//check for empty message.......
                break;
            case "OtherProfile":
                OtherPlayerProfileData.Instance.mainPostContainer.gameObject.SetActive(true);
                OtherPlayerProfileData.Instance.ResetMainScrollDefaultTopPos();
               Debug.Log("Full Video Screen back calling from OtherProfile");
                break;
            case "FeedPage":
               Debug.Log("Full Video Screen back calling from MyPostFed");
                break;
            default:
                feedUiScreen.SetActive(true);
                break;
        }
        feedFullViewScreenCallingFrom = "";

        foreach (Transform item in videofeedParent)
        {
            Destroy(item.gameObject);
        }
    }
    public void OnClickVideoItemBackButton(bool ComeFromBackButton)
    {
        feedVideoScreen.SetActive(false);
        switch (feedFullViewScreenCallingFrom)
        {
            case "MyProfile":
               Debug.Log("Full Video Screen back calling from MyProfile");
                MyProfileDataManager.Instance.mainPostContainer.gameObject.SetActive(true);
                MyProfileDataManager.Instance.ResetMainScrollDefaultTopPos();
                MyProfileDataManager.Instance.SetupEmptyMsgForPhotoTab(false);//check for empty message.......
                break;
            case "OtherProfile":
                OtherPlayerProfileData.Instance.mainPostContainer.gameObject.SetActive(true);
                OtherPlayerProfileData.Instance.ResetMainScrollDefaultTopPos();
               Debug.Log("Full Video Screen back calling from OtherProfile");
                break;
            case "FeedPage":
               Debug.Log("Full Video Screen back calling from MyPostFed");
                if (!ComeFromBackButton)
                {
                    bottomTabManager.OnClickProfileButton();
                }
                bottomTabManager.OnClickProfileButton();
                break;
            default:
                feedUiScreen.SetActive(true);
                break;
        }
        feedFullViewScreenCallingFrom = "";

        foreach (Transform item in videofeedParent)
        {
            Destroy(item.gameObject);
        }
    }

    public void OnClickGiftScreenBackButton()
    {
        giftItemScreens.SetActive(false);
        otherPlayerProfileScreen.SetActive(true);
    }

    //this method is used to Find Friend Button click.......
    public void OnClickSearchUserButton()
    {
        //findFriendInputField.text = "";
        findFriendInputFieldAdvanced.Text = "";
        findFriendScreen.SetActive(true);
    }

    #region find User references
    //this method is used to On find value inputfield value change.......
    public void OnValueChangeFindFriend()
    {
        //if (!string.IsNullOrEmpty(findFriendInputField.text))
        if (!string.IsNullOrEmpty(findFriendInputFieldAdvanced.Text))
        {
            //APIManager.Instance.RequestGetSearchUser(findFriendInputField.text);
            APIManager.Instance.RequestGetSearchUser(findFriendInputFieldAdvanced.Text);
        }
        else
        {
            //if user typed character clear then clear all search user list.
            foreach (Transform item in findFriendContainer)
            {
                Destroy(item.gameObject);
            }
        }
    }

    //this method is used to back button click find friend screen.......
    public void OnClickBackFindFriendButton()
    {
        RemoveUnFollowedUserFromFollowingTab();

        //findFriendInputField.text = "";
        findFriendInputFieldAdvanced.Text = "";
        findFriendScreen.SetActive(false);
        foreach (Transform item in findFriendContainer)
        {
            Destroy(item.gameObject);
        }
    }
    #endregion

    #region Create Feed
    public void CreateFeedAPICall(string url, string thumbnail)
    {
        switch (imageOrVideo)
        {
            case "Image":
                //string s1 = createFeedTitle.text;
                //string s1 = createFeedTitleAdvanced.RichText;
                string s1 = APIManager.Instance.userName;
                //string s2 = createFeedDescription.text;
                string s2 = createFeedDescriptionAdvanced.RichText;

                if (string.IsNullOrEmpty(s1))
                {
                    s1 = "@new";
                }
                else
                {
                    s1 = "@" + s1;
                }

                if (string.IsNullOrEmpty(s2))
                {
                    s2 = "  ";
                }
                APIManager.Instance.RequestCreateFeed(APIManager.EncodedString(s1), APIManager.EncodedString(s2), url, "", thumbnail, "true", "", "MyProfileCreateFeed");
                break;
            case "Video":
                //string s11 = createFeedTitle.text;
                //string s11 = createFeedTitleAdvanced.RichText;
                string s11 = APIManager.Instance.userName;
                //string s22 = createFeedDescription.text;
                string s22 = createFeedDescriptionAdvanced.RichText;

                if (string.IsNullOrEmpty(s11))
                {
                    s11 = "@new";
                }
                else
                {
                    s11 = "@" + s11;
                }

                if (string.IsNullOrEmpty(s22))
                {
                    s22 = "  ";
                }
                APIManager.Instance.RequestCreateFeed(APIManager.EncodedString(s11), APIManager.EncodedString(s22), "", url, thumbnail, "true", "", "MyProfileCreateFeed");
                break;
            default:
                break;
        }
    }

    static long GetFileSize(string FilePath)
    {
        if (File.Exists(FilePath))
        {
            return new FileInfo(FilePath).Length;
        }
        return 0;
    }

    public bool lastPostCreatedImageDownload = false;
    public string imageOrVideo = "";
    public string createFeedLastPickFilePath;
    public string createFeedLastPickFileName;
    public void OnClickCreateFeedPickImageOrVideo()
    {
        ResetAndClearCreateFeedData();

        /*createFeedTitle.text = "";
        createFeedDescription.text = "";
        createFeedImage.sprite = null;
        imageOrVideo = "";

        createFeedLastPickFilePath = "";
        createFeedLastPickFileName = "";*/

        createFeedImage.gameObject.SetActive(false);
        createFeedVideoObj.SetActive(false);
        createFeedMediaPlayer.gameObject.SetActive(false);
        createFeedScreen.transform.localScale = Vector3.one;

        /*NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                string fileExtention = Path.GetExtension(path);
                switch (NativeGallery.GetMediaTypeOfFile(path))
                {
                    case NativeGallery.MediaType.Image:
                        Texture2D texture = NativeGallery.LoadImageAtPath(path, 1024, false);
                        if (texture == null)
                        {
                            Debug.Log("Couldn't load texture from " + path);
                            return;
                        }
                        createFeedImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
                        imageOrVideo = "Image";
                        createFeedImage.gameObject.SetActive(true);
                        Debug.Log("Picked image");
                        break;
                    case NativeGallery.MediaType.Video:
                        createFeedVideoObj.SetActive(true);
                        createFeedMediaPlayer.gameObject.SetActive(true);
                        createFeedMediaPlayer.OpenMedia(new MediaPath(path, MediaPathType.AbsolutePathOrURL), autoPlay: true);
                        createFeedMediaPlayer.Play();
                        Debug.Log("Picked video");
                        imageOrVideo = "Video";
                        break;
                    default:
                        Debug.Log("Probably picked something else");
#if UNITY_EDITOR
                        if (fileExtention == ".heic")
                        {
                            imageOrVideo = "Image";
                            break;
                        }
                        else
                        {
                            if (SNSNotificationManager.Instance != null)
                            {
                                SNSNotificationManager.Instance.ShowNotificationMsg("Please upload valid image or video file");
                            }
                            return;
                        }
#else
                        if (SNSNotificationManager.Instance != null)
                        {
                            SNSNotificationManager.Instance.ShowNotificationMsg("Please upload valid image or video file");
                        }
                        return;
#endif
                }

                createFeedLastPickFilePath = path;

                string[] pathArry = path.Split('/');

                //string fileName = pathArry[pathArry.Length - 1];
                string fileName = Path.GetFileName(path);
                createFeedLastPickFileName = (Time.time + fileName);
               Debug.Log("createFeedLastPickFileName  :" + createFeedLastPickFileName + " :fileName   :" + fileName);

                createFeedScreen.SetActive(true);
            }
        });
        Debug.Log("Permission result: " + permission);
        return;*/

        if (NativeGallery.CanSelectMultipleMediaTypesFromGallery())
        {
            NativeGallery.Permission permission = NativeGallery.GetMixedMediaFromGallery((path) =>
            {
                //UnityToolbag.Dispatcher.Invoke(() =>
                //{

                string fileExtention = Path.GetExtension(path);
                //Debug.Log("Path:" + path + "   :File extention:" + fileExtention + "    :MediaType:" + NativeGallery.GetMediaTypeOfFile(path));

                if (path != null)
                {
                    // Determine if user has picked an image, video or neither of these
                    switch (NativeGallery.GetMediaTypeOfFile(path))
                    {
                        case NativeGallery.MediaType.Image:
                            Texture2D texture = NativeGallery.LoadImageAtPath(path, 1024, false);
                            if (texture == null)
                            {
                                Debug.Log("Couldn't load texture from " + path);
                                return;
                            }
                            createFeedImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
                            imageOrVideo = "Image";
                            createFeedImage.gameObject.SetActive(true);
                            createFeedScreen.SetActive(true);
                            //Debug.Log("Picked image");
                            break;
                        case NativeGallery.MediaType.Video:
                            ShowLoader(true);//show loader for prepare video.......
                            createFeedScreen.transform.localScale = Vector3.zero;
                            createFeedScreen.SetActive(true);
                            createFeedVideoObj.SetActive(true);
                            createFeedMediaPlayer.gameObject.SetActive(true);
                            createFeedMediaPlayer.OpenMedia(new MediaPath(path, MediaPathType.AbsolutePathOrURL), autoPlay: true);
                            createFeedMediaPlayer.Events.AddListener(OnFeedCreateMediaPlayerEvent);
                            createFeedMediaPlayer.Play();
                            //Debug.Log("Picked video");
                            imageOrVideo = "Video";
                            break;
                        default:
                            //SNSWarningMessageManager.Instance.ShowWarningMessage("Only upload image or video");
                            Debug.Log("Probably picked something else:" + fileExtention);
#if UNITY_EDITOR
                            if (fileExtention == ".heic")
                            {
                                imageOrVideo = "Image";
                                createFeedScreen.SetActive(true);
                                break;
                            }
                            else
                            {
                                if (SNSNotificationManager.Instance != null)
                                {
                                    SNSNotificationManager.Instance.ShowNotificationMsg("Please upload valid image or video file");
                                }
                                return;
                            }
#else
                            if (SNSNotificationManager.Instance != null)
                            {
                                SNSNotificationManager.Instance.ShowNotificationMsg("Please upload valid image or video file");
                            }
                            return;
#endif
                    }

                    createFeedLastPickFilePath = path;

                    string[] pathArry = path.Split('/');

                    //string fileName = pathArry[pathArry.Length - 1];
                    string fileName = Path.GetFileName(path);
                    createFeedLastPickFileName = (Time.time + fileName);
                   Debug.Log("createFeedLastPickFileName  :" + createFeedLastPickFileName + " :fileName   :" + fileName);

                    //createFeedScreen.SetActive(true);
                }
                //});
            }, NativeGallery.MediaType.Image | NativeGallery.MediaType.Video, "Select an image or video");

            Debug.Log("Permission result: " + permission);
        }
    }

    public void FeedCreateVideoTimeGet()
    {
        //Debug.Log("Duration:" + createFeedMediaPlayer.Info.GetDuration() + "   :frame:" + createFeedMediaPlayer.Info.GetDurationFrames());
        if (createFeedMediaPlayer.Info.GetDuration() > 15)
        {
            SNSWarningMessageManager.Instance.ShowWarningMessage("Please upload video upto 15 seconds");
            createFeedMediaPlayer.CloseMedia();
            imageOrVideo = "";
            createFeedLastPickFilePath = "";
            createFeedLastPickFileName = "";
            createFeedScreen.SetActive(false);
            Resources.UnloadUnusedAssets();
        }
        createFeedScreen.transform.localScale = Vector3.one;
        ShowLoader(false);//show loader for prepare video.......
    }

    public void OnFeedCreateMediaPlayerEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
    {
        //Debug.Log("OnFeedCreateMediaPlayerEvent:" + et);
        switch (et)
        {
            case MediaPlayerEvent.EventType.MetaDataReady:
                //Debug.Log("MetaDataReady: " + createFeedMediaPlayer.Info.GetDuration());
                FeedCreateVideoTimeGet();
                break;
            case MediaPlayerEvent.EventType.FirstFrameReady:
                //Debug.Log("FirstFrameReady: " + createFeedMediaPlayer.Info.GetDuration());
                FeedCreateVideoTimeGet();
                break;
            case MediaPlayerEvent.EventType.ReadyToPlay:
                //Debug.Log("ReadyToPlay: " + createFeedMediaPlayer.Info.GetDuration());
                FeedCreateVideoTimeGet();
                break;
            case MediaPlayerEvent.EventType.Error:
                FeedCreateVideoError();
                break;
        }
    }

    public void FeedCreateVideoError()
    {
       Debug.Log("Error to load feed seleted video");
        SNSNotificationManager.Instance.ShowNotificationMsg("video can't load please try again");
        createFeedMediaPlayer.CloseMedia();
        imageOrVideo = "";
        createFeedLastPickFilePath = "";
        createFeedLastPickFileName = "";
        createFeedScreen.SetActive(false);
        createFeedScreen.transform.localScale = Vector3.one;
        ShowLoader(false);//show loader for prepare video.......
        Resources.UnloadUnusedAssets();
    }

    //this method is used to post Feed 
    public void OnClickCreateFeedPostBtn()
    {
        print("post btn");
        if (PremiumUsersDetails.Instance != null && !PremiumUsersDetails.Instance.CheckSpecificItem("post button"))
        {
            //PremiumUsersDetails.Instance.PremiumUserUI.SetActive(true);
            print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            print("Horayyy you have Access");
        }

        ShowLoader(true);//active loader

        string iscompress = "";
        if (imageOrVideo == "Image")
        {
            iscompress = "true";
        }
        else if (imageOrVideo == "Video")
        {
            createFeedMediaPlayer.Pause();
        }
        AWSHandler.Instance.PostObjectFeed(createFeedLastPickFilePath, createFeedLastPickFileName, "CreateFeed", iscompress);
    }

    public void OnClickCreateFeedBackBtn(bool isDataNotReset)
    {
        createFeedScreen.SetActive(false);

        createFeedImage.gameObject.SetActive(false);
        createFeedVideoObj.SetActive(false);
        if (createFeedMediaPlayer.gameObject.activeSelf)
        {
           Debug.Log("Close media player");
            createFeedMediaPlayer.CloseMedia();
            createFeedMediaPlayer.gameObject.SetActive(false);
        }

        if (!isDataNotReset)
        {
            ResetAndClearCreateFeedData();
        }

        /*createFeedTitle.text = "";
        createFeedDescription.text = "";
        createFeedImage.sprite = null;
        imageOrVideo = "";

        createFeedLastPickFilePath = "";
        createFeedLastPickFileName = "";*/
    }

    public void ResetAndClearCreateFeedData()
    {
        lastPostCreatedImageDownload = false;

        createFeedTitle.text = "";
        createFeedDescription.text = "";
        createFeedTitleAdvanced.Text = "";
        createFeedDescriptionAdvanced.Text = "";
        /*if (createFeedImage.sprite != null)
        {
            Destroy(createFeedImage.sprite);
        }*/
        createFeedImage.sprite = null;
        imageOrVideo = "";

        createFeedLastPickFilePath = "";
        createFeedLastPickFileName = "";

        Resources.UnloadUnusedAssets();
        //Caching.ClearCache();
        //GC.Collect();
    }
    #endregion

    #region Following Tab Feed Remove and refresh after unfollow user
    //this method is used to check add and remove from unfollowed list.......
    public void FollowingAddAndRemoveUnFollowedUser(int userID, bool isComesFromUnFollowed)
    {
        if (isComesFromUnFollowed)
        {
            if (!unFollowedUserListForFollowingTab.Contains(userID))
            {
                unFollowedUserListForFollowingTab.Add(userID);
            }
        }
        else
        {
            if (unFollowedUserListForFollowingTab.Contains(userID))
            {
                unFollowedUserListForFollowingTab.Remove(userID);
            }
        }
    }

    //this method is used to Remove feed from following tab.......
    public void RemoveUnFollowedUserFromFollowingTab(string callingFrom = "")
    {
        //Debug.Log("RemoveUnFollowedUserFromFollowingTab.......:" + unFollowedUserListForFollowingTab.Count);
        if (unFollowedUserListForFollowingTab.Count > 0)
        {
            APIManager.Instance.FeedFollowingSaveAndUpdateJson(unFollowedUserListForFollowingTab, callingFrom);
        }
    }
    #endregion

    #region Profile Follower and Following list Screen Methods
    int tempFollowFollowingScreenOpenCount = 0;
    //this method is used to profile follower button click.......
    public void ProfileFollowerFollowingScreenSetup(int Tabindex, string userName)
    {
        string titleLocalize = TextLocalization.GetLocaliseTextByKey("s friends");
        if (GameManager.currentLanguage == "en" && !CustomLocalization.forceJapanese)
        {
            titleLocalize = "'" + titleLocalize;
        }

        profileFFScreenTitleText.text = userName + titleLocalize;
        profileFollowerFollowingListScreen.SetActive(true);
        if (tempFollowFollowingScreenOpenCount == 0)
        {
            StartCoroutine(WaitToProfileFollowerFollowingHorizontalScroll(Tabindex));
        }
        else
        {
            profileFollowerFollowingHorizontalScroll.GoToScreen(Tabindex);
        }
        ProfileFFLineSelectionSetup(Tabindex);

        for (int i = 0; i < profileFFScreenScrollrectFasterEXList.Length; i++)
        {
            profileFFScreenScrollrectFasterEXList[i].verticalNormalizedPosition = 1;
        }
        tempFollowFollowingScreenOpenCount++;
        //MyProfileDataManager.Instance.MyProfileSceenShow(false);
    }

    IEnumerator WaitToProfileFollowerFollowingHorizontalScroll(int Tabindex)
    {
        yield return new WaitForSeconds(0.02f);
        profileFollowerFollowingHorizontalScroll.GoToScreen(Tabindex);
    }

    public void ProfileFFSelectionOnValueChange()
    {
        //float xPos = profileFFSelectionTab[profileFollowerFollowingHorizontalScroll.CurrentPage].position.x;
        //profileFFLineSelection.transform.DOMoveX(xPos, .01f);
        ProfileFFLineSelectionSetup(profileFollowerFollowingHorizontalScroll.CurrentPage);
    }

    public void ProfileFFLineSelectionSetup(int index)
    {
        float xPos = profileFFSelectionTab[index].position.x;
        profileFFLineSelection.transform.DOMoveX(xPos, .2f);

        for (int i = 0; i < profileFFSelectionTabText.Length; i++)
        {
            if (i == index)
            {
                profileFFSelectionTabText[i].color = selectedColor;
                profileFFSelectionTabText[i].fontStyle = FontStyles.Bold;
            }
            else
            {
                profileFFSelectionTabText[i].color = unSelectedColor;
                profileFFSelectionTabText[i].fontStyle = FontStyles.Normal;
            }
        }
    }

    public void OnClickProfileFollowerFollowingBackButton()
    {
        RemoveUnFollowedUserFromFollowingTab();

        profileFollowerFollowingListScreen.SetActive(false);

        //MyProfileDataManager.Instance.MyProfileSceenShow(true);
    }

    public void ProfileGetAllFollower(int pageNum)
    {
        //Debug.Log("ProfileGetAllFollower:" + APIManager.Instance.profileAllFollowerRoot.data.rows.Count + "    :pageNum:" + pageNum);
        for (int i = 0; i < APIManager.Instance.profileAllFollowerRoot.data.rows.Count; i++)
        {
            if (!profileFollowerLoadedItemIDList.Contains(APIManager.Instance.profileAllFollowerRoot.data.rows[i].follower.id))
            {
                GameObject followerObject = Instantiate(followerPrefab, profileFollowerListContainer);
                followerObject.GetComponent<FollowerItemController>().SetupData(APIManager.Instance.profileAllFollowerRoot.data.rows[i]);
                profileFollowerItemControllersList.Add(followerObject.GetComponent<FollowerItemController>());
                profileFollowerLoadedItemIDList.Add(APIManager.Instance.profileAllFollowerRoot.data.rows[i].follower.id);
            }
        }

        if (waitToProfileFollowerDataLoadCo != null)
        {
            StopCoroutine(waitToProfileFollowerDataLoadCo);
        }
        waitToProfileFollowerDataLoadCo = StartCoroutine(WaitToProfileFollowerDataLoad(pageNum));
    }

    Coroutine waitToProfileFollowerDataLoadCo;
    IEnumerator WaitToProfileFollowerDataLoad(int pageNum)
    {
        yield return new WaitForSeconds(0.5f);
        isProfileFollowerDataLoaded = true;
        if (pageNum > 1 && APIManager.Instance.profileAllFollowerRoot.data.rows.Count > 0)
        {
            profileFollowerPaginationPageNo += 1;
        }
    }

    public void ProfileGetAllFollowing(int pageNum)
    {
        //Debug.Log("ProfileGetAllFollowing:" + APIManager.Instance.profileAllFollowingRoot.data.rows.Count + "    :pageNum:" + pageNum);
        for (int i = 0; i < APIManager.Instance.profileAllFollowingRoot.data.rows.Count; i++)
        {
            if (!profileFollowingLoadedItemIDList.Contains(APIManager.Instance.profileAllFollowingRoot.data.rows[i].following.id))
            {
                GameObject followingObject = Instantiate(followingPrefab, profileFollowingListContainer);
                followingObject.GetComponent<FollowingItemController>().SetupData(APIManager.Instance.profileAllFollowingRoot.data.rows[i]);
                profileFollowingItemControllersList.Add(followingObject.GetComponent<FollowingItemController>());
                profileFollowingLoadedItemIDList.Add(APIManager.Instance.profileAllFollowingRoot.data.rows[i].following.id);
            }
        }

        if (waitToProfileFollowingDataLoadCo != null)
        {
            StopCoroutine(waitToProfileFollowingDataLoadCo);
        }
        waitToProfileFollowingDataLoadCo = StartCoroutine(WaitToProfileFollowingDataLoad(pageNum));
    }

    Coroutine waitToProfileFollowingDataLoadCo;
    IEnumerator WaitToProfileFollowingDataLoad(int pageNum)
    {
        yield return new WaitForSeconds(0.5f);
        isProfileFollowingDataLoaded = true;
        if (pageNum > 1 && APIManager.Instance.profileAllFollowingRoot.data.rows.Count > 0)
        {
            profileFollowingPaginationPageNo += 1;
        }
    }

    public void ProfileFollowerFollowingListClear()
    {
        profileFollowerPaginationPageNo = 1;
        profileFollowingPaginationPageNo = 1;
        isProfileFollowerDataLoaded = false;
        isProfileFollowingDataLoaded = false;

        profileFollowerItemControllersList.Clear();
        profileFollowerLoadedItemIDList.Clear();
        foreach (Transform item in profileFollowerListContainer)
        {
            Destroy(item.gameObject);
        }

        profileFollowingItemControllersList.Clear();
        profileFollowingLoadedItemIDList.Clear();
        foreach (Transform item in profileFollowingListContainer)
        {
            Destroy(item.gameObject);
        }
    }

    public void ProfileFollowerPaginationAPICall()
    {
        //Debug.Log("ProfileFollowerFollowingPagination : " + profileFFScreenScrollrectFasterEXList[0].verticalNormalizedPosition + " :CurrentPage:" + profileFollowerFollowingHorizontalScroll.CurrentPage);
        if (profileFFScreenScrollrectFasterEXList[0].verticalNormalizedPosition <= 0.01f && isProfileFollowerDataLoaded)
        {
            if (APIManager.Instance.profileAllFollowerRoot.data.rows.Count > 0)
            {
                //Debug.Log("ProfileFollowerFollowingPagination follower currentPage:" + profileFollowerPaginationPageNo);
                isProfileFollowerDataLoaded = false;
                APIManager.Instance.RequestGetAllFollowersFromProfile(MyProfileDataManager.Instance.myProfileData.id.ToString(), (profileFollowerPaginationPageNo + 1), 50);
            }
        }
    }

    public void ProfileFollowingPaginationAPICall()
    {
        //Debug.Log("ProfileFollowerFollowingPagination : " + profileFFScreenScrollrectFasterEXList[1].verticalNormalizedPosition + " :CurrentPage:" + profileFollowerFollowingHorizontalScroll.CurrentPage);
        if (profileFFScreenScrollrectFasterEXList[1].verticalNormalizedPosition <= 0.01f && isProfileFollowingDataLoaded)
        {
            if (APIManager.Instance.profileAllFollowingRoot.data.rows.Count > 0)
            {
                //Debug.Log("ProfileFollowerFollowingPagination following currentPage:" + profileFollowingPaginationPageNo);
                isProfileFollowingDataLoaded = false;
                APIManager.Instance.RequestGetAllFollowingFromProfile(MyProfileDataManager.Instance.myProfileData.id.ToString(), (profileFollowingPaginationPageNo + 1), 50);
            }
        }
    }
    #endregion

    #region Edit Delete Feed Methods.......
    public void SetupEditDeleteFeedScreen()
    {
        if (!string.IsNullOrEmpty(feedEditOrDeleteData.feedImage))
        {
            bool isUrlContainsHttpAndHttps = APIManager.Instance.CheckUrlDropboxOrNot(feedEditOrDeleteData.feedImage);
            if (isUrlContainsHttpAndHttps)
            {
                AssetCache.Instance.EnqueueOneResAndWait(feedEditOrDeleteData.feedImage, feedEditOrDeleteData.feedImage, (success) =>
                {
                    if (success)
                    {
                        AssetCache.Instance.LoadSpriteIntoImage(editDeleteCurrentFeedImage, feedEditOrDeleteData.feedImage, changeAspectRatio: true);
                    }
                });
            }
            else
            {
                GetImageFromAWS(feedEditOrDeleteData.feedImage, editDeleteCurrentFeedImage);//Get image from aws and save/load into asset cache.......
            }

            float diff = editDeleteCurrentFeedImage.sprite.rect.width - editDeleteCurrentFeedImage.sprite.rect.height;
            if (diff < -160)
            {
                editDeleteCurrentFeedImage.GetComponent<AspectRatioFitter>().aspectRatio = 0.1f;
            }
            else
            {
                editDeleteCurrentFeedImage.GetComponent<AspectRatioFitter>().aspectRatio = 2.2f;
            }
        }
        else
        {
            editDeleteCurrentFeedImage.gameObject.SetActive(false);
            editDeleteVideoDisplay.SetActive(true);
            //editDeleteMideaPlayer.gameObject.SetActive(true);
            editDeleteCurrentPostFeedVideoItem.feedMediaPlayer.Pause();
            editDeleteVideoDisplay.GetComponent<DisplayUGUI>().CurrentMediaPlayer = editDeleteCurrentPostFeedVideoItem.feedMediaPlayer;
            //editDeleteMideaPlayer = editDeleteCurrentPostFeedVideoItem.feedMediaPlayer;
        }
        if (GameManager.currentLanguage == "ja")
        {
            editDeleteFeedUserNameText.text = feedEditOrDeleteData.userData.Name + " " + TextLocalization.GetLocaliseTextByKey("Post by");
        }
        else
        {
            editDeleteFeedUserNameText.text = TextLocalization.GetLocaliseTextByKey("Post by") + " " + feedEditOrDeleteData.userData.Name;
        }
        if (feedEditOrDeleteData.UpdatedAt != null)
        {
            DateTime timeUtc = feedEditOrDeleteData.UpdatedAt;
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, TimeZoneInfo.Local);

            TimeSpan timeDiff = (DateTime.Now - today);
           Debug.Log("timeDiff:" + timeDiff + " :timeUtc:" + timeUtc.ToString("yyyy/MM/dd tt hh:MM"));

            editDeleteFeedDateTimeText.text = timeUtc.ToString("yyyy/MM/dd tt hh:MM");
        }
    }

    //this method is used show edit delete feed screen.......
    public void OnShowEditDeleteFeedScreen(bool isActive)
    {
        if (isActive)
        {
            SetupEditDeleteFeedScreen();
        }
        editDeleteFeedScreen.SetActive(isActive);
    }

    //this method is used to Edit/Delete Feed Close Popup Button Click.......
    public void OnClickEditDeleteClosePopupButton()
    {
        StartCoroutine(WaitToOnClickEditDeleteClose());
    }
    IEnumerator WaitToOnClickEditDeleteClose()
    {
        yield return new WaitForSeconds(0.25f);
        editDeleteCurrentFeedImage.gameObject.SetActive(true);
        editDeleteVideoDisplay.SetActive(false);
        //editDeleteMideaPlayer.gameObject.SetActive(false);

        editDeleteFeedUserNameText.text = TextLocalization.GetLocaliseTextByKey("Post") + " " + TextLocalization.GetLocaliseTextByKey("by");
        editDeleteFeedUserNameText.text = "";
        AssetCache.Instance.RemoveFromMemory(editDeleteCurrentFeedImage.sprite);
        editDeleteCurrentFeedImage.sprite = null;

        if (!string.IsNullOrEmpty(feedEditOrDeleteData.feedVideo))
        {
            editDeleteCurrentPostFeedVideoItem.feedMediaPlayer.Play();
        }

        //editDeleteMideaPlayer = null;
        Resources.UnloadUnusedAssets();
        //Caching.ClearCache();
        //GC.Collect();

        if (editDeleteCurrentPostFeedVideoItem != null)
        {
            editDeleteCurrentPostFeedVideoItem = null;
        }
    }

    //this method is used to Edit/Delete Feed Screen Delete button click.......
    public void OnClickEditDeleteFeedDeleteButton()
    {
        deleteFeedConfirmationScreen.SetActive(true);
    }

    //this method is used to Edit/Delete Feed Screen Edit button click.......
    public void OnClickEditDeleteFeedEditButton()
    {
        editFeedCurrentFeedImage.gameObject.SetActive(true);
        editFeedCurrentVideoDisplay.SetActive(false);

        if (!string.IsNullOrEmpty(feedEditOrDeleteData.feedImage))
        {
            editFeedCurrentFeedImage.sprite = editDeleteCurrentFeedImage.sprite;
            editFeedCurrentFeedImage.GetComponent<AspectRatioFitter>().aspectRatio = editDeleteCurrentFeedImage.GetComponent<AspectRatioFitter>().aspectRatio;
        }
        else
        {
            editFeedCurrentFeedImage.gameObject.SetActive(false);
            editFeedCurrentVideoDisplay.SetActive(true);

            editFeedCurrentVideoDisplay.GetComponent<DisplayUGUI>().CurrentMediaPlayer = editDeleteCurrentPostFeedVideoItem.feedMediaPlayer;
        }

        if (!string.IsNullOrEmpty(feedEditOrDeleteData.feedDescriptions))
        {
            editFeedDescriptionInputField.Text = APIManager.DecodedString(feedEditOrDeleteData.feedDescriptions);
        }
        else
        {
            editFeedDescriptionInputField.Text = "";
        }

        editFeedScreen.SetActive(true);
    }

    //this method is used to Edit/Delete Feed Screen Share button click.......
    public void OnClickEditDeleteFeedShareButton()
    {
        string url = editDeleteCurrentPostFeedVideoItem.shareMediaUrl;

        new NativeShare().AddFile("Scoial Image")
       .SetSubject("Subject goes here").SetText("Xana App!").SetUrl(url)
       .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
       .Share();
    }

    //this method is used to Delete Feed Confirmation Screen Ok button click.......
    public void OnClickDeleteFeedConfirmationOkButton()
    {
        //ShowLoader(true);
        APIManager.Instance.RequestDeleteFeed(feedEditOrDeleteData.feedId.ToString(), "DeleteFeed");
        //deleteFeedConfirmationScreen.SetActive(false);
    }

    //this method is used to Delete Feed Confirmation Screen Cancel button click.......
    public void OnClickDeleteFeedConfirmationCancelButton()
    {
        deleteFeedConfirmationScreen.SetActive(false);
    }

    //this method is used to success Delete Feed Response.......
    public void OnSuccessDeleteFeed()
    {
        DeleteFeedAfterRemoveAndRefreshData();
    }

    //this method is used to delete after remove prefab and remove from list and refresh.......
    void DeleteFeedAfterRemoveAndRefreshData()
    {
        Debug.Log("feedEditOrDeleteData.feedId" + feedEditOrDeleteData.feedId);
        int PostIndex = MyProfileDataManager.Instance.loadedMyPostAndVideoIdInFeedPage.FindIndex(o => o.Equals(feedEditOrDeleteData.feedId));
        int HotPostIndex = APIController.Instance.feedHotIdList.FindIndex(o => o.Equals(feedEditOrDeleteData.feedId));
        APIController.Instance.feedHotIdList.Remove(feedEditOrDeleteData.feedId);
        MyProfileDataManager.Instance.loadedMyPostAndVideoId.Remove(feedEditOrDeleteData.feedId);
        MyProfileDataManager.Instance.loadedMyPostAndVideoIdInFeedPage.Remove(feedEditOrDeleteData.feedId);
        AllFeedByUserIdRow allFeedByUserIdForAPI = APIManager.Instance.allFeedWithUserIdRoot.Data.Rows.Find(x => x.Id.Equals(feedEditOrDeleteData.feedId));
        APIManager.Instance.allFeedWithUserIdRoot.Data.Rows.Remove(allFeedByUserIdForAPI);

        //for image.......
        if (!string.IsNullOrEmpty(feedEditOrDeleteData.feedImage))
        {
            AllFeedByUserIdRow allFeedByUserIdRow = MyProfileDataManager.Instance.allMyFeedImageRootDataList.Find((x) => x.Id == feedEditOrDeleteData.feedId);
            Debug.Log("Image Before");
            int imageIndex = MyProfileDataManager.Instance.allMyFeedImageRootDataList.IndexOf(allFeedByUserIdRow);
            Debug.Log("Image " + imageIndex);
            if (MyProfileDataManager.Instance.allPhotoContainer.childCount > imageIndex)
            {
                DestroyImmediate(MyProfileDataManager.Instance.allPhotoContainer.GetChild(imageIndex).gameObject);
                Debug.Log("Image Delete");
            }
            MyProfileDataManager.Instance.allMyFeedImageRootDataList.Remove(allFeedByUserIdRow);
        }
        else
        {
            AllFeedByUserIdRow allFeedByUserIdRow = MyProfileDataManager.Instance.allMyFeedVideoRootDataList.Find((x) => x.Id == feedEditOrDeleteData.feedId);
            Debug.Log("Video Before");
            int videoIndex = MyProfileDataManager.Instance.allMyFeedVideoRootDataList.IndexOf(allFeedByUserIdRow);
            Debug.Log("Video " + videoIndex);
            if (MyProfileDataManager.Instance.allMovieContainer.childCount > videoIndex)
            {
                DestroyImmediate(MyProfileDataManager.Instance.allMovieContainer.GetChild(videoIndex).gameObject);
                Debug.Log("Video Delete");
            }
            MyProfileDataManager.Instance.allMyFeedVideoRootDataList.Remove(allFeedByUserIdRow);
        }

        HotFeed myhotfeed = APIManager.Instance.allhotFeedRoot.data.rows.Find((x) => x.id == feedEditOrDeleteData.feedId);
        Debug.Log("Hot Image Before");
        int index = APIManager.Instance.allhotFeedRoot.data.rows.IndexOf(myhotfeed);
        Debug.Log("Hot Image " + index);
        APIManager.Instance.allhotFeedRoot.data.rows.Remove(myhotfeed);

        Debug.Log("PostIndex:" + PostIndex);
        if (PostIndex >= 0 && forYouFeedTabContainer.childCount > PostIndex)
        {
            Destroy(forYouFeedTabContainer.GetChild(PostIndex).gameObject);
        }
        Debug.Log("HotPostIndex:" + HotPostIndex);
        if (HotPostIndex >= 0 && hotTabContainer.childCount > HotPostIndex)
        {
            Destroy(hotTabContainer.GetChild(HotPostIndex).gameObject);
        }

        if (editDeleteCurrentPostFeedVideoItem != null)
        {
            DestroyImmediate(editDeleteCurrentPostFeedVideoItem.gameObject);
            editDeleteCurrentPostFeedVideoItem = null;
        }

        if (APIManager.Instance.allFeedWithUserIdRoot.Data.Rows.Count == 0)
        {
            AllFeedScreenMessageTextActive(true, 2, TextLocalization.GetLocaliseTextByKey("There's nothing to show here."));
        }
        else
        {
            AllFeedScreenMessageTextActive(false, 2, TextLocalization.GetLocaliseTextByKey(""));
        }

        if (videoFeedRect.GetComponent<ScrollSnapRect>().startingPage > 0)
        {
            videoFeedRect.GetComponent<ScrollSnapRect>().startingPage = videoFeedRect.GetComponent<ScrollSnapRect>().startingPage - 1;
        }

        if (videofeedParent.childCount > 0)
        {
            videoFeedRect.GetComponent<ScrollSnapRect>().StartScrollSnap();//refresh and setup upto top Full view feed screen.......
        }

        OnClickDeleteFeedConfirmationCancelButton();
        editDeleteFeedScreen.GetComponent<OnEnableDisable>().ClosePopUp();

        //SNSNotificationManager.Instance.ShowNotificationMsg("Post deleted");//this method is used to show SNS notification.......

        MyProfileDataManager.Instance.RequestGetUserDetails();

        if (videofeedParent.childCount <= 0)
        {
            MyProfileDataManager.Instance.totalPostText.text = "0";
            OnClickVideoItemBackButton();
        }
    }

    /// <summary>
    /// feed edit-----------------------------
    /// </summary>
    //this method is used to Edit/Delete Feed Screen Edit button click.......
    string lastUpdatedFeedDescriptionStr = "";
    public void OnClickEditFeedDoneButton()
    {
        int updatefeedDescription = 0;
        lastUpdatedFeedDescriptionStr = "";
        if (!string.IsNullOrEmpty(feedEditOrDeleteData.feedDescriptions))
        {
            if (feedEditOrDeleteData.feedDescriptions != editFeedDescriptionInputField.Text)
            {
                //lastUpdatedFeedDescriptionStr = editFeedDescriptionInputField.Text;
                lastUpdatedFeedDescriptionStr = editFeedDescriptionInputField.Text;
                updatefeedDescription = 1;
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(editFeedDescriptionInputField.Text))
            {
                //lastUpdatedFeedDescriptionStr = editFeedDescriptionInputField.Text;
                lastUpdatedFeedDescriptionStr = editFeedDescriptionInputField.Text;
                updatefeedDescription = 1;
            }
        }

        if (updatefeedDescription == 1)
        {
            if (string.IsNullOrEmpty(lastUpdatedFeedDescriptionStr))
            {
                lastUpdatedFeedDescriptionStr = " ";
            }

            ShowLoader(true);
            APIManager.Instance.RequestEditFeed(feedEditOrDeleteData.feedId.ToString(), APIManager.EncodedString(lastUpdatedFeedDescriptionStr), feedEditOrDeleteData.feedImage, feedEditOrDeleteData.feedVideo);
        }
        editFeedScreen.SetActive(true);
    }

    //this method is used to success response of Feed Edit.......
    public void OnSuccessFeedEdit()
    {
        //for image.......
        if (!string.IsNullOrEmpty(feedEditOrDeleteData.feedImage))
        {
            AllFeedByUserIdRow allFeedByUserIdRow = MyProfileDataManager.Instance.allMyFeedImageRootDataList.Find((x) => x.Id == feedEditOrDeleteData.feedId);
            allFeedByUserIdRow.Descriptions = lastUpdatedFeedDescriptionStr;

            int index = MyProfileDataManager.Instance.allMyFeedImageRootDataList.IndexOf(allFeedByUserIdRow);
            if (MyProfileDataManager.Instance.allPhotoContainer.childCount > index)
            {
                MyProfileDataManager.Instance.allPhotoContainer.GetChild(index).GetComponent<UserPostItem>().userData.Descriptions = lastUpdatedFeedDescriptionStr;
            }
            editDeleteCurrentPostFeedVideoItem.userData.Descriptions = lastUpdatedFeedDescriptionStr;
            editDeleteCurrentPostFeedVideoItem.descriptionText.text = lastUpdatedFeedDescriptionStr;
        }
        else
        {
            AllFeedByUserIdRow allFeedByUserIdRow = MyProfileDataManager.Instance.allMyFeedVideoRootDataList.Find((x) => x.Id == feedEditOrDeleteData.feedId);
            allFeedByUserIdRow.Descriptions = lastUpdatedFeedDescriptionStr;

            int index = MyProfileDataManager.Instance.allMyFeedVideoRootDataList.IndexOf(allFeedByUserIdRow);
            if (MyProfileDataManager.Instance.allMovieContainer.childCount > index)
            {
                MyProfileDataManager.Instance.allMovieContainer.GetChild(index).GetComponent<UserPostItem>().userData.Descriptions = lastUpdatedFeedDescriptionStr;
            }
            editDeleteCurrentPostFeedVideoItem.userData.Descriptions = lastUpdatedFeedDescriptionStr;
            editDeleteCurrentPostFeedVideoItem.descriptionText.text = lastUpdatedFeedDescriptionStr;
        }

        feedEditOrDeleteData.feedDescriptions = lastUpdatedFeedDescriptionStr;

        editDeleteCurrentPostFeedVideoItem.RefreshDescriptionAfterEdit(lastUpdatedFeedDescriptionStr);

        editFeedScreen.SetActive(false);
    }

    #endregion

    #region Get Image And Video From AWS
    public void GetVideoUrl(string key)
    {
        /*var request_1 = new GetPreSignedUrlRequest()
        {
            BucketName = AWSHandler.Instance.Bucketname,
            Key = key,
            Expires = DateTime.Now.AddHours(6)
        };
        //Debug.Log("Feed Video file sending url request:" + AWSHandler.Instance._s3Client);

        AWSHandler.Instance._s3Client.GetPreSignedURLAsync(request_1, (callback) =>
        {
            if (callback.Exception == null)
            {
                string mediaUrl = callback.Response.Url;
                UnityToolbag.Dispatcher.Invoke(() =>
                {
                    //Debug.Log("Feed Video URL " + mediaUrl);
                    //feedMediaPlayer.OpenMedia(new MediaPath(mediaUrl, MediaPathType.AbsolutePathOrURL), autoPlay: false);
                });
            }
            else
               Debug.Log(callback.Exception);
        });*/

        if (key != "")
        {
            string mediaUrl = "";

            if (key.Contains("https"))
            {
                mediaUrl = key;
            }
            else
            {
                mediaUrl = ConstantsGod.AWS_VIDEO_BASE_URL + key;
            }

            Debug.Log($"<color=green> Video Key = FeedFollowItemController : </color>{mediaUrl}");
            UnityToolbag.Dispatcher.Invoke(() =>
            {
                //Debug.Log("Feed Video URL " + mediaUrl);
                //feedMediaPlayer.OpenMedia(new MediaPath(mediaUrl, MediaPathType.AbsolutePathOrURL), autoPlay: false);
            });
        }
    }

    public void GetImageFromAWS(string key, Image mainImage)
    {
        //Debug.Log("GetImageFromAWS key:" + key);
        //GetExtentionType(key);
        if (AssetCache.Instance.HasFile(key))
        {
            //Debug.Log("Image Available on Disk");
            AssetCache.Instance.LoadSpriteIntoImage(mainImage, key, changeAspectRatio: true);
            return;
        }
        else
        {
            AssetCache.Instance.EnqueueOneResAndWait(key, (ConstantsGod.r_AWSImageKitBaseUrl + key), (success) =>
            {
                if (success)
                {
                    AssetCache.Instance.LoadSpriteIntoImage(mainImage, key, changeAspectRatio: true);
                }
            });
        }
    }

    /*public static ExtentionType currentExtention;
    public static ExtentionType GetExtentionType(string path)
    {
        if (string.IsNullOrEmpty(path))
            return (ExtentionType)0;

        string extension = Path.GetExtension(path);
        if (string.IsNullOrEmpty(extension))
            return (ExtentionType)0;

        if (extension[0] == '.')
        {
            if (extension.Length == 1)
                return (ExtentionType)0;

            extension = extension.Substring(1);
        }

        extension = extension.ToLowerInvariant();
        //Debug.Log("ExtentionType " + extension);
        if (extension == "png" || extension == "jpg" || extension == "jpeg" || extension == "gif" || extension == "bmp" || extension == "tiff" || extension == "heic")
        {
            currentExtention = ExtentionType.Image;
            return ExtentionType.Image;
        }
        else if (extension == "mp4" || extension == "mov" || extension == "wav" || extension == "avi")
        {
            currentExtention = ExtentionType.Video;
            return ExtentionType.Video;
        }
        else if (extension == "mp3" || extension == "aac" || extension == "flac")
        {
            currentExtention = ExtentionType.Audio;
            return ExtentionType.Audio;
        }
        return (ExtentionType)0;
    }*/
    #endregion

    #region Feed Comment Screeen Methods
    //this method is used to open comment screen.......
    public void OpenCommentPanel()
    {
        if (!commentPanel.activeInHierarchy)
        {
            commentInputFieldAdvanced.Clear();
            commentPanel.SetActive(true);
        }
    }

    //this method is used to Full Feed View screen comment button click.......
    public void OnClickFeedBottomCommentButton()
    {
        OpenCommentPanel();
        commentInputFieldAdvanced.Select();
    }

    //this method is used to comment on feed after update require feed response.......
    public void CommentSuccessAfterUpdateRequireFeedResponse()
    {
       Debug.Log("CommentSuccessAfterUpdateRequireFeedResponse:" + feedFullViewScreenCallingFrom);
        switch (feedFullViewScreenCallingFrom)
        {
            case "MyProfile":
                PostFeedVideoItem postFeedVideoItem = videofeedParent.GetChild(videoFeedRect.GetComponent<ScrollSnapRect>()._currentPage).GetComponent<PostFeedVideoItem>();
                //current full feed selected item data update.......
                postFeedVideoItem.userData.commentCount += 1;

                string commentCountSTR = GetAbreviation(postFeedVideoItem.userData.commentCount);
                if (commentCountSTR != "0")
                {
                    postFeedVideoItem.commentBtn.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = commentCountSTR;
                }

                //for image.......
                if (!string.IsNullOrEmpty(postFeedVideoItem.userData.Image))
                {
                    //for
                    AllFeedByUserIdRow allFeedByUserIdRow = MyProfileDataManager.Instance.allMyFeedImageRootDataList.Find((x) => x.Id == postFeedVideoItem.userData.Id);
                    allFeedByUserIdRow.commentCount = postFeedVideoItem.userData.commentCount;

                    int index = MyProfileDataManager.Instance.allMyFeedImageRootDataList.IndexOf(allFeedByUserIdRow);
                    if (MyProfileDataManager.Instance.allPhotoContainer.childCount > index)
                    {
                        MyProfileDataManager.Instance.allPhotoContainer.GetChild(index).GetComponent<UserPostItem>().userData.commentCount = allFeedByUserIdRow.commentCount;
                    }
                }
                else
                {
                    AllFeedByUserIdRow allFeedByUserIdRow = MyProfileDataManager.Instance.allMyFeedVideoRootDataList.Find((x) => x.Id == postFeedVideoItem.userData.Id);
                    allFeedByUserIdRow.commentCount = postFeedVideoItem.userData.commentCount;

                    int index = MyProfileDataManager.Instance.allMyFeedVideoRootDataList.IndexOf(allFeedByUserIdRow);
                    if (MyProfileDataManager.Instance.allMovieContainer.childCount > index)
                    {
                        MyProfileDataManager.Instance.allMovieContainer.GetChild(index).GetComponent<UserPostItem>().userData.commentCount = allFeedByUserIdRow.commentCount;
                    }
                }
               Debug.Log("Full Feed Screen Comment calling from MyProfile");
                break;
            case "FeedPage":
                PostFeedVideoItem postFeedPageVideoItem = videofeedParent.GetChild(videoFeedRect.GetComponent<ScrollSnapRect>()._currentPage).GetComponent<PostFeedVideoItem>();
                //current full feed selected item data update.......
                postFeedPageVideoItem.userData.commentCount += 1;

                string FeedcommentCountSTR = GetAbreviation(postFeedPageVideoItem.userData.commentCount);
                if (FeedcommentCountSTR != "0")
                {
                    postFeedPageVideoItem.commentBtn.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = FeedcommentCountSTR;
                }

                //for image.......
                if (!string.IsNullOrEmpty(postFeedPageVideoItem.userData.Image))
                {
                    //for
                    AllFeedByUserIdRow allFeedByUserIdRow = MyProfileDataManager.Instance.allMyFeedImageRootDataList.Find((x) => x.Id == postFeedPageVideoItem.userData.Id);
                    allFeedByUserIdRow.commentCount = postFeedPageVideoItem.userData.commentCount;

                    int index = MyProfileDataManager.Instance.allMyFeedImageRootDataList.IndexOf(allFeedByUserIdRow);
                    if (MyProfileDataManager.Instance.allPhotoContainer.childCount > index)
                    {
                        MyProfileDataManager.Instance.allPhotoContainer.GetChild(index).GetComponent<UserPostItem>().userData.commentCount = allFeedByUserIdRow.commentCount;
                    }
                }
                else
                {
                    AllFeedByUserIdRow allFeedByUserIdRow = MyProfileDataManager.Instance.allMyFeedVideoRootDataList.Find((x) => x.Id == postFeedPageVideoItem.userData.Id);
                    allFeedByUserIdRow.commentCount = postFeedPageVideoItem.userData.commentCount;

                    int index = MyProfileDataManager.Instance.allMyFeedVideoRootDataList.IndexOf(allFeedByUserIdRow);
                    if (MyProfileDataManager.Instance.allMovieContainer.childCount > index)
                    {
                        MyProfileDataManager.Instance.allMovieContainer.GetChild(index).GetComponent<UserPostItem>().userData.commentCount = allFeedByUserIdRow.commentCount;
                    }
                }
               Debug.Log("Full Feed Screen Comment calling from FeedPage");
                break;
            case "OtherProfile":
                //current full feed selected item data update.......
                PostFeedVideoItem postFeedVideoItem1 = videofeedParent.GetChild(videoFeedRect.GetComponent<ScrollSnapRect>()._currentPage).GetComponent<PostFeedVideoItem>();
                postFeedVideoItem1.userData.commentCount += 1;

                string commentCountSTR1 = GetAbreviation(postFeedVideoItem1.userData.commentCount);
                if (commentCountSTR1 != "0")
                {
                    postFeedVideoItem1.commentBtn.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = commentCountSTR1;
                }

                //for image.......
                if (!string.IsNullOrEmpty(postFeedVideoItem1.userData.Image))
                {
                    AllFeedByUserIdRow allFeedByUserIdRow = OtherPlayerProfileData.Instance.allMyFeedImageRootDataList.Find((x) => x.Id == postFeedVideoItem1.userData.Id);
                    allFeedByUserIdRow.commentCount = postFeedVideoItem1.userData.commentCount;

                    int index = OtherPlayerProfileData.Instance.allMyFeedImageRootDataList.IndexOf(allFeedByUserIdRow);
                    if (OtherPlayerProfileData.Instance.userPostParent.childCount > index)
                    {
                        OtherPlayerProfileData.Instance.userPostParent.GetChild(index).GetComponent<UserPostItem>().userData.commentCount = allFeedByUserIdRow.commentCount;
                    }
                }
                else
                {
                    AllFeedByUserIdRow allFeedByUserIdRow = OtherPlayerProfileData.Instance.allMyFeedVideoRootDataList.Find((x) => x.Id == postFeedVideoItem1.userData.Id);
                    allFeedByUserIdRow.commentCount = postFeedVideoItem1.userData.commentCount;

                    int index = OtherPlayerProfileData.Instance.allMyFeedVideoRootDataList.IndexOf(allFeedByUserIdRow);
                    if (OtherPlayerProfileData.Instance.allMovieContainer.childCount > index)
                    {
                        OtherPlayerProfileData.Instance.allMovieContainer.GetChild(index).GetComponent<UserPostItem>().userData.commentCount = allFeedByUserIdRow.commentCount;
                    }
                }
               Debug.Log("Full Feed Screen Comment calling from OtherProfile");
                break;
            case "HotTab":
                CommentAfterRefereshDiscoverAndHotFeedItemResponse();
               Debug.Log("Full Feed Screen Comment calling from HotTab");
                break;
            case "FollowingTab":
                CommentAfterRefereshFollowingFeedItemResponse();
               Debug.Log("Full Feed Screen Comment calling from FollowingTab");
                break;
            case "DiscoverTab":
                CommentAfterRefereshDiscoverAndHotFeedItemResponse();
               Debug.Log("Full Feed Screen Comment calling from DiscoverTab");
                break;
            default:
                break;
        }
    }

    //this method is used to refresh comment response Following feed items.......
    void CommentAfterRefereshFollowingFeedItemResponse()
    {
        //current full feed selected item data update.......
        FollowingUserFeedItem feedFollowingItemController = videofeedParent.GetChild(videoFeedRect.GetComponent<ScrollSnapRect>()._currentPage).GetComponent<FollowingUserFeedItem>();
        feedFollowingItemController.FollowingUserFeedData.commentCount += 1;

        string commentCountSTR = GetAbreviation(feedFollowingItemController.FollowingUserFeedData.commentCount);
        if (commentCountSTR != "0")
        {
            feedFollowingItemController.commentBtn.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = commentCountSTR;
        }

        FeedsByFollowingUserRow feedsByFollowingUserRow = APIManager.Instance.allFollowingUserRootList.Find((x) => x.Id == feedFollowingItemController.FollowingUserFeedData.Id);
        if (feedsByFollowingUserRow != null)
        {
            feedsByFollowingUserRow.commentCount = feedFollowingItemController.FollowingUserFeedData.commentCount;
        }
    }

    //this method is used to refresh comment response Hot and Discover feed items.......
    void CommentAfterRefereshDiscoverAndHotFeedItemResponse()
    {
        //current full feed selected item data update.......
        FeedVideoItem feedVideoItem = videofeedParent.GetChild(videoFeedRect.GetComponent<ScrollSnapRect>()._currentPage).GetComponent<FeedVideoItem>();
        feedVideoItem.hotFeed.commentCount += 1;

        string commentCountSTR = GetAbreviation(feedVideoItem.hotFeed.commentCount);
        if (commentCountSTR != "0")
        {
            feedVideoItem.commentBtn.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = commentCountSTR;
        }
        if (feedFullViewScreenCallingFrom == "HotTab")
        {
           HotFeed hotFeed = APIManager.Instance.allhotFeedRoot.data.rows.Find((x) => x.id == feedVideoItem.hotFeed.id);

            if (hotFeed != null)
            {
                
                    hotFeed.commentCount = feedVideoItem.hotFeed.commentCount;
            }
        }
        else
        {
            AllUserWithFeedRow allUserWithFeedRow = APIManager.Instance.allUserRootList.Find((x) => x.id == feedVideoItem.FeedRawData.id);

            if (allUserWithFeedRow != null)
            {
                AllUserWithFeed allUserWithFeed = allUserWithFeedRow.feeds.Find((x) => x.id == feedVideoItem.FeedData.id);

                if (allUserWithFeed != null)
                {
                    allUserWithFeed.commentCount = feedVideoItem.FeedData.commentCount;
                }
            }
        }
    }
    #endregion

    #region Feed Like or DisLike methods.......
    //this method is used to like or dislike feed after update require feed response.......
    public void LikeDislikeSuccessAfterUpdateRequireFeedResponse(bool isLike, int likeCount)
    {
       Debug.Log("LikeDislikeSuccessAfterUpdateRequireFeedResponse:" + feedFullViewScreenCallingFrom + "  :isLike:" + isLike + "  :LikeCount:" + likeCount);
        switch (feedFullViewScreenCallingFrom)
        {
            case "MyProfile":
                //current full feed selected item data update.......
                PostFeedVideoItem postFeedVideoItem = videofeedParent.GetChild(videoFeedRect.GetComponent<ScrollSnapRect>()._currentPage).GetComponent<PostFeedVideoItem>();
                postFeedVideoItem.userData.LikeCount = likeCount;
                postFeedVideoItem.userData.isLike = isLike;
                postFeedVideoItem.LikeCountAndUISetup(postFeedVideoItem.userData.LikeCount);

                int index = APIManager.Instance.allFeedWithUserIdRoot.Data.Rows.FindIndex(x => x.Id == postFeedVideoItem.userData.Id);
                APIManager.Instance.allFeedWithUserIdRoot.Data.Rows[index].LikeCount = likeCount;
                APIManager.Instance.allFeedWithUserIdRoot.Data.Rows[index].isLike = isLike;
                //for image.......
                if (!string.IsNullOrEmpty(postFeedVideoItem.userData.Image))
                {
                    //for
                    AllFeedByUserIdRow allFeedByUserIdRow = MyProfileDataManager.Instance.allMyFeedImageRootDataList.Find((x) => x.Id == postFeedVideoItem.userData.Id);
                    allFeedByUserIdRow.LikeCount = postFeedVideoItem.userData.LikeCount;
                    allFeedByUserIdRow.isLike = postFeedVideoItem.userData.isLike;
                }
                else
                {
                    AllFeedByUserIdRow allFeedByUserIdRow = MyProfileDataManager.Instance.allMyFeedVideoRootDataList.Find((x) => x.Id == postFeedVideoItem.userData.Id);
                    allFeedByUserIdRow.isLike = postFeedVideoItem.userData.isLike;
                }
                // Debug.Log("Full Feed Screen like or dislike calling from MyProfile");
                break;
            case "OtherProfile":
                //current full feed selected item data update.......
                PostFeedVideoItem postFeedVideoItem1 = videofeedParent.GetChild(videoFeedRect.GetComponent<ScrollSnapRect>()._currentPage).GetComponent<PostFeedVideoItem>();
                postFeedVideoItem1.userData.LikeCount = likeCount;
                postFeedVideoItem1.userData.isLike = isLike;
                postFeedVideoItem1.LikeCountAndUISetup(postFeedVideoItem1.userData.LikeCount);

                //for image.......
                if (!string.IsNullOrEmpty(postFeedVideoItem1.userData.Image))
                {
                    AllFeedByUserIdRow allFeedByUserIdRow = OtherPlayerProfileData.Instance.allMyFeedImageRootDataList.Find((x) => x.Id == postFeedVideoItem1.userData.Id);
                    allFeedByUserIdRow.LikeCount = postFeedVideoItem1.userData.LikeCount;
                    allFeedByUserIdRow.isLike = postFeedVideoItem1.userData.isLike;
                }
                else
                {
                    AllFeedByUserIdRow allFeedByUserIdRow = OtherPlayerProfileData.Instance.allMyFeedVideoRootDataList.Find((x) => x.Id == postFeedVideoItem1.userData.Id);
                    allFeedByUserIdRow.LikeCount = postFeedVideoItem1.userData.LikeCount;
                    allFeedByUserIdRow.isLike = postFeedVideoItem1.userData.isLike;
                }
                // Debug.Log("Full Feed Screen like or dislike calling from OtherProfile");
                break;
            case "HotTab":
                LikeOrDisLikeAfterRefereshDiscoverAndHotFeedItemResponse(isLike, likeCount);
                //  Debug.Log("Full Feed Screen like or dislike calling from HotTab");
                break;
            case "FollowingTab":
                LikeOrDisLikeAfterRefereshFollowingFeedItemResponse(isLike, likeCount);
                // Debug.Log("Full Feed Screen like or dislike calling from FollowingTab");
                break;
            case "DiscoverTab":
                LikeOrDisLikeAfterRefereshDiscoverAndHotFeedItemResponse(isLike, likeCount);
                break;
            case "FeedPage":
                PostFeedVideoItem postFeedVideoItemFeedPage = videofeedParent.GetChild(videoFeedRect.GetComponent<ScrollSnapRect>()._currentPage).GetComponent<PostFeedVideoItem>();
                postFeedVideoItemFeedPage.userData.LikeCount = likeCount;
                postFeedVideoItemFeedPage.userData.isLike = isLike;
                postFeedVideoItemFeedPage.LikeCountAndUISetup(postFeedVideoItemFeedPage.userData.LikeCount);

                //for image.......
                if (!string.IsNullOrEmpty(postFeedVideoItemFeedPage.userData.Image))
                {
                    //for
                    AllFeedByUserIdRow allFeedByUserIdRow = MyProfileDataManager.Instance.allMyFeedImageRootDataList.Find((x) => x.Id == postFeedVideoItemFeedPage.userData.Id);
                    allFeedByUserIdRow.LikeCount = postFeedVideoItemFeedPage.userData.LikeCount;
                    allFeedByUserIdRow.isLike = postFeedVideoItemFeedPage.userData.isLike;
                }
                else
                {
                    AllFeedByUserIdRow allFeedByUserIdRow = MyProfileDataManager.Instance.allMyFeedVideoRootDataList.Find((x) => x.Id == postFeedVideoItemFeedPage.userData.Id);
                    allFeedByUserIdRow.isLike = postFeedVideoItemFeedPage.userData.isLike;
                }
                // Debug.Log("Full Feed Screen like or dislike calling from DiscoverTab");
                break;
            default:
                break;
        }
    }

    //this method is used to refresh like or dislike response Following feed items.......
    void LikeOrDisLikeAfterRefereshFollowingFeedItemResponse(bool isLike, int likeCount)
    {
        //current full feed selected item data update.......
        FollowingUserFeedItem feedFollowingItemController = videofeedParent.GetChild(videoFeedRect.GetComponent<ScrollSnapRect>()._currentPage).GetComponent<FollowingUserFeedItem>();
        // Debug.Log("LikeOrDisLike islike:" + isLike + "     :FeedId:" + feedFollowingItemController.FollowingUserFeedData.Id + "    :likeCount:" + feedFollowingItemController.FollowingUserFeedData.LikeCount);
        feedFollowingItemController.FollowingUserFeedData.LikeCount = likeCount;
        feedFollowingItemController.FollowingUserFeedData.isLike = isLike;

        feedFollowingItemController.LikeCountAndUISetup(feedFollowingItemController.FollowingUserFeedData.LikeCount);

        FeedsByFollowingUserRow feedsByFollowingUserRow = APIManager.Instance.allFollowingUserRootList.Find((x) => x.Id == feedFollowingItemController.FollowingUserFeedData.Id);
        if (feedsByFollowingUserRow != null)
        {
            feedsByFollowingUserRow.LikeCount = feedFollowingItemController.FollowingUserFeedData.LikeCount;
            feedsByFollowingUserRow.isLike = feedFollowingItemController.FollowingUserFeedData.isLike;
        }
    }

    //this method is used to refresh comment response Hot and Discover feed items.......
    void LikeOrDisLikeAfterRefereshDiscoverAndHotFeedItemResponse(bool isLike, int likeCount)
    {
        //current full feed selected item data update.......
        FeedVideoItem feedVideoItem = videofeedParent.GetChild(videoFeedRect.GetComponent<ScrollSnapRect>()._currentPage).GetComponent<FeedVideoItem>();
        // Debug.Log("LikeOrDisLike islike:" + isLike + "     :feedRawData Id:" + feedVideoItem.FeedRawData.id + "    :FeedId:" + feedVideoItem.FeedData.id + "   :likeCount:" + feedVideoItem.FeedData.likeCount);
        if (feedFullViewScreenCallingFrom == "HotTab")
        {
            feedVideoItem.hotFeed.likeCount = likeCount;
            feedVideoItem.hotFeed.isLike = isLike;
            feedVideoItem.LikeCountAndUISetup(feedVideoItem.hotFeed.likeCount);
            HotFeed hotfeed = APIManager.Instance.allhotFeedRoot.data.rows.Find((x) => x.id == feedVideoItem.hotFeed.id);
            if (hotfeed != null)
            {
                hotfeed.likeCount = feedVideoItem.hotFeed.likeCount;
                hotfeed.isLike = feedVideoItem.hotFeed.isLike;
            }
        }
        else
        {
            feedVideoItem.FeedData.likeCount = likeCount;
            feedVideoItem.FeedData.isLike = isLike;
            feedVideoItem.LikeCountAndUISetup(feedVideoItem.FeedData.likeCount);
            AllUserWithFeedRow allUserWithFeedRow = APIManager.Instance.allUserRootList.Find((x) => x.id == feedVideoItem.FeedRawData.id);

            if (allUserWithFeedRow != null)
            {
                AllUserWithFeed allUserWithFeed = allUserWithFeedRow.feeds.Find((x) => x.id == feedVideoItem.FeedData.id);

                if (allUserWithFeed != null)
                {
                    allUserWithFeed.likeCount = feedVideoItem.FeedData.likeCount;
                    allUserWithFeed.isLike = feedVideoItem.FeedData.isLike;
                }
            }
        }
    }
    #endregion

    #region Get Int Value in 1K 1M 1B.......
    public string GetAbreviation(int value)
    {
        if (value < 0)
        {
            value = 0;
        }
        int NrOfDigits = GetNumberOfDigits(value);
        double FirstDigits;
        string Abreviation = "";
        //Debug.Log (NrOfDigits);
        if (NrOfDigits % 3 == 0)
        {
            FirstDigits = value / (Mathf.Pow(10, NrOfDigits - 3));
            if ((Mathf.Pow(10, NrOfDigits - 3)) / 1000 == 1)
            {
                Abreviation = "K";
            }
            else if ((Mathf.Pow(10, NrOfDigits - 3)) / 1000000 == 1)
            {
                Abreviation = "M";
            }
            else if ((Mathf.Pow(10, NrOfDigits - 3)) / 1000000000 == 1)
            {
                Abreviation = "B";
            }
        }
        else
        {
            FirstDigits = value / (Mathf.Pow(10, NrOfDigits - NrOfDigits % 3));
            if ((Mathf.Pow(10, NrOfDigits - NrOfDigits % 3)) / 1000 == 1)
            {
                Abreviation = "K";
            }
            else if ((Mathf.Pow(10, NrOfDigits - NrOfDigits % 3)) / 1000000 == 1)
            {
                Abreviation = "M";
            }
            else if ((Mathf.Pow(10, NrOfDigits - NrOfDigits % 3)) / 1000000000 == 1)
            {
                Abreviation = "B";
            }
        }
        int valueOfFlaot = 0;
        if (FirstDigits % 1 > 0)
        {
            valueOfFlaot = 2;
        }
        return FirstDigits.ToString("F" + valueOfFlaot.ToString()) + Abreviation;
    }

    private int GetNumberOfDigits(int value)
    {
        int NrOfDigits = 0;
        while (value > 0)
        {
            value = value / 10;
            NrOfDigits++;
        }
        return NrOfDigits;
    }

    public void TestKMBValue(int value)
    {
       Debug.Log("Test Value:" + GetAbreviation(value));
    }
    #endregion

    #region Feed Comment and Like Socket Event Refresh Data.......
    public void FeedCommentAndLikeSocketEventSuccessAfterUpdateRequireFeedResponse(int feedId, int createdBY, int likeCount, int commentCount, bool isComesFromLike)
    {
       Debug.Log("LikeDislikeSuccessAfterUpdateRequireFeedResponse:" + feedFullViewScreenCallingFrom);
        switch (feedFullViewScreenCallingFrom)
        {
            case "MyProfile":
                //  Debug.Log("Full Feed Screen like or dislike or feed comment socket event calling from MyProfile");
                FeedCommentAndLikeSocketEventAfterRefereshMyProfileResponse(feedId, likeCount, commentCount, isComesFromLike, true);
                break;
            case "OtherProfile":
                // Debug.Log("Full Feed Screen like or dislike or feed comment socket event calling from OtherProfile");
                FeedCommentAndLikeSocketEventAfterRefereshOtherUserProfileResponse(feedId, likeCount, commentCount, isComesFromLike, true);
                break;
            case "HotTab":
                //  Debug.Log("Full Feed Screen like or dislike or feed comment socket event calling from HotTab");
                FeedCommentAndLikeSocketEventAfterRefereshDiscoverAndHotFeedItemResponse(feedId, createdBY, likeCount, commentCount, isComesFromLike, true);
                break;
            case "FollowingTab":
                //  Debug.Log("Full Feed Screen like or dislike or feed comment socket event calling from FollowingTab");
                FeedCommentAndLikeSocketEventAfterRefereshFollowingFeedItemResponse(feedId, likeCount, commentCount, isComesFromLike, true);
                break;
            case "DiscoverTab":
                //  Debug.Log("Full Feed Screen like or dislike or feed comment socket event calling from DiscoverTab");
                FeedCommentAndLikeSocketEventAfterRefereshDiscoverAndHotFeedItemResponse(feedId, createdBY, likeCount, commentCount, isComesFromLike, true);
                break;
            case "FeedPage":
                //  Debug.Log("Full Feed Screen like or dislike or feed comment socket event calling from DiscoverTab");
                FeedCommentAndLikeSocketEventAfterRefereshMyProfileResponse(feedId, likeCount, commentCount, isComesFromLike, true);
                break;
            default:
               Debug.Log("Default call.......");
                FeedCommentAndLikeSocketEventAfterRefereshDiscoverAndHotFeedItemResponse(feedId, createdBY, likeCount, commentCount, isComesFromLike, false);
                FeedCommentAndLikeSocketEventAfterRefereshFollowingFeedItemResponse(feedId, likeCount, commentCount, isComesFromLike, false);
                FeedCommentAndLikeSocketEventAfterRefereshMyProfileResponse(feedId, likeCount, commentCount, isComesFromLike, false);
                FeedCommentAndLikeSocketEventAfterRefereshOtherUserProfileResponse(feedId, likeCount, commentCount, isComesFromLike, false);
                //  FeedCommentAndLikeSocketEventAfterRefereshMyProfileResponse(feedId, likeCount, commentCount, isComesFromLike, true);
                break;
        }
    }

    //this method is used to refresh like or dislike or comment socket event response Following Tab.......
    void FeedCommentAndLikeSocketEventAfterRefereshFollowingFeedItemResponse(int feedId, int likeCount, int commentCount, bool isComesFromLike, bool isFullFeedScreen)
    {
        //Debug.Log("FeedCommentAndLikeSocketEventAfterRefereshFollowingFeedItemResponse feed ID:" + feedId + "  :LikeCount:" + likeCount + "    :CommentCount:" + commentCount + "  :IsComesFromLike:" + isComesFromLike + "    :IsFullScreen:" + isFullFeedScreen);
        if (APIController.Instance.feedFollowingIdList.Contains(feedId))
        {
            //Debug.Log("Following tab feed data updated");
            FeedsByFollowingUserRow feedsByFollowingUserRow = APIManager.Instance.allFollowingUserRootList.Find((x) => x.Id == feedId);
            if (feedsByFollowingUserRow != null)
            {
                if (isComesFromLike)
                {
                    feedsByFollowingUserRow.LikeCount = likeCount;
                }
                else
                {
                    feedsByFollowingUserRow.commentCount = commentCount;
                }
            }

            if (isFullFeedScreen)
            {
                int index = APIController.Instance.feedFollowingIdList.IndexOf(feedId);
                //Debug.Log("Index:" + index);
                //current full feed selected item data update.......
                FollowingUserFeedItem feedFollowingItemController = videofeedParent.GetChild(index).GetComponent<FollowingUserFeedItem>();
                if (isComesFromLike)
                {
                    feedFollowingItemController.FollowingUserFeedData.LikeCount = likeCount;
                    feedFollowingItemController.LikeCountAndUISetup(feedFollowingItemController.FollowingUserFeedData.LikeCount);
                }
                else
                {
                    feedFollowingItemController.FollowingUserFeedData.commentCount = commentCount;
                    feedFollowingItemController.CommentCountUISetup(feedFollowingItemController.FollowingUserFeedData.commentCount);

                    //Debug.Log("Comment Panel.......:" + commentPanel.activeInHierarchy);
                    if (commentPanel.activeInHierarchy)//if comment screen is open then refresh comment list api.......
                    {
                        feedFollowingItemController.OnClickCommentButton(true);
                    }
                }
            }
        }
    }

    //this method is used to refresh like or dislike or comment socket event response Hot and Discover Tab.......
    void FeedCommentAndLikeSocketEventAfterRefereshDiscoverAndHotFeedItemResponse(int feedId, int createdBY, int likeCount, int commentCount, bool isComesFromLike, bool isFullFeedScreen)
    {
        //Debug.Log("FeedCommentAndLikeSocketEventAfterRefereshDiscoverAndHotFeedItemResponse feed ID:" + feedId + "    :CreatedBY:" + createdBY + "  :LikeCount:" + likeCount + "    :CommentCount:" + commentCount + "  :IsComesFromLike:" + isComesFromLike + "    :IsFullScreen:" + isFullFeedScreen);
        if (APIController.Instance.feedHotIdList.Contains(feedId))
        {
           Debug.Log("here 1");
            //if (APIController.Instance.feedForYouIdList.Contains(feedId))
            //{
           Debug.Log("Hot or Discover tab feed data updated");
           Debug.Log("createdBY => " + createdBY);
            //Debug.Log("createdBY => "+ createdBY);




            /*
            AllUserWithFeedRow allUserWithFeedRow = APIManager.Instance.allUserRootList.Find((x) => x.UserProfile.id == createdBY);

            if (allUserWithFeedRow != null)
            {
                AllUserWithFeed allUserWithFeed = allUserWithFeedRow.feeds.Find((x) => x.id == feedId);

                if (allUserWithFeed != null)
                {
                    if (isComesFromLike)
                    {
                        allUserWithFeed.likeCount = likeCount;
                    }
                    else
                    {
                        allUserWithFeed.commentCount = commentCount;
                    }
                }
            }
            */

            if (isFullFeedScreen)
            {
                //Debug.Log("isfullscreen");
                int index = -1;

                for (int i = 0; i < videofeedParent.childCount; i++)
                {

                    if (videofeedParent.GetChild(i).GetComponent<FeedVideoItem>().FeedData.id == feedId)
                    {
                        index = i;
                    }
                }
                //   int index = APIController.Instance.feedForYouIdList.IndexOf(feedId);
                // Debug.Log("Index:" + index);
                //current full feed selected item data update.......
                FeedVideoItem feedVideoItem = videofeedParent.GetChild(index).GetComponent<FeedVideoItem>();
                //  Debug.Log("FeedvideoItem:" + feedVideoItem.FeedData.id + "  feediID :" + feedId);

                if (isComesFromLike)
                {
                    if (feedFullViewScreenCallingFrom == "HotTab")
                    {
                        feedVideoItem.hotFeed.likeCount = likeCount;
                        feedVideoItem.LikeCountAndUISetup(feedVideoItem.hotFeed.likeCount);
                    }
                    else
                    {
                        feedVideoItem.FeedData.likeCount = likeCount;
                        feedVideoItem.LikeCountAndUISetup(feedVideoItem.FeedData.likeCount);
                    }
                }
                else
                {
                    if (feedFullViewScreenCallingFrom == "HotTab")
                    {
                        feedVideoItem.hotFeed.commentCount = commentCount;
                        feedVideoItem.CommentCountUISetup(feedVideoItem.hotFeed.commentCount);
                    }
                    else
                    {
                        feedVideoItem.FeedData.commentCount = commentCount;
                        feedVideoItem.CommentCountUISetup(feedVideoItem.FeedData.commentCount);
                    }

                    //Debug.Log("Comment Panel.......:" + commentPanel.activeInHierarchy);
                    if (commentPanel.activeInHierarchy)//if comment screen is open then refresh comment list api.......
                    {
                        if (APIManager.Instance.feedIdTemp == feedVideoItem.FeedData.id)
                        {
                            feedVideoItem.OnClickCommentButton(true);
                        }
                    }
                }
            }
        }
        // }
    }

    //this method is used to refresh like or dislike or comment socket event response MyProfile screen.......
    void FeedCommentAndLikeSocketEventAfterRefereshMyProfileResponse(int feedId, int likeCount, int commentCount, bool isComesFromLike, bool isFullFeedScreen)
    {
       Debug.Log("FeedCommentAndLikeSocketEventAfterRefereshMyProfileResponse feed ID:" + feedId + "  :LikeCount:" + likeCount + "    :CommentCount:" + commentCount + "  :IsComesFromLike:" + isComesFromLike + "    :IsFullScreen:" + isFullFeedScreen);
        if (MyProfileDataManager.Instance.loadedMyPostAndVideoId.Contains(feedId))
        {
            AllFeedByUserIdRow allFeedByUserIdRow = MyProfileDataManager.Instance.allMyFeedImageRootDataList.Find((x) => x.Id == feedId);
            if (allFeedByUserIdRow == null)
            {
                allFeedByUserIdRow = MyProfileDataManager.Instance.allMyFeedVideoRootDataList.Find((x) => x.Id == feedId);
            }
            //Debug.Log("here");
            if (allFeedByUserIdRow != null)
            {
                //Debug.Log("my profile feed data updated");
                if (isComesFromLike)
                {
                    allFeedByUserIdRow.LikeCount = likeCount;
                }
                else
                {
                    allFeedByUserIdRow.commentCount = commentCount;
                }
                if (isFullFeedScreen)
                {
                    // Debug.Log("here 2");
                    int index;
                    if (!string.IsNullOrEmpty(allFeedByUserIdRow.Image))
                    {
                        index = MyProfileDataManager.Instance.allMyFeedImageRootDataList.IndexOf(allFeedByUserIdRow);
                    }
                    else
                    {
                        index = MyProfileDataManager.Instance.allMyFeedVideoRootDataList.IndexOf(allFeedByUserIdRow);
                    }

                    if (index < videofeedParent.childCount)
                    {
                        PostFeedVideoItem postFeedVideoItem = videofeedParent.GetChild(index).GetComponent<PostFeedVideoItem>();
                        // Debug.Log("index:" + index + " :postFeedVideoItem Id:" + postFeedVideoItem.userData.Id);
                        //Debug.Log("name => "+ postFeedVideoItem.transform.name);
                        if (postFeedVideoItem.userData.Id == feedId)
                        {
                            //Debug.Log("here 3");
                            if (isComesFromLike)
                            {
                                // Debug.Log("likeCount" + likeCount);
                                postFeedVideoItem.userData.LikeCount = likeCount;
                                postFeedVideoItem.LikeCountAndUISetup(postFeedVideoItem.userData.LikeCount);
                            }
                            else
                            {
                                postFeedVideoItem.userData.commentCount = commentCount;
                                postFeedVideoItem.CommentCountUISetup(postFeedVideoItem.userData.commentCount);
                                Debug.Log("Comment Panel.......:" + commentPanel.activeInHierarchy);
                                if (commentPanel.activeInHierarchy)//if comment screen is open then refresh comment list api.......
                                {
                                    if (APIManager.Instance.feedIdTemp == feedId)
                                    {
                                        postFeedVideoItem.OnClickCommentButton(true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        if (MyProfileDataManager.Instance.loadedMyPostAndVideoIdInFeedPage.Contains(feedId))
        {
            AllFeedByUserIdRow allFeedByUserIdRow = APIManager.Instance.allFeedWithUserIdRoot.Data.Rows.Find((x) => x.Id == feedId);
            //Debug.Log("here");
            if (allFeedByUserIdRow != null)
            {
                //Debug.Log("my profile feed data updated");
                if (isComesFromLike)
                {
                    allFeedByUserIdRow.LikeCount = likeCount;
                }
                else
                {
                    allFeedByUserIdRow.commentCount = commentCount;
                }
                int index;
                index = APIManager.Instance.allFeedWithUserIdRoot.Data.Rows.IndexOf(allFeedByUserIdRow);
                if (isComesFromLike)
                {
                    APIManager.Instance.allFeedWithUserIdRoot.Data.Rows[index].LikeCount = likeCount;
                }
                else
                {
                    APIManager.Instance.allFeedWithUserIdRoot.Data.Rows[index].commentCount = commentCount;
                }
            }
        }
    }

    //this method is used to refresh like or dislike or comment socket event response Other user Profile screen.......
    void FeedCommentAndLikeSocketEventAfterRefereshOtherUserProfileResponse(int feedId, int likeCount, int commentCount, bool isComesFromLike, bool isFullFeedScreen)
    {
       Debug.Log("FeedCommentAndLikeSocketEventAfterRefereshOtherUserProfileResponse feed ID:" + feedId + "  :LikeCount:" + likeCount + "    :CommentCount:" + commentCount + "  :IsComesFromLike:" + isComesFromLike + "    :IsFullScreen:" + isFullFeedScreen);
        if (OtherPlayerProfileData.Instance.loadedMyPostAndVideoId.Contains(feedId))
        {
            AllFeedByUserIdRow allFeedByUserIdRow = OtherPlayerProfileData.Instance.allMyFeedImageRootDataList.Find((x) => x.Id == feedId);
            if (allFeedByUserIdRow == null)
            {
                allFeedByUserIdRow = OtherPlayerProfileData.Instance.allMyFeedVideoRootDataList.Find((x) => x.Id == feedId);
            }

            if (allFeedByUserIdRow != null)
            {
                //Debug.Log("my profile feed data updated");
                if (isComesFromLike)
                {
                    allFeedByUserIdRow.LikeCount = likeCount;
                }
                else
                {
                    allFeedByUserIdRow.commentCount = commentCount;
                }
                if (isFullFeedScreen)
                {
                    int index;
                    if (!string.IsNullOrEmpty(allFeedByUserIdRow.Image))
                    {
                        index = OtherPlayerProfileData.Instance.allMyFeedImageRootDataList.IndexOf(allFeedByUserIdRow);
                    }
                    else
                    {
                        index = OtherPlayerProfileData.Instance.allMyFeedVideoRootDataList.IndexOf(allFeedByUserIdRow);
                    }

                    if (index < videofeedParent.childCount)
                    {
                        PostFeedVideoItem postFeedVideoItem = videofeedParent.GetChild(index).GetComponent<PostFeedVideoItem>();
                        //Debug.Log("index:" + index + " :postFeedVideoItem Id:" + postFeedVideoItem.userData.Id);
                        if (postFeedVideoItem.userData.Id == feedId)
                        {
                            if (isComesFromLike)
                            {
                                postFeedVideoItem.userData.LikeCount = likeCount;
                                postFeedVideoItem.LikeCountAndUISetup(postFeedVideoItem.userData.LikeCount);
                            }
                            else
                            {
                                postFeedVideoItem.userData.commentCount = commentCount;
                                postFeedVideoItem.CommentCountUISetup(postFeedVideoItem.userData.commentCount);
                                //Debug.Log("Comment Panel.......:" + commentPanel.activeInHierarchy);
                                if (commentPanel.activeInHierarchy)//if comment screen is open then refresh comment list api.......
                                {
                                    if (APIManager.Instance.feedIdTemp == feedId)
                                    {
                                        postFeedVideoItem.OnClickCommentButton(true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    #endregion
}