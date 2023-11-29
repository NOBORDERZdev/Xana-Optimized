using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace RFM.Managers
{
    /// <summary>
    /// Creates and manages the PUN2 rooms for RFM
    /// </summary>
    /// <remarks>https://muneebullah.com</remarks>
    public class RFMRoomManager : MonoBehaviourPun
    {
        private void Start()
        {
            photonView.RPC(nameof(PlayerJoined), RpcTarget.AllBuffered);
            Debug.Log("RFM PlayerJoined() RPC Requested by " + PhotonNetwork.NickName);

            InvokeRepeating(nameof(CheckForGameStartCondition), 1, 1);
        }

        [PunRPC]
        private void PlayerJoined()
        {
            CheckForGameStartCondition();
        }

        private void CheckForGameStartCondition()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                if (Globals.gameState != Globals.GameState.InLobby) return;
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.RaiseEvent(PhotonEventCodes.StartRFMEventCode, 
                        null,
                        new RaiseEventOptions { Receivers = ReceiverGroup.All }, 
                        SendOptions.SendReliable);
                }
                CancelInvoke(nameof(CheckForGameStartCondition));
            }
            else
            {
                RFM.Managers.RFMManager.Instance.RFMStartInterrupted();
            }
        }
    }
}