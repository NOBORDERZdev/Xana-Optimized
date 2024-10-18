using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class XanaliaSocketHandler : MonoBehaviour
{
    public string socketAddress = "";
    public SocketManager Manager;
    public static XanaliaSocketHandler Instance;
    public bool socketConnected;
    public TelegramRoot telegramdataRoot = new TelegramRoot();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        Application.runInBackground = true;
        Invoke(nameof(InitializeSocket), 1f);
    }

    public void InitializeSocket()
    {
        socketAddress = ConstantsGod.XANALIA_SOCKET_ADDRESS;
        Manager = new SocketManager(new Uri((socketAddress)));
        Manager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
        Manager.Socket.On<CustomError>(SocketIOEventTypes.Error, OnError);
        Manager.Socket.On<string>("builderInfo", OnTelegramLogin);
    }

    private void OnTelegramLogin(string data)
    {
       // Debug.Log("OnTelegramLogin");
        //Debug.Log("Telegramdata: " + data);
        telegramdataRoot = JsonUtility.FromJson<TelegramRoot>(data);
        if (WebViewManager.Instance != null)
        {
            WebViewManager.Instance.CloseWebView();
        }
        if (telegramdataRoot != null)
        {
            if (telegramdataRoot.data != null)
            {
                if (Web3AuthCustom.Instance!=null)
                {
                    Web3AuthCustom.Instance.OnTelegramLogin(telegramdataRoot);
                }
                
                //SaveDeviceFromXanalia();
            }
        }
    }

    public void SaveDeviceFromXanalia()
    {
        StartCoroutine(IESaveDeviceFromXanalia());
    }

    public IEnumerator IESaveDeviceFromXanalia()
    {
        string url = ConstantsGod.API_BASEURL + ConstantsGod.SAVE_DEVICE_XANALIA;
       // Debug.Log("SaveDeviceFromXanalia URL:" + url);
        WWWForm form = new WWWForm();
        form.AddField("deviceId", UniqueID());
        form.AddField("forced", "false");
        //Debug.Log("Device ID:" + UniqueID());

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            yield return www.SendWebRequest();
          //  Debug.Log("SaveDeviceFromXanalia API Logs: " + www.downloadHandler.text);
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("Errorin Validate User API: " + www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
            //    Debug.Log("SaveDeviceFromXanalia User data: " + data);
                SaveDeviceRoot saveDevice = new SaveDeviceRoot();
                saveDevice = JsonConvert.DeserializeObject<SaveDeviceRoot>(data);
                if (saveDevice.success)
                {
                    //GetUserDetail();
                }
            }
        }
    }

    public string UniqueID()
    {
        return SystemInfo.deviceUniqueIdentifier;
        if (PlayerPrefs.GetString("AppID2") == "")
        {
            int z1 = UnityEngine.Random.Range(0, 1000);
            int z2 = UnityEngine.Random.Range(0, 1000);
            string uid = z1.ToString() + z2.ToString();
            PlayerPrefs.SetString("AppID2", uid);
            PlayerPrefs.Save();
            return PlayerPrefs.GetString("AppID2");
        }
        else
        {
            return PlayerPrefs.GetString("AppID2");
        }
    }
    private void OnDestroy()
    {
        if (Manager != null)
        {
            Manager.Close();
        }
        //Debug.Log("OnDestroy called ");
    }

    void OnConnected(ConnectResponse resp)
    {
        socketConnected = true;
        //Debug.Log("XanaliaSocketHandler OnConnected:");
    }

    void OnError(CustomError args)
    {
        socketConnected = false;
        //Debug.LogError(string.Format("Error: {0}", args.ToString()));
    }
    
    [System.Serializable]
    public class SaveDeviceData
    {
        public Device device = new Device();
        public List<int> user;
    }

    [System.Serializable]
    public class Device
    {
        public int id;
        public int userId;
        public string deviceId;
        public DateTime expiredIn;
        public int userInfo;
        public string native_app_id;
        public DateTime updatedAt;
        public DateTime createdAt;
    }

    [System.Serializable]
    public class SaveDeviceRoot
    {
        public bool success;
        public SaveDeviceData data = new SaveDeviceData();
        public string msg;
    }
}
[Serializable]
public class TelegramData
{
    public string access_token;
    public string xanaToken;
    public string nftDuelToken;
    public string refresh_token;
    public TelegramUser user = new TelegramUser();
    public bool restrict;
    public bool new_login;
    public string builderLoginHash;
}

[Serializable]
public class TelegramRoot
{
    public TelegramData data = new TelegramData();
}

[Serializable]
public class TelegramUser
{
    public int id;
    public string username;
    public string email;
    public object avatar;
    public string walletAddress;
}