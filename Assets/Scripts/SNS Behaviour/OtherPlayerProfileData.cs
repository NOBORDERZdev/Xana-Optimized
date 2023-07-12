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

    public SingleUserProfileData singleUserProfileData;

    public AllUserWithFeedRow FeedRawData;

    public List<AllFeedByUserIdRow> allMyFeedImageRootDataList = new List<AllFeedByUserIdRow>();//image feed list
    public List<AllFeedByUserIdRow> allMyFeedVideoRootDataList = new List<AllFeedByUserIdRow>();//video feed list

    public AllFeedByUserIdRoot currentPageAllFeedWithUserIdRoot = new AllFeedByUserIdRoot();

    public int lastUserId;
    public bool lastUserIsFollowFollowing;

    public bool isFollowFollowing = false;

    [Space]
    [Header("Screen Reference")]
    public GameObject otherUserSettingScreen;

    [Space]
    [Header("Other Profile Screen Refresh Object")]
    public GameObject mainFullScreenContainer;
    public GameObject mainProfileDetailPart;
    public GameObject bioDetailPart;

    [Space]
    [Header("Refresh Object")]
    public Transform mainPostContainer;
    public GameObject userPostMainPart;
    public GameObject userPostPrefab;
    public Transform userPostParent;
    public Transform userTagPostParent;
    public Transform allMovieContainer;
    public ScrollRectGiftScreen tabScrollRectGiftScreen;
    public ParentHeightResetScript parentHeightResetScript;

    [Header("post empty message reference")]
    public GameObject emptyPhotoPostMsgObj;
    public GameObject emptyMoviePostMsgObj;

    [Space]
    [Header("Player info References")]
    public Image profileImage;
    public TextMeshProUGUI textPlayerName;
    public TextMeshProUGUI textPlayerTottleFollower;
    public TextMeshProUGUI textPlayerTottleFollowing;
    public TextMeshProUGUI textPlayerTottlePost;
    public TextMeshProUGUI jobText;
    public TextMeshProUGUI textUserBio;
    public TextMeshProUGUI WebsiteText;
    public TextMeshProUGUI textHeaderUserName;

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

    [Space]
    [Header("Premium UserRole Referense")]
    public UserRolesView userRolesView;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        ClearDummyData();//clear dummy data.......
        InvokeRepeating(nameof(RefreshUserData), 0, 2);
    }
    public void RefreshUserData()
    {
        if (textPlayerTottleFollower.gameObject.activeInHierarchy)
        {
            StartCoroutine(IERequestGetUserDetails(singleUserProfileData.id, false));
        }
    }
    private void OnDisable()
    {
        //Debug.LogError("isTempDirectMessageScreenOpen:" + isTempDirectMessageScreenOpen);
        if (backKeyManageList.Count > 0 && !isTempDirectMessageScreenOpen)
        {
            /*switch (backKeyManageList[backKeyManageList.Count - 1])
            {
                case "GroupDetailsScreen":
                    Debug.LogError("OnDisable OtherPlayerProfileData other user profile disable and clear data");
                    break;
                default:
                    break;
            }*/
            //Debug.LogError("OnDisable OtherPlayerProfileData feed screen enable");
            RemoveAndCheckBackKey();
            FeedUIController.Instance.otherPlayerProfileScreen.SetActive(false);
            FeedUIController.Instance.feedUiScreen.SetActive(true);
        }
    }

    private void Update()
    {
        if (FeedUIController.Instance.otherPlayerProfileScreen.activeSelf)
        {
            ProfileAPiPagination();
        }
    }

    //this method is used to clear the dummy data.......
    public void ClearDummyData()
    {
        textPlayerName.text = "";
        textHeaderUserName.text = "";
        jobText.text = "";
        WebsiteText.text = "";
        jobText.gameObject.SetActive(false);
        textUserBio.text = "";
        //websiteText.text = "";
        //websiteText.gameObject.SetActive(false);
    }

    //this method is used to Other user Menu button click.......
    public void OnClickMenuButton()
    {
        otherUserSettingScreen.SetActive(true);
    }

    public void LoadUserData(bool isFirstTime)
    {
        Debug.LogError("Other user profile load data");
        lastUserId = singleUserProfileData.id;

        lastUserIsFollowFollowing = singleUserProfileData.isFollowing;

        OnSetUserUi(singleUserProfileData.isFollowing);

        textPlayerName.text = singleUserProfileData.name;
        textHeaderUserName.text = singleUserProfileData.name;
        textPlayerTottleFollower.text = singleUserProfileData.followerCount.ToString();
        textPlayerTottleFollowing.text = singleUserProfileData.followingCount.ToString();
        textPlayerTottlePost.text = singleUserProfileData.feedCount.ToString();

        if (isFirstTime)
        {
            profileImage.sprite = defultProfileImage;

            //Debug.LogError("user" + FeedRawData.UserProfile);
            if (singleUserProfileData.userProfile != null)
            {
                if (!string.IsNullOrEmpty(singleUserProfileData.userProfile.job))
                {
                    jobText.text = APIManager.DecodedString(singleUserProfileData.userProfile.job);
                    jobText.gameObject.SetActive(true);
                }
                else
                {
                    jobText.gameObject.SetActive(false);
                }

                if (!string.IsNullOrEmpty(singleUserProfileData.userProfile.bio))
                {
                    textUserBio.text = APIManager.DecodedString(singleUserProfileData.userProfile.bio);
                    SetupBioPart(textUserBio.text);//check and show only 10 line.......
                }
                else
                {
                    //textUserBio.text = "You have no bio yet.";
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
                        WebsiteText.text = websiteHost.Host.ToString();
                    }
                    else
                    {
                        Debug.Log("Given URL is Invalid");
                        WebsiteText.text = singleUserProfileData.userProfile.website.ToString();
                    }
                    WebsiteText.gameObject.SetActive(true);
                }
                else
                {
                    WebsiteText.gameObject.SetActive(false);
                }
            }
            else
            {
                //textUserBio.text = "You have no bio yet.";
                seeMoreBioButton.SetActive(false);
                textUserBio.text = TextLocalization.GetLocaliseTextByKey("You have no bio yet.");
            }

            if (!string.IsNullOrEmpty(singleUserProfileData.avatar))//set avatar image.......
            {
                bool isAvatarUrlFromDropbox = APIManager.Instance.CheckUrlDropboxOrNot(singleUserProfileData.avatar);
                //Debug.LogError("isAvatarUrlFromDropbox: " + isAvatarUrlFromDropbox + " :name:" + FeedsByFollowingUserRowData.User.Name);
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
            StartCoroutine(WaitToRefreshProfileScreen());
        }
    }

    public void OnClickWebsiteButtonClick()
    {
        string websiteUrl = "";
        websiteUrl = singleUserProfileData.userProfile.website;
        //Debug.LogError("WebsiteURL:" + websiteUrl);
        Uri uriResult;
        bool result = Uri.TryCreate(singleUserProfileData.userProfile.website, UriKind.Absolute, out uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        if (result)
        {
            Debug.Log("Given URL is valid");
            websiteUrl = singleUserProfileData.userProfile.website;
        }
        else
        {
            Debug.Log("Given URL is Invalid");
            websiteUrl = "https://" + singleUserProfileData.userProfile.website;
        }
        Application.OpenURL(websiteUrl);
    }

    IEnumerator WaitToRefreshProfileScreen()
    {
        bioDetailPart.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        yield return new WaitForSeconds(0.01f);

        bioDetailPart.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        mainProfileDetailPart.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        yield return new WaitForSeconds(0.01f);
        mainProfileDetailPart.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        mainFullScreenContainer.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        yield return new WaitForSeconds(0.01f);
        mainFullScreenContainer.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    string tempBioOnly10LineStr = "";
    public void SetupBioPart(string bioText)
    {
        int numLines = bioText.Split('\n').Length;
        //Debug.LogError("Bio Line Count:" + numLines);

        if (numLines > 10)
        {
            string[] bioLineSTR = bioText.Split('\n').Take(10).ToArray();
            //Debug.LogError("Result:" + bioLineSTR);

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
            textUserBio.text = APIManager.DecodedString(singleUserProfileData.userProfile.bio);
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
            //Debug.LogError("New User");
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

            userRolesView.ResetBadges();//clear data and reset user roles.......
        }
    }

    //this method is used to set pagination for other user profile.......
    public void ProfileAPiPagination()
    {
        //Debug.LogError("Profile y pos:" + profileMainScrollRectFasterEx.verticalEndPos + "  :verticalnormalize pos:"+ profileMainScrollRectFasterEx.verticalNormalizedPosition + "  :normalize:"+profileMainScrollRectFasterEx.normalizedPosition + "   :isLoaded:"+ isFeedLoaded);
        if (profileMainScrollRectFasterEx.verticalNormalizedPosition < 0.01f && isFeedLoaded)
        {
            //Debug.LogError("scrollRect pos :" + profileMainScrollRectFasterEx.verticalNormalizedPosition + " rows count:" + allFeedWithUserIdRoot.Data.Rows.Count + "   :pageIndex:" + (profileFeedAPiCurrentPageIndex+1));
            //lastVerticalNormalizedPosition = profileMainScrollRectFasterEx.verticalNormalizedPosition;
            if (currentPageAllFeedWithUserIdRoot.Data.Rows.Count > 0)
            {
                //Debug.LogError("isDataLoad False");
                isFeedLoaded = false;
                APIManager.Instance.RequestGetFeedsByUserId(singleUserProfileData.id, (profileFeedAPiCurrentPageIndex + 1), 10, "OtherPlayerFeed");
            }
        }
    }

    public List<int> loadedMyPostAndVideoId = new List<int>();
    public void AllFeedWithUserId(int pageNumb)
    {
        /*foreach (Transform item in userPostParent)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in allMovieContainer)
        {
            Destroy(item.gameObject);
        }*/
        currentPageAllFeedWithUserIdRoot = APIManager.Instance.allFeedWithUserIdRoot;

        //FeedUIController.Instance.ShowLoader(false);

        FeedUIController.Instance.OnClickCheckOtherPlayerProfile();

        for (int i = 0; i < currentPageAllFeedWithUserIdRoot.Data.Rows.Count; i++)
        {
            if (!loadedMyPostAndVideoId.Contains(currentPageAllFeedWithUserIdRoot.Data.Rows[i].Id))
            {
                bool isVideo = false;

                Transform parent = userPostParent;
                if (!string.IsNullOrEmpty(currentPageAllFeedWithUserIdRoot.Data.Rows[i].Image))
                {
                    parent = userPostParent;
                }
                else if (!string.IsNullOrEmpty(currentPageAllFeedWithUserIdRoot.Data.Rows[i].Video))
                {
                    isVideo = true;
                    parent = allMovieContainer;
                }

                //GameObject userTagPostObject = Instantiate(userPostPrefab, userPostParent);
                GameObject userTagPostObject = Instantiate(userPostPrefab, parent);
                UserPostItem userPostItem = userTagPostObject.GetComponent<UserPostItem>();
                userPostItem.userData = currentPageAllFeedWithUserIdRoot.Data.Rows[i];

                FeedsByFollowingUser feedUserData = new FeedsByFollowingUser();
                feedUserData.Id = singleUserProfileData.id;
                feedUserData.Name = singleUserProfileData.name;
                feedUserData.Email = singleUserProfileData.email;
                feedUserData.Avatar = singleUserProfileData.avatar;
                userPostItem.feedUserData = feedUserData;

                userPostItem.avtarUrl = singleUserProfileData.avatar;
                userPostItem.LoadFeed();

                loadedMyPostAndVideoId.Add(currentPageAllFeedWithUserIdRoot.Data.Rows[i].Id);

                if (!isVideo)//image
                {
                    allMyFeedImageRootDataList.Add(currentPageAllFeedWithUserIdRoot.Data.Rows[i]);
                }
                else
                {
                    allMyFeedVideoRootDataList.Add(currentPageAllFeedWithUserIdRoot.Data.Rows[i]);
                }
            }
        }

        StartCoroutine(WaitToFeedLoadedUpdate(pageNumb));
    }

    IEnumerator WaitToFeedLoadedUpdate(int pageNum)
    {
        yield return new WaitForSeconds(0.1f);
        //userPostMainPart.GetComponent<ParentHeightResetScript>().GetAndCheckMaxHeightInAllTab();

        SetupEmptyMsgForPhotoTab(false);//check for empty message.......

        yield return new WaitForSeconds(0.2f);

        FeedUIController.Instance.ShowLoader(false);

        yield return new WaitForSeconds(0.8f);

        isFeedLoaded = true;
        if (pageNum > 1 && currentPageAllFeedWithUserIdRoot.Data.Rows.Count > 0)
        {
            profileFeedAPiCurrentPageIndex += 1;
        }
        //Debug.LogError("other Profile AllFeedWithUserId:" + isFeedLoaded);
    }

    public void OnClickPhotoButton()
    {
        tabScrollRectGiftScreen.LerpToPage(0);
        //tabScrollRectGiftScreen.SetPage(0);
        parentHeightResetScript.OnHeightReset(0);
    }
    public void OnClickMovieButton()
    {
        tabScrollRectGiftScreen.LerpToPage(1);
        //tabScrollRectGiftScreen.SetPage(1);
        parentHeightResetScript.OnHeightReset(1);
    }
    public IEnumerator AllTagFeed()
    {
        foreach (Transform item in userTagPostParent)
        {
            Destroy(item.gameObject);
        }
        yield return new WaitForSeconds(0.5f);
        //FeedUIController.Instance.ApiLoaderScreen.SetActive(false);
        //FeedUIController.Instance.OnClickCheckOtherPlayerProfile();
        //Debug.LogError("qqqq");
        for (int i = 0; i < APIManager.Instance.taggedFeedsByUserIdRoot.data.rows.Count; i++)
        {
            GameObject userPostObject = Instantiate(userPostPrefab, userTagPostParent);
            //Debug.LogError("tagdata" + APIManager.Instance.taggedFeedsByUserIdRoot.data.rows[i]);
            UserPostItem userPostItem = userPostObject.GetComponent<UserPostItem>();
            userPostItem.tagUserData = APIManager.Instance.taggedFeedsByUserIdRoot.data.rows[i];

            FeedsByFollowingUser feedUserData = new FeedsByFollowingUser();
            feedUserData.Id = singleUserProfileData.id;
            feedUserData.Name = singleUserProfileData.name;
            feedUserData.Email = singleUserProfileData.email;
            feedUserData.Avatar = singleUserProfileData.avatar;
            userPostItem.feedUserData = feedUserData;

            userPostItem.avtarUrl = singleUserProfileData.avatar;
            userPostItem.LoadFeed();
        }
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
            //followText.text = "Following";
            followText.text = TextLocalization.GetLocaliseTextByKey("Following");
            followButtonImage.sprite = followingSprite;
            followText.color = FollowingTextColor;
            if (!isOtherPlayerProfileNew)
            {
                tagTabPrivateObject.SetActive(false);
                tabTagPublicObject.SetActive(true);
            }
        }
        else
        {
            //followText.text = "Follow";
            followText.text = TextLocalization.GetLocaliseTextByKey("Follow");
            followButtonImage.sprite = followSprite;
            followText.color = followtextColor;
            if (!isOtherPlayerProfileNew)
            {
                tagTabPrivateObject.SetActive(true);
                tabTagPublicObject.SetActive(false);
            }
        }

        if (currentFindFriendWithNameItemScript != null)
        {
            currentFindFriendWithNameItemScript.searchUserRow.isFollowing = isFollow;
            currentFindFriendWithNameItemScript.FollowFollowingSetUp(isFollow);
        }
        //followText.GetComponent<TextLocalization>().LocalizeTextText();
        //Debug.LogError("Other profile follow text:" + followText.text);
    }

    //this is used to destroy user from hot tab after follow.......
    public void DestroyUserFromHotTabAfterFollow()
    {
        APIController.Instance.RemoveFollowedUserFromHot(singleUserProfileData.id);
        APIManager.Instance.OnFeedAPiCalling();
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
        Debug.LogError("Other Player Profile Back Button Click");
        if (currentFindFriendWithNameItemScript != null)
        {
            currentFindFriendWithNameItemScript = null;
        }

        if (backKeyManageList.Count > 0)
        {
            Debug.LogError("lastUserIsFollowFollowing:" + lastUserIsFollowFollowing + "  :isFollowFollowing:" + isFollowFollowing);
            if (lastUserIsFollowFollowing != isFollowFollowing && isFollowFollowing)
            {
                //DestroyUserFromHotTabAfterFollow();
                APIController.Instance.RemoveFollowedUserFromHot(singleUserProfileData.id);
            }

            switch (backKeyManageList[backKeyManageList.Count - 1])
            {
                case "FollowerFollowingListScreen":
                    Debug.LogError("Last Comes from Follower Following list screen my profile");
                    MyProfileDataManager.Instance.myProfileScreen.SetActive(true);
                    FeedUIController.Instance.profileFollowerFollowingListScreen.SetActive(true);
                    FeedUIController.Instance.footerCan.GetComponent<BottomTabManager>().SetDefaultButtonSelection(4);
                    RefreshDataFollowerAndFollowingScreen();
                    break;
                case "HotTabScreen":
                    Debug.LogError("Last Comes from Hot or discover tab Screen");
                    FeedUIController.Instance.RemoveUnFollowedUserFromFollowingTab();
                    FeedUIController.Instance.feedUiScreen.SetActive(true);
                    break;
                case "FollowingTabScreen":
                    Debug.LogError("Last Comes from Following tab screen");
                    FeedUIController.Instance.RemoveUnFollowedUserFromFollowingTab("FollowingTabScreen");
                    FeedUIController.Instance.feedUiScreen.SetActive(true);
                    break;
                case "GroupDetailsScreen":
                    Debug.LogError("Last Comes from message module group details screen");
                    isProfiletranzistFromMessage = true;
                    RemoveAndCheckBackKey();
                    FeedUIController.Instance.footerCan.GetComponent<BottomTabManager>().OnClickWorldButton();
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

        FeedUIController.Instance.otherPlayerProfileScreen.SetActive(false);

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
        Debug.LogError("OnClickFollowUser id" + singleUserProfileData.id.ToString() + " :FollowText:" + followText.text);
        //FeedUIController.Instance.ShowLoader(true);
        if (isFollowFollowing)
        {
            //unfollow.......
            APIManager.Instance.RequestUnFollowAUser(singleUserProfileData.id.ToString(), "OtherUserProfile");
        }
        else
        {
            //follow.......
            APIManager.Instance.RequestFollowAUser(singleUserProfileData.id.ToString(), "OtherUserProfile");
        }
    }

    public void OnClickMessageButtonClick()
    {
        isTempDirectMessageScreenOpen = true;
        FeedUIController.Instance.footerCan.GetComponent<BottomTabManager>().OnClickWorldButton();
        if (!PremiumUsersDetails.Instance.PremiumUserUI.activeSelf)
        {
            //Debug.LogError("OnClickMessageButtonClick000000");
            if (MessageController.Instance != null)
            {
                isDirectMessageScreenOpen = true;
                if (backKeyManageList.Contains("GroupDetailsScreen"))
                {
                    backKeyManageList.Remove("GroupDetailsScreen");
                    //RemoveAndCheckBackKey();
                }
                //Debug.LogError("OnClickMessageButtonClick");
                MessageController.Instance.OnDirectMessageFromOtherUserProfile(singleUserProfileData.id, singleUserProfileData.name, profileImage.sprite);
            }
        }
        isTempDirectMessageScreenOpen = false;
    }

    #region Get User Details API Integrate........
    public void RequestGetUserDetails(SingleUserProfileData singleUserProfileData1)
    {
        singleUserProfileData = singleUserProfileData1;

        CheckAndResetFeedClickOnUserProfile();//check for user and if new user then clear old data.......

        LoadUserData(true);

        FeedUIController.Instance.ShowLoader(true);

        //Debug.LogError("RequestGetUserDetails:" + singleUserProfileData1.id);
        StartCoroutine(IERequestGetUserDetails(singleUserProfileData1.id));
        APIManager.Instance.RequestGetFeedsByUserId(singleUserProfileData1.id, 1, 30, "OtherPlayerFeed");
        RequestGetOtherUserRole(singleUserProfileData1.id);
    }

    public void RequestGetUserDetails(int id)
    {
        singleUserProfileData.id = id;
        CheckAndResetFeedClickOnUserProfile();//check for user and if new user then clear old data.......

        LoadUserData(true);

        FeedUIController.Instance.ShowLoader(true);
        //Debug.LogError("RequestGetUserDetails:" + singleUserProfileData1.id);
        StartCoroutine(IERequestGetUserDetails(id));
        APIManager.Instance.RequestGetFeedsByUserId(id, 1, 30, "OtherPlayerFeed");
        RequestGetOtherUserRole(id);
    }

    public bool isUserProfileDataDiff = false;
    public IEnumerator IERequestGetUserDetails(int userId, bool IsFirstTime = true)
    {
        WWWForm form = new WWWForm();

        form.AddField("userId", userId);

        isUserProfileDataDiff = false;

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetSingleUserProfile), form))
        {
            www.SetRequestHeader("Authorization", APIManager.Instance.userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.LogError("IERequestGetSingleUserDetails data:" + data);
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
                    //List<string> tempStr = new List<string>();
                    //userRolesView.SetUpUserRole("", tempStr);//this method is used to set user role.......
                }
            }
        }
        if (true)
        {
            //Riken
            //LoadUserData(isUserProfileDataDiff);
            LoadUserData(true);
        }
        else
        {
            textPlayerName.text = singleUserProfileData.name;
            textHeaderUserName.text = singleUserProfileData.name;
            textPlayerTottleFollower.text = singleUserProfileData.followerCount.ToString();
            textPlayerTottleFollowing.text = singleUserProfileData.followingCount.ToString();
            textPlayerTottlePost.text = singleUserProfileData.feedCount.ToString();
        }
    }

    //this method is used to Get Other userRole and pass info.......
    public void RequestGetOtherUserRole(int userId)
    {
        Debug.LogError("RequestGetOtherUserRole userId:" + userId);
        StartCoroutine(IERequestGetOtherUserRole(userId));
    }

    IEnumerator IERequestGetOtherUserRole(int userId)
    {
        //using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetSingleUserRole), form))
        Debug.LogError("url:" + ConstantsGod.API_BASEURL_XANALIA + ConstantsGod.r_url_GetSingleUserRole + userId);
        using (UnityWebRequest www = UnityWebRequest.Get(ConstantsGod.API_BASEURL_XANALIA + ConstantsGod.r_url_GetSingleUserRole + userId))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.LogError("IERequestGetOtherUserRole data:" + data);
                SingleUserRoleRoot singleUserRoleRoot = JsonConvert.DeserializeObject<SingleUserRoleRoot>(data);
                if (singleUserRoleRoot.success)
                {
                    List<string> tempUserList = new List<string>();
                    tempUserList = singleUserRoleRoot.data;
                    string userPriorityRole = UserRegisterationManager.instance.GetOtherUserPriorityRole(tempUserList);
                    userRolesView.SetUpUserRole(userPriorityRole, tempUserList);//this method is used to set user role.......                    
                }
            }
        }
    }
    #endregion

    #region Get Image From AWS
    public void GetImageFromAWS(string key, Image mainImage)
    {
        //Debug.LogError("GetImageFromAWS key:" + key);
        //GetExtentionType(key);
        if (AssetCache.Instance.HasFile(key))
        {
            //Debug.LogError("Chat Image Available on Disk");
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
                    //Debug.LogError("Save and Image download success");
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
        Debug.LogError("ExtentionType: " + extension);
        if (extension == "png" || extension == "jpg" || extension == "jpeg" || extension == "gif" || extension == "bmp" || extension == "tiff" || extension == "heic")
        {
            currentExtention = ExtentionType.Image;
            return ExtentionType.Image;
        }
        else if (extension == "mp4" || extension == "mov" || extension == "wav" || extension == "avi")
        {
            currentExtention = ExtentionType.Video;
            // Debug.LogError("vvvvvvvvvvvvvvvvvvvvvvvvvvvv");
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