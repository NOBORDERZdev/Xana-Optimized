using Photon.Pun;
using UnityEngine;

namespace RFM.Character
{
    public abstract class Runner : MonoBehaviour, IPunObservable
    {
        public abstract void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info);
    }
}