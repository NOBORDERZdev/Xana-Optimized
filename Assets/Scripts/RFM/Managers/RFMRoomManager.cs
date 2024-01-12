using Cysharp.Threading.Tasks;
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
    public class RFMRoomManager : MonoBehaviourPunCallbacks
    {
        private void Start()
        {
            //photonView.RPC(nameof(PlayerJoined), RpcTarget.AllBuffered);
            Debug.Log("RFM PlayerJoined() RPC Requested by " + PhotonNetwork.NickName);

            //InvokeRepeating(nameof(CheckForGameStartCondition), 1, 1);
        }

        //[PunRPC]
        //private void PlayerJoined()
        //{
        //    CheckForGameStartCondition();
        //}

        //private void CheckForGameStartCondition()
        //{
        //    if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        //    {
        //        if (Globals.gameState != Globals.GameState.InLobby) return;
        //        if (PhotonNetwork.IsMasterClient)
        //        {
        //            PhotonNetwork.RaiseEvent(PhotonEventCodes.StartRFMEventCode, 
        //                null,
        //                new RaiseEventOptions { Receivers = ReceiverGroup.All }, 
        //                SendOptions.SendReliable);
        //        }
        //        CancelInvoke(nameof(CheckForGameStartCondition));
        //    }
        //    //else
        //    //{
        //    //    //RFM.Managers.RFMManager.Instance.RFMStartInterrupted();
        //    //}
        //}

        public async override void OnJoinedRoom()
        {
            Debug.LogError("RFM OnJoinedRoom() after reconnecting");

            var sceneName = RFM.Globals.DevMode ? "RFMDev" : "RFMDummy";

            await UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);

            await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
            
            LoadingHandler.Instance.HideLoading();
            
            // Reset all timers
            //FindObjectsOfType<RFM.Timer>().ForEachItem(t => Destroy(t.gameObject));

            // Reset all scores

            // Enable or Disable respective UI elements


            //StartCoroutine(RFM.Managers.RFMManager.Instance.Start());

        }
    }
}