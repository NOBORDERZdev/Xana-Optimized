using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedirectionComponent : MonoBehaviour
{
    public string Url;
    [Tooltip("This is not required added for special cases")]
    public string Msg;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PhotonLocalPlayer" && other.GetComponent<PhotonView>().IsMine)
        {
            BuilderEventManager.OpenRedirectionPopup?.Invoke(Url,Msg);
        }
    }

}
