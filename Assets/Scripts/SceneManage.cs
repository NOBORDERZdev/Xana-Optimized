using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun;
using Metaverse;
using System.Collections;
using System;

public class SceneManage : MonoBehaviourPunCallbacks
{
    public static bool callRemove;
    public bool isAddressableScene = true;
    public GameObject AnimHighlight;
    public GameObject popupPenal;
    public GameObject spawnCharacterObject;
    public GameObject spawnCharacterObjectRemote;
    public GameObject EventEndedPanel;
    public string mainScene = "Main";
    bool exitOnce = true;

    private void OnEnable()
    {
        mainScene = "Main";
        if (SceneManager.GetActiveScene().name == "Main")
        {
            AvatarManager.sendDataValue = false;
        }
        if (LoadFromFile.instance)
        {
            LoadFromFile.instance._uiReferences = this;
        }
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
        if (XanaConstants.xanaConstants.EnviornmentName.Contains("XANA Lobby"))
        {
            if (SoundManagerSettings.soundManagerSettings != null)
            {
                if (SoundManagerSettings.soundManagerSettings.bgmSource)
                SoundManagerSettings.soundManagerSettings.bgmSource.enabled = false;
                if (SoundManagerSettings.soundManagerSettings.videoSource)
                SoundManagerSettings.soundManagerSettings.videoSource.enabled = false;
                if (SoundManagerSettings.soundManagerSettings.effectsSource)
                SoundManagerSettings.soundManagerSettings.effectsSource.enabled = false;
            }
        }
    }
    public void LoadMain(bool changeOritentationChange)
    {
        if (XanaConstants.xanaConstants.metaverseType == XanaConstants.MetaverseType.PMY)
        {
            XanaConstants.xanaConstants.isBackFromPMY = true;
            //if (PlayerPrefs.GetInt("IsLoggedIn") != 1 && PlayerPrefs.GetInt("WalletLogin") != 1)
            //{
            //    PlayerPrefs.DeleteKey("IsProcessComplete");
            //    PlayerPrefs.DeleteKey("shownWelcome");
            //}
        }

        disableSoundXanalobby();
        XanaConstants.xanaConstants.isBackFromWorld = true;
        if (exitOnce)
        {
            exitOnce = false;
            if (XanaConstants.xanaConstants.isFromXanaLobby && !XanaConstants.xanaConstants.EnviornmentName.Contains("XANA Lobby"))
            {
                StartCoroutine(LobbySceneSwitch("XANA Lobby")); // to Lobby if player enter in world from Xana lobby
            }
            else if (XanaConstants.xanaConstants.isFromPMYLobby && !XanaConstants.xanaConstants.EnviornmentName.Contains("PMY ACADEMY"))
                StartCoroutine(LobbySceneSwitch("PMY ACADEMY")); // to Lobby if player enter in world from Xana lobby
            else
            {

                if (changeOritentationChange)
                {
                    XanaConstants.xanaConstants.JjWorldSceneChange = false;
                    XanaConstants.xanaConstants.orientationchanged = false;
                    XanaConstants.xanaConstants.mussuemEntry = JJMussuemEntry.Null;
                }
                if (GameManager.currentLanguage == "ja")
                {
                    LoadingHandler.Instance.UpdateLoadingStatusText("ホームに戻っています");
                }
                else if (GameManager.currentLanguage == "en")
                {
                    LoadingHandler.Instance.UpdateLoadingStatusText("Going Back to Home");
                }
                LoadingHandler.Instance.ShowLoading();
                //LoadingHandler.Instance.ShowLoading(ScreenOrientation.LandscapeLeft);

                if (XanaConstants.xanaConstants.needToClearMemory && AddressableDownloader.Instance)
                    AddressableDownloader.Instance.MemoryManager.RemoveAllAddressables();
                else
                    XanaConstants.xanaConstants.needToClearMemory = true;

                GC.Collect();
                AssetBundle.UnloadAllAssetBundles(true);
                Resources.UnloadUnusedAssets();
                StartCoroutine(LoadMainEnumerator());
            }
        }
    }
    private IEnumerator LobbySceneSwitch(string sceneToLoad)
    {
        LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
        if (!XanaConstants.xanaConstants.JjWorldSceneChange && !XanaConstants.xanaConstants.orientationchanged)
            Screen.orientation = ScreenOrientation.LandscapeLeft;

        yield return new WaitForSeconds(.2f);
        XanaConstants.xanaConstants.isBuilderScene = false;
        XanaConstants.xanaConstants.JjWorldSceneChange = true;
        XanaConstants.xanaConstants.JjWorldTeleportSceneName = sceneToLoad;
        StartCoroutine(LoadMainEnumerator());
    }
    IEnumerator LoadMainEnumerator()
    {
        LeaveRoom();
        yield return new WaitForSeconds(.5f);
        if (XanaConstants.xanaConstants.museumAssetLoaded != null)
            XanaConstants.xanaConstants.museumAssetLoaded.Unload(true);
    }
    public void LoadWorld()
    {
        if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
        {
            UIManager.Instance.LoginRegisterScreen.transform.SetAsLastSibling();
            UIManager.Instance.LoginRegisterScreen.SetActive(true);
        }
        else
        {
            UIManager.Instance.IsWorldClicked();
        }
    }
    public void LeaveRoom()
    {
        if (isAddressableScene)
        {
            callRemove = true;
            Launcher.instance.working = ScenesList.MainMenu;
            PhotonNetwork.LeaveRoom(false);
            PhotonNetwork.LeaveLobby();
            UserAnalyticsHandler.onUpdateWorldRelatedStats?.Invoke(false, false, false, true);
            Debug.Log("Exit: Api Called");
            PhotonNetwork.DestroyAll(true);
        }
        StartSceneLoading();
    }
    public void StartSceneLoading()
    {
        print("Hello Scene Manager");
        StartCoroutine(LoadMianScene());
    }
    IEnumerator LoadMianScene()
    {
        yield return new WaitForSeconds(.2f);
        Resources.UnloadUnusedAssets();
        print("mian scne " + mainScene);
        XanaConstants.xanaConstants.isBackFromWorld = true;
        if (XanaConstants.xanaConstants.JjWorldSceneChange)
        {
            float _rand;
            if (XanaConstants.xanaConstants.isBuilderScene)
                _rand = UnityEngine.Random.Range(25f, 30f);
            else
                _rand = UnityEngine.Random.Range(6f, 10f);
            LoadingHandler.Instance.randCurrentValue = _rand;
            StartCoroutine(LoadingHandler.Instance.IncrementSliderValue(_rand, true));
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene("Main");
        }
        else
        {
            if (XanaConstants.xanaConstants.isBuilderScene)
            {
                float _rand = UnityEngine.Random.Range(10f, 15f);
                LoadingHandler.Instance.randCurrentValue = _rand;
                StartCoroutine(LoadingHandler.Instance.IncrementSliderValue(_rand, true));
            }
            else
            {
                StartCoroutine(LoadingHandler.Instance.IncrementSliderValue(UnityEngine.Random.Range(6f, 10f), true));
            }
            yield return new WaitForSeconds(3f);
            SceneManager.LoadSceneAsync(mainScene);
        }
    }
}