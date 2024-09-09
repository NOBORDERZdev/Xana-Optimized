using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;
using System.Threading.Tasks;

public class EmailEntryController : MonoBehaviour
{
    public string WorldIdTestnet;
    public string WorldIdMainnet;
    public string WorldId;
    private bool alreadyTriggered;

    private void OnEnable()
    {
        if (APIBasepointManager.instance.IsXanaLive)
            WorldId = WorldIdMainnet;
        else
            WorldId = WorldIdTestnet;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("player enter : " + other.gameObject.name);
        //if (other.gameObject.GetComponent<PhotonView>() && other.gameObject.GetComponent<PhotonView>().IsMine)
        //{
        //    GamePlayUIHandler.inst.ref_PlayerControllerNew.m_IsMovementActive = false;
        //}
        if (PhotonNetwork.InRoom)
        {
            if (other.tag == "PhotonLocalPlayer" && other.GetComponent<PhotonView>().IsMine && !alreadyTriggered)
            {
                alreadyTriggered = true;
                if (ConstantsHolder.MultiSectionPhoton)
                {
                    ConstantsHolder.DiasableMultiPartPhoton = true;
                }
                TriggerSceneLoading(WorldId);
                DisableCollider();
            }
        }
    }

    void TriggerSceneLoading(string WorldId)
    {
        BuilderEventManager.LoadSceneByName?.Invoke(WorldId, transform.GetChild(0).transform.position);
    }

    async void DisableCollider()
    {
        await Task.Delay(2000);
        alreadyTriggered = false;
    }

}
