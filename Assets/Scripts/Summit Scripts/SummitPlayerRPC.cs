using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using Photon.Voice.PUN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;


public class SummitPlayerRPC : MonoBehaviour,IInRoomCallbacks
{
    [SerializeField]
    public PhotonView view;
    private PunVoiceClient voiceNetwork;


    public CharacterController charcontroller;
    public ArrowManager arrowManager;
    public PhotonTransformView Transformview;
    public PhotonAnimatorView AnimatorView;
    public PhotonVoiceView VoiceView;

    public Animator animator;
    private CharacterController parentCharacterController;
    private PlayerController parentPlayerController;
    private Camera camera;

    private Transform Parent;
    private bool isdriver;
    private bool StopCar = false;
    private string Group;
    
    public bool isInsideCAr = false;
    private bool showExit = false;
    int carID;

    private GameplayEntityLoader loader ;
    
    public bool isInsideWheel = false;
    int WheelSeat = -1;

    private void Awake()
    {
        isInsideCAr = false;
        PhotonNetwork.AddCallbackTarget(this);

        if (view.IsMine)
        {
            loader = GameplayEntityLoader.instance;
            parentCharacterController = loader.mainController.GetComponent<CharacterController>();
            parentPlayerController = loader.mainController.GetComponent<PlayerController>();
            camera = parentPlayerController.firstPersonCameraObj.GetComponent<Camera>();
        }
    }

    // Start is called before the first frame update
    public void ExitCar()
    {
        view.RPC("ExitCAr", RpcTarget.All);
        GameplayEntityLoader.instance.IsJoinSummitWorld = false;
    }

    private void FixedUpdate()
    {
        

        if(StopCar  && CarNavigationManager.CarNavigationInstance)
        {
            PhotonView carview;
            CarNavigationManager.CarNavigationInstance.Cars.TryGetValue(carID, out carview);
            if(carview == null) { return; }

            StopCar = false;
            var car = carview.gameObject.GetComponent<SplineFollower>();
           
            isInsideCAr = true;
            
         
            if (isdriver)
            {

                car.DriverSeatEmpty = false;
                if (view.IsMine)
                {
                    ConstantsHolder.TempDiasableMultiPartPhoton = true;
                    parentCharacterController.enabled = false;
                     parentPlayerController.enabled = false;
                    charcontroller.enabled = false;
                    arrowManager.enabled = false;
                    Transformview.enabled = false;
                 

                    Parent = loader.mainPlayer.transform;
                    loader.mainController.transform.parent =  car.transform;
                    transform.localPosition = Vector3.zero;
                     loader.mainController.transform.localPosition = car.DriverPosition.transform.localPosition;


                    PlayerCameraController.instance.EnableCameraRecenter();
                    CarNavigationManager.CarNavigationInstance.EnableExitCanvas();
                    SummitCarUIHandler.SummitCarUIHandlerInstance.UpdateUIelement(false);


                    transform.rotation = new Quaternion(0, 0, 0, 0);
                     loader.mainController.transform.rotation = new Quaternion(0, 0, 0, 0);

                    /* ZEEL 
                    if (voiceNetwork == null) { voiceNetwork = PunVoiceClient.Instance; }
                    Debug.Log("RoomChanger " + voiceNetwork.Client.OpChangeGroups(new byte[] { voiceNetwork.Client.GlobalInterestGroup }, new byte[] { car.PrivateRoomName }));
                    */
                    CarNavigationManager.CarNavigationInstance.OnExitpress += Exit;
                    CarNavigationManager.CarNavigationInstance.OnCancelPress += CancelExit;

                }
                else
                {
                    Transformview.enabled = false;
                   charcontroller.enabled = false;
                   arrowManager.enabled = false;


                    Parent = transform.parent;
                    GameObject gasme = new GameObject();
                    gasme.transform.parent = car.transform;
                    transform.parent = gasme.transform;
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = new Quaternion(0, 0, 0, 0);
                    gasme.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
                    gasme.transform.localPosition = car.DriverPosition.transform.localPosition;
                    gasme.transform.localRotation = new Quaternion(0, 0, 0, 0);
                }
                car.PlayerListinCar.Add(view.Owner, 0);
                //transform.position = car.DriverPos;
                if (gameObject.name.Contains("XanaAvatar2.0_Female"))
                {
                    car._isDriverMale = false;
                }
                if (!car._isPassengerMale && car._isDriverMale && !car.PasengerSeatEmty && !car.DriverSeatEmpty)
                {
                    car.showLove();
                }
                else if (car._isPassengerMale && !car._isDriverMale && !car.PasengerSeatEmty && car.DriverSeatEmpty)
                {
                    car.showLove();
                }
               animator.SetTrigger("EnterCar");

            }
            else
            {
                car.PasengerSeatEmty = false;
                if (view.IsMine)
                {
                    ConstantsHolder.TempDiasableMultiPartPhoton = true;
                     parentCharacterController.enabled = false;
                     parentPlayerController.enabled = false;
                   charcontroller.enabled = false;
                   arrowManager.enabled = false;
                    Transformview.enabled = false;
                    PlayerCameraController.instance.EnableCameraRecenter();
                    Parent = loader.mainPlayer.transform;
                    loader.mainController.transform.parent =  car.transform;
                    transform.localPosition = Vector3.zero;
                     loader.mainController.transform.localPosition = car.PacengerPosition.transform.localPosition;
                    CarNavigationManager.CarNavigationInstance.EnableExitCanvas();
                    SummitCarUIHandler.SummitCarUIHandlerInstance.UpdateUIelement(false);
                    transform.rotation = new Quaternion(0, 0, 0, 0);
                     loader.mainController.transform.rotation = new Quaternion(0, 0, 0, 0);
                    CarNavigationManager.CarNavigationInstance.OnExitpress += Exit;
                    CarNavigationManager.CarNavigationInstance.OnCancelPress += CancelExit;
                    /* ZEEL 
                    if (voiceNetwork == null ) { voiceNetwork = PunVoiceClient.Instance; }
                    Debug.Log("RoomChanger " + voiceNetwork.Client.OpChangeGroups(new byte[] { voiceNetwork.Client.GlobalInterestGroup }, new byte[] { car.PrivateRoomName }));
                    */
                }
                else
                {
                   charcontroller.enabled = false;
                   arrowManager.enabled = false;
                    Transformview.enabled = false;


                    Parent = transform.parent;
                    GameObject gasme = new GameObject();
                    gasme.transform.parent = car.transform;
                    transform.parent = gasme.transform;
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = new Quaternion(0, 0, 0, 0);
                    gasme.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
                    gasme.transform.localPosition = car.PacengerPosition.transform.localPosition;
                    gasme.transform.localRotation = new Quaternion(0, 0, 0, 0);
                }
                car.PlayerListinCar.Add(view.Owner, 1);
               animator.SetTrigger("EnterCar");
                if (gameObject.name.Contains("XanaAvatar2.0_Female"))
                {
                    car._isPassengerMale = false;
                }
                if (!car._isPassengerMale && car._isDriverMale && !car.PasengerSeatEmty && !car.DriverSeatEmpty)
                {
                    car.showLove();
                }
                else if (car._isPassengerMale && !car._isDriverMale && !car.PasengerSeatEmty && !car.DriverSeatEmpty)
                {
                    car.showLove();
                }


            }
        }
        
    }

    [PunRPC]
    void ExitCAr()
    {

            CarNavigationManager.CarNavigationInstance.ExitCar(carID);
            StartCoroutine(exitCoolDown());

    }
    IEnumerator exitCoolDown()
    {
        yield return new WaitForSeconds(1.5f) ;
        var car = CarNavigationManager.CarNavigationInstance.Cars[carID].gameObject.GetComponent<SplineFollower>();
       animator.SetTrigger("ExitCar");
        showExit = false;
     
        if (isdriver)
        {

            car.DriverSeatEmpty = true;
            if (view.IsMine)
            {

                ConstantsHolder.TempDiasableMultiPartPhoton = false;
                CarNavigationManager.CarNavigationInstance.DisableExitCanvas();
                SummitCarUIHandler.SummitCarUIHandlerInstance.UpdateUIelement(true);
                loader.mainController.transform.parent =  Parent;
                 loader.mainController.transform.position = car.DriverExitPosition.transform.position;
                PlayerCameraController.instance.DisableCameraRecenter();
                 parentCharacterController.enabled = true;
                 parentPlayerController.enabled = true;
               charcontroller.enabled = true;
               arrowManager.enabled = true;
                Transformview.enabled = true;
                /* ZEEL 
                if (voiceNetwork == null) { voiceNetwork = PunVoiceClient.Instance; }
                Debug.Log("RoomChanger " + voiceNetwork.Client.OpChangeGroups(new byte[] { car.PrivateRoomName }, new byte[] { voiceNetwork.Client.GlobalInterestGroup }));
                */
                }
                else
            {
                transform.parent = loader.mainPlayer.transform.transform.parent;
                transform.position = car.DriverExitPosition.transform.position;
                transform.localScale = Vector3.one * 1.14f;
               charcontroller.enabled = true;
               arrowManager.enabled = true;
                Transformview.enabled = true;
              
            }
            car.PlayerListinCar.Remove(view.Owner);
            //transform.position = car.DriverPos;
           car.hidelove();

        }
        else
        {
            car.PasengerSeatEmty = true;
            if (view.IsMine)
            {

                ConstantsHolder.TempDiasableMultiPartPhoton = false;    
                CarNavigationManager.CarNavigationInstance.DisableExitCanvas();
                SummitCarUIHandler.SummitCarUIHandlerInstance.UpdateUIelement(true);
                loader.mainController.transform.parent =  Parent;
                 loader.mainController.transform.position = car.PassengerExitPosition.transform.position;
                 parentCharacterController.enabled = true;
                 parentPlayerController.enabled = true;
               charcontroller.enabled = true;
               arrowManager.enabled = true;
                Transformview.enabled = true;
                PlayerCameraController.instance.DisableCameraRecenter();
                if (voiceNetwork == null) { voiceNetwork = PunVoiceClient.Instance; }
             

            }
            else
            {
                transform.parent = loader.mainPlayer.transform.transform.parent; ;
                transform.position = car.PassengerExitPosition.transform.position;
                transform.localScale = Vector3.one * 1.14f;
               charcontroller.enabled = true;
               arrowManager.enabled = true;
                Transformview.enabled = true;
               
            }
            car.PlayerListinCar.Remove(view.Owner);

            car.hidelove();
        }


        
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
        carID = id;
       this.isdriver = isDriver;
        StopCar = true;
     //   WaitforInstance(id, isDriver);

    }


   
    public async Task WaitforInstance(int id, bool isDriver)
    {
        while (!CarNavigationManager.CarNavigationInstance|| CarNavigationManager.CarNavigationInstance.Cars.Count<8)
        {
            await new WaitForSeconds(1f);
        }

   
    }
    

    public  void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (ConstantsHolder.xanaConstants.EnviornmentName != "XANA Summit") { return; }

            Debug.LogError("OnPlayerEnteredRoo");
        if (isInsideCAr && view.IsMine)
        {
            view.RPC("EnterCAr", newPlayer, carID, isdriver);
        }



        string name = PhotonNetwork.CurrentRoom.CustomProperties["Sector"].ToString();
        if (name == "Wheel")
        {
            if (isInsideWheel && view.IsMine)
            {
                view.RPC("EnterWheel", newPlayer, WheelSeat);
            }

          
        }
  }
    private void Start()
    {
         loader =  GameplayEntityLoader.instance;
        if (ConstantsHolder.xanaConstants.EnviornmentName != "XANA Summit") { return; }
        string name = PhotonNetwork.CurrentRoom.CustomProperties["Sector"].ToString();
        if (name == "Wheel")
        {
            getcar();
        }
        
    }

    async void getcar()
    {
        while (GiantWheelManager.Instance.car == null)
        {
            await new WaitForSeconds(1);
            Debug.Log("Stuck in loop");
        }
      
        if(view.Owner.IsMasterClient &&view.IsMine)
        {
            CallEnterWheelRPC();
        }
        else
        {
            waitForWheel();
        }


    }

    [PunRPC]
    public void EnterWheel(int wheelSeat)
    {
        string name = PhotonNetwork.CurrentRoom.CustomProperties["Sector"].ToString();
        if (name == "Wheel")
        {
            
                ConstantsHolder.DisableFppRotation = true;
                isInsideCAr = true;
                if (wheelSeat==1)
                {

                    var car = GiantWheelManager.Instance.car;
                    car.isfirstPlayerEmpty = false;
                    WheelSeat = 1;
                    car.PlayerSeat.Add(view.Owner, 1);
                    if (view.IsMine)
                    {
                        LOD = new List<LODGroup>(FindObjectsOfType<LODGroup>());
                        foreach (var item in LOD)
                        {
                            item.enabled = false;
                        }

                         parentCharacterController.enabled = false;
                         parentPlayerController.enabled = false;
                       charcontroller.enabled = false;
                       arrowManager.enabled = false;
                        Transformview.enabled = false;
                        Parent = loader.mainPlayer.transform;
                        loader.mainController.transform.parent =  car.FirstPlayerPos;
                        transform.localPosition = Vector3.zero;
                         loader.mainController.transform.localPosition = Vector3.zero;
                        GiantWheelManager.Instance.WheelCar.SetActive(false);
                        CarNavigationManager.CarNavigationInstance.EnableExitCanvas();
                        transform.rotation = new Quaternion(0, 0, 0, 0);
                         loader.mainController.transform.rotation = new Quaternion(0, 0, 0, 0);
                    SummitCarUIHandler.SummitCarUIHandlerInstance.UpdateUIelement(false);
                    CarNavigationManager.CarNavigationInstance.OnExitpress += Exit;
                        CarNavigationManager.CarNavigationInstance.OnCancelPress += CancelExit;
                       camera.useOcclusionCulling = false;
                        GamePlayButtonEvents.inst.OnSwitchCameraClick();
                    }
                    else
                    {
                        Transformview.enabled = false;
                       charcontroller.enabled = false;
                       arrowManager.enabled = false;


                        Parent = transform.parent;
                        GameObject gasme = new GameObject();
                        gasme.transform.parent = car.FirstPlayerPos;
                        transform.parent = gasme.transform;
                        transform.localPosition = Vector3.zero;
                        transform.localRotation = new Quaternion(0, 0, 0, 0);
                        gasme.transform.localScale = Vector3.one;
                        gasme.transform.localPosition = Vector3.zero;
                        gasme.transform.localRotation = new Quaternion(0, 0, 0, 0);
                    }


                }
                else
                         if (wheelSeat ==2)
                {
                    var car = GiantWheelManager.Instance.car;
                    car.issecondPlayerEmpty = false;
                    WheelSeat = 2;
                    car.PlayerSeat.Add(view.Owner, 2);
                    if (view.IsMine)
                    {
                        LOD = new List<LODGroup>(FindObjectsOfType<LODGroup>());
                        foreach (var item in LOD)
                        {
                            item.enabled = false;
                        }
                         parentCharacterController.enabled = false;
                         parentPlayerController.enabled = false;
                       charcontroller.enabled = false;
                       arrowManager.enabled = false;
                        Transformview.enabled = false;
                        Parent = loader.mainPlayer.transform;
                        loader.mainController.transform.parent =  car.SecondPlayerPos;
                        transform.localPosition = Vector3.zero;
                         loader.mainController.transform.localPosition = Vector3.zero;
                    SummitCarUIHandler.SummitCarUIHandlerInstance.UpdateUIelement(false);
                    CarNavigationManager.CarNavigationInstance.EnableExitCanvas();
                        transform.rotation = new Quaternion(0, 0, 0, 0);
                         loader.mainController.transform.rotation = new Quaternion(0, 0, 0, 0);

                        CarNavigationManager.CarNavigationInstance.OnExitpress += Exit;
                        CarNavigationManager.CarNavigationInstance.OnCancelPress += CancelExit;
                       camera.useOcclusionCulling = false;
                        GamePlayButtonEvents.inst.OnSwitchCameraClick();
                    }
                    else
                    {
                        Transformview.enabled = false;
                       charcontroller.enabled = false;
                       arrowManager.enabled = false;


                        Parent = transform.parent;
                        GameObject gasme = new GameObject();
                        gasme.transform.parent = car.SecondPlayerPos;
                        transform.parent = gasme.transform;
                        transform.localPosition = Vector3.zero;
                        transform.localRotation = new Quaternion(0, 0, 0, 0);
                        gasme.transform.localScale = Vector3.one;
                        gasme.transform.localPosition = Vector3.zero;
                        gasme.transform.localRotation = new Quaternion(0, 0, 0, 0);
                    }


                }
                else
                         if (wheelSeat == 3)
                {
                    var car = GiantWheelManager.Instance.car;
                    car.isThirdPlayerEmpty = false;
                    WheelSeat = 3;
                    car.PlayerSeat.Add(view.Owner, 3);
                    if (view.IsMine)
                    {
                        LOD = new List<LODGroup>(FindObjectsOfType<LODGroup>());
                        foreach (var item in LOD)
                        {
                            item.enabled = false;
                        }
                         parentCharacterController.enabled = false;
                         parentPlayerController.enabled = false;
                       charcontroller.enabled = false;
                       arrowManager.enabled = false;
                        Transformview.enabled = false;
                        Parent = loader.mainPlayer.transform;
                        loader.mainController.transform.parent =  car.ThirdPlayerPos;
                        transform.localPosition = Vector3.zero;
                         loader.mainController.transform.localPosition = Vector3.zero;
                    SummitCarUIHandler.SummitCarUIHandlerInstance.UpdateUIelement(false);
                    CarNavigationManager.CarNavigationInstance.EnableExitCanvas();
                        transform.rotation = new Quaternion(0, 0, 0, 0);
                         loader.mainController.transform.rotation = new Quaternion(0, 0, 0, 0);

                        CarNavigationManager.CarNavigationInstance.OnExitpress += Exit;
                        CarNavigationManager.CarNavigationInstance.OnCancelPress += CancelExit;
                       camera.useOcclusionCulling = false;
                        GamePlayButtonEvents.inst.OnSwitchCameraClick();
                    }
                    else
                    {
                        Transformview.enabled = false;
                       charcontroller.enabled = false;
                       arrowManager.enabled = false;


                        Parent = transform.parent;
                        GameObject gasme = new GameObject();
                        gasme.transform.parent = car.ThirdPlayerPos;
                        transform.parent = gasme.transform;
                        transform.localPosition = Vector3.zero;
                        transform.localRotation = new Quaternion(0, 0, 0, 0);
                        gasme.transform.localScale = Vector3.one;
                        gasme.transform.localPosition = Vector3.zero;
                        gasme.transform.localRotation = new Quaternion(0, 0, 0, 0);
                    }


                }
                else
                         if (wheelSeat==4)
                {
                    var car = GiantWheelManager.Instance.car;
                    car.isfirstPlayerEmpty = false;
                    WheelSeat = 4;
                    car.PlayerSeat.Add(view.Owner, 4);
                    if (view.IsMine)
                    {
                        LOD = new List<LODGroup>(FindObjectsOfType<LODGroup>());
                        foreach (var item in LOD)
                        {
                            item.enabled = false;
                        }
                         parentCharacterController.enabled = false;
                         parentPlayerController.enabled = false;
                       charcontroller.enabled = false;
                       arrowManager.enabled = false;
                        Transformview.enabled = false;
                        Parent = loader.mainPlayer.transform;
                        loader.mainController.transform.parent =  car.ForthPlayerPos;
                        transform.localPosition = Vector3.zero;
                         loader.mainController.transform.localPosition = Vector3.zero;
                    SummitCarUIHandler.SummitCarUIHandlerInstance.UpdateUIelement(false);
                    CarNavigationManager.CarNavigationInstance.EnableExitCanvas();
                        transform.rotation = new Quaternion(0, 0, 0, 0);
                         loader.mainController.transform.rotation = new Quaternion(0, 0, 0, 0);

                        CarNavigationManager.CarNavigationInstance.OnExitpress += Exit;
                        CarNavigationManager.CarNavigationInstance.OnCancelPress += CancelExit;
                       camera.useOcclusionCulling = false;

                        GamePlayButtonEvents.inst.OnSwitchCameraClick();
                    }
                    else
                    {
                        Transformview.enabled = false;
                       charcontroller.enabled = false;
                       arrowManager.enabled = false;


                        Parent = transform.parent;
                        GameObject gasme = new GameObject();
                        gasme.transform.parent = car.ForthPlayerPos;
                        transform.parent = gasme.transform;
                        transform.localPosition = Vector3.zero;
                        transform.localRotation = new Quaternion(0, 0, 0, 0);
                        gasme.transform.localScale = Vector3.one;
                        gasme.transform.localPosition = Vector3.zero;
                        gasme.transform.localRotation = new Quaternion(0, 0, 0, 0);
                    }


                }
               animator.SetTrigger("EnterCar");
            

        }

    }

    [PunRPC]
    public void WheelStoped()
    {
        GiantWheelManager.Instance.EnterwheelCar?.Invoke();
        
    }
    private List<LODGroup> LOD;
    [PunRPC]
    public void EnterWheelCar()
    {
        Debug.Log("RPC Received");
     if(GiantWheelManager.Instance.car != null)
        {
            ConstantsHolder.DisableFppRotation = true;
            isInsideCAr = true;
          if(GiantWheelManager.Instance.car.isfirstPlayerEmpty)
            {

                var car = GiantWheelManager.Instance.car;
                car.isfirstPlayerEmpty = false;
                WheelSeat = 1;
                car.PlayerSeat.Add(view.Owner, 1);
                if (view.IsMine)
                {
                    LOD = new List<LODGroup>(FindObjectsOfType<LODGroup>());
                    foreach (var item in LOD)
                    {
                        item.enabled = false;
                    }
                    
                     parentCharacterController.enabled = false;
                     parentPlayerController.enabled = false;
                   charcontroller.enabled = false;
                   arrowManager.enabled = false;
                    Transformview.enabled = false;
                    Parent = loader.mainPlayer.transform;
                    loader.mainController.transform.parent =  car.FirstPlayerPos;
                    transform.localPosition = Vector3.zero;
                     loader.mainController.transform.localPosition = Vector3.zero;
                    GiantWheelManager.Instance.WheelCar.SetActive(false);
                    CarNavigationManager.CarNavigationInstance.EnableExitCanvas();
                    transform.rotation = new Quaternion(0, 0, 0, 0);
                     loader.mainController.transform.rotation = new Quaternion(0, 0, 0, 0);
                    SummitCarUIHandler.SummitCarUIHandlerInstance.UpdateUIelement(false);
                    CarNavigationManager.CarNavigationInstance.OnExitpress += Exit;
                    CarNavigationManager.CarNavigationInstance.OnCancelPress += CancelExit;
                   camera.useOcclusionCulling = false;
                    GamePlayButtonEvents.inst.OnSwitchCameraClick();
                }
                else
                {
                    Transformview.enabled = false;
                   charcontroller.enabled = false;
                   arrowManager.enabled = false;

                   
                    Parent = transform.parent;
                    GameObject gasme = new GameObject();
                    gasme.transform.parent = car.FirstPlayerPos;
                    transform.parent = gasme.transform;
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = new Quaternion(0, 0, 0, 0);
                    gasme.transform.localScale = Vector3.one;
                    gasme.transform.localPosition = Vector3.zero;
                    gasme.transform.localRotation = new Quaternion(0, 0, 0, 0);
                }


            }else
                   if (GiantWheelManager.Instance.car.issecondPlayerEmpty)
            {
                var car = GiantWheelManager.Instance.car;
                car.issecondPlayerEmpty = false;
                WheelSeat = 2;
                car.PlayerSeat.Add(view.Owner, 2);
                if (view.IsMine)
                {
                    LOD = new List<LODGroup>(FindObjectsOfType<LODGroup>());
                    foreach (var item in LOD)
                    {
                        item.enabled = false;
                    }
                     parentCharacterController.enabled = false;
                     parentPlayerController.enabled = false;
                   charcontroller.enabled = false;
                   arrowManager.enabled = false;
                    Transformview.enabled = false;
                    Parent = loader.mainPlayer.transform;
                    loader.mainController.transform.parent =  car.SecondPlayerPos;
                    transform.localPosition = Vector3.zero;
                     loader.mainController.transform.localPosition = Vector3.zero;
                    SummitCarUIHandler.SummitCarUIHandlerInstance.UpdateUIelement(false);
                    CarNavigationManager.CarNavigationInstance.EnableExitCanvas();
                    transform.rotation = new Quaternion(0, 0, 0, 0);
                     loader.mainController.transform.rotation = new Quaternion(0, 0, 0, 0);

                    CarNavigationManager.CarNavigationInstance.OnExitpress += Exit;
                    CarNavigationManager.CarNavigationInstance.OnCancelPress += CancelExit;
                   camera.useOcclusionCulling = false;
                    GamePlayButtonEvents.inst.OnSwitchCameraClick();
                }
                else
                {
                    Transformview.enabled = false;
                   charcontroller.enabled = false;
                   arrowManager.enabled = false;


                    Parent = transform.parent;
                    GameObject gasme = new GameObject();
                    gasme.transform.parent = car.SecondPlayerPos;
                    transform.parent = gasme.transform;
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = new Quaternion(0, 0, 0, 0);
                    gasme.transform.localScale = Vector3.one;
                    gasme.transform.localPosition = Vector3.zero;
                    gasme.transform.localRotation = new Quaternion(0, 0, 0, 0);
                }


            }
            else
                   if (GiantWheelManager.Instance.car.isThirdPlayerEmpty)
            {
                var car = GiantWheelManager.Instance.car;
                car.isThirdPlayerEmpty = false;
                WheelSeat = 3;
                car.PlayerSeat.Add(view.Owner, 3);
                if (view.IsMine)
                {
                    LOD = new List<LODGroup>(FindObjectsOfType<LODGroup>());
                    foreach (var item in LOD)
                    {
                        item.enabled = false;
                    }
                     parentCharacterController.enabled = false;
                     parentPlayerController.enabled = false;
                   charcontroller.enabled = false;
                   arrowManager.enabled = false;
                    Transformview.enabled = false;
                    Parent = loader.mainPlayer.transform;
                    loader.mainController.transform.parent =  car.ThirdPlayerPos;
                    transform.localPosition = Vector3.zero;
                     loader.mainController.transform.localPosition = Vector3.zero;
                    SummitCarUIHandler.SummitCarUIHandlerInstance.UpdateUIelement(false);
                    CarNavigationManager.CarNavigationInstance.EnableExitCanvas();
                    transform.rotation = new Quaternion(0, 0, 0, 0);
                     loader.mainController.transform.rotation = new Quaternion(0, 0, 0, 0);

                    CarNavigationManager.CarNavigationInstance.OnExitpress += Exit;
                    CarNavigationManager.CarNavigationInstance.OnCancelPress += CancelExit;
                   camera.useOcclusionCulling = false;
                    GamePlayButtonEvents.inst.OnSwitchCameraClick();
                }
                else
                {
                    Transformview.enabled = false;
                   charcontroller.enabled = false;
                   arrowManager.enabled = false;


                    Parent = transform.parent;
                    GameObject gasme = new GameObject();
                    gasme.transform.parent = car.ThirdPlayerPos;
                    transform.parent = gasme.transform;
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = new Quaternion(0, 0, 0, 0);
                    gasme.transform.localScale = Vector3.one;
                    gasme.transform.localPosition = Vector3.zero;
                    gasme.transform.localRotation = new Quaternion(0, 0, 0, 0);
                }


            }
            else
                   if (GiantWheelManager.Instance.car.isForthPlayerEmpty)
            {
                var car = GiantWheelManager.Instance.car;
                car.isfirstPlayerEmpty = false;
                WheelSeat = 4;
                car.PlayerSeat.Add(view.Owner, 4);
                if (view.IsMine)
                {
                    LOD = new List<LODGroup>(FindObjectsOfType<LODGroup>());
                    foreach (var item in LOD)
                    {
                        item.enabled = false;
                    }
                     parentCharacterController.enabled = false;
                     parentPlayerController.enabled = false;
                   charcontroller.enabled = false;
                   arrowManager.enabled = false;
                    Transformview.enabled = false;
                    Parent = loader.mainPlayer.transform;
                    loader.mainController.transform.parent =  car.ForthPlayerPos;
                    transform.localPosition = Vector3.zero;
                     loader.mainController.transform.localPosition = Vector3.zero;
                    SummitCarUIHandler.SummitCarUIHandlerInstance.UpdateUIelement(false);
                    CarNavigationManager.CarNavigationInstance.EnableExitCanvas();
                    transform.rotation = new Quaternion(0, 0, 0, 0);
                     loader.mainController.transform.rotation = new Quaternion(0, 0, 0, 0);

                    CarNavigationManager.CarNavigationInstance.OnExitpress += Exit;
                    CarNavigationManager.CarNavigationInstance.OnCancelPress += CancelExit;
                   camera.useOcclusionCulling = false;
                    
                    GamePlayButtonEvents.inst.OnSwitchCameraClick();
                }
                else
                {
                    Transformview.enabled = false;
                   charcontroller.enabled = false;
                   arrowManager.enabled = false;


                    Parent = transform.parent;
                    GameObject gasme = new GameObject();
                    gasme.transform.parent = car.ForthPlayerPos;
                    transform.parent = gasme.transform;
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = new Quaternion(0, 0, 0, 0);
                    gasme.transform.localScale = Vector3.one;
                    gasme.transform.localPosition = Vector3.zero;
                    gasme.transform.localRotation = new Quaternion(0, 0, 0, 0);
                }


            }
           animator.SetTrigger("EnterCar");
        }
        

    }

    [PunRPC]
    void ExitWheelCar()
    {




        if (view.IsMine)
        {

            foreach (var item in LOD)
            {
                item.enabled = true;
            }
            ConstantsHolder.DisableFppRotation = false;
            MutiplayerController.instance.disableSector = false;
            CarNavigationManager.CarNavigationInstance.DisableExitCanvas();
            loader.mainController.transform.parent = Parent;
            loader.mainController.transform.position = GiantWheelManager.Instance.Exit.position;
            SummitCarUIHandler.SummitCarUIHandlerInstance.UpdateUIelement(true);
            parentCharacterController.enabled = true;
            parentPlayerController.enabled = true;
            charcontroller.enabled = true;
            arrowManager.enabled = true;
            Transformview.enabled = true;
            animator.SetTrigger("ExitCar");

            GiantWheelManager.Instance.WheelCar.SetActive(true);
            GamePlayButtonEvents.inst.OnSwitchCameraClick();
            StartCoroutine(ExitSectorAfterDelay(1));

        }
        else
        {
            transform.parent = loader.mainPlayer.transform.transform.parent;
            transform.position = GiantWheelManager.Instance.Exit.position;
            transform.localScale = Vector3.one * 1.14f;
            charcontroller.enabled = true;
            arrowManager.enabled = true;
            Transformview.enabled = true;
            animator.SetTrigger("ExitCar");


        }
        
    }
    IEnumerator ExitSectorAfterDelay(int time)
    {
        yield return new WaitForSeconds(time);
        MutiplayerController.instance.Ontriggered("Default");
        GiantWheelManager.Instance.CarAdded = false;
    }

    public void waitForWheel()
    {
        GiantWheelManager.Instance.onCarStop += CarStop;
        GiantWheelManager.Instance.EnterwheelCar += CallEnterWheelRPC;
        GiantWheelManager.Instance.StopWheel(this);
    }

    private void CallEnterWheelRPC()
    {
       if(!isInsideWheel&&view.IsMine)
        {
            Debug.Log("Calling  RPC");
            view.RPC("EnterWheelCar", RpcTarget.All);
        }
    }

    private void CarStop()
    {
       if(PhotonNetwork.IsMasterClient)
        {
            view.RPC("WheelStoped", RpcTarget.All);
            GameplayEntityLoader.instance.IsJoinSummitWorld = false;
        }
    }

    public void Exit()
    {
        showExit = true;
        CarNavigationManager.CarNavigationInstance.DisableExitCanvas();
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

    public void CheckForExitWheel()
    {
        if (view.IsMine)
        {
            if (showExit)
            {
                view.RPC("ExitWheelCar", RpcTarget.All);
             
            }
        }

    }

    public void checkforDisableExit()
    {
       if(view.IsMine)
            CarNavigationManager.CarNavigationInstance.DisableExitCanvas();
        
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
     
    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
      
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
       
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
      
    }
}
