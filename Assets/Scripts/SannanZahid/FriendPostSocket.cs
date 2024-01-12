using System;
using UnityEngine;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using static UserPostFeature;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;
using SimpleJSON;
using static RestAPI;

public class FriendPostSocket : MonoBehaviour
{
    public SocketManager Manager;
    string address;

    public string socketId;
    public static FriendPostSocket instance;

    public Action<ReceivedFriendPostData> updateFriendPostDelegate;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Manager = new SocketManager(new Uri(PrepareApiURL("Socket")));

        Manager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
        Manager.Socket.On<CustomError>(SocketIOEventTypes.Error, OnError);
        Manager.Socket.On<CustomError>(SocketIOEventTypes.Disconnect, OnSocketDisconnect);
    }
    void OnSocketDisconnect(CustomError args)
    {
        Debug.Log("<color=blue> Post -- Disconnect  </color>");
    }
    void OnError(CustomError args)
    {
        Debug.LogError("POST -- Connection Error");
    }
    void OnConnected(ConnectResponse resp)
    {
        socketId = resp.sid;
        Debug.LogError("POST -- Connected");

        EmitUserSocketToApi(); // calling api to update user Socket id for BE to recive messages

        // Bind Events to listen
        Manager.Socket.On<string>("send_xana_text_post_info", ReceivePost);
    }
    void ReceivePost(string msg)
    {
        Debug.Log("<color=blue> Post -- Received : </color>");
        ReceivedFriendPostData data = JsonConvert.DeserializeObject<ReceivedFriendPostData>(msg);
        updateFriendPostDelegate?.Invoke(data);

        // Use this data to show post on screen

        Debug.Log("<color=blue> Post -- Msg : </color>" + data.msg);
        Debug.Log("<color=blue> Post -- ID : </color>" + data.creatorId);
        Debug.Log("<color=blue> Post -- Post : </color>" + data.text_post);
        Debug.Log("<color=blue> Post -- Mood : </color>" + data.text_mood);
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
                    address = ConstantsGod.API_BASEURL;
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

        while (PlayerPrefs.GetString("UserNameAndPassword") == "")
            yield return new WaitForSeconds(0.5f);

        Debug.LogError(" ----> OnConnected --- User ---- >  " + XanaConstants.xanaConstants.userId + " --- Socket Id :---- >  " + socketId);

        string FinalUrl = PrepareApiURL("SocketFriendUpdate");
        // Debug.LogError("Prepared URL SendSocketIdOfUserForPost ----> " + FinalUrl);
        WWWForm form = new WWWForm();
        form.AddField("userId", int.Parse(XanaConstants.xanaConstants.userId));
        form.AddField("socketId", socketId);
        using (UnityWebRequest www = UnityWebRequest.Post(FinalUrl, form))
        {
            Debug.LogError("Token ----> " + ConstantsGod.AUTH_TOKEN);
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            www.SendWebRequest();
            while (!www.isDone)
                yield return new WaitForSecondsRealtime(Time.deltaTime);

            // while (!www.isDone)
            //     yield return null;
            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                //Debug.LogError("SendSocketIdOfUserForPost ---->   ERROR  ----->  "+ www.downloadHandler.text);
                Debug.LogError("Error PostSocket ID update  --->  " + www.downloadHandler.text);
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


[System.Serializable]
public class ReceivedFriendPostData
{
    public string msg;
    public string creatorId;

    public string text_post;
    public string text_mood;
}
