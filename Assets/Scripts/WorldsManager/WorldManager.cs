using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using UnityEditor;
using AdvancedInputFieldPlugin;
using System.Threading.Tasks;
using Photon.Pun.Demo.PunBasics;
using UnityEngine.UI;

public class WorldManager : MonoBehaviour
{
    [Header("Main prefab")]
    public GameObject eventPrefab;
    public GameObject eventPrefabLobby;
    public GameObject eventPrefabTab;
    public GameObject descriptionParentPanel;
    [Header("Home Page Scrollviews")]
    public Transform listParentHotSection;
    public Transform listParentAllWorlds;
    public Transform listParentMyWorlds;
    public Transform listParentGameWorlds;
    public Transform listParentEventWorlds;
    private Transform listParent;

    [Header("world Page Scrollviews")]
    public Transform world_HotScroll;
    public Transform world_NewScroll;
    public Transform world_myworldScroll;
    public Transform world_GameWorldScroll;
    public Transform world_EventWorldScroll;

    [Header("Full World List")]
    private List<GameObject> hotWorldList = new List<GameObject>();
    private List<GameObject> newWorldList = new List<GameObject>();
    private List<GameObject> myworldWorldList = new List<GameObject>();
    private List<GameObject> gameWorldList = new List<GameObject>();
    private List<GameObject> eventWorldList = new List<GameObject>();

    [HideInInspector]
    public bool orientationchanged = false;

    [Header("Api Parameter's")]
    private string finalAPIURL;
    private string status = "Publish";  //Publish //Draft
    private int pageNumberHot = 1;
    private int pageNumberAllWorld = 1;
    private int pageNumberMyWorld = 1;
    private int pageNumberGameWorld = 1;
    private int pageNumberEventWorld = 1;
    private int pageCount = 200;
    private bool loadOnce = true;
    private bool dataIsFatched = false;
    public WorldsInfo _WorldInfo;
    private APIURL aPIURLGlobal;

    public static WorldManager instance;
    public AllWorldManage m_AllWorldManage;

    ScrollRect s1;
    ScrollSnapRect s2;


    [SerializeField]
    [NonReorderable]
    List<AutoSwtichEnv> AutoSwtichWorldList;

    static int AutoSwtichIndex=0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);


        if (!System.IO.Directory.Exists(Application.persistentDataPath + "/MainMenuData"))
        {
            //Debug.Log("start creating directory");
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

    // Start is called before the first frame update
    void Start()
    {
        if (XanaConstants.xanaConstants.screenType == XanaConstants.ScreenType.TabScreen)
            eventPrefab = eventPrefabTab;
        BuilderEventManager.OnWorldTabChange += OnWorldTabChange;
        BuilderEventManager.OnBuilderWorldLoad += GetBuilderWorlds;
        //ScrollRectEx.OnDragEndVerticalCustom += CheckForReloading;
        PixelPerfectScrollRect.OnDragEndVerticalCustom += CheckForReloading;


        OnWorldTabChange(APIURL.Hot, true);
        GetBuilderWorlds(APIURL.Hot, (a) => { });

        //Invoke(nameof(SetAutoSwtichStreaming),2);
        
        Invoke(nameof(LoadJjworld), 5);
    }

    void SetAutoSwtichStreaming(){ 
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
            //if (AutoSwtichIndex< AutoSwtichWorldList.Count-1)
            //{
            //    AutoSwtichIndex++;
            //}
            //else
            //{
            //    AutoSwtichIndex=0;
            //}
            LoadingHandler.Instance.streamingLoading.UpdateLoadingText(true);
            XanaConstants.xanaConstants.newStreamEntery=true;
         }    
    }

    private void OnDestroy()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        Resources.UnloadUnusedAssets();
        //   Caching.ClearCache();


        BuilderEventManager.OnWorldTabChange -= OnWorldTabChange;
        BuilderEventManager.OnBuilderWorldLoad -= GetBuilderWorlds;
        //ScrollRectEx.OnDragEndVerticalCustom -= CheckForReloading;
        PixelPerfectScrollRect.OnDragEndVerticalCustom -= CheckForReloading;
    }


    void CheckForReloading(float scrollPos)
    {
        //Debug.LogError(scrollPos);
        if (scrollPos < .1f && dataIsFatched && listParent.gameObject.activeInHierarchy)
        {
            loadOnce = true;
            dataIsFatched = false;
            GetBuilderWorlds(aPIURLGlobal, (a) => { });
        }
    }

    void OnWorldTabChange(APIURL _enumValue, bool isHomePage)
    {
        switch (_enumValue)
        {
            case APIURL.Hot:
                aPIURLGlobal = APIURL.Hot;
                if (isHomePage)
                    listParent = listParentHotSection;
                else
                    listParent = world_HotScroll;
                break;
            case APIURL.AllWorld:
                aPIURLGlobal = APIURL.AllWorld;
                if (isHomePage)
                    listParent = listParentAllWorlds;
                else
                    listParent = world_NewScroll;
                break;
            case APIURL.MyWorld:
                aPIURLGlobal = APIURL.MyWorld;
                if (isHomePage)
                    listParent = listParentMyWorlds;
                else
                    listParent = world_myworldScroll;
                break;
            case APIURL.GameWorld:
                aPIURLGlobal = APIURL.GameWorld;
                if (isHomePage)
                    listParent = listParentGameWorlds;
                else
                    listParent = world_GameWorldScroll;
                break;
            case APIURL.EventWorld:
                aPIURLGlobal = APIURL.EventWorld;
                if (isHomePage)
                    listParent = listParentEventWorlds;
                else
                    listParent = world_EventWorldScroll;
                break;
            default:
                aPIURLGlobal = APIURL.Hot;
                listParent = listParentHotSection;
                break;
        }
    }


    string PrepareApiURL(APIURL aPIURL)
    {
        switch (aPIURL)
        {
            case APIURL.Hot:
                return ConstantsGod.API_BASEURL + ConstantsGod.MUSEUMENVBUILDERWORLDSCOMBINED + pageNumberHot + "/" + pageCount;
            case APIURL.AllWorld:
                return ConstantsGod.API_BASEURL + ConstantsGod.ALLBUILDERWORLDS + status + "/" + pageNumberAllWorld + "/" + pageCount;
            case APIURL.MyWorld:
                return ConstantsGod.API_BASEURL + ConstantsGod.MYBUILDERWORLDS + status + "/" + pageNumberMyWorld + "/" + pageCount;
            case APIURL.GameWorld:
                return ConstantsGod.API_BASEURL + ConstantsGod.WORLDSBYCATEGORY + pageNumberGameWorld + "/" + pageCount + "/" + status + "/GAME";
            case APIURL.EventWorld:
                return ConstantsGod.API_BASEURL + ConstantsGod.WORLDSBYCATEGORY + pageNumberEventWorld + "/" + pageCount + "/" + status + "/EVENT";
            default:
                return ConstantsGod.API_BASEURL + ConstantsGod.MUSEUMENVBUILDERWORLDSCOMBINED + pageNumberHot + "/" + pageCount;
        }
    }

    void UpdatePageNumber(APIURL aPIURL)
    {
        switch (aPIURL)
        {
            case APIURL.Hot:
                pageNumberHot += 1;
                return;
            case APIURL.AllWorld:
                pageNumberAllWorld += 1;
                return;
            case APIURL.MyWorld:
                pageNumberMyWorld += 1;
                return;
            case APIURL.GameWorld:
                pageNumberGameWorld += 1;
                return;
            case APIURL.EventWorld:
                pageNumberEventWorld += 1;
                return;
            default:
                pageNumberHot += 1;
                return;
        }
    }


    public void GetBuilderWorlds(APIURL aPIURL, Action<bool> CallBack)
    {
        finalAPIURL = PrepareApiURL(aPIURL);
        loadOnce = false;
        LoadingHandler.Instance.worldLoadingScreen.SetActive(true);
        StartCoroutine(FetchUserMapFromServer(finalAPIURL, (isSucess) =>
        {
            if (isSucess)
            {
                InstantiateWorlds();
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
                GetBuilderWorlds(aPIURLGlobal, (a) => { });
                CallBack(false);
            }
        }));
    }

    IEnumerator FetchUserMapFromServer(string apiURL, Action<bool> callback)
    {
        Debug.Log("World API: " + apiURL);
        using (UnityWebRequest www = UnityWebRequest.Get(apiURL))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            //Debug.LogError("Token :- "+ ConstantsGod.AUTH_TOKEN);
            www.SendWebRequest();
            while (!www.isDone)
                yield return null;
            Debug.Log(www.downloadHandler.text);
            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                callback(false);
            }
            else
            {
                _WorldInfo = JsonUtility.FromJson<WorldsInfo>(www.downloadHandler.text);
                worldstr = www.downloadHandler.text;
                callback(true);
            }
        }
    }
    public string worldstr;
    bool isLobbyActive = false;
    void InstantiateWorlds()
    {
        for (int i = 0; i < _WorldInfo.data.rows.Count; i++)
        {
            GameObject TempObject;
            if (_WorldInfo.data.rows[i].name.Contains("XANA Lobby"))
            {
                isLobbyActive = true;
                TempObject = eventPrefabLobby;
                TempObject.transform.SetParent(listParent.transform.parent);
                TempObject.transform.SetAsFirstSibling();

            }
            else
            {
                TempObject = Instantiate(eventPrefab);
                TempObject.transform.SetParent(listParent);
            }
            FeedEventPrefab _event = TempObject.GetComponent<FeedEventPrefab>();

            if (PlayerPrefs.GetInt("ShowLiveUserCounter", 0) > 0)
            {
                _event.joinedUserCount.transform.parent.gameObject.SetActive(true);
            }

            _event.idOfObject = _WorldInfo.data.rows[i].id;
            _event.m_EnvironmentName = _WorldInfo.data.rows[i].name;
            try
            {
                _event.m_ThumbnailDownloadURL = _WorldInfo.data.rows[i].thumbnail.Replace("https://cdn.xana.net/xanaprod", "https://aydvewoyxq.cloudimg.io/_xanaprod_/xanaprod");

                if (_WorldInfo.data.rows[i].entityType == WorldType.USER_WORLD.ToString())
                {
                    _event.m_ThumbnailDownloadURL = _event.m_ThumbnailDownloadURL + "?width=" + 512 + "&height=" + 512;
                }
            }
            catch (Exception e)
            {
                _event.m_ThumbnailDownloadURL = _WorldInfo.data.rows[i].thumbnail;
            }
            _event.m_BannerLink = _WorldInfo.data.rows[i].banner;
            _event.m_WorldDescription = _WorldInfo.data.rows[i].description;
            _event.entityType = _WorldInfo.data.rows[i].entityType;
            _event.m_PressedIndex = int.Parse(_WorldInfo.data.rows[i].id);
            _event.updatedAt = _WorldInfo.data.rows[i].updatedAt;
            _event.createdAt = _WorldInfo.data.rows[i].createdAt;

            if (_WorldInfo.data.rows[i].tags != null)
                _event.worldTags = _WorldInfo.data.rows[i].tags;

            if (_WorldInfo.data.rows[i].entityType == WorldType.USER_WORLD.ToString())
            {
                _event.creatorName = _WorldInfo.data.rows[i].user.name;
                _event.userAvatarURL = _WorldInfo.data.rows[i].user.avatar;
                _event.userLimit = "10";
            }
            else
            {
                if (!string.IsNullOrEmpty(_WorldInfo.data.rows[i].creator))
                    _event.creatorName = _WorldInfo.data.rows[i].creator;
                else
                    _event.creatorName = "XANA";
                _event.userLimit = _WorldInfo.data.rows[i].user_limit;
            }


            TempObject.transform.localScale = new Vector3(1, 1, 1);
            _event.Init();
            if (!_WorldInfo.data.rows[i].name.Contains("XANA Lobby"))
            {
                if (aPIURLGlobal == APIURL.Hot)
                    hotWorldList.Add(TempObject);
                else if (aPIURLGlobal == APIURL.AllWorld)
                    newWorldList.Add(TempObject);
                else if (aPIURLGlobal == APIURL.MyWorld)
                    myworldWorldList.Add(TempObject);
                else if (aPIURLGlobal == APIURL.GameWorld)
                    gameWorldList.Add(TempObject);
                else if (aPIURLGlobal == APIURL.EventWorld)
                    eventWorldList.Add(TempObject);
            }
        }
        if (!isLobbyActive) // lobby is not active so disable the lobby button from scene
        {
            eventPrefabLobby.SetActive(false);
            listParentHotSection.GetComponent<GridLayoutGroup>().padding.top = 25;
        }

        LoadingHandler.Instance.worldLoadingScreen.SetActive(false);

        TutorialsManager.instance.ShowTutorials();
    }




    private void CreateLightingAsset(FeedEventPrefab _event)
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


    public async void JoinEvent()
    {
        //Debug.LogError(" +++++++++++++++++++  WaqasJoinEvent +++++++++++++++");
        _callSingleTime = true;

        if (!UserRegisterationManager.instance.LoggedIn && PlayerPrefs.GetInt("IsLoggedIn") == 0)
        {
            if (FeedEventPrefab.m_EnvName != "DEEMO THE MOVIE Metaverse Museum")    /////// Added By Abdullah Rashid 
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
            //////
        }
        else
        {
            print("play btnn here");
            if (PlayerPrefs.HasKey("Equiped"))
            {
                Task<bool> task = UserRegisterationManager.instance._web3APIforWeb2.CheckSpecificNFTAndReturnAsync((PlayerPrefs.GetInt("nftID")).ToString());
                bool _IsInOwnerShip = await task;
                print("_IsInOwnerShip :: " + _IsInOwnerShip);
                if (!_IsInOwnerShip)
                {
                    print("Show UI NFT not available");
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
            }
            print("_NFTID :: " + PlayerPrefs.GetInt("nftID").ToString());
            if (FeedEventPrefab.m_EnvName == "DEEMO THE MOVIE Metaverse Museum")    /////// Added By Abdullah Rashid 
            {
                if (!XanaConstants.xanaConstants.IsDeemoNFT)
                {
                    Debug.Log("YOU DONT HAVE DEEMO NFT");
                    GameManager.Instance.RequiredNFTPopUP.SetActive(true);
                    return;
                }
            }
            /////


            AssetBundle.UnloadAllAssetBundles(false);
            Resources.UnloadUnusedAssets();

            // Added By WaqasAhmad [20 July 23]
            //Caching.ClearCache();
            GC.Collect();
            //

            //Screen.orientation = ScreenOrientation.LandscapeLeft;
            XanaConstants.xanaConstants.EnviornmentName = FeedEventPrefab.m_EnvName;

            LoadingHandler.Instance.ShowFadderWhileOriantationChanged(ScreenOrientation.LandscapeLeft);

//#if UNITY_EDITOR
//            orientationchanged = true;
//            LoadingHandler.Instance.ShowLoading();
//            LoadingHandler.Instance.UpdateLoadingSlider(0);
//            LoadingHandler.Instance.UpdateLoadingStatusText("Loading World");
//            Debug.Log("loading scene");
//            //this is added to fix 20% loading stuck issue internally photon reload scenes to sync 
//            Photon.Pun.PhotonHandler.levelName = "AddressableScene";
//            LoadingHandler.Instance.LoadSceneByIndex("AddressableScene");
//            //StartCoroutine(DownloadFile());
//#else
            //StartCoroutine(Check_Orientation(() =>
            //{
             LoadingHandler.Instance.ShowLoading();
            LoadingHandler.Instance.UpdateLoadingSlider(0);
            LoadingHandler.Instance.UpdateLoadingStatusText("Loading World");
            //this is added to fix 20% loading stuck issue internally photon reload scenes to sync 
            Photon.Pun.PhotonHandler.levelName = "AddressableScene";
            LoadingHandler.Instance.LoadSceneByIndex("AddressableScene");
               // StartCoroutine(DownloadFile());
            //}));
//#endif

        }
    }
    public async void JoinBuilderWorld()
    {
        if (!UserRegisterationManager.instance.LoggedIn && PlayerPrefs.GetInt("IsLoggedIn") == 0)
        {
            if (FeedEventPrefab.m_EnvName != "DEEMO THE MOVIE Metaverse Museum")    /////// Added By Abdullah Rashid 
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
        //////
        else
        {
            print("play btnn here");
            if (PlayerPrefs.HasKey("Equiped"))
            {
                Task<bool> task = UserRegisterationManager.instance._web3APIforWeb2.CheckSpecificNFTAndReturnAsync((PlayerPrefs.GetInt("nftID")).ToString());
                bool _IsInOwnerShip = await task;
                print("_IsInOwnerShip :: " + _IsInOwnerShip);

                if (!_IsInOwnerShip)
                {
                    print("Show UI NFT not available");
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
                if (FeedEventPrefab.m_EnvName == "DEEMO THE MOVIE Metaverse Museum")    /////// Added By Abdullah Rashid 
                {
                    if (!XanaConstants.xanaConstants.IsDeemoNFT)
                    {
                        Debug.Log("YOU DONT HAVE DEEMO NFT");
                        GameManager.Instance.RequiredNFTPopUP.SetActive(true);
                        return;
                    }
                }
            }
            print("_NFTID :: " + PlayerPrefs.GetInt("nftID").ToString());



            //Screen.orientation = ScreenOrientation.LandscapeLeft;
            XanaConstants.xanaConstants.EnviornmentName = FeedEventPrefab.m_EnvName;
            LoadingHandler.Instance.ShowFadderWhileOriantationChanged(ScreenOrientation.LandscapeLeft);

            // Added By WaqasAhmad [20 July 23]
            AssetBundle.UnloadAllAssetBundles(false);
            Resources.UnloadUnusedAssets();

            //Caching.ClearCache();
            GC.Collect();
            //


//#if UNITY_EDITOR
//            LoadingHandler.Instance.ShowLoading();
//            LoadingHandler.Instance.UpdateLoadingSlider(0);
//            LoadingHandler.Instance.UpdateLoadingStatusText("Loading World");
//            orientationchanged = true;
//            //this is added to fix 20% loading stuck issue internally photon reload scenes to sync 
//            Photon.Pun.PhotonHandler.levelName = "Builder";
//            LoadingHandler.Instance.LoadSceneByIndex("Builder");
//#else
//LoadingHandler.Instance.Loading_WhiteScreen.SetActive(true);
            //StartCoroutine(Check_Orientation(()=> 
            //{
                LoadingHandler.Instance.ShowLoading();
                LoadingHandler.Instance.UpdateLoadingSlider(0);
                LoadingHandler.Instance.UpdateLoadingStatusText("Loading World");
                //this is added to fix 20% loading stuck issue internally photon reload scenes to sync 
                Photon.Pun.PhotonHandler.levelName = "Builder";
                LoadingHandler.Instance.LoadSceneByIndex("Builder");
            //}));
//#endif

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




        // Added By WaqasAhmad [20 July 23]
        AssetBundle.UnloadAllAssetBundles(false);
        Resources.UnloadUnusedAssets();
        //Caching.ClearCache();
        GC.Collect();
        //
        //Debug.LogError(" +++++++++++++++++++  WaqasPlay +++++++++++++++");
        if (XanaConstants.xanaConstants.isBuilderScene)
        {
            if (!XanaConstants.xanaConstants.JjWorldSceneChange)
            {
                //Screen.orientation = ScreenOrientation.LandscapeLeft;
                LoadingHandler.Instance.ShowFadderWhileOriantationChanged(ScreenOrientation.LandscapeLeft);
            }


            XanaConstants.xanaConstants.EnviornmentName = FeedEventPrefab.m_EnvName;
//#if UNITY_EDITOR
//            LoadingHandler.Instance.ShowLoading();
//            LoadingHandler.Instance.UpdateLoadingSlider(0);
//            LoadingHandler.Instance.UpdateLoadingStatusText("Loading World");
//            orientationchanged = true;
//            //this is added to fix 20% loading stuck issue internally photon reload scenes to sync 
//            Photon.Pun.PhotonHandler.levelName = "Builder";
//            LoadingHandler.Instance.LoadSceneByIndex("Builder");
//#else
            //LoadingHandler.Instance.Loading_WhiteScreen.SetActive(true);
            //StartCoroutine(Check_Orientation(()=> 
            //{
                LoadingHandler.Instance.ShowLoading();
                LoadingHandler.Instance.UpdateLoadingSlider(0);
                LoadingHandler.Instance.UpdateLoadingStatusText("Loading World");
                //this is added to fix 20% loading stuck issue internally photon reload scenes to sync 
                Photon.Pun.PhotonHandler.levelName = "Builder";
                LoadingHandler.Instance.LoadSceneByIndex("Builder");
            //}));
//#endif
        }
        else
        {
            if (!XanaConstants.xanaConstants.JjWorldSceneChange)
            {
                //Screen.orientation = ScreenOrientation.LandscapeLeft;
                LoadingHandler.Instance.ShowFadderWhileOriantationChanged(ScreenOrientation.LandscapeLeft);
            }
            XanaConstants.xanaConstants.EnviornmentName = FeedEventPrefab.m_EnvName;
//#if UNITY_EDITOR
//            orientationchanged = true;
//            LoadingHandler.Instance.ShowLoading();
//            LoadingHandler.Instance.UpdateLoadingSlider(0);
//            LoadingHandler.Instance.UpdateLoadingStatusText("Loading World");
//            //this is added to fix 20% loading stuck issue internally photon reload scenes to sync 
//            Photon.Pun.PhotonHandler.levelName = "AddressableScene";
//            LoadingHandler.Instance.LoadSceneByIndex("AddressableScene");
//#else
//LoadingHandler.Instance.Loading_WhiteScreen.SetActive(true);
//            StartCoroutine(Check_Orientation(() =>
//            {
             LoadingHandler.Instance.ShowLoading();
            LoadingHandler.Instance.UpdateLoadingSlider(0);
            LoadingHandler.Instance.UpdateLoadingStatusText("Loading World");
            //this is added to fix 20% loading stuck issue internally photon reload scenes to sync 
            Photon.Pun.PhotonHandler.levelName = "AddressableScene";
            LoadingHandler.Instance.LoadSceneByIndex("AddressableScene");
//            }));
//#endif
        }
    }



    public void OpenAllWorldPage()
    {
        for (int i = 0; i < hotWorldList.Count; i++)
        {
            hotWorldList[i].gameObject.transform.SetParent(world_HotScroll.transform);
        }

        for (int i = 0; i < newWorldList.Count; i++)
        {
            newWorldList[i].gameObject.transform.SetParent(world_NewScroll.transform);
        }

        for (int i = 0; i < myworldWorldList.Count; i++)
        {
            myworldWorldList[i].gameObject.transform.SetParent(world_myworldScroll.transform);
        }

        for (int i = 0; i < gameWorldList.Count; i++)
        {
            gameWorldList[i].gameObject.transform.SetParent(world_GameWorldScroll.transform);
        }

        for (int i = 0; i < eventWorldList.Count; i++)
        {
            eventWorldList[i].gameObject.transform.SetParent(world_EventWorldScroll.transform);
        }


        m_AllWorldManage.WorldHotPage();
    }

    public void OpenXANAWorldPage()
    {
        for (int i = 0; i < hotWorldList.Count; i++)
        {
            hotWorldList[i].gameObject.transform.SetParent(listParentHotSection.transform);
        }

        for (int i = 0; i < newWorldList.Count; i++)
        {
            newWorldList[i].gameObject.transform.SetParent(listParentAllWorlds.transform);
        }

        for (int i = 0; i < myworldWorldList.Count; i++)
        {
            myworldWorldList[i].gameObject.transform.SetParent(listParentMyWorlds.transform);
        }

        for (int i = 0; i < gameWorldList.Count; i++)
        {
            gameWorldList[i].gameObject.transform.SetParent(listParentGameWorlds.transform);
        }

        for (int i = 0; i < eventWorldList.Count; i++)
        {
            eventWorldList[i].gameObject.transform.SetParent(listParentEventWorlds.transform);
        }
    }


    #region Clear Resource Unload Unused Asset File.......
    private int unloadUnusedFileCount;
    public void ResourcesUnloadAssetFile()
    {
        if (unloadUnusedFileCount >= 15)
        {
            unloadUnusedFileCount = 0;
            Resources.UnloadUnusedAssets();
            //  Caching.ClearCache();
            //AssetBundle.UnloadAllAssetBundles(false);
            //GC.Collect();
        }
        unloadUnusedFileCount += 1;
    }

    public void Dispose()
    {

        //   GC.SuppressFinalize(this);
    }
    #endregion

    /// <summary>
    /// Load jj world 
    /// </summary>
    public void LoadJjworld()
    {
        SetAutoSwtichStreaming();
        if (XanaConstants.xanaConstants.JjWorldSceneChange)
        {
            // Commented By WaqasAhmad
            //LoadingHandler.Instance.Loading_WhiteScreen.SetActive(false);
           
            LoadingHandler.Instance.characterLoading.SetActive(false);
            LoadingHandler.Instance.presetCharacterLoading.SetActive(false);
            
            // Commented By WaqasAhmad
            //LoadingHandler.Instance.Loading_WhiteScreen.SetActive(false);
            
            LoadingHandler.Instance.characterLoading.SetActive(false);
            LoadingHandler.Instance.worldLoadingScreen.SetActive(false);
            LoadingHandler.Instance.loadingPanel.SetActive(false);
            LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
            LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
            //LoadingHandler.Instance.UpdateLoadingSliderForJJ(UnityEngine.Random.Range(0.4f, 0.7f), 0.5f, true);
            XanaConstants.xanaConstants.EnviornmentName = XanaConstants.xanaConstants.JjWorldTeleportSceneName;
            FeedEventPrefab.m_EnvName = XanaConstants.xanaConstants.JjWorldTeleportSceneName;
            if (XanaConstants.xanaConstants.JjWorldTeleportSceneName == "Xana Festival")
            {
                XanaConstants.xanaConstants.userLimit = "16";
            }
            else
            {
                if (XanaConstants.xanaConstants.isBuilderScene)
                {
                    XanaConstants.xanaConstants.userLimit = "10";
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

}
[Serializable]
class AutoSwtichEnv{ 
    public string name;
    public bool isMussuem= false;
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
    // Updated By WaqasAhmad According to Swagger
    public string id;
    public string name;
    public string user_limit;
    public string thumbnail;
    public string banner;
    public string description;
    public string creator;
    public string createdAt;
    public string updatedAt;
    public string entityType;
    public string status;
    public string createdBy;
    public string[] tags;
    public UserInfo user;
}

[System.Serializable]
public class UserInfo
{
    public string id;
    public string name;
    public string email;
    public string avatar;
}

[System.Serializable]
public class SearchWorldData
{
    public int count;
    public List<SearchWorld> rows = new List<SearchWorld>();
}

[System.Serializable]
public class SearchworldRoot
{
    public bool success;
    public SearchWorldData data = new SearchWorldData();
    public string msg;
}

[System.Serializable]
public class SearchWorld
{
    public int id;
    public string name;
    public int? user_limit;
    public string thumbnail;
    public string banner;
    public string description;
    public int? version;
    public int orderNo;
    public DateTime createdAt;
    public DateTime updatedAt;
    public string entityType;
    public string status;
    public string? creator;
    public object users;
    public string[] tags;
    public string map_json_link;
    public string map_code;
    public object nft_token_id;
    public WorldUser user = new WorldUser();
}

[System.Serializable]
public class WorldUser
{
    public int id;
    public string name;
    public string email;
    public string avatar;
}
public enum APIURL
{
    Hot, AllWorld, MyWorld, GameWorld, EventWorld
}

public enum WorldType
{
    None, MUSEUM, ENVIRONMENT, USER_WORLD
}
