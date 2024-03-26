using Metaverse;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun.Demo.PunBasics;

public class RpcManager : MonoBehaviourPunCallbacks
{
    public bool Publictest;
    private float timer;
    public bool DifferentAnimClicked = false;
    // Start is called before the first frame update
    void Start()
    {
        checkPlayerInstance();
    }
    void checkPlayerInstance()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("PhotonPlayer");

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i].GetComponent<PhotonView>().IsMine)
            {
                PhotonNetwork.Destroy(objects[i]);
                Destroy(objects[i]);
            }


        }
    }

    public void CheckIfDifferentAnimClicked(bool isEnable)
    {
        GetComponent<PhotonView>().RPC(nameof(UpdateStatsAnimClicked), RpcTarget.AllBuffered, GetComponent<PhotonView>().ViewID, isEnable);
    }

    [PunRPC]
    public void UpdateStatsAnimClicked(int viewId, bool isEnable)
    {
        for (int i = 0; i < MutliplayerController.instance.playerobjects.Count; i++)
        {
            if (MutliplayerController.instance.playerobjects[i] != null)
            {
                if (MutliplayerController.instance.playerobjects[i].GetComponent<PhotonView>().ViewID == viewId)
                {
                    if (!isEnable)
                    {
                        MutliplayerController.instance.playerobjects[i].GetComponent<Animator>().SetBool("Stand", true);
                        MutliplayerController.instance.playerobjects[i].GetComponent<Animator>().SetBool("EtcAnimStart", false);
                    }
                    MutliplayerController.instance.playerobjects[i].GetComponent<RpcManager>().DifferentAnimClicked = isEnable;
                    break;
                }
            }
        }
    }

    public void BackToIdleAnimBeforeJump()                  //Added by Ali Hamza
    {
        GetComponent<PhotonView>().RPC(nameof(JumpAnimSitLying), RpcTarget.AllBuffered, GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    void JumpAnimSitLying(int viewId)                       //Added by Ali Hamza
    {
        for (int i = 0; i < MutliplayerController.instance.playerobjects.Count; i++)
        {
            if (MutliplayerController.instance.playerobjects[i] != null)
            {
                if (MutliplayerController.instance.playerobjects[i].GetComponent<PhotonView>().ViewID == viewId)
                {
                    if (MutliplayerController.instance.playerobjects[i].GetComponent<Animator>().GetBool("EtcAnimStart"))
                    {
                        if (!MutliplayerController.instance.playerobjects[i].GetComponent<Animator>().GetBool("Stand"))
                        {
                            MutliplayerController.instance.playerobjects[i].GetComponent<Animator>().SetBool("EtcAnimStart", false);
                            MutliplayerController.instance.playerobjects[i].GetComponent<Animator>().SetBool("Stand", true);
                            break;
                        }
                    }
                }
            }
        }
    }

    public void CheckRpcPlayer()
    {
        this.GetComponent<PhotonView>().RPC("CheckRpc", RpcTarget.All);
    }
    [PunRPC]
    void CheckRpc(int i)
    {
        if (i == this.GetComponent<PhotonView>().ViewID)
        {
            PhotonNetwork.RemoveBufferedRPCs(i);
            if (this.GetComponent<PhotonView>().IsMine)
            {
                PhotonNetwork.Destroy(this.gameObject);
                PhotonNetwork.SendAllOutgoingCommands();
            }
            else
            {
                this.GetComponent<PhotonView>().TransferOwnership(this.GetComponent<PhotonView>().ViewID);
                PhotonNetwork.DestroyPlayerObjects(this.GetComponent<PhotonView>().ViewID, false);
                PhotonNetwork.SendAllOutgoingCommands();
            }
        }

    }
    private void OnDestroy()
    {

    }

    private void OnApplicationQuit()
    {
        if (this.GetComponent<PhotonView>().IsMine)
        {
            this.GetComponent<PhotonView>().RPC("CheckRpc", RpcTarget.All, this.GetComponent<PhotonView>().ViewID);

            PhotonNetwork.SendAllOutgoingCommands();
        }

    }


}
