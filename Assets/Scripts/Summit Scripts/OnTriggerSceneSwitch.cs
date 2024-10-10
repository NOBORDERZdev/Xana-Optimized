using Photon.Pun;
using System.Threading.Tasks;
using UnityEngine;


public class OnTriggerSceneSwitch : MonoBehaviour
{
    [Tooltip("Set it to -1 if you are manually loading scene")]
    public int DomeId;
    public string WorldIdTestnet;
    public string WorldIdMainnet;
    public GameObject textMeshPro;
    [HideInInspector]
    public string WorldId;
    private bool alreadyTriggered;

    private void OnEnable()
    {
        if (APIBasepointManager.instance.IsXanaLive)
            WorldId = WorldIdMainnet;
        else
            WorldId = WorldIdTestnet;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.InRoom)
        {
            if (other.tag == "PhotonLocalPlayer" && other.GetComponent<PhotonView>().IsMine && !alreadyTriggered)
            {
                alreadyTriggered = true;
                if (ConstantsHolder.MultiSectionPhoton)
                {
                    ConstantsHolder.DiasableMultiPartPhoton = true;
                }
                if (DomeId == -1)
                {
                    TriggerSceneLoading(WorldId);
                }
                else
                    TriggerSceneLoading();

                DisableCollider();
            }
        }
    }

    void TriggerSceneLoading()
    {
        //GameplayEntityLoader.instance.AssignRaffleTickets(DomeId);
        BuilderEventManager.LoadNewScene?.Invoke(DomeId, transform.GetChild(0).transform.position);
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
