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
    public int randomPreset;
    [SerializeField]
    GameObject offlineFriendName, onlineFriendName;
    private void OnEnable()
    {
        SocketController.instance.spaceJoinedFriendStatus += SpaceJoinedFriends;
        SocketController.instance.spaceJoinedFriendStatus += SpaceExitFriends;
    }
    private void OnDisable()
    {
        SocketController.instance.spaceJoinedFriendStatus -= SpaceJoinedFriends;
        SocketController.instance.spaceJoinedFriendStatus -= SpaceExitFriends;
    }
    private void Start()
    {
        offlineFriendName.GetComponent<Button>().onClick.AddListener(onclickFriendNameButton);
    }
    public void SpaceJoinedFriends(FriendOnlineStatus friendOnlineStatus)
    {
        if(friendOnlineStatus.userId==friendId)
        {
            if (friendOnlineStatus.isOnline)
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
    }
    public void SpaceExitFriends(FriendOnlineStatus friendOnlineStatus)
    {
        if (friendOnlineStatus.userId == friendId)
        {
            if (friendOnlineStatus.isOnline)
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
    }
    public void onclickFriendNameButton()
    {
        GameManager.Instance.bottomTabManagerInstance.InitProfileData();
        FeedUIController.Instance.ShowLoader(true);
        //print("Getting Click here" + _data.user_id);
        if(friendId!= 0)
            APIManager.Instance.GetHomeFriendProfileData<CheckOnlineFriend>(friendId, this);
        else
            GameManager.Instance.bottomTabManagerInstance.OnClickProfileButton();
    }
    public void SetupHomeFriendProfile(SearchUserRow searchUserRow)
    {
        //print("Getting Click here name" + _feedUserData.name);
        //Debug.Log("Search User id:" + _feedUserData.id);
        APIManager.Instance.RequestGetUserLatestAvatarData<CheckOnlineFriend>(searchUserRow.id.ToString(), this);
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
        ProfileUIHandler.instance.SetUserAvatarDefaultClothing();

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
        if (APIManager.Instance.VisitedUserAvatarData != null)
        {
            ProfileUIHandler.instance.SetUserAvatarClothing(APIManager.Instance.VisitedUserAvatarData.json);
        }
        else
        {
            ProfileUIHandler.instance.SetUserAvatarDefaultClothingForHomeScreen(randomPreset);
        }
    }
}
