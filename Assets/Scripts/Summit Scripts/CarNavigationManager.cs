using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class CarNavigationManager : MonoBehaviour
{

    public static CarNavigationManager instance;
    
  
    public Dictionary<int, PhotonView> Cars = new Dictionary<int, PhotonView>();
    
    [SerializeField]
    private GameObject CarCanvas;

    public Action onExitpress,onCancelPress;
  

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
       SummitEntityManager.instance.InstantiateCAR();
    }

    public void StopCar(GameObject car)
    {
        if(PhotonNetwork.IsMasterClient)
        StartCoroutine(WaitAtCarStop(car));

        
    }
    public IEnumerator TPlayer(GameObject Car, List<GameObject> Players ,CarStopTrigger triger)
    {
         
            yield return new WaitForSeconds(1f);
        var car = Car.GetComponent<SplineFollower>();
       if(car.driverseatempty)
        {
            Players[0].GetComponent<SummitPlayerRPC>().EnterCar(car.view.ViewID, true);
            
            triger.Pop();
        }
       if(car.pasengerseatemty && Players.Count>0)
        {
            Players[0].GetComponent<SummitPlayerRPC>().EnterCar(car.view.ViewID, false);
            triger.Pop();
        }
        //teleport to car pending.....

    }

    IEnumerator WaitAtCarStop(GameObject car)
    {
        yield return new WaitForSeconds(.5f);
        car.GetComponent<SplineFollower>().speed = 0;
       // car.GetComponent<SplineFollower>().stopcar = true;
        yield return new WaitForSeconds(3f);
        car.GetComponent<SplineFollower>().speed = 5;
        //car.GetComponent<SplineFollower>().stopcar = false;
    }

    public void EnableExitCanvas()
    {

        CarCanvas.SetActive(true);

    }
    public void DisableExitCanvas()
    {

        CarCanvas.SetActive(false);

    }

    public void ExitCar()
    {
       onExitpress?.Invoke();

    }
    public void CancelExitCar()
    {
        onCancelPress?.Invoke();

    }

    public void ExitCar(int carID)
    {
        StartCoroutine(WaitAtCarStop(Cars[carID].gameObject));
    }
 
    public void EnterCar(int id, bool isDriver)
    {
      
    }
}
