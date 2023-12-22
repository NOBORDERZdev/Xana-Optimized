using System;
using UnityEngine;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using static UserPostFeature;
using UnityEngine.Networking;
using System.Collections;

public class FriendPostSocket : MonoBehaviour
{
    string socketTestnet = "https://api-test.xana.net/";
    string socketMainnet = "https://chat-prod.xana.net/";

    /// <summary>
    /// https://api-test.xana.net/users/update-user-socket
    /// </summary>

    public SocketManager Manager;

    string address;
    string fetchAllMsgApi;

    public string socketId;
    public string oldChatResponse;
    public ChatUserData receivedMsgForTesting;
    int eventId = 1;
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

    void Start()
    {
        Debug.LogError("FInal Address ----> " + address);
        Manager = new SocketManager(new Uri(PrepareApiURL("Socket")));
        Manager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
        Manager.Socket.On<CustomError>(SocketIOEventTypes.Error, OnError);
        Manager.Socket.On<CustomError>(SocketIOEventTypes.Disconnect, OnSocketDisconnect);

        Manager.Socket.On<ChatUserData>("send_xana_text_post_info ", ReceivePost);
    }
    void OnSocketDisconnect(CustomError args)
    {
        Debug.LogError("FInal Address ----> OnSocketDisconnect");
        Debug.LogError("<color=red>" + string.Format("Error: {0}", args.ToString()) + "</color>");
    }
    void OnError(CustomError args)
    {
        Debug.LogError("FInal Address ----> OnError");
        Debug.LogError("<color=red>" + string.Format("Error: {0}", args.ToString()) + "</color>");
    }
    void OnConnected(ConnectResponse resp)
    {
        socketId = resp.sid;
        Debug.LogError("FInal Address ----> OnConnected");
        Debug.LogError("<color=blue> XanaChat -- SocketConnected : " + resp.sid + "</color>");
        EmitUserSocketToApi();

    }
    void ReceivePost(ChatUserData msg)
    {
        Debug.LogError("<color=blue> XanaChat -- MsgReceive : " + msg.username + " : " + msg.message + "</color>");
    }
    void SendPost(ChatUserData msg)
    {
        Debug.LogError("<color=blue> XanaChat -- MsgReceive : " + msg.username + " : " + msg.message + "</color>");
    }
    void EmitUserSocketToApi()
    {
        StartCoroutine(SendSocketIdOfUserForPost());
    }
    string PrepareApiURL(string urlType)
    {
        switch (urlType)
        {
            case "Socket":
                {
                    if (APIBaseUrlChange.instance.IsXanaLive)
                        address = socketMainnet;
                    else
                        address = socketTestnet;

                    if (!address.EndsWith("/"))
                        address = address + "/";

                    return address;
                }
            case "SocketFriendUpdate":
                return ConstantsGod.API_BASEURL + "/users/update-user-socket";
       

            default:
                return "";
        }
    }
    IEnumerator SendSocketIdOfUserForPost()
    {
        while (ConstantsGod.AUTH_TOKEN == "AUTH_TOKEN")
            yield return new WaitForSeconds(0.5f);

        // WWWForm form = new WWWForm();
        // form.AddField("user_id", XanaConstants.xanaConstants.userId);
        Debug.LogError("----- AUTH_TOKEN GIVEN ----> ");

        while (PlayerPrefs.GetString("UserNameAndPassword") == "")
            yield return new WaitForSeconds(0.5f);

        Debug.LogError(" ----> OnConnected --- User ---- >  " + XanaConstants.xanaConstants.userId + " --- Socket ---- >  " + socketId);

        string FinalUrl = PrepareApiURL("SocketFriendUpdate");
        Debug.LogError("Prepared URL SendSocketIdOfUserForPost ----> " + FinalUrl);
        WWWForm form = new WWWForm();
        form.AddField("userId", int.Parse(XanaConstants.xanaConstants.userId));
        form.AddField("socketId", socketId);
        using (UnityWebRequest www = UnityWebRequest.Post(FinalUrl, form))
        {
            // Debug.LogError("Token ----> " + ConstantsGod.AUTH_TOKEN);
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            www.SendWebRequest();
            while (!www.isDone)
                yield return new WaitForSecondsRealtime(Time.deltaTime);

            // while (!www.isDone)
            //     yield return null;
            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                  Debug.LogError("SendSocketIdOfUserForPost ---->   ERROR  ----->  "+ www.downloadHandler.text);
                //  Debug.LogError("Error Post --->  "+www.downloadHandler.text);
            }
            else
            {
                 Debug.LogError("SendSocketIdOfUserForPost Success ---->  " + www.downloadHandler.text);
            }
            www.Dispose();
        }
    }
}
[System.Serializable]
public class PostFriendData
{
    public string socket_id;
    public string username;
    public string avatar;
    public string message;
    public string world;
    public int event_id;
    public int world_id;
    public long time;
}
