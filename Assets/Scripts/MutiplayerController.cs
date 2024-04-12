// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MutiplayerController.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in "PUN Basic tutorial" to connect, and join/create room automatically
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using Metaverse;
using System.Collections;
using System.Linq;

namespace Photon.Pun.Demo.PunBasics
{
    public enum ServerConnectionStates { ConnectedToServer, NotConnectedToServer, ConnectingToServer, FailedToConnectToServer }
    public enum NetworkStates { ConnectedToInternet, NotConnectedToInternet }
    public enum MatchMakingStates { InLobby, InRoom, NoState }
    public enum ScenesList { MainMenu, AddressableScene }
#pragma warning disable 649

    /// <summary>
    /// Launch manager. Connect, join a random room or create one if none or all full.
    /// </summary>
    public class MutiplayerController : MonoBehaviourPunCallbacks
    {
        public ServerConnectionStates connectionState = ServerConnectionStates.NotConnectedToServer;
        public MatchMakingStates matchMakingState = MatchMakingStates.NoState;
        public NetworkStates internetState = NetworkStates.NotConnectedToInternet;

        public static MutiplayerController instance;
        public ScenesList working;
        #region Private Serializable Fields
        [Tooltip("The maximum number of players per room")]
        [SerializeField]
        byte maxPlayersPerRoom = 20;

        [HideInInspector]
        public RoomOptions roomOptions;

        public GameplayEntityLoader LFF;
        public List<GameObject> playerobjects;
        public static string sceneName;
        string lobbyName;

        #endregion

        #region Private Fields
        /// <summary>
        /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon, 
        /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
        /// Typically this is used for the OnConnectedToMaster() callback.
        /// </summary>
        public bool isConnecting;

        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        string gameVersion = "12";
        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                working = ScenesList.MainMenu;
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    internetState = NetworkStates.NotConnectedToInternet;
                    StartCoroutine(WaitForInternetToConnect());
                }
                else
                {
                    internetState = NetworkStates.ConnectedToInternet;
                    if (connectionState == ServerConnectionStates.NotConnectedToServer)
                    {
                        connectionState = ServerConnectionStates.ConnectingToServer;
                        PhotonNetwork.ConnectUsingSettings();
                        PhotonNetwork.GameVersion = this.gameVersion;
                    }
                }
                // #Critical
                // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
                PhotonNetwork.AutomaticallySyncScene = true;
            }
            else
            {
                DestroyImmediate(this);
            }
        }
        private void Start()
        {
            Connect(ConstantsHolder.xanaConstants.EnviornmentName);
        }
        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>

        IEnumerator WaitForInternetToConnect()
        {
            yield return new WaitForSeconds(1f);
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                internetState = NetworkStates.NotConnectedToInternet;
            }
            else
            {
                internetState = NetworkStates.ConnectedToInternet;
            }
        }

        #endregion
        #region Public Methods
        public void SetMaxPlayer(int max)
        {
            maxPlayersPerRoom = (byte)max;
        }
        /// <summary>
        /// Start the connection process. 
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        /// 
        public void Connect(string lobbyN)
        {
            if (isConnecting)
                return;
            working = ScenesList.AddressableScene;
            lastSceneName = SceneManager.GetActiveScene().name;
            lastLobbyName = lobbyN;

            if (!PlayerPrefs.GetString(ConstantsGod.PLAYERNAME).Contains("ゲスト") &&
                    !PlayerPrefs.GetString(ConstantsGod.PLAYERNAME).Contains("Guest") && !string.IsNullOrEmpty(PlayerPrefs.GetString(ConstantsGod.PLAYERNAME)))
            {
                string guidAsString = PlayerPrefs.GetString(ConstantsGod.PLAYERNAME);
                PhotonNetwork.NickName = guidAsString;
            }
            else
            {
                PhotonNetwork.NickName = "Guest";
            }
            if (XanaEventDetails.eventDetails.DataIsInitialized)
            {
                string deepLinkLobbyName = $"{XanaEventDetails.eventDetails.eventType}{XanaEventDetails.eventDetails.id}";
                lobbyName = deepLinkLobbyName;
            }
            else
            {
                lobbyName = lobbyN;
            }
            sceneName = SceneManager.GetActiveScene().name;
            PlayerPrefs.SetString("loadscene", SceneManager.GetActiveScene().name);
            PlayerPrefs.SetString("lb", lobbyN);
            PlayerPrefs.Save();
            // we want to make sure the log is clear everytime we connect, we might have several failed attempted if connection failed.
            // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
            isConnecting = true;
            // hide the Play button for visual consistency
            // start the loader animation for visual effect.
            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                isConnecting = false;
                PhotonNetwork.JoinLobby(new TypedLobby(lobbyName, LobbyType.Default));
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = this.gameVersion;
            }

            SetMaxPlayer(int.Parse(ConstantsHolder.xanaConstants.userLimit));
            //SetMaxPlayer(10);
        }

        void LogFeedback(string message)
        {
            // we do not assume there is a feedbackText defined.
        }
        #endregion
        #region MonoBehaviourPunCallbacks CallBacks
        // below, we implement some callbacks of PUN
        // you can find PUN's callbacks in the class MonoBehaviourPunCallbacks

        /// <summary>
        /// Called after the connection to the master is established and authenticated
        /// </summary>
        public override void OnConnectedToMaster()
        {
            connectionState = ServerConnectionStates.ConnectedToServer;
            if (working == ScenesList.MainMenu)
                return;
            // we don't want to do anything if we are not attempting to join a room. 
            // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
            // we don't want to do anything.
            PhotonNetwork.JoinLobby(new TypedLobby(lobbyName, LobbyType.Default));
            if (isConnecting)
            {
                LogFeedback("OnConnectedToMaster: Next -> try to Join Random Room");
            }


        }

        public override void OnJoinedLobby()
        {

        }
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            GameplayEntityLoader.instance._uiReferences.LoadMain(true);
        }
        public override void OnCreatedRoom()
        {
            print("OnCreatedRoom called");
        }
        public override void OnLeftLobby()
        {
            if (working == ScenesList.AddressableScene)
            {
                working = ScenesList.MainMenu;
            }
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            bool joinedRoom = false;
            string CameraManRoomName = null;
            foreach (RoomInfo info in roomList)
            {
                int maxPlayer;
                if (ConstantsHolder.xanaConstants.EnviornmentName == "Xana Festival") // to reserve the place for camera man (Show room is full to other players)
                {
                    maxPlayer = info.MaxPlayers - 1;
                }
                else
                {
                    maxPlayer = info.MaxPlayers;
                }
                if (info.PlayerCount < maxPlayer)
                {
                    lastRoomName = info.Name;
                    if (!ConstantsHolder.xanaConstants.isCameraMan)
                    {
                        PhotonNetwork.JoinRoom(lastRoomName);
                        joinedRoom = true;
                        break;
                    }
                }
            }
            if (ConstantsHolder.xanaConstants.isCameraMan)
            {
                if (roomList.Count > 0)
                {
                    List<RoomInfo> tempRooms = new List<RoomInfo>(roomList);
                    tempRooms.Sort((a, b) => b.PlayerCount.CompareTo(a.PlayerCount));
                    CameraManRoomName = tempRooms[0].Name;
                }
                else
                {
                    // there is no room for stremaing so move to main menu to switch other world
                    GameplayEntityLoader.instance._uiReferences.LoadMain(false);
                }
            }

            if (joinedRoom == false)
            {
                string temp;
                do
                {
                    temp = PhotonNetwork.CurrentLobby.Name + UnityEngine.Random.Range(0, 9999).ToString();
                } 
                while (roomNames.Contains(temp));
                if (!ConstantsHolder.xanaConstants.isCameraMan)
                    PhotonNetwork.JoinOrCreateRoom(temp, RoomOptionsRequest(), new TypedLobby(lobbyName, LobbyType.Default), null);
                else
                {
                    if (!CameraManRoomName.IsNullOrEmpty())
                        PhotonNetwork.JoinRoom(CameraManRoomName);
                }
            }
        }

        public List<string> roomNames;

        public RoomOptions RoomOptionsRequest()
        {
            roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = (byte)(int.Parse(ConstantsHolder.xanaConstants.userLimit));
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;

            roomOptions.PublishUserId = true;
            roomOptions.CleanupCacheOnLeave = true;
            return roomOptions;
        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            if (!ConstantsHolder.xanaConstants.isCameraMan)
            {
                PhotonNetwork.CreateRoom(null, RoomOptionsRequest(), new TypedLobby(lobbyName, LobbyType.Default), null);
            }
        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            playerobjects.Clear();
            LogFeedback("<Color=Red>OnDisconnected</Color> " + cause);
            PlayerPrefs.SetInt("leftRoom", 1);
            // #Critical: we failed to connect or got disconnected. There is not much we can do. Typically, a UI system should be in place to let the user attemp to connect again.
            isConnecting = false;
        }



        public override void OnJoinedRoom()
        {
            lastRoomName = PhotonNetwork.CurrentRoom.Name;

            if (PhotonNetwork.CurrentRoom.PlayerCount >= 1)
            {
                PlayerPrefs.SetString("roomname", PhotonNetwork.CurrentRoom.Name);
                PlayerPrefs.Save();
            }
            if (!(SceneManager.GetActiveScene().name == "GamePlayScene") || !(SceneManager.GetActiveScene().name.Contains("Museum")))
            {
                AvatarSpawnerOnDisconnect.Instance.InitCharacter();
            }
            else
            {
                Application.runInBackground = true;
            }
            if (SceneManager.GetActiveScene().name.Contains("Museum"))
            {
                StartCoroutine(LFF.SpawnPlayer());
            }

            else
            {
                LFF.LoadFile();
            }
        }
        public void Disconnect()
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LeaveLobby();
            UserAnalyticsHandler.onUpdateWorldRelatedStats?.Invoke(false, false, false, true);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (newPlayer.NickName == "XANA_XANA")
            {
                ConstantsHolder.xanaConstants.isCameraManInRoom = true;
            }
        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (otherPlayer.NickName == "XANA_XANA")
            {
                ConstantsHolder.xanaConstants.isCameraManInRoom = false;
            }
            for (int x = 0; x < playerobjects.Count; x++)
            {
                if (otherPlayer.ActorNumber == playerobjects[x].GetComponent<PhotonView>().OwnerActorNr)
                {
                    playerobjects.RemoveAt(x);
                }
            }
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (ConstantsHolder.xanaConstants.isBuilderScene)
                GamificationComponentData.instance.MasterClientSwitched(newMasterClient);
        }
        #endregion
        public string lastSceneName, lastLobbyName, lastRoomName;

        public void JoinRoomManually(string name)
        {
            PhotonNetwork.JoinRoom(name);
        }

    }
}
