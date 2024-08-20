using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun;
using Metaverse;
using System.Collections;
using System;
using UnityEngine.Scripting;
using Photon.Realtime;

public class HomeSceneLoader : MonoBehaviourPunCallbacks
{
    public GameObject EventEndedPanel;
    private string mainScene = "Home";
    private string lobbyScene = "RooftopParty";
    bool exitOnce = true;
    GameManager gameManager;
    private void Awake()
    {
        gameManager = GameManager.Instance;
    }
    private void OnEnable()
    {
        if (GameplayEntityLoader.instance)
        {
            GameplayEntityLoader.instance._uiReferences = this;
        }
        MainSceneEventHandler.MemoryRelaseAfterLoading += ReleaseUnsedMemory;
    }

    private void OnDisable()
    {
        MainSceneEventHandler.MemoryRelaseAfterLoading -= ReleaseUnsedMemory;
    }

    public void OpenARScene()
    {
        SceneManager.LoadScene("ARHeadWebCamTextureExample");
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Camera))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.Camera);
            PlayerPrefs.SetInt("RequestSend", 1);
        }
    }
    public void disableSoundXanalobby() // Disabling Audio Sources in Xana Lobby on exit to avoid sound increase on Loding screen after exit
    {
        if (ConstantsHolder.xanaConstants.EnviornmentName.Contains("XANA Lobby"))
        {
            if (SoundSettings.soundManagerSettings != null)
            {
                if (SoundSettings.soundManagerSettings.bgmSource)
                    SoundSettings.soundManagerSettings.bgmSource.enabled = false;
                if (SoundSettings.soundManagerSettings.videoSource)
                    SoundSettings.soundManagerSettings.videoSource.enabled = false;
                if (SoundSettings.soundManagerSettings.effectsSource)
                    SoundSettings.soundManagerSettings.effectsSource.enabled = false;
            }
        }
    }
    public void LoadMain(bool changeOritentationChange)
    {
        disableSoundXanalobby();
        ConstantsHolder.xanaConstants.isBackFromWorld = true;
        
       
        if (exitOnce)
        {
            exitOnce = false;

            if (SceneManager.GetActiveScene().name == lobbyScene)
            {
                Application.Quit();
            }

            else if (ConstantsHolder.xanaConstants.isFromXanaLobby && !ConstantsHolder.xanaConstants.EnviornmentName.Contains("XANA Lobby"))
            {
                StartCoroutine(LobbySceneSwitch()); // to Lobby if player enter in world from Xana lobby
            }
            else
            {
                if (changeOritentationChange)
                {
                    ConstantsHolder.xanaConstants.JjWorldSceneChange = false;
                    ConstantsHolder.xanaConstants.orientationchanged = false;
                    ConstantsHolder.xanaConstants.mussuemEntry = JJMussuemEntry.Null;
                }
                if (GameManager.currentLanguage == "ja")
                {
                    LoadingHandler.Instance.UpdateLoadingStatusText("ホームに戻っています");
                }
                else if (GameManager.currentLanguage == "en")
                {
                    LoadingHandler.Instance.UpdateLoadingStatusText("Going Back to Home");
                }

                Screen.orientation = ScreenOrientation.LandscapeLeft;
                LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));

                LoadingHandler.Instance.ShowLoading();
                StartCoroutine(LoadingHandler.Instance.IncrementSliderValue(1f,true));

                if (ConstantsHolder.xanaConstants.needToClearMemory)
                    AddressableDownloader.Instance.MemoryManager.RemoveAllAddressables();
                else
                    ConstantsHolder.xanaConstants.needToClearMemory = true;

                if (ConstantsHolder.xanaConstants.isXanaPartyWorld && ConstantsHolder.xanaConstants.isJoinigXanaPartyGame)
                {
                    ConstantsHolder.xanaConstants.isJoinigXanaPartyGame = false;
                    ConstantsHolder.xanaConstants.XanaPartyGameId = 0;
                    ConstantsHolder.xanaConstants.XanaPartyGameName = "";
                    ConstantsHolder.xanaConstants.isBuilderScene = false;
                    ConstantsHolder.xanaConstants.builderMapID = 0;
                    // Load the main scene
                    Screen.orientation = ScreenOrientation.LandscapeLeft;
                    LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));

                    MutiplayerController.instance.working = ScenesList.AddressableScene;
                    if (PhotonNetwork.Server == ServerConnection.GameServer)
                    {
                        PhotonNetwork.LeaveRoom();
                    }
                    PhotonNetwork.LeaveLobby();
                    PhotonNetwork.DestroyAll(true);
                    StartSceneLoading();
                }
                else
                {
                    LeaveRoom();
                }
            }
        }
    }
    private IEnumerator LobbySceneSwitch()
    {
        LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
        if (!ConstantsHolder.xanaConstants.JjWorldSceneChange && !ConstantsHolder.xanaConstants.orientationchanged)
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        ConstantsHolder.xanaConstants.isBuilderScene = false;
         ConstantsHolder.xanaConstants.isXanaPartyWorld = false;
        ConstantsHolder.xanaConstants.JjWorldSceneChange = true;
        ConstantsHolder.xanaConstants.JjWorldTeleportSceneName = "XANA Lobby";

        // While Retruing from sub world to Xana Lobby
        // Storing Xana Lobby Ids

        if ((ConstantsGod.API_BASEURL.Contains("test")))
            ConstantsHolder.xanaConstants.MuseumID = "406";
        else
            ConstantsHolder.xanaConstants.MuseumID = "38";

        LeaveRoom();

        yield return null;
    }
    public void LoadWorld()
    {
        if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
        {
            gameManager.UiManager.LoginRegisterScreen.transform.SetAsLastSibling();
            gameManager.UiManager.LoginRegisterScreen.SetActive(true);
        }
        else
        {
            gameManager.UiManager.IsWorldClicked();
        }
    }
    public void LeaveRoom()
    {
        MutiplayerController.instance.working = ScenesList.MainMenu;
        PhotonNetwork.LeaveRoom(false);
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.DestroyAll(true);
        UserAnalyticsHandler.onUpdateWorldRelatedStats?.Invoke(false, false, false, true);
        StartSceneLoading();
    }
    public void StartSceneLoading()
    {
        PhotonHandler.levelName = "Addressable";
        ConstantsHolder.xanaConstants.CurrentSceneName = "Addressable";
        ConstantsHolder.xanaConstants.isBackFromWorld = true;
        SceneManager.LoadSceneAsync(mainScene);
    }

    public void ReleaseUnsedMemory()
    {
        StartCoroutine(ReleaseUnsedMemoryDelay());
    }

    IEnumerator ReleaseUnsedMemoryDelay()
    {
        yield return new WaitForSeconds(3f);
        GC.Collect();
        Resources.UnloadUnusedAssets();
        Debug.LogError("memory released here..");
    }


}