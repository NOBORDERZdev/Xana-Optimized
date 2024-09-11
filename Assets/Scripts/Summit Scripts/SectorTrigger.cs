using Photon.Pun;
using System.Collections;
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
    IEnumerator routine;
    private void OnTriggerExit(Collider other)
    {
        if (PhotonNetwork.InRoom)
        {
            if (other.gameObject.tag == "PhotonLocalPlayer")
            {   if(routine != null)
                {
                    StopCoroutine(routine);
                }
                routine = waitforPhotonview(other);
                StartCoroutine(routine);
            }
        }
    }
    IEnumerator waitforPhotonview(Collider other)
    {
        yield return new WaitUntil(() => other.GetComponent<PhotonView>());
        if (other.GetComponent<PhotonView>().IsMine)
        {
            SectorManager.Instance.TriggeredExit(gameObject.name);
        }
        routine = null;
    }
}
