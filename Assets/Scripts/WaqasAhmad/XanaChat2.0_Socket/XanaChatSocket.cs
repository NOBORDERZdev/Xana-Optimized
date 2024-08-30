using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;
using System.Net.Http;
using System.Threading.Tasks;

using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Text;

public enum CallBy { User, UserNpc, FreeSpeechNpc, NpcToNpc };
public class XanaChatSocket : MonoBehaviour
{
    // /api/v1/fetch-world-chat-byId/worldId/:userId/:page/:limit

    // Test-Net
    // https://chat-testing.xana.net/
    // https://chat-testing.xana.net/api/v1/fetch-world-chat-byId/
    // https://chat-testing.xana.net/api/v1/set-device-id-against-socketId

    // Main-Net
    // https://chat-prod.xana.net/
    // https://chat-prod.xana.net/api/v1/fetch-world-chat-byId/
    // https://chat-prod.xana.net/api/v1/set-device-id-against-socketId

    string socketTestnet = "https://chat-testing.xana.net/";
    // Fetch API updated https://chat-testing.xana.net/api/v1/fetch-world-chat-byEventId/:worldId/:eventId/:userId/:page/:limit
    //string fetchApiTestnet = "https://chat-testing.xana.net/api/v1/fetch-world-chat-byId/";
    //string fetchApiTestnet = "https://chat-testing.xana.net/api/v1/fetch-world-chat-byEventId/";
    string apiGusetNameTestnet = "https://chat-testing.xana.net/api/v1/set-device-id-against-socketId";


    string socketMainnet = "https://chat-prod.xana.net/";
    //string fetchApiMainnet = "https://chat-prod.xana.net/api/v1/fetch-world-chat-byId/";
    string apiGusetNameMainnet = "https://chat-prod.xana.net/api/v1/set-device-id-against-socketId";


    string fetchApi = "api/v1/fetch-world-chat-byEventId/";

    public SocketManager Manager;

    string address;
    string fetchAllMsgApi;
    string setGuestNameApi;
    int worldId,
        eventId = 1,
        pageNumber = 1, // API Parameters
        dataLimit = 40; //200;

    public string socketId;
    public string oldChatResponse;
    public ChatUserData receivedMsgForTesting;
    bool isJoinRoom = false;

    public static XanaChatSocket instance;

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
    public static Action<string, string, CallBy, string> onSendMsg;
    public static Action callApi;
    public Action<string> npcSendMsg;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        if (APIBaseUrlChange.instance.IsXanaLive)
        {
            address = socketMainnet;
            fetchAllMsgApi = address + fetchApi;
            setGuestNameApi = apiGusetNameMainnet;
        }
        else
        {
            address = socketTestnet;
            fetchAllMsgApi = address + fetchApi;
            setGuestNameApi = apiGusetNameTestnet;
        }

        if (!address.EndsWith("/"))
            address = address + "/";

        // Default Method
        Manager = new SocketManager(new Uri((address)));
        Manager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
        Manager.Socket.On<CustomError>(SocketIOEventTypes.Error, OnError);
        Manager.Socket.On<CustomError>(SocketIOEventTypes.Disconnect, OnSocketDisconnect);

        if (XanaConstants.xanaConstants.EnviornmentName.Contains("PMY"))
        {
            eventId = XanaConstants.xanaConstants.pmySchooldDataID;
        }
        else if (XanaEventDetails.eventDetails.DataIsInitialized)
        {
            eventId = XanaEventDetails.eventDetails.id;
        }

        // Custom Method
        Manager.Socket.On<ChatUserData>("message", ReceiveMsgs);
        StartCoroutine(FetchOldMessages());
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

        // Switch Off This Socket
        Manager.Close();
    }


    #region Socket Calls Handling

    void OnConnected(ConnectResponse resp)
    {
        socketId = resp.sid;
        //Debug.Log("<color=blue> XanaChat -- SocketConnected : " + resp.sid + "</color>");
        //XanaChatSystem.instance.DisplayErrorMsg_FromSocket("Xana Chat Connected", "Yes");

        //socket.Off("event", listener);
        //Manager.Socket.Off();



        // is it reconnected or First time
        if (isJoinRoom)
        {
            // Socket ID Update After Reconnect 
            // Need To Emit joinRoom again with new Socket Id

            onJoinRoom?.Invoke(XanaConstants.xanaConstants.MuseumID);
        }

        if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
            StartCoroutine(SubmitGuestUserNameWithJson());
    }
    void OnError(CustomError args)
    {
        //Debug.Log("<color=red>" + string.Format("Error: {0}", args.ToString()) + "</color>");
        //XanaChatSystem.instance.DisplayErrorMsg_FromSocket("Xana Chat Reconnecting", "Error");
    }
    void Onresult(CustomError args)
    {
        //Debug.Log("<color=red>" + string.Format("Error: {0}", args.ToString()) + "</color>");
    }
    void OnSocketDisconnect(CustomError args)
    {
        //Debug.Log("<color=red>" + string.Format("Error: {0}", args.ToString()) + "</color>");
        //XanaChatSystem.instance.DisplayErrorMsg_FromSocket("Xana Chat Reconnecting", "Error");
    }


    void UserJoinRoom(string _worldId)
    {
        worldId = int.Parse(_worldId);
        string userId = XanaConstants.xanaConstants.userId;
        var data = new { username = userId, room = _worldId };
        //Debug.Log("<color=blue> XanaChat -- JoinRoom : " + userId + " - " + _worldId + "</color>");

        isJoinRoom = true;
        Manager.Socket.Emit("joinRoom", data);
    }
    void SendMsg(string world_Id, string msg, CallBy callBy, string npcId = "")
    {
        if (string.IsNullOrEmpty(msg))
        {
            //Debug.Log("<color=blue> XanaChat -- EmptyMsg </color>");
            return;
        }

        if (msg.All(c => char.IsWhiteSpace(c)))
        {
            //Debug.Log("<color=blue> XanaChat -- EmptySpacedMsg </color>");
            return;
        }

        string userId = XanaConstants.xanaConstants.userId;
        string event_Id = "1";  // If there is no Event  then send 1 as a default

        // Checking For Event
        if (XanaEventDetails.eventDetails.DataIsInitialized)
        {
            event_Id = XanaEventDetails.eventDetails.id.ToString();
        }
        else if (XanaConstants.xanaConstants.EnviornmentName.Contains("PMY"))
        {
            event_Id = "" + XanaConstants.xanaConstants.pmySchooldDataID;
        }

        eventId = int.Parse(event_Id);

        if (!npcId.IsNullOrEmpty())
            userId = "npc-" + npcId;

        if (callBy.Equals(CallBy.NpcToNpc))
            npcSendMsg.Invoke(msg);

        // //Debug.Log("<color=red> XanaChat -- MsgSend : " + userId /*+ " - " + event_Id + " - " + world_Id + " - " + msg */ + " : " + npcId + "</color>");
        var data = new { userId, eventId = event_Id, worldId = world_Id, msg = msg };
        //Debug.Log("XanaChat --:" + data);
        Manager.Socket.Emit("chatMessage", data);
    }

    void ReceiveMsgs(ChatUserData msg)
    {
        //Debug.Log("<color=blue> XanaChat -- MsgReceive : " + msg.username + " : " + msg.message + "</color>");

        if (string.IsNullOrEmpty(msg.message))
            return;

        if (eventId != msg.event_id)
            return;

        string tempUser = msg.name;//msg.username;
        if (PlayerPrefs.GetInt("IsLoggedIn") == 0 && string.IsNullOrEmpty(msg.name))
            tempUser = msg.guestusername; 
        else if(string.IsNullOrEmpty(msg.name))
            tempUser = msg.username;

        receivedMsgForTesting = msg;

        if (CheckUserNameIsValid(tempUser))
        {
            //tempUser = msg.socket_id;
            tempUser = "XanaUser-(" + msg.socket_id + ")";//XanaUser-(userId)
        }
        if (XanaChatSystem.instance.gameObject.activeSelf)
            XanaChatSystem.instance.DisplayMsg_FromSocket(tempUser, msg.message);
    }
    bool CheckUserNameIsValid(string _UserName)
    {
        if (string.IsNullOrEmpty(_UserName) ||
            _UserName.All(c => char.IsWhiteSpace(c)) ||
            _UserName.Contains("null") ||
            _UserName.Contains("Null"))
            return true;
        else
            return false;
    }

    #endregion

    //To fetch Old Messages from a server against any world
    public void CallApiForMessages()
    {
        //Debug.Log("Calling API");
        //StartCoroutine(FetchOldMessages());


        DisplayOldChat(oldChatResponse);
    }
    public IEnumerator FetchOldMessages()
    {
        yield return new WaitForSeconds(5f);

        string token = ConstantsGod.AUTH_TOKEN;
        WWWForm form = new WWWForm();

        string api = fetchAllMsgApi + XanaConstants.xanaConstants.MuseumID + "/" + eventId + "/" + socketId + "/" + pageNumber + "/" + dataLimit;
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
            //Debug.Log("<color=green> XanaChat -- OldMessages : " + www.downloadHandler.text + "</color>");

            // Locally Save the Response
            oldChatResponse = www.downloadHandler.text;
            DisplayOldChat(www.downloadHandler.text);
        }
        else
            //Debug.Log("<color=red> XanaChat -- NetWorkissue </color>");

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
            //string tempUserName = "";
            for (int i = rootData.data.Count - 1; i > -1; i--)
            {
                string tempUser = rootData.data[i].name;
                if (rootData.data[i].guest)
                {
                    tempUser = rootData.data[i].guest_username;
                }
                else if (string.IsNullOrEmpty(tempUser) || tempUser.Contains("null"))
                {
                    tempUser = tempUser = "XanaUser-(" + socketId + ")";//XanaUser-(userId)
                }

                XanaChatSystem.instance.DisplayMsg_FromSocket(tempUser, rootData.data[i].message);
            }
        }


    }

    private IEnumerator SubmitGuestUserNameWithJson()
    {
        // Create a data object and serialize it to JSON
        string tempDeviceID = SystemInfo.deviceUniqueIdentifier;
        string tempUserName = PlayerPrefs.GetString(ConstantsGod.GUSTEUSERNAME);
        if (string.IsNullOrEmpty(tempUserName))
        {
            tempUserName = XanaChatSystem.instance.UserName;
        }



        ApiParameter requestData = new ApiParameter { username = tempUserName, deviceId = tempDeviceID, socketId = socketId };
        string jsonData = JsonUtility.ToJson(requestData);


        //Debug.LogError("<color=red> XanaChat -- UserNameData : " + socketId + "  :  " + tempDeviceID + "  :  " + tempUserName + "</color>");
        // //Debug.LogError("<color=red> XanaChat -- UserNameAPI : " + setGuestNameApi + "</color>");

        // Create a UnityWebRequest for the POST request
        using (UnityWebRequest request = new UnityWebRequest(setGuestNameApi, "POST"))
        {
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            //if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            //{
            //    //Debug.LogError("Error: " + request.error);
            //}
            //else
            //{
            //    // Request was successful
            //    //Debug.LogError("Request Successful");
            //    //Debug.LogError("Response: " + request.downloadHandler.text);
            //}
        }
    }
}


[System.Serializable]
public class ChatUserData
{
    public string userId;
    public string socket_id;
    public string username;
    public string name;
    public string avatar;
    public string message;
    public string world;
    public string world_entity;
    public int event_id;
    public int world_id;
    public string emotion;
    public string time;
    public string timestamp;
    public string guestusername;
    public string profileIconColor;
    public string image;
    public string video;
    public bool isLiked;
    public int likesCount;
}
//{
//    socket_id: _socketId,
//    username: _username,
//    avatar: _avatar,
//    message: _text,
//    world: _worldName,
//    event_id: _eventId,
//    world_id: _worldId,
//    time: Date.now()
//  }


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
    public string name;
    public string avatar;
    public string message;
    public DateTime time;
    public DateTime createdAt;
    public bool guest;
    public string guest_username;
}
[System.Serializable]
public class RootData
{
    public bool success;
    public List<MessageData> data;
    public int count;
}

[System.Serializable]
public class ApiParameter
{
    public string socketId;
    public string deviceId;
    public string username;
}