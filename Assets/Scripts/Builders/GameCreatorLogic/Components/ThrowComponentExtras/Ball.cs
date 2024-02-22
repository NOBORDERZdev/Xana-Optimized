using Photon.Pun;
using UnityEngine;

public class Ball : MonoBehaviourPun
{
    [SerializeField] private Rigidbody _rb;
    private bool _isGhost;

    private void OnEnable()
    {
        if (photonView.IsMine)
            return;
        if (!GamificationComponentData.instance.withMultiplayer)
            gameObject.SetActive(false);
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
        Init(velocity, isGhost);
    }
}