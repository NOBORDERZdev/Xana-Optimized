using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class UpdateRoomPlayerCount : MonoBehaviourPunCallbacks
{
    private const string GlobalPlayerCountKey = "GlobalPlayerCounts";

    public override void OnJoinedRoom()
    {
        UpdateGlobalPlayerCount();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateGlobalPlayerCount();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateGlobalPlayerCount();
    }

    void UpdateGlobalPlayerCount()
    {
        if (!PhotonNetwork.InRoom)
            return;

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        ExitGames.Client.Photon.Hashtable globalPlayerCounts = PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(GlobalPlayerCountKey)
            ? (ExitGames.Client.Photon.Hashtable)PhotonNetwork.CurrentRoom.CustomProperties[GlobalPlayerCountKey]
            : new ExitGames.Client.Photon.Hashtable();

        // Assuming RoomName uniquely identifies each room
        globalPlayerCounts[PhotonNetwork.CurrentRoom.Name] = playerCount;

        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            { GlobalPlayerCountKey, globalPlayerCounts }
        });
    }
}
