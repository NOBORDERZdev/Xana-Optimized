using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;

public class XANAPartyMulitplayer : MonoBehaviour
{
    bool singleTimeCall = true;
    private RoomOptions roomOptions;

    public IEnumerator MovePlayersToRandomGame()
    {
        print("MOVE PLAYER Random game call");
        if (!singleTimeCall) yield return null;
        singleTimeCall = false;
        // Select a random room
        //string newRoom = GetComponent<MultiplayerXanaParty>().GetXanaPartyWorld();
        print("~~~~~~ calling gameID" + XANAPartyManager.Instance.name);
        GameData gameData = XANAPartyManager.Instance.GetRandomAndRemove();
        print("GAME ID " + gameData.Id + " : " + gameData.WorldName);
        //string roomName;
        ////do
        ////{
        ////    roomName = PhotonNetwork.CurrentLobby.Name + UnityEngine.Random.Range(0, 9999).ToString();
        ////}
        ////while (roomNames.Contains(roomName));
        //roomName = gameId.WorldName + UnityEngine.Random.Range(0, 9999).ToString();;
        MutiplayerController.instance.CreateGameRoom(gameData.WorldName);
       // PhotonNetwork.CreateRoom(roomName, RoomOptionsRequest(), new TypedLobby(roomName, LobbyType.Default));

        yield return new WaitForSeconds(1f);
        GameplayEntityLoader.instance.PenguinPlayer.GetComponent<PhotonView>().RPC(nameof(MovePlayersToRoom), RpcTarget.AllBuffered, gameData.Id, gameData.WorldName); // Calling RPC from Master

    }

     public RoomOptions RoomOptionsRequest()
    {
            roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = (byte)(int.Parse(ConstantsHolder.xanaConstants.userLimit));
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;

            roomOptions.PublishUserId = true;
            roomOptions.CleanupCacheOnLeave = true;
            return roomOptions;
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
        ConstantsHolder.xanaConstants.builderMapID = gameId;
        //PhotonNetwork.LeaveRoom();
        GameplayEntityLoader.instance._uiReferences.LoadMain(false);
        // Join the new room
        //PhotonNetwork.JoinOrCreateRoom(roomName, RoomOptionsRequest(), new TypedLobby(lobbyName, LobbyType.Default), null);
    }
}
