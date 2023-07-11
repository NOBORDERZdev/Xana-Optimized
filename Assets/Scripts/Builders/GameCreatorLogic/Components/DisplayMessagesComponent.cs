using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using Photon.Pun;

//[RequireComponent(typeof(Rigidbody))]
public class DisplayMessagesComponent : MonoBehaviour
{
    [SerializeField]
    private DisplayMessageComponentData displayMessageComponentData;
    public static IEnumerator currentCoroutine;
    public bool isCoroutineRunning = false;

    public void Init(DisplayMessageComponentData displayMessageComponentData)
    {
        this.displayMessageComponentData = displayMessageComponentData;
    }

    //oncollisionEnter to OnTriggerEnter
    private void OnCollisionEnter(Collision _other)
    {

        //}
        //private void OnTriggerEnter(Collider _other)
        //{
        Debug.Log("Display Message Collision Enter " + _other.gameObject.name);
        if (_other.gameObject.CompareTag("Player") || (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine))
        {
            isCoroutineRunning = true;

            //TimeStats.canRun = false;
            if (displayMessageComponentData.isStart)
            {
                BuilderEventManager.OnDisplayMessageCollisionEnter?.Invoke(displayMessageComponentData.startDisplayMessage, displayMessageComponentData.startTimerCount, true);
            }
            else
            {
                BuilderEventManager.OnDisplayMessageCollisionEnter?.Invoke(displayMessageComponentData.endDisplayMessage, 5, false);
            }
        }
    }
}