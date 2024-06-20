﻿// --------------------------------------------------------------------------------------------------------------------
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
using System.Threading.Tasks;

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

        public static string CurrLobbyName, CurrRoomName;

        private RoomOptions roomOptions;
        private List<RoomInfo> availableRoomList=new List<RoomInfo>();
        public List<string> roomNames;
        public List<GameObject> playerobjects;

        public GameplayEntityLoader LFF;

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
        string gameVersion = "18";
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
                        //PhotonNetwork.ConnectUsingSettings();
                        //PhotonNetwork.GameVersion = this.gameVersion;
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
            // Seperate the live and test environment
            string _LobbyName = APIBasepointManager.instance.IsXanaLive ? ("Live" + ConstantsHolder.xanaConstants.EnviornmentName) : ("Test" + ConstantsHolder.xanaConstants.EnviornmentName);
            Debug.Log("Lobby Name: " + _LobbyName);
            Connect(_LobbyName);
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
        /// <summary>
        /// Start the connection process. 
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        /// 
        public void Connect(string lobbyN)
        {
            working = ScenesList.AddressableScene;
            CurrLobbyName = lobbyN;

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
                CurrLobbyName = deepLinkLobbyName;
            }
            if (PhotonNetwork.IsConnected)
            {
                JoinLobby(CurrLobbyName);
            }
            else
            {
                //Once it connected to server OnConnectedToMaster callback it sent from their we can join lobby.
                bool isConnected=PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = this.gameVersion;
                JoinLobby(CurrLobbyName);
            }
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
        }

        private async void JoinLobby(String lobbyName)
        {
            while(!PhotonNetwork.IsConnectedAndReady)
            await Task.Delay(1);
            PhotonNetwork.JoinLobby(new TypedLobby(lobbyName, LobbyType.Default));
        }

        public override void OnJoinedLobby()
        {
            Debug.LogError("On Joined lobby :- " + PhotonNetwork.CurrentLobby.Name+"--"+Time.time);
            CheckRoomAvailability();
        }

        public override void OnLeftLobby()
        {
            if (working == ScenesList.AddressableScene)
            {
                working = ScenesList.MainMenu;
            }
            playerobjects.Clear();
            availableRoomList.Clear();
            roomNames.Clear();
        }

        bool roomListUpdated = false;
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            availableRoomList = roomList;
            roomListUpdated = true;
        }

        async Task WaitUntilRoomListUpdated()
        {
            while (!roomListUpdated)
            {
                await Task.Delay(1000);
            }
        }

        async void CheckRoomAvailability()
        {
            await WaitUntilRoomListUpdated();
            if (ConstantsHolder.xanaConstants.isCameraMan)
            {
                JoinRoomForCameraMan();
            }
            else
            {
                JoinRoomCustom();
            }
        }


        void JoinRoomForCameraMan()
        {
            if (ConstantsHolder.xanaConstants.isCameraMan)
            {
                if (availableRoomList.Count > 0)
                {
                    List<RoomInfo> tempRooms = new List<RoomInfo>(availableRoomList);
                    tempRooms.Sort((a, b) => b.PlayerCount.CompareTo(a.PlayerCount));
                    PhotonNetwork.JoinRoom(tempRooms[0].Name);
                }
                else
                {
                    //there is no room for stremaing so move to main menu to switch other world
                    GameplayEntityLoader.instance._uiReferences.LoadMain(false);
                }
            }
        }

        private void JoinRoomCustom()
        {
            bool joinedRoom = false;
            if (availableRoomList.Count > 0)
                foreach (RoomInfo info in availableRoomList)
                {
                    roomNames.Add(info.Name);
                    Debug.LogError(info.PlayerCount+"--"+ info.MaxPlayers+"--"+info.Name);
                    if (info.PlayerCount < info.MaxPlayers)
                    {
                        CurrRoomName = info.Name;
                        joinedRoom = PhotonNetwork.JoinRoom(CurrRoomName);
                        return;
                    }
                }
            if (joinedRoom == false)
            {
                string roomName;
                do
                {
                    roomName = PhotonNetwork.CurrentLobby.Name + UnityEngine.Random.Range(0, 9999).ToString();
                }
                while (roomNames.Contains(roomName));

                PhotonNetwork.JoinOrCreateRoom(roomName, RoomOptionsRequest(), new TypedLobby(CurrLobbyName, LobbyType.Default));
            }
        }

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

        public override void OnCreatedRoom()
        {
            print("OnCreatedRoom called");
        }

        public override void OnJoinedRoom()
        {
            CurrRoomName = PhotonNetwork.CurrentRoom.Name;
            LFF.LoadFile();
        }
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (newPlayer.NickName == "XANA_XANA")
            {
                ConstantsHolder.xanaConstants.isCameraManInRoom = true;
            }
            if (ConstantsHolder.xanaConstants.isBuilderScene && ConstantsHolder.xanaConstants.isXanaPartyWorld)
                GamificationComponentData.instance.StartXANAPartyRace();
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

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            GameplayEntityLoader.instance._uiReferences.LoadMain(true);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {

        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            playerobjects.Clear();
        }

        public void Disconnect()
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LeaveLobby();
            UserAnalyticsHandler.onUpdateWorldRelatedStats?.Invoke(false, false, false, true);
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            //if (ConstantsHolder.xanaConstants.isBuilderScene)
            //    GamificationComponentData.instance.MasterClientSwitched(newMasterClient);
        }
        #endregion


        public void JoinRoomManually(string name)
        {
            PhotonNetwork.JoinRoom(name);
        }

        public void CreateGameRoom(string game){
            string roomName;
            roomName = game + UnityEngine.Random.Range(0, 9999).ToString();
            print("newly "+game + " : " + roomName);
            PhotonNetwork.CreateRoom(roomName, RoomOptionsRequest(), new TypedLobby(CurrLobbyName, LobbyType.Default));  
        }

    }
}
