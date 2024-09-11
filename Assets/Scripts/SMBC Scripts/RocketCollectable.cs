using Photon.Pun;
using UnityEngine;

public class RocketCollectable : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PhotonLocalPlayer")  && collision.gameObject.GetComponent<PhotonView>().IsMine)
        {
            gameObject.SetActive(false);
        }
    }
}
