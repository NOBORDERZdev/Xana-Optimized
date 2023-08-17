using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Collections;

public class SocketHandler : MonoBehaviour
{
    public string address = "https://tcg-api-test.xana.net/";  
    public static SocketHandler Instance;
    public SocketManager Manager;
     public  string mainURL = "https://tcg-api.xana.net/";   
    string url_ChatGetMessages = "/chat/get-messages";
    public SocketResponce msgResponce = new SocketResponce();
    public GroupLeaveResponceRoot leaveGroupResponce = new GroupLeaveResponceRoot();  

    private void Awake()
    {
        /*if (Instance == null)
        {
            Instance = this;
        }*/
    }

    private void OnEnable()
    {
        Instance = this;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            SendDisconnect();
        }    
    }    
    void Start()
    {

        //if(CryptouserData.instance.Testnet)
        //{
        //    Manager = new SocketManager(new Uri(address));

        //}
        //else
        //{
        //    Manager = new SocketManager(new Uri(mainURL));
        // }
        Manager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
        Manager.Socket.On<CustomError>(SocketIOEventTypes.Error, OnError);

      //  ResetListener();
    }  
    private void OnApplicationQuit()
    {
        print("quit");
        SendDisconnect();
    }  
    public void ResetListener()
    {
        Debug.Log("Listen");

        //Manager.Socket.On<string>("MessageReceived", MessageReceivedResponce);
        ////Manager.Socket.On<SocketResponce>("MessageReceived", MessageReceivedResponce);
        //Manager.Socket.On<string>("GroupCreated", GroupCreatedResponce);

        //Manager.Socket.On<string>("GroupLeaved", GroupLeaveUserResponce);
    }
     public void GroupLeaveUserResponce(string s)
    {
       Debug.Log("Group Leave responce:" + s);
        leaveGroupResponce = JsonConvert.DeserializeObject<GroupLeaveResponceRoot>(s);
 
    }
     public void Connect()
    {
        Debug.Log("hi from server");
    }  
    void OnConnected(ConnectResponse resp)
    {
       Debug.Log("Connect to Server");      
      // SendDisconnect();   
        //Manager.Socket.Emit("hi", "hiiii");
    }   
    private sendDeviceIdAndUserID DeviceIDObj;  
    public void SendDisconnect()
    {
        print("SendDisconnect");
        //socket.emit("disconnectWallet", { userId: 5439, deviceId: "12345" });
        DeviceIDObj = new sendDeviceIdAndUserID();   
        DeviceIDObj =  DeviceIDObj.VerifySignedClassFtn(PlayerPrefs.GetString("LoginToken"), PlayerPrefs.GetString("myDeviceToken"));
        //  DeviceIDObj =  DeviceIDObj.VerifySignedClassFtn("17365", "4dc8c73b0de1d673b545f68b89337fbc35cde24d");   
        var jsonObj2 = JsonUtility.ToJson(DeviceIDObj);
        Manager.Socket.Emit("disconnectWallet", jsonObj2);
           print(jsonObj2);    
    }                       
       
    [System.Serializable]
    public class sendDeviceIdAndUserID
    {
        public string authorization;
        public string deviceId;
       
        public sendDeviceIdAndUserID VerifySignedClassFtn(string _Auth, string _deviceID)
        {
            sendDeviceIdAndUserID obj1 = new sendDeviceIdAndUserID();
            obj1.authorization = _Auth;   
             obj1.deviceId = _deviceID;
            return obj1;  
        }  
    }


    void OnError(CustomError args)
    {
       Debug.Log(string.Format("Error: {0}", args.ToString()));
    }
    void Onresult(CustomError args)
    {
       Debug.Log(string.Format("Error: {0}", args.ToString()));
    }
  
    public void RequestChatGetMessagesSocket(int message_pageNumber, int message_pageSize, int message_receiverId, int message_receivedGroupId)
    {
      //  APIManager.Instance.r_isCreateMessage = true;
      //   StartCoroutine(IERequestChatGetMessagesSocket(message_pageNumber, message_pageSize, message_receiverId, message_receivedGroupId));
    }       
    
    /*
    public IEnumerator IERequestChatGetMessagesSocket(int message_pageNumber, int message_pageSize, int message_receiverId, int message_receivedGroupId)
    {
        WWWForm form = new WWWForm();
        form.AddField("pageNumber", message_pageNumber);
        form.AddField("pageSize", message_pageSize);
        if (message_receivedGroupId != 0)
        {
            form.AddField("receivedGroupId", message_receivedGroupId);
        }
        else if (message_receiverId != 0)
        {
            form.AddField("receiverId", message_receiverId);
        }

        using (UnityWebRequest www = UnityWebRequest.Post((mainURL + url_ChatGetMessages), form))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                if (message_receivedGroupId != 0)
                {
                    // MessageController.Instance.ChatScreen.SetActive(true);
                    // MessageController.Instance.MessageListScreen.SetActive(false);
                    APIManager.Instance.r_isCreateMessage = false;
                }
            }
            else
            {
                // Debug.Log("Form upload complete!");
                string data = www.downloadHandler.text;
               Debug.Log("socket Message Chat: " + data);
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                APIManager.Instance.allChatMessagesRoot = JsonConvert.DeserializeObject<ChatGetMessagesRoot>(data, settings);
                //allChatMessagesRoot.data.rows.Reverse();
                APIController.Instance.GetAllChat(message_pageNumber);
                //GetAllChat(message_pageNumber);//rik use this method from APIManager.
            }
        }
    }
    */
   
    /*
    public void GetAllChat(int pageNumber)
    {
       Debug.Log("SoketHandler GetAllChat........");
        // allChatMessageId.Clear();
        for (int i = 0; i < APIManager.Instance.allChatMessagesRoot.data.rows.Count; i++)
        {
            if (!APIController.Instance.allChatMessageId.Contains(APIManager.Instance.allChatMessagesRoot.data.rows[i].id))
            {
                APIController.Instance.lastMsgTime = APIManager.Instance.allChatMessagesRoot.data.rows[i].createdAt;

                if (APIManager.Instance.r_isCreateMessage)//rik.......
                {
                    APIController.Instance.SetChetDay(APIManager.Instance.allChatMessagesRoot.data.rows[i].updatedAt, pageNumber);
                }                

               Debug.Log("i : " + i + "+PageNum:" + pageNumber + ":responce:" + APIManager.Instance.allChatMessagesRoot.data.rows[i]);
                if (APIManager.Instance.allChatMessagesRoot.data.rows[i].senderId == APIManager.Instance.userId)
                {
                    if (APIManager.Instance.allChatMessagesRoot.data.rows[i].message.attachments.Count > 0)
                    {
                        //SetChetDay(APIManager.Instance.allChatMessagesRoot.data.rows[i].updatedAt, pageNumber);
                        GameObject ChatPhotoObject = Instantiate(APIController.Instance.chatPhotoPrefabUser, MessageController.Instance.chatPrefabParent);
                        ChatPhotoObject.GetComponent<ChatDataScript>().MessageRow = APIManager.Instance.allChatMessagesRoot.data.rows[i];
                        ChatPhotoObject.GetComponent<ChatDataScript>().LoadFeed();
                       Debug.Log("isCreateMessage" + APIManager.Instance.r_isCreateMessage);
                        if (pageNumber == 1 && APIManager.Instance.r_isCreateMessage)
                        {
                            ChatPhotoObject.transform.SetAsLastSibling();
                        }
                        else
                        {
                            ChatPhotoObject.transform.SetAsFirstSibling();
                        }
                    }
                    else if (!string.IsNullOrEmpty(APIManager.Instance.allChatMessagesRoot.data.rows[i].message.msg))
                    {
                        //SetChetDay(APIManager.Instance.allChatMessagesRoot.data.rows[i].updatedAt, pageNumber);
                        GameObject ChatObject = Instantiate(APIController.Instance.chatPrefabUser, MessageController.Instance.chatPrefabParent);
                        ChatObject.GetComponent<ChatDataScript>().MessageRow = APIManager.Instance.allChatMessagesRoot.data.rows[i];
                        ChatObject.GetComponent<ChatDataScript>().LoadFeed();
                        if (pageNumber == 1 && APIManager.Instance.r_isCreateMessage)
                        {
                            ChatObject.transform.SetAsLastSibling();
                        }
                        else
                        {
                            ChatObject.transform.SetAsFirstSibling();
                        }
                    }
                }
                else
                {
                    if (APIManager.Instance.allChatMessagesRoot.data.rows[i].message.attachments.Count > 0)
                    {
                        //SetChetDay(APIManager.Instance.allChatMessagesRoot.data.rows[i].updatedAt, pageNumber);
                        GameObject ChatPhotoObject = Instantiate(APIController.Instance.chatPhotoPrefabOther, MessageController.Instance.chatPrefabParent);
                        ChatPhotoObject.GetComponent<ChatDataScript>().MessageRow = APIManager.Instance.allChatMessagesRoot.data.rows[i];
                        ChatPhotoObject.GetComponent<ChatDataScript>().LoadFeed();
                        if (pageNumber == 1 && APIManager.Instance.r_isCreateMessage)
                        {
                            ChatPhotoObject.transform.SetAsLastSibling();
                        }
                        else
                        {
                            ChatPhotoObject.transform.SetAsFirstSibling();
                        }
                    }
                    else if (!string.IsNullOrEmpty(APIManager.Instance.allChatMessagesRoot.data.rows[i].message.msg))
                    {
                        //SetChetDay(APIManager.Instance.allChatMessagesRoot.data.rows[i].updatedAt, pageNumber);
                        GameObject ChatObject = Instantiate(APIController.Instance.chatPrefabOther, MessageController.Instance.chatPrefabParent);
                        ChatObject.GetComponent<ChatDataScript>().MessageRow = APIManager.Instance.allChatMessagesRoot.data.rows[i];
                        ChatObject.GetComponent<ChatDataScript>().LoadFeed();
                        if (pageNumber == 1 && APIManager.Instance.r_isCreateMessage)
                        {
                            ChatObject.transform.SetAsLastSibling();
                        }
                        else
                        {
                            ChatObject.transform.SetAsFirstSibling();
                        }
                    }
                }

                if (!APIManager.Instance.r_isCreateMessage)//rik.......
                {
                    APIController.Instance.SetChetDay(APIManager.Instance.allChatMessagesRoot.data.rows[i].updatedAt, pageNumber);
                }

                APIController.Instance.allChatMessageId.Add(APIManager.Instance.allChatMessagesRoot.data.rows[i].id);
            }
        }

        MessageController.Instance.chatPrefabParent.gameObject.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        //MessageController.Instance.ChatScreen.SetActive(true);
        // MessageController.Instance.MessageListScreen.SetActive(false);
        APIManager.Instance.r_isCreateMessage = false;
        ChatScreenDataScript.Instance.allChatGetConversationDatum = MessageController.Instance.allChatGetConversationDatum;
        //Invoke("SetChatScreen", 0.08f);
        if (setChatScreenCo != null)
        {
            StopCoroutine(setChatScreenCo);
        }
        setChatScreenCo = StartCoroutine(SetChatScreen());
    }
    */
    Coroutine setChatScreenCo;
    /*
    public IEnumerator SetChatScreen()
    {
        yield return new WaitForSeconds(0.08f);
        MessageController.Instance.chatPrefabParent.gameObject.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        if (APIManager.Instance.allChatMessagesRoot.data.rows.Count > 0)
        {
           Debug.Log("here");
            MessageController.Instance.isChatDataLoaded = false;
        }
    }
    */
}


class CustomError : Error
{
    public ErrorData data;

    public override string ToString()
    {
        return $"[CustomError {message}, {data?.code}, {data?.content}]";
    }
}

class ErrorData
{
    public int code;
    public string content;
}

[System.Serializable]
public class SocketResponce
{
    public List<string> userList;
}

[System.Serializable]
public class GroupLeaveResponceRoot
{
    public List<int> userList = new List<int>();
    public int groupId; 
}