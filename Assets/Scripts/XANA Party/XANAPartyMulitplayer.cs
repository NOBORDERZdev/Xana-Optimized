using Photon.Pun;
using System.Collections;
using UnityEngine;

public class XANAPartyMulitplayer : MonoBehaviour
{
    private PhotonView photonView;
    private HomeSceneLoader uiReferences;
    private ConstantsHolder xanaConstants;

    // Cache references in Awake for better performance
    private void Awake()
    {
        photonView = GameplayEntityLoader.instance.PenguinPlayer.GetComponent<PhotonView>();
        uiReferences = GameplayEntityLoader.instance._uiReferences;
        xanaConstants = ConstantsHolder.xanaConstants;
    }

    // Coroutine to move players to a random game
    public IEnumerator MovePlayersToRandomGame()
    {
        // If already joining a game, exit coroutine
        if (xanaConstants.isJoinigXanaPartyGame) yield break;
        xanaConstants.isJoinigXanaPartyGame = true;

        // Get a random game data
        GameData gameData = XANAPartyManager.Instance.GetRandomAndRemove();
        yield return new WaitForSeconds(1f);

        // Call the RPC to move players to the selected room
        photonView.RPC(nameof(MovePlayersToRoom), RpcTarget.AllBuffered, gameData.Id, gameData.WorldName);
    }

    // RPC to move players to the selected room
    [PunRPC]
    void MovePlayersToRoom(int gameId, string gameName)
    {

        // Set the game details in the constants holder
        xanaConstants.isJoinigXanaPartyGame = true;
        xanaConstants.XanaPartyGameId = gameId;
        xanaConstants.XanaPartyGameName = gameName;
        xanaConstants.isBuilderScene = true;
        xanaConstants.builderMapID = gameId;
        xanaConstants.isMasterOfGame = PhotonNetwork.IsMasterClient;

        // Load the main scene
        uiReferences.LoadMain(false);
    }

    // Method to move the player back to the lobby
    public void BackToLobby()
    {
        // Reset the game details in the constants holder
        xanaConstants.isJoinigXanaPartyGame = false;
        xanaConstants.XanaPartyGameId = 0;
        xanaConstants.XanaPartyGameName = "";
        xanaConstants.isBuilderScene = false;
        xanaConstants.builderMapID = 0;
        xanaConstants.isMasterOfGame = false;

        // Load the main scene
        uiReferences.LoadMain(false);
    }
}
