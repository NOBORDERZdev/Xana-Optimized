using Crosstales;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using SuperStar.Helpers;
using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
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

    public SubWorldsHandler SubWorldsHandlerInstance;

    public XANASummitDataContainer dataContainer;

    [SerializeField]
    private DomeMinimapDataHolder _domeMiniMap;

    public SummitDomePAAIController DomePerformerAvatarHandler;

    public delegate void SetPlayerOnSubworldBack();
    public static event SetPlayerOnSubworldBack setPlayerPositionDelegate;

    int prevstate;

    private void OnEnable()
    {
        setPlayerPositionDelegate = null;
        BuilderEventManager.LoadNewScene += LoadingFromDome;
        BuilderEventManager.LoadSceneByName += LoadingSceneByIDOrName;
        BuilderEventManager.LoadSummitScene += LoadDomesData;
        BuilderEventManager.AfterPlayerInstantiated += SetPlayerTransform;
        GamePlayButtonEvents.OnExitButtonXANASummit += LoadingXANASummitOnBack;
        OnJoinSubItem += SummitMiniMapStatusOnSceneChange;


        if (LoadingHandler.Instance.nftLoadingScreen.activeInHierarchy || LoadingHandler.Instance.LoadingScreenSummit.activeInHierarchy)
        {
            LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
            LoadingHandler.Instance.LoadingScreenSummit.SetActive(false);
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
        if (!makeActive)
        {
            prevstate = ConstantsHolder.xanaConstants.minimap;
            ConstantsHolder.xanaConstants.minimap = 0;
        }
        else { ConstantsHolder.xanaConstants.minimap = prevstate; }

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
        int worldId = domeGeneralData.worldType == true ? domeGeneralData.builderWorldId : domeGeneralData.worldId;
        ConstantsHolder.visitorCount = await dataContainer.GetVisitorCount(worldId.ToString());
        if (string.IsNullOrEmpty(domeGeneralData.world))
            return;

        if (domeGeneralData.is_penpenz && ConstantsHolder.xanaConstants.LoggedInAsGuest)
        {
            GamePlayUIHandler.inst.SignInPopupForGuestUser.SetActive(true);
            return;
        }

        if (domeGeneralData.isSubWorld)
        {
            ConstantsHolder.domeId = domeId;
            ConstantsHolder.isFromXANASummit = true;
            ReferencesForGamePlay.instance.ChangeExitBtnImage(false);
            LoadingHandler.Instance.DisableDomeLoading();
            bool Success = await SubWorldsHandlerInstance.CreateSubWorldList(domeGeneralData, playerPos);
            if (Success)
                return;
        }
        else
        {
            #region WaitingForPLayerApproval
            LoadingHandler.Instance.showApprovaldomeloading(domeGeneralData);

            while (LoadingHandler.Instance.WaitForInput)
            {
                await Task.Delay(1000);
            }
            if (!LoadingHandler.Instance.enter) { ConstantsHolder.DiasableMultiPartPhoton = false; return; }

            LoadingHandler.Instance.enter = false;

            #endregion
            if (ConstantsHolder.MultiSectionPhoton)
            {
                ConstantsHolder.DiasableMultiPartPhoton = true;
            }
        }

        SummitMiniMapStatusOnSceneChange(false);
        //StartCoroutine(LoadingHandler.Instance.FadeIn());
        //  LoadingHandler.Instance.ShowVideoLoading();


        Vector3[] currentPlayerPos = GetPlayerPosition(playerPos);


        string sceneTobeUnload = WorldItemView.m_EnvName;

        XANASummitDataContainer.StackInfoWorld subWorldInfo = new XANASummitDataContainer.StackInfoWorld();
        subWorldInfo.id = ConstantsHolder.xanaConstants.MuseumID;
        subWorldInfo.name = sceneTobeUnload;
        subWorldInfo.isBuilderWorld = ConstantsHolder.xanaConstants.isBuilderScene;
        subWorldInfo.user_limit = ConstantsHolder.userLimit;
        subWorldInfo.domeId = ConstantsHolder.domeId;
        subWorldInfo.thumbnail = ConstantsHolder.Thumbnail;
        subWorldInfo.description = ConstantsHolder.description;
        subWorldInfo.haveSubWorlds = ConstantsHolder.HaveSubWorlds;
        subWorldInfo.isFromSummitWorld = ConstantsHolder.isFromXANASummit;
        subWorldInfo.playerTrasnform = currentPlayerPos;
        XANASummitDataContainer.LoadedScenesInfo.Push(subWorldInfo);



        ConstantsHolder.domeId = domeId;

        WorldItemView.m_EnvName = domeGeneralData.world;
        ConstantsHolder.xanaConstants.EnviornmentName = domeGeneralData.world;
        gameplayEntityLoader.addressableSceneName = domeGeneralData.world;
        ConstantsHolder.userLimit = domeGeneralData.maxPlayer;
        ConstantsHolder.isPenguin = domeGeneralData.IsPenguin;
        ConstantsHolder.xanaConstants.isXanaPartyWorld = domeGeneralData.is_penpenz;
        ConstantsHolder.isFixedHumanoid = domeGeneralData.Ishumanoid;
        ConstantsHolder.Thumbnail = domeGeneralData.world360Image;
        ConstantsHolder.description = domeGeneralData.description;
        ConstantsHolder.AvatarIndex = domeGeneralData.AvatarIndex;
        if (domeGeneralData.worldType)
            ConstantsHolder.xanaConstants.MuseumID = domeGeneralData.builderWorldId.ToString();
        else
            ConstantsHolder.xanaConstants.MuseumID = domeGeneralData.worldId.ToString();
        ConstantsHolder.HaveSubWorlds = domeGeneralData.isSubWorld;
        if (domeGeneralData.Ishumanoid)
        {
            XANASummitDataContainer.FixedAvatarJson = domeGeneralData.Avatarjson;
            ConstantsHolder.xanaConstants.SetPlayerProperties(XANASummitDataContainer.FixedAvatarJson);
        }
        else
        {
            ConstantsHolder.xanaConstants.SetPlayerProperties();
        }
        gameplayEntityLoader.currentEnvironment = null;
        multiplayerController.singlePlayerInstance = domeGeneralData.experienceType != "double";
        multiplayerController.isConnecting = false;
        gameplayEntityLoader.isEnvLoaded = false;
        gameplayEntityLoader.isAlreadySpawned = true;
        ReferencesForGamePlay.instance.ChangeExitBtnImage(false);
        ConstantsHolder.isFromXANASummit = true;
        if (!domeGeneralData.isSubWorld) LoadingHandler.Instance.startLoading();
        XanaWorldDownloader.ResetAll();
        BuilderEventManager.ResetSummit?.Invoke();
        while(MutiplayerController.instance.isShifting||!PhotonNetwork.InRoom)
        {
            await Task.Delay(1000);
        }
        multiplayerController.Disconnect();
        multiplayerController.playerobjects.Clear();

        ReferencesForGamePlay.instance.m_34player.transform.localScale = new Vector3(0, 0, 0);

        await UnloadScene(sceneTobeUnload);

        await HomeSceneLoader.ReleaseUnsedMemory();

        if (domeGeneralData.worldType)
            LoadBuilderSceneLoading(domeGeneralData.builderWorldId);
        else
        {
            ConstantsHolder.xanaConstants.LastLobbyName = "XANA Summit-" + ConstantsHolder.domeId + "-" + domeGeneralData.world;
            multiplayerController.Connect("XANA Summit-" + ConstantsHolder.domeId + "-" + domeGeneralData.world);
        }

        DomePerformerAvatarHandler.InitPerformerAvatarNPC();

        // Summit Analytics Part
        if (_stayTimeTrackerForSummit != null)
        {
            if (_stayTimeTrackerForSummit.IsTrackingTimeForExteriorArea)
            {
                _stayTimeTrackerForSummit.StopTrackingTime();
                //_stayTimeTrackerForSummit.CalculateAndLogStayTime();
                _stayTimeTrackerForSummit.IsTrackingTimeForExteriorArea = false;
            }
            _stayTimeTrackerForSummit.DomeId = domeId;
            _stayTimeTrackerForSummit.IsBuilderWorld = domeGeneralData.worldType;
            string eventName;
            if (domeGeneralData.worldType)
            {
                _stayTimeTrackerForSummit.DomeWorldId = domeGeneralData.builderWorldId;
                eventName = "TV_Dome_" + domeId + "_BW_" + domeGeneralData.builderWorldId;
            }
            else
            {
                _stayTimeTrackerForSummit.DomeWorldId = domeGeneralData.worldId;
                eventName = "TV_Dome_" + domeId + "_XW_" + domeGeneralData.worldId;
            }
            GlobalConstants.SendFirebaseEventForSummit(eventName);
            _stayTimeTrackerForSummit.StartTrackingTime();
        }

        if (ReferencesForGamePlay.instance.playerControllerNew.isFirstPerson)
        {
            GamePlayUIHandler.inst.OnSwitchCameraClick();
        }
        //GameplayEntityLoader.instance.ForcedMapOpenForSummitScene();

        if (ActionManager.IsAnimRunning)
        {
            ActionManager.StopActionAnimation?.Invoke();

            //  EmoteAnimationHandler.Instance.StopAnimation();
            //  EmoteAnimationHandler.Instance.StopAllCoroutines();
        }
        GameplayEntityLoader.instance.AssignRaffleTickets(domeId);
    }



    public async void LoadingSceneByIDOrName(string worldId, Vector3 playerPos)
    {
        if (string.IsNullOrEmpty(worldId))
        {
            Debug.LogError("null World");
            return;
        }

        //StartCoroutine(LoadingHandler.Instance.FadeIn());
        //LoadingHandler.Instance.ShowVideoLoading();
        //SummitMiniMapStatusOnSceneChange(false);
        Vector3[] currentPlayerPos = GetPlayerPosition(playerPos);

        string sceneToBeUnload = WorldItemView.m_EnvName;

        ConstantsHolder.visitorCount = await dataContainer.GetVisitorCount(worldId);
        SingleWorldInfo worldInfo = await GetSingleWorldData(worldId);

        #region WaitingForPLayerApproval
        LoadingHandler.Instance.showApprovaldomeloading(worldInfo, SubWorldsHandlerInstance.selectedWold);
        SubWorldsHandlerInstance.OnEnteredIntoWorld();
        while (LoadingHandler.Instance.WaitForInput)
        {
            await Task.Delay(1000);
        }
        if (!LoadingHandler.Instance.enter) { ConstantsHolder.DiasableMultiPartPhoton = false; return; }

        LoadingHandler.Instance.enter = false;

        #endregion

        SummitMiniMapStatusOnSceneChange(false);

        if (ConstantsHolder.MultiSectionPhoton)
        {
            ConstantsHolder.DiasableMultiPartPhoton = true;
        }
        XANASummitDataContainer.StackInfoWorld subWorldInfo = new XANASummitDataContainer.StackInfoWorld();
        subWorldInfo.id = ConstantsHolder.xanaConstants.MuseumID;
        subWorldInfo.name = sceneToBeUnload;
        subWorldInfo.isBuilderWorld = ConstantsHolder.xanaConstants.isBuilderScene;
        subWorldInfo.user_limit = ConstantsHolder.userLimit;
        subWorldInfo.domeId = ConstantsHolder.domeId;
        subWorldInfo.haveSubWorlds = ConstantsHolder.HaveSubWorlds;
        subWorldInfo.isFromSummitWorld = ConstantsHolder.isFromXANASummit;
        subWorldInfo.thumbnail = ConstantsHolder.Thumbnail;
        subWorldInfo.description = ConstantsHolder.description;
        subWorldInfo.playerTrasnform = currentPlayerPos;
        XANASummitDataContainer.LoadedScenesInfo.Push(subWorldInfo);

        WorldItemView.m_EnvName = worldInfo.data.name;
        ConstantsHolder.xanaConstants.EnviornmentName = worldInfo.data.name;
        gameplayEntityLoader.addressableSceneName = worldInfo.data.name;
        ConstantsHolder.userLimit = worldInfo.data.user_limit;
        ConstantsHolder.xanaConstants.MuseumID = worldInfo.data.id;
        //ConstantsHolder.HaveSubWorlds = false;
        ConstantsHolder.Thumbnail = worldInfo.data.thumbnail;
        ConstantsHolder.description = worldInfo.data.description;
        ConstantsHolder.xanaConstants.isBuilderScene = worldInfo.data.entityType == "USER_WORLD" ? true : false;
        gameplayEntityLoader.currentEnvironment = null;
        multiplayerController.isConnecting = false;
        gameplayEntityLoader.isEnvLoaded = false;
        gameplayEntityLoader.isAlreadySpawned = true;
        LoadingHandler.Instance.startLoading();
        ReferencesForGamePlay.instance.m_34player.transform.localScale = new Vector3(0, 0, 0);
        while (MutiplayerController.instance.isShifting || !PhotonNetwork.InRoom)
        {
            await Task.Delay(1000);
        }
        multiplayerController.Disconnect();

        XanaWorldDownloader.ResetAll();
        BuilderEventManager.ResetSummit?.Invoke();


        multiplayerController.playerobjects.Clear();

        await UnloadScene(sceneToBeUnload);

        await HomeSceneLoader.ReleaseUnsedMemory();

        if (ConstantsHolder.xanaConstants.isBuilderScene)
            LoadBuilderSceneLoading(int.Parse(worldInfo.data.id));
        else
        {
            ConstantsHolder.xanaConstants.LastLobbyName = "XANA Summit-" + ConstantsHolder.domeId + "-" + worldInfo.data.name;
            multiplayerController.Connect("XANA Summit-" + ConstantsHolder.domeId + "-" + worldInfo.data.name);
        }

        if (ReferencesForGamePlay.instance.playerControllerNew.isFirstPerson)
        {
            GamePlayUIHandler.inst.OnSwitchCameraClick();
        }
        if (SubWorldsHandlerInstance.IsEnteringInSubWorld)
        {
            SubWorldsHandlerInstance.IsEnteringInSubWorld = false;
            SubWorldsHandlerInstance.CallAnalyticsForSubWorlds();
        }

        //GameplayEntityLoader.instance.ForcedMapOpenForSummitScene();
        if (ActionManager.IsAnimRunning)
        {
            ActionManager.StopActionAnimation?.Invoke();

            //  EmoteAnimationHandler.Instance.StopAnimation();
            //  EmoteAnimationHandler.Instance.StopAllCoroutines();
        }

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
        Debug.Log("Loading builder Scene...");
        ConstantsHolder.xanaConstants.builderMapID = builderMapId;
        ConstantsHolder.xanaConstants.isBuilderScene = true;
        gameplayEntityLoader.addressableSceneName = null;

        AsyncOperation handle = await LoadingHandler.Instance.LoadSceneByIndex("Builder", true, LoadSceneMode.Additive);
        // handle = SceneManager.LoadSceneAsync("Builder", LoadSceneMode.Additive);


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
                //_stayTimeTrackerForSummit.CalculateAndLogStayTime();
                _stayTimeTrackerForSummit.IsTrackingTimeForExteriorArea = true;
            }
        }
        setPlayerPositionDelegate += SetPlayerOnback;

        //StartCoroutine(LoadingHandler.Instance.FadeIn());
        // LoadingHandler.Instance.ShowVideoLoading();
        XANASummitDataContainer.StackInfoWorld subWorldInfo = new XANASummitDataContainer.StackInfoWorld();
        subWorldInfo = XANASummitDataContainer.LoadedScenesInfo.Pop();
        ConstantsHolder.visitorCount = await dataContainer.GetVisitorCount(subWorldInfo.id);
        LoadingHandler.Instance.showDomeLoading(subWorldInfo);

        if (GamePlayUIHandler.inst.LeaderboardPanel.activeInHierarchy)
        {
            GamePlayUIHandler.inst.LeaderboardPanel.SetActive(false);
        }

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
        ConstantsHolder.description = subWorldInfo.description;
        ConstantsHolder.Thumbnail = subWorldInfo.thumbnail;
        ConstantsHolder.isPenguin = false;
        ConstantsHolder.isFixedHumanoid = false;
        ConstantsHolder.xanaConstants.SetPlayerProperties();
        gameplayEntityLoader.currentEnvironment = null;
        multiplayerController.isConnecting = false;
        gameplayEntityLoader.isEnvLoaded = false;
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

        ReferencesForGamePlay.instance.SetGameplayForPenpenz(true);

        ReferenceForPenguinAvatar referenceForPenguin = GameplayEntityLoader.instance.referenceForPenguin;
        referenceForPenguin.ActiveXanaUIData(true);

        // Map Working
        _domeMiniMap.SummitSceneReloaded();
        //SummitMiniMapStatusOnSceneChange(true);

        ConstantsHolder.xanaConstants.comingFrom = ConstantsHolder.ComingFrom.None;
        if (ReferencesForGamePlay.instance.playerControllerNew.isFirstPerson)
        {
            GamePlayUIHandler.inst.OnSwitchCameraClick();
        }
        //GameplayEntityLoader.instance.ForcedMapOpenForSummitScene();
        if (ActionManager.IsAnimRunning)
        {
            ActionManager.StopActionAnimation?.Invoke();

            //  EmoteAnimationHandler.Instance.StopAnimation();
            //  EmoteAnimationHandler.Instance.StopAllCoroutines();
        }
        //
    }
    XANASummitDataContainer.DomeGeneralData GetDomeData(int domeId)
    {
        XANASummitDataContainer.DomeGeneralData domeGeneralData = new XANASummitDataContainer.DomeGeneralData();
        for (int i = 0; i < dataContainer.summitData.domes.Count; i++)
        {
            if (dataContainer.summitData.domes[i].id == domeId)
            {
                domeGeneralData.id = dataContainer.summitData.domes[i].id;

                if (dataContainer.summitData.domes[i].world.Contains("D + Infinity Labo") || dataContainer.summitData.domes[i].world.Contains("D +  Infinity Labo"))
                {
                    dataContainer.summitData.domes[i].world = "D_Infinity_Labo";
                }

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
                domeGeneralData.creatorName = dataContainer.summitData.domes[i].creatorName;
                domeGeneralData.description = dataContainer.summitData.domes[i].description;
                domeGeneralData.isSubWorld = dataContainer.summitData.domes[i].isSubWorld;
                domeGeneralData.world360Image = dataContainer.summitData.domes[i].world360Image;
                domeGeneralData.companyLogo = dataContainer.summitData.domes[i].companyLogo;
                domeGeneralData.SubWorlds = dataContainer.summitData.domes[i].SubWorlds;
                domeGeneralData.domeCategory = dataContainer.summitData.domes[i].domeCategory;
                domeGeneralData.domeType = dataContainer.summitData.domes[i].domeType;

                domeGeneralData.is_penpenz = dataContainer.summitData.domes[i].is_penpenz;
                domeGeneralData.description = dataContainer.summitData.domes[i].description;
                domeGeneralData.creatorName = dataContainer.summitData.domes[i].creatorName;


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
        LoadingHandler.Instance.DisableDomeLoading();
    }

    void SetPlayerOnback()
    {
        GameplayEntityLoader.instance.mainController.transform.position = playerPos;
        GameplayEntityLoader.instance.mainController.transform.rotation = playerRot.CTQuaternion();
        GameplayEntityLoader.instance.mainController.transform.localScale = playerScale;
        XanaWorldDownloader.initialPlayerPos = playerPos;
        if (WorldItemView.m_EnvName == "XANA Summit")
        {
            ConstantsHolder.isFromXANASummit = false;
            ReferencesForGamePlay.instance.ChangeExitBtnImage(true);
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
        public UserDetails user;
    }
    [System.Serializable]
    public class UserDetails
    {
        public string name;

    }


}
