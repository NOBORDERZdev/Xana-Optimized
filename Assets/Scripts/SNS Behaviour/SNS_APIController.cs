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

    private void Awake()
    {
        /*if (Instance == null)
        {
            Instance = this;
        }*/
    }

    private void OnEnable()
    {
        Instance = this;
    }

    #region Feed Module Reference................................................................................

    //int objectIndex = 0;
    //this method is used to instantiate Following tab items.......
    public void OnGetAllFeedForFollowingTab(int pageNum, string callingFrom)
    {
       Debug.Log("OnGetAllFeedFollowingTab:" + SNS_APIResponseManager.Instance.followingUserRoot.Data.Rows.Count);
        if (SNS_APIResponseManager.Instance.followingUserRoot.Data.Rows.Count > 0)
        {
            //set defaut followingFeedInitiateTotalCount and followingFeedImageLoadedCount 0
            //FeedsManager.Instance.followingFeedInitiateTotalCount = 0;
            //FeedsManager.Instance.followingFeedImageLoadedCount = 0;

            for (int i = 0; i < SNS_APIResponseManager.Instance.followingUserRoot.Data.Rows.Count; i++)
            {
                Transform followingFeedTabContainer;

                if (!feedFollowingIdList.Contains(SNS_APIResponseManager.Instance.followingUserRoot.Data.Rows[i].Id))
                {
                    /*if (objectIndex % 2 == 0)//new cmnt
                    {
                        followingFeedTabContainer = FeedsManager.Instance.followingFeedTabLeftContainer;
                    }
                    else
                    {
                        followingFeedTabContainer = FeedsManager.Instance.followingFeedTabRightContainer;
                    }*/
                    followingFeedTabContainer = FeedsManager.Instance.followingFeedTabContainer;

                    //Debug.Log("prefab");
                    GameObject followingFeedObject = Instantiate(followingFeedPrefab, followingFeedTabContainer);
                    FeedFollowingItemController feedFollowingItemController = followingFeedObject.GetComponent<FeedFollowingItemController>();
                    feedFollowingItemController.FeedsByFollowingUserRowData = SNS_APIResponseManager.Instance.followingUserRoot.Data.Rows[i];
                    //followingFeedObject.GetComponent<FeedFollowingItemController>().FeedData = SNS_APIResponseManager.Instance.root.data.rows[i].feeds[j];
                    followingFeedObject.name = "Following_" + feedFollowingItemController.FeedsByFollowingUserRowData.Id;
                    feedFollowingItemController.LoadFeed();
                    if (callingFrom == "PullRefresh")
                    {
                        feedFollowingIdList.Insert(0, feedFollowingItemController.FeedsByFollowingUserRowData.Id);
                        followingFeedObject.transform.SetAsFirstSibling();
                    }
                    else
                    {
                        feedFollowingIdList.Add(feedFollowingItemController.FeedsByFollowingUserRowData.Id);
                    }
                    //objectIndex += 1;
                }
            }
            //StartCoroutine(SetContentOnFeed());//new cmnt
            //Debug.Log("isDataLoad true");
            StartCoroutine(WaitToEnableDataLoadedBool(pageNum));
        }
        // OLD FEED UI
        ////if (FeedsManager.Instance.allFeedMessageTextList[1].gameObject.activeSelf)
        ////{
        ////    if (feedFollowingIdList.Count == 0)
        ////    {
        ////        FeedsManager.Instance.AllFeedScreenMessageTextActive(true, 1, UITextLocalization.GetLocaliseTextByKey("no following feed available"));
        ////    }
        ////    else
        ////    {
        ////        FeedsManager.Instance.AllFeedScreenMessageTextActive(false, 1, UITextLocalization.GetLocaliseTextByKey(""));
        ////    }
        ////}
        // END OLD FEED UI
    }

    public IEnumerator SetContentOnFeed()
    {
        yield return new WaitForSeconds(0.01f);
        FeedsManager.Instance.followingFeedMainContainer.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        yield return new WaitForSeconds(0.05f);
        FeedsManager.Instance.followingFeedMainContainer.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    IEnumerator WaitToEnableDataLoadedBool(int pageNum)
    {
        yield return new WaitForSeconds(0.5f);
        FeedsManager.Instance.isDataLoad = true;
        if (pageNum > 1 && SNS_APIResponseManager.Instance.followingUserRoot.Data.Rows.Count > 0)
        {
            FeedsManager.Instance.followingUserCurrentpage += 1;
        }
    }

    //this method is used to instantiate discover/foryou tab items.......
    public void AllUserForYouFeeds(int pageNum, string callingFrom)
    {
        //set defaut hotForYouFeedInitiateTotalCount and hotForYouFeedImageLoadedCount 0
        //FeedsManager.Instance.hotForYouFeedInitiateTotalCount = 0;
        //FeedsManager.Instance.hotForYouFeedImageLoadedCount = 0;

        /*Debug.Log("AllUserForYouFeeds.......:" + SNS_APIResponseManager.Instance.root.data.rows.Count + "  :CallingFrom:" + callingFrom);
        for (int i = 0; i < SNS_APIResponseManager.Instance.root.data.rows.Count; i++)
        {
            if (SNS_APIResponseManager.Instance.root.data.rows[i].feeds.Count > 0)
            {
                //Debug.Log("AllUserForYouFeeds1111111.......:" + SNS_APIResponseManager.Instance.root.data.rows[i].feeds.Count);
                for (int j = 0; j < SNS_APIResponseManager.Instance.root.data.rows[i].feeds.Count; j++)
                {
                    if (!feedForYouIdList.Contains(SNS_APIResponseManager.Instance.root.data.rows[i].feeds[j].id))
                    {
                        //Debug.Log("add prebab ");
                        GameObject forYouFeedObject = Instantiate(forYouFeedPrefab, FeedsManager.Instance.forYouFeedTabContainer);
                        FeedForYouItemController feedForYouItemController = forYouFeedObject.GetComponent<FeedForYouItemController>();

                        feedForYouItemController.FeedRawData = SNS_APIResponseManager.Instance.root.data.rows[i];
                        feedForYouItemController.FeedData = SNS_APIResponseManager.Instance.root.data.rows[i].feeds[j];
                        feedForYouItemController.LoadFeed();
                        forYouFeedObject.name = "Discover_" + feedForYouItemController.FeedData.id.ToString();

                        if (callingFrom == "PullRefresh")
                        {
                            feedForYouIdList.Insert(0, feedForYouItemController.FeedData.id);
                            forYouFeedObject.transform.SetAsFirstSibling();
                        }
                        else
                        {
                            feedForYouIdList.Add(feedForYouItemController.FeedData.id);
                        }                        
                    }
                }
            }
        }*/     //RIKEN

        if (FeedsManager.Instance.allFeedMessageTextList[2].gameObject.activeSelf)
        {
            /*if (feedForYouIdList.Count == 0)
            {
                FeedsManager.Instance.AllFeedScreenMessageTextActive(true, 2, UITextLocalization.GetLocaliseTextByKey("no discover feed available"));
            }
            else
            {
                FeedsManager.Instance.AllFeedScreenMessageTextActive(false, 2, UITextLocalization.GetLocaliseTextByKey(""));
            }*/
        }

        //Debug.Log("isDataLoad true");
        StartCoroutine(HotWaitToEnableDataLoadedBool(pageNum));
        //FeedsManager.Instance.isDataLoad = true;        
    }

    public IEnumerator HotWaitToEnableDataLoadedBool(int pageNum)
    {
        yield return new WaitForSeconds(0.5f);
        //Debug.Log("isDataLoad true");
        FeedsManager.Instance.isDataLoad = true;
        //Riken
        //if (pageNum > 1 && SNS_APIResponseManager.Instance.root.data.rows.Count > 0)
        if (pageNum > 1 && SNS_APIResponseManager.Instance.hotFeedRoot.data.rows.Count > 0)
        {
            FeedsManager.Instance.allFeedCurrentpage += 1;
        }
    }

    //this method is used to instantiate hot tab items.......
    public void AllUsersWithHotFeeds(string callingFrom)
    {
        //set defaut hotFeedInitiateTotalCount and HotFeedImageLoadedCount 0
        //FeedsManager.Instance.hotFeedInitiateTotalCount = 0;
        //FeedsManager.Instance.HotFeedImageLoadedCount = 0;

       Debug.Log("AllUsersWithHotFeeds.......:" + SNS_APIResponseManager.Instance.root.data.rows.Count + "    :CallingFrom:" + callingFrom);
        /*for (int i = 0; i < SNS_APIResponseManager.Instance.root.data.rows.Count; i++)
        {
            if (!feedHotIdList.Contains(SNS_APIResponseManager.Instance.root.data.rows[i].id))
            {
                if (SNS_APIResponseManager.Instance.root.data.rows[i].feeds.Count > 0)
                {
                    GameObject hotFeedFeedObject = Instantiate(hotFeedPrefab, FeedsManager.Instance.hotTabContainer);
                    //hotFeedFeedObject.GetComponent<FeedRawItemController>().FeedRawData = SNS_APIResponseManager.Instance.root.data.rows[i];
                    hotFeedFeedObject.GetComponent<FeedRawItemController>().LoadFeed(SNS_APIResponseManager.Instance.root.data.rows[i]);
                    hotFeedFeedObject.name = "Hot_" + SNS_APIResponseManager.Instance.root.data.rows[i].id.ToString();
                    //if (callingFrom == "PullRefresh")
                    //{
                    //    feedHotIdList.Insert(0, SNS_APIResponseManager.Instance.root.data.rows[i].id);
                    //    hotFeedFeedObject.transform.SetAsFirstSibling();
                    //}
                    //else
                    //{
                    //    feedHotIdList.Add(SNS_APIResponseManager.Instance.root.data.rows[i].id);
                    //}
                    for (int z = 0; z < SNS_APIResponseManager.Instance.root.data.rows[i].feeds.Count; z++)
                    {
                        if (z <= 2)
                        {

                            if (callingFrom == "PullRefresh")
                            {
                                feedHotIdList.Insert(0, SNS_APIResponseManager.Instance.root.data.rows[i].feeds[z].id);
                                hotFeedFeedObject.transform.SetAsFirstSibling();
                            }
                            else
                            {
                                feedHotIdList.Add(SNS_APIResponseManager.Instance.root.data.rows[i].feeds[z].id);
                            }
                        }

                    }
                }
            }
        }*/
        if (SNS_APIResponseManager.Instance.allhotFeedRoot.data.rows.Count > 0)
        {
            for (int i = 0; i < SNS_APIResponseManager.Instance.allhotFeedRoot.data.rows.Count; i++)
            {
                if (!feedHotIdList.Contains(SNS_APIResponseManager.Instance.allhotFeedRoot.data.rows[i].id))
                {
                    //Debug.Log("prefab");
                    GameObject HotFeedObject = Instantiate(NewHotPrefab, FeedsManager.Instance.hotTabContainer);
                    FeedItemController HotFeedItemController = HotFeedObject.GetComponent<FeedItemController>();
                    HotFeedItemController.HotFeed = SNS_APIResponseManager.Instance.allhotFeedRoot.data.rows[i];
                    //followingFeedObject.GetComponent<FeedFollowingItemController>().FeedData = SNS_APIResponseManager.Instance.root.data.rows[i].feeds[j];
                    HotFeedObject.name = "Hot_" + HotFeedItemController.HotFeed.id;
                    HotFeedItemController.LoadFeed();
                    //Debug.Log("APICONTROLLER callingFrom: " + callingFrom);
                    if (callingFrom == "PullRefresh")
                    {
                        feedHotIdList.Insert(0, HotFeedItemController.HotFeed.id);
                        HotFeed hot = SNS_APIResponseManager.Instance.allhotFeedRoot.data.rows[i];
                        SNS_APIResponseManager.Instance.allhotFeedRoot.data.rows.Insert(0, SNS_APIResponseManager.Instance.allhotFeedRoot.data.rows[i]);
                        SNS_APIResponseManager.Instance.allhotFeedRoot.data.rows.Remove(SNS_APIResponseManager.Instance.allhotFeedRoot.data.rows[i]);
                        HotFeedObject.transform.SetAsFirstSibling();
                    }
                    else
                    {
                        feedHotIdList.Add(HotFeedItemController.HotFeed.id);
                    }
                    //objectIndex += 1;
                }
            }
        }
        //////////////////////////////////////////////////////////////////////////
        /*for (int i = 0; i < SNS_APIResponseManager.Instance.hotFeedRoot.data.rows.Count; i++)
        {
            GameObject hotFeedFeedObject = Instantiate(NewHotPrefab, FeedsManager.Instance.hotTabContainer);
            hotFeedFeedObject.GetComponent<FeedItemController>().HotFeed = SNS_APIResponseManager.Instance.hotFeedRoot.data.rows[i];
        }*/
        if (FeedsManager.Instance.allFeedMessageTextList[0].gameObject.activeSelf)
        {
            //Riken
            //if (feedHotIdList.Count == 0)
            if (SNS_APIResponseManager.Instance.hotFeedRoot.data.rows.Count == 0)
            {
                FeedsManager.Instance.AllFeedScreenMessageTextActive(true, 0, UITextLocalization.GetLocaliseTextByKey("no hot feed available"));
            }
            else
            {
                FeedsManager.Instance.AllFeedScreenMessageTextActive(false, 0, UITextLocalization.GetLocaliseTextByKey(""));
            }
        }

        //Debug.Log("isDataLoad true");
        //FeedsManager.Instance.isDataLoad = true;
    }

    //this method is used to Remove items and reset data of hot and discover tab.......
    public void RemoveFollowedUserFromHot(int id)
    {
       Debug.Log("RemoveFollowedUserFromHot id:" + id);
        if (feedHotIdList.Contains(id))
        {
            int index = feedHotIdList.IndexOf(id);
            //Debug.Log("Index:" + index);
            SNS_APIResponseManager.Instance.HotAndDiscoverSaveAndUpdateJson(id, index);//remove data from main data list and updatejson.......

            List<AllUserWithFeed> allFeedsForUser = new List<AllUserWithFeed>();

            //Debug.Log("Deleted Feed Item index:" + index + " :MainId:" + id + "    :ChildCount:" + FeedsManager.Instance.hotTabContainer.childCount);
            if (FeedsManager.Instance.hotTabContainer.childCount > 0 && index >= 0)
            {
                FeedRawItemController feedRawItemController = FeedsManager.Instance.hotTabContainer.GetChild(index).GetComponent<FeedRawItemController>();
                allFeedsForUser = feedRawItemController.FeedRawData.feeds;
                for (int i = 0; i < feedRawItemController.hotItemPrefabParent.childCount; i++)
                {
                    if (!feedRawItemController.hotItemPrefabParent.GetChild(i).GetComponent<FeedItemController>().isImageSuccessDownloadAndSave)
                    {
                        FeedsManager.Instance.hotFeedInitiateTotalCount -= 1;
                    }
                    feedRawItemController.hotItemPrefabParent.GetChild(i).GetComponent<FeedItemController>().ClearMemoryAfterDestroyObj();
                }
                feedRawItemController.ClearMororyAfterDestroyObject();
               Debug.Log("Delete from hot.......index:" + index);
                DestroyImmediate(FeedsManager.Instance.hotTabContainer.GetChild(index).gameObject);
                feedHotIdList.RemoveAt(index);
            }
            if (FeedsManager.Instance.forYouFeedTabContainer.childCount > 0 && allFeedsForUser != null && allFeedsForUser.Count > 0)
            {
                for (int i = 0; i < allFeedsForUser.Count; i++)
                {
                    int feedIndex = feedForYouIdList.IndexOf(allFeedsForUser[i].id);
                   Debug.Log("allFeedsForUser id:" + allFeedsForUser[i].id + "    :FeedIndex:" + feedIndex);
                    if (feedIndex >= 0)
                    {
                        if (!FeedsManager.Instance.forYouFeedTabContainer.GetChild(feedIndex).GetComponent<FeedForYouItemController>().isImageSuccessDownloadAndSave)
                        {
                            FeedsManager.Instance.hotForYouFeedInitiateTotalCount -= 1;
                        }
                        FeedsManager.Instance.forYouFeedTabContainer.GetChild(feedIndex).GetComponent<FeedForYouItemController>().ClearMemoryAfterDestroyObj();
                        DestroyImmediate(FeedsManager.Instance.forYouFeedTabContainer.GetChild(feedIndex).gameObject);
                        feedForYouIdList.RemoveAt(feedIndex);
                    }
                }
            }
            //Resources.UnloadUnusedAssets();
            //Caching.ClearCache();
            //GC.Collect();
            SNS_APIResponseManager.Instance.OnFeedAPiCalling("PullRefresh");
        }
    }

    //this method is used to Remove items and reset data of following tab.......
    public void RemoveFollowingItemAndResetData(int id)
    {
       Debug.Log("RemoveUnFollowedUserFromFollowing id:" + id);
        if (feedFollowingIdList.Contains(id))
        {
            int index = feedFollowingIdList.IndexOf(id);

            //Debug.Log("Deleted Feed Item index:" + index + " :MainId:" + id + "    :ChildCount:" + FeedsManager.Instance.followingFeedTabContainer.childCount);
            if (FeedsManager.Instance.followingFeedTabContainer.childCount > 0 && index >= 0)
            {
                FeedFollowingItemController feedFollowingItemController = FeedsManager.Instance.followingFeedTabContainer.GetChild(index).GetComponent<FeedFollowingItemController>();

                if (!feedFollowingItemController.isImageSuccessDownloadAndSave)
                {
                    FeedsManager.Instance.followingFeedInitiateTotalCount -= 1;
                }
                feedFollowingItemController.ClearMemoryAfterDestroyObj();
                //Debug.Log("Delete from Following tab.......index:" + index);
                DestroyImmediate(FeedsManager.Instance.followingFeedTabContainer.GetChild(index).gameObject);
                feedFollowingIdList.RemoveAt(index);
            }
        }
    }


    //this method is used to Instantiate search user.......
    public void FeedGetAllSearchUser()
    {
         FeedsManager.Instance.AddFrndNoSearchFound.SetActive(false);
        foreach (Transform item in FeedsManager.Instance.findFriendContainer)
        {
            Destroy(item.gameObject);
        }
        if (FeedsManager.Instance.findFriendInputFieldAdvanced.Text != "")
        {
            if (SNS_APIResponseManager.Instance.searchUserRoot.data.rows.Count > 0)
            {
                for (int j = 0; j < SNS_APIResponseManager.Instance.searchUserRoot.data.rows.Count; j++)
                {
                    if (!SNS_APIResponseManager.Instance.searchUserRoot.data.rows[j].id.Equals(SNS_APIResponseManager.Instance.userId)){ 
                        GameObject searchUserObj = Instantiate(findFriendFeedPrefab, FeedsManager.Instance.findFriendContainer);
                        //searchUserObj.GetComponent<FindFriendWithNameItem>().searchUserRow = SNS_APIResponseManager.Instance.searchUserRoot.data.rows[j];
                        searchUserObj.GetComponent<FindFriendWithNameItem>().SetupData(SNS_APIResponseManager.Instance.searchUserRoot.data.rows[j],true);
                    }
                }
                if (SNS_APIResponseManager.Instance.searchUserRoot.data.rows.Count> 10 )
                {
                    GameObject extra = Instantiate(FeedsManager.Instance.ExtraPrefab,FeedsManager.Instance.findFriendContainer);
                }
            }
            else
            {
                FeedsManager.Instance.AddFrndNoSearchFound.SetActive(true);
            }
        }
    }
    public void FeedGetAllSearchUserForProfile()
    {
        FeedsManager.Instance.profileNoSearchFound.SetActive(false);
        foreach (Transform item in FeedsManager.Instance.profileSerachResultsContainer)
        {
            Destroy(item.gameObject);
        }
        if (FeedsManager.Instance.profileFinfFriendAdvancedInputField.Text != "")
        {
            if (SNS_APIResponseManager.Instance.searchUserRoot.data.rows.Count > 0)
            {
                for (int j = 0; j < SNS_APIResponseManager.Instance.searchUserRoot.data.rows.Count; j++)
                {
                    if (!SNS_APIResponseManager.Instance.searchUserRoot.data.rows[j].id.Equals(SNS_APIResponseManager.Instance.userId))
                    {
                        GameObject searchUserObj = Instantiate(findFriendFeedPrefab, FeedsManager.Instance.profileSerachResultsContainer);
                        //searchUserObj.GetComponent<FindFriendWithNameItem>().searchUserRow = SNS_APIResponseManager.Instance.searchUserRoot.data.rows[j];
                        searchUserObj.GetComponent<FindFriendWithNameItem>().SetupData(SNS_APIResponseManager.Instance.searchUserRoot.data.rows[j], true);
                    }
                }
                if (SNS_APIResponseManager.Instance.searchUserRoot.data.rows.Count > 10)
                {
                    GameObject extra = Instantiate(FeedsManager.Instance.ExtraPrefab, FeedsManager.Instance.profileSerachResultsContainer);
                }
            }
            else
            {
                FeedsManager.Instance.profileNoSearchFound.SetActive(true);
            }
        }
    }

    public void ShowHotFirend(HotUsersRoot hotUserRoot)
    {
        foreach (Transform item in FeedsManager.Instance.hotFriendContainer.transform)
        {
            Destroy(item.gameObject);
        }
        if (hotUserRoot.data.rows.Count > 0)
        {
            for (int j = 0; j <= hotUserRoot.data.rows.Count; j++)
            {
                if (j < hotUserRoot.data.rows.Count)
                {
                    if (!hotUserRoot.data.rows[j].user.id.Equals(SNS_APIResponseManager.Instance.userId))
                    {
                        GameObject searchUserObj = Instantiate(findFriendFeedPrefab, FeedsManager.Instance.hotFriendContainer.transform);
                        //searchUserObj.GetComponent<FindFriendWithNameItem>().searchUserRow = SNS_APIResponseManager.Instance.searchUserRoot.data.rows[j];
                        searchUserObj.GetComponent<FindFriendWithNameItem>().SetupDataHotUsers(hotUserRoot.data.rows[j].user, hotUserRoot.data.rows[j].am_i_following, hotUserRoot.data.rows[j].is_following_me, hotUserRoot.data.rows[j].is_close_friend);
                    }
                }
                //else
                //{
                //    for (int i = 0; i < 4; i++)
                //    {
                //        GameObject searchUserObj = Instantiate(findFriendFeedPrefab, FeedsManager.Instance.hotFriendContainer.transform);
                //        //searchUserObj.GetComponent<FindFriendWithNameItem>().searchUserRow = SNS_APIResponseManager.Instance.searchUserRoot.data.rows[j];
                //        searchUserObj.GetComponent<FindFriendWithNameItem>().SetupDataHotUsers(hotUserRoot.data.rows[0].user, hotUserRoot.data.rows[0].am_i_following, hotUserRoot.data.rows[0].is_following_me, hotUserRoot.data.rows[0].is_close_friend, true);
                //    }
                //}
            }
            if (hotUserRoot.data.rows.Count > 10)
            {
                GameObject extra = Instantiate(FeedsManager.Instance.ExtraPrefab, FeedsManager.Instance.hotFriendContainer.transform);
            }
        }
        GameManager.Instance.m_MainCamera.gameObject.SetActive(true);
    }


     public void ShowRecommendedFriends(SearchUserRoot searchUserRoot)
    {
        foreach (Transform item in FeedsManager.Instance.AddFrndRecommendedContainer.transform)
        {
            Destroy(item.gameObject);
        }
        if (searchUserRoot.data.rows.Count > 0)
        {
            for (int j = 0; j < searchUserRoot.data.rows.Count; j++)
            {
                if (!searchUserRoot.data.rows[j].id.Equals(SNS_APIResponseManager.Instance.userId)){
                    GameObject searchUserObj = Instantiate(findFriendFeedPrefab, FeedsManager.Instance.AddFrndRecommendedContainer.transform);
                    //searchUserObj.GetComponent<FindFriendWithNameItem>().searchUserRow = SNS_APIResponseManager.Instance.searchUserRoot.data.rows[j];
                    searchUserObj.GetComponent<FindFriendWithNameItem>().SetupData(SNS_APIResponseManager.Instance.searchUserRoot.data.rows[j]);
                }
            }
            if (searchUserRoot.data.rows.Count > 10 )
            {
                GameObject extra = Instantiate(FeedsManager.Instance.ExtraPrefab,FeedsManager.Instance.AddFrndRecommendedContainer.transform);
            }
        }
    }

     public void ShowMutalFrnds(SearchUserRoot searchUserRoot)
    {
        foreach (Transform item in FeedsManager.Instance.AddFrndMutalFrndContainer.transform)
        {
            Destroy(item.gameObject);
        }
        if (searchUserRoot.data.rows.Count > 0)
        {
            for (int j = 0; j < searchUserRoot.data.rows.Count; j++)
            {
                if(!searchUserRoot.data.rows[j].id.Equals(SNS_APIResponseManager.Instance.userId)){
                    GameObject searchUserObj = Instantiate(mutalFrndPrefab, FeedsManager.Instance.AddFrndMutalFrndContainer.transform);
                    searchUserObj.GetComponent<FindFriendWithNameItem>().SetupData(searchUserRoot.data.rows[j]);
                }
            }
            if (searchUserRoot.data.rows.Count > 10 )
            {
                GameObject extra = Instantiate(FeedsManager.Instance.ExtraPrefab,FeedsManager.Instance.AddFrndMutalFrndContainer.transform);
            }
        }
    }

    //this method is used to create feed top story panel in follower item.......
    public void GetSetAllfollowerInTopStoryPanelUser()
    {
        foreach (Transform item in FeedsManager.Instance.TopPanelMainContainerObj)
        {
            Destroy(item.gameObject);
        }

        for (int i = 0; i < SNS_APIResponseManager.Instance.AllFollowerRoot.data.rows.Count; i++)
        {
            if (SNS_APIResponseManager.Instance.userId != SNS_APIResponseManager.Instance.AllFollowerRoot.data.rows[i].followedBy)
            {
                GameObject followerObj = Instantiate(feedTopStoryFollowerPrefab, FeedsManager.Instance.TopPanelMainContainerObj);
                followerObj.GetComponent<FeedStoryAndCategoryItem>().LoadData(SNS_APIResponseManager.Instance.AllFollowerRoot.data.rows[i]);
            }
        }
         //Old UI CODE
        ////if (SNS_APIResponseManager.Instance.AllFollowerRoot.data.rows.Count > 0)
        ////{
        ////    FeedsManager.Instance.SetupFollowerAndFeedScreen(true);
        ////}
        ////else
        ////{
        ////    FeedsManager.Instance.SetupFollowerAndFeedScreen(false);
        ////}
        //END Old UI CODE
    }

    public void AdFrndFollowingFetch(){
        foreach (Transform item in FeedsManager.Instance.adFrndFollowingListContainer.transform)
        {
            Destroy(item.gameObject);
        }
        SNS_APIResponseManager.Instance.SetAdFrndFollowing();
    }

    public void SpwanAdFrndFollowing()
    {
        FeedsManager.Instance.AddFrndNoFollowing.SetActive(false);
        if (SNS_APIResponseManager.Instance.adFrndFollowing.data.rows.Count > 0)
        {
            for (int i = 0; i <= SNS_APIResponseManager.Instance.adFrndFollowing.data.rows.Count; i++)
            {
                if (i < SNS_APIResponseManager.Instance.adFrndFollowing.data.rows.Count)
                {
                    if (SNS_APIResponseManager.Instance.userId == SNS_APIResponseManager.Instance.adFrndFollowing.data.rows[i].followedBy && !SNS_APIResponseManager.Instance.adFrndFollowing.data.rows[i].userId.Equals(SNS_APIResponseManager.Instance.userId))
                    {
                        GameObject followingObject = Instantiate(FeedsManager.Instance.adFriendFollowingPrefab, FeedsManager.Instance.adFrndFollowingListContainer);
                        followingObject.GetComponent<FollowingItemController>().SetupData(SNS_APIResponseManager.Instance.adFrndFollowing.data.rows[i], false);
                        //followingObject.GetComponent<Button>().enabled = false;
                        print("~~" + followingObject.GetComponent<Button>() + "~~~~~" + followingObject);
                        //followingObject.GetComponent<Button>().onClick.AddListener(FeedsManager.Instance.CheckFollowingCount);
                        print(followingObject.gameObject.activeInHierarchy + "-------------");
                        followingObject.GetComponent<FindFriendWithNameItem>().IsInFollowingTab = true;
                        //#if UNITY_EDITOR
                        //    //GameObject go = new GameObject("myObject");
                        //    UnityEditor.Events.UnityEventTools.AddPersistentListener(followingObject.GetComponent<Button>().onClick, new UnityAction(() =>
                        //{
                        //  check();
                        //}));

                        //#else
                        //followingObject.GetComponent<Button>().onClick.AddListener(() =>
                        //{
                        //    FeedsManager.Instance.CheckFollowingCount();
                        //});
                        //#endif
                    }
                }
                //else
                //{
                //    for (int j = 0; j < 4; j++)
                //    {
                //        GameObject followingObject = Instantiate(FeedsManager.Instance.adFriendFollowingPrefab, FeedsManager.Instance.adFrndFollowingListContainer);
                //        followingObject.GetComponent<FollowingItemController>().SetupData(SNS_APIResponseManager.Instance.adFrndFollowing.data.rows[0], false, true);
                //    }
                //}
            }
            if (SNS_APIResponseManager.Instance.adFrndFollowing.data.rows.Count > 10)
            {
                GameObject extra = Instantiate(FeedsManager.Instance.ExtraPrefab, FeedsManager.Instance.adFrndFollowingListContainer);
            }
        }
        else
        {
            FeedsManager.Instance.AddFrndNoFollowing.SetActive(true);
        }
    }
    public void SpwanProfileFollowing()
    {
        FeedsManager.Instance.noProfileFollowing.SetActive(false);
        foreach (Transform item in FeedsManager.Instance.profileFollowingListContainer.transform)
        {
            Destroy(item.gameObject);
        }
        if (SNS_APIResponseManager.Instance.profileAllFollowingRoot.data.rows.Count > 0)
        {
            for (int i = 0; i <= SNS_APIResponseManager.Instance.profileAllFollowingRoot.data.rows.Count; i++)
            {
                if (i < SNS_APIResponseManager.Instance.profileAllFollowingRoot.data.rows.Count)
                {
                    if (SNS_APIResponseManager.Instance.userId == SNS_APIResponseManager.Instance.profileAllFollowingRoot.data.rows[i].followedBy && !SNS_APIResponseManager.Instance.profileAllFollowingRoot.data.rows[i].userId.Equals(SNS_APIResponseManager.Instance.userId))
                    {
                        GameObject followingObject = Instantiate(FeedsManager.Instance.followingPrefab, FeedsManager.Instance.profileFollowingListContainer);
                        followingObject.GetComponent<FollowingItemController>().SetupData(SNS_APIResponseManager.Instance.profileAllFollowingRoot.data.rows[i], false);
                        //followingObject.GetComponent<Button>().enabled = false;
                        followingObject.GetComponent<FindFriendWithNameItem>().IsInFollowingTab = true;
                    }
                }
                //else
                //{
                //    for (int j = 0; j < 4; j++)
                //    {
                //        GameObject followingObject = Instantiate(FeedsManager.Instance.followingPrefab, FeedsManager.Instance.profileFollowingListContainer);
                //        followingObject.GetComponent<FollowingItemController>().SetupData(SNS_APIResponseManager.Instance.profileAllFollowingRoot.data.rows[0], false, true);
                //    }
                //}
            }
            if (SNS_APIResponseManager.Instance.profileAllFollowingRoot.data.rows.Count > 10)
            {
                GameObject extra = Instantiate(FeedsManager.Instance.ExtraPrefab, FeedsManager.Instance.profileFollowingListContainer);
            }
        }
        else
        {
            FeedsManager.Instance.noProfileFollowing.SetActive(true);
        }
    }
    #endregion

    #region Chat Module Reference................................................................................
    //this method is used to instantiate following user in chat module.......
    public void GetAllFollowingUser(int pageNum)
    {
        //Debug.Log("GetAllFollowingUser");
        if (pageNum == 1)
        {
            allFollowingUserList.Clear();
            foreach (Transform item in SNS_SMSModuleManager.Instance.followingUserParent)
            {
                Destroy(item.gameObject);
            }
            foreach (Transform item in SNS_SMSModuleManager.Instance.selectedFriendItemPrefabParent)
            {
                Destroy(item.gameObject);
            }
            //SNS_SMSModuleManager.Instance.ActiveSelectionScroll();

            SNS_SMSModuleManager.Instance.tottleFollowing = 0;
        }

        bool isMatch = false;
        if (SNS_APIResponseManager.Instance.allFollowingRoot.data.rows.Count > 0)
        {
            for (int j = 0; j < SNS_APIResponseManager.Instance.allFollowingRoot.data.rows.Count; j++)
            {
                if (SNS_SMSModuleManager.Instance.addFrindCallingScreenIndex == 1)
                {
                    for (int k = 0; k < SNS_SMSModuleManager.Instance.allChatGetConversationDatum.group.groupUsers.Count; k++)
                    {
                        if (SNS_APIResponseManager.Instance.allFollowingRoot.data.rows[j].userId == SNS_SMSModuleManager.Instance.allChatGetConversationDatum.group.groupUsers[k].userId)
                        {
                            isMatch = true;
                            break;
                        }
                        else
                        {
                            isMatch = false;
                        }
                    }
                }
                //Debug.Log("Ismatch:" + isMatch);
                if (!isMatch)
                {
                    GameObject followingUserObject = Instantiate(followingUser, SNS_SMSModuleManager.Instance.followingUserParent);
                    //followingUserObject.GetComponent<MessageUserDataScript>().allFollowingRow = SNS_APIResponseManager.Instance.allFollowingRoot.data.rows[j];
                    followingUserObject.GetComponent<MessageUserDataScript>().LoadFeed(SNS_APIResponseManager.Instance.allFollowingRoot.data.rows[j]);
                    allFollowingUserList.Add(SNS_APIResponseManager.Instance.allFollowingRoot.data.rows[j].following.name);
                    SNS_SMSModuleManager.Instance.tottleFollowing += 1;
                }
            }

            SNS_SMSModuleManager.Instance.searchManagerFindFriends.SetUpAllMessageUserData(pageNum);
        }

        SNS_SMSModuleManager.Instance.isSelectFriendDataLoaded = true;

        //SNS_SMSModuleManager.Instance.LoaderShow(false);//False api loader.
        //SNS_SMSModuleManager.Instance.tottleFollowingText.text = ("Following " + SNS_SMSModuleManager.Instance.tottleFollowing);
    }

    //this method is used to instantiate conversation user.......
    public void GetAllConversation()
    {
        allConversationList.Clear();
        foreach (Transform item in SNS_SMSModuleManager.Instance.conversationPrefabParent)
        {
            //Debug.Log("dgfg");
            Destroy(item.gameObject);
        }
        for (int i = 0; i < SNS_APIResponseManager.Instance.allChatGetConversationRoot.data.Count; i++)
        {
            //if (!conversationUserList.Contains(SNS_APIResponseManager.Instance.allChatGetConversationRoot.data[i].id))
            //  {
            GameObject ChatGetConversationObject = Instantiate(conversationPrefab, SNS_SMSModuleManager.Instance.conversationPrefabParent);
            //Debug.Log("here");
            ChatGetConversationObject.GetComponent<AllConversationData>().allChatGetConversationDatum = SNS_APIResponseManager.Instance.allChatGetConversationRoot.data[i];
            ChatGetConversationObject.GetComponent<AllConversationData>().LoadFeed();
            //  conversationUserList.Add(SNS_APIResponseManager.Instance.allChatGetConversationRoot.data[i].id);
            // }

            if (!string.IsNullOrEmpty(SNS_SMSModuleManager.Instance.isDirectCreateFirstTimeGroupName) && SNS_APIResponseManager.Instance.allChatGetConversationRoot.data[i].group != null)//rik first time create group assign current data
            {
               Debug.Log("for first time group:" + SNS_SMSModuleManager.Instance.isDirectCreateFirstTimeGroupName + "   :Id:" + SNS_APIResponseManager.Instance.allChatGetConversationRoot.data[i].group.name);
                if (SNS_APIResponseManager.Instance.allChatGetConversationRoot.data[i].group.name == SNS_SMSModuleManager.Instance.isDirectCreateFirstTimeGroupName)
                {
                    SNS_SMSModuleManager.Instance.allChatGetConversationDatum = SNS_APIResponseManager.Instance.allChatGetConversationRoot.data[i];
                    SNS_SMSModuleManager.Instance.isDirectCreateFirstTimeGroupName = "";
                }
            }
            else if (!string.IsNullOrEmpty(SNS_SMSModuleManager.Instance.isDirectMessageFirstTimeRecivedID))//rik first time create one to one message to assign current data
            {
               Debug.Log("for first time message:" + SNS_SMSModuleManager.Instance.isDirectMessageFirstTimeRecivedID + "   :Id:" + SNS_APIResponseManager.Instance.allChatGetConversationRoot.data[i].receiverId);
                if (SNS_APIResponseManager.Instance.allChatGetConversationRoot.data[i].receiverId == int.Parse(SNS_SMSModuleManager.Instance.isDirectMessageFirstTimeRecivedID) || SNS_APIResponseManager.Instance.allChatGetConversationRoot.data[i].senderId == int.Parse(SNS_SMSModuleManager.Instance.isDirectMessageFirstTimeRecivedID))
                {
                    SNS_SMSModuleManager.Instance.allChatGetConversationDatum = SNS_APIResponseManager.Instance.allChatGetConversationRoot.data[i];
                    SNS_SMSModuleManager.Instance.isDirectMessageFirstTimeRecivedID = "";
                    SNS_SMSModuleManager.Instance.isDirectMessage = false;
                }
            }

            if (SNS_SMSModuleManager.Instance.addFrindCallingScreenIndex == 1 && SNS_APIResponseManager.Instance.allChatGetConversationRoot.data[i].group != null)//added member then after refresh details screen.
            {
                if (SNS_SMSModuleManager.Instance.allChatGetConversationDatum.group.id == SNS_APIResponseManager.Instance.allChatGetConversationRoot.data[i].group.id)
                {
                   Debug.Log("Add meber after refresh details screen");
                    SNS_SMSModuleManager.Instance.allChatGetConversationDatum = SNS_APIResponseManager.Instance.allChatGetConversationRoot.data[i];
                    SetChatMember();
                    SNS_SMSModuleManager.Instance.addFrindCallingScreenIndex = 0;
                    SNS_SMSModuleManager.Instance.LoaderShow(false);//false api loader.
                }
            }
        }
        // SNS_SMSModuleManager.Instance.CreateNewMessageUserList.Clear();
        if (SNS_SMSModuleManager.Instance.startAndWaitMessageText.gameObject.activeSelf)
        {
            SNS_SMSModuleManager.Instance.StartAndWaitMessageTextActive(false, UITextLocalization.GetLocaliseTextByKey(""));//start and wait message text show.......
        }
        if (SNS_SMSModuleManager.Instance.conversationPrefabParent.childCount <= 0)
        {
            SNS_SMSModuleManager.Instance.startConversationPopup.SetActive(true);
            //SNS_SMSModuleManager.Instance.StartAndWaitMessageTextActive(true, UITextLocalization.GetLocaliseTextByKey("start conversation"));//start and wait message text show.......
        }
        else
        {
            SNS_SMSModuleManager.Instance.startConversationPopup.SetActive(false);
        }

        //Debug.Log("befor calling.......:" + SNS_SMSModuleManager.Instance.conversationPrefabParent.childCount);
        StartCoroutine(SNS_SMSModuleManager.Instance.searchManagerAllConversation.SetUpAllConversationData());//rik
    }

    //this method is used to instantiate chat message.......
    public List<int> allChatMessageId = new List<int>();
    public DateTime lastMsgTime;
    public void GetAllChat(int pageNumber, string callingFrom)
    {
        //Debug.Log("SNS_APIController GetAllChat pageNumber.......:" + pageNumber);

        // allChatMessageId.Clear();
        for (int i = 0; i < SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows.Count; i++)
        {
            if (!allChatMessageId.Contains(SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows[i].id))
            {
                lastMsgTime = SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows[i].createdAt;

                if (SNS_APIResponseManager.Instance.r_isCreateMessage)//rik.......
                {
                    SetChetDay(SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows[i].updatedAt, pageNumber);
                }

                bool isSpecialMsg = false;
                //rik show message for user leaved and other special message
                if (!string.IsNullOrEmpty(SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows[i].message.type))
                {
                    //Debug.Log("message type:" + SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows[i].message.type);
                    if (SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows[i].message.type == "LeaveGroup")
                    {
                        GameObject leaveUserMsg = Instantiate(chatTimePrefab, SNS_SMSModuleManager.Instance.chatPrefabParent);

                        string newSTR = SNS_APIResponseManager.DecodedString(SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows[i].message.msg);
                        newSTR = newSTR.Replace("Left", UITextLocalization.GetLocaliseTextByKey("Left"));
                        //newSTR = SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows[i].message.msg.Replace("Left", UITextLocalization.GetLocaliseTextByKey("Left"));
                        leaveUserMsg.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = newSTR;
                        if (pageNumber == 1 && SNS_APIResponseManager.Instance.r_isCreateMessage)
                        {
                            leaveUserMsg.transform.SetAsLastSibling();
                        }
                        else
                        {
                            leaveUserMsg.transform.SetAsFirstSibling();
                        }
                        isSpecialMsg = true;
                    }
                }//end rik

                if (!isSpecialMsg)//rik for all other message show
                {
                    // Debug.Log("i : " + i + "+PageNum:" + pageNumber + ":responce:" + SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows[i]);
                    if (SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows[i].senderId == SNS_APIResponseManager.Instance.userId)
                    {
                        if (SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows[i].message.attachments.Count > 0)
                        {
                            //Debug.Log("urllllll " + SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows[i].message.attachments[0].url);
                            // SNS_SMSModuleManager.Instance.ChatScreen.SetActive(true);
                            //SetChetDay(SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows[i].updatedAt, pageNumber);
                            GameObject ChatPhotoObject = Instantiate(chatPhotoPrefabUser, SNS_SMSModuleManager.Instance.chatPrefabParent);
                            ChatPhotoObject.GetComponent<ChatDataScript>().MessageRow = SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows[i];
                            ChatPhotoObject.GetComponent<ChatDataScript>().LoadFeed();

                            //Debug.Log("r_isCreateMessage" + SNS_APIResponseManager.Instance.r_isCreateMessage);
                            if (pageNumber == 1 && SNS_APIResponseManager.Instance.r_isCreateMessage)
                            {
                                ChatPhotoObject.transform.SetAsLastSibling();
                            }
                            else
                            {
                                ChatPhotoObject.transform.SetAsFirstSibling();
                            }
                        }
                        else if (!string.IsNullOrEmpty(SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows[i].message.msg))
                        {
                            //SetChetDay(SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows[i].updatedAt, pageNumber);
                            GameObject ChatObject = Instantiate(chatPrefabUser, SNS_SMSModuleManager.Instance.chatPrefabParent);
                            ChatObject.GetComponent<ChatDataScript>().MessageRow = SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows[i];
                            ChatObject.GetComponent<ChatDataScript>().LoadFeed();
                            if (pageNumber == 1 && SNS_APIResponseManager.Instance.r_isCreateMessage)
                            {
                                ChatObject.transform.SetAsLastSibling();
                            }
                            else
                            {
                                ChatObject.transform.SetAsFirstSibling();
                            }
                        }
                    }
                    else
                    {
                        if (SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows[i].message.attachments.Count > 0)
                        {
                            // SNS_SMSModuleManager.Instance.ChatScreen.SetActive(true);
                            //SetChetDay(SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows[i].updatedAt, pageNumber);
                            GameObject ChatPhotoObject = Instantiate(chatPhotoPrefabOther, SNS_SMSModuleManager.Instance.chatPrefabParent);
                            ChatPhotoObject.GetComponent<ChatDataScript>().MessageRow = SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows[i];
                            ChatPhotoObject.GetComponent<ChatDataScript>().LoadFeed();
                            if (pageNumber == 1 && SNS_APIResponseManager.Instance.r_isCreateMessage)
                            {
                                ChatPhotoObject.transform.SetAsLastSibling();
                            }
                            else
                            {
                                ChatPhotoObject.transform.SetAsFirstSibling();
                            }
                        }
                        else if (!string.IsNullOrEmpty(SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows[i].message.msg))
                        {
                            //SetChetDay(SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows[i].updatedAt, pageNumber);
                            GameObject ChatObject = Instantiate(chatPrefabOther, SNS_SMSModuleManager.Instance.chatPrefabParent);
                            ChatObject.GetComponent<ChatDataScript>().MessageRow = SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows[i];
                            ChatObject.GetComponent<ChatDataScript>().LoadFeed();
                            if (pageNumber == 1 && SNS_APIResponseManager.Instance.r_isCreateMessage)
                            {
                                ChatObject.transform.SetAsLastSibling();
                            }
                            else
                            {
                                ChatObject.transform.SetAsFirstSibling();
                            }
                        }
                    }
                }

                if (!SNS_APIResponseManager.Instance.r_isCreateMessage)//rik.......
                {
                    SetChetDay(SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows[i].updatedAt, pageNumber);
                }

                allChatMessageId.Add(SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows[i].id);
            }
        }

        SNS_SMSModuleManager.Instance.chatPrefabParent.gameObject.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        if (callingFrom != "SNSSocketHandler")
        {
            SNS_SMSModuleManager.Instance.ChatScreen.SetActive(true);
            SNS_SMSModuleManager.Instance.MessageListScreen.SetActive(false);
        }
        SNS_APIResponseManager.Instance.r_isCreateMessage = false;
        ChatScreenData.Instance.allChatGetConversationDatum = SNS_SMSModuleManager.Instance.allChatGetConversationDatum;
        //Invoke("SetChatScreen", 0.1f);
        if (setChatScreenCo != null)
        {
            StopCoroutine(setChatScreenCo);
        }
        setChatScreenCo = StartCoroutine(SetChatScreen());

        if (pageNumber == 1)
        {
            if (resetEndPosCo != null)
            {
                StopCoroutine(resetEndPosCo);
            }
            resetEndPosCo = StartCoroutine(ResetScrollEndPosition());
        }
    }

    Coroutine setChatScreenCo;
    public IEnumerator SetChatScreen()
    {
        yield return new WaitForSeconds(0.1f);
        SNS_SMSModuleManager.Instance.chatPrefabParent.gameObject.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        if (SNS_APIResponseManager.Instance.allChatMessagesRoot.data.rows.Count > 0)
        {
            //Debug.Log("here");
            SNS_SMSModuleManager.Instance.isChatDataLoaded = false;
        }
    }

    Coroutine resetEndPosCo;
    IEnumerator ResetScrollEndPosition()
    {
        yield return new WaitForSeconds(0.15f);
        SNS_SMSModuleManager.Instance.sNSChatView.ResetContainerPosition();
    }

    //this method is used to instantiate chat date tiem message.......
    GameObject chatTimeObject1 = null;
    GameObject chatTimeObject2 = null;
    GameObject chatTimeObject3 = null;
    public void SetChetDay(DateTime updatedAt, int pageNumber)
    {
        if (DateTime.Now.Date == updatedAt.Date)
        {
            if (!chatTimeList.Contains("TODAY"))
            {
               Debug.Log("TODAY");
                chatTimeObject1 = Instantiate(chatTimePrefab, SNS_SMSModuleManager.Instance.chatTimeParent);
                chatTimeObject1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = UITextLocalization.GetLocaliseTextByKey("TODAY");
                //chatTimeObject1.transform.GetChild(0).GetComponent<UITextLocalization>().LocalizeTextText();
                chatTimeList.Add("TODAY");
                if (pageNumber == 1 && SNS_APIResponseManager.Instance.r_isCreateMessage)
                {
                    chatTimeObject1.transform.SetAsLastSibling();
                }
                else
                {
                    chatTimeObject1.transform.SetAsFirstSibling();
                }
                //chatTimeObject1.transform.SetAsFirstSibling();
            }
            else
            {
                //Debug.Log("today chatTimeObject:" + chatTimeObject1);
                if (chatTimeObject1 != null && !SNS_APIResponseManager.Instance.r_isCreateMessage)
                {
                    chatTimeObject1.transform.SetAsFirstSibling();
                }
            }
        }
        else
        {
            DateTime converTime = TimeZoneInfo.ConvertTimeFromUtc(updatedAt, TimeZoneInfo.Local);
            TimeSpan dateDiff = (DateTime.Now.Date - converTime.Date);
            //Debug.Log("dateDiff" + dateDiff);
            if (dateDiff.TotalDays == 1)
            {
                if (!chatTimeList.Contains("YESTERDAY"))
                {
                    //Debug.Log("YESTERDAY");
                    chatTimeObject2 = Instantiate(chatTimePrefab, SNS_SMSModuleManager.Instance.chatTimeParent);
                    chatTimeObject2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = UITextLocalization.GetLocaliseTextByKey("YESTERDAY");
                    //chatTimeObject2.transform.GetChild(0).GetComponent<UITextLocalization>().LocalizeTextText();
                    chatTimeList.Add("YESTERDAY");

                    if (pageNumber == 1 && SNS_APIResponseManager.Instance.r_isCreateMessage)
                    {
                        chatTimeObject2.transform.SetAsLastSibling();
                    }
                    else
                    {
                        chatTimeObject2.transform.SetAsFirstSibling();
                    }

                    //chatTimeObject2.transform.SetAsFirstSibling();
                }
                else
                {
                    if (chatTimeObject2 != null)
                    {
                        chatTimeObject2.transform.SetAsFirstSibling();
                    }
                }
            }
            else
            {
                string msgDateStr = converTime.Date.ToString("dd/MM/yyyy") + " " + UITextLocalization.GetLocaliseTextByKey(converTime.DayOfWeek.ToString());
                if (!chatTimeList.Contains(msgDateStr))
                {
                    //Debug.Log("DATE" + converTime.Date + "days" + converTime.DayOfWeek);
                    chatTimeObject3 = Instantiate(chatTimePrefab, SNS_SMSModuleManager.Instance.chatTimeParent);
                    chatTimeObject3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = msgDateStr;
                    chatTimeList.Add(msgDateStr);

                    if (pageNumber == 1 && SNS_APIResponseManager.Instance.r_isCreateMessage)
                    {
                        chatTimeObject3.transform.SetAsLastSibling();
                    }
                    else
                    {
                        chatTimeObject3.transform.SetAsFirstSibling();
                    }

                    //chatTimeObject3.transform.SetAsFirstSibling();
                }
                else
                {
                    if (chatTimeObject3 != null)
                    {
                        chatTimeObject3.transform.SetAsFirstSibling();
                    }
                }
            }
        }
    }

    //this method is used to instantiate chat attachments message.......
    public void GetAllAttachments(int index)
    {
        if (index == 0)
        {
            //Debug.Log("herwer");
            foreach (Transform item in SNS_SMSModuleManager.Instance.chatShareAttechmentparent)
            {
                Destroy(item.gameObject);
            }

            if (SNS_APIResponseManager.Instance.AllChatAttachmentsRoot.data.rows.Count > 0)
            {
                SNS_SMSModuleManager.Instance.chatShareAttechmentparent.gameObject.SetActive(true);
            }
            else
            {
                SNS_SMSModuleManager.Instance.chatShareAttechmentparent.gameObject.SetActive(false);
            }

            /*SNS_SMSModuleManager.Instance.chatShareAttechmentPhotoPanel.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            SNS_SMSModuleManager.Instance.chatShareAttechmentMainPanel.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            SNS_SMSModuleManager.Instance.chatMemberParent.parent.parent.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            SNS_SMSModuleManager.Instance.chatShareAttechmentPanel.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;*/

            for (int i = 0; i < SNS_APIResponseManager.Instance.AllChatAttachmentsRoot.data.rows.Count; i++)
            {
                // Debug.Log("herwer");
                if (i < 4)
                {
                    GameObject attechmentObject = Instantiate(chatShareAttechmentPrefab, SNS_SMSModuleManager.Instance.chatShareAttechmentparent);
                    attechmentObject.GetComponent<AttechmentData>().attachmentsRow = SNS_APIResponseManager.Instance.AllChatAttachmentsRoot.data.rows[i];

                    attechmentObject.GetComponent<AttechmentData>().LoadData(false);
                }
            }
            SetChatMember();
        }
        else if (index == 1)
        {
            foreach (Transform item in SNS_SMSModuleManager.Instance.chooseAttechmentparent)
            {
                Destroy(item.gameObject);
            }

            for (int i = 0; i < SNS_APIResponseManager.Instance.AllChatAttachmentsRoot.data.rows.Count; i++)
            {
                GameObject chooseattechmentObject = Instantiate(chooseAttechmentprefab, SNS_SMSModuleManager.Instance.chooseAttechmentparent);
                chooseattechmentObject.GetComponent<AttechmentData>().attachmentsRow = SNS_APIResponseManager.Instance.AllChatAttachmentsRoot.data.rows[i];
                chooseattechmentObject.GetComponent<AttechmentData>().LoadData(true);
            }
            SNS_SMSModuleManager.Instance.chatShareAttechmentparent.gameObject.SetActive(true);
        }

        /* if (SNS_APIResponseManager.Instance.AllChatAttachmentsRoot.data.rows.Count > 0)
         {
             chatShareAttechmentparent.gameObject.SetActive(true);
         }
         else
         {
             chatShareAttechmentparent.gameObject.SetActive(false);
         }
         for (int i = 0; i < SNS_APIResponseManager.Instance.AllChatAttachmentsRoot.data.rows.Count; i++)
         {
             if (index == 0)
             {
                 if (i < 4)
                 {
                     GameObject attechmentObject = Instantiate(chatShareAttechmentPrefab, chatShareAttechmentparent);
                     attechmentObject.GetComponent<AttechmentData>().attachmentsRow = SNS_APIResponseManager.Instance.AllChatAttachmentsRoot.data.rows[i];

                     attechmentObject.GetComponent<AttechmentData>().LoadData();
                 }
             }
             else if (index == 1)
             {
                 GameObject chooseattechmentObject = Instantiate(chooseAttechmentprefab, chooseAttechmentparent);
                 chooseattechmentObject.GetComponent<AttechmentData>().attachmentsRow = SNS_APIResponseManager.Instance.AllChatAttachmentsRoot.data.rows[i];
                 chooseattechmentObject.GetComponent<AttechmentData>().LoadData();
             }
         }*/
        SNS_SMSModuleManager.Instance.NoAttechmentScreen.SetActive(false);

        SNS_SMSModuleManager.Instance.LoaderShow(false);//False api loader.

        // Invoke("SetDetailScreen", 0.03f);
    }

    //this method is used to instantiate group member in chat details screen.......
    public void SetChatMember()
    {
        //SNS_SMSModuleManager.Instance.chatShareAttechmentPhotoPanel.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        //SNS_SMSModuleManager.Instance.chatShareAttechmentMainPanel.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        //SNS_SMSModuleManager.Instance.chatMemberParent.parent.parent.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        //SNS_SMSModuleManager.Instance.chatShareAttechmentPanel.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
       Debug.Log("SetChatMember calling.......");
        foreach (Transform item in SNS_SMSModuleManager.Instance.chatMemberParent)
        {
            Destroy(item.gameObject);
        }

        if (SNS_SMSModuleManager.Instance.allChatGetConversationDatum != null)
        {
            if (SNS_SMSModuleManager.Instance.allChatGetConversationDatum.receivedGroupId != 0)
            {
                for (int i = 0; i < SNS_SMSModuleManager.Instance.allChatGetConversationDatum.group.groupUsers.Count; i++)
                {
                    if (SNS_SMSModuleManager.Instance.allChatGetConversationDatum.group.groupUsers[i].user.id != SNS_APIResponseManager.Instance.userId)
                    {
                        GameObject chatMemberObject = Instantiate(chatMemberPrefab, SNS_SMSModuleManager.Instance.chatMemberParent);
                        ChatMemberData chatMemberData = chatMemberObject.GetComponent<ChatMemberData>();
                        chatMemberData.chatGetConversationUser = SNS_SMSModuleManager.Instance.allChatGetConversationDatum.group.groupUsers[i];
                        chatMemberData.createdGroupId = SNS_SMSModuleManager.Instance.allChatGetConversationDatum.group.createdBy;
                        chatMemberData.LoadData(0);
                    }
                }
            }
            else
            {
                GameObject chatMemberObject = Instantiate(chatMemberPrefab, SNS_SMSModuleManager.Instance.chatMemberParent);
                ChatMemberData chatMemberData = chatMemberObject.GetComponent<ChatMemberData>();
                chatMemberData.allChatGetConversationDatum = SNS_SMSModuleManager.Instance.allChatGetConversationDatum;
                chatMemberData.createdGroupId = SNS_SMSModuleManager.Instance.allChatGetConversationDatum.group.createdBy;
                chatMemberData.LoadData(1);
            }
        }
        SNS_SMSModuleManager.Instance.chatShareAttechmentPanel.transform.parent.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;

        SNS_SMSModuleManager.Instance.OnClickChatDetailsScreenAllowNotification();
        SNS_SMSModuleManager.Instance.MessageDetailScreen.SetActive(true);
        SNS_SMSModuleManager.Instance.ChatScreen.SetActive(false);
        SNS_SMSModuleManager.Instance.MessageDetailsSceenLeaveChatActive();
        // Invoke("SetDetailScreen",5f);
        //StartCoroutine(SetDetailScreen());
        StartCoroutine(WaitToSetDetailsScreen());
    }

    public IEnumerator WaitToSetDetailsScreen()
    {
        SNS_SMSModuleManager.Instance.chatShareAttechmentPanel.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        //SNS_SMSModuleManager.Instance.chatShareAttechmentMainPanel.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        yield return new WaitForSeconds(0.15f);
        //SNS_SMSModuleManager.Instance.chatShareAttechmentMainPanel.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        SNS_SMSModuleManager.Instance.chatShareAttechmentPanel.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        SNS_SMSModuleManager.Instance.chatShareAttechmentPanel.GetComponent<ContentSizeFitter>().enabled = false;
        yield return new WaitForSeconds(0.05f);
        SNS_SMSModuleManager.Instance.chatShareAttechmentPanel.GetComponent<ContentSizeFitter>().enabled = true;
    }


    public IEnumerator SetDetailScreen()
    {
        yield return new WaitForSeconds(0.03f);
        SNS_SMSModuleManager.Instance.chatShareAttechmentPanel.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        SNS_SMSModuleManager.Instance.chatShareAttechmentPhotoPanel.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        //yield return new WaitForSeconds(0.01f);
        //SNS_SMSModuleManager.Instance.chatShareAttechmentMainPanel.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        yield return new WaitForSeconds(0.01f);
        SNS_SMSModuleManager.Instance.chatMemberParent.parent.parent.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        yield return new WaitForSeconds(0.01f);
        SNS_SMSModuleManager.Instance.chatShareAttechmentPanel.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    //this method is used to leave the chat callback.......
    public void LeaveTheChatCallBack(string groupId)
    {
        ChatGetConversationGroupUser etc = SNS_SMSModuleManager.Instance.allChatGetConversationDatum.group.groupUsers.Find((x) => x.userId == SNS_APIResponseManager.Instance.userId);

        SNS_SMSModuleManager.Instance.allChatGetConversationDatum = null;
        SNS_SMSModuleManager.Instance.currentConversationData = null;
        SNS_SMSModuleManager.Instance.MessageDetailScreen.SetActive(false);
        SNS_SMSModuleManager.Instance.OnClickMessageButton();//active message list screen and refreshing list api

        SNS_SMSModuleManager.Instance.isLeaveGroup = true;
        //after leave group then create leave user msg on this group.......
        SNS_APIResponseManager.Instance.r_isCreateMessage = true;
       Debug.Log("removed User Name:" + etc.user.name);
        string messageStr = etc.user.name + " Left";
        SNS_APIResponseManager.Instance.RequestChatCreateMessage(0, int.Parse(groupId), messageStr, "LeaveGroup", "");
    }
    #endregion

    #region Test Update Avatar
    public void OnClickTestAvatarUpdate(int maxSize)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize, false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

               Debug.Log("OnPickGroupAvatarFromGellery path: " + path);

                string[] pathArry = path.Split('/');

                //string fileName = pathArry[pathArry.Length - 1];
                string fileName = Path.GetFileName(path);
               Debug.Log("OnPickGroupAvatarFromGellery FileName: " + fileName);

                AWSDataHandler.Instance.PostAvatarObject(path, fileName, "UpdateUserAvatar");
            }
        });
        Debug.Log("Permission result: " + permission);
    }

    public void UpdateAvatarOnServer(string key, string callingFrom)
    {
       Debug.Log("test update avatr key:" + key);
        SNS_APIResponseManager.Instance.RequestUpdateUserAvatar(key, callingFrom);
    }
    #endregion
}

//this class are Global Veriable Store
public static class GlobalVeriableClass
{
    public static string callingScreen = "";
    public static string GettingBackFromScene = "";
}