using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Photon.Pun;

//[RequireComponent(typeof(Rigidbody))]
public class TimerCountdownComponent : ItemComponent
{

    public TimerCountdownComponentData timerCountdownComponentData;

    public int timerLimit, i, defaultValue;

    private bool activateComponent = false;
    //public GameObject timerPanel;
    public Sprite[] countdownSprites;



    //private void Start()
    //{
    //    Debug.Log("Timer Countdown Component");
    //    GetComponent<Rigidbody>().isKinematic = true;
    //    GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
    //    Collider[] _childCollider = transform.GetComponentsInChildren<Collider>();
    //    for (int i = 0; i < _childCollider.Length; i++)
    //    {
    //        _childCollider[i].isTrigger = true;
    //    }
    //}
    public void Init(TimerCountdownComponentData timerCountdownComponentData)
    {
        Debug.Log(JsonUtility.ToJson(timerCountdownComponentData));
        this.timerCountdownComponentData = timerCountdownComponentData;

        activateComponent = true;
        defaultValue = (int)timerCountdownComponentData.setTimer - 1;

    }
    private void OnCollisionEnter(Collision other)
    {
        //}
        //private void OnTriggerEnter(Collider other)
        //{
        Debug.Log("Timer Countdown Trigger: " + other.gameObject.name);
        if (other.gameObject.tag == "Player" || (other.gameObject.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>().IsMine))
        {
            i = defaultValue;

            //TimeStatsCountdown._timeStop?.Invoke();
            //TimeStatsCountdown._timeStart?.Invoke(i);

            //TimeStats.canRun = false;
            BuilderEventManager.OnTimerCountDownTriggerEnter?.Invoke(i, true);
        }
    }
}