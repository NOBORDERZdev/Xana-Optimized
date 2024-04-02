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

public class Web3AuthCustom : Singleton<Web3AuthCustom>
{
  
    [Header("Web3Auth Project settings")]
    private string redirectURI = "web3auth://com.nbi.xana/auth";
    private string ClientId;
    private string loginVerifier;
    private string loginSubVerifier;
    private string passwordLessClientId;
    private Web3Auth.Network network;

    [Header("Refs")]
    [SerializeField] Web3Auth web3Auth;

    string console;
    internal string Userresponsce;
    internal string mysignature1 , mysignature2;
    private string privateKey;
    private Web3UserInfo userInfo;
    bool isNewReg;
    internal string publicAdress;
    internal string msg1 ,msg2;
    

    private void Start()
    {  
        //For Testnet
        ClientId = "BMwTnf6I4qw7qwOWP1J1BsgHKEZDGG0peo-DpCMBmurc1RUSY16Ag8LdC4on55hLiStTQxm0FJ2wOuIZU2m9gr0";
        network = Web3Auth.Network.TESTNET;
       
      
        var EmailPasswordlessConfigItem = new LoginConfigItem()
        {
            verifier = "ppp-social-login-2",
            verifierSubIdentifier = "ppp-passwordless-login",
            clientId = "kV31v4CokK8xEHgNcHki1nAVDCh3Friu",
            typeOfLogin = TypeOfLogin.JWT,
        };
        var GoogleConfig = new LoginConfigItem()
        {
            verifier = "ppp-social-login-2",
            verifierSubIdentifier = "ppp-google-login",
            clientId = "792163717588-h9t0is3ng39opqmt1meflma087ov18k3.apps.googleusercontent.com",
            typeOfLogin = TypeOfLogin.GOOGLE,
        };
        var AppleConfigItem = new LoginConfigItem()
        {
            verifier = "ppp-social-login-2",
            verifierSubIdentifier = "ppp-apple-login",
            clientId = "QRQW2fY3167OZTzreWBqHTBQU7gGXUD0",
            typeOfLogin = TypeOfLogin.APPLE,
        };


        web3Auth.setOptions(new Web3AuthOptions()
        {
            clientId = ClientId,
            redirectUrl = new Uri(redirectURI),
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
    }

    public void PasswordLessEmailLogin(bool isnewreg)
    {
        var selectedProvider = Provider.JWT;
        isNewReg = isnewreg;
        var options = new LoginParams()
        {
            loginProvider = selectedProvider,
            extraLoginOptions = new ExtraLoginOptions()
            {
                domain = "https://dev-px4cfed8eh5nu1bn.jp.auth0.com",
                verifierIdField = "email",
                isVerifierIdCaseSensitive = false,
                prompt = Prompt.LOGIN,
            }
        };


        web3Auth.login(options);
    }
   
    
    public void GoogleLogin(bool isnewreg)
    {
        var selectedProvider = Provider.GOOGLE;
        isNewReg = isnewreg;
        var options = new LoginParams()
        {
            loginProvider = selectedProvider,
           
        };


        web3Auth.login(options);
    }

    public void AppleLogin(bool isnewreg)
    {
        var selectedProvider = Provider.APPLE;
        isNewReg = isnewreg;
        var options = new LoginParams()
        {
            loginProvider = selectedProvider,
            extraLoginOptions = new ExtraLoginOptions()
            {
                domain = "https://dev-px4cfed8eh5nu1bn.jp.auth0.com",
                verifierIdField = "email",
                isVerifierIdCaseSensitive = false,
                connection = "apple",
                prompt = Prompt.LOGIN,
            }
        };


        web3Auth.login(options);
    }
    private void onLogin(Web3AuthResponse response)
    {
        Debug.Log(JsonConvert.SerializeObject(response, Formatting.Indented));
        userInfo = response.userInfo;
        privateKey = response.privKey;
        publicAdress = EthECKey.GetPublicAddress(privateKey);
        GetSignature();
        updateConsole(JsonConvert.SerializeObject(response, Formatting.Indented));
        StartCoroutine(LoginExternalWallet());
        Web3AuthSociallogin type = Web3AuthSociallogin.None;
        try
        {
            LoadingHandler.Instance.nftLoadingScreen.SetActive(true);
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
                    ConnectingWallet.instance.StartCoroutine(ConnectingWallet.instance.SaveChainSafeNonce(mysignature1, publicAdress, msg1));

                    break;


                case Web3AuthSociallogin.NewRegistration:
                    UserLoginSignupManager.instance.signUpPanel.SetActive(false);
                    ConnectingWallet.instance.StartCoroutine(ConnectingWallet.instance.SaveChainSafeNonce(mysignature1, publicAdress, msg1));

                    break;

                default:
                    break;
            }
            PlayerPrefs.Save();
        }
        catch (Exception ex)
        {
            LoadingHandler.Instance.nftLoadingScreen.SetActive(false);

        }

    }
    private void OnDestroy()
    {
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

        using (UnityWebRequest www = UnityWebRequest.Post(ConstantsGod.xanaliaTestAPI + ConstantsGod.loginExternalWalletURL, "POST"))
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
