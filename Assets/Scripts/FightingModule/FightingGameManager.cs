using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightingGameManager : MonoBehaviour
{
    public static FightingGameManager instance;
    public bool startDirectly = false;

    public PhotonView PV;

    public UFE3D.CharacterInfo P1SelectedChar;
    public UFE3D.CharacterInfo P2SelectedChar;

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
