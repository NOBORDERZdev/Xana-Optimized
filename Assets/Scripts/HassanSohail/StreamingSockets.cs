using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class StreamingSockets : MonoBehaviour
{
    public bool EnableEventStreaming ;
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
        if (isFristTime ) 
        {
            isFristTime= false;
            if (APIBasepointManager.instance.IsXanaLive)
            {
               socketAddress = "https://app-api.xana.net";
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
        if ( EnableEventStreaming)
        {

        
        EventDataDetails eventDetails = JsonUtility.FromJson<EventDataDetails>(EventDetail);
        Debug.Log("<color=green> EventDetail --  : " + eventDetails.data.id + " : " + eventDetails.data.environmentName + "</color>");
        if (isInEvent)
        {
           return;
        }
        if (EventDetail != null)
        {
            StreamingSockets.Instance.isEventTriggered= true;
            print("Recive STREAMING SOCKETS " + EventDetail);
           
            if (isInWorld && !isInEvent)
            {
                 LoadingHandler.Instance.streamingLoading.UpdateLoadingText(false);
                //LoadingHandler.Instance.StartCoroutine (LoadingHandler.Instance.streamingLoading.ResetLoadingBar());
                LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
                ConstantsHolder.xanaConstants.JjWorldSceneChange = true;
                StreamingSockets.Instance.isInWorld=false;
                GameplayEntityLoader.instance._uiReferences.LoadMain(false);
                return;
                //await Task.Delay(12000);
            }
            if (SceneManager.GetActiveScene().name!="Main")
            {
                return;
            }
            // ReSetStreamingEvent();
            XanaEventDetails.eventDetails = eventDetails.data;
            XanaEventDetails.eventDetails.DataIsInitialized = true;
            ConstantsHolder.xanaConstants.newStreamEntery=true;
            isInEvent= true;
            DynamicEventManager.Instance.SetSceneData();
        }
        }
    }

    
     public bool isInEvent = false;
     public bool isEventTriggered= false;
     public bool isInWorld= false;
    public  async void ReSetStreamingEvent(){ 
        //isInEvent= false;
        //isEventTriggered =false;
       // XanaEventDetails.eventDetails.DataIsInitialized= false;
        //XanaEventDetails.eventDetails =null;
    }

    

    private void OnDisable()
    {
        //onConnect -= SetSteramingEvent;
        if(Manager != null)
        Manager.Socket.Disconnect();
    }
}