using Photon.Pun;
using UnityEngine;

namespace RFM.Character
{
    public abstract class Hunter : MonoBehaviour, IPunObservable
    {
        public Transform cameraTarget;

        public abstract void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info);
    }

}