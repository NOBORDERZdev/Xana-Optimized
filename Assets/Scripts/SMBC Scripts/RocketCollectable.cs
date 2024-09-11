using Photon.Pun;
using UnityEngine;

public class RocketCollectable : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PhotonLocalPlayer")  && collision.gameObject.GetComponent<PhotonView>().IsMine)
        {
            BuilderEventManager.OnDoorKeyCollisionEnter?.Invoke("Rocket part collected, Redirecting to Earth");
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            Invoke(nameof(BackToEarth), 3f);
        }
    }
    private void BackToEarth()
    {
        GamePlayButtonEvents.OnExitButtonXANASummit?.Invoke();
    }
}
