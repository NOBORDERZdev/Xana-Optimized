using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class CarNavigationManager : MonoBehaviour
{
    //Public Variables
    public static CarNavigationManager CarNavigationInstance;
    public Action OnExitpress;
    public Action OnCancelPress;
    public Dictionary<int, PhotonView> Cars = new Dictionary<int, PhotonView>();

    //Private variables
    [SerializeField]
    private GameObject CarCanvas;

    private void Awake()
    {
        CarNavigationInstance = this;
    }

    private void Start()
    {
        if (ConstantsHolder.xanaConstants.EnviornmentName == "XANA Summit")
        {
            SummitEntityManager.instance.InstantiateCAR();
        }
        if (SummitCarUIHandler.SummitCarUIHandlerInstance)
        {
            CarCanvas = SummitCarUIHandler.SummitCarUIHandlerInstance.CarCanvas;
            //SummitCarUIHandler.SummitCarUIHandlerInstance.ExitButton.onClick.RemoveAllListeners();
            //SummitCarUIHandler.SummitCarUIHandlerInstance.ExitButton.onClick.AddListener(ExitCar);
        }
    }

    public void StopCar(GameObject car)
    {

        StartCoroutine(WaitAtCarStop(car));


    }
    public async Task TPlayer(GameObject Car, GameObject Players, CarStopTrigger triger)
    {

       await new WaitForSeconds(1.5f);
        var car = Car.GetComponent<SplineFollower>();
        XANASummitSceneLoading.OnJoinSubItem?.Invoke(false);
        var playerrpc = Players.GetComponent<SummitPlayerRPC>();
        if (playerrpc.joiningCar||playerrpc.isInsideCAr) { triger.Pop(); return; }
        if (car.DriverSeatEmpty)
        {
            playerrpc.joiningCar = true;
            playerrpc.EnterCar(car.View.ViewID, true);
            triger.Pop();
            car.DriverSeatEmpty = false;
            return;
        }else
        if (car.PasengerSeatEmty)
        {
            playerrpc.joiningCar = true;
            Players.GetComponent<SummitPlayerRPC>().EnterCar(car.View.ViewID, false);
            triger.Pop();
            car.PasengerSeatEmty = false;
           return;
        }
        //teleport to car pending.....

    }

    IEnumerator WaitAtCarStop(GameObject car)
    {
        yield return new WaitForSeconds(1f);
        car.GetComponent<SplineFollower>().Speed = 0;
        car.GetComponent<SplineFollower>().StopCar = true;
        yield return new WaitForSeconds(2f);
        car.GetComponent<SplineFollower>().Speed = 5;
        car.GetComponent<SplineFollower>().StopCar = false;
    }

    public void EnableExitCanvas()
    {

        CarCanvas.SetActive(true);
        if (SummitCarUIHandler.SummitCarUIHandlerInstance)
        {
            SummitCarUIHandler.SummitCarUIHandlerInstance.ExitButton.gameObject.SetActive(true);
            SummitCarUIHandler.SummitCarUIHandlerInstance.ExitButton.onClick.RemoveAllListeners();
            SummitCarUIHandler.SummitCarUIHandlerInstance.ExitButton.onClick.AddListener(ExitCar);
        }
    }
    public void DisableExitCanvas()
    {
        CarCanvas.SetActive(false);
        if (SummitCarUIHandler.SummitCarUIHandlerInstance)
        {
            SummitCarUIHandler.SummitCarUIHandlerInstance.ExitButton.gameObject.SetActive(false);
        }
    }

    public void ExitCar()
    {
        OnExitpress?.Invoke();

    }
    public void CancelExitCar()
    {
        OnCancelPress?.Invoke();

    }

    public void ExitCar(int carID)
    {
        StartCoroutine(WaitAtCarStop(Cars[carID].gameObject));
    }

    public void EnterCar(int id, bool isDriver)
    {

    }
}
