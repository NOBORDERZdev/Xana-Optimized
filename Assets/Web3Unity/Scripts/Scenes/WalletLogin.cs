using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WalletLogin: MonoBehaviour
{
    ProjectConfigScriptableObject projectConfigSO = null;
    //public Toggle rememberMe;
    //UserRegisterationManager registerationManager;
    [SerializeField] ConnectWallet connectingWallet;
    //[SerializeField] GameObject SuccessfulPopUp;
    
    void Start() {
       // registerationManager = UserRegisterationManager.instance;
     SetChainSafeInfo();
        // if remember me is checked, set the account to the saved account
        if(PlayerPrefs.HasKey("RememberMe") && PlayerPrefs.HasKey("publicID"))
        {
            if (PlayerPrefs.GetInt("RememberMe") == 1 && PlayerPrefs.GetString("publicID") != "")
            {
                if (!ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
                {
                    LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
                }
                else
                {
                    LoadingHandler.Instance.LoadingScreenSummit.SetActive(false);
                }
                // move to next scene
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }


    void SetChainSafeInfo() {
        if (APIBasepointManager.instance.IsXanaLive)
        {
            projectConfigSO = (ProjectConfigScriptableObject)Resources.Load("ProjectConfigDataMainNet", typeof(ScriptableObject));
        }
        else
        {
            projectConfigSO = (ProjectConfigScriptableObject)Resources.Load("ProjectConfigData", typeof(ScriptableObject));
        }
        PlayerPrefs.SetString("ProjectID", projectConfigSO.ProjectId);
        PlayerPrefs.SetString("ChainID", projectConfigSO.ChainId);
        PlayerPrefs.SetString("Chain", projectConfigSO.Chain);
        PlayerPrefs.SetString("Network", projectConfigSO.Network);
        PlayerPrefs.SetString("RPC", projectConfigSO.Rpc);    
    }
    async public void OnLogin(bool isNewReg)
    {
        ConstantsHolder.xanaConstants.isWalletLoadingbool = true;
        SetChainSafeInfo();
        WalletConnectCallType type = WalletConnectCallType.None;
        try
        {
            if (!ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
            {
                LoadingHandler.Instance.nftLoadingScreen.SetActive(true);
            }
            else
            {
                LoadingHandler.Instance.LoadingScreenSummit.SetActive(true);
            }
            if (isNewReg)
            {
                type = WalletConnectCallType.NewRegistration;
            }
            else
            {
                type = WalletConnectCallType.Login;
            }

            // get current timestamp
            int timestamp = (int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
            // set expiration time
            int expirationTime = timestamp + 60;
            // set message
            string message = expirationTime.ToString();
             string signature;
            string account;
            // sign message
             signature = await Web3Wallet.Sign(message);
             account = await EVM.Verify(message, signature);

            // verify account

            
            int now = (int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
            // validate
            if (account.Length == 42 && expirationTime >= now) {
                // save account
                PlayerPrefs.SetString("publicID", account);
                    PlayerPrefs.SetInt("RememberMe", 1);
                //if (rememberMe.isOn)
                //else
                //    PlayerPrefs.SetInt("RememberMe", 0);
                // load next scene
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                switch (type)
                {
                    case WalletConnectCallType.NewRegistration:
                        //registerationManager.LoaderBool = false;
                        // PlayerPrefs.SetInt("WalletConnect", 1);
                        connectingWallet.isWalletNewReg= true;
                        // registerationManager.LoginWithWallet();
                        connectingWallet.StartCoroutine(connectingWallet.SaveChainSafeNonce(signature,account,message));
                        //Invoke(nameof(OpenNamePanel),1f);
                        // SetNameInServer();

                        break;
                    case WalletConnectCallType.Login:
                        connectingWallet.isWalletNewReg= false;
                        connectingWallet.StartCoroutine(connectingWallet.SaveChainSafeNonce(signature,account,message));
                        break;
                    default:
                        break;
                }
                PlayerPrefs.Save();
            }
        }
        catch (Exception e)
        {
            if (!ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
            {
                LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
            }
            else
            {
                LoadingHandler.Instance.LoadingScreenSummit.SetActive(false);
            }
            UserLoginSignupManager.instance.emailOrWalletLoginPanel.SetActive(true);
            throw new System.Exception("Unbale to Connect wallet on wallet connect btn at "+type+" state  with exception : "+e );
        }

    }



    // [System.Serializable]
    //public class JsonObjectBase
    //{
    //}
    //[Serializable]
    //public class MyClassOfPostingName : JsonObjectBase
    //{
    //    public string name;
    //    public MyClassOfPostingName GetNamedata(string name)
    //    {
    //        MyClassOfPostingName myObj = new MyClassOfPostingName();
    //        myObj.name = name;
    //        return myObj;
    //    }
    //}
    // //void SetNameInServer()
    // //{
    // //   MyClassOfPostingName myObject = new MyClassOfPostingName();
    // //   string bodyJsonOfName = JsonUtility.ToJson(myObject.GetNamedata(PlayerPrefs.GetString("Useridxanalia")));

    // //   StartCoroutine(HitNameAPIWithNewTechnique(ConstantsGod.API_BASEURL+ConstantsGod.NameAPIURL, bodyJsonOfName, PlayerPrefs.GetString("Useridxanalia")));
    // //}
    // // IEnumerator HitNameAPIWithNewTechnique(string url, string Jsondata, string localUsername)
    // // {
    // //   print("Body " + Jsondata);
    // //   var request = new UnityWebRequest(url, "POST");
    // //   byte[] bodyRaw = Encoding.UTF8.GetBytes(Jsondata);
    // //   request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
    // //   request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    // //   request.SetRequestHeader("Content-Type", "application/json");
    // //   request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
    // //   yield return request.SendWebRequest();
    // //   Debug.Log(request.downloadHandler.text);
    // //   MyClassNewApi myObject1 = new MyClassNewApi();
    // //   if (!request.isHttpError && !request.isNetworkError)
    // //   {
    // //       myObject1 = CheckResponceJsonNewApi(request.downloadHandler.text);
    // //       if (request.error == null)
    // //       {
    // //           Debug.Log(request.downloadHandler.text);
    // //            if (myObject1.success)
    // //           {
    // //               print("Success in name  field ");
    // //               PlayerPrefs.SetInt("IsLoggedIn", 1);
    // //               PlayerPrefs.SetInt("FristPresetSet", 1);
    // //               ServerSideUserDataHandler.Instance.GetDataFromServer();  
    // //               PlayerPrefs.SetString("PlayerName", localUsername);
    // //               if (GameManager.Instance.UiManager != null)//rik  
    // //               {
    // //                   GameManager.Instance.UiManager._footerCan.transform.GetChild(0).GetComponent<HomeFooterHandler>().HomeSceneFooterSNSButtonIntrectableTrueFalse();
    // //               }
    // //            }
    // //       }
    // //   }
    // //   else
    // //   {
    // //       if (request.isNetworkError)
    // //       {
                
    // //       }
    // //       else
    // //       {
    // //           if (request.error != null)
    // //           {
                    
    // //            }
    // //       }
    // //   }
    // // }

    //[Serializable]
    //public class MyClassNewApi
    //{
    //    public MyClassNewApi myObject;
    //    public bool success;
    //    public string msg;
    //    public string data;
    //    public MyClassNewApi Load(string savedData)
    //    {
    //        myObject = new MyClassNewApi();
    //        print("savedData " + savedData);
    //        myObject = JsonUtility.FromJson<MyClassNewApi>(savedData);
    //        return myObject;
    //    }
    //}
    //MyClassNewApi CheckResponceJsonNewApi(string Localdata)
    //{
    //    MyClassNewApi myObject = new MyClassNewApi();
    //    myObject = myObject.Load(Localdata);
    //    print("myObject " + myObject.data);
    //    return myObject;
    //}
    //[System.Serializable]
    public enum WalletConnectCallType{ 
        None,
        NewRegistration,
        Login
    }
}
