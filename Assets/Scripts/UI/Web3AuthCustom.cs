using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nethereum.Signer;
using UnityEngine.Networking;
using System.Security.Principal;
using static WalletLogin;
using static System.Net.WebRequestMethods;
using UnityEngine.UI;

public class Web3AuthCustom : MonoBehaviour
{
  
    [Header("Web3Auth Project settings")]
    public string redirectURIAndroid = "web3auth://com.nbi.xana/auth";
    public string redirectURIiOS = "web3auth://com.nbmetaverse.xana/auth";
    private string clientIdEmail,clientIdGoole,clientIdApple,clientIdLine,ClientId ;
    private string loginVerifier;
    private string loginSubVerifierEmail, loginSubVerifierGoole, loginSubVerifierApple, loginSubVerifierLine;
    private Web3Auth.Network network;
    private string domains, domainsLine;

    [Header("Refs")]
    [SerializeField] Web3Auth web3Auth;

    string console;
    String ExternalApitoCall;
    internal string Userresponsce;
    internal string mysignature1 , mysignature2;
    private string privateKey;
    private Web3UserInfo userInfo;
    bool isNewReg;
    internal string publicAdress;
    internal string msg1 ,msg2,currentLan;
    public List<Button> myButtons;
    public float cooldownTime;
    public static Web3AuthCustom Instance;
    public Action<string> onLoginAction;

    private void Awake()
    {
        Instance = this;
        //if(Instance==null)
        //{
        //    Instance = this;
        //    DontDestroyOnLoad(this);
        //}
        //else
        //{
        //    Destroy(this.gameObject);
        //}
    }

    private void Start()
    {
       if (APIBasepointManager.instance.IsXanaLive) {
            //For Mainnet
            ClientId = "BPnWnv68o43X4uLNUNrBEWgu6GgletwK5bOU4SLpHFrKrkATivj36lOX3B1DE7u3qeFTksKqK30arrFLYAzAgGY";
            network = Web3Auth.Network.SAPPHIRE_MAINNET;
            loginVerifier = "social-aggregate-verifier";
            //...verifierSubIdentifier for testnet
            loginSubVerifierEmail = "ppp-passwordless-login";
            loginSubVerifierGoole = "ppp-google-login";
            loginSubVerifierApple = "ppp-apple-login";
           // loginSubVerifierLine = "ppp-line-login";
            //...
            clientIdEmail = "fr46GR3TzfOJFNvgEcIcQKLtLi48cm3c";
            clientIdGoole = "1041808867611-576ma9t6bva7b94irmvbt88n02tvoujn.apps.googleusercontent.com";
            clientIdApple = "bJgmFBdg8eSa2gAzh1yv3ABinO9NIq1z";
            // clientIdLine = "Y0EkN53ZYHQmE3BTlv3ylvKAg5dt38CP";
            //...
            domains = "https://dev-i7bsu7bon4og1n64.us.auth0.com";
            // domainsLine = "https://dev-px4cfed8eh5nu1bn.jp.auth0.com";
            ExternalApitoCall = ConstantsGod.xanaliaProductionAPI;
        }
        else {
            //For Testnet
            ClientId = "BMwTnf6I4qw7qwOWP1J1BsgHKEZDGG0peo-DpCMBmurc1RUSY16Ag8LdC4on55hLiStTQxm0FJ2wOuIZU2m9gr0";
            network = Web3Auth.Network.TESTNET;
            loginVerifier = "ppp-social-login-2";
            //...verifierSubIdentifier for testnet
            loginSubVerifierEmail = "ppp-passwordless-login";
            loginSubVerifierGoole= "ppp-google-login";
            loginSubVerifierApple = "ppp-apple-login";
            clientIdEmail = "kV31v4CokK8xEHgNcHki1nAVDCh3Friu";
            clientIdGoole = "792163717588-h9t0is3ng39opqmt1meflma087ov18k3.apps.googleusercontent.com";
            clientIdApple = "QRQW2fY3167OZTzreWBqHTBQU7gGXUD0";
            //clientIdLine = "Y0EkN53ZYHQmE3BTlv3ylvKAg5dt38CP";
            //...
            domains = "https://dev-px4cfed8eh5nu1bn.jp.auth0.com";
            //  domainsLine = "https://dev-px4cfed8eh5nu1bn.jp.auth0.com";
            ExternalApitoCall = ConstantsGod.xanaliaTestAPI;
        }

        var EmailPasswordlessConfigItem = new LoginConfigItem()
        {
            verifier = loginVerifier,
            verifierSubIdentifier = loginSubVerifierEmail,
            clientId = clientIdEmail,
            typeOfLogin = TypeOfLogin.JWT,
        };
        var GoogleConfig = new LoginConfigItem()
        {
            verifier = loginVerifier,
            verifierSubIdentifier = loginSubVerifierGoole,
            clientId = clientIdGoole,
            typeOfLogin = TypeOfLogin.GOOGLE,
        };
        var AppleConfigItem = new LoginConfigItem()
        {
            verifier = loginVerifier,
            verifierSubIdentifier = loginSubVerifierApple,
            clientId = clientIdApple,
            typeOfLogin = TypeOfLogin.APPLE,
        };

        web3Auth.setOptions(new Web3AuthOptions()
        {
            clientId = ClientId,
#if UNITY_IOS
            redirectUrl = new Uri(redirectURIiOS),
#endif
#if UNITY_ANDROID
   redirectUrl = new Uri(redirectURIAndroid),
#endif

            network = network,
            loginConfig = new Dictionary<string, LoginConfigItem>
            {
                 { "google", GoogleConfig },
                { "jwt", EmailPasswordlessConfigItem },
                { "apple", AppleConfigItem },
            }
        });
        web3Auth.onLogin += onLogin;
        web3Auth.onLogout += onLogout;
        updateConsole("Ready to Login!");
        detectsystemLanguage();
    }

    public void PasswordLessEmailLogin(bool isnewreg)
    {
        var selectedProvider = Provider.JWT;
        isNewReg = isnewreg;
        var options = new LoginParams()
        {
            mfaLevel = MFALevel.NONE,
            loginProvider = selectedProvider,
            
            extraLoginOptions = new ExtraLoginOptions()
            {
                domain = domains,
                verifierIdField = "email",
                isVerifierIdCaseSensitive = false,
                ui_locales=currentLan,
                prompt = Prompt.LOGIN,
            }

        };
        foreach (Button button in myButtons)
        {
            button.interactable = false;
            StartCoroutine(EnableButtonAfterCooldown());
        }

        web3Auth.login(options);
    }
   
    
    public void GoogleLogin(bool isnewreg)
    {
        var selectedProvider = Provider.GOOGLE;
        isNewReg = isnewreg;

        var options = new LoginParams()
        {
            mfaLevel = MFALevel.NONE,
            loginProvider = selectedProvider,
           

        };
        foreach (Button button in myButtons)
        {
            button.interactable = false;
            StartCoroutine(EnableButtonAfterCooldown());
        }

        web3Auth.login(options);
    }

    public void AppleLogin(bool isnewreg)
    {
        var selectedProvider = Provider.APPLE;
        isNewReg = isnewreg;
        var options = new LoginParams()
        {
            loginProvider = selectedProvider,
            mfaLevel=MFALevel.NONE,
            extraLoginOptions = new ExtraLoginOptions()
            {
                domain = domains,
                verifierIdField = "email",
                isVerifierIdCaseSensitive = false,
                connection = "apple",
                prompt = Prompt.LOGIN,
            }
        };
        foreach (Button button in myButtons)
        {
            button.interactable = false;
            StartCoroutine(EnableButtonAfterCooldown());
        }

        web3Auth.login(options);
    }

    private void onLogin(Web3AuthResponse response)
    {
#if UNITY_IOS

        if (PlayerPrefs.GetInt("PlayerLoginFlag") == 1)
            PlayerPrefs.SetInt("FirstTimeappOpen", 1);

        if (PlayerPrefs.GetInt("PlayerLoginFlag") == 0)
            return;

#endif
        GlobalConstants.SendFirebaseEvent(GlobalConstants.FirebaseTrigger.Signup_Wallet_Completed.ToString());
        UserLoginSignupManager.instance.StartCoroutine(UserLoginSignupManager.instance.LoginGuest(ConstantsGod.API_BASEURL + ConstantsGod.guestAPI, true));
        UserLoginSignupManager.instance.LoggedInAsGuest = false;
        Debug.Log(JsonConvert.SerializeObject(response, Formatting.Indented));
        LoadingHandler.Instance.nftLoadingScreen.SetActive(true);
        userInfo = response.userInfo;
        privateKey = response.privKey;
        PlayerPrefs.SetString("LoggedInMail", response.userInfo.email);
        onLoginAction?.Invoke(userInfo.email);
        publicAdress = EthECKey.GetPublicAddress(privateKey);
        GetSignature();
        updateConsole(JsonConvert.SerializeObject(response, Formatting.Indented));
        StartCoroutine(LoginExternalWallet());
        Web3AuthSociallogin type = Web3AuthSociallogin.None;
        try
        {
            if (isNewReg)
            {
                type = Web3AuthSociallogin.NewRegistration;

            }
            else
            {
                type = Web3AuthSociallogin.Login;

            }
            PlayerPrefs.SetString("publicID", publicAdress);

            switch (type)
            {


                case Web3AuthSociallogin.Login:
                    UserLoginSignupManager.instance.emailOrWalletLoginPanel.SetActive(false);
                    ConnectWallet.instance.StartCoroutine(ConnectWallet.instance.SaveChainSafeNonce(mysignature1, publicAdress, msg1));

                    break;


                case Web3AuthSociallogin.NewRegistration:
                    UserLoginSignupManager.instance.signUpPanel.SetActive(false);
                    ConnectWallet.instance.StartCoroutine(ConnectWallet.instance.SaveChainSafeNonce(mysignature1, publicAdress, msg1));

                    break;

                default:
                    break;
            }
            PlayerPrefs.Save();
        }
        catch (Exception ex)
        {
            LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
            UserLoginSignupManager.instance.emailOrWalletLoginPanel.SetActive(true);

        }

    }
    private void OnDestroy()
    {
        Debug.Log("Logged out!");
        logout();
    }

    public void logout()
    {
        web3Auth.logout();
        
    }

    private void onLogout()
    {
        privateKey = null;
        userInfo = null;
        
        Debug.Log("Logged out!");
        updateConsole("Logged out!");
    }
    IEnumerator EnableButtonAfterCooldown()
    {
        yield return new WaitForSeconds(cooldownTime); // Wait for 10 seconds
        foreach (Button button in myButtons)
        {
            button.interactable = true;
        } // Re-enable button interaction
    }
    string GetSignature()
    {
        // get current timestamp
        int timestamp = (int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
        // set expiration time
        int expirationTime = timestamp + 60;
        // set message
        msg1 = expirationTime.ToString();
        msg2 = "Welcome. By signing this message you are verifying your digital identity. This is completely secure and does not cost anything!";
        var signer = new EthereumMessageSigner();
        var signature1 = signer.EncodeUTF8AndSign(msg1, new EthECKey(privateKey));
        var signature2 = signer.EncodeUTF8AndSign(msg2, new EthECKey(privateKey));
        mysignature1= signature1;
        mysignature2 = signature2;
        updateConsole("Signature 1" + signature1.ToString());
        updateConsole("Signature 2" + signature2.ToString());
        return signature1;
    }

    public void updateConsole(string message)
    {
        console = $"{console}\n{message}";
    }
    public void detectsystemLanguage() {
        string newLanguage = Application.systemLanguage.ToString();
        if (newLanguage == "English")
        {
            currentLan= "en";
        }
        else if (newLanguage == "Japanese")
        {
            currentLan= "ja";
        }

    }
   
    public enum Web3AuthSociallogin
    {
        None,
        NewRegistration,
        Login
    }
    #region API Call
    public IEnumerator LoginExternalWallet()
    {
        ExternalWalletData dataToSend = new ExternalWalletData();
        dataToSend.signature = mysignature2;
        dataToSend.address = publicAdress;
        dataToSend.email = userInfo.email;
        dataToSend.is_web3_auth = 1;
        dataToSend.deviceId = SystemInfo.deviceUniqueIdentifier.ToLower();
        
        string jsonData = JsonUtility.ToJson(dataToSend);

        using (UnityWebRequest www = UnityWebRequest.Post(ExternalApitoCall + ConstantsGod.loginExternalWalletURL, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
             yield return www.SendWebRequest();

            Debug.Log("My LoginExternalWallet response: " + www.downloadHandler.text);
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("....Sucess Posted....");
            }
        }
    }

    [Serializable]
    public class ExternalWalletData
    {
        public string signature;
        public string address;
        public string email;
        public int is_web3_auth;
        public string builderLoginHash;
        public string deviceId;
        public string type;
    }
    #endregion

}
