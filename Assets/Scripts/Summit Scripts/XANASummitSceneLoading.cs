using Crosstales;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class XANASummitSceneLoading : MonoBehaviour
{
    [SerializeField]
    private StayTimeTrackerForSummit _stayTimeTrackerForSummit;
    public static Vector3 playerPos;
    public static Vector3 playerRot;
    public static Vector3 playerScale;
    public static Action<bool> OnJoinSubItem; // Car, GiantWheel, Planets

    public MutiplayerController multiplayerController;

    public GameplayEntityLoader gameplayEntityLoader;

    public XANASummitDataContainer dataContainer;

    [SerializeField]
    private DomeMinimapDataHolder _domeMiniMap;

    public delegate void SetPlayerOnSubworldBack();
    public event SetPlayerOnSubworldBack setPlayerPositionDelegate;


    private void OnEnable()
    {
        setPlayerPositionDelegate = null;
        BuilderEventManager.LoadNewScene += LoadingFromDome;
        BuilderEventManager.LoadSceneByName += LoadingSceneByIDOrName;
        BuilderEventManager.LoadSummitScene += LoadDomesData;
        BuilderEventManager.AfterPlayerInstantiated += SetPlayerTransform;
        GamePlayButtonEvents.OnExitButtonXANASummit += LoadingXANASummitOnBack;
        OnJoinSubItem += SummitMiniMapStatusOnSceneChange;


        if (LoadingHandler.Instance.nftLoadingScreen.activeInHierarchy)
        {
            LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
        }
    }

    private void OnDisable()
    {
        BuilderEventManager.LoadNewScene -= LoadingFromDome;
        BuilderEventManager.LoadSceneByName -= LoadingSceneByIDOrName;
        BuilderEventManager.LoadSummitScene -= LoadDomesData;
        BuilderEventManager.AfterPlayerInstantiated -= SetPlayerTransform;
        GamePlayButtonEvents.OnExitButtonXANASummit -= LoadingXANASummitOnBack;
        OnJoinSubItem -= SummitMiniMapStatusOnSceneChange;
    }

    void LoadDomesData()
    {
        dataContainer.GetAllDomesData();
    }


    void SummitMiniMapStatusOnSceneChange(bool makeActive)
    {
        GameplayEntityLoader.instance.IsJoinSummitWorld = !makeActive;

        if (makeActive && ConstantsHolder.xanaConstants.minimap == 1)
        {
            ReferencesForGamePlay.instance.minimap.SetActive(true);
            ReferencesForGamePlay.instance.SumitMapStatus(true);
        }
        else
        {
            ReferencesForGamePlay.instance.SumitMapStatus(false);
            ReferencesForGamePlay.instance.minimap.SetActive(false);
        }
    }

    async void LoadingFromDome(int domeId, Vector3 playerPos)
    {
        XANASummitDataContainer.DomeGeneralData domeGeneralData = new XANASummitDataContainer.DomeGeneralData();
        domeGeneralData = GetDomeData(domeId);

        if (string.IsNullOrEmpty(domeGeneralData.world))
            return;

        SummitMiniMapStatusOnSceneChange(false);
        //StartCoroutine(LoadingHandler.Instance.FadeIn());
        LoadingHandler.Instance.ShowVideoLoading();
        Vector3[] currentPlayerPos = GetPlayerPosition(playerPos);

        ConstantsHolder.domeId = domeId;
        string sceneTobeUnload = WorldItemView.m_EnvName;

        XANASummitDataContainer.StackInfoWorld subWorldInfo = new XANASummitDataContainer.StackInfoWorld();
        subWorldInfo.id = ConstantsHolder.xanaConstants.MuseumID;
        subWorldInfo.name = sceneTobeUnload;
        subWorldInfo.isBuilderWorld = ConstantsHolder.xanaConstants.isBuilderScene;
        subWorldInfo.user_limit = ConstantsHolder.userLimit;
        subWorldInfo.domeId = ConstantsHolder.domeId;
        subWorldInfo.haveSubWorlds = ConstantsHolder.HaveSubWorlds;
        subWorldInfo.isFromSummitWorld = ConstantsHolder.isFromXANASummit;
        subWorldInfo.playerTrasnform = currentPlayerPos;
        XANASummitDataContainer.LoadedScenesInfo.Push(subWorldInfo);

        WorldItemView.m_EnvName = domeGeneralData.world;
        ConstantsHolder.xanaConstants.EnviornmentName = domeGeneralData.world;
        gameplayEntityLoader.addressableSceneName = domeGeneralData.world;
        ConstantsHolder.userLimit = domeGeneralData.maxPlayer;
        ConstantsHolder.isPenguin = domeGeneralData.IsPenguin;
        ConstantsHolder.isFixedHumanoid = domeGeneralData.Ishumanoid;
        if (domeGeneralData.worldType)
            ConstantsHolder.xanaConstants.MuseumID = domeGeneralData.builderWorldId.ToString();
        else
            ConstantsHolder.xanaConstants.MuseumID = domeGeneralData.worldId.ToString();
        ConstantsHolder.HaveSubWorlds = domeGeneralData.isSubWorld;
        if (domeGeneralData.Ishumanoid)
            XANASummitDataContainer.FixedAvatarJson = domeGeneralData.Avatarjson;
        gameplayEntityLoader.currentEnvironment = null;
        multiplayerController.singlePlayerInstance = domeGeneralData.experienceType != "double";
        multiplayerController.isConnecting = false;
        gameplayEntityLoader.isEnvLoaded = false;
        gameplayEntityLoader.isAlreadySpawned = true;
        ConstantsHolder.isFromXANASummit = true;

        XanaWorldDownloader.ResetAll();
        BuilderEventManager.ResetSummit?.Invoke();

        multiplayerController.Disconnect();
        multiplayerController.playerobjects.Clear();

        ReferencesForGamePlay.instance.m_34player.transform.localScale = new Vector3(0, 0, 0);

        await UnloadScene(sceneTobeUnload);

        await HomeSceneLoader.ReleaseUnsedMemory();

        if (domeGeneralData.worldType)
            LoadBuilderSceneLoading(domeGeneralData.builderWorldId);
        else
            multiplayerController.Connect("XANA Summit-" + domeGeneralData.world);

        // Summit Analytics Part
        if (_stayTimeTrackerForSummit != null)
        {
            if (_stayTimeTrackerForSummit.IsTrackingTimeForExteriorArea)
            {
                _stayTimeTrackerForSummit.StopTrackingTime();
                _stayTimeTrackerForSummit.CalculateAndLogStayTime();
                _stayTimeTrackerForSummit.IsTrackingTimeForExteriorArea = false;
            }
            _stayTimeTrackerForSummit.DomeId = domeId;
            if (domeGeneralData.worldType)
                _stayTimeTrackerForSummit.DomeWorldId = domeGeneralData.builderWorldId;
            else
                _stayTimeTrackerForSummit.DomeWorldId = domeGeneralData.worldId;
            _stayTimeTrackerForSummit.IsBuilderWorld = domeGeneralData.worldType;
            _stayTimeTrackerForSummit.StartTrackingTime();
        }
        string eventName;
        if (domeGeneralData.worldType)
            eventName = "TV_Dome_" + domeId + "_BW_" + domeGeneralData.builderWorldId;
        else
            eventName = "TV_Dome_" + domeId + "_XW_" + domeGeneralData.worldId;

        GameplayEntityLoader.instance.AssignRaffleTickets(domeId);
        GlobalConstants.SendFirebaseEventForSummit(eventName);
    }

    public async void LoadingSceneByIDOrName(string worldId, Vector3 playerPos)
    {
        if (string.IsNullOrEmpty(worldId))
            return;

        //StartCoroutine(LoadingHandler.Instance.FadeIn());
        LoadingHandler.Instance.ShowVideoLoading();
        SummitMiniMapStatusOnSceneChange(false);
        Vector3[] currentPlayerPos = GetPlayerPosition(playerPos);

        string sceneToBeUnload = WorldItemView.m_EnvName;

        SingleWorldInfo worldInfo = await GetSingleWorldData(worldId);

        XANASummitDataContainer.StackInfoWorld subWorldInfo = new XANASummitDataContainer.StackInfoWorld();
        subWorldInfo.id = ConstantsHolder.xanaConstants.MuseumID;
        subWorldInfo.name = sceneToBeUnload;
        subWorldInfo.isBuilderWorld = ConstantsHolder.xanaConstants.isBuilderScene;
        subWorldInfo.user_limit = ConstantsHolder.userLimit;
        subWorldInfo.domeId = ConstantsHolder.domeId;
        subWorldInfo.haveSubWorlds = ConstantsHolder.HaveSubWorlds;
        subWorldInfo.isFromSummitWorld = ConstantsHolder.isFromXANASummit;
        subWorldInfo.playerTrasnform = currentPlayerPos;
        XANASummitDataContainer.LoadedScenesInfo.Push(subWorldInfo);

        WorldItemView.m_EnvName = worldInfo.data.name;
        ConstantsHolder.xanaConstants.EnviornmentName = worldInfo.data.name;
        gameplayEntityLoader.addressableSceneName = worldInfo.data.name;
        ConstantsHolder.userLimit = worldInfo.data.user_limit;
        ConstantsHolder.xanaConstants.MuseumID = worldInfo.data.id;
        ConstantsHolder.HaveSubWorlds = false;
        ConstantsHolder.xanaConstants.isBuilderScene = worldInfo.data.entityType == "USER_WORLD" ? true : false;
        gameplayEntityLoader.currentEnvironment = null;
        multiplayerController.isConnecting = false;
        gameplayEntityLoader.isEnvLoaded = false;
        gameplayEntityLoader.isAlreadySpawned = true;

        ReferencesForGamePlay.instance.m_34player.transform.localScale = new Vector3(0, 0, 0);

        multiplayerController.Disconnect();

        XanaWorldDownloader.ResetAll();
        BuilderEventManager.ResetSummit?.Invoke();


        multiplayerController.playerobjects.Clear();

        await UnloadScene(sceneToBeUnload);

        await HomeSceneLoader.ReleaseUnsedMemory();

        if (ConstantsHolder.xanaConstants.isBuilderScene)
            LoadBuilderSceneLoading(int.Parse(worldInfo.data.id));
        else
            multiplayerController.Connect("XANA Summit-" + worldInfo.data.name);



    }
    async Task UnloadScene(string sceneName)
    {
        if (SceneManager.GetSceneByName("Builder").isLoaded)
        {
            await SceneManager.UnloadSceneAsync("Builder");
        }
        else
        {
            await SceneManager.UnloadSceneAsync(sceneName);
        }
    }


    async void LoadBuilderSceneLoading(int builderMapId)
    {
        ConstantsHolder.xanaConstants.builderMapID = builderMapId;
        ConstantsHolder.xanaConstants.isBuilderScene = true;
        gameplayEntityLoader.addressableSceneName = null;
        AsyncOperation handle = await SceneManager.LoadSceneAsync("Builder", LoadSceneMode.Additive);
        handle.completed += Handle_completed;
    }

    private void Handle_completed(AsyncOperation obj)
    {
        obj.allowSceneActivation = true;
    }

    async void LoadingXANASummitOnBack()
    {
        if (ConstantsHolder.isFromXANASummit == false)
            return;
        if (_stayTimeTrackerForSummit != null)
        {
            if (_stayTimeTrackerForSummit.IsTrackingTime)
            {
                _stayTimeTrackerForSummit.StopTrackingTime();
                _stayTimeTrackerForSummit.CalculateAndLogStayTime();
                _stayTimeTrackerForSummit.IsTrackingTimeForExteriorArea = true;
            }
        }
        setPlayerPositionDelegate = SetPlayerOnback;

        //StartCoroutine(LoadingHandler.Instance.FadeIn());
        LoadingHandler.Instance.ShowVideoLoading();
        XANASummitDataContainer.StackInfoWorld subWorldInfo = new XANASummitDataContainer.StackInfoWorld();
        subWorldInfo = XANASummitDataContainer.LoadedScenesInfo.Pop();

        playerPos = subWorldInfo.playerTrasnform[0];
        playerRot = subWorldInfo.playerTrasnform[1];
        playerScale = subWorldInfo.playerTrasnform[2];

        string sceneToBeUnload = WorldItemView.m_EnvName;
        //string sceneName = "XANA Summit";
        WorldItemView.m_EnvName = subWorldInfo.name;
        ConstantsHolder.xanaConstants.EnviornmentName = subWorldInfo.name;
        ConstantsHolder.userLimit = subWorldInfo.user_limit;
        ConstantsHolder.xanaConstants.isBuilderScene = subWorldInfo.isBuilderWorld;
        ConstantsHolder.xanaConstants.MuseumID = subWorldInfo.id;
        ConstantsHolder.isFromXANASummit = subWorldInfo.isFromSummitWorld;
        ConstantsHolder.HaveSubWorlds = subWorldInfo.haveSubWorlds;

        ConstantsHolder.isPenguin = false;
        ConstantsHolder.isFixedHumanoid = false;
        gameplayEntityLoader.currentEnvironment = null;
        multiplayerController.isConnecting = false;
        gameplayEntityLoader.isEnvLoaded = false;
        //ConstantsHolder.isFromXANASummit = false;
        gameplayEntityLoader.isAlreadySpawned = true;
        multiplayerController.Disconnect();

        XanaWorldDownloader.ResetAll();
        BuilderEventManager.ResetSummit?.Invoke();

        multiplayerController.playerobjects.Clear();

        await UnloadScene(sceneToBeUnload);

        await HomeSceneLoader.ReleaseUnsedMemory();

        if (subWorldInfo.isBuilderWorld)
            LoadBuilderSceneLoading(int.Parse(subWorldInfo.id));
        else
            multiplayerController.Connect(subWorldInfo.name);

        ConstantsHolder.DiasableMultiPartPhoton = false;

        // Map Working
        _domeMiniMap.SummitSceneReloaded();
        SummitMiniMapStatusOnSceneChange(true);
        //
    }
    XANASummitDataContainer.DomeGeneralData GetDomeData(int domeId)
    {
        XANASummitDataContainer.DomeGeneralData domeGeneralData = new XANASummitDataContainer.DomeGeneralData();
        for (int i = 0; i < dataContainer.summitData.domes.Count; i++)
        {
            if (dataContainer.summitData.domes[i].id == domeId)
            {
                domeGeneralData.world = dataContainer.summitData.domes[i].world;
                domeGeneralData.worldType = dataContainer.summitData.domes[i].worldType;
                domeGeneralData.experienceType = dataContainer.summitData.domes[i].experienceType;
                domeGeneralData.builderWorldId = dataContainer.summitData.domes[i].builderWorldId;
                domeGeneralData.worldId = dataContainer.summitData.domes[i].worldId;
                domeGeneralData.maxPlayer = dataContainer.summitData.domes[i].maxPlayer;
                domeGeneralData.IsPenguin = dataContainer.summitData.domes[i].IsPenguin;
                domeGeneralData.Ishumanoid = dataContainer.summitData.domes[i].Ishumanoid;
                domeGeneralData.Avatarjson = dataContainer.summitData.domes[i].Avatarjson;
                domeGeneralData.AvatarIndex = dataContainer.summitData.domes[i].AvatarIndex;
                domeGeneralData.name = dataContainer.summitData.domes[i].name;
                domeGeneralData.isSubWorld = dataContainer.summitData.domes[i].isSubWorld;
                domeGeneralData.world360Image = dataContainer.summitData.domes[i].world360Image;
                domeGeneralData.companyLogo = dataContainer.summitData.domes[i].companyLogo;
                //if (dataContainer.summitData1.domes[i].worldType)
                //    return new Tuple<string[],string>(new[] { dataContainer.summitData1.domes[i].world, "1", dataContainer.summitData1.domes[i].builderWorldId }, dataContainer.summitData1.domes[i].experienceType);
                //else
                //    return new[] { dataContainer.summitData1.domes[i].world, "0", dataContainer.summitData1.domes[i].builderWorldId };
            }
        }
        return domeGeneralData;
        //return new[] { string.Empty, "0", "0" };
    }

    Vector3[] GetPlayerPosition(Vector3 _playerPos)
    {
        playerPos = _playerPos;
        playerRot = GameplayEntityLoader.instance.mainController.transform.rotation.eulerAngles;
        playerScale = GameplayEntityLoader.instance.mainController.transform.localScale;

        return new[] { playerPos, playerRot, playerScale };
    }

    void SetPlayerTransform()
    {
        //if (ConstantsHolder.isFromXANASummit == false)
        //    return;

        setPlayerPositionDelegate?.Invoke();


        //StartCoroutine(LoadingHandler.Instance.FadeOut());
        LoadingHandler.Instance.DisableVideoLoading();
    }

    void SetPlayerOnback()
    {
        GameplayEntityLoader.instance.mainController.transform.position = playerPos;
        GameplayEntityLoader.instance.mainController.transform.rotation = playerRot.CTQuaternion();
        GameplayEntityLoader.instance.mainController.transform.localScale = playerScale;
        if (WorldItemView.m_EnvName == "XANA Summit")
        {
            ConstantsHolder.isFromXANASummit = false;
        }
        setPlayerPositionDelegate = null;
    }



    async Task<SingleWorldInfo> GetSingleWorldData(string WorldID)
    {
        string url;
        url = ConstantsGod.API_BASEURL + ConstantsGod.SINGLEWORLDINFO + WorldID;

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            await www.SendWebRequest();
            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                www.Dispose();
                return null;
            }
            else
            {
                SingleWorldInfo worldInfo = new SingleWorldInfo();
                worldInfo = JsonUtility.FromJson<SingleWorldInfo>(www.downloadHandler.text);
                www.Dispose();
                return worldInfo;
            }

        }
    }

    [System.Serializable]
    public class SingleWorldInfo
    {
        public bool success;
        public DataClassSingle data;
    }
    [System.Serializable]
    public class DataClassSingle
    {
        public string id;
        public string name;
        public int user_limit;
        public string thumbnail;
        public string banner;
        public string description;
        public string creator;
        public string entityType;
    }


}
