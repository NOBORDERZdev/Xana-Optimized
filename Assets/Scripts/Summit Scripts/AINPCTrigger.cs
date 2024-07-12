using Photon.Pun;
using UnityEngine;

public class AINPCTrigger : MonoBehaviour
{
    public string[] welcomeMsgs;
    public int npcID;
    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PhotonLocalPlayer" && other.GetComponent<PhotonView>() && other.GetComponent<PhotonView>().IsMine)
        {
            BuilderEventManager.AINPCActivated?.Invoke(npcID,welcomeMsgs);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "PhotonLocalPlayer" && other.GetComponent<PhotonView>() && other.GetComponent<PhotonView>().IsMine)
        {
            BuilderEventManager.AINPCDeactivated?.Invoke(npcID);
        }
    }


}
