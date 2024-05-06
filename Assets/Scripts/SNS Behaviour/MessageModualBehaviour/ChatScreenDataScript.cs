using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatScreenDataScript : MonoBehaviour
{
    //public static ChatScreenDataScript Instance;
    //public ChatGetConversationDatum allChatGetConversationDatum;

    //private void Awake()
    //{
    //    if (Instance == null)
    //    {
    //        Instance = this;
    //    }
    //}

    //public void OnRefereshGetMessageApi()
    //{
    //    SNS_APIManager.Instance.r_isCreateMessage = true;
    //    if (allChatGetConversationDatum.receiverId != 0)
    //    {
    //        if (allChatGetConversationDatum.receiverId == SNS_APIManager.Instance.userId)
    //        {
    //            SNS_APIManager.Instance.RequestChatGetMessages(1, 50, allChatGetConversationDatum.senderId, 0, "");
    //        }
    //        else
    //        {
    //            SNS_APIManager.Instance.RequestChatGetMessages(1, 50, allChatGetConversationDatum.receiverId, 0, "");
    //        }
    //        //Debug.LogError("receiverId" + allChatGetConversationDatum.receiverId);
    //    }
    //    else if (allChatGetConversationDatum.receivedGroupId != 0)
    //    {
    //        //Debug.LogError("receivedGroupId" + allChatGetConversationDatum.receivedGroupId);
    //        SNS_APIManager.Instance.RequestChatGetMessages(1, 50, 0, allChatGetConversationDatum.receivedGroupId, "");
    //    }
    //    SNS_MessageController.Instance.allChatGetConversationDatum = allChatGetConversationDatum;
    //}
}