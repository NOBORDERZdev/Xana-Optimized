using System.Collections;
using System.Collections.Generic;
//using Invector.vCharacterController;
using UnityEngine;

public class AnimFunctionController : MonoBehaviour
{
    private GameObject player;
    [SerializeField]
    UIController_Shine uIController_Shine;

    void Start()
    {      
        player = GameObject.FindGameObjectWithTag("PhotonLocalPlayer");
    }
    private void WashFin() {
        FindChildren(player.transform);        
    }

    void FindChildren(Transform parent) {
        foreach (Transform child in parent) {
            if (child.name == "RightHand") {

                for (int i = 0; i < child.childCount; i++) {
                    if (child.GetChild(i).CompareTag("Tool")) {
                        Destroy(child.GetChild(i).gameObject);
                    }
                }

                //player.GetComponent<vThirdPersonController>().freeSpeed.rotationSpeed = 16;
                //player.GetComponent<vThirdPersonController>().freeSpeed.walkSpeed = 2;
                //player.GetComponent<vThirdPersonController>().freeSpeed.runningSpeed = 4;
                //player.GetComponent<vThirdPersonController>().freeSpeed.sprintSpeed = 6;
                break;
            }
            FindChildren(child);
        }
    }

    private void BowTwiceFin() {
        uIController_Shine.GetWorshipGameUI().gameObject.SetActive(true);
    }

    private void ClapFin() {
        uIController_Shine.GetWorshipGameUI().gameObject.SetActive(true);
    }

    private void PrayFin() {
        uIController_Shine.GetWorshipGameUI().gameObject.SetActive(true);
    }

    private void BowOnceFin() {
        uIController_Shine.GetOmikuziUI().gameObject.SetActive(true);
    }
}
