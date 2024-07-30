using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        SummitEntityManager.instance.InstantiateCAR();
        if (SummitCarUIHandler.SummitCarUIHandlerInstance)
        {
            CarCanvas = SummitCarUIHandler.SummitCarUIHandlerInstance.CarCanvas;
            SummitCarUIHandler.SummitCarUIHandlerInstance.ExitButton.onClick.RemoveAllListeners();
            SummitCarUIHandler.SummitCarUIHandlerInstance.ExitButton.onClick.AddListener(ExitCar);
        }
    }

    public void StopCar(GameObject car)
    {

        StartCoroutine(WaitAtCarStop(car));


    }
    public IEnumerator TPlayer(GameObject Car, GameObject Players, CarStopTrigger triger)
    {

        yield return new WaitForSeconds(1.5f);
        var car = Car.GetComponent<SplineFollower>();
        if (car.driverseatempty)
        {
            Players.GetComponent<SummitPlayerRPC>().EnterCar(car.view.ViewID, true);
            triger.Pop();
            yield break;
        }
        if (car.pasengerseatemty)
        {
            Players.GetComponent<SummitPlayerRPC>().EnterCar(car.view.ViewID, false);
            triger.Pop();
            yield break;
        }
        //teleport to car pending.....

    }

    IEnumerator WaitAtCarStop(GameObject car)
    {
        yield return new WaitForSeconds(1f);
        car.GetComponent<SplineFollower>().speed = 0;
        car.GetComponent<SplineFollower>().stopcar = true;
        yield return new WaitForSeconds(2f);
        car.GetComponent<SplineFollower>().speed = 5;
        car.GetComponent<SplineFollower>().stopcar = false;
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
