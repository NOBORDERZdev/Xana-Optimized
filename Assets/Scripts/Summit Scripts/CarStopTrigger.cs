using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CarStopTrigger : MonoBehaviour
{

    private bool StopCar = false;
    bool running = false;
    private List<GameObject> Players= new List<GameObject>();
    private void OnEnable()
    {
        MutiplayerController.onRespawnPlayer += () => { Players.Clear(); StopCar = false; };
    }
    private async void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "PhotonLocalPlayer")
        {

            if (!other.gameObject.GetComponent<SummitPlayerRPC>()) { return; }

            var summitrpc = other.gameObject.GetComponent<SummitPlayerRPC>();
            if (summitrpc.isInsideCAr)
            {

                return;
            }

            if (!Players.Contains(other.gameObject))
            {

                Players.Add(other.gameObject);
                StopCar = true;
            }

        }
        if (running) { return; }
        if (other.gameObject.tag == "CAR" && StopCar && (other.GetComponent<SplineFollower>().DriverSeatEmpty || other.GetComponent<SplineFollower>().PasengerSeatEmty))
        {
            running = true;

            CarNavigationManager.CarNavigationInstance.StopCar(other.gameObject);
            if (PhotonNetwork.IsMasterClient)
            {
                foreach (var player in Players)
                {
                    if (player != null)
                    {

                        await (CarNavigationManager.CarNavigationInstance.TPlayer(other.gameObject, player, this));
                       
                     
                    }

                }
                running = false;
            }
        }

    }

  

    public void Pop()
    {
        Players.RemoveAt(0);
        if (Players.Count == 0)
            StopCar = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "PhotonLocalPlayer")
        {

            var summitrpc = other.gameObject.GetComponent<SummitPlayerRPC>();
          
            if (summitrpc.isInsideCAr)
            {
               
                return;
            }

         
            Players.Remove(other.gameObject);
            if(Players.Count == 0) 
            StopCar = false;

        }
    }

}
