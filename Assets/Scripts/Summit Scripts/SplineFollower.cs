using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SplineFollower : MonoBehaviour,IPunObservable, IInRoomCallbacks
{



    public enum MovementType {
        Normalized,
        Units
    }

    [SerializeField] public SplineDone spline;
    [SerializeField] public float speed = 1f;
    [SerializeField] private MovementType movementType;



    [HideInInspector]
    public Vector3 DriverPos = new Vector3(-0.308f    ,0.25f,- .507f);
    [HideInInspector]
    public Vector3 PacengerPosr = new Vector3(0.292f, 0.25f, -0.478f);
    public Dictionary<Player,int> PlayerListinCar = new Dictionary<Player,int>();

    public GameObject DriverPosition,PacengerPosition ,DriverExitPosition,PassengerExitPosition,Love;
    public bool driverseatempty = true;
    public bool pasengerseatemty = true;
    public bool isDriverMale = true; 
    public bool isPassengerMale = true;

    
    public float moveAmount;
    private float maxMoveAmount;
    public PhotonView view;
    [HideInInspector]
    public byte PrivateRoomName;
    public bool stopcar = false;
    public Rigidbody rigidbody;
    private bool checkforrigidbody = true;
    bool NeedToAddReference = true;

    Vector3 newRot;
    float counter = 1;
    float time = .5f;
    Vector3 currentRot;
    [SerializeField]
    Transform onewheel, twowheel, threewheel, fourwheel;
    private void Awake()
    {

      //  MutiplayerController.instance.ADDReference += addReferences;
    }

    private void Start()
    {
        newRot = onewheel.localEulerAngles + new Vector3(90, 0, 0);
        currentRot = onewheel.localEulerAngles;
        if (PhotonNetwork.IsMasterClient)
        {
            rigidbody.freezeRotation = false;
        }
        else
        {
            rigidbody.freezeRotation = true;
        }
    }
    


    public void Setup(byte Name) {
        spline = SplineDone.Instance;
        switch (movementType) {
            default:
            case MovementType.Normalized:
                maxMoveAmount = 1f;
                break;
            case MovementType.Units:
                maxMoveAmount = spline.GetSplineLength(0.001f);
                Debug.Log("Spline Length " + spline.GetSplineLength(0.001f));
                break;
        }
        // syncdata(moveAmount);
        view.RPC("syncdata", RpcTarget.AllBufferedViaServer, moveAmount,Name);
    }


    [PunRPC]
    public void syncdata(float moveAmount,byte Room)
    {
        Debug.Log("Buffered RPC");
        this.moveAmount = moveAmount;

        PrivateRoomName = Room;
        
      transform.position = new Vector3(transform.position.x,.5f,transform.position.z);
    }

    private void Update()
    {

        if (counter < 1)
        {
          
            if (stopcar)
            {
                return;
            }
            counter += Time.deltaTime;
            counter = counter / time;


            onewheel.localEulerAngles = Vector3.Slerp(currentRot, newRot, counter);
            twowheel.localEulerAngles = Vector3.Slerp(currentRot, newRot, counter);
            threewheel.localEulerAngles = Vector3.Slerp(currentRot, newRot, counter);
            fourwheel.localEulerAngles = Vector3.Slerp(currentRot, newRot, counter);
        }
        else
        {
            counter = 0;
            newRot = onewheel.localEulerAngles + new Vector3(90, 0, 0);
            currentRot = onewheel.localEulerAngles;
        }

    }


    private void FixedUpdate() {

        if (NeedToAddReference && CarNavigationManager.CarNavigationInstance)
        {
            CarNavigationManager.CarNavigationInstance.Cars.Add(view.ViewID, view);
            spline = SplineDone.Instance;
            maxMoveAmount = spline.GetSplineLength(0.001f);
            NeedToAddReference = false;
        }
        if (spline == null || !PhotonNetwork.IsMasterClient) { return; }
      
      
     /*   if (checkforrigidbody)
        {
            if (GetComponent<Rigidbody>() == null) { var rigid = gameObject.AddComponent<Rigidbody>(); rigid.useGravity = true; rigid.mass = 50; } checkforrigidbody = false;
        }*/
        moveAmount = (moveAmount + (Time.deltaTime * speed)) % maxMoveAmount;

        switch (movementType) {
            default:
            case MovementType.Normalized:
                var pos = spline.GetPositionAt(moveAmount);
                transform.position = new Vector3(pos.x,transform.position.y,pos.z);
                var forw = spline.GetForwardAt(moveAmount);
                transform.forward = new Vector3(forw.x, transform.position.y, forw.z);
                break;
            case MovementType.Units:
                var pose = spline.GetPositionAtUnits(moveAmount,.001f);
                transform.position = new Vector3(pose.x, transform.position.y, pose.z);
                var forwa = spline.GetForwardAtUnits(moveAmount,.001f);
                transform.forward = forwa; //new Vector3(forwa.x, transform.position.y, forwa.z);
                break;
        }



    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(moveAmount);
        }
        else
        {
            moveAmount= (float)stream.ReceiveNext();
        }
    }
    public void showLove()
    {
        Love.SetActive(true);
    }

    public void hidelove()
    {
        Love.SetActive(false);
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        int pos = -1;
        PlayerListinCar.TryGetValue(otherPlayer, out pos);   
        if (pos != -1) { 
            PlayerListinCar.Remove(otherPlayer);
            if(pos == 0) { driverseatempty = true; } else { driverseatempty=false; }
        }
    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            rigidbody.freezeRotation = false;
        }
    }
}
