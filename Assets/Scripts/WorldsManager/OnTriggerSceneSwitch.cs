using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerSceneSwitch : MonoBehaviour
{

    public string sceneName;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="PhotonLocalPlayer")
        {
            TriggerSceneLoading();
        }
    }

    void TriggerSceneLoading()
    {
        BuilderEventManager.LoadNewScene?.Invoke(sceneName);
    }
}
