using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummitEntityManager : MonoBehaviour
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
    private void Start()
    {
      
    }

    public void InstantiateCAR()
    {
        if(!PhotonNetwork.IsMasterClient) { return; }
            CarSpline = SplineDone.Instance;
        var length = CarSpline.GetSplineLength(0.001f);
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

}
