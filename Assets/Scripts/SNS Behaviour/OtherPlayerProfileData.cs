using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SuperStar.Helpers;
using UnityEngine.UI;
using System.IO;
using UnityEngine.UI.Extensions;
using System.Linq;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;

public class OtherPlayerProfileData : MonoBehaviour
{
    public static OtherPlayerProfileData Instance;

    [SerializeField] SingleUserProfileData singleUserProfileData;
    [SerializeField] SingleUserProfileData visitedUserProfileAssetsData;

    public AllUserWithFeedRow FeedRawData;

    public List<AllFeedByUserIdRow> allMyFeedImageRootDataList = new List<AllFeedByUserIdRow>();//image feed list
    public List<FeedResponseRow> allMyTextPostRootDataList = new List<FeedResponseRow>();//image feed list
    //For Temp use needs to be deleted later 
    public List<AllFeedByUserIdRow> allMyFeedVideoRootDataList = new List<AllFeedByUserIdRow>();//video feed list

    [SerializeField] AllFeedByUserIdRoot currentPageAllFeedWithUserIdRoot = new AllFeedByUserIdRoot();
    [SerializeField] AllTextPostByUserIdRoot currentPageAllTextPostWithUserIdRoot = new AllTextPostByUserIdRoot();
    //Used temp need to to delete later
    [SerializeField] FeedResponse currentPageAllTextPostFeedWithUserIdRoot = new FeedResponse();


    public int lastUserId;
    [SerializeField] bool lastUserIsFollowFollowing;

    [SerializeField] bool isFollowFollowing = false;

    [Space]
    [Header("Screen Reference")]
    public GameObject myPlayerdataObj;

    [Space]
    [Header("Other Profile Screen Refresh Object")]
    [SerializeField] GameObject mainFullScreenContainer;
    [SerializeField] GameObject mainProfileDetailPart;
    [SerializeField] GameObject bioDetailPart;

    [Space]
    [Header("Refresh Object")]
    [SerializeField] GameObject userPostMainPart;
    [SerializeField] Transform userPostParent;
    [SerializeField] Transform allMovieContainer;
    [SerializeField] ParentHeightResetScript parentHeightResetScript;

    [Header("post empty message reference")]
    [SerializeField] GameObject emptyPhotoPostMsgObj;
    [SerializeField] GameObject emptyMoviePostMsgObj;

    [Space]
    [Header("Player info References")]
    [SerializeField] Image profileImage;
    [SerializeField] TextMeshProUGUI textPlayerName;
    [SerializeField] TextMeshProUGUI textPlayerTottleFollower;
    [SerializeField] TextMeshProUGUI textPlayerTottleFollowing;
    public TextMeshProUGUI textPlayerTottlePost;
    [SerializeField] TextMeshProUGUI userNameText;
    [SerializeField] TextMeshProUGUI textUserBio;
    public GameObject seeMoreBioButton;
    public GameObject seeMoreButtonTextObj;
    public GameObject seeLessButtonTextObj;

    [Space]
    [Header("Follow Message Button References")]
    public Image followButtonImage;
    public Sprite followSprite, followingSprite;
    public TextMeshProUGUI followText;
    public Color followtextColor, FollowingTextColor;

    public GameObject tagTabPrivateObject, tabTagPublicObject;

    public Sprite defultProfileImage;

    public bool isOtherPlayerProfileNew;

    [Space]
    [Header("For API Pagination")]
    public ScrollRectFasterEx profileMainScrollRectFasterEx;
    public bool isFeedLoaded = false;
    public int profileFeedAPiCurrentPageIndex = 1;

    [Space]
    [Header("Current Selected Search Friend Script Reference")]
    public FindFriendWithNameItem currentFindFriendWithNameItemScript;

    [Space]
    [Header("current selected follower item script")]
    public FollowerItemController currentFollowerItemScript;

    [Space]
    [Header("current selected following item script")]
    public FollowingItemController currentFollowingItemScript;

    [Space]
    [Header("For Ditrect Message check for back")]
    public bool isDirectMessageScreenOpen = false;
    bool isTempDirectMessageScreenOpen = false;

    public bool isProfiletranzistFromMessage = false;
    string tempBioOnly10LineStr = "";
    ProfileUIHandler profileUIHandler;
    public List<int> loadedMyPostAndVideoId = new List<int>();
    [SerializeField] FeedUIController  feedUIController;
    //HomeScoketHandler socketController;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
    }

    private void Start()
    {
        if (feedUIController == null)
        {
            feedUIController = FeedUIController.Instance;
        }
        //socketController = HomeScoketHandler.instance;
        profileUIHandler = ProfileUIHandler.instance;
        ClearDummyData();//clear dummy data.......
        RefreshUserData();
    }
    public void RefreshUserData()
    {
        //print("Visited User ID here " + singleUserProfileData.id);
        if (gameObject.activeInHierarchy && singleUserProfileData.id != 0)
        {
            Debug.Log("RefreshUserData");
            StartCoroutine(IERequestGetUserDetails(singleUserProfileData.id, false));
        }
    }

    private void OnDisable()
    {
        if (backKeyManageList.Count > 0 && !isTempDirectMessageScreenOpen)
        {
            //Debug.Log("OnDisable OtherPlayerProfileData feed screen enable");
            RemoveAndCheckBackKey();
            FeedUIController.Instance.feedUiScreen.SetActive(true);
        }
    }

    public void SocketOtherProfileUpdate(int id){
           StartCoroutine(IERequestGetUserDetails(id, false));  
    }

    //this method is used to clear the dummy data.......
    public void ClearDummyData()
    {
        textPlayerName.text = "";
        userNameText.text = "";
        userNameText.gameObject.SetActive(false);
        textUserBio.text = "";
    }

    public void LoadUserData(bool isFirstTime)
    {
        if (profileUIHandler)
        {
            profileUIHandler.followerBtn.interactable = false;
            profileUIHandler.followingBtn.interactable = false;
            profileUIHandler.editProfileBtn.SetActive(false);
            profileUIHandler.followProfileBtn.SetActive(true);
        }

        //Debug.Log("Other user profile load data");
        lastUserId = singleUserProfileData.id;

        lastUserIsFollowFollowing = singleUserProfileData.isFollowing;

        OnSetUserUi(singleUserProfileData.isFollowing);

        textPlayerName.text = singleUserProfileData.name;
        textPlayerTottleFollower.text = singleUserProfileData.followerCount.ToString();
        textPlayerTottleFollowing.text = singleUserProfileData.followingCount.ToString();

        UpdateUserTags();

        if (isFirstTime)
        {
            profileImage.sprite = defultProfileImage;

            //Debug.Log("user" + FeedRawData.UserProfile);
            if (singleUserProfileData.userProfile != null)
            {
                if (!string.IsNullOrEmpty(singleUserProfileData.userProfile.username))
                {
                    string _userName = SNS_APIManager.DecodedString(singleUserProfileData.userProfile.username);
                    if (!_userName.StartsWith("@"))
                    {
                        userNameText.text = "@" + _userName;
                    }
                    userNameText.gameObject.SetActive(true);
                }
                else
                {
                    userNameText.gameObject.SetActive(false);
                }

                if (!string.IsNullOrEmpty(singleUserProfileData.userProfile.bio))
                {
                    textUserBio.text = SNS_APIManager.DecodedString(singleUserProfileData.userProfile.bio);
                    SetupBioPart(textUserBio.text);//check and show only 10 line.......
                }
                else
                {
                    seeMoreBioButton.SetActive(false);
                    textUserBio.text = TextLocalization.GetLocaliseTextByKey("You have no bio yet.");
                }
                if (!string.IsNullOrEmpty(singleUserProfileData.userProfile.website))
                {
                    Debug.Log("Profile Website:" + singleUserProfileData.userProfile.website);
                    Uri uriResult;
                    bool result = Uri.TryCreate(singleUserProfileData.userProfile.website, UriKind.Absolute, out uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                    if (result)
                    {
                        Debug.Log("Given URL is valid");
                        Uri websiteHost = new Uri(singleUserProfileData.userProfile.website);
                    }
                    else
                    {
                        Debug.Log("Given URL is Invalid");
                    }
                }
            }
            else
            {
                seeMoreBioButton.SetActive(false);
                textUserBio.text = TextLocalization.GetLocaliseTextByKey("You have no bio yet.");
            }

            if (!string.IsNullOrEmpty(singleUserProfileData.avatar))//set avatar image.......
            {
                bool isAvatarUrlFromDropbox = SNS_APIManager.Instance.CheckUrlDropboxOrNot(singleUserProfileData.avatar);
                //Debug.Log("isAvatarUrlFromDropbox: " + isAvatarUrlFromDropbox + " :name:" + FeedsByFollowingUserRowData.User.Name);
                if (isAvatarUrlFromDropbox)
                {
                    AssetCache.Instance.EnqueueOneResAndWait(singleUserProfileData.avatar, singleUserProfileData.avatar, (success) =>
                    {
                        if (success)
                            AssetCache.Instance.LoadSpriteIntoImage(profileImage, singleUserProfileData.avatar, changeAspectRatio: true);
                    });
                }
                else
                {
                    GetImageFromAWS(singleUserProfileData.avatar, profileImage);//Get image from aws and save/load into asset cache.......
                }
            }
            else
            {
                profileImage.sprite = defultProfileImage;
            }


            if(gameObject.activeSelf)
                StartCoroutine(WaitToRefreshProfileScreen());
            ConstantsHolder.xanaConstants.SnsProfileID = singleUserProfileData.id;
            ConstantsHolder.xanaConstants.IsProfileVisit = true;
            ConstantsHolder.xanaConstants.IsOtherProfileVisit= true;
        }
    }

    public void UpdateUserTags()
    {
        if (singleUserProfileData.tags != null && singleUserProfileData.tags.Length > 0)
        {
            if (profileUIHandler)
            {
                profileUIHandler.UserTagsParent.transform.parent.gameObject.SetActive(true);
                if (profileUIHandler.UserTagsParent.transform.childCount > singleUserProfileData.tags.Length)
                {
                    for (int i = 0; i < profileUIHandler.UserTagsParent.transform.childCount; i++)
                    {
                        if (i >= singleUserProfileData.tags.Length)
                        {
                            Destroy(profileUIHandler.UserTagsParent.transform.GetChild(i).transform.gameObject);
                        }
                        else
                        {
                            profileUIHandler.UserTagsParent.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = singleUserProfileData.tags[i];
                        }
                    }
                }
                else if (profileUIHandler.UserTagsParent.transform.childCount < singleUserProfileData.tags.Length)
                {
                    if (profileUIHandler.UserTagsParent.transform.childCount == 0)
                    {
                        for (int i = 0; i < singleUserProfileData.tags.Length; i++)
                        {
                            GameObject _tagobject = Instantiate(profileUIHandler.TagPrefab, profileUIHandler.UserTagsParent.transform);
                            _tagobject.name = "TagPrefab" + i;
                            _tagobject.GetComponentInChildren<TextMeshProUGUI>().text = singleUserProfileData.tags[i];
                        }
                        profileUIHandler.UserTagsParent.GetComponent<HorizontalLayoutGroup>().spacing = 18.01f;
                    }
                    else
                    {
                        for (int i = 0; i < singleUserProfileData.tags.Length; i++)
                        {
                            if (i >= profileUIHandler.UserTagsParent.transform.childCount)
                            {
                                GameObject _tagobject = Instantiate(profileUIHandler.TagPrefab, profileUIHandler.UserTagsParent.transform);
                                _tagobject.name = "TagPrefab" + i;
                                _tagobject.GetComponentInChildren<TextMeshProUGUI>().text = singleUserProfileData.tags[i];
                            }
                            else
                            {
                                profileUIHandler.UserTagsParent.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = singleUserProfileData.tags[i];
                            }
                        }
                        profileUIHandler.UserTagsParent.GetComponent<HorizontalLayoutGroup>().spacing = 18.01f;
                    }
                }
            }
        }
        else
        {
            if (profileUIHandler)
            {
                profileUIHandler.UserTagsParent.transform.parent.gameObject.SetActive(false);
            }
        }
    }


    private IEnumerator SetVerticalFitMode(GameObject target, ContentSizeFitter.FitMode mode)
    {
        var fitter = target.GetComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        yield return new WaitForSeconds(0.01f);
        fitter.verticalFit = mode;
    }

    IEnumerator WaitToRefreshProfileScreen()
    {
        yield return StartCoroutine(SetVerticalFitMode(bioDetailPart, ContentSizeFitter.FitMode.PreferredSize));
        yield return StartCoroutine(SetVerticalFitMode(mainProfileDetailPart, ContentSizeFitter.FitMode.PreferredSize));
        //yield return StartCoroutine(SetVerticalFitMode(mainFullScreenContainer, ContentSizeFitter.FitMode.PreferredSize, 0.1f));
    }

    
    public void SetupBioPart(string bioText)
    {
        int numLines = bioText.Split('\n').Length;
        //Debug.Log("Bio Line Count:" + numLines);

        if (numLines > 10)
        {
            string[] bioLineSTR = bioText.Split('\n').Take(10).ToArray();
            //Debug.Log("Result:" + bioLineSTR);

            tempBioOnly10LineStr = "";
            for (int i = 0; i < bioLineSTR.Length; i++)
            {
                tempBioOnly10LineStr += bioLineSTR[i] + "\n";
            }
            textUserBio.text = tempBioOnly10LineStr;

            SeeMoreLessBioTextSetup(true);
            seeMoreBioButton.SetActive(true);
        }
        else
        {
            //false see more button
            seeMoreBioButton.SetActive(false);
        }
    }

    //this method is used to Bio SeeMore or Less button click.......
    public void OnClickBioSeeMoreLessButton()
    {
        if (seeMoreButtonTextObj.activeSelf)
        {
            textUserBio.text = SNS_APIManager.DecodedString(singleUserProfileData.userProfile.bio);
            SeeMoreLessBioTextSetup(false);
        }
        else
        {
            textUserBio.text = tempBioOnly10LineStr;
            SeeMoreLessBioTextSetup(true);
        }
        ResetMainScrollDefaultTopPos();
        StartCoroutine(WaitToRefreshProfileScreen());
    }

    void SeeMoreLessBioTextSetup(bool isSeeMore)
    {
        seeMoreButtonTextObj.SetActive(isSeeMore);
        seeLessButtonTextObj.SetActive(!isSeeMore);
    }

    //this method is used to reset to main scroll default position to top.......
    public void ResetMainScrollDefaultTopPos()
    {
        profileMainScrollRectFasterEx.verticalNormalizedPosition = 1;
    }

    //this method is used to check clickes user profile and reset Feed data.......
    public void CheckAndResetFeedClickOnUserProfile()
    {
        if (lastUserId != singleUserProfileData.id)
        {
            //Debug.Log("New User");
            foreach (Transform item in userPostParent)
            {
                Destroy(item.gameObject);
            }
            foreach (Transform item in allMovieContainer)
            {
                Destroy(item.gameObject);
            }
            loadedMyPostAndVideoId.Clear();
            allMyFeedImageRootDataList.Clear();
            allMyFeedVideoRootDataList.Clear();
            profileFeedAPiCurrentPageIndex = 1;
            isFeedLoaded = false;
        }
    }

    //this method is used to set pagination for other user profile.......
    public void ProfileAPiPagination()
    {
        if (profileMainScrollRectFasterEx.verticalNormalizedPosition < 0.01f && isFeedLoaded)
        {
            //Debug.Log("scrollRect pos :" + profileMainScrollRectFasterEx.verticalNormalizedPosition + " rows count:" + allFeedWithUserIdRoot.Data.Rows.Count + "   :pageIndex:" + (profileFeedAPiCurrentPageIndex+1));
            if (currentPageAllFeedWithUserIdRoot.Data.Rows.Count > 0)
            {
                //Debug.Log("isDataLoad False");
                isFeedLoaded = false;
                SNS_APIManager.Instance.RequestGetFeedsByUserId(singleUserProfileData.id, (profileFeedAPiCurrentPageIndex + 1), 10, "OtherPlayerFeed");
            }
        }
    }

  
    public void AllFeedWithUserId(int pageNumb, bool _callFromFindFriendWithName = false)
    {
        mainFullScreenContainer.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        currentPageAllTextPostWithUserIdRoot = SNS_APIManager.Instance.allTextPostWithUserIdRoot;
        FeedUIController.Instance.OnClickCheckOtherPlayerProfile(_callFromFindFriendWithName);

        for (int i = 0; i <= currentPageAllTextPostWithUserIdRoot.data.rows.Count; i++)
        {
            if (i < currentPageAllTextPostWithUserIdRoot.data.rows.Count)
            {
                if (!loadedMyPostAndVideoId.Contains(currentPageAllTextPostWithUserIdRoot.data.rows[i].id))
                {
                    bool isVideo = false;

                    Transform parent = userPostParent;
                    if (!string.IsNullOrEmpty(currentPageAllTextPostWithUserIdRoot.data.rows[i].text_post))
                    {
                        parent = userPostParent;
                    }
                    GameObject userTagPostObject = Instantiate(MyProfileDataManager.Instance.photoPrefab, parent);
                    userTagPostObject.AddComponent<LayoutElement>();
                    FeedData userPostItem = userTagPostObject.GetComponent<FeedData>();
                    userPostItem.SetFeedPrefab(currentPageAllTextPostWithUserIdRoot.data.rows[i], false);
                    userPostItem.isProfileScene = true;
                    loadedMyPostAndVideoId.Add(currentPageAllTextPostWithUserIdRoot.data.rows[i].id);
                    allMyTextPostRootDataList.Add(currentPageAllTextPostWithUserIdRoot.data.rows[i]);
                }
            }
            else//Case added to instantiate empty object at end of posts so last one wont get hidden behide bottom UI
            {
                StartCoroutine(WaitToFeedLoadedUpdate(pageNumb));
            }
    }
        GlobalVeriableClass.callingScreen = "";
    }

    IEnumerator WaitToFeedLoadedUpdate(int pageNum)
    {
        yield return new WaitForSeconds(0.4f);
        userPostMainPart.GetComponent<ParentHeightResetScript>().SetParentheight(userPostParent.GetComponent<RectTransform>().sizeDelta);
        SetupEmptyMsgForPhotoTab(false);//check for empty message.......
        yield return new WaitForSeconds(0.1f);
        FeedUIController.Instance.ShowLoader(false);

        yield return new WaitForSeconds(0.2f);

        isFeedLoaded = true;
        if (pageNum > 1 && currentPageAllTextPostFeedWithUserIdRoot.data.rows.Count > 0)
        {
            profileFeedAPiCurrentPageIndex += 1;
        }
        //parentHeightResetScript.OnHeightReset(0);
        //Debug.Log("other Profile AllFeedWithUserId:" + isFeedLoaded);
    }

    public void OnClickPhotoButton()
    {
        profileUIHandler.otherUserButtonPanelScriptRef.OnSelectedClick(0);
        parentHeightResetScript.OnHeightReset(0);
    }
    public void OnClickMovieButton()
    {
        profileUIHandler.otherUserButtonPanelScriptRef.OnSelectedClick(1);
        parentHeightResetScript.OnHeightReset(1);
    }

    //this method is used to check and setup ui for Empty photo tab message.......
    public void SetupEmptyMsgForPhotoTab(bool isReset)
    {
        //check for photo.......
        if (userPostParent.childCount > 0 || isReset)
        {
            userPostParent.gameObject.SetActive(true);
            emptyPhotoPostMsgObj.SetActive(false);
        }
        else
        {
            userPostParent.gameObject.SetActive(false);
            emptyPhotoPostMsgObj.SetActive(true);
        }

        //check for movie.......
        if (allMovieContainer.childCount > 0 || isReset)
        {
            allMovieContainer.gameObject.SetActive(true);
            emptyMoviePostMsgObj.SetActive(false);
        }
        else
        {
            allMovieContainer.gameObject.SetActive(false);
            emptyMoviePostMsgObj.SetActive(true);
        }
    }

    public void OnSetUserUi(bool isFollow)
    {
        isFollowFollowing = isFollow;//set user follow or unfollow.......

        if (isFollow)
        {
            followText.text = TextLocalization.GetLocaliseTextByKey("UnFollow");
            if (!isOtherPlayerProfileNew)
            {
                tagTabPrivateObject.SetActive(false);
                tabTagPublicObject.SetActive(true);
            }
        }
        else
        {
            followText.text = TextLocalization.GetLocaliseTextByKey("Follow");
            if (!isOtherPlayerProfileNew)
            {
                tagTabPrivateObject.SetActive(true);
                tabTagPublicObject.SetActive(false);
            }
        }

        if (currentFindFriendWithNameItemScript != null)
        {
            currentFindFriendWithNameItemScript.searchUserRow.is_following_me = isFollow;
            currentFindFriendWithNameItemScript.FollowFollowingSetUp(isFollow);
        }
    }


    //this is used to follower increase or decrease after hit follower api.......
    public void OnFollowerIncreaseOrDecrease(bool isIscrease)
    {
        if (isIscrease)
        {
            singleUserProfileData.followerCount += 1;//temp after follow increse follower.......
        }
        else
        {
            singleUserProfileData.followerCount -= 1;//temp after follow increse follower.......
        }
        textPlayerTottleFollower.text = singleUserProfileData.followerCount.ToString();

        OnSetUserUi(isIscrease);//follower following setup.......
    }

    public void OnClickOtherPalyerProfileBackButton()
    {
       Debug.Log("Other Player Profile Back Button Click");
        if (currentFindFriendWithNameItemScript != null)
        {
            currentFindFriendWithNameItemScript = null;
        }

        if (backKeyManageList.Count > 0)
        {
           Debug.Log("lastUserIsFollowFollowing:" + lastUserIsFollowFollowing + "  :isFollowFollowing:" + isFollowFollowing);

            switch (backKeyManageList[backKeyManageList.Count - 1])
            {
                case "FollowerFollowingListScreen":
                   Debug.Log("Last Comes from Follower Following list screen my profile");
                    MyProfileDataManager.Instance.myProfileScreen.SetActive(true);
                    FeedUIController.Instance.profileFollowerFollowingListScreen.SetActive(true);
                    //FeedUIController.Instance.footerCan.GetComponent<HomeFooterHandler>().SetDefaultButtonSelection(4);
                    RefreshDataFollowerAndFollowingScreen();
                    break;
                case "HotTabScreen":
                   Debug.Log("Last Comes from Hot or discover tab Screen");
                    //FeedUIController.Instance.RemoveUnFollowedUserFromFollowingTab();
                    FeedUIController.Instance.feedUiScreen.SetActive(true);
                    break;
                case "FollowingTabScreen":
                   Debug.Log("Last Comes from Following tab screen");
                    //FeedUIController.Instance.RemoveUnFollowedUserFromFollowingTab("FollowingTabScreen");
                    FeedUIController.Instance.feedUiScreen.SetActive(true);
                    break;
                case "GroupDetailsScreen":
                   Debug.Log("Last Comes from message module group details screen");
                    isProfiletranzistFromMessage = true;
                    RemoveAndCheckBackKey();
                    FeedUIController.Instance.footerCan.GetComponent<HomeFooterHandler>().OnClickWorldButton();
                    break;
                default:
                    FeedUIController.Instance.feedUiScreen.SetActive(true);
                    break;
            }
        }
        else
        {
            FeedUIController.Instance.feedUiScreen.SetActive(true);
        }

        RemoveAndCheckBackKey();
        SetupEmptyMsgForPhotoTab(true);//check for empty message.......
    }

    void RefreshDataFollowerAndFollowingScreen()
    {
        if (currentFollowerItemScript != null)
        {
            if (currentFollowerItemScript.followerRawData.isFollowing != isFollowFollowing)
            {
                currentFollowerItemScript.followerRawData.isFollowing = isFollowFollowing;
                currentFollowerItemScript.followerRawData.followerCount = singleUserProfileData.followerCount;
                currentFollowerItemScript.followerRawData.followingCount = singleUserProfileData.followingCount;
                currentFollowerItemScript.followerRawData.feedCount = singleUserProfileData.feedCount;

                int userId = currentFollowerItemScript.followerRawData.follower.id;
                FollowingItemController followingItemController = FeedUIController.Instance.profileFollowingItemControllersList.Find((x) => x.followingRawData.following.id == userId);
                if (followingItemController != null)
                {
                    followingItemController.followingRawData.isFollowing = isFollowFollowing;
                    followingItemController.followingRawData.followerCount = singleUserProfileData.followerCount;
                    followingItemController.followingRawData.followingCount = singleUserProfileData.followingCount;
                    followingItemController.followingRawData.feedCount = singleUserProfileData.feedCount;
                }
                MyProfileDataManager.Instance.RequestGetUserDetails();
            }
            currentFollowerItemScript = null;
        }
        else if (currentFollowingItemScript != null)
        {
            if (currentFollowingItemScript.followingRawData.isFollowing != isFollowFollowing)
            {
                currentFollowingItemScript.followingRawData.isFollowing = isFollowFollowing;
                currentFollowingItemScript.followingRawData.followerCount = singleUserProfileData.followerCount;
                currentFollowingItemScript.followingRawData.followingCount = singleUserProfileData.followingCount;
                currentFollowingItemScript.followingRawData.feedCount = singleUserProfileData.feedCount;

                int userId = currentFollowingItemScript.followingRawData.following.id;
                FollowerItemController followerItemController = FeedUIController.Instance.profileFollowerItemControllersList.Find((x) => x.followerRawData.follower.id == userId);
                if (followerItemController != null)
                {
                    followerItemController.followerRawData.isFollowing = isFollowFollowing;
                    followerItemController.followerRawData.followerCount = singleUserProfileData.followerCount;
                    followerItemController.followerRawData.followingCount = singleUserProfileData.followingCount;
                    followerItemController.followerRawData.feedCount = singleUserProfileData.feedCount;
                }
                MyProfileDataManager.Instance.RequestGetUserDetails();
            }
            currentFollowingItemScript = null;
        }
    }

    public void OnClickFollowUserButton()
    {
       Debug.Log("OnClickFollowUser id" + singleUserProfileData.id.ToString() + " :FollowText:" + followText.text);
        if (isFollowFollowing)
        {
            //unfollow.......
            FeedUIController.Instance.ConfirmUnfollowPanel.SetActive(true);
            FeedUIController.Instance.UnfollowButton.onClick.RemoveAllListeners(); // To Avoid multiple function calls
            FeedUIController.Instance.UnfollowButton.onClick.AddListener(UnFollowAUser);
        }
        else
        {
            //follow.......
            //feedUIController.ShowLoader(true);
            //ProfileUIHandler.instance.followProfileBtn.GetComponent<Button>().interactable = false;
            SNS_APIManager.Instance.RequestFollowAUser(singleUserProfileData.id.ToString(), "OtherUserProfile");
            //ProfileUIHandler.instance.followProfileBtn.GetComponentInChildren<TextLocalization>().LocalizeTextText("Unfollow");
        }
    }
    public void UnFollowAUser()
    {
        //feedUIController.ShowLoader(true);
        //ProfileUIHandler.instance.followProfileBtn.GetComponent<Button>().interactable = false;
        SNS_APIManager.Instance.RequestUnFollowAUser(singleUserProfileData.id.ToString(), "OtherUserProfile");
        //ProfileUIHandler.instance.followProfileBtn.GetComponentInChildren<TextLocalization>().LocalizeTextText("Follow");
    }

    #region Get User Details API Integrate........
    public void RequestGetUserDetails(SingleUserProfileData singleUserProfileData1, bool _callFromFindFriendWithName = false)
    {
        singleUserProfileData = singleUserProfileData1;
        visitedUserProfileAssetsData = singleUserProfileData1;
        CheckAndResetFeedClickOnUserProfile();//check for user and if new user then clear old data.......
        LoadUserData(true);
        //Debug.Log("RequestGetUserDetails:" + singleUserProfileData1.id);
        StartCoroutine(IERequestGetUserDetails(singleUserProfileData1.id));
        SNS_APIManager.Instance.RequestGetFeedsByUserId(singleUserProfileData1.id, 1, 40, "OtherPlayerFeed", _callFromFindFriendWithName);
    }

    public void RequestGetUserDetails(int id)
    {
        singleUserProfileData.id = id;
        CheckAndResetFeedClickOnUserProfile();//check for user and if new user then clear old data.......
        LoadUserData(true);
        //Debug.Log("RequestGetUserDetails:" + singleUserProfileData1.id);
        StartCoroutine(IERequestGetUserDetails(id));
        SNS_APIManager.Instance.RequestGetFeedsByUserId(id, 1, 40, "OtherPlayerFeed");
    }

    public bool isUserProfileDataDiff = false;
    public IEnumerator IERequestGetUserDetails(int userId, bool IsFirstTime = true)
    {
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        isUserProfileDataDiff = false;
        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetSingleUserProfile), form))
        {
            www.SetRequestHeader("Authorization", SNS_APIManager.Instance.userAuthorizeToken);
            // Start the stopwatch
            //Stopwatch stopwatch = Stopwatch.StartNew();
            www.SendWebRequest();
            // Stop the stopwatch
            //stopwatch.Stop();
            while (!www.isDone)
            {
                yield return null;
            }

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //// Print the elapsed time
                string data = www.downloadHandler.text;
                Debug.Log("IERequestGetSingleUserDetails data:" + data);
                SingleUserProfileRoot singleUserProfileRoot = JsonConvert.DeserializeObject<SingleUserProfileRoot>(data);
                if (singleUserProfileRoot != null)
                {
                    if (singleUserProfileRoot.data != null)
                    {
                        if (singleUserProfileRoot.data.userProfile != null)
                        {
                            if (singleUserProfileData != null)
                            {
                                if (singleUserProfileData.userProfile != null)
                                {
                                    if (singleUserProfileData.userProfile.userId != singleUserProfileRoot.data.userProfile.userId)
                                    {
                                        isUserProfileDataDiff = true;
                                    }
                                }
                            }
                        }
                    }
                    singleUserProfileData = singleUserProfileRoot.data;
                }
            }
        }
        if (true)
        {
            //Riken
            LoadUserData(true);
          
            //if (socketController == null)
            //{
            //    socketController = HomeScoketHandler.instance;
            //}
            //socketController.ConnectSNSSockets(userId);

        }
    }

    #endregion

    #region Get Image From AWS
    public void GetImageFromAWS(string key, Image mainImage)
    {
        //Debug.Log("GetImageFromAWS key:" + key);
        if (AssetCache.Instance.HasFile(key))
        {
            //Debug.Log("Chat Image Available on Disk");
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
                    //Debug.Log("Save and Image download success");
                }
            });
        }
    }

    #endregion

    #region Back Key Manages and Reference
    public List<string> backKeyManageList = new List<string>();
    public void RemoveAndCheckBackKey()
    {
        if (backKeyManageList.Count > 0)
        {
            backKeyManageList.RemoveAt(backKeyManageList.Count - 1);
        }
    }
    #endregion
}