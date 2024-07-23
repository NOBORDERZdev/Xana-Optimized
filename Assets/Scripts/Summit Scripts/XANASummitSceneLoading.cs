using Photon.Pun.Demo.PunBasics;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class XANASummitSceneLoading : MonoBehaviour
{
    public static Vector3 playerPos;
    public static Quaternion playerRot;
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
        BuilderEventManager.LoadNewScene += LoadingFromDome;
        BuilderEventManager.LoadSceneByName += LoadingSceneByIDOrName;
        BuilderEventManager.LoadSummitScene += LoadDomesData;
        BuilderEventManager.AfterPlayerInstantiated += SetPlayerTransform;
        GamePlayButtonEvents.OnExitButtonXANASummit += LoadingXANASummitOnBack;
        OnJoinSubItem += SummitMiniMapStatusOnSceneChange;
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

    void LoadingFromDome(int domeId, Vector3 playerPos)
    {
        XANASummitDataContainer.DomeGeneralData domeGeneralData = new XANASummitDataContainer.DomeGeneralData();
        domeGeneralData = GetDomeData(domeId);

        if (string.IsNullOrEmpty(domeGeneralData.world))
            return;

        SummitMiniMapStatusOnSceneChange(false);
        StartCoroutine(LoadingHandler.Instance.FadeIn());
        GetPlayerPosition(playerPos);

        ConstantsHolder.domeId = domeId;
        string existingSceneName = WorldItemView.m_EnvName;

        XANASummitDataContainer.StackInfoWorld subWorldInfo = new XANASummitDataContainer.StackInfoWorld();
        subWorldInfo.id = domeGeneralData.worldId.ToString();
        subWorldInfo.name = domeGeneralData.world;
        subWorldInfo.isBuilderWorld = domeGeneralData.worldType;
        subWorldInfo.thumbnail = domeGeneralData.thumbnail;
        subWorldInfo.user_limit = domeGeneralData.maxPlayer;
        XANASummitDataContainer.loadedScenes.Push(subWorldInfo);

        WorldItemView.m_EnvName = domeGeneralData.world;
        ConstantsHolder.xanaConstants.EnviornmentName = domeGeneralData.world;
        gameplayEntityLoader.addressableSceneName = domeGeneralData.world;
        ConstantsHolder.userLimit = domeGeneralData.maxPlayer;
        ConstantsHolder.isPenguin = domeGeneralData.IsPenguin;
        ConstantsHolder.isFixedHumanoid = domeGeneralData.Ishumanoid;
        ConstantsHolder.xanaConstants.MuseumID = domeGeneralData.worldId.ToString();
        if (domeGeneralData.Ishumanoid)
            XANASummitDataContainer.fixedAvatarJson = domeGeneralData.Avatarjson;
        gameplayEntityLoader.currentEnvironment = null;
        multiplayerController.singlePlayerInstance = domeGeneralData.experienceType != "double";
        multiplayerController.isConnecting = false;
        gameplayEntityLoader.isEnvLoaded = false;
        gameplayEntityLoader.isAlreadySpawned = true;
        ConstantsHolder.isFromXANASummit = true;

        multiplayerController.Disconnect();
        XanaWorldDownloader.ResetAll();
        multiplayerController.playerobjects.Clear();
        SceneManager.UnloadSceneAsync(existingSceneName);
        if (domeGeneralData.worldType)
            LoadBuilderSceneLoading(domeGeneralData.builderWorldId);

        multiplayerController.Connect("XANA Summit-" + domeGeneralData.world);
    }

    public async void LoadingSceneByIDOrName(string worldId, Vector3 playerPos)
    {
        if (string.IsNullOrEmpty(worldId))
            return;

        SummitMiniMapStatusOnSceneChange(false);
        GetPlayerPosition(playerPos);
        string existingSceneName = WorldItemView.m_EnvName;

        WorldInfo worldInfo =await GetSingleWorldData(worldId);

        XANASummitDataContainer.StackInfoWorld subWorldInfo = new XANASummitDataContainer.StackInfoWorld();
        subWorldInfo.id = worldInfo.id;
        subWorldInfo.name = worldInfo.name;
        subWorldInfo.isBuilderWorld = (worldInfo.entityType== WorldType.USER_WORLD.ToString())?true : false;
        subWorldInfo.thumbnail = worldInfo.thumbnail_new;
        subWorldInfo.user_limit = worldInfo.user_limit;
        subWorldInfo.domeId = ConstantsHolder.domeId;
        subWorldInfo.haveSubWorlds = ConstantsHolder.haveSubWorlds;
        XANASummitDataContainer.loadedScenes.Push(subWorldInfo);

        WorldItemView.m_EnvName = worldInfo.name;
        ConstantsHolder.xanaConstants.EnviornmentName = worldInfo.name;
        gameplayEntityLoader.addressableSceneName = worldInfo.name;
        ConstantsHolder.userLimit = worldInfo.user_limit;
        ConstantsHolder.xanaConstants.MuseumID = worldInfo.id.ToString();
        gameplayEntityLoader.currentEnvironment = null;
        multiplayerController.isConnecting = false;
        gameplayEntityLoader.isEnvLoaded = false;
        gameplayEntityLoader.isAlreadySpawned = true;
        ConstantsHolder.isFromXANASummit = true;
        //XANASummitDataContainer.fixedAvatarJson = dataContainer.avatarJson[domeGeneralData.avatarId];

        multiplayerController.Disconnect();

        XanaWorldDownloader.ResetAll();

        multiplayerController.playerobjects.Clear();

        await SceneManager.UnloadSceneAsync(existingSceneName);

        if (subWorldInfo.isBuilderWorld)
            LoadBuilderSceneLoading(int.Parse(worldInfo.id));

        multiplayerController.Connect("XANA Summit-" + worldInfo.name);
    }

    void AddStack()
    {

    }


    void LoadBuilderSceneLoading(int builderMapId)
    {
        ConstantsHolder.xanaConstants.builderMapID = builderMapId;
        ConstantsHolder.xanaConstants.isBuilderScene = true;
        gameplayEntityLoader.addressableSceneName = null;
        WorldItemView.m_EnvName = "Builder";
        ConstantsHolder.xanaConstants.EnviornmentName = "Builder";
        AsyncOperation handle = SceneManager.LoadSceneAsync("Builder", LoadSceneMode.Additive);
        handle.completed += Handle_completed;
    }

    private void Handle_completed(AsyncOperation obj)
    {
        obj.allowSceneActivation = true;
    }

    void LoadingXANASummitOnBack()
    {
        if (ConstantsHolder.isFromXANASummit == false)
            return;

        setPlayerPositionDelegate = SetPlayerOnback;

        StartCoroutine(LoadingHandler.Instance.FadeIn());
        XANASummitDataContainer.StackInfoWorld subWorldInfo = new XANASummitDataContainer.StackInfoWorld();
        subWorldInfo = XANASummitDataContainer.loadedScenes.Pop();

        string existingSceneName = WorldItemView.m_EnvName;
        //string sceneName = "XANA Summit";
        WorldItemView.m_EnvName = subWorldInfo.name;
        ConstantsHolder.xanaConstants.EnviornmentName = subWorldInfo.name;
        ConstantsHolder.userLimit = subWorldInfo.user_limit;
        ConstantsHolder.xanaConstants.isBuilderScene = subWorldInfo.isBuilderWorld;
        ConstantsHolder.xanaConstants.MuseumID = subWorldInfo.id;

        ConstantsHolder.isPenguin = false;
        ConstantsHolder.isFixedHumanoid = false;
        gameplayEntityLoader.currentEnvironment = null;
        multiplayerController.isConnecting = false;
        gameplayEntityLoader.isEnvLoaded = false;
        gameplayEntityLoader.isAlreadySpawned = true;
        ConstantsHolder.isFromXANASummit = false;
        multiplayerController.Disconnect();

        XanaWorldDownloader.ResetAll();

        multiplayerController.playerobjects.Clear();

        SceneManager.UnloadSceneAsync(existingSceneName);

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
                domeGeneralData.maxPlayer = dataContainer.summitData.domes[i].maxPlayer;
                domeGeneralData.IsPenguin = dataContainer.summitData.domes[i].IsPenguin;
                domeGeneralData.Ishumanoid = dataContainer.summitData.domes[i].Ishumanoid;
                domeGeneralData.Avatarjson = dataContainer.summitData.domes[i].Avatarjson;
                domeGeneralData.AvatarIndex = dataContainer.summitData.domes[i].AvatarIndex;
                domeGeneralData.name = dataContainer.summitData.domes[i].name;
                //if (dataContainer.summitData1.domes[i].worldType)
                //    return new Tuple<string[],string>(new[] { dataContainer.summitData1.domes[i].world, "1", dataContainer.summitData1.domes[i].builderWorldId }, dataContainer.summitData1.domes[i].experienceType);
                //else
                //    return new[] { dataContainer.summitData1.domes[i].world, "0", dataContainer.summitData1.domes[i].builderWorldId };
            }
        }
        return domeGeneralData;
        //return new[] { string.Empty, "0", "0" };
    }

    void GetPlayerPosition(Vector3 _playerPos)
    {
        playerPos = _playerPos;
        playerRot = GameplayEntityLoader.instance.mainController.transform.rotation;
        playerScale = GameplayEntityLoader.instance.mainController.transform.localScale;
    }

    void SetPlayerTransform()
    {
        if (ConstantsHolder.isFromXANASummit == false)
            return;

        setPlayerPositionDelegate?.Invoke();


        StartCoroutine(LoadingHandler.Instance.FadeOut());
    }

    void SetPlayerOnback()
    {
        GameplayEntityLoader.instance.mainController.transform.position = playerPos;
        GameplayEntityLoader.instance.mainController.transform.rotation = playerRot;
        GameplayEntityLoader.instance.mainController.transform.localScale = playerScale;

        if (WorldItemView.m_EnvName == "XANA Summit")
        {
            ConstantsHolder.isFromXANASummit = false;
        }

        setPlayerPositionDelegate = null;
    }



    async Task<WorldInfo> GetSingleWorldData(string worldID)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(ConstantsGod.API_BASEURL+ConstantsGod.SINGLEWORLDINFO+worldID))
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
                WorldInfo worldInfo = new WorldInfo();
                worldInfo= JsonUtility.FromJson<WorldInfo>(www.downloadHandler.text);
                www.Dispose();
                return worldInfo;
            }
            
        }
    }


    [System.Serializable]
    public class WorldInfo
    {
        public string id;
        public string name;
        public int user_limit;
        public string thumbnail;
        public string banner;
        public string thumbnail_new;
        public string description;
        public string creator;
        public string entityType;
    }


  
}
