using Photon.Pun;
using UnityEngine;

public class AINPCTrigger : MonoBehaviour
{
    public string[] welcomeMsgs;
    public int npcID;

    private bool IsAlreadyTriggered=true;
    private void Start()
    {
        Invoke("NPCRigidBodySetup", 1);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PhotonLocalPlayer" && other.GetComponent<PhotonView>() && other.GetComponent<PhotonView>().IsMine && IsAlreadyTriggered)
        {
            IsAlreadyTriggered = false;
            BuilderEventManager.AINPCActivated?.Invoke(npcID, welcomeMsgs);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "PhotonLocalPlayer" && other.GetComponent<PhotonView>() && other.GetComponent<PhotonView>().IsMine)
        {
            IsAlreadyTriggered = true;
            BuilderEventManager.AINPCDeactivated?.Invoke(npcID);
        }
    }

    void NPCRigidBodySetup()
    {
        if (GetComponent<Rigidbody>())
            Destroy(GetComponent<Rigidbody>());
    }
}
