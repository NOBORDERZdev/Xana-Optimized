using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightingGameManager : MonoBehaviourPunCallbacks
{
    public static FightingGameManager instance;
    public bool startDirectly = false;

    public PhotonView PV;

    public UFE3D.CharacterInfo P1SelectedChar;
    public UFE3D.CharacterInfo P2SelectedChar;

    public PlayerDataClass player1Data = new PlayerDataClass();
    public PlayerDataClass player2Data = new PlayerDataClass();
    public string myName = ""; //Attizaz
    public string opponentName = "";
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        myName = "Player : " + Random.Range(0, 20).ToString();
        PhotonNetwork.NickName = myName;
        if (startDirectly)
        {
            print("Starting"); //kush
            UFE.SetPlayer1(P1SelectedChar);
            UFE.SetPlayer2(P2SelectedChar);
            UFE.StartLoadingBattleScreen();
        }
        Invoke("GetPlayerData", 5);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            GetPlayerData();
        }
    }
    public void GetPlayerData()
    {
        Debug.LogError("GetPlayerData");
        Debug.LogError("player actornumber:" + PhotonNetwork.LocalPlayer.ActorNumber);
        player1Data = new PlayerDataClass(
            PhotonNetwork.LocalPlayer.CustomProperties["PlayerName"].ToString(),
            PhotonNetwork.LocalPlayer.CustomProperties["NFTURL"].ToString(),
            PhotonNetwork.LocalPlayer.CustomProperties["ClothJson"].ToString()
            );
        if (PhotonNetwork.PlayerListOthers.Length > 0)
        {
            player2Data = new PlayerDataClass(
                PhotonNetwork.PlayerListOthers[0].CustomProperties["PlayerName"].ToString(),
                PhotonNetwork.PlayerListOthers[0].CustomProperties["NFTURL"].ToString(),
                PhotonNetwork.PlayerListOthers[0].CustomProperties["ClothJson"].ToString()
                );
        }
    }

    public override void OnJoinedRoom()
    {
        GetPlayerData();
    }
    #region Attizaz's code
    public void CallRPC()
    {
        PV.RPC("PlayerSelection", RpcTarget.All);
    }

    [PunRPC]
    public void PlayerSelection()
    {
        print("!!!!!!!!!!!");
        print("My name is : " + PhotonNetwork.NickName);
        Photon.Realtime.Player[] otherPlayers = PhotonNetwork.PlayerListOthers;
        opponentName = "Opponent: " + otherPlayers[0].NickName;
        print("Opponent name is : " + opponentName);
        //UFE.SetPlayer1(P1SelectedChar);
        //UFE.SetPlayer2(P2SelectedChar);

        //UFE.StartLoadingBattleScreen();
    }


    public void RestartScene()
    {
        StartCoroutine("RestartingScene");

    }

    IEnumerator RestartingScene()
    {
        yield return new WaitForSeconds(0.2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    #endregion

}

[System.Serializable]
public class PlayerDataClass
{
    public string name;
    public string NFT;
    public string clothJson;

    public PlayerDataClass()
    {

    }
    public PlayerDataClass(string n, string nft, string c)
    {
        name = n;
        NFT = nft;
        clothJson = c;
    }
}