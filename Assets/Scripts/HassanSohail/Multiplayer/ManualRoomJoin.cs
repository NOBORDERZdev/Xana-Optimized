using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun;
using Photon.Realtime;

public class ManualRoomJoin : MonoBehaviour
{
    public TMP_Text RoomName;
    public TMP_Text CountTxt;

    void Start()
    {

    }

    public void Int(string roomName, string Count)
    {
        RoomName.text = roomName;
        CountTxt.text = Count;
    }

    public void JoinRoom()
    {
        // PhotonNetwork.JoinRoom();
        MutiplayerController.instance.JoinRoomManually(RoomName.text);
    }
}
