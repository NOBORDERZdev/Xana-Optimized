using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using System;
using System.Threading.Tasks;

public class StreamingSockets : MonoBehaviour
{
    public static StreamingSockets Instance;
    //public string [] WorldNames ;
    public SocketManager Manager;
    string socketAddress="";
    bool isFristTime= true;

    public static Action<string[]> onConnect;
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            //ConnectSockets();
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnEnable()
    {
        isInWorld =false;
       // onConnect += SetSteramingEvent;
    }

    public void ConnectSockets()
    {
        if (isFristTime)
        {
            isFristTime= false;
            if (APIBaseUrlChange.instance.IsXanaLive)
            {
               // socketAddress
            }
            else
            {
                socketAddress = "https://api-test.xana.net";
            }
            if (!socketAddress.EndsWith("/"))
            {
                socketAddress = socketAddress + "/";
            }
            Manager = new SocketManager(new Uri((socketAddress)));
            Manager.Socket.On<CustomError>(SocketIOEventTypes.Error, OnError);
            Manager.Socket.On<ConnectResponse>(SocketIOEventTypes.Disconnect, OnSocketDisconnect);
            Manager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnSocketConnect);
            // Custom Method
            Manager.Socket.On<string>("send_world_event_info", RecvieStreamingEvent);
        }
    }


   void OnError(CustomError args){
        Debug.Log("<color=red>  STREAMING SOCKETS : " + string.Format("Error: {0}", args.ToString()) + "</color>");
   }

    void OnSocketDisconnect(ConnectResponse args)
    {
        Debug.Log("<color=red>  STREAMING SOCKETS : " + string.Format("Disconnect: {0}", args.ToString()) + "</color>");
    }

    void OnSocketConnect(ConnectResponse args) {
        Debug.Log("<color=red> STREAMING SOCKETS : " + string.Format("Connect: {0}", args.ToString()) + "</color>");
        //onConnect.Invoke(WorldNames);
    }

    public async void RecvieStreamingEvent(string EventDetail){
      //  print("~~~~~~~~~~~~~~ "+EventDetail);
        EventDataDetails eventDetails = JsonUtility.FromJson<EventDataDetails>(EventDetail);
        Debug.Log("<color=green> EventDetail --  : " + eventDetails.data.id + " : " + eventDetails.data.environmentName + "</color>");

        if (EventDetail != null)
        {
            StreamingSockets.Instance.isEventTriggered= true;
            print("Recive STREAMING SOCKETS " + EventDetail);
            //if (/*!XanaConstants.xanaConstants.isBackFromWorld*/ !isInEvent)
            //{
            //    isInEvent =true;
            //    DynamicEventManager.Instance.SetSceneData();
            //}
            if (isInWorld )
            {
                 LoadingHandler.Instance.streamingLoading.UpdateLoadingText(false);
                //LoadingHandler.Instance.StartCoroutine (LoadingHandler.Instance.streamingLoading.ResetLoadingBar());
                LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
                XanaConstants.xanaConstants.JjWorldSceneChange = true;
                StreamingSockets.Instance.isInWorld=false;
                LoadFromFile.instance._uiReferences.LoadMain(false);
                return;
                //await Task.Delay(12000);
            }
            //else
            //{
            //    StreamingSockets.Instance.isEventTriggered=true;
            //}
            XanaEventDetails.eventDetails = eventDetails.data;
            XanaEventDetails.eventDetails.DataIsInitialized = true;
            XanaConstants.xanaConstants.newStreamEntery=true;
            DynamicEventManager.Instance.SetSceneData();
        }
    }

    //public void RejoinOnEventFromWorld(XanaEventDetails eventDetails){ 
    //    if (eventDetails != null)
    //    {
    //        print("Rejoining from the world  " + eventDetails);
    //       // XanaEventDetails.eventDetails = eventDetails.data;
    //        //XanaEventDetails.eventDetails.DataIsInitialized = true;
    //        if (/*!XanaConstants.xanaConstants.isBackFromWorld*/ true)
    //        {
    //            isInEvent =true;
    //            DynamicEventManager.Instance.SetSceneData();
    //        }
           
    //    }
    //}

     public bool isEventTriggered= false;
     public bool isInWorld= false;
    public void ReSetStreamingEvent(){ 
        XanaEventDetails.eventDetails =null;
         XanaEventDetails.eventDetails.DataIsInitialized= false;
    }

    //void SetSteramingEvent( params string[] worldNames){
    //    if (worldNames.Length>0)
    //    {
    //        for (int i = 0; i < worldNames.Length; i++)
    //        {
    //            Debug.Log("<color=red> STREAMING WORLD NAME : " + worldNames[i].ToString() + "</color>");
    //        }
    //       // List<string> name = new List<string>();
    //       // name.Add("list 1");
    //       // name.Add("list 2");
    //       ////string[] names = new string[]{"Xana Festival"};
    //       //Manager.Socket.Emit("register_world_ai_server",/*"Xana Festival" */ worldNames/*name*/);
    //          Manager.Socket.Emit("register_world_ai_server", new string[]{"Test 1", "test 2" });
    //    }
    //}

    //public void TestEmit(){ 
    //  onConnect.Invoke(WorldNames); 
    //}

    private void OnDisable()
    {
        //onConnect -= SetSteramingEvent;
        if(Manager != null)
        Manager.Socket.Disconnect();
    }
}