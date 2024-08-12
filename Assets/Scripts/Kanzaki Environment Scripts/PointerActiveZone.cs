using UnityEngine;
using Photon.Pun;

public class PointerActiveZone : MonoBehaviour
{
    [SerializeField]
    private UIController_Shine uIController_Shine;

    [SerializeField]
    private PointerUIChild _child;
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("PhotonLocalPlayer") && other.GetComponent<PhotonView>().IsMine)
        {
            GameObject pointerIconUI = uIController_Shine.GetWorldPointerUI().transform.GetChild((int)_child).gameObject;
            if (!pointerIconUI.activeSelf) {
                pointerIconUI.SetActive(true);
            }            
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("PhotonLocalPlayer") && other.GetComponent<PhotonView>().IsMine)
        {
            GameObject pointerIconUI = uIController_Shine.GetWorldPointerUI().transform.GetChild((int)_child).gameObject;
            if (pointerIconUI.activeSelf) {
                pointerIconUI.SetActive(false);
            }            
        }
    }
}
