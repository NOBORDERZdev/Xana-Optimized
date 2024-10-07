using System.Threading.Tasks;
using Photon.Pun;
using UnityEngine;

public class AINPCTrigger : MonoBehaviour
{
    public string[] welcomeMsgs;
    public int npcID;
    public Vector3 NPCPosition;

    private bool IsAlreadyTriggered=true;
    private void OnEnable()
    {
        NPCRigidBodySetup();
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
        gameObject.transform.position = NPCPosition;
        GetComponent<Rigidbody>().useGravity = true;
        await Task.Delay(1000);
        GetComponent<Rigidbody>().useGravity = false;
    }
}
