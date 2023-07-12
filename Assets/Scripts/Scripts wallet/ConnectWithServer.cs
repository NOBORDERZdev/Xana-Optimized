using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using System.Net;
using System.Text;
using WebSocketSharp;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
public class ConnectWithServer : MonoBehaviour
{
    public GameObject Page1;
    public GameObject Page2;
 
    WebSocket websocket;
   public Text msgtxt;
    public int AppID;
    public GameObject QRGenrate;
    public string AppUrlForAndroid;
    public string bundleIdofLunchingApp;
    bool CheckLunchingFail = false;  
    Sprite mySprite;
      

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void UnityOnStart(int num);
#endif


    // Start is called before the first frame update
    void Start()
    {
         Page1.SetActive(true);
        Page2.SetActive(false);   
         AppID = PlayerPrefs.GetInt("AppID");    
          msgtxt.text = "No msg";
    websocket = new WebSocket("ws://54.255.221.170:9898/");
    websocket.OnOpen += (o, e) => {
      Debug.Log("Open");
    };      

    websocket.Connect();  
    websocket.OnMessage += (o, e) => {
          ExampleMainThreadCall(e.Data);
     };      
   }
    public void ExampleMainThreadCall(string getText)
    {
        print("gamedata3");
        UnityMainThreadDispatcher.Instance().Enqueue(ThisWillBeExecutedOnTheMainThread(getText));
    }  
    public IEnumerator ThisWillBeExecutedOnTheMainThread(string txt)
    {
        Debug.Log("This is executed from the main thread");
        GetText(txt);  
          yield return null;  
    }     

    void GetText(string txt)  
    {
         print(txt);
         ReadResponse myResponseObj = new ReadResponse();
        myResponseObj = myResponseObj.returnResponse(txt);
        print(myResponseObj.data);


        if (myResponseObj.data == "app disconnected")
        {
            print("Hey string compared");
            Page1.SetActive(true);
            Page2.SetActive(false);
        }
        else
        {
            Page1.SetActive(false);
            Page2.SetActive(true);
            print("Implement QR generate");
            GenerateQRCode();
        }  

        if (myResponseObj.type == "success")
        {
            print("In success");

             // OpenXanaliaApp();
        }
        else if (myResponseObj.type == "connected")
        {
            print("in Connected");
          //   OpenXanaliaApp(); 
        }  
        else  
        {
            print(myResponseObj.data);
        }     
         msgtxt.text = txt;  
    }    
    void GenerateQRCode()
    {
        newGenrate();
    }
    
    public void OpenXanaliaApp()
    {
        print("Open Xanalia");

#if UNITY_IOS && !UNITY_EDITOR
        UnityOnStart(AppID);
#endif
#if UNITY_ANDROID
        OpenAppForAndroid();
#endif
     }
 
    void OpenAppForAndroid()
    {
        string message = "xanaliaapp://connect/";
        message += AppID.ToString();
         print(message);
        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");
        AndroidJavaObject launchIntent = null;
        try
        {
            launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleIdofLunchingApp);
            launchIntent.Call<AndroidJavaObject>("putExtra", "arguments", message);
        }
        catch (System.Exception e)
        {  
            CheckLunchingFail = true;
        }
        if (CheckLunchingFail)
        {
            Debug.Log("app not found");
            Application.OpenURL(AppUrlForAndroid);
        }
        else
        {
            ca.Call("startActivity", launchIntent);
        }
        up.Dispose();
        ca.Dispose();
        packageManager.Dispose();
        launchIntent.Dispose();
    }

    [System.Serializable]
    class ReadResponse
    {
        public string type;
        public string data;
         public ReadResponse returnResponse(string getText)
        {
            ReadResponse obj = new ReadResponse();
            obj = JsonUtility.FromJson<ReadResponse>(getText);
             return obj; 
        }  
     }  
 
      
    public void DisconnectRequestToServer()
    {  
        Disconnect1 dataObj = new Disconnect1();
        dataObj = dataObj._Disconnect(AppID);
        var jsonObj = JsonUtility.ToJson(dataObj);
        print(jsonObj);
        // print(new JsonObject(JsonUtility.ToJson(dataObj)).ToString());
        websocket.Send(jsonObj);
    }  

    [System.Serializable]
    class Disconnect1
    {
        public string type = "disconnect";
        public Disconnect2 data;
        public Disconnect1 _Disconnect(int _AppID)
        {
            Disconnect1 myObj = new Disconnect1();
            Disconnect2 obj2 = new Disconnect2();
             myObj.type = type;
            myObj.data = obj2._Disconnect2(_AppID);
            return myObj;  
         }  
     }
       

    [System.Serializable]
    class Disconnect2
    {
        public string type = "app_disconnect";
        public Disconnect3 data;
        public Disconnect2 _Disconnect2(int _AppID)
        {
            Disconnect2 myObj = new Disconnect2();
            Disconnect3 myObj2 = new Disconnect3();
             myObj.type = type;
            myObj.data = myObj2._Disconnect3(_AppID);
            return myObj;  
        }
    }
    [System.Serializable]
    class Disconnect3
    {  
        public string appId = "";
        public Disconnect3 _Disconnect3(int _AppID)   
        {
            Disconnect3 myObj = new Disconnect3();
            myObj.appId = _AppID.ToString();
             return myObj;
        }   
     }  

    public void ConnectingRequestToServer()
    {
    websocket.Connect();  
    first11 dataObj = new first11();
    dataObj = dataObj.getData(AppID);
    var jsonObj = JsonUtility.ToJson(dataObj);
    print(jsonObj);
    // print(new JsonObject(JsonUtility.ToJson(dataObj)).ToString());
    websocket.Send(jsonObj);
    }    
  [System.Serializable]
  class first11
  {
    public string type = "connect";
    public first22 data;
    public first11 getData(int myAppID)
    {  

      first11 Obj1 = new first11();
       first22 Obj2 = new first22();
        Obj1.data = Obj2.getData(myAppID);
      return Obj1;
    } 
  }
  [System.Serializable]
  class first22
  {
    public string type = "app";
    public data2 data;

    public first22 getData(int myAppID)
    {
      first22 Obj1 = new first22();
     data2 Obj2 = new data2();
      Obj1.type = type;
      Obj1.data = Obj2.GetConnectValue(myAppID);
       return Obj1;
    }
  }
  

  [System.Serializable]
  class data2
  {   
    public string appId = "11";
    public string name = "XANA";
    public string description = "hello i come from xana";    
    public string icon = "https://angeluim-metaverse.s3.ap-southeast-1.amazonaws.com/unitydata/Icon+and+Logo/logo.png";  
    public string url = "https://angeluim-metaverse.s3.ap-southeast-1.amazonaws.com/unitydata/Icon+and+Logo/XANA+2021.09+logo+1.png";
     public string jwt = ConstantsGod.AUTH_TOKEN;    
     public data2 GetConnectValue(int myAppID)
    {
      data2 Obj = new data2();    
      Obj.appId = myAppID.ToString();
      Obj.name = name;
      Obj.description = description;
      Obj.icon = icon;
      Obj.url = url;
      Obj.jwt = jwt;
      return Obj;  
    }   
  }
      
    public void newGenrate()
    {
        Texture2D myQR = generateQR(AppID.ToString());
        mySprite = Sprite.Create(myQR, new Rect(0.0f, 0.0f, myQR.width, myQR.height), new Vector2(0.5f, 0.5f), 100.0f);
         QRGenrate.GetComponent<Image>().sprite = mySprite;
    } 

    public Texture2D generateQR(string text)
    {
        var encoded = new Texture2D(256, 256);
        var color32 = Encode(text, encoded.width, encoded.height);
        encoded.SetPixels32(color32);

        encoded.Apply();
        return encoded;
    }

    private static Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }
  }

///************************************   GET Arguments In Android     ***********************************//
///
/*
 public Text argumentTxt;
    private bool focusbool;
    // Start is called before the first frame update
    void Start()
    {
        UpdateArguments();
     }  


    public void UpdateArguments()
    {
        string arguments = "";
          AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
         AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
        bool hasExtra = intent.Call<bool>("hasExtra", "arguments");
        if (hasExtra)
        {
            AndroidJavaObject extras = intent.Call<AndroidJavaObject>("getExtras");
            arguments = extras.Call<string>("getString", "arguments");
            argumentTxt.text = arguments;
        }    
        else
        {
            argumentTxt.text = "No orguments";
        }    
    }     

    private void OnApplicationPause(bool pause)
    {
        
    }
    private void OnApplicationFocus(bool focus)
    {
        focusbool = focus;
        if (focusbool)
        {
            UpdateArguments();
        }  
    }  


 */



