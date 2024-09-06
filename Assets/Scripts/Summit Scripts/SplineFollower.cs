using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SplineFollower : MonoBehaviour, IPunObservable, IInRoomCallbacks
{



    public enum MovementType
    {
        Normalized,
        Units
    }

    [SerializeField] 
    public SplineDone Spline;
    [SerializeField] 
    public float Speed = 1f;
    
    [HideInInspector]
    public byte PrivateRoomName;
    [HideInInspector]
    public Vector3 DriverPos = new Vector3(-0.308f, 0.25f, -.507f);
    [HideInInspector]
    public Vector3 PacengerPosr = new Vector3(0.292f, 0.25f, -0.478f);
    public Dictionary<Player, int> PlayerListinCar = new Dictionary<Player, int>();
    public GameObject DriverPosition, PacengerPosition, DriverExitPosition, PassengerExitPosition, Love;
    public bool DriverSeatEmpty = true;
    public bool PasengerSeatEmty = true;
    public bool _isDriverMale = true;
    public bool _isPassengerMale = true;
    public float StepSize =0.001f;
    
    public float MoveAmount;
    private float MaxMoveAmount;
    public PhotonView View;
   
    public bool StopCar = false;
    public Rigidbody Rigidbody;
    private bool CheckForRigidbody = true;
    bool NeedToAddReference = true;

    public float _carT;
    public float _carTF;
    public float _splineUnitDistance = 0f;
    public Vector3 _lastPosition;
    
    [SerializeField]
    Transform _oneWheel, _twoWheel, _threeWheel, _fourWheel;
    [SerializeField]
    private MovementType _movementType;
    Vector3 _newRot;
    float _counter = 1;
    float _time = .5f;
    Vector3 _currentRot;
  
    private void Awake()
    {

        //  MutiplayerController.instance.ADDReference += addReferences;
    }

    private void Start()
    {
        _newRot = _oneWheel.localEulerAngles + new Vector3(90, 0, 0);
        _currentRot = _oneWheel.localEulerAngles;
        if (PhotonNetwork.IsMasterClient)
        {
            Rigidbody.freezeRotation = false;
        }
        else
        {
            Rigidbody.freezeRotation = true;
        }
    }



    public void Setup(byte Name)
    {
        Spline = SplineDone.Instance;
        switch (_movementType)
        {
            default:
            case MovementType.Normalized:
                MaxMoveAmount = 1f;
                break;
            case MovementType.Units:
                MaxMoveAmount = Spline.GetSplineLength(StepSize);
                Debug.Log("Spline Length " + Spline.GetSplineLength(StepSize));
                break;
        }
        // syncdata(MoveAmount);
        View.RPC("syncdata", RpcTarget.AllBufferedViaServer, MoveAmount, Name);
    }


    [PunRPC]
    public void syncdata(float moveAmount, byte Room)
    {
        Debug.Log("Buffered RPC");
        this.MoveAmount = moveAmount;

        PrivateRoomName = Room;

        transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);
    }

    private void Update()
    {

        if (_counter < 1)
        {

            if (StopCar)
            {
                return;
            }
            _counter += Time.deltaTime;
            _counter = _counter / _time;


            _oneWheel.localEulerAngles = Vector3.Slerp(_currentRot, _newRot, _counter);
            _twoWheel.localEulerAngles = Vector3.Slerp(_currentRot, _newRot, _counter);
            _threeWheel.localEulerAngles = Vector3.Slerp(_currentRot, _newRot, _counter);
            _fourWheel.localEulerAngles = Vector3.Slerp(_currentRot, _newRot, _counter);
        }
        else
        {
            _counter = 0;
            _newRot = _oneWheel.localEulerAngles + new Vector3(90, 0, 0);
            _currentRot = _oneWheel.localEulerAngles;
        }

    }


    private void FixedUpdate()
    {

        if (NeedToAddReference && CarNavigationManager.CarNavigationInstance)
        {
            if (!CarNavigationManager.CarNavigationInstance.Cars.ContainsKey(View.ViewID))
                CarNavigationManager.CarNavigationInstance.Cars.Add(View.ViewID, View);
            Spline = SplineDone.Instance;
            MaxMoveAmount = Spline.GetSplineLength(StepSize);
            NeedToAddReference = false;
        }
        if (Spline == null || !PhotonNetwork.IsMasterClient) { return; }


        /*   if (CheckForRigidbody)
           {
               if (GetComponent<Rigidbody>() == null) { var rigid = gameObject.AddComponent<Rigidbody>(); rigid.useGravity = true; rigid.mass = 50; } CheckForRigidbody = false;
           }*/
        if ((MoveAmount + (Time.deltaTime * Speed)) >= MaxMoveAmount) { _carT = 0;_carTF = 0; }
        MoveAmount = (MoveAmount + (Time.deltaTime * Speed)) % MaxMoveAmount;

        switch (_movementType)
        {
            default:
            case MovementType.Normalized:
                var pos = Spline.GetPositionAt(MoveAmount);
                transform.position = new Vector3(pos.x, transform.position.y, pos.z);
                var forw = Spline.GetForwardAt(MoveAmount);
                transform.forward = new Vector3(forw.x, transform.position.y, forw.z);
                break;
            case MovementType.Units:
                var pose = Spline.GetPositionAtUnits(MoveAmount,this,StepSize);
                transform.position = new Vector3(pose.x, transform.position.y, pose.z);
                var forwa = Spline.GetForwardAtUnits(MoveAmount, this, StepSize);
                transform.forward = forwa; //new Vector3(forwa.x, transform.position.y, forwa.z);
                break;
        }



    }
    private void OnDestroy()
    {
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(MoveAmount);
        }
        else
        {
            MoveAmount = (float)stream.ReceiveNext();
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
        if (pos != -1)
        {
            PlayerListinCar.Remove(otherPlayer);
            if (pos == 0) { DriverSeatEmpty = true; } else { DriverSeatEmpty = false; }
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
        if (PhotonNetwork.IsMasterClient)
        {
            Rigidbody.freezeRotation = false;
        }
    }
}
