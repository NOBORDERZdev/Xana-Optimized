using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightingGameManager : MonoBehaviour
{
    public static FightingGameManager instance;
    public bool startDirectly = false;

    public PhotonView PV;

    public UFE3D.CharacterInfo P1SelectedChar;
    public UFE3D.CharacterInfo P2SelectedChar;
    public PlayerDataClass player1Data = new PlayerDataClass();
    public PlayerDataClass player2Data = new PlayerDataClass();
    public string winnerClothJson;

    public string myName = ""; //Attizaz
    public string opponentName = "";

    public AudioSource mainAudioSource;
    public AudioClip crowdSound,menuMusic;


    public UFE3D.CharacterInfo[] profiles;

    public string PlayerClothJson;
    public string opponentClothJson;


    public Button OpenButton;
    [HideInInspector]public int id1;
    [HideInInspector] public int id2;

    public GameObject player1, player2,winnerAvatar;

    public bool isAITestingMode = false;
    [Tooltip("If above bool is on then you will be to select AI profile of your choice")]public int AIProfileNumber=0;


    public GameObject buttonLayoutPanel;
    public static Action activateLayoutPanel;
    
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnEnable()
    {
        activateLayoutPanel += ActivateButtonsLayoutPanel;
    }

    private void OnDisable()
    {
        activateLayoutPanel -= ActivateButtonsLayoutPanel;
    }
    private void Start()
    {
        /*myName = "Player : " + UnityEngine.Random.Range(0, 20).ToString();
        PhotonNetwork.NickName = myName;*/
        if (startDirectly)
        {
            print("Starting"); //kush
            UFE.SetPlayer1(P1SelectedChar);
            UFE.SetPlayer2(P2SelectedChar);
            UFE.StartLoadingBattleScreen();
        }
    }

    public void ActivateButtonsLayoutPanel() {
        buttonLayoutPanel.SetActive(true);
    }


    #region Attizaz's code
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

    /////Sound Settings
    public void PlayMenuMusic()
    {
        if (mainAudioSource != null)
        {
            mainAudioSource.Pause();
            mainAudioSource.clip = menuMusic;
            mainAudioSource.Play();
        }
    }

    public void PlayCrowdSound()
    {
        if (mainAudioSource != null)
        {
            mainAudioSource.Pause();
            mainAudioSource.clip = crowdSound;
            mainAudioSource.Play();
        }
    }

    //public bool testonly = false;
    //private void Update()
    //{
    //    if (testonly) {
    //        testonly = false;
    //        FindPlayersAndManageWin();
    //    }
    //}

    public void FindPlayersAndManageWin() {
        player1 = GameObject.Find("Player1");
        player2 = GameObject.Find("Player2");
        
        
        SoundChanger soundChanger = FindObjectOfType<SoundChanger>();
        winnerAvatar = soundChanger.WinnerAvatar;
        winnerAvatar.GetComponent<AvatarController>().staticClothJson = winnerClothJson;

        player1.SetActive(false);
        player2.SetActive(false);

        
        winnerAvatar.SetActive(true);
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