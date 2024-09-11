using Photon.Pun;
using UnityEngine;

public class RocketCollectable : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PhotonLocalPlayer")  && collision.gameObject.GetComponent<PhotonView>().IsMine)
        {
            BuilderEventManager.OnDoorKeyCollisionEnter?.Invoke("Rocket part collected, Redirecting to Earth");
            gameObject.SetActive(false);
        }
    }
}
