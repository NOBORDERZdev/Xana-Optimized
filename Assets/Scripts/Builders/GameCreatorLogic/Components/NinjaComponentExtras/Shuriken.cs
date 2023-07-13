using Photon.Pun;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    private void OnTriggerEnter(Collider _other)
    {
        if (!(_other.gameObject.CompareTag("Player") || (_other.gameObject.CompareTag("PhotonLocalPlayer") && _other.gameObject.GetComponent<PhotonView>().IsMine)))
        {
            Destroy(gameObject);
        }
    }
}
