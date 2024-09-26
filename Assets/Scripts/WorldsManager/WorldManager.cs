using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using UnityEditor;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Photon.Pun;


public class WorldManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI worldFoundText;
    [Header("World View prefab")]
    public GameObject EventPrefabLobby;
    [HideInInspector]
    public bool orientationchanged = false;
    [Header("Api Parameter's")]
    private string finalAPIURL;
    private string status = "Publish";
    [HideInInspector]
    public int hotFeatSpacePN = 1, hotSpacePN = 1, hotGamesPN = 1, followingPN = 1, mySpacesPN = 1, seeAllPN = 1;
    /*private int pageNumberHot = 1;
    private int pageNumberAllWorld = 1;
    private int pageNumberMyWorld = 1;
    private int pageNumberGameWorld = 1;
    private int pageNumberEventWorld = 1;
    private int pageNumberTestWorld = 1;*/
    private int pageNumberSearchWorld = 1;
    private int recordPerPage = 30;
    private bool loadOnce = true;
    public bool dataIsFatched = false;
    public APIURL aPIURLGlobal;
    [SerializeField]
    [NonReorderable]
    List<AutoSwtichEnv> AutoSwtichWorldList;

    [Header("Fighting Module PopUp")]
    public GameObject fightingModulePopUp;
    public bool isCheckFightingModulePopUp;
    public bool HaveFighterNFT;
    public static Action LoadHomeScreenWorlds;
    public static Action ReloadFollowingSpace;
    static int AutoSwtichIndex = 0;

    public int SearchPageNumb = 1;
    public int SearchPageSize = 15;
    public int SearchTagPageNumb = 1;
    public int SearchTagPageSize = 15;
    public string SearchKey = default;
    public string previousSearchKey;
    public string searchResponse;

    public string worldstr;

    public List<WorldItemDetail> resultWorldList = new List<WorldItemDetail>();

    public WorldItemManager WorldItemManager;
    public WorldsInfo _WorldInfo;
    public SingleWorldInfo singleWorldInfo;
    public WorldSpacesHomeScreen worldSpaceHomeScreenRef;
    public WorldDescriptionPopupPreview worldItemPreviewTabRef;
    public SearchWorldUIController worldSearchManager;
    public SearchWorldHandler searchWorldControllerRef;
    public UIHandler uiHandlerRef;
    public static WorldManager instance;
    //[HideInInspector]
    public bool changeFollowState = false;
    public static event Action<BackButtonHandler.screenTabs> OnScreenTabStateChange;
    public APIURL GetCurrentTabSelected()
    {
        return aPIURLGlobal;
    }
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);


        if (!System.IO.Directory.Exists(Application.persistentDataPath + "/MainMenuData"))
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/MainMenuData");
            if (!System.IO.Directory.Exists(Application.persistentDataPath + "/MainMenuData/MuseumData"))
            {
                System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/MainMenuData/" + "MuseumData");
            }
            if (!System.IO.Directory.Exists(Application.persistentDataPath + "/MainMenuData/EnvData"))
            {
                System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/MainMenuData/" + "EnvData");
            }
            if (!System.IO.Directory.Exists(Application.persistentDataPath + "/MainMenuData/BuilderData"))
            {
                System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/MainMenuData/" + "BuilderData");
            }
        }

    }

    private void OnEnable()
    {
        Invoke(nameof(LoadJjworld), 0);
        MainSceneEventHandler.OpenLandingScene += OpenLandingScene;
    }


    public IEnumerator xanaParty()
    {
        if (ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
            yield return null;
        if (!XANAPartyManager.Instance.EnableXANAPartyGuest)
        {
            LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.PenpenzLoading(FadeAction.In));
            while (PlayerPrefs.GetInt("WalletLogin") == 1 && ConstantsHolder.userId.IsNullOrEmpty() && ConstantsHolder.userName.IsNullOrEmpty())
            {
                yield return new WaitForSeconds(0.5f);
            }

            if (PlayerPrefs.GetInt("IsProcessComplete") == 1 && PlayerPrefs.GetString("DownloadPermission", "false") == "false")
            {
                UserLoginSignupManager.instance.DownloadPermissionPopup.SetActive(true);
            }
            else if (PlayerPrefs.GetInt("IsProcessComplete") == 1 && PlayerPrefs.GetString("DownloadPermission", "false") == "true")
            {
                XANAPartyManager.Instance.GetComponent<XANAPartyManager>().EnablingXANAParty();
                yield return null;
            }
        }
        else if (XANAPartyManager.Instance.EnableXANAPartyGuest)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;

            if (PlayerPrefs.GetInt("IsProcessComplete") == 1 && PlayerPrefs.GetString("DownloadPermission") == "false")
            {
                UserLoginSignupManager.instance.DownloadPermissionPopup.SetActive(true);
            }
            else if (PlayerPrefs.GetInt("IsProcessComplete") == 1 && PlayerPrefs.GetString("DownloadPermission") == "true")
            {
                XANAPartyManager.Instance.GetComponent<XANAPartyManager>().EnablingXANAParty();
                yield return null;
            }
        }

    }

    private void OnDisable()
    {
        MainSceneEventHandler.OpenLandingScene -= OpenLandingScene;
    }

    void OpenLandingScene()
    {
        int WorldId;
        try
        {
            if (APIBasepointManager.instance.IsXanaLive)
                WorldId = 6239;    //Mainnet
            else
                WorldId = 2455;
        }
        catch(Exception e)
        {
            WorldId = 6239;
        }
        

        string url = ConstantsGod.API_BASEURL + ConstantsGod.SINGLEWORLDINFO + WorldId;
        StartCoroutine(GetSingleWorldData(url, (check) =>
        {
            ConstantsHolder.userLimit = int.Parse(singleWorldInfo.data.user_limit);
            ConstantsHolder.isPenguin = false;
            ConstantsHolder.xanaConstants.openLandingSceneDirectly = false;
            ConstantsHolder.IsXSummitApp = true;
            ConstantsHolder.xanaConstants.isBuilderScene = false;
            ConstantsHolder.xanaConstants.isFromHomeTab = true;
            ConstantsHolder.xanaConstants.MuseumID = singleWorldInfo.data.id;
            WorldItemView.m_EnvName = singleWorldInfo.data.name;
            ConstantsHolder.Thumbnail = singleWorldInfo.data.thumbnail;
            ConstantsHolder.description = singleWorldInfo.data.description;
            ConstantsHolder.xanaConstants.EnviornmentName = WorldItemView.m_EnvName;
            ConstantsHolder.xanaConstants.UserMicEnable = singleWorldInfo.data.userMicEnable;
            LoadingHandler.Instance.GetComponent<CanvasGroup>().alpha = 1;
            LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
            LoadingHandler.Instance.LoadingScreenSummit.SetActive(false);
            LoadingHandler.Instance.ShowLoading();
            LoadingHandler.StopLoader = true;
            //LoadingHandler.Instance.UpdateLoadingSlider(0);
            LoadingHandler.Instance.UpdateLoadingStatusText("Loading World");
            //this is added to fix 20% loading stuck issue internally photon reload scenes to sync 
            //Photon.Pun.PhotonHandler.levelName = "GamePlayScene";

            Invoke(nameof(AddingDeleyToLoadScene), .5f);
        }));

    }

    async void AddingDeleyToLoadScene()
    {
        while (!ConstantsHolder.isAddressableCatalogDownload)
            await Task.Delay(500);
        LoadingHandler.Instance.LoadSceneByIndex("GamePlayScene");
        XANAPartyManager xANAPartyManager = XANAPartyManager.Instance;
        xANAPartyManager.StartCoroutine(xANAPartyManager.FetchXanaPartyGames());
    }

    IEnumerator GetSingleWorldData(string apiURL, Action<bool> callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(apiURL))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            www.SendWebRequest();
            while (!www.isDone)
                yield return null;
            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                callback(false);
            }
            else
            {
                //Debug.LogError(www.downloadHandler.text);
                singleWorldInfo = JsonUtility.FromJson<SingleWorldInfo>(www.downloadHandler.text);
                worldstr = www.downloadHandler.text;
                callback(true);
            }
            www.Dispose();
        }
    }

    public void ChangeWorldTab(APIURL tab, string _searchKey)
    {
        aPIURLGlobal = tab;
        SearchKey = _searchKey;
        GetBuilderWorlds(tab, (a) => { });
    }
    public void SetaPIURLGlobal(APIURL chnager)
    {
        aPIURLGlobal = chnager;
    }

    public void SearchWorldCall(string searchKey, bool isFromTag = false)
    {
        WorldLoadingText(APIURL.Temp);
        if (searchKey != previousSearchKey && !string.IsNullOrEmpty(searchKey))
        {
            if (isFromTag)
                aPIURLGlobal = APIURL.SearchWorldByTag;
            else
                aPIURLGlobal = APIURL.SearchWorld;
            /*this.WorldItemManager.ClearListInDictionary(aPIURLGlobal.ToString());*/
            WorldScrollReset();
            seeAllPN = 1;
            SearchPageNumb = 1;
            SearchTagPageNumb = 1;
            SearchPageSize = 40;
            SearchTagPageSize = 40;
            SearchKey = searchKey;
            LoadingHandler.Instance.SearchLoadingCanvas.SetActive(true);
            GetBuilderWorlds(aPIURLGlobal, (a) => { });

            searchWorldControllerRef.scroller.ScrollPosition = 0f;    // my changes
        }
        else
        {
            /* this.WorldItemManager.ClearListInDictionary(aPIURLGlobal.ToString());*/
            WorldScrollReset();
            previousSearchKey = SearchKey = searchKey;
            LoadingHandler.Instance.SearchLoadingCanvas.SetActive(false);
        }
    }
    void SetAutoSwtichStreaming()
    {
        if (ConstantsHolder.xanaConstants.isCameraMan)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;

            ConstantsHolder.xanaConstants.JjWorldSceneChange = true;
            ConstantsHolder.xanaConstants.JjWorldTeleportSceneName = AutoSwtichWorldList[AutoSwtichIndex].name;
            ConstantsHolder.xanaConstants.IsMuseum = AutoSwtichWorldList[AutoSwtichIndex].isMussuem;
            if (APIBasepointManager.instance.IsXanaLive)
            {
                ConstantsHolder.xanaConstants.MuseumID = AutoSwtichWorldList[AutoSwtichIndex].mainnetId.ToString();
            }
            else
            {
                ConstantsHolder.xanaConstants.MuseumID = AutoSwtichWorldList[AutoSwtichIndex].testnetId.ToString();
            }
            if (AutoSwtichIndex < AutoSwtichWorldList.Count - 1)
            {
                AutoSwtichIndex++;
            }
            else
            {
                AutoSwtichIndex = 0;
            }
            LoadingHandler.Instance.streamingLoading.UpdateLoadingText(true);
        }
    }

    public void WorldPageLoading()
    {
        if (dataIsFatched)
        {
            loadOnce = true;
            dataIsFatched = false;
            LoadingHandler.Instance.worldLoadingScreen.SetActive(true);
            GetBuilderWorlds(aPIURLGlobal, (a) => { });
        }
    }


    public string PrepareApiURL(APIURL aPIURL, int recordPerPage = 30)
    {
        switch (aPIURL)
        {
            case APIURL.FeaturedSpaces:
                //return ConstantsGod.API_BASEURL + ConstantsGod.FEATUREDSPACES + hotFeatSpacePN + "/" + recordPerPage;
                return ConstantsGod.API_BASEURL + ConstantsGod.FEATUREDSPACES + seeAllPN + "/" + recordPerPage;
            case APIURL.HotSpaces:
                //return ConstantsGod.API_BASEURL + ConstantsGod.HOTSPACES + hotSpacePN + "/" + recordPerPage;
                return ConstantsGod.API_BASEURL + ConstantsGod.HOTSPACES + seeAllPN + "/" + recordPerPage;

            case APIURL.HotGames:
                //return ConstantsGod.API_BASEURL + ConstantsGod.HOTGAMES /*+ status + "/" */+ hotGamesPN + "/" + recordPerPage;
                return ConstantsGod.API_BASEURL + ConstantsGod.HOTGAMES /*+ status + "/" */+ seeAllPN + "/" + recordPerPage;

            case APIURL.MySpace:
                //return ConstantsGod.API_BASEURL + ConstantsGod.MYBUILDERWORLDS + status + "/" + mySpacesPN + "/" + recordPerPage;
                return ConstantsGod.API_BASEURL + ConstantsGod.MYBUILDERWORLDS + status + "/" + seeAllPN + "/" + recordPerPage;

            case APIURL.FolloingSpace:
                //return ConstantsGod.API_BASEURL + ConstantsGod.FOLLOWINGSPACES + followingPN + "/" + recordPerPage; //+ "/" + status + "/GAME";
                return ConstantsGod.API_BASEURL + ConstantsGod.FOLLOWINGSPACES + seeAllPN + "/" + recordPerPage; //+ "/" + status + "/GAME";

            //case APIURL.EventWorld:
            //    return ConstantsGod.API_BASEURL + ConstantsGod.WORLDSBYCATEGORY + pageNumberEventWorld + "/" + pageCount + "/" + status + "/EVENT";
            //case APIURL.TestWorld:
            //    return ConstantsGod.API_BASEURL + ConstantsGod.WORLDSBYCATEGORY + pageNumberTestWorld + "/" + pageCount + "/" + status + "/TEST";
            case APIURL.SearchWorld:
                //return ConstantsGod.API_BASEURL + ConstantsGod.SearchWorldAPI + SearchKey + "/" + SearchPageNumb + "/" + SearchPageSize;
                return ConstantsGod.API_BASEURL + ConstantsGod.SearchWorldAPI + SearchKey + "/" + seeAllPN + "/" + SearchPageSize;

            case APIURL.SearchWorldByTag:
                //return ConstantsGod.API_BASEURL + ConstantsGod.SEARCHWORLDBYTAG + SearchKey + "/" + SearchTagPageNumb + "/" + SearchTagPageSize;
                return ConstantsGod.API_BASEURL + ConstantsGod.SEARCHWORLDBYTAG + SearchKey + "/" + seeAllPN + "/" + SearchTagPageSize;

            default:
                //return ConstantsGod.API_BASEURL + ConstantsGod.HOTSPACES + hotSpacePN + "/" + recordPerPage;
                return ConstantsGod.API_BASEURL + ConstantsGod.HOTSPACES + seeAllPN + "/" + recordPerPage;

        }
    }
    void UpdatePageNumber(APIURL aPIURL)
    {
        seeAllPN += 1;
        //switch (aPIURL)
        //{
        //    case APIURL.FeaturedSpaces:
        //        //hotFeatSpacePN += 1;
        //        seeAllPN += 1;
        //        return;
        //    case APIURL.HotSpaces:
        //        //hotSpacePN += 1;
        //        seeAllPN += 1;
        //        return;
        //    case APIURL.HotGames:
        //        //hotGamesPN += 1;
        //        seeAllPN += 1;
        //        return;
        //    case APIURL.MySpace:
        //        //mySpacesPN += 1;
        //        seeAllPN += 1;
        //        return;
        //    case APIURL.FolloingSpace:
        //        //followingPN += 1;
        //        seeAllPN += 1;
        //        return;
        //    /*case APIURL.EventWorld:
        //        pageNumberEventWorld += 1;
        //        return;*/
        //    case APIURL.SearchWorld:
        //        //SearchPageNumb += 1;
        //        seeAllPN += 1;
        //        return;
        //    case APIURL.SearchWorldByTag:
        //        //SearchTagPageNumb += 1;
        //        seeAllPN += 1;
        //        return;
        //    /*case APIURL.TestWorld:
        //        pageNumberTestWorld += 1;
        //        return;*/
        //    default:
        //        //hotSpacePN += 1;
        //        seeAllPN += 1;
        //        return;
        //}
    }
    bool NotProcessRequest = false;
    int CallBackCheck = 0;
    Coroutine FetchUserMapFromServerCO;
    public void GetBuilderWorlds(APIURL aPIURL, Action<bool> CallBack)
    {
        //Debug.Log("Current Data in Input field: " + worldSearchManager.searchWorldInput.Text);
        finalAPIURL = PrepareApiURL(aPIURL);
        loadOnce = false;
        //Debug.LogError(finalAPIURL);
        if (FetchUserMapFromServerCO != null)
            StopCoroutine(FetchUserMapFromServerCO);
        FetchUserMapFromServerCO = StartCoroutine(FetchUserMapFromServer(finalAPIURL, (isSucess) =>
        {
            if (isSucess)
            {
                CallBackCheck = 0;
                dataIsFatched = true;

                if (_WorldInfo.data.rows.Count > 0)
                {
                    InstantiateWorlds(aPIURL, isSucess);
                    UpdatePageNumber(aPIURL);
                    CallBack(true);
                }
                else
                {
                    if (_WorldInfo.data.rows.Count > 0)
                    {
                        WorldLoadingText(APIURL.Temp);  //remove loading text from search screen
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(worldSearchManager.searchWorldInput.Text) && _WorldInfo.data.count <= 0)
                        {
                            WorldLoadingText(APIURL.SearchWorld);
                        }
                        else
                        {
                            if (_WorldInfo.data.count <= 0)
                            {
                                WorldLoadingText(aPIURL);
                            }
                        }
                    }
                    CallBack(false);
                }
            }
            else
            {
                loadOnce = true;
                if (++CallBackCheck > 17)
                {
                    LoadingHandler.Instance.worldLoadingScreen.SetActive(false);
                    CallBackCheck = 0;
                    return;
                }
                WorldLoadingText(APIURL.SearchWorld);
                LoadingHandler.Instance.SearchLoadingCanvas.SetActive(false);

                GetBuilderWorlds(aPIURLGlobal, (a) => { });
                CallBack(false);
            }
            previousSearchKey = SearchKey;
        }));
    }

    IEnumerator FetchUserMapFromServer(string apiURL, Action<bool> callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(apiURL))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            www.SendWebRequest();
            while (!www.isDone)
                yield return null;
            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                callback(false);
            }
            else
            {
                searchResponse = www.downloadHandler.text;
                //Debug.LogError(apiURL+"-------"+www.downloadHandler.text);

                if (SearchKey == "" && (aPIURLGlobal == APIURL.SearchWorld || aPIURLGlobal == APIURL.SearchWorldByTag))
                    callback(false);
                else
                {
                    _WorldInfo = JsonUtility.FromJson<WorldsInfo>(www.downloadHandler.text);
                    worldstr = www.downloadHandler.text;
                    callback(true);
                }
            }
            www.Dispose();
        }
    }

    void InstantiateWorlds(APIURL _apiURL, bool APIResponse)
    {
        //searchWorldControllerRef.scroller.ScrollPosition = 0f;    // my changes
        Debug.Log("Category worlds list count: " + _WorldInfo.data.rows.Count);
        resultWorldList.Clear();
        for (int i = 0; i < _WorldInfo.data.rows.Count; i++)
        {
            WorldItemDetail _event;
            //if (_WorldInfo.data.rows[i].name.Contains("XANA Lobby"))
            //{
            //    isLobbyActive = true;
            //}
            _event = new WorldItemDetail();
            _event.IdOfWorld = _WorldInfo.data.rows[i].id;
            _event.EnvironmentName = _WorldInfo.data.rows[i].name;
            try
            {
                if (_WorldInfo.data.rows[i].entityType != null)
                {
                    string IThumbnailDownloadURL = "";
                    //Modify Path for Thumbnail
                    if (!string.IsNullOrEmpty(_WorldInfo.data.rows[i].banner_new))
                    {
                        IThumbnailDownloadURL = _WorldInfo.data.rows[i].banner_new;

                        IThumbnailDownloadURL = _WorldInfo.data.rows[i].banner_new.Replace("https://cdn.xana.net/xanaprod", "https://aydvewoyxq.cloudimg.io/_xanaprod_/xanaprod");
                        // Test-net
                        IThumbnailDownloadURL = IThumbnailDownloadURL.Replace("https://cdn.xana.net/apitestxana/Defaults", "https://aydvewoyxq.cloudimg.io/_apitestxana_/apitestxana/Defaults");
                        // Main-net
                        IThumbnailDownloadURL = IThumbnailDownloadURL.Replace("https://ik.imagekit.io/xanalia/xanaprod/Defaults", "https://aydvewoyxq.cloudimg.io/_xanaprod_/xanaprod/Defaults");
                        _event.ThumbnailDownloadURL = IThumbnailDownloadURL + "?width=" + 640 + "&height=" + 360;
                    }
                    else
                    {
                        IThumbnailDownloadURL = _WorldInfo.data.rows[i].thumbnail.Replace("https://cdn.xana.net/xanaprod", "https://aydvewoyxq.cloudimg.io/_xanaprod_/xanaprod");
                        // Test-net
                        IThumbnailDownloadURL = IThumbnailDownloadURL.Replace("https://cdn.xana.net/apitestxana/Defaults", "https://aydvewoyxq.cloudimg.io/_apitestxana_/apitestxana/Defaults");
                        // Main-net
                        IThumbnailDownloadURL = IThumbnailDownloadURL.Replace("https://ik.imagekit.io/xanalia/xanaprod/Defaults", "https://aydvewoyxq.cloudimg.io/_xanaprod_/xanaprod/Defaults");
                        _event.ThumbnailDownloadURL = IThumbnailDownloadURL + "?width=" + 640 + "&height=" + 360;
                    }
                }
            }
            catch
            {
                _event.ThumbnailDownloadURL = _WorldInfo.data.rows[i].thumbnail;
            }
            _event.BannerLink = _WorldInfo.data.rows[i].banner;
            _event.WorldDescription = _WorldInfo.data.rows[i].description;
            _event.EntityType = _WorldInfo.data.rows[i].entityType;
            _event.PressedIndex = int.Parse(_WorldInfo.data.rows[i].id);
            _event.UpdatedAt = _WorldInfo.data.rows[i].updatedAt;
            _event.CreatedAt = _WorldInfo.data.rows[i].createdAt;
            //_event.WorldVisitCount = _WorldInfo.data.rows[i].totalVisits; // TotalVisit Variable Used for Web
            _event.WorldVisitCount = _WorldInfo.data.rows[i].xanaAppVisitCount;
            _event.UserMicEnable = _WorldInfo.data.rows[i].userMicEnable;
            _event.isFavourite = _WorldInfo.data.rows[i].isFavourite;
            if (_WorldInfo.data.rows[i].tags != null)
                _event.WorldTags = _WorldInfo.data.rows[i].tags;

            //if (_WorldInfo.data.rows[i].creatorDetails != null)
            //{
            //    _event.Creator_Name = _WorldInfo.data.rows[i].creatorDetails.userName;
            //    _event.CreatorDescription = _WorldInfo.data.rows[i].creatorDetails.description;
            //    _event.CreatorAvatarURL = _WorldInfo.data.rows[i].creatorDetails.avatar;
            //}
            if (_WorldInfo.data.rows[i].user.userProfile != null)
            {
                if (!string.IsNullOrEmpty(_WorldInfo.data.rows[i].user.userProfile.bio))
                    _event.CreatorDescription = _WorldInfo.data.rows[i].user.userProfile.bio;

                _event.CreatorDescription = _WorldInfo.data.rows[i].user.userProfile.bio;
            }
            if (_WorldInfo.data.rows[i].user != null)
            {
                _event.UserAvatarURL = _WorldInfo.data.rows[i].user.avatar;
            }
            if (_WorldInfo.data.rows[i].entityType == WorldType.USER_WORLD.ToString())
            {
                _event.Creator_Name = _WorldInfo.data.rows[i].user.name;
                _event.CreatorDescription = _WorldInfo.data.rows[i].user.userProfile.bio;
                _event.UserAvatarURL = _WorldInfo.data.rows[i].user.avatar;
                _event.UserLimit = "10";
            }
            else
            {
                if (!string.IsNullOrEmpty(_WorldInfo.data.rows[i].user.name))
                    _event.Creator_Name = _WorldInfo.data.rows[i].user.name;
                else
                    _event.Creator_Name = "XANA";
                _event.UserLimit = _WorldInfo.data.rows[i].user_limit;
            }
            if (!string.IsNullOrEmpty(_WorldInfo.data.rows[i].creator))
                _event.Creator_Name = _WorldInfo.data.rows[i].creator;

            //if (_WorldInfo.data.rows[i].name.Contains("XANA Lobby"))
            //{
            //    isLobbyActive = true;
            //    // if(EventPrefabLobby.activeInHierarchy)
            //    EventPrefabLobby.GetComponent<WorldItemView>().InitItem(-1, Vector2.zero, _event);
            //}
            //else
            //{
            //WorldItemManager.AddWorld(_apiURL, _event);

            resultWorldList.Add(_event);
            //}
            //if (!isLobbyActive)
            //{
            //    if (EventPrefabLobby.gameObject.activeInHierarchy)
            //    {
            //        EventPrefabLobby.GetComponent<LobbyWorldViewFlagHandler>().ActivityFlag(false);
            //        EventPrefabLobby.SetActive(false);
            //        AllWorldTabReference.LobbyInactiveCallBack();
            //    }
            //}  
        }
        /*if (WorldItemManager.gameObject.activeInHierarchy && _WorldInfo.data.count > 0)
        {
            WorldItemManager.DisplayWorlds(_apiURL);
            WorldItemManager.WorldLoadingText(APIURL.Temp);  //remove loading text from search screen
        }
        else if (WorldItemManager.gameObject.activeInHierarchy)
        {
            WorldItemManager.WorldLoadingText(_apiURL);
        }*/

        //if (_WorldInfo.data.rows.Count > 0)
        //{
        //    WorldLoadingText(APIURL.Temp);  //remove loading text from search screen
        //}
        //else
        //{
        //    if (searchWorldControllerRef.scroller.Container.transform.childCount > 3)
        //        WorldLoadingText(APIURL.SearchWorld);
        //    else
        //        WorldLoadingText(_apiURL);
        //}

        LoadingHandler.Instance.worldLoadingScreen.SetActive(false);
        LoadingHandler.Instance.SearchLoadingCanvas.SetActive(false);
        searchWorldControllerRef.LoadData(_WorldInfo.data.rows.Count, seeAllPN);
        //if (!GameManager.Instance.UiManager.IsSplashActive)
        //{
        //    Invoke(nameof(ShowTutorial), 1f);
        //}

    }
    public void SetFriendsJoinedWorldInfo(RowList _worldInfo, WorldItemView worldItemView)
    {
        WorldItemDetail _event;
        _event = new WorldItemDetail();
        _event.IdOfWorld = _worldInfo.id;
        _event.EnvironmentName = _worldInfo.name;
        try
        {
            if (_worldInfo.entityType != null)
            {
                string IThumbnailDownloadURL = "";
                //Modify Path for Thumbnail
                if (!string.IsNullOrEmpty(_worldInfo.banner_new))
                {
                    IThumbnailDownloadURL = _worldInfo.banner_new;
                    IThumbnailDownloadURL = _worldInfo.banner_new.Replace("https://cdn.xana.net/xanaprod", "https://aydvewoyxq.cloudimg.io/_xanaprod_/xanaprod");
                    // Test-net
                    IThumbnailDownloadURL = IThumbnailDownloadURL.Replace("https://cdn.xana.net/apitestxana/Defaults", "https://aydvewoyxq.cloudimg.io/_apitestxana_/apitestxana/Defaults");
                    // Main-net
                    IThumbnailDownloadURL = IThumbnailDownloadURL.Replace("https://ik.imagekit.io/xanalia/xanaprod/Defaults", "https://aydvewoyxq.cloudimg.io/_xanaprod_/xanaprod/Defaults");
                    _event.ThumbnailDownloadURL = IThumbnailDownloadURL + "?width=" + 640 + "&height=" + 360;
                }
                else
                {
                    IThumbnailDownloadURL = _worldInfo.thumbnail.Replace("https://cdn.xana.net/xanaprod", "https://aydvewoyxq.cloudimg.io/_xanaprod_/xanaprod");
                    // Test-net
                    IThumbnailDownloadURL = IThumbnailDownloadURL.Replace("https://cdn.xana.net/apitestxana/Defaults", "https://aydvewoyxq.cloudimg.io/_apitestxana_/apitestxana/Defaults");
                    // Main-net
                    IThumbnailDownloadURL = IThumbnailDownloadURL.Replace("https://ik.imagekit.io/xanalia/xanaprod/Defaults", "https://aydvewoyxq.cloudimg.io/_xanaprod_/xanaprod/Defaults");
                    _event.ThumbnailDownloadURL = IThumbnailDownloadURL + "?width=" + 640 + "&height=" + 360;
                }
            }
        }
        catch
        {
            Debug.LogError("Check Exception world thumbnail Image");
            _event.ThumbnailDownloadURL = _worldInfo.thumbnail;
        }
        _event.BannerLink = _worldInfo.banner;
        _event.WorldDescription = _worldInfo.description;
        _event.EntityType = _worldInfo.entityType;
        _event.PressedIndex = int.Parse(_worldInfo.id);
        _event.UpdatedAt = _worldInfo.updatedAt;
        _event.CreatedAt = _worldInfo.createdAt;
        _event.WorldVisitCount = _worldInfo.totalVisits;
        _event.UserMicEnable = _worldInfo.userMicEnable;
        _event.isFavourite = _worldInfo.isFavourite;
        if (_worldInfo.tags != null)
            _event.WorldTags = _worldInfo.tags;

        if (_worldInfo.user.userProfile != null)
        {
            if (!string.IsNullOrEmpty(_worldInfo.user.userProfile.bio))
                _event.CreatorDescription = _worldInfo.user.userProfile.bio;

            //_event.CreatorDescription = _WorldInfo.data.rows[i].user.userProfile.bio;

            if (_worldInfo.entityType == WorldType.USER_WORLD.ToString())
            {
                _event.Creator_Name = _worldInfo.user.name;
                //_event.CreatorDescription = _WorldInfo.data.rows[i].creatorDetails.description; // due to wrong API response commited this
                _event.CreatorDescription = _worldInfo.user.userProfile.bio;
                _event.UserAvatarURL = _worldInfo.user.avatar;
                _event.UserLimit = "10";
            }
            else
            {
                if (!string.IsNullOrEmpty(_worldInfo.user.name))
                    _event.Creator_Name = _worldInfo.user.name;
                else
                    _event.Creator_Name = "XANA";

                if (!string.IsNullOrEmpty(_worldInfo.creator))
                    _event.Creator_Name = _worldInfo.creator;

                if (!string.IsNullOrEmpty(_worldInfo.user.avatar))
                    _event.UserAvatarURL = _worldInfo.user.avatar;
            }
        }
        _event.UserLimit = _worldInfo.user_limit;

        worldItemView.InitItem(_event);
    }

    public void WorldLoadingText(APIURL aPIURL)
    {

        LoadingHandler.Instance.SearchLoadingCanvas.SetActive(false);
        switch (aPIURL)
        {
            case APIURL.FeaturedSpaces:
                worldFoundText.text = "";
                return;
            case APIURL.HotSpaces:
                worldFoundText.text = "";
                return;
            case APIURL.HotGames:
                worldFoundText.text = "";
                return;
            case APIURL.MySpace:
                worldFoundText.text = "";
                return;
            case APIURL.FolloingSpace:
                worldFoundText.text = "";
                return;
            case APIURL.SearchWorld:
                worldFoundText.text = TextLocalization.GetLocaliseTextByKey("No space found with given search key");
                return;
            case APIURL.SearchWorldByTag:
                worldFoundText.text = TextLocalization.GetLocaliseTextByKey("No space found with given search tag");
                return;
            case APIURL.Temp:
                worldFoundText.text = "";
                return;
            default:
                worldFoundText.text = TextLocalization.GetLocaliseTextByKey("No space found with given search key");
                return;
        }
    }

    public void ShowTutorial()
    {
        TutorialsHandler.instance.ShowTutorials();
    }

    public void WorldScrollReset()
    {
        searchWorldControllerRef.scroller.ClearAll();
        searchWorldControllerRef.ClearData();
    }
    /*public void WorldPageStateHandler(bool _checkCheck)
    {
        WorldItemManager.WorldPageStateHandler(_checkCheck);
    }
    public void ClearWorldScrollWorlds()
    {
        WorldItemManager.ClearWorldScrollWorlds();
    }*/
    private void CreateLightingAsset(WorldItemView _event)
    {
        string path = "Assets/Resources/Environment Data/" + _event.m_EnvironmentName + "Data";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        EnvironmentProperties Prop = null;
        if (!Directory.Exists(path + "/LightingData"))
        {
            Directory.CreateDirectory(path + "/LightingData");
            Prop = ScriptableObject.CreateInstance<EnvironmentProperties>();
#if UNITY_EDITOR
            AssetDatabase.CreateAsset(Prop, path + "/LightingData/LightingData.asset");
            AssetDatabase.SaveAssets();
#endif
        }
    }

    public void OnClickEnterAsParticipant()
    {
        CloseFightingModulePopUp();
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        FightingModuleManager.Instance.OnClickMainMenu();
    }
    public void OnClickEnterAsSpectator()
    {
        isCheckFightingModulePopUp = true;
        CloseFightingModulePopUp();
        WorldItemView.m_EnvName = "BreakingDown Arena";
        JoinEvent();
    }
    public void CloseFightingModulePopUp()
    {
        fightingModulePopUp.SetActive(false);
    }

    public async void JoinEvent()
    {
        MainSceneEventHandler.MakeScreenSpaceAdditive?.Invoke();

        //MainSceneEventHandler.MemoryRelaseAfterLoading?.Invoke();
        
        /// <summary>
        /// As creator name is different from actual scene name
        /// </summary>
        if (WorldItemView.m_EnvName == "D + Infinity Labo" || WorldItemView.m_EnvName == "D +  Infinity Labo")
        {
            WorldItemView.m_EnvName = "D_Infinity_Labo";
            ConstantsHolder.xanaConstants.EnviornmentName = WorldItemView.m_EnvName;
            GlobalConstants.SendFirebaseEvent(GlobalConstants.FirebaseTrigger.THA_Home_Thumbnail_PlayBtn.ToString());
        }


        _callSingleTime = true;
        //if (!ConstantsHolder.loggedIn && PlayerPrefs.GetInt("IsLoggedIn") == 0)
        //{
        //    if (WorldItemView.m_EnvName != "DEEMO THE MOVIE Metaverse Museum")    /////// Added By Abdullah Rashid 
        //    {
        //        //UserLoginSignupManager.instance.LoginRegisterScreen.transform.SetAsLastSibling();
        //        UserLoginSignupManager.instance.LoginRegisterScreen.SetActive(true);
        //    }
        //    else
        //    {
        //        if (!ConstantsHolder.xanaConstants.IsDeemoNFT)
        //        {
        //            Debug.Log("YOU DONT HAVE DEEMO NFT");
        //            GameManager.Instance.RequiredNFTPopUP.SetActive(true);
        //            return;
        //        }
        //    }
        //}
        //else
        //{
        if (PlayerPrefs.HasKey("Equiped"))
        {
            Task<bool> task = UserLoginSignupManager.instance._web3APIforWeb2.CheckSpecificNFTAndReturnAsync((PlayerPrefs.GetInt("nftID")).ToString());
            bool _IsInOwnerShip = await task;
            if (!_IsInOwnerShip)
            {
                PlayerPrefs.DeleteKey("Equiped");
                PlayerPrefs.DeleteKey("nftID");
                ConstantsHolder.xanaConstants.isNFTEquiped = false;
                BoxerNFTEventManager.OnNFTUnequip?.Invoke();
                NftDataScript.Instance.NftWorldEquipPanel.SetActive(true);
                return;
            }
            else
            {
                List<List> fighterNFTlist = UserLoginSignupManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTlistdata.list.FindAll(o => o.collection.name.StartsWith("XANA x BreakingDown"));
                List list = fighterNFTlist.Find(o => o.nftId.Equals(PlayerPrefs.GetInt("Equiped")));
                if (list != null)
                {
                    HaveFighterNFT = true;
                }
                else
                {
                    HaveFighterNFT = false;
                }
                if (WorldItemView.m_EnvName == "BreakingDown Arena" && !isCheckFightingModulePopUp && HaveFighterNFT)
                {
                    Debug.Log("Breaking down Arena World");
                    fightingModulePopUp.SetActive(true);
                    return;
                }
            }
        }
        //if (WorldItemView.m_EnvName == "DEEMO THE MOVIE Metaverse Museum")    /////// Added By Abdullah Rashid 
        //{
        //    if (!ConstantsHolder.xanaConstants.IsDeemoNFT)
        //    {
        //        GameManager.Instance.RequiredNFTPopUP.SetActive(true);
        //        return;
        //    }
        //}
        //AssetBundle.UnloadAllAssetBundles(false);
        //Resources.UnloadUnusedAssets();
        //GC.Collect();
        ConstantsHolder.xanaConstants.EnviornmentName = WorldItemView.m_EnvName;
        //LoadingHandler.Instance.ShowFadderWhileOriantationChanged(ScreenOrientation.LandscapeLeft);
        LoadingHandler.Instance.ShowLoading();
        LoadingHandler.Instance.UpdateLoadingSlider(0);
        LoadingHandler.Instance.UpdateLoadingStatusText("Loading World");
        await Task.Delay(500);
        //this is added to fix 20% loading stuck issue internally photon reload scenes to sync 
        //Photon.Pun.PhotonHandler.levelName = "GamePlayScene";
         LoadingHandler.Instance.LoadSceneByIndex("GamePlayScene");
        //PhotonNetwork.LoadLevel("GamePlayScene");
        // }
        if (WorldItemView.m_EnvName == "ZONE-X")
            GlobalConstants.SendFirebaseEvent(GlobalConstants.FirebaseTrigger.Home_Thumbnail_PlayBtn.ToString());
    }

    public async void JoinBuilderWorld()
    {
        MainSceneEventHandler.MakeScreenSpaceAdditive?.Invoke();

        //MainSceneEventHandler.MemoryRelaseAfterLoading?.Invoke();

        
        //if (!ConstantsHolder.loggedIn && PlayerPrefs.GetInt("IsLoggedIn") == 0)
        //{
        //    if (WorldItemView.m_EnvName != "DEEMO THE MOVIE Metaverse Museum")    /////// Added By Abdullah Rashid 
        //    {
        //        GameManager.Instance.UiManager.LoginRegisterScreen.transform.SetAsLastSibling();
        //        GameManager.Instance.UiManager.LoginRegisterScreen.SetActive(true);
        //    }
        //    else
        //    {
        //        if (!ConstantsHolder.xanaConstants.IsDeemoNFT)
        //        {
        //            Debug.Log("YOU DONT HAVE DEEMO NFT");
        //            GameManager.Instance.RequiredNFTPopUP.SetActive(true);
        //            return;
        //        }
        //    }
        //}
        //else
        //{
        if (PlayerPrefs.HasKey("Equiped"))
        {
            Task<bool> task = UserLoginSignupManager.instance._web3APIforWeb2.CheckSpecificNFTAndReturnAsync((PlayerPrefs.GetInt("nftID")).ToString());
            bool _IsInOwnerShip = await task;
            if (!_IsInOwnerShip)
            {
                PlayerPrefs.DeleteKey("Equiped");
                PlayerPrefs.DeleteKey("nftID");
                ConstantsHolder.xanaConstants.isNFTEquiped = false;
                BoxerNFTEventManager.OnNFTUnequip?.Invoke();
                NftDataScript.Instance.NftWorldEquipPanel.SetActive(true);
                return;
            }
            else
            {
                print("NFT is in your OwnerShip Enjoy");
            }
            //if (WorldItemView.m_EnvName == "DEEMO THE MOVIE Metaverse Museum")    /////// Added By Abdullah Rashid 
            //{
            //    if (!ConstantsHolder.xanaConstants.IsDeemoNFT)
            //    {
            //        GameManager.Instance.RequiredNFTPopUP.SetActive(true);
            //        return;
            //    }
            //}
        }
        ConstantsHolder.xanaConstants.EnviornmentName = WorldItemView.m_EnvName;
        //LoadingHandler.Instance.ShowFadderWhileOriantationChanged(ScreenOrientation.LandscapeLeft);
        LoadingHandler.Instance.ShowLoading();
        LoadingHandler.Instance.UpdateLoadingSlider(0);
        LoadingHandler.Instance.UpdateLoadingStatusText("Loading World");
        await Task.Delay(500);
        //this is added to fix 20% loading stuck issue internally photon reload scenes to sync 
        //Photon.Pun.PhotonHandler.levelName = "Builder";
        LoadingHandler.Instance.LoadSceneByIndex("Builder");
        //PhotonNetwork.LoadLevel("Builder");

        // }
    }

    private IEnumerator Check_Orientation(Action CallBack)
    {
        CheckAgain:
        yield return new WaitForSeconds(.2f);
        if (Screen.orientation == ScreenOrientation.LandscapeLeft || ConstantsHolder.xanaConstants.JjWorldSceneChange)
        {
            orientationchanged = true;
        }
        if (orientationchanged)
        {
            LoadingHandler.Instance.Loading_WhiteScreen.SetActive(false);
            CallBack();
        }
        else
            goto CheckAgain;

    }

    bool _callSingleTime = false;
    public void PlayWorld()
    {
        // For Analitics & User Count
        if (!_callSingleTime)
        {
            string worldType = "";
            if (ConstantsHolder.xanaConstants.isBuilderScene)
                worldType = "USER";
            else if (ConstantsHolder.xanaConstants.IsMuseum)
                worldType = "MUSEUM";
            else
                worldType = "ENVIRONMENT";

            if (ConstantsHolder.xanaConstants.EnviornmentName.Contains("Lobby"))
            {
                if ((ConstantsGod.API_BASEURL.Contains("test")))
                    ConstantsHolder.xanaConstants.customWorldId = 163;
                else
                    ConstantsHolder.xanaConstants.customWorldId = 77;

                worldType = "ENVIRONMENT";
            }
            UserAnalyticsHandler.onGetWorldId?.Invoke(ConstantsHolder.xanaConstants.customWorldId, worldType);
        }
        if (ConstantsHolder.xanaConstants.isBuilderScene)
        {
            //if (!ConstantsHolder.xanaConstants.JjWorldSceneChange)
            //{
            //    LoadingHandler.Instance.ShowFadderWhileOriantationChanged(ScreenOrientation.LandscapeLeft);
            //}
            ConstantsHolder.xanaConstants.EnviornmentName = WorldItemView.m_EnvName;
            LoadingHandler.Instance.ShowLoading();
            LoadingHandler.Instance.UpdateLoadingSlider(0);
            LoadingHandler.Instance.UpdateLoadingStatusText("Loading World");
            //this is added to fix 20% loading stuck issue internally photon reload scenes to sync 
            StartCoroutine(JoinWorldDelay());
        }
        else
        {
            //if (!ConstantsHolder.xanaConstants.JjWorldSceneChange)
            //{
            //    LoadingHandler.Instance.ShowFadderWhileOriantationChanged(ScreenOrientation.LandscapeLeft);
            //}
            ConstantsHolder.xanaConstants.EnviornmentName = WorldItemView.m_EnvName;
            LoadingHandler.Instance.ShowLoading();
            LoadingHandler.Instance.UpdateLoadingSlider(0);
            LoadingHandler.Instance.UpdateLoadingStatusText("Loading World");
            //this is added to fix 20% loading stuck issue internally photon reload scenes to sync 
            // Photon.Pun.PhotonHandler.levelName = "GamePlayScene";
             LoadingHandler.Instance.LoadSceneByIndex("GamePlayScene");
           // PhotonNetwork.LoadLevel("GamePlayScene");

        }
    }

    IEnumerator JoinWorldDelay()
    {
        yield return new WaitForSeconds(2f);
        //Photon.Pun.PhotonHandler.levelName = "Builder";
         LoadingHandler.Instance.LoadSceneByIndex("Builder");
       // PhotonNetwork.LoadLevel("Builder");

    }

    public void LoadJjworld()
    {
        SetAutoSwtichStreaming();
        if (ConstantsHolder.xanaConstants.JjWorldSceneChange)
        {
            Debug.LogError("load jj scene here");
            LoadingHandler.Instance.characterLoading.SetActive(false);
            LoadingHandler.Instance.presetCharacterLoading.SetActive(false);
            LoadingHandler.Instance.characterLoading.SetActive(false);
            LoadingHandler.Instance.worldLoadingScreen.SetActive(false);
          //  LoadingHandler.Instance.loadingPanel.SetActive(false);
            LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
            LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
            ConstantsHolder.xanaConstants.EnviornmentName = ConstantsHolder.xanaConstants.JjWorldTeleportSceneName;
            WorldItemView.m_EnvName = ConstantsHolder.xanaConstants.JjWorldTeleportSceneName;
            if (ConstantsHolder.xanaConstants.JjWorldTeleportSceneName == "Xana Festival")
            {
                ConstantsHolder.userLimit = 16;
            }
            if (ConstantsHolder.xanaConstants.JjWorldTeleportSceneName == "XANA_DUNE")
            {
                ConstantsHolder.userLimit = 1;
            }
            else
            {
                if (ConstantsHolder.xanaConstants.isBuilderScene)
                {
                    ConstantsHolder.userLimit = 10;
                }
                else
                {
                    ConstantsHolder.userLimit = 15;
                }
            }
            PlayWorld();
        }
    }

    public void GoToUGC()
    {
        GameManager.Instance.HomeCameraInputHandler(false);
        OnScreenTabStateChange?.Invoke(BackButtonHandler.screenTabs.FaceCam);
        SceneManager.LoadScene("UGC");
    }
    public void ClearHomePageData()
    {
        //worldSpaceHomeScreenRef.RemoveThumbnailImages();
    }
}
[Serializable]
class AutoSwtichEnv
{
    public string name;
    public bool isMussuem = false;
    public int mainnetId;
    public int testnetId;
}
[System.Serializable]
public class WorldsInfo
{
    public bool success;
    public DataClass data;
    public string msg;
}
[System.Serializable]
public class DataClass
{
    public int count;
    public List<RowList> rows;
}

[System.Serializable]
public class SingleWorldInfo
{
    public bool success;
    public RowList data;
    public string msg;
}

[System.Serializable]
public class RowList
{
    public string id;
    public string name;
    public string user_limit;
    public string thumbnail;
    public string banner;
    public string thumbnail_new;
    public string banner_new;
    public string description;
    public string creator;
    public string createdAt;
    public string updatedAt;
    public string entityType;
    public string status;
    public string createdBy;
    public string[] tags;
    public string totalVisits;
    public string xanaAppVisitCount;
    public bool userMicEnable;

    public bool isFavourite;
    public UserInfo user;
    public WorldCreatorDetail creatorDetails;
}
[System.Serializable]
public class UserInfo
{
    public string id;
    public string name;
    public string email;
    public string avatar;
    public UserProfileInfo userProfile;
}

[System.Serializable]
public class UserProfileInfo
{
    public string bio;
}

[Serializable]
public class WorldCreatorDetail
{
    public string userName;
    public string avatar;
    public string description;
}


[Serializable]
public class WorldItemDetail
{
    public string IdOfWorld;
    public string EnvironmentName;
    public string WorldDescription;
    public string ThumbnailDownloadURL;
    public string ThumbnailDownloadURLHigh;
    //public string CreatorName;
    public string CreatedAt;
    public string UserLimit;
    public string UserAvatarURL;
    public string UpdatedAt = "00";
    public string EntityType = "None";
    public string BannerLink;
    public int PressedIndex;
    public string[] WorldTags;
    public string Creator_Name;
    public string CreatorAvatarURL;
    public string CreatorDescription;
    public string WorldVisitCount;
    public bool UserMicEnable;
    public bool isFavourite;
}





//public enum APIURL
//{
//    Hot, AllWorld, MyWorld, GameWorld, EventWorld, SearchWorld, TestWorld, SearchWorldByTag
//}

public enum APIURL
{
    FeaturedSpaces, HotSpaces, HotGames, FolloingSpace, MySpace, SearchWorld, SearchWorldByTag, Temp
}

public enum WorldType
{
    None, MUSEUM, ENVIRONMENT, USER_WORLD
}