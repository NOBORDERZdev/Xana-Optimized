using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerSceneSwitch : MonoBehaviour
{
    public int domeId;
    [Tooltip("This only require when dome id is set to -1")]
    public string worldId;

    public TMPro.TextMeshPro textMeshPro;
    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.InRoom)
        {
            if (other.tag == "PhotonLocalPlayer" && other.GetComponent<PhotonView>().IsMine)
            {
                if (ConstantsHolder.MultiSectionPhoton)
                {
                    ConstantsHolder.DiasableMultiPartPhoton = true;
                }
                if (domeId == -1 && !string.IsNullOrEmpty(worldId))
                    TriggerSceneLoading(worldId);
                else
                    TriggerSceneLoading();
            }
        }
    }

    void TriggerSceneLoading()
    {
        GameplayEntityLoader.instance.AssignRaffleTickets(domeId);
        BuilderEventManager.LoadNewScene?.Invoke(domeId, transform.GetChild(0).transform.position);
    }

    void TriggerSceneLoading(string worldId)
    {
        BuilderEventManager.LoadSceneByName?.Invoke(worldId, transform.GetChild(0).transform.position);
    }
}
