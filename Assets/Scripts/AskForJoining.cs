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
    private CameraLook[] _cameraLooks;



    private void Awake()
    {
        _cameraLooks = FindObjectsOfType<CameraLook>();

    }


    void LoadMain()
    {
        XanaConstantsHolder.xanaConstants.isFromXanaLobby =false;
        XanaConstantsHolder.xanaConstants.JjWorldSceneChange = false;

        float _rand = UnityEngine.Random.Range(6f, 10f);
        LoadingController.Instance.randCurrentValue = _rand;
        StartCoroutine(LoadingController.Instance.IncrementSliderValue(_rand, true));
        XanaConstantsHolder.xanaConstants.isBackFromWorld = true;  
        LoadingController.Instance.ShowLoading();
        print("Hello Ask to Join");
        //string a = UITextLocalization.GetLocaliseTextByKey("Going Back to Home");
        //LoadingController.Instance.UpdateLoadingStatusText("Going Back to Home");
        if (GameManager.currentLanguage == "ja")
        {
            LoadingController.Instance.UpdateLoadingStatusText("ホームに戻っています");
        }
        else if (GameManager.currentLanguage == "en")
        {
            LoadingController.Instance.UpdateLoadingStatusText("Going Back to Home");
        }
        asyncLoading = SceneManager.LoadSceneAsync("Main");
        //InvokeRepeating("AsyncProgress", 0.1f, 0.1f);

        // Connection Lost Going To Main Update User Count
        UserAnalyticsManager.onUpdateWorldRelatedStats(false, false, false, true);
    }

    void AsyncProgress()
    {
        //LoadingController.Instance.UpdateLoadingSlider(asyncLoading.progress * 1.1f);
    }

    public void GoToMainMenu()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            LoadMain();
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
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {

            return;
        }
        else
        {
            if (ReferrencesForDynamicMuseum.instance != null)
                ReferrencesForDynamicMuseum.instance.workingCanvas.SetActive(false);

            float _rand = UnityEngine.Random.Range(6f, 10f);
            LoadingController.Instance.randCurrentValue = _rand;
            StartCoroutine(LoadingController.Instance.IncrementSliderValue(_rand, true));

            LoadingController.Instance.ShowLoading();
            if (ChangeOrientation_waqas._instance != null && ChangeOrientation_waqas._instance.isPotrait)
            {
                ChangeOrientation_waqas._instance.MyOrientationChangeCode(DeviceOrientation.LandscapeLeft);
            }

            //LoadingController.Instance.UpdateLoadingSlider(0.5f);
            Launcher.instance.Connect(Launcher.instance.lastLobbyName);
            AvatarManager.Instance.InstantiatePlayerAgain();
            BuilderEventManager.ResetComponentUI?.Invoke(Constants.ItemComponentType.none);
            TurnCameras(true);
            Destroy(this.gameObject);

        }
    }


    private void TurnCameras(bool active)
    {
        if (active)
        {
            CameraLook.instance.AllowControl();
        }
        else
        {
            CameraLook.instance.DisAllowControl();
        }
    }
}

