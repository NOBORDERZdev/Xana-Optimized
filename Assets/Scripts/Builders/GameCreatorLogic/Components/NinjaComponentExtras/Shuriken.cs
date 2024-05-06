using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using UnityEngine;

public class Shuriken : MonoBehaviourPun
{
    Player player;

    private void OnEnable()
    {
        player = FindPlayerusingPhotonView(photonView);
        if (photonView.IsMine)
            return;
        if (!GamificationComponentData.instance.withMultiplayer)
        {
            gameObject.SetActive(false);
            return;
        }

        
    }

    Player FindPlayerusingPhotonView(PhotonView pv)
    {
        Player player = pv.Owner;
        foreach (GameObject playerObject in MutiplayerController.instance.playerobjects)
        {
            PhotonView _photonView = playerObject.GetComponent<PhotonView>();
            if (_photonView.Owner == player && _photonView.GetComponent<AvatarController>())
            {
                return _photonView.Owner;
            }
        }
        return null;
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
        if (player == photonView.Owner)
        {
            GetComponent<Rigidbody>().AddForce(force);
            Destroy(gameObject, 10f);
        }
    }
}
