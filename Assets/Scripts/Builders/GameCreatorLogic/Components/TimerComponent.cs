using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Models;
using Photon.Pun;

//[RequireComponent(typeof(Rigidbody))]
public class TimerComponent : ItemComponent
{

    private bool isActivated = false;
    private TimerComponentData timerComponentData;

    public void Init(TimerComponentData timerComponentData)
    {
        this.timerComponentData = timerComponentData;

        isActivated = true;
    }


    private void OnCollisionEnter(Collision _other)
    {
        //}
        //private void OnTriggerEnter(Collider _other)
        //{

        if (isActivated && timerComponentData.IsStart)
        {
            if (_other.gameObject.CompareTag("Player") || (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine))
            {
                //StartTriggerEvent?.Invoke();

                //TimeStats.canRun = false;
                BuilderEventManager.OnTimerTriggerEnter?.Invoke("", timerComponentData.Timer + 1);
            }
        }
        if (isActivated && timerComponentData.IsEnd)
        {
            if (_other.gameObject.CompareTag("Player") || (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine))
            {
                //EndTriggerEvent?.Invoke();

                //TimeStats.canRun = false;
                BuilderEventManager.OnTimerTriggerEnter?.Invoke("", 0);
            }
        }
    }

}// End of class