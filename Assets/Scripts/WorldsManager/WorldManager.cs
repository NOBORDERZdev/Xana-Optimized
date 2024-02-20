using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using UnityEditor;
using System.Threading.Tasks;
using Photon.Pun.Demo.PunBasics;
using UnityEngine.SceneManagement;
using DG.Tweening;

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
    public int hotSpacePN = 1, hotGamesPN = 1, followingPN = 1, mySpacesPN = 1;
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

    /*public WorldItemManager WorldItemManager;*/
    public WorldsInfo _WorldInfo;
    public AllWorldManage AllWorldTabReference;
    public WorldSpacesHomeScreen worldSpaceHomeScreenRef;
    public WorldItemPreviewTab worldItemPreviewTabRef;
    public WorldSearchManager worldSearchManager;
    public SearchWorldController searchWorldControllerRef;
    public static WorldManager instance;
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
    void Start()
    {
        //ChangeWorldTab(APIURL.Hot);
        Invoke(nameof(LoadJjworld), 0);
    }
    /*public void CheckWorldTabAndReset(APIURL tab)
    {
        if (WorldItemManager.GetWorldCountPresentInMemory(tab.ToString()) > 0)
        {
            //Debug.LogError("display world");
            WorldItemManager.DisplayWorlds(tab);
            LoadingHandler.Instance.worldLoadingScreen.SetActive(false);
        }
        else
        {
            //Debug.LogError("api hit again");
            ChangeWorldTab(tab);
        }
    }
    public void ChangeWorld(APIURL tab)
    {
        if (UIManager.Instance.IsSplashActive)
        {
            LoadingHandler.Instance.worldLoadingScreen.SetActive(false);
        }
        else
        {
            LoadingHandler.Instance.worldLoadingScreen.SetActive(true);
        }

        WorldItemManager.DisplayWorlds(APIURL.Temp);
        StartCoroutine(WorldCall(tab));
    }
    IEnumerator WorldCall(APIURL tab)
    {
        //while (!dataIsFatched)
        //{
        //    Debug.LogError("Clear Fetch");
        //    yield return null;
        //    NotProcessRequest = true;
        //}
        yield return new WaitForEndOfFrame();
        CheckWorldTabAndReset(tab);
    }*/
    public void ChangeWorldTab(APIURL tab)
    {
        aPIURLGlobal = tab;
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
        if (XanaConstants.xanaConstants.isCameraMan)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;

            XanaConstants.xanaConstants.JjWorldSceneChange = true;
            XanaConstants.xanaConstants.JjWorldTeleportSceneName = AutoSwtichWorldList[AutoSwtichIndex].name;
            XanaConstants.xanaConstants.IsMuseum = AutoSwtichWorldList[AutoSwtichIndex].isMussuem;
            if (APIBaseUrlChange.instance.IsXanaLive)
            {
                XanaConstants.xanaConstants.MuseumID = AutoSwtichWorldList[AutoSwtichIndex].mainnetId.ToString();
            }
            else
            {
                XanaConstants.xanaConstants.MuseumID = AutoSwtichWorldList[AutoSwtichIndex].testnetId.ToString();
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
            case APIURL.HotSpaces:
                return ConstantsGod.API_BASEURL + ConstantsGod.HOTSPACES + hotSpacePN + "/" + recordPerPage;
            case APIURL.HotGames:
                return ConstantsGod.API_BASEURL + ConstantsGod.HOTGAMES /*+ status + "/" */+ hotGamesPN + "/" + recordPerPage;
            case APIURL.MySpace:
                return ConstantsGod.API_BASEURL + ConstantsGod.MYBUILDERWORLDS + status + "/" + mySpacesPN + "/" + recordPerPage;
            case APIURL.FolloingSpace:
                return ConstantsGod.API_BASEURL + ConstantsGod.FOLLOWINGSPACES + followingPN + "/" + recordPerPage; //+ "/" + status + "/GAME";
            //case APIURL.EventWorld:
            //    return ConstantsGod.API_BASEURL + ConstantsGod.WORLDSBYCATEGORY + pageNumberEventWorld + "/" + pageCount + "/" + status + "/EVENT";
            //case APIURL.TestWorld:
            //    return ConstantsGod.API_BASEURL + ConstantsGod.WORLDSBYCATEGORY + pageNumberTestWorld + "/" + pageCount + "/" + status + "/TEST";
            case APIURL.SearchWorld:
                return ConstantsGod.API_BASEURL + ConstantsGod.SearchWorldAPI + SearchKey + "/" + SearchPageNumb + "/" + SearchPageSize;
            case APIURL.SearchWorldByTag:
                return ConstantsGod.API_BASEURL + ConstantsGod.SEARCHWORLDBYTAG + SearchKey + "/" + SearchTagPageNumb + "/" + SearchTagPageSize;
            default:
                return ConstantsGod.API_BASEURL + ConstantsGod.HOTSPACES + hotSpacePN + "/" + recordPerPage;
        }
    }
    void UpdatePageNumber(APIURL aPIURL)
    {
        switch (aPIURL)
        {
            case APIURL.HotSpaces:
                hotSpacePN += 1;
                return;
            case APIURL.HotGames:
                hotGamesPN += 1;
                return;
            case APIURL.MySpace:
                mySpacesPN += 1;
                return;
            case APIURL.FolloingSpace:
                followingPN += 1;
                return;
            /*case APIURL.EventWorld:
                pageNumberEventWorld += 1;
                return;*/
            case APIURL.SearchWorld:
                SearchPageNumb += 1;
                return;
            case APIURL.SearchWorldByTag:
                SearchTagPageNumb += 1;
                return;
            /*case APIURL.TestWorld:
                pageNumberTestWorld += 1;
                return;*/
            default:
                hotSpacePN += 1;
                return;
        }
    }
    bool NotProcessRequest = false;
    int CallBackCheck = 0;
    public void GetBuilderWorlds(APIURL aPIURL, Action<bool> CallBack)
    {
        finalAPIURL = PrepareApiURL(aPIURL);
        loadOnce = false;
        //Debug.LogError(finalAPIURL);
        StartCoroutine(FetchUserMapFromServer(finalAPIURL, (isSucess) =>
        {
            if (isSucess)
            {
                CallBackCheck = 0;
                InstantiateWorlds(aPIURL, isSucess);
                dataIsFatched = true;
                UpdatePageNumber(aPIURL);
                if (_WorldInfo.data.count > 0)
                    CallBack(true);
                else
                    CallBack(false);
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
                GetBuilderWorlds(aPIURLGlobal, (a) => { });
                CallBack(false);
            }
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
        searchWorldControllerRef.scroller.ScrollPosition = 0f;    // my changes

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
            _event.WorldVisitCount = _WorldInfo.data.rows[i].totalVisits;
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

            if (_WorldInfo.data.rows[i].entityType == WorldType.USER_WORLD.ToString())
            {
                _event.Creator_Name = _WorldInfo.data.rows[i].user.name;
                _event.CreatorDescription = _WorldInfo.data.rows[i].creatorDetails.description;
                _event.UserAvatarURL = _WorldInfo.data.rows[i].user.avatar;
                _event.UserLimit = "15";
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


        searchWorldControllerRef.LoadData(_WorldInfo.data.rows.Count);
        if (_WorldInfo.data.count > 0)
        {
            WorldLoadingText(APIURL.Temp);  //remove loading text from search screen
        }
        else
        {
            if (searchWorldControllerRef.scroller.Container.transform.childCount > 3)
                WorldLoadingText(APIURL.Temp);
            else
                WorldLoadingText(_apiURL);
        }



        previousSearchKey = SearchKey;
        LoadingHandler.Instance.worldLoadingScreen.SetActive(false);
        //if (!UIManager.Instance.IsSplashActive)
        //{
        //    Invoke(nameof(ShowTutorial), 1f);
        //}

    }


    public void WorldLoadingText(APIURL aPIURL)
    {

        LoadingHandler.Instance.SearchLoadingCanvas.SetActive(false);
        switch (aPIURL)
        {
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
                worldFoundText.text = "No world found with given search key";
                return;
            case APIURL.SearchWorldByTag:
                worldFoundText.text = "No world found with given search tag";
                return;
            case APIURL.Temp:
                worldFoundText.text = "";
                return;
            default:
                worldFoundText.text = "No world found with given search key";
                return;
        }
    }




    public void ShowTutorial()
    {
        TutorialsManager.instance.ShowTutorials();
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
        _callSingleTime = true;
        if (!UserRegisterationManager.instance.LoggedIn && PlayerPrefs.GetInt("IsLoggedIn") == 0)
        {
            if (WorldItemView.m_EnvName != "DEEMO THE MOVIE Metaverse Museum")    /////// Added By Abdullah Rashid 
            {
                UIManager.Instance.LoginRegisterScreen.transform.SetAsLastSibling();
                UIManager.Instance.LoginRegisterScreen.SetActive(true);
            }
            else
            {
                if (!XanaConstants.xanaConstants.IsDeemoNFT)
                {
                    Debug.Log("YOU DONT HAVE DEEMO NFT");
                    GameManager.Instance.RequiredNFTPopUP.SetActive(true);
                    return;
                }
            }
        }
        else
        {
            if (PlayerPrefs.HasKey("Equiped"))
            {
                Task<bool> task = UserRegisterationManager.instance._web3APIforWeb2.CheckSpecificNFTAndReturnAsync((PlayerPrefs.GetInt("nftID")).ToString());
                bool _IsInOwnerShip = await task;
                if (!_IsInOwnerShip)
                {
                    PlayerPrefs.DeleteKey("Equiped");
                    PlayerPrefs.DeleteKey("nftID");
                    XanaConstants.xanaConstants.isNFTEquiped = false;
                    BoxerNFTEventManager.OnNFTUnequip?.Invoke();
                    NftDataScript.Instance.NftWorldEquipPanel.SetActive(true);
                    return;
                }
                else
                {
                    print("NFT is in your OwnerShip Enjoy " + PlayerPrefs.GetInt("Equiped"));
                }
            }
            List<List> fighterNFTlist = UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTlistdata.list.FindAll(o => o.collection.name.StartsWith("XANA x BreakingDown"));
            Debug.LogError("fighterNFTlist count: " + fighterNFTlist.Count);
            if (fighterNFTlist.Count > 0)
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
                FightingModuleManager.Instance.EqipRandomNFT();
                fightingModulePopUp.SetActive(true);
                return;
            }
            if (WorldItemView.m_EnvName == "DEEMO THE MOVIE Metaverse Museum")    /////// Added By Abdullah Rashid 
            {
                if (!XanaConstants.xanaConstants.IsDeemoNFT)
                {
                    GameManager.Instance.RequiredNFTPopUP.SetActive(true);
                    return;
                }
            }
            AssetBundle.UnloadAllAssetBundles(false);
            Resources.UnloadUnusedAssets();
            GC.Collect();
            XanaConstants.xanaConstants.EnviornmentName = WorldItemView.m_EnvName;
            //LoadingHandler.Instance.ShowFadderWhileOriantationChanged(ScreenOrientation.LandscapeLeft);
            LoadingHandler.Instance.ShowLoading();
            LoadingHandler.Instance.UpdateLoadingSlider(0);
            LoadingHandler.Instance.UpdateLoadingStatusText("Loading World");
            //this is added to fix 20% loading stuck issue internally photon reload scenes to sync 
            Photon.Pun.PhotonHandler.levelName = "AddressableScene";
            LoadingHandler.Instance.LoadSceneByIndex("AddressableScene");
        }
        if (WorldItemView.m_EnvName == "ZONE-X")
            GlobalConstants.SendFirebaseEvent(GlobalConstants.FirebaseTrigger.Home_Thumbnail_PlayBtn.ToString());
    }
    public async void JoinBuilderWorld()
    {
        if (!UserRegisterationManager.instance.LoggedIn && PlayerPrefs.GetInt("IsLoggedIn") == 0)
        {
            if (WorldItemView.m_EnvName != "DEEMO THE MOVIE Metaverse Museum")    /////// Added By Abdullah Rashid 
            {
                UIManager.Instance.LoginRegisterScreen.transform.SetAsLastSibling();
                UIManager.Instance.LoginRegisterScreen.SetActive(true);
            }
            else
            {
                if (!XanaConstants.xanaConstants.IsDeemoNFT)
                {
                    Debug.Log("YOU DONT HAVE DEEMO NFT");
                    GameManager.Instance.RequiredNFTPopUP.SetActive(true);
                    return;
                }
            }
        }
        else
        {
            if (PlayerPrefs.HasKey("Equiped"))
            {
                Task<bool> task = UserRegisterationManager.instance._web3APIforWeb2.CheckSpecificNFTAndReturnAsync((PlayerPrefs.GetInt("nftID")).ToString());
                bool _IsInOwnerShip = await task;
                if (!_IsInOwnerShip)
                {
                    PlayerPrefs.DeleteKey("Equiped");
                    PlayerPrefs.DeleteKey("nftID");
                    XanaConstants.xanaConstants.isNFTEquiped = false;
                    BoxerNFTEventManager.OnNFTUnequip?.Invoke();
                    NftDataScript.Instance.NftWorldEquipPanel.SetActive(true);
                    return;
                }
                else
                {
                    print("NFT is in your OwnerShip Enjoy");
                }
                if (WorldItemView.m_EnvName == "DEEMO THE MOVIE Metaverse Museum")    /////// Added By Abdullah Rashid 
                {
                    if (!XanaConstants.xanaConstants.IsDeemoNFT)
                    {
                        GameManager.Instance.RequiredNFTPopUP.SetActive(true);
                        return;
                    }
                }
            }
            XanaConstants.xanaConstants.EnviornmentName = WorldItemView.m_EnvName;
            //LoadingHandler.Instance.ShowFadderWhileOriantationChanged(ScreenOrientation.LandscapeLeft);
            LoadingHandler.Instance.ShowLoading();
            LoadingHandler.Instance.UpdateLoadingSlider(0);
            LoadingHandler.Instance.UpdateLoadingStatusText("Loading World");
            //this is added to fix 20% loading stuck issue internally photon reload scenes to sync 
            Photon.Pun.PhotonHandler.levelName = "Builder";
            LoadingHandler.Instance.LoadSceneByIndex("Builder");
        }
    }

    private IEnumerator Check_Orientation(Action CallBack)
    {
        CheckAgain:
        yield return new WaitForSeconds(.2f);
        if (Screen.orientation == ScreenOrientation.LandscapeLeft || XanaConstants.xanaConstants.JjWorldSceneChange)
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
            if (XanaConstants.xanaConstants.isBuilderScene)
                worldType = "USER";
            else if (XanaConstants.xanaConstants.IsMuseum)
                worldType = "MUSEUM";
            else
                worldType = "ENVIRONMENT";

            if (XanaConstants.xanaConstants.EnviornmentName.Contains("Lobby"))
            {
                if ((ConstantsGod.API_BASEURL.Contains("test")))
                    XanaConstants.xanaConstants.customWorldId = 163;
                else
                    XanaConstants.xanaConstants.customWorldId = 77;

                worldType = "ENVIRONMENT";
            }
            UserAnalyticsHandler.onGetWorldId?.Invoke(XanaConstants.xanaConstants.customWorldId, worldType);
        }
        if (XanaConstants.xanaConstants.isBuilderScene)
        {
            //if (!XanaConstants.xanaConstants.JjWorldSceneChange)
            //{
            //    LoadingHandler.Instance.ShowFadderWhileOriantationChanged(ScreenOrientation.LandscapeLeft);
            //}
            XanaConstants.xanaConstants.EnviornmentName = WorldItemView.m_EnvName;
            LoadingHandler.Instance.ShowLoading();
            LoadingHandler.Instance.UpdateLoadingSlider(0);
            LoadingHandler.Instance.UpdateLoadingStatusText("Loading World");
            //this is added to fix 20% loading stuck issue internally photon reload scenes to sync 
            StartCoroutine(JoinWorldDelay());
        }
        else
        {
            //if (!XanaConstants.xanaConstants.JjWorldSceneChange)
            //{
            //    LoadingHandler.Instance.ShowFadderWhileOriantationChanged(ScreenOrientation.LandscapeLeft);
            //}
            XanaConstants.xanaConstants.EnviornmentName = WorldItemView.m_EnvName;
            LoadingHandler.Instance.ShowLoading();
            LoadingHandler.Instance.UpdateLoadingSlider(0);
            LoadingHandler.Instance.UpdateLoadingStatusText("Loading World");
            //this is added to fix 20% loading stuck issue internally photon reload scenes to sync 
            Photon.Pun.PhotonHandler.levelName = "AddressableScene";
            LoadingHandler.Instance.LoadSceneByIndex("AddressableScene");
        }
    }

    IEnumerator JoinWorldDelay()
    {
        yield return new WaitForSeconds(2f);
        Photon.Pun.PhotonHandler.levelName = "Builder";
        LoadingHandler.Instance.LoadSceneByIndex("Builder");
    }

    public void LoadJjworld()
    {
        SetAutoSwtichStreaming();
        if (XanaConstants.xanaConstants.JjWorldSceneChange)
        {
            LoadingHandler.Instance.characterLoading.SetActive(false);
            LoadingHandler.Instance.presetCharacterLoading.SetActive(false);
            LoadingHandler.Instance.characterLoading.SetActive(false);
            LoadingHandler.Instance.worldLoadingScreen.SetActive(false);
            LoadingHandler.Instance.loadingPanel.SetActive(false);
            LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
            LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
            XanaConstants.xanaConstants.EnviornmentName = XanaConstants.xanaConstants.JjWorldTeleportSceneName;
            WorldItemView.m_EnvName = XanaConstants.xanaConstants.JjWorldTeleportSceneName;
            if (XanaConstants.xanaConstants.JjWorldTeleportSceneName == "Xana Festival")
            {
                XanaConstants.xanaConstants.userLimit = "16";
            }
            else
            {
                if (XanaConstants.xanaConstants.isBuilderScene)
                {
                    XanaConstants.xanaConstants.userLimit = "15";
                }
                else
                {
                    XanaConstants.xanaConstants.userLimit = "15";
                }
            }
            Launcher.sceneName = XanaConstants.xanaConstants.JjWorldTeleportSceneName;
            PlayWorld();
        }
    }

    public void GoToUGC()
    {
        GameManager.Instance.HomeCameraInputHandler(false);

        SceneManager.LoadScene("UGC");
    }
    public void ClearHomePageData()
    {
        worldSpaceHomeScreenRef.RemoveThumbnailImages();
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
    public bool isFavourite;
}





//public enum APIURL
//{
//    Hot, AllWorld, MyWorld, GameWorld, EventWorld, SearchWorld, TestWorld, SearchWorldByTag
//}

public enum APIURL
{
    HotSpaces, HotGames, FolloingSpace, MySpace, SearchWorld, SearchWorldByTag, Temp
}

public enum WorldType
{
    None, MUSEUM, ENVIRONMENT, USER_WORLD
}