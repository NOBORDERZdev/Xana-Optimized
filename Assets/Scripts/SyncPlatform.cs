using UnityEngine;
using Photon.Pun;

public class SyncPlatform : MonoBehaviourPun, IPunObservable
{
    private Vector3 realPosition;
    private Quaternion realRotation;
    private float positionLerpRate = 10f;
    private float rotationLerpRate = 5f;

    void Start()
    {
        realPosition = transform.position;
        realRotation = transform.rotation;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // Network player, receive data
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            // Smoothly interpolate to the received position and rotation
            transform.position = Vector3.Lerp(transform.position, realPosition, positionLerpRate * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, rotationLerpRate * Time.deltaTime);
        }
    }
}