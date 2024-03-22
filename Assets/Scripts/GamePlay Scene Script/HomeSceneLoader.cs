using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun;
using Metaverse;
using System.Collections;
using System;

public class HomeSceneLoader : MonoBehaviourPunCallbacks
{
    public static bool callRemove;
    public bool isAddressableScene = true;
    public GameObject EventEndedPanel;
    private string mainScene = "Main";
    bool exitOnce = true;
    private void OnEnable()
    {
        if (GameplayEntityLoader.instance)
        {
            GameplayEntityLoader.instance._uiReferences = this;
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
        if (XanaConstantsHolder.xanaConstants.EnviornmentName.Contains("XANA Lobby"))
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
        disableSoundXanalobby();
        XanaConstantsHolder.xanaConstants.isBackFromWorld = true;
        if (exitOnce)
        {
            exitOnce = false;
            if (XanaConstantsHolder.xanaConstants.isFromXanaLobby && !XanaConstantsHolder.xanaConstants.EnviornmentName.Contains("XANA Lobby"))
            {
                StartCoroutine(LobbySceneSwitch()); // to Lobby if player enter in world from Xana lobby
            }
            else
            {

                if (changeOritentationChange)
                {
                    XanaConstantsHolder.xanaConstants.JjWorldSceneChange = false;
                    XanaConstantsHolder.xanaConstants.orientationchanged = false;
                    XanaConstantsHolder.xanaConstants.mussuemEntry = JJMussuemEntry.Null;
                }
                if (GameManager.currentLanguage == "ja")
                {
                    LoadingController.Instance.UpdateLoadingStatusText("ホームに戻っています");
                }
                else if (GameManager.currentLanguage == "en")
                {
                    LoadingController.Instance.UpdateLoadingStatusText("Going Back to Home");
                }
                LoadingController.Instance.ShowLoading();
                //LoadingManager.Instance.ShowLoading(ScreenOrientation.LandscapeLeft);

                if (XanaConstantsHolder.xanaConstants.needToClearMemory)
                    AddressableDownloader.Instance.MemoryManager.RemoveAllAddressables();
                else
                    XanaConstantsHolder.xanaConstants.needToClearMemory = true;

                GC.Collect();
                AssetBundle.UnloadAllAssetBundles(true);
                Resources.UnloadUnusedAssets();
                StartCoroutine(LoadMainEnumerator());
            }
        }
    }
    private IEnumerator LobbySceneSwitch()
    {
        LoadingController.Instance.StartCoroutine(LoadingController.Instance.TeleportFader(FadeAction.In));
        if (!XanaConstantsHolder.xanaConstants.JjWorldSceneChange && !XanaConstantsHolder.xanaConstants.orientationchanged)
            Screen.orientation = ScreenOrientation.LandscapeLeft;

        yield return new WaitForSeconds(.2f);
        XanaConstantsHolder.xanaConstants.isBuilderScene = false;
        XanaConstantsHolder.xanaConstants.JjWorldSceneChange = true;
        XanaConstantsHolder.xanaConstants.JjWorldTeleportSceneName = "XANA Lobby";
        StartCoroutine(LoadMainEnumerator());
    }
    IEnumerator LoadMainEnumerator()
    {
        LeaveRoom();
        yield return new WaitForSeconds(.5f);
        if (XanaConstantsHolder.xanaConstants.museumAssetLoaded != null)
            XanaConstantsHolder.xanaConstants.museumAssetLoaded.Unload(true);
    }
    public void LoadWorld()
    {
        if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
        {
            UIHandler.Instance.LoginRegisterScreen.transform.SetAsLastSibling();
            UIHandler.Instance.LoginRegisterScreen.SetActive(true);
        }
        else
        {
            UIHandler.Instance.IsWorldClicked();
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
            UserAnalyticsManager.onUpdateWorldRelatedStats?.Invoke(false, false, false, true);
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
        XanaConstantsHolder.xanaConstants.CurrentSceneName = "Addressable";
        yield return new WaitForSeconds(.2f);
        Resources.UnloadUnusedAssets();
        print("mian scne " + mainScene);
        XanaConstantsHolder.xanaConstants.isBackFromWorld = true;
        if (XanaConstantsHolder.xanaConstants.JjWorldSceneChange)
        {
            float _rand;
            if (XanaConstantsHolder.xanaConstants.isBuilderScene)
                _rand = UnityEngine.Random.Range(25f, 30f);
            else
                _rand = UnityEngine.Random.Range(6f, 10f);
            LoadingController.Instance.randCurrentValue = _rand;
            StartCoroutine(LoadingController.Instance.IncrementSliderValue(_rand, true));
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene("Main");
        }
        else
        {
            if (XanaConstantsHolder.xanaConstants.isBuilderScene)
            {
                float _rand = UnityEngine.Random.Range(10f, 15f);
                LoadingController.Instance.randCurrentValue = _rand;
                StartCoroutine(LoadingController.Instance.IncrementSliderValue(_rand, true));
            }
            else
            {
                StartCoroutine(LoadingController.Instance.IncrementSliderValue(UnityEngine.Random.Range(6f, 10f), true));
            }
            yield return new WaitForSeconds(3f);
            SceneManager.LoadSceneAsync(mainScene);
        }
    }
}