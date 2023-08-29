using System;
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


public class XanaChatSocket_Waqas : MonoBehaviour
{
    public string address = "https://chat-testing.xana.net/";
    public SocketManager Manager;

    public TMP_Text connectedText;
    public TMP_Text joinedText;
    public TMP_Text receivedMsg;
    public TMP_Text oldMsgRec;
    // /api/v1/fetch-world-chat-byId/worldId/:userId/:page/:limit
    public string fetchAllMsgApi = "https://chat-testing.xana.net/api/v1/fetch-world-chat-byId/";

    public int worldId, pageNumber, dataLimit;

    public string testMsgString = "";
    public string socketId;
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

    public static Action<int> onJoinRoom;
    public static Action<int, int, string> onSendMsg;


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
    }
    private void OnDisable()
    {
        onJoinRoom -= UserJoinRoom;
        onSendMsg -= SendMsg;
    }


    #region Socket Calls Handling

    void OnConnected(ConnectResponse resp)
    {
        socketId = resp.sid;
        connectedText.text = "XanaChat -- SocketConnected : " + resp.sid;
        Manager.Socket.On<string>("message", ReceiveMsgs);

        //Debug.Log("<color=green> XanaChat -- Socket ConnectCallBack" + "</color>");
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


    void ReceiveMsgs(string msg)
    {
        Debug.Log("<color=gray> MsgReceive : " + msg + "</color>");
        receivedMsg.text = msg;
    }
    void UserJoinRoom(int worldId)
    {
        //Debug.Log("<color=blue> XanaChat -- JoinRoom : " + worldId + "</color>");
        //Manager.Socket.Emit("joinRoom", userName, worldId);

        joinedText.text = "XanaChat -- JoinRoom : " + worldId;
        string roomId = worldId.ToString();
        var data = new { username = socketId, room = roomId };
        Manager.Socket.Emit("joinRoom", data);
    }
    void SendMsg(int event_Id, int world_Id, string msg)
    {
        if (string.IsNullOrEmpty(msg))
        {
            Debug.Log("<color=blue> XanaChat -- NoMsgTyped </color>");
            return;
        }


        Debug.Log("<color=blue> XanaChat -- MsgSend : " + event_Id + " - " + world_Id + " - " + msg + "</color>");
        //Manager.Socket.Emit("chatMessage", userName, eventId, worldId, msg);

        var data = new { userId = socketId, eventId = event_Id, worldId = world_Id, msg = msg };
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

        string api = fetchAllMsgApi + worldId + "/" + socketId + "/" + pageNumber + "/" + dataLimit;
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
            oldMsgRec.text = www.downloadHandler.text;
            Debug.Log("<color=green> XanaChat -- OldMessages : " + www.downloadHandler.text + "</color>");
        }
        else
            Debug.Log("<color=red> XanaChat -- NetWorkissue </color>");

        www.Dispose();
    }
    #endregion


    // Testing Function
    public void CallJoinRoom()
    {
        UserJoinRoom(worldId);
    }
    public void TestSendMsg()
    {
        SendMsg(1, 1011, testMsgString);
    }


}