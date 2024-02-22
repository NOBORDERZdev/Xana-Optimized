using Photon.Pun;
using UnityEngine;

public class Shuriken : MonoBehaviourPun
{
    private void OnEnable()
    {
        if (photonView.IsMine)
            return;
        if (!GamificationComponentData.instance.withMultiplayer)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (!(_other.gameObject.CompareTag("Player") || (_other.gameObject.CompareTag("PhotonLocalPlayer") && _other.gameObject.GetComponent<PhotonView>().IsMine)))
        {
            if (this.photonView.Owner == PhotonNetwork.LocalPlayer)
                PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    void AddForce(Vector3 force)
    {
        GetComponent<Rigidbody>().AddForce(force);
        Destroy(gameObject, 10f);
    }
}
