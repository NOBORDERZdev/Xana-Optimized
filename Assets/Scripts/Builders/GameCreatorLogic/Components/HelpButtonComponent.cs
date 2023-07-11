using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Models;
using Photon.Pun;

//[RequireComponent(typeof(Rigidbody))]
public class HelpButtonComponent : ItemComponent
{
    [SerializeField]
    private HelpButtonComponentData helpButtonComponentData;

    string collectingAllTheStrings;
    public void Init(HelpButtonComponentData helpButtonComponentData)
    {
        Debug.Log(JsonUtility.ToJson(helpButtonComponentData));

        this.helpButtonComponentData = helpButtonComponentData;
        //GetComponent<Rigidbody>().isKinematic = true;
        //GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void SetHelpButtonNarration()
    {
        Debug.Log("SetHelpButtonNarration");

        //TimeStats.canRun = false;
        BuilderEventManager.OnHelpButtonCollisionEnter?.Invoke(helpButtonComponentData.titleHelpButtonText, helpButtonComponentData.helpButtonData);

    }


    private void OnCollisionEnter(Collision _other)
    {
        //}
        //private void OnTriggerEnter(Collider _other)
        //{
        Debug.Log("Help Button Collision Enter: " + _other.gameObject.name);
        if (_other.gameObject.CompareTag("Player") || (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine))
        {
            //if (CanvasComponenetsManager._instance != null)
            {
                BuilderEventManager.OnHelpButtonCollisionEnter?.Invoke(helpButtonComponentData.titleHelpButtonText, helpButtonComponentData.helpButtonData);
            }
            //StartTriggerEvent?.Invoke();
        }
    }

    private void OnCollisionExit(Collision _other)
    {
        Debug.Log("Help Button Collision Exit: " + _other.gameObject.name);
        if (_other.gameObject.CompareTag("Player") || (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine))
        {
            BuilderEventManager.OnHelpButtonCollisionExit?.Invoke();
        }
    }
}