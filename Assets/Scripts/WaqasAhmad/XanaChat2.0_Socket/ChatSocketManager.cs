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
using System.Text;

public enum CallBy { User, UserNpc, FreeSpeechNpc, NpcToNpc };
public class ChatSocketManager : MonoBehaviour
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

    public static ChatSocketManager instance;


    private bool isConnected = false;
    private bool wasPaused = false;
    private int retryInterval = 5; // Interval (in seconds) between retry attempts
    private int maxRetries = 5;  // Maximum number of retries
    private int currentRetry = 0;

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

    #region MonoBehaviour functions

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


    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        if (APIBasepointManager.instance.IsXanaLive)
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


        InitializeSocket();
        StartCoroutine(RetryConnection());

        // Default Method

    }

    #endregion

    #region Socket Calls Handling

    void InitializeSocket()
    {
        Manager = new SocketManager(new Uri((address)));
        Manager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
        Manager.Socket.On<CustomError>(SocketIOEventTypes.Error, OnError);
        Manager.Socket.On<CustomError>(SocketIOEventTypes.Disconnect, OnSocketDisconnect);
        if (XanaEventDetails.eventDetails.DataIsInitialized)
        {
            eventId = XanaEventDetails.eventDetails.id;
        }
        // Custom Method
        Manager.Socket.On<ChatUserData>("message", ReceiveMsgs);

        StartCoroutine(FetchOldMessages());
    }

    IEnumerator RetryConnection()
    {
        yield return new WaitForSeconds(2f);
        while (!isConnected && currentRetry < maxRetries)
        {
            Debug.Log($"Attempting to connect, try {currentRetry + 1}/{maxRetries}...");

            // Re-initialize the socket manager to reconnect
            Manager.Socket.Off(); // Remove existing event handlers to avoid duplicates
            InitializeSocket();

            yield return new WaitForSeconds(retryInterval);

            currentRetry++;
        }

        if (isConnected)
        {
            Debug.Log("Successfully connected after retries.");
        }
        else
        {
            Debug.LogError($"Failed to connect after {maxRetries} attempts.");
        }
    }

    void OnConnected(ConnectResponse resp)
    {

        Debug.Log("Socket Connected");
        isConnected = true;
        currentRetry = 0; // Reset retry count on successful connection

        socketId = resp.sid;
        // is it reconnected or First time
        if (isJoinRoom)
        {
            onJoinRoom?.Invoke(ConstantsHolder.xanaConstants.MuseumID); //Socket ID Update After Reconnect Need To Emit joinRoom again with new Socket Id
        }

        if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
            StartCoroutine(SubmitGuestUserNameWithJson());
    }
    void OnError(CustomError args)
    {
        Debug.LogError("Socket Error: " + args.message);
        isConnected = false;
    }
    void Onresult(CustomError args)
    {
        //Debug.Log("<color=red>" + string.Format("Error: {0}", args.ToString()) + "</color>");
    }
    void OnSocketDisconnect(CustomError args)
    {
        Debug.Log("Socket Disconnected: " + args.message);
        isConnected = false;
    }

    void OnApplicationPause(bool pauseStatus)
    {
        // If the application was paused, mark it for reconnection when it resumes
        if (pauseStatus)
        {
            wasPaused = true;
            Debug.Log("App is paused, preparing to reconnect.");
        }
        else
        {
            if (wasPaused)
            {
                Debug.Log("App has resumed, attempting reconnection.");

                // Attempt reconnection when resuming
                if (!isConnected)
                {
                    StartCoroutine(RetryConnection());
                }

                wasPaused = false;
            }
        }
    }


    void UserJoinRoom(string _worldId)
    {
        worldId = int.Parse(_worldId);
        string userId = ConstantsHolder.userId;
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

        string userId = ConstantsHolder.userId;
        string event_Id = "1";

        // Checking For Event
        if (XanaEventDetails.eventDetails.DataIsInitialized)
        {
            event_Id = XanaEventDetails.eventDetails.id.ToString();
        }
        eventId = int.Parse(event_Id);

        if (!npcId.IsNullOrEmpty())
            userId = "npc-" + npcId;

        if (callBy.Equals(CallBy.NpcToNpc))
            npcSendMsg.Invoke(msg);

        var data = new { userId, eventId = event_Id, worldId = world_Id, msg = msg };
        //Debug.Log("Data:::" + data);
        Manager.Socket.Emit("chatMessage", data);
    }

    void ReceiveMsgs(ChatUserData msg)
    {

        if (string.IsNullOrEmpty(msg.message))
            return;

        if (eventId != msg.event_id)
            return;

        if (ConstantsHolder.xanaConstants.MuseumID != msg.world_id.ToString())
            return;

        string tempUser = msg.name;
        if (PlayerPrefs.GetInt("IsLoggedIn") == 0 && string.IsNullOrEmpty(msg.name))
            tempUser = msg.guestusername;
        else if (string.IsNullOrEmpty(msg.name))
            tempUser = msg.username;

        receivedMsgForTesting = msg;

        if (CheckUserNameIsValid(tempUser))
        {
            //tempUser = msg.socket_id;
            tempUser = "XanaUser-(" + msg.socket_id + ")";//XanaUser-(userId)
        }
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
        DisplayOldChat(oldChatResponse);
    }
    public IEnumerator FetchOldMessages()
    {
        yield return new WaitForSeconds(5f);

        string token = ConstantsGod.AUTH_TOKEN;
        WWWForm form = new WWWForm();

        string api = fetchAllMsgApi + ConstantsHolder.xanaConstants.MuseumID + "/" + eventId + "/" + socketId + "/" + pageNumber + "/" + dataLimit;
        //Debug.Log("<color=red> XanaChat -- API : " + api + "</color>");

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
            Debug.Log("<color=red> XanaChat -- NetWorkissue </color>");

        www.Dispose();
    }
    void DisplayOldChat(string OldChat)
    {
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

        // Create a UnityWebRequest for the POST request
        using (UnityWebRequest request = new UnityWebRequest(setGuestNameApi, "POST"))
        {
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();
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
    public string guestusername;

    public string avatar;
    public string message;
    public string world;
    public string world_entity;
    public int event_id;
    public int world_id;
    public string emotion;
    public string time;
    public string timestamp;
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