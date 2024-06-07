using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerSceneSwitch : MonoBehaviour
{
    public int domeId;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="PhotonLocalPlayer" && other.GetComponent<PhotonView>().IsMine)
        {
            TriggerSceneLoading();
        }
    }

    void TriggerSceneLoading()
    {
        ConstantsHolder.domeId = domeId;
        BuilderEventManager.LoadNewScene?.Invoke(domeId,transform.GetChild(0).transform.position);
    }
}
