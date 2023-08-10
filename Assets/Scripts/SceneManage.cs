using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun;
using Metaverse;
using System.Collections;
using System;
using System.IO;

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

    private AsyncOperation asyncLoading;

    bool exitOnce = true;


    private void OnEnable()
    {

        if (SceneManager.GetActiveScene().name == "Main")
        {
            AvatarManager.sendDataValue = false;
        }
        if (LoadFromFile.instance)
        {
            LoadFromFile.instance._uiReferences = this;
        }
    }

    private void OnDisable()
    {
        // AssetBundle.UnloadAllAssetBundles(false);
        Resources.UnloadUnusedAssets();
        // Caching.ClearCache();
    }

    private void OnDestroy()
    {
        Resources.UnloadUnusedAssets();
        //  Caching.ClearCache();
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

    public void LoadMain(bool changeOritentationChange)
    {
        if (exitOnce)
        {
            exitOnce = false;
            //if ( !XanaConstants.xanaConstants.JjWorldSceneChange && !XanaConstants.xanaConstants.orientationchanged)
            //    Screen.orientation = ScreenOrientation.LandscapeLeft;
            if (XanaConstants.xanaConstants.isFromXanaLobby && !XanaConstants.xanaConstants.EnviornmentName.Contains("XANA Lobby"))
            {
                StartCoroutine(LobbySceneSwitch()); // to Lobby if player enter in world from Xana lobby
            }
            else
            {

                if (changeOritentationChange)
                {
                    Screen.orientation = ScreenOrientation.LandscapeLeft;
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
                Debug.Log("~~~~~~ LoadMain call");

                LoadingHandler.Instance.ShowLoading();

                GC.Collect();
                AssetBundle.UnloadAllAssetBundles(true);
                Resources.UnloadUnusedAssets();

                // Added By WaqasAhmad [20 July 23]
                //Caching.ClearCache();
                //

                //   Caching.ClearCache();
                StartCoroutine(LoadMainEnumerator());
            }

        }

    }

    private IEnumerator LobbySceneSwitch()
    {
        LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
        if (!XanaConstants.xanaConstants.JjWorldSceneChange && !XanaConstants.xanaConstants.orientationchanged)
            Screen.orientation = ScreenOrientation.LandscapeLeft;

        yield return new WaitForSeconds(1f);
        XanaConstants.xanaConstants.isBuilderScene = false;
        XanaConstants.xanaConstants.JjWorldSceneChange = true;
        XanaConstants.xanaConstants.JjWorldTeleportSceneName = "XANA Lobby";
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
            PhotonNetwork.DestroyAll(true);
            UserAnalyticsHandler.onUpdateWorldRelatedStats?.Invoke(false, false, false, true);
            Debug.Log("Exit: Api Called");
        }
        StartSceneLoading();
    }

    public void StartSceneLoading()
    {
        print("Hello Scene Manager");
        // string unit = "Going Back to Home";
        //string a= TextLocalization.GetLocaliseTextByKey();
        //LoadingHandler.Instance.UpdateLoadingStatusText("Going Back to Home");
        //asyncLoading = SceneManager.LoadSceneAsync(mainScene);
        //InvokeRepeating("AsyncProgress", 0.1f, 0.1f);
        StartCoroutine(LoadMianScene());
    }

    /// <summary>
    /// To load main scene 
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadMianScene()
    {
        yield return new WaitForSeconds(.2f);
        //yield return new WaitForSeconds(.4f);
        LoadingHandler.Instance.UpdateLoadingSlider(0.3f);
        yield return new WaitForSeconds(.4f);
        //yield return new WaitForSeconds(.6f);
        LoadingHandler.Instance.UpdateLoadingSlider(0.6f);
        print("loading mainmenu");

        Resources.UnloadUnusedAssets();
        //  Caching.ClearCache();
        // GC.Collect();
        print("mian scne " + mainScene);
        if (XanaConstants.xanaConstants.JjWorldSceneChange)
        {
            SceneManager.LoadScene("Main");
        }
        else
        {
            SceneManager.LoadScene(mainScene);
        }
    }
    void AsyncProgress()
    {
        LoadingHandler.Instance.UpdateLoadingSlider(asyncLoading.progress * 1.1f);
    }

    //public void Dispose()
    //{
    //    // Dispose of unmanaged resources.
    //    Dispose(true);
    //    // Suppress finalization.
    //    GC.SuppressFinalize(this);
    //}
    //protected virtual void Dispose(bool disposing)
    //{
    //    if (!_disposedValue)
    //    {
    //        if (disposing)
    //        {
    //            // TODO: dispose managed state (managed objects)
    //        }

    //        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
    //        // TODO: set large fields to null
    //        _disposedValue = true;
    //    }
    //}
}
