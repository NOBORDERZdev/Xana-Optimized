using Photon.Pun;
using UnityEngine;

public class RocketCollectable : MonoBehaviour
{
    private bool alreadyTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PhotonLocalPlayer") && other.GetComponent<PhotonView>().IsMine && !alreadyTriggered)
        {
            alreadyTriggered = true;
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
