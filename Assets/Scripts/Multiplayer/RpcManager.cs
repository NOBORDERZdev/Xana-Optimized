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

        for(int i = 0; i < objects.Length; i++)
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
        for (int i = 0; i < Launcher.instance.playerobjects.Count; i++)
        {
            if (Launcher.instance.playerobjects[i] != null)
            {
                if (Launcher.instance.playerobjects[i].GetComponent<PhotonView>().ViewID == viewId)
                {
                    Launcher.instance.playerobjects[i].GetComponent<RpcManager>().DifferentAnimClicked = isEnable;
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
        for (int i = 0; i < Launcher.instance.playerobjects.Count; i++)
        {
            if (Launcher.instance.playerobjects[i] != null)
            {
                if (Launcher.instance.playerobjects[i].GetComponent<PhotonView>().ViewID == viewId)
                {
                    if (Launcher.instance.playerobjects[i].GetComponent<Animator>().GetBool("EtcAnimStart"))
                    {
                        if (!Launcher.instance.playerobjects[i].GetComponent<Animator>().GetBool("Stand")) 
                        {
                            Launcher.instance.playerobjects[i].GetComponent<Animator>().SetBool("EtcAnimStart", false);
                            Launcher.instance.playerobjects[i].GetComponent<Animator>().SetBool("Stand", true);
                            break;
                        }
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Publictest)
        //{
        //    CheckRpcPlayer();
        //}
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
            //Debug.Log("value rpc===" + i);
            if (this.GetComponent<PhotonView>().IsMine)
            {
                //Debug.Log("this.GetComponent<PhotonView>().IsMine"+ this.GetComponent<PhotonView>().IsMine);

                PhotonNetwork.Destroy(this.gameObject);
                PhotonNetwork.SendAllOutgoingCommands();


            }
            else
            {
                //Debug.Log("this" + this.GetComponent<PhotonView>().IsMine);
                this.GetComponent<PhotonView>().TransferOwnership(this.GetComponent<PhotonView>().ViewID);
                PhotonNetwork.DestroyPlayerObjects(this.GetComponent<PhotonView>().ViewID,false);
                PhotonNetwork.SendAllOutgoingCommands();
                //DestroyImmediate(this.gameObject);
            }
        }

    }
    private void OnDestroy()
    {
       
    }

    private void OnApplicationQuit()
    {
        Debug.Log("App quit call");
        AvatarManager.sendDataValue = false;
        if (this.GetComponent<PhotonView>().IsMine)
        {
                 this.GetComponent<PhotonView>().RPC("CheckRpc",RpcTarget.All,this.GetComponent<PhotonView>().ViewID);
       
        PhotonNetwork.SendAllOutgoingCommands();
        }
        // CheckRpcPlayer();
   
    }
   

}
