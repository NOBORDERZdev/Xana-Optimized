using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdvancedInputFieldPlugin;
using TMPro;
using System.Linq;
using Sign_Up_Scripts;
using System;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using System.Text;

public class UserLoginSignupManager : MonoBehaviour
{
    [Header("Terms and Condition")]
    public GameObject termsConditionPanel;

    [Header("User Signup Section")]
    public GameObject signupOrLoginPanel;
    public GameObject signUpPanel;

    [Space(100)]
    public GameObject signUpWithEmailPanel;
    public AdvancedInputField emailField;
    public AdvancedInputField passwordField;
    public AdvancedInputField repeatPasswordField;
    public GameObject signupLoader;

    [Space(100)]
    public GameObject verficationCodePanel;
    public GameObject verficationPlaceHolder;
    public GameObject nextButton;
    public GameObject sendAgainTimer;
    public GameObject sendAgainButton;

    [Space(100)]
    public GameObject avatarSelectionPanel;
    public GameObject presetsScrollview;
    public GameObject aiMagicSelfieButton;
    public GameObject avatarSelectionNextButton;

    [Space(100)]
    public GameObject enterNamePanel;
    public GameObject enterNameField;
    public GameObject nameScreenNextButton;

    [Header("Validation Popup Panel")]
    public ErrorHandler errorHandler;
    public GameObject validationPopupPanel;
    public TextMeshProUGUI errorTextMsg;


    //Scripts References 
    [Header("Scripts References")]
    public Web3APIforWeb2 _web3APIforWeb2;

    #region SignUp Functions 

    public void OnClickSignUp()
    {
        signupOrLoginPanel.SetActive(false);
        signUpPanel.SetActive(true);
    }

    public void SignUpWithEmail()
    {
        signUpPanel.SetActive(false);
        signUpWithEmailPanel.SetActive(true);
    }


    public bool CheckForValidations(string emailText, string passwordText)
    {
        string L_LoginEmail = emailText;
        string L_loginPassword = passwordText;

        if (L_LoginEmail == "" || L_loginPassword == "")
        {
            validationPopupPanel.SetActive(true);
            errorTextMsg.color = new Color(0.44f, 0.44f, 0.44f, 1f);
            errorHandler.ShowErrorMessage(ErrorType.Fields__empty.ToString(), errorTextMsg);
            return false;
        }
        else if (L_LoginEmail.Contains(" "))
        {
            validationPopupPanel.SetActive(true);
            errorTextMsg.color = new Color(0.44f, 0.44f, 0.44f, 1f);
            errorHandler.ShowErrorMessage(ErrorType.Please_enter_valid_email.ToString(), errorTextMsg);
            return false;
        }
        if (IsValidEmail(L_LoginEmail))
        {
            return true;
        }
        else if (!L_LoginEmail.Contains("+") && L_LoginEmail.Any(char.IsLetter))
        {
            validationPopupPanel.SetActive(true);
            errorTextMsg.color = new Color(0.44f, 0.44f, 0.44f, 1f);
            errorHandler.ShowErrorMessage(ErrorType.Please_enter_valid_email.ToString(), errorTextMsg);
            return false;
        }

        return true;
    }


    bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }


    public const string motif = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
    public static bool IsPhoneNbr(string number)
    {
        if (number != null) return Regex.IsMatch(number, motif);
        else return false;
    }


    string uniqueID()
    {
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



    void SubmitCredentialsForSignUp()
    {
        string _LoginEmail = emailField.Text;
        string _loginPassword = passwordField.Text;

        _LoginEmail = _LoginEmail.Trim();
        _loginPassword = _loginPassword.Trim();
        _LoginEmail = _LoginEmail.ToLower();

        if (CheckForValidations(_LoginEmail, _loginPassword))
        {
            string bodyJson;
            MyClassOfLoginJson myObject = new MyClassOfLoginJson();

            if (IsPhoneNbr(_LoginEmail))
                bodyJson = JsonUtility.ToJson(myObject.GetdataFromClass("", _LoginEmail, _loginPassword));
            else
                bodyJson = JsonUtility.ToJson(myObject.GetdataFromClass(_LoginEmail, "", _loginPassword, uniqueID()));

            string url = ConstantsGod.API_BASEURL + ConstantsGod.LoginAPIURL;
            StartCoroutine(LoginUser(url, bodyJson, signupLoader));
        }

    }

    IEnumerator LoginUser(string url, string Jsondata, GameObject _loader = null, bool AutoLoginBool = false)
    {
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(Jsondata);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        ClassWithToken myObject1 = new ClassWithToken();
        myObject1 = ClassWithToken.CreateFromJSON(request.downloadHandler.text);

        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            if (myObject1.success)
            {
                if (_loader != null)
                    _loader.SetActive(false);

                XanaConstants.xanaliaToken = myObject1.data.xanaliaToken;
                XanaConstants.xanaToken = myObject1.data.token;
                XanaConstants.isAdmin = myObject1.data.isAdmin;
                XanaConstants.isGuestLogin = false;
                XanaConstants.xanaConstants.LoginasGustprofile = true;
                XanaConstants.userId = myObject1.data.user.id.ToString();
                XanaConstants.userName = myObject1.data.user.name;
                ConstantsGod.AUTH_TOKEN = myObject1.data.token;



                PlayerPrefs.SetString("UserNameAndPassword", Jsondata);
                PlayerPrefs.SetString("TermsConditionAgreement", "Agree");
                PlayerPrefs.SetInt("shownWelcome", 1);
                PlayerPrefs.SetInt("firstTime", 1);
                PlayerPrefs.SetInt("WalletLogin", 0);
                PlayerPrefs.SetString("LoginTokenxanalia", myObject1.data.xanaliaToken);
                PlayerPrefs.SetString("publicID", myObject1.data.user.walletAddress);
                PlayerPrefs.SetString("UserName", myObject1.data.user.id);
                PlayerPrefs.SetInt("IsLoggedIn", 1);
                PlayerPrefs.SetInt("FristPresetSet", 1);
                PlayerPrefs.SetString("PlayerName", myObject1.data.user.name);
                PlayerPrefs.SetString("LoggedInMail", myObject1.data.user.email);
                PlayerPrefs.Save();

                PremiumUsersDetails.Instance.GetGroupDetails("freeuser");
                PremiumUsersDetails.Instance.GetGroupDetailsForComingSoon();

                GetOwnedNFTsFromAPI();
                SubmitSetDeviceToken();
                GetUserClothData();
                CheckCameraMan(myObject1.data.user.email);

                DynamicEventManager.deepLink?.Invoke("Login user here");
                XanaConstants.OnSucessFullLogin?.Invoke();
            }
        }
        else
        {
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                validationPopupPanel.SetActive(true);
                errorTextMsg.color = new Color(0.44f, 0.44f, 0.44f, 1f);
                errorHandler.ShowErrorMessage(ErrorType.Poor_Connection.ToString(), errorTextMsg);
                if (_loader != null)
                    _loader.SetActive(false);
            }
            else
            {
                if (request.error != null)
                {
                    if (!myObject1.success)
                    {
                        validationPopupPanel.SetActive(true);
                        errorTextMsg.color = new Color(0.44f, 0.44f, 0.44f, 1f);
                        if (myObject1.msg.Contains("You are already logged in another device"))
                        {
                            if (AutoLoginBool)
                            {
                                PlayerPrefs.DeleteAll();
                                yield return null;
                            }
                            else
                            {
                               /* PlayerPrefs.SetString("JSONdataforlogin", Jsondata);
                                LogoutfromOtherDevicePanel.SetActive(true);
                                MyClassOfLoginJson myObject = new MyClassOfLoginJson();
                                myObject = myObject.CreateFromJSON(Jsondata);
                                MyClassOfLogoutDevice logoutObj = new MyClassOfLogoutDevice();
                                string bodyJson2 = JsonUtility.ToJson(logoutObj.GetdataFromClass(myObject.email, myObject.phoneNumber, myObject.password));
                                PlayerPrefs.SetString("LogoutFromDeviceJSON", bodyJson2);*/
                            }
                        }

                        errorHandler.ShowErrorMessage(myObject1.msg, errorTextMsg);

                        if (_loader != null)
                            _loader.SetActive(false);
                    }
                }
            }
        }

        request.Dispose();
    }

    void CheckCameraMan(string email)
    {
        if (email.Contains("xanacameraman@yopmail.com"))
            XanaConstants.xanaConstants.isCameraMan = true;
        else
            XanaConstants.xanaConstants.isCameraMan = false;
    }

    public void GetOwnedNFTsFromAPI()
    {
        _web3APIforWeb2.GetWeb2UserData(PlayerPrefs.GetString("publicID"));
    }

    public void SubmitSetDeviceToken()
    {
        string l_DeivceID = uniqueID();
        MyClassForSettingDeviceToken myObject = new MyClassForSettingDeviceToken();
        string bodyJson = JsonUtility.ToJson(myObject.GetUpdatedDeviceToken(l_DeivceID)); ;
        StartCoroutine(HitSetDeviceTokenAPI(ConstantsGod.API_BASEURL + ConstantsGod.SetDeviceTokenAPI, bodyJson, l_DeivceID));
    }

    IEnumerator HitSetDeviceTokenAPI(string url, string Jsondata, string LocalGetDeviceID)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(Jsondata);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            MyClassNewApi myObject1 = new MyClassNewApi();
            myObject1 = myObject1.Load(request.downloadHandler.text);
            if (myObject1.success)
            {
                PlayerPrefs.SetString("DeviceToken", LocalGetDeviceID);
            }
        }

        request.Dispose();
    }

    void GetUserClothData()
    {
        ServerSIdeCharacterHandling.Instance.GetDataFromServer();
    }

    #endregion





    #region ClassStructure for Login and Signup


    [Serializable]
    public class MyClassOfLoginJson
    {
        public string email;
        public string phoneNumber;
        public string password;
        public string deviceId;
        public MyClassOfLoginJson GetdataFromClass(string L_eml, string L_phonenbr, string passwrd, string _Deviceid = "")
        {
            MyClassOfLoginJson myObj = new MyClassOfLoginJson();
            myObj.email = L_eml;
            myObj.phoneNumber = L_phonenbr;
            myObj.password = passwrd;
            myObj.deviceId = _Deviceid;
            return myObj;
        }
        public MyClassOfLoginJson CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<MyClassOfLoginJson>(jsonString);
        }
    }


    [System.Serializable]
    public class ClassWithToken
    {
        public static ClassWithToken _UserData;
        public bool success;
        public JustToken data;
        public string msg;
        public static ClassWithToken CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<ClassWithToken>(jsonString);
        }
    }

    [System.Serializable]
    public class JustToken
    {
        public string token;
        public string encryptedId;
        public string xanaliaToken;
        public UserData user;
        public bool isAdmin;
        public static JustToken CreateFromJSON(string jsonString)
        {
            //print("Person " + jsonString);
            return JsonUtility.FromJson<JustToken>(jsonString);
        }
    }

    [System.Serializable]
    public class UserData
    {
        public string id;
        public string name;
        public string email;
        public string phoneNumber;
        public string coins;
        public string walletAddress;
    }


    [Serializable]
    public class MyClassForSettingDeviceToken
    {
        public string deviceToken;
        public MyClassForSettingDeviceToken GetUpdatedDeviceToken(string L_deviceTkn)
        {
            MyClassForSettingDeviceToken myObj = new MyClassForSettingDeviceToken();
            myObj.deviceToken = L_deviceTkn;
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
    #endregion
}
