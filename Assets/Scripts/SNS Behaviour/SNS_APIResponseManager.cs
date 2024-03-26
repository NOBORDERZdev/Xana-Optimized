using AdvancedInputFieldPlugin;
using Newtonsoft.Json;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static ServerSideUserDataHandler;

public class SNS_APIResponseManager : MonoBehaviour
{
    public static SNS_APIResponseManager Instance;

    [Header("Loagin Token Reference")]
    public string userAuthorizeToken;
    public int userId;
    public string userName;
    public bool isTestDefaultToken = false;

    [Space]
    [Header("Check For is login from same Id or Different Id")]
    public bool isLoginFromDifferentId;

    [Space]
    public bool r_isCreateMessage = false;

    [Space]
    [Header("For Feed Comment")]
    public int feedIdTemp;
    private string checkText = "Newest";
    private int commentPageCount = 1;
    private int commnetFeedPagesize = 50;
    private bool scrollToTop;
    public bool isCommentDataLoaded = false;
    private int BFCount = 0;
    private int maxBfCount = 10;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        //if (!isTestDefaultToken)
        //{
        //if (!string.IsNullOrEmpty(PlayerPrefs.GetString("LoginToken")) || !string.IsNullOrEmpty(PlayerPrefs.GetString("UserName")))
        //{
        userAuthorizeToken = ConstantsGod.AUTH_TOKEN;
        userId = int.Parse(PlayerPrefs.GetString("UserName"));
        //}
        //}
    }

    private void OnEnable()
    {
        Instance = this;
        Debug.Log("<color=red> SNS_APIResponseManager Start UserToken:" + ConstantsGod.AUTH_TOKEN + "    :userID:" + PlayerPrefs.GetString("UserName") + "</color>");
        if (!isTestDefaultToken)
        {
            if (userAuthorizeToken != ConstantsGod.AUTH_TOKEN)
            {
                isLoginFromDifferentId = true;
            }
            else
            {
                isLoginFromDifferentId = false;
            }

            userAuthorizeToken = ConstantsGod.AUTH_TOKEN;
            userId = int.Parse(PlayerPrefs.GetString("UserName"));
            userName = PlayerPrefs.GetString("PlayerName");
        }
        else
        {
            ConstantsGod.API_BASEURL = "https://api-test.xana.net";
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        /*if (Application.internetReachability == NetworkReachability.NotReachable) //rik
        {
            if (File.Exists(Application.persistentDataPath + "/FeedData.json"))
            {
                LoadJson();
            }

            if (File.Exists(Application.persistentDataPath + "/FeedFollowingData.json"))
            {
                LoadJsonFollowingFeed();
            }
        }
        else
        {
            //Debug.Log("dfdfsd");
            RequestGetAllUsersWithFeeds(1, 20);
            RequestGetFeedsByFollowingUser(1, 20);
        }*/
    }

    public void OnFeedAPiCalling(string callingFrom = "")
    {
        Debug.Log("OnFeedAPiCalling");
        RequestGetAllUsersWithFeeds(1, 10, callingFrom);

        if (followingTabCo != null)
        {
            StopCoroutine(followingTabCo);
        }
        followingTabCo = StartCoroutine(WaitToCallFollowingTabAPI(callingFrom));
    }

    Coroutine followingTabCo;
    IEnumerator WaitToCallFollowingTabAPI(string callingFrom)
    {
        yield return new WaitForSeconds(.5f);
        RequestGetFeedsByFollowingUser(1, 10, callingFrom);
    }
    public void LoadJson()
    {
        using (StreamReader r = new StreamReader(Application.persistentDataPath + "/FeedData.json"))
        {
            string json = r.ReadToEnd();
            //Debug.Log("json " + json);
            StartCoroutine(SaveAndLoadJson(json, 0, 1, ""));
            //  FeedsManager.Instance.isDataLoad = true;
        }
    }

    public void LoadJsonFollowingFeed()
    {
        using (StreamReader r = new StreamReader(Application.persistentDataPath + "/FeedFollowingData.json"))
        {
            string json = r.ReadToEnd();
            //Debug.Log("json " + json);
            StartCoroutine(SaveAndLoadJsonFollowingFeed(json, 0, 1, ""));
            //  FeedsManager.Instance.isDataLoad = true;
        }
    }

    #region HotAPI..........   
    Coroutine requestGetAllUsersWithFeedsCoroutine;
    public void RequestGetAllUsersWithFeeds(int pageNum, int pageSize, string callingFrom = "")
    {
        if (requestGetAllUsersWithFeedsCoroutine != null)
        {
            StopCoroutine(requestGetAllUsersWithFeedsCoroutine);
        }
        // FeedsManager.Instance.ApiLoaderScreen.SetActive(true);
        requestGetAllUsersWithFeedsCoroutine = StartCoroutine(IERequestGetAllUsersWithFeeds(pageNum, pageSize, callingFrom));
    }
    public IEnumerator IERequestGetAllUsersWithFeeds(int pageNum, int pageSize, string callingFrom)
    {
        //using (UnityWebRequest www = UnityWebRequest.Get((ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetAllUsersWithFeeds + "/" + pageNum + "/" + pageSize)))
        using (UnityWebRequest www = UnityWebRequest.Get((ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetHotFeeds + "/" + pageNum + "/" + pageSize)))
        {
            //Debug.Log(ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetAllUsersWithFeeds + "/" + pageNum + "/" + pageSize + "   :Token:" + userAuthorizeToken);
            Debug.Log(ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetHotFeeds + "/" + pageNum + "/" + pageSize + "   :Token:" + userAuthorizeToken);

            www.SetRequestHeader("Authorization", userAuthorizeToken);

            if (LoaderHandler.Instance != null)//main feed top loader start
            {
                LoaderHandler.Instance.isLoaderGetApiResponce = false;
            }

            www.SendWebRequest();

            while (!www.isDone)
            {
                yield return null;
            }

            if (LoaderHandler.Instance != null)//main feed top loader stop
            {
                LoaderHandler.Instance.isLoaderGetApiResponce = true;
            }

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                // OLD FEED UI
                //if (FeedsManager.Instance.allFeedMessageTextList[0].gameObject.activeSelf)
                //{
                //    FeedsManager.Instance.AllFeedScreenMessageTextActive(true, 0, UITextLocalization.GetLocaliseTextByKey("bad internet connection please try again"));
                //}
                //if (FeedsManager.Instance.allFeedMessageTextList[2].gameObject.activeSelf)
                //{
                //    FeedsManager.Instance.AllFeedScreenMessageTextActive(true, 2, UITextLocalization.GetLocaliseTextByKey("bad internet connection please try again"));
                //}
                // END OLD FEED UI
            }
            else
            {
                //Debug.Log("Form upload complete! IERequestGetAllUsersWithFeeds pageNum:" + pageNum + "   :pageSize:" + pageSize);
                string data = www.downloadHandler.text;
                //Debug.Log("IERequestGetAllUsersWithFeeds PageNum:" + pageNum + "    :PageSize:" + pageSize + "     :Data:" + data);
                // FeedsManager.Instance.ApiLoaderScreen.SetActive(false);

                StartCoroutine(SaveAndLoadJson(data, 1, pageNum, callingFrom));
                //LoaderHandler.Instance.isLoaderGetApiResponce = true;
                //FeedsManager.Instance.allFeedCurrentpage += 1;
            }
        }
    }
    Coroutine requestGetFeedsByFollowingUserCoroutine;
    public void RequestGetFeedsByFollowingUser(int pageNum, int pageSize, string callingFrom = "")
    {
        if (requestGetFeedsByFollowingUserCoroutine != null)
        {
            StopCoroutine(requestGetFeedsByFollowingUserCoroutine);
        }
        //FeedsManager.Instance.ApiLoaderScreen.SetActive(true);
        requestGetFeedsByFollowingUserCoroutine = StartCoroutine(IERequestGetFeedsByFollowingUser(pageNum, pageSize, callingFrom));
    }
    public IEnumerator IERequestGetFeedsByFollowingUser(int pageNum, int pageSize, string callingFrom)
    {
        //yield return new WaitForSeconds(0.5f);
        using (UnityWebRequest www = UnityWebRequest.Get((ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetFeedsByFollowingUser + "/" + pageNum + "/" + pageSize)))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            if (LoaderHandler.Instance != null)//main feed top loader start
            {
                LoaderHandler.Instance.isLoaderGetApiResponce = false;
            }

            www.SendWebRequest();

            while (!www.isDone)
            {
                yield return null;
            }

            if (LoaderHandler.Instance != null)//main feed top loader stop
            {
                LoaderHandler.Instance.isLoaderGetApiResponce = true;
            }

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);

                // OLD FEED UI
                //if (FeedsManager.Instance.allFeedMessageTextList[1].gameObject.activeSelf)
                //{
                //    FeedsManager.Instance.AllFeedScreenMessageTextActive(true, 1, UITextLocalization.GetLocaliseTextByKey("bad internet connection please try again"));
                //}
                // END OLD FEED UI
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.Log("Form upload complete! IERequestGetFeedsByFollowingUser pageNum:" + pageNum + "   :pageSize:" + pageSize + " :Data:" + data);
                //FeedsManager.Instance.ApiLoaderScreen.SetActive(false);
                //  followingUserRoot = JsonConvert.DeserializeObject<FeedsByFollowingUserRoot>(data);
                // SNS_APIController.Instance.OnGetAllFeedForFollowingTab();

                StartCoroutine(SaveAndLoadJsonFollowingFeed(data, 1, pageNum, callingFrom));
                //LoaderHandler.Instance.isLoaderGetApiResponce = true;
                //FeedsManager.Instance.followingUserCurrentpage += 1;
            }
        }
    }
    public string HotdataStr;
    public IEnumerator SaveAndLoadJson(string data, int caller, int pageNum, string callingFrom)
    {
        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };
        //root = JsonConvert.DeserializeObject<AllUserWithFeedRoot>(data, settings);
        HotdataStr = data;
        hotFeedRoot = JsonConvert.DeserializeObject<HotFeedRoot>(data, settings);
        yield return new WaitForSeconds(.5f);
        for (int i = 0; i < hotFeedRoot.data.rows.Count; i++)
        {
            if (!SNS_APIController.Instance.feedHotIdList.Contains(hotFeedRoot.data.rows[i].id))
            {
                //SNS_APIController.Instance.feedHotIdList.Add(hotFeedRoot.data.rows[i].id);
                allhotFeedRoot.data.rows.Add(hotFeedRoot.data.rows[i]);
            }
        }
        //Debug.Log("root data count:" + root.data.rows.Count + "    :Caller:"+caller);
        /*if (caller == 0)
        {
            for (int i = 0; i < root.data.rows.Count; i++)
            {
                if (root.data.rows[i].feedCount != 0)
                {
                    allUserRootList.Add(root.data.rows[i]);
                }
            }
        }
        else
        {
            for (int i = 0; i < root.data.rows.Count; i++)
            {
                if (root.data.rows[i].feedCount != 0)
                {
                    List<AllUserWithFeedRow> matches = allUserRootList.Where(p => p.id == root.data.rows[i].id).ToList();
                    for (int k = 0; k < matches.Count; k++)
                    {
                        allUserRootList.Remove(matches[k]);
                        hotSaveRootList.Remove(matches[k]);
                    }

                    allUserRootList.Add(root.data.rows[i]);
                    hotSaveRootList.Add(root.data.rows[i]);
                    if (hotSaveRootList.Count > 20)
                    {
                        hotSaveRootList.Remove(hotSaveRootList[0]);
                    }
                }
            }
            //hotSavejsonList.data = root.data;
            hotSavejsonList.data.rows = hotSaveRootList;
            hotSavejsonList.success = root.success;
        }*/

        yield return new WaitForSeconds(0.1f);

        ///////////////////////////////////////////////
        SNS_APIController.Instance.AllUsersWithHotFeeds(callingFrom);
        //Debug.Log("Feed Load");
        FeedsManager.Instance.myPostCurrentPage += 1;
        RequestGetFeedsByUserId(userId, (FeedsManager.Instance.myPostCurrentPage), 10, "FeedPage");
        yield return new WaitForSeconds(1f);
        MyProfileManager.Instance.AllFeedWithUserId((FeedsManager.Instance.myPostCurrentPage), FeedsManager.Instance.forYouFeedTabContainer);
        //SNS_APIController.Instance.AllUserForYouFeeds(pageNum, callingFrom);
        StartCoroutine(SNS_APIController.Instance.HotWaitToEnableDataLoadedBool(pageNum));
        //Riken
        /* if (hotSaveRootList.Count != 0)
         {
             string feedData = JsonUtility.ToJson(hotSavejsonList);
             File.WriteAllText(Application.persistentDataPath + "/FeedData.json", feedData);
             //Debug.Log("path " + Application.persistentDataPath + "/FeedData.json");
             //Debug.Log("json  " + feedData);
         }*/
    }

    public void LoadMyPost()
    {
        StartCoroutine(IELoadMyPost());
    }

    public IEnumerator IELoadMyPost()
    {
        RequestGetFeedsByUserId(userId, (1), 10, "FeedPage");
        yield return new WaitForSeconds(1f);
        MyProfileManager.Instance.AllFeedWithUserId((1), FeedsManager.Instance.forYouFeedTabContainer, true);
        yield return new WaitForSeconds(.5f);
        SNS_APIResponseManager.Instance.RequestGetAllUsersWithFeeds(1, 10, "PullRefresh");
    }
    public void HotAndDiscoverSaveAndUpdateJson(int feedId, int index)
    {
        AllUserWithFeedRow allUserWithFeedRow = allUserRootList[index];
        bool isFindSuccess = false;
        //Debug.Log("HotAndDiscoverSaveAndUpdateJson:" + allUserWithFeedRow.id + "   :feedId:" + feedId);
        if (allUserWithFeedRow.id == feedId)
        {
            isFindSuccess = true;
        }
        else
        {
            AllUserWithFeedRow allUserWithFeedRow1 = allUserRootList.Find((x) => x.id == feedId);

            if (allUserWithFeedRow1 != null)
            {
                isFindSuccess = true;
                allUserWithFeedRow = allUserWithFeedRow1;
            }
        }

        //Debug.Log("Find Success:" + isFindSuccess + "   :id:" + allUserWithFeedRow.id);
        if (isFindSuccess)
        {
            if (hotSaveRootList.Contains(allUserWithFeedRow))
            {
                hotSaveRootList.Remove(allUserWithFeedRow);

                hotSavejsonList.data.rows = hotSaveRootList;
                hotSavejsonList.success = root.success;

                if (hotSaveRootList.Count != 0)
                {
                    string feedData = JsonUtility.ToJson(hotSavejsonList);
                    File.WriteAllText(Application.persistentDataPath + "/FeedData.json", feedData);
                    //Debug.Log("path " + Application.persistentDataPath + "/FeedData.json");
                }
            }
            allUserRootList.Remove(allUserWithFeedRow);
        }
    }

    public IEnumerator SaveAndLoadJsonFollowingFeed(string data, int caller, int pageNum, string callingFrom)
    {
        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };
        followingUserRoot = JsonConvert.DeserializeObject<FeedsByFollowingUserRoot>(data, settings);
        //Debug.Log("following user root data count:" + followingUserRoot.Data.Rows.Count + "    :Caller:" + caller);
        if (caller == 0)
        {
            for (int i = 0; i < followingUserRoot.Data.Rows.Count; i++)
            {
                allFollowingUserRootList.Add(followingUserRoot.Data.Rows[i]);
            }
        }
        else
        {
            for (int i = 0; i < followingUserRoot.Data.Rows.Count; i++)
            {
                // Debug.Log("id :" + followingUserRootList[i].Id + "DataId" + followingUserRoot.Data.Rows[i].Id);
                List<FeedsByFollowingUserRow> matches = allFollowingUserRootList.Where(p => p.Id == followingUserRoot.Data.Rows[i].Id).ToList();
                //  Debug.Log("matches" + matches.Count);
                for (int k = 0; k < matches.Count; k++)
                {
                    allFollowingUserRootList.Remove(matches[k]);
                    followingUserTabSaveRootList.Remove(matches[k]);
                }

                allFollowingUserRootList.Add(followingUserRoot.Data.Rows[i]);
                followingUserTabSaveRootList.Add(followingUserRoot.Data.Rows[i]);

                if (followingUserTabSaveRootList.Count > 20)
                {
                    followingUserTabSaveRootList.Remove(followingUserTabSaveRootList[0]);
                }
            }
            //followingUserTabSavejsonList.Data = followingUserRoot.Data;
            followingUserTabSavejsonList.Data.Rows = followingUserTabSaveRootList;
            followingUserTabSavejsonList.Success = followingUserRoot.Success;
            followingUserTabSavejsonList.Data.Count = followingUserRoot.Data.Count;
        }
        yield return new WaitForSeconds(0.1f);

        SNS_APIController.Instance.OnGetAllFeedForFollowingTab(pageNum, callingFrom);

        if (followingUserTabSaveRootList.Count != 0)
        {
            string feedData = JsonUtility.ToJson(followingUserTabSavejsonList);
            File.WriteAllText(Application.persistentDataPath + "/FeedFollowingData.json", feedData);
            //Debug.Log("path " + Application.persistentDataPath + "/FeedFollowingData.json");
            //Debug.Log("json  " + feedData);
        }
    }

    public void FeedFollowingSaveAndUpdateJson(List<int> unFollowingUserList, string callingFrom = "")
    {
        for (int i = 0; i < unFollowingUserList.Count; i++)
        {
            //Debug.Log("UmFollow Id:" + unFollowingUserList[i]);
            List<FeedsByFollowingUserRow> matches = allFollowingUserRootList.Where(p => p.CreatedBy == unFollowingUserList[i]).ToList();
            //Debug.Log("matches" + matches.Count);
            for (int k = 0; k < matches.Count; k++)
            {
                SNS_APIController.Instance.RemoveFollowingItemAndResetData(matches[k].Id);
                allFollowingUserRootList.Remove(matches[k]);
                followingUserTabSaveRootList.Remove(matches[k]);
            }
            followingUserRoot.Data.Count -= matches.Count;
        }
        //followingUserTabSavejsonList.Data = followingUserRoot.Data;
        followingUserTabSavejsonList.Data.Rows = followingUserTabSaveRootList;
        followingUserTabSavejsonList.Success = followingUserRoot.Success;
        followingUserTabSavejsonList.Data.Count = followingUserRoot.Data.Count;

        if (followingUserTabSaveRootList.Count != 0)
        {
            string feedData = JsonUtility.ToJson(followingUserTabSavejsonList);
            File.WriteAllText(Application.persistentDataPath + "/FeedFollowingData.json", feedData);
            //Debug.Log("path " + Application.persistentDataPath + "/FeedFollowingData.json");
            //Debug.Log("json  " + feedData);
        }

        FeedsManager.Instance.unFollowedUserListForFollowingTab.Clear();//clear 

        Resources.UnloadUnusedAssets();
        //Caching.ClearCache();
        //GC.Collect();
        if (FeedsManager.Instance.followingFeedTabContainer.childCount <= 0)
        {
            //Debug.Log("RequestGetFeedsByFollowingUser.......");
            RequestGetFeedsByFollowingUser(1, 10);
        }
        switch (callingFrom)
        {
            case "FollowingTabScreen":
                //if User unfollow user from other user profile screen comes from following tab then refresh hot tab api.......
                RequestGetAllUsersWithFeeds(1, 10, "PullRefresh");
                break;
            default:
                break;
        }
    }

    //this api is used to get feed for single user.......
    public void RequestGetFeedsByUserId(int userId, int pageNum, int pageSize, string callingFrom, bool _callFromFindFriendWithName = false)
    {
        StartCoroutine(IERequestGetFeedsByUserId(userId, pageNum, pageSize, callingFrom, _callFromFindFriendWithName));
    }
    public IEnumerator IERequestGetFeedsByUserId(int userId, int pageNum, int pageSize, string callingFrom, bool _callFromFindFriendWithName = false)
    {
        #region Old Picture and video type feed fetching code
        //////////////////////Old Picture and video type feed fetching code
        //using (UnityWebRequest www = UnityWebRequest.Get((ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetFeedsByUserId + "/" + userId + "/" + pageNum + "/" + pageSize)))
        //{
        //    www.SetRequestHeader("Authorization", userAuthorizeToken);

        //    www.SendWebRequest();
        //    while (!www.isDone)
        //    {
        //        yield return null;
        //    }

        //    if (www.isNetworkError || www.isHttpError)
        //    {
        //        Debug.Log(www.error);
        //        FeedsManager.Instance.ShowLoader(false);

        //        switch (callingFrom)
        //        {
        //            case "OtherPlayerFeed":
        //                if (OtherUserProfileManager.Instance != null && pageNum == 1)
        //                {
        //                    OtherUserProfileManager.Instance.RemoveAndCheckBackKey();
        //                }
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        string data = www.downloadHandler.text;
        //        //Debug.Log("IERequestGetFeedsByUserId success data" + data);
        //        var settings = new JsonSerializerSettings
        //        {
        //            NullValueHandling = NullValueHandling.Ignore,
        //            MissingMemberHandling = MissingMemberHandling.Ignore
        //        };
        //        AllFeedByUserIdRoot test = JsonConvert.DeserializeObject<AllFeedByUserIdRoot>(data, settings);
        //        if (allFeedWithUserIdRoot.Data.Rows.Count > test.Data.Rows.Count)
        //        {
        //            //below line of clearing was commented earlier by riken but uncommented now after start of profile 2.0 as it is working fine for me ----- UMER
        //            allFeedWithUserIdRoot.Data.Rows.Clear();

        //            for (int i = 0; i < test.Data.Rows.Count; i++)
        //            {
        //                // myList.Where(p => p.Name == nameToExtract);
        //                // allFeedWithUserIdRoot.Data.Rows.Where(p => p.Id == test.Data.Rows[i].Id);

        //                if (!allFeedWithUserIdRoot.Data.Rows.Any(x => x.Id == test.Data.Rows[i].Id))
        //                {
        //                    allFeedWithUserIdRoot.Data.Rows.Add(test.Data.Rows[i]);
        //                }
        //                //    if (!allFeedWithUserIdRoot.Data.Rows.Contains(test.Data.Rows[i]))
        //                //{
        //                //}
        //            }
        //        }
        //        else
        //        {

        //            allFeedWithUserIdRoot = test;
        //        }
        //        if (callingFrom == "OtherPlayerFeed")
        //        {
        //            allFeedWithUserIdRoot = test;
        //        }
        //        switch (callingFrom)
        //        {
        //            case "OtherPlayerFeed":
        //                OtherUserProfileManager.Instance.AllFeedWithUserId(pageNum);
        //                break;
        //            case "MyProfile":
        //                MyProfileManager.Instance.AllFeedWithUserId(pageNum);
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //}

        #endregion
        //////////////////////New text post type feed fetching code
        using (UnityWebRequest www = UnityWebRequest.Get((ConstantsGod.API_BASEURL + ConstantsGod.GetUserAllTextPosts + userId + "/" + pageNum + "/" + pageSize)))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);
            // Start the stopwatch
            //Stopwatch stopwatch = Stopwatch.StartNew();
            www.SendWebRequest();
            while (!www.isDone)
            {
                yield return null;
            }
            //// stop the stopwatch
            //stopwatch.stop();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                FeedsManager.Instance.ShowLoader(false);

                switch (callingFrom)
                {
                    case "OtherPlayerFeed":
                        if (OtherUserProfileManager.Instance != null && pageNum == 1)
                        {
                            OtherUserProfileManager.Instance.RemoveAndCheckBackKey();
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                //// Print the elapsed time
                //UnityEngine.Debug.Log("User Posts data Request completed in: " + stopwatch.ElapsedMilliseconds + " milliseconds");
                string data = www.downloadHandler.text;
                Debug.Log("IERequestGetFeedsByUserId success data" + data);
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                AllTextPostByUserIdRoot test = JsonConvert.DeserializeObject<AllTextPostByUserIdRoot>(data, settings);
                if (callingFrom == "MyProfile")
                {
                    MyProfileManager.Instance.totalPostText.text = test.data.Count.ToString();
                    allTextPostWithUserIdRoot.data.rows.Clear();
                }
                else
                {
                    OtherUserProfileManager.Instance.textPlayerTottlePost.text = test.data.Count.ToString();
                }
                //FeedResponse test = JsonConvert.DeserializeObject<FeedResponse>(data, settings);
                if (allTextPostWithUserIdRoot.data.rows.Count >= test.data.rows.Count)
                {
                    //below line of clearing was commented earlier by riken but uncommented now after start of profile 2.0 as it is working fine for me ----- UMER
                    allTextPostWithUserIdRoot.data.rows.Clear();

                    for (int i = 0; i < test.data.rows.Count; i++)
                    {
                        // myList.Where(p => p.Name == nameToExtract);
                        // allFeedWithUserIdRoot.Data.Rows.Where(p => p.Id == test.Data.Rows[i].Id);

                        if (!allTextPostWithUserIdRoot.data.rows.Any(x => x.id == test.data.rows[i].id))
                        {
                            allTextPostWithUserIdRoot.data.rows.Add(test.data.rows[i]);
                        }
                        //    if (!allFeedWithUserIdRoot.Data.Rows.Contains(test.Data.Rows[i]))
                        //{
                        //}
                    }
                }
                else
                {
                    if (allTextPostWithUserIdRoot.data.rows.Count > 0)
                    {
                        allTextPostWithUserIdRoot.data.rows.Clear();
                    }

                    allTextPostWithUserIdRoot = test;
                }
                if (callingFrom == "OtherPlayerFeed")
                {
                    allTextPostWithUserIdRoot = test;
                }
                switch (callingFrom)
                {
                    case "OtherPlayerFeed":
                        OtherUserProfileManager.Instance.AllFeedWithUserId(pageNum, _callFromFindFriendWithName);
                        break;
                    case "MyProfile":
                        MyProfileManager.Instance.AllFeedWithUserId(pageNum);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    //this api is used to get tagged feed for user.......
    public void RequesturlGetTaggedFeedsByUserId(int userId, int pageNum, int pageSize)
    {
        //  FeedsManager.Instance.ApiLoaderScreen.SetActive(true);
        StartCoroutine(IERequestGetTaggedFeedsByUserId(userId, pageNum, pageSize));
    }
    public IEnumerator IERequestGetTaggedFeedsByUserId(int userId, int pageNum, int pageSize)
    {
        using (UnityWebRequest www = UnityWebRequest.Get((ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetTaggedFeedsByUserId + "/" + userId + "/" + pageNum + "/" + pageSize)))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log("Form upload complete!");
                string data = www.downloadHandler.text;
                //  FeedsManager.Instance.ApiLoaderScreen.SetActive(false);
                Debug.Log("taggedFeedsByUserIdRoot" + data);
                taggedFeedsByUserIdRoot = JsonConvert.DeserializeObject<TaggedFeedsByUserIdRoot>(data);
                StartCoroutine(OtherUserProfileManager.Instance.AllTagFeed());
                // Debug.Log(root.data.count);
            }
        }
    }
    #endregion

    #region Follower And Following.......
    //this api is used to get all following user.......
    public void RequestGetAllFollowing(int pageNum, int pageSize, string getFollowingFor)
    {
        StartCoroutine(IERequestGetAllFollowing(pageNum, pageSize, getFollowingFor));
    }
    public IEnumerator IERequestGetAllFollowing(int pageNum, int pageSize, string getFollowingFor)
    {
        using (UnityWebRequest www = UnityWebRequest.Get((ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetAllFollowing + "/" + pageNum + "/" + pageSize)))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            switch (getFollowingFor)
            {
                case "Message"://chat scene loader false
                    SNS_SMSModuleManager.Instance.LoaderShow(false);//False api loader.
                    break;
                default:
                    break;
            }

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log("Form upload complete!");
                string data = www.downloadHandler.text;
                Debug.Log("GetAllFollowing Data" + data);
                allFollowingRoot = JsonConvert.DeserializeObject<AllFollowingRoot>(data);

                switch (getFollowingFor)
                {
                    case "Message":
                        SNS_APIController.Instance.GetAllFollowingUser(pageNum);
                        break;
                    default:
                        break;
                }
                // Debug.Log(root.data.count);
            }
        }
    }

    public void SetAdFrndFollowing()
    {
        if (FeedsManager.Instance != null)
        {
            FeedsManager.Instance.ShowLoader(true);
        }
        StartCoroutine(IEAdFrndAllFollowing(1, 100));
    }

    public AllFollowingRoot adFrndFollowing;
    public IEnumerator IEAdFrndAllFollowing(int pageNum, int pageSize)
    {
        string uri = ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetAllFollowing + userId + "/" + pageNum + "/" + pageSize;
        print("uri" + uri);
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            print("Authorization" + userAuthorizeToken);
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                if (FeedsManager.Instance != null)
                {
                    FeedsManager.Instance.ShowLoader(false);
                }
                Debug.Log(www.error);
            }
            else
            {
                if (FeedsManager.Instance != null)
                {
                    FeedsManager.Instance.ShowLoader(false);
                }
                //Debug.Log("Form upload complete!");
                string data = www.downloadHandler.text;
                Debug.Log("GetAllFollowing Data" + data);
                //adFrndFollowing = JsonConvert.DeserializeObject<AllFollowingRoot>(data);
                adFrndFollowing = JsonUtility.FromJson<AllFollowingRoot>(data);
                SNS_APIController.Instance.SpwanAdFrndFollowing();
                GetBestFriend();

                //switch (getFollowingFor)
                //{
                //    case "Message":
                //        SNS_APIController.Instance.GetAllFollowingUser(pageNum);
                //        break;
                //    default:
                //        break;
                //}
                // Debug.Log(root.data.count);
            }
        }
    }

    //this api is used to get all followers.......
    public void RequestGetAllFollowers(int pageNum, int pageSize, string callingFrom)
    {
        StartCoroutine(IERequestGetAllFollowers(pageNum, pageSize, callingFrom));
    }
    public IEnumerator IERequestGetAllFollowers(int pageNum, int pageSize, string callingFrom)
    {
        using (UnityWebRequest www = UnityWebRequest.Get((ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetAllFollowers + "/" + pageNum + "/" + pageSize)))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log("Get Follower Success!");
                string data = www.downloadHandler.text;
                Debug.Log("<color = red> GetAllFollowers Data" + data + "</color>");
                AllFollowerRoot = JsonUtility.FromJson<AllFollowersRoot>(data);

                switch (callingFrom)
                {
                    case "FeedStart":
                        SNS_APIController.Instance.GetSetAllfollowerInTopStoryPanelUser();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    //this api is used to follow user.......
    public void RequestFollowAUser(string user_Id, string callingFrom)
    {
        //Debug.Log("RequestFollowAUser:" + user_Id + "    :Calling From:" + callingFrom);
        StartCoroutine(IERequestFollowAUser(user_Id, callingFrom));
    }
    public IEnumerator IERequestFollowAUser(string user_Id, string callingFrom)
    {
        WWWForm form = new WWWForm();
        form.AddField("userId", user_Id);

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_FollowAUser), form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                if (FeedsManager.Instance != null)
                {
                    FeedsManager.Instance.ShowLoader(false);
                }
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.Log("Follow User data:" + data + "   :Follow User ID:" + user_Id + ":CallingFrom:" + callingFrom);
                switch (callingFrom)
                {
                    case "OtherUserProfile":
                        FeedsManager.Instance.ShowLoader(false);
                        //OtherUserProfileManager.Instance.OnSetUserUi(true);
                        OtherUserProfileManager.Instance.OnFollowerIncreaseOrDecrease(true);//Inscrease follower count.......
                        //OtherUserProfileManager.Instance.DestroyUserFromHotTabAfterFollow();

                        FeedsManager.Instance.FollowingAddAndRemoveUnFollowedUser(int.Parse(user_Id), false);
                        break;
                    case "Feed":
                        if (FeedsManager.Instance != null)
                        {
                            StartCoroutine(WaitToFalseLoader());
                        }

                        SNS_APIController.Instance.currentFeedRawItemController.OnFollowUserSuccessful();
                        SNS_APIController.Instance.currentFeedRawItemController.isFollow = true;
                        SNS_APIController.Instance.currentFeedRawItemController = null;
                        //OtherUserProfileManager.Instance.OnSetUserUi(SNS_APIController.Instance.currentFeedRawItemController.isFollow);                        
                        break;
                    default:
                        break;
                }
            }
        }
    }

    //this api is used to un follow user.......
    public void RequestUnFollowAUser(string user_Id, string callingFrom)
    {
        StartCoroutine(IERequestUnFollowAUser(user_Id, callingFrom));
    }
    public IEnumerator IERequestUnFollowAUser(string user_Id, string callingFrom)
    {
        WWWForm form = new WWWForm();
        form.AddField("userId", user_Id);

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_UnFollowAUser), form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                if (FeedsManager.Instance != null)
                {
                    FeedsManager.Instance.ShowLoader(false);
                }
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.Log("Un Follow a user data:" + data + "  :user id:" + user_Id + "   :CallingFrom:" + callingFrom);
                switch (callingFrom)
                {
                    case "OtherUserProfile":
                        FeedsManager.Instance.ShowLoader(false);
                        //OtherUserProfileManager.Instance.OnSetUserUi(false);
                        OtherUserProfileManager.Instance.OnFollowerIncreaseOrDecrease(false);//Descrease follower count.......

                        FeedsManager.Instance.FollowingAddAndRemoveUnFollowedUser(int.Parse(user_Id), true);
                        break;
                    case "Feed":
                        if (FeedsManager.Instance != null)
                        {
                            StartCoroutine(WaitToFalseLoader());
                        }

                        SNS_APIController.Instance.currentFeedRawItemController.OnFollowUserSuccessful();
                        //OtherUserProfileManager.Instance.OnSetUserUi(SNS_APIController.Instance.currentFeedRawItemController.isFollow);                        
                        break;
                    default:
                        break;
                }
            }
        }
    }

    IEnumerator WaitToFalseLoader()
    {
        yield return new WaitForSeconds(1.7f);
        FeedsManager.Instance.ShowLoader(false);
    }

    //this api is used to make favourite follower.......
    public void RequestMakeFavouriteFollower(string user_Id)
    {
        StartCoroutine(IERequestMakeFavouriteFollower(user_Id));
    }
    public IEnumerator IERequestMakeFavouriteFollower(string user_Id)
    {
        WWWForm form = new WWWForm();
        form.AddField("userId", user_Id);

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_MakeFavouriteFollower), form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.Log("MakeFavouriteFollower data:" + data);
                // root = JsonUtility.FromJson<MakeAllFavouriteFollowerRoot>(data);
            }
        }
    }
    #endregion

    #region Profile Follower Following list APIs
    //this api is used to get all follower user for this player.......
    public void RequestGetAllFollowersFromProfile(string user_Id, int pageNum, int pageSize)
    {
        StartCoroutine(IERequestGetAllFollowersFromProfile(user_Id, pageNum, pageSize));
    }
    public IEnumerator IERequestGetAllFollowersFromProfile(string user_Id, int pageNum, int pageSize)
    {
        using (UnityWebRequest www = UnityWebRequest.Get((ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetAllFollowers + "/" + user_Id + "/" + pageNum + "/" + pageSize)))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            //FeedsManager.Instance.ShowLoader(false);

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.Log("<color = red> GetAllFollowersFromProfile data:" + data + "</color>");
                profileAllFollowerRoot = JsonUtility.FromJson<AllFollowersRoot>(data);

                FeedsManager.Instance.ProfileGetAllFollower(pageNum);
            }
        }
    }

    //this api is used to get all following user for this player.......
    public void RequestGetAllFollowingFromProfile(string user_Id, int pageNum, int pageSize)
    {
        StartCoroutine(IERequestGetAllFollowingFromProfile(user_Id, pageNum, pageSize));
    }
    public IEnumerator IERequestGetAllFollowingFromProfile(string user_Id, int pageNum, int pageSize)
    {
        using (UnityWebRequest www = UnityWebRequest.Get((ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetAllFollowing /*+ "/"*/ + user_Id + "/" + pageNum + "/" + pageSize)))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            //FeedsManager.Instance.ShowLoader(false);

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.Log("<color = red> GetAllFollowingFromProfile data:" + data + "</color>");
                //profileAllFollowingRoot = JsonConvert.DeserializeObject<AllFollowingRoot>(data);
                //FeedsManager.Instance.ProfileGetAllFollowing(pageNum);
                profileAllFollowingRoot = JsonUtility.FromJson<AllFollowingRoot>(data);
                SNS_APIController.Instance.SpwanProfileFollowing();
            }
        }
    }


    //public void AdFrndFollowingFetch(){ 
    //   foreach (Transform item in FeedsManager.Instance.AddFriendPanelFollowingCont.transform)
    //   {
    //        Destroy(item.gameObject);
    //   }

    //}

    // public IEnumerator IEAdFrndFollowingUser(string user_Id, int pageNum, int pageSize)
    //{
    //    using (UnityWebRequest www = UnityWebRequest.Get((ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetAllFollowing + "/" + user_Id + "/" + pageNum + "/" + pageSize)))
    //    {
    //        www.SetRequestHeader("Authorization", userAuthorizeToken);

    //        yield return www.SendWebRequest();

    //        //FeedsManager.Instance.ShowLoader(false);

    //        if (www.isNetworkError || www.isHttpError)
    //        {
    //            Debug.Log(www.error);
    //        }
    //        else
    //        {
    //            string data = www.downloadHandler.text;
    //            Debug.Log("<color = red> GetAllFollowingFromProfile data:" + data + "</color>");
    //            AdFrndFollowingRoot  = JsonConvert.DeserializeObject<AllFollowingRoot>(data);
    //            FeedsManager.Instance.AdFrndGetAllFollowing(pageNum);
    //        }
    //    }
    //}
    #endregion

    #region Feed Comment.......
    int lastCommentTotalCount;
    //this method is used to Comment button click and get comment list for current feed.......
    public void CommentListGetAndClickFeedCommentButton(int currentId, bool isRefresh, int commentCount)
    {
        Debug.Log("CommentListGetAndClickFeedCommentButton CurrentId:" + currentId + "   :FeedIdTemp:" + feedIdTemp + "    :IsRefresh:" + isRefresh + "    :CommentCount:" + commentCount);

        if (!isRefresh && lastCommentTotalCount != commentCount)
        {
            isRefresh = true;
            //Debug.Log("CommentListGetAndClickFeedCommentButton1111111");
        }

        if (feedIdTemp != currentId || isRefresh)
        {
            isCommentDataLoaded = false;
            commentPageCount = 1;
            scrollToTop = false;
            Debug.Log("FeedIdTemp change: " + isRefresh + " not same :" + (feedIdTemp != currentId));
            feedIdTemp = currentId;
            RequestFeedCommentList(feedIdTemp, 1, 1, commnetFeedPagesize);
        }
    }

    //for Feed Commnet.......
    public void SendComment(InputField text)
    {
        RequestCommentFeed(feedIdTemp.ToString(), text.text.ToString());
    }

    public void OnClickSendCommentButton(AdvancedInputField advancedInputField)
    {
        //Debug.Log("On Send comment buttonClick");
        string message = advancedInputField.RichText;
        if (!string.IsNullOrEmpty(message))
        {
            advancedInputField.Clear();
            RequestCommentFeed(feedIdTemp.ToString(), message);
            advancedInputField.transform.GetChild(5).gameObject.GetComponent<InputFieldButton>().interactable = false;
        }
    }

    //this api is used to create comment for feed.......
    public void RequestCommentFeed(string feed_feedId, string feed_comment)
    {
        StartCoroutine(IERequestCommentFeed(feed_feedId, EncodedString(feed_comment)));
    }
    public IEnumerator IERequestCommentFeed(string feed_feedId, string feed_comment)
    {
        //Debug.Log("Feed Id:" + feed_feedId + "   :Feed_Comment:" + feed_comment);
        WWWForm form = new WWWForm();
        form.AddField("feedId", feed_feedId);
        form.AddField("comment", feed_comment);

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_CommentFeed), form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);
            Debug.Log("Comment API:" + ConstantsGod.API_BASEURL + ConstantsGod.r_url_CommentFeed);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.Log("<color = red> IERequestCommentFeed success data:" + data + "  :Feed Id:" + feed_feedId + "   :Feed_Comment:" + feed_comment + "</color>");

                //CommentPostDetail bean = Gods.DeserializeJSON<CommentPostDetail>(data.Trim());
                CommentPostDetail bean = JsonConvert.DeserializeObject<CommentPostDetail>(data);

                //if (!bean.Equals("") || !bean.Equals(null))
                if (bean.data != null)
                {
                    CommentCountTextSetup(bean.data.count);//set comment count on commet panel.......

                    if (bean.data.commentPost != null)
                    {
                        GameObject CommentObject = Instantiate(FeedsManager.Instance.commentListItemPrefab, FeedsManager.Instance.commentContentPanel.transform);

                        if (!checkText.Equals("Oldest"))
                        {
                            CommentObject.transform.SetAsFirstSibling();
                            FeedsManager.Instance.commentContentPanel.transform.GetChild(1).SetAsFirstSibling();
                        }

                        //FeedsManager.Instance.CommentCount.text = bean.data.count.ToString();

                        CommentRow commentRow = new CommentRow();
                        commentRow.id = bean.data.commentPost.id;
                        commentRow.feedId = bean.data.commentPost.feedId;
                        commentRow.comment = bean.data.commentPost.comment;
                        commentRow.createdBy = bean.data.commentPost.createdBy;
                        commentRow.createdAt = bean.data.commentPost.createdAt;
                        commentRow.updatedAt = bean.data.commentPost.updatedAt;
                        commentRow.user = bean.data.commentPost.user;


                        FeedCommentItemController feedCommentItemController = CommentObject.GetComponent<FeedCommentItemController>();
                        feedCommentItemController.SetupData(commentRow);

                        if (checkText.Equals("Oldest"))
                        {
                            FeedsManager.Instance.commentScrollPosition.verticalNormalizedPosition = 0f;
                        }
                        else if (checkText.Equals("Newest"))
                        {
                            FeedsManager.Instance.commentScrollPosition.verticalNormalizedPosition = 1f;
                        }

                        FeedsManager.Instance.CommentSuccessAfterUpdateRequireFeedResponse();
                    }
                }
            }
        }
    }

    public void ScrollToTop(ScrollRect scrollRect)
    {
        //Debug.Log("comment ScrollToTop:" + scrollRect.verticalNormalizedPosition);
        if (scrollRect.verticalNormalizedPosition <= 0f && isCommentDataLoaded)
        {
            if (commentFeedList.data.rows.Count > 0)
            {
                //Debug.Log("Comment pagination api call.......");
                isCommentDataLoaded = false;
                if (checkText.Equals("Oldest"))
                {
                    scrollToTop = true;
                    commentPageCount++;
                    RequestFeedCommentList(feedIdTemp, 2, commentPageCount, commnetFeedPagesize);
                }
                else if (checkText.Equals("Newest"))
                {
                    scrollToTop = true;
                    commentPageCount++;
                    RequestFeedCommentList(feedIdTemp, 1, commentPageCount, commnetFeedPagesize);
                }
            }
        }
    }

    public void resetObject()
    {
        if (FeedsManager.Instance.commentContentPanel.transform.childCount > 1)
        {
            foreach (Transform child in FeedsManager.Instance.commentContentPanel.transform)
            {
                if (!child.transform.name.Equals("HeaderCommentCount"))
                {
                    GameObject.Destroy(child.gameObject);
                }
                //Invoke("resetObject", 1f);
            }
            //Invoke("callobjects", 1f);
        }
    }

    public void dropdownFilterComment(string text)
    {
        scrollToTop = false;
        checkText = text.ToString();
        // fitertextDropdown.text = text;
        FeedsManager.Instance.commentFitertextDropdown.text = UITextLocalization.GetLocaliseTextByKey(text);

        if (checkText.Equals("Oldest"))
        {
            RequestFeedCommentList(feedIdTemp, 2, 1, commnetFeedPagesize);
        }
        else if (checkText.Equals("Newest"))
        {
            RequestFeedCommentList(feedIdTemp, 1, 1, commnetFeedPagesize);
        }
    }

    //this api is used to get comment list for feed.......
    public void RequestFeedCommentList(int feedId, int sortOrder, int pageNumber, int pageSize)
    {
        //Debug.Log("RequestFeedCommentList:" + feedId + "   :sortOrder:" + sortOrder + "    :pageNum:" + pageNumber + "   :PageSize:" + pageSize);
        StartCoroutine(IERequestFeedCommentList(feedId, sortOrder, pageNumber, pageSize));
    }

    public IEnumerator IERequestFeedCommentList(int feedId, int sortOrder, int pageNumber, int pageSize)
    {
        if (!scrollToTop)
        {
            while (FeedsManager.Instance.commentContentPanel.transform.childCount > 1)
            {
                resetObject();
                yield return null;
            }
        }

        //Debug.Log("feedid===" + feedId);
        using (UnityWebRequest www = UnityWebRequest.Get((ConstantsGod.API_BASEURL + ConstantsGod.r_url_FeedCommentList + "/" + feedId + "/" + sortOrder + "/" + pageNumber + "/" + pageSize)))
        {
            Debug.Log("CommentList API Request: " + ConstantsGod.API_BASEURL + ConstantsGod.r_url_FeedCommentList + "/" + feedId + "/" + sortOrder + "/" + pageNumber + "/" + pageSize);
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.Log("IERequestFeedCommentList success data:" + data);

                //commentFeedList = Gods.DeserializeJSON<CommentDetails>(data.Trim());
                commentFeedList = JsonConvert.DeserializeObject<CommentDetails>(data);

                //if (!commentFeedList.Equals("") || !commentFeedList.Equals(null))
                if (commentFeedList.data != null)
                {
                    //FeedsManager.Instance.CommentCount.text = commentFeedList.data.count.ToString();
                    CommentCountTextSetup(commentFeedList.data.count);//set comment count on commet panel.......

                    for (int i = 0; i < commentFeedList.data.rows.Count; i++)
                    {
                        GameObject CommentObject = Instantiate(FeedsManager.Instance.commentListItemPrefab, FeedsManager.Instance.commentContentPanel.transform);

                        FeedCommentItemController feedCommentItemController = CommentObject.GetComponent<FeedCommentItemController>();
                        feedCommentItemController.SetupData(commentFeedList.data.rows[i]);
                    }

                    if (commentDataLoadedCoroutine != null)//for comment data loaded.......
                    {
                        StopCoroutine(commentDataLoadedCoroutine);
                    }
                    commentDataLoadedCoroutine = StartCoroutine(waitToSetCommentDataLoaded());
                }
            }
        }
    }

    Coroutine commentDataLoadedCoroutine;
    IEnumerator waitToSetCommentDataLoaded()
    {
        yield return new WaitForSeconds(0.05f);
        isCommentDataLoaded = true;//this is used to comment data loaded.......
    }

    //this api is used to delete feed comment.......
    public void RequestDeleteComment(string feed_commentID, string feed_feedId)
    {
        StartCoroutine(IERequestDeleteComment(feed_commentID, feed_feedId));
    }
    public IEnumerator IERequestDeleteComment(string feed_commentID, string feed_feedId)
    {
        WWWForm form = new WWWForm();
        form.AddField("commentId", feed_commentID);
        form.AddField("feedId", feed_feedId);

        using (UnityWebRequest www = UnityWebRequest.Delete((ConstantsGod.API_BASEURL + ConstantsGod.r_url_DeleteComment)))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log("Form upload complete!");
                string data = www.downloadHandler.text;
                Debug.Log("DeleteComment data:" + data);
                // root = JsonUtility.FromJson<AllCommentFeedRoot>(data);
            }
        }
    }

    public void CommentCountTextSetup(int count)
    {
        lastCommentTotalCount = count;
        //Debug.Log("Comment Count:" + count);
        if (GameManager.currentLanguage == "ja" || LocalizationManager.forceJapanese)
        {
            FeedsManager.Instance.CommentCount.text = UITextLocalization.GetLocaliseTextByKey("Comments") + "<color=blue>" + count.ToString() + "</color>" + UITextLocalization.GetLocaliseTextByKey("s");
        }
        else
        {
            FeedsManager.Instance.CommentCount.text = "<color=blue>" + count.ToString() + "</color> " + UITextLocalization.GetLocaliseTextByKey("Comments");
        }
    }
    //End comment.......
    #endregion

    #region Feed............
    //this api is used to get all feed.......
    public void RequestGetAllFeed(int pageNum, int pageSize)
    {
        StartCoroutine(IERequestGetAllFeed(pageNum, pageSize));
    }
    public IEnumerator IERequestGetAllFeed(int pageNum, int pageSize)
    {
        using (UnityWebRequest www = UnityWebRequest.Get((ConstantsGod.API_BASEURL + ConstantsGod.r_url_AllFeed + "/" + pageNum + "/" + pageSize)))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log("Form upload complete!");
                string data = www.downloadHandler.text;
                Debug.Log(" <color = red> GetAllFeed data:" + data + "</color>");
                //   root = JsonUtility.FromJson<AllFeedRoot>(data);
                // Debug.Log(root.data.count);
            }
        }
    }

    //this api is used to create feed api.......
    public void RequestCreateFeed(string feed_title, string feed_descriptions, string feed_image, string feed_video, string thumbnail, string feed_isAllowComment, string feed_tagUserIds, string callingFrom)
    {
        StartCoroutine(IERequestCreateFeed(feed_title, feed_descriptions, feed_image, feed_video, thumbnail, feed_isAllowComment, feed_tagUserIds, callingFrom));
    }
    public IEnumerator IERequestCreateFeed(string feed_title, string feed_descriptions, string feed_image, string feed_video, string thumbnail, string feed_isAllowComment, string feed_tagUserIds, string callingFrom)
    {
        //Debug.Log("Create Feed API Calling from:" + callingFrom);
        WWWForm form = new WWWForm();
        form.AddField("title", feed_title);
        form.AddField("descriptions", feed_descriptions);
        form.AddField("image", feed_image);
        form.AddField("video", feed_video);
        form.AddField("thumbnail", thumbnail);
        form.AddField("isAllowComment", feed_isAllowComment);
        form.AddField("tagUserIds", feed_tagUserIds);

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_CreateFeed), form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();
            if (AWSDataHandler.Instance.currentSNSApiLoaderController != null)
            {
                AWSDataHandler.Instance.currentSNSApiLoaderController.ShowUploadStatusImage(false);
            }
            switch (callingFrom)
            {
                case "MyProfileCreateFeed":
                    if (FeedsManager.Instance != null)
                    {
                        FeedsManager.Instance.ShowLoader(false);//false api loader.......
                        FeedsManager.Instance.OnClickCreateFeedBackBtn(true);
                    }
                    break;
                case "RoomCreateFeed":
                    if (ARFaceModuleManager.Instance != null)//this condition disable loader of Room screen if avtive....... 
                    {
                        if (ARFaceModuleManager.Instance.apiLoaderController.mainLoaderObj.activeSelf)
                        {
                            ARFaceModuleManager.Instance.ShowLoader(false);
                        }
                    }
                    break;
                default:
                    break;
            }

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Debug.Log("Create Feed complete!");
                string data = www.downloadHandler.text;
                Debug.Log("CreateFeed data:" + data + "    :Calling from:" + callingFrom);
                // root = JsonUtility.FromJson<AllCreateFeedRoot>(data);
                switch (callingFrom)
                {
                    case "MyProfileCreateFeed":
                        if (MyProfileManager.Instance != null)
                        {
                            MyProfileManager.Instance.ProfileTabButtonClick();
                            Invoke(nameof(LoadMyPost), 0);
                            //RequestGetAllUsersWithFeeds(1, 10, "PullRefresh");
                        }
                        break;
                    case "RoomCreateFeed":
                        if (ARFaceModuleManager.Instance != null)
                        {
                            ARFaceModuleManager.Instance.CreateFeedSuccess();
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }

    //this api is used to delete feed.......
    public void RequestDeleteFeed(string feed_Id, string callingFrom)
    {
        SNSNotificationHandler.Instance.DeleteLoaderShow(true);//delete loader active

        StartCoroutine(IERequestDeleteFeed(feed_Id, callingFrom));
    }
    public IEnumerator IERequestDeleteFeed(string feed_Id, string callingFrom)
    {
        Debug.Log("Delete Feed Id:" + feed_Id + "  :" + (ConstantsGod.API_BASEURL + ConstantsGod.r_url_DeleteFeed + "/" + feed_Id));
        WWWForm form = new WWWForm();
        form.AddField("feedId", feed_Id);

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_DeleteFeed), form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            SNSNotificationHandler.Instance.DeleteLoaderShow(false);//delete loader disable
            //FeedsManager.Instance.ShowLoader(false);

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log("Feed Delete Success!");
                string data = www.downloadHandler.text;
                Debug.Log("<color = red> DeleteFeed data:" + data + "</color>");
                switch (callingFrom)
                {
                    case "DeleteFeed":
                        FeedsManager.Instance.OnSuccessDeleteFeed();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    //this api is used to edit feed.......
    public void RequestEditFeed(string feedID, string description, string image, string video)
    {
        StartCoroutine(IERequestEdit(feedID, description, image, video));
    }
    public IEnumerator IERequestEdit(string feedID, string description, string image, string video)
    {
        Debug.Log("IERequestEdit Post API Calling feedId:" + feedID);

        WWWForm form = new WWWForm();

        form.AddField("feedId", feedID);
        form.AddField("descriptions", description);
        form.AddField("image", image);
        form.AddField("video", video);

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_EditFeed), form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            FeedsManager.Instance.ShowLoader(false);

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                //Debug.Log("data" + form);
            }
            else
            {
                //Debug.Log("feed update complete!");
                string data = www.downloadHandler.text;
                // Debug.Log("Edit Feed data:" + data);
                FeedsManager.Instance.OnSuccessFeedEdit();
            }
        }
    }

    //this api is used to Like or DisLike Feed.......
    public void RequestLikeOrDisLikeFeed(string feedId, Button likeButton)
    {
        // Debug.Log("RequestLikeOrDisLikeFeed feedId:" + feedId);
        likeButton.interactable = false;//like button interactable false untill response.......

        if (IERequestLikeOrDisLikeFeedCo != null)
        {
            StopCoroutine(IERequestLikeOrDisLikeFeedCo);
        }
        IERequestLikeOrDisLikeFeedCo = StartCoroutine(IERequestLikeOrDisLikeFeed(feedId, likeButton));
    }
    Coroutine IERequestLikeOrDisLikeFeedCo;
    public IEnumerator IERequestLikeOrDisLikeFeed(string feedId, Button likeButton)
    {
        WWWForm form = new WWWForm();
        form.AddField("feedId", feedId);

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_FeedLikeDisLike), form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            likeButton.interactable = true;//like button interactable true.......

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                //Debug.Log("data" + form);
            }
            else
            {
                //Debug.Log("Feed Like or DisLike success!");
                string data = www.downloadHandler.text;
                // Debug.Log("LikeOrDisLikeFeed data:" + data);
                FeedLikeDisLikeRoot feedLikeDisLikeRoot = JsonConvert.DeserializeObject<FeedLikeDisLikeRoot>(data);

                //if (feedLikeDisLikeRoot.data == null)
                if (feedLikeDisLikeRoot.msg.Equals("Feed disLike successfully"))
                {
                    FeedsManager.Instance.LikeDislikeSuccessAfterUpdateRequireFeedResponse(false, feedLikeDisLikeRoot.data.likeCount);
                }
                else
                {
                    FeedsManager.Instance.LikeDislikeSuccessAfterUpdateRequireFeedResponse(true, feedLikeDisLikeRoot.data.likeCount);
                }
            }
        }
    }

    //this api is used to delete avatar.......
    public void DeleteAvatarDataFromServer(string token, string UserId)
    {
        StartCoroutine(DeleteUserData(token, UserId));
    }
    IEnumerator DeleteUserData(string token, string userID)   // delete data if Exist
    {
        UnityWebRequest www = UnityWebRequest.Delete(ConstantsGod.API_BASEURL + ConstantsGod.DELETEOCCUPIDEUSER + userID);
        www.SetRequestHeader("Authorization", token);
        yield return www.SendWebRequest();
        if (www.responseCode == 200)
        {
            Debug.Log("<color = red> Occupied Asset Delete Successfully </color>");
        }
    }

    //this api is used to get search user list......
    public void RequestGetSearchUser(string name)
    {
        StartCoroutine(IERequestGetSearchUser(name));
    }
    public IEnumerator IERequestGetSearchUser(string name)
    {
        WWWForm form = new WWWForm();
        if (!name.All(char.IsDigit)) // is seraching with name
        {
            form.AddField("name", name);
            form.AddField("userId", 0);
        }
        else // is searching with number
        {
            form.AddField("name", "");
            form.AddField("userId", name);
        }


        string uri = ConstantsGod.API_BASEURL + ConstantsGod.r_url_SearchUser + "1/50";
        using (UnityWebRequest www = UnityWebRequest.Post(uri, form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.Log("Search user name data:" + data);
                searchUserRoot = JsonUtility.FromJson<SearchUserRoot>(data);
                SNS_APIController.Instance.FeedGetAllSearchUser();
            }
        }
    }

    public void GetFeedUserProfileData<T>(int _userid, T obj) where T : class
    {
        StartCoroutine(IERequestFeedUserProfileData(_userid, result =>
        {
            FeedData _feedUserData = obj as FeedData;
            _feedUserData.SetupFeedUserProfile(result);
        }));
    }

    IEnumerator IERequestFeedUserProfileData(int _userid, Action<SearchUserRow> result)
    {
        WWWForm form = new WWWForm();
        form.AddField("userId", _userid);
        string uri = ConstantsGod.API_BASEURL + ConstantsGod.r_url_SearchUser + "1/1";
        using (UnityWebRequest www = UnityWebRequest.Post(uri, form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                string data = www.downloadHandler.text;
                //Debug.Log("Feed user profile data:" + data);
                searchUserRoot = JsonUtility.FromJson<SearchUserRoot>(data);
                if (searchUserRoot.msg.Contains("yourself"))
                {
                    if (FeedsManager.Instance)
                    {
                        FeedsManager.Instance.bottomTabManager.OnClickProfileButton();
                    }
                }
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.Log("Feed user profile data:" + data);
                searchUserRoot = JsonUtility.FromJson<SearchUserRoot>(data);
                result(searchUserRoot.data.rows[0]);
            }
        }
    }

    public void RequestGetUserLatestAvatarData<T>(string userid, T obj) where T : class
    {
        StartCoroutine(IERequestGetUserLatestAvatarData(userid, success =>
             {
                 if (success)
                 {
                     if (obj is FollowerItemController)
                     {
                         FollowerItemController _followerObj = obj as FollowerItemController;
                         _followerObj.DressUpUserAvatar();
                     }
                     else if (obj is FollowingItemController)
                     {
                         FollowingItemController _followingObj = obj as FollowingItemController;
                         _followingObj.DressUpUserAvatar();
                     }else if (obj is FindFriendWithNameItem)
                     {
                         FindFriendWithNameItem _followingObj = obj as FindFriendWithNameItem;
                         _followingObj.DressUpUserAvatar();
                     }else if (obj is FeedData)
                     {
                         FeedData _followingObj = obj as FeedData;
                         _followingObj.DressUpUserAvatar();
                     }
                 }
                 else
                 {
                     RequestGetUserLatestAvatarData<T>(userid, obj);
                 }
             }));
    }

    public IEnumerator IERequestGetUserLatestAvatarData(string _userID, Action<bool> success)
    {
        string url = ConstantsGod.API_BASEURL + ConstantsGod.USERLATESTOCCUPIEDASSET + _userID;
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            // Start the stopwatch
            //Stopwatch stopwatch = Stopwatch.StartNew();
            www.SendWebRequest();

            while (!www.isDone)
                yield return new WaitForSeconds(Time.deltaTime);

            // Stop the stopwatch
            //stopwatch.Stop();

            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                Debug.LogError("Error while receiving Avatar Data" + www.error);
                success(false);
            }
            else
            {
                print("Received Avatar Json1: " + www.downloadHandler.text);
                //// Print the elapsed time
                //Debug.Log("User avatar data Request completed in: " + stopwatch.ElapsedMilliseconds + " milliseconds");
                UserLatestAvatarRoot _userAvatarData = JsonUtility.FromJson<UserLatestAvatarRoot>(www.downloadHandler.text);
                if (_userAvatarData.data.name != null)
                {
                    VisitedUserAvatarData = _userAvatarData.data;
                }
                else
                {
                    print("Avatar data is null");
                    VisitedUserAvatarData = null;
                }
                success(true);
            }

            www.Dispose();
        }
    }

    public void RequestGetSearchUserForProfile(string name)
    {
        StartCoroutine(IERequestGetSearchUserForProfile(name));
    }
    public IEnumerator IERequestGetSearchUserForProfile(string name)
    {
        WWWForm form = new WWWForm();
        if (!name.All(char.IsDigit)) // is seraching with name
        {
            form.AddField("name", name);
            form.AddField("userId", 0);
        }
        else // is searching with number
        {
            form.AddField("name", "");
            form.AddField("userId", name);
        }


        string uri = ConstantsGod.API_BASEURL + ConstantsGod.r_url_SearchUser + "1/50";
        using (UnityWebRequest www = UnityWebRequest.Post(uri, form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.Log("Search user name data:" + data);
                searchUserRoot = JsonUtility.FromJson<SearchUserRoot>(data);
                SNS_APIController.Instance.FeedGetAllSearchUserForProfile();
            }
        }
    }
    #endregion

    #region Friends

    public void SetHotFriend()
    {
        if (FeedsManager.Instance != null)
        {
            FeedsManager.Instance.ShowLoader(true);
        }
        StartCoroutine(IERequestHotFirends());
    }

    IEnumerator IERequestHotFirends()
    {
        string uri = ConstantsGod.API_BASEURL + ConstantsGod.r_url_HotUsers + "1/100";
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                if (FeedsManager.Instance != null)
                {
                    FeedsManager.Instance.ShowLoader(false);
                }
                Debug.Log(www.error);
            }
            else
            {
                if (FeedsManager.Instance != null)
                {
                    FeedsManager.Instance.ShowLoader(false);
                }
                string data = www.downloadHandler.text;
                Debug.Log("~~~~~~ Hot Friends Data" + data);
                hotUsersRoot = JsonUtility.FromJson<HotUsersRoot>(data);
                SNS_APIController.Instance.ShowHotFirend(hotUsersRoot);
                //SNS_APIController.Instance.FeedGetAllSearchUser();
            }
        }
    }


    public void SetRecommendedFriend()
    {
        if (FeedsManager.Instance != null)
        {
            FeedsManager.Instance.ShowLoader(true);
        }
        StartCoroutine(IERequestRecommendedFirends());
    }

    IEnumerator IERequestRecommendedFirends()
    {
        string uri = ConstantsGod.API_BASEURL + ConstantsGod.r_url_RecommendedUser + "1/50";
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                if (FeedsManager.Instance != null)
                {
                    FeedsManager.Instance.ShowLoader(false);
                }
                Debug.Log(www.error);
            }
            else
            {
                if (FeedsManager.Instance != null)
                {
                    FeedsManager.Instance.ShowLoader(false);
                }
                string data = www.downloadHandler.text;
                Debug.Log("~~~~~~ Recommended Friends Data" + data);
                searchUserRoot = JsonUtility.FromJson<SearchUserRoot>(data);
                SNS_APIController.Instance.ShowRecommendedFriends(searchUserRoot);
                //SNS_APIController.Instance.FeedGetAllSearchUser();
            }
        }
    }


    public void SetMutalFrndList()
    {
        if (FeedsManager.Instance != null)
        {
            FeedsManager.Instance.ShowLoader(true);
        }
        StartCoroutine(IERequestSetMutalFrndList());
    }

    IEnumerator IERequestSetMutalFrndList()
    {
        string uri = ConstantsGod.API_BASEURL + ConstantsGod.r_url_MutalFrnd + SNS_APIResponseManager.Instance.userId + "/1/100";
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                if (FeedsManager.Instance != null)
                {
                    FeedsManager.Instance.ShowLoader(false);
                }
                Debug.Log(www.error);
            }
            else
            {
                if (FeedsManager.Instance != null)
                {
                    FeedsManager.Instance.ShowLoader(false);
                }
                string data = www.downloadHandler.text;
                Debug.Log("~~~~~~ MutalFrnd Data" + data);
                SearchUserRoot mutalFrnd = JsonUtility.FromJson<SearchUserRoot>(data);
                FeedsManager.Instance.AddFrndNoMutalFrnd.SetActive(false);
                if (mutalFrnd.data.count > 0)
                {
                    SNS_APIController.Instance.ShowMutalFrnds(mutalFrnd);
                }
                else
                { // to Show no mutal Frnd
                    FeedsManager.Instance.AddFrndNoMutalFrnd.SetActive(true);
                }
            }
        }
    }

    public void GetBestFriend()
    {
        StartCoroutine(IEGetBestFriends());
    }

    IEnumerator IEGetBestFriends()
    {
        string uri = ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetBestFrnd + SNS_APIResponseManager.Instance.userId;
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.Log("~~~~~~ Get Best Friends list : " + data);
                CloseFrndRoot CloseFrnds = JsonUtility.FromJson<CloseFrndRoot>(data);
                BFCount = CloseFrnds.data.count;
            }
        }
    }

    public void AddBestFriend(int userId, GameObject FrndBtn)
    {
        if (BFCount < maxBfCount)
        {
            StartCoroutine(IEAddBestFriend(userId, FrndBtn));
        }
        else
        {
            UIHandler.Instance._footerCan.GetComponent<CanvasGroup>().alpha = 0;
            UIHandler.Instance._footerCan.GetComponent<CanvasGroup>().interactable = false;
            UIHandler.Instance._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = false;
            FeedsManager.Instance.BestFriendFull.SetActive(true);
            //SNSNotificationHandler.Instance.ShowNotificationMsg("Best Friend limit is reached");
        }
    }
    IEnumerator IEAddBestFriend(int userId, GameObject FrndBtn)
    {
        if (FeedsManager.Instance != null)
        {
            FeedsManager.Instance.ShowLoader(true);
        }
        string uri = ConstantsGod.API_BASEURL + ConstantsGod.r_url_AdBestFrnd + userId.ToString();
        using (UnityWebRequest www = UnityWebRequest.Post(uri, "POST"))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                if (FeedsManager.Instance != null)
                {
                    FeedsManager.Instance.ShowLoader(false);
                }
                Debug.Log(www.error);
            }
            else
            {
                if (FeedsManager.Instance != null)
                {
                    FeedsManager.Instance.ShowLoader(false);
                }
                string data = www.downloadHandler.text;
                Debug.Log("~~~~~~ Add Best Friend : " + data);
                AdCloseFrndRoot AdCloseFrnds = JsonUtility.FromJson<AdCloseFrndRoot>(data);
                if (AdCloseFrnds.success)
                {
                    //BFCount++;
                    GetBestFriend();
                    if (FrndBtn.GetComponent<FollowingItemController>())
                    {
                        FrndBtn.GetComponent<FollowingItemController>().UpdateBfBtn(true);
                    }
                    else if (FrndBtn.GetComponent<FindFriendWithNameItem>())
                    {
                        FrndBtn.GetComponent<FindFriendWithNameItem>().UpdateBfBtn(true);
                    }

                }
            }
        }
    }

    public void RemoveBestFriend(int userId, GameObject FrndBtn)
    {
        if (FeedsManager.Instance != null)
        {
            FeedsManager.Instance.ShowLoader(true);
        }
        StartCoroutine(IERemoveBestFriend(userId, FrndBtn));
    }
    IEnumerator IERemoveBestFriend(int userId, GameObject FrndBtn)
    {
        string uri = ConstantsGod.API_BASEURL + ConstantsGod.r_url_RemoveBestFrnd + userId.ToString();
        //WWWForm form = new WWWForm();
        //form.AddField("friendId", userId.ToString());
        using (UnityWebRequest www = UnityWebRequest.Delete(uri))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                if (FeedsManager.Instance != null)
                {
                    FeedsManager.Instance.ShowLoader(false);
                }
                Debug.Log(www.error);
            }
            else
            {
                //if (BFCount<0)
                //{
                //   BFCount--;
                //}
                if (FeedsManager.Instance != null)
                {
                    FeedsManager.Instance.ShowLoader(false);
                }
                GetBestFriend();
                if (FrndBtn.GetComponent<FollowingItemController>())
                {
                    FrndBtn.GetComponent<FollowingItemController>().UpdateBfBtn(false);
                }
                else if (FrndBtn.GetComponent<FindFriendWithNameItem>())
                {
                    FrndBtn.GetComponent<FindFriendWithNameItem>().UpdateBfBtn(false);
                }
            }
        }
    }

    #endregion

    #region UserAPI.......... 

    public void RequestSetName(string setName_name)
    {
        StartCoroutine(IERequestSetName(setName_name));
    }
    public IEnumerator IERequestSetName(string setName_name)
    {
        WWWForm form = new WWWForm();

        form.AddField("name", setName_name);

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_SetName), form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log("Form upload complete!");
                string data = www.downloadHandler.text;
                Debug.Log("SetName data:" + data);
                // root = JsonUtility.FromJson<SetNameRoot>(data);
                var jo = Newtonsoft.Json.Linq.JObject.Parse(data);
                var msg = jo["msg"].ToString();
                if (msg == "This name is already taken by other user.")
                {
                    //Debug.Log("Username already exists");
                    MyProfileManager.Instance.ShowEditProfileNameErrorMessage("Username already exists");
                }
                else
                {
                    if (string.IsNullOrEmpty(ConstantsHolder.xanaConstants.userProfileLink) || ConstantsHolder.xanaConstants.userProfileLink.Contains("Profil") || ConstantsHolder.xanaConstants.userProfileLink.Contains("userProfile"))
                    {
                        // Profile is not Modified by User
                            ProfilePictureManager.instance.MakeProfilePicture(setName_name);
                    }
                }
            }
        }
    }

    public void RequestGetUserDetails(string callingFrom)
    {
        StartCoroutine(IERequestGetUserDetails(callingFrom));
    }
    public IEnumerator IERequestGetUserDetails(string callingFrom)
    {
        using (UnityWebRequest www = UnityWebRequest.Get((ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetUserDetails)))
        {
            // Start the stopwatch
            //Stopwatch stopwatch = Stopwatch.StartNew();
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            www.SendWebRequest();

            while (!www.isDone)
                yield return null;

            // Stop the stopwatch
            //stopwatch.Stop();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("IERequestGetUserDetails error:" + www.error);
                if (FeedsManager.Instance != null)
                {
                    FeedsManager.Instance.ShowLoader(false);
                    switch (callingFrom)
                    {
                        case "EditProfileAvatar":
                            MyProfileManager.Instance.EditProfileDoneButtonSetUp(true);//setup edit profile done button.......
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                // Print the elapsed time
                //Debug.Log("User details Request completed in: " + stopwatch.ElapsedMilliseconds + " milliseconds");
                //Debug.Log("IERequestGetUserDetails Form upload complete!");
                string data = www.downloadHandler.text;
                Debug.Log("IERequestGetUserDetails Loaded Completed data:" + data + "      :Calling From:" + callingFrom);
                //Debug.Log("callingFrom" + callingFrom);
                myProfileDataRoot = JsonUtility.FromJson<GetUserDetailRoot>(data);
                switch (callingFrom)
                {
                    case "myProfile":
                        MyProfileManager.Instance.SetupData(myProfileDataRoot.data, callingFrom);//setup and load my profile data.......                        
                        break;
                    case "EditProfileAvatar":
                        MyProfileManager.Instance.SetupData(myProfileDataRoot.data, callingFrom);//setup and load my profile data.......
                        break;
                    case "messageScreen":
                        SNS_SMSModuleManager.Instance.GetSuccessUserDetails(myProfileDataRoot.data);
                        break;
                    case "MyAccount":
                        MyProfileManager.Instance.myProfileData = myProfileDataRoot.data;
                        SNSSettingManager.Instance.SetUpPersonalInformationScreen();
                        break;
                    default:
                        break;
                }

                PlayerPrefs.SetString("PlayerName", myProfileDataRoot.data.name);

                if (string.IsNullOrEmpty(myProfileDataRoot.data.avatar))
                {
                    if (ProfilePictureManager.instance)
                    {
                        ProfilePictureManager.instance.MakeProfilePicture(myProfileDataRoot.data.name);
                    }
                }
            }
            www.Dispose();
        }
    }

    public void RequestUpdateUserAvatar(string user_avatar, string callingFrom)
    {
        StartCoroutine(IERequestUpdateUserAvatar(user_avatar, callingFrom));
    }
    public IEnumerator IERequestUpdateUserAvatar(string user_avatar, string callingFrom)
    {
        WWWForm form = new WWWForm();

        form.AddField("avatar", user_avatar);

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_UpdateUserAvatar), form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                switch (callingFrom)
                {
                    case "EditProfileAvatar":
                        RequestGetUserDetails(callingFrom);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                //Debug.Log("Form upload complete!");
                string data = www.downloadHandler.text;
                Debug.Log("UpdateUserAvatar data:" + data);
                // root = JsonUtility.FromJson<UpdateUserAvatarRoot>(data);
                switch (callingFrom)
                {
                    case "EditProfileAvatar":
                        RequestGetUserDetails(callingFrom);
                        MyProfileManager.Instance.AfterUpdateAvatarSetTempSprite();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    // Old API
    public void RequestUpdateUserProfile(string user_gender, string user_job, string user_country, string user_website, string user_bio)
    {
        StartCoroutine(IERequestUpdateUserProfile(user_gender, user_job, user_country, user_website, user_bio));
    }

    public IEnumerator IERequestUpdateUserProfile(string user_gender, string user_job, string user_country, string user_website, string user_bio)
    {
        WWWForm form = new WWWForm();
        Debug.Log("BaseUrl:" + ConstantsGod.API_BASEURL + "job:" + user_job + "  :bio:" + user_bio);
        form.AddField("gender", user_gender);
        form.AddField("job", user_job);
        form.AddField("country", user_country);
        form.AddField("website", user_website);
        form.AddField("bio", user_bio);

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_UpdateUserProfile), form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                //Debug.Log("data" + form);
            }
            else
            {
                //Debug.Log("Form upload complete!");
                string data = www.downloadHandler.text;
                Debug.Log("<color=red> UpdateUserProfile data:" + data + "</color>");
                // root = JsonUtility.FromJson<UpdateUserProfileRoot>(data);
            }
        }
    }

    // New API
    public void RequestUpdateUserProfile(string unique_Name, string user_gender, string user_job, string user_country, string user_website, string user_bio, string[] _tags)
    {
        StartCoroutine(IERequestUpdateUserProfile(unique_Name, user_gender, user_job, user_country, user_website, user_bio, _tags));
    }


    class UserProfile
    {
        public string bio;
        public string username;
        public string[] tags;
    }

    class UniqueUserNameError
    {
        public bool success;
        public string msg;
    }

    public IEnumerator IERequestUpdateUserProfile(string unique_Name, string user_gender, string user_job, string user_country, string user_website, string user_bio, string[] _tags)
    {
        WWWForm form = new WWWForm();
        Debug.Log("BaseUrl:" + ConstantsGod.API_BASEURL + "   job:" + user_job + "  :bio:" + user_bio);
        //form.AddField("gender", user_gender);
        //form.AddField("job", user_job);
        //form.AddField("country", user_country);
        //form.AddField("website", user_website);
        //form.AddField("bio", user_bio);
        //form.AddField("username", unique_Name);
        if(user_bio == "")
        {
            user_bio = " ";
        }
        UserProfile userProfile = new UserProfile
        {
            bio = user_bio,
            username = unique_Name,
            tags = _tags,
        };

        string jsonData = JsonUtility.ToJson(userProfile);
        string apiUrl = ConstantsGod.API_BASEURL + ConstantsGod.r_url_UpdateUserProfile;

        // using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_UpdateUserProfile), form))
        using (UnityWebRequest www = new UnityWebRequest(apiUrl, "POST"))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();
            UniqueUserNameError test = JsonConvert.DeserializeObject<UniqueUserNameError>(www.downloadHandler.text);
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) //(www.result.isNetworkError || www.isHttpError)
            {
                Debug.Log("<color=red> ------Edit API Error " + www.error + www.downloadHandler.text + "</color>");
                if (test.msg.Contains("Username"))
                {
                    MyProfileManager.Instance.isEditProfileNameAlreadyExists = true;
                    MyProfileManager.Instance.ShowEditProfileUniqueNameErrorMessage("The username must include letters");
                }
                //Jugar for mainnet issue as API is not deployed yet on mainnet
                //MyProfileManager.Instance.isEditProfileNameAlreadyExists = true;
                //MyProfileManager.Instance.ShowEditProfileUniqueNameErrorMessage("The User Name field should be Unique and not empty");
                //Debug.Log("data" + form);
            }
            else
            {
                //Debug.Log("Form upload complete!");

                //string data = www.downloadHandler.text;

                if (!test.success)
                {
                    if (test.msg.Contains("Username"))
                    {
                        MyProfileManager.Instance.isEditProfileNameAlreadyExists = true;
                        MyProfileManager.Instance.ShowEditProfileUniqueNameErrorMessage("Username already taken");
                    }
                }
                Debug.Log("<color=red> UpdateUserProfile data:" + www.downloadHandler.text + "</color>");
                // root = JsonUtility.FromJson<UpdateUserProfileRoot>(data);
            }
        }
    }

    public void RequestDeleteAccount()
    {
        StartCoroutine(IERequestDeleteAccount());
    }
    public IEnumerator IERequestDeleteAccount()
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_DeleteAccount), form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log("Form upload complete!");
                string data = www.downloadHandler.text;
                Debug.Log("DeleteAccount data:" + data);
                // root = JsonUtility.FromJson<DeletAccountRoot>(data);
            }
        }
    }
    #endregion

    #region MessageApi.........
    //this api is used to get all conversation.......
    public void RequestChatGetConversation()
    {
        //Debug.Log("111111");
        StartCoroutine(IERequestChatGetConversation());
    }
    public IEnumerator IERequestChatGetConversation()
    {
        using (UnityWebRequest www = UnityWebRequest.Get((ConstantsGod.API_BASEURL + ConstantsGod.r_url_ChatGetConversation)))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                if (SNS_SMSModuleManager.Instance.startAndWaitMessageText.gameObject.activeSelf)
                {
                    SNS_SMSModuleManager.Instance.StartAndWaitMessageTextActive(true, UITextLocalization.GetLocaliseTextByKey("bad internet connection please try again"));//start and wait message text show.......
                }
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log("Form upload complete!");
                string data = www.downloadHandler.text;
                Debug.Log("ChatGetConversation data:" + data);
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                allChatGetConversationRoot = JsonConvert.DeserializeObject<ChatGetConversationRoot>(data, settings);

                SNS_APIController.Instance.GetAllConversation();
                // Debug.Log(root.data.count);
            }
        }
    }

    //this api is used to chate mute unmute conversation.......
    public void RequestChatMuteUnMuteConversation(int conversationId)
    {
        Debug.Log("RequestChatMuteUnMuteConversation conversation id:" + conversationId);
        if (IERequestChatMuteUnMuteConversationCo != null)
        {
            StopCoroutine(IERequestChatMuteUnMuteConversationCo);
        }
        IERequestChatMuteUnMuteConversationCo = StartCoroutine(IERequestChatMuteUnMuteConversation(conversationId));
    }
    Coroutine IERequestChatMuteUnMuteConversationCo;
    public IEnumerator IERequestChatMuteUnMuteConversation(int conversationId)
    {
        WWWForm form = new WWWForm();
        form.AddField("conversationId", conversationId);

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_ChatMuteUnMuteConversation), form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            SNS_SMSModuleManager.Instance.LoaderShow(false);//false api loader 
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Debug.Log("Form upload complete!");
                string data = www.downloadHandler.text;
                Debug.Log("Mute UnMute conversation success: " + data);
                ChatMuteUnMuteRoot chatMuteUnMuteRoot = JsonConvert.DeserializeObject<ChatMuteUnMuteRoot>(data);

                if (chatMuteUnMuteRoot != null)//refresh current conversation data after mute unmute.......
                {
                    if (chatMuteUnMuteRoot.msg == "conversation muted successfully")
                    {
                        SNS_SMSModuleManager.Instance.allChatGetConversationDatum.isMutedConversations = true;
                    }
                    else
                    {
                        SNS_SMSModuleManager.Instance.allChatGetConversationDatum.isMutedConversations = false;
                    }
                }
            }
        }
    }

    //this api is used to get all message for user.......
    public void RequestChatGetMessages(int message_pageNumber, int message_pageSize, int message_receiverId, int message_receivedGroupId, string callingFrom)
    {
        StartCoroutine(IERequestChatGetMessages(message_pageNumber, message_pageSize, message_receiverId, message_receivedGroupId, callingFrom));
    }
    public IEnumerator IERequestChatGetMessages(int message_pageNumber, int message_pageSize, int message_receiverId, int message_receivedGroupId, string callingFrom)
    {
        WWWForm form = new WWWForm();
        form.AddField("pageNumber", message_pageNumber);
        form.AddField("pageSize", message_pageSize);
        if (message_receivedGroupId != 0)
        {
            form.AddField("receivedGroupId", message_receivedGroupId);
        }
        else if (message_receiverId != 0)
        {
            form.AddField("receiverId", message_receiverId);
        }

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_ChatGetMessages), form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            SNS_SMSModuleManager.Instance.LoaderShow(false);//rik loader false.......

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                if (message_receivedGroupId != 0)
                {
                    SNS_SMSModuleManager.Instance.ChatScreen.SetActive(true);
                    SNS_SMSModuleManager.Instance.MessageListScreen.SetActive(false);
                    r_isCreateMessage = false;
                }
            }
            else
            {
                // Debug.Log("Form upload complete!");
                string data = www.downloadHandler.text;
                Debug.Log("Message Chat: " + data);
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                allChatMessagesRoot = JsonConvert.DeserializeObject<ChatGetMessagesRoot>(data, settings);
                //allChatMessagesRoot.data.rows.Reverse();
                SNS_APIController.Instance.GetAllChat(message_pageNumber, "");
                switch (callingFrom)
                {
                    case "Conversation":
                        if (CommonAPIHandler.Instance != null)//For Get All Chat UnRead Message Count.......
                        {
                            CommonAPIHandler.Instance.RequestGetAllChatUnReadMessagesCount();
                        }
                        break;
                    default:
                        break;
                }
                // Debug.Log(root.data.count);                
            }
        }
    }

    //this api is used to send attachments.......
    public void RequestChatGetAttachments(int message_pageNumber, int message_pageSize, int message_receiverId, int message_receivedGroupId, int index)
    {
        SNS_SMSModuleManager.Instance.LoaderShow(true);//active api loader.

        StartCoroutine(IERequestChatAttachments(message_pageNumber, message_pageSize, message_receiverId, message_receivedGroupId, index));
    }
    public IEnumerator IERequestChatAttachments(int message_pageNumber, int message_pageSize, int message_receiverId, int message_receivedGroupId, int index)
    {
        WWWForm form = new WWWForm();
        form.AddField("pageNumber", message_pageNumber);
        form.AddField("pageSize", message_pageSize);
        if (message_receivedGroupId != 0)
        {
            form.AddField("receivedGroupId", message_receivedGroupId);
        }
        else if (message_receiverId != 0)
        {
            form.AddField("receiverId", message_receiverId);
        }

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_ChatGetAttachments), form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                // Debug.Log(www.error);
                //Debug.Log("data" + www.downloadHandler);
                // Debug.Log("data" + www.downloadHandler.text);

                SNS_SMSModuleManager.Instance.LoaderShow(false);//False api loader.

                string data = www.downloadHandler.text;
                Debug.Log("Get Attachment Error:" + data);
                AllChatAttachmentsRoot = JsonConvert.DeserializeObject<ChatAttachmentsRoot>(data);
                if (AllChatAttachmentsRoot != null)
                {
                    if (AllChatAttachmentsRoot.msg == "No attachments found")
                    {
                        if (index == 0)
                        {
                            foreach (Transform item in SNS_SMSModuleManager.Instance.chatShareAttechmentparent)
                            {
                                Destroy(item.gameObject);
                            }
                            SNS_APIController.Instance.SetChatMember();
                        }
                        else if (index == 1)
                        {
                            SNS_SMSModuleManager.Instance.NoAttechmentScreen.SetActive(true);
                            SNS_SMSModuleManager.Instance.chatShareAttechmentparent.gameObject.SetActive(false);
                        }
                    }
                }
            }
            else
            {
                // Debug.Log("Form upload complete!");
                string data = www.downloadHandler.text;
                Debug.Log("Get Attachment Data: " + data);
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                AllChatAttachmentsRoot = JsonConvert.DeserializeObject<ChatAttachmentsRoot>(data, settings);
                //AllChatAttachmentsRoot = JsonConvert.DeserializeObject<ChatAttachmentsRoot>(data);

                SNS_APIController.Instance.GetAllAttachments(index);
                // Debug.Log(root.data.count);
            }
        }
    }

    //this api is used to create group.......
    public void RequestChatCreateGroup(string createGroupName, string createGroupUserIds, string groupAvatarUrl)
    {
        StartCoroutine(IERequestChatCreateGroup(createGroupName, createGroupUserIds, groupAvatarUrl));
    }
    public IEnumerator IERequestChatCreateGroup(string createGroupName, string createGroupUserIds, string groupAvatarUrl)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", createGroupName);
        form.AddField("userIds", createGroupUserIds);
        form.AddField("avatar", groupAvatarUrl);

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_ChatCreateGroup), form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Debug.Log("Form upload complete!");
                string data = www.downloadHandler.text;
                Debug.Log("Message Chat: " + data);
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                ChatCreateGroupRoot = JsonConvert.DeserializeObject<ChatCreateGroupRoot>(data, settings);

                //Debug.Log("msg : " + SNS_SMSModuleManager.Instance.typeMessageText.text);
                //Debug.Log("msg : " + SNS_SMSModuleManager.Instance.chatTypeMessageInputfield.Text);
                yield return new WaitForSeconds(0.1f);

                // RequestChatCreateMessage(0, ChatCreateGroupRoot.data.id,SNS_SMSModuleManager.Instance.typeMessageText.text,"");
                // RequestChatGetConversation();
                // RequestChatGetMessages(1, 50, 0,ChatCreateGroupRoot.data.id);
                // SNS_SMSModuleManager.Instance.typeMessageText.text = "";
                // SNS_APIController.Instance.GetAllChat();
                // Debug.Log(root.data.count);
            }
        }
    }

    //this api is used to add member on group.......
    public void RequestAddGroupMember(string groupId, string conversationId, string userIds)
    {
        StartCoroutine(IERequestAddGroupMember(groupId, conversationId, userIds));
    }
    public IEnumerator IERequestAddGroupMember(string groupId, string conversationId, string userIds)
    {
        WWWForm form = new WWWForm();
        form.AddField("groupId", groupId);
        form.AddField("conversationId", conversationId);
        form.AddField("userIds", userIds);

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_AddGroupMember), form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);

                SNS_SMSModuleManager.Instance.LoaderShow(false);//false api loader 
            }
            else
            {
                // Debug.Log("Form upload complete!");
                string data = www.downloadHandler.text;
                Debug.Log("Add member success on group: " + data);
                RequestChatGetConversation();//refresh conversation list to update group data
            }
        }
    }

    //this api is used to Update group info.......
    public void RequestUpdateGroupInfo(string groupId, string groupName, string avatar)
    {
        StartCoroutine(IERequestUpdateGroupInfo(groupId, groupName, avatar));
    }
    public IEnumerator IERequestUpdateGroupInfo(string groupId, string groupName, string avatar)
    {
        WWWForm form = new WWWForm();
        form.AddField("groupId", groupId);
        form.AddField("name", groupName);
        form.AddField("avatar", avatar);

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_UpdateGroupInfo), form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            SNS_SMSModuleManager.Instance.LoaderShow(false);//false loader screen.

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Debug.Log("Form upload complete!");
                string data = www.downloadHandler.text;
                Debug.Log("Update Success Group Info: " + data);
                SNS_SMSModuleManager.Instance.UpdateGroupInFoSuccessResponce();
            }
        }
    }

    //this api is used to create message.......
    public void RequestChatCreateMessage(int createMessageReceiverId, int createMessageReceivedGroupId, string createMessageMsg, string createMessageType, string createMessageAttachments)
    {
        StartCoroutine(IERequestChatCreateMessage(createMessageReceiverId, createMessageReceivedGroupId, createMessageMsg, createMessageType, createMessageAttachments));
    }
    public IEnumerator IERequestChatCreateMessage(int createMessageReceiverId, int createMessageReceivedGroupId, string createMessageMsg, string createMessageType, string createMessageAttachments)
    {
        WWWForm form = new WWWForm();
        if (createMessageReceivedGroupId != 0)
        {
            form.AddField("receivedGroupId", createMessageReceivedGroupId);
        }
        else if (createMessageReceiverId != 0)
        {
            form.AddField("receiverId", createMessageReceiverId);
        }

        if (!string.IsNullOrEmpty(createMessageMsg))
        {
            string encodeSTR = EncodedString(createMessageMsg);
            Debug.Log("Encode STR:" + encodeSTR);
            form.AddField("msg", encodeSTR);
        }

        if (!string.IsNullOrEmpty(createMessageType))
        {
            form.AddField("type", createMessageType);
        }

        if (!string.IsNullOrEmpty(createMessageAttachments))
        {
            Debug.Log("attachments: " + createMessageAttachments);
            form.AddField("attachments", createMessageAttachments);
        }

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_ChatCreateMessage), form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();
            //Debug.Log("receiverId" + createMessageReceiverId);
            //Debug.Log("receivedGroupId" + createMessageReceivedGroupId);
            //Debug.Log("msg" + createMessageMsg);
            // Debug.Log("attachments" + createMessageAttachments);
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                r_isCreateMessage = false;

                SNS_SMSModuleManager.Instance.isLeaveGroup = false;//if create message is failed  then false LeaveGroup bool.

                SNS_SMSModuleManager.Instance.LoaderShow(false);//rik loader false.......
                SNS_SMSModuleManager.Instance.OnClcikSendMessageButtonbool = false;
            }
            else
            {
                // Debug.Log("Form upload complete!");
                string data = www.downloadHandler.text;
                //Debug.Log("Message : " + data);
                // RequestChatGetConversation();
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                AllChatCreateMessageRoot = JsonConvert.DeserializeObject<ChatCreateMessageRoot>(data, settings);

                yield return new WaitForSeconds(0f);

                SNS_SMSModuleManager.Instance.LoaderShow(false);//rik loader false.......
                Debug.Log("Chat CreateMessage success:" + SNS_SMSModuleManager.Instance.isLeaveGroup + "  :Data:" + data);
                if (!SNS_SMSModuleManager.Instance.isLeaveGroup)//not get message api call after leave group.......
                {
                    if (AllChatCreateMessageRoot.data.receivedGroupId != 0)
                    {
                        //Debug.Log("receivedGroupId" + AllChatCreateMessageRoot.data.receivedGroupId);
                        //SNS_SMSModuleManager.Instance.LoaderShow(true);//rik loader active.......
                        RequestChatGetMessages(1, 50, 0, AllChatCreateMessageRoot.data.receivedGroupId, "Conversation");
                    }
                    else
                    {
                        //Debug.Log("user id:" + userId + "    :receiverId:" + AllChatCreateMessageRoot.data.receiverId);
                        if (AllChatCreateMessageRoot.data.receiverId == userId)
                        {
                            //SNS_SMSModuleManager.Instance.LoaderShow(true);//rik loader active.......
                            RequestChatGetMessages(1, 50, AllChatCreateMessageRoot.data.senderId, 0, "Conversation");
                        }
                        else
                        {
                            //SNS_SMSModuleManager.Instance.LoaderShow(true);//rik loader active.......
                            RequestChatGetMessages(1, 50, AllChatCreateMessageRoot.data.receiverId, 0, "Conversation");
                        }
                        //  RequestChatGetMessages(1, 50, AllChatCreateMessageRoot.data.receiverId, 0);
                    }
                    //SNS_SMSModuleManager.Instance.typeMessageText.text = "";
                    SNS_SMSModuleManager.Instance.chatTypeMessageInputfield.Text = "";
                    SNS_SMSModuleManager.Instance.OnChatVoiceOrSendButtonEnable();
                    // Debug.Log(root.data.count);

                    if (!string.IsNullOrEmpty(SNS_SMSModuleManager.Instance.isDirectMessageFirstTimeRecivedID))
                    {
                        RequestChatGetConversation();
                    }
                }
                else
                {
                    SNS_SMSModuleManager.Instance.isLeaveGroup = false;//set leave group bool false.......
                }
                SNS_SMSModuleManager.Instance.OnClcikSendMessageButtonbool = false;
            }
        }
    }

    //this api is used to Leave the chat.......
    public void RequestLeaveTheChat(string groupId, string callingFrom)
    {
        StartCoroutine(IERequestLeaveTheChat(groupId, callingFrom));
    }
    public IEnumerator IERequestLeaveTheChat(string groupId, string callingFrom)
    {
        Debug.Log("Group ID:" + groupId + "    :CallingFrom:" + callingFrom);
        WWWForm form = new WWWForm();
        form.AddField("id", groupId);

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_LeaveTheChat), form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log("Leave The Chat success!");
                string data = www.downloadHandler.text;
                Debug.Log("Leave The Chat: " + data);
                switch (callingFrom)
                {
                    case "ConversationScreen":
                        SNS_SMSModuleManager.Instance.DeleteConversationWithLeaveGroupApiResponseSuccess(groupId);
                        break;
                    case "DetailsScreen":
                        SNS_APIController.Instance.LeaveTheChatCallBack(groupId);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    //this api is used to Remove member from group chat.......
    public void RequestRemoveGroupMember(int groupId, int userId)
    {
        SNS_SMSModuleManager.Instance.LoaderShow(true);//rik loader false.......
        StartCoroutine(IERequestRemoveGroupMember(groupId, userId));
    }
    public IEnumerator IERequestRemoveGroupMember(int groupId, int userId)
    {
        Debug.Log("Remove Group member Group ID:" + groupId + "    :UserId:" + userId);
        WWWForm form = new WWWForm();
        form.AddField("groupId", groupId);
        form.AddField("userId", userId);

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_RemoveGroupMember), form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            SNS_SMSModuleManager.Instance.LoaderShow(false);//rik loader false.......

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Remove group member from Group Chat success!");
                string data = www.downloadHandler.text;
                Debug.Log("Remove group member data: " + data);
                SNS_SMSModuleManager.Instance.RemoveMemberApiResponseSuccess();
            }
        }
    }

    //this api is used to Delete Conversation.......
    public void RequestDeleteConversation(int conversationId)
    {
        SNS_SMSModuleManager.Instance.LoaderShow(true);//rik loader false.......
        StartCoroutine(IERequestDeleteConversation(conversationId));
    }
    public IEnumerator IERequestDeleteConversation(int conversationId)
    {
        Debug.Log("Delete conversation ID:" + conversationId);
        WWWForm form = new WWWForm();
        form.AddField("conversationId", conversationId);

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_DeleteConversation), form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            SNS_SMSModuleManager.Instance.LoaderShow(false);//rik loader false.......

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Delete conversation success!");
                string data = www.downloadHandler.text;
                Debug.Log("Delete conversation data: " + data);
                SNS_SMSModuleManager.Instance.DeleteConversationApiResponseSuccess("Conversation Deleted");
            }
        }
    }

    //this api is used to Delete Conversation.......
    public void RequestDeleteChatGroup(int groupId, string callingFrom)
    {
        SNS_SMSModuleManager.Instance.LoaderShow(true);//rik loader false.......
        StartCoroutine(IERequestDeleteChatGroup(groupId, callingFrom));
    }
    public IEnumerator IERequestDeleteChatGroup(int groupId, string callingFrom)
    {
        Debug.Log("Delete Group Chat GroupID:" + groupId);
        WWWForm form = new WWWForm();
        form.AddField("groupId", groupId);

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_DeleteChatGroup), form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            SNS_SMSModuleManager.Instance.LoaderShow(false);//rik loader false.......

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log("Delete Group Chat success!");
                string data = www.downloadHandler.text;
                Debug.Log("Delete Group Chat data: " + data);
                switch (callingFrom)
                {
                    case "ConversationScreen":
                        SNS_SMSModuleManager.Instance.DeleteConversationApiResponseSuccess("Group Deleted");
                        break;
                    case "DetailsScreen":
                        SNS_SMSModuleManager.Instance.DeleteGroupChatApiResponseSuccess();
                        break;
                    default:
                        break;
                }
            }
        }
    }
    #endregion

    #region Encode or Decode String.......
    public static string EncodedString(string encodeSTR)
    {
        return System.Net.WebUtility.UrlEncode(encodeSTR);
    }

    public static string DecodedString(string decodeSTR)
    {
        return System.Net.WebUtility.UrlDecode(decodeSTR);
    }
    #endregion

    #region Clear Resource Unload Unused Asset File.......
    public int unloadUnusedFileCount;
    public void ResourcesUnloadAssetFile()
    {
        if (unloadUnusedFileCount >= 15)
        {
            unloadUnusedFileCount = 0;
            Resources.UnloadUnusedAssets();
            //Caching.ClearCache();
            //GC.Collect();
        }
        unloadUnusedFileCount += 1;
    }
    #endregion

    #region Clear FeedDataAfterLogout.......
    public void ClearAllFeedDataForLogout()
    {
        allUserRootList.Clear();
        hotSaveRootList.Clear();
        followingUserTabSaveRootList.Clear();
        allFollowingUserRootList.Clear();
    }
    #endregion

    #region Check Usl is contains http/https or not
    public bool CheckUrlDropboxOrNot(string url)
    {
        string[] splitArray = url.Split(':');
        if (splitArray.Length > 0)
        {
            if (splitArray[0] == "https" || splitArray[0] == "http")
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    [Header("My Profile Data")]
    public GetUserDetailRoot myProfileDataRoot = new GetUserDetailRoot();

    [Header("Hot Tab Data")]
    public AllUserWithFeedRoot root = new AllUserWithFeedRoot();
    public HotFeedRoot hotFeedRoot = new HotFeedRoot();
    public HotFeedRoot allhotFeedRoot = new HotFeedRoot();
    public List<AllUserWithFeedRow> allUserRootList = new List<AllUserWithFeedRow>();
    private List<AllUserWithFeedRow> hotSaveRootList = new List<AllUserWithFeedRow>();
    private AllUserWithFeedRoot hotSavejsonList = new AllUserWithFeedRoot();

    [Header("Following Tab Data")]
    public FeedsByFollowingUserRoot followingUserRoot = new FeedsByFollowingUserRoot();
    public List<FeedsByFollowingUserRow> allFollowingUserRootList = new List<FeedsByFollowingUserRow>();
    private List<FeedsByFollowingUserRow> followingUserTabSaveRootList = new List<FeedsByFollowingUserRow>();
    private FeedsByFollowingUserRoot followingUserTabSavejsonList = new FeedsByFollowingUserRoot();

    //public AllFeedRoot AlluserData = new AllFeedRoot();
    //public AllFeedRow userPostData = new AllFeedRow();
    [Header("Single User All Feed Data")]
    public AllFeedByUserIdRoot allFeedWithUserIdRoot = new AllFeedByUserIdRoot();
    public AllTextPostByUserIdRoot allTextPostWithUserIdRoot = new AllTextPostByUserIdRoot();
    //For Temp use
    public FeedResponse allTextPostFeedWithUserIdRoot = new FeedResponse();
    public TaggedFeedsByUserIdRoot taggedFeedsByUserIdRoot = new TaggedFeedsByUserIdRoot();

    public SearchUserRoot searchUserRoot = new SearchUserRoot();
    public HotUsersRoot hotUsersRoot = new HotUsersRoot();
    public AllFollowersRoot AllFollowerRoot = new AllFollowersRoot();
    public AllFollowingRoot allFollowingRoot = new AllFollowingRoot();
    public AllFollowingRoot adFrndFollowingRoot = new AllFollowingRoot();

    [Space]
    [Header("Profile Follower Following")]
    public AllFollowersRoot profileAllFollowerRoot = new AllFollowersRoot();
    public AllFollowingRoot profileAllFollowingRoot = new AllFollowingRoot();
    public AllFollowingRoot AdFrndFollowingRoot = new AllFollowingRoot();
    public UserLatestAvatarData VisitedUserAvatarData = new UserLatestAvatarData();

    [Space]
    [Header("Current Feed Comment List Response")]
    [SerializeField]
    private CommentDetails commentFeedList = new CommentDetails();

    [Space]
    [Header("Message Module")]
    public ChatGetConversationRoot allChatGetConversationRoot = new ChatGetConversationRoot();
    public ChatGetMessagesRoot allChatMessagesRoot = new ChatGetMessagesRoot();
    public ChatCreateGroupRoot ChatCreateGroupRoot = new ChatCreateGroupRoot();
    public ChatCreateMessageRoot AllChatCreateMessageRoot = new ChatCreateMessageRoot();
    public ChatAttachmentsRoot AllChatAttachmentsRoot = new ChatAttachmentsRoot();
    //private Sprite sprite;    
}

public enum ExtentionType { Image, Video, Audio };

/// <summary>
/// ////////////////////////////////////////////ALL API Classes///////////////////////////////////////////////////////////
/// </summary>

[System.Serializable]
public class GetUserDetailProfileData
{
    public int id;
    public int userId;
    public string gender;
    public string job;
    public string country;
    public string website;
    public string bio;
    public string username; // Unique UserName
    public bool isDeleted;
    public DateTime createdAt;
    public DateTime updatedAt;
}

[System.Serializable]
public class GetUserDetailData
{
    public int id;
    public string name;
    public string dob;
    public string phoneNumber;
    public string email;
    public string avatar;
    public int role;
    public string coins;
    public bool isVerified;
    public bool isRegister;
    public bool isDeleted;
    public string[] tags;
    public DateTime createdAt;
    public DateTime updatedAt;
    public GetUserDetailProfileData userProfile;
    public int followerCount;
    public int followingCount;
    public int feedCount;
}

[System.Serializable]
public class GetUserDetailRoot
{
    public bool success;
    public GetUserDetailData data;
    public string msg;
}

[System.Serializable]
public class UpdateUserAvatarRoot
{
    public bool success;
    public object data;
    public string msg;
}

[System.Serializable]
public class UpdateUserProfileData
{
    public bool isDeleted;
    public int id;
    public string gender;
    public string job;
    public string country;
    public string bio;
    public int userId;
    public DateTime updatedAt;
    public DateTime createdAt;
}

[System.Serializable]
public class UpdateUserProfileRoot
{
    public bool success;
    public UpdateUserProfileData data;
    public string msg;
}

[System.Serializable]
public class DeletAccountRoot
{
    public bool success;
    public object data;
    public string msg;
}

[System.Serializable]
public class WebSiteValidRoot
{
    public bool success;
    public string data;
    public string msg;
}

#region Feed Classes................................................................................
//.................................Other userRole class.............................
public class SingleUserRoleRoot
{
    public bool success;
    public List<string> data;
}
//.................................................................

//.................................single user profile class.............................
[System.Serializable]
public class SingleUserProfile
{
    public int id;
    public int userId;
    public string gender;
    public string job;
    public string country;
    public string website;
    public string bio;
}

[System.Serializable]
public class SingleUserProfileData
{
    public int id;
    public string name;
    public string email;
    public string avatar;
    public string[] tags;
    public SingleUserProfile userProfile;
    public List<FollowerFollowingUserAvatarData> userOccupiedAssets;
    public int followerCount;
    public int followingCount;
    public int feedCount;
    public bool isFollowing;
}

[System.Serializable]
public class SingleUserProfileRoot
{
    public bool success;
    public SingleUserProfileData data;
    public string msg;
}
//.................................................................

[System.Serializable]
public class AllUserWithFeed
{
    public int id;
    public string title;
    public string descriptions;
    public string image;
    public string video;
    public string thumbnail;
    public int likeCount;
    public bool isAllowComment;
    public bool isHide;
    public bool isDeleted;
    public int createdBy;
    public DateTime createdAt;
    public DateTime updatedAt;
    public bool isLike;
    public int commentCount;
}

[System.Serializable]
public class AllUserWithFeedUserProfile
{
    public int id;
    public int userId;
    public string gender;
    public string job;
    public string country;
    public string website;
    public string bio;
    public bool isDeleted;
    public DateTime createdAt;
    public DateTime updatedAt;
}

[System.Serializable]
public class AllUserWithFeedRow
{
    public int id;
    public string name;
    public string dob;
    public string phoneNumber;
    public string email;
    public string avatar;
    public int role;
    public bool isVerified;
    public bool isRegister;
    public bool isDeleted;
    public DateTime createdAt;
    public DateTime updatedAt;
    public List<AllUserWithFeed> feeds;
    public AllUserWithFeedUserProfile UserProfile;
    public int FollowerCount;
    public int FollowingCount;
    public int feedCount;
}

[System.Serializable]
public class AllUserWithFeedData
{
    public int count;
    public List<AllUserWithFeedRow> rows;
}

[System.Serializable]
public class AllUserWithFeedRoot
{
    public bool success;
    public AllUserWithFeedData data = new AllUserWithFeedData();
    public string msg;
}

[System.Serializable]
public class FeedsByFollowingUser
{
    public int Id;
    public string Name;
    public string Email;
    public string Avatar;
}

[System.Serializable]
public class FeedsByFollowingUserFeedComment
{
    public int Id;
    public int FeedId;
    public string Comment;
    public int CreatedBy;
    public DateTime CreatedAt;
    public DateTime UpdatedAt;
    public FeedsByFollowingUser User;
}

[System.Serializable]
public class FeedsByFollowingUserFeedTag
{
    public int id;
    public int feedId;
    public int userId;
    public DateTime createdAt;
    public DateTime updatedAt;
    public FeedsByFollowingUser user;
}

[System.Serializable]
public class FeedsByFollowingUserRow
{
    public int Id;
    public string Title;
    public string Descriptions;
    public string Image;
    public string Video;
    public string thumbnail;
    public int LikeCount;
    public bool IsAllowComment;
    public bool IsHide;
    public bool IsDeleted;
    public int CreatedBy;
    public DateTime CreatedAt;
    public DateTime UpdatedAt;
    public List<FeedsByFollowingUserFeedComment> FeedComments;
    public FeedsByFollowingUser User;
    public List<FeedsByFollowingUserFeedTag> FeedTags;
    public bool isLike;
    public int commentCount;
}

[System.Serializable]
public class FeedsByFollowingUserData
{
    public int Count;
    public List<FeedsByFollowingUserRow> Rows;
}

[System.Serializable]
public class FeedsByFollowingUserRoot
{
    public bool Success;
    public FeedsByFollowingUserData Data = new FeedsByFollowingUserData();
    public string Msg;
}

[System.Serializable]
public class TaggedFeedsByUserIdFeed
{
    public int id;
    public string title;
    public string descriptions;
    public string image;
    public string video;
    public int likeCount;
    public bool isAllowComment;
    public bool isHide;
    public bool isDeleted;
    public int createdBy;
    public DateTime createdAt;
    public DateTime updatedAt;
    public bool isLike;
    public int commentCount;
}

[System.Serializable]
public class TaggedFeedsByUserIdRow
{
    public int id;
    public int feedId;
    public int userId;
    public DateTime createdAt;
    public DateTime updatedAt;
    public TaggedFeedsByUserIdFeed feed;
}

[System.Serializable]
public class TaggedFeedsByUserIdData
{
    public int count;
    public List<TaggedFeedsByUserIdRow> rows;
}

[System.Serializable]
public class TaggedFeedsByUserIdRoot
{
    public bool success;
    public TaggedFeedsByUserIdData data;
    public string msg;
}

[System.Serializable]
public class AllFeedByUserIdRow
{
    public int Id;
    public string Title;
    public string Descriptions;
    public string Image;
    public string Video;
    public string thumbnail;
    public int LikeCount;
    public bool IsAllowComment;
    public bool IsHide;
    public bool IsDeleted;
    public int CreatedBy;
    public DateTime CreatedAt;
    public DateTime UpdatedAt;
    public bool isLike;
    public int commentCount;
}

[System.Serializable]
public class AllFeedByUserIdData
{
    public int Count;
    public List<AllFeedByUserIdRow> Rows;
}

[System.Serializable]
public class AllFeedByUserIdRoot
{
    public bool Success;
    public AllFeedByUserIdData Data;
    public string Msg;
}

[System.Serializable]
public class AllTextPostByUserIdRoot
{
    public bool success;
    public AllTextPostByUserIdData data;
    public string msg;
}

[System.Serializable]
public class AllTextPostByUserIdData
{
    public int Count;
    public List<FeedResponseRow> rows;
}

[System.Serializable]
public class AllTextPostByUserIdRow
{
    public int id;
    public int user_id;
    public string text_post;
    public string text_mood;
    public int like_count;
    public DateTime createdAt;
    public DateTime updatedAt;
    public bool isLikedByUser;
    public UserData user;
}

[System.Serializable]
public class UserData
{
    public int id;
    public string name;
    public string avatar;
}

/// <summary>
/// All Following Classes
/// </summary>
[System.Serializable]
public class HotFeedData
{
    public int count;
    public List<HotFeed> rows = new List<HotFeed>();
}

[System.Serializable]
public class HotFeedRoot
{
    public bool success;
    public HotFeedData data = new HotFeedData();
    public string msg;
}

[System.Serializable]
public class HotFeed
{
    public int id;
    public string title;
    public string descriptions;
    public string image;
    public string video;
    public string thumbnail;
    public int likeCount;
    public bool isAllowComment;
    public bool isHide;
    public bool isDeleted;
    public int createdBy;
    public DateTime createdAt;
    public DateTime updatedAt;
    public HotFeedUser user = new HotFeedUser();
    public List<string> feedTags = new List<string>();
    public bool isLike;
    public int commentCount;
}

[System.Serializable]
public class FeedComment
{
    public int id;
    public int feedId;
    public string comment;
    public int createdBy;
    public DateTime createdAt;
    public DateTime updatedAt;
    public User user;
}
[System.Serializable]
public class HotFeedUser
{
    public int id;
    public string name;
    public string email;
    public string avatar;
}

/// /// <summary>
/// User Latest Avatar Data Classes
/// </summary>
/// 
[System.Serializable]
public class UserLatestAvatarRoot
{
    public string success;
    public UserLatestAvatarData data;
    public string msg;
}

//[System.Serializable]
//public class UserLatestAvatarRows
//{
//    public int count;
//    public List<UserLatestAvatarData> rows;
//}

[System.Serializable]
public class UserLatestAvatarData
{
    public int id;
    public string name;
    public string thumbnail;
    public SavingCharacterDataClass json;
    public string description;
    public bool isDeleted;
    public int createdBy;
    public DateTime createdAt;
    public DateTime updatedAt;
    public LatestAvatarUserData user;
}

[System.Serializable]
public class LatestAvatarUserData
{
    public int id;
    public string name;
    public string avatar;
}

/// /// <summary>
/// All Following Classes
/// </summary>
/// 

[System.Serializable]
public class FollowerFollowingUserAvatarData
{
    public int id;
    public string name;
    public string thumbnail;
    public SavingCharacterDataClass json;
    public string description;
    public bool isDeleted;
    public int createdBy;
    public DateTime createdAt;
    public DateTime updatedAt;
}

[System.Serializable]
public class AllFollowing
{
    public int id;
    public string name;
    public string email;
    public string avatar;
    public bool is_close_friend;
    public bool isFollowing;
    public AllUserWithFeedUserProfile userProfile;
}

[System.Serializable]
public class AllFollowingRow
{
    public int id;
    public int userId;
    public bool isFav;
    public int followedBy;
    public DateTime createdAt;
    public DateTime updatedAt;
    public AllFollowing following;
    public List<FollowerFollowingUserAvatarData> userOccupiedAssets;
    public int followerCount;
    public int followingCount;
    public int feedCount;
    public bool isFollowing;
}

[System.Serializable]
public class AllFollowingData
{
    public int count;
    public List<AllFollowingRow> rows;
}

[System.Serializable]
public class AllFollowingRoot
{
    public bool success;
    public AllFollowingData data;
    public string msg;
}

[System.Serializable]
public class CloseFrndRoot
{
    public bool success;
    public CloseFrndData data;
    public string msg;
}

[System.Serializable]
public class CloseFrndData
{
    public int count;
    public List<CloseFrndRow> rows;
    public string msg;
}


[System.Serializable]
public class CloseFrndRow
{
    public int id;
    public string name;
    public string thumbnail;
    public Row json;
}

[System.Serializable]
public class AdCloseFrndRoot
{
    public bool success;
    public AdCloseFrndRow data;
    public string msg;
}

//[System.Serializable]
//public class AdCloseFrndData
//{
//    public int count;
//    public List<AdCloseFrndRow> rows;
//}


[System.Serializable]
public class AdCloseFrndRow
{
    public int id;
    public int userId;
    public int friendId;
    public DateTime updatedAt;
    public DateTime createdAt;
}


//---------------------------------------------------

/// <summary>
/// All Follower Calsses
/// </summary>
[System.Serializable]
public class AllFollower
{
    public int id;
    public string name;
    public string email;
    public string avatar;
    public AllUserWithFeedUserProfile userProfile;

}

[System.Serializable]
public class AllFollowersRows
{
    public int id;
    public int userId;
    public bool isFav;
    public int followedBy;
    public DateTime createdAt;
    public DateTime updatedAt;
    public AllFollower follower;
    public List<FollowerFollowingUserAvatarData> userOccupiedAssets;
    public int followerCount;
    public int followingCount;
    public int feedCount;
    public bool isFollowing;
    public bool isFriend;
}

[System.Serializable]
public class AllFollowersData
{
    public int count;
    public List<AllFollowersRows> rows;
}

[System.Serializable]
public class AllFollowersRoot
{
    public bool success;
    public AllFollowersData data;
    public string msg;
}
//-----------------------------------------------

[System.Serializable]
public class AllFollowAUserData
{
    public bool isFav;
    public int id;
    public int followedBy;
    public int userId;
    public DateTime updatedAt;
    public DateTime createdAt;
}

[System.Serializable]
public class AllFollowAUserRoot
{
    public bool success;
    public AllFollowAUserData data;
    public string msg;
}

[System.Serializable]
public class MakeAllFavouriteFollowerRoot
{
    public bool success;
    public string data;
    public string msg;
}

[System.Serializable]
public class AllFeedRow
{
    public int id;
    public string title;
    public string descriptions;
    public string image;
    public string video;
    public int likeCount;
    public bool isAllowComment;
    public bool isHide;
    public bool isDeleted;
    public int createdBy;
    public DateTime createdAt;
    public DateTime updatedAt;
    public List<string> feedComments;
    public List<string> feedTags;
}

[System.Serializable]
public class AllFeedData
{
    public int count;
    public List<AllFeedRow> rows;
}

[System.Serializable]
public class AllFeedRoot
{
    public bool success;
    public AllFeedData data;
    public string msg;
}

[System.Serializable]
public class AllCreateFeedData
{
    public int likeCount;
    public bool isHide;
    public bool isDeleted;
    public int id;
    public string title;
    public string descriptions;
    public string image;
    public string video;
    public bool isAllowComment;
    public int createdBy;
    public DateTime updatedAt;
    public DateTime createdAt;
}

[System.Serializable]
public class AllCreateFeedRoot
{
    public bool success;
    public AllCreateFeedData data;
    public string msg;
}

[System.Serializable]
public class SearchUserRow
{
    public int id;
    public string name;
    public string avatar;
    public int followingCount;
    public int feedCount;
    public int followerCount;
    public bool is_following_me;
    public bool am_i_following;
    public bool is_close_friend;
    public AllUserWithFeedUserProfile userProfile;
    public List<FollowerFollowingUserAvatarData> userOccupiedAssets;
}

[System.Serializable]
public class SearchUserData
{
    public int count;
    public List<SearchUserRow> rows;
}

[System.Serializable]
public class SearchUserRoot
{
    public bool success;
    public SearchUserData data;
    public string msg;
}

/// <summary>
/// Feed Edit or Delete Option Class.......
/// </summary>
/// 
[System.Serializable]
public class FeedEditOrDeleteData
{
    public int feedId;
    public string feedTitle;
    public string feedDescriptions;
    public string feedImage;
    public string feedVideo;
    public int feedCreatedBy;
    public DateTime CreatedAt;
    public DateTime UpdatedAt;
    public FeedsByFollowingUser userData;
}
//----------------------------------------------------

/// <summary>
/// Feed Like or DisLike Class.......
/// </summary>
/// 
[System.Serializable]
public class FeedLikeDisLikeData
{
    public int id;
    public int feedId;
    public int createdBy;
    public DateTime updatedAt;
    public DateTime createdAt;
    public int likeCount;
}

[System.Serializable]
public class FeedLikeDisLikeRoot
{
    public bool success;
    public FeedLikeDisLikeData data;
    public string msg;
}
//----------------------------------------------------
#endregion

#region chat classes........................................................................
[System.Serializable]
public class ChatMuteUnMuteRoot
{
    public bool success;
    public string msg;
}


[System.Serializable]
public class ChatCreateGroupData
{
    public int id;
    public string name;
    public int createdBy;
    public string avatar;
    public DateTime updatedAt;
    public DateTime createdAt;
}

[System.Serializable]
public class ChatCreateGroupRoot
{
    public bool success;
    public ChatCreateGroupData data;
    public string msg;
}

[System.Serializable]
public class ChatGetConversationReceiver
{
    public int id;
    public string name;
    public string email;
    public string avatar;
}

[System.Serializable]
public class ChatGetConversationSender
{
    public int id;
    public string name;
    public string email;
    public string avatar;
}

[System.Serializable]
public class ChatGetConversationUser
{
    public int id;
    public string name;
    public string email;
    public string avatar;
}

[System.Serializable]
public class ChatGetConversationGroupUser
{
    public int id;
    public int userId;
    public int groupId;
    public DateTime createdAt;
    public DateTime updatedAt;
    public ChatGetConversationUser user;
    public bool isFollowing;
}

[System.Serializable]
public class ChatGetConversationGroup
{
    public int id;
    public string name;
    public string avatar;
    public int createdBy;
    public bool isDeleted;
    public List<ChatGetConversationGroupUser> groupUsers = new List<ChatGetConversationGroupUser>();
}
[System.Serializable]
public class ChatGetConversationsReadCount
{
    public int id;
    public int conversationId;
    public int userId;
    public int unReadCount;
}

[System.Serializable]
public class ChatGetConversationDatum
{
    public int id;
    public int receiverId;
    public int receivedGroupId;
    public int senderId;
    public string lastMsg;
    public DateTime createdAt;
    public DateTime updatedAt;
    public bool isDeleted;
    public ChatGetConversationReceiver ConReceiver;
    public ChatGetConversationSender ConSender;
    public ChatGetConversationGroup group;
    public List<ChatGetConversationsReadCount> conversationsReadCounts;
    public List<string> mutedConversations;
    public bool isMutedConversations;
}

[System.Serializable]
public class ChatGetConversationRoot
{
    public bool success;
    public List<ChatGetConversationDatum> data = new List<ChatGetConversationDatum>();
    public string msg;
}

[System.Serializable]
public class ChatGetMessagesReceiver
{
    public int id;
    public string name;
    public string email;
    public string avatar;
}

[System.Serializable]
public class ChatGetMessagesSender
{
    public int id;
    public string name;
    public string email;
    public string avatar;
}

[System.Serializable]
public class ChatGetMessagesAttachment
{
    public int id;
    public string url;
}

[System.Serializable]
public class ChatGetMessagesMessage
{
    public int id;
    public string msg;
    public string type;
    public List<ChatGetMessagesAttachment> attachments;
}

[System.Serializable]
public class ChatGetMessagesRow
{
    public int id;
    public int receiverId;
    public int receivedGroupId;
    public int messageId;
    public int senderId;
    public bool isRead;
    public DateTime createdAt;
    public DateTime updatedAt;
    public ChatGetMessagesReceiver receiver;
    public ChatGetMessagesSender sender;
    public ChatGetMessagesMessage message;
}

[System.Serializable]
public class ChatGetMessagesData
{
    public int count;
    public List<ChatGetMessagesRow> rows;
}

[System.Serializable]
public class ChatGetMessagesRoot
{
    public bool success;
    public ChatGetMessagesData data;
    public string msg;
}

[System.Serializable]
public class ChatCreateMessageData
{
    public int id;
    public int senderId;
    public int messageId;
    public int receiverId;
    public DateTime updatedAt;
    public DateTime createdAt;
    public int receivedGroupId;
    public int userId;
    public int groupId;
}

[System.Serializable]
public class ChatCreateMessageRoot
{
    public bool success;
    public ChatCreateMessageData data;
    public string msg;
}

[System.Serializable]
public class ChatAttachmentMessageRecipient
{
    public int id;
    public int receiverId;
    public string receivedGroupId;
    public int messageId;
    public int senderId;
    public DateTime createdAt;
    public DateTime updatedAt;
    public ChatGetMessagesReceiver receiver;
    public ChatGetMessagesSender sender;
}

[System.Serializable]
public class ChatAttachmentMessage
{
    public int id;
    public string msg;
    public string type;
    public ChatAttachmentMessageRecipient messageRecipient;
}

[System.Serializable]
public class ChatAttachmentsRow
{
    public int id;
    public string url;
    public DateTime createdAt;
    public DateTime updatedAt;
    public ChatAttachmentMessage message;
}

[System.Serializable]
public class ChatAttachmentsData
{
    public int count;
    public List<ChatAttachmentsRow> rows;
}

[System.Serializable]
public class ChatAttachmentsRoot
{
    public bool success;
    public ChatAttachmentsData data;
    public string msg;
}
#endregion

#region Feed Comment.......
///comment list
///
[System.Serializable]
public class CommentUser
{
    public int id;
    public string name;
    public string email;
    public string avatar;
}

[System.Serializable]
public class CommentRow
{
    public int id;
    public int feedId;
    public string comment;
    public int createdBy;
    public DateTime createdAt;
    public DateTime updatedAt;
    public CommentUser user;
}

[System.Serializable]
public class CommentData
{
    public int count;
    public List<CommentRow> rows;
}

[System.Serializable]
public class CommentDetails
{
    public bool success;
    public CommentData data;
    public string msg;
}

[System.Serializable]
public class CommentPost
{
    public int id;
    public int feedId;
    public string comment;
    public int createdBy;
    public DateTime createdAt;
    public DateTime updatedAt;
    public CommentUser user;
}

[System.Serializable]
public class CommentPostData
{
    public int count;
    public CommentPost commentPost;
}

[System.Serializable]
public class CommentPostDetail
{
    public bool success;
    public CommentPostData data;
    public string msg;
}
#endregion
#region Hot Users API Classes //Most Active Users in 24 hours
[System.Serializable]
public class HotUsersRoot
{
    public bool success;
    public HotUsersData data;
    public string msg;
}
[System.Serializable]
public class HotUsersData
{
    public int count;
    public List<HotUsersRow> rows;
}
[System.Serializable]
public class HotUsersRow
{
    public int totalActivityCount;
    public bool is_following_me;
    public bool am_i_following;
    public bool is_close_friend;
    public SearchUserRow user;
}
#endregion