using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerReferences : MonoBehaviourPunCallbacks
{
    #region Variables
    [SerializeField] List<PhotonView> playerViews = new List<PhotonView>();
    [SerializeField] PlayerController _playerController;
    [SerializeField] TakeDamage _takeDamage;
    [SerializeField] PhotonView Myview;

    GameObject otherPlayer;

    #endregion

    #region Unity Funcs
    void Start()
    {
        Myview = GetComponent<PhotonView>();
        StartCoroutine(GetOtherPlayers());
    }

    public void OtherPlayerReady()
    {
        photonView.RPC("OtherPlayerReady_RPC", RpcTarget.All);
    }

    [PunRPC]
    public void OtherPlayerReady_RPC()
    {
        StartCoroutine(GetOtherPlayers());
    }
    #endregion

    #region PhotonCallbacks
    //public override void OnPlayerEnteredRoom(Player newPlayer)
    //{
    //    base.OnPlayerEnteredRoom(newPlayer);
    //    StartCoroutine(GetOtherPlayers());
    //    print(newPlayer.NickName + " Joined To " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount);
    //}
    #endregion

    #region User Funcs

    /// <summary>
    /// Getting players PVs for references
    /// </summary>

    IEnumerator GetOtherPlayers()
    {
        yield return new WaitForSeconds(5f);
        playerViews.Clear();
        var temp = FindObjectsOfType<PhotonView>();
        foreach (PhotonView pv in temp)
            playerViews.Add(pv);

        GetOtherPlayerController();
    }


    /// <summary>
    /// Settng up player referneces
    /// </summary>
   
    void GetOtherPlayerController()
    {
        foreach (PhotonView pv in playerViews)
        {
            if (pv != Myview)
            {
                if (pv.ViewID != 1)
                {
                    otherPlayer = PhotonView.Find(pv.ViewID).gameObject;
                    _playerController.otherAnim = otherPlayer.GetComponent<Animator>();
                    _playerController.otherPlayerController = otherPlayer.GetComponent<PlayerController>();
                    _takeDamage.otherPlayerTakeDamage=otherPlayer.GetComponent<TakeDamage>();
                    _takeDamage.otherPlayerCountdown=otherPlayer.GetComponent<CountdownManager>();
                    _takeDamage.otherPlayer = otherPlayer;
                    _takeDamage.otherPlayerController = otherPlayer.GetComponent<PlayerController>();
                }
            }
        }
    }
    #endregion
}
