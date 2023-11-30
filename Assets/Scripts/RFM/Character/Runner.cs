using Photon.Pun;
using UnityEngine;

public abstract class Runner : MonoBehaviour, IPunObservable
{
    public abstract void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info);
}
