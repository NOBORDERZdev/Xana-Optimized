using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using WebSocketSharp;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;
using System;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
public class ConnectWallet : MonoBehaviour
{
    public static ConnectWallet instance;
    WebSocket websocket;
    public string AppID;
    public GameObject QRGenrate;
    public string AppUrlForAndroid;
    public string bundleIdofLunchingApp;
    bool CheckLunchingFail = false;
    string ServerNounceXanalia = "";

    string SignedSigXanalia = "";
    private bool XanaliaSignedMsg = false;
    Sprite mySprite;
    public string GetUserNounceURL = "";
    public string VerifySignedURL = "";
    public string VerifySignedXanaliaURL = "";
    public string GetXanaliaNounceURL = "";
    public string GetXanaliaNFTURL = "";
    private string ServerNounce;
    private GameObject WalletLoginLoader;
    private bool LoaderBool;
    private bool LoginXanaliaBool;
    public GameObject SuccessfulPopUp;
    public string NameAPIURL = "";
    public bool walletFunctionalitybool = false;
    public List<GameObject> WalletUIObj;
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void UnityOnStart(int num);
#endif

    public UserLoginSignupManager UserLoginSignupManagerInstance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        LoaderBool = false;
        XanaliaSignedMsg = false;
        // websocket = new WebSocket("ws://54.255.221.170:9898/");     
        ////  websocket = new WebSocket("ws://192.168.18.55:9898/");
        //websocket.OnOpen += (o, e) => {
        //    Debug.Log("Open");
        //};
        //websocket.Connect();   
        //websocket.OnMessage += (o, e) => {
        //    ExampleMainThreadCall(e.Data);
        //};

        StartCoroutine(WalletLoginCheck());
        // }


    }
    IEnumerator walletFunctionality()
    {
        yield return new WaitForEndOfFrame();
        if (walletFunctionalitybool)
        {
            foreach (GameObject go in WalletUIObj)
            {
                go.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject go in WalletUIObj)
            {
                go.SetActive(false);
            }
        }
    }

    private IEnumerator WalletLoginCheck()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(ConstantsGod.API_BASEURL + ConstantsGod.WALLETSTATUS))
        {
            request.SendWebRequest();
            while(!request.isDone)
            {
                yield return null;
            }
            RootWalletLogin JsonDataObj = new RootWalletLogin();
            JsonDataObj = JsonUtility.FromJson<RootWalletLogin>(request.downloadHandler.text);
            walletFunctionalitybool = JsonDataObj.data.isWalletEnabled;
            StartCoroutine(walletFunctionality());
            request.Dispose();
        }
    }

    string uniqueID()
    {
        int z1 = UnityEngine.Random.Range(0, 1000);
        int z2 = UnityEngine.Random.Range(0, 1000);
        string uid = z1.ToString() + z2.ToString();
        return uid;
    }

    
    public void ExampleMainThreadCall(string getText)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(ThisWillBeExecutedOnTheMainThread(getText));
    }
    public IEnumerator ThisWillBeExecutedOnTheMainThread(string txt)
    {
        Debug.Log("This is executed from the main thread");
        ///walletComment
        //GetText(txt);
        //End
        yield return null;
    }
    
    public void GenerateMsg(bool XanaliaBool = false)
    {
        XanaliaSignedMsg = XanaliaBool;
        WalletConnectDataClasses.GenerateMsgClass MsgGenObj = new WalletConnectDataClasses.GenerateMsgClass();
        //  GenerateMsgClass MsgGenObj = new GenerateMsgClass();   
        if (!XanaliaBool)
            MsgGenObj = MsgGenObj.msgClassFtn(ServerNounce, AppID);
        else
            MsgGenObj = MsgGenObj.msgClassFtn(ServerNounceXanalia, AppID);
        var jsonObj = JsonUtility.ToJson(MsgGenObj);
        //websocket.Send(jsonObj);          
    }

    void GenerateQRCode()
    {
        newGenrate();
    }


    public void OpenXanaliaApp()
    {
        print("Open Xanalia");
#if UNITY_IOS && !UNITY_EDITOR
        UnityOnStart(int.Parse(AppID));   
       //    OpenXanaliaAppWithURL();
#endif
#if UNITY_ANDROID || UNITY_EDITOR
        OpenAppForAndroid();
        //  OpenAppForAndroidURL();
        //  OpenXanaliaAppWithURL();
#endif
    }
    void OpenAppForAndroid()
    {
        string message = "xanaliaapp://connect/";
        message += AppID.ToString();
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
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "https://www.xanalia.com/");

            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject(
                            "android.content.Intent",
                            intentClass.GetStatic<string>("ACTION_VIEW"),
                            uriObject
            );

            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

            currentActivity.Call("startActivity", intentObject);
            // Application.OpenURL(message);

        }
        else
        {
            ca.Call("startActivity", launchIntent);
        }
        up.Dispose();
        ca.Dispose();
        packageManager.Dispose();
        launchIntent.Dispose();
        //checkPackageAppIsPresent("com.xanaliaApp");

    }
    public void OpenXanaliaAppWithURL()
    {
        print("Open Xanalia");
        OpenAppForAndroidURL();
    }

    void OpenAppForAndroidURL()
    {
        string message = "xanaliaapp://connect/";
        message += AppID.ToString();
        Application.OpenURL(message);
    }

    private void checkPackageAppIsPresent(string package)
    {
        bool fail = false;
        string bundleId = package; //target bundle id for gallery!?
        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject packagemanager = ca.Call<AndroidJavaObject>("getPackage$$anonymous$$anager");

        AndroidJavaObject launchIntent = null;
        try
        {
            launchIntent = packagemanager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleId);
        }
        catch (System.Exception e)
        {
            fail = true;
        }

        if (fail)
        {

            //open app in store
            //  Application.OpenURL("https://play.google.com/store/apps/details?id=com.xanaliaApp");
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "https://play.google.com/store/apps/details?id=com.xanaliaApp");

            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject(
                            "android.content.Intent",
                            intentClass.GetStatic<string>("ACTION_VIEW"),
                            uriObject
            );

            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

            currentActivity.Call("startActivity", intentObject);
        }
        else //I want to open Gallery App? But what activity?
            ca.Call("startActivity", launchIntent);

        up.Dispose();
        ca.Dispose();
        packagemanager.Dispose();
        launchIntent.Dispose();
    }
    public void VerifySignature()
    {
        print("Signature verify here ");
    }
    [HideInInspector] public bool isWalletNewReg = false; // for new registration of wallet  
    public void DisconnectRequestToServer()
    {
        WalletConnectDataClasses.Disconnect1 dataObj = new WalletConnectDataClasses.Disconnect1();
        dataObj = dataObj._Disconnect(AppID);
        var jsonObj = JsonUtility.ToJson(dataObj);
        if (isWalletNewReg)
        {
            //UserRegisterationManager.instance.OpenUIPanal(5);
        }
        
        LoadingController.Instance.nftLoadingScreen.SetActive(false);
        
    }
    IEnumerator waitForLoader(GameObject loaderObj = null)
    {
        yield return new WaitUntil(() => !LoaderBool);
        if (WalletLoginLoader != null)
        {
            WalletLoginLoader.SetActive(false);
        }
    }
    IEnumerator waitForbool()
    {
        yield return new WaitForSeconds(15);
        LoaderBool = false;
    }
    public void ConnectingRequestToServer()
    {
        if (LoaderBool)
        {
            return;
        }
        LoaderBool = true;
        LoginXanaliaBool = true;
        WalletLoginLoader = EventSystem.current.currentSelectedGameObject;
        WalletLoginLoader = WalletLoginLoader.transform.Find("Loader").gameObject;
        StartCoroutine(waitForLoader(WalletLoginLoader));
        StartCoroutine(waitForbool());
        if (WalletLoginLoader == null)
        {
            return;
        }
        WalletLoginLoader.SetActive(true);
       
        AppID = uniqueID();
        websocket.Connect();
        WalletConnectDataClasses.first11 dataObj = new WalletConnectDataClasses.first11();
        dataObj = dataObj.getData(AppID);
        var jsonObj = JsonUtility.ToJson(dataObj);
        websocket.Send(jsonObj);
    }
    public void ConnectingSignUp()
    {
        LoginXanaliaBool = false;
        AppID = uniqueID();
        websocket.Connect();
        WalletConnectDataClasses.first11 dataObj = new WalletConnectDataClasses.first11();
        dataObj = dataObj.getData(AppID);
        var jsonObj = JsonUtility.ToJson(dataObj);
        websocket.Send(jsonObj);
    }



    private void newGenrate()
    {
        Texture2D myQR = generateQR(AppID.ToString());
        mySprite = Sprite.Create(myQR, new Rect(0.0f, 0.0f, myQR.width, myQR.height), new Vector2(0.5f, 0.5f), 100.0f);
        QRGenrate.GetComponent<Image>().sprite = mySprite;
    }

    private Texture2D generateQR(string text)
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


    public IEnumerator SaveChainSafeNonce(string sign, string walletAddress, string nonce)
    {
        string url = ConstantsGod.API_BASEURL + ConstantsGod.SaveNonce;
        UnityWebRequest request;
        WWWForm form = new WWWForm();
        form.AddField("walletAddress", walletAddress);
        form.AddField("nonce", nonce);
        request = UnityWebRequest.Post(url, form);
        request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            AppID = uniqueID();
            GetXanaliaNounce(sign, walletAddress, nonce);
            yield return new WaitForSeconds(0.5f);
        }
        request.Dispose();
    }

    
    public void GetXanaliaNounce(string sign, string walletAddress, string nonce)
    {
        WalletConnectDataClasses.NounceClassForXanalia NounceObj = new WalletConnectDataClasses.NounceClassForXanalia();
        NounceObj = NounceObj.NounceClassFtnForXanalia(PlayerPrefs.GetString("publicID"));
        var jsonObj = JsonUtility.ToJson(NounceObj);
        StartCoroutine(HitGetNounceFromXANALIAServerAPI(ConstantsGod.API_BASEURL_XANALIA + ConstantsGod.GetXanaliaNounceURL, jsonObj, sign, walletAddress, nonce));
    }

    string XanaliaNonce;
    // API,s calling
    IEnumerator HitGetNounceFromXANALIAServerAPI(string url, string Jsondata, string sign, string walletAddress, string nonce)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(Jsondata);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SendWebRequest();
        while (!request.isDone)
            yield return null;
        WalletConnectDataClasses.NounceMsgXanalia NounceReadObjXanalia = JsonUtility.FromJson<WalletConnectDataClasses.NounceMsgXanalia>(request.downloadHandler.text);
        //Debug.LogError(request.downloadHandler.text);
        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            if (NounceReadObjXanalia.success)
            {
                ServerNounceXanalia = NounceReadObjXanalia.data;
                XanaliaSignedMsg = true;
                StartCoroutine(HitChainSafeVerifySignatureAPI(sign, walletAddress, nonce));

                GenerateMsg(XanaliaSignedMsg);
            }
        }
        else
        {
            if (!NounceReadObjXanalia.success)
            {
                //  DisconnectRequestToServer();
                Debug.Log("Success false in  get Nounce of Xanalia");
            }
        }
        request.Dispose();
    }


    IEnumerator HitChainSafeVerifySignatureAPI(string sign, string walletAddress, string nonce)
    {
        string url = ConstantsGod.API_BASEURL + ConstantsGod.VerifySignedURL;
        UnityWebRequest request;
        WWWForm form = new WWWForm();
        form.AddField("signature", sign);
        form.AddField("nonce", nonce);
        request = UnityWebRequest.Post(url, form);
        request.SendWebRequest();
        //Debug.LogError("request has sent already");
        while (!request.isDone)
        {
            yield return null;
        }
        WalletConnectDataClasses.ClassWithToken VerifySignatureReadObj = new WalletConnectDataClasses.ClassWithToken();
        VerifySignatureReadObj = WalletConnectDataClasses.ClassWithToken.CreateFromJSON(request.downloadHandler.text);
        //Debug.LogError(request.downloadHandler.text);
        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            if (VerifySignatureReadObj.success)
            { 
                PlayerPrefs.SetInt("WalletConnect", 1);
                PlayerPrefs.SetString("LoginToken", VerifySignatureReadObj.data.token);
                ConstantsGod.AUTH_TOKEN = VerifySignatureReadObj.data.token;
                ConstantsHolder.xanaToken = VerifySignatureReadObj.data.token;
                ConstantsHolder.loggedIn = true;
                PlayerPrefs.SetString("UserName", VerifySignatureReadObj.data.user.id.ToString());

                UserLoginSignupManager.instance.LoginWithWallet();
                PlayerPrefs.Save();
                SetNameInServer();
                GetNFTList();

                WalletConnectDataClasses.VerifySignedMsgClass VerifySignatureObj = new WalletConnectDataClasses.VerifySignedMsgClass();
                VerifySignatureObj = VerifySignatureObj.VerifySignedClassFtn(VerifySignatureObj.nonce, sign);
                var jsonObj2 = JsonUtility.ToJson(VerifySignatureObj);
                //Debug.LogError(sign + "--" + ServerNounceXanalia);
                StartCoroutine(HitChainSafeVerifySignatureXanaliaAPI(ConstantsGod.API_BASEURL_XANALIA + ConstantsGod.VerifySignedXanaliaURL, sign, ServerNounceXanalia));
            }
        }
        else
        {
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                DisconnectRequestToServer();
                Debug.Log("Network error in Verify signature");
            }
            else
            {
                if (request.error != null)
                {
                    if (!VerifySignatureReadObj.success)
                    {
                        DisconnectRequestToServer();
                        Debug.Log("Success false in  verify sig");
                    }
                }
            }
        }

        request.Dispose();
    }

    IEnumerator HitChainSafeVerifySignatureXanaliaAPI(string url, string sign, string nonce)
    {
        UnityWebRequest request;
        WWWForm form = new WWWForm();

        form.AddField("signature", sign);
        form.AddField("nonce", nonce);
        request = UnityWebRequest.Post(url, form);
        request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        WalletConnectDataClasses.VerifyReadSignedMsgFromServerXanalia VerifySignatureReadObj = new WalletConnectDataClasses.VerifyReadSignedMsgFromServerXanalia();
        VerifySignatureReadObj = JsonUtility.FromJson<WalletConnectDataClasses.VerifyReadSignedMsgFromServerXanalia>(request.downloadHandler.text);
        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            if (request.error == null)
            {
                if (VerifySignatureReadObj.success)
                {
                    // free // premium // alpha-pass
                    VerifySignatureReadObj.data.user.userNftRole = VerifySignatureReadObj.data.user.userNftRole.ToLower();
                   
                    switch (VerifySignatureReadObj.data.user.userNftRole)
                    {
                        case "alpha-pass":
                            {
                                UserPassManager.Instance.GetGroupDetails("Access Pass");
                                break;
                            }
                        case "premium":
                            {
                                UserPassManager.Instance.GetGroupDetails("Extra NFT");
                                break;
                            }
                        case "dj-event":
                            {
                                UserPassManager.Instance.GetGroupDetails("djevent");
                                break;
                            }
                        case "free":
                            {
                                UserPassManager.Instance.GetGroupDetails("freeuser");

                                break;
                            }
                    }
                    PlayerPrefs.SetString("LoginTokenxanalia", VerifySignatureReadObj.data.token);
                    if (VerifySignatureReadObj.data.user.title != null)
                    {
                        PlayerPrefs.SetString("Useridxanalia", VerifySignatureReadObj.data.user.title.ToString());
                    }
                    else
                    {
                        String s = VerifySignatureReadObj.data.user.username.ToString();
                        PlayerPrefs.SetString("Useridxanalia", s.Substring(0, 4));
                    }
                    if (WalletLoginLoader != null)
                        WalletLoginLoader.SetActive(false);
                    LoaderBool = false;
                    PlayerPrefs.SetInt("WalletConnect", 1);
                    UserLoginSignupManager.instance.LoginWithWallet();
                    PlayerPrefs.Save();
                    SetNameInServer();
                    GetNFTList();
                }
            }
        }
        else
        {
            //Api is not working from Xanalia side so commented for now to temp fix issue -18-03-2024

            //if (request.result == UnityWebRequest.Result.ConnectionError)
            //{
            //    UserLoginSignupManagerInstance.ShowValidationPop(Sign_Up_Scripts.ErrorType.Could_not_verify_signature);
            //    UserLoginSignupManager.instance.ShowWelcomeScreen();
            //    DisconnectRequestToServer();
            //    Debug.Log("Network error in Verify signature of xanalia");
            //}
            //else
            //{
            //    if (request.error != null)
            //    {
            //        if (!VerifySignatureReadObj.success)
            //        {
            //            UserLoginSignupManagerInstance.ShowValidationPop(Sign_Up_Scripts.ErrorType.Could_not_verify_signature);
            //            UserLoginSignupManager.instance.ShowWelcomeScreen();
            //            DisconnectRequestToServer();
            //            Debug.Log("Success false in  verify sig  of xanalia");
            //        }
            //    }
            //}
        }
        request.Dispose();
    }

   
    public void GetNFTList()
    {
        WalletConnectDataClasses.NFTList NFTCreateJson = new WalletConnectDataClasses.NFTList();
        // NFTCreateJson = NFTCreateJson.AssignNFTList(30, PlayerPrefs.GetString("Useridxanalia") , "testnet", "mycollection", PlayerPrefs.GetString("Useridxanalia") , 1);
        NFTCreateJson = NFTCreateJson.AssignNFTList(2, PlayerPrefs.GetString("publicKey"), "testnet", "mycollection", PlayerPrefs.GetString("publicKey"), 1);
        var jsonObj = JsonUtility.ToJson(NFTCreateJson);
        StartCoroutine(HitGetXanaliaNFTAPI(ConstantsGod.API_BASEURL_XANALIA + ConstantsGod.GetXanaliaNFTURL, jsonObj));
    }

    IEnumerator HitGetXanaliaNFTAPI(string url, string Jsondata)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(Jsondata);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", PlayerPrefs.GetString("LoginTokenxanalia"));
        request.SendWebRequest();
        while (!request.isDone)
            yield return null;
        WalletConnectDataClasses.Root ReadObj = new WalletConnectDataClasses.Root();
        ReadObj = JsonUtility.FromJson<WalletConnectDataClasses.Root>(request.downloadHandler.text);

        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            if (ReadObj.success)
            { 
            }
        }
        else
        {
            if (request.result==UnityWebRequest.Result.Success)
            {
                DisconnectRequestToServer();
                Debug.Log("Network error in Getting NFT list of Xanalia");
            }
            else
            {
                if (request.error != null)
                {
                    if (!ReadObj.success)
                    {
                        DisconnectRequestToServer();
                        Debug.Log("Success false in  Getting NFT list of Xanalia");
                    }
                }
            }
        }

        request.Dispose();
    }

    void SetNameInServer()
    {
        MyClassOfPostingName myObject = new MyClassOfPostingName();
        string bodyJsonOfName = JsonUtility.ToJson(myObject.GetNamedata(PlayerPrefs.GetString("Useridxanalia")));
        StartCoroutine(HitNameAPIWithNewTechnique(ConstantsGod.API_BASEURL + ConstantsGod.NameAPIURL, bodyJsonOfName, PlayerPrefs.GetString("Useridxanalia")));
        ConstantsHolder.xanaConstants.LoginasGustprofile = true;
    }

    IEnumerator HitNameAPIWithNewTechnique(string url, string Jsondata, string localUsername)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(Jsondata);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        request.SendWebRequest();
        while (!request.isDone)
            yield return null;
        MyClassNewApi myObject1 = new MyClassNewApi();
        if (request.result!=UnityWebRequest.Result.ConnectionError && request.result==UnityWebRequest.Result.Success)
        {
            myObject1 = CheckResponceJsonNewApi(request.downloadHandler.text);
            if (request.error == null)
            {
                if (myObject1.success)
                {
                    PlayerPrefs.SetInt("IsLoggedIn", 1);
                    PlayerPrefs.SetInt("FristPresetSet", 1);
                    ServerSideUserDataHandler.Instance.GetDataFromServer();
                    PlayerPrefs.SetString("PlayerName", localUsername);
                    if (UIHandler.Instance != null)//rik  
                    {
                        UIHandler.Instance._footerCan.transform.GetChild(0).GetComponent<HomeFooterTabCanvas>().HomeSceneFooterSNSButtonIntrectableTrueFalse();
                    }
                }
            }
        }
        else
        {
        }

        request.Dispose();
    }
    [System.Serializable]
    public class JsonObjectBase
    {
    }
    [Serializable]
    public class MyClassOfPostingName : JsonObjectBase
    {
        public string name;
        public MyClassOfPostingName GetNamedata(string name)
        {
            MyClassOfPostingName myObj = new MyClassOfPostingName();
            myObj.name = name;
            return myObj;
        }
    }


    [Serializable]
    public class MyClassNewApi
    {
        public MyClassNewApi myObject;
        public bool success;
        public string msg;
        public string data;
        public MyClassNewApi Load(string savedData)
        {
            myObject = new MyClassNewApi();
            myObject = JsonUtility.FromJson<MyClassNewApi>(savedData);
            return myObject;
        }
    }
    MyClassNewApi CheckResponceJsonNewApi(string Localdata)
    {
        MyClassNewApi myObject = new MyClassNewApi();
        myObject = myObject.Load(Localdata);
        return myObject;
    }

}










// 

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



[System.Serializable]
public class DataWalletLogin
{
    public int id;
    public bool isWalletEnabled;
    public DateTime createdAt;
    public DateTime updatedAt;
}

[System.Serializable]
public class RootWalletLogin
{
    public bool success;
    public DataWalletLogin data;
    public string msg;
}