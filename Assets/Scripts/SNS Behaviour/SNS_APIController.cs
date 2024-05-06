using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SuperStar.Helpers;
using UnityEngine.Networking;
using System;
using System.IO;
using UnityEngine.Events;

public class SNS_APIController : MonoBehaviour
{
    public static SNS_APIController Instance;

    [Header("Feed")]
    public GameObject followingFeedPrefab;
    //public Transform followingFeedTabLeftContainer; //rik
    //public Transform followingFeedTabRightContainer; //rik

    public GameObject forYouFeedPrefab;
    //public Transform forYouFeedTabContainer; //rik

    public GameObject hotFeedPrefab;
    public GameObject NewHotPrefab;
    //public Transform hotTabContainer; //rik
    public GameObject hotItemPrefab;

    public GameObject videofeedPrefab;
    //public Transform videofeedParent; //rik

    public GameObject FollowingUserVideoFeedPrefab;
    public GameObject PostVideoFeedPrefab;

    //public GameObject followingFeedMainContainer; //rik

    public GameObject findFriendFeedPrefab;
    public GameObject mutalFrndPrefab;

    public GameObject feedTopStoryFollowerPrefab;

    public List<int> feedFollowingIdList = new List<int>();
    public List<int> feedForYouIdList = new List<int>();
    public List<int> feedHotIdList = new List<int>();

    public FeedRawItemController currentFeedRawItemController;

    [Space]
    [Header("Message")]
    public GameObject followingUser;
    //public Transform followingUserParent; //rik
    public GameObject conversationPrefab;
    //public Transform conversationPrefabParent; //rik
    public GameObject selectedFriendItemPrefab;
    //public Transform selectedFriendItemPrefabParent; //rik
    public GameObject chatPrefabUser, chatPhotoPrefabUser, chatPrefabOther, chatPhotoPrefabOther;
    //public Transform chatPrefabParent; //rik
    public GameObject chatShareAttechmentPrefab;
    //public Transform chatShareAttechmentparent, chatShareAttechmentPhotoPanel, chatShareAttechmentMainPanel; //rik
    //public Transform chooseAttechmentparent; //rik
    public GameObject chooseAttechmentprefab;
    //public GameObject chatShareAttechmentPanel; //rik
    public GameObject chatMemberPrefab;
    //public Transform chatMemberParent; //rik
    public GameObject chatTimePrefab;
    //public Transform chatTimeParent; //rik
    public GameObject saveAttechmentPrefab;
    //public Transform saveAttechmentParent; //rik
    public List<string> allFollowingUserList = new List<string>();
    public List<string> allChatMemberList = new List<string>();
    public List<string> allConversationList = new List<string>();
    public List<string> chatTimeList = new List<string>();

    [Header("Default Avatar Url")]
    public Sprite defaultAvatarSP;


    private void OnEnable()
    {
        Instance = this;
    }

    #region Feed Module Reference................................................................................

    //this method is used to instantiate discover/foryou tab items.......
    //public void AllUserForYouFeeds(int pageNum, string callingFrom)
    //{
    //    StartCoroutine(HotWaitToEnableDataLoadedBool(pageNum));
    //}

    //public IEnumerator HotWaitToEnableDataLoadedBool(int pageNum)
    //{
    //    yield return new WaitForSeconds(0.5f);

    //}



    //this method is used to Instantiate search user.......
    public void FeedGetAllSearchUser()
    {
        FeedUIController.Instance.AddFrndNoSearchFound.SetActive(false);
        foreach (Transform item in FeedUIController.Instance.findFriendContainer)
        {
            Destroy(item.gameObject);
        }
        if (FeedUIController.Instance.findFriendInputFieldAdvanced.Text != "")
        {
            if (SNS_APIManager.Instance.searchUserRoot.data.rows.Count > 0)
            {
                for (int j = 0; j < SNS_APIManager.Instance.searchUserRoot.data.rows.Count; j++)
                {
                    if (!SNS_APIManager.Instance.searchUserRoot.data.rows[j].id.Equals(SNS_APIManager.Instance.userId))
                    {
                        GameObject searchUserObj = Instantiate(findFriendFeedPrefab, FeedUIController.Instance.findFriendContainer);
                        //searchUserObj.GetComponent<FindFriendWithNameItem>().searchUserRow = SNS_APIManager.Instance.searchUserRoot.data.rows[j];
                        searchUserObj.GetComponent<FindFriendWithNameItem>().SetupData(SNS_APIManager.Instance.searchUserRoot.data.rows[j], true);
                    }
                }
                if (SNS_APIManager.Instance.searchUserRoot.data.rows.Count > 10)
                {
                    GameObject extra = Instantiate(FeedUIController.Instance.ExtraPrefab, FeedUIController.Instance.findFriendContainer);
                }
            }
            else
            {
                FeedUIController.Instance.AddFrndNoSearchFound.SetActive(true);
            }
        }
    }
    public void FeedGetAllSearchUserForProfile()
    {
        FeedUIController.Instance.profileNoSearchFound.SetActive(false);
        foreach (Transform item in FeedUIController.Instance.profileSerachResultsContainer)
        {
            Destroy(item.gameObject);
        }
        if (FeedUIController.Instance.profileFinfFriendAdvancedInputField.Text != "")
        {
            if (SNS_APIManager.Instance.searchUserRoot.data.rows.Count > 0)
            {
                for (int j = 0; j < SNS_APIManager.Instance.searchUserRoot.data.rows.Count; j++)
                {
                    if (!SNS_APIManager.Instance.searchUserRoot.data.rows[j].id.Equals(SNS_APIManager.Instance.userId))
                    {
                        GameObject searchUserObj = Instantiate(findFriendFeedPrefab, FeedUIController.Instance.profileSerachResultsContainer);
                        //searchUserObj.GetComponent<FindFriendWithNameItem>().searchUserRow = SNS_APIManager.Instance.searchUserRoot.data.rows[j];
                        searchUserObj.GetComponent<FindFriendWithNameItem>().SetupData(SNS_APIManager.Instance.searchUserRoot.data.rows[j], true);
                    }
                }
                if (SNS_APIManager.Instance.searchUserRoot.data.rows.Count > 10)
                {
                    GameObject extra = Instantiate(FeedUIController.Instance.ExtraPrefab, FeedUIController.Instance.profileSerachResultsContainer);
                }
            }
            else
            {
                FeedUIController.Instance.profileNoSearchFound.SetActive(true);
            }
        }
    }

    public void ShowHotFirend(HotUsersRoot hotUserRoot)
    {
        foreach (Transform item in FeedUIController.Instance.hotFriendContainer.transform)
        {
            Destroy(item.gameObject);
        }
        if (hotUserRoot.data.rows.Count > 0)
        {
            for (int j = 0; j <= hotUserRoot.data.rows.Count; j++)
            {
                if (j < hotUserRoot.data.rows.Count)
                {
                    if (!hotUserRoot.data.rows[j].user.id.Equals(SNS_APIManager.Instance.userId))
                    {
                        GameObject searchUserObj = Instantiate(findFriendFeedPrefab, FeedUIController.Instance.hotFriendContainer.transform);
                        //searchUserObj.GetComponent<FindFriendWithNameItem>().searchUserRow = SNS_APIManager.Instance.searchUserRoot.data.rows[j];
                        searchUserObj.GetComponent<FindFriendWithNameItem>().SetupDataHotUsers(hotUserRoot.data.rows[j].user, hotUserRoot.data.rows[j].am_i_following, hotUserRoot.data.rows[j].is_following_me, hotUserRoot.data.rows[j].is_close_friend);
                    }
                }
                //else
                //{
                //    for (int i = 0; i < 4; i++)
                //    {
                //        GameObject searchUserObj = Instantiate(findFriendFeedPrefab, FeedUIController.Instance.hotFriendContainer.transform);
                //        //searchUserObj.GetComponent<FindFriendWithNameItem>().searchUserRow = SNS_APIManager.Instance.searchUserRoot.data.rows[j];
                //        searchUserObj.GetComponent<FindFriendWithNameItem>().SetupDataHotUsers(hotUserRoot.data.rows[0].user, hotUserRoot.data.rows[0].am_i_following, hotUserRoot.data.rows[0].is_following_me, hotUserRoot.data.rows[0].is_close_friend, true);
                //    }
                //}
            }
            if (hotUserRoot.data.rows.Count > 10)
            {
                GameObject extra = Instantiate(FeedUIController.Instance.ExtraPrefab, FeedUIController.Instance.hotFriendContainer.transform);
            }
        }
        GameManager.Instance.m_MainCamera.gameObject.SetActive(true);
    }


    public void ShowRecommendedFriends(SearchUserRoot searchUserRoot)
    {
        foreach (Transform item in FeedUIController.Instance.AddFrndRecommendedContainer.transform)
        {
            Destroy(item.gameObject);
        }
        if (searchUserRoot.data.rows.Count > 0)
        {
            for (int j = 0; j < searchUserRoot.data.rows.Count; j++)
            {
                if (!searchUserRoot.data.rows[j].id.Equals(SNS_APIManager.Instance.userId))
                {
                    GameObject searchUserObj = Instantiate(findFriendFeedPrefab, FeedUIController.Instance.AddFrndRecommendedContainer.transform);
                    //searchUserObj.GetComponent<FindFriendWithNameItem>().searchUserRow = SNS_APIManager.Instance.searchUserRoot.data.rows[j];
                    searchUserObj.GetComponent<FindFriendWithNameItem>().SetupData(SNS_APIManager.Instance.searchUserRoot.data.rows[j]);
                }
            }
            if (searchUserRoot.data.rows.Count > 10)
            {
                GameObject extra = Instantiate(FeedUIController.Instance.ExtraPrefab, FeedUIController.Instance.AddFrndRecommendedContainer.transform);
            }
        }
    }

    public void ShowMutalFrnds(SearchUserRoot searchUserRoot)
    {
        foreach (Transform item in FeedUIController.Instance.AddFrndMutalFrndContainer.transform)
        {
            Destroy(item.gameObject);
        }
        if (searchUserRoot.data.rows.Count > 0)
        {
            for (int j = 0; j < searchUserRoot.data.rows.Count; j++)
            {
                if (!searchUserRoot.data.rows[j].id.Equals(SNS_APIManager.Instance.userId))
                {
                    GameObject searchUserObj = Instantiate(mutalFrndPrefab, FeedUIController.Instance.AddFrndMutalFrndContainer.transform);
                    searchUserObj.GetComponent<FindFriendWithNameItem>().SetupData(searchUserRoot.data.rows[j]);
                }
            }
            if (searchUserRoot.data.rows.Count > 10)
            {
                GameObject extra = Instantiate(FeedUIController.Instance.ExtraPrefab, FeedUIController.Instance.AddFrndMutalFrndContainer.transform);
            }
        }
    }


    public void AdFrndFollowingFetch()
    {
        foreach (Transform item in FeedUIController.Instance.adFrndFollowingListContainer.transform)
        {
            Destroy(item.gameObject);
        }
        SNS_APIManager.Instance.SetAdFrndFollowing();
    }

    public void SpwanAdFrndFollowing()
    {
        FeedUIController.Instance.AddFrndNoFollowing.SetActive(false);
        if (SNS_APIManager.Instance.adFrndFollowing.data.rows.Count > 0)
        {
            for (int i = 0; i <= SNS_APIManager.Instance.adFrndFollowing.data.rows.Count; i++)
            {
                if (i < SNS_APIManager.Instance.adFrndFollowing.data.rows.Count)
                {
                    if (SNS_APIManager.Instance.userId == SNS_APIManager.Instance.adFrndFollowing.data.rows[i].followedBy && !SNS_APIManager.Instance.adFrndFollowing.data.rows[i].userId.Equals(SNS_APIManager.Instance.userId))
                    {
                        GameObject followingObject = Instantiate(FeedUIController.Instance.adFriendFollowingPrefab, FeedUIController.Instance.adFrndFollowingListContainer);
                        followingObject.GetComponent<FollowingItemController>().SetupData(SNS_APIManager.Instance.adFrndFollowing.data.rows[i], false);
                        print("~~" + followingObject.GetComponent<Button>() + "~~~~~" + followingObject);
                        print(followingObject.gameObject.activeInHierarchy + "-------------");
                        followingObject.GetComponent<FindFriendWithNameItem>().IsInFollowingTab = true;
                    }
                }
            }
            if (SNS_APIManager.Instance.adFrndFollowing.data.rows.Count > 10)
            {
                GameObject extra = Instantiate(FeedUIController.Instance.ExtraPrefab, FeedUIController.Instance.adFrndFollowingListContainer);
            }
        }
        else
        {
            FeedUIController.Instance.AddFrndNoFollowing.SetActive(true);
        }
    }
    public void SpwanProfileFollowing()
    {
        FeedUIController.Instance.noProfileFollowing.SetActive(false);
        foreach (Transform item in FeedUIController.Instance.profileFollowingListContainer.transform)
        {
            Destroy(item.gameObject);
        }
        if (SNS_APIManager.Instance.profileAllFollowingRoot.data.rows.Count > 0)
        {
            for (int i = 0; i <= SNS_APIManager.Instance.profileAllFollowingRoot.data.rows.Count; i++)
            {
                if (i < SNS_APIManager.Instance.profileAllFollowingRoot.data.rows.Count)
                {
                    if (SNS_APIManager.Instance.userId == SNS_APIManager.Instance.profileAllFollowingRoot.data.rows[i].followedBy && !SNS_APIManager.Instance.profileAllFollowingRoot.data.rows[i].userId.Equals(SNS_APIManager.Instance.userId))
                    {
                        GameObject followingObject = Instantiate(FeedUIController.Instance.followingPrefab, FeedUIController.Instance.profileFollowingListContainer);
                        followingObject.GetComponent<FollowingItemController>().SetupData(SNS_APIManager.Instance.profileAllFollowingRoot.data.rows[i], false);
                        //followingObject.GetComponent<Button>().enabled = false;
                        followingObject.GetComponent<FindFriendWithNameItem>().IsInFollowingTab = true;
                    }
                }
            }
            if (SNS_APIManager.Instance.profileAllFollowingRoot.data.rows.Count > 10)
            {
                GameObject extra = Instantiate(FeedUIController.Instance.ExtraPrefab, FeedUIController.Instance.profileFollowingListContainer);
            }
        }
        else
        {
            FeedUIController.Instance.noProfileFollowing.SetActive(true);
        }
    }
    #endregion


    public void UpdateAvatarOnServer(string key, string callingFrom)
    {
        Debug.Log("test update avatr key:" + key);
        SNS_APIManager.Instance.RequestUpdateUserAvatar(key, callingFrom);
    }

    public void ProfileDataUpdateFromSocket(int id)
    {
        if (ConstantsHolder.xanaConstants.IsProfileVisit && id == ConstantsHolder.xanaConstants.SnsProfileID )
        {
            if (ConstantsHolder.xanaConstants.IsOtherProfileVisit) // check is other profile page open
            {
                OtherPlayerProfileData.Instance.SocketOtherProfileUpdate(ConstantsHolder.xanaConstants.SnsProfileID);
            }
            else
            {
                MyProfileDataManager.Instance.RequestGetUserDetails();
            }
        }
    }

}

//this class are Global Veriable Store
public static class GlobalVeriableClass
{
    public static string callingScreen = "";
    public static string GettingBackFromScene = "";
}