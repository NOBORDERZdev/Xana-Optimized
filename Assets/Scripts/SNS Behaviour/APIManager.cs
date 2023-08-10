using AdvancedInputFieldPlugin;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class APIManager : MonoBehaviour
{
    public static APIManager Instance;

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
        Debug.Log("<color=red> APIManager Start UserToken:" + ConstantsGod.AUTH_TOKEN + "    :userID:" + PlayerPrefs.GetString("UserName") + "</color>");
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
        yield return new WaitForSeconds(2f);
        RequestGetFeedsByFollowingUser(1, 10, callingFrom);
    }
    public void LoadJson()
    {
        using (StreamReader r = new StreamReader(Application.persistentDataPath + "/FeedData.json"))
        {
            string json = r.ReadToEnd();
            //Debug.Log("json " + json);
            StartCoroutine(SaveAndLoadJson(json, 0, 1, ""));
            //  FeedUIController.Instance.isDataLoad = true;
        }
    }

    public void LoadJsonFollowingFeed()
    {
        using (StreamReader r = new StreamReader(Application.persistentDataPath + "/FeedFollowingData.json"))
        {
            string json = r.ReadToEnd();
            //Debug.Log("json " + json);
            StartCoroutine(SaveAndLoadJsonFollowingFeed(json, 0, 1, ""));
            //  FeedUIController.Instance.isDataLoad = true;
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
        // FeedUIController.Instance.ApiLoaderScreen.SetActive(true);
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

            if (LoaderController.Instance != null)//main feed top loader start
            {
                LoaderController.Instance.isLoaderGetApiResponce = false;
            }

            yield return www.SendWebRequest();

            if (LoaderController.Instance != null)//main feed top loader stop
            {
                LoaderController.Instance.isLoaderGetApiResponce = true;
            }

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                if (FeedUIController.Instance.allFeedMessageTextList[0].gameObject.activeSelf)
                {
                    FeedUIController.Instance.AllFeedScreenMessageTextActive(true, 0, TextLocalization.GetLocaliseTextByKey("bad internet connection please try again"));
                }
                if (FeedUIController.Instance.allFeedMessageTextList[2].gameObject.activeSelf)
                {
                    FeedUIController.Instance.AllFeedScreenMessageTextActive(true, 2, TextLocalization.GetLocaliseTextByKey("bad internet connection please try again"));
                }
            }
            else
            {
                //Debug.Log("Form upload complete! IERequestGetAllUsersWithFeeds pageNum:" + pageNum + "   :pageSize:" + pageSize);
                string data = www.downloadHandler.text;
                //Debug.Log("IERequestGetAllUsersWithFeeds PageNum:" + pageNum + "    :PageSize:" + pageSize + "     :Data:" + data);
                // FeedUIController.Instance.ApiLoaderScreen.SetActive(false);

                StartCoroutine(SaveAndLoadJson(data, 1, pageNum, callingFrom));
                //LoaderController.Instance.isLoaderGetApiResponce = true;
                //FeedUIController.Instance.allFeedCurrentpage += 1;
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
        //FeedUIController.Instance.ApiLoaderScreen.SetActive(true);
        requestGetFeedsByFollowingUserCoroutine = StartCoroutine(IERequestGetFeedsByFollowingUser(pageNum, pageSize, callingFrom));
    }
    public IEnumerator IERequestGetFeedsByFollowingUser(int pageNum, int pageSize, string callingFrom)
    {
        //yield return new WaitForSeconds(0.5f);
        using (UnityWebRequest www = UnityWebRequest.Get((ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetFeedsByFollowingUser + "/" + pageNum + "/" + pageSize)))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            if (LoaderController.Instance != null)//main feed top loader start
            {
                LoaderController.Instance.isLoaderGetApiResponce = false;
            }

            yield return www.SendWebRequest();

            if (LoaderController.Instance != null)//main feed top loader stop
            {
                LoaderController.Instance.isLoaderGetApiResponce = true;
            }

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);

                if (FeedUIController.Instance.allFeedMessageTextList[1].gameObject.activeSelf)
                {
                    FeedUIController.Instance.AllFeedScreenMessageTextActive(true, 1, TextLocalization.GetLocaliseTextByKey("bad internet connection please try again"));
                }
            }
            else
            {
                string data = www.downloadHandler.text;
               Debug.Log("Form upload complete! IERequestGetFeedsByFollowingUser pageNum:" + pageNum + "   :pageSize:" + pageSize + " :Data:" + data);
                //FeedUIController.Instance.ApiLoaderScreen.SetActive(false);
                //  followingUserRoot = JsonConvert.DeserializeObject<FeedsByFollowingUserRoot>(data);
                // APIController.Instance.OnGetAllFeedForFollowingTab();

                StartCoroutine(SaveAndLoadJsonFollowingFeed(data, 1, pageNum, callingFrom));
                //LoaderController.Instance.isLoaderGetApiResponce = true;
                //FeedUIController.Instance.followingUserCurrentpage += 1;
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
            if (!APIController.Instance.feedHotIdList.Contains(hotFeedRoot.data.rows[i].id))
            {
                //APIController.Instance.feedHotIdList.Add(hotFeedRoot.data.rows[i].id);
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
        APIController.Instance.AllUsersWithHotFeeds(callingFrom);
        //Debug.Log("Feed Load");
        FeedUIController.Instance.myPostCurrentPage += 1;
        RequestGetFeedsByUserId(userId, (FeedUIController.Instance.myPostCurrentPage), 10, "FeedPage");
        yield return new WaitForSeconds(1f);
        MyProfileDataManager.Instance.AllFeedWithUserId((FeedUIController.Instance.myPostCurrentPage), FeedUIController.Instance.forYouFeedTabContainer);
        //APIController.Instance.AllUserForYouFeeds(pageNum, callingFrom);
        StartCoroutine(APIController.Instance.HotWaitToEnableDataLoadedBool(pageNum));
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
        MyProfileDataManager.Instance.AllFeedWithUserId((1), FeedUIController.Instance.forYouFeedTabContainer, true);
        yield return new WaitForSeconds(.5f);
        APIManager.Instance.RequestGetAllUsersWithFeeds(1, 10, "PullRefresh");
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

        APIController.Instance.OnGetAllFeedForFollowingTab(pageNum, callingFrom);

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
                APIController.Instance.RemoveFollowingItemAndResetData(matches[k].Id);
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

        FeedUIController.Instance.unFollowedUserListForFollowingTab.Clear();//clear 

        Resources.UnloadUnusedAssets();
        //Caching.ClearCache();
        //GC.Collect();
        if (FeedUIController.Instance.followingFeedTabContainer.childCount <= 0)
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
    public void RequestGetFeedsByUserId(int userId, int pageNum, int pageSize, string callingFrom)
    {
        StartCoroutine(IERequestGetFeedsByUserId(userId, pageNum, pageSize, callingFrom));
    }
    public IEnumerator IERequestGetFeedsByUserId(int userId, int pageNum, int pageSize, string callingFrom)
    {
        using (UnityWebRequest www = UnityWebRequest.Get((ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetFeedsByUserId + "/" + userId + "/" + pageNum + "/" + pageSize)))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                FeedUIController.Instance.ShowLoader(false);

                switch (callingFrom)
                {
                    case "OtherPlayerFeed":
                        if (OtherPlayerProfileData.Instance != null && pageNum == 1)
                        {
                            OtherPlayerProfileData.Instance.RemoveAndCheckBackKey();
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                string data = www.downloadHandler.text;
                //Debug.Log("IERequestGetFeedsByUserId success data" + data);
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                AllFeedByUserIdRoot test = JsonConvert.DeserializeObject<AllFeedByUserIdRoot>(data, settings);
                if (allFeedWithUserIdRoot.Data.Rows.Count > test.Data.Rows.Count)
                {
                    //allFeedWithUserIdRoot.Data.Rows.Clear(); //Riken
                    for (int i = 0; i < test.Data.Rows.Count; i++)
                    {
                        // myList.Where(p => p.Name == nameToExtract);
                        // allFeedWithUserIdRoot.Data.Rows.Where(p => p.Id == test.Data.Rows[i].Id);

                        if (!allFeedWithUserIdRoot.Data.Rows.Any(x => x.Id == test.Data.Rows[i].Id))
                        {
                            allFeedWithUserIdRoot.Data.Rows.Add(test.Data.Rows[i]);
                        }
                        //    if (!allFeedWithUserIdRoot.Data.Rows.Contains(test.Data.Rows[i]))
                        //{
                        //}
                    }
                }
                else
                {

                    allFeedWithUserIdRoot = test;
                }
                if (callingFrom == "OtherPlayerFeed")
                {
                    allFeedWithUserIdRoot = test;
                }
                switch (callingFrom)
                {
                    case "OtherPlayerFeed":
                        OtherPlayerProfileData.Instance.AllFeedWithUserId(pageNum);
                        break;
                    case "MyProfile":
                        MyProfileDataManager.Instance.AllFeedWithUserId(pageNum);
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
        //  FeedUIController.Instance.ApiLoaderScreen.SetActive(true);
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
                //  FeedUIController.Instance.ApiLoaderScreen.SetActive(false);
               Debug.Log("taggedFeedsByUserIdRoot" + data);
                taggedFeedsByUserIdRoot = JsonConvert.DeserializeObject<TaggedFeedsByUserIdRoot>(data);
                StartCoroutine(OtherPlayerProfileData.Instance.AllTagFeed());
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
                    MessageController.Instance.LoaderShow(false);//False api loader.
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
                        APIController.Instance.GetAllFollowingUser(pageNum);
                        break;
                    default:
                        break;
                }
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
                        APIController.Instance.GetSetAllfollowerInTopStoryPanelUser();
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
                if (FeedUIController.Instance != null)
                {
                    FeedUIController.Instance.ShowLoader(false);
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
                        FeedUIController.Instance.ShowLoader(false);
                        //OtherPlayerProfileData.Instance.OnSetUserUi(true);
                        OtherPlayerProfileData.Instance.OnFollowerIncreaseOrDecrease(true);//Inscrease follower count.......
                        //OtherPlayerProfileData.Instance.DestroyUserFromHotTabAfterFollow();

                        FeedUIController.Instance.FollowingAddAndRemoveUnFollowedUser(int.Parse(user_Id), false);
                        break;
                    case "Feed":
                        if (FeedUIController.Instance != null)
                        {
                            StartCoroutine(WaitToFalseLoader());
                        }

                        APIController.Instance.currentFeedRawItemController.OnFollowUserSuccessful();
                        APIController.Instance.currentFeedRawItemController.isFollow = true;
                        APIController.Instance.currentFeedRawItemController = null;
                        //OtherPlayerProfileData.Instance.OnSetUserUi(APIController.Instance.currentFeedRawItemController.isFollow);                        
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
                if (FeedUIController.Instance != null)
                {
                    FeedUIController.Instance.ShowLoader(false);
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
                        FeedUIController.Instance.ShowLoader(false);
                        //OtherPlayerProfileData.Instance.OnSetUserUi(false);
                        OtherPlayerProfileData.Instance.OnFollowerIncreaseOrDecrease(false);//Descrease follower count.......

                        FeedUIController.Instance.FollowingAddAndRemoveUnFollowedUser(int.Parse(user_Id), true);
                        break;
                    case "Feed":
                        if (FeedUIController.Instance != null)
                        {
                            StartCoroutine(WaitToFalseLoader());
                        }

                        APIController.Instance.currentFeedRawItemController.OnFollowUserSuccessful();
                        //OtherPlayerProfileData.Instance.OnSetUserUi(APIController.Instance.currentFeedRawItemController.isFollow);                        
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
        FeedUIController.Instance.ShowLoader(false);
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

            //FeedUIController.Instance.ShowLoader(false);

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.Log("<color = red> GetAllFollowersFromProfile data:" + data + "</color>");
                profileAllFollowerRoot = JsonUtility.FromJson<AllFollowersRoot>(data);

                FeedUIController.Instance.ProfileGetAllFollower(pageNum);
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
        using (UnityWebRequest www = UnityWebRequest.Get((ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetAllFollowing + "/" + user_Id + "/" + pageNum + "/" + pageSize)))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            //FeedUIController.Instance.ShowLoader(false);

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.Log("<color = red> GetAllFollowingFromProfile data:" + data + "</color>");
                profileAllFollowingRoot = JsonConvert.DeserializeObject<AllFollowingRoot>(data);

                FeedUIController.Instance.ProfileGetAllFollowing(pageNum);
            }
        }
    }
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
                Debug.Log("<color = red> IERequestCommentFeed success data:" + data + "  :Feed Id:" + feed_feedId + "   :Feed_Comment:" + feed_comment  + "</color>");

                //CommentPostDetail bean = Gods.DeserializeJSON<CommentPostDetail>(data.Trim());
                CommentPostDetail bean = JsonConvert.DeserializeObject<CommentPostDetail>(data);

                //if (!bean.Equals("") || !bean.Equals(null))
                if (bean.data != null)
                {
                    CommentCountTextSetup(bean.data.count);//set comment count on commet panel.......

                    if (bean.data.commentPost != null)
                    {
                        GameObject CommentObject = Instantiate(FeedUIController.Instance.commentListItemPrefab, FeedUIController.Instance.commentContentPanel.transform);

                        if (!checkText.Equals("Oldest"))
                        {
                            CommentObject.transform.SetAsFirstSibling();
                            FeedUIController.Instance.commentContentPanel.transform.GetChild(1).SetAsFirstSibling();
                        }

                        //FeedUIController.Instance.CommentCount.text = bean.data.count.ToString();

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
                            FeedUIController.Instance.commentScrollPosition.verticalNormalizedPosition = 0f;
                        }
                        else if (checkText.Equals("Newest"))
                        {
                            FeedUIController.Instance.commentScrollPosition.verticalNormalizedPosition = 1f;
                        }

                        FeedUIController.Instance.CommentSuccessAfterUpdateRequireFeedResponse();
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
        if (FeedUIController.Instance.commentContentPanel.transform.childCount > 1)
        {
            foreach (Transform child in FeedUIController.Instance.commentContentPanel.transform)
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
        FeedUIController.Instance.commentFitertextDropdown.text = TextLocalization.GetLocaliseTextByKey(text);

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
            while (FeedUIController.Instance.commentContentPanel.transform.childCount > 1)
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
                    //FeedUIController.Instance.CommentCount.text = commentFeedList.data.count.ToString();
                    CommentCountTextSetup(commentFeedList.data.count);//set comment count on commet panel.......

                    for (int i = 0; i < commentFeedList.data.rows.Count; i++)
                    {
                        GameObject CommentObject = Instantiate(FeedUIController.Instance.commentListItemPrefab, FeedUIController.Instance.commentContentPanel.transform);

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
        if (GameManager.currentLanguage == "ja" || CustomLocalization.forceJapanese)
        {
            FeedUIController.Instance.CommentCount.text = TextLocalization.GetLocaliseTextByKey("Comments") + "<color=blue>" + count.ToString() + "</color>" + TextLocalization.GetLocaliseTextByKey("s");
        }
        else
        {
            FeedUIController.Instance.CommentCount.text = "<color=blue>" + count.ToString() + "</color> " + TextLocalization.GetLocaliseTextByKey("Comments");
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
            if (AWSHandler.Instance.currentSNSApiLoaderController != null)
            {
                AWSHandler.Instance.currentSNSApiLoaderController.ShowUploadStatusImage(false);
            }
            switch (callingFrom)
            {
                case "MyProfileCreateFeed":
                    if (FeedUIController.Instance != null)
                    {
                        FeedUIController.Instance.ShowLoader(false);//false api loader.......
                        FeedUIController.Instance.OnClickCreateFeedBackBtn(true);
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
                        if (MyProfileDataManager.Instance != null)
                        {
                            MyProfileDataManager.Instance.ProfileTabButtonClick();
                            Invoke(nameof(LoadMyPost),0);
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
        SNSNotificationManager.Instance.DeleteLoaderShow(true);//delete loader active

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

            SNSNotificationManager.Instance.DeleteLoaderShow(false);//delete loader disable
            //FeedUIController.Instance.ShowLoader(false);

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
                        FeedUIController.Instance.OnSuccessDeleteFeed();
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

            FeedUIController.Instance.ShowLoader(false);

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
                FeedUIController.Instance.OnSuccessFeedEdit();
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
                    FeedUIController.Instance.LikeDislikeSuccessAfterUpdateRequireFeedResponse(false, feedLikeDisLikeRoot.data.likeCount);
                }
                else
                {
                    FeedUIController.Instance.LikeDislikeSuccessAfterUpdateRequireFeedResponse(true, feedLikeDisLikeRoot.data.likeCount);
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

        form.AddField("name", name);

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_SearchUser), form))
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
                APIController.Instance.FeedGetAllSearchUser();
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
                    MyProfileDataManager.Instance.ShowEditProfileNameErrorMessage("Username already exists");
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
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
               Debug.Log("IERequestGetUserDetails error:" + www.error);
                if (FeedUIController.Instance != null)
                {
                    FeedUIController.Instance.ShowLoader(false);
                    switch (callingFrom)
                    {
                        case "EditProfileAvatar":
                            MyProfileDataManager.Instance.EditProfileDoneButtonSetUp(true);//setup edit profile done button.......
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                //Debug.Log("IERequestGetUserDetails Form upload complete!");
                string data = www.downloadHandler.text;
               Debug.Log("IERequestGetUserDetails Loaded Completed data:" + data + "      :Calling From:" + callingFrom);
                //Debug.Log("callingFrom" + callingFrom);
                myProfileDataRoot = JsonUtility.FromJson<GetUserDetailRoot>(data);
                switch (callingFrom)
                {
                    case "myProfile":
                        MyProfileDataManager.Instance.SetupData(myProfileDataRoot.data, callingFrom);//setup and load my profile data.......                        
                        break;
                    case "EditProfileAvatar":
                        MyProfileDataManager.Instance.SetupData(myProfileDataRoot.data, callingFrom);//setup and load my profile data.......
                        break;
                    case "messageScreen":
                        MessageController.Instance.GetSuccessUserDetails(myProfileDataRoot.data);
                        break;
                    case "MyAccount":
                        MyProfileDataManager.Instance.myProfileData = myProfileDataRoot.data;
                        SNSSettingController.Instance.SetUpPersonalInformationScreen();
                        break;
                    default:
                        break;
                }

                PlayerPrefs.SetString("PlayerName", myProfileDataRoot.data.name);
            }
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
                        MyProfileDataManager.Instance.AfterUpdateAvatarSetTempSprite();
                        break;
                    default:
                        break;
                }
            }
        }
    }

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
                Debug.Log("<color = red> UpdateUserProfile data:" + data + "</color>");
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
                if (MessageController.Instance.startAndWaitMessageText.gameObject.activeSelf)
                {
                    MessageController.Instance.StartAndWaitMessageTextActive(true, TextLocalization.GetLocaliseTextByKey("bad internet connection please try again"));//start and wait message text show.......
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

                APIController.Instance.GetAllConversation();
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

            MessageController.Instance.LoaderShow(false);//false api loader 
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
                        MessageController.Instance.allChatGetConversationDatum.isMutedConversations = true;
                    }
                    else
                    {
                        MessageController.Instance.allChatGetConversationDatum.isMutedConversations = false;
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

            MessageController.Instance.LoaderShow(false);//rik loader false.......

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                if (message_receivedGroupId != 0)
                {
                    MessageController.Instance.ChatScreen.SetActive(true);
                    MessageController.Instance.MessageListScreen.SetActive(false);
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
                APIController.Instance.GetAllChat(message_pageNumber, "");
                switch (callingFrom)
                {
                    case "Conversation":
                        if (CommonAPIManager.Instance != null)//For Get All Chat UnRead Message Count.......
                        {
                            CommonAPIManager.Instance.RequestGetAllChatUnReadMessagesCount();
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
        MessageController.Instance.LoaderShow(true);//active api loader.

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

                MessageController.Instance.LoaderShow(false);//False api loader.

                string data = www.downloadHandler.text;
               Debug.Log("Get Attachment Error:" + data);
                AllChatAttachmentsRoot = JsonConvert.DeserializeObject<ChatAttachmentsRoot>(data);
                if (AllChatAttachmentsRoot != null)
                {
                    if (AllChatAttachmentsRoot.msg == "No attachments found")
                    {
                        if (index == 0)
                        {
                            foreach (Transform item in MessageController.Instance.chatShareAttechmentparent)
                            {
                                Destroy(item.gameObject);
                            }
                            APIController.Instance.SetChatMember();
                        }
                        else if (index == 1)
                        {
                            MessageController.Instance.NoAttechmentScreen.SetActive(true);
                            MessageController.Instance.chatShareAttechmentparent.gameObject.SetActive(false);
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

                APIController.Instance.GetAllAttachments(index);
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

                //Debug.Log("msg : " + MessageController.Instance.typeMessageText.text);
                //Debug.Log("msg : " + MessageController.Instance.chatTypeMessageInputfield.Text);
                yield return new WaitForSeconds(0.1f);

                // RequestChatCreateMessage(0, ChatCreateGroupRoot.data.id,MessageController.Instance.typeMessageText.text,"");
                // RequestChatGetConversation();
                // RequestChatGetMessages(1, 50, 0,ChatCreateGroupRoot.data.id);
                // MessageController.Instance.typeMessageText.text = "";
                // APIController.Instance.GetAllChat();
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

                MessageController.Instance.LoaderShow(false);//false api loader 
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

            MessageController.Instance.LoaderShow(false);//false loader screen.

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Debug.Log("Form upload complete!");
                string data = www.downloadHandler.text;
               Debug.Log("Update Success Group Info: " + data);
                MessageController.Instance.UpdateGroupInFoSuccessResponce();
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

                MessageController.Instance.isLeaveGroup = false;//if create message is failed  then false LeaveGroup bool.

                MessageController.Instance.LoaderShow(false);//rik loader false.......
                MessageController.Instance.OnClcikSendMessageButtonbool = false;
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

                MessageController.Instance.LoaderShow(false);//rik loader false.......
               Debug.Log("Chat CreateMessage success:" + MessageController.Instance.isLeaveGroup + "  :Data:" + data);
                if (!MessageController.Instance.isLeaveGroup)//not get message api call after leave group.......
                {
                    if (AllChatCreateMessageRoot.data.receivedGroupId != 0)
                    {
                        //Debug.Log("receivedGroupId" + AllChatCreateMessageRoot.data.receivedGroupId);
                        //MessageController.Instance.LoaderShow(true);//rik loader active.......
                        RequestChatGetMessages(1, 50, 0, AllChatCreateMessageRoot.data.receivedGroupId, "Conversation");
                    }
                    else
                    {
                        //Debug.Log("user id:" + userId + "    :receiverId:" + AllChatCreateMessageRoot.data.receiverId);
                        if (AllChatCreateMessageRoot.data.receiverId == userId)
                        {
                            //MessageController.Instance.LoaderShow(true);//rik loader active.......
                            RequestChatGetMessages(1, 50, AllChatCreateMessageRoot.data.senderId, 0, "Conversation");
                        }
                        else
                        {
                            //MessageController.Instance.LoaderShow(true);//rik loader active.......
                            RequestChatGetMessages(1, 50, AllChatCreateMessageRoot.data.receiverId, 0, "Conversation");
                        }
                        //  RequestChatGetMessages(1, 50, AllChatCreateMessageRoot.data.receiverId, 0);
                    }
                    //MessageController.Instance.typeMessageText.text = "";
                    MessageController.Instance.chatTypeMessageInputfield.Text = "";
                    MessageController.Instance.OnChatVoiceOrSendButtonEnable();
                    // Debug.Log(root.data.count);

                    if (!string.IsNullOrEmpty(MessageController.Instance.isDirectMessageFirstTimeRecivedID))
                    {
                        RequestChatGetConversation();
                    }
                }
                else
                {
                    MessageController.Instance.isLeaveGroup = false;//set leave group bool false.......
                }
                MessageController.Instance.OnClcikSendMessageButtonbool = false;
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
                        MessageController.Instance.DeleteConversationWithLeaveGroupApiResponseSuccess(groupId);
                        break;
                    case "DetailsScreen":
                        APIController.Instance.LeaveTheChatCallBack(groupId);
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
        MessageController.Instance.LoaderShow(true);//rik loader false.......
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

            MessageController.Instance.LoaderShow(false);//rik loader false.......

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Remove group member from Group Chat success!");
                string data = www.downloadHandler.text;
               Debug.Log("Remove group member data: " + data);
                MessageController.Instance.RemoveMemberApiResponseSuccess();
            }
        }
    }

    //this api is used to Delete Conversation.......
    public void RequestDeleteConversation(int conversationId)
    {
        MessageController.Instance.LoaderShow(true);//rik loader false.......
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

            MessageController.Instance.LoaderShow(false);//rik loader false.......

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Delete conversation success!");
                string data = www.downloadHandler.text;
               Debug.Log("Delete conversation data: " + data);
                MessageController.Instance.DeleteConversationApiResponseSuccess("Conversation Deleted");
            }
        }
    }

    //this api is used to Delete Conversation.......
    public void RequestDeleteChatGroup(int groupId, string callingFrom)
    {
        MessageController.Instance.LoaderShow(true);//rik loader false.......
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

            MessageController.Instance.LoaderShow(false);//rik loader false.......

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
                        MessageController.Instance.DeleteConversationApiResponseSuccess("Group Deleted");
                        break;
                    case "DetailsScreen":
                        MessageController.Instance.DeleteGroupChatApiResponseSuccess();
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
    public TaggedFeedsByUserIdRoot taggedFeedsByUserIdRoot = new TaggedFeedsByUserIdRoot();

    public SearchUserRoot searchUserRoot = new SearchUserRoot();
    public AllFollowersRoot AllFollowerRoot = new AllFollowersRoot();
    public AllFollowingRoot allFollowingRoot = new AllFollowingRoot();

    [Space]
    [Header("Profile Follower Following")]
    public AllFollowersRoot profileAllFollowerRoot = new AllFollowersRoot();
    public AllFollowingRoot profileAllFollowingRoot = new AllFollowingRoot();

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
    public SingleUserProfile userProfile;
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

/// <summary>
/// All Following Classes
/// </summary>
[System.Serializable]
public class HotFeedData
{
    public int count;
    public List<HotFeed> rows=new List<HotFeed>();
}

[System.Serializable]
public class HotFeedRoot
{
    public bool success;
    public HotFeedData data=new HotFeedData();
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
    public HotFeedUser user=new HotFeedUser();
    public List<string> feedTags=new List<string>();
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
/// All Following Classes
/// </summary>
[System.Serializable]
public class AllFollowing
{
    public int id;
    public string name;
    public string email;
    public string avatar;
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
    public int followerCount;
    public int followingCount;
    public int feedCount;
    public bool isFollowing;
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
    public AllUserWithFeedUserProfile userProfile;
    public int followerCount;
    public int followingCount;
    public int feedCount;
    public bool isFollowing;
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