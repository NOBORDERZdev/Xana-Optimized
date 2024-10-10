using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarStopTrigger : MonoBehaviour
{

    private bool StopCar = false;

    private List<GameObject> Players= new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        
        if(other.gameObject.tag == "PhotonLocalPlayer")
        {
            var summitrpc = other.gameObject.GetComponent<SummitPlayerRPC>();
            if (summitrpc.isInsideCAr)
            {
                summitrpc.checkforExit();
                return;
            }
            

           Players.Add(other.gameObject);
            StopCar = true;
            XANASummitSceneLoading.OnJoinSubItem?.Invoke(false);
        }

        if(other.gameObject.tag == "CAR"&& StopCar &&(other.GetComponent<SplineFollower>().driverseatempty|| other.GetComponent<SplineFollower>().pasengerseatemty) )
        {
            
            CarNavigationManager.CarNavigationInstance.StopCar(other.gameObject);
            if (PhotonNetwork.IsMasterClient)
            {foreach (var player in Players)
                {
                    if (player != null)
                    {
                        StartCoroutine(CarNavigationManager.CarNavigationInstance.TPlayer(other.gameObject, player, this));
                    }
                }
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
