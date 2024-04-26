using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCountViewer : MonoBehaviourPunCallbacks
{
    private void Update()
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("GlobalPlayerCounts"))
        {
            var globalPlayerCounts = (ExitGames.Client.Photon.Hashtable)PhotonNetwork.CurrentRoom.CustomProperties["GlobalPlayerCounts"];
            if (globalPlayerCounts.ContainsKey("THA_Meeting_Room"))  // Replace "RoomName1" with the actual room name
            {
                int playerCountRoom1 = (int)globalPlayerCounts["THA_Meeting_Room"];
                Debug.Log("Player count in Room 1: " + playerCountRoom1);
            }
        }
    }
}
