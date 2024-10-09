using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTrigger : MonoBehaviour
{
    bool waitforresponce = false;
    private void Start()
    {
        LoadingHandler.Instance.EnterWheel += ((bo) => { if (waitforresponce && bo) { waitforresponce = false; ChangeSector(); } });
    }
    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.InRoom)
        {
            string name = (string)PhotonNetwork.CurrentRoom.CustomProperties["Sector"];
            if (other.gameObject.tag == "PhotonLocalPlayer")
            {
                if (other.GetComponent<PhotonView>().IsMine)
                {
                    if (name != "Wheel"&& !other.GetComponent<SummitPlayerRPC>().isInsideCAr)
                    {
                       waitforresponce=true;
                        LoadingHandler.Instance.showApprovalWheelloading();
                    }
                    else
                    {
                      //  other.GetComponent<SummitPlayerRPC>().CheckForExitWheel();
                    }
                }
            }

            if (name == "Wheel")
            {
                if (other.gameObject.tag == "WheelCar")
                {
                    Debug.Log("WheelCar Triggered ");
                    GiantWheelManager.Instance.CheckForWheelCar();


                }
                if (other.gameObject.tag == "Wheel" && !GiantWheelManager.Instance.CarAdded)
                {

                    GiantWheelManager.Instance.AddCar();

                }


            }
        }
    }
    async void ChangeSector()
    {
        while (MutiplayerController.instance.isShifting)
        {
            await new WaitForSeconds(1f);
        }
        //XANASummitSceneLoading.OnJoinSubItem?.Invoke(false);
        MutiplayerController.instance.Ontriggered("Wheel", true);
        MutiplayerController.instance.disableSector = true;
    }
}
