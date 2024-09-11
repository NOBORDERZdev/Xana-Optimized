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
        // Comment this for parent inherit
        public ServerConnectionStates connectionState = ServerConnectionStates.NotConnectedToServer;
        public MatchMakingStates matchMakingState = MatchMakingStates.NoState;
        public NetworkStates internetState = NetworkStates.NotConnectedToInternet;

        public static MutiplayerController instance;
        public ScenesList working;
        #region Private Serializable Fields

        public static string CurrLobbyName, CurrRoomName;

        private RoomOptions roomOptions;
        private List<RoomInfo> availableRoomList = new List<RoomInfo>();
        public List<string> roomNames;
        public List<GameObject> playerobjects;

        public GameplayEntityLoader LFF;

        [HideInInspector]
        public bool singlePlayerInstance;

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
        string gameVersion = "Summit20VoiceNew";
        #endregion

        #region Multtisection Fields
        /// <summary>
        /// True when player is changhing section.
        /// </summary>
        public bool isShifting;

        [Space]
        [Header("PhotonSectors")]
        private List<GameObject> playerobjectRoom;
        private string SectorName = "Default";
        public bool disableSector;
        private bool isWheel;
        #endregion
        #region MonoBehaviour CallBacks

        public void Awake()
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
        /// <summary>
        /// Start the connection process. 
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        /// 
        public void Connect(string lobbyN)
        {
            CurrLobbyName = APIBasepointManager.instance.IsXanaLive ? ("Live" + lobbyN) : ("Test" + lobbyN);

            working = ScenesList.AddressableScene;

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
                bool isConnected = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = this.gameVersion;
                JoinLobby(CurrLobbyName);
            }

            if (PhotonNetwork.NetworkingClient.State.ToString() != "Leaving")
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
        }

        public string getSector()
        {
            return SectorName;
        }

        public bool getIsWheel()
        {
            return isWheel;
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
            while (!PhotonNetwork.IsConnectedAndReady)
                await Task.Delay(1);
            PhotonNetwork.JoinLobby(new TypedLobby(lobbyName, LobbyType.Default));
        }

        public override void OnJoinedLobby()
        {

            Debug.LogError("On Joined lobby :- " + PhotonNetwork.CurrentLobby.Name + "--" + Time.time);
            if (SectorManager.Instance)
            {
                SectorManager.Instance.UpdateMultisector();
            }
            CheckRoomAvailability();
            if(isShifting)
            {
                LoadingHandler.Instance.DomeLoadingProgess(25);
            }
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
            availableRoomList.Clear();
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
            roomListUpdated = false;
            if (ConstantsHolder.xanaConstants.isCameraMan)
            {
                JoinRoomForCameraMan();
            }
            else if (ConstantsHolder.isFromXANASummit && singlePlayerInstance)
            {
                JoinRoomSeperateSingleRoom();
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

                    if (info.PlayerCount < info.MaxPlayers)
                    {
                        if (info.Name.Contains(PhotonNetwork.CurrentLobby.Name))
                        {
                            if (ConstantsHolder.MultiSectionPhoton)
                            {
                                object sector;
                                if (info.CustomProperties.TryGetValue("Sector", out sector))
                                {


                                    if (((string)sector) != SectorName) { continue; }
                                }
                                else { continue; }
                            }

                            CurrRoomName = info.Name;
                            Debug.LogError("Joining room   " + SectorName);
                            joinedRoom = PhotonNetwork.JoinRoom(CurrRoomName);
                            return;
                        }
                    }
                }
            if (joinedRoom == false)
            {
                int x = 1;
                string roomName;
                do
                {
                    if (ConstantsHolder.MultiSectionPhoton)
                    {
                        roomName = PhotonNetwork.CurrentLobby.Name +" "+ SectorName + "-Room:" + x.ToString(); //Prevents Race Condition.
                    }
                    else
                    {
                        roomName = PhotonNetwork.CurrentLobby.Name + "-Room:" + x.ToString();
                    }
                        x++;
                }
                while (roomNames.Contains(roomName));

                if (!isWheel)
                {
                    PhotonNetwork.JoinOrCreateRoom(roomName, RoomOptionsRequest(ConstantsHolder.userLimit, ConstantsHolder.MultiSectionPhoton), new TypedLobby(CurrLobbyName, LobbyType.Default));
                }
                else
                {
                    Debug.LogError("Joining room   " + SectorName);
                    PhotonNetwork.JoinOrCreateRoom(roomName, RoomOptionsRequest(4, ConstantsHolder.MultiSectionPhoton), new TypedLobby(CurrLobbyName, LobbyType.Default));
                }
                //   PhotonNetwork.JoinOrCreateRoom(roomName, RoomOptionsRequest(), new TypedLobby(CurrLobbyName, LobbyType.Default));
            }
        }

        private void JoinRoomSeperateSingleRoom()
        {
            Debug.LogError("Joining Seprateroom Lobby  " + PhotonNetwork.CurrentLobby.Name);
            string roomName;
            do
            {
                roomName = PhotonNetwork.CurrentLobby.Name + UnityEngine.Random.Range(0, 9999).ToString();
            }
            while (roomNames.Contains(roomName));

            PhotonNetwork.JoinOrCreateRoom(roomName, RoomOptionsRequest(ConstantsHolder.userLimit), new TypedLobby(CurrLobbyName, LobbyType.Default));
        }


        public RoomOptions RoomOptionsRequest(int Maxplayer, bool MultiSectionPhoton = false)
        {
            roomOptions = new RoomOptions();
            if (ConstantsHolder.xanaConstants.isXanaPartyWorld)
            {
                roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { "lastRank", 0 } };
                roomOptions.CustomRoomPropertiesForLobby = new string[] { "lastRank" };
            }
            roomOptions.MaxPlayers =(byte) Maxplayer;
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;

            if (MultiSectionPhoton)
            {
                roomOptions.CustomRoomPropertiesForLobby = new string[] { "Sector" };
                Debug.Log("Joining Sector  " + SectorName);
                roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { "Sector", SectorName } };
            }

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
            Debug.Log("Joined room   " + PhotonNetwork.CurrentRoom.Name);
            CurrRoomName = PhotonNetwork.CurrentRoom.Name;
            if (!isShifting)
            {
                LFF.LoadFile();

            }
            else {
               LoadingHandler.Instance.DomeLoadingProgess(90);
               GameplayEntityLoader.instance.SetPlayer(); DestroyPlayerDelay(); 
            }
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
            bool raceFinishStatus = false;
            Debug.Log("OnPlayerLeft  ..... " + otherPlayer.NickName);
            if (otherPlayer.NickName == "XANA_XANA")
            {
                ConstantsHolder.xanaConstants.isCameraManInRoom = false;
            }
            for (int x = 0; x < playerobjects.Count; x++)
            {
                if (playerobjects[x]==null)
                {
                    playerobjects.RemoveAt(x);
                }
            }
            if (ConstantsHolder.xanaConstants.isXanaPartyWorld)
            {
                if (ConstantsHolder.xanaConstants.isJoinigXanaPartyGame)
                {
                    ReferencesForGamePlay.instance.ReduceActivePlayerCountInCurrentLevel();
                    if (XANAPartyManager.Instance.GetComponent<PenpenzLpManager>().isLeaderboardShown)
                    {
                        if (PhotonNetwork.IsMasterClient && (XANAPartyManager.Instance.GameIndex < XANAPartyManager.Instance.GamesToVisitInCurrentRound.Count))
                        {
                            StartCoroutine(GamificationComponentData.instance.MovePlayersToNextGame());
                        }
                    }
                    else
                    {
                        GamificationComponentData.instance.UpdateRaceStatusIfPlayerLeaveWithoutCompletiting(raceFinishStatus);
                    }

                    if (GamificationComponentData.instance != null && !GamificationComponentData.instance.isRaceStarted && ReferencesForGamePlay.instance != null)
                    {
                        ReferencesForGamePlay.instance.IsLevelPropertyUpdatedOnlevelLoad = false;
                        ReferencesForGamePlay.instance.CheckActivePlayerInCurrentLevel();
                    }
                }

            }
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

                    if (GamificationComponentData.instance != null && !GamificationComponentData.instance.isRaceStarted && ReferencesForGamePlay.instance != null)
                    {
                        ReferencesForGamePlay.instance.IsLevelPropertyUpdatedOnlevelLoad = false;
                        ReferencesForGamePlay.instance.CheckActivePlayerInCurrentLevel();
                    }
                }

            }
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {

            //GameplayEntityLoader.instance._uiReferences.LoadMain(true);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {

        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            if (targetPlayer == PhotonNetwork.LocalPlayer)
            {
                if (ConstantsHolder.xanaConstants.isXanaPartyWorld && ConstantsHolder.xanaConstants.isJoinigXanaPartyGame && ReferencesForGamePlay.instance != null)
                {
                    ReferencesForGamePlay.instance.IsLevelPropertyUpdatedOnlevelLoad = false;
                    ReferencesForGamePlay.instance.CheckActivePlayerInCurrentLevel();
                }
            }
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

            if (ConstantsHolder.xanaConstants.isBuilderScene)
                GamificationComponentData.instance.MasterClientSwitched(newMasterClient);
        }
        #endregion


        public void JoinRoomManually(string name)
        {

            PhotonNetwork.JoinRoom(name);
        }
        async void DestroyPlayerDelay()
        {
            await new WaitForSeconds(2);
            foreach (var item in playerobjectRoom)
            {
                DestroyImmediate(item);
            }
            await new WaitForEndOfFrame();
            isShifting = false;
        }


        #region Sector Management

        public void Ontriggered(string SectorName, bool isWheel = false)
        {
            if (SectorName == this.SectorName || (disableSector && !isWheel) || ConstantsHolder.DiasableMultiPartPhoton) return;

            isShifting = true;
            var player = ReferencesForGamePlay.instance.m_34player;
            Debug.Log("Triggering...." + SectorName);
            this.SectorName = SectorName;
            Debug.Log("Triggered...." + this.SectorName);
            this.isWheel = isWheel;
            SummitPlayerRPC summitplayer = player.GetComponent<SummitPlayerRPC>();
            if (summitplayer)
            {
                Destroy(summitplayer.AnimatorView);
                Destroy(summitplayer.VoiceView);
                Destroy(summitplayer.Transformview);
                Destroy(summitplayer.view);
            }
          
            foreach (var p in playerobjects)
            {
                if (p == player||!p) continue;
                summitplayer = p.GetComponent<SummitPlayerRPC>();
                Destroy(summitplayer.AnimatorView);
                Destroy(summitplayer.VoiceView);
                Destroy(summitplayer.Transformview);
                Destroy(summitplayer.view);
            }
            XANASummitSceneLoading.OnJoinSubItem?.Invoke(ConstantsHolder.xanaConstants.minimap == 1);

            PhotonNetwork.LeaveRoom();

        }

        public override void OnLeftRoom()
        {
            Debug.Log("OnLeftRoom  ..... " + PhotonNetwork.IsConnectedAndReady);
            if (ConstantsHolder.xanaConstants.isXanaPartyWorld && ConstantsHolder.xanaConstants.isJoinigXanaPartyGame)
            {
                ReferencesForGamePlay.instance.ResetActivePlayerStatusInCurrentLevel();
            }

            // PhotonNetwork.ConnectUsingSettings();
            if (isShifting)
            {
                playerobjectRoom = new List<GameObject>(playerobjects);
                playerobjects.Clear();
                JoinLobby(CurrLobbyName);
                CarNavigationManager.CarNavigationInstance.Cars.Clear();
                LoadingHandler.Instance.DomeLoadingProgess(10);
            }
        }

        #endregion
    }
}
