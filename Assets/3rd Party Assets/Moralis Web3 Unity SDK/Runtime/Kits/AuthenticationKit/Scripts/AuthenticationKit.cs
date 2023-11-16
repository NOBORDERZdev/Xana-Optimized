using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MoralisUnity.Platform.Objects;
using MoralisUnity.Sdk.Exceptions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Unity;
using WalletConnectSharp.Unity.Models;

#pragma warning disable CS1998
namespace MoralisUnity.Kits.AuthenticationKit
{
    /// <summary>
    /// Moralis "kits" each provide drag-and-drop functionality for developers.
    /// Developers add a kit at edit-time to give additional runtime functionality for users.
    ///
    /// The "AuthenticationKit" provides cross-platform 
    /// support for web3 authentication (See <see cref="AuthenticationKitPlatform"/>).
    ///
    /// Usage:
    /// Drag-and-drop the <see cref="AuthenticationKit.prefab"/> into any Unity Scene.
    ///
    /// This <see cref="AuthenticationKit"/> class is the wrapper for the <see cref="AuthenticationKitController"/>.
    /// 
    /// </summary>
    public class AuthenticationKit : MonoBehaviour
    {
        public static AuthenticationKit Instance;
        // irfan Script //
        public GameObject multipleLoginPanal;
        public GameObject LoaderObj;  
        public string ServerNounce;    
        public string GetUserNounceURL = "https://app-api.xana.net/auth/get-user-nonce";
        public string saveUserNounceURL = "save-user-nonce";

        public string VerifySignedURL = "verify-signature";
        public string VerifySignedURL2 = "v2/verify-signature";

        public GameObject BackBtn;

        public string BaseURLMainNet = "https://tcg-api-test.xana.net/auth/";
        public string BaseURLTestNet = "https://tcg-api-test.xana.net/api/auth/";

        string SetDeviceTokenAPI = "https://app-api.xana.net/users/set-device-token";
        // END here //


        //  Properties ------------------------------------
        public bool WillInitializeOnStart
        {
            get { return _willInitializeOnStart; }
        }


        [Header("Settings")] [SerializeField] private bool _willInitializeOnStart = true;
        [SerializeField] private bool _signAndLoginToMoralis = true;

        //  Events ----------------------------------------

        /// <summary>
        /// Invoked when State==AuthenticationKitState.MoralisLoggedIn
        /// </summary>
        [Header("Events")] public UnityEvent OnConnected = new UnityEvent();

        /// <summary>
        /// Invoked when State==AuthenticationKitState.Disconnected
        /// </summary>
        public UnityEvent OnDisconnected = new UnityEvent();

        /// <summary>
        /// Invoked upon any change to the <see cref="AuthenticationKitState"/>
        /// </summary>
        public AuthenticationKitStateUnityEvent OnStateChanged = new AuthenticationKitStateUnityEvent();

        /// <summary>
        /// Get the current <see cref="AuthenticationKitState"/>
        /// </summary>
        public AuthenticationKitState State
        {
            get { return _stateObservable.Value; }
            set { _stateObservable.Value = value; }
        }

        private AuthenticationKitStateObservable _stateObservable = new AuthenticationKitStateObservable();

        //  Fields ----------------------------------------
        [Header("3rd Party")] [SerializeField] private WalletConnect _walletConnect;

        //  Properties ------------------------------------
        /// <summary>
        /// Get the current <see cref="AuthenticationKitPlatform"/>
        /// </summary>
        public AuthenticationKitPlatform AuthenticationKitPlatform
        {
            get
            {
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    return AuthenticationKitPlatform.WalletConnect;
                }       
#if UNITY_ANDROID
                return AuthenticationKitPlatform.Android;
#elif UNITY_IOS
                return AuthenticationKitPlatform.iOS;
#elif UNITY_WEBGL
                return AuthenticationKitPlatform.WebGL;
#elif UNITY_EDITOR
                return AuthenticationKitPlatform.WalletConnect;
#else  
                return AuthenticationKitPlatform.WalletConnect;
#endif
            }
        }

        //  Unity Methods ---------------------------------
        public AuthenticationKit()
        {
            // Any state changes here are likely too 'early'
            // to be observed externally. That is ok. Just FYI.
            State = AuthenticationKitState.PreInitialized;
            _stateObservable.OnValueChanged.AddListener(StateObservable_OnValueChanged);
        }

        protected async void Start()
        {
            Instance = this;
            // Add the EventSystem if there is none
            if (FindObjectOfType<EventSystem>() == null)
            {
                var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }

            if (_willInitializeOnStart)
            {
                await InitializeAsync();
            }
        }

        /// <summary>
        /// Initialize the <see cref="AuthenticationKitController"/>.
        /// </summary>
        /// <returns></returns>
        public async UniTask InitializeAsync()
        {
            State = AuthenticationKitState.Initializing;

            // Initialize Moralis
            Moralis.Start();

            // Log out any old users so we do a full authentication cycle
            await Moralis.LogOutAsync();

            State = AuthenticationKitState.Initialized;
        }


        //  Methods ---------------------------------------

        /// <summary>
        /// Connect to Web3.
        /// </summary>
        public void Connect()
        {
            State = AuthenticationKitState.WalletConnecting;
        }

        /// <summary>
        /// User presses the retry button
        /// </summary>
        public void Retry()
        {
            // Based on which state we are one the Retry button does different things
            switch (State)
            {
                // If the Wallet is trying to connect
                case AuthenticationKitState.WalletConnecting:
                    switch (AuthenticationKitPlatform)
                    {
                        case AuthenticationKitPlatform.WalletConnect:
                            // With the QR code there is no need for a retry button
                            // TODO Add a manual refresh button if the QR is not working
                            break;
                        case AuthenticationKitPlatform.Android:
                            // Retry to open the DeepLink
                            _walletConnect.OpenDeepLink();
                            break;
                        case AuthenticationKitPlatform.iOS:
                            // Let users go back to the wallet select screen
                            State = AuthenticationKitState.WalletConnecting;
                            break;
                        case AuthenticationKitPlatform.WebGL:
                            // TODO Add a retry on a fail instead of disconnect and start over
                            break;
                        default:
                            SwitchDefaultException.Throw(AuthenticationKitPlatform);
                            break;
                    }

                    break;
                case AuthenticationKitState.WalletSigning:
                    switch (AuthenticationKitPlatform)
                    {
                        case AuthenticationKitPlatform.WalletConnect:
                            // TODO Add a retry option if the wallet fails. Chance of failure is small. 
                            break;
                        case AuthenticationKitPlatform.Android:
                        case AuthenticationKitPlatform.iOS:
                            _walletConnect.OpenMobileWallet();
                            break;
                        case AuthenticationKitPlatform.WebGL:
                            // TODO Add a retry on a fail instead of disconnect and start over
                            break;
                        default:
                            SwitchDefaultException.Throw(AuthenticationKitPlatform);
                            break;
                    }

                    break;
            }
        }
         /// <summary>
        /// LogIn to Web3 session.
        /// </summary>
        public async UniTask LoginWithWeb3()
        {
#if !UNITY_WEBGL
            new PlatformNotSupportedException();
#else
            string userAddr = "";
            
            // Try to sign and catch the Exception when a user cancels the request
            try
            {
                if (!Web3GL.IsConnected())
                {
                    await Moralis.SetupWeb3();
                    userAddr = Web3GL.Account();
                }
                else
                {
                    userAddr = Web3GL.Account();
                }
            }
            catch
            {
                // Disconnect and start over if a user cancels the connecting request or there is an error
                Disconnect();
                return;
            }
            
            State = AuthenticationKitState.WalletConnected;

            if (_signAndLoginToMoralis)
            {

                State = AuthenticationKitState.WalletSigning;

                if (string.IsNullOrWhiteSpace(userAddr))
                {
                    Debug.LogError("Could not login or fetch account from web3.");
                }
                else
                {
                    string address = Web3GL.Account().ToLower();
                    string appId = Moralis.DappId;
                    long serverTime = 0;

                    // Retrieve server time from Moralis Server for message signature
                    Dictionary<string, object> serverTimeResponse =
                        await Moralis.Cloud.RunAsync<Dictionary<string, object>>("getServerTime",
                            new Dictionary<string, object>());

                    if (serverTimeResponse == null || !serverTimeResponse.ContainsKey("dateTime") ||
                        !long.TryParse(serverTimeResponse["dateTime"].ToString(), out serverTime))
                    {
                        Debug.LogError("Failed to retrieve server time from Moralis Server!");
                    }

                    string signMessage = $"Moralis Authentication\n\nId: {appId}:{serverTime}";

                    string signature = null;

                    // Try to sign and catch the Exception when a user cancels the request
                    try
                    {
                        signature = await Web3GL.Sign(signMessage);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                        // Disconnect and start over if a user cancels the singing request or there is an error
                        Disconnect();
                        return;
                    }

                    State = AuthenticationKitState.WalletSigned;

                    State = AuthenticationKitState.MoralisLoggingIn;

                    // Create Moralis auth data from message signing response.
                    Dictionary<string, object> authData = new Dictionary<string, object>
                    {
                        { "id", address }, { "signature", signature }, { "data", signMessage }
                    };

                    // Get chain Id
                    int chainId = Web3GL.ChainId();

                    // Attempt to login user.
                    MoralisUser user = await Moralis.LogInAsync(authData, chainId);

                    if (user != null)
                    {
                        State = AuthenticationKitState.MoralisLoggedIn;
                    }
                }
            }
#endif
        }

        /// <summary>
        /// Handles WalletConnect connecting and setting up Web3
        /// </summary>
        /// <returns></returns>
        private async void WalletConnect_Connect()
        {
            // CLear out the session so it is re-establish on sign-in.
            _walletConnect.CLearSession();

            // Enable auto save to remember the session for future use 
            _walletConnect.autoSaveAndResume = true;

            // Don't start a new session on disconnect automatically
            _walletConnect.createNewSessionOnSessionDisconnect = false;
              LoaderObj.SetActive(true);  
            // Warning the _walletConnect.Connect() won't finish until a user approved Wallet connection has been established
            await _walletConnect.Connect();
             // If WalletConnect is connected set the state to WalletConnected or Disconnect and start over
            if (_walletConnect.Connected)
            {
                State = AuthenticationKitState.WalletConnected;
            }
            else
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Handles WalletConnect signing when <see cref="WalletConnect"/> has a session connected.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private async void WalletConnect_SignAndLoginToMoralis(WalletConnectUnitySession session)
        {
            // Debug.Log($"WalletConnect_OnConnectedEventSession() wcSessionData = {session}");

            // If there is already a Moralis user we can skip the sign and login and go straight to connected
            if (await Moralis.GetUserAsync() != null)
            {
                State = AuthenticationKitState.MoralisLoggedIn;
                return;
            }

            State = AuthenticationKitState.WalletSigning;

            // Extract wallet address from the Wallet Connect Session data object.
            string address = session.Accounts[0].ToLower();
            string appId = Moralis.DappId;
            long serverTime = 0;

            // Retrieve server time from Moralis Server for message signature
            Dictionary<string, object> serverTimeResponse = await Moralis.Cloud
                .RunAsync<Dictionary<string, object>>("getServerTime", new Dictionary<string, object>());

            if (serverTimeResponse == null || !serverTimeResponse.ContainsKey("dateTime") ||
                !long.TryParse(serverTimeResponse["dateTime"].ToString(), out serverTime))
            {
                Debug.LogError("Failed to retrieve server time from Moralis Server!");
            }

            string signMessage = $"Moralis Authentication\n\nId: {appId}:{serverTime}";

            // Irfan Script // 
            ServerNounce = signMessage;
            ConnectServerDataExtraction.NounceClass NounceObj = new ConnectServerDataExtraction.NounceClass();
            NounceObj = NounceObj.NounceClassFtn(address, ServerNounce);
            var jsonObj = JsonUtility.ToJson(NounceObj);
            print("Nouce JSON is  " + jsonObj);

            UnityWebRequest request;
            if (CryptouserData.instance.Testnet)
            {

                saveUserNounceURL = BaseURLTestNet + saveUserNounceURL;
            }
            else
            {
                saveUserNounceURL = BaseURLMainNet + saveUserNounceURL;
            }
            print("save user nounce URL " + saveUserNounceURL);
            print(CryptouserData.instance.Testnet);
            print(BaseURLMainNet);
            print(saveUserNounceURL);
            print(BaseURLTestNet);
            request = await CallingXanaAPI(saveUserNounceURL, jsonObj);
            print("After response");
            ConnectServerDataExtraction.NounceMsg1 NounceReadObj = JsonUtility.FromJson<ConnectServerDataExtraction.NounceMsg1>(request.downloadHandler.text);
            if (!request.isHttpError && !request.isNetworkError)
            {
                if (request.error == null)
                {
                    Debug.Log(request.downloadHandler.text);
                    if (NounceReadObj.success)
                    {
                        print(" in Success Nounce Is here " + NounceReadObj.data.nonce);
                        ServerNounce = NounceReadObj.data.nonce;
                    }
                }
            }
            else
            {
                if (request.isNetworkError)
                {
                    Debug.LogError("Network error in Get Nounce");
                }
                else
                {
                    if (request.error != null)
                    {
                        if (!NounceReadObj.success)
                        {
                            Debug.LogError("Success false in  get Nounce");
                        }
                    }
                }
            }
            request.Dispose();
            // END here 
  

            string signature = null;

            // Try to sign and catch the Exception when a user cancels the request
            try
            {
                signature = await _walletConnect.Session.EthPersonalSign(address, signMessage);
            }
            catch
            {
                // Disconnect and start over if a user cancels the singing request or there is an error
                Disconnect();
                return;
            }

            State = AuthenticationKitState.WalletSigned;

            State = AuthenticationKitState.MoralisLoggingIn;

            // Create Moralis auth data from message signing response.
            Dictionary<string, object> authData = new Dictionary<string, object>
            {
                { "id", address }, { "signature", signature }, { "data", signMessage }
            };

            // Attempt to login user.
            MoralisUser user = await Moralis.LogInAsync(authData, session.ChainId);
             if (user != null)
            {
                 print(user);
                print("Logged in here");
                State = AuthenticationKitState.MoralisLoggedIn;
                ConnectServerDataExtraction.VerifySignedMsgClass VerifySignatureObj = new ConnectServerDataExtraction.VerifySignedMsgClass();
                string _deviceID = SystemInfo.deviceUniqueIdentifier;
                print(_deviceID);
                VerifySignatureObj = VerifySignatureObj.VerifySignedClassFtn(ServerNounce, signature, _deviceID);
                var jsonObj2 = JsonUtility.ToJson(VerifySignatureObj);
                print("Verify Signed msg Json is  " + jsonObj2);
                //StartCoroutine(HitVerifySignatureAPI(VerifySignedURL , jsonObj2));
                UnityWebRequest request1;
                if (CryptouserData.instance.Testnet)
                {
                    VerifySignedURL = BaseURLTestNet + VerifySignedURL2;
                }
                else
                {
                    VerifySignedURL = BaseURLMainNet + VerifySignedURL2;
                }
                print("VerifySignedURL is : " + VerifySignedURL);
                request1 = await HitVerifySignatureAPI(VerifySignedURL, jsonObj2);
                ConnectServerDataExtraction.ClassWithToken VerifySignatureReadObj = new ConnectServerDataExtraction.ClassWithToken();
                VerifySignatureReadObj = ConnectServerDataExtraction.ClassWithToken.CreateFromJSON(request1.downloadHandler.text);
                print("success is : " + VerifySignatureReadObj.success);
                if (!request1.isHttpError && !request1.isNetworkError)
                {
                    if (request1.error == null)
                    {
                        Debug.Log(request1.downloadHandler.text);
                        if (VerifySignatureReadObj.success)
                        {
                            PlayerPrefs.SetString("myDeviceToken", _deviceID);
                            PlayerPrefs.SetInt("walletlogin", 1);
                            PlayerPrefs.SetString("LoginToken", VerifySignatureReadObj.data.token);
                            PlayerPrefs.SetString("UserID", VerifySignatureReadObj.data.user.id.ToString());
                            PlayerPrefs.SetString("player_name", VerifySignatureReadObj.data.user.name);
                            PlayerPrefs.SetString("UserName", VerifySignatureReadObj.data.user.id.ToString());   
                             print("JWT token of user is  " + VerifySignatureReadObj.data.token);   
                            print("UserID UserID  " + VerifySignatureReadObj.data.user.id);
                            PlayerPrefs.Save();
                            if (CryptouserData.instance != null)
                            {
                                print(" success here");
                                CryptouserData.instance.UserAddress = address;
                                PlayerPrefs.SetString("PublicAddress", address);
                                CryptouserData.instance.CryptoLogin = true;
                                PlayerPrefs.SetString("publicID", address);
                                PlayerPrefs.Save();    
                                UserNFTlistClass.instance.CallOwnedNFTListAPIAsync();   
                                // StartCoroutine (SetToken());   
                            }      
                        }
                        else
                        {
                            Debug.LogError("already Logged in");
                            multipleLoginPanal.SetActive(true);
                            //    

                        }
                    }
                }
                else
                {
                     if (request1.isNetworkError)
                    {
                        Debug.LogError("Network error in Verify signature");
                    }
                    else
                    {
                        if (request1.error != null)
                        {
                            if (!VerifySignatureReadObj.success)
                            {
                                Debug.LogError("Success false in  verify sig");
                            }
                        }
                    }
                }
                request1.Dispose();

            }
            else
            {
                 LoaderObj.SetActive(false);
             }
        }

        public void Reload()
        {
            multipleLoginPanal.SetActive(false);

            SceneManager.LoadScene("Main");
        }

        public IEnumerator SetToken()

        {

            WWWForm wWWForm = new WWWForm();
            string abc = SystemInfo.deviceUniqueIdentifier;
            print(abc);

            wWWForm.AddField("deviceToken", abc);

            using (UnityWebRequest www = UnityWebRequest.Post(SetDeviceTokenAPI, wWWForm))
            {
                www.SetRequestHeader("Authorization", PlayerPrefs.GetString("LoginToken"));
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                    //  Settoken = true;
                }
                else
                {
                    // Settoken = true;
                    string resultdata = www.downloadHandler.text;
                    Debug.Log("Device Token Updated here " + resultdata);
                    PlayerPrefs.SetString("myDeviceToken", abc);
                    PlayerPrefs.Save();

                }
            }
        }






        // Irfan Script // 
        [Obsolete]
        public async Task<UnityWebRequest> CallingXanaAPI(string url, string Jsondata)
        {
            print(Jsondata);
            var request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(Jsondata);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            await request.SendWebRequest();
            print("lastly");
            return request;
        }

        public async Task<UnityWebRequest> HitVerifySignatureAPI(string url, string Jsondata)
        {
            // print(Jsondata);
            var request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(Jsondata);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            // yield return request.SendWebRequest();
            await request.SendWebRequest();
            return request;
            print("Json data of Signed signature is   = " + request.downloadHandler.text);
            //    ConnectServerDataExtraction.VerifyReadSignedMsgFromServer VerifySignatureReadObj = JsonUtility.FromJson<ConnectServerDataExtraction.VerifyReadSignedMsgFromServer>(request.downloadHandler.text);

        }
        public void CancelWalletLogin()
        {
            print("Back cancel");
            //LogOut();
            print("Back cancel2");
            change();
        }
        async void change()
        {
            Disconnect();
            SceneManager.LoadScene("SplashScene");
        }

        // END here 



        // If the user cancels the connect Disconnect and start over
        public async void WalletConnect_OnDisconnectedEvent(WalletConnectUnitySession session)
        {
            // Debug.Log("WalletConnect_OnDisconnectedEvent");

            // Only run if we are not already disconnecting
            if (!AuthenticationKitState.Disconnecting.Equals(State))
            {
                Disconnect();
            }
        }

        // If something goes wrong Disconnect and start over
        public async void WalletConnect_OnConnectionFailedEvent(WalletConnectUnitySession session)
        {
            // Debug.Log("WalletConnect_OnConnectionFailedEvent");

            // Only run if we are not already disconnecting
            if (!AuthenticationKitState.Disconnecting.Equals(State))
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Disconnect Moralis and WalletConnect.
        /// </summary>
        public async void Disconnect()
        {
            State = AuthenticationKitState.Disconnecting;
            try
            {
                // Logout the Moralis User.
                await Moralis.LogOutAsync();
                await SceneManager.UnloadSceneAsync("MoralisScene");
             }  
            catch (Exception e)
            {
                // Send error to the log but not as an error as this is expected behavior from W.C.
                Debug.LogError($"Disconnect() failed. Error: {e.Message}");
            }

#if !UNITY_WEBGL
            try
            {
                // Close the WalletConnect Transport Session
                await _walletConnect.Session.Transport.Close();

                // Disconnect the WalletConnect session
                await _walletConnect.Session.Disconnect();
            }
            catch (Exception e)
            {
                //Reason for Aborted warning is unknown, but expected. 
                if (e.Message != "Aborted")
                {
                    // Send error to the log but not as an error as this is expected behavior from W.C.
                    Debug.LogWarning($"[WalletConnect] Error = {e.Message}");
                }
            }
#endif

            State = AuthenticationKitState.Disconnected;
        }

        //  Event Handlers --------------------------------
        private async void StateObservable_OnValueChanged(AuthenticationKitState value)
        {
            // Debug.Log("StateObservable_OnValueChanged " + value);
            // Order matters here.

            // 1. Broadcast
            OnStateChanged.Invoke(_stateObservable.Value);

            // 2. Step the state. Rarely.
            switch (_stateObservable.Value)
            {
                case AuthenticationKitState.WalletConnecting:

                    switch (AuthenticationKitPlatform)
                    {
                        case AuthenticationKitPlatform.Android:
                            var cancellationTokenSourceAndroid = new CancellationTokenSource();
                            cancellationTokenSourceAndroid.CancelAfterSlim(TimeSpan.FromSeconds(15));

                            try
                            {
                                // Connect to the WalletConnect server
                                WalletConnect_Connect();

                                // Check if WalletConnect is ready in 15 seconds or else disconnect and start over
                                await UniTask.WaitUntil(() => _walletConnect.Session.ReadyForUserPrompt || _walletConnect.Connected,
                                    PlayerLoopTiming.Update, cancellationTokenSourceAndroid.Token);

                                if (_walletConnect.Session.ReadyForUserPrompt)
                                {
                                    // Only works if a users has a app installed that handles "wc:" links
                                    _walletConnect.OpenDeepLink();
                                }

                                // TODO check if the app is paused with OnApplicationPause to see if the link working  
                            }
                            catch (OperationCanceledException ex)
                            {
                                if (ex.CancellationToken == cancellationTokenSourceAndroid.Token)
                                {
                                    // WalletConnect connection timeout so let's start over
                                    Disconnect();
                                }
                            }

                            break;
                        case AuthenticationKitPlatform.iOS:
                            var cancellationTokenSourceIOS = new CancellationTokenSource();
                            cancellationTokenSourceIOS.CancelAfterSlim(TimeSpan.FromSeconds(15));

                            try
                            {
                                // Connect to the WalletConnect server
                                WalletConnect_Connect();

                                // Check if WalletConnect is ready in 15 seconds or else disconnect and start over
                                await UniTask.WaitUntil(() => _walletConnect.Session.ReadyForUserPrompt || _walletConnect.Connected,
                                    PlayerLoopTiming.Update, cancellationTokenSourceIOS.Token);

                                // TODO check if the app is paused with OnApplicationPause to see if the link working  
                            }
                            catch (OperationCanceledException ex)
                            {
                                if (ex.CancellationToken == cancellationTokenSourceIOS.Token)
                                {
                                    // WalletConnect connection timeout so let's start over
                                    Disconnect();
                                }
                            }
                            break;
                        case AuthenticationKitPlatform.WalletConnect:
                            // Connect to the WalletConnect server
                            WalletConnect_Connect();

                            break;
                        case AuthenticationKitPlatform.WebGL:
                            if (!Application.isEditor)
                            {
                                await LoginWithWeb3();
                            }

                            break;
                        default:
                            SwitchDefaultException.Throw(AuthenticationKitPlatform);
                            break;
                    }

                    break;

                case AuthenticationKitState.WalletConnected:

                    switch (AuthenticationKitPlatform)
                    {
                        case AuthenticationKitPlatform.Android:
                        case AuthenticationKitPlatform.iOS:
                        case AuthenticationKitPlatform.WalletConnect:

                            // If the Wallet connection has been accepted first Setup Web3
                            await Moralis.SetupWeb3();

                            // If there is a Wallet connected and we got a session
                            // try to Sign and Login to Moralis or else Disconnect and start over
                            if (_walletConnect.Session != null)
                            {
                                if (_signAndLoginToMoralis)
                                {
                                    WalletConnect_SignAndLoginToMoralis(_walletConnect.Session);
                                }
                            }
                            else
                            {
                                Disconnect();
                            }

                            break;
                        case AuthenticationKitPlatform.WebGL:
                            // TODO Break up the LoginWithWeb3 method to a separate the connecting signing and logging 
                            break;
                        default:
                            SwitchDefaultException.Throw(AuthenticationKitPlatform);
                            break;
                    }

                    break;

                case AuthenticationKitState.MoralisLoggedIn:

                    // Invoke OnConnected event
                    OnConnected.Invoke();
                    break;

                case AuthenticationKitState.Disconnected:

                    // Reset the UI so its like BEFORE we ever authenticated
                    await InitializeAsync();

                    // Invoke OnDisconnected event
                    OnDisconnected.Invoke();
                    break;

                default:
                    // Switch default is ok here since not all known conditions are declared above
                    break;
            }
        }
    }
}