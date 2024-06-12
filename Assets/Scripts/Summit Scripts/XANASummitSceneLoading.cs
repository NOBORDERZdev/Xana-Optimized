using Photon.Pun.Demo.PunBasics;
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
        BuilderEventManager.AfterPlayerInstantiated += SetPlayerTransform;
        GamePlayButtonEvents.OnExitButtonXANASummit += LoadingXANASummitOnBack;
    }

    private void OnDisable()
    {
        BuilderEventManager.LoadNewScene -= LoadingNewScene;
        BuilderEventManager.AfterPlayerInstantiated -= SetPlayerTransform;
        GamePlayButtonEvents.OnExitButtonXANASummit -= LoadingXANASummitOnBack;
    }

    void LoadingNewScene(int domeId, Vector3 playerPos)
    {
        string[] sceneData = GetSceneName(domeId);

        if (string.IsNullOrEmpty(sceneData[0]))
            return;
            //SNSNotificationHandler.Instance.ShowNotificationMsg("");

        GetPlayerPosition(playerPos);
        string existingSceneName = WorldItemView.m_EnvName;
        WorldItemView.m_EnvName = sceneData[0];
        ConstantsHolder.xanaConstants.EnviornmentName = sceneData[0];
        gameplayEntityLoader.currentEnvironment = null;
        multiplayerController.isConnecting = false;
        gameplayEntityLoader.isEnvLoaded = false;
        gameplayEntityLoader.isAlreadySpawned = true;
        ConstantsHolder.isFromXANASummit = true;
        multiplayerController.Disconnect();

        XanaWorldDownloader.ResetAll();

        multiplayerController.playerobjects.Clear();

        SceneManager.UnloadSceneAsync(existingSceneName);

        if (sceneData[1] == "1")
            LoadBuilderSceneLoading(sceneData);

        multiplayerController.Connect(sceneData[0]);
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


    void LoadBuilderSceneLoading(string[] sceneData)
    {
        ConstantsHolder.xanaConstants.builderMapID = int.Parse(sceneData[2]);
        ConstantsHolder.xanaConstants.isBuilderScene = true;
        SceneManager.LoadSceneAsync("Builder", LoadSceneMode.Additive);
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
        ConstantsHolder.isFromXANASummit = true;
        multiplayerController.Disconnect();

        XanaWorldDownloader.ResetAll();

        multiplayerController.playerobjects.Clear();

        SceneManager.UnloadSceneAsync(existingSceneName);

        multiplayerController.Connect(sceneName);
    }
    string[] GetSceneName(int sceneId)
    {
        for (int i = 0; i < dataContainer.summitData.domes.Count; i++)
        {
            if (dataContainer.summitData.domes[i].id == sceneId)
            {
                if (dataContainer.summitData.domes[i].worldType)
                    return new[] { dataContainer.summitData.domes[i].name, "1", dataContainer.summitData.domes[i].builderWorldId };
                else
                    return new[] { dataContainer.summitData.domes[i].name, "0", dataContainer.summitData.domes[i].builderWorldId };
            }
        }

        return new[] { string.Empty, "0", "0" };
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
