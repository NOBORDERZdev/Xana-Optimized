using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        string name = PhotonNetwork.CurrentRoom.CustomProperties["Sector"].ToString();
        if (other.gameObject.tag == "PhotonLocalPlayer")
        {
            if (other.GetComponent<PhotonView>().IsMine)
            {
                if (name != "Wheel")
                {
                    ChangeSector();
                }
                else
                {
                    other.GetComponent<SummitPlayerRPC>().CheckForExitWheel();
                }
            }
        }
     
        if (name == "Wheel")
        {
            if (other.gameObject.tag == "WheelCar")
            {
                
                GiantWheelManager.Instance.CheckForWheelCar();

            }
            if (other.gameObject.tag == "Wheel" && !GiantWheelManager.Instance.CarAdded)
            {

                GiantWheelManager.Instance.AddCar();

            }
          

        }
    }
    async void ChangeSector()
    {
        while (MutiplayerController.instance.isShifting)
        {
            await new WaitForSeconds(1f);
        }
        MutiplayerController.instance.Ontriggered("Wheel", true);
        MutiplayerController.instance.disableSector = true;
    }
}
