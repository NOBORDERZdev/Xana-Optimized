using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using Photon.Voice.PUN;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Created By Zeel for Summit Multiplayer Management
/// </summary>
public class MultiplayerMultisectionController : MonoBehaviourPunCallbacks
{


    public ServerConnectionStates connectionState = ServerConnectionStates.NotConnectedToServer;
    public MatchMakingStates matchMakingState = MatchMakingStates.NoState;
    public NetworkStates internetState = NetworkStates.NotConnectedToInternet;

    public static MutiplayerController instance;
    public ScenesList working;
    #region Private Serializable Fields

    public static string CurrLobbyName, CurrRoomName;

    protected RoomOptions roomOptions;
    protected List<RoomInfo> availableRoomList = new List<RoomInfo>();
    public List<string> roomNames;
    public List<GameObject> playerobjects;

    public GameplayEntityLoader LFF;



    #endregion

    #region Multtisection Fields
    /// <summary>
    /// True when player is changhing section.
    /// </summary>
    private bool isShifting;

    [Space]
    [Header("PhotonSectors")]
    private List<GameObject> playerobjectRoom;
    private string SectorName = "Default" ;
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
    protected string gameVersion = "13";
    #endregion

    public virtual void Awake()
    {
        
        Debug.Log(gameObject.name);
    
    }

    #region MonoBehaviour CallBacks


   /* protected virtual void Start()
    {
        // Seperate the live and test environment
        string _LobbyName = APIBasepointManager.instance.IsXanaLive ? ("Live" + ConstantsHolder.xanaConstants.EnviornmentName) : ("Test" + ConstantsHolder.xanaConstants.EnviornmentName);
        Debug.Log("Lobby Name: " + _LobbyName);
        Connect(_LobbyName);
    }*/
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
    public virtual void Connect(string lobbyN)
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
            bool isConnected = PhotonNetwork.ConnectUsingSettings();
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

    private async void JoinLobby(string lobbyName)
    {
        while (!PhotonNetwork.IsConnectedAndReady)
            await Task.Delay(1);
        PhotonNetwork.JoinLobby(new TypedLobby(lobbyName, LobbyType.Default));
    }

    public override void OnJoinedLobby()
    {
        Debug.LogError("On Joined lobby :- " + PhotonNetwork.CurrentLobby.Name + "--" + Time.time);
        CheckRoomAvailabilitty();
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

    async Task WaitUntilRoomListUpdateed()
    {
        while (!roomListUpdated)
        {
            await Task.Delay(1000);
        }
    }

    async void CheckRoomAvailabilitty()
    {
        await WaitUntilRoomListUpdateed();
        if (ConstantsHolder.xanaConstants.isCameraMan)
        {
            JoinRoomForCameraaMan();
        }
        else
        {
            JoinRoomCustomm();
        }
    }


    void JoinRoomForCameraaMan()
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

    private void JoinRoomCustomm()
    {
        bool joinedRoom = false;
        if (availableRoomList.Count > 0)
            foreach (RoomInfo info in availableRoomList)
            {
                roomNames.Add(info.Name);
                Debug.LogError(info.PlayerCount + "--" + info.MaxPlayers + "--" + info.Name);
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

    private  RoomOptions RoomOptionsRequest()
    {
        roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)(int.Parse(ConstantsHolder.xanaConstants.userLimit));
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "Sector" };
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable {{ "Sector",SectorName }};
        roomOptions.PublishUserId = true;
        roomOptions.CleanupCacheOnLeave = false;
       
        return roomOptions;
    }

    public override void OnCreatedRoom()
    {
        print("OnCreatedRoom called");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Current Sector ==  " + PhotonNetwork.CurrentRoom.CustomProperties["Sector"]);
        CurrRoomName = PhotonNetwork.CurrentRoom.Name;
        if (!isShifting)
            LFF.LoadFile();
        else { GameplayEntityLoader.instance.SetPlayer(); isShifting = false; } // StartCoroutine(GameplayEntityLoader.instance.SpawnPlayerSection());
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

    
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError(message);
        GameplayEntityLoader.instance._uiReferences.LoadMain(true);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {

    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected");
        playerobjects.Clear();
    }

    public virtual void Disconnect()
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

    #region Sector Management

    public void Ontriggered(string SectorName)
    {
        if (SectorName == this.SectorName) return;
      
        isShifting = true;
        var player = ReferencesForGamePlay.instance.m_34player;
      
        Destroy(player.GetComponent<PhotonAnimatorView>());
        Destroy(player.GetComponent<PhotonTransformView>());
        Destroy(player.GetComponent<PhotonView>());
        Destroy(player.GetComponent<PhotonVoiceView>());
        foreach(var p in playerobjects)
        {
            Destroy(p.GetComponent<PhotonAnimatorView>());
            Destroy(p.GetComponent<PhotonTransformView>());
            Destroy(p.GetComponent<PhotonView>());
            Destroy(p.GetComponent<PhotonVoiceView>());
        }
        this.SectorName = SectorName;
        PhotonNetwork.LeaveRoom();
        
    }

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom  ..... " + PhotonNetwork.IsConnectedAndReady);


        // PhotonNetwork.ConnectUsingSettings();
        if (isShifting)
        {
            playerobjectRoom = new List<GameObject>(playerobjects);
            playerobjects.Clear();
            JoinLobby(CurrLobbyName);
        }
    }

    #endregion

    public virtual void JoinRoomManually(string name)
    {
        PhotonNetwork.JoinRoom(name);
    }


   
}



