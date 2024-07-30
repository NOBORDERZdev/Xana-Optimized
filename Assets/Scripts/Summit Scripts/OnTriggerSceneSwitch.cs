using Photon.Pun;
using System.Threading.Tasks;
using UnityEngine;


public class OnTriggerSceneSwitch : MonoBehaviour
{
    public int DomeId;
    [Tooltip("This only require when dome id is set to -1")]
    public string WorldId;
    public TMPro.TextMeshPro DomeIndexText;


    private bool alreadyTriggered;
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
                if (DomeId == -1 && !string.IsNullOrEmpty(WorldId))
                    TriggerSceneLoading(WorldId);
                else
                    TriggerSceneLoading();

                DisableCollider();
            }
        }
    }

    void TriggerSceneLoading()
    {
        GameplayEntityLoader.instance.AssignRaffleTickets(DomeId);
        BuilderEventManager.LoadNewScene?.Invoke(DomeId, transform.GetChild(0).transform.position);
    }

    void TriggerSceneLoading(string worldId)
    {
        BuilderEventManager.LoadSceneByName?.Invoke(worldId, transform.GetChild(0).transform.position);
    }

    async void DisableCollider()
    {
        await Task.Delay(2000);
        alreadyTriggered = false;
    }
}
