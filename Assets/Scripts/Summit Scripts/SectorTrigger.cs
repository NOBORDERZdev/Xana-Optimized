using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SectorTrigger : MonoBehaviour
{
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.InRoom)
        {
            if (other.gameObject.tag == "PhotonLocalPlayer")
            {
                if (other.GetComponent<PhotonView>().IsMine)
                {
                    SectorManager.Instance.Triggered();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (PhotonNetwork.InRoom)
        {
            if (other.gameObject.tag == "PhotonLocalPlayer")
            {
                if (other.GetComponent<PhotonView>().IsMine)
                {
                    SectorManager.Instance.TriggeredExit(gameObject.name);
                }
            }
        }
    }
}
