using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using UnityEditor;
using System.Threading.Tasks;
using Photon.Pun.Demo.PunBasics;
using TMPro;
using AdvancedInputFieldPlugin;

public class WorldManager : MonoBehaviour
{
    [Header("World View prefab")]
    public GameObject EventPrefabLobby;
    [HideInInspector]
    public bool orientationchanged = false;
    [Header("Api Parameter's")]
    private string finalAPIURL;
    private string status = "Publish";
    private int pageNumberHot = 1;
    private int pageNumberAllWorld = 1;
    private int pageNumberMyWorld = 1;
    private int pageNumberGameWorld = 1;
    private int pageNumberEventWorld = 1;
    private int pageNumberSearchWorld = 1;
    private int pageNumberTestWorld = 1;
    private int pageCount = 30;
    private bool loadOnce = true;
    public bool dataIsFatched = false;
    public WorldsInfo _WorldInfo;
    private APIURL aPIURLGlobal;
    public AllWorldManage AllWorldTabReference;
    public static WorldManager instance;
    [SerializeField]
    [NonReorderable]
    List<AutoSwtichEnv> AutoSwtichWorldList;

    [Header("Fighting Module PopUp")]
    public GameObject fightingModulePopUp;
    public bool isCheckFightingModulePopUp;
    public bool HaveFighterNFT;

    static int AutoSwtichIndex = 0;
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
        //if (XanaConstants.xanaConstants.screenType == XanaConstants.ScreenType.TabScreen)
        BuilderEventManager.OnBuilderWorldLoad += GetBuilderWorlds;
        ChangeWorldTab(APIURL.Hot);
        Invoke(nameof(LoadJjworld), 0);
    }
    public void CheckWorldTabAndReset(APIURL tab)
    {
        if (WorldItemManager.GetWorldCountPresentInMemory(tab.ToString()) > 0)
        {
            WorldItemManager.DisplayWorlds(tab.ToString());
            LoadingHandler.Instance.worldLoadingScreen.SetActive(false);
        }
        else
        {
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

        WorldItemManager.DisplayWorlds("Temp");
        StartCoroutine(WorldCall(tab));
    }
    IEnumerator WorldCall(APIURL tab)
    {
        yield return new WaitForSeconds(1f);
        while (!dataIsFatched)
        {
            //Debug.LogError("Clear Fetch");
            yield return null;
            NotProcessRequest = true;
        }
        CheckWorldTabAndReset(tab);
    }
    public void ChangeWorldTab(APIURL tab)
    {
        aPIURLGlobal = tab;
        GetBuilderWorlds(tab, (a) => { }, false);
    }
    public void SetaPIURLGlobal(APIURL chnager)
    {
        aPIURLGlobal = chnager;
    }
    public string previousSearchKey;
    public void SearchWorldCall(string searchKey, bool isFromTag = false)
    {
        if (searchKey != previousSearchKey && !string.IsNullOrEmpty(searchKey))
        {
            if (isFromTag)
                aPIURLGlobal = APIURL.SearchWorldByTag;
            else
                aPIURLGlobal = APIURL.SearchWorld;
            this.WorldItemManager.ClearListInDictionary(aPIURLGlobal.ToString());
            ClearWorldScrollWorlds();
            SearchPageNumb = 1;
            SearchTagPageNumb = 1;
            SearchPageSize = 40;
            SearchTagPageSize = 40;
            SearchKey = searchKey;
            GetBuilderWorlds(aPIURLGlobal, (a) => { } , true);
        }
        else
        {
            this.WorldItemManager.ClearListInDictionary(aPIURLGlobal.ToString());
            ClearWorldScrollWorlds();
            previousSearchKey = SearchKey = searchKey;
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

    private void OnDestroy()
    {
        BuilderEventManager.OnBuilderWorldLoad -= GetBuilderWorlds;
    }
    public void WorldPageLoading()
    {
        if (dataIsFatched)
        {
            loadOnce = true;
            dataIsFatched = false;
            LoadingHandler.Instance.worldLoadingScreen.SetActive(true);
            GetBuilderWorlds(aPIURLGlobal, (a) => { }, false);
        }
    }
    public int SearchPageNumb = 1;
    public int SearchPageSize = 15;
    public int SearchTagPageNumb = 1;
    public int SearchTagPageSize = 15;
    public string SearchKey = default;
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
            case APIURL.TestWorld:
                return ConstantsGod.API_BASEURL + ConstantsGod.WORLDSBYCATEGORY + pageNumberTestWorld + "/" + pageCount + "/" + status + "/TEST";
            case APIURL.SearchWorld:
                return ConstantsGod.API_BASEURL + ConstantsGod.SearchWorldAPI + SearchKey + "/" + SearchPageNumb + "/" + SearchPageSize;
            case APIURL.SearchWorldByTag:
                return ConstantsGod.API_BASEURL + ConstantsGod.SEARCHWORLDBYTAG + SearchKey + "/" + SearchTagPageNumb + "/" + SearchTagPageSize;
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
            case APIURL.SearchWorld:
                SearchPageNumb += 1;
                return;
            case APIURL.SearchWorldByTag:
                SearchTagPageNumb += 1;
                return;
            case APIURL.TestWorld:
                pageNumberTestWorld += 1;
                return;
            default:
                pageNumberHot += 1;
                return;
        }
    }
    bool NotProcessRequest = false;
    int CallBackCheck = 0;
    public void GetBuilderWorlds(APIURL aPIURL, Action<bool> CallBack, bool _searchActive)
    {
        finalAPIURL = PrepareApiURL(aPIURL);
        loadOnce = false;
        if (_searchActive)
        {
            LoadingHandler.Instance.SearchLoadingCanvas.SetActive(true);
        }
        //if (UIManager.Instance.IsSplashActive)
        //{
        //    LoadingHandler.Instance.worldLoadingScreen.SetActive(false);
        //}
        //else
        //{
        //    LoadingHandler.Instance.worldLoadingScreen.SetActive(true);
        //}
        StartCoroutine(FetchUserMapFromServer(finalAPIURL, (isSucess) =>
        {
            if (isSucess)
            {
                //if (NotProcessRequest)
                //{
                //    Debug.LogError("Reset Clear Fetch");
                //    dataIsFatched = true;
                //    NotProcessRequest = false;
                //    LoadingHandler.Instance.worldLoadingScreen.SetActive(false);
                //    return;
                //}
                CallBackCheck = 0;
                InstantiateWorlds(aPIURL.ToString(), isSucess);
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
                GetBuilderWorlds(aPIURLGlobal, (a) => { }, false);
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
                //Debug.LogError(www.downloadHandler.text);
                callback(false);
            }
            else
            {
                //Debug.LogError(www.downloadHandler.text);
                _WorldInfo = JsonUtility.FromJson<WorldsInfo>(www.downloadHandler.text);
                worldstr = www.downloadHandler.text;
                callback(true);
            }
            www.Dispose();
        }
    }
    public string worldstr;
    bool isLobbyActive = false;
    public WorldItemManager WorldItemManager;
    void InstantiateWorlds(string _apiURL, bool APIResponse)
    {
        for (int i = 0; i < _WorldInfo.data.rows.Count; i++)
        {
            WorldItemDetail _event;
            if (_WorldInfo.data.rows[i].name.Contains("XANA Lobby"))
            {
                isLobbyActive = true;
            }
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


                        if (!_event.EnvironmentName.Contains("XANA Lobby"))
                        {
                            _event.ThumbnailDownloadURL = IThumbnailDownloadURL + "?width=" + 256 + "&height=" + 256;
                            _event.ThumbnailDownloadURLHigh = IThumbnailDownloadURL + "?width=" + 640 + "&height=" + 360;
                        }
                        else
                        {
                            _event.ThumbnailDownloadURL = IThumbnailDownloadURL;
                        }
                    }
                    else
                    {
                        IThumbnailDownloadURL = _WorldInfo.data.rows[i].thumbnail.Replace("https://cdn.xana.net/xanaprod", "https://aydvewoyxq.cloudimg.io/_xanaprod_/xanaprod");
                        // Test-net
                        IThumbnailDownloadURL = IThumbnailDownloadURL.Replace("https://cdn.xana.net/apitestxana/Defaults", "https://aydvewoyxq.cloudimg.io/_apitestxana_/apitestxana/Defaults");
                        // Main-net
                        IThumbnailDownloadURL = IThumbnailDownloadURL.Replace("https://ik.imagekit.io/xanalia/xanaprod/Defaults", "https://aydvewoyxq.cloudimg.io/_xanaprod_/xanaprod/Defaults");



                        if (!_event.EnvironmentName.Contains("XANA Lobby"))
                        {
                            _event.ThumbnailDownloadURL = IThumbnailDownloadURL + "?width=" + 256 + "&height=" + 256;
                            _event.ThumbnailDownloadURLHigh = IThumbnailDownloadURL + "?width=" + 640 + "&height=" + 360;
                        }
                        else
                        {
                            _event.ThumbnailDownloadURL = IThumbnailDownloadURL;
                        }
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
            if (_WorldInfo.data.rows[i].tags != null)
                _event.WorldTags = _WorldInfo.data.rows[i].tags;

            if (_WorldInfo.data.rows[i].creatorDetails != null)
            {
                _event.Creator_Name = _WorldInfo.data.rows[i].creatorDetails.userName;
                _event.CreatorDescription = _WorldInfo.data.rows[i].creatorDetails.description;
                _event.CreatorAvatarURL = _WorldInfo.data.rows[i].creatorDetails.avatar;
            }

            if (_WorldInfo.data.rows[i].entityType == WorldType.USER_WORLD.ToString())
            {
                _event.CreatorName = _WorldInfo.data.rows[i].user.name;
                _event.UserAvatarURL = _WorldInfo.data.rows[i].user.avatar;
                _event.UserLimit = "15";
            }
            else
            {
                if (!string.IsNullOrEmpty(_WorldInfo.data.rows[i].creator))
                    _event.CreatorName = _WorldInfo.data.rows[i].creator;
                else
                    _event.CreatorName = "XANA";
                _event.UserLimit = _WorldInfo.data.rows[i].user_limit;
            }
            if (_WorldInfo.data.rows[i].name.Contains("XANA Lobby"))
            {
                isLobbyActive = true;
                // if(EventPrefabLobby.activeInHierarchy)
                EventPrefabLobby.GetComponent<WorldItemView>().InitItem(-1, Vector2.zero, _event);
            }
            else
            {
                WorldItemManager.AddWorld(_apiURL, _event);
            }
        }
        if (!isLobbyActive)
        {
            if (EventPrefabLobby.gameObject.activeInHierarchy)
            {
                EventPrefabLobby.GetComponent<LobbyWorldViewFlagHandler>().ActivityFlag(false);
                EventPrefabLobby.SetActive(false);
                AllWorldTabReference.LobbyInactiveCallBack();
            }
        }
        if (WorldItemManager.gameObject.activeInHierarchy)
            WorldItemManager.DisplayWorlds(_apiURL);
        previousSearchKey = SearchKey;
        LoadingHandler.Instance.worldLoadingScreen.SetActive(false);
        //if (!UIManager.Instance.IsSplashActive)
        //{
        //    Invoke(nameof(ShowTutorial), 1f);
        //}

    }

    public void ShowTutorial()
    {
        TutorialsManager.instance.ShowTutorials();
    }

    public void WorldPageStateHandler(bool _checkCheck)
    {
        WorldItemManager.WorldPageStateHandler(_checkCheck);
    }
    public void WorldScrollReset()
    {
        WorldItemManager.WorldScrollReset();
    }
    public void ClearWorldScrollWorlds()
    {
        WorldItemManager.ClearWorldScrollWorlds();
    }
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
                    print("NFT is in your OwnerShip Enjoy " + PlayerPrefs.GetInt("Equiped"));
                    List<List> fighterNFTlist = UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTlistdata.list.FindAll(o => o.collection.name.StartsWith("XANA x BreakingDown"));
                    Debug.LogError("fighterNFTlist count: " + fighterNFTlist.Count);
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
            print("_NFTID :: " + PlayerPrefs.GetInt("nftID").ToString());
            if (WorldItemView.m_EnvName == "DEEMO THE MOVIE Metaverse Museum")    /////// Added By Abdullah Rashid 
            {
                if (!XanaConstants.xanaConstants.IsDeemoNFT)
                {
                    Debug.Log("YOU DONT HAVE DEEMO NFT");
                    GameManager.Instance.RequiredNFTPopUP.SetActive(true);
                    return;
                }
            }
            // Added By WaqasAhmad [PMY ClassRoom Dummy Work]
            else if (XanaConstants.xanaConstants.EnviornmentName == "PMY ACADEMY" && !XanaConstants.xanaConstants.pmy_isTesting)
            {
                if (XanaConstants.xanaConstants.buttonClicked != null && !XanaConstants.xanaConstants.buttonClicked.GetComponent<WorldItemView>().worldItemPreview.enterClassCodePanel.activeInHierarchy)
                {
                    XanaConstants.xanaConstants.buttonClicked.GetComponent<WorldItemView>().worldItemPreview.classCodeInputField.Text= "";
                    XanaConstants.xanaConstants.buttonClicked.GetComponent<WorldItemView>().worldItemPreview.enterClassCodePanel.SetActive(true);
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
        else if (WorldItemView.m_EnvName == "PMY ACADEMY")
            GlobalConstants.SendFirebaseEvent(GlobalConstants.FirebaseTrigger.Home_Thumbnail_PlayBtn_PMY.ToString());
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
                if (WorldItemView.m_EnvName == "DEEMO THE MOVIE Metaverse Museum")    /////// Added By Abdullah Rashid 
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
            LoadingHandler.Instance.loadingPanel.SetActive(true);
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
            //Debug.LogError("Off Loading");
            LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
            LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
            XanaConstants.xanaConstants.EnviornmentName = XanaConstants.xanaConstants.JjWorldTeleportSceneName;
            WorldItemView.m_EnvName = XanaConstants.xanaConstants.JjWorldTeleportSceneName;
            if (XanaConstants.xanaConstants.JjWorldTeleportSceneName == "Xana Festival")
            {
                XanaConstants.xanaConstants.userLimit = "16";
            }
            //else if(XanaConstants.xanaConstants.JjWorldTeleportSceneName == "PMYRoomA")
            //    XanaConstants.xanaConstants.userLimit = "1";
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
}


[Serializable]
public class WorldCreatorDetail
{
    public string userName;
    public string avatar;
    public string description;
}

public enum APIURL
{
    Hot, AllWorld, MyWorld, GameWorld, EventWorld, SearchWorld, TestWorld, SearchWorldByTag
}
public enum WorldType
{
    None, MUSEUM, ENVIRONMENT, USER_WORLD
}