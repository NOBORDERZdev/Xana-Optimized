using Photon.Pun;
using UnityEngine;

/// <summary>///
/// Use on exit portal trigger of sub world to exit to main world.
/// </summary>

public class BackToMainWorldTrigger : MonoBehaviour
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
