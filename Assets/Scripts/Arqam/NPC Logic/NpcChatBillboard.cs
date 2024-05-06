using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace NPC
{
    public class NpcChatBillboard : MonoBehaviour
    {
        public GameObject chatBoard;
        public TMP_Text chatText;


        public void ShowNpcMessage(string msg)
        {
            ////For multiplayer
            //gameObject.GetComponent<PhotonView>().RPC(nameof (HUDMessage), RpcTarget.All, msg, ReferencesForGamePlay.instance.m_34player.GetComponent<PhotonView>().ViewID);

            chatText.text = "";
            chatText.text = msg;
            chatBoard.SetActive(true);           
        }

        //[PunRPC]
        //public void HUDMessage(string chat, int viewId)
        //{
        //    chatText.text = "";
        //    if (gameObject.GetComponent<PhotonView>().ViewID == viewId)
        //    {
        //        chatText.text = chat;
        //        chatBoard.SetActive(true);
        //    }
        //}


    }
}
