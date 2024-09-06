using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Collections;

public class SNSSocketHandler : MonoBehaviour
{
//    public string address = ConstantsGod.API_BASEURL;
//    public static SNSSocketHandler Instance;
//    public SocketManager Manager;

//    public bool isSNSFeedSocketEvent = false;

//    private SocketResponce msgResponce = new SocketResponce();
//    private GroupLeaveResponceRoot leaveGroupResponce = new GroupLeaveResponceRoot();

//    private void Awake()
//    {
//        /*if (Instance == null)
//        {
//            Instance = this;
//        }*/
//    }

//    private void OnEnable()
//    {
//        Instance = this;
//    }

//    IEnumerator Start()
//    {
//        yield return new WaitForSeconds(1f);
//        address = ConstantsGod.API_BASEURL;
//        //Debug.Log("Address:" + address);
//        if (!address.EndsWith("/"))
//        {
//            address = address + "/";
//        }
//        Debug.Log("<color=red> Socket Handler Address:" + address + "</color>");
//        Manager = new SocketManager(new Uri((address)));
//        Manager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
//        Manager.Socket.On<CustomError>(SocketIOEventTypes.Error, OnError);

//        //ResetListener();
//    }

//    public void Connect()
//    {
//        //Debug.Log("hi from server");
//    }

//    void OnConnected(ConnectResponse resp)
//    {
//        //Debug.Log("SNSSocketHandler OnConnected:"+isSNSFeedSocketEvent);
//        Manager.Socket.Emit("hi", "hiiii");
//        if (!isSNSFeedSocketEvent)
//        {
//            //OnGetMessageAfterReconnectSocket();
//        }
//    }

//    void OnError(CustomError args)
//    {
//        //Debug.Log("SNSSocketHandler OnError");
//       Debug.Log(string.Format("Error: {0}", args.ToString()));
//    }

//    void Onresult(CustomError args)
//    {
//       Debug.Log("SNSSocketHandler Onresult");
//       Debug.Log(string.Format("Error: {0}", args.ToString()));
//    }

//    //public void ResetListener()
//    //{
//    //    //Debug.Log("Listen");
//    //    if (isSNSFeedSocketEvent)
//    //    {
//    //        Manager.Socket.On<string>("FeedComment", FeedCommentResponse);

//    //        Manager.Socket.On<string>("FeedLike", FeedLikeResponse);
//    //    }
//    //    else
//    //    {
//    //        Manager.Socket.On<string>("MessageReceived", MessageReceivedResponse);

//    //        Manager.Socket.On<string>("GroupCreated", GroupCreatedResponse);

//    //        Manager.Socket.On<string>("GroupLeaved", GroupLeaveUserResponse);

//    //        Manager.Socket.On<string>("GroupDeleted", GroupDeleteResponse);
//    //    }
//    //}

//    #region SNS Message Module Socket Events.......
//    //public void GroupDeleteResponse(string s)
//    //{
//    //    if (!this.gameObject.activeInHierarchy)
//    //        return;

//    //    //Debug.Log("Group delete response:" + s);
//    //    DeleteGroupRoot deleteGroupResponce = JsonConvert.DeserializeObject<DeleteGroupRoot>(s);

//    //    if (SNS_MessageController.Instance.allChatGetConversationDatum != null)//user in chat or details screen show popup and ok press goto conversation screen and clear data 
//    //    {
//    //        //Debug.Log("GroupDeleted:" + SNS_MessageController.Instance.allChatGetConversationDatum.group.createdBy);
//    //        if(SNS_MessageController.Instance.allChatGetConversationDatum.receivedGroupId == deleteGroupResponce.groupId && SNS_MessageController.Instance.allChatGetConversationDatum.group.createdBy != SNS_APIManager.Instance.userId)
//    //        {
//    //            SNS_MessageController.Instance.groupDeletedShowPopupForOtherUser.SetActive(true);
//    //        }
//    //    }
//    //    else
//    //    {
//    //        //check group id on conversation list is available then clear require data and destroy object.......
//    //        SNS_MessageController.Instance.ResetAndRefreshMessageModule();
//    //    }
//    //}

//    ////this method is used to Group leave user response.......
//    //public void GroupLeaveUserResponse(string s)
//    //{
//    //    if (!this.gameObject.activeInHierarchy)
//    //        return;

//    //    //Debug.Log("Group Leave Responce Data:" + s);
//    //    leaveGroupResponce = JsonConvert.DeserializeObject<GroupLeaveResponceRoot>(s);
//    //    SNS_MessageController.Instance.LeaveGroupAfterRemoveMemberFromCurrentConversation(leaveGroupResponce);
//    //}

//    //public void GroupCreatedResponse(string s)
//    //{
//    //    //if (!this.gameObject.activeInHierarchy)
//    //        //return;

//    //    //Debug.Log("socket Group Created Response Data:" + s);
//    //    msgResponce = JsonConvert.DeserializeObject<SocketResponce>(s);
//    //    for (int i = 0; i < msgResponce.userList.Count; i++)
//    //    {
//    //        if (int.Parse(msgResponce.userList[i]) == SNS_APIManager.Instance.userId)
//    //        {
//    //            SNS_APIManager.Instance.RequestChatGetConversation();
//    //        }
//    //    }
//    //}

//    ////this method is used to Reconnect socket event and chat screen open then call get message api.......
//    //bool isReconnectGetmessage = false;
//    //void OnGetMessageAfterReconnectSocket()
//    //{
//    //    //Debug.Log("OnGetMessageAfterReconnectSocket0000000.......:"+ isReconnectGetmessage);
//    //    if (!isReconnectGetmessage && SNS_MessageController.Instance != null && SNS_MessageController.Instance.allChatGetConversationDatum != null)
//    //    {
//    //        if (!SNS_MessageController.Instance.ChatScreen.activeSelf)
//    //        {
//    //            //Debug.Log("Refresh  Conversation call.......");
//    //            return;
//    //        }
//    //        //Debug.Log("OnGetMessageAfterReconnectSocket1111111.......");
//    //        //Debug.Log("2");
//    //        if (SNS_MessageController.Instance.allChatGetConversationDatum.receivedGroupId != 0)
//    //        {
//    //            //Debug.Log("3");
//    //            RequestChatGetMessagesSocket(1, 50, 0, SNS_MessageController.Instance.allChatGetConversationDatum.receivedGroupId);
//    //        }
//    //        else if (SNS_MessageController.Instance.allChatGetConversationDatum.receiverId != 0)
//    //        {
//    //            //Debug.Log("4");
//    //            if (SNS_MessageController.Instance.allChatGetConversationDatum.receiverId == SNS_APIManager.Instance.userId)
//    //            {
//    //                //Debug.Log("5");
//    //                RequestChatGetMessagesSocket(1, 50, SNS_MessageController.Instance.allChatGetConversationDatum.senderId, 0);
//    //            }
//    //            else
//    //            {
//    //                //  Debug.Log("6");
//    //                RequestChatGetMessagesSocket(1, 50, SNS_MessageController.Instance.allChatGetConversationDatum.receiverId, 0);
//    //            }
//    //        }
//    //    }
//    //}

//    ////this method is used to message received response.......
//    //public void MessageReceivedResponse(string s)
//    //{
//    //    //Debug.Log("socket MessageReceivedResponce Data:" + s + ":Name:"+this.transform.parent.parent.name);
//    //    //if (!this.gameObject.activeInHierarchy)
//    //        //return;

//    //    //  SNS_APIManager.Instance.isCreateMessage = true;
//    //    msgResponce = JsonConvert.DeserializeObject<SocketResponce>(s);

//    //    //Debug.Log("MessageReceivedResponse:" + msgResponce.userList.Count);
//    //    for (int i = 0; i < msgResponce.userList.Count; i++)
//    //    {
//    //        //Debug.Log("MessageReceivedResponce:"+msgResponce.userList[i]);
//    //        if (int.Parse(msgResponce.userList[i]) == SNS_APIManager.Instance.userId)
//    //        {
//    //            if (SNS_MessageController.Instance != null && !SNS_MessageController.Instance.ChatScreen.activeSelf)
//    //            {
//    //                //Debug.Log("Refresh  Conversation call.......");
//    //                SNS_APIManager.Instance.RequestChatGetConversation();
//    //            }

//    //            if (!this.gameObject.activeInHierarchy)
//    //                return;

//    //            if (SNS_MessageController.Instance.allChatGetConversationDatum != null)
//    //            {
//    //                //Debug.Log("2");
//    //                if (SNS_MessageController.Instance.allChatGetConversationDatum.receivedGroupId != 0)
//    //                {
//    //                    //Debug.Log("3");
//    //                    RequestChatGetMessagesSocket(1, 50, 0, SNS_MessageController.Instance.allChatGetConversationDatum.receivedGroupId);
//    //                }
//    //                else if (SNS_MessageController.Instance.allChatGetConversationDatum.receiverId != 0)
//    //                {
//    //                    //Debug.Log("4");
//    //                    if (SNS_MessageController.Instance.allChatGetConversationDatum.receiverId == SNS_APIManager.Instance.userId)
//    //                    {
//    //                        //Debug.Log("5");
//    //                        RequestChatGetMessagesSocket(1, 50, SNS_MessageController.Instance.allChatGetConversationDatum.senderId, 0);
//    //                    }
//    //                    else
//    //                    {
//    //                        //  Debug.Log("6");
//    //                        RequestChatGetMessagesSocket(1, 50, SNS_MessageController.Instance.allChatGetConversationDatum.receiverId, 0);
//    //                    }
//    //                }
//    //            }
//    //            break;
//    //        }
//    //    }
//    //}

//    //public void RequestChatGetMessagesSocket(int message_pageNumber, int message_pageSize, int message_receiverId, int message_receivedGroupId)
//    //{
//    //    isReconnectGetmessage = true;
//    //    SNS_APIManager.Instance.r_isCreateMessage = true;
//    //    StartCoroutine(IERequestChatGetMessagesSocket(message_pageNumber, message_pageSize, message_receiverId, message_receivedGroupId));
//    //}
//    //public IEnumerator IERequestChatGetMessagesSocket(int message_pageNumber, int message_pageSize, int message_receiverId, int message_receivedGroupId)
//    //{
//    //    WWWForm form = new WWWForm();
//    //    form.AddField("pageNumber", message_pageNumber);
//    //    form.AddField("pageSize", message_pageSize);
//    //    if (message_receivedGroupId != 0)
//    //    {
//    //        form.AddField("receivedGroupId", message_receivedGroupId);
//    //    }
//    //    else if (message_receiverId != 0)
//    //    {
//    //        form.AddField("receiverId", message_receiverId);
//    //    }

//    //    using (UnityWebRequest www = UnityWebRequest.Post((address + ConstantsGod.r_url_ChatGetMessages), form))
//    //    {
//    //        www.SetRequestHeader("Authorization", SNS_APIManager.Instance.userAuthorizeToken);

//    //        yield return www.SendWebRequest();

//    //        isReconnectGetmessage = false;

//    //        if (www.isNetworkError || www.isHttpError)
//    //        {
//    //            Debug.Log(www.error);
//    //            if (message_receivedGroupId != 0)
//    //            {
//    //                SNS_APIManager.Instance.r_isCreateMessage = false;
//    //            }
//    //        }
//    //        else
//    //        {
//    //            // Debug.Log("Form upload complete!");
//    //            string data = www.downloadHandler.text;
//    //            //Debug.Log("socket Message Chat: " + data);
//    //            var settings = new JsonSerializerSettings
//    //            {
//    //                NullValueHandling = NullValueHandling.Ignore,
//    //                MissingMemberHandling = MissingMemberHandling.Ignore
//    //            };
//    //            SNS_APIManager.Instance.allChatMessagesRoot = JsonConvert.DeserializeObject<ChatGetMessagesRoot>(data, settings);

//    //            if (CommonAPIManager.Instance != null)//For Get All Chat UnRead Message Count.......
//    //            {
//    //                CommonAPIManager.Instance.RequestGetAllChatUnReadMessagesCount();
//    //            }
//    //            yield return new WaitForSeconds(0.03f);
//    //            SNS_APIController.Instance.GetAllChat(message_pageNumber, "SNSSocketHandler");
//    //        }
//    //    }
//    //}
//    #endregion

//    #region SNS Feed Module Socket Events.......
//    //public void FeedCommentResponse(string s)
//    //{
//    //    if (!this.gameObject.activeInHierarchy)
//    //        return;
//    //    Debug.Log("Feed Comment response:" + s + ":Name:" + this.transform.parent.parent.name);

//    //    FeedCommentSocketRoot feedCommentSocketRoot = JsonConvert.DeserializeObject<FeedCommentSocketRoot>(s);

//    //    FeedUIController.Instance.FeedCommentAndLikeSocketEventSuccessAfterUpdateRequireFeedResponse(feedCommentSocketRoot.feedId, feedCommentSocketRoot.createdBy, 0, feedCommentSocketRoot.commentCount, false);
//    //}

//    //public void FeedLikeResponse(string s)
//    //{
//    //    if (!this.gameObject.activeInHierarchy)
//    //        return;
//    //   Debug.Log("Feed Like response:" + s + ":Name:" + this.transform.parent.parent.name);

//    //    FeedLikeSocketRoot feedLikeSocketRoot = JsonConvert.DeserializeObject<FeedLikeSocketRoot>(s);
//    //   Debug.Log("feedLikeSocketRoot.createdBy" + feedLikeSocketRoot.createdBy );
//    //   Debug.Log("SNS_APIManager.Instance.userId" + SNS_APIManager.Instance.userId);
//    //   Debug.Log("SNS_APIManager.Instance.userId" + SNS_APIManager.Instance.userId);

//    //    //if (feedLikeSocketRoot.createdBy == SNS_APIManager.Instance.userId)
//    //    //    return;
//    //    // LikeDislikeSuccessAfterUpdateRequireFeedResponse
//    //    FeedUIController.Instance.FeedCommentAndLikeSocketEventSuccessAfterUpdateRequireFeedResponse(feedLikeSocketRoot.feedId, feedLikeSocketRoot.createdBy, feedLikeSocketRoot.likeCount, 0, true);
//    //}
//    #endregion
//}


//class CustomError : Error
//{
//    public ErrorData data;

//    public override string ToString()
//    {
//        return $"[CustomError {message}, {data?.code}, {data?.content}]";
//    }
}

//class ErrorData
//{
//    public int code;
//    public string content;
//}

//[System.Serializable]
//public class SocketResponce
//{
//    public List<string> userList;
//}

//[System.Serializable]
//public class GroupLeaveResponceRoot
//{
//    public List<int> userList = new List<int>();
//    public int groupId; 
//}

//[System.Serializable]
//public class DeleteGroupRoot
//{
//    public int groupId;
//}

//[System.Serializable]
//public class FeedLikeSocketRoot
//{
//    public int createdBy;
//    public int feedId;
//    public int likeCount;
//    public int feedCreatedBy;
//}

//[System.Serializable]
//public class FeedCommentSocketRoot
//{
//    public int createdBy;
//    public int feedId;
//    public int commentCount;
//    public int feedCreatedBy;
//}