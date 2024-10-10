using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    public async void InstantiateCAR()
    {
        if(!PhotonNetwork.IsMasterClient || CarNavigationManager.CarNavigationInstance.Cars.Count>0) { return; }
            CarSpline = SplineDone.Instance;
        var length = CarSpline.GetSplineLength(.0005f);
       var distancebetweencar = length / 12;

        float firstSpawn = Random.Range(0, (int)length);
        
        for (int i = 0; i < 12; i++) {

        var obje =   PhotonNetwork.InstantiateRoomObject("Historic_Mickey_Car", CarSpawnpoint.position + new Vector3(0f, 2f, 0f), Quaternion.identity);
            var splineFollow = obje.GetComponent<SplineFollower>();
            
            splineFollow.MoveAmount = firstSpawn;
            firstSpawn = (firstSpawn + distancebetweencar +Random.Range(0,20)) % length;
            splineFollow.Setup((byte)(80 +i));
            splineFollow.Spline = CarSpline;
            await Task.Delay(100);
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
       if(PhotonNetwork.IsMasterClient && ConstantsHolder.xanaConstants.EnviornmentName == "XANA Summit")
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
