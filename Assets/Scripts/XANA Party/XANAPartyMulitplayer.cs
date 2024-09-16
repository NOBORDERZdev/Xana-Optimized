using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using PhysicsCharacterController;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class XANAPartyMulitplayer : MonoBehaviour, IPunInstantiateMagicCallback
{
    //[SerializeField] Animator animator;
    PhotonView photonView;

    private ConstantsHolder _XanaConstants = ConstantsHolder.xanaConstants;

    public int UserId;
    public int RaceFinishCount = 0;
    public bool isRaceFinished = false;
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        Invoke(nameof(DisbleAnimatedController),0.1f);
    }
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        MutiplayerController.instance.playerobjects.Add(info.photonView.gameObject);
    }

    void DisbleAnimatedController()
    {
        if (PhotonNetwork.IsMasterClient &&photonView.IsMine )
        {
            return;
        }
        if (photonView != null && !photonView.IsMine) {
            this.GetComponentInChildren<AnimatedController>().enabled = false;
        }
       
    }
    // Coroutine to move players to a random game
    public IEnumerator MovePlayersToRandomGame()
    {
        // If already joining a game, exit coroutine
        if (ConstantsHolder.xanaConstants.isJoinigXanaPartyGame) yield break;
        ConstantsHolder.xanaConstants.isJoinigXanaPartyGame = true;

        // Get a random game data
        GameData gameData = XANAPartyManager.Instance.GetGameToVisitNow();
        yield return new WaitForSeconds(1f);

        // Call the RPC to move players to the selected room
        GameplayEntityLoader.instance.PenguinPlayer.GetComponent<PhotonView>().RPC(nameof(MovePlayersToRoom), RpcTarget.AllBuffered, gameData.Id, gameData.WorldName);
    }



    public IEnumerator ShowLobbyCounter()
    {
        if (GetComponent<PhotonView>().IsMine && PhotonNetwork.IsMasterClient)
        {
            ReferencesForGamePlay.instance.isCounterStarted = true;
            if (!ReferencesForGamePlay.instance.isMatchingTimerFinished)
                yield return new WaitForSeconds(10f);                     // wait to show that other player spwan and then lobby full
            else
                yield return new WaitForSeconds(0f);
            GameplayEntityLoader.instance.PenguinPlayer.GetComponent<PhotonView>().RPC(nameof(StartLobbyCounter), RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void StartLobbyCounter()
    {
        StartCoroutine(ReferencesForGamePlay.instance.ShowLobbyCounterAndMovePlayer());
    }

    // RPC to move players to the selected room
    [PunRPC]
    void MovePlayersToRoom(int gameId, string gameName)
    {
        
        XANAPartyManager.Instance.GetComponent<PenpenzLpManager>().isLeaderboardShown = false;
        // Set the game details in the constants holder
        _XanaConstants.isJoinigXanaPartyGame = true;
        _XanaConstants.XanaPartyGameId = gameId;
        _XanaConstants.XanaPartyGameName = gameName;
        _XanaConstants.isBuilderScene = true;
        _XanaConstants.builderMapID = gameId;
        _XanaConstants.isMasterOfGame = PhotonNetwork.IsMasterClient;
       
        print("!! move to level");
        // Make the room invisible if the current player is the master client

        // SceneManager.UnloadScene("GamePlayScene");
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    Photon.Pun.PhotonHandler.levelName = "Builder";
        //    PhotonNetwork.LoadLevel("Builder");
        //}
        // Load the main scene
        //GameplayEntityLoader.instance._uiReferences.LoadMain(false);
        Photon.Pun.PhotonHandler.levelName = "Builder";
        ReferencesForGamePlay.instance.LoadLevel("Builder");
    }

    public IEnumerator MoveToLobby()
    {
        // Reset the game details in the constants holder
        _XanaConstants.isJoinigXanaPartyGame = false;
        _XanaConstants.XanaPartyGameId = 0;
        _XanaConstants.XanaPartyGameName = "";
        _XanaConstants.isBuilderScene = false;
        _XanaConstants.builderMapID = 0;
        _XanaConstants.GameIsFinished = true;
        // Load the main scene
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.PenpenzLoading(FadeAction.In));
        yield return new WaitForSeconds(2f);
        GameplayEntityLoader.instance._uiReferences.LoadMain(false);
    }

    

    [PunRPC]
    public void MovePlayerToNextGameOnReconnect()
    {
        if (PhotonNetwork.IsMasterClient && (XANAPartyManager.Instance.GameIndex < XANAPartyManager.Instance.GamesToVisitInCurrentRound.Count))
        {
            StartCoroutine(GamificationComponentData.instance.MovePlayersToNextGame());
        }
    }
    public void ResetValuesOnCompleteRace()
    {
        _XanaConstants.isJoinigXanaPartyGame = false;
        _XanaConstants.XanaPartyGameId = 0;
        _XanaConstants.XanaPartyGameName = "";
        _XanaConstants.isBuilderScene = false;
        _XanaConstants.builderMapID = 0;
    }


    [PunRPC]
    public void RPC_AddPlayerID(int playerID)
    {
        if (!XANAPartyManager.Instance.GetComponent<PenpenzLpManager>().PlayerIDs.Contains(playerID))
        {
            XANAPartyManager.Instance.GetComponent<PenpenzLpManager>().PlayerIDs.Add(playerID);
        }
    }

    [PunRPC]
    public void RPC_AddWinnerId(int winnerID)
    {
        if (!XANAPartyManager.Instance.GetComponent<PenpenzLpManager>().WinnerPlayerIds.Contains(winnerID))
        {
            XANAPartyManager.Instance.GetComponent<PenpenzLpManager>().WinnerPlayerIds.Add(winnerID);
        }
    }

    [PunRPC]
    public void UpdateStatusOnRaceFinish(int actorNumber, bool status)
    {
        if(photonView.Owner.ActorNumber == actorNumber)
        {
            isRaceFinished = status;
        }
    }

    [PunRPC]
    public void PlayerCountAtStartOfRace(int playerCount)
    {
        XANAPartyManager.Instance.GetComponent<PenpenzLpManager>().RaceStartWithPlayers = playerCount;
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
