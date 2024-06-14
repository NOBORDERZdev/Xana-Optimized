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
        }

        if(other.gameObject.tag == "CAR"&& StopCar &&(other.GetComponent<SplineFollower>().driverseatempty|| other.GetComponent<SplineFollower>().pasengerseatemty) && PhotonNetwork.IsMasterClient)
        {
            CarNavigationManager.instance.StopCar(other.gameObject);
            StartCoroutine(CarNavigationManager.instance.TPlayer(other.gameObject, Players,this));
        }
        

    }

    public void Pop()
    {
        Debug.Log("Pop Player from waiting"); 
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
