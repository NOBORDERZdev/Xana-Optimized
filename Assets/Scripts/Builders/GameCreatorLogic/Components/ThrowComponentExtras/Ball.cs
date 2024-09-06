using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using UnityEngine;

public class Ball : MonoBehaviourPun
{
    [SerializeField] private Rigidbody _rb;
    private bool _isGhost;
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

    public void Init(Vector3 velocity, bool isGhost)
    {
        _isGhost = isGhost;
        _rb.AddForce(velocity, ForceMode.Impulse);
        Destroy(this.gameObject, 10f);
    }

    private void OnTriggerEnter(Collider other)
    {
        print("Collid" + (other.gameObject.name));
        CollectibleComponent c = other.gameObject.GetComponentInParent<CollectibleComponent>();
        if (c != null)
            Destroy(c.gameObject, 0f);
    }

    [PunRPC]
    void ThrowBall(Vector3 velocity, bool isGhost)
    {
        if (player == photonView.Owner)
            Init(velocity, isGhost);
    }
}