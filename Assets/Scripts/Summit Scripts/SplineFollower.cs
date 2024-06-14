using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineFollower : MonoBehaviour {



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

    private void Awake()
    {

        MutiplayerController.instance.ADDReference += addReferences;
    }

    private void Start()
    {
    }

    private void addReferences()
    {

        CarNavigationManager.instance.Cars.Add(view.ViewID, view);
        spline = SplineDone.Instance;
        maxMoveAmount = spline.GetSplineLength(0.001f);
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
        
      transform.position = new Vector3(transform.position.x,0f,transform.position.z);
    }

   


    private void Update() {
        if(spline == null|| stopcar || !PhotonNetwork.IsMasterClient) { return; }
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

    public void showLove()
    {
        Love.SetActive(true);
    }

    public void hidelove()
    {
        Love.SetActive(false);
    }
}
