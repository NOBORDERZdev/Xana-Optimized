using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerPhotonInfo : MonoBehaviour
{
    public bool IsMine;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<PhotonView>().IsMine)
            IsMine = true;
    }

}
