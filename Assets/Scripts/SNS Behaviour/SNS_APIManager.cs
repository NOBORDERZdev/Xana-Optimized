using AdvancedInputFieldPlugin;
using BestHTTP.JSON.LitJson;
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

public class SNS_APIManager : MonoBehaviour
{
    public static SNS_APIManager Instance;

    private bool isCoroutineRunning=false;
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
    public int BFCount = 0;
    private int maxBfCount = 10;
    public AllFollowingRoot adFrndFollowing;
    GameManager gameManager;
    [SerializeField] SNS_APIController apiController;
    [SerializeField] FeedUIController feedUIController;
    [SerializeField] OtherPlayerProfileData otherPlayerProfileData;
    [SerializeField] MyProfileDataManager myProfileDataManager;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        userAuthorizeToken = ConstantsGod.AUTH_TOKEN;
        userId = int.Parse(PlayerPrefs.GetString("UserId"));
        gameManager = GameManager.Instance;
        if (apiController == null)
        {
            apiController = SNS_APIController.Instance;
        }

        if (feedUIController == null)
        {
            feedUIController = FeedUIController.Instance;
        }
        if (otherPlayerProfileData == null)
        {
            otherPlayerProfileData = OtherPlayerProfileData.Instance;
        }
        if (myProfileDataManager == null)
        {
            myProfileDataManager = MyProfileDataManager.Instance;
        }
    }

    private void OnEnable()
    {
        Instance = this;
        Debug.Log("<color=red> SNS_APIManager Start UserToken:" + ConstantsGod.AUTH_TOKEN + "    :userID:" + PlayerPrefs.GetString("UserName") + "</color>");
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
            userId = int.Parse(PlayerPrefs.GetString("UserId"));
            userName = PlayerPrefs.GetString("UserName");
        }
        else
        {
            ConstantsGod.API_BASEURL = "https://api-test.xana.net";
        }
    }

    void Start()
    {
         GetBestFriend();
    }


    #region HotAPI..........   
    //this api is used to get feed for single user.......
    public void RequestGetFeedsByUserId(int userId, int pageNum, int pageSize, string callingFrom, bool _callFromFindFriendWithName = false)
    {
        StartCoroutine(IERequestGetFeedsByUserId(userId, pageNum, pageSize, callingFrom, _callFromFindFriendWithName));
    }
    public IEnumerator IERequestGetFeedsByUserId(int userId, int pageNum, int pageSize, string callingFrom, bool _callFromFindFriendWithName = false)
    {
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
                feedUIController.ShowLoader(false);

                switch (callingFrom)
                {
                    case "OtherPlayerFeed":
                        if (otherPlayerProfileData != null && pageNum == 1)
                        {
                            otherPlayerProfileData.RemoveAndCheckBackKey();
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                //// Print the elapsed time
                string data = www.downloadHandler.text;
                Debug.Log("IERequestGetFeedsByUserId success data" + data);
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                AllTextPostByUserIdRoot jsonData = JsonConvert.DeserializeObject<AllTextPostByUserIdRoot>(data, settings);
                if (callingFrom == "MyProfile")
                {
                    myProfileDataManager.totalPostText.text = CheckforEmptyTextPosts(jsonData).ToString();
                    allTextPostWithUserIdRoot.data.rows.Clear();
                }
                else
                {
                    otherPlayerProfileData.textPlayerTottlePost.text = CheckforEmptyTextPosts(jsonData).ToString();
                }
                if (allTextPostWithUserIdRoot.data.rows.Count >= jsonData.data.rows.Count)
                {
                    //below line of clearing was commented earlier by riken but uncommented now after start of profile 2.0 as it is working fine for me ----- UMER
                    allTextPostWithUserIdRoot.data.rows.Clear();

                    for (int i = 0; i < jsonData.data.rows.Count; i++)
                    {
                       

                        if (!allTextPostWithUserIdRoot.data.rows.Any(x => x.id == jsonData.data.rows[i].id))
                        {
                            allTextPostWithUserIdRoot.data.rows.Add(jsonData.data.rows[i]);
                        }
                       
                    }
                }
                else
                {
                    if (allTextPostWithUserIdRoot.data.rows.Count > 0)
                    {
                        allTextPostWithUserIdRoot.data.rows.Clear();
                    }

                    allTextPostWithUserIdRoot = jsonData;
                }
                if (callingFrom == "OtherPlayerFeed")
                {
                    allTextPostWithUserIdRoot = jsonData;
                }
                switch (callingFrom)
                {
                    case "OtherPlayerFeed":
                        otherPlayerProfileData.AllFeedWithUserId(pageNum, _callFromFindFriendWithName);
                        break;
                    case "MyProfile":
                        myProfileDataManager.AllFeedWithUserId(pageNum);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public int CheckforEmptyTextPosts(AllTextPostByUserIdRoot _userTextPostData)
    {
        int emptyPostCount = 0;

        // Use foreach loop for better readability and performance
        foreach (var row in _userTextPostData.data.rows)
        {
            if (row.text_post == "null" || string.IsNullOrEmpty(row.text_post))
            {
                emptyPostCount++;
            }
        }
        int nullPostCount = _userTextPostData.data.Count - emptyPostCount;
        return nullPostCount;
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

            www.SendWebRequest();
            while(!www.isDone)
            {
                yield return null;
            }
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                allFollowingRoot = JsonConvert.DeserializeObject<AllFollowingRoot>(data);
            }
        }
    }

    /// <summary>
    /// For Add Friend Following
    /// </summary>
    public void SetAdFrndFollowing()
    {
        if (feedUIController != null)
        {
            feedUIController.ShowLoader(true);
        }
        StartCoroutine(IEAdFrndAllFollowing(1, 100));
    }
   
    public IEnumerator IEAdFrndAllFollowing(int pageNum, int pageSize)
    {
        string uri = ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetAllFollowing + userId + "/" + pageNum + "/" + pageSize;
        print("uri" + uri);
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            print("Authorization" + userAuthorizeToken);
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            www.SendWebRequest();
            while(!www.isDone)
            {
                yield return null;
            }

            if (www.result==UnityWebRequest.Result.ConnectionError || www.result==UnityWebRequest.Result.ProtocolError)
            {
                if (feedUIController != null)
                {
                    feedUIController.ShowLoader(false);
                }
                Debug.Log(www.error);
            }
            else
            {
                if (feedUIController != null)
                {
                    feedUIController.ShowLoader(false);
                }
                //Debug.Log("Form upload complete!");
                string data = www.downloadHandler.text;
                //adFrndFollowing = JsonConvert.DeserializeObject<AllFollowingRoot>(data);
                adFrndFollowing = JsonUtility.FromJson<AllFollowingRoot>(data);
                apiController.SpwanAdFrndFollowing();
                GetBestFriend();
            }
        }
    }

    //this api is used to follow user.......
    public void RequestFollowAUser(string user_Id, string callingFrom)
    {
        if(!isCoroutineRunning)
            StartCoroutine(IERequestFollowAUser(user_Id, callingFrom));
    }
    public IEnumerator IERequestFollowAUser(string user_Id, string callingFrom)
    {
        isCoroutineRunning = true;
        WWWForm form = new WWWForm();
        form.AddField("userId", user_Id);

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_FollowAUser), form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            www.SendWebRequest();

            while(!www.isDone)
            {
                yield return null;
            }

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                if (feedUIController != null)
                {
                    feedUIController.ShowLoader(false);
                }
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                switch (callingFrom)
                {
                    case "OtherUserProfile":
                        //feedUIController.ShowLoader(false);
                        otherPlayerProfileData.OnFollowerIncreaseOrDecrease(true);//Inscrease follower count.......
                        feedUIController.FollowingAddAndRemoveUnFollowedUser(int.Parse(user_Id), false);
                        //ProfileUIHandler.instance.followProfileBtn.GetComponent<Button>().interactable = true;
                        break;
                    case "Feed":
                        if (feedUIController != null)
                        {
                            StartCoroutine(WaitToFalseLoader());
                        }

                        apiController.currentFeedRawItemController.OnFollowUserSuccessful();
                        apiController.currentFeedRawItemController.isFollow = true;
                        apiController.currentFeedRawItemController = null;
                        break;
                    default:
                        break;
                }
            }
            isCoroutineRunning = false;
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

            www.SendWebRequest();
            while(!www.isDone)
            {
                yield return null;
            }

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                if (feedUIController != null)
                {
                    feedUIController.ShowLoader(false);
                }
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                if (SNS_APIManager.Instance.BFCount > 0)
                    SNS_APIManager.Instance.BFCount -= 1;
                Debug.Log("Un Follow a user data:" + data + "  :user id:" + user_Id + "   :CallingFrom:" + callingFrom);
                switch (callingFrom)
                {
                    case "OtherUserProfile":
                        //feedUIController.ShowLoader(false);
                        otherPlayerProfileData.OnFollowerIncreaseOrDecrease(false);//Descrease follower count.......
                        feedUIController.FollowingAddAndRemoveUnFollowedUser(int.Parse(user_Id), true);
                        //ProfileUIHandler.instance.followProfileBtn.GetComponent<Button>().interactable = true;
                        if (feedUIController.ConfirmUnfollowPanel.activeInHierarchy)
                            feedUIController.ConfirmUnfollowPanel.SetActive(false);
                        break;
                    case "Feed":
                        if (feedUIController != null)
                        {
                            StartCoroutine(WaitToFalseLoader());
                        }

                        apiController.currentFeedRawItemController.OnFollowUserSuccessful();
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
        feedUIController.ShowLoader(false);
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

            www.SendWebRequest();
            while(!www.isDone)
            {
                yield return null;
            }

            //feedUIController.ShowLoader(false);

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                profileAllFollowerRoot = JsonUtility.FromJson<AllFollowersRoot>(data);

                feedUIController.ProfileGetAllFollower(pageNum);
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

            www.SendWebRequest();
            while(!www.isDone) 
            { 
                yield return null;
            }

            //feedUIController.ShowLoader(false);

            if (www.result==UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                profileAllFollowingRoot = JsonUtility.FromJson<AllFollowingRoot>(data);
                apiController.SpwanProfileFollowing();
            }
        }
    }
   
    #endregion
   

    //this api is used to get search user list......
    public void RequestGetSearchUser(string name)
    {
        StartCoroutine(IERequestGetSearchUser(name));
    }
    public IEnumerator IERequestGetSearchUser(string name)
    {
        WWWForm form = new WWWForm();
        //if (!name.All(char.IsDigit)) // is seraching with name
        {
            form.AddField("name", name);
            form.AddField("userId", 0);  //disables search with userid because user can't check their id.
        }
        //else // is searching with number
        //{
        //    form.AddField("name", "");
        //    form.AddField("userId", name);
        //}


        string uri = ConstantsGod.API_BASEURL + ConstantsGod.r_url_SearchUser + "1/50";
        using (UnityWebRequest www = UnityWebRequest.Post(uri, form))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            www.SendWebRequest();
            while(!www.isDone)
            {
                yield return null;
            }

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                searchUserRoot = JsonUtility.FromJson<SearchUserRoot>(data);
                apiController.FeedGetAllSearchUser();
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
    public void GetHomeFriendProfileData<T>(int _userid, T obj) where T : class
    {
        StartCoroutine(IERequestFeedUserProfileData(_userid, result =>
        {
            CheckOnlineFriend checkOnlineFriend = obj as CheckOnlineFriend;
            checkOnlineFriend.SetupHomeFriendProfile(result);
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

            www.SendWebRequest();
            while(!www.isDone)
            {
                yield return null;
            }

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                string data = www.downloadHandler.text;
                //Debug.Log("Feed user profile data:" + data);
                searchUserRoot = JsonUtility.FromJson<SearchUserRoot>(data);
                if (searchUserRoot.msg.Contains("yourself"))
                {
                    if (feedUIController)
                    {
                        feedUIController.bottomTabManager.OnClickProfileButton();
                    }
                }
            }
            else
            {
                string data = www.downloadHandler.text;
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
                     else if (obj is CheckOnlineFriend)
                     {
                         CheckOnlineFriend _OnlineObj = obj as CheckOnlineFriend;
                         _OnlineObj.DressUpUserAvatar();
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

            www.SendWebRequest();

            while(!www.isDone)
            {
                yield return null;
            }

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.Log("Search user name data:" + data);
                searchUserRoot = JsonUtility.FromJson<SearchUserRoot>(data);
                apiController.FeedGetAllSearchUserForProfile();
            }
        }
    }
    

    #region Friends

    public void SetHotFriend()
    {
        if (feedUIController != null)
        {
            feedUIController.ShowLoader(true);
        }
        StartCoroutine(IERequestHotFirends());
    }

    IEnumerator IERequestHotFirends()
    {
        string uri = ConstantsGod.API_BASEURL + ConstantsGod.r_url_HotUsers + "1/100";
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);
            www.SendWebRequest();
            while(!www.isDone)
            {
                yield return null;
            }
            if (www.result==UnityWebRequest.Result.ConnectionError || www.result==UnityWebRequest.Result.ProtocolError)
            {
                if (feedUIController != null)
                {
                    feedUIController.ShowLoader(false);
                }
                Debug.Log(www.error);
            }
            else
            {
                if (feedUIController != null)
                {
                    feedUIController.ShowLoader(false);
                }
                string data = www.downloadHandler.text;
                hotUsersRoot = JsonUtility.FromJson<HotUsersRoot>(data);
                apiController.ShowHotFirend(hotUsersRoot);
            }
        }
    }


    public void SetRecommendedFriend()
    {
        if (feedUIController != null)
        {
            feedUIController.ShowLoader(true);
        }
        StartCoroutine(IERequestRecommendedFirends());
    }

    IEnumerator IERequestRecommendedFirends()
    {
        string uri = ConstantsGod.API_BASEURL + ConstantsGod.r_url_RecommendedUser + "1/50";
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);
            www.SendWebRequest();
            while(!www.isDone)
            {
                yield return null;
            }
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ConnectionError)
            {
                if (feedUIController != null)
                {
                    feedUIController.ShowLoader(false);
                }
                Debug.Log(www.error);
            }
            else
            {
                if (feedUIController != null)
                {
                    feedUIController.ShowLoader(false);
                }
                string data = www.downloadHandler.text;
                searchUserRoot = JsonUtility.FromJson<SearchUserRoot>(data);
                apiController.ShowRecommendedFriends(searchUserRoot);
            }
        }
    }


    public void SetMutalFrndList()
    {
        if (feedUIController != null)
        {
            feedUIController.ShowLoader(true);
        }
        StartCoroutine(IERequestSetMutalFrndList());
    }

    IEnumerator IERequestSetMutalFrndList()
    {
        string uri = ConstantsGod.API_BASEURL + ConstantsGod.r_url_MutalFrnd + SNS_APIManager.Instance.userId + "/1/100";
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);
            www.SendWebRequest();
            while(!www.isDone)
            {
                yield return null;
            }
            if (www.result==UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                if (feedUIController != null)
                {
                    feedUIController.ShowLoader(false);
                }
                Debug.Log(www.error);
            }
            else
            {
                if (feedUIController != null)
                {
                    feedUIController.ShowLoader(false);
                }
                string data = www.downloadHandler.text;
                SearchUserRoot mutalFrnd = JsonUtility.FromJson<SearchUserRoot>(data);
                feedUIController.AddFrndNoMutalFrnd.SetActive(false);
                if (mutalFrnd.data.count > 0)
                {
                    apiController.ShowMutalFrnds(mutalFrnd);
                }
                else
                { // to Show no mutal Frnd
                    feedUIController.AddFrndNoMutalFrnd.SetActive(true);
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
        string uri = ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetBestFrnd + SNS_APIManager.Instance.userId;
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);
            www.SendWebRequest();
            while(!www.isDone)
            {
                yield return null;
            }
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                CloseFrndRoot CloseFrnds = JsonUtility.FromJson<CloseFrndRoot>(data);
                BFCount = CloseFrnds.data.count;
            }
        }
    }

    public void AddBestFriend(int userId, GameObject FrndBtn)
    {
        var footerCanvasGroup = gameManager.UiManager._footerCan.GetComponent<CanvasGroup>();

        if (BFCount < maxBfCount)
        {
            StartCoroutine(IEAddBestFriend(userId, FrndBtn));
        }
        else
        {
            footerCanvasGroup.alpha = 0;
            footerCanvasGroup.interactable = false;
            footerCanvasGroup.blocksRaycasts = false;
            feedUIController.BestFriendFull.SetActive(true);
        }
    }
    IEnumerator IEAddBestFriend(int userId, GameObject FrndBtn)
    {
        feedUIController?.ShowFriendLoader(true);
        string uri = ConstantsGod.API_BASEURL + ConstantsGod.r_url_AdBestFrnd + userId.ToString();
        using (UnityWebRequest www = UnityWebRequest.Post(uri, "POST"))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);
            www.SendWebRequest();
            while(!www.isDone) 
            {
               yield return null;
            }

           

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result==UnityWebRequest.Result.ProtocolError    )
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                AdCloseFrndRoot AdCloseFrnds = JsonUtility.FromJson<AdCloseFrndRoot>(data);
                if (AdCloseFrnds.success)
                {
                    GetBestFriend();
                    var followingController = FrndBtn.GetComponent<FollowingItemController>();
                    var findFriendController = FrndBtn.GetComponent<FindFriendWithNameItem>();

                    if (followingController != null)
                    {
                        followingController.UpdateBfBtn(true);
                    }
                    else if (findFriendController != null)
                    {
                        findFriendController.UpdateBfBtn(true);
                    }
                }
            }
             feedUIController?.ShowFriendLoader(false);
        }
    }

    public void RemoveBestFriend(int userId, GameObject FrndBtn)
    {
       
        StartCoroutine(IERemoveBestFriend(userId, FrndBtn));
    }
    IEnumerator IERemoveBestFriend(int userId, GameObject FrndBtn)
    {
        feedUIController?.ShowFriendLoader(true);
        string uri = ConstantsGod.API_BASEURL + ConstantsGod.r_url_RemoveBestFrnd + userId.ToString();
        using (UnityWebRequest www = UnityWebRequest.Delete(uri))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);
            www.SendWebRequest();
            while(!www.isDone)
            {
                yield return null;
            }

            feedUIController?.ShowLoader(false);

            if (www.result==UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                GetBestFriend();
                var followingController = FrndBtn.GetComponent<FollowingItemController>();
                var findFriendController = FrndBtn.GetComponent<FindFriendWithNameItem>();

                if (followingController != null)
                {
                    followingController.UpdateBfBtn(false);
                }
                else if (findFriendController != null)
                {
                    findFriendController.UpdateBfBtn(false);
                }
            }
            feedUIController?.ShowFriendLoader(false);
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

            www.SendWebRequest();
            while (!www.isDone)
            {
                yield return null;
            }
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log("Form upload complete!");
                string data = www.downloadHandler.text;
                
                var jo = Newtonsoft.Json.Linq.JObject.Parse(data);
                var msg = jo["msg"].ToString();
                if (msg == "This name is already taken by other user.")
                {
                    //Debug.Log("Username already exists");
                    myProfileDataManager.ShowEditProfileNameErrorMessage("Username already exists");
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
            if (www.result==UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("IERequestGetUserDetails error:" + www.error);
                if (feedUIController != null)
                {
                    feedUIController.ShowLoader(false);
                    switch (callingFrom)
                    {
                        case "EditProfileAvatar":
                            myProfileDataManager.EditProfileDoneButtonSetUp(true);//setup edit profile done button.......
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
                //Debug.Log("callingFrom" + callingFrom);
                myProfileDataRoot = JsonUtility.FromJson<GetUserDetailRoot>(data);
                switch (callingFrom)
                {
                    case "myProfile":
                        myProfileDataManager.SetupData(myProfileDataRoot.data, callingFrom);//setup and load my profile data.......                        
                        break;
                    case "EditProfileAvatar":
                        myProfileDataManager.SetupData(myProfileDataRoot.data, callingFrom);//setup and load my profile data.......
                        break;
                    //case "messageScreen":
                    //    SNS_MessageController.Instance.GetSuccessUserDetails(myProfileDataRoot.data);
                    //    break;
                    case "MyAccount":
                        myProfileDataManager.myProfileData = myProfileDataRoot.data;
                        feedUIController.SNSSettingController.SetUpPersonalInformationScreen();
                        break;
                    default:
                        break;
                }

                PlayerPrefs.SetString("PlayerName", myProfileDataRoot.data.name);
                PlayerPrefs.SetString("UserName", myProfileDataRoot.data.name);

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

            www.SendWebRequest();
            while (!www.isDone)
            {
                yield return null;
            }

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
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
                // root = JsonUtility.FromJson<UpdateUserAvatarRoot>(data);
                switch (callingFrom)
                {
                    case "EditProfileAvatar":
                        RequestGetUserDetails(callingFrom);
                        myProfileDataManager.AfterUpdateAvatarSetTempSprite();
                        break;
                    default:
                        break;
                }
            }
        }
    }

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

        using (UnityWebRequest www = new UnityWebRequest(apiUrl, "POST"))
        {
            www.SetRequestHeader("Authorization", userAuthorizeToken);

            byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            www.SendWebRequest();
            while(!www.isDone)
            {
                yield return null;
            }
            UniqueUserNameError test = JsonConvert.DeserializeObject<UniqueUserNameError>(www.downloadHandler.text);
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) //(www.result.isNetworkError || www.isHttpError)
            {
                Debug.Log("<color=red> ------Edit API Error " + www.error + www.downloadHandler.text + "</color>");
                if (test.msg.Contains("Username"))
                {
                    myProfileDataManager.isEditProfileNameAlreadyExists = true;
                    myProfileDataManager.ShowEditProfileUniqueNameErrorMessage("The username must include letters");
                }
            }
            else
            {
                if (!test.success)
                {
                    if (test.msg.Contains("Username"))
                    {
                        myProfileDataManager.isEditProfileNameAlreadyExists = true;
                        myProfileDataManager.ShowEditProfileUniqueNameErrorMessage("Username already taken");
                    }
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
        }
        unloadUnusedFileCount += 1;
    }
    #endregion

    #region Clear FeedDataAfterLogout.......
    
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
   
    [Header("Single User All Feed Data")]
    public AllTextPostByUserIdRoot allTextPostWithUserIdRoot = new AllTextPostByUserIdRoot();

    public SearchUserRoot searchUserRoot = new SearchUserRoot();
    public HotUsersRoot hotUsersRoot = new HotUsersRoot();
    public AllFollowingRoot allFollowingRoot = new AllFollowingRoot();

    [Space]
    [Header("Profile Follower Following")]
    public AllFollowersRoot profileAllFollowerRoot = new AllFollowersRoot();
    public AllFollowingRoot profileAllFollowingRoot = new AllFollowingRoot();
    public AllFollowingRoot AdFrndFollowingRoot = new AllFollowingRoot();
    public UserLatestAvatarData VisitedUserAvatarData = new UserLatestAvatarData();
}

public enum ExtentionType { Image, Video, Audio };


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
    public string username;
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

[System.Serializable]
public class AdCloseFrndData
{
    public int count;
    public List<AdCloseFrndRow> rows;
}


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