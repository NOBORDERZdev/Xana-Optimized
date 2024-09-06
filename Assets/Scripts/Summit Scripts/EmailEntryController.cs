using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailEntryController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("player enter : " + other.gameObject.name);
        if (other.gameObject.GetComponent<PhotonView>() && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            GamePlayUIHandler.inst.ref_PlayerControllerNew.m_IsMovementActive = false;
        }
    }

}
