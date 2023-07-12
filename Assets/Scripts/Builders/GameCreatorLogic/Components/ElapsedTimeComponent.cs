using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Threading.Tasks;
using Models;
using Photon.Pun;

//[RequireComponent(typeof(Rigidbody))]
public class ElapsedTimeComponent : ItemComponent
{
    [Tooltip("Set Total Time")]
    [SerializeField]
    private float m_TotalTime;
    private bool isActivated = false;
    [SerializeField]
    private ElapsedTimeComponentData elapsedTimeComponentData;


    public void Init(ElapsedTimeComponentData elapsedTimeComponentData)
    {
        //Debug.Log("Elapsed Time INit");
        //Debug.Log(JsonUtility.ToJson(elapsedTimeComponentData));
        this.elapsedTimeComponentData = elapsedTimeComponentData;

        isActivated = true;
    }


    private void OnCollisionEnter(Collision _other)
    {

        //}
        //private void OnTriggerEnter(Collider _other)
        //{
        //Debug.Log("Elapsed Time Trigger " + _other.gameObject.name + "Activated: " + isActivated + "  " + elapsedTimeComponentData.IsStart + " IsEnd " + elapsedTimeComponentData.IsEnd);

        if (_other.gameObject.CompareTag("Player") || (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine))
        {
            if (isActivated && elapsedTimeComponentData.IsStart)
            {

                // Debug.Log("Elapse Time start");
                //TimeStats.canRun = false;
                TimeStats._timeStop?.Invoke(0,()=> { TimeStats._timeStart?.Invoke(); });
                
                
            }
            if (isActivated && elapsedTimeComponentData.IsEnd)
            {
                //Debug.Log("Elapse Time end");
                TimeStats._timeStop?.Invoke(5,()=>{ });
            }
        }
    }
}