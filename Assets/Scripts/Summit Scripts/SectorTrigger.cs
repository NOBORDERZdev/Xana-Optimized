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
        
            if (other.gameObject.tag == "Player")
        {
            SectorManager.Instance.TriggeredExit(gameObject.name);
        }
        
    }
   
}
