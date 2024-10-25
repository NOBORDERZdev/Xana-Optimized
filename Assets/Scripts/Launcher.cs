// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Launcher.cs" company="Exit Games GmbH">
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
    public class Launcher : MonoBehaviourPunCallbacks
    {
        public ServerConnectionStates connectionState = ServerConnectionStates.NotConnectedToServer;
        public MatchMakingStates matchMakingState = MatchMakingStates.NoState;
        public NetworkStates internetState = NetworkStates.NotConnectedToInternet;

        public static Launcher instance;
        public ScenesList working;
        #region Private Serializable Fields
        [Tooltip("The maximum number of players per room")]
        [SerializeField]
        byte maxPlayersPerRoom = 20;

        public static bool isRoom = false;
        public static bool isLoading = false;
        [HideInInspector]
        public LoadingManager Loader;
        public RoomOptions roomOptions;

        public bool lobbyJoined, roomJoined, movingToScene;

        public LoadFromFile LFF;
        public List<GameObject> playerobjects;
        public static string sceneName;
        string lobbyName;
        private bool currentRoom = false;

        #endregion

        [Header("Population Elements")]
        public GameObject[] populationPrefab;

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
		string gameVersion = "11";
		private int count;


        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
		{
			print("Launcher "+ "Awake");
			//if(SceneManager.GetActiveScene().name != "AddressableScene")//AddressableScene
			//XanaConstants.xanaConstants.EnviornmentName = SceneManager.GetActiveScene().name;
			//print("Envirornment "+XanaConstants.xanaConstants.EnviornmentName);
			if (instance == null)
			{
				instance = this;
			//	DontDestroyOnLoad(this);
				currentRoom = false;
				working = ScenesList.MainMenu;
				if(Application.internetReachability == NetworkReachability.NotReachable)
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
                        //StartCoroutine(CheckConnectionToServer());
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

            Loader = LoadingManager.Instance;
            Connect(XanaConstants.xanaConstants.EnviornmentName);
            print(XanaConstants.xanaConstants.EnviornmentName);
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

        IEnumerator CheckConnectionToServer()
        {
            yield return new WaitForSeconds(1);
        }

        #endregion
        #region Public Methods
        public void SetMaxPlayer(int max)
        {
            print("Launcher " + "SetMaxPlayer");
            maxPlayersPerRoom = (byte)max;
        }
        public void JoinCurrentRoom()
        {
            print("Launcher " + "JoinCurrentRoom");
            print("Join Current room in Launcher");
            currentRoom = true;
            Connect(PlayerPrefs.GetString("lb"));

        }
        public void LeaveGoToMainMenu()
        {
            print("Launcher " + "LeaveGoToMainMenu");
            print("Go To Menu Launcher");
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
            print("Launcher " + "Connect");
            working = ScenesList.AddressableScene;
            lobbyJoined = false;
            lastSceneName = SceneManager.GetActiveScene().name;
            lastLobbyName = lobbyN;
            print("Connecting: ");
            AvatarManager.timercall = false;
            //	RPCCallforBufferPlayers.playerobjects = null;

            Guid guid = System.Guid.NewGuid();



            Debug.Log("Login check===" + PlayerPrefs.GetString(ConstantsGod.PLAYERNAME));
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

            //LoadingManager.Instance.ShowLoading();
            //LoadingHandler.Instance.ShowLoading();
            isRoom = true;
            if (XanaEventDetails.eventDetails.DataIsInitialized)
            {
                string deepLinkLobbyName = $"{XanaEventDetails.eventDetails.eventType}{XanaEventDetails.eventDetails.id}";
                lobbyName = deepLinkLobbyName;
            }
            else if (XanaConstants.xanaConstants.pmy_isClassAvailable)
            {
                lobbyN += (lobbyN == "PMYRoomA") ? 
                    (APIBaseUrlChange.instance.IsXanaLive)? XanaConstants.xanaConstants.pmy_classRoomID_Main : XanaConstants.xanaConstants.pmy_classRoomID_Test
                    : "";
                string pmy_className = $"{lobbyN}{XanaConstants.xanaConstants.pmy_joinedClassCode}";
                Debug.Log("<color=red> PMY -- Joining The Class : " + pmy_className + "</color>");
                lobbyName = pmy_className;
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

                //if (lobbyName == "PMYRoomA")
                //    lobbyName = Guid.NewGuid().ToString();    // Generate unique room name everytime for PMYRoomA only

                print("Join Random Room in: " + lobbyName);
                PhotonNetwork.JoinLobby(new TypedLobby(lobbyName, LobbyType.Default));
            }
            else
            {
                print("Connecting: to Server using settings");
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = this.gameVersion;
            }

			SetMaxPlayer(int.Parse(XanaConstants.xanaConstants.userLimit));
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
            rejoin = true;
            print("Launcher " + "OnConnectedToMaster");
            // we don't want to do anything if we are not attempting to join a room. 
            // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
            // we don't want to do anything.
            PhotonNetwork.JoinLobby(new TypedLobby(lobbyName, LobbyType.Default));
            if (isConnecting)
            {
                LogFeedback("OnConnectedToMaster: Next -> try to Join Random Room");
                print("OnConnectedToMaster: Next -> try to Join Random Room");

            }


        }

        public override void OnJoinedLobby()
        {
            print("Launcher " + "OnJoinedLobby");
            //LoadingHandler.Instance.UpdateLoadingSlider(0.75f, true);
            //LoadingHandler.Instance.UpdateLoadingStatusText("Joining World");

        }
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            print("Launcher " + "OnJoinRoomFailed : Returining Main" );
            print(returnCode.ToString() + "	" + message);
            LoadFromFile.instance._uiReferences.LoadMain(true);
        }
        public override void OnCreatedRoom()
        {
            print("Launcher " + "OnCreatedRoom");
            print("OnCreatedRoom called");
        }
        public override void OnLeftLobby()
        {
            print("Launcher " + "OnLeftLobby");
            if (working == ScenesList.AddressableScene)
            {
                working = ScenesList.MainMenu;
            }
        }
        bool rejoin = true;
       
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
          
            bool joinedRoom = false;
            string CameraManRoomName=null;
            foreach (RoomInfo info in roomList)
            {
                Debug.Log("Max player :- "+info.Name+"--"+info.MaxPlayers+"--"+info.PlayerCount);
               // print("Max Players can Join " + info.MaxPlayers);
                int maxPlayer;
                //if (info.PlayerCount < info.MaxPlayers) {
                if (XanaConstants.xanaConstants.EnviornmentName == "Xana Festival") // to reserve the place for camera man (Show room is full to other players)
                {
                    maxPlayer = info.MaxPlayers - 1;
                }
                else
                {
                    maxPlayer = info.MaxPlayers;
                }
                if (info.PlayerCount < maxPlayer)
                {
                    print(info.MaxPlayers + "	" + info.Name);
                    lastRoomName = info.Name;
                    if (!XanaConstants.xanaConstants.isCameraMan)
                    {
                        Debug.Log("joining room here-----");
                        PhotonNetwork.JoinRoom(lastRoomName);
                        joinedRoom = true;
                        break;
                    }
                   
                }
                
                //roomNames.Add(info.Name);
            }
            if (XanaConstants.xanaConstants.isCameraMan)
            {
                if (roomList.Count>0)
                {
                    List<RoomInfo> tempRooms = new List<RoomInfo>(roomList);
                    tempRooms.Sort((a, b) => b.PlayerCount.CompareTo(a.PlayerCount));
                    //PhotonNetwork.JoinRoom(tempRooms[0].Name);
                    CameraManRoomName= tempRooms[0].Name;
                    //print(" VALUE IS "+tempRooms);
                    //Debug.Log("Is cameraman :--" + XanaConstants.xanaConstants.isCameraMan);
                    //if (!roomNames.Contains(info.Name)) // create new room btn
                    //{
                    //    Debug.Log("Initiate Room");
                    //    LoadingHandler.Instance.GetComponent<ManualRoomController>().InitiateRoomBtn(info.Name, info.PlayerCount + "/" + maxPlayer);
                    //}
                    //else// update previous room data
                    //{
                    //    if (info.PlayerCount > 0)
                    //    {
                    //        LoadingHandler.Instance.GetComponent<ManualRoomController>().UpdateRoomBtn(info.Name, info.PlayerCount + "/" + maxPlayer);
                    //    }
                    //    else
                    //    {
                    //        LoadingHandler.Instance.GetComponent<ManualRoomController>().DeleteRoomBtn(info.Name);
                    //    }
                    //}
                } else
                { 
                    // there is no room for stremaing so move to main menu to switch other world
                    LoadFromFile.instance._uiReferences.LoadMain(false); 
                }
            }
           
            if (joinedRoom == false)
            {
                Debug.Log("Player has not joined any room "+ XanaConstants.xanaConstants.isCameraMan);
                string temp;
                do
                {
                    temp = PhotonNetwork.CurrentLobby.Name + UnityEngine.Random.Range(0, 9999).ToString();
                } while (roomNames.Contains(temp));
                if (!XanaConstants.xanaConstants.isCameraMan)
                    PhotonNetwork.JoinOrCreateRoom(temp, RoomOptionsRequest(), new TypedLobby(lobbyName, LobbyType.Default), null);
                else
                {
                   // List<RoomInfo> tempRooms = new List<RoomInfo>(roomList);
                   // tempRooms.Sort((a, b) => b.PlayerCount.CompareTo(a.PlayerCount));
                   if(!CameraManRoomName.IsNullOrEmpty())
                    PhotonNetwork.JoinRoom(CameraManRoomName);
                }
            }
    }

        private void JoinRoomOrCreateRoom()
        {
            print("Launcher " + "JoinRoomOrCreateRoom");
        }

        public List<string> roomNames;

        public RoomOptions RoomOptionsRequest()
        {
			roomOptions = new RoomOptions();
			roomOptions.MaxPlayers = (byte)(int.Parse(XanaConstants.xanaConstants.userLimit));
			//if (XanaConstants.xanaConstants.EnviornmentName == "DJ Event")
			//{
			//	roomOptions.MaxPlayers = (byte)3;
			//}
			//else
			//{
			//	roomOptions.MaxPlayers = (byte)20;
			//}
			roomOptions.IsOpen = true;
			roomOptions.IsVisible = true;

            roomOptions.PublishUserId = true;
            roomOptions.CleanupCacheOnLeave = true;
            return roomOptions;
        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            print("Launcher " + "OnJoinRandomFailed");
            if (!XanaConstants.xanaConstants.isCameraMan)
            {
                PhotonNetwork.CreateRoom(null, RoomOptionsRequest(), new TypedLobby(lobbyName, LobbyType.Default), null);
            }
        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            playerobjects.Clear();
            print("Launcher " + "OnDisconnected");
            LogFeedback("<Color=Red>OnDisconnected</Color> " + cause);
            Debug.Log("PUN Basics Tutorial/Launcher:Disconnected");
            PlayerPrefs.SetInt("leftRoom", 1);
            // #Critical: we failed to connect or got disconnected. There is not much we can do. Typically, a UI system should be in place to let the user attemp to connect again.
            isConnecting = false;
            checkInternet();
        }

        public void checkInternet()
        {
            print("Launcher " + "checkInternet");
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Invoke("checkInternet", 1);
            }
        }

        public override void OnJoinedRoom()
        {
            print("Launcher " + "OnJoinedRoom");
            
            //LoadingHandler.Instance.UpdateLoadingSlider(0.8f, true);
            //LoadingHandler.Instance.UpdateLoadingStatusText("Joining World");

            lastRoomName = PhotonNetwork.CurrentRoom.Name;

            if (PhotonNetwork.CurrentRoom.PlayerCount >= 1)
            {
                PlayerPrefs.SetString("roomname", PhotonNetwork.CurrentRoom.Name);
                PlayerPrefs.Save();
            }
            if (!(SceneManager.GetActiveScene().name == "AddressableScene") || !(SceneManager.GetActiveScene().name.Contains("Museum")))
            {
                AvatarManager.Instance.InitCharacter();
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
                //StartCoroutine(LFF.VoidCalculation());
            }


            
        }
        public void Disconnect()
        {
            Debug.Log("Launcher Disconnect...");
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LeaveLobby();
            UserAnalyticsHandler.onUpdateWorldRelatedStats?.Invoke(false, false, false, true);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (newPlayer.NickName == "XANA_XANA")
            {
                XanaConstants.xanaConstants.isCameraManInRoom = true;
            }
        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (otherPlayer.NickName == "XANA_XANA")
            {
                XanaConstants.xanaConstants.isCameraManInRoom = false;
            }

            print("A player left room");
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
            if (XanaConstants.xanaConstants.isBuilderScene)
                GamificationComponentData.instance.MasterClientSwitched(newMasterClient);
        }
        #endregion
        public string lastSceneName, lastLobbyName, lastRoomName;

        public void JoinRoomManually(string name) {
            print("!! Room Name !!"+name);
           // PhotonNetwork.JoinOrCreateRoom(name, RoomOptionsRequest(), new TypedLobby(lobbyName, LobbyType.Default), null);
           PhotonNetwork.JoinRoom(name);
            //PhotonNetwork.JoinRoom(PhotonNetwork.CurrentRoom.Name, true);
        }

    }
}
