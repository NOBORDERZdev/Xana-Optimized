using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;

public class LaunchManager : MonoBehaviourPunCallbacks
{
    #region Vars
    public GameObject _namePanel, _connectionPanel, _lobbyPanel;
    public TextMeshProUGUI name;
    #endregion

    #region UnityFuncs
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true; // will force clients to join same room as master client
    }
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            //PhotonNetwork.LeaveRoom();
            _lobbyPanel.SetActive(true);
            _connectionPanel.SetActive(false);
            return;
        }
        _namePanel.SetActive(true);
        _connectionPanel.SetActive(false);
        _lobbyPanel.SetActive(false);
        //PhotonNetwork.ConnectUsingSettings();// use to connect with settings mentioned in photon settings file
    }
    #endregion

    #region PhotonCallBacks
    public override void OnConnected() // Check if we have connection
    {
        print("Connected to Internet");
    }

    public override void OnConnectedToMaster() // this method is called when we are connected to photon server.
    {
        print(PhotonNetwork.NickName + " Connected to master");
        _lobbyPanel.SetActive(true);
        _connectionPanel.SetActive(false);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)// this method is called when we are failed to connect to random room.
    {
        base.OnJoinRandomFailed(returnCode, message);
        print(message);
       // CreateAndJoinRoom();
    }

    public override void OnJoinedRoom() // Gets called on room join
    {
        print(PhotonNetwork.NickName + " Joined " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.LoadLevel("GamePlay");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        print(newPlayer.NickName + " Joined To " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount);
    }
    #endregion

    #region PublicMethods
    public void ConnectToServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();// use to connect with settings mentioned in photon settings file
            _connectionPanel.SetActive(true);
            if (string.IsNullOrEmpty(PhotonNetwork.NickName))
            {
                int rand = Random.Range(0, 500);
                PhotonNetwork.NickName = "Player " + rand.ToString();
            }
            name.text = PhotonNetwork.NickName + " is Connecting";
            _namePanel.SetActive(false);
            
           
        }
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.LoadLevel("PlayerCustomization");
        //  PhotonNetwork.JoinRandomRoom();
    }
    #endregion

    #region Private Method
    //void CreateAndJoinRoom()
    //{
    //    string roomName = "Room " + Random.Range(0, 100);
    //    RoomOptions roomOptions = new RoomOptions();
    //    roomOptions.IsOpen = true;
    //    roomOptions.IsVisible = true;
    //    roomOptions.MaxPlayers = 2;
    //    PhotonNetwork.CreateRoom(roomName, roomOptions);
    //}
    #endregion
}
