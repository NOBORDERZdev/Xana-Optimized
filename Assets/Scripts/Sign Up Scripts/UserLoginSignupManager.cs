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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
    public GameObject LoginRegisterScreen;

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
    public GameObject EditProfilePanel;
    public AdvancedInputField displayrNameField;
    public AdvancedInputField userUsernameField;
    public GameObject UserNameFieldObj;
    public Image SelectedPresetImage;
    public Image SelectPresetImageforEditProfil;
    public RawImage AiPresetImage;
    public RawImage AiPresetImageforEditProfil;
    public Button NameScreenNextButton;
    public Button ProfilePicNextButton;
    public Sprite NameFeildSelectedSprite;
    public Sprite NameFeildUnSelectedSprite;
    public Image NameScreenNextButtonImage;
    public GameObject NameScreenLoader;
    public GameObject ProfilePicScreenLoader;
    public Image EditProfileImage;
    public string SetProfileAvatarTempPath = "";
    public string SetProfileAvatarTempFilename = "";
    public string PermissionCheck = "";
    public GameObject PickImageOptionScreen;
    [Space(5)]
    public GameObject permissionPopup;

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
    public Web3Web2Handler _web3APIforWeb2;
    public ConnectWallet connectingWalletRef;
    public userRoleScript userRoleScriptScriptableObj;
    public static UserLoginSignupManager instance;
    public Action logoutAction;
    EyesBlinking ref_EyesBlinking;
    [Header("Bools Fields")]
    private bool _isUserClothDataFetched = false;

    public float DisplayNameFieldMoveUpValue = 500f; // Distance to move the input field up
    Vector2 _originalPosition;
    AdvancedInputField _displayNameInputField;
    //public bool LoggedInAsGuest = false;

    private void OnEnable()
    {
        instance = this;
        if (!File.Exists(GameManager.Instance.GetStringFolderPath()))
        {
            SaveCharacterProperties.instance.CreateFileFortheFirstTime();
        }
        verficationPlaceHolder.OnValueChanged.AddListener(delegate { ValueChangeCheck(); });
        Web3Web2Handler.AllDataFetchedfromServer += Web3EventForNFTData;

        CheckForAutoLogin();
        if (ref_EyesBlinking == null)
            ref_EyesBlinking = GameManager.Instance.mainCharacter.GetComponent<EyesBlinking>();

        if (ref_EyesBlinking)
        {
            ref_EyesBlinking.StoreBlendShapeValues();
            StartCoroutine(ref_EyesBlinking.BlinkingStartRoutine());
        }
        string saveDir = Path.Combine(Application.persistentDataPath, "UserProfilePic");
        if (!Directory.Exists(saveDir))
        {
            Directory.CreateDirectory(saveDir);
        }

        _displayNameInputField = displayrNameField.GetComponent<AdvancedInputField>();

        _originalPosition = _displayNameInputField.GetComponent<RectTransform>().anchoredPosition;

        
        if (_displayNameInputField != null)
        {
            _displayNameInputField.OnBeginEdit.AddListener(MoveInputFieldUp);
            _displayNameInputField.OnEndEdit.AddListener(MoveInputFieldDown);
        }
    }

    private void OnDisable()
    {
        verficationPlaceHolder.OnValueChanged.RemoveListener(delegate { ValueChangeCheck(); });
        Web3Web2Handler.AllDataFetchedfromServer -= Web3EventForNFTData;
    }


    private void MoveInputFieldUp(BeginEditReason reason)
    {
        // Move the input field up by a certain distance
        _displayNameInputField.GetComponent<RectTransform>().anchoredPosition = new Vector2(
            _originalPosition.x,
            _originalPosition.y + DisplayNameFieldMoveUpValue
        );
    }

    private void MoveInputFieldDown(string text, EndEditReason reason)
    {
        // Move the input field back to its original position
        _displayNameInputField.GetComponent<RectTransform>().anchoredPosition = _originalPosition;
    }


    void CheckForAutoLogin()
    {
        // If already logged in than Return
        if (ConstantsHolder.loggedIn)
        {
            InventoryManager.instance.SetDefaultValues();
            return;
        }

        //if (PlayerPrefs.GetInt("IsLoggedIn") == 1 && PlayerPrefs.GetInt("WalletLogin") != 1)
        //{
        //    MyClassOfLoginJson LoginObj = new MyClassOfLoginJson();
        //    LoginObj = LoginObj.CreateFromJSON(PlayerPrefs.GetString("UserNameAndPassword"));
        //    StartCoroutine(LoginUser(ConstantsGod.API_BASEURL + ConstantsGod.LoginAPIURL, PlayerPrefs.GetString("UserNameAndPassword"), (isSucess) =>
        //    {
        //        //write if you want something on sucessfull login
        //    }));
        //}
        //else if (PlayerPrefs.GetInt("WalletLogin") == 1)
        //{
        //    ConstantsGod.AUTH_TOKEN = PlayerPrefs.GetString("LoginToken");
        //    ConstantsHolder.xanaToken = PlayerPrefs.GetString("LoginToken");
        //    ConstantsHolder.isWalletLogin = true;
        //    WalletAutoLogin();
        //}
        //else
        //{
        //    ShowWelcomeScreen();
        //}


        if (PlayerPrefs.GetInt("IsLoggedIn") == 1 || PlayerPrefs.GetInt("WalletLogin") == 1)
        {
            LoadingHandler.Instance.GetComponent<CanvasGroup>().alpha = 1;
            if (!ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
            {
                LoadingHandler.Instance.nftLoadingScreen.SetActive(true);
            }
            else
            {
                LoadingHandler.Instance.LoadingScreenSummit.SetActive(true);
            }
            StartCoroutine(RefreshXanaTokenAPI());
        }
        else
        {
            if (!ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
            {
                LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
            }
            else
            {
                LoadingHandler.Instance.LoadingScreenSummit.SetActive(false);
            }
            ShowWelcomeScreen();
        }
    }
    IEnumerator RefreshXanaTokenAPI()
    {
        string _FinalUrl = ConstantsGod.API_BASEURL + ConstantsGod.REFRESHXANATOKEN;
        UnityWebRequest www = UnityWebRequest.Post(_FinalUrl, new Dictionary<string, string>
        {
            { "token", PlayerPrefs.GetString("LoginToken") }
        });

        www.SendWebRequest();
        while (!www.isDone)
        {
            yield return null;
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log($"Token Refresh Error: {www.error}");
        }
        else
        {
            try
            {
                JObject _JsonObj = JObject.Parse(www.downloadHandler.text);
                bool _IsSuccess = _JsonObj["success"].ToObject<bool>();
                if (_IsSuccess)
                {
                    string _Token = _JsonObj["data"]["token"].ToString();
                    ConstantsGod.AUTH_TOKEN = _Token;
                    ConstantsHolder.xanaToken = _Token;
                    PlayerPrefs.SetString("LoginToken", _Token);
                    PlayerPrefs.Save();

                    AutoLogin();
                }
                else
                {
                    Debug.Log($"Token Refresh Error: {_JsonObj["msg"]}");
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Error parsing token refresh response: {ex.Message}");
            }
        }
    }
    void AutoLogin()
    {
        ConstantsGod.AUTH_TOKEN = PlayerPrefs.GetString("LoginToken");
        ConstantsHolder.xanaToken = PlayerPrefs.GetString("LoginToken");
        ConstantsHolder.isWalletLogin = true;
        WalletAutoLogin();
    }
    #region SignUp Functions 

    public void ShowWelcomeScreen()
    {
        if (PlayerPrefs.GetInt("shownWelcome") == 0)
        {
            signUpOrloginSelectionPanel.SetActive(true);
        }
        else
        {
            signUpOrloginSelectionPanel.SetActive(false);
        }
        StartCoroutine(LoginGuest(ConstantsGod.API_BASEURL + ConstantsGod.guestAPI));
        ClearInputFieldsData();
    }
    public void OnClickGuestSelection() {
        if (PlayerPrefs.GetInt("IsProcessComplete") == 1)
        {
            if (PlayerPrefs.GetInt("iSignup") == 1)
            {
                PlayerPrefs.SetInt("presetPanel", 1);
                DefaultClothDatabase.instance.GetComponent<SaveCharacterProperties>().SavePlayerProperties();
                InventoryManager.instance.OnSaveBtnClicked();  // reg complete go home
            }
            if (ConstantsHolder.xanaConstants.SwitchXanaToXSummit && !ConstantsHolder.xanaConstants.openLandingSceneDirectly)
            {
                if (Screen.orientation == ScreenOrientation.LandscapeRight || Screen.orientation == ScreenOrientation.LandscapeLeft)
                {
                    Screen.orientation = ScreenOrientation.Portrait;
                    signUpOrloginSelectionPanel.SetActive(false);
                }

            }
        }
        else
        {
            signUpOrloginSelectionPanel.SetActive(false);

            if (!PlayerPrefs.HasKey("shownWelcome"))
            {
                if (!ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
                {
                    InventoryManager.instance.StartPanel_PresetParentPanel.SetActive(true);
                }
                else
                {
                    InventoryManager.instance.StartPanel_PresetParentPanelSummit.SetActive(true);
                }
            }
        }


    }
    public void ContinueAsGuest()
    {
        //GameManager.Instance.NotNowOfSignManager();
        LoginRegisterScreen.SetActive(false);
        Screen.orientation = ScreenOrientation.Portrait;
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
        if (ConstantsHolder.xanaConstants.LoggedInAsGuest)
        {
            LoginRegisterScreen.SetActive(false);
            emailOrWalletLoginPanel.SetActive(true);
            ClearInputFieldsData();
        }
        else
        {
            emailOrWalletLoginPanel.SetActive(true);
            signUpOrloginSelectionPanel.SetActive(false);
            signUpPanel.SetActive(false);
            signUpWithEmailPanel.SetActive(false);
            ClearInputFieldsData();

        }
    }

    public void BackFromLoginSelection()
    {
       
        if (!ConstantsHolder.xanaConstants.openLandingSceneDirectly && ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
        {

            signUpOrloginSelectionPanel.SetActive(true);
        }
        else {
            
            signUpOrloginSelectionPanel.SetActive(true);
        }
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


    public void OpenUserNamePanel()
    {
        enterNamePanel.SetActive(true);
        displayrNameField.Clear();
        userUsernameField.Clear();
    }
    public void BackFromUserNamePanel()
    {
        if (!ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
        {
            enterNamePanel.SetActive(false);
            displayrNameField.Clear();
            userUsernameField.Clear();
            InventoryManager.instance.StartPanel_PresetParentPanel.SetActive(true);
        }
        else {
            enterNamePanel.SetActive(false);
            displayrNameField.Clear();
            userUsernameField.Clear();
            InventoryManager.instance.StartPanel_PresetParentPanelSummit.SetActive(true);
        }
    }


    private void ClearInputFieldsData()
    {
        emailField.Clear();
        emailFieldLogin.Clear();
        passwordField.Clear();
        passwordFieldLogin.Clear();
        repeatPasswordField.Clear();
        displayrNameField.Clear();
        userUsernameField.Clear();
    }

    //wallet login functions 
    public void WalletAutoLogin()
    {
        if (!ConstantsHolder.loggedIn)
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
        ConstantsHolder.userId = PlayerPrefs.GetString("UserId");
        ConstantsHolder.loggedIn = true;
        ConstantsHolder.isWalletLogin = true;

        if (ConstantsHolder.xanaConstants.openLandingSceneDirectly)
        {
            GetUserClothData();
            return;
        }

        if (!_isUserClothDataFetched)
        {
            GetUserClothData();
            _isUserClothDataFetched = true;
        }
        GetOwnedNFTsFromAPI();
        InventoryManager.instance.WalletLoggedinCall();
        UserPassManager.Instance.GetGroupDetails("freeuser");
        UserPassManager.Instance.GetGroupDetailsForComingSoon();
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
        // DynamicEventManager.deepLink?.Invoke("moralis wait and come");
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
                    case "vip-pass":
                        {
                            UserPassManager.Instance.GetGroupDetails("vip-pass");
                            break;
                        }
                    case "astroboy":
                        {
                            UserPassManager.Instance.GetGroupDetails("astroboy");
                            break;
                        }
                }
            }
        }
        else
        {
            //print("you have no Premium Access ");
            UserPassManager.Instance.GetGroupDetails("freeuser");
        }
        UserPassManager.Instance.GetGroupDetailsForComingSoon();
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
                    //        ConstantsHolder.xanaConstants.isNFTEquiped = true;
                    //        BoxerNFTEventManager.OnNFTequip?.Invoke(false);
                    //    }
                    //}
                    //else
                    //{
                    //    PlayerPrefs.DeleteKey("Equiped");
                    //    PlayerPrefs.DeleteKey("nftID");
                    //    ConstantsHolder.xanaConstants.isNFTEquiped = false;
                    //    BoxerNFTEventManager.OnNFTUnequip?.Invoke();
                    //    LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
                    //}
                }
            }
            else
            {
                if (!ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
                {
                    LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
                }
                else
                {
                    LoadingHandler.Instance.LoadingScreenSummit.SetActive(false);
                }
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
        if (!ConstantsHolder.loggedIn)
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
        ConstantsHolder.loggedIn = true;
        ConstantsHolder.isWalletLogin = true;
        SubmitSetDeviceToken();
        WebViewManager.Instance.CloseWebView();
        if (signUpOrloginSelectionPanel.activeInHierarchy)
        {
            signUpOrloginSelectionPanel.SetActive(false);
        }
        if (ConstantsHolder.xanaConstants.openLandingSceneDirectly)
        {
            GetUserClothData();
            return;
        }

        if (!_isUserClothDataFetched)
        {
            GetUserClothData();
            _isUserClothDataFetched = true;
        }
        GetOwnedNFTsFromAPI();
        UserPassManager.Instance.GetGroupDetails("freeuser");
        UserPassManager.Instance.GetGroupDetailsForComingSoon();
        StartCoroutine(GameManager.Instance.mainCharacter.GetComponent<CharacterOnScreenNameHandler>().IERequestGetUserDetails());
        CharacterHandler.instance.playerPostCanvas.GetComponent<LookAtCamera>().GetLatestPost();
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

    public void EnterUserName()
    {
        if (!ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
        {
            NameScreenLoader.SetActive(true);
            NameScreenNextButton.interactable = false;

        }
        string displayrname = displayrNameField.Text;
        string userUsername = userUsernameField.Text;
        string keytoLocalize;
        if (ConstantsHolder.xanaConstants.LoggedInAsGuest)
        {
            if (displayrname == "")
            {
                keytoLocalize = TextLocalization.GetLocaliseTextByKey("Display name or username should not be empty.");
                UserDisplayNameErrors(keytoLocalize);
                return;
            }

            else if (displayrname.StartsWith(" "))
            {
                UserDisplayNameErrors(ErrorType.UserName_Has_Space.ToString());
                return;
            }
            else if (displayrname.EndsWith(" "))
            {
                displayrname = displayrname.TrimEnd(' ');
            }

        }
        else if (ConstantsHolder.xanaConstants.SwitchXanaToXSummit) {

            if (displayrname == "")
            {
                keytoLocalize = TextLocalization.GetLocaliseTextByKey("Display name or username should not be empty.");
                UserDisplayNameErrors(keytoLocalize);
                return;
            }

            else if (displayrname.StartsWith(" "))
            {
                UserDisplayNameErrors(ErrorType.UserName_Has_Space.ToString());
                return;
            }
            else if (displayrname.EndsWith(" "))
            {
                displayrname = displayrname.TrimEnd(' ');
            }
        }
        else
        {

            if (displayrname == "" || userUsername == "")
            {
                keytoLocalize = TextLocalization.GetLocaliseTextByKey("Display name or username should not be empty.");
                UserDisplayNameErrors(keytoLocalize);
                return;
            }

            else if (displayrname.StartsWith(" ") || userUsername.StartsWith(" "))
            {
                UserDisplayNameErrors(ErrorType.UserName_Has_Space.ToString());
                return;
            }
            else if (userUsername.All(char.IsDigit))
            {
                keytoLocalize = TextLocalization.GetLocaliseTextByKey("The username must include letters.");
                UserDisplayNameErrors(keytoLocalize);
                return;
            }
            else if (userUsername.Length < 5 || userUsername.Length > 15)
            {
                keytoLocalize = TextLocalization.GetLocaliseTextByKey("The username must be between 5 and 15 characters.");
                UserDisplayNameErrors(keytoLocalize);
                return;
            }
            else if (!userUsername.Any(c => char.IsDigit(c) || c == '_'))
            {
                keytoLocalize = TextLocalization.GetLocaliseTextByKey("The username must not include Space. Alphabet, Numbers, or Underscore allowed.");
                UserDisplayNameErrors(keytoLocalize);
                return;

            }
            else if (displayrname.EndsWith(" "))
            {
                displayrname = displayrname.TrimEnd(' ');
            }
            else if (userUsername.EndsWith(" "))
            {
                userUsername = userUsername.TrimEnd(' ');
            }
        }
        if (PlayerPrefs.GetInt("shownWelcome") == 0 && PlayerPrefs.GetInt("IsProcessComplete") == 0 && PlayerPrefs.GetInt("iSignup") == 0)
        {
            Debug.LogError("Set Name for Guest User");
            //DynamicEventManager.deepLink?.Invoke("come from Guest Registration");
            PlayerPrefs.SetString(ConstantsGod.GUSTEUSERNAME, displayrname);
            if (!ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
            {
                NameScreenNextButton.interactable = true;
                NameScreenLoader.SetActive(false);
            }
            enterNamePanel.SetActive(false);
            //checkbool_preser_start = true;
            PlayerPrefs.SetInt("shownWelcome", 1);
            if (PlayerPrefs.GetInt("shownWelcome") == 1)
            {
                InventoryManager.instance.OnSaveBtnClicked();
            }
            if (!ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
            {
                LoadingHandler.Instance.nftLoadingScreen.SetActive(true);
            }
            else
            {
                LoadingHandler.Instance.LoadingScreenSummit.SetActive(true);
            }
            ConstantsHolder.userName = PlayerPrefs.GetString(ConstantsGod.GUSTEUSERNAME);
            PlayerPrefs.SetInt("IsProcessComplete", 1);// user is registered as guest/register.
            GameManager.Instance.mainCharacter.GetComponent<CharacterOnScreenNameHandler>().SetNameOfPlayerAgain();
            if (ConstantsHolder.xanaConstants.openLandingSceneDirectly)
            {
                LoadSummit();
            }
            else {
                if (ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
                    if (Screen.orientation == ScreenOrientation.LandscapeRight || Screen.orientation == ScreenOrientation.LandscapeLeft)
                {
                    Screen.orientation = ScreenOrientation.Portrait;
                }
                LoadingHandler.Instance.LoadingScreenSummit.SetActive(false);
            }
            return;
        }
        ConstantsHolder.uniqueUserName = userUsername;
        PlayerPrefs.SetInt("IsProcessComplete", 1);
        MyClassOfPostingName myObject = new MyClassOfPostingName();
        string bodyJsonOfName = JsonUtility.ToJson(myObject.GetNamedata(displayrname));

        string url = ConstantsGod.API_BASEURL + ConstantsGod.RegisterWithEmail;
        MyClassOfRegisterWithEmail myobjectOfEmail = new MyClassOfRegisterWithEmail();
        string _bodyJson = JsonUtility.ToJson(myobjectOfEmail.GetdataFromClass(emailForSignup, passwordForSignup));



        if (ConstantsHolder.isWalletLogin)
        {


            StartCoroutine(HitNameAPIWithNewTechnique(ConstantsGod.API_BASEURL + ConstantsGod.NameAPIURL, bodyJsonOfName, displayrname, (isSucess) =>
            {

                Debug.Log("Wallet Signup");

                GlobalConstants.SendFirebaseEvent(GlobalConstants.FirebaseTrigger.Signup_Wallet_Completed.ToString());

            }));
           
            if (!ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
            {
               // LoadingHandler.Instance.nftLoadingScreen.SetActive(true);
                RequestSubmitUsername(userUsername);
            }
            else
            {
                GameManager.Instance.mainCharacter.GetComponent<CharacterOnScreenNameHandler>().SetNameOfPlayerAgain();
                LoadingHandler.Instance.LoadingScreenSummit.SetActive(true);
                if (ConstantsHolder.xanaConstants.openLandingSceneDirectly)
                {
                    MainSceneEventHandler.OpenLandingScene?.Invoke();
                    return;
                }
                else {
                    Screen.orientation = ScreenOrientation.Portrait;
                    LoadingHandler.Instance.LoadingScreenSummit.SetActive(false);
                    enterNamePanel.SetActive(false);
                }
            }
           
        }
        
    }
    public void UserDisplayNameErrors(string errorMSg)
    {

        validationPopupPanel.SetActive(true);
        errorTextMsg.color = new Color(0.44f, 0.44f, 0.44f, 1f);
        errorHandler.ShowErrorMessage(errorMSg, errorTextMsg);
        NameScreenLoader.SetActive(false);
        NameScreenNextButton.interactable = true;

    }
    public void OnValueChangedSprite() {
        if (NameFeildSelectedSprite != null && NameFeildUnSelectedSprite != null)
        {
            if (!string.IsNullOrEmpty(displayrNameField.Text))
            {
                NameScreenNextButtonImage.sprite = NameFeildSelectedSprite;
            }
            else
            {
                NameScreenNextButtonImage.sprite = NameFeildUnSelectedSprite;
            }
        }


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
                ConstantsHolder.xanaToken = myObject.data.token;
                ConstantsHolder.userId = myObject.data.user.id;


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
                        //PlayerPrefs.SetInt("WalletLogin", 0); // in Each case now we are login with Wallet
                        PlayerPrefs.SetString("PlayerName", NameofUser);
                        ConstantsHolder.userName = NameofUser;
                        ConstantsHolder.loggedIn = true;
                        ConstantsHolder.isWalletLogin = false;
                        OpenUIPanel(16);
                        DefaultClothDatabase.instance.GetComponent<SaveCharacterProperties>().SavePlayerProperties();
                        // DynamicEventManager.deepLink?.Invoke("Sign Up Flow");
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
                PlayerPrefs.SetString("PlayerName", localUsername);
                GameManager.Instance.mainCharacter.GetComponent<CharacterOnScreenNameHandler>().UpdateNameText(localUsername);
            }
        }
        else
        {
            //if (request.result == UnityWebRequest.Result.ConnectionError)
            //{
            //}
            //else
            //{
            //    if (!myObject1.success)
            //    {
            //        validationPopupPanel.SetActive(true);
            //        errorTextMsg.color = new Color(0.44f, 0.44f, 0.44f, 1f);
            //        errorHandler.ShowErrorMessage(myObject1.msg, errorTextMsg);
            //    }
            //}
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
                if (!ConstantsHolder.loggedIn)
                {
                    GlobalConstants.SendFirebaseEvent(GlobalConstants.FirebaseTrigger.Login_Email_Success.ToString());
                }

                ConstantsHolder.xanaliaToken = myObject1.data.xanaliaToken;
                ConstantsHolder.xanaToken = myObject1.data.token;
                ConstantsHolder.isAdmin = myObject1.data.isAdmin;
                ConstantsHolder.isGuestLogin = false;
                ConstantsHolder.xanaConstants.LoginasGustprofile = true;
                ConstantsHolder.userId = myObject1.data.user.id.ToString();
                ConstantsHolder.userName = myObject1.data.user.name;
                ConstantsHolder.loggedIn = true;
                ConstantsHolder.isWalletLogin = false;
                ConstantsGod.AUTH_TOKEN = myObject1.data.token;

                PlayerPrefs.SetString("UserNameAndPassword", Jsondata);
                PlayerPrefs.SetInt("shownWelcome", 1);
                //PlayerPrefs.SetInt("WalletLogin", 0); //  in Each case now we are login with Wallet
                PlayerPrefs.SetString("LoginTokenxanalia", myObject1.data.xanaliaToken);
                PlayerPrefs.SetString("publicID", myObject1.data.user.walletAddress);
                PlayerPrefs.SetString("UserName", myObject1.data.user.id);
                PlayerPrefs.SetInt("IsLoggedIn", 1);
                PlayerPrefs.SetInt("FristPresetSet", 1);
                PlayerPrefs.SetString("PlayerName", myObject1.data.user.name);
                PlayerPrefs.SetString("LoggedInMail", myObject1.data.user.email);
                PlayerPrefs.Save();

                UserPassManager.Instance.GetGroupDetails("freeuser");
                UserPassManager.Instance.GetGroupDetailsForComingSoon();

                SubmitSetDeviceToken();

                if (ConstantsHolder.xanaConstants.openLandingSceneDirectly)
                {
                    GetUserClothData();
                    CallBack(true);
                    yield break;
                }

                GetOwnedNFTsFromAPI();
                GetUserClothData();
                CheckCameraMan(myObject1.data.user.email);
                OpenUIPanel(21);

                //DynamicEventManager.deepLink?.Invoke("Login user here");
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
            ConstantsHolder.xanaConstants.isCameraMan = true;
        else
            ConstantsHolder.xanaConstants.isCameraMan = false;
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
        ServerSideUserDataHandler.Instance.GetDataFromServer();
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
                    //    InventoryManager.instance.StartPanel_PresetParentPanel.SetActive(true);
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
                    displayrNameField.Clear();
                    userUsernameField.Clear();
                    //TutorialsHandler.instance.ShowTutorials();
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
                    PlayerPrefs.SetInt("iSignup", 0);// going for guest user registration
                    ConstantsHolder.xanaConstants.LoggedInAsGuest = true;
                    ConstantsHolder.xanaConstants.LoginasGustprofile = true;
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
                //if (onSucess)
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
        InventoryManager.instance.CheckWhenUserLogin();
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

    public IEnumerator LoginGuest(string url, bool ComesFromLogOut = false)
    {
        ConstantsHolder.userId = PlayerPrefs.GetString("UserId");
        Debug.Log("Login Guest userId: " + ConstantsHolder.userId);
        using (UnityWebRequest www = UnityWebRequest.Post(url, "POST"))
        {
            ConstantsHolder.xanaConstants.LoggedInAsGuest = true;
            ConstantsHolder.xanaConstants.LoginasGustprofile = true;
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                yield return null;
            }
            ClassWithToken myObject1 = new ClassWithToken();
            myObject1 = ClassWithToken.CreateFromJSON(www.downloadHandler.text);
            if (!www.isHttpError && !www.isNetworkError)
            {
                if (www.error == null)
                {
                    if (myObject1.success)
                    {

                        ConstantsGod.AUTH_TOKEN = myObject1.data.token;

                        if (PlayerPrefs.GetInt("shownWelcome") == 1)
                        {
                            //DynamicEventManager.deepLink?.Invoke("Guest login");
                        }
                        if (PlayerPrefs.GetString("PremiumUserType") == "Access Pass" || PlayerPrefs.GetString("PremiumUserType") == "Extra NFT" || PlayerPrefs.GetString("PremiumUserType") == "djevent" || PlayerPrefs.GetString("PremiumUserType") == "astroboy")
                        {
                            UserPassManager.Instance.GetGroupDetails(PlayerPrefs.GetString("PremiumUserType"));
                        }
                        else
                        {
                            if (PlayerPrefs.GetInt("WalletLogin") != 1)
                            {
                                UserPassManager.Instance.GetGroupDetails("guest");
                            }
                        }

                        if (ConstantsHolder.userId.IsNullOrEmpty())
                        {
                            ConstantsHolder.userId = myObject1.data.user.id.ToString();
                            PlayerPrefs.SetString("UserId", ConstantsHolder.userId);
                            UserPassManager.Instance.GetGroupDetailsForComingSoon();
                            PlayerPrefs.SetInt("FirstTime", 1);
                        }
                        ConstantsHolder.userName = PlayerPrefs.GetString(ConstantsGod.GUSTEUSERNAME);
                        PlayerPrefs.Save();
                        LoadSummit();
                    }
                }
            }
        }

    }


    void LoadSummit()
    {
        if (ConstantsHolder.xanaConstants.openLandingSceneDirectly && PlayerPrefs.GetInt("IsProcessComplete") == 1)
        {
            
            print("Initialize ---=======  LoggedInAsGuest " + ConstantsHolder.xanaConstants.LoggedInAsGuest);
            if (ConstantsHolder.xanaConstants.LoggedInAsGuest)
            {
                Debug.Log("Initialize Avatar with Guest");
                MainSceneEventHandler.OpenLandingScene?.Invoke();
            }


        }
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
        _isUserClothDataFetched = false;
        Debug.Log("Logout Successfully");
        GameManager.Instance.FriendsHomeManager.GetComponent<FriendHomeManager>().RemoveAllFriends();
        logoutAction?.Invoke();
        PlayerPrefs.SetInt("IsLoggedIn", 0);
        PlayerPrefs.SetInt("WalletLogin", 0);
        userRoleScriptScriptableObj.userNftRoleSlist.Clear();
        ConstantsGod.AUTH_TOKEN = string.Empty;
        ConstantsHolder.xanaliaToken = string.Empty;
        ConstantsHolder.xanaToken = string.Empty;
        ConstantsHolder.userId = null;
        ConstantsHolder.isAdmin = false;
        ConstantsHolder.loggedIn = false;
        ConstantsHolder.xanaConstants.LoginasGustprofile = false;
        ConstantsHolder.xanaConstants.LoggedInAsGuest = false;
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
        Web3AuthCustom.Instance.logout();
        if (LoadingHandler.Instance.nftLoadingScreen.activeInHierarchy)
        {
            LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
        }
        //[Waqas] Reset Guest Username After Delete All
        PlayerPrefs.SetString("publicID", "");
        PlayerPrefs.Save();
        //UserPassManager.Instance.testing = false; // Forces Enabled
        if (FeedUIController.Instance.SNSSettingController != null)
        {
            FeedUIController.Instance.SNSSettingController.LogoutSuccess();
        }
        yield return StartCoroutine(LoginGuest(ConstantsGod.API_BASEURL + ConstantsGod.guestAPI, true));
        ConstantsGod.UserRoles = new List<string>() { "Guest" };
        if (InventoryManager.instance.MultipleSave)
        {
            LoadPlayerAvatar.instance_loadplayer.avatarButton.gameObject.SetActive(false);
        }
        LoadingHandler.Instance.characterLoading.gameObject.SetActive(false);
        LoadingHandler.Instance.HideLoading();
        ConstantsHolder.xanaConstants.isCameraMan = false;
        ConstantsHolder.xanaConstants.IsDeemoNFT = false;
        InventoryManager.instance.CheckWhenUserLogin();
        if (ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            LoginRegisterScreen.SetActive(true);
           // signUpOrloginSelectionPanel.SetActive(true);
        }
        else
        {
            signUpOrloginSelectionPanel.SetActive(true);
        }
        //signUpOrloginSelectionPanel.SetActive(true);
        if (_web3APIforWeb2._OwnedNFTDataObj != null)
        {
            _web3APIforWeb2._OwnedNFTDataObj.ClearAllLists();
        }

        yield return null;
    }

    public void RequestSubmitUsername(string unique_Name)
    {
        StartCoroutine(IERequestSubmitUsername(unique_Name));
    }

    public IEnumerator IERequestSubmitUsername(string unique_Name)
    {
        WWWForm form = new WWWForm();

        UserProfile userProfile = new UserProfile
        {
            username = unique_Name,
        };

        string jsonData = JsonUtility.ToJson(userProfile);
        string apiUrl = ConstantsGod.API_BASEURL + ConstantsGod.r_url_UpdateUserProfile;

        using (UnityWebRequest www = new UnityWebRequest(apiUrl, "POST"))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);

            byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            www.SendWebRequest();
            while (!www.isDone)
            {
                yield return null;
            }
            string bykeyLocalize;
            UniqueUserNameError APIResponse = JsonConvert.DeserializeObject<UniqueUserNameError>(www.downloadHandler.text);
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) //(www.result.isNetworkError || www.isHttpError)
            {
                Debug.Log("<color=red> ------Edit NormalAPI Error " + www.error + www.downloadHandler.text + "</color>");

                if (APIResponse.msg.Contains("Username"))
                {
                    bykeyLocalize = TextLocalization.GetLocaliseTextByKey("The username must not include Space. Alphabet, Numbers, or Underscore allowed.");
                    UserDisplayNameErrors(bykeyLocalize);


                }
            }
            else if (!APIResponse.success)
            {
                if (APIResponse.msg.Contains("Username"))
                {

                    bykeyLocalize = TextLocalization.GetLocaliseTextByKey("Username already exists");
                    UserDisplayNameErrors(bykeyLocalize);


                }
            }
            else if (APIResponse.success)
            {
               
                if (!ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
                {
                    OpenUIPanel(16);
                    EditProfilePanel.SetActive(true);
                    NameScreenLoader.SetActive(false);
                    NameScreenNextButton.interactable = true;
                }
                else {
                    OpenUIPanel(16);
                    NameScreenLoader.SetActive(false);
                    NameScreenNextButton.interactable = true;
                    if (ConstantsHolder.xanaConstants.openLandingSceneDirectly)
                        MainSceneEventHandler.OpenLandingScene?.Invoke();
                    
                }


            }



        }
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
    public class UserProfile
    {
        public string username;
    }
    [Serializable]
    class UniqueUserNameError
    {
        public bool success;
        public string msg;
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

    #region Work for Pick ProfilePicFromGallery 

    public void OnClickChangeProfilePicButton()
    {
        //  mainFullScreenContainer.SetActive(false);//fo disable profile screen post part.......
        PickImageOptionScreen.SetActive(true);
    }

    public void CheckPermissionStatus(int maxSize)
    {
        if (Application.isEditor)
        {
            permissionPopup.SetActive(true);
        }
        else
        {
            NativeGallery.Permission permission = NativeGallery.CheckPermission(NativeGallery.PermissionType.Read, NativeGallery.MediaType.Image);
#if UNITY_ANDROID
            if (permission == NativeGallery.Permission.ShouldAsk) //||permission == NativeCamera.Permission.ShouldAsk
            {
                permissionPopup.SetActive(true);
            }
            else
            {
                OnPickImageFromGellery(maxSize);
            }
#elif UNITY_IOS
                if(PlayerPrefs.GetInt("PicPermission", 0) == 0){
                     permissionPopup.SetActive(true);
                }
                else
                {
                    OnPickImageFromGellery(maxSize);
                }
#endif

        }
    }

    public void OnPickImageFromGellery(int maxSize)
    {
#if UNITY_IOS
        PlayerPrefs.SetInt("PicPermission", 1);

        if (PermissionCheck == "false")
        {
            string url = MyNativeBindings.GetSettingsURL();
            Debug.Log("the settings url is:" + url);
            Application.OpenURL(url);
        }
        else
        {
            iOSCameraPermission.VerifyPermission(gameObject.name, "SampleCallback");
        }
          SetProfileAvatarTempPath = "";
        SetProfileAvatarTempFilename = "";
        //setGroupFromCamera = false;

        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                if (PickImageOptionScreen.activeSelf)//false meadia option screen.
                {
                    PickImageOptionScreen.SetActive(false);
                }

                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize, false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                //setGroupTempAvatarTexture = texture;

               Debug.Log("OnPickGroupAvatarFromGellery path: " + path);

                //string[] pathArry = path.Split('/');

                //string fileName = pathArry[pathArry.Length - 1];
                string fileName = Path.GetFileName(path);
               Debug.Log("OnPickGroupAvatarFromGellery FileName: " + fileName);

                string[] fileNameArray = fileName.Split('.');
                string str = DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".";
                fileName = fileNameArray[0] + str + fileNameArray[1];

                SetProfileAvatarTempPath = Path.Combine(Application.persistentDataPath, "UserProfilePic", fileName); ;
                SetProfileAvatarTempFilename = fileName;

                CropProfilePic(texture,  SetProfileAvatarTempPath);

                //editProfileImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            }
        });
        Debug.Log("Permission result: " + permission);
       
#elif UNITY_ANDROID
        SetProfileAvatarTempPath = "";
        SetProfileAvatarTempFilename = "";
        //setGroupFromCamera = false;

        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                if (PickImageOptionScreen.activeSelf)//false meadia option screen.
                {
                    PickImageOptionScreen.SetActive(false);

                }

                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize, false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                //setGroupTempAvatarTexture = texture;

                Debug.Log("OnPickGroupAvatarFromGellery path: " + path);

                //string[] pathArry = path.Split('/');

                //string fileName = pathArry[pathArry.Length - 1];
                string fileName = Path.GetFileName(path);
                Debug.Log("OnPickGroupAvatarFromGellery FileName: " + fileName);

                string[] fileNameArray = fileName.Split('.');
                string str = DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".";
                fileName = fileNameArray[0] + str + fileNameArray[1];

                SetProfileAvatarTempPath = Path.Combine(Application.persistentDataPath, "UserProfilePic", fileName); ;
                SetProfileAvatarTempFilename = fileName;

                CropProfilePic(texture, SetProfileAvatarTempPath);

            }
        });

        if (permission != NativeGallery.Permission.Granted)
        {
            using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                string packageName = currentActivityObject.Call<string>("getPackageName");

                using (var uriClass = new AndroidJavaClass("android.net.Uri"))
                using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
                using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject))
                {
                    intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                    intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                    currentActivityObject.Call("startActivity", intentObject);
                }
            }
        }
        Debug.Log("Permission result: " + permission);
#endif
    }

    public void CropProfilePic(Texture2D LoadedTexture, string path)
    {
        // If image cropper is already open, do nothing
        if (ImageCropper.Instance.IsOpen)
            return;

        StartCoroutine(_setImageProfilePicCropper(LoadedTexture, path));

        //Invoke("ProfilePostPartShow", 1f);
    }

    private IEnumerator _setImageProfilePicCropper(Texture2D screenshot, string path)
    {
        yield return new WaitForEndOfFrame();

        bool ovalSelection = true;
        bool autoZoom = true;

        float minAspectRatio = 1, maxAspectRatio = 1;

        ImageCropper.Instance.Show(screenshot, (bool result, Texture originalImage, Texture2D croppedImage) =>
        {
            // If screenshot was cropped successfully
            if (result)
            {
                Sprite s = Sprite.Create(croppedImage, new Rect(0, 0, croppedImage.width, croppedImage.height), new Vector2(0, 0), 1);
                EditProfileImage.sprite = s;

                try
                {
                    byte[] bytes = croppedImage.EncodeToPNG();
                    File.WriteAllBytes(path, bytes);
                    Debug.Log("File SAVE");
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
            else
            {
                //Debug.Log("--------Image not cropped");
                SetProfileAvatarTempPath = "";
                //croppedImageHolder.enabled = false;
                //croppedImageSize.enabled = false;
            }
            // Destroy the screenshot as we no longer need it in this case
            Destroy(screenshot);
            Resources.UnloadUnusedAssets();
            Caching.ClearCache();


        },
        settings: new ImageCropper.Settings()
        {
            ovalSelection = ovalSelection,
            autoZoomEnabled = autoZoom,
            imageBackground = Color.clear, // transparent background
            selectionMinAspectRatio = minAspectRatio,
            selectionMaxAspectRatio = maxAspectRatio,
            markTextureNonReadable = false
        },
        croppedImageResizePolicy: (ref int width, ref int height) =>
        {
            // uncomment lines below to save cropped image at half resolution
            //width /= 2;
            //height /= 2;
        });
    }

    public IEnumerator EditProfilePic()
    {
        if (string.IsNullOrEmpty(SetProfileAvatarTempPath))
        {
            EditProfilePanel.SetActive(false);
        }
        else
        {
            ProfilePicNextButton.interactable = false;
            ProfilePicScreenLoader.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            AWSHandler.Instance.PostAvatarObject(SetProfileAvatarTempPath, SetProfileAvatarTempFilename, "SignupProfilePicUpload");//upload avatar image on AWS.
        }
    }

    public void UpdateProfilePic()
    {
        StartCoroutine(EditProfilePic());

        if (ConstantsHolder.xanaConstants.openLandingSceneDirectly)
            MainSceneEventHandler.OpenLandingScene?.Invoke();
    }

    public void RequestUpdateUserProfilePic(string user_avatar, string callingFrom)
    {
        if(UserLoginSignupManager.instance!=null)
        StartCoroutine(IERequestUpdateUserProfilePic(user_avatar, callingFrom));
    }

    public IEnumerator IERequestUpdateUserProfilePic(string user_avatar, string callingFrom)
    {
        WWWForm form = new WWWForm();

        form.AddField("avatar", user_avatar);

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_UpdateUserAvatar), form))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);

            www.SendWebRequest();
            while (!www.isDone)
            {
                ProfilePicNextButton.interactable = true;
                ProfilePicScreenLoader.SetActive(false);
                yield return null;
            }

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
                ProfilePicNextButton.interactable = true;
                ProfilePicScreenLoader.SetActive(false);
            }
            else
            {

                string data = www.downloadHandler.text;
                ProfilePicNextButton.interactable = true;
                ProfilePicScreenLoader.SetActive(false);
                EditProfilePanel.SetActive(false);

            }
        }
    }
    public void SampleCallback(string permissionWasGranted)
    {
        Debug.Log("Callback.permissionWasGranted = " + permissionWasGranted);

        if (permissionWasGranted == "true")
        {
            // You can now use the device camera.
        }
        else
        {
            PermissionCheck = permissionWasGranted;

            // permission denied, no access should be visible, when activated when requested permission
            return;

            // You cannot use the device camera.  You may want to display a message to the user
            // about changing the camera permission in the Settings app.
            // You may want to re-enable the button to display the Settings message again.
        }
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

