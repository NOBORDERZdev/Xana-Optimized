using System;
using UnityEngine;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;


public class HomeSocketHandler : MonoBehaviour
{
    public SocketManager Manager;
    string address;

    public string socketId;
    public static HomeSocketHandler instance;

    public Action<ReceivedFriendPostData> updateFriendPostDelegate;
    public Action<FeedLikeSocket> updateFeedLike;
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
      //  Debug.Log("<color=blue> Post -- Disconnect  </color>");
    }
    void OnError(CustomError args)
    {
       // Debug.Log("<color=blue> Post -- Connection Error  </color>");
    }
    void OnConnected(ConnectResponse resp)
    {
        socketId = resp.sid;
       // Debug.Log("<color=blue> Post -- Connected  </color>");
        EmitUserSocketToApi(); // calling api to update user Socket id for BE to recive messages

        // Bind Events to listen
        Manager.Socket.On<string>("send_xana_text_post_info", ReceivePost);
        //Manager.Socket.On<FeedLikeSocket>("likeTextPost", FeedLikeUpdate);
        Manager.Socket.On<string>("likeTextPost", FeedLikeUpdate);

    }
    void ReceivePost(string msg)
    {
       // Debug.Log("<color=blue> Post -- Received : </color>");
        ReceivedFriendPostData data = JsonConvert.DeserializeObject<ReceivedFriendPostData>(msg);
        updateFriendPostDelegate?.Invoke(data);
    }
    void FeedLikeUpdate(string msg)
    {
      // Debug.Log("<color=blue> Post -- FeedLikeUpdate : </color>" + msg);
        FeedLikeSocket socketInput = JsonConvert.DeserializeObject<FeedLikeSocket>(msg);
       updateFeedLike?.Invoke(socketInput);

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

      //  Debug.Log(" ----> OnConnected --- User ---- >  " + ConstantsHolder.xanaConstants.userId + " --- Socket Id :---- >  " + socketId);

        string FinalUrl = PrepareApiURL("SocketFriendUpdate");
        // Debug.LogError("Prepared URL SendSocketIdOfUserForPost ----> " + FinalUrl);
        WWWForm form = new WWWForm();
        form.AddField("userId", int.Parse(ConstantsHolder.userId));
        form.AddField("socketId", socketId);
        using (UnityWebRequest www = UnityWebRequest.Post(FinalUrl, form))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            www.SendWebRequest();
            while (!www.isDone)
                yield return new WaitForSecondsRealtime(Time.deltaTime);

            // while (!www.isDone)
            //     yield return null;
            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                //Debug.LogError("SendSocketIdOfUserForPost ---->   ERROR  ----->  "+ www.downloadHandler.text);
               // Debug.Log("Error PostSocket ID update  --->  " + www.downloadHandler.text);
            }
            else
            {
               // Debug.Log("SendSocketIdOfUserForPost Success ---->  " + www.downloadHandler.text);
            }
            www.Dispose();
        }
    }

    


    private void OnDisable()
    {
        if (Manager != null)
        {
            Manager.Socket.Off();
            Manager.Close();
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


[Serializable]
public class FeedLikeSocket
{
    public int textPostId;
    public int likeCount;
}
