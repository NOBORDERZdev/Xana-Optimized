using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;

namespace Metaverse
{
    /// <summary> this script is Handling Build in avatar models data and spawning  avatar buttons on  Ui.</summary>/// 
    public class AvatarSpawnerOnDisconnect : MonoBehaviourPunCallbacks
    {
        private GameObject InternetLost;
        public RuntimeAnimatorController Defaultanimator;

        // Start is called before the first frame update
        public GameObject spawnPoint;

        public GameObject currentDummyPlayer;

        public GameObject JoinCurrentRoomPanel;

        public static event Action OninternetDisconnect;
        public static event Action OninternetConnected;

        public Sprite FavouriteAnimationSprite;
        public Sprite NormalAnimationSprite;

        public static AvatarSpawnerOnDisconnect Instance;
        private void Awake()
        {
            Instance = this;
        }

        private void OnApplicationQuit()
        {
            PhotonNetwork.Destroy(currentDummyPlayer);
            HomeSceneLoader.callRemove = true;
            PhotonNetwork.LeaveRoom(false);
            PhotonNetwork.LeaveLobby();
            UserAnalyticsManager.onUpdateWorldRelatedStats?.Invoke(false, false, false, true);
        }

        public void ShowJoinRoomPanel()
        {
            if (InternetLost == null)
            {
                ConstantsHolder.xanaConstants.needToClearMemory = false;
                if (LoadingController.Instance)
                    LoadingController.Instance.HideLoading();
                GameObject go = Instantiate(JoinCurrentRoomPanel) as GameObject;
                InternetLost = go;
            }

            if (LoadingController.Instance != null && !LoadingController.Instance.gameObject.transform.GetChild(0).gameObject.activeInHierarchy)
            {
                OffSelfie();
                TurnCameras(false);
            }

        }
        public void InstantiatePlayerAgain()
        {
            StartCoroutine(MainReconnect());
        }

        public void InitCharacter()
        {
            if (Instance == null)
            {
                Instance = this;
            }
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
                yield return new WaitForSeconds(0.2f);
            }

            string lastRoomName = PlayerPrefs.GetString("roomname");
            if (!PhotonNetwork.ReconnectAndRejoin())
            {
                if (PhotonNetwork.RejoinRoom(lastRoomName))
                {
                    Debug.Log(" Successful reconnected!", this);
                }
            }
            else
            {
                Debug.Log("Successful reconnected and joined!", this);
                PhotonNetwork.AutomaticallySyncScene = true;
                roomOptions = new RoomOptions();
                roomOptions.MaxPlayers = 20;
                roomOptions.IsOpen = true;
                roomOptions.IsVisible = true;
                roomOptions.PublishUserId = true;
                roomOptions.CleanupCacheOnLeave = true;
                PhotonNetwork.JoinOrCreateRoom(PlayerPrefs.GetString("roomname"), roomOptions, new TypedLobby(PlayerPrefs.GetString("lb"), LobbyType.Default), null);
                Invoke("ChangeButtonCommit", 2);
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
            if (!SceneManager.GetActiveScene().name.Contains("Main"))
            {
                //UserAnalyticsHandler.onUserJoinedLeaved?.Invoke(isGameFocus);
                if (isGameFocus)
                {
                    UserAnalyticsManager.onUpdateWorldStatCustom?.Invoke(true, false);
                }
                else
                {
                    UserAnalyticsManager.onUpdateWorldRelatedStats?.Invoke(false, false, false, true);
                }
            }

        }

        private void SetLayerRecursively(GameObject Parent, int Layer)
        {
            Parent.layer = Layer;
            foreach (Transform child in Parent.transform)
            {
                SetLayerRecursively(child.gameObject, Layer);
            }
        }
    }




}
[System.Serializable]
public class AvatarData
{
    public GameObject prefab;
    public RuntimeAnimatorController Controller;
    public int ModelNumber = 6;
    public Sprite avatarImage;
    public bool isHost;
}