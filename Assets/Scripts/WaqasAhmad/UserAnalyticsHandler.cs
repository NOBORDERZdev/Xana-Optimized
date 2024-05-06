using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Linq;
using UnityEngine.SceneManagement;


using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using Newtonsoft.Json;


public class UserAnalyticsHandler : MonoBehaviour
{
    public static Action<int, string> onGetWorldId;
    public static Action<bool, bool, bool, bool> onUpdateWorldRelatedStats;
    public static Action<bool, bool> onUpdateWorldStatCustom;
    public static Action<int, string, Text> onGetSingleWorldStats;
    public static Action<string> onChangeJoinUserStats;
    public static Action<bool> onUserJoinedLeaved;

    //string API_GetWorldId = "/analytical/enter-xana-world";
    //string API_GetWorldId_Guest = "/analytical/guest-enter-xana-world";

    //string API_GetSingleWorldStats = "/analytical/get-single-world-stats/";
    //string API_GetSingleWorldStats_Guest = "/analytical/guest-get-single-world-stats/";

    //string API_UpdateWorldStats = "/analytical/update-xana-world-stats";
    //string API_UpdateWorldStats_Guest = "/analytical/guest-update-xana-world-stats";

    string address = "https://api-test.xana.net/";

    public string userDataString;
    public string jsonPrefix;

    public SocketManager Manager;


    void Start()
    {
        address = ConstantsGod.API_BASEURL;

        if (!address.EndsWith("/"))
        {
            address = address + "/";
        }

        Manager = new SocketManager(new Uri((address)));
        Manager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
        Manager.Socket.On<CustomError>(SocketIOEventTypes.Error, OnError);

        Manager.Socket.On<string>("player_count", UserData);
        Manager.Socket.On<string>("PlayerSocketId", PlayerSocketID);
        Manager.Socket.On<string>("get_all_world_data", UserCountUpdate);
    }
    private void OnEnable()
    {
        onGetWorldId += Call_GetWorldId_Coroutine;
        onUpdateWorldRelatedStats += Call_UpdateWorldRelatedStats_Courtine;
        onUpdateWorldStatCustom += Call_UpdateWorldRelatedStats_Courtine;
        onGetSingleWorldStats += Call_GetSingleWorldStats_Courtine;
        onUserJoinedLeaved += OnUserJoinedFunction;
        // onGetWorldId?.Invoke(5,"True")
    }
    private void OnDisable()
    {
       
        onGetWorldId -= Call_GetWorldId_Coroutine;
        onUpdateWorldRelatedStats -= Call_UpdateWorldRelatedStats_Courtine;
        onGetSingleWorldStats -= Call_GetSingleWorldStats_Courtine;
        onUserJoinedLeaved -= OnUserJoinedFunction;
        onUpdateWorldStatCustom -= Call_UpdateWorldRelatedStats_Courtine;
    }
    private void OnApplicationQuit()
    {
        if(Manager!=null){ 
            StartCoroutine(SetSession(false));
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            if(Manager!=null){ 
                StartCoroutine(SetSession(true));
            }
        }
        else
        {
            if(Manager!=null){ 
                StartCoroutine(SetSession(false));
            }
        }
    }

    #region Api Calls Handling

    void Call_GetWorldId_Coroutine(int worldId, string worldType)
    {
        StartCoroutine(GetWorldId(worldId, ModifyWorldType(worldType)));
    }
    void Call_GetSingleWorldStats_Courtine(int worldId, string worldType, Text visitCountText)
    {
        StartCoroutine(GetSingleWorldStats(worldId, ModifyWorldType(worldType), visitCountText));
    }
    void Call_UpdateWorldRelatedStats_Courtine(bool isJoined, bool nftClicked, bool urlClicked, bool isExit)
    {
//#if UNITY_EDITOR
//        return;
//#else
        StartCoroutine(UpdateWorldRelatedStats(isJoined, nftClicked, urlClicked, isExit));

//#endif
    }
    void Call_UpdateWorldRelatedStats_Courtine(bool isJoined, bool isExit)
    {
//#if UNITY_EDITOR
//        return;
//#else
        StartCoroutine(UpdateWorldRelatedStats(isJoined, isExit));

//#endif
    }


    // Get Custom WorldRecord ID From Database
    IEnumerator GetWorldId(int worldId, string worldType)
    {
        string token = ConstantsGod.AUTH_TOKEN;
        WWWForm form = new WWWForm();

        form.AddField("world_id", worldId);
        form.AddField("world_type", worldType);
        UnityWebRequest www;
        
        if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
            www = UnityWebRequest.Post(ConstantsGod.API_BASEURL + ConstantsGod.API_GetWorldId_Guest, form);
        else
            www = UnityWebRequest.Post(ConstantsGod.API_BASEURL + ConstantsGod.API_GetWorldId, form);
        
        www.SetRequestHeader("Authorization", token);
        www.SendWebRequest();
        
        while (!www.isDone)
        {
            yield return null;
        }

        string str = www.downloadHandler.text;
        APIResponse response = new APIResponse();

        if (!www.isHttpError && !www.isNetworkError)
        {
            response = JsonUtility.FromJson<APIResponse>(str);
            ConstantsHolder.xanaConstants.worldIdFromApi = response.data;
            Debug.Log("<color=green> Analytics -- Record ID : " + response.data + "</color>");
        }
        else
            Debug.Log("<color=red> Analytics -- NetWorkissue </color>");

        www.Dispose();
    }

    // Get Visited User Count for single World
    IEnumerator GetSingleWorldStats(int worldId, string worldType, Text visitCountText)
    {
        string token = ConstantsGod.AUTH_TOKEN;
        WWWForm form = new WWWForm();

        UnityWebRequest www;
        if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
            www = UnityWebRequest.Get(ConstantsGod.API_BASEURL + ConstantsGod.API_GetSingleWorldStats_Guest + worldId + "/" + worldType);
        else
            www = UnityWebRequest.Get(ConstantsGod.API_BASEURL + ConstantsGod.API_GetSingleWorldStats + worldId + "/" + worldType);
        
        www.SetRequestHeader("Authorization", token);
        www.SendWebRequest();

        while (!www.isDone)
        {
            yield return null;
        }

        Debug.Log("<color=green> Analytics -- WorldVisit : " + www.downloadHandler.text + "</color>");
        string str = www.downloadHandler.text;
        APIResponse2 response2 = new APIResponse2();

        if (!www.isHttpError && !www.isNetworkError)
        {
            response2 = JsonUtility.FromJson<APIResponse2>(str);
            if (response2.data != null && response2.data.Length > 0)
                visitCountText.text = ConverThousand_Millions(float.Parse((response2.data[0].count)));
            else
                visitCountText.text = "" + 0;
            
            Debug.Log("<color=green> Analytics -- Api Responce Success </color>");
        }
        else
           Debug.Log("NetWorkissue");

        www.Dispose();
    }

    /// <summary>
    ///  Update World Related Stats for Live Users
    ///  Send Values which are True
    /// </summary>

    IEnumerator UpdateWorldRelatedStats(bool isJoined, bool nftClicked, bool urlClicked, bool isExit)
    {
        string token = ConstantsGod.AUTH_TOKEN;
        WWWForm form = new WWWForm();

        form.AddField("record_id", ConstantsHolder.xanaConstants.worldIdFromApi);

        if (isJoined)
        {
            form.AddField("is_joined", isJoined.ToString());
            if(!string.IsNullOrEmpty(ConstantsHolder.xanaConstants.playerSocketID))
                form.AddField("socket_id", ConstantsHolder.xanaConstants.playerSocketID);
        }

        if (nftClicked)
            form.AddField("clicked_nft", "" + nftClicked.ToString());

        if (urlClicked)
            form.AddField("clicked_url", "" + urlClicked.ToString());

        if (isExit)
            form.AddField("is_exit", "" + isExit.ToString());

        Debug.Log("####### " + ConstantsHolder.xanaConstants.worldIdFromApi + "   -  " + isJoined + "   -  " + nftClicked + "   -  " + urlClicked + "   -  " + isExit);

        UnityWebRequest www;
        if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
            www = UnityWebRequest.Post(ConstantsGod.API_BASEURL + ConstantsGod.API_UpdateWorldStats_Guest, form);
        else
            www = UnityWebRequest.Post(ConstantsGod.API_BASEURL + ConstantsGod.API_UpdateWorldStats, form);
        
        www.SetRequestHeader("Authorization", token);
        www.SendWebRequest();
        while (!www.isDone)
        {
            yield return null;
        }

        if (!www.isHttpError && !www.isNetworkError)
        {
            if (isJoined)
                Manager.Socket.Emit("enter_world");
        }
        else
           Debug.Log("API NetWorkissue");
        www.Dispose();
    }
    
    /// <summary>
    /// Send both Parametes value In API
    /// </summary>
    IEnumerator UpdateWorldRelatedStats(bool isJoined, bool isExit)
    {
        string token = ConstantsGod.AUTH_TOKEN;
        WWWForm form = new WWWForm();

        form.AddField("record_id", ConstantsHolder.xanaConstants.worldIdFromApi);
        form.AddField("is_joined", isJoined.ToString());
        form.AddField("is_exit", "" + isExit.ToString());

        //if (isJoined)
        //{
        //    if(!string.IsNullOrEmpty(ConstantsHolder.xanaConstants.playerSocketID))
        //        form.AddField("socket_id", ConstantsHolder.xanaConstants.playerSocketID);
        //}


        Debug.Log("####### " + ConstantsHolder.xanaConstants.worldIdFromApi + "   -  " + isJoined + "   -  " + isExit);

        UnityWebRequest www;
        if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
            www = UnityWebRequest.Post(ConstantsGod.API_BASEURL + ConstantsGod.API_UpdateWorldStats_Guest, form);
        else
            www = UnityWebRequest.Post(ConstantsGod.API_BASEURL + ConstantsGod.API_UpdateWorldStats, form);
        
        www.SetRequestHeader("Authorization", token);
        www.SendWebRequest();
        while (!www.isDone)
        {
            yield return null;
        }

        //Debug.Log(www.downloadHandler.text);
        if (!www.isHttpError && !www.isNetworkError)
        {
            //Debug.Log("API Called Success ");
            if (isJoined)
            {
                Manager.Socket.Emit("enter_world");
            }
        }
        else
           Debug.Log("API NetWorkissue");
        www.Dispose();
    }


    string ModifyWorldType(string worldType)
    {
        if (worldType.Contains("_"))
        {
            worldType = worldType.Split("_").First();
        }
        else if(worldType == "ENVIRONMENTS")
        {
            worldType = "ENVIRONMENT";
        }
        else if (worldType == "MUSEUMS")
        {
            worldType = "MUSEUM";
        } 

        return worldType;
    }
    string ConverThousand_Millions(float amount)
    {
        float tempAmount = 0.0f;
        float million = 1000000;
        float thousand = 1000;

        string modifyAmount = "";

        if (amount >= million)
        {
            tempAmount = MathF.Round((amount / million), 2);
            modifyAmount = tempAmount.ToString("F") + " M";
        }
        else if (amount >= thousand)
        {
            tempAmount = MathF.Round((amount / thousand), 2);
            modifyAmount = tempAmount.ToString("F") + " K";
        }
        else
        {
            modifyAmount = amount.ToString("");
        }

        return modifyAmount;
    }
#endregion

#region Socket Calls Handling

    void OnConnected(ConnectResponse resp)
    {
        //Debug.Log("Analatics -- Socket Connect :");
        //Debug.Log("<color=green> Analatics -- Socket Connect" + "</color>");
        Manager.Socket.Emit("get_all_world_data");
        Manager.Socket.Emit("enter_world");

        if (!SceneManager.GetActiveScene().name.Contains("Home"))
        {
            //Debug.Log("<color=green> Analatics -- Again Sending Call " + "</color>");
            Call_UpdateWorldRelatedStats_Courtine(true, false);
        }
        StartCoroutine(SetSession(true));
    }
    void OnError(CustomError args)
    {
        //Debug.Log(string.Format("Error: {0}", args.ToString()));
    }
    void Onresult(CustomError args)
    {
        //Debug.Log(string.Format("Error: {0}", args.ToString()));
    }


    void OnUserJoinedFunction(bool isJoined)
    {
        if (isJoined)
        {
            Manager.Socket.Emit("enter_world");
        }
        else
        {
            Manager.Socket.Emit("exit_world");
        }
    }
    public void UserData(string _data)
    {
        _data = jsonPrefix + _data + "}";
       // Debug.LogError("Analytics -- Data : " + _data);
        //Debug.Log("<color=green> Analytics -- Data : " + _data + "</color>");
        userDataString = _data;
        onChangeJoinUserStats?.Invoke(_data);
    }
    void UserCountUpdate(string _data)
    {
       // Debug.LogError("Analytics -- Data : " + _data);
        //Debug.Log("<color=green> Analytics -- Data : " + _data + "</color>");
        _data = jsonPrefix + _data + "}";
        userDataString = _data;
        onChangeJoinUserStats?.Invoke(_data);
    }

    void PlayerSocketID(string socketId)
    {
        //Debug.Log("<color=green> Analytics-- SocketId: " + socketId + "</color>");
        ConstantsHolder.xanaConstants.playerSocketID = socketId;
    }
#endregion

    /// <summary>
    /// User Session 
    /// </summary>
    /// <param name="isStart"> true on applicaiton start and false on application kill</param>
    IEnumerator SetSession( bool isStart){
        while (ConstantsHolder.userId == "") {
            yield return new WaitForSeconds(2f);
        }
        string userId = ConstantsHolder.userId;
        string product ;
#if !UNITY_EDITOR
    #if UNITY_ANDROID
        product = "xanaappandroid";

    #elif UNITY_IOS
        product ="xanaappios";
    #endif     
        var data = new { userId , product};
        Debug.Log("Data:::" + data);
        if (isStart)
        {
                Manager.Socket.Emit("user_start_date", data);
        }
        else
        {
                Manager.Socket.Emit("user_end_date", data);
        }
#endif
    }
}

[Serializable]
public class APIResponse
{
    public bool success;
    public int data;
    public string msg;
}
public class APIResponse2
{
    public bool success;
    // Data Class Already create 
    // Using the same call with Additional Variable "count"
    public Data[] data;
}

[System.Serializable]
public class AllWorldData // Actice User in World
{
    public SingleWorldData[] player_count;
}

[System.Serializable]
public class SingleWorldData
{
    public int world_id;
    public string world_type;
    public int count;
}