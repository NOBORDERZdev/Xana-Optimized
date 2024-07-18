using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummitEntityManager : MonoBehaviour, IMatchmakingCallbacks
{
     public static SummitEntityManager instance;

  
    [SerializeField]
    private Transform CarSpawnpoint;

    [HideInInspector]
    public SplineDone CarSpline;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    private void Start()
    {
      
    }

    public void InstantiateCAR()
    {
        if(!PhotonNetwork.IsMasterClient) { return; }
            CarSpline = SplineDone.Instance;
        var length = CarSpline.GetSplineLength(.0005f);
       var distancebetweencar = length / 8;

        float firstSpawn = Random.Range(0, (int)length);
        
        for (int i = 0; i < 8; i++) {

        var obje =   PhotonNetwork.InstantiateRoomObject("Historic_Mickey_Car", CarSpawnpoint.position, Quaternion.identity);
            var splineFollow = obje.GetComponent<SplineFollower>();
            
            splineFollow.moveAmount = firstSpawn;
            firstSpawn = (firstSpawn + distancebetweencar) % length;
            splineFollow.Setup((byte)(80 +i));
            splineFollow.spline = CarSpline;

        }
        
    }

    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
       
    }

    public void OnCreatedRoom()
    {
       
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
     
    }

    public void OnJoinedRoom()
    {
       if(PhotonNetwork.IsMasterClient)
        {

            InstantiateCAR();

        }
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
      
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
      
    }

    public void OnLeftRoom()
    {
       
    }
}
