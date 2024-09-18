using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckOnlineFriend : MonoBehaviour
{
    public int friendId;
    public SavingCharacterDataClass json;
    [HideInInspector]
    public string randomPresetGender;
    [HideInInspector]
    public int randomPreset;
    [SerializeField]
    GameObject offlineFriendName, onlineFriendName;
    WorldItemView worldItemView;
    private void OnEnable()
    {
        HomeScoketHandler.instance.spaceJoinedFriendStatus += SpaceJoinedFriend;
        HomeScoketHandler.instance.spaceExitFriendStatus += SpaceExitFriend;
    }
    private void OnDisable()
    {
        HomeScoketHandler.instance.spaceJoinedFriendStatus -= SpaceJoinedFriend;
        HomeScoketHandler.instance.spaceExitFriendStatus -= SpaceExitFriend;
    }
    private void Start()
    {
        worldItemView = GetComponent<WorldItemView>();
        if (offlineFriendName)
            offlineFriendName.GetComponent<Button>().onClick.AddListener(onclickFriendNameButton);
        if (onlineFriendName)
            onlineFriendName.GetComponent<Button>().onClick.AddListener(GotoSpace);
    }
    public void SpaceJoinedFriend(FriendOnlineStatus friendOnlineStatus)
    {
        if (friendOnlineStatus.userId == friendId)
        {
            ToggleOnlineStatus(friendOnlineStatus.isOnline);
            WorldManager.instance.SetFriendsJoinedWorldInfo(friendOnlineStatus.worldDetails, worldItemView);
        }
    }
    public void SpaceExitFriend(FriendOnlineStatus friendOnlineStatus)
    {
        if (friendOnlineStatus.userId == friendId)
        {
            ToggleOnlineStatus(friendOnlineStatus.isOnline);
        }
    }
    public void ToggleOnlineStatus(bool toggle)
    {
        if (toggle)
        {
            onlineFriendName.SetActive(true);
            offlineFriendName.SetActive(false);
        }
        else
        {
            onlineFriendName.SetActive(false);
            offlineFriendName.SetActive(true);
        }
    }
    void GotoSpace()
    {
        GameManager.Instance.HomeCameraInputHandler(false);
        ConstantsHolder.xanaConstants.isFromHomeTab = true;
        worldItemView.OnClickPrefab();
    }
    public void onclickFriendNameButton()
    {
        if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
        {
            if (ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
            {
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                UserLoginSignupManager.instance.signUpOrloginSelectionPanel.SetActive(true);
            }
            else
            {
                UserLoginSignupManager.instance.LoginRegisterScreen.SetActive(true);
            }
        }
        else
        {
            GameManager.Instance.bottomTabManagerInstance.InitProfileData();
            FeedUIController.Instance.ShowLoader(true);
            //print("Getting Click here" + _data.user_id);
            if (friendId != 0)
                SNS_APIManager.Instance.GetHomeFriendProfileData<CheckOnlineFriend>(friendId, this);
            else
                GameManager.Instance.bottomTabManagerInstance.OnClickProfileButton();

            if (MyProfileDataManager.Instance)
            {
                MyProfileDataManager.Instance.UpdateBackButtonAction(GameManager.Instance.bottomTabManagerInstance.OnClickHomeButton);
            }
        }

    }
    public void SetupHomeFriendProfile(SearchUserRow searchUserRow)
    {
        //print("Getting Click here name" + _feedUserData.name);
        //Debug.Log("Search User id:" + _feedUserData.id);
        int bodyLayer = LayerMask.NameToLayer("Body");
        SNS_APIManager.Instance.RequestGetUserLatestAvatarData<CheckOnlineFriend>(searchUserRow.id.ToString(), this);
        //DressUpUserAvatar();
        if (MyProfileDataManager.Instance)
        {
            MyProfileDataManager.Instance.OtherPlayerdataObj.SetActive(true);
            OtherPlayerProfileData.Instance.ResetMainScrollDefaultTopPos();
            MyProfileDataManager.Instance.myProfileScreen.SetActive(true);
            FeedUIController.Instance.profileFollowerFollowingListScreen.SetActive(false);
            MyProfileDataManager.Instance.gameObject.SetActive(false);
            FeedUIController.Instance.AddFriendPanel.SetActive(false);
        }
        else
        {
            OtherPlayerProfileData.Instance.myPlayerdataObj.SetActive(false);
            OtherPlayerProfileData.Instance.ResetMainScrollDefaultTopPos();
            OtherPlayerProfileData.Instance.myPlayerdataObj.GetComponent<MyProfileDataManager>().myProfileScreen.SetActive(true);
            //MyProfileDataManager.Instance.myProfileScreen.SetActive(true);
            FeedUIController.Instance.profileFollowerFollowingListScreen.SetActive(false);
            FeedUIController.Instance.AddFriendPanel.SetActive(false);
            //MyProfileDataManager.Instance.gameObject.SetActive(false);
        }
        ProfileUIHandler.instance._renderTexCamera.GetComponent<Camera>().cullingMask &= ~(1 << bodyLayer);
        ProfileUIHandler.instance.SwitchBetweenUserAndOtherProfileUI(false);
        ProfileUIHandler.instance.SetMainScrollRefs();
        ProfileUIHandler.instance.editProfileBtn.SetActive(false);
        if (searchUserRow.am_i_following)
        {
            ProfileUIHandler.instance.followProfileBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Unfollow";
        }
        else
        {
            ProfileUIHandler.instance.followProfileBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Follow";
        }
        ProfileUIHandler.instance.followProfileBtn.SetActive(true);
        //ProfileUIHandler.instance.SetUserAvatarDefaultClothing();

        AllUserWithFeedRow feedRawData = new AllUserWithFeedRow();
        feedRawData.id = searchUserRow.id;
        feedRawData.name = searchUserRow.name;
        feedRawData.avatar = searchUserRow.avatar;
        feedRawData.UserProfile = searchUserRow.userProfile;
        feedRawData.FollowerCount = searchUserRow.followerCount;
        feedRawData.FollowingCount = searchUserRow.followingCount;
        feedRawData.feedCount = searchUserRow.feedCount;

        //FeedUIController.Instance.ShowLoader(true);

        //OtherPlayerProfileData.Instance.currentFindFriendWithNameItemScript = this;

        OtherPlayerProfileData.Instance.FeedRawData = feedRawData;
        ////OtherPlayerProfileData.Instance.OnSetUserUi(_feedUserData.isFollowing);
        ////OtherPlayerProfileData.Instance.LoadData();

        //OtherPlayerProfileData.Instance.backKeyManageList.Add("FindFriendScreen");//For back mamages.......

        //APIManager.Instance.RequesturlGetTaggedFeedsByUserId(FeedRawData.id, 1, FeedRawData.feedCount);//rik cmnt
        //APIManager.Instance.RequestGetFeedsByUserId(_feedUserData.id, 1, 30, "OtherPlayerFeed");

        //this api get any user profile data and feed for other player profile....... 
        SingleUserProfileData singleUserProfileData = new SingleUserProfileData();
        singleUserProfileData.id = searchUserRow.id;
        singleUserProfileData.name = searchUserRow.name;
        singleUserProfileData.email = "";
        singleUserProfileData.avatar = searchUserRow.avatar;
        singleUserProfileData.followerCount = searchUserRow.followerCount;
        singleUserProfileData.followingCount = searchUserRow.followingCount;
        singleUserProfileData.feedCount = searchUserRow.feedCount;
        singleUserProfileData.isFollowing = searchUserRow.is_following_me;
        singleUserProfileData.userOccupiedAssets = searchUserRow.userOccupiedAssets;
        print("_feedUserData occupied asstes count: " + searchUserRow.userOccupiedAssets.Count);
        print("singleUserProfileData occupied asstes count: " + singleUserProfileData.userOccupiedAssets.Count);

        SingleUserProfile singleUserProfile = new SingleUserProfile();
        singleUserProfileData.userProfile = singleUserProfile;
        if (searchUserRow.userProfile != null)
        {
            singleUserProfileData.userProfile.id = searchUserRow.userProfile.id;
            singleUserProfileData.userProfile.userId = searchUserRow.userProfile.userId;
            singleUserProfileData.userProfile.gender = searchUserRow.userProfile.gender;
            singleUserProfileData.userProfile.job = searchUserRow.userProfile.job;
            singleUserProfileData.userProfile.country = searchUserRow.userProfile.country;
            singleUserProfileData.userProfile.website = searchUserRow.userProfile.website;
            singleUserProfileData.userProfile.bio = searchUserRow.userProfile.bio;
        }
        OtherPlayerProfileData.Instance.RequestGetUserDetails(singleUserProfileData, true);
    }
    public void DressUpUserAvatar()
    {
        ////Other player avatar initialization required here
        if (SNS_APIManager.Instance.VisitedUserAvatarData != null)
        {
            ProfileUIHandler.instance.SetUserAvatarClothing(SNS_APIManager.Instance.VisitedUserAvatarData.json);
        }
        else
        {
            //ProfileUIHandler.instance.SetUserAvatarRandomClothingForProfile(randomPreset, randomPresetGender);
            ProfileUIHandler.instance.SetUserAvatarDefaultClothing();
        }
    }
}
