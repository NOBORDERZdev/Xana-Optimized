using Photon.Pun;
using Photon.Voice.PUN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummitPlayerRPC : MonoBehaviour
{
    [SerializeField]
    private PhotonView view;
    private PhotonVoiceNetwork voiceNetwork;

   
    
    private Transform Parent;
    private bool isdriver;
    private string Group;

    public bool isInsideCAr = false;
    private bool showExit = false;
    int carID;

 

    private void Awake()
    {
        voiceNetwork = FindObjectOfType<PhotonVoiceNetwork>();
    }

    // Start is called before the first frame update
    public void ExitCar()
    {
        view.RPC("ExitCAr", RpcTarget.All);

    }

    [PunRPC]
    void ExitCAr()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            CarNavigationManager.instance.ExitCar(carID);
        }

      
        StartCoroutine(exitCoolDown());
    

    }
    IEnumerator exitCoolDown()
    {
        yield return new WaitForSeconds(1) ;
        var car = CarNavigationManager.instance.Cars[carID].gameObject.GetComponent<SplineFollower>();
        GetComponent<Animator>().SetTrigger("ExitCar");
        showExit = false;
        ConstantsHolder.TempDiasableMultiPartPhoton = false;
        if (isdriver)
        {

            car.driverseatempty = true;
            if (view.IsMine)
            {


                CarNavigationManager.instance.DisableExitCanvas();
                transform.parent.transform.parent = Parent;
                transform.parent.transform.position = car.DriverExitPosition.transform.position;

                transform.parent.gameObject.GetComponent<CharacterController>().enabled = true;
                transform.parent.gameObject.GetComponent<PlayerController>().enabled = true;
                gameObject.GetComponent<CharacterController>().enabled = true;
                gameObject.GetComponent<ArrowManager>().enabled = true;


            }
            else
            {
                gameObject.GetComponent<CharacterController>().enabled = true;
                gameObject.GetComponent<ArrowManager>().enabled = true;

                transform.parent = Parent;
                transform.position = car.DriverExitPosition.transform.position;
            }
            //transform.position = car.DriverPos;
            car.hidelove();

        }
        else
        {
            car.pasengerseatemty = true;
            if (GetComponent<PhotonView>().IsMine)
            {


                CarNavigationManager.instance.DisableExitCanvas();
                transform.parent.transform.parent = Parent;
                transform.parent.transform.position = car.PassengerExitPosition.transform.position;
                transform.parent.gameObject.GetComponent<CharacterController>().enabled = true;
                transform.parent.gameObject.GetComponent<PlayerController>().enabled = true;
                gameObject.GetComponent<CharacterController>().enabled = true;
                gameObject.GetComponent<ArrowManager>().enabled = true;


            }
            else
            {
                gameObject.GetComponent<CharacterController>().enabled = true;
                gameObject.GetComponent<ArrowManager>().enabled = true;

                transform.parent = Parent;
                transform.position = car.PassengerExitPosition.transform.position;
            }
            car.hidelove();
        }


        Debug.Log("RoomChanger " + voiceNetwork.Client.OpChangeGroups(new byte[] { car.PrivateRoomName }, new byte[] { voiceNetwork.Client.GlobalInterestGroup }));
        yield return new WaitForSeconds(2);
        isInsideCAr = false;
    }

    public void EnterCar(int id, bool isDriver)
    {
        view.RPC("EnterCAr", RpcTarget.All, id, isDriver);

    }

    [PunRPC]
    void EnterCAr(int id, bool isDriver)
    {

        var car = CarNavigationManager.instance.Cars[id].gameObject.GetComponent<SplineFollower>();
        this.isdriver = isDriver;
        isInsideCAr = true;
        ConstantsHolder.TempDiasableMultiPartPhoton = true;
        carID = id;
        if (isDriver)
        {

            car.driverseatempty = false;
            if (view.IsMine)
            {
                transform.parent.gameObject.GetComponent<CharacterController>().enabled = false;
                transform.parent.gameObject.GetComponent<PlayerController>().enabled = false;
                gameObject.GetComponent<CharacterController>().enabled = false;
                gameObject.GetComponent<ArrowManager>().enabled = false;

                Parent = transform.parent.transform.parent;
                transform.parent.transform.parent = car.transform;
                transform.localPosition = Vector3.zero;
                transform.parent.transform.localPosition = car.DriverPosition.transform.localPosition;
                CarNavigationManager.instance.EnableExitCanvas();
                transform.rotation = new Quaternion(0, 0, 0, 0);
                transform.parent.transform.rotation = new Quaternion(0, 0, 0, 0);

                CarNavigationManager.instance.onExitpress += Exit;
                CarNavigationManager.instance.onCancelPress += CancelExit;

            }
            else
            {
                gameObject.GetComponent<CharacterController>().enabled = false;
                gameObject.GetComponent<ArrowManager>().enabled = false;
                Parent = transform.parent;
                transform.parent = car.transform;
                transform.localPosition = car.DriverPosition.transform.localPosition;
            }
            //transform.position = car.DriverPos;
            if (gameObject.name.Contains("XanaAvatar2.0_Female"))
            {
                car.isDriverMale = false;
            }
            if (!car.isPassengerMale && car.isDriverMale && car.pasengerseatemty && car.driverseatempty)
            {
                car.showLove();
            }
            else if (car.isPassengerMale && !car.isDriverMale && car.pasengerseatemty && car.driverseatempty)
            {
                car.showLove();
            }
            GetComponent<Animator>().SetTrigger("EnterCar");

        }
        else
        {
            car.pasengerseatemty = false;
            if (GetComponent<PhotonView>().IsMine)
            {
                transform.parent.gameObject.GetComponent<CharacterController>().enabled = false;
                transform.parent.gameObject.GetComponent<PlayerController>().enabled = false;
                gameObject.GetComponent<CharacterController>().enabled = false;
                gameObject.GetComponent<ArrowManager>().enabled = false;

                Parent = transform.parent.transform.parent;
                transform.parent.transform.parent = car.transform;
                transform.localPosition = Vector3.zero;
                transform.parent.transform.localPosition = car.PacengerPosition.transform.localPosition;
                CarNavigationManager.instance.EnableExitCanvas();
             
                transform.rotation = new Quaternion(0, 0, 0, 0);
                transform.parent.transform.rotation = new Quaternion(0, 0, 0, 0);
                CarNavigationManager.instance.onExitpress += Exit;
                CarNavigationManager.instance.onCancelPress += CancelExit;

            }
            else
            {
                gameObject.GetComponent<CharacterController>().enabled = false;
                gameObject.GetComponent<ArrowManager>().enabled = false;
                Parent = transform.parent;
                transform.parent = car.transform;
                transform.localPosition = car.PacengerPosition.transform.localPosition;
            }
            GetComponent<Animator>().SetTrigger("EnterCar");
            if (gameObject.name.Contains("XanaAvatar2.0_Female"))
            {
                car.isPassengerMale = false;
            }
            if (!car.isPassengerMale && car.isDriverMale && car.pasengerseatemty && car.driverseatempty)
            {
                car.showLove();
            }
            else if (car.isPassengerMale && !car.isDriverMale && car.pasengerseatemty && car.driverseatempty)
            {
                car.showLove();
            }
            Debug.Log("RoomChanger " + voiceNetwork.Client.OpChangeGroups(new byte[] { voiceNetwork.Client.GlobalInterestGroup }, new byte[] { car.PrivateRoomName }));
           
        }

    }

    public void Exit()
    {
        showExit = true;
        CarNavigationManager.instance.DisableExitCanvas();
    }
    public void CancelExit()
    {
       showExit = false;
    }

    public void checkforExit()
    {
        if (view.IsMine)
        {
            if (showExit)
            {
                ExitCar();
            }
        }
    }
    public void checkforDisableExit()
    {
       if(view.IsMine)
            CarNavigationManager.instance.DisableExitCanvas();
        
    }
}
