using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINPCTrigger : MonoBehaviour
{
    public string[] welcomeMsgs;
    public string npcID;
    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PhotonLocalPlayer" && other.GetComponent<PhotonView>().IsMine)
        {
            BuilderEventManager.AINPCActivated?.Invoke(npcID,welcomeMsgs);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "PhotonLocalPlayer" && other.GetComponent<PhotonView>().IsMine)
        {
            BuilderEventManager.AINPCActivated?.Invoke(npcID, welcomeMsgs);
        }
    }


}
