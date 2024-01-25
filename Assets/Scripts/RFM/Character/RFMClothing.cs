using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RFMClothing : MonoBehaviour
{
    public bool isHunter;
    public bool isEscapeeNPC;
    public string clothJson;
    public string hunterClothJson;
    public string[] escapeeNPCClothesJSON;
    public string escappeeClothJSON;
    public int escappeeClothIndex;
    public PhotonView photonView;
    public AvatarController avatarController;
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        SetCloths();
    }
    void Start()
    {

    }

    public void SetCloths()
    {
        if (isEscapeeNPC)
        {
            if (photonView.IsMine)
            {
                escappeeClothIndex = Random.Range(0, escapeeNPCClothesJSON.Length);
                clothJson = escappeeClothJSON = escapeeNPCClothesJSON[escappeeClothIndex];
                photonView.RPC(nameof(RunnerClothRPC), RpcTarget.AllBuffered, escappeeClothJSON);
                avatarController.moduleClothJson = clothJson;
            }
        }
        else if (isHunter)
        {
            clothJson = hunterClothJson;
            avatarController.moduleClothJson = clothJson;
        }
        avatarController.OnEnable();
    }

    [PunRPC]
    public void RunnerClothRPC(string cloth)
    {
        clothJson = escappeeClothJSON = cloth;
        avatarController.moduleClothJson = clothJson;
        avatarController.OnEnable();
    }
}
