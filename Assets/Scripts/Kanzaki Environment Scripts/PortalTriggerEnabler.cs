using Photon.Pun;
using UnityEngine;

public class PortalTriggerEnabler : MonoBehaviour
{
    public BoxCollider daisenTrigger;
    public BoxCollider duneTrigger;
    private bool _once;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PhotonLocalPlayer") && other.GetComponent<PhotonView>().IsMine)
        {
            if (!_once)
            {
                duneTrigger.enabled = true;
                daisenTrigger.enabled = true;
            }
            _once = true;

        }
    }
}