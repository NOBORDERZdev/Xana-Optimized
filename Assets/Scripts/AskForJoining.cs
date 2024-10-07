using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun;
using UnityEngine.UI;
using Metaverse;
using UnityEngine.SceneManagement;

public class AskForJoining : MonoBehaviour
{
    //  public GameObject ss;
    // Start is called before the first frame update
    // Start is called before the first frame update

    AsyncOperation asyncLoading;
    private PlayerCameraController[] _cameraLooks;



    private void Awake()
    {
        _cameraLooks = FindObjectsOfType<PlayerCameraController>();

    }


    //void LoadMain()
    //{
    //    ConstantsHolder.xanaConstants.isFromXanaLobby =false;
    //    ConstantsHolder.xanaConstants.JjWorldSceneChange = false;

    //    float _rand = UnityEngine.Random.Range(6f, 10f);
    //    LoadingHandler.Instance.randCurrentValue = _rand;
    //    StartCoroutine(LoadingHandler.Instance.IncrementSliderValue(_rand, true));
    //    ConstantsHolder.xanaConstants.isBackFromWorld = true;  
    //    LoadingHandler.Instance.ShowLoading();
    //    print("Hello Ask to Join");
    //    //string a = TextLocalization.GetLocaliseTextByKey("Going Back to Home");
    //    //LoadingHandler.Instance.UpdateLoadingStatusText("Going Back to Home");
    //    if (GameManager.currentLanguage == "ja")
    //    {
    //        LoadingHandler.Instance.UpdateLoadingStatusText("ホームに戻っています");
    //    }
    //    else if (GameManager.currentLanguage == "en")
    //    {
    //        LoadingHandler.Instance.UpdateLoadingStatusText("Going Back to Home");
    //    }
    //    asyncLoading = SceneManager.LoadSceneAsync("Home");
    //    //InvokeRepeating("AsyncProgress", 0.1f, 0.1f);

    //    // Connection Lost Going To Main Update User Count
    //    UserAnalyticsHandler.onUpdateWorldRelatedStats(false, false, false, true);
    //}

    void AsyncProgress()
    {
        //LoadingHandler.Instance.UpdateLoadingSlider(asyncLoading.progress * 1.1f);
    }

    public void GoToMainMenu()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
           // LoadMain();
            GameplayEntityLoader.instance._uiReferences.LoadMain(true);
            TurnCameras(true);
            try
            {
                ChracterPosition.currSpwanPos = "";
            }
            catch (Exception e)
            {
                Debug.LogError("error :--- chracterposition script not found handled for tif2021.");
            }



            Destroy(this.gameObject);
        }
    }
    public void joinCurrentRoom()
    {
        Debug.Log("Joining Room");
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            // Handle the case where the internet is not reachable
            Debug.Log("Internet not reachable.");
            return;
        }
        else
        {
            if (ReferencesForGamePlay.instance != null)
            {
                ReferencesForGamePlay.instance.workingCanvas.SetActive(false);
            }

            float _rand = UnityEngine.Random.Range(6f, 10f);
            LoadingHandler.Instance.randCurrentValue = _rand;
            StartCoroutine(LoadingHandler.Instance.IncrementSliderValue(_rand, true));

            ConstantsHolder.xanaConstants.EnviornmentName = WorldItemView.m_EnvName;
            //LoadingHandler.Instance.ShowFadderWhileOriantationChanged(ScreenOrientation.LandscapeLeft);
            LoadingHandler.Instance.ShowLoading();
            LoadingHandler.Instance.UpdateLoadingSlider(0.98f);
            LoadingHandler.Instance.UpdateLoadingStatusText("Loading World");

            MutiplayerController.instance.Connect(MutiplayerController.CurrLobbyName);
            AvatarSpawnerOnDisconnect.Instance.InstantiatePlayerAgain();
            BuilderEventManager.ResetComponentUI?.Invoke(Constants.ItemComponentType.none);
            TurnCameras(true);

            if (ScreenOrientationManager._instance != null && ScreenOrientationManager._instance.isPotrait)
            {
                ScreenOrientationManager._instance.MyOrientationChangeCode(DeviceOrientation.LandscapeLeft);
            }

            Destroy(this.gameObject);
        }
    }


    private void TurnCameras(bool active)
    {
        if (active)
        {
            PlayerCameraController.instance.AllowControl();
        }
        else
        {
            PlayerCameraController.instance.DisAllowControl();
        }
    }
}

