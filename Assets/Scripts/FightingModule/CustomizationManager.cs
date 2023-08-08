using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CustomizationManager : MonoBehaviourPunCallbacks
{
    #region Vars
    public Animator _animator;
    public GameObject reactionPlayer;
    public TextMeshProUGUI hintLabel;
    public static CustomizationManager _instance;
    private string gameMode;
    public Button _startButton;
    public GameObject loadingPanel;
    #endregion

    #region Unity Functions
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    private void Start()
    {
        hintLabel.text = "Preselect combo moves";
        StartCoroutine(checkoff(4f));
    }
    #endregion

    #region PhotonCallBacks
    public override void OnJoinRandomFailed(short returnCode, string message)// this method is called when we are failed to connect to random room.
    {
        base.OnJoinRandomFailed(returnCode, message);
        print(message);
        CreateAndJoinRoom();
    }

    public override void OnJoinedRoom() // Gets called on room join
    {
        print(PhotonNetwork.NickName + " Joined " + PhotonNetwork.CurrentRoom.Name);
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gm"))
        {
            object gameModeName;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gm", out gameModeName))
            {
                print(gameModeName.ToString());
            }
        }
        PhotonNetwork.LoadLevel("GamePlay");
    }
    #endregion

    #region UserFuncs
    /// <summary>
    /// This Function is called on start button here we check if user has selected all the required combos
    /// then start match according to the mode i.e 1 or 3 rounds selected by the player
    /// </summary>

    public void JoinRandomRoom()
    {
        if (ShotsSelectionData._instance.combo1Selected == -1 || ShotsSelectionData._instance.combo2Selected == -1 || ShotsSelectionData._instance._specialComboVal == -1 ||
           ShotsSelectionData._instance._throwVal == -1)// ShotsSelectionData._instance._grabVal == -1 || )
        {
            hintLabel.text = "Preselect combo moves.";
            StartCoroutine(checkoff(2f));
            return;
        }
        //ONLY FOR TESTING PURPOSES ALL VALUES OF SHOTS ARE SET TO 1 LATER DO UNCOMMENT ABOVE IF STATEMENT AND REMOVE ALL VALUE ASSIGNED 1 VALUES
        print("<color=yellow><b>ONLY FOR TESTING PURPOSES ALL VALUES OF SHOTS ARE SET TO 1 LATER DO UNCOMMENT ABOVE IF STATEMENT AND REMOVE ALL VALUE ASSIGNED 1 VALUES</b></color>");
        //ShotsSelectionData._instance.combo1Selected = 0;
        //ShotsSelectionData._instance.combo2Selected = 0;
        //ShotsSelectionData._instance._specialComboVal = 0;
        //ShotsSelectionData._instance._grabVal = 0;
        //ShotsSelectionData._instance._throwVal = 0;
        loadingPanel.SetActive(true); 
        _startButton.interactable = false;
        if (gameMode == null)
        {
            gameMode = "1";
        }
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", gameMode } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 2);
    }
    /// <summary>
    /// this function is called on 1 and 3 UI button where we simply set Game Mode 
    /// </summary>

    public void SetMode(string _gameMode)
    {
        gameMode = _gameMode;
    }
    /// <summary>
    /// it turns off text which tells user to select all combos first
    /// </summary>

    IEnumerator checkoff(float _val)
    {
        hintLabel.gameObject.SetActive(true);
        yield return new WaitForSeconds(_val);
        hintLabel.gameObject.SetActive(false);
    }
    #endregion

    #region Private Functions
    /// <summary>
    /// If user fails to join a room or there are no rooms then create one room for the player
    /// </summary>

    void CreateAndJoinRoom()
    {
        if (gameMode == null)
        {
            gameMode = "1";
        }
        string roomName = "Room " + Random.Range(0, 100);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        string[] roomPropsInLobby = { "gm" };//gm =game mode
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", gameMode } };
        roomOptions.CustomRoomPropertiesForLobby = roomPropsInLobby;
        roomOptions.CustomRoomProperties = customProperties;
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }
    #endregion
}
