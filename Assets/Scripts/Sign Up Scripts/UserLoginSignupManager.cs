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
using UnityEngine.UI;
using System.IO;
using Photon.Pun.Demo.PunBasics;

public class UserLoginSignupManager : MonoBehaviour
{
    [Header("Terms and Condition")]
    public GameObject termsConditionPanel;

    [Header("User Signup Section")]
    public GameObject signUpOrloginSelectionPanel;
    public GameObject signUpPanel;

    [Header("User Login Section")]
    public GameObject emailOrWalletLoginPanel;
    public GameObject emailLoginPanel;

    [Space(10)]
    public GameObject signUpWithEmailPanel;
    public AdvancedInputField emailField;
    public AdvancedInputField passwordField;
    public AdvancedInputField repeatPasswordField;
    public GameObject signupLoader;
    private string emailForSignup;
    private string passwordForSignup;

    [Space(10)]
    public GameObject verficationCodePanel;
    public AdvancedInputField verficationPlaceHolder;
    public Button otpnextButton;
    public GameObject otpLoader;
    public GameObject sendAgainTimer;
    public Button sendAgainButton;
    public Text[] otpTexts;

    [Space(10)]
    public GameObject enterNamePanel;
    public AdvancedInputField userNameField;
    public Image selectedPresetImage;
    public RawImage aiPresetImage;
    public Button nameScreenNextButton;
    public GameObject nameScreenLoader;

    [Header("Validation Popup Panel")]
    public ErrorHandler errorHandler;
    public GameObject validationPopupPanel;
    public TextMeshProUGUI errorTextMsg;

    [Header("Login Fields")]
    public AdvancedInputField emailFieldLogin;
    public AdvancedInputField passwordFieldLogin;
    public GameObject loginLoader;
    public Button loginButton;

    //Scripts References 
    [Header("Scripts References")]
    public Web3APIforWeb2 _web3APIforWeb2;
    public ConnectingWallet connectingWalletRef;
    public userRoleScript userRoleScriptScriptableObj;

    public static UserLoginSignupManager instance;

    private void OnEnable()
    {
        instance = this;
        if (!File.Exists(GameManager.Instance.GetStringFolderPath()))
        {
            SavaCharacterProperties.instance.CreateFileFortheFirstTime();
        }
        verficationPlaceHolder.OnValueChanged.AddListener(delegate { ValueChangeCheck(); });
        Web3APIforWeb2.AllDataFetchedfromServer += Web3EventForNFTData;

        CheckForAutoLogin();
        EyesBlinking.instance.StoreBlendShapeValues();
        StartCoroutine(EyesBlinking.instance.BlinkingStartRoutine());
    }

    private void OnDisable()
    {
        verficationPlaceHolder.OnValueChanged.RemoveListener(delegate { ValueChangeCheck(); });
        Web3APIforWeb2.AllDataFetchedfromServer -= Web3EventForNFTData;
    }


    void CheckForAutoLogin()
    {
        // If already logged in than Return
        if (XanaConstants.loggedIn)
        {
            Debug.Log("Already Login Dont Call API");
            return;
        }
        Debug.Log("Auto Login");

        if (PlayerPrefs.GetInt("IsLoggedIn") == 1 && PlayerPrefs.GetInt("WalletLogin") != 1)
        {
            MyClassOfLoginJson LoginObj = new MyClassOfLoginJson();
            LoginObj = LoginObj.CreateFromJSON(PlayerPrefs.GetString("UserNameAndPassword"));
            StartCoroutine(LoginUser(ConstantsGod.API_BASEURL + ConstantsGod.LoginAPIURL, PlayerPrefs.GetString("UserNameAndPassword"), (isSucess) =>
            {
                //write if you want something on sucessfull login
            }));
        }
        else if (PlayerPrefs.GetInt("WalletLogin") == 1)
        {
            ConstantsGod.AUTH_TOKEN = PlayerPrefs.GetString("LoginToken");
            XanaConstants.xanaToken = PlayerPrefs.GetString("LoginToken");
            XanaConstants.isWalletLogin = true;
            StoreManager.instance.WalletLoggedinCall();
            WalletAutoLogin();
        }
        else
        {
            ShowWelcomeScreen();
        }
    }


    #region SignUp Functions 

    public void ShowWelcomeScreen()
    {
        signUpOrloginSelectionPanel.SetActive(true);
        ClearInputFieldsData();
    }

    public void OnClickSignUpSelection()
    {
        signUpOrloginSelectionPanel.SetActive(false);
        emailOrWalletLoginPanel.SetActive(false);
        emailLoginPanel.SetActive(false);
        signUpPanel.SetActive(true);
        ClearInputFieldsData();
    }

    public void BackFromSignUpSelection()
    {
        signUpOrloginSelectionPanel.SetActive(true);
        signUpPanel.SetActive(false);
    }

    public void OnClickSignUpWithEmail()
    {
        signUpPanel.SetActive(false);
        signUpWithEmailPanel.SetActive(true);
        ClearInputFieldsData();
    }

    public void BackFromSignUpEmail()
    {
        signUpPanel.SetActive(true);
        signUpWithEmailPanel.SetActive(false);
    }

    public void OnClickLoginSelection()
    {
        emailOrWalletLoginPanel.SetActive(true);
        signUpOrloginSelectionPanel.SetActive(false);
        signUpPanel.SetActive(false);
        signUpWithEmailPanel.SetActive(false);
        ClearInputFieldsData();
    }

    public void BackFromLoginSelection()
    {
        emailOrWalletLoginPanel.SetActive(false);
        signUpOrloginSelectionPanel.SetActive(true);
    }

    public void OnClickLoginWithEmail()
    {
        emailOrWalletLoginPanel.SetActive(false);
        emailLoginPanel.SetActive(true);
        ClearInputFieldsData();
    }

    public void BackFromLoginWithEmail()
    {
        emailOrWalletLoginPanel.SetActive(true);
        emailLoginPanel.SetActive(false);
    }

    public void BackFromOTPPanel()
    {
        verficationCodePanel.SetActive(false);
        signUpWithEmailPanel.SetActive(true);
        otpnextButton.interactable = true;
        otpLoader.SetActive(false);
    }

    public void ClickOnWalletSign()
    {
        emailOrWalletLoginPanel.SetActive(false);
        signUpPanel.SetActive(false);
    }


    //
    public void OpenUserNamePanel()
    {
        enterNamePanel.SetActive(true);
        userNameField.Clear();
    }
    public void BackFromUserNamePanel()
    {
        enterNamePanel.SetActive(false);
        userNameField.Clear();
        MainSceneEventHandler.OpenPresetPanel?.Invoke();
    }


    private void ClearInputFieldsData()
    {
        emailField.Clear();
        emailFieldLogin.Clear();
        passwordField.Clear();
        passwordFieldLogin.Clear();
        repeatPasswordField.Clear();
        userNameField.Clear();
    }

    //wallet login functions 
    public void WalletAutoLogin()
    {
        if (!XanaConstants.loggedIn)
        {
            //Debug.Log("Firebase: Wallet Login Event");
            GlobalConstants.SendFirebaseEvent(GlobalConstants.FirebaseTrigger.Login_Wallet_Success.ToString());
        }

        PlayerPrefs.SetInt("IsLoggedIn", 1);
        PlayerPrefs.SetInt("FristPresetSet", 1);
        PlayerPrefs.SetInt("FirstTime", 1);
        PlayerPrefs.SetInt("WalletLogin", 1);
        PlayerPrefs.SetInt("shownWelcome", 1);
        PlayerPrefs.Save();
        XanaConstants.loggedIn = true;
        XanaConstants.isWalletLogin = true;
        GetUserClothData();
        GetOwnedNFTsFromAPI();
        
        PremiumUsersDetails.Instance.GetGroupDetails("freeuser");
        PremiumUsersDetails.Instance.GetGroupDetailsForComingSoon();
        StartCoroutine(WaitForDeepLink());
        StartCoroutine(GameManager.Instance.mainCharacter.GetComponent<CharacterOnScreenNameHandler>().IERequestGetUserDetails());
        if (GameManager.Instance.UiManager != null)//rik
        {
            GameManager.Instance.bottomTabManagerInstance.HomeSceneFooterSNSButtonIntrectableTrueFalse();
            GameManager.Instance.bottomTabManagerInstance.CheckLoginOrNotForFooterButton();
        }
    }
    IEnumerator WaitForDeepLink()
    {
        yield return new WaitForSeconds(2);
        DynamicEventManager.deepLink?.Invoke("moralis wait and come");
    }

    IEnumerator WalletLoggedInAccessGroup(bool loadData = false)
    {
        if (userRoleScriptScriptableObj.userNftRoleSlist.Count > 0)
        {
            int x = (int)NftRolePriority.guest;
            string userNftRole = "free";
            ConstantsGod.UserRoles = userRoleScriptScriptableObj.userNftRoleSlist;
            foreach (string s in userRoleScriptScriptableObj.userNftRoleSlist)
            {
                int rolePriority = ReturnNftRole(s);
                if (rolePriority <= x)
                {
                    x = rolePriority;
                    ConstantsGod.UserPriorityRole = s;
                }
                userNftRole = s.ToLower();

                switch (userNftRole)
                {
                    case "alpha-pass":
                        {
                            PremiumUsersDetails.Instance.GetGroupDetails("Access Pass");
                            break;
                        }
                    case "premium":
                        {
                            PremiumUsersDetails.Instance.GetGroupDetails("Extra NFT");
                            break;
                        }
                    case "dj-event":
                        {
                            PremiumUsersDetails.Instance.GetGroupDetails("djevent");
                            break;
                        }
                    case "free":
                        {
                            PremiumUsersDetails.Instance.GetGroupDetails("freeuser");
                            break;
                        }
                    case "vip-pass":
                        {
                            PremiumUsersDetails.Instance.GetGroupDetails("vip-pass");
                            break;
                        }
                    case "astroboy":
                        {
                            PremiumUsersDetails.Instance.GetGroupDetails("astroboy");
                            break;
                        }
                }
            }
        }
        else
        {
            //print("you have no Premium Access ");
            PremiumUsersDetails.Instance.GetGroupDetails("freeuser");
        }
        PremiumUsersDetails.Instance.GetGroupDetailsForComingSoon();
        yield return null;
    }

    int ReturnNftRole(string role)
    {
        role = role.Replace('-', '_');
        switch (role)
        {
            case "dj_event":
                {
                    return ((int)NftRolePriority.dj_event);
                }
            case "alpha_pass":
                {
                    return ((int)NftRolePriority.alpha_pass);
                }
            case "premium":
                {
                    return ((int)NftRolePriority.premium);
                }
            case "free":
                {
                    return ((int)NftRolePriority.free);
                }
            case "guest":
                {
                    return ((int)NftRolePriority.guest);
                }
            case "vip-pass":
                {
                    return ((int)NftRolePriority.vip_pass);
                }
            case "astroboy":
                {
                    return ((int)NftRolePriority.Astroboy);
                }
        }
        return ((int)NftRolePriority.free);
    }

    private async void Web3EventForNFTData(string _userType)
    {
        if (_userType == "Web2")
        {
            if (_web3APIforWeb2._OwnedNFTDataObj.NFTlistdata.count > 0)
            {
                await _web3APIforWeb2._OwnedNFTDataObj.FillAllListAsyncWaiting();
                Debug.Log("<color=red> BoxerNFT: Wear NFT Funtionality Disabled </color>");
                {
                    //if (_web3APIforWeb2._OwnedNFTDataObj._NFTIDs.Contains(PlayerPrefs.GetInt("nftID")))
                    //{
                    //    if (PlayerPrefs.HasKey("Equiped"))
                    //    {
                    //        XanaConstants.xanaConstants.isNFTEquiped = true;
                    //        BoxerNFTEventManager.OnNFTequip?.Invoke(false);
                    //    }
                    //}
                    //else
                    //{
                    //    PlayerPrefs.DeleteKey("Equiped");
                    //    PlayerPrefs.DeleteKey("nftID");
                    //    XanaConstants.xanaConstants.isNFTEquiped = false;
                    //    BoxerNFTEventManager.OnNFTUnequip?.Invoke();
                    //    LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
                    //}
                }
            }
            else
            {
                LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
            }
        }
        else
        {
            ////Debug.Log("not Logged in");
        }
    }


    public void LoginWithWallet()
    {
        Debug.Log("Login With Wallet");
        if (!XanaConstants.loggedIn)
        {
            //Debug.Log("Firebase: Wallet Login Event");
            GlobalConstants.SendFirebaseEvent(GlobalConstants.FirebaseTrigger.Login_Wallet_Success.ToString());
        }

        PlayerPrefs.SetInt("IsLoggedIn", 1);
        PlayerPrefs.SetInt("FristPresetSet", 1);
        PlayerPrefs.SetInt("FirstTime", 1);
        PlayerPrefs.SetInt("WalletLogin", 1);
        PlayerPrefs.SetInt("shownWelcome", 1);
        PlayerPrefs.Save();
        XanaConstants.loggedIn = true;
        XanaConstants.isWalletLogin = true;
        SubmitSetDeviceToken();
        GetUserClothData();
        GetOwnedNFTsFromAPI();
        PremiumUsersDetails.Instance.GetGroupDetails("freeuser");
        PremiumUsersDetails.Instance.GetGroupDetailsForComingSoon();
        StartCoroutine(GameManager.Instance.mainCharacter.GetComponent<CharacterOnScreenNameHandler>().IERequestGetUserDetails());
        if (GameManager.Instance.UiManager != null)//rik
        {
            GameManager.Instance.bottomTabManagerInstance.HomeSceneFooterSNSButtonIntrectableTrueFalse();
            GameManager.Instance.bottomTabManagerInstance.CheckLoginOrNotForFooterButton();
        }
    }

    public void CheckForValidationAndSignUp(bool resendOtp = false)
    {
        string _email = emailField.Text;
        _email = _email.Trim();
        _email = _email.ToLower();

        bool ValidEmail = EmailValidation(_email, false);
        if (!ValidEmail)
            return;
        bool validPassword = PasswordValidation(passwordField.Text, repeatPasswordField.Text);
        if (ValidEmail && validPassword)
        {
            emailForSignup = _email;
            passwordForSignup = passwordField.Text;
            string url;
            if (resendOtp)
                url = ConstantsGod.API_BASEURL + ConstantsGod.ResendOTPAPI;
            else
                url = ConstantsGod.API_BASEURL + ConstantsGod.SendEmailOTP;
            MyClassOfPostingEmail myobjectOfEmail = new MyClassOfPostingEmail();
            string bodyJson = JsonUtility.ToJson(myobjectOfEmail.GetEmaildata(_email));
            StartCoroutine(VerficationOTPViaAPI(url, bodyJson, _email, signupLoader));
        }
    }

    bool EmailValidation(string emailText, bool checkForLogin)
    {
        string L_LoginEmail = emailText;
        if (L_LoginEmail == "")
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
        else if (checkForLogin && IsPhoneNbr(L_LoginEmail))
        {
            return true;
        }
        else
        {
            validationPopupPanel.SetActive(true);
            errorTextMsg.color = new Color(0.44f, 0.44f, 0.44f, 1f);
            errorHandler.ShowErrorMessage(ErrorType.Please_enter_valid_email.ToString(), errorTextMsg);
            return false;
        }
    }


    bool PasswordValidationOnLogin(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            validationPopupPanel.SetActive(true);
            errorTextMsg.gameObject.SetActive(true);
            errorTextMsg.color = new Color(0.44f, 0.44f, 0.44f, 1f);
            errorHandler.ShowErrorMessage(ErrorType.Password_field__empty.ToString(), errorTextMsg);
            return false;
        }
        return true;
    }

    bool PasswordValidation(string password1, string password2)
    {
        string pass1 = password1;
        string pass2 = password2;
        if (pass1 == "" || pass2 == "")
        {
            validationPopupPanel.SetActive(true);
            errorTextMsg.gameObject.SetActive(true);
            errorTextMsg.color = new Color(0.44f, 0.44f, 0.44f, 1f);
            errorHandler.ShowErrorMessage(ErrorType.Password_field__empty.ToString(), errorTextMsg);
            return false;
        }

        if (pass1.Length < 8 || pass2.Length < 8)
        {
            validationPopupPanel.SetActive(true);
            errorTextMsg.gameObject.SetActive(true);
            errorTextMsg.color = new Color(0.44f, 0.44f, 0.44f, 1f);
            errorHandler.ShowErrorMessage(ErrorType.Passwords_cannot_less_than_eight_charcters.ToString(), errorTextMsg);
            return false;
        }

        bool allCharactersInStringAreDigits = false;
        string specialCh = @"%!@#$%^&*()?/>.<,:;'\|}]{[_~`+=-" + "\"";
        char[] specialChArray = specialCh.ToCharArray();
        if (pass1.Any(char.IsDigit) && pass1.Any(char.IsLower) && pass1.Any(char.IsUpper) && !pass1.Any(char.IsWhiteSpace))
        {
            foreach (char ch in specialChArray)
            {
                if (pass1.Contains(ch))
                    allCharactersInStringAreDigits = true;
            }

        }

        if (!allCharactersInStringAreDigits)
        {
            validationPopupPanel.SetActive(true);
            errorTextMsg.gameObject.SetActive(true);
            errorTextMsg.color = new Color(0.44f, 0.44f, 0.44f, 1f);
            errorHandler.ShowErrorMessage(ErrorType.Password_must_Contain_Number.ToString(), errorTextMsg);
            return false;
        }

        if (pass1 == pass2)
        {
            return true;
        }
        else
        {
            validationPopupPanel.SetActive(true);
            errorTextMsg.gameObject.SetActive(true);
            errorTextMsg.color = new Color(0.44f, 0.44f, 0.44f, 1f);
            errorHandler.ShowErrorMessage(ErrorType.Passwords_do_not_match.ToString(), errorTextMsg);
            return false;
        }
    }


    bool IsValidEmail(string email)
    {
        bool validEmail = Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.IgnoreCase);
        if (!validEmail)
            return false;
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


    string UniqueID()
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

    public IEnumerator VerficationOTPViaAPI(string url, string Jsondata, string localEmail, GameObject _loader = null)
    {
        if (_loader)
            _loader.SetActive(true);
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(Jsondata);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        MyClassNewApi myObject1 = new MyClassNewApi();
        myObject1 = myObject1.Load(request.downloadHandler.text);
        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            if (myObject1.success)
            {
                OpenUIPanel(3);
            }
        }
        else
        {
            if (!myObject1.success)
            {
                validationPopupPanel.SetActive(true);
                errorTextMsg.gameObject.SetActive(true);
                errorHandler.ShowErrorMessage(myObject1.msg, errorTextMsg);
                errorTextMsg.color = new Color(0.44f, 0.44f, 0.44f, 1f);


            }
        }
        if (_loader != null)
            _loader.SetActive(false);

        request.Dispose();
    }

    //Validation Screen code
    public void ValueChangeCheck()
    {
        string[] myOtpTxt = new string[otpTexts.Length];
        char[] charArr = new char[verficationPlaceHolder.Text.Length];
        charArr = verficationPlaceHolder.Text.ToCharArray();
        for (int i = 0; i < myOtpTxt.Length; i++)
        {
            if (i < charArr.Length)//1 2 3 4
            {
                myOtpTxt[i] = charArr[i].ToString();
                otpTexts[i].text = myOtpTxt[i].ToString();
            }
            else
            {
                myOtpTxt[i] = "";
                otpTexts[i].text = myOtpTxt[i].ToString();
            }
        }
    }


    public void SubmitOTP()
    {
        otpLoader.SetActive(true);
        otpnextButton.interactable = false;

        string OTP = "";
        OTP = verficationPlaceHolder.Text;
        if (OTP == "" || OTP.Length < 4)
        {
            validationPopupPanel.SetActive(true);
            errorTextMsg.color = new Color(0.44f, 0.44f, 0.44f, 1f);
            errorHandler.ShowErrorMessage(ErrorType.OTP_fields__empty.ToString(), errorTextMsg);
            otpLoader.SetActive(false);
            otpnextButton.interactable = true;
            return;
        }
        string url = ConstantsGod.API_BASEURL + ConstantsGod.VerifyEmailOTP;
        MyClassOfPostingOTP myObject = new MyClassOfPostingOTP();
        string bodyJson = JsonUtility.ToJson(myObject.GetdataFromClass(emailForSignup, OTP));
        StartCoroutine(HitOTPAPI(url, bodyJson, () =>
         {
             otpLoader.SetActive(false);
             otpnextButton.interactable = true;
         }));

        //for now only via email user can signup
        //if (ForgetPasswordBool)
        //{
        //    string url = ConstantsGod.API_BASEURL + ConstantsGod.ForgetPasswordOTPAPI;
        //    MyClassOfPostingForgetPasswordOTP myobjectOfPhone = new MyClassOfPostingForgetPasswordOTP();
        //    string bodyJson = JsonUtility.ToJson(myobjectOfPhone.GetdataFromClass(ForgetPasswordEmlOrPhnContainer, OTP));
        //    StartCoroutine(HitOTPAPI(url, bodyJson));
        //}
        //else
        //{
        //// Phone OTP sending Section
        //if (SignUpWithPhoneBool)
        //{
        //    string url = ConstantsGod.API_BASEURL + ConstantsGod.VerifyPhoneOTPAPI;
        //    MyClassOfPostingPhoneOTP myobjectOfPhone = new MyClassOfPostingPhoneOTP();
        //    string bodyJson = JsonUtility.ToJson(myobjectOfPhone.GetdataFromClass(LocalPhoneNumber, OTP));
        //    StartCoroutine(HitOTPAPI(url, bodyJson));
        //}
        //// Email OTP sending Section
        //else
        //{
        //string url = ConstantsGod.API_BASEURL + ConstantsGod.VerifyEmailOTP;
        //MyClassOfPostingOTP myObject = new MyClassOfPostingOTP();
        //string bodyJson = JsonUtility.ToJson(myObject.GetdataFromClass(Email, OTP));
        //StartCoroutine(HitOTPAPI(url, bodyJson));
        //}
        //}
    }
    IEnumerator HitOTPAPI(string url, string Jsondata, Action callback)
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
        MyClassNewApi myObjectForOPT = new MyClassNewApi();
        myObjectForOPT = myObjectForOPT.Load(request.downloadHandler.text);
        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            if (myObjectForOPT.success)
            {
                OpenUIPanel(4);
            }
        }
        else
        {
            if (!myObjectForOPT.success)
            {
                validationPopupPanel.SetActive(true);
                errorTextMsg.color = new Color(0.44f, 0.44f, 0.44f, 1f);
                errorHandler.ShowErrorMessage(ErrorType.Authentication_Code_is_Incorrect.ToString(), errorTextMsg);
            }
        }
        callback();
        request.Dispose();
    }

    public void ResendOTP()
    {
        CheckForValidationAndSignUp(true);
    }


    //Enter User Name Section 
    public void EnterUserName()
    {
        nameScreenLoader.SetActive(true);
        nameScreenNextButton.interactable = false;

        string Localusername = userNameField.Text;

        if (Localusername == "")
        {
            validationPopupPanel.SetActive(true);
            errorTextMsg.color = new Color(0.44f, 0.44f, 0.44f, 1f);
            errorHandler.ShowErrorMessage(ErrorType.Name_Field__empty.ToString(), errorTextMsg);
            nameScreenLoader.SetActive(false);
            nameScreenNextButton.interactable = true;
            return;
        }
        else if (Localusername.StartsWith(" "))
        {
            validationPopupPanel.SetActive(true);
            errorTextMsg.color = new Color(0.44f, 0.44f, 0.44f, 1f);
            errorHandler.ShowErrorMessage(ErrorType.UserName_Has_Space.ToString(), errorTextMsg);
            nameScreenLoader.SetActive(false);
            nameScreenNextButton.interactable = true;
            return;
        }

        if (Localusername.EndsWith(" "))
        {
            Localusername = Localusername.TrimEnd(' ');
        }
        PlayerPrefs.SetInt("IsProcessComplete", 1);
        MyClassOfPostingName myObject = new MyClassOfPostingName();
        string bodyJsonOfName = JsonUtility.ToJson(myObject.GetNamedata(Localusername));

        string url = ConstantsGod.API_BASEURL + ConstantsGod.RegisterWithEmail;
        MyClassOfRegisterWithEmail myobjectOfEmail = new MyClassOfRegisterWithEmail();
        string _bodyJson = JsonUtility.ToJson(myobjectOfEmail.GetdataFromClass(emailForSignup, passwordForSignup));


        if (XanaConstants.isWalletLogin)
        {
            StartCoroutine(HitNameAPIWithNewTechnique(ConstantsGod.API_BASEURL + ConstantsGod.NameAPIURL, bodyJsonOfName, Localusername, (isSucess) =>
            {
                OpenUIPanel(16);
                nameScreenLoader.SetActive(false);
                nameScreenNextButton.interactable = true;
                
                Debug.Log("Wallet Signup");
                GlobalConstants.SendFirebaseEvent(GlobalConstants.FirebaseTrigger.Signup_Wallet_Completed.ToString());
            }));
        }
        else
        {
            StartCoroutine(RegisterUserWithNewTechnique(url, _bodyJson, bodyJsonOfName, Localusername, (isSucess) =>
            {
                nameScreenLoader.SetActive(false);
                nameScreenNextButton.interactable = true;
                
                Debug.Log("Email Signup");
                GlobalConstants.SendFirebaseEvent(GlobalConstants.FirebaseTrigger.Signup_Email_Completed.ToString());
                PremiumUsersDetails.Instance.GetGroupDetails("freeuser");
            }));
        }


        //ProfilePictureManager.instance.MakeProfilePicture(Localusername);
    }
    IEnumerator RegisterUserWithNewTechnique(string url, string Jsondata, string JsonOfName, String NameofUser, Action<bool> CallBack)
    {
        _web3APIforWeb2._OwnedNFTDataObj.ClearAllLists();
        _web3APIforWeb2._OwnedNFTDataObj.FillAllListAsyncWaiting();
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(Jsondata);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        ClassWithToken myObject = new ClassWithToken();
        myObject = ClassWithToken.CreateFromJSON(request.downloadHandler.text);
        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            if (myObject.success)
            {
                MyClassOfRegisterWithEmail myobjectOfEmail = new MyClassOfRegisterWithEmail();
                myobjectOfEmail = MyClassOfRegisterWithEmail.CreateFromJSON(Jsondata);
                MyClassOfLoginJson myObject1 = new MyClassOfLoginJson();
                string bodyJson = JsonUtility.ToJson(myObject1.GetdataFromClass(myobjectOfEmail.email, "", myobjectOfEmail.password, UniqueID()));

                ConstantsGod.AUTH_TOKEN = myObject.data.token;
                XanaConstants.xanaToken = myObject.data.token;
                XanaConstants.userId = myObject.data.user.id;


                PlayerPrefs.SetString("UserNameAndPassword", bodyJson);
                PlayerPrefs.SetString("UserName", myObject.data.user.id);
                PlayerPrefs.Save();

                StartCoroutine(HitNameAPIWithNewTechnique(ConstantsGod.API_BASEURL + ConstantsGod.NameAPIURL, JsonOfName, NameofUser, (isSucess) =>
                {
                    if (isSucess)
                    {
                        PlayerPrefs.SetInt("IsLoggedIn", 1);
                        PlayerPrefs.SetInt("FristPresetSet", 1);
                        PlayerPrefs.SetInt("FirstTime", 1);
                        PlayerPrefs.SetInt("WalletLogin", 0);
                        PlayerPrefs.SetString("PlayerName", NameofUser);
                        XanaConstants.userName = NameofUser;
                        XanaConstants.loggedIn = true;
                        XanaConstants.isWalletLogin = false;
                        OpenUIPanel(16);
                        ItemDatabase.instance.GetComponent<SavaCharacterProperties>().SavePlayerProperties();
                        DynamicEventManager.deepLink?.Invoke("Sign Up Flow");
                        MainSceneEventHandler.OnSucessFullLogin?.Invoke();
                        CallBack(true);
                    }
                    else
                        CallBack(false);
                }));
                GameManager.Instance.bottomTabManagerInstance.HomeSceneFooterSNSButtonIntrectableTrueFalse();
            }
        }
        else
        {
            if (!myObject.success)
            {
                validationPopupPanel.SetActive(true);
                errorTextMsg.gameObject.SetActive(true);
                errorTextMsg.color = new Color(0.44f, 0.44f, 0.44f, 1f);
                errorHandler.ShowErrorMessage(myObject.msg, errorTextMsg);
                CallBack(false);
            }
        }
        request.Dispose();
    }
    IEnumerator HitNameAPIWithNewTechnique(string url, string Jsondata, string localUsername, Action<bool> UserRegisteredCallBack)
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
        MyClassNewApi myObject1 = new MyClassNewApi();
        Debug.LogError(request.downloadHandler.text);
        myObject1 = myObject1.Load(request.downloadHandler.text);
        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            if (myObject1.success)
            {
                if (myObject1.msg == "This name is already taken by other user.")
                {
                    validationPopupPanel.SetActive(true);
                    errorTextMsg.color = new Color(0.44f, 0.44f, 0.44f, 1f);
                    errorHandler.ShowErrorMessage("Username already exists", errorTextMsg);
                    UserRegisteredCallBack(false);
                }
                else
                {
                    UserRegisteredCallBack(true);
                }
                GameManager.Instance.mainCharacter.GetComponent<CharacterOnScreenNameHandler>().UpdateNameText(localUsername);
            }
        }
        else
        {
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
            }
            else
            {
                if (!myObject1.success)
                {
                    validationPopupPanel.SetActive(true);
                    errorTextMsg.color = new Color(0.44f, 0.44f, 0.44f, 1f);
                    errorHandler.ShowErrorMessage(myObject1.msg, errorTextMsg);
                }
            }
            UserRegisteredCallBack(false);
        }

        request.Dispose();
    }



    public void SubmitCredentialsForSignIn()
    {
        loginLoader.SetActive(true);
        loginButton.interactable = false;

        string _LoginEmail = emailFieldLogin.Text;
        string _loginPassword = passwordFieldLogin.Text;

        _LoginEmail = _LoginEmail.Trim();
        _loginPassword = _loginPassword.Trim();
        _LoginEmail = _LoginEmail.ToLower();

        if (EmailValidation(_LoginEmail, true) && PasswordValidationOnLogin(_loginPassword))
        {

            string bodyJson;
            MyClassOfLoginJson myObject = new MyClassOfLoginJson();

            if (IsPhoneNbr(_LoginEmail))
                bodyJson = JsonUtility.ToJson(myObject.GetdataFromClass("", _LoginEmail, _loginPassword));
            else
                bodyJson = JsonUtility.ToJson(myObject.GetdataFromClass(_LoginEmail, "", _loginPassword, UniqueID()));

            string url = ConstantsGod.API_BASEURL + ConstantsGod.LoginAPIURL;
            StartCoroutine(LoginUser(url, bodyJson, (isSucess) =>
            {
                loginLoader.SetActive(false);
                loginButton.interactable = true;

                
            }));
        }
        else
        {
            loginLoader.SetActive(false);
            loginButton.interactable = true;
        }

    }

    IEnumerator LoginUser(string url, string Jsondata, Action<bool> CallBack)
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
                if(!XanaConstants.loggedIn)
                {
                    Debug.Log("Email Login");
                    GlobalConstants.SendFirebaseEvent(GlobalConstants.FirebaseTrigger.Login_Email_Success.ToString());
                }

                XanaConstants.xanaliaToken = myObject1.data.xanaliaToken;
                XanaConstants.xanaToken = myObject1.data.token;
                XanaConstants.isAdmin = myObject1.data.isAdmin;
                XanaConstants.isGuestLogin = false;
                XanaConstants.xanaConstants.LoginasGustprofile = true;
                XanaConstants.userId = myObject1.data.user.id.ToString();
                XanaConstants.userName = myObject1.data.user.name;
                XanaConstants.loggedIn = true;
                XanaConstants.isWalletLogin = false;
                ConstantsGod.AUTH_TOKEN = myObject1.data.token;

                PlayerPrefs.SetString("UserNameAndPassword", Jsondata);
                PlayerPrefs.SetInt("shownWelcome", 1);
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
                OpenUIPanel(21);

                DynamicEventManager.deepLink?.Invoke("Login user here");
                MainSceneEventHandler.OnSucessFullLogin?.Invoke();

                

                CallBack(true);
            }
        }
        else
        {
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                validationPopupPanel.SetActive(true);
                errorTextMsg.color = new Color(0.44f, 0.44f, 0.44f, 1f);
                errorHandler.ShowErrorMessage(ErrorType.Poor_Connection.ToString(), errorTextMsg);
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
                            //if (AutoLoginBool)
                            //{
                            //    PlayerPrefs.DeleteAll();
                            //    yield return null;
                            //}
                            //else
                            //{
                            //    /* PlayerPrefs.SetString("JSONdataforlogin", Jsondata);
                            //     LogoutfromOtherDevicePanel.SetActive(true);
                            //     MyClassOfLoginJson myObject = new MyClassOfLoginJson();
                            //     myObject = myObject.CreateFromJSON(Jsondata);
                            //     MyClassOfLogoutDevice logoutObj = new MyClassOfLogoutDevice();
                            //     string bodyJson2 = JsonUtility.ToJson(logoutObj.GetdataFromClass(myObject.email, myObject.phoneNumber, myObject.password));
                            //     PlayerPrefs.SetString("LogoutFromDeviceJSON", bodyJson2);*/
                            //}
                        }

                        errorHandler.ShowErrorMessage(myObject1.msg, errorTextMsg);
                    }
                }
            }
            CallBack(false);
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
        string l_DeivceID = UniqueID();
        MyClassForSettingDeviceToken myObject = new MyClassForSettingDeviceToken();
        string bodyJson = JsonUtility.ToJson(myObject.GetUpdatedDeviceToken(l_DeivceID));
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




    //Control UI From Here
    public void OpenUIPanel(int ActivePanalCounter)
    {
        switch (ActivePanalCounter)
        {
            case 1:
                {
                    ShowWelcomeScreen();
                    break;
                }
            case 2:
                {
                    //SignUpPanal.SetActive(true);
                    //if (!WalletScreen.activeInHierarchy)
                    //    OnSignUpPhoneTabPressed();
                    //else
                    //    OnSignUpWalletTabPressed();
                    //EmailFieldNew.Text = "";
                    //PhoneFieldNew.Text = "";
                    break;
                }
            case 3:
                {
                    signUpWithEmailPanel.SetActive(false);
                    verficationCodePanel.SetActive(true);
                    verficationPlaceHolder.Text = "";
                    verficationPlaceHolder.Select();
                    for (int i = 0; i < otpTexts.Length; i++)
                    {
                        otpTexts[i].text = "";
                    }
                    break;
                }
            case 4:
                {
                    verficationCodePanel.SetActive(false);
                    verficationPlaceHolder.Clear();
                    MainSceneEventHandler.OpenPresetPanel?.Invoke();
                    break;
                }
            case 5:
                {
                    //UsernameFieldAdvance.Text = "";
                    break;
                }
            case 6:
                {

                    emailLoginPanel.SetActive(true);
                    emailFieldLogin.Clear();
                    passwordFieldLogin.Clear();
                    emailFieldLogin.Select();
                    savePasswordList.instance.DeleteONStart();
                    break;
                }
            case 7:
                {
                    //if (shownWelcome)
                    //{
                    //    PlayerPrefs.SetInt("shownWelcome", 1);
                    //    LoggedIn = true;
                    //}
                    //else
                    //{
                    //    LoggedIn = true;
                    //}
                    break;
                }
            case 8:
                {
                    //Debug.Log("Signup here");
                    //PlayerPrefs.SetInt("iSignup", 1);// going for register user
                    //SignUpPanal.SetActive(true);
                    //EmailFieldNew.Text = "";
                    //Password1New.Text = "";
                    //Password2New.Text = "";
                    break;
                }
            case 13:
                {

                    //if (PlayerPrefs.GetInt("WalletLogin") != 1)
                    //{
                    //    RegistrationCompletePanal.SetActive(true);
                    //    StoreManager.instance.StartPanel_PresetParentPanel.SetActive(true);
                    //}
                    //if (shownWelcome)
                    //    ShowWelcomeClosed();
                    break;
                }
            case 14:
                {
                    //ForgetPasswordTokenAfterVerifyling = "";
                    //ForgetenterUserNamePanal.SetActive(true);
                    //EmailOrPhone_Forget_NewField.Text = "";
                    break;
                }
            case 15:
                {
                    //ForgetEnterPasswordPanal.SetActive(true);
                    //Password1_ForgetNewField.Text = "";
                    //Password2_ForgetNewField.Text = "";
                    break;
                }
            case 16:
                {
                    enterNamePanel.SetActive(false);
                    userNameField.Clear();
                    //TutorialsManager.instance.ShowTutorials();
                    break;
                }
            case 17:
                {
                    //if (shownWelcome)
                    //{

                    //    if (PlayerPrefs.GetInt("iSignup") == 1)
                    //    {
                    //        PlayerPrefs.SetInt("iSignup", 0);

                    //        welcomeScreen.SetActive(true);
                    //        shownWelcome = true;
                    //    }
                    //    else
                    //        ShowWelcomeClosed();
                    //}
                    //else
                    //{
                    //    FirstPanal.SetActive(true);
                    //}
                    break;
                }

            case 18:
                {

                    //PlayerPrefs.SetInt("iSignup", 1);// going for Wallet register user
                    break;
                }
            case 19:
                {
                    //PlayerPrefs.SetInt("iSignup", 0);// going for guest user registration
                    //XanaConstants.xanaConstants.LoginasGustprofile = true;
                    break;
                }

            case 20:
                {
                    //if (!WalletScreen.activeInHierarchy)
                    //{
                    //    if (SignUpButtonSelected == 1)
                    //    {
                    //        OnSignUpPhoneTabPressed();
                    //        PhoneFieldNew.Text = "";
                    //        Password1New.Text = "";
                    //        Password2New.Text = "";
                    //    }
                    //    else if (SignUpButtonSelected == 2)
                    //    {
                    //        OnSignUpEmailTabPressed();
                    //        EmailFieldNew.Text = "";
                    //        Password1New.Text = "";
                    //        Password2New.Text = "";
                    //    }
                    //    SignUpPanal.SetActive(true);
                    //}
                    break;
                }
            case 21:
                {
                    emailLoginPanel.SetActive(false);
                    break;
                }
        }
    }


    public void ShowValidationPop(ErrorType errorMsg)
    {
        validationPopupPanel.SetActive(true);
        errorTextMsg.color = new Color(0.44f, 0.44f, 0.44f, 1f);
        errorHandler.ShowErrorMessage(errorMsg.ToString(), errorTextMsg);
    }


    //logout account 
    public void LogoutAccount()
    {
        string deviceToken = UniqueID();
        if (!string.IsNullOrEmpty(deviceToken))
        {
            StartCoroutine(HitLogOutAPI(ConstantsGod.API_BASEURL + ConstantsGod.LogOutAPI, deviceToken, (onSucess) =>
            {

                StartCoroutine(OnSucessLogout());
            }
            ));
        }
    }

    //Account Delete functions 
    public void DeleteAccount(Action callback)
    {
        string deviceToken = UniqueID();
        if (!string.IsNullOrEmpty(deviceToken))
            StartCoroutine(HitLogOutAPI(ConstantsGod.API_BASEURL + ConstantsGod.LogOutAPI, deviceToken, (onSucess) =>
            {
                if (onSucess)
                    StartCoroutine(DeleteAccountApi((deleteSucess) =>
                    {
                        if (deleteSucess)
                        {
                            StartCoroutine(OnSucessLogout());
                            callback();
                        }

                    }));
            }
            ));
        StoreManager.instance.CheckWhenUserLogin();
    }

    public IEnumerator HitLogOutAPI(string url, string Jsondata, Action<bool> CallBack)
    {
        LoadingHandler.Instance.characterLoading.gameObject.SetActive(true);
        MyClassForSettingDeviceToken myObject = new MyClassForSettingDeviceToken();
        string bodyJson = JsonUtility.ToJson(myObject.GetUpdatedDeviceToken(Jsondata));
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJson);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        MyClassNewApi myObject1 = new MyClassNewApi();
        myObject1 = myObject1.Load(request.downloadHandler.text);
        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            CallBack(true);
            yield break;
        }
        else
        {
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
            }
            else
            {
                //on wallet logout we are getting internal server error so commented this lines 

                //if (!myObject1.success)
                //{
                //    validationPopupPanel.SetActive(true);
                //    errorTextMsg.gameObject.SetActive(true);
                //    if (errorTextMsg)
                //    {
                //        errorTextMsg.color = new Color(0.44f, 0.44f, 0.44f, 1f);
                //        errorHandler.ShowErrorMessage(ErrorType.Default_Message.ToString(), errorTextMsg);
                //    }
                //}
            }
            LoadingHandler.Instance.characterLoading.gameObject.SetActive(false);
            LoadingHandler.Instance.HideLoading();
            CallBack(false);
        }
        request.Dispose();
    }

    IEnumerator DeleteAccountApi(Action<bool> CallBack)
    {

        string url = ConstantsGod.API_BASEURL + ConstantsGod.r_url_DeleteAccount;
        var request = new UnityWebRequest(url, "POST");

        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        DeleteApiRes myObject1 = new DeleteApiRes();
        myObject1 = JsonUtility.FromJson<DeleteApiRes>(request.downloadHandler.text);

        if (myObject1.success)
            CallBack(true);
        else
            CallBack(false);

        request.Dispose();
    }

    IEnumerator OnSucessLogout()
    {
        Debug.Log("Logout Successfully");
        if (_web3APIforWeb2._OwnedNFTDataObj != null)
        {
            _web3APIforWeb2._OwnedNFTDataObj.ClearAllLists();
        }

        PlayerPrefs.SetInt("IsLoggedIn", 0);
        PlayerPrefs.SetInt("WalletLogin", 0);
        userRoleScriptScriptableObj.userNftRoleSlist.Clear();
        ConstantsGod.AUTH_TOKEN = string.Empty;
        XanaConstants.xanaliaToken = string.Empty;
        XanaConstants.xanaToken = string.Empty;
        XanaConstants.userId = null;
        XanaConstants.isAdmin = false;
        XanaConstants.loggedIn = false;
        XanaConstants.xanaConstants.LoginasGustprofile = false;

        PlayerPrefs.SetString("SaveuserRole", "");
        if (CryptouserData.instance != null)
        {
            CryptouserData.instance.UltramanPass = false;
            CryptouserData.instance.AlphaPass = false;
            CryptouserData.instance.AstroboyPass = false;
        }

        PlayerPrefs.SetString("UserName", "");

        int simultaneousConnectionsValue = PlayerPrefs.GetInt("ShowLiveUserCounter");

        PlayerPrefs.DeleteAll();//Delete All PlayerPrefs After Logout Success.......
        PlayerPrefs.SetString("TermsConditionAgreement", "Agree");
        PlayerPrefs.SetInt("ShowLiveUserCounter", simultaneousConnectionsValue);

        //[Waqas] Reset Guest Username After Delete All
        PlayerPrefs.SetString("publicID", "");
        PlayerPrefs.Save();
        PremiumUsersDetails.Instance.testing = false;
        if (FeedUIController.Instance.SNSSettingController != null)
        {
            FeedUIController.Instance.SNSSettingController.LogoutSuccess();
        }
        ConstantsGod.UserRoles = new List<string>() { "Guest" };
        if (StoreManager.instance.MultipleSave)
            LoadPlayerAvatar.instance_loadplayer.avatarButton.gameObject.SetActive(false);

        LoadingHandler.Instance.characterLoading.gameObject.SetActive(false);
        LoadingHandler.Instance.HideLoading();
        XanaConstants.xanaConstants.isCameraMan = false;
        XanaConstants.xanaConstants.IsDeemoNFT = false;
        StoreManager.instance.CheckWhenUserLogin();
        yield return null;
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


    [Serializable]
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

    [Serializable]
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

    [Serializable]
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

    [Serializable]
    public class MyClassOfPostingEmail
    {
        public string email;
        public MyClassOfPostingEmail GetEmaildata(string eml)
        {
            MyClassOfPostingEmail myObj = new MyClassOfPostingEmail();
            myObj.email = eml;
            return myObj;
        }
    }


    [Serializable]
    public class MyClassOfPostingOTP
    {
        public string email;
        public string otp;
        public MyClassOfPostingOTP GetdataFromClass(string email, string otp)
        {
            MyClassOfPostingOTP myObj = new MyClassOfPostingOTP();
            myObj.email = email;
            myObj.otp = otp;
            return myObj;
        }
    }

    [Serializable]
    public class MyClassOfPostingName
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
    public class MyClassOfRegisterWithEmail
    {
        public string email;
        public string password;
        public MyClassOfRegisterWithEmail GetdataFromClass(string L_eml, string passwrd)
        {
            MyClassOfRegisterWithEmail myObj = new MyClassOfRegisterWithEmail();
            myObj.email = L_eml;
            myObj.password = passwrd;
            return myObj;
        }

        public static MyClassOfRegisterWithEmail CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<MyClassOfRegisterWithEmail>(jsonString);
        }
    }

    [System.Serializable]
    public class DeleteApiRes
    {
        public bool success;
        public string data;
        public string msg;
    }
    #endregion

    enum NftRolePriority
    {
        alpha_pass,
        dj_event,
        vip_pass,
        premium,
        free,
        guest,
        Astroboy
    }
}

