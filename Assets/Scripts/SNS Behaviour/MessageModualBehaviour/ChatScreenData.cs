using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatScreenData : MonoBehaviour
{
    public static ChatScreenData Instance;
    public ChatGetConversationDatum allChatGetConversationDatum;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void OnRefereshGetMessageApi()
    {
        SNS_APIResponseManager.Instance.r_isCreateMessage = true;
        if (allChatGetConversationDatum.receiverId != 0)
        {
            if (allChatGetConversationDatum.receiverId == SNS_APIResponseManager.Instance.userId)
            {
                SNS_APIResponseManager.Instance.RequestChatGetMessages(1, 50, allChatGetConversationDatum.senderId, 0, "");
            }
            else
            {
                SNS_APIResponseManager.Instance.RequestChatGetMessages(1, 50, allChatGetConversationDatum.receiverId, 0, "");
            }
            //Debug.LogError("receiverId" + allChatGetConversationDatum.receiverId);
        }
        else if (allChatGetConversationDatum.receivedGroupId != 0)
        {
            //Debug.LogError("receivedGroupId" + allChatGetConversationDatum.receivedGroupId);
            SNS_APIResponseManager.Instance.RequestChatGetMessages(1, 50, 0, allChatGetConversationDatum.receivedGroupId, "");
        }
        SNS_SMSModuleManager.Instance.allChatGetConversationDatum = allChatGetConversationDatum;
    }
}