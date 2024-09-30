using AdvancedInputFieldPlugin;
using DG.Tweening;
using RenderHeads.Media.AVProVideo;
using SuperStar.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class FeedUIController : MonoBehaviour
{
    public static FeedUIController Instance;

    [Header("-------FooterCan-------")]
    public GameObject footerCan;
    public HomeFooterHandler bottomTabManager;
    [Space]
    [Header("-------All Screens-------")]
    public GameObject feedUiScreen;
  
    public SNSAPILoaderController apiLoaderController;
    [Space]
    [Header("HorizontalScrollSnap")]

    public GameObject feedUiSelectionLine;
    public Transform[] feedUiSelectionTab;
    public TextMeshProUGUI[] feedUiTabTitleText;
    public Color selectedColor, unSelectedColor;
    [Space]
    [Header("Feed All Tabs Loaded checking Variable")]
    public int followingFeedInitiateTotalCount;
    public int hotFeedInitiateTotalCount;
    public int hotForYouFeedInitiateTotalCount;
    [Space]
    //this list is used to unfollowed user feed removed from following tab.......
    public List<int> unFollowedUserListForFollowingTab = new List<int>();


    [Space]
    [Header("Find Friend screen References")]
    public GameObject findFriendScreen;
    public Transform findFriendContainer;
    public AdvancedInputField findFriendInputFieldAdvanced;

    [Space]
    [Header("FadeInOut Screen Reference")]
    public GameObject fadeInOutScreen;
    public Color fadeInOutColor;

    [Space]
    [Header("Profile Follower and Following list Reference")]
    public GameObject profileFollowerFollowingListScreen;
    public GameObject profileFollowersPanel;
    public Transform profileFollowerListContainer;
    public GameObject noProfileFollowers;
    public GameObject profileFollowingPanel;
    public Transform profileFollowingListContainer;
    public GameObject noProfileFollowing;
    public Transform adFrndFollowingListContainer;
    public GameObject followerPrefab;
    public GameObject followingPrefab;
    public GameObject adFriendFollowingPrefab;
    public GameObject profileSerachBar;
    public GameObject profileSerachBarContainer;
    public Transform profileSerachResultsContainer;
    public GameObject profileFinfFriendScreen;
    public AdvancedInputField profileFinfFriendAdvancedInputField;
    public GameObject profileNoSearchFound;
    public Transform profileFFLineSelection;
    public Transform[] profileFFSelectionTab;
    public TextMeshProUGUI[] profileFFSelectionTabText;
    public Image[] profileFFSelectionTabLine;
    public int profileFollowerPaginationPageNo = 1;
    public int profileFollowingPaginationPageNo = 1;
    public int adFrndFollowingPaginationPageNo = 1;
    public bool isProfileFollowerDataLoaded = false;
    public bool isProfileFollowingDataLoaded = false;
    public bool isAdFrndFollowingDataLoaded = false;

    private List<int> profileFollowerLoadedItemIDList = new List<int>();
    private List<int> profileFollowingLoadedItemIDList = new List<int>();
    private List<int> adFrndFollowingLoadedItemIDList = new List<int>();
    public List<FollowerItemController> profileFollowerItemControllersList = new List<FollowerItemController>();
    public List<FollowingItemController> profileFollowingItemControllersList = new List<FollowingItemController>();
    public List<FollowingItemController> AdFrndFollowingItemControllersList = new List<FollowingItemController>();

    [Space]
    [Header("Add Friends")]
    [SerializeField] public List<TMP_Text> FrndsPanelBtns;
    [SerializeField] public List<Image> FrndsPanelBtnLines;
    [SerializeField] public GameObject AddFriendPanel;
    [SerializeField] public GameObject HotFriendPanel;
    public GameObject hotFriendContainer;
    [SerializeField] GameObject AddFrndRecommendedPanel;
    [SerializeField] public GameObject AddFrndRecommendedContainer;
    [SerializeField] GameObject AddFrndFollowingPanel;
    [SerializeField] GameObject AddFrndFollowingContainer;
    [SerializeField] public GameObject AddFrndNoFollowing;
    [SerializeField] GameObject AddFrndMutalFrndPanel;
    [SerializeField] public GameObject AddFrndMutalFrndContainer;
    [SerializeField] public GameObject AddFrndNoMutalFrnd;
    [SerializeField] public GameObject BestFriendFull;
    [SerializeField] GameObject AddFriendSerachBar;
    [SerializeField] GameObject AddFriendFollowing;
    [SerializeField] public GameObject AddFrndNoSearchFound;
    [SerializeField] public GameObject AddFriendPanelFollowingCont;
    [SerializeField] public GameObject AddFreindContainer;
    [SerializeField] public GameObject ExtraPrefab;
    public string SearchFriendInput = "";
    public string FollowFollowingSearchFriendInput = "";

    [Header("Feed 2.0")]
    [SerializeField] GameObject FeedSerachBar;
    public FeedController feedController;
    public SNSSettingController SNSSettingController;

    [Space]
    [Header("Confirm To Unfollow")]
    public GameObject ConfirmUnfollowPanel;
    public Button UnfollowButton;

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
        if (feedController == null)
        {
            feedController = feedUiScreen.GetComponentInChildren<FeedController>();
        }
    }

    IEnumerator WaitToStartCallForFeedScene()
    {
        yield return new WaitForSeconds(0.2f);
       Debug.Log("FeedUIController isLoginFromDifferentId:" + SNS_APIManager.Instance.isLoginFromDifferentId);
        if (SNS_APIManager.Instance.isLoginFromDifferentId)
        {
           Debug.Log("FeedUI Controller new user login and calling feed start function");
            ResetAllFeedScreen(false);
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
    public void ShowFriendLoader(bool isOnHere)
    {
        apiLoaderController.ShowFriendApiLoader(isOnHere);
    }

    public void OnFeedButtonTabBtnClick()
    {
       Debug.Log("OnFeedButtonTabBtnClick.......");
        feedUiScreen.SetActive(true);
    }

    public void ResetAllFeedScreen(bool isFeedScreen)
    {
        FeedUIController.Instance.findFriendInputFieldAdvanced.Text = "";
        FeedUIController.Instance.findFriendScreen.gameObject.SetActive(false);
        profileFollowerFollowingListScreen.SetActive(false);
        if (OtherPlayerProfileData.Instance)
        {
            OtherPlayerProfileData.Instance.backKeyManageList.Clear();
        }
        FeedUIController.Instance.SNSSettingController.myAccountScreen.SetActive(false);
        FeedUIController.Instance.SNSSettingController.myAccountPersonalInfoScreen.SetActive(false);
    }

    public void SetAddFriendScreen(bool flag){
        AddFriendPanel.SetActive(flag);    
        AddFriendSerachBar.SetActive(false);
        AddFreindContainer.GetComponent<VerticalLayoutGroup>().padding.top=50;
        AddFriendFollowing.SetActive(false);
        AddFrndNoSearchFound.SetActive(false);
        UpdateAdFrndBtnStatus(0);
    }

    public void OnClickAddFriendSearchBtn(){
        AddFriendSerachBar.SetActive(!AddFriendSerachBar.activeInHierarchy);
        if (AddFriendSerachBar.activeInHierarchy)
        {
            HotFriendPanel.GetComponentInParent<FollowParentHeight>().AddPading = true;
        }
        else
        {
            HotFriendPanel.GetComponentInParent<FollowParentHeight>().AddPading = false;
        }
        HotFriendPanel.GetComponentInParent<FollowParentHeight>().AddToHeightPaddingForSearchUI();
        if (AddFriendSerachBar.activeInHierarchy)
        {
            AddFreindContainer.GetComponent<VerticalLayoutGroup>().padding.top = 105;
        }
        else
        {
            AddFreindContainer.GetComponent<VerticalLayoutGroup>().padding.top = 50;
        }
        FeedUIController.Instance.findFriendInputFieldAdvanced.Text = "";
        FeedUIController.Instance.findFriendScreen.gameObject.SetActive(false);
    }
    public void OnClickProfileSearchBtn()
    {

        profileSerachBar.SetActive(!profileSerachBar.activeInHierarchy);
        if (profileSerachBar.activeInHierarchy)
        {
            profileSerachBarContainer.GetComponent<VerticalLayoutGroup>().padding.top = 105;
        }
        else
        {
            profileSerachBarContainer.GetComponent<VerticalLayoutGroup>().padding.top = 50;
        }
        profileFinfFriendAdvancedInputField.Text = "";
        profileFinfFriendScreen.gameObject.SetActive(false);
    }
    public void OnClickFeedSearchBtn()
    {
        FeedSerachBar.SetActive(!FeedSerachBar.activeInHierarchy);
    }
    public void ClearAllFeedDataAfterLogOut()
    {
        //SNS_APIManager.Instance.ClearAllFeedDataForLogout();

        SNS_APIController.Instance.feedFollowingIdList.Clear();
        SNS_APIController.Instance.feedForYouIdList.Clear();
        SNS_APIController.Instance.feedHotIdList.Clear();
        SNS_APIController.Instance.feedHotIdList.Clear();
        MyProfileDataManager.Instance.myProfileData = new GetUserDetailData();
        MyProfileDataManager.Instance.allMyFeedImageRootDataList.Clear();
        MyProfileDataManager.Instance.allMyFeedVideoRootDataList.Clear();
        if (OtherPlayerProfileData.Instance != null)
        {
            OtherPlayerProfileData.Instance.allMyFeedImageRootDataList.Clear();
            OtherPlayerProfileData.Instance.allMyFeedVideoRootDataList.Clear();
            OtherPlayerProfileData.Instance.backKeyManageList.Clear();
        }

        if (MyProfileDataManager.Instance.allPhotoContainer)
        {
            foreach (Transform item in MyProfileDataManager.Instance.allPhotoContainer)
            {
                Destroy(item.gameObject);
            }
        }

        if (MyProfileDataManager.Instance.allMovieContainer)
        {
            foreach (Transform item in MyProfileDataManager.Instance.allMovieContainer)
            {
                Destroy(item.gameObject);
            }
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

    public void OnBackToMainXanaBtnClick()
    {
        Initiate.Fade("Home", Color.black, 1.0f);
    }

    public void OnClickCheckOtherPlayerProfile(bool _callFromFindFriendWithName=false)
    {

        if (OtherPlayerProfileData.Instance.backKeyManageList.Count > 0)
        {
            switch (OtherPlayerProfileData.Instance.backKeyManageList[OtherPlayerProfileData.Instance.backKeyManageList.Count - 1])
            {
                case "FollowerFollowingListScreen":
                    //Commented in order to make profile 2.0 work after ahsan removed old feedui object from scene ----- UMER
                    profileFollowerFollowingListScreen.SetActive(false);
                    //footerCan.GetComponent<HomeFooterHandler>().SetDefaultButtonSelection(4);
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


  
    public void OnSetSelectionLine()
    {
        if (feedUiScreen.activeSelf)
        {
            isChangeMainScrollRect = true;
            StartCoroutine(WaitChangeScrollRectFasterOnMain());
        }
    }

    public IEnumerator SetContentOnFollowingItemScreen()
    {
        yield return new WaitForSeconds(0.01f);
        //followingFeedMainContainer.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        yield return new WaitForSeconds(0.05f);
        //followingFeedMainContainer.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    public bool isChangeMainScrollRect = false;
    IEnumerator WaitChangeScrollRectFasterOnMain()
    {
        yield return new WaitForSeconds(2f);
        isChangeMainScrollRect = false;
    }


    public float verticalNormalizedPosition;

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
        DateTime today = DateTime.SpecifyKind(timeUtc, DateTimeKind.Local);

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


    //this method is used to Find Friend Button click.......
    public void OnClickSearchUserButton()
    {
        findFriendInputFieldAdvanced.Text = "";
        SNS_APIManager.Instance.GetBestFriend();
    }

    /// <summary>
    /// On click following btn of ad friends screen
    /// </summary>
    public void OnClickAdFriendsFollowingBtn()
    {
        if (!AddFrndFollowingPanel.activeInHierarchy)
        {
            HotFriendPanel.SetActive(false);
            AddFrndRecommendedPanel.SetActive(false);
            AddFrndMutalFrndPanel.SetActive(false);
            AddFrndFollowingPanel.SetActive(true);
            findFriendInputFieldAdvanced.Text = "";
            findFriendScreen.SetActive(false);
            SNS_APIController.Instance.AdFrndFollowingFetch();
            UpdateAdFrndBtnStatus(2);
            HotFriendPanel.GetComponentInParent<FollowParentHeight>().SetChildHeight();
        }
    }

    #region find User references
    //this method is used to On find value inputfield value change.......
    public void OnValueChangeFindFriend()
    {
            //if (!string.IsNullOrEmpty(findFriendInputField.text))
            if (!string.IsNullOrEmpty(findFriendInputFieldAdvanced.Text))
            {
            if (!SearchFriendInput.Equals(findFriendInputFieldAdvanced.Text))
            {
                //SNS_APIManager.Instance.RequestGetSearchUser(findFriendInputField.text);
                SNS_APIManager.Instance.RequestGetSearchUser(findFriendInputFieldAdvanced.Text);
                if (!findFriendScreen.gameObject.activeInHierarchy)
                {
                    findFriendScreen.gameObject.SetActive(true);
                }
            }
            }
            else
            {
                //if user typed character clear then clear all search user list.
                if (findFriendScreen.gameObject.activeInHierarchy)
                {
                    findFriendInputFieldAdvanced.Text = "";
                    findFriendScreen.gameObject.SetActive(false);
                }
                foreach (Transform item in findFriendContainer)
                {
                    Destroy(item.gameObject);
                }
            }
            SearchFriendInput = findFriendInputFieldAdvanced.Text;
    }

    public void OnValueChangedProfileSearch()
    {
        //if (!string.IsNullOrEmpty(findFriendInputField.text))
        if (!string.IsNullOrEmpty(profileFinfFriendAdvancedInputField.Text))
        {
            if (!FollowFollowingSearchFriendInput.Equals(profileFinfFriendAdvancedInputField.Text))
            {
                //SNS_APIManager.Instance.RequestGetSearchUser(findFriendInputField.text);
                SNS_APIManager.Instance.RequestGetSearchUserForProfile(profileFinfFriendAdvancedInputField.Text);
                if (!profileFinfFriendScreen.gameObject.activeInHierarchy)
                {
                    profileFinfFriendScreen.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            //if user typed character clear then clear all search user list.
            if (profileFinfFriendScreen.gameObject.activeInHierarchy)
            {
                profileFinfFriendAdvancedInputField.Text = "";
                profileFinfFriendScreen.gameObject.SetActive(false);
            }
            foreach (Transform item in profileSerachResultsContainer)
            {
                Destroy(item.gameObject);
            }
        }
        FollowFollowingSearchFriendInput = profileFinfFriendAdvancedInputField.Text;
    }
    //this method is used to back button click find friend screen.......
    public void OnClickBackFindFriendButton()
    {
       // RemoveUnFollowedUserFromFollowingTab();

        //findFriendInputField.text = "";
        findFriendInputFieldAdvanced.Text = "";
        //findFriendScreen.SetActive(false);
        foreach (Transform item in findFriendContainer)
        {
            Destroy(item.gameObject);
        }
    }
    #endregion

    #region Create Feed

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

    #endregion

    #region Profile Follower and Following list Screen Methods
    int tempFollowFollowingScreenOpenCount = 0;
    //this method is used to profile follower button click.......
    public void ProfileFollowerFollowingScreenSetup(int Tabindex, string userName)
    {
        profileFollowerFollowingListScreen.SetActive(true);
        if (ProfileUIHandler.instance)
        {
            ProfileUIHandler.instance.gameObject.SetActive(false);
        }
        ProfileFFLineSelectionSetup(Tabindex);
        MyProfileDataManager.Instance.MyProfileSceenShow(false);
    }


    public void ProfileFFLineSelectionSetup(int index)
    {

        for (int i = 0; i < profileFFSelectionTabText.Length; i++)
        {
            if (i == index)
            {
                profileFFSelectionTabText[i].color = selectedColor;
                profileFFSelectionTabLine[i].gameObject.SetActive(true);
            }
            else
            {
                profileFFSelectionTabText[i].color = unSelectedColor;
                profileFFSelectionTabLine[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnClickProfileFollowerFollowingBackButton()
    {
        MyProfileDataManager.Instance.MyProfileSceenShow(true);
        profileFollowerFollowingListScreen.SetActive(false);
    }
    public void OnClickStartFollowingButton()
    {
        profileFollowerFollowingListScreen.SetActive(false);
        MyProfileDataManager.Instance.myProfileScreen.SetActive(false); 
        AddFriendPanel.SetActive(true);
        AddFriendSerachBar.SetActive(false);
        HotFriendPanel.GetComponentInParent<FollowParentHeight>().AddPading = false;
        HotFriendPanel.GetComponentInParent<FollowParentHeight>().HeightPadding = 190f;
        HotFriendPanel.GetComponentInParent<FollowParentHeight>().SetChildHeight();
        OnClickHotFrnd();
        SNS_APIManager.Instance.SetHotFriend();
    }
    public void ProfileGetAllFollower(int pageNum)
    {
        noProfileFollowers.SetActive(false);
        foreach (Transform item in profileFollowerListContainer.transform)
        {
            Destroy(item.gameObject);
        }
        if (SNS_APIManager.Instance.profileAllFollowerRoot.data.rows.Count > 0)
        {
            for (int i = 0; i <= SNS_APIManager.Instance.profileAllFollowerRoot.data.rows.Count; i++)
            {
                if (i < SNS_APIManager.Instance.profileAllFollowerRoot.data.rows.Count)
                {
                    if (!profileFollowerLoadedItemIDList.Contains(SNS_APIManager.Instance.profileAllFollowerRoot.data.rows[i].follower.id))
                    {
                        GameObject followerObject = Instantiate(followerPrefab, profileFollowerListContainer);
                        followerObject.GetComponent<FindFriendWithNameItem>().SetupData(SNS_APIManager.Instance.profileAllFollowerRoot.data.rows[i]);
                    }
                }
            }
            if (SNS_APIManager.Instance.profileAllFollowerRoot.data.rows.Count > 10)
            {
                GameObject extra = Instantiate(FeedUIController.Instance.ExtraPrefab, profileFollowerListContainer);
            }
        }
        else
        {
            noProfileFollowers.SetActive(true);
        }
    }

    public void ProfileGetAllFollowing(int pageNum)
    {
        //Debug.Log("ProfileGetAllFollowing:" + SNS_APIManager.Instance.profileAllFollowingRoot.data.rows.Count + "    :pageNum:" + pageNum);
        for (int i = 0; i < SNS_APIManager.Instance.profileAllFollowingRoot.data.rows.Count; i++)
        {
            if (!profileFollowingLoadedItemIDList.Contains(SNS_APIManager.Instance.profileAllFollowingRoot.data.rows[i].following.id))
            {
                GameObject followingObject = Instantiate(followingPrefab, profileFollowingListContainer);
                followingObject.GetComponent<FollowingItemController>().SetupData(SNS_APIManager.Instance.profileAllFollowingRoot.data.rows[i]);
                profileFollowingItemControllersList.Add(followingObject.GetComponent<FollowingItemController>());
                profileFollowingLoadedItemIDList.Add(SNS_APIManager.Instance.profileAllFollowingRoot.data.rows[i].following.id);
            }
        }

        if (waitToProfileFollowingDataLoadCo != null)
        {
            StopCoroutine(waitToProfileFollowingDataLoadCo);
        }
        waitToProfileFollowingDataLoadCo = StartCoroutine(WaitToProfileFollowingDataLoad(pageNum));
    }

    Coroutine waitToAdFrndFollowingDataLoadCo;
    public void AdFrndGetAllFollowing(int pageNum)
    {
        //Debug.Log("ProfileGetAllFollowing:" + SNS_APIManager.Instance.profileAllFollowingRoot.data.rows.Count + "    :pageNum:" + pageNum);
        for (int i = 0; i < SNS_APIManager.Instance.AdFrndFollowingRoot.data.rows.Count; i++)
        {
            if (!adFrndFollowingLoadedItemIDList.Contains(SNS_APIManager.Instance.AdFrndFollowingRoot.data.rows[i].following.id))
            {
                GameObject followingObject = Instantiate(followingPrefab, adFrndFollowingListContainer);
                followingObject.GetComponent<FollowingItemController>().SetupData(SNS_APIManager.Instance.AdFrndFollowingRoot.data.rows[i]);
                AdFrndFollowingItemControllersList.Add(followingObject.GetComponent<FollowingItemController>());
                adFrndFollowingLoadedItemIDList.Add(SNS_APIManager.Instance.AdFrndFollowingRoot.data.rows[i].following.id);
            }
        }

        if (waitToAdFrndFollowingDataLoadCo != null)
        {
            StopCoroutine(waitToAdFrndFollowingDataLoadCo);
        }
        waitToAdFrndFollowingDataLoadCo = StartCoroutine(WaitToAdFrndFollowingDataLoad(pageNum));
    }

    Coroutine waitToProfileFollowingDataLoadCo;
    IEnumerator WaitToProfileFollowingDataLoad(int pageNum)
    {
        yield return new WaitForSeconds(0.5f);
        isProfileFollowingDataLoaded = true;
        if (pageNum > 1 && SNS_APIManager.Instance.profileAllFollowingRoot.data.rows.Count > 0)
        {
            profileFollowingPaginationPageNo += 1;
        }
    }

    IEnumerator WaitToAdFrndFollowingDataLoad(int pageNum)
    {
        yield return new WaitForSeconds(0.5f);
        isAdFrndFollowingDataLoaded = true;
        if (pageNum > 1 && SNS_APIManager.Instance.AdFrndFollowingRoot.data.rows.Count > 0)
        {
            adFrndFollowingPaginationPageNo += 1;
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

    #endregion

    public void UpdateAdFrndBtnStatus(int index){
        foreach (TMP_Text text in FrndsPanelBtns)
        {
            text.color= unSelectedColor;
        }
        foreach (Image line in FrndsPanelBtnLines)
        {
            line.color =new Vector4(1,1,1,0);
            
        }

        FrndsPanelBtns[index].color= selectedColor;
        FrndsPanelBtnLines[index].color= selectedColor;
        FrndsPanelBtnLines[index].color = new Color(selectedColor.r,selectedColor.g,selectedColor.b,1);
    }

    public void OnClickHotFrnd()
    {
        if (!HotFriendPanel.activeInHierarchy)
        {
            HotFriendPanel.SetActive(true);
            AddFrndFollowingPanel.SetActive(false);
            AddFrndMutalFrndPanel.SetActive(false);
            AddFrndRecommendedPanel.SetActive(false);
            findFriendInputFieldAdvanced.Text = "";
            findFriendScreen.SetActive(false);
            Debug.LogError("On Click Hot Friend :- ");
            SNS_APIManager.Instance.SetHotFriend();
            UpdateAdFrndBtnStatus(0);
        }
    }
    public void OnClickHotFrndfromProfile()
    {
        if (!HotFriendPanel.activeInHierarchy)
        {
            SetAddFriendScreen(true);
            SNS_APIManager.Instance.SetHotFriend();
            findFriendInputFieldAdvanced.Text = "";
            findFriendScreen.gameObject.SetActive(false);
            OnClickHotFrnd();
            ResetAllFeedScreen(true);
        }
    }
    public void OnClickRecommedationFrnd(){
        if (!AddFrndRecommendedPanel.activeInHierarchy){ 
            HotFriendPanel.SetActive(false);
            AddFrndFollowingPanel.SetActive(false);
            AddFrndMutalFrndPanel.SetActive(false);
            AddFrndRecommendedPanel.SetActive(true);
            findFriendInputFieldAdvanced.Text = "";
            findFriendScreen.SetActive(false);
            SNS_APIManager.Instance.SetRecommendedFriend();
            UpdateAdFrndBtnStatus(1);
        }
     }
     
    public void OnClickMutalFrnd(){
         if (!AddFrndMutalFrndPanel.activeInHierarchy){ 
            HotFriendPanel.SetActive(false);
            AddFrndFollowingPanel.SetActive(false);
            AddFrndRecommendedPanel.SetActive(false);
            AddFrndMutalFrndPanel.SetActive(true);
            findFriendInputFieldAdvanced.Text = "";
            findFriendScreen.SetActive(false);
            SNS_APIManager.Instance.SetMutalFrndList();
            UpdateAdFrndBtnStatus(3);
        }
    }

    public void SetMainMenuFooter(){ 
        GameManager.Instance.UiManager._footerCan.GetComponent<CanvasGroup>().alpha=1;
        GameManager.Instance.UiManager._footerCan.GetComponent<CanvasGroup>().interactable=true;
        GameManager.Instance.UiManager._footerCan.GetComponent<CanvasGroup>().blocksRaycasts=true;    
    }

    public void OnClickProfileFollowerButton() 
    {
        profileFollowersPanel.SetActive(true);
        profileFollowingPanel.SetActive(false);
        profileFinfFriendScreen.SetActive(false);
        SNS_APIManager.Instance.RequestGetAllFollowersFromProfile(SNS_APIManager.Instance.userId.ToString(), 1, 50);
        ProfileFollowerFollowingScreenSetup(0, "");
    }
    public void OnClickProfileFollowingButton()
    {
        profileFollowingPanel.SetActive(true);
        profileFollowersPanel.SetActive(false);
        profileFinfFriendScreen.SetActive(false);
        SNS_APIManager.Instance.RequestGetAllFollowingFromProfile(SNS_APIManager.Instance.userId.ToString(), 1, 50);
        ProfileFollowerFollowingScreenSetup(1, "");
    }
    /// <summary>
    /// Check following count. if count is less than zero than show no following panel
    /// </summary>
    /// 
    public void CheckFollowingCount(){ 
        StartCoroutine(IEnumCheckFollowingCount());
    }
    IEnumerator IEnumCheckFollowingCount(){
        yield return new WaitForSeconds(1);
        foreach(Transform child in AddFrndFollowingContainer.transform)
        {
            if(child.gameObject.activeInHierarchy)
            {
               
              yield break;
            }
        }
        AddFrndNoFollowing.SetActive(true);
    }
}
#endregion
