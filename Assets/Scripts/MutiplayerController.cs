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
        public static Action onRespawnPlayer;

        public ScenesList working;
        #region Private Serializable Fields

        public static string CurrLobbyName, CurrRoomName;

        public RoomOptions roomOptions;
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
        string gameVersion = "XanaSummit241008";  // YYYYMMDD

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
                // PhotonNetwork.AutomaticallySyncScene = true;  Zeel Removed confilicting with sector

            }
            else
            {
                DestroyImmediate(this);
            }

        }
        private void Start()
        {
            if (ConstantsHolder.xanaConstants.isXanaPartyWorld)
            {
                if(!ConstantsHolder.xanaConstants.isJoinigXanaPartyGame && !string.IsNullOrEmpty(ConstantsHolder.xanaConstants.LastLobbyName))
                    Connect(ConstantsHolder.xanaConstants.LastLobbyName);
            }
            else
            {
                Connect(ConstantsHolder.xanaConstants.EnviornmentName);
            }
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
        public async void Connect(string lobbyN)
        {
            while(isShifting)
            {
                Debug.LogError("herere");
                await Task.Delay(1000);
            }
            
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
            Debug.Log("Connected to Master");
            connectionState = ServerConnectionStates.ConnectedToServer;
            if (working == ScenesList.MainMenu)
                return;

           // JoinLobby(CurrLobbyName);
        }

        private async void JoinLobby(String lobbyName)
        {
            //if (ConstantsHolder.xanaConstants.isXanaPartyWorld)
            //{
            //    lobbyName = ConstantsHolder.xanaConstants.XanaPartyGameName;
            //}
            while (!PhotonNetwork.IsConnectedAndReady)
                await Task.Delay(1);

            Debug.Log("Joining lobby: " + lobbyName);
            PhotonNetwork.JoinLobby(new TypedLobby(lobbyName, LobbyType.Default));
        }

        public override void OnJoinedLobby()
        {

            Debug.Log("<color=red>On Joined lobby :- " + PhotonNetwork.CurrentLobby.Name + "--" + Time.time + " Shifting " + isShifting + "</color>");
            if (SectorManager.Instance)
            {
                SectorManager.Instance.UpdateMultisector();
            }
            CheckRoomAvailability();
            if (isShifting && isWheel)
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
            Debug.Log("OnRoomListUpdate called. Number of rooms: " + roomList.Count);

            availableRoomList.Clear();
            availableRoomList = roomList;
            roomListUpdated = true;

            foreach (var room in roomList)
            {
                Debug.Log($"Room: {room.Name}, Players: {room.PlayerCount}/{room.MaxPlayers}, IsVisible: {room.IsVisible}");
            }
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
            print(" ~~~~~~~ Join Room Custom call");
            bool joinedRoom = false;
            if (availableRoomList.Count > 0)
                foreach (RoomInfo info in availableRoomList)
                {
                    roomNames.Add(info.Name);
                    print(" ~~~~~~~roomNames : " + info.Name);

                    if (info.PlayerCount < info.MaxPlayers)
                    {
                        print(" ~~~~~~~less then max player");
                        if (info.Name.Contains(PhotonNetwork.CurrentLobby.Name))
                        {
                            print(" ~~~~~~~same room found : " + PhotonNetwork.CurrentLobby.Name);
                            print(" ~~~~~~~ConstantsHolder.MultiSectionPhoton : " + ConstantsHolder.MultiSectionPhoton);

                            if (ConstantsHolder.MultiSectionPhoton && !ConstantsHolder.xanaConstants.isXanaPartyWorld)
                            {
                                print(" ~~~~~~~multi selection is true");
                               
                              
                                object sector;
                                if (info.CustomProperties.TryGetValue("Sector", out sector))
                                {
                                    print("~~~~~~~Sector" + sector);
                                    if (((string)sector) != SectorName) { continue; }
                                }
                                else { continue; }
                            }
                            object IsVisible;
                            if (info.CustomProperties.TryGetValue("IsVisible", out IsVisible))
                            {
                                print("~~~~~~~IsVisible" + IsVisible);
                                if (((bool)IsVisible) != true)
                                {
                                    print("~~~~~~~  found IsVisible : false");
                                    continue; }


                            }
                            else {
                                print("~~~~~~~ no found IsVisible");
                                continue; 
                                }
                            CurrRoomName = info.Name;
                            Debug.Log("<color=red> ~~~~~~~ Joining room   " + " Shifting " + isShifting + "  " + SectorName + "</color>");
                            joinedRoom = PhotonNetwork.JoinRoom(CurrRoomName);
                            return;
                        }
                    }
                }
            if (joinedRoom == false)
            {
                print(" ~~~~~~~ Join Room Custom call 2"+ joinedRoom);
                int x = 1;
                string roomName;
                do
                {
                    if (ConstantsHolder.MultiSectionPhoton)
                    {
                        roomName = PhotonNetwork.CurrentLobby.Name + " " + SectorName + "-Room:" + x.ToString(); //Prevents Race Condition.
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
                    print(" ~~~~~~~ is not on wheel so createing or joinging world" );

                    PhotonNetwork.JoinOrCreateRoom(roomName, RoomOptionsRequest(ConstantsHolder.userLimit, ConstantsHolder.MultiSectionPhoton), new TypedLobby(CurrLobbyName, LobbyType.Default));
                }
                else
                {
                    Debug.Log("<color=red>Joining room   Shifting " + isShifting + " " + SectorName + "</color>");
                    PhotonNetwork.JoinOrCreateRoom(roomName, RoomOptionsRequest(4, ConstantsHolder.MultiSectionPhoton), new TypedLobby(CurrLobbyName, LobbyType.Default));
                }
                //   PhotonNetwork.JoinOrCreateRoom(roomName, RoomOptionsRequest(), new TypedLobby(CurrLobbyName, LobbyType.Default));
            }
        }

        private void JoinRoomSeperateSingleRoom()
        {
            Debug.Log("<color=red>Joining Seprateroom Lobby  " + PhotonNetwork.CurrentLobby.Name + "</color>");
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
            roomOptions.MaxPlayers = (byte)Maxplayer;
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;

            // Initialize the custom properties hashtable
            var customProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "IsVisible", true }
            };

            if (MultiSectionPhoton)
            {
                roomOptions.CustomRoomPropertiesForLobby = new string[] { "Sector", "IsVisible" };
                Debug.Log("Joining Sector  " + SectorName);
                customProperties.Add("Sector", SectorName);
            }
            else
            {
                print("~~~ IsVisible add in room properties");
                roomOptions.CustomRoomPropertiesForLobby = new string[] { "IsVisible" };
            }

            roomOptions.CustomRoomProperties = customProperties;
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
            Debug.Log("Joined room   " + PhotonNetwork.CurrentRoom.Name + " Shifting " + isShifting);
            CurrRoomName = PhotonNetwork.CurrentRoom.Name;
            if (!isShifting)
            {

                LFF.LoadFile();


            }
            else
            {
                if (isWheel)
                {
                    LoadingHandler.Instance.DomeLoadingProgess(90);
                }
                GameplayEntityLoader.instance.SetPlayer(); 
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
                if (ConstantsHolder.xanaConstants.isXanaPartyWorld && ConstantsHolder.xanaConstants.isJoinigXanaPartyGame)
                {
                    if (otherPlayer.ActorNumber == playerobjects[x].GetComponent<PhotonView>().OwnerActorNr)
                    {
                        raceFinishStatus = playerobjects[x].GetComponent<XANAPartyMulitplayer>().isRaceFinished;
                    }
                }
                if (playerobjects[x] == null)
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

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("<color=red>Failed to join</color>");
            OnJoinedLobby();
            //GameplayEntityLoader.instance._uiReferences.LoadMain(true);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            print("OnJoinRandomFailed called" + message);
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
       public async void DestroyPlayerDelay()
        {
            await new WaitForSeconds(.4f);
            foreach (var item in playerobjectRoom)
            {
                DestroyImmediate(item);
            }
            await new WaitForEndOfFrame();
            Debug.Log("Updated Is Shifting..." + isShifting);
            isShifting = false;
        }


        #region Sector Management

        public async void Ontriggered(string SectorName, bool isWheel = false)
        {
          

            while (isShifting)
            {
                await Task.Delay(1000);
            }
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
                if (p == player || !p) continue;
                summitplayer = p.GetComponent<SummitPlayerRPC>();
                Destroy(summitplayer.AnimatorView);
                Destroy(summitplayer.VoiceView);
                Destroy(summitplayer.Transformview);
                Destroy(summitplayer.view);
            }
            if (isWheel)
            {
                XANASummitSceneLoading.OnJoinSubItem?.Invoke(false);
            }
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }

        }

        public override void OnLeftRoom()
        {
            Debug.Log("OnLeftRoom  ..... " + PhotonNetwork.IsConnectedAndReady + "Is Shifting" + isShifting);


            // PhotonNetwork.ConnectUsingSettings();
            if (isShifting)
            {
                playerobjectRoom = new List<GameObject>(playerobjects);
                playerobjects.Clear();
                onRespawnPlayer?.Invoke();
                JoinLobby(CurrLobbyName);
                CarNavigationManager.CarNavigationInstance.Cars.Clear();
                if (isWheel)
                {
                    LoadingHandler.Instance.DomeLoadingProgess(10);
                }
            }
        }

        #endregion

        #region XANA Party

        public void MakeRoomPrivate()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(ChangeRoomVisibilityAfterDelay(0.1f));
            }
        }

        private IEnumerator ChangeRoomVisibilityAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (PhotonNetwork.IsMasterClient)
            {
                print("~~~~~~ converting the bool ");
                PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "IsVisible", false } });
            }
        }

        #endregion
        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            base.OnRoomPropertiesUpdate(propertiesThatChanged);

            //foreach (var key in propertiesThatChanged.Keys)
            //{
            //    Debug.Log($"Room property '{key}' has been updated to {propertiesThatChanged[key]}.");
            //}
        }
    }
}
