using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;

/// <summary> this script is Handling Build in avatar models data and spawning  avatar buttons on  Ui.</summary>/// 
public class AvatarSpawnerOnDisconnect : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public GameObject currentDummyPlayer;

    // Start is called before the first frame update
    public GameObject spawnPoint;
    public GameObject JoinCurrentRoomPanel;
    public Sprite FavouriteAnimationSprite;
    public Sprite NormalAnimationSprite;


    public static event Action OninternetDisconnect;
    public static event Action OninternetConnected;

    public static AvatarSpawnerOnDisconnect Instance;

    private GameObject InternetLost;

    private void Awake()
    {
        Instance = this;
    }


    private void OnApplicationQuit()
    {
        PhotonNetwork.Destroy(currentDummyPlayer);
        PhotonNetwork.LeaveRoom(false);
        PhotonNetwork.LeaveLobby();
        UserAnalyticsHandler.onUpdateWorldRelatedStats?.Invoke(false, false, false, true);
    }

    public void ShowJoinRoomPanel()
    {
        //InternetLost = null;
        if (InternetLost == null)
        {
            ConstantsHolder.xanaConstants.needToClearMemory = false;
            if (LoadingHandler.Instance)
                LoadingHandler.Instance.HideLoading();
            GameObject go = Instantiate(JoinCurrentRoomPanel) as GameObject;
            InternetLost = go;
        }

        if (LoadingHandler.Instance != null && !LoadingHandler.Instance.gameObject.transform.GetChild(0).gameObject.activeInHierarchy)
        {
            //PlayerCameraController.instance.DisAllowControl();
            OffSelfie();
            TurnCameras(false);
            //Instantiate(JoinCurrentRoomPanel);
        }

    }
    public void InstantiatePlayerAgain()
    {
        StartCoroutine(MainReconnect());
    }

    private void OffSelfie()
    {
        PlayerSelfieController.Instance.SwitchFromSelfieControl();
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

    RoomOptions roomOptions;


    private IEnumerator MainReconnect()
    {
        while (PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState != ExitGames.Client.Photon.PeerStateValue.Disconnected)
        {
            //Debug.Log("Waiting for client to be fully disconnected..", this);
            yield return new WaitForSeconds(0.2f);
        }

        string lastRoomName = Photon.Pun.Demo.PunBasics.MutiplayerController.CurrRoomName;
        if (!PhotonNetwork.ReconnectAndRejoin())
        {
            if (PhotonNetwork.RejoinRoom(lastRoomName))
            {
                Debug.Log(" Successful reconnected!", this);
            }
        }
        else
        {
           /* PhotonNetwork.AutomaticallySyncScene = true;                      //Zeel Removed this cusing sector issue
            roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 20;
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;
            roomOptions.PublishUserId = true;
            roomOptions.CleanupCacheOnLeave = true;*/
            PhotonNetwork.JoinOrCreateRoom(lastRoomName,MutiplayerController.instance.roomOptions, new TypedLobby(Photon.Pun.Demo.PunBasics.MutiplayerController.CurrLobbyName, LobbyType.Default), null);
        }

    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        if (OninternetDisconnect != null)
            OninternetDisconnect.Invoke();
        ShowJoinRoomPanel();
    }


    public override void OnConnected()
    {
        if (OninternetConnected != null)
            OninternetConnected.Invoke();
    }

    void OnApplicationFocus(bool isGameFocus)
    {
        // User Analatics 
        if (!SceneManager.GetActiveScene().name.Contains("Home"))
        {
            //UserAnalyticsHandler.onUserJoinedLeaved?.Invoke(isGameFocus);
            if (isGameFocus)
            {
                UserAnalyticsHandler.onUpdateWorldStatCustom?.Invoke(true, false);
            }
            else
            {
                UserAnalyticsHandler.onUpdateWorldRelatedStats?.Invoke(false, false, false, true);
            }
        }

    }
}