using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CommonAPIManager : MonoBehaviour
{
    public static CommonAPIManager Instance;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("SNS_APIManager Start UserToken:" + ConstantsGod.AUTH_TOKEN + "    :userID:" + PlayerPrefs.GetString("UserName"));
        SetUpBottomUnReadCount(0);//default message footer message unread count set false.......
        ConnetSocketManagerAndListener();
    }

    #region Common Socket Handler Event.......
    [Header("Common Socket Handler")]
    public SocketManager Manager;
    public string address;

    public void ConnetSocketManagerAndListener()
    {
        address = ConstantsGod.API_BASEURL;
        //Debug.Log("ConnetSocketManagerAndListener Address:" + address);
        if (!address.EndsWith("/"))
        {
            address = address + "/";
        }
        //Debug.Log("ConnetSocketManagerAndListener Address:" + address);
        Manager = new SocketManager(new Uri((address)));
        Manager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
        Manager.Socket.On<CustomError>(SocketIOEventTypes.Error, OnError);

        ResetListener();
    }

    public void Connect()
    {
        Debug.Log("hi from server");
    }

    void OnConnected(ConnectResponse resp)
    {
        //Debug.Log("Connect");
        //Manager.Socket.Emit("hi", "hiiii");
    }

    void OnError(CustomError args)
    {
        //Debug.Log(string.Format("Error: {0}", args.ToString()));
    }

    void Onresult(CustomError args)
    {
        //Debug.Log(string.Format("Error: {0}", args.ToString()));
    }

    public void ResetListener()
    {
        print("Listen");
        //Manager.Socket.On<string>("FeedComment", FeedCommentResponse);
        //Manager.Socket.On<string>("MessageReceived", MessageReceivedResponse);
    }

    //public void MessageReceivedResponse(string s)
    //{
    //   Debug.Log("Common Socket Handler MessageReceivedResponce.......");
    //    if (SNS_MessageController.Instance != null)
    //    {
    //        if (SNS_MessageController.Instance.ChatScreen.activeInHierarchy)
    //        {
    //            return;
    //        }

    //        if (!SNS_MessageController.Instance.gameObject.activeInHierarchy)//this condition is used to once message screen open then go to another screen then back to message screen refresh conversation list.......
    //        {
    //            SNS_MessageController.Instance.isNeedToRefreshConversationAPI = true;
    //        }
    //    }
    //   Debug.Log("Common Socket Handler MessageReceivedResponce111111111.......");
    //    RequestGetAllChatUnReadMessagesCount();//For Get All Chat UnRead Message Count.......
    //}

    #endregion

    //-----------------------------------------------------------------------------------------------------------------------------------------------

    #region Common APi Handler.......

    #region SNS Module APis.......
    [Space]
    [Header("Comman APi Handler")]
    HomeFooterHandler[] bottomTabManagers;
    //this api is used to get all UnRead Messages Count.......
    public void RequestGetAllChatUnReadMessagesCount()
    {
        //Debug.Log("RequestGetAllChatUnReadMessagesCount0000000.......");
        if (!string.IsNullOrEmpty(ConstantsGod.AUTH_TOKEN))
        {
            //Debug.Log("RequestGetAllChatUnReadMessagesCount1111111.......");
            if (IERequestGetAllChatUnReadMessagesCountCo != null)
            {
                StopCoroutine(IERequestGetAllChatUnReadMessagesCountCo);
            }
            IERequestGetAllChatUnReadMessagesCountCo = StartCoroutine(IERequestGetAllChatUnReadMessagesCount());
        }
    }
    Coroutine IERequestGetAllChatUnReadMessagesCountCo;
    public IEnumerator IERequestGetAllChatUnReadMessagesCount()
    {
        using (UnityWebRequest www = UnityWebRequest.Get((ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetAllChatUnReadMessagesCount)))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);

            www.SendWebRequest();

            while(!www.isDone)
            {
                yield return null;
            }

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;   
                MessageUnreadCountRoot myDeserializedClass = JsonConvert.DeserializeObject<MessageUnreadCountRoot>(data);
                SetUpBottomUnReadCount(myDeserializedClass.data);
            }
        }
    }

    //This method is used to setup footer message unread count setup.......
    public void SetUpBottomUnReadCount(int count)
    {
        bottomTabManagers = Resources.FindObjectsOfTypeAll<HomeFooterHandler>();
        Debug.LogError("finding bottom tab manager  :- ");
        for (int i = 0; i < bottomTabManagers.Length; i++)
        {
            bottomTabManagers[i].MessageUnReadCountSetUp(count);
        }
    }
    #endregion

    #endregion
}

public class MessageUnreadCountRoot
{
    public bool success;
    public int data;
    public string msg;
}