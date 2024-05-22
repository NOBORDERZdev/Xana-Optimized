using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class XANASummitSceneLoading : MonoBehaviour
{
    public MutiplayerController multiplayerController;

    public GameplayEntityLoader gameplayEntityLoader;


    private void OnEnable()
    {
        BuilderEventManager.LoadNewScene += LoadingNewScene;
    }

    private void OnDisable()
    {
        BuilderEventManager.LoadNewScene -= LoadingNewScene;
    }

    void LoadingNewScene(string sceneName)
    {
        WorldItemView.m_EnvName = sceneName;
        ConstantsHolder.xanaConstants.EnviornmentName = sceneName;
        gameplayEntityLoader.currentEnvironment = null;
        multiplayerController.isConnecting = false;
        gameplayEntityLoader.isEnvLoaded = false;
        gameplayEntityLoader.isAlreadySpawned = true;
        multiplayerController.fromXANASUmmit = true;
        multiplayerController.Disconnect();

        multiplayerController.Connect(sceneName);

        SceneManager.UnloadSceneAsync("XANA Summit");
    }
}
