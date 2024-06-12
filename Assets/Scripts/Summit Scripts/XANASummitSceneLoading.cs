using Photon.Pun.Demo.PunBasics;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class XANASummitSceneLoading : MonoBehaviour
{
    public static Vector3 playerPos;
    public static Quaternion playerRot;
    public static Vector3 playerScale;

    public MutiplayerController multiplayerController;

    public GameplayEntityLoader gameplayEntityLoader;

    public XANASummitDataContainer dataContainer;
    private void OnEnable()
    {
        BuilderEventManager.LoadNewScene += LoadingNewScene;
        BuilderEventManager.LoadSummitScene += LoadDomesData;
        BuilderEventManager.AfterPlayerInstantiated += SetPlayerTransform;
        GamePlayButtonEvents.OnExitButtonXANASummit += LoadingXANASummitOnBack;
    }

    private void OnDisable()
    {
        BuilderEventManager.LoadNewScene -= LoadingNewScene;
        BuilderEventManager.LoadSummitScene -= LoadDomesData;
        BuilderEventManager.AfterPlayerInstantiated -= SetPlayerTransform;
        GamePlayButtonEvents.OnExitButtonXANASummit -= LoadingXANASummitOnBack;
    }

    void LoadDomesData()
    {
        dataContainer.GetAllDomesData();
    }

    void LoadingNewScene(int domeId, Vector3 playerPos)
    {
        //zero index SceneName
        //1st index builder or not
        //2nd index Builder Id
        XANASummitDataContainer.DomeGeneralData domeGeneralData = new XANASummitDataContainer.DomeGeneralData();
        //string[] sceneData = GetSceneName(domeId);
        domeGeneralData = GetDomeData(domeId);

        if (string.IsNullOrEmpty(domeGeneralData.world))
            return;

        GetPlayerPosition(playerPos);
        string existingSceneName = WorldItemView.m_EnvName;
        WorldItemView.m_EnvName = domeGeneralData.world;
        ConstantsHolder.xanaConstants.EnviornmentName = domeGeneralData.world;
        gameplayEntityLoader.currentEnvironment = null;
        gameplayEntityLoader.addressableSceneName = string.Empty;
        multiplayerController.isConnecting = false;
        multiplayerController.singlePlayerInstance = domeGeneralData.experienceType;
        gameplayEntityLoader.isEnvLoaded = false;
        gameplayEntityLoader.isAlreadySpawned = true;
        ConstantsHolder.isFromXANASummit = true;
        multiplayerController.Disconnect();

        XanaWorldDownloader.ResetAll();

        multiplayerController.playerobjects.Clear();

        SceneManager.UnloadSceneAsync(existingSceneName);
        
        if (domeGeneralData.worldType)
            LoadBuilderSceneLoading(domeGeneralData.builderWorldId);

        multiplayerController.Connect(domeGeneralData.world);
    }

    public void LoadingNewScene(string SceneName,Vector3 playerPos)
    {
        if (string.IsNullOrEmpty(SceneName))
            return;
        
        GetPlayerPosition(playerPos);
        string existingSceneName = WorldItemView.m_EnvName;
        WorldItemView.m_EnvName = SceneName;
        ConstantsHolder.xanaConstants.EnviornmentName = SceneName;
        gameplayEntityLoader.currentEnvironment = null;
        gameplayEntityLoader.addressableSceneName = string.Empty;
        multiplayerController.isConnecting = false;
        gameplayEntityLoader.isEnvLoaded = false;
        gameplayEntityLoader.isAlreadySpawned = true;
        ConstantsHolder.isFromXANASummit = true;
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
        AsyncOperation handle=SceneManager.LoadSceneAsync("Builder", LoadSceneMode.Additive);
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

        string sceneName = "XANA Summit";
        string existingSceneName = WorldItemView.m_EnvName;
        WorldItemView.m_EnvName = sceneName;
        ConstantsHolder.xanaConstants.EnviornmentName = sceneName;
        gameplayEntityLoader.currentEnvironment = null;
        multiplayerController.isConnecting = false;
        gameplayEntityLoader.isEnvLoaded = false;
        gameplayEntityLoader.isAlreadySpawned = true;
        multiplayerController.Disconnect();

        XanaWorldDownloader.ResetAll();

        multiplayerController.playerobjects.Clear();

        SceneManager.UnloadSceneAsync(existingSceneName);

        multiplayerController.Connect(sceneName);
    }
    XANASummitDataContainer.DomeGeneralData GetDomeData(int sceneId)
    {
        XANASummitDataContainer.DomeGeneralData domeGeneralData=new XANASummitDataContainer.DomeGeneralData();
        for (int i = 0; i < dataContainer.summitData1.domes.Count; i++)
        {
            if (dataContainer.summitData1.domes[i].id == sceneId)
            {
                domeGeneralData.world = dataContainer.summitData1.domes[i].world;
                domeGeneralData.worldType= dataContainer.summitData1.domes[i].worldType;
                domeGeneralData.experienceType = dataContainer.summitData1.domes[i].experienceType;
                domeGeneralData.builderWorldId= dataContainer.summitData1.domes[i].builderWorldId;
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

        if (WorldItemView.m_EnvName == "XANA Summit")
        {
            GameplayEntityLoader.instance.mainController.transform.position = playerPos;
            GameplayEntityLoader.instance.mainController.transform.rotation = playerRot;
            GameplayEntityLoader.instance.mainController.transform.localScale = playerScale;
            ConstantsHolder.isFromXANASummit = false;
        }
    }
}
