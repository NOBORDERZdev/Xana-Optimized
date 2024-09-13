using Photon.Pun;
using UnityEngine;

public class BackToEarthTrigger : MonoBehaviour
{
    private bool alreadyTriggered;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PhotonLocalPlayer") && other.GetComponent<PhotonView>().IsMine && !alreadyTriggered)
        {
            alreadyTriggered = true;
            GamePlayButtonEvents.OnExitButtonXANASummit?.Invoke();
        }
    }
}
