using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XANAPartyMulitplayer : MonoBehaviour
{
    bool singleTimeCall = true;

     public void MovePlayersToRandomGame()
        {
            print("MOVE PLAYER Random game call");
            if (!singleTimeCall) return;
            singleTimeCall = false;
             // Select a random room
            //string newRoom = GetComponent<MultiplayerXanaParty>().GetXanaPartyWorld();
            print("~~~~~~ calling gameID" + XANAPartyManager.Instance.name);
            GameData gameId =XANAPartyManager.Instance.GetRandomAndRemove();
            print("GAME ID "+ gameId.Id + " : "+ gameId.WorldName);
            GameplayEntityLoader.instance.PenguinPlayer.GetComponent<PhotonView>().RPC(nameof(MovePlayersToRoom), RpcTarget.AllBuffered, gameId.Id, gameId.WorldName); // Calling RPC from Master
        }

    [PunRPC]
    public void MovePlayersToRoom(int gameId, string gameName)
    {
        print("RPC CALL "+ gameId + " : "+ gameName);
        // Leave the current room
        ConstantsHolder.xanaConstants.isJoinigXanaPartyGame=true;
        ConstantsHolder.xanaConstants.XanaPartyGameId = gameId;
        ConstantsHolder.xanaConstants.XanaPartyGameName = gameName;
        ConstantsHolder.xanaConstants.isBuilderScene = true;
        ConstantsHolder.xanaConstants.MuseumID = gameId.ToString();
        PhotonNetwork.LeaveRoom();
        GameplayEntityLoader.instance._uiReferences.LoadMain(false);
        // Join the new room
        //PhotonNetwork.JoinOrCreateRoom(roomName, RoomOptionsRequest(), new TypedLobby(lobbyName, LobbyType.Default), null);
    }
}
