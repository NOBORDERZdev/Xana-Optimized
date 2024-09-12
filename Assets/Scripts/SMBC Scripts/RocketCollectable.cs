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
            SMBCManager.Instance.AddRocketPart();
            Destroy(gameObject, 0.1f);
        }
    }
}
