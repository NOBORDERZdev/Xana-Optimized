using Photon.Pun;
using UnityEngine;

namespace RFM.Character
{
    public abstract class Hunter : MonoBehaviour, IPunObservable
    {
        public Transform cameraTarget;

        public string nickName = "Player";


        //internal int participationAmount = 100;
        //public int rewardMultiplier = 0;

        public abstract void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info);

        internal virtual void OnGameStarted()
        {
            // If prizePool exists on custom properties of current room, add participation amount to it. otherwise, create it.
            //if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("prizePool"))
            //{
            //    PhotonNetwork.CurrentRoom.CustomProperties["prizePool"] = (int)PhotonNetwork.CurrentRoom.CustomProperties["prizePool"] + participationAmount;
            //}
            //else
            //{
            //    PhotonNetwork.CurrentRoom.CustomProperties.Add("prizePool", participationAmount);
            //}
        }
    }

}