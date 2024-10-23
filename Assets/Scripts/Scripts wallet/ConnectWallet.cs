using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Text;
using WebSocketSharp;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;
using System;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using Photon.Pun.Demo.Procedural;
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
           // print("IS true " + walletFunctionalitybool);
            foreach (GameObject go in WalletUIObj)
            {
                go.SetActive(true);
            }
        }
        else
        {
            //print("IS false " + walletFunctionalitybool);
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
            //print("request");
            // request.SetRequestHeader("Authorization", PlayerPrefs.GetString("Token"));
            //print("Sending Web Request");
            yield return request.SendWebRequest();
            //print("REsult is " + request.downloadHandler.text);
            RootWalletLogin JsonDataObj = new RootWalletLogin();
            JsonDataObj = JsonUtility.FromJson<RootWalletLogin>(request.downloadHandler.text);
            walletFunctionalitybool = JsonDataObj.data.isWalletEnabled;
            StartCoroutine(walletFunctionality());
        }
    }

    string uniqueID()
    {
        //print("Give unique key");
        //  DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        // int currentEpochTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
        int z1 = UnityEngine.Random.Range(0, 1000);
        int z2 = UnityEngine.Random.Range(0, 1000);
        // string uid = currentEpochTime + ":" + z1 + ":" + z2;
        string uid = z1.ToString() + z2.ToString();
        return uid;
    }

    //public void OpenMenu(string menuName)
    //{
    //    switch (menuName)
    //    {

    //        case "GenerateQR":
    //            {
    //                print("Implement QR generate");
    //                //   OpenXanaliaApp();
    //                if (LoginXanaliaBool)
    //                {
    //                    //  OpenXanaliaAppWithURL();
    //                    OpenXanaliaApp();
    //                }
    //                else
    //                {
    //                    UserRegisterationManager.instance.OnSignUpWalletTabPressed();
    //                    GenerateQRCode();
    //                }
    //                break;
    //            }

    //        case "connected":
    //            {
    //                break;
    //            }
    //        case "MessageForwardToWallet":
    //            {
    //                break;
    //            }

    //        case "disconnected":
    //            {
    //                if (WalletLoginLoader != null)
    //                    WalletLoginLoader.SetActive(false);
    //                LoaderBool = false;
    //                break;
    //            }
    //        case "Rejected":
    //            {
    //                LoaderBool = false;

    //                print("I am in rejected Case");
    //                if (WalletLoginLoader != null)
    //                    WalletLoginLoader.SetActive(false);
    //                break;
    //            }

    //        case "VerifySignature":
    //            {
    //                break;
    //            }

    //        case "Removed":
    //            {
    //                if (PlayerPrefs.GetInt("WalletConnect") == 1)
    //                {
    //                    PlayerPrefs.SetInt("IsLoggedIn", 0);
    //                    PlayerPrefs.SetInt("WalletConnect", 0);

    //                    if (UserRegisterationManager.instance != null)
    //                    {
    //                        UserRegisterationManager.instance.LoggedIn = false;
    //                    }
    //                    LoginXanaliaBool = false;
    //                    PlayerPrefs.Save();
    //                    print("removed here 22");
    //                    if (SNSSettingController.Instance != null)
    //                    {
    //                        SNSSettingController.Instance.LogoutSuccess();
    //                    }
    //                    LoaderBool = false;
    //                }
    //                break;
    //            }

    //        case "OpenJWTPage":
    //            {
    //                print("Congrats JWT received ");
    //                if (WalletLoginLoader != null)
    //                    WalletLoginLoader.SetActive(false);
    //                LoaderBool = false;

    //                PlayerPrefs.SetInt("WalletConnect", 1);
    //                //SuccessfulPopUp.SetActive(true);
    //                UserRegisterationManager.instance.LoginWithWallet();
    //                PlayerPrefs.Save();
    //                SetNameInServer();


    //                PlayerPrefs.Save();
    //                //  GetXanaliaNounce();

    //                //  UserRegisterationManager.instance.LoginWithWallet();
    //                break;
    //            }
    //    }
    //}
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
    //void GetText(string txt)
    //{
    //    print(txt);
    //    WalletConnectDataClasses.GeneralClassFields GeneralFields = JsonUtility.FromJson<WalletConnectDataClasses.GeneralClassFields>(txt);
    //    if (GeneralFields.status == "error")
    //    {
    //        WalletConnectDataClasses.ErrorClass objerror = new WalletConnectDataClasses.ErrorClass();
    //        Debug.Log("Error in Response");
    //        Debug.Log(objerror.data);
    //        Debug.Log(objerror.type);
    //        if (WalletLoginLoader != null)
    //            WalletLoginLoader.SetActive(false);
    //        LoaderBool = false;
    //    }
    //    else if (GeneralFields.status == "success")
    //    {
    //        switch (GeneralFields.type)
    //        {
    //            case "app connect":
    //                WalletConnectDataClasses.AppConnectClass objConnectServer = new WalletConnectDataClasses.AppConnectClass();
    //                objConnectServer = JsonUtility.FromJson<WalletConnectDataClasses.AppConnectClass>(txt);
    //                OpenMenu("GenerateQR");
    //                break;
    //            case "connection approved":
    //                WalletConnectDataClasses.ConnectedClass objConnected = new WalletConnectDataClasses.ConnectedClass();
    //                objConnected = JsonUtility.FromJson<WalletConnectDataClasses.ConnectedClass>(txt);
    //                OpenMenu("connected");
    //                print("Implement COnnnected");
    //                print("Wallet address is " + objConnected.data.address);
    //                print("Wallet id is " + objConnected.data.walletId);
    //                print("Wallet msg is " + objConnected.data.msg);
    //                // string walletPublicID = objConnected.data.address;
    //                //WalletConnectDataClasses.NounceClass NounceObj = new WalletConnectDataClasses.NounceClass();
    //                //NounceObj = NounceObj.NounceClassFtn(walletPublicID);
    //                //var jsonObj = JsonUtility.ToJson(NounceObj);
    //                //print("Nouce JSON is  " + jsonObj);  
    //                //     StartCoroutine(HitGetNounceFromServerAPI(GetUserNounceURL, jsonObj));
    //                break;
    //            case "disconnect":
    //                print("Disconnected");
    //                OpenMenu("disconnected");
    //                break;
    //            case "connection reject":
    //                print("rejected here");
    //                WalletConnectDataClasses.AppRejectedClass objRejected = new WalletConnectDataClasses.AppRejectedClass();
    //                objRejected = JsonUtility.FromJson<WalletConnectDataClasses.AppRejectedClass>(txt);

    //                print(objRejected.data.msg + "  " + objRejected.data.walletId);
    //                OpenMenu("Rejected");
    //                XanaliaSignedMsg = false;
    //                break;
    //            case "verifysig":
    //                print("type verifysig");
    //                print(txt);
    //                WalletConnectDataClasses.VerifySignatureClass objVerify = new WalletConnectDataClasses.VerifySignatureClass();
    //                objVerify = JsonUtility.FromJson<WalletConnectDataClasses.VerifySignatureClass>(txt);
    //                print("public key is " + objVerify.data.pubKey);
    //                PlayerPrefs.SetString("publicKey", objVerify.data.pubKey);
    //                print("signature key is " + objVerify.data.sig);
    //                print("Nounce is " + objVerify.data.nonce);
    //                PlayerPrefs.SetString("publicKey", objVerify.data.pubKey);
    //                ServerNounce = objVerify.data.nonce;

    //                ServerNounceXanalia = objVerify.data.nonceXanalia;
    //                SignedSigXanalia = objVerify.data.sigXanalia;
    //                OpenMenu("VerifySignature");
    //                string SignedSignature = objVerify.data.sig;
    //                if (!XanaliaSignedMsg)
    //                {
    //                    WalletConnectDataClasses.VerifySignedMsgClass VerifySignatureObj = new WalletConnectDataClasses.VerifySignedMsgClass();
    //                    VerifySignatureObj = VerifySignatureObj.VerifySignedClassFtn(ServerNounce, objVerify.data.sig);
    //                    var jsonObj2 = JsonUtility.ToJson(VerifySignatureObj);
    //                    print("Verify Signed msg Json is  " + jsonObj2);
    //                    ///Wallet Commect  
    //                    ///StartCoroutine(HitVerifySignatureAPI(ConstantsGod.API_BASEURL+ConstantsGod.VerifySignedURL, jsonObj2));
    //                    ///End
    //                }
    //                else
    //                {
    //                    //WalletConnectDataClasses.VerifySignedMsgClass VerifySignatureObj = new WalletConnectDataClasses.VerifySignedMsgClass();
    //                    // VerifySignatureObj = VerifySignatureObj.VerifySignedClassFtn(ServerNounceXanalia, objVerify.data.sigXanalia);
    //                    //var jsonObj2 = JsonUtility.ToJson(VerifySignatureObj);  
    //                    //print("Xanalia Verify Signed msg Json is  " + jsonObj2);  
    //                    // StartCoroutine(HitVerifySignatureXanaliaAPI(VerifySignedXanaliaURL, jsonObj2));  
    //                }


    //                //    https://testapi.xanalia.com/auth/verify-signature
    //                //     post(verifySigUrl, {
    //                //      nonce: nonce,
    //                //         signature: signature,
    //                //     });


    //                break;
    //            //   { "status": "success", "type": "remove", "data":{ "msg":"walletId removed","walletId":"0xfaE360CBaf3f31E8F5511e7b06e4A50C956B438a"} }
    //            case "remove":
    //                print("removed here");
    //                WalletConnectDataClasses.AppRejectedClass objRemoved = new WalletConnectDataClasses.AppRejectedClass();
    //                objRemoved = JsonUtility.FromJson<WalletConnectDataClasses.AppRejectedClass>(txt);
    //                print(objRemoved.data.msg + "  " + objRemoved.data.walletId);
    //                OpenMenu("Removed");
    //                XanaliaSignedMsg = false;

    //                if (SNSSettingController.Instance != null)
    //                {
    //                    SNSSettingController.Instance.LogoutSuccess();
    //                }
    //                break;
    //        }
    //    }

    //}


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
        //print("Asking class " + jsonObj);
        //websocket.Send(jsonObj);          
    }

    void GenerateQRCode()
    {
        newGenrate();
    }


    public void OpenXanaliaApp()
    {
        //print("Open Xanalia");
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
        //print("app not found bool" + CheckLunchingFail);
        if (CheckLunchingFail)
        {
            //print("app not found");
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
        print(message);
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
        //  Disconnect1 dataObj = new Disconnect1();
        dataObj = dataObj._Disconnect(AppID);
        var jsonObj = JsonUtility.ToJson(dataObj);
        print(jsonObj);
        if (isWalletNewReg)
        {
            //UserRegisterationManager.instance.OpenUIPanal(5);
        }
        //SuccessfulPopUp.SetActive(true);
        if (!ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
        {
            LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
        }
        else
        {
            LoadingHandler.Instance.LoadingScreenSummit.SetActive(false);
        }
        // print(new JsonObject(JsonUtility.ToJson(dataObj)).ToString());
        ///Wallet Connect 
        //// websocket.Send(jsonObj);    
        ///
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
        // "appId":"1646310332:677007:973992
        // 1646310332:677007:973992
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
        //if (WalletLoginLoader.activeInHierarchy && LoginXanaliaBool)
        //   return;
        WalletLoginLoader.SetActive(true);
        //if(PlayerPrefs.GetString("AppID")=="")
        //{
        //    AppID = uniqueID();
        //    PlayerPrefs.SetString("AppID", AppID);

        //}
        //else
        //{
        //    AppID = PlayerPrefs.GetString("AppID");
        //}
        AppID = uniqueID();
        websocket.Connect();
        WalletConnectDataClasses.first11 dataObj = new WalletConnectDataClasses.first11();
        //   first11 dataObj = new first11();
        dataObj = dataObj.getData(AppID);
        var jsonObj = JsonUtility.ToJson(dataObj);
        print(jsonObj);
        websocket.Send(jsonObj);
    }
    public void ConnectingSignUp()
    {
        //  return;
        LoginXanaliaBool = false;
        print("App ID " + PlayerPrefs.GetString("AppID"));
        //if (PlayerPrefs.GetString("AppID") == "")
        //{
        //    AppID = uniqueID();
        //    PlayerPrefs.SetString("AppID", AppID);
        // }           
        //else
        //{
        //    AppID = PlayerPrefs.GetString("AppID");
        //}  
        AppID = uniqueID();
        websocket.Connect();
        WalletConnectDataClasses.first11 dataObj = new WalletConnectDataClasses.first11();
        //   first11 dataObj = new first11();
        dataObj = dataObj.getData(AppID);
        var jsonObj = JsonUtility.ToJson(dataObj);
        print(jsonObj);
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

    // API,s calling
    //IEnumerator HitGetNounceFromServerAPI(string url, string Jsondata)
    //{
    //    // print(Jsondata);
    //    var request = new UnityWebRequest(url, "POST");
    //    byte[] bodyRaw = Encoding.UTF8.GetBytes(Jsondata);
    //    request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
    //    request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    //    request.SetRequestHeader("Content-Type", "application/json");
    //    yield return request.SendWebRequest();
    //    print("Json data is  = " + request.downloadHandler.text);
    //    //  NounceMsg1
    //    WalletConnectDataClasses.NounceMsg1 NounceReadObj = JsonUtility.FromJson<WalletConnectDataClasses.NounceMsg1>(request.downloadHandler.text);
    //    if (!request.isHttpError && !request.isNetworkError)
    //    {
    //        if (request.error == null)
    //        {
    //            Debug.Log(request.downloadHandler.text);
    //            if (NounceReadObj.success)
    //            {
    //                print(" in Success Nounce Is here " + NounceReadObj.data.nonce);
    //                ServerNounce = NounceReadObj.data.nonce;
    //                GenerateMsg();
    //            }
    //        }
    //    }
    //    else
    //    {
    //        if (request.isNetworkError)
    //        {
    //            DisconnectRequestToServer();
    //            Debug.Log("Network error in Get Nounce");
    //        }
    //        else
    //        {
    //            if (request.error != null)
    //            {
    //                if (!NounceReadObj.success)
    //                {
    //                    DisconnectRequestToServer();
    //                    Debug.Log("Success false in  get Nounce");
    //                }
    //            }
    //        }
    //    }
    //}

    //IEnumerator HitVerifySignatureAPI(string url, string Jsondata)
    //{  
    //    // print(Jsondata);
    //    var request = new UnityWebRequest(url, "POST");
    //    byte[] bodyRaw = Encoding.UTF8.GetBytes(Jsondata);
    //    request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
    //    request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    //    request.SetRequestHeader("Content-Type", "application/json");
    //    yield return request.SendWebRequest();
    //    print("Json data of Signed signature is   = " + request.downloadHandler.text);
    //    //    WalletConnectDataClasses.VerifyReadSignedMsgFromServer VerifySignatureReadObj = JsonUtility.FromJson<WalletConnectDataClasses.VerifyReadSignedMsgFromServer>(request.downloadHandler.text);
    //    WalletConnectDataClasses.ClassWithToken VerifySignatureReadObj = new WalletConnectDataClasses.ClassWithToken();  
    //    VerifySignatureReadObj = WalletConnectDataClasses.ClassWithToken.CreateFromJSON(request.downloadHandler.text);  
    //       if (!request.isHttpError && !request.isNetworkError)
    //       {
    //           if (request.error == null)  
    //           {
    //               Debug.Log(request.downloadHandler.text);
    //               if (VerifySignatureReadObj.success)
    //               {
    //                PlayerPrefs.SetInt("WalletLogin", 1);
    //                 PlayerPrefs.SetString("LoginToken", VerifySignatureReadObj.data.token);
    //                PlayerPrefs.SetString("UserName", VerifySignatureReadObj.data.user.id.ToString());  
    //                OpenMenu("OpenJWTPage");      
    //                print("JWT token of user is  " + VerifySignatureReadObj.data.token);


    //                WalletConnectDataClasses.VerifySignedMsgClass VerifySignatureObj = new WalletConnectDataClasses.VerifySignedMsgClass();
    //                VerifySignatureObj = VerifySignatureObj.VerifySignedClassFtn(ServerNounceXanalia, SignedSigXanalia);
    //                var jsonObj2 = JsonUtility.ToJson(VerifySignatureObj);
    //                print("Xanalia Verify Signed msg Json is  " + jsonObj2);
    //                StartCoroutine(HitVerifySignatureXanaliaAPI(ConstantsGod.API_BASEURL_XANALIA+ConstantsGod.VerifySignedXanaliaURL, jsonObj2));

    //            }      
    //           }    
    //       }  
    //       else
    //       {
    //           if (request.isNetworkError)
    //           {
    //               DisconnectRequestToServer();
    //              Debug.Log("Network error in Verify signature");
    //           }
    //           else
    //           {
    //               if (request.error != null)
    //               {
    //                   if (!VerifySignatureReadObj.success)
    //                   {
    //                       DisconnectRequestToServer();
    //                      Debug.Log("Success false in  verify sig");
    //                   }
    //               }
    //           }
    //       }
    //}


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
                PlayerPrefs.SetString("UserId", VerifySignatureReadObj.data.user.id.ToString());
                ConstantsGod.AUTH_TOKEN = VerifySignatureReadObj.data.token;
                ConstantsHolder.xanaToken = VerifySignatureReadObj.data.token;
                //Debug.Log(VerifySignatureReadObj.data.token);
                ConstantsHolder.userId = VerifySignatureReadObj.data.user.id.ToString();
                //ConstantsHolder.loggedIn = true; // Updating Value in LoginWithWallet();
                PlayerPrefs.SetString("UserName", VerifySignatureReadObj.data.user.name.ToString());
                ConstantsHolder.userName = VerifySignatureReadObj.data.user.name.ToString();
                PlayerPrefs.Save();
                SetNameInServer();
                GetNFTList();
                UserLoginSignupManager.instance.LoginWithWallet();
                MainSceneEventHandler.OnSucessFullLogin?.Invoke();
                WalletConnectDataClasses.VerifySignedMsgClass VerifySignatureObj = new WalletConnectDataClasses.VerifySignedMsgClass();
                VerifySignatureObj = VerifySignatureObj.VerifySignedClassFtn(VerifySignatureObj.nonce, sign);
                var jsonObj2 = JsonUtility.ToJson(VerifySignatureObj);
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
                        //Debug.Log("Success false in  verify sig");
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
                    print(" userNftRole " + VerifySignatureReadObj.data.user.userNftRole);
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

    // 
    //IEnumerator HitVerifySignatureXanaliaAPI(string url, string Jsondata)
    //{
    //     print(Jsondata);      
    //    var request = new UnityWebRequest(url, "POST");
    //    byte[] bodyRaw = Encoding.UTF8.GetBytes(Jsondata);
    //    request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
    //    request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    //    request.SetRequestHeader("Content-Type", "application/json");  
    //    yield return request.SendWebRequest();    
    //     WalletConnectDataClasses.VerifyReadSignedMsgFromServerXanalia VerifySignatureReadObj = new WalletConnectDataClasses.VerifyReadSignedMsgFromServerXanalia();
    //    VerifySignatureReadObj = JsonUtility.FromJson<WalletConnectDataClasses.VerifyReadSignedMsgFromServerXanalia>(request.downloadHandler.text);
    //    print("Login Xanalia is "+request.downloadHandler.text);    
    //       if (!request.isHttpError && !request.isNetworkError)
    //        {   
    //          if (request.error == null)  
    //          {
    //             if (VerifySignatureReadObj.success)
    //              {
    //                 print(" userNftRole " + VerifySignatureReadObj.data.user.userNftRole);
    //                // free // premium // alpha-pass
    //                VerifySignatureReadObj.data.user.userNftRole = VerifySignatureReadObj.data.user.userNftRole.ToLower();

    //                switch(VerifySignatureReadObj.data.user.userNftRole)
    //                 {
    //                    case "alpha-pass":   
    //                        {
    //                            UserPassManager.Instance.GetGroupDetails("Access Pass");
    //                             break;
    //                        }
    //                    case "premium":
    //                        {
    //                            UserPassManager.Instance.GetGroupDetails("Extra NFT");
    //                             break;
    //                        }
    //                    case "dj-event":  
    //                        {
    //                            UserPassManager.Instance.GetGroupDetails("djevent");  
    //                            break;
    //                        }    
    //                    case "free":
    //                        {
    //                            UserPassManager.Instance.GetGroupDetails("freeuser");

    //                            break;
    //                        }
    //                }  

    //                /*
    //                if (VerifySignatureReadObj.data.user.userNftRole =="")
    //                {  
    //                  if (VerifySignatureReadObj.data.user.userNftRole.Contains("access"))
    //                      {
    //                         UserPassManager.Instance.GetGroupDetails("Access Pass");
    //                      }     
    //                    else  
    //                    {
    //                          UserPassManager.Instance.GetGroupDetails("Extra NFT");  
    //                     }      
    //                  }    
    //                else  
    //                {
    //                     UserPassManager.Instance.GetGroupDetails("freeuser");
    //                }           
    //                */
    //                 PlayerPrefs.SetString("LoginTokenxanalia", VerifySignatureReadObj.data.token);  
    //                if(VerifySignatureReadObj.data.user.title != null)
    //                {
    //                    PlayerPrefs.SetString("Useridxanalia", VerifySignatureReadObj.data.user.title.ToString());
    //                 }
    //                else
    //                {

    //                    print(VerifySignatureReadObj.data.user.username);
    //                    String s = VerifySignatureReadObj.data.user.username.ToString();
    //                    print("The first four character of the string is: " + s.Substring(0, 4));  
    //                    PlayerPrefs.SetString("Useridxanalia", s.Substring(0, 4));  
    //                    Debug.Log("title is null");  
    //                }
    //                print("JWT token of xanalia is   " + PlayerPrefs.GetString("LoginTokenxanalia"));
    //               //  PlayerPrefs.SetString("UserName", PlayerPrefs.GetString("Useridxanalia"));
    //                PlayerPrefs.SetInt("WalletConnect", 1);
    //                SuccessfulPopUp.SetActive(true);
    //                UserRegisterationManager.instance.LoginWithWallet();  
    //                PlayerPrefs.Save();    
    //                 SetNameInServer();
    //                 print("ID of UserName is  :  " + PlayerPrefs.GetString("Useridxanalia"));
    //                  GetNFTList();
    //             }  
    //        }
    //      }
    //      else
    //      {
    //          if (request.isNetworkError)
    //          {
    //           //   DisconnectRequestToServer();
    //             Debug.Log("Network error in Verify signature of xanalia");
    //          }
    //          else
    //          {
    //              if (request.error != null)
    //              {
    //                  if (!VerifySignatureReadObj.success)
    //                  {
    //                     // DisconnectRequestToServer();
    //                     Debug.Log("Success false in  verify sig  of xanalia");
    //                  }
    //              }
    //          }
    //      }

    //}
    /*
    {limit: 25,
    loggedIn: "0x7ebe14ab1e82f9d230d8235c5ca7d3b77d92b07d",
    networkType: "testnet",
    nftType: "mycollection",
    owner: "0x7ebe14ab1e82f9d230d8235c5ca7d3b77d92b07d",
    page: 1}
    */
    public void GetNFTList()
    {
        // print("JWT token of xanalia is   " + PlayerPrefs.GetString("LoginTokenxanalia"));
        //print("ID of Xanalia is  :  " + PlayerPrefs.GetString("Useridxanalia"));
       // print("Get list is ");
        WalletConnectDataClasses.NFTList NFTCreateJson = new WalletConnectDataClasses.NFTList();
        // NFTCreateJson = NFTCreateJson.AssignNFTList(30, PlayerPrefs.GetString("Useridxanalia") , "testnet", "mycollection", PlayerPrefs.GetString("Useridxanalia") , 1);
        NFTCreateJson = NFTCreateJson.AssignNFTList(2, PlayerPrefs.GetString("publicKey"), "testnet", "mycollection", PlayerPrefs.GetString("publicKey"), 1);
        var jsonObj = JsonUtility.ToJson(NFTCreateJson);
       // print("Json is  : " + jsonObj);
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
        // print("List of NFT's are   = " + request.downloadHandler.text);
        WalletConnectDataClasses.Root ReadObj = new WalletConnectDataClasses.Root();
        ReadObj = JsonUtility.FromJson<WalletConnectDataClasses.Root>(request.downloadHandler.text);

        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            if (ReadObj.success)
            {
               // print("Success is " + ReadObj.success);
               // print("Counter is " + ReadObj.count);
                //  print("Event  is " + ReadObj.data[0].returnValues["0"]);    
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
                        //Debug.Log("Success false in  Getting NFT list of Xanalia");
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
        //print("Body " + Jsondata);
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(Jsondata);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        request.SendWebRequest();
        while (!request.isDone)
            yield return null;
        //Debug.Log(request.downloadHandler.text);
        MyClassNewApi myObject1 = new MyClassNewApi();
        if (request.result!=UnityWebRequest.Result.ConnectionError && request.result==UnityWebRequest.Result.Success)
        {
            myObject1 = CheckResponceJsonNewApi(request.downloadHandler.text);
            if (request.error == null)
            {
                Debug.Log(request.downloadHandler.text);
                if (myObject1.success)
                {
                   // print("Success in name  field ");
                    PlayerPrefs.SetInt("IsLoggedIn", 1);
                    PlayerPrefs.SetInt("FristPresetSet", 1);
                    ServerSideUserDataHandler.Instance.GetDataFromServer();
                    PlayerPrefs.SetString("PlayerName", localUsername);
                    if (GameManager.Instance.UiManager != null)//rik  
                    {
                        GameManager.Instance.UiManager._footerCan.transform.GetChild(0).GetComponent<HomeFooterHandler>().HomeSceneFooterSNSButtonIntrectableTrueFalse();
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
            print("savedData " + savedData);
            myObject = JsonUtility.FromJson<MyClassNewApi>(savedData);
            return myObject;
        }
    }
    MyClassNewApi CheckResponceJsonNewApi(string Localdata)
    {
        MyClassNewApi myObject = new MyClassNewApi();
        myObject = myObject.Load(Localdata);
        print("myObject " + myObject.data);
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