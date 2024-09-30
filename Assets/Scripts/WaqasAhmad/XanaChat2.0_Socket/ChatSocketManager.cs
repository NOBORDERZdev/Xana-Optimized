using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using SimpleJSON;
using UnityEngine.UI;
using Unity.Mathematics;


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


    //string fetchApi = "api/v1/fetch-world-chat-byEventId/";
    string fetchApi = "api/v3/fetch-world-chat-byEventId/";

    string blockMsgApi = "api/v1/block-message/"; // --- /api/v1/block-message/:messageId/:loginUserId
    string blockUserApi = "api/v1/block-user/"; //--- /api/v1/block-user/:blockUserId/:loginUserId


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
    public GameObject MsgPrefab;
    public Transform MsgParentObj;

    private List<ChatMsgDataHolder> allMsgData = new List<ChatMsgDataHolder>();
    internal ScrollRect MsgParentObjScrollRect;


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
        BuilderEventManager.AfterPlayerInstantiated += LoadChatAfterPlayerInstantiate;

        if (MsgParentObj != null)
            MsgParentObjScrollRect = MsgParentObj.parent.GetComponent<ScrollRect>();
    }
    private void OnDisable()
    {
        onJoinRoom -= UserJoinRoom;
        onSendMsg -= SendMsg;
        callApi -= CallApiForMessages;
        BuilderEventManager.AfterPlayerInstantiated -= LoadChatAfterPlayerInstantiate;
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
        //Manager.Socket.On<ChatUserData>("message", ReceiveMsgs);
        Manager.Socket.On<ChatUserData>("messagev2", ReceiveMsgs);

        // StartCoroutine(FetchOldMessages());
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
        //XanaChatSystem.instance.DisplayMsg_FromSocket(tempUser, msg.message);
        AddNewMsg(tempUser, msg.message, msg.message_id.ToString(), msg.userId, 0);
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

    void LoadChatAfterPlayerInstantiate()
    {
        ClearAllMessages();
        StartCoroutine(FetchOldMessages());
    }

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

        string api = fetchAllMsgApi + ConstantsHolder.xanaConstants.MuseumID + "/" + eventId + "/" + socketId + "/" + pageNumber + "/" + dataLimit + "/" + ConstantsHolder.userId;
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
            Debug.Log("<color=red> XanaChat -- NetWorkissue </color>");

        www.Dispose();
    }
    void DisplayOldChat(string OldChat)
    {
        RootData rootData = JsonUtility.FromJson<RootData>(OldChat);
        if (rootData != null && rootData.count > 0)
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

                AddNewMsg(tempUser, rootData.data[i].message, rootData.data[i].id, rootData.data[i].user_id, rootData.data[i].block_message);
            }
        }
    }

    public void AddNewMsg(string userName, string msg, string msgId, string userId, int blockMessage)
    {
        Debug.LogFormat("AddNewMsg {0}", userName);
        GameObject _newMsg = Instantiate(MsgPrefab, MsgParentObj);
        ChatMsgDataHolder _dataHolder = _newMsg.GetComponent<ChatMsgDataHolder>();
        RectTransform rectTransform = _dataHolder.MsgText.GetComponent<RectTransform>();
#if UNITY_IOS
        rectTransform.sizeDelta = new Vector2(204.6f, rectTransform.sizeDelta.y);
#elif UNITY_ANDROID
        rectTransform.sizeDelta = new Vector2(250.6f, rectTransform.sizeDelta.y);
#endif
        _dataHolder.SetRequireData(msg, msgId, userId, blockMessage);

        if (!ConstantsHolder.xanaConstants.chatFlagBtnStatus || userId.Equals(ConstantsHolder.userId))
        {
            // That My msg, and i cannot flag or block it
            _dataHolder.DotedBtn.SetActive(false);
        }
        MsgParentObj.GetComponent<VerticalLayoutGroup>().enabled = false;
        Invoke("DelayAdded", 0.05f);

        //StartCoroutine(nameof(Delay));
        XanaChatSystem.instance.DisplayMsg_FromSocket(userName, msg, _dataHolder.MsgText);

        // Add to List
        if (allMsgData == null)
            allMsgData = new List<ChatMsgDataHolder>();
        allMsgData.Add(_dataHolder);
        Refresh();
    }

    void Refresh()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(MsgParentObj.GetComponent<RectTransform>());    
    }

  

    void DelayAdded()
    {
        MsgParentObj.GetComponent<VerticalLayoutGroup>().enabled = true;
        if(MsgParentObjScrollRect)
            MsgParentObjScrollRect.verticalNormalizedPosition = 1f;
    }

    // Submit Guest User Name
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

    // Flag Message
    public void FlagMessages(string msgID, Action<bool> callback)
    {
        if(ConstantsHolder.xanaConstants.chatFlagBtnStatus) // if flag functionality is disabled than no need to call API
            StartCoroutine(FlagMessagesRoutine(msgID, callback));
    }
    IEnumerator FlagMessagesRoutine(string msgID, Action<bool> callback)
    {
        string token = ConstantsGod.AUTH_TOKEN;
        string api = address + blockMsgApi + msgID + "/" + ConstantsHolder.userId;

        UnityWebRequest www;
        www = UnityWebRequest.Post(api, "");

        www.SetRequestHeader("Authorization", token);
        www.SendWebRequest();

        while (!www.isDone)
        {
            yield return null;
        }


        if (!www.isHttpError && !www.isNetworkError)
        {
            Debug.Log("<color=green> XanaChat -- FlagMsg : " + www.downloadHandler.text + "</color>");
            JObject jsonObject = JObject.Parse(www.downloadHandler.text);
            string dataValue = "";

            if (jsonObject.ContainsKey("data"))
                dataValue = jsonObject["data"].ToString();


            if (dataValue.Equals("message blocked"))
                callback(true);
            else
            {
                callback(false);
            }
        }
        else
        {
            Debug.Log("<color=red> XanaChat -- FlagMsg -- NetWorkissue </color>");
            callback(false);
        }

        www.Dispose();
    }

    // Block Message
    public void BlockUser(string userId, Action<bool> callback)
    {
        StartCoroutine(BlockUserRoutine(userId, callback));
    }
    IEnumerator BlockUserRoutine(string blockUserId, Action<bool> callback)
    {
        string token = ConstantsGod.AUTH_TOKEN;
        string api = address + blockUserApi + blockUserId + "/" + ConstantsHolder.userId;

        UnityWebRequest www;
        www = UnityWebRequest.Post(api, "");

        www.SetRequestHeader("Authorization", token);
        www.SendWebRequest();

        while (!www.isDone)
        {
            yield return null;
        }


        if (!www.isHttpError && !www.isNetworkError)
        {
            //oldMsgRec.text = www.downloadHandler.text;
            Debug.Log("<color=green> XanaChat -- BlockUser : " + www.downloadHandler.text + "</color>");
            callback(true);
        }
        else
        {
            Debug.Log("<color=red> XanaChat -- BlockUser -- NetWorkissue </color>");
            callback(false);
        }

        www.Dispose();
    }

    public void DisableAllBtn()
    {
        foreach (var item in allMsgData)
        {
            item.BtnForcedStatus(false);
        }
    }

    public void ClearAllMessages()
    {
        if (allMsgData.Count > 0)
            foreach (var item in allMsgData)
            {
                Destroy(item.gameObject);
            }
        allMsgData.Clear();
    }
}


[System.Serializable]
public class ChatUserData
{
    public string userId;
    public string id; // MessageID
    public int message_id;
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
    public string id; // messageID
    public string user_id;
    public int block_message;
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