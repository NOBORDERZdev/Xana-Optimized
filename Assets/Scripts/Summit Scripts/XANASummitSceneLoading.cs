using Crosstales;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class XANASummitSceneLoading : MonoBehaviour
{
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
        
        Vector3[] currentPlayerPos=GetPlayerPosition(playerPos);
        Debug.LogError(currentPlayerPos[0]);
        Debug.LogError(currentPlayerPos[1]);
        Debug.LogError(currentPlayerPos[2]);

        ConstantsHolder.domeId = domeId;
        string existingSceneName = WorldItemView.m_EnvName;

        XANASummitDataContainer.StackInfoWorld subWorldInfo = new XANASummitDataContainer.StackInfoWorld();
        subWorldInfo.id = ConstantsHolder.xanaConstants.MuseumID;
        subWorldInfo.name = existingSceneName;
        subWorldInfo.isBuilderWorld = ConstantsHolder.xanaConstants.isBuilderScene;
        subWorldInfo.user_limit = ConstantsHolder.userLimit;
        subWorldInfo.domeId = ConstantsHolder.domeId;
        subWorldInfo.haveSubWorlds = ConstantsHolder.haveSubWorlds;
        subWorldInfo.isFromSummitWorld = ConstantsHolder.isFromXANASummit;
        subWorldInfo.playerTrasnform = currentPlayerPos;
        XANASummitDataContainer.loadedScenes.Push(subWorldInfo);

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
        ConstantsHolder.haveSubWorlds = domeGeneralData.isSubWorld;
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

        ReferencesForGamePlay.instance.m_34player.transform.localScale = new Vector3(0, 0, 0);

        multiplayerController.Connect("XANA Summit-" + domeGeneralData.world);
    }

    public async void LoadingSceneByIDOrName(string worldId, Vector3 playerPos)
    {
        if (string.IsNullOrEmpty(worldId))
            return;

        StartCoroutine(LoadingHandler.Instance.FadeIn());
        SummitMiniMapStatusOnSceneChange(false);
        Vector3[] currentPlayerPos=GetPlayerPosition(playerPos);
        Debug.LogError(currentPlayerPos[0]);
        Debug.LogError(currentPlayerPos[1]);
        Debug.LogError(currentPlayerPos[2]);
        string existingSceneName = WorldItemView.m_EnvName;

        SingleWorldInfo worldInfo = await GetSingleWorldData(worldId);

        XANASummitDataContainer.StackInfoWorld subWorldInfo = new XANASummitDataContainer.StackInfoWorld();
        subWorldInfo.id = ConstantsHolder.xanaConstants.MuseumID;
        subWorldInfo.name = existingSceneName;
        subWorldInfo.isBuilderWorld = ConstantsHolder.xanaConstants.isBuilderScene;
        subWorldInfo.user_limit = ConstantsHolder.userLimit;
        subWorldInfo.domeId = ConstantsHolder.domeId;
        subWorldInfo.haveSubWorlds = ConstantsHolder.haveSubWorlds;
        subWorldInfo.isFromSummitWorld = ConstantsHolder.isFromXANASummit;
        subWorldInfo.playerTrasnform = currentPlayerPos;
        XANASummitDataContainer.loadedScenes.Push(subWorldInfo);

        WorldItemView.m_EnvName = worldInfo.data.name;
        ConstantsHolder.xanaConstants.EnviornmentName = worldInfo.data.name;
        gameplayEntityLoader.addressableSceneName = worldInfo.data.name;
        ConstantsHolder.userLimit = worldInfo.data.user_limit;
        ConstantsHolder.xanaConstants.MuseumID = worldInfo.data.id;
        ConstantsHolder.haveSubWorlds = false;
        gameplayEntityLoader.currentEnvironment = null;
        multiplayerController.isConnecting = false;
        gameplayEntityLoader.isEnvLoaded = false;
        gameplayEntityLoader.isAlreadySpawned = true;

        ReferencesForGamePlay.instance.m_34player.transform.localScale = new Vector3(0, 0, 0);

        multiplayerController.Disconnect();

        XanaWorldDownloader.ResetAll();

        multiplayerController.playerobjects.Clear();

        await SceneManager.UnloadSceneAsync(existingSceneName);

        if (subWorldInfo.isBuilderWorld)
            LoadBuilderSceneLoading(int.Parse(worldInfo.data.id));

        multiplayerController.Connect("XANA Summit-" + worldInfo.data.name);
    }

    void AddStack()
    {

    }


    async void LoadBuilderSceneLoading(int builderMapId)
    {
        ConstantsHolder.xanaConstants.builderMapID = builderMapId;
        ConstantsHolder.xanaConstants.isBuilderScene = true;
        gameplayEntityLoader.addressableSceneName = null;
        WorldItemView.m_EnvName = "Builder";
        ConstantsHolder.xanaConstants.EnviornmentName = "Builder";
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

        setPlayerPositionDelegate = SetPlayerOnback;

        StartCoroutine(LoadingHandler.Instance.FadeIn());
        XANASummitDataContainer.StackInfoWorld subWorldInfo = new XANASummitDataContainer.StackInfoWorld();
        subWorldInfo = XANASummitDataContainer.loadedScenes.Pop();

        playerPos = subWorldInfo.playerTrasnform[0];
        playerRot= subWorldInfo.playerTrasnform[1];
        playerScale= subWorldInfo.playerTrasnform[2];

        Debug.LogError(subWorldInfo.playerTrasnform[0]);
        Debug.LogError(subWorldInfo.playerTrasnform[1]);
        Debug.LogError(subWorldInfo.playerTrasnform[2]);

        string existingSceneName = WorldItemView.m_EnvName;
        //string sceneName = "XANA Summit";
        WorldItemView.m_EnvName = subWorldInfo.name;
        ConstantsHolder.xanaConstants.EnviornmentName = subWorldInfo.name;
        ConstantsHolder.userLimit = subWorldInfo.user_limit;
        ConstantsHolder.xanaConstants.isBuilderScene = subWorldInfo.isBuilderWorld;
        ConstantsHolder.xanaConstants.MuseumID = subWorldInfo.id;
        ConstantsHolder.isFromXANASummit = subWorldInfo.isFromSummitWorld;

        ConstantsHolder.isPenguin = false;
        ConstantsHolder.isFixedHumanoid = false;
        gameplayEntityLoader.currentEnvironment = null;
        multiplayerController.isConnecting = false;
        gameplayEntityLoader.isEnvLoaded = false;
        //ConstantsHolder.isFromXANASummit = false;
        gameplayEntityLoader.isAlreadySpawned = true;
        multiplayerController.Disconnect();

        XanaWorldDownloader.ResetAll();

        multiplayerController.playerobjects.Clear();

        await SceneManager.UnloadSceneAsync(existingSceneName);

        if (subWorldInfo.isBuilderWorld)
            LoadBuilderSceneLoading(int.Parse(subWorldInfo.id));

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
                domeGeneralData.isSubWorld= dataContainer.summitData.domes[i].isSubWorld;
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

        return new []{playerPos,playerRot,playerScale };
    }

    void SetPlayerTransform()
    {
        //if (ConstantsHolder.isFromXANASummit == false)
        //    return;

        setPlayerPositionDelegate?.Invoke();


        StartCoroutine(LoadingHandler.Instance.FadeOut());
    }

    void SetPlayerOnback()
    {
        Debug.LogError(playerPos);
        GameplayEntityLoader.instance.mainController.transform.position = playerPos;
        GameplayEntityLoader.instance.mainController.transform.rotation = playerRot.CTQuaternion();
        GameplayEntityLoader.instance.mainController.transform.localScale = playerScale;
        if (WorldItemView.m_EnvName == "XANA Summit")
        {
            ConstantsHolder.isFromXANASummit = false;
        }
        setPlayerPositionDelegate = null;
    }



    async Task<SingleWorldInfo> GetSingleWorldData(string worldID)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(ConstantsGod.API_BASEURL + ConstantsGod.SINGLEWORLDINFO + worldID))
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
                Debug.LogError(www.downloadHandler.text);
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
