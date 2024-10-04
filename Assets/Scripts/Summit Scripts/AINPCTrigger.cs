using System.Threading.Tasks;
using Photon.Pun;
using UnityEngine;

public class AINPCTrigger : MonoBehaviour
{
    public string[] welcomeMsgs;
    public int npcID;

    private bool IsAlreadyTriggered=true;
    private void OnEnable()
    {
        BuilderEventManager.AfterWorldInstantiated += NPCRigidBodySetup;
        BuilderEventManager.AfterWorldOffcialWorldsInatantiated += NPCRigidBodySetup;
    }

    private void OnDisable()
    {
        BuilderEventManager.AfterWorldInstantiated -= NPCRigidBodySetup;
        BuilderEventManager.AfterWorldOffcialWorldsInatantiated -= NPCRigidBodySetup;
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

    async void NPCRigidBodySetup()
    {
        GetComponent<Rigidbody>().useGravity = true;
        await Task.Delay(1000);
        if (GetComponent<Rigidbody>())
            Destroy(GetComponent<Rigidbody>());
    }
}
