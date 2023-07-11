using Metaverse;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RpcManager : MonoBehaviourPunCallbacks
{
    public bool Publictest;
    private float timer;

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
            //Debug.LogError("value rpc===" + i);
            if (this.GetComponent<PhotonView>().IsMine)
            {
                //Debug.LogError("this.GetComponent<PhotonView>().IsMine"+ this.GetComponent<PhotonView>().IsMine);

                PhotonNetwork.Destroy(this.gameObject);
                PhotonNetwork.SendAllOutgoingCommands();


            }
            else
            {
                //Debug.LogError("this" + this.GetComponent<PhotonView>().IsMine);
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
        Debug.LogError("App quit call");
        AvatarManager.sendDataValue = false;
        if (this.GetComponent<PhotonView>().IsMine)
        {
                 this.GetComponent<PhotonView>().RPC("CheckRpc",RpcTarget.All,this.GetComponent<PhotonView>().ViewID);
       
        PhotonNetwork.SendAllOutgoingCommands();
        }
        // CheckRpcPlayer();
   
    }
   

}
