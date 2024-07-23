using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerSceneSwitch : MonoBehaviour
{
    public int domeId;
    [Tooltip("This only require when dome id is set to -1")]
    public string sceneName;

    public TMPro.TextMeshPro textMeshPro;
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (PhotonNetwork.InRoom)
    //    {
    //        if (other.tag == "PhotonLocalPlayer" && other.GetComponent<PhotonView>().IsMine)
    //        {
    //            if (ConstantsHolder.MultiSectionPhoton)
    //            {
    //                ConstantsHolder.DiasableMultiPartPhoton = true;
    //            }
    //            if (domeId == -1 && !string.IsNullOrEmpty(sceneName))
    //                TriggerSceneLoading(sceneName);
    //            else
    //                TriggerSceneLoading();
    //        }
    //    }
    //}

    //void TriggerSceneLoading()
    //{
    //    GameplayEntityLoader.instance.AssignRaffleTickets(domeId);
    //    ConstantsHolder.domeId = domeId;
    //    BuilderEventManager.LoadNewScene?.Invoke(domeId, transform.GetChild(0).transform.position);
    //}

    //void TriggerSceneLoading(string sceneName)
    //{
    //    BuilderEventManager.LoadSceneByName?.Invoke(sceneName, transform.GetChild(0).transform.position);
    //}
}
