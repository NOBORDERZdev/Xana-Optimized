using Photon.Pun;
using PhysicsCharacterController;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class XANAPartyMulitplayer : MonoBehaviour
{
    //[SerializeField] Animator animator;
    PhotonView photonView;
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }
    // Coroutine to move players to a random game
    public IEnumerator MovePlayersToRandomGame()
    {
        // If already joining a game, exit coroutine
        if (ConstantsHolder.xanaConstants.isJoinigXanaPartyGame) yield break;
        ConstantsHolder.xanaConstants.isJoinigXanaPartyGame = true;

        // Get a random game data
        GameData gameData = XANAPartyManager.Instance.GetRandomAndRemove();
        yield return new WaitForSeconds(1f);

        // Call the RPC to move players to the selected room
        GameplayEntityLoader.instance.PenguinPlayer.GetComponent<PhotonView>().RPC(nameof(MovePlayersToRoom), RpcTarget.AllBuffered, gameData.Id, gameData.WorldName);
    }

    // RPC to move players to the selected room
    [PunRPC]
    void MovePlayersToRoom(int gameId, string gameName)
    {
        ConstantsHolder xanaConstants = ConstantsHolder.xanaConstants;
        // Set the game details in the constants holder
        xanaConstants.isJoinigXanaPartyGame = true;
        xanaConstants.XanaPartyGameId = gameId;
        xanaConstants.XanaPartyGameName = gameName;
        xanaConstants.isBuilderScene = true;
        xanaConstants.builderMapID = gameId;
        xanaConstants.isMasterOfGame = PhotonNetwork.IsMasterClient;

        // Load the main scene
        GameplayEntityLoader.instance._uiReferences.LoadMain(false);
    }

    // Method to move the player back to the lobby
    public void BackToLobby()
    {
        ConstantsHolder xanaConstants = ConstantsHolder.xanaConstants;
        // Reset the game details in the constants holder
        xanaConstants.isJoinigXanaPartyGame = false;
        xanaConstants.XanaPartyGameId = 0;
        xanaConstants.XanaPartyGameName = "";
        xanaConstants.isBuilderScene = false;
        xanaConstants.builderMapID = 0;
        xanaConstants.isMasterOfGame = false;
        // Load the main scene
        GameplayEntityLoader.instance._uiReferences.LoadMain(false);
    }



    //public void JumpRPCTrigger(){
    //    print("Trigger JUMP RPC");
    //    PhotonView tempPenguin = GameplayEntityLoader.instance.PenguinPlayer.GetComponent<PhotonView>();
    //    int id = tempPenguin.ViewID;
    //    tempPenguin.RPC(nameof(JumpRPC), RpcTarget.All, id);
    //}

    //[PunRPC]
    //void JumpRPC(int photonID)
    //{
    //    print("Target id " + photonID);
    //    print("CALLING JUMP RPC "+photonView.ViewID);
    //    if (photonView != null && photonView.ViewID == photonID)
    //    {
    //        print("Setting JUMP RPC");
    //        GameplayEntityLoader.instance.PenguinPlayer.GetComponent<CharacterManager>().MoveJump();
    //    }
    //}

}
