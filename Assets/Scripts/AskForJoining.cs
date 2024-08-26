using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun;
using UnityEngine.UI;
using Metaverse;
using UnityEngine.SceneManagement;
using Photon.Realtime;

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


    void LoadMain()
    {
        ConstantsHolder.xanaConstants.isFromXanaLobby =false;
        ConstantsHolder.xanaConstants.JjWorldSceneChange = false;

        float _rand = UnityEngine.Random.Range(6f, 10f);
        LoadingHandler.Instance.randCurrentValue = _rand;
        StartCoroutine(LoadingHandler.Instance.IncrementSliderValue(_rand, true));
        ConstantsHolder.xanaConstants.isBackFromWorld = true;  
        LoadingHandler.Instance.ShowLoading();
        print("Hello Ask to Join");
        //string a = TextLocalization.GetLocaliseTextByKey("Going Back to Home");
        //LoadingHandler.Instance.UpdateLoadingStatusText("Going Back to Home");
        if (GameManager.currentLanguage == "ja")
        {
            LoadingHandler.Instance.UpdateLoadingStatusText("ホームに戻っています");
        }
        else if (GameManager.currentLanguage == "en")
        {
            LoadingHandler.Instance.UpdateLoadingStatusText("Going Back to Home");
        }
        asyncLoading = SceneManager.LoadSceneAsync("Home");
        //InvokeRepeating("AsyncProgress", 0.1f, 0.1f);

        // Connection Lost Going To Main Update User Count
        UserAnalyticsHandler.onUpdateWorldRelatedStats(false, false, false, true);
    }

    void AsyncProgress()
    {
        //LoadingHandler.Instance.UpdateLoadingSlider(asyncLoading.progress * 1.1f);
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
    private string lastRoomName;
    public void joinCurrentRoom()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {

            return;
        }
        else
        {
            if (ReferencesForGamePlay.instance != null)
                ReferencesForGamePlay.instance.workingCanvas.SetActive(false);

            float _rand = UnityEngine.Random.Range(6f, 10f);
            LoadingHandler.Instance.randCurrentValue = _rand;
            StartCoroutine(LoadingHandler.Instance.IncrementSliderValue(_rand, true));

            LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
            LoadingHandler.Instance.ShowLoading();

            if (ScreenOrientationManager._instance != null && ScreenOrientationManager._instance.isPotrait)
            {
                ScreenOrientationManager._instance.MyOrientationChangeCode(DeviceOrientation.LandscapeLeft);
            }

            //LoadingHandler.Instance.UpdateLoadingSlider(0.5f);
            lastRoomName = PlayerPrefs.GetString("roomname"); // Store the current room name
            // Connect to the Master Server if not connected
            
            StartCoroutine(AttemptRejoin());

        }
    }

    private IEnumerator AttemptRejoin()
    {

        while (!PhotonNetwork.IsConnected || PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer)
        {
            PhotonNetwork.ConnectUsingSettings(); // Automatically connect to the Master Server
            yield return new WaitForSeconds(0.2f);
        }
        if (!string.IsNullOrEmpty(lastRoomName))
        {
            if (ConstantsHolder.xanaConstants.isJoinigXanaPartyGame)
            {
                bool rejoinSuccessful = PhotonNetwork.RejoinRoom(lastRoomName);
                if (!rejoinSuccessful)
                {
                    Debug.LogWarning("Rejoin failed, attempting to join a new room.");
                    HandleFailedRejoin();
                    yield break;
                }
            }
            else if (ConstantsHolder.xanaConstants.isXanaPartyWorld)
            {
                MutiplayerController.instance.Connect(MutiplayerController.CurrLobbyName);
                AvatarSpawnerOnDisconnect.Instance.InstantiatePlayerAgain();
            }
        }
        else
        {
            // No last room, join a new room
            MutiplayerController.instance.Connect(MutiplayerController.CurrLobbyName);
        }
        BuilderEventManager.ResetComponentUI?.Invoke(Constants.ItemComponentType.none);
        TurnCameras(true);
        if (ConstantsHolder.xanaConstants.isXanaPartyWorld && ConstantsHolder.xanaConstants.isJoinigXanaPartyGame
            && XANAPartyManager.Instance.GetComponent<PenpenzLpManager>().isLeaderboardShown)
        {
            StartCoroutine(MovePlayerToNextGameOnReconnection());
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void HandleFailedRejoin()
    {
        // Handle what happens if rejoining a room fails
        MutiplayerController.instance.Connect(MutiplayerController.CurrLobbyName);
        Debug.Log("Connected to a new room after rejoin failed.");
    }

    IEnumerator MovePlayerToNextGameOnReconnection()
    {
        while (GameplayEntityLoader.instance == null || GameplayEntityLoader.instance.PenguinPlayer == null)
        {
            yield return new WaitForSeconds(0.2f);
        }
        GameplayEntityLoader.instance.PenguinPlayer.GetComponent<PhotonView>().RPC("MovePlayerToNextGameOnReconnect", RpcTarget.AllBuffered);
        Destroy(this.gameObject);
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

