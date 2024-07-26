using Photon.Pun.Demo.PunBasics;
using System;
using UnityEngine;
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

    private int previousUserLimit;

    public delegate void SetPlayerOnSubworldBack();
    public event SetPlayerOnSubworldBack setPlayerPositionDelegate;
    private void OnEnable()
    {
        BuilderEventManager.LoadNewScene += LoadingNewScene;
        BuilderEventManager.LoadSceneByName += LoadingNewScene;
        BuilderEventManager.LoadSummitScene += LoadDomesData;
        BuilderEventManager.AfterPlayerInstantiated += SetPlayerTransform;
        GamePlayButtonEvents.OnExitButtonXANASummit += LoadingXANASummitOnBack;
        OnJoinSubItem += SummitMiniMapStatusOnSceneChange; 


        if(LoadingHandler.Instance.nftLoadingScreen.activeInHierarchy)
        {
            LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
        }
    }

    private void OnDisable()
    {
        BuilderEventManager.LoadNewScene -= LoadingNewScene;
        BuilderEventManager.LoadSceneByName -= LoadingNewScene;
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

    void LoadingNewScene(int domeId, Vector3 playerPos)
    {
        XANASummitDataContainer.DomeGeneralData domeGeneralData = new XANASummitDataContainer.DomeGeneralData();
        domeGeneralData = GetDomeData(domeId);

        if (string.IsNullOrEmpty(domeGeneralData.world))
            return;

        SummitMiniMapStatusOnSceneChange(false);
        StartCoroutine(LoadingHandler.Instance.FadeIn());

        GetPlayerPosition(playerPos);
        string existingSceneName = WorldItemView.m_EnvName;
        ConstantsHolder.loadedScenes.Push(existingSceneName);
        WorldItemView.m_EnvName = domeGeneralData.world;
        ConstantsHolder.xanaConstants.EnviornmentName = domeGeneralData.world;
        previousUserLimit = ConstantsHolder.userLimit;
        ConstantsHolder.userLimit = domeGeneralData.maxPlayer;
        ConstantsHolder.isPenguin = domeGeneralData.IsPenguin;
        ConstantsHolder.isFixedHumanoid = domeGeneralData.Ishumanoid;
        if (domeGeneralData.Ishumanoid)
            XANASummitDataContainer.fixedAvatarJson = domeGeneralData.Avatarjson;
        gameplayEntityLoader.currentEnvironment = null;
        gameplayEntityLoader.addressableSceneName = domeGeneralData.world;
        multiplayerController.isConnecting = false;
        multiplayerController.singlePlayerInstance = domeGeneralData.experienceType != "double";
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

    public void LoadingNewScene(string SceneName, Vector3 playerPos)
    {
        if (string.IsNullOrEmpty(SceneName))
            return;

        SummitMiniMapStatusOnSceneChange(false);
        GetPlayerPosition(playerPos);
        string existingSceneName = WorldItemView.m_EnvName;
        ConstantsHolder.loadedScenes.Push(existingSceneName);
        WorldItemView.m_EnvName = SceneName;
        ConstantsHolder.xanaConstants.EnviornmentName = SceneName;
        previousUserLimit = ConstantsHolder.userLimit;
        ConstantsHolder.userLimit = 25;
        //XANASummitDataContainer.fixedAvatarJson = dataContainer.avatarJson[domeGeneralData.avatarId];
        gameplayEntityLoader.currentEnvironment = null;
        gameplayEntityLoader.addressableSceneName = SceneName;
        multiplayerController.isConnecting = false;
        gameplayEntityLoader.isEnvLoaded = false;
        gameplayEntityLoader.isAlreadySpawned = true;
        ConstantsHolder.isFromXANASummit = true;

        ReferencesForGamePlay.instance.m_34player.transform.localScale = new Vector3(0, 0, 0);

        multiplayerController.Disconnect();

        XanaWorldDownloader.ResetAll();

        multiplayerController.playerobjects.Clear();

        SceneManager.UnloadSceneAsync(existingSceneName);

        multiplayerController.Connect(SceneName);
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
        string sceneName = ConstantsHolder.loadedScenes.Pop();

        //string sceneName = "XANA Summit";
        string existingSceneName = WorldItemView.m_EnvName;
        WorldItemView.m_EnvName = sceneName;
        ConstantsHolder.xanaConstants.EnviornmentName = sceneName;
        ConstantsHolder.userLimit = previousUserLimit;
        ConstantsHolder.isPenguin = false;
        ConstantsHolder.isFixedHumanoid = false;
        gameplayEntityLoader.currentEnvironment = null;
        ConstantsHolder.xanaConstants.isBuilderScene = false;
        multiplayerController.isConnecting = false;
        gameplayEntityLoader.isEnvLoaded = false;
        gameplayEntityLoader.isAlreadySpawned = true;
        ConstantsHolder.isFromXANASummit = false;
        multiplayerController.Disconnect();

        XanaWorldDownloader.ResetAll();

        multiplayerController.playerobjects.Clear();

        SceneManager.UnloadSceneAsync(existingSceneName);

        multiplayerController.Connect(sceneName);
        ConstantsHolder.DiasableMultiPartPhoton = false;

        // Map Working
        _domeMiniMap.SummitSceneReloaded();
        SummitMiniMapStatusOnSceneChange(true);
        //
    }
    XANASummitDataContainer.DomeGeneralData GetDomeData(int sceneId)
    {
        XANASummitDataContainer.DomeGeneralData domeGeneralData = new XANASummitDataContainer.DomeGeneralData();
        for (int i = 0; i < dataContainer.summitData.domes.Count; i++)
        {
            if (dataContainer.summitData.domes[i].id == sceneId)
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


}
