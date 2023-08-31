using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;


using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using Newtonsoft.Json;
using System.Collections.Generic;

public class XanaChatSocket_Waqas : MonoBehaviour
{
    public string address = "https://chat-testing.xana.net/";
    public SocketManager Manager;

    //public TMP_Text connectedText;
    //public TMP_Text joinedText;
    //public TMP_Text receivedMsg;
    //public TMP_Text oldMsgRec;
    // /api/v1/fetch-world-chat-byId/worldId/:userId/:page/:limit
    public string fetchAllMsgApi = "https://chat-testing.xana.net/api/v1/fetch-world-chat-byId/";

    public int worldId, pageNumber, dataLimit;

    public string socketId;
    public string testMsgString = "";
    public ChatUserData receivedMsgForTesting;
    bool isJoinRoom = false;



    #region Summery

    // Join Room : socket.emit('joinRoom', { username, room });
    //             Params: username : user id
    //                     room: world id

    // Sending Message : socket.emit('chatMessage', {username, event_id, room, msg});
    //                   Params: username : user id
    //                    username : user id
    //                    event_id : event id in the specific world (if there is no event id then send 1 as a default)
    //                        room : world id
    //                         msg : message

    // Receiving Message : socket.on('message', function)
    //           Keyword : message
    //          function : Function which invoke when callback received

    #endregion

    public static Action<string> onJoinRoom;
    public static Action<string, string> onSendMsg;
    public static Action callApi;


    void Start()
    {
        if (!address.EndsWith("/"))
            address = address + "/";

        Manager = new SocketManager(new Uri((address)));
        Manager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
        Manager.Socket.On<CustomError>(SocketIOEventTypes.Error, OnError);
        Manager.Socket.On<CustomError>(SocketIOEventTypes.Disconnect, OnSocketDisconnect);
    }
    private void OnEnable()
    {
        onJoinRoom += UserJoinRoom;
        onSendMsg += SendMsg;
        callApi += CallApiForMessages;
    }
    private void OnDisable()
    {
        onJoinRoom -= UserJoinRoom;
        onSendMsg -= SendMsg;
        callApi -= CallApiForMessages;
    }


    #region Socket Calls Handling

    void OnConnected(ConnectResponse resp)
    {
        socketId = resp.sid;
        Debug.Log("<color=blue> XanaChat -- SocketConnected : " + resp.sid + "</color>");
        Manager.Socket.On<ChatUserData>("message", ReceiveMsgs);

        if (isJoinRoom)
        {
            // Socket ID Update After Reconnect 
            // Need To Emit joinRoom again with new Socket Id

            onJoinRoom?.Invoke(XanaConstants.xanaConstants.MuseumID);
        }
    }
    void OnError(CustomError args)
    {
        Debug.Log("<color=red>" + string.Format("Error: {0}", args.ToString()) + "</color>");
    }
    void Onresult(CustomError args)
    {
        Debug.Log("<color=red>" + string.Format("Error: {0}", args.ToString()) + "</color>");
    }
    void OnSocketDisconnect(CustomError args)
    {
        Debug.Log("<color=red>" + string.Format("Error: {0}", args.ToString()) + "</color>");
    }


    void ReceiveMsgs(ChatUserData msg)
    {
        if (string.IsNullOrEmpty(msg.message))
            return;

        Debug.Log("<color=blue> XanaChat -- MsgReceive : " + msg.username + " : " + msg.message + "</color>");
        if (string.IsNullOrEmpty(msg.username))
        {
            msg.username = socketId;
        }
        receivedMsgForTesting = msg;
        XanaChatSystem.instance.DisplayMsg_FromSocket(msg.username, msg.message);
    }
    void UserJoinRoom(string worldId)
    {
        string userId = XanaConstants.xanaConstants.userId;
        var data = new { username = userId, room = worldId };
        Debug.Log("<color=blue> XanaChat -- JoinRoom : " + userId + " - " + worldId + "</color>");

        isJoinRoom = true;
        Manager.Socket.Emit("joinRoom", data);
    }
    void SendMsg(string world_Id, string msg)
    {
        string userId = XanaConstants.xanaConstants.userId;
        string event_Id = "1";

        // Checking For Event
        if (XanaEventDetails.eventDetails.DataIsInitialized)
        {
            event_Id = XanaEventDetails.eventDetails.id.ToString();
        }

        Debug.Log("<color=blue> XanaChat -- MsgSend : " + userId + " - " + event_Id + " - " + world_Id + " - " + msg + "</color>");


        var data = new { userId = userId, eventId = event_Id, worldId = world_Id, msg = msg };
        Manager.Socket.Emit("chatMessage", data);
    }

    //To fetch Old Messages from a server against any world
    public void CallApiForMessages()
    {
        Debug.Log("Calling API");
        StartCoroutine(FetchOldMessages());
    }
    IEnumerator FetchOldMessages()
    {
        string token = ConstantsGod.AUTH_TOKEN;
        WWWForm form = new WWWForm();



        string api = fetchAllMsgApi + XanaConstants.xanaConstants.MuseumID + "/" + socketId + "/" + pageNumber + "/" + dataLimit;
        Debug.Log("<color=red> XanaChat -- API : " + api + "</color>");

        UnityWebRequest www;
        www = UnityWebRequest.Get(api);


        //www.SetRequestHeader("Authorization", token);
        www.SendWebRequest();

        while (!www.isDone)
        {
            yield return null;
        }


        if (!www.isHttpError && !www.isNetworkError)
        {
            //oldMsgRec.text = www.downloadHandler.text;
            Debug.Log("<color=green> XanaChat -- OldMessages : " + www.downloadHandler.text + "</color>");
            DisplayOldChat(www.downloadHandler.text);
        }
        else
            Debug.Log("<color=red> XanaChat -- NetWorkissue </color>");

        www.Dispose();
    }

    void DisplayOldChat(string OldChat)
    {
        //if(!string.IsNullOrEmpty(OldChat))

        //OldChatOutPut myChat = JsonUtility.FromJson<OldChatOutPut>(OldChat);
        ////JsonUtility.FromJson<S3NftDetail>(jsonData);
        //if (myChat.count > 0)
        //{
        //    string tempUserName = "";
        //    for (int i = myChat.data.Count - 1; i > -1; i--)
        //    {
        //        if (string.IsNullOrEmpty(myChat.data[i].username))
        //            tempUserName = "Xana";
        //        else
        //            tempUserName = myChat.data[i].username;

        //        XanaChatSystem.instance.DisplayMsg_FromSocket(tempUserName, myChat.data[i].message);
        //    }
        //}


        RootData rootData = JsonUtility.FromJson<RootData>(OldChat);
        if (rootData.count > 0)
        {
            string tempUserName = "";
            for (int i = rootData.data.Count - 1; i > -1; i--)
            {
                if (string.IsNullOrEmpty(rootData.data[i].username))
                    tempUserName = "Xana";
                else
                    tempUserName = rootData.data[i].username;

                XanaChatSystem.instance.DisplayMsg_FromSocket(tempUserName, rootData.data[i].message);
            }
        }


    }

    #endregion


    // Testing Function
    public void CallJoinRoom()
    {
        UserJoinRoom(worldId.ToString());
    }
    public void TestSendMsg()
    {
        SendMsg(worldId.ToString(), testMsgString);
    }


}


[System.Serializable]
public class ChatUserData
{
    public string name;
    public string username;
    public string avatar;
    public string message;
    public string world;
    public string eventId;
    public long time;
}


//[System.Serializable]
//public class OldChatOutPut
//{
//    public bool success;
//    public List<OldChatMessage> data;
//    public int count;
//}

//[System.Serializable]
//public class OldChatMessage
//{
//    public string username;
//    public string avatar;
//    public string message;
//    public DateTime time;
//    public DateTime createdAt;
//}

[System.Serializable]
public class MessageData
{
    public string username;
    public string avatar;
    public string message;
    public DateTime time;
    public DateTime createdAt;
}
[System.Serializable]
public class RootData
{
    public bool success;
    public List<MessageData> data;
    public int count;
}
