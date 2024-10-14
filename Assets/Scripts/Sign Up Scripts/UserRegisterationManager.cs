using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Mail;
using UnityEngine.Networking;
using System;
using System.Text;
using Newtonsoft.Json;
using TMPro;

using Sign_Up_Scripts;
using UnityEngine.EventSystems;
using System.IO;

using AdvancedInputFieldPlugin;
using System.Linq;
using System.Threading.Tasks;


public class UserRegisterationManager : MonoBehaviour
{

    public static UserRegisterationManager instance;
    private string XanaliaAPI = "https://api.xanalia.com/user/my-collection";
    [Header("Total-Panal")]
    public GameObject FirstPanal;
    //public GameObject EmailPanal;

    public GameObject OTPPanal;
    public GameObject PasswordPanal;
    public GameObject usernamePanal;
    public GameObject LoginPanal;
    public GameObject SignUpPanal;
    //public GameObject SignUpPanalwithPhone;
    //public GameObject ChangePasswordPanal;
    //public GameObject UpdateprofilePanal;
    //public GameObject TestingAPIsPanal;
    public GameObject RegistrationCompletePanal;
    public GameObject ForgetenterUserNamePanal;
    public GameObject ForgetEnterPasswordPanal;
    public GameObject LogoutfromOtherDevicePanel;
    public GameObject BlackScreen;
    public GameObject validationMessagePopUP;
    bool passwordBool = false;
    bool emailBool = false;
    //Waheed Changes
    //public GameObject setAvatarGiftPanal;

    //hardik changes
    public string nftlist;
    //end
    public Color NormalColor;
    public Color HighlightedColor;
    [Header("SignUp-InputFields")]
    //  public MobileInputField EmailInputTextNew;
    public AdvancedInputField EmailFieldNew;
    //   public MobileInputField UsernameTextNew;
    public AdvancedInputField UsernameFieldAdvance;
    //  public AdvancedInputField Username2FieldAdvance;
    public Text CountryCodeText;
    // public MobileInputField PhoneInputTextNew;      
    public AdvancedInputField PhoneFieldNew;
    [Header("Password-InputFields")]
    public ShiftCode Password1InputTextShiftCode;
    public ShiftCode Password2ConfirmInputTextShiftCode;
    public AdvancedInputField Password1New;
    public AdvancedInputField Password2New;

    [Header("OTP-InputFields")]
    // public List<MobileInputField> pinNew;
    //  public MobileInputField mainfield_for_opt;
    public AdvancedInputField mainfieldOTPNew;
    public Text[] text_to_show;
    public Image[] image_to_Change;
    public Sprite OTPbox_highlighter;
    public Sprite oldOTP_Box;
    [Space(5)]
    [Header("Login-InputFields")]
    // public MobileInputField LoginEmailNew;
    public ShiftCode LoginPasswordShiftCode;
    public AdvancedInputField LoginEmailOrPhone;
    public AdvancedInputField LoginPassword;

    [Space(5)]
    [Header("ForgetPassword-InputFields")]
    // public MobileInputField EmailOrPhone_ForgetPasswrod;
    public AdvancedInputField EmailOrPhone_Forget_NewField;
    //  public MobileInputField Password1_ForgetPasswrod;
    // public MobileInputField Standard1_ForgetPasswrod;
    public AdvancedInputField Password1_ForgetNewField;
    public AdvancedInputField Password2_ForgetNewField;
    //  public MobileInputField Password2_ForgetPasswrod;
    //   public MobileInputField Standard2_ForgetPasswrod;
    public ShiftCode InputTextShiftCodeChangePass;
    public ShiftCode InputTextShiftCodeChangePass2;
    [Space(5)]
    [Header("Change Password-InputFields")]
    // public MobileInputField OldPasswordField;
    // public MobileInputField ChangePassword1;
    // public MobileInputField ChangePassword2;
    [Space(5)]
    [Header("Update Profile-InputFields")]
    //public MobileInputField GenderField;
    //public MobileInputField JobField;
    //public MobileInputField CountryField;
    // public MobileInputField BioField;
    [Space(10)]
    [Header("Error Texts GameObjects")]
    public GameObject errorTextEmail;
    public GameObject errorTextPassword;
    public GameObject errorTextNumber;
    public GameObject errorTextName;
    public GameObject errorTextPIN;
    public GameObject errorTextLogin;
    public GameObject errorTextForgetAPI;
    public GameObject errorTextResetPasswordAPI;
    public ErrorHandler errorHandler;

    public List<string> myData;
    string Email;
    string ForgetPasswordEmlOrPhnContainer;
    private string LocalPhoneNumber;
    string password;
    string Username;
    private bool isCheckingOTP;
    string CurrentOTP;
    [HideInInspector]
    public bool LoggedIn;
    string ForgetPasswordTokenAfterVerifyling;
    [Header("Flow Bools")]
    [HideInInspector]
    public bool SignUpWithPhoneBool;
    bool SignUpWithEmailBool;
    bool BackBool;
    private bool ResendOTPBool;
    private bool OTPFieldBool;
    bool ForgetPasswordBool;
    private bool mobile_number;


    [Header("SignUp Tab Selector UI")]
    public GameObject phoneTab;
    public GameObject emailTab;
    public GameObject numberScreen;
    public GameObject WalletScreen;
    public Image WalletSelectedImg;
    public Image PhoneSelectedImg;
    public Image EmailSelectedImg;
    public Image PhoneSelectedImgPos2;
    public Image EmailSelectedImgPos2;
    public GameObject emailScreen;
    public Text phoneTabText;
    public Text emailTabText;
    public Text WalletTabText;
    public GameObject phoneTabSelected;
    public GameObject emailTabSelected;
    public GameObject WalletTabSelected;



    [Tooltip("Pass Animator Component attached to the slider")]
    public Animator tabSelectorAnimator;

    public bool SavingBool;
    [HideInInspector]
    private Button currentSelectedNxtButton;
    public bool LoggedInAsGuest = false;
    public GameObject Name_Screen_Preset_Panel;
    public bool checkbool_preser_start;
    public string XanaliaUserTokenId = "";
    public userRoleScript userRoleObj;
    /// <Web 3.0 and Web 2.0>
    public bool XanaliaBool;
    public Web3Web2Handler _web3APIforWeb2;
    /// </Web 3.0 and Web 2.0>
    /// 
    public int btnClickedNo = 0;
    public void ShowCommingsoonPopup()
    {
        SNSNotificationHandler.Instance.ShowNotificationMsg("Coming soon");
    }

    #region WelcomeScreen
    public GameObject welcomeScreen;
    public bool shownWelcome;
    [Header("Premium user UI")]
    public GameObject PremiumUserUI;
    [Header("Moralis WorkFlow")]
    public GameObject MoralismainObj;
    public GameObject ConnectionEstablished_popUp;


    private bool _disposedValue;


    public GameObject EntertheWorld_Panal;
    public GameObject NewSignUp_Panal, LoginScreenNew, UsernamescreenLoader;
    public GameObject LogoImage, LogoImage2, LogoImage3;
    public RawImage renderImage;
    public TextMeshProUGUI UserNameSetter;
    public GameObject NewLoadingScreen;
    public Text _NewLoadingText;
    String _LoadingTitle = "";
    public bool _IsWalletSignUp = false;
    public int SignUpButtonSelected = 0;

    public void ShowWelcomeScreen()
    {
        if (!PlayerPrefs.HasKey("shownWelcome"))
        {

            if (PlayerPrefs.GetInt("IsProcessComplete") == 0)
            {
                welcomeScreen.SetActive(true);
                shownWelcome = true;
            }
        }
    }
    public void ShowWelcomeScreenessintial()
    {
        if (PlayerPrefs.GetInt("IsProcessComplete") == 0)
        {
            if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
            {
                welcomeScreen.SetActive(true);
            }
        }

    }
    public void ShowWelcomeClosed()
    {
        if (PlayerPrefs.GetInt("IsProcessComplete") == 1)
        {
            if (PlayerPrefs.GetInt("iSignup") == 1)
            {
                PlayerPrefs.SetInt("presetPanel", 1);
                DefaultClothDatabase.instance.GetComponent<SaveCharacterProperties>().SavePlayerProperties();
                InventoryManager.instance.OnSaveBtnClicked();  // reg complete go home
            }
        }
        else
        {

            welcomeScreen.SetActive(false);
            shownWelcome = false;
            if (!PlayerPrefs.HasKey("shownWelcome"))
            {
                InventoryManager.instance.StartPanel_PresetParentPanel.SetActive(true);
            }
        }

    }
    // if comming back from character screen 
    public void ShowWellComeCloseRetrack()
    {
        PlayerPrefs.DeleteKey("shownWelcome");
        PlayerPrefs.SetInt("iSignup", 0);
        checkbool_preser_start = true;
        ShowWelcomeScreen();

    }

    public void NewWalletSignUp()
    {
        _IsWalletSignUp = true;
    }

    public void WalletLoginCheck()
    {
        _IsWalletSignUp = false;

        LoginPanal.SetActive(false);
    }

    public void NextScreenAfterWalletConnected()
    {
        Debug.LogError("here comes");
        if (_IsWalletSignUp)
        {
            iwanto_signUp();
        }
    }

    public void iwanto_signUp()
    {
        PlayerPrefs.SetInt("iSignup", 1);

        if (PlayerPrefs.GetInt("CloseLoginScreen") == 0)
        {
            PlayerPrefs.SetInt("CloseLoginScreen", 1);
            PlayerPrefs.SetInt("iSignup", 1);
            PlayerPrefs.SetInt("IsProcessComplete", 1);
            PlayerPrefs.SetInt("shownWelcome", 1);
        }
        InventoryManager.instance.StartPanel_PresetParentPanel.SetActive(true);
    }

    //private void Awake()
    //{
    //    int x = ReturnNftRole("Free");
    //    checkbool_preser_start = true;
    //    _web3APIforWeb2 = this.gameObject.GetComponent<Web3Web2Handler>();
    //    instance = this;
    //    if (!File.Exists(GameManager.Instance.GetStringFolderPath()))
    //    {
    //        SaveCharacterProperties.instance.CreateFileFortheFirstTime();
    //    }
    //    if (!PlayerPrefs.HasKey("iSignup"))
    //    {
    //        PlayerPrefs.SetInt("iSignup", 0);
    //        PlayerPrefs.SetInt("IsProcessComplete", 0); // check if guest or signup process is complete or not 
    //    }
    //}




    public void CloseLoginScreen()
    {
        if (PlayerPrefs.GetInt("shownWelcome") == 0 && PlayerPrefs.GetInt("CloseLoginScreen") == 0)
        {
            welcomeScreen.SetActive(true);
            LoginScreenNew.SetActive(false);

        }
        else
        {
            LoginScreenNew.SetActive(false);
        }
    }

    private void OnDisable()
    {
        Web3Web2Handler.AllDataFetchedfromServer -= eventcalled;
    }

    public void BacktoAvatarSelectionPanel()
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

    public void LoginScreenClicked(int btn)
    {
        btnClickedNo = btn;
    }

    public void BackFromLoginScreen()
    {
        if (btnClickedNo == 0)
        {
            welcomeScreen.SetActive(false);
            LoginScreenNew.SetActive(true);
        }

        if (btnClickedNo == 1)
        {
            NewSignUp_Panal.SetActive(true);
        }
        LoginEmailOrPhone.gameObject.GetComponent<InputFieldKeyboardClient>().enabled = false;
        LoginPassword.gameObject.GetComponent<InputFieldKeyboardClient>().enabled = false;
        LoginPanal.SetActive(false);
        if (chk_forAccountALreadyLogedin)
        {
            chk_forAccountALreadyLogedin = false;
            ShowWelcomeScreen();

        }
    }

    private async void eventcalled(string _userType)
    {
        if (_userType == "Web3")
        {

            if (CryptouserData.instance.CryptoLogin)
            {
                GetOwnedNFTsFromAPI();
                ConstantsGod.AUTH_TOKEN = PlayerPrefs.GetString("LoginToken");
                LoginWithMoralisSDK();
                userRoleObj.userNftRoleSlist.Add("free");
                StartCoroutine(WalletLoggedInAccessGroup());
            }
        }
        else if (_userType == "Web2")
        {
            if (_web3APIforWeb2._OwnedNFTDataObj.NFTlistdata.count > 0)
            {
                await _web3APIforWeb2._OwnedNFTDataObj.FillAllListAsyncWaiting();

                if (_web3APIforWeb2._OwnedNFTDataObj._NFTIDs.Contains(PlayerPrefs.GetInt("nftID")))
                {
                    if (PlayerPrefs.HasKey("Equiped"))
                    {
                        ConstantsHolder.xanaConstants.isNFTEquiped = true;
                        BoxerNFTEventManager.OnNFTequip?.Invoke(false);
                    }
                }
                else
                {
                    PlayerPrefs.DeleteKey("Equiped");
                    PlayerPrefs.DeleteKey("nftID");
                    ConstantsHolder.xanaConstants.isNFTEquiped = false;
                    BoxerNFTEventManager.OnNFTUnequip?.Invoke();
                    LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
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
    public void GetOwnedNFTsFromAPI()
    {
        _web3APIforWeb2.GetWeb2UserData(PlayerPrefs.GetString("publicID"));
    }

    public void LoginWithMoralisSDK(bool auto = false)
    {
        PlayerPrefs.SetInt("IsLoggedIn", 1);
        PlayerPrefs.SetInt("FristPresetSet", 1);
        PlayerPrefs.SetInt("WalletLogin", 1);
        PlayerPrefs.SetInt("IsLoggedIn", 1);
        getdatafromserver();
        if (!auto)
        {
            OpenUIPanal(7);
            ConnectionEstablished_popUp.SetActive(true);

            Invoke(nameof(showPresetPanel), 1f);
            //DynamicEventManager.deepLink?.Invoke("Moralis side");

            if (InventoryManager.instance != null)
                InventoryManager.instance.WalletLoggedinCall();
        }
        else
        {
            StartCoroutine(WaitForDeepLink());
        }
        LoggedInAsGuest = false;
        usernamePanal.SetActive(false);
        PlayerPrefs.Save();
        if (GameManager.Instance.UiManager != null)//rik
        {
            GameManager.Instance.UiManager._footerCan.transform.GetChild(0).GetComponent<HomeFooterHandler>().HomeSceneFooterSNSButtonIntrectableTrueFalse();
            GameManager.Instance.UiManager._footerCan.transform.GetChild(0).GetComponent<HomeFooterHandler>().CheckLoginOrNotForFooterButton();
        }
    }
    IEnumerator WaitForDeepLink()
    {
        yield return new WaitForSeconds(2);
        //DynamicEventManager.deepLink?.Invoke("moralis wait and come");
    }
    /// <summary>
    /// To show preset panel if playe login form wallet and on perset is applied
    /// </summary>
    public void showPresetPanel()
    {
        if (File.Exists(GameManager.Instance.GetStringFolderPath()) && File.ReadAllText(GameManager.Instance.GetStringFolderPath()) != "")
        {
            SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();
            _CharacterData = _CharacterData.CreateFromJSON(File.ReadAllText(GameManager.Instance.GetStringFolderPath()));
            if (_CharacterData.myItemObj.Count <= 0)
            {
                PlayerPrefs.SetInt("presetPanel", 1);
                InventoryManager.instance.StartPanel_PresetParentPanel.SetActive(true);
            }
        }
    }

    // Start is called before the first frame update
    //void Start()
    //{
    //    UserNFTlistClass.AllDataFetchedfromServer += eventcalled;
    //    Web3Web2Handler.AllDataFetchedfromServer += eventcalled;

    //    mainfieldOTPNew.OnValueChanged.AddListener(delegate { ValueChangeCheck(); });

    //    BackBool = false;
    //    UIHandler.Instance.LoginRegisterScreen = FirstPanal;
    //    UIHandler.Instance.SignUpScreen = SignUpPanal;
    //    CountryCodeText.text = "+81";
    //    mobile_number = false;

    //    if (PlayerPrefs.GetInt("IsLoggedIn") == 1 && PlayerPrefs.GetInt("WalletLogin") != 1)
    //    {
    //        MyClassOfLoginJson LoginObj = new MyClassOfLoginJson();
    //        LoginObj = LoginObj.CreateFromJSON(PlayerPrefs.GetString("UserNameAndPassword"));
    //        StartCoroutine(LoginUserWithNewT(ConstantsGod.API_BASEURL + ConstantsGod.LoginAPIURL, PlayerPrefs.GetString("UserNameAndPassword"), null, true));
    //        LoggedInAsGuest = false;
    //    }
    //    else if (PlayerPrefs.GetInt("WalletLogin") == 1)
    //    {
    //        PlayerPrefs.SetInt("IsLoggedIn", 1);
    //        PlayerPrefs.SetInt("FristPresetSet", 1);
    //        Debug.LogError(PlayerPrefs.GetString("LoginToken"));
    //        ConstantsGod.AUTH_TOKEN = PlayerPrefs.GetString("LoginToken");
    //        LoggedInAsGuest = false;
    //        InventoryManager.instance.WalletLoggedinCall();
    //        LoginWithMoralisSDK(true);
    //        StartCoroutine(WalletLoggedInAccessGroup(true));
    //        LoadingHandler.Instance.nftLoadingScreen.SetActive(true);
    //    }
    //    else
    //    {

    //        LoggedInAsGuest = true;
    //        GameManager.Instance.mainCharacter.GetComponent<AvatarController>().IntializeAvatar();
    //        SaveCharacterProperties.instance.LoadMorphsfromFile();
    //        StartCoroutine(LoginGuest(ConstantsGod.API_BASEURL + ConstantsGod.guestAPI));
    //    }

    //    EyesBlinking.instance.StoreBlendShapeValues();          // Added by Ali Hamza
    //    StartCoroutine(EyesBlinking.instance.BlinkingStartRoutine());
    //    if (PlayerPrefs.GetInt("IsProcessComplete") == 0 && PlayerPrefs.GetInt("IsLoggedIn") == 0)
    //        welcomeScreen.SetActive(true);
    //}

    void CheckCameraMan()
    {
        MyClassOfLoginJson LoginObj = new MyClassOfLoginJson();
        LoginObj = LoginObj.CreateFromJSON(PlayerPrefs.GetString("UserNameAndPassword"));
        if (LoginObj.email.Contains("xanacameraman@yopmail.com" /*"xanavip1@gmail.com"*/))
        {
            ConstantsHolder.xanaConstants.isCameraMan = true;

        }
        else
        {
            ConstantsHolder.xanaConstants.isCameraMan = false;
        }
    }


    IEnumerator WalletLoggedInAccessGroup(bool loadData = false)
    {
        if (loadData)
        {
            GetOwnedNFTsFromAPI();
            yield return new WaitForSeconds(.1f);
        }
        yield return new WaitForSeconds(.8f);
        if (userRoleObj.userNftRoleSlist.Count > 0)
        {
            int x = (int)NftRolePriority.guest;
            string userNftRole = "free";
            ConstantsGod.UserRoles = userRoleObj.userNftRoleSlist;
            foreach (string s in userRoleObj.userNftRoleSlist)
            {
                int rolePriority = ReturnNftRole(s);
                if (rolePriority <= x)
                {
                    x = rolePriority;
                    ConstantsGod.UserPriorityRole = s;
                }
                userNftRole = s.ToLower();
                //print("Hey role is " + userNftRole);

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
    }

    #region Sign Up Animations

    public void OnSignUpEmailTabPressed()
    {
        float r = 0.2f, g = 0.3f, b = 0.7f, a = 0.6f;
        if (emailScreen.activeInHierarchy)
            return;

        emailScreen.SetActive(true);
        numberScreen.SetActive(false);
        WalletScreen.SetActive(false);

        EmailFieldNew.Text = "";
        if (ConnectWallet.instance.walletFunctionalitybool)
        {
            StartCoroutine(Animate(EmailSelectedImg.rectTransform));
        }
        else
        {
            StartCoroutine(Animate(EmailSelectedImgPos2.rectTransform));
        }
        emailTabText.gameObject.GetComponent<Text>().color = HighlightedColor;
        phoneTabText.gameObject.GetComponent<Text>().color = NormalColor;
        WalletTabText.gameObject.GetComponent<Text>().color = NormalColor;
        emailTabSelected.SetActive(true);
        phoneTabSelected.SetActive(false);
        WalletTabSelected.SetActive(false);
    }

    public void OnSignUpPhoneTabPressed()
    {
        //////Debug.Log("before if");
        PhoneFieldNew.Text = "";
        if (numberScreen.activeInHierarchy)
            return;
        emailScreen.SetActive(false);
        numberScreen.SetActive(true);
        WalletScreen.SetActive(false);
        PhoneFieldNew.Text = "";
        if (ConnectWallet.instance.walletFunctionalitybool)
        {
            StartCoroutine(Animate(PhoneSelectedImg.rectTransform));
        }
        else
        {
            StartCoroutine(Animate(PhoneSelectedImgPos2.rectTransform));
        }
        phoneTabText.gameObject.GetComponent<Text>().color = HighlightedColor;
        emailTabText.gameObject.GetComponent<Text>().color = NormalColor;
        WalletTabText.gameObject.GetComponent<Text>().color = NormalColor;
        emailTabSelected.SetActive(false);
        phoneTabSelected.SetActive(true);
        WalletTabSelected.SetActive(false);
    }
    public void OnSignUpWalletTabPressed()
    {
        emailScreen.SetActive(false);
        numberScreen.SetActive(false);
        StartCoroutine(Animate(WalletSelectedImg.rectTransform));
        WalletTabText.gameObject.GetComponent<Text>().color = HighlightedColor;
        emailTabText.gameObject.GetComponent<Text>().color = NormalColor;
        phoneTabText.gameObject.GetComponent<Text>().color = NormalColor;
        emailTabSelected.SetActive(false);
        phoneTabSelected.SetActive(false);
    }
    private IEnumerator Animate(RectTransform targetPos)
    {
        if (GameManager.Instance.mainCharacter)
        {
            GameManager.Instance.mainCharacter.GetComponent<CharacterBodyParts>().DefaultTexture();
        }
        SaveCharacterProperties.instance.LoadMorphsfromFile();
        float t = 0;
        var target = tabSelectorAnimator.GetComponent<Image>().rectTransform.position;

        while (t < .5f)
        {
            tabSelectorAnimator.GetComponent<Image>().rectTransform.position = Vector3.Lerp(tabSelectorAnimator.GetComponent<Image>().rectTransform.position, targetPos.position, t * 3);
            t += Time.deltaTime;
            yield return null;
        }
    }
    #endregion
    public void SignUpMethodSelected(int btn)
    {
        SignUpButtonSelected = btn;
    }

    public void BackFtn(int Openbackint)
    {
        if (ForgetPasswordBool)
        {
            OpenUIPanal(14);
            ForgetPasswordBool = false;
        }
        else
        {
            OpenUIPanal(20);
        }
        image_to_Change[0].sprite = OTPbox_highlighter;
        image_to_Change[3].sprite = oldOTP_Box;
    }

    public void GoToRegistrationScreen(int R_Integer)
    {
        if (R_Integer == 9)
        {
            SignUpWithPhoneBool = true;
        }
        else if (R_Integer == 2)
        {
            SignUpWithPhoneBool = false;
        }
        OpenUIPanal(R_Integer);
    }

    IEnumerator WaitandActive()
    {
        yield return new WaitForSeconds(.01f);
    }
    TouchScreenKeyboard Currentkeyboard;


    // Invoked when the value of the text field changes.
    public void ValueChangeCheck()
    {
        image_to_Change[0].sprite = OTPbox_highlighter;
        string[] myOtpTxt = new string[text_to_show.Length];
        char[] charArr = new char[mainfieldOTPNew.Text.Length];
        charArr = mainfieldOTPNew.Text.ToCharArray();
        for (int i = 0; i < myOtpTxt.Length; i++)
        {
            if (i == 0)
            {
                image_to_Change[0].sprite = OTPbox_highlighter;
            }
            if (i < charArr.Length)//1 2 3 4
            {
                myOtpTxt[i] = charArr[i].ToString();
                text_to_show[i].text = myOtpTxt[i].ToString();
                if (i < 3)
                {
                    image_to_Change[i + 1].sprite = OTPbox_highlighter;
                    image_to_Change[i].sprite = oldOTP_Box;
                }

            }
            else
            {
                myOtpTxt[i] = "";
                text_to_show[i].text = myOtpTxt[i].ToString();
                if (charArr.Length < 4) //1 2 3 
                {
                    if (charArr.Length != 3)
                    {
                        image_to_Change[charArr.Length + 1].sprite = oldOTP_Box;
                    }
                }

            }
        }
    }

    public void BackButtonPressedhere()
    {

    }

    public void NewBack()
    {
        LoginEmailOrPhone.gameObject.GetComponent<InputFieldKeyboardClient>().enabled = false;
        LoginPassword.gameObject.GetComponent<InputFieldKeyboardClient>().enabled = false;
        LoginPanal.SetActive(false);
        if (chk_forAccountALreadyLogedin)
        {
            chk_forAccountALreadyLogedin = false;
            ShowWelcomeScreen();

        }
        else

            OpenUIPanal(1);
    }
    bool chk_forAccountALreadyLogedin;
    public void OpenUIPanal(int ActivePanalCounter)
    {
        if (!checkbool_preser_start && PlayerPrefs.GetInt("WalletLogin") != 1)  // guest
        {
            PresetData_Jsons.clickname = null;
            InventoryManager.instance.StartPanel_PresetParentPanel.SetActive(true);
            usernamePanal.SetActive(false);

            if (GameManager.Instance.isStoreAssetDownloading)
            {
                LoadingHandler.Instance.presetCharacterLoading.gameObject.SetActive(true);
            }
            else
            {
                LoadingHandler.Instance.presetCharacterLoading.gameObject.SetActive(false);
            }

            return;
        }

        if (isSetXanaliyaUserName)
        {
            isSetXanaliyaUserName = false;
        }

        FirstPanal.SetActive(false);
        OTPPanal.SetActive(false);
        PasswordPanal.SetActive(false);
        usernamePanal.SetActive(false);
        LoginPanal.SetActive(false);
        SignUpPanal.SetActive(false);
        RegistrationCompletePanal.SetActive(false);
        ForgetenterUserNamePanal.SetActive(false);
        ForgetEnterPasswordPanal.SetActive(false);

        switch (ActivePanalCounter)
        {
            case 1:
                {
                    if (shownWelcome)
                    {
                        if (PlayerPrefs.GetInt("iSignup") == 1)
                        {
                            PlayerPrefs.SetInt("iSignup", 0);

                            welcomeScreen.SetActive(true);
                            shownWelcome = true;
                        }
                        else
                            ShowWelcomeClosed();
                    }
                    else
                    {
                        welcomeScreen.SetActive(true);
                    }
                    break;
                }
            case 2:
                {
                    SignUpPanal.SetActive(true);
                    if (!WalletScreen.activeInHierarchy)
                        OnSignUpPhoneTabPressed();
                    else
                        OnSignUpWalletTabPressed();
                    EmailFieldNew.Text = "";
                    PhoneFieldNew.Text = "";
                    break;
                }
            case 3:
                {
                    OTPPanal.SetActive(true);
                    mainfieldOTPNew.Text = "";
                    mainfieldOTPNew.Select();
                    for (int i = 0; i < text_to_show.Length; i++)
                    {
                        text_to_show[i].text = "";
                    }
                    break;
                }
            case 4:
                {
                    Invoke("blackscrreefalse", 0.2f);
                    this.gameObject.GetComponent<SplashVideoPlay>().OnAvatarSelectionPanal();
                    break;
                }
            case 5:
                {
                    UsernameFieldAdvance.Text = "";
                    break;
                }
            case 6:
                {

                    LoginPanal.SetActive(true);
                    LoginEmailOrPhone.Text = "";
                    LoginEmailOrPhone.Select();
                    savePasswordList.instance.DeleteONStart();
                    LoginPassword.Text = "";
                    chk_forAccountALreadyLogedin = true;
                    break;
                }
            case 7:
                {
                    if (shownWelcome)
                    {
                        PlayerPrefs.SetInt("shownWelcome", 1);
                        LoggedIn = true;
                    }
                    else
                    {
                        LoggedIn = true;
                    }
                    break;
                }
            case 8:
                {
                    Debug.Log("Signup here");
                    PlayerPrefs.SetInt("iSignup", 1);// going for register user
                    SignUpPanal.SetActive(true);
                    EmailFieldNew.Text = "";
                    Password1New.Text = "";
                    Password2New.Text = "";
                    break;
                }
            case 13:
                {

                    if (PlayerPrefs.GetInt("WalletLogin") != 1)
                    {
                        RegistrationCompletePanal.SetActive(true);
                        InventoryManager.instance.StartPanel_PresetParentPanel.SetActive(true);
                    }
                    if (shownWelcome)
                        ShowWelcomeClosed();
                    break;
                }
            case 14:
                {
                    ForgetPasswordTokenAfterVerifyling = "";
                    ForgetenterUserNamePanal.SetActive(true);
                    EmailOrPhone_Forget_NewField.Text = "";
                    break;
                }
            case 15:
                {
                    ForgetEnterPasswordPanal.SetActive(true);
                    Password1_ForgetNewField.Text = "";
                    Password2_ForgetNewField.Text = "";
                    break;
                }
            case 16:
                {
                    if (PlayerPrefs.GetInt("iSignup") == 1)
                    {
                        LoadingFadeOutScreen();
                    }
                    else
                    {
                        if (PlayerPrefs.GetInt("WalletLogin") != 1)
                        {
                            InventoryManager.instance.StartPanel_PresetParentPanel.SetActive(true);
                        }
                        if (shownWelcome)
                            ShowWelcomeClosed();
                    }
                    break;
                }
            case 17:
                {
                    if (shownWelcome)
                    {

                        if (PlayerPrefs.GetInt("iSignup") == 1)
                        {
                            PlayerPrefs.SetInt("iSignup", 0);

                            welcomeScreen.SetActive(true);
                            shownWelcome = true;
                        }
                        else
                            ShowWelcomeClosed();
                    }
                    else
                    {
                        FirstPanal.SetActive(true);
                    }
                    break;
                }

            case 18:
                {

                    PlayerPrefs.SetInt("iSignup", 1);// going for Wallet register user
                    break;
                }
            case 19:
                {
                    PlayerPrefs.SetInt("iSignup", 0);// going for guest user registration
                    ConstantsHolder.xanaConstants.LoginasGustprofile = true;
                    break;
                }

            case 20:
                {
                    if (!WalletScreen.activeInHierarchy)
                    {
                        if (SignUpButtonSelected == 1)
                        {
                            OnSignUpPhoneTabPressed();
                            PhoneFieldNew.Text = "";
                            Password1New.Text = "";
                            Password2New.Text = "";
                        }
                        else if (SignUpButtonSelected == 2)
                        {
                            OnSignUpEmailTabPressed();
                            EmailFieldNew.Text = "";
                            Password1New.Text = "";
                            Password2New.Text = "";
                        }
                        SignUpPanal.SetActive(true);
                    }
                    break;
                }
            case 21:
                {
                    LoginScreenNew.SetActive(true);
                    break;
                }
        }
    }
    /// <SignUpWithPhoneNumber>
    // DifferentAPI,s Call

    // Started Device Token 
    public void blackscrreefalse()
    {

        BlackScreen.SetActive(false);
    }
    string uniqueID2()
    {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int currentEpochTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
        int z1 = UnityEngine.Random.Range(0, 1000000);
        int z2 = UnityEngine.Random.Range(0, 1000000);
        string uid = currentEpochTime + ":" + z1 + ":" + z2;
        return uid;
    }
    public void SignUpCompletedPresetApplied()
    {
        //print("Waiting ... preset Applying");
        StartCoroutine(WaitPresetApplied());
    }
    IEnumerator WaitPresetApplied()
    {
        yield return new WaitForSeconds(3);
        if (PlayerPrefs.GetInt("RegistrationOnce") == 0)
        {
            if (PlayerPrefs.GetInt("IsProcessComplete") == 1 && PlayerPrefs.GetInt("iSignup") == 1)
            {
                PlayerPrefs.SetInt("RegistrationOnce", 1);
                //DynamicEventManager.deepLink?.Invoke("Sign Up Flow");
            }
        }
    }

    public void SubmitSetDeviceToken()
    {
        //  //print("submit Set device ID here");
        string l_DeivceID = uniqueID();
        // string l_DeivceID = PlayerPrefs.GetString("AppID2");
        MyClassForSettingDeviceToken myObject = new MyClassForSettingDeviceToken();
        string bodyJson = JsonUtility.ToJson(myObject.GetUpdatedDeviceToken(l_DeivceID)); ;
        StartCoroutine(HitSetDeviceTokenAPI(ConstantsGod.API_BASEURL + ConstantsGod.SetDeviceTokenAPI, bodyJson, l_DeivceID));

        // TokenAfterRegister
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
        MyClassNewApi myObject1 = new MyClassNewApi();
        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            myObject1 = CheckResponceJsonNewApi(request.downloadHandler.text);
            if (request.error == null)
            {
                if (myObject1.success)
                {
                    PlayerPrefs.SetString("DeviceToken", LocalGetDeviceID);
                }
            }
        }
        else
        {
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                ////Debug.Log("Network error in set device token");
            }
            else
            {
                if (request.error != null)
                {
                    if (!myObject1.success)
                    {
                        ////Debug.Log("Success false in  in set device token");
                    }
                }
            }
        }
    }




    [Serializable]
    public class MyClassForSettingDeviceToken : JsonObjectBase
    {
        public string deviceToken;
        public MyClassForSettingDeviceToken GetUpdatedDeviceToken(string L_deviceTkn)
        {
            MyClassForSettingDeviceToken myObj = new MyClassForSettingDeviceToken();
            myObj.deviceToken = L_deviceTkn;
            return myObj;
        }
    }
    IEnumerator WaitAndLogout()
    {
        if (FeedUIController.Instance.SNSSettingController != null)
        {
            FeedUIController.Instance.SNSSettingController.LogoutSuccess();
        }
        yield return null;
    }
    public IEnumerator HitLogOutAPI(string url, string Jsondata, Action<bool> CallBack)
    {
        LoadingHandler.Instance.characterLoading.gameObject.SetActive(true);
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
        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            myObject1 = CheckResponceJsonNewApi(request.downloadHandler.text);
            if (request.error == null)
            {
                CallBack(true);
                yield break;
            }
        }
        else
        {
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                //if (!ConnectionEstablished_popUp.activeInHierarchy)
                //{
                //    validationMessagePopUP.SetActive(true);
                //    errorTextPassword.SetActive(true);
                //    if (errorTextPassword.GetComponent<TextMeshProUGUI>())
                //    {
                //        errorTextPassword.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                //        // errorTextPassword.GetComponent<Animator>().SetBool("playAnim", true);
                //        //errorTextPassword.GetComponent<Text>().text = request.error.ToUpper();
                //        errorHandler.ShowErrorMessage(ErrorType.Poor_Connection.ToString(), errorTextPassword.GetComponent<TextMeshProUGUI>());
                //    }

                //}
            }
            else
            {
                if (request.error != null)
                {
                    myObject1 = CheckResponceJsonNewApi(request.downloadHandler.text);
                    if (!myObject1.success)
                    {
                        validationMessagePopUP.SetActive(true);
                        errorTextPassword.SetActive(true);
                        if (errorTextPassword.GetComponent<TextMeshProUGUI>())
                        {
                            errorTextPassword.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                            //  errorTextPassword.GetComponent<Animator>().SetBool("playAnim", true);
                            //errorTextPassword.GetComponent<Text>().text = myObject1.msg.ToUpper();
                            errorHandler.ShowErrorMessage(ErrorType.Default_Message.ToString(), errorTextPassword.GetComponent<TextMeshProUGUI>());
                        }
                        // StartCoroutine(WaitUntilAnimationFinished(errorTextPassword.GetComponent<Animator>()));
                    }
                }
            }
            LoadingHandler.Instance.characterLoading.gameObject.SetActive(false);
            LoadingHandler.Instance.HideLoading();
            InventoryManager.instance.CheckWhenUserLogin();
        }
        CallBack(false);
    }

    IEnumerator OnSucessLogout()
    {
        //--> remove for xana avatar2.0
        // BoxerNFTEventManager.OnNFTUnequip?.Invoke();
        if (_web3APIforWeb2._OwnedNFTDataObj != null)
        {
            _web3APIforWeb2._OwnedNFTDataObj.ClearAllLists();
        }

        PlayerPrefs.SetInt("IsLoggedIn", 0);
        PlayerPrefs.SetInt("WalletLogin", 0);
        userRoleObj.userNftRoleSlist.Clear();
        ConstantsGod.AUTH_TOKEN = null;
        ConstantsHolder.userId = null;
        ConstantsHolder.xanaConstants.LoginasGustprofile = false;

        PlayerPrefs.SetString("SaveuserRole", "");
        if (CryptouserData.instance != null)
        {
            CryptouserData.instance.UltramanPass = false;
            CryptouserData.instance.AlphaPass = false;
            CryptouserData.instance.AstroboyPass = false;
        }


        LoggedInAsGuest = true;

        yield return new WaitForSeconds(0.1f);
        resetClothstoGuest();

        PlayerPrefs.SetString("UserName", "");
        LoggedIn = false;
        ConstantsHolder.loggedIn = false;
        // [Waqas] Store Guest Username Locally
        string tempName1 = PlayerPrefs.GetString(ConstantsGod.GUSTEUSERNAME);
        string tempName2 = PlayerPrefs.GetString(ConstantsGod.PLAYERNAME);

        int simultaneousConnectionsValue = PlayerPrefs.GetInt("ShowLiveUserCounter");

        PlayerPrefs.DeleteAll();//Delete All PlayerPrefs After Logout Success.......
        PlayerPrefs.SetString("TermsConditionAgreement", "Agree");
        PlayerPrefs.SetInt("ShowLiveUserCounter", simultaneousConnectionsValue);

        //[Waqas] Reset Guest Username After Delete All
        PlayerPrefs.SetString(ConstantsGod.GUSTEUSERNAME, tempName1);
        PlayerPrefs.SetString(ConstantsGod.PLAYERNAME, tempName2);
        PlayerPrefs.SetString("publicID", "");



        PlayerPrefs.Save();
        //UserPassManager.Instance.testing = false; // Forces Enabled
        yield return StartCoroutine(WaitAndLogout());
        yield return StartCoroutine(LoginGuest(ConstantsGod.API_BASEURL + ConstantsGod.guestAPI, true));
        ConstantsGod.UserRoles = new List<string>() { "Guest" };
        if (InventoryManager.instance.MultipleSave)
            LoadPlayerAvatar.instance_loadplayer.avatarButton.gameObject.SetActive(false);

        LoadingHandler.Instance.characterLoading.gameObject.SetActive(false);
        LoadingHandler.Instance.HideLoading();
        ConstantsHolder.xanaConstants.isCameraMan = false;
        ConstantsHolder.xanaConstants.IsDeemoNFT = false;
        InventoryManager.instance.CheckWhenUserLogin();
    }


    public void ResetDataAfterLogoutSuccess()//rik
    {
        shownWelcome = false;
        PresetData_Jsons.clickname = "";
        if (InventoryManager.instance.PresetArrayContent)
        {
            for (int i = 0; i < InventoryManager.instance.PresetArrayContent.transform.childCount; i++)
            {
                InventoryManager.instance.PresetArrayContent.transform.GetChild(i).transform.GetChild(0).gameObject.SetActive(false);
            }
            InventoryManager.instance.PresetArrayContent.transform.parent.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
        }
        //end reset
        if (InventoryManager.instance != null)
        {
            InventoryManager.instance.GetComponent<RioPresetHandler>().turnAllPresetOff();
        }
    }

    void resetClothstoGuest()
    {
        GameManager.Instance.mainCharacter.GetComponent<AvatarController>().InitializeAvatar();
        SaveCharacterProperties.instance.LoadMorphsfromFile();
    }

    // Submit Logout
    public string GetDeviceToken()
    {
        if (PlayerPrefs.GetString("AppID2") == "")
        {
            SubmitSetDeviceToken();
            return null;
        }
        else
        {
            MyClassForSettingDeviceToken myObject = new MyClassForSettingDeviceToken();
            string bodyJson = JsonUtility.ToJson(myObject.GetUpdatedDeviceToken(uniqueID()));
            return bodyJson;
        }
    }

    // Submit GetUser Details        
    public void SubmitGetUserDetails()
    {
        StartCoroutine(HitGetUserDetails(ConstantsGod.API_BASEURL + ConstantsGod.GetUserDetailsAPI, ""));
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

    }

    public void DeleteAccount(Action callback)
    {
        string deviceToken = GetDeviceToken();
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
    }


    public void LogoutAccount()
    {
        string deviceToken = GetDeviceToken();
        if (!string.IsNullOrEmpty(deviceToken))
            StartCoroutine(HitLogOutAPI(ConstantsGod.API_BASEURL + ConstantsGod.LogOutAPI, deviceToken, (onSucess) =>
             {
                 if (onSucess)
                     StartCoroutine(OnSucessLogout());
             }
            ));
    }



    [System.Serializable]
    public class DeleteApiRes
    {
        public bool success;
        public string data;
        public string msg;
    }

    [System.Serializable]
    public class ClassforUserDetails : JsonObjectBase
    {
        public string success;
        public JsondataOfUserDetails data;
        public string msg;
        public ClassforUserDetails CreateFromJSON(string jsonString)
        {
            //  //print("Person " + jsonString);
            return JsonUtility.FromJson<ClassforUserDetails>(jsonString);
        }
    }

    [System.Serializable]
    public class JsondataOfUserDetails : JsonObjectBase
    {
        public string id;
        public string name;
        public string dob;
        public string phoneNumber;
        public string email;
        public string avatar;
        public string isVerified;
        public string isRegister;
        public string isDeleted;
        public string createdAt;
        public string updatedAt;
        public UserProfileForUserDetails userProfile;

        public static JsondataOfUserDetails CreateFromJSON(string jsonString)
        {
            //  //print("Person " + jsonString);
            return JsonUtility.FromJson<JsondataOfUserDetails>(jsonString);
        }
    }

    [System.Serializable]
    public class UserProfileForUserDetails : JsonObjectBase
    {
        public string id;
        public string userId;
        public string gender;
        public string job;
        public string country;
        public string bio;
        public string isDeleted;
        public string createdAt;
        public string updatedAt;
        public static JsondataOfUserDetails CreateFromJSON(string jsonString)
        {
            //  //print("Person " + jsonString);
            return JsonUtility.FromJson<JsondataOfUserDetails>(jsonString);
        }
    }

    IEnumerator HitGetUserDetails(string url, string Jsondata)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            request.SendWebRequest();
            while (!request.isDone)
            {
                yield return null;
            }
            ClassforUserDetails myObject1 = new ClassforUserDetails();
            if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
            {
                myObject1 = myObject1.CreateFromJSON(request.downloadHandler.text);
                if (request.error == null)
                {
                    if (myObject1.success == "true")
                    {
                        //   //print("Success of user details");
                    }
                }
            }
            else
            {
                if (request.isNetworkError)
                {
                    //if (!ConnectionEstablished_popUp.activeInHierarchy)
                    //{
                    //    validationMessagePopUP.SetActive(true);
                    //    errorTextPassword.SetActive(true);
                    //    if (errorTextPassword.GetComponent<TextMeshProUGUI>()) { 
                    //        errorTextPassword.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                    //    //  errorTextPassword.GetComponent<Animator>().SetBool("playAnim", true);
                    //    //errorTextPassword.GetComponent<Text>().text = request.error.ToUpper();
                    //    errorHandler.ShowErrorMessage(ErrorType.Poor_Connection.ToString(), errorTextPassword.GetComponent<TextMeshProUGUI>());
                    //}
                    //    // StartCoroutine(WaitUntilAnimationFinished(errorTextPassword.GetComponent<Animator>()));
                    //}
                }
                else
                {
                    if (request.error != null)
                    {
                        if (myObject1.success == "false")
                        {
                            validationMessagePopUP.SetActive(true);
                            errorTextPassword.SetActive(true);
                            if (errorTextPassword.GetComponent<TextMeshProUGUI>())
                            {
                                errorTextPassword.GetComponent<Text>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                                //      //print("Hey success false " + myObject1.msg);
                                //  errorTextPassword.GetComponent<Animator>().SetBool("playAnim", true);
                                //errorTextPassword.GetComponent<Text>().text = myObject1.msg.ToUpper();
                                errorHandler.ShowErrorMessage(myObject1.msg, errorTextPassword.GetComponent<TextMeshProUGUI>());
                            }
                            //   StartCoroutine(WaitUntilAnimationFinished(errorTextPassword.GetComponent<Animator>()));
                        }
                    }
                }
            }
        }
    }

    //Start Submit UpdateUserAvatar
    public void SubmitUpdateUserAvatar()
    {
        MyClassForUpdatingUserAvatar myObject = new MyClassForUpdatingUserAvatar();
        string bodyJson = JsonUtility.ToJson(myObject.GetUpdatedUserAvatar("updated avatar")); ;
        StartCoroutine(HitUpdateAvatarAPI(ConstantsGod.API_BASEURL + ConstantsGod.UpdateAvatarAPI, bodyJson));
    }

    [Serializable]
    public class MyClassForUpdatingUserAvatar : JsonObjectBase
    {
        public string avatar;
        public MyClassForUpdatingUserAvatar GetUpdatedUserAvatar(string L_userAvt)
        {
            MyClassForUpdatingUserAvatar myObj = new MyClassForUpdatingUserAvatar();
            myObj.avatar = L_userAvt;
            return myObj;
        }
    }

    IEnumerator HitUpdateAvatarAPI(string url, string Jsondata)
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
        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            myObject1 = CheckResponceJsonNewApi(request.downloadHandler.text);
            if (request.error == null)
            {
                if (myObject1.success)
                {
                    //    //print("Avatar Updated Success");
                }
            }
        }
        else
        {
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                //if (!ConnectionEstablished_popUp.activeInHierarchy)
                //{
                //    validationMessagePopUP.SetActive(true);
                //    errorTextPassword.SetActive(true);
                //    errorTextPassword.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                //    // errorTextPassword.GetComponent<Animator>().SetBool("playAnim", true);
                //    //errorTextPassword.GetComponent<Text>().text = request.error.ToUpper();
                //    errorHandler.ShowErrorMessage(ErrorType.Poor_Connection.ToString(), errorTextPassword.GetComponent<TextMeshProUGUI>());
                //    // StartCoroutine(WaitUntilAnimationFinished(errorTextPassword.GetComponent<Animator>()));
                //}
            }
            else
            {
                if (request.error != null)
                {
                    myObject1 = CheckResponceJsonNewApi(request.downloadHandler.text);
                    if (!myObject1.success)
                    {
                        validationMessagePopUP.SetActive(true);
                        errorTextPassword.SetActive(true);
                        //     //print("Hey success false " + myObject1.msg);
                        errorTextPassword.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                        //  errorTextPassword.GetComponent<Animator>().SetBool("playAnim", true);
                        //errorTextPassword.GetComponent<Text>().text = myObject1.msg.ToUpper();
                        errorHandler.ShowErrorMessage(ErrorType.Default_Message.ToString(), errorTextPassword.GetComponent<TextMeshProUGUI>());
                        // StartCoroutine(WaitUntilAnimationFinished(errorTextPassword.GetComponent<Animator>()));
                    }
                }
            }
        }
    }

    //END Submit UpdateUserAvatar

    // Submit ForgetPassword Section

    public void SubmitForgetPassword()
    {
        string ForgetPassword_EmlOrPhone = EmailOrPhone_Forget_NewField.Text;
        if (ForgetPassword_EmlOrPhone == "")
        {
            errorTextForgetAPI.GetComponent<Animator>().SetBool("playAnim", true);
            //  if (Application.systemLanguage == SystemLanguage.Japanese  )
            // {
            // errorTextForgetAPI.GetComponent<Text>().text = "全ての欄に入力してください";
            // }
            // else
            // {
            // errorTextForgetAPI.GetComponent<Text>().text = "Fields Should not be empty";
            // }
            errorHandler.ShowErrorMessage(ErrorType.Fields__empty.ToString(), errorTextForgetAPI.GetComponent<TextMeshProUGUI>());
            StartCoroutine(WaitUntilAnimationFinished(errorTextForgetAPI.GetComponent<Animator>()));
            return;
        }
        GameObject _ForgetPasswordBtnObject = EventSystem.current.currentSelectedGameObject;
        _ForgetPasswordBtnObject = _ForgetPasswordBtnObject.transform.Find("Loader").gameObject;

        if (_ForgetPasswordBtnObject.activeInHierarchy)
            return;
        _ForgetPasswordBtnObject.SetActive(true);

        string url = ConstantsGod.API_BASEURL + ConstantsGod.ForgetPasswordAPI;
        ForgetPassword_EmlOrPhone = ForgetPassword_EmlOrPhone.Trim();
        ForgetPassword_EmlOrPhone = ForgetPassword_EmlOrPhone.ToLower();
        MyClassOfPostingForgetPassword myObject = new MyClassOfPostingForgetPassword();
        string bodyJson;
        bodyJson = JsonUtility.ToJson(myObject.GetForgetPassworddata(ForgetPassword_EmlOrPhone));
        StartCoroutine(HitForgetPasswordAPI(url, bodyJson, ForgetPassword_EmlOrPhone, _ForgetPasswordBtnObject));
    }
    public IEnumerator HitForgetPasswordAPI(string url, string Jsondata, string localEmail_oR_PhoneNumber, GameObject _loader)
    {
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
        myObject1 = CheckResponceJsonNewApi(request.downloadHandler.text);
        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            if (request.error == null)
            {
                if (myObject1.success)
                {
                    if (_loader != null)
                        _loader.SetActive(false);
                    ForgetPasswordBool = true;
                    SignUpWithPhoneBool = false;
                    OpenUIPanal(3);
                    ForgetPasswordEmlOrPhnContainer = localEmail_oR_PhoneNumber;
                }
            }
        }
        else
        {
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                //if (!ConnectionEstablished_popUp.activeInHierarchy)
                //{
                //    if (_loader != null)
                //        _loader.SetActive(false);
                //    errorTextForgetAPI.GetComponent<Animator>().SetBool("playAnim", true);

                //    // if (Application.systemLanguage == SystemLanguage.Japanese  )
                //    // {
                //    //     errorTextForgetAPI.GetComponent<Text>().text = "接続状態が悪く繋がりません";
                //    // }
                //    // else
                //    // {
                //    //     errorTextForgetAPI.GetComponent<Text>().text = request.error.ToUpper();
                //    // }
                //    errorHandler.ShowErrorMessage(ErrorType.Poor_Connection.ToString(), errorTextForgetAPI.GetComponent<TextMeshProUGUI>());
                //    StartCoroutine(WaitUntilAnimationFinished(errorTextForgetAPI.GetComponent<Animator>()));
                //}
            }
            else
            {
                if (_loader != null)
                    _loader.SetActive(false);
                if (request.error != null)
                {
                    if (!myObject1.success)
                    {
                        errorTextForgetAPI.GetComponent<Animator>().SetBool("playAnim", true);
                        // if (Application.systemLanguage == SystemLanguage.Japanese  )
                        // {
                        //     //3: Email address is already exists
                        //     errorTextForgetAPI.GetComponent<Text>().text = "同じメールアドレスが登録されています";
                        // }
                        // else
                        // {
                        //     errorTextForgetAPI.GetComponent<Text>().text = myObject1.msg.ToUpper();
                        // }
                        //errorHandler.ShowErrorMessage(ErrorType.User_Does_Not_Exist_with_Email.ToString(), errorTextForgetAPI.GetComponent<Text>());
                        errorHandler.ShowErrorMessage(myObject1.msg, errorTextForgetAPI.GetComponent<TextMeshProUGUI>());
                        StartCoroutine(WaitUntilAnimationFinished(errorTextForgetAPI.GetComponent<Animator>()));
                    }
                }
            }
        }
    }
    private string NewPasswordForgetApi;

    public void SubmitResetPassword()
    {
        string NewPassword = Password1_ForgetNewField.Text;
        string ReNewPassword = Password2_ForgetNewField.Text;

        if (NewPassword == "" || ReNewPassword == "")
        {
            errorTextResetPasswordAPI.GetComponent<Animator>().SetBool("playAnim", true);
            // if (Application.systemLanguage == SystemLanguage.Japanese  )
            // {
            //     errorTextResetPasswordAPI.GetComponent<Text>().text = "全ての欄に入力してください";
            // }
            // else
            // {
            //     errorTextResetPasswordAPI.GetComponent<Text>().text = "Fields should not be empty";
            // }
            errorHandler.ShowErrorMessage(ErrorType.Fields__empty.ToString(), errorTextResetPasswordAPI.GetComponent<TextMeshProUGUI>());
            StartCoroutine(WaitUntilAnimationFinished(errorTextResetPasswordAPI.GetComponent<Animator>()));
            return;
        }
        if (NewPassword.Length < 8 || ReNewPassword.Length < 8)
        {
            errorTextResetPasswordAPI.GetComponent<Animator>().SetBool("playAnim", true);
            errorHandler.ShowErrorMessage(ErrorType.Passwords_cannot_less_than_eight_charcters.ToString(), errorTextResetPasswordAPI.GetComponent<TextMeshProUGUI>());
            StartCoroutine(WaitUntilAnimationFinished(errorTextResetPasswordAPI.GetComponent<Animator>()));
            return;
        }
        bool allCharactersInStringAreDigits = false;
        string specialCh = @"%!@#$%^&*()?/>.<,:;'\|}]{[_~`+=-" + "\"";
        char[] specialChArray = specialCh.ToCharArray();
        if (NewPassword.Any(char.IsDigit) && NewPassword.Any(char.IsLower) && NewPassword.Any(char.IsUpper) && !NewPassword.Any(char.IsWhiteSpace))
        {
            foreach (char ch in specialChArray)
            {
                if (NewPassword.Contains(ch))
                    allCharactersInStringAreDigits = true;
            }
        }
        if (!allCharactersInStringAreDigits)
        {
            errorTextResetPasswordAPI.GetComponent<Animator>().SetBool("playAnim", true);
            errorHandler.ShowErrorMessage(ErrorType.Password_must_Contain_Number.ToString(), errorTextResetPasswordAPI.GetComponent<TextMeshProUGUI>());
            StartCoroutine(WaitUntilAnimationFinished(errorTextResetPasswordAPI.GetComponent<Animator>()));
            return;
        }

        if (NewPassword != ReNewPassword)
        {
            errorTextResetPasswordAPI.GetComponent<Animator>().SetBool("playAnim", true);
            errorHandler.ShowErrorMessage(ErrorType.Passwords_do_not_match.ToString(), errorTextResetPasswordAPI.GetComponent<TextMeshProUGUI>());

            // if (Application.systemLanguage == SystemLanguage.Japanese  )
            // {
            //     errorTextResetPasswordAPI.GetComponent<Text>().text = "パスワードが一致していません";
            // }
            // else
            // {
            //     errorTextResetPasswordAPI.GetComponent<Text>().text = "Password not matched";
            // }
            errorHandler.ShowErrorMessage(ErrorType.Passwords_do_not_match.ToString(), errorTextResetPasswordAPI.GetComponent<TextMeshProUGUI>());

            StartCoroutine(WaitUntilAnimationFinished(errorTextResetPasswordAPI.GetComponent<Animator>()));
            return;
        }
        MyClassOfPostingReset myObject = new MyClassOfPostingReset();
        string bodyJson = JsonUtility.ToJson(myObject.GetdataFromClass(NewPassword));
        NewPasswordForgetApi = NewPassword;
        StartCoroutine(HitResetAPI(ConstantsGod.API_BASEURL + ConstantsGod.ForgetPasswordResetAPI, bodyJson));
    }

    IEnumerator HitResetAPI(string url, string Jsondata)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(Jsondata);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", ForgetPasswordTokenAfterVerifyling);
        yield return request.SendWebRequest();
        MyClassNewApi myObject1 = new MyClassNewApi();
        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            myObject1 = CheckResponceJsonNewApi(request.downloadHandler.text);
            if (request.error == null)
            {
                if (myObject1.success)
                {
                    if (ForgetPasswordBool)
                    {
                        savePasswordList.instance.saveDataFromForgetPassword(ForgetPasswordEmlOrPhnContainer, NewPasswordForgetApi);
                        OpenUIPanal(6);
                        ForgetPasswordTokenAfterVerifyling = "";
                        ForgetPasswordBool = false;
                    }
                    else
                    {
                        OpenUIPanal(16);
                        GameManager.Instance.SignInSignUpCompleted();
                        usernamePanal.SetActive(false);
                        LoggedIn = true;
                    }
                }
            }
        }
        else
        {
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                //if (!ConnectionEstablished_popUp.activeInHierarchy)
                //{
                //    // errorTextName.GetComponent<Animator>().SetBool("playAnim", true);
                //    validationMessagePopUP.SetActive(true);
                //    errorTextName.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                //    // if (Application.systemLanguage == SystemLanguage.Japanese  )
                //    // {
                //    //     errorTextName.GetComponent<Text>().text = "接続状態が悪く繋がりません";
                //    // }
                //    // else
                //    // {
                //    //     errorTextName.GetComponent<Text>().text = request.error.ToUpper();
                //    // }
                //    errorHandler.ShowErrorMessage(ErrorType.Poor_Connection.ToString(), errorTextName.GetComponent<TextMeshProUGUI>());
                //    // StartCoroutine(WaitUntilAnimationFinished(errorTextName.GetComponent<Animator>()));
                //}
            }
            else
            {
                if (request.error != null)
                {
                    myObject1 = CheckResponceJsonNewApi(request.downloadHandler.text);
                    if (!myObject1.success)
                    {
                        validationMessagePopUP.SetActive(true);
                        errorTextName.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                        // if (Application.systemLanguage == SystemLanguage.Japanese  )
                        // {
                        //     errorTextName.GetComponent<Text>().text = "名前が無効です";
                        // }
                        // else
                        // {
                        //     errorTextName.GetComponent<Text>().text = myObject1.msg.ToUpper();
                        // }
                        errorHandler.ShowErrorMessage(myObject1.msg, errorTextName.GetComponent<TextMeshProUGUI>());
                        // StartCoroutine(WaitUntilAnimationFinished(errorTextName.GetComponent<Animator>()));
                    }
                }
            }
        }
    }



    [Serializable]
    public class MyClassOfPostingForgetPassword : JsonObjectBase
    {
        public string userName;
        public MyClassOfPostingForgetPassword GetForgetPassworddata(string eml)
        {
            MyClassOfPostingForgetPassword myObj = new MyClassOfPostingForgetPassword();
            myObj.userName = eml;
            return myObj;
        }
    }
    [Serializable]
    public class MyClassOfPostingForgetPasswordOTP : JsonObjectBase
    {
        public string userName;
        public string otp;
        public MyClassOfPostingForgetPasswordOTP GetdataFromClass(string usrnme, string otp)
        {
            MyClassOfPostingForgetPasswordOTP myObj = new MyClassOfPostingForgetPasswordOTP();
            myObj.userName = usrnme;
            myObj.otp = otp;
            return myObj;
        }
    }
    [Serializable]
    public class MyClassOfPostingReset : JsonObjectBase
    {
        public string password;
        public MyClassOfPostingReset GetdataFromClass(string Pass)
        {
            MyClassOfPostingReset myObj = new MyClassOfPostingReset();
            myObj.password = Pass;
            return myObj;
        }
    }

    [System.Serializable]
    public class ClassWithTokenofResetPassword
    {
        public string success;
        public JustTokenResetPassword data;
        public string msg;

        public static ClassWithTokenofResetPassword CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<ClassWithTokenofResetPassword>(jsonString);
        }
    }

    [System.Serializable]
    public class JustTokenResetPassword
    {
        public string otpMatched;
        public string tempToken;

        public static JustTokenResetPassword CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<JustTokenResetPassword>(jsonString);
        }
    }

    public void SubmitDeleteAccount()
    {
        //  //print("Submit Delete Account");
    }
    public void SubmitUpdateProfile()
    {
    }

    [Serializable]
    public class MyClassForUpdatingProfile : JsonObjectBase
    {
        public string gender;
        public string job;
        public string country;
        public string bio;
        public MyClassForUpdatingProfile GetUpdateProfiledata(string gnder, string jobb, string cntry, string bioo)
        {
            MyClassForUpdatingProfile myObj = new MyClassForUpdatingProfile();
            myObj.gender = gnder;
            myObj.job = jobb;
            myObj.country = cntry;
            myObj.bio = bioo;
            return myObj;
        }
    }
    /// <SignUpWithPhoneNumber>
    public void SubmitPhoneNumber()
    {
        if (PhoneFieldNew.Text == "")
        {
            validationMessagePopUP.SetActive(true);
            errorTextNumber.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
            // if (Application.systemLanguage == SystemLanguage.Japanese  )
            // {
            //     // 5: Phone Number should not be empty
            //     errorTextNumber.GetComponent<Text>().text = "携帯番号を入力してください";
            // }   
            // else
            // {
            //     errorTextNumber.GetComponent<Text>().text = "Phone number should not be empty";
            // } 
            //
            errorHandler.ShowErrorMessage(ErrorType.Phone_number__empty.ToString(), errorTextNumber.GetComponent<TextMeshProUGUI>());

            // StartCoroutine(WaitUntilAnimationFinished(errorTextNumber.GetComponent<Animator>()));
            return;
        }

        if (PhoneFieldNew.Text.Length > 10)
        {
            validationMessagePopUP.SetActive(true);
            errorTextNumber.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);

            errorHandler.ShowErrorMessage(ErrorType.Enter_Valid_Number.ToString(), errorTextNumber.GetComponent<TextMeshProUGUI>());
            if (GameManager.currentLanguage == "ja" && CountryCodeText.text == "+81")
            {
                // 5: Phone Number should not be empty
                errorTextNumber.GetComponent<TextMeshProUGUI>().text = "市外局番を抜いて入力ください（例 080→80、090→90）";
            }
            return;
        }

        else
        {
            PhoneFieldNew.Text = PhoneFieldNew.Text.Trim();
            string phonenumberText = CountryCodeText.text + PhoneFieldNew.Text;
            string url = ConstantsGod.API_BASEURL + ConstantsGod.SendPhoneOTPAPI;
            if (ResendOTPBool)
            {
                url = ConstantsGod.API_BASEURL + ConstantsGod.ResendOTPAPI;
                ResendOTPBool = false;
            }

            GameObject NxtButtonObj = EventSystem.current.currentSelectedGameObject;
            if (NxtButtonObj)
            {
                currentSelectedNxtButton = NxtButtonObj.GetComponent<Button>();
            }

            if (NxtButtonObj.transform.Find("Loader") != null)
                NxtButtonObj = NxtButtonObj.transform.Find("Loader").gameObject;
            else
                NxtButtonObj = null;

            if (NxtButtonObj == null)
            {
                MyClassOfPhoneNumber myObject = new MyClassOfPhoneNumber();
                string bodyJson = JsonUtility.ToJson(myObject.GetdataFromClass(phonenumberText));
                StartCoroutine(HitPhoneAPI(url, bodyJson, phonenumberText));
            }
            else
            {
                if (NxtButtonObj.activeInHierarchy)
                    return;
                NxtButtonObj.SetActive(true);
                if (currentSelectedNxtButton)
                {
                    currentSelectedNxtButton.interactable = false;
                }
                MyClassOfPhoneNumber myObject = new MyClassOfPhoneNumber();
                string bodyJson = JsonUtility.ToJson(myObject.GetdataFromClass(phonenumberText));
                StartCoroutine(HitPhoneAPI(url, bodyJson, phonenumberText, NxtButtonObj));
            }
        }
    }

    public void NotNowHuman()
    {

        GameManager.Instance.NotNowOfSignManager();
    }

    /// <ChangePassword>
    public void SubmitChangePassword()
    {

    }

    IEnumerator HitChangePasswordAPI(string url, string Jsondata)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(Jsondata);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", PlayerPrefs.GetString("LoginToken"));
        request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        MyClassNewApi myObject1 = new MyClassNewApi();
        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            myObject1 = CheckResponceJsonNewApi(request.downloadHandler.text);
            if (request.error == null)
            {
                if (myObject1.success)
                {
                    //print("Change Password Successfully");
                }
            }
        }
        else
        {
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                //if (!ConnectionEstablished_popUp.activeInHierarchy)
                //{
                //    validationMessagePopUP.SetActive(true);
                //    errorTextPassword.SetActive(true);
                //    errorTextPassword.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                //    //errorTextPassword.GetComponent<Animator>().SetBool("playAnim", true);
                //    //errorTextPassword.GetComponent<Text>().text = request.error.ToUpper();
                //    errorHandler.ShowErrorMessage(ErrorType.Poor_Connection.ToString(), errorTextPassword.GetComponent<TextMeshProUGUI>());
                //    //  StartCoroutine(WaitUntilAnimationFinished(errorTextPassword.GetComponent<Animator>()));
                //}
            }
            else
            {
                if (request.error != null)
                {
                    myObject1 = CheckResponceJsonNewApi(request.downloadHandler.text);
                    if (!myObject1.success)
                    {
                        validationMessagePopUP.SetActive(true);
                        errorTextPassword.SetActive(true);
                        errorTextPassword.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                        //         //print("Hey success false " + myObject1.msg);
                        //  errorTextPassword.GetComponent<Animator>().SetBool("playAnim", true);
                        //errorTextPassword.GetComponent<Text>().text = myObject1.msg.ToUpper();
                        //errorHandler.ShowErrorMessage(ErrorType.Wrong_Password, errorTextPassword.GetComponent<Text>());
                        errorHandler.ShowErrorMessage(myObject1.msg, errorTextPassword.GetComponent<TextMeshProUGUI>());
                        // StartCoroutine(WaitUntilAnimationFinished(errorTextPassword.GetComponent<Animator>()));
                    }
                }
            }
        }
    }

    //  EndChangePassword
    // Send OTP to Phone Number
    IEnumerator HitPhoneAPI(string url, string Jsondata, string LPhoneNumber, GameObject _loader = null)
    {
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
        if (request.downloadHandler.text.Contains("Enter Valid Number"))
        {
            mobile_number = true;
        }
        else if (request.downloadHandler.text.Contains("User Already Exists With This Number"))
        {
            mobile_number = false;
        }
        MyClassNewApi myObject1 = new MyClassNewApi();
        myObject1 = CheckResponceJsonNewApi(request.downloadHandler.text);

        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            if (request.error == null)
            {
                if (_loader != null)
                    _loader.SetActive(false);
                if (myObject1.success)
                {
                    if (currentSelectedNxtButton)
                    {
                        currentSelectedNxtButton.interactable = true;
                    }
                    SignUpWithPhoneBool = true;
                    SignUpWithEmailBool = false;
                    LocalPhoneNumber = LPhoneNumber;
                    OpenUIPanal(3);
                }
            }
        }
        else
        {
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                //if (!ConnectionEstablished_popUp.activeInHierarchy)
                //{
                //    if (_loader != null)
                //    {
                //        if (currentSelectedNxtButton)
                //        {
                //            currentSelectedNxtButton.interactable = true;
                //        }

                //        _loader.SetActive(false);
                //    }

                //    // errorTextNumber.GetComponent<Animator>().SetBool("playAnim", true);

                //    validationMessagePopUP.SetActive(true);
                //    errorTextNumber.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                //    // if (Application.systemLanguage == SystemLanguage.Japanese  )
                //    // {
                //    //     //6: Cannot Connect to Destination Host
                //    //     errorTextNumber.GetComponent<Text>().text = "接続状態が悪く繋がりません";
                //    // }
                //    // else
                //    // {
                //    //     errorTextNumber.GetComponent<Text>().text = request.error.ToUpper();
                //    // }   
                //    errorHandler.ShowErrorMessage(ErrorType.Poor_Connection.ToString(), errorTextNumber.GetComponent<TextMeshProUGUI>());
                //    // StartCoroutine(WaitUntilAnimationFinished(errorTextNumber.GetComponent<Animator>()));
                //}
            }
            else
            {
                if (request.error != null)
                {
                    if (!myObject1.success)
                    {
                        validationMessagePopUP.SetActive(true);
                        errorTextNumber.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                        // if (Application.systemLanguage == SystemLanguage.Japanese  )
                        // {
                        //     //4: Phone number is already exists
                        //     errorTextNumber.GetComponent<Text>().text = "同じ携帯番号が登録されています";
                        // }     
                        // else
                        // {
                        //     errorTextNumber.GetComponent<Text>().text = myObject1.msg.ToUpper();
                        // }
                        //if (!mobile_number)
                        //{
                        //    errorHandler.ShowErrorMessage(ErrorType.User_Already_Exist, errorTextNumber.GetComponent<Text>());
                        //}

                        //else if (mobile_number)
                        //{
                        //    errorHandler.ShowErrorMessage(ErrorType.Enter_Valid_Number, errorTextNumber.GetComponent<Text>());
                        //}

                        errorHandler.ShowErrorMessage(myObject1.msg, errorTextNumber.GetComponent<TextMeshProUGUI>());


                        if (_loader != null)
                        {
                            if (currentSelectedNxtButton)
                            {
                                currentSelectedNxtButton.interactable = true;
                            }
                            _loader.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    public void ResendOTP()
    {
        if (ForgetPasswordBool)
        {
            SubmitForgetPassword();
        }
        else
        {
            ResendOTPBool = true;
            if (SignUpWithPhoneBool)
            {
                SubmitPhoneNumber();
            }
            else
            {
                SubmitEmail();
            }
        }


    }
    // END
    /// </SignUpWithPhoneNumber>

    // SignUpwithEmail
    public void SubmitEmail()
    {
        if (EmailFieldNew.Text == "")
        {
            validationMessagePopUP.SetActive(true);
            errorTextEmail.SetActive(true);
            errorTextEmail.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
            //  errorTextEmail.GetComponent<Animator>().SetBool("playAnim", true);
            errorHandler.ShowErrorMessage(ErrorType.Email_field__empty.ToString(), errorTextEmail.GetComponent<TextMeshProUGUI>());
            // StartCoroutine(WaitUntilAnimationFinished(errorTextEmail.GetComponent<Animator>()));
            return;
        }
        else
        {
            EmailFieldNew.Text = EmailFieldNew.Text.Trim();

            if (IsValidEmail(EmailFieldNew.Text))
            {
                string email = EmailFieldNew.Text;
                string url = ConstantsGod.API_BASEURL + ConstantsGod.SendEmailOTP;
                email = email.ToLower();
                if (ResendOTPBool)
                {
                    url = ConstantsGod.API_BASEURL + ConstantsGod.ResendOTPAPI;
                    ResendOTPBool = false;
                }

                GameObject NxtButtonObj = EventSystem.current.currentSelectedGameObject;
                if (NxtButtonObj)
                {
                    currentSelectedNxtButton = NxtButtonObj.GetComponent<Button>();
                }

                NxtButtonObj = NxtButtonObj.transform.Find("Loader").gameObject;

                if (NxtButtonObj.activeInHierarchy)
                    return;
                NxtButtonObj.SetActive(true);
                if (currentSelectedNxtButton)
                {
                    currentSelectedNxtButton.interactable = false;
                }

                MyClassOfPostingEmail myobjectOfEmail = new MyClassOfPostingEmail();
                string bodyJson = JsonUtility.ToJson(myobjectOfEmail.GetEmaildata(email));
                StartCoroutine(HitEmailAPIWithNewTechnique(url, bodyJson, email, NxtButtonObj));
            }
            else
            {
                validationMessagePopUP.SetActive(true);
                errorTextEmail.SetActive(true);
                errorTextEmail.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                //  errorTextEmail.GetComponent<Animator>().SetBool("playAnim", true);
                errorHandler.ShowErrorMessage(ErrorType.Please_enter_valid_email.ToString(), errorTextEmail.GetComponent<TextMeshProUGUI>());
                //   StartCoroutine(WaitUntilAnimationFinished(errorTextEmail.GetComponent<Animator>()));
            }
        }
    }

    [Serializable]
    public class MyClassOfPostingEmail : JsonObjectBase
    {
        public string email;
        public MyClassOfPostingEmail GetEmaildata(string eml)
        {
            MyClassOfPostingEmail myObj = new MyClassOfPostingEmail();
            myObj.email = eml;
            return myObj;
        }
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
    public IEnumerator HitEmailAPIWithNewTechnique(string url, string Jsondata, string localEmail, GameObject _loader = null)
    {
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
        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            if (_loader != null)
            {
                _loader.SetActive(false);
            }

            myObject1 = CheckResponceJsonNewApi(request.downloadHandler.text);
            if (request.error == null && passwordBool)
            {
                if (myObject1.success)
                {
                    if (currentSelectedNxtButton)
                    {
                        currentSelectedNxtButton.interactable = true;
                    }
                    emailBool = true;
                    OpenUIPanal(3);
                    Email = localEmail;
                    SignUpWithPhoneBool = false;
                }
            }
        }
        else
        {
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                //if (!ConnectionEstablished_popUp.activeInHierarchy)
                //{
                //    if (_loader != null)
                //    {
                //        if (currentSelectedNxtButton)
                //        {
                //            currentSelectedNxtButton.interactable = true;
                //        }
                //        _loader.SetActive(false);
                //    }
                //    validationMessagePopUP.SetActive(true);
                //    errorTextEmail.SetActive(true);
                //    errorTextEmail.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);

                //    // if (Application.systemLanguage == SystemLanguage.Japanese  )
                //    // {
                //    //     errorTextEmail.GetComponent<Text>().text = "接続状態が悪く繋がりません";
                //    // }
                //    // else
                //    // {
                //    //     errorTextEmail.GetComponent<Text>().text = request.error.ToUpper();
                //    // }
                //    errorHandler.ShowErrorMessage(ErrorType.Poor_Connection.ToString(), errorTextEmail.GetComponent<TextMeshProUGUI>());
                //    //print("getting text from here");
                //    // StartCoroutine(WaitUntilAnimationFinished(errorTextEmail.GetComponent<Animator>()));
                //}
            }
            else
            {
                if (request.error != null)
                {
                    myObject1 = CheckResponceJsonNewApi(request.downloadHandler.text);
                    if (!myObject1.success)
                    {
                        validationMessagePopUP.SetActive(true);
                        errorTextEmail.SetActive(true);
                        errorHandler.ShowErrorMessage(myObject1.msg, errorTextEmail.GetComponent<TextMeshProUGUI>());
                        errorTextEmail.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                        if (_loader != null)
                        {
                            if (currentSelectedNxtButton)
                            {
                                currentSelectedNxtButton.interactable = true;
                            }
                            _loader.SetActive(false);
                        }
                    }
                }
            }
        }
        currentSelectedNxtButton.interactable = true;
    }
    ///////  ENDEmailSection


    // Submitting OTP from Phone Button
    public void SubmitOTP()
    {
        string OTP = "";
        NewLoadingScreen.SetActive(true);
        _NewLoadingText.text = "";
        OTP = mainfieldOTPNew.Text;
        if (OTP == "" || OTP.Length < 4)
        {
            validationMessagePopUP.SetActive(true);
            errorTextPIN.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
            // if (Application.systemLanguage == SystemLanguage.Japanese  )
            // {
            //     errorTextPIN.GetComponent<Text>().text = "認証コードを入力してください";
            // }
            // else
            // {
            //     errorTextPIN.GetComponent<Text>().text = "OTP fields should not be empty";
            // }
            errorHandler.ShowErrorMessage(ErrorType.OTP_fields__empty.ToString(), errorTextPIN.GetComponent<TextMeshProUGUI>());
            //  StartCoroutine(WaitUntilAnimationFinished(errorTextPIN.GetComponent<Animator>()));
            return;
        }
        if (ForgetPasswordBool)
        {
            string url = ConstantsGod.API_BASEURL + ConstantsGod.ForgetPasswordOTPAPI;
            MyClassOfPostingForgetPasswordOTP myobjectOfPhone = new MyClassOfPostingForgetPasswordOTP();
            string bodyJson = JsonUtility.ToJson(myobjectOfPhone.GetdataFromClass(ForgetPasswordEmlOrPhnContainer, OTP));
            StartCoroutine(HitOTPAPI(url, bodyJson));
        }
        else
        {
            // Phone OTP sending Section
            if (SignUpWithPhoneBool)
            {
                string url = ConstantsGod.API_BASEURL + ConstantsGod.VerifyPhoneOTPAPI;
                MyClassOfPostingPhoneOTP myobjectOfPhone = new MyClassOfPostingPhoneOTP();
                string bodyJson = JsonUtility.ToJson(myobjectOfPhone.GetdataFromClass(LocalPhoneNumber, OTP));
                StartCoroutine(HitOTPAPI(url, bodyJson));
            }
            // Email OTP sending Section
            else
            {
                string url = ConstantsGod.API_BASEURL + ConstantsGod.VerifyEmailOTP;
                MyClassOfPostingOTP myObject = new MyClassOfPostingOTP();
                string bodyJson = JsonUtility.ToJson(myObject.GetdataFromClass(Email, OTP));
                StartCoroutine(HitOTPAPI(url, bodyJson));
            }
        }

    }


    // Start Register User with password
    [Serializable]
    public class MyClassOfRegisterWithNumber : JsonObjectBase
    {

        public string phoneNumber;
        public string password;
        public MyClassOfRegisterWithNumber GetdataFromClass(string L_phonenbr, string passwrd)
        {
            MyClassOfRegisterWithNumber myObj = new MyClassOfRegisterWithNumber();

            myObj.phoneNumber = L_phonenbr;
            myObj.password = passwrd;
            return myObj;
        }

        public static MyClassOfRegisterWithNumber CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<MyClassOfRegisterWithNumber>(jsonString);
        }
    }


    [Serializable]
    public class MyClassOfRegisterWithEmail : JsonObjectBase
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


    public void SubmitPassword()
    {
        string pass1 = Password1New.Text;

        string pass2 = Password2New.Text;

        if (pass1 == "" || pass2 == "")
        {
            passwordBool = false;
            validationMessagePopUP.SetActive(true);
            errorTextPassword.SetActive(true);
            //print("Password Field should not be empty");
            //errorTextPassword.GetComponent<Animator>().SetBool("playAnim", true);
            errorTextPassword.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
            errorHandler.ShowErrorMessage(ErrorType.Password_field__empty.ToString(), errorTextPassword.GetComponent<TextMeshProUGUI>());
            //  StartCoroutine(WaitUntilAnimationFinished(errorTextPassword.GetComponent<Animator>()));
            return;
        }



        if (pass1.Length < 8 || pass2.Length < 8)
        {
            passwordBool = false;
            validationMessagePopUP.SetActive(true);
            errorTextPassword.SetActive(true);
            // errorTextPassword.GetComponent<Animator>().SetBool("playAnim", true);
            errorTextPassword.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
            errorHandler.ShowErrorMessage(ErrorType.Passwords_cannot_less_than_eight_charcters.ToString(), errorTextPassword.GetComponent<TextMeshProUGUI>());
            //  StartCoroutine(WaitUntilAnimationFinished(errorTextPassword.GetComponent<Animator>()));
            return;
        }

        bool allCharactersInStringAreDigits = false;
        string specialCh = @"%!@#$%^&*()?/>.<,:;'\|}]{[_~`+=-" + "\"";
        char[] specialChArray = specialCh.ToCharArray();
        if (pass1.Any(char.IsDigit) && pass1.Any(char.IsLower) && pass1.Any(char.IsUpper) && !pass1.Any(char.IsWhiteSpace))
        {
            passwordBool = false;
            foreach (char ch in specialChArray)
            {
                if (pass1.Contains(ch))
                    allCharactersInStringAreDigits = true;
            }

        }
        if (!allCharactersInStringAreDigits)
        {
            passwordBool = false;
            validationMessagePopUP.SetActive(true);
            errorTextPassword.SetActive(true);
            errorTextPassword.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
            //errorTextPassword.GetComponent<Animator>().SetBool("playAnim", true);
            errorHandler.ShowErrorMessage(ErrorType.Password_must_Contain_Number.ToString(), errorTextPassword.GetComponent<TextMeshProUGUI>());
            //StartCoroutine(WaitUntilAnimationFinished(errorTextPassword.GetComponent<Animator>()));
            return;
        }

        if (pass1 == pass2)
        {
            password = pass1;
            passwordBool = true;
        }
        else
        {
            passwordBool = false;
            validationMessagePopUP.SetActive(true);
            errorTextPassword.SetActive(true);
            errorTextPassword.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);

            // errorTextPassword.GetComponent<Animator>().SetBool("playAnim", true);
            errorHandler.ShowErrorMessage(ErrorType.Passwords_do_not_match.ToString(), errorTextPassword.GetComponent<TextMeshProUGUI>());
            //StartCoroutine(WaitUntilAnimationFinished(errorTextPassword.GetComponent<Animator>()));
            //   print("Password not matched");
        }
    }
    public ClassWithToken TokenDataClass = new ClassWithToken();
    IEnumerator RegisterUserWithNewTechnique(string url, string Jsondata, string JsonOfName, String NameofUser, bool registerWithEmail = true)
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
        TokenDataClass = myObject = ClassWithToken.CreateFromJSON(request.downloadHandler.text);
        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            if (request.error == null)
            {
                if (myObject.success)
                {
                    if (registerWithEmail)
                    {
                        MyClassOfRegisterWithEmail myobjectOfEmail = new MyClassOfRegisterWithEmail();
                        myobjectOfEmail = MyClassOfRegisterWithEmail.CreateFromJSON(Jsondata);
                        MyClassOfLoginJson myObject1 = new MyClassOfLoginJson();
                        string bodyJson = JsonUtility.ToJson(myObject1.GetdataFromClass(myobjectOfEmail.email, "", myobjectOfEmail.password, uniqueID()));
                        PlayerPrefs.SetString("UserNameAndPassword", bodyJson);
                    }
                    else
                    {
                        MyClassOfRegisterWithNumber myobjectOfPhone = new MyClassOfRegisterWithNumber();
                        myobjectOfPhone = MyClassOfRegisterWithNumber.CreateFromJSON(Jsondata);
                        MyClassOfLoginJson myObject1 = new MyClassOfLoginJson();
                        string bodyJson = JsonUtility.ToJson(myObject1.GetdataFromClass("", myobjectOfPhone.phoneNumber, myobjectOfPhone.password, uniqueID()));
                        PlayerPrefs.SetString("UserNameAndPassword", bodyJson);
                    }

                    ConstantsGod.AUTH_TOKEN = myObject.data.token;
                    var parts = myObject.data.token.Split('.');
                    if (parts.Length > 2)
                    {
                        var decode = parts[1];
                        var padLength = 4 - decode.Length % 4;
                        if (padLength < 4)
                        {
                            decode += new string('=', padLength);
                        }
                        var bytes = System.Convert.FromBase64String(decode);
                        var userInfo = System.Text.ASCIIEncoding.ASCII.GetString(bytes);
                        LoginClass L_LoginObject = new LoginClass();
                        L_LoginObject = CheckResponceJsonOfLogin(userInfo);
                        PlayerPrefs.SetString("UserName", L_LoginObject.id);
                        PlayerPrefs.SetInt("IsLoggedIn", 1);
                        PlayerPrefs.SetInt("FristPresetSet", 1);
                        ConstantsHolder.userId = L_LoginObject.id;

                    }
                    PlayerPrefs.Save();
                    LoggedIn = true;
                    StartCoroutine(HitNameAPIWithNewTechnique(ConstantsGod.API_BASEURL + ConstantsGod.NameAPIURL, JsonOfName, NameofUser));
                    GameManager.Instance.UiManager._footerCan.transform.GetChild(0).GetComponent<HomeFooterHandler>().HomeSceneFooterSNSButtonIntrectableTrueFalse();
                }
            }
        }
        else
        {

            if (request.isNetworkError)
            {
                //if (!ConnectionEstablished_popUp.activeInHierarchy)
                //{
                //    validationMessagePopUP.SetActive(true);
                //    errorTextPassword.SetActive(true);
                //    errorTextPassword.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                //    //errorTextPassword.GetComponent<Animator>().SetBool("playAnim", true);
                //    errorHandler.ShowErrorMessage(ErrorType.Poor_Connection.ToString(), errorTextPassword.GetComponent<TextMeshProUGUI>());
                //    //StartCoroutine(WaitUntilAnimationFinished(errorTextPassword.GetComponent<Animator>()));
                //}
            }
            else
            {
                if (request.error != null)
                {
                    if (!myObject.success)
                    {
                        validationMessagePopUP.SetActive(true);
                        errorTextPassword.SetActive(true);
                        //    //print("Hey success false " + myObject.msg);
                        errorTextPassword.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                        //   errorTextPassword.GetComponent<Animator>().SetBool("playAnim", true);
                        errorHandler.ShowErrorMessage(myObject.msg, errorTextPassword.GetComponent<TextMeshProUGUI>());
                        // StartCoroutine(WaitUntilAnimationFinished(errorTextPassword.GetComponent<Animator>()));
                    }
                }
            }
        }
    }

    // End Register User with password

    public void LoadingFadeOutScreen()
    {
        BlackScreen.SetActive(true);
        BlackScreen.GetComponent<Image>().color = new Color(0, 0, 0, 1);
        StartCoroutine(LerpFunction(new Color(0, 0, 0, 0), 2));
        TutorialsHandler.instance.ShowTutorials();
        DefaultClothDatabase.instance.GetComponent<SaveCharacterProperties>().SavePlayerProperties();
    }
    IEnumerator LerpFunction(Color endValue, float duration)
    {
        float time = 0;
        Color startValue = BlackScreen.GetComponent<Image>().color;
        while (time < duration)
        {
            BlackScreen.GetComponent<Image>().color = Color.Lerp(startValue, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        BlackScreen.GetComponent<Image>().color = endValue;
        BlackScreen.SetActive(false);
    }




    public void EnterUserName()
    {
        GameObject NxtButtonObj = EventSystem.current.currentSelectedGameObject;
        if (NxtButtonObj)
        {
            currentSelectedNxtButton = NxtButtonObj.GetComponent<Button>();
        }
        currentSelectedNxtButton.interactable = false;
        UsernamescreenLoader.SetActive(true);
        string Localusername = UsernameFieldAdvance.Text;
        UserNameSetter.text = UsernameFieldAdvance.Text;


        if (Localusername == "")
        {
            validationMessagePopUP.SetActive(true);
            errorTextName.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
            // if (Application.systemLanguage == SystemLanguage.Japanese  )
            // {
            //     errorTextName.GetComponent<Text>().text = "名前を入力してください";
            // }
            // else
            // {
            //     errorTextName.GetComponent<Text>().text = "Name Field should not be empty";
            //  }
            errorHandler.ShowErrorMessage(ErrorType.Name_Field__empty.ToString(), errorTextName.GetComponent<TextMeshProUGUI>());
            currentSelectedNxtButton.interactable = true;
            UsernamescreenLoader.SetActive(false);
            return;
        }
        else if (Localusername.StartsWith(" "))
        {
            validationMessagePopUP.SetActive(true);
            errorTextName.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
            // if (Application.systemLanguage == SystemLanguage.Japanese  )
            // {
            //     errorTextName.GetComponent<Text>().text = "名前を入力してください";
            // }
            // else
            // {
            //     errorTextName.GetComponent<Text>().text = "Name Field should not be empty";
            //  }
            errorHandler.ShowErrorMessage(ErrorType.UserName_Has_Space.ToString(), errorTextName.GetComponent<TextMeshProUGUI>());
            currentSelectedNxtButton.interactable = true;
            UsernamescreenLoader.SetActive(false);
            return;
        }

        if (Localusername.EndsWith(" "))
        {
            Localusername = Localusername.TrimEnd(' ');
            currentSelectedNxtButton.interactable = true;
            UsernamescreenLoader.SetActive(false);
        }
        GameManager.Instance.mainCharacter.GetComponent<CharacterOnScreenNameHandler>().UpdateNameText(Localusername);
        if (isSetXanaliyaUserName)//rik
        {
            Debug.LogError("Xanalia User Name");
            MyClassOfPostingName tempMyObject = new MyClassOfPostingName();
            string bodyJsonOfName1 = JsonUtility.ToJson(tempMyObject.GetNamedata(Localusername));
            StartCoroutine(HitNameAPIWithXanaliyaUser(ConstantsGod.API_BASEURL + ConstantsGod.NameAPIURL, bodyJsonOfName1, Localusername));
            currentSelectedNxtButton.interactable = true;
            UsernamescreenLoader.SetActive(false);
            return;
        }

        if (PlayerPrefs.GetInt("shownWelcome") == 0 && PlayerPrefs.GetInt("IsProcessComplete") == 0 && PlayerPrefs.GetInt("iSignup") == 0)
        {
            Debug.LogError("Set Name for Guest User");
            //DynamicEventManager.deepLink?.Invoke("come from Guest Registration");
            PlayerPrefs.SetString(ConstantsGod.GUSTEUSERNAME, Localusername);
            currentSelectedNxtButton.interactable = true;
            UsernamescreenLoader.SetActive(false);
            usernamePanal.SetActive(false);
            checkbool_preser_start = true;
            PlayerPrefs.SetInt("shownWelcome", 1);
            if (PlayerPrefs.GetInt("shownWelcome") == 1)
            {
                InventoryManager.instance.OnSaveBtnClicked();
            }
            PlayerPrefs.SetInt("IsProcessComplete", 1);// user is registered as guest/register.
            return;
        }
        PlayerPrefs.SetInt("IsProcessComplete", 1);
        MyClassOfPostingName myObject = new MyClassOfPostingName();
        string bodyJsonOfName = JsonUtility.ToJson(myObject.GetNamedata(Localusername));
        if (PlayerPrefs.GetInt("IsLoggedIn") == 1)
        {
            Debug.LogError("Set Name for logged in user");
            ////Debug.Log("User Already loged in set name api call.......");
            StartCoroutine(HitNameAPIWithNewTechnique(ConstantsGod.API_BASEURL + ConstantsGod.NameAPIURL, bodyJsonOfName, Localusername));
        }
        else
        {
            Debug.LogError("Set Name when not logged in");
            if (SignUpWithPhoneBool)
            {
                Debug.LogError("register with phone number");
                string url = ConstantsGod.API_BASEURL + ConstantsGod.RegisterPhoneAPI;
                MyClassOfRegisterWithNumber myobjectOfPhone = new MyClassOfRegisterWithNumber();
                string _bodyJson = JsonUtility.ToJson(myobjectOfPhone.GetdataFromClass(LocalPhoneNumber, password));
                StartCoroutine(RegisterUserWithNewTechnique(url, _bodyJson, bodyJsonOfName, Localusername, false));
                PlayerPrefs.SetInt("CloseLoginScreen", 1);
            }
            else
            {
                Debug.LogError("register with Email");
                string url = ConstantsGod.API_BASEURL + ConstantsGod.RegisterWithEmail;
                MyClassOfRegisterWithEmail myobjectOfEmail = new MyClassOfRegisterWithEmail();
                ProfilePictureManager.instance.MakeProfilePicture(Localusername);
                string _bodyJson = JsonUtility.ToJson(myobjectOfEmail.GetdataFromClass(Email, password));
                StartCoroutine(RegisterUserWithNewTechnique(url, _bodyJson, bodyJsonOfName, Localusername, true));
                PlayerPrefs.SetInt("CloseLoginScreen", 1);
            }
        }
    }



    public void SubmitLoginCredentials()
    {
        savePasswordList.instance.DisableOnLoginButton();
        string L_LoginEmail = LoginEmailOrPhone.Text;
        string L_loginPassword = LoginPassword.Text;
        if (L_LoginEmail == "" || L_loginPassword == "")
        {
            validationMessagePopUP.SetActive(true);
            errorTextLogin.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
            // if (Application.systemLanguage == SystemLanguage.Japanese  )
            // {
            //     errorTextLogin.GetComponent<Text>().text = "全ての欄に入力してください";
            // }
            // else
            // {
            //     errorTextLogin.GetComponent<Text>().text = "Fields should not be empty";
            //  }
            errorHandler.ShowErrorMessage(ErrorType.Fields__empty.ToString(), errorTextLogin.GetComponent<TextMeshProUGUI>());
            // StartCoroutine(WaitUntilAnimationFinished(errorTextLogin.GetComponent<Animator>()));
            return;
        }
        else if (L_LoginEmail.Contains(" "))
        {
            validationMessagePopUP.SetActive(true);
            errorTextLogin.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
            errorHandler.ShowErrorMessage(ErrorType.Please_enter_valid_email.ToString(), errorTextLogin.GetComponent<TextMeshProUGUI>());
            // StartCoroutine(WaitUntilAnimationFinished(errorTextLogin.GetComponent<Animator>()));
            return;
        }
        string url = ConstantsGod.API_BASEURL + ConstantsGod.LoginAPIURL;

        L_LoginEmail = L_LoginEmail.Trim();

        MyClassOfLoginJson myObject = new MyClassOfLoginJson();
        L_LoginEmail = L_LoginEmail.Trim();
        L_loginPassword = L_loginPassword.Trim();
        string bodyJson;

        GameObject _loginBtnObject = EventSystem.current.currentSelectedGameObject;
        _loginBtnObject = _loginBtnObject.transform.Find("Loader").gameObject;

        if (_loginBtnObject.activeInHierarchy)
            return;

        _loginBtnObject.SetActive(true);

        if (IsValidEmail(L_LoginEmail))
        {

            L_LoginEmail = L_LoginEmail.ToLower();
            bodyJson = JsonUtility.ToJson(myObject.GetdataFromClass(L_LoginEmail, "", L_loginPassword, uniqueID()));
        }
        else if (!L_LoginEmail.Contains("+") && L_LoginEmail.Any(char.IsLetter))
        {
            validationMessagePopUP.SetActive(true);
            errorTextLogin.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
            errorHandler.ShowErrorMessage(ErrorType.Please_enter_valid_email.ToString(), errorTextLogin.GetComponent<TextMeshProUGUI>());
            _loginBtnObject.SetActive(false);
            return;
        }
        else
        {
            bodyJson = JsonUtility.ToJson(myObject.GetdataFromClass("", L_LoginEmail, L_loginPassword));
        }
        StartCoroutine(LoginUserWithNewT(url, bodyJson, _loginBtnObject));
    }



    //   AppID = uniqueID();
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
    public void ReloadUnloadScene()
    {
        StartCoroutine(GameManager.Instance.HitReloadUnloadScene());
    }

    public void LogOutFromOtherDevice()
    {
        StartCoroutine(HitLogOutFromOtherDevice(ConstantsGod.API_BASEURL + ConstantsGod.LogoutFromotherDeviceAPI, PlayerPrefs.GetString("LogoutFromDeviceJSON")));
    }


    public IEnumerator HitLogOutFromOtherDevice(string URL, string _json)
    {
        var request = new UnityWebRequest(URL, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(_json);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        MyClassNewApi obj_LogOut = new MyClassNewApi();
        obj_LogOut = obj_LogOut.Load(request.downloadHandler.text);

        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            if (request.error == null)
            {
                if (obj_LogOut.success)
                {
                    LogoutfromOtherDevicePanel.SetActive(false);
                    StartCoroutine(LoginUserWithNewT(ConstantsGod.API_BASEURL + ConstantsGod.LoginAPIURL, PlayerPrefs.GetString("JSONdataforlogin"), null, false));
                }
            }
        }
        else
        {
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                ////Debug.Log("Network error in logout from other device");
            }
            else
            {
                if (request.error != null)
                {
                    if (!obj_LogOut.success)
                    {
                        ////Debug.Log("Success false in logout from other device  " + obj_LogOut.msg);
                    }
                }
            }
        }
    }

    [Serializable]
    public class MyClassOfLogoutDevice : JsonObjectBase
    {
        public string email;
        public string phoneNumber;
        public string password;
        public MyClassOfLogoutDevice GetdataFromClass(string L_eml = "", string L_phonenbr = "", string passwrd = "")
        {
            MyClassOfLogoutDevice myObj = new MyClassOfLogoutDevice();
            myObj.email = L_eml;
            myObj.phoneNumber = L_phonenbr;
            myObj.password = passwrd;
            return myObj;
        }

        public MyClassOfLoginJson CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<MyClassOfLoginJson>(jsonString);
        }

    }

    public IEnumerator HitEmailAPI(string URL, string localEmail, WWWForm form)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
        {
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                yield return null;
            }
            if (www.result != UnityWebRequest.Result.ConnectionError)
            {
                validationMessagePopUP.SetActive(true);
                errorTextEmail.SetActive(true);
                // ////Debug.Log("Network Error");
                errorTextEmail.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                // StartCoroutine(WaitUntilAnimationFinished(errorTextEmail.GetComponent<Animator>()));
                errorTextEmail.GetComponent<TextMeshProUGUI>().text = www.error.ToUpper();
                //  ////Debug.Log("WWW Error: " + www.error);  
            }
            else
            {
                if (operation.isDone)
                {
                    MyClassNewApi myObject = new MyClassNewApi();
                    myObject = CheckResponceJsonNewApi(www.downloadHandler.text);
                    if (myObject.success)
                    {
                        OpenUIPanal(3);
                        Email = localEmail;
                    }
                    else
                    {
                        validationMessagePopUP.SetActive(true);
                        errorTextEmail.SetActive(true);
                        errorTextEmail.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                        // StartCoroutine(WaitUntilAnimationFinished(errorTextEmail.GetComponent<Animator>()));
                        errorTextEmail.GetComponent<TextMeshProUGUI>().text = myObject.msg.ToUpper();
                        //    //print("Error Occured " + myObject.msg);
                    }
                }
            }
        }
    }
    ClassWithTokenofResetPassword myObjectofOTPForResetPassword;
    MyClassNewApi myObjectForOPT;
    IEnumerator HitOTPAPI(string url, string Jsondata)
    {
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
        if (ForgetPasswordBool)
        {
            myObjectofOTPForResetPassword = new ClassWithTokenofResetPassword();
            myObjectofOTPForResetPassword = ClassWithTokenofResetPassword.CreateFromJSON(request.downloadHandler.text);
        }
        else
        {
            myObjectForOPT = new MyClassNewApi();
            myObjectForOPT = CheckResponceJsonNewApi(request.downloadHandler.text);
        }

        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            if (request.error == null)
            {
                if (ForgetPasswordBool)
                {
                    if (myObjectofOTPForResetPassword.success == "true")
                    {
                        ForgetPasswordTokenAfterVerifyling = myObjectofOTPForResetPassword.data.tempToken;
                        OpenUIPanal(15);

                        NewLoadingScreen.SetActive(false);

                    }
                }
                else
                {
                    if (myObjectForOPT.success)
                    {
                        BlackScreen.SetActive(true);
                        OpenUIPanal(4);
                        NewLoadingScreen.SetActive(false);
                        SignUpPanal.SetActive(false);
                    }
                }

            }
        }
        else
        {
            if (request.isNetworkError)
            {
                //if (!ConnectionEstablished_popUp.activeInHierarchy)
                //{
                //    // errorTextPIN.GetComponent<Animator>().SetBool("playAnim", true);
                //    validationMessagePopUP.SetActive(true);
                //    errorTextPIN.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                //    // if (Application.systemLanguage == SystemLanguage.Japanese  )
                //    // {
                //    //     errorTextPIN.GetComponent<Text>().text = "接続状態が悪く繋がりません";
                //    // }
                //    // else
                //    // {
                //    //     errorTextPIN.GetComponent<Text>().text = request.error.ToUpper();
                //    // }
                //    errorHandler.ShowErrorMessage(ErrorType.Poor_Connection.ToString(), errorTextPIN.GetComponent<TextMeshProUGUI>());
                //    // StartCoroutine(WaitUntilAnimationFinished(errorTextPIN.GetComponent<Animator>()));
                //}
            }
            else
            {
                if (request.error != null)
                {
                    if (ForgetPasswordBool)
                    {
                        if (myObjectofOTPForResetPassword.success == "false")
                        {
                            validationMessagePopUP.SetActive(true);
                            errorTextPIN.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                            // if (Application.systemLanguage == SystemLanguage.Japanese  )
                            // {
                            //     errorTextPIN.GetComponent<Text>().text = "認証コードが正しくありません";
                            // }
                            // else
                            // {
                            //          errorTextPIN.GetComponent<Text>().text = myObjectofOTPForResetPassword.msg.ToUpper();
                            //   }
                            errorHandler.ShowErrorMessage(ErrorType.Authentication_Code_is_Incorrect.ToString(), errorTextPIN.GetComponent<TextMeshProUGUI>());
                            //  StartCoroutine(WaitUntilAnimationFinished(errorTextPIN.GetComponent<Animator>()));
                        }
                    }
                    else
                    {
                        if (!myObjectForOPT.success)
                        {
                            validationMessagePopUP.SetActive(true);
                            errorTextPIN.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                            // if (Application.systemLanguage == SystemLanguage.Japanese  )
                            // {
                            //     errorTextPIN.GetComponent<Text>().text = "認証コードが正しくありません";
                            // }
                            // else
                            // {
                            //          errorTextPIN.GetComponent<Text>().text = myObjectForOPT.msg.ToUpper();
                            //  }
                            errorHandler.ShowErrorMessage(ErrorType.Authentication_Code_is_Incorrect.ToString(), errorTextPIN.GetComponent<TextMeshProUGUI>());
                            // StartCoroutine(WaitUntilAnimationFinished(errorTextPIN.GetComponent<Animator>()));
                        }

                    }
                }
            }
        }
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
        {
            yield return null;
        }
        MyClassNewApi myObject1 = new MyClassNewApi();
        if (!request.isHttpError && !request.isNetworkError)
        {
            myObject1 = CheckResponceJsonNewApi(request.downloadHandler.text);
            if (request.error == null)
            {
                if (myObject1.success)
                {
                    if (myObject1.msg == "This name is already taken by other user.")
                    {
                        validationMessagePopUP.SetActive(true);
                        errorTextName.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                        errorHandler.ShowErrorMessage("Username already exists", errorTextName.GetComponent<TextMeshProUGUI>());
                        currentSelectedNxtButton.interactable = true;
                        UsernamescreenLoader.SetActive(false);
                    }
                    else
                    {
                        PlayerPrefs.SetInt("IsLoggedIn", 1);
                        PlayerPrefs.SetInt("FristPresetSet", 1);
                        SubmitSetDeviceToken();
                        LoggedInAsGuest = false;
                        PlayerPrefs.SetString("PlayerName", localUsername);

                        OpenUIPanal(16);
                        usernamePanal.SetActive(false);
                        currentSelectedNxtButton.interactable = true;
                        UsernamescreenLoader.SetActive(false);
                        LoggedIn = true;
                    }
                    GameManager.Instance.mainCharacter.GetComponent<CharacterOnScreenNameHandler>().UpdateNameText(localUsername);
                }
            }
        }
        else
        {
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                //if (!ConnectionEstablished_popUp.activeInHierarchy)
                //{
                //    // errorTextName.GetComponent<Animator>().SetBool("playAnim", true);
                //    validationMessagePopUP.SetActive(true);
                //    errorTextName.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                //    // if (Application.systemLanguage == SystemLanguage.Japanese  )
                //    // {
                //    //     errorTextName.GetComponent<Text>().text = "接続状態が悪く繋がりません"; 
                //    // }  
                //    // else
                //    // {
                //    //     errorTextName.GetComponent<Text>().text = request.error.ToUpper();
                //    // }  
                //    errorHandler.ShowErrorMessage(ErrorType.Poor_Connection.ToString(), errorTextName.GetComponent<TextMeshProUGUI>());
                //    currentSelectedNxtButton.interactable = true;
                //    UsernamescreenLoader.SetActive(false);
                //    // StartCoroutine(WaitUntilAnimationFinished(errorTextName.GetComponent<Animator>()));
                //}
            }
            else
            {
                if (request.error != null)
                {
                    myObject1 = CheckResponceJsonNewApi(request.downloadHandler.text);
                    if (!myObject1.success)
                    {
                        validationMessagePopUP.SetActive(true);
                        errorTextName.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                        // if (Application.systemLanguage == SystemLanguage.Japanese  )
                        // {
                        //     errorTextName.GetComponent<Text>().text = "名前が無効です";
                        // }
                        // else
                        // {
                        //     errorTextName.GetComponent<Text>().text = myObject1.msg.ToUpper();
                        // }
                        //errorHandler.ShowErrorMessage(ErrorType.Invalid_Username , errorTextName.GetComponent<Text>());
                        errorHandler.ShowErrorMessage(myObject1.msg, errorTextName.GetComponent<TextMeshProUGUI>());
                        currentSelectedNxtButton.interactable = true;
                        UsernamescreenLoader.SetActive(false);
                    }
                }
            }
        }
    }

    public bool isSetXanaliyaUserName = false;


    IEnumerator HitNameAPIWithXanaliyaUser(string url, string Jsondata, string localUsername)//rik
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

        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            myObject1 = CheckResponceJsonNewApi(request.downloadHandler.text);
            if (request.error == null)
            {
                if (myObject1.success)
                {
                    if (myObject1.msg == "This name is already taken by other user.")
                    {
                        validationMessagePopUP.SetActive(true);
                        errorTextName.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                        errorHandler.ShowErrorMessage("Username already exists", errorTextName.GetComponent<TextMeshProUGUI>());
                        // StartCoroutine(WaitUntilAnimationFinished(errorTextName.GetComponent<Animator>()));
                        currentSelectedNxtButton.interactable = true;
                        UsernamescreenLoader.SetActive(false);
                    }
                    else
                    {

                        isSetXanaliyaUserName = false;
                        PlayerPrefs.SetString("PlayerName", localUsername);
                        usernamePanal.transform.Find("Back-Btn (1)").gameObject.SetActive(true);
                        usernamePanal.SetActive(false);
                        currentSelectedNxtButton.interactable = true;
                        UsernamescreenLoader.SetActive(false);
                    }
                }
            }
        }
        else
        {
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                //if (!ConnectionEstablished_popUp.activeInHierarchy)
                //{
                //    // errorTextName.GetComponent<Animator>().SetBool("playAnim", true);
                //    validationMessagePopUP.SetActive(true);
                //    errorTextName.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                //    errorHandler.ShowErrorMessage(ErrorType.Poor_Connection.ToString(), errorTextName.GetComponent<TextMeshProUGUI>());
                //    currentSelectedNxtButton.interactable = true;
                //    UsernamescreenLoader.SetActive(false);
                //    // StartCoroutine(WaitUntilAnimationFinished(errorTextName.GetComponent<Animator>()));
                //}
            }
            else
            {
                if (request.error != null)
                {
                    myObject1 = CheckResponceJsonNewApi(request.downloadHandler.text);
                    if (!myObject1.success)
                    {
                        validationMessagePopUP.SetActive(true);
                        errorTextName.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                        errorHandler.ShowErrorMessage(myObject1.msg, errorTextName.GetComponent<TextMeshProUGUI>());
                        currentSelectedNxtButton.interactable = true;
                        UsernamescreenLoader.SetActive(false);
                    }
                }
            }
        }
    }

    IEnumerator LoginGuest(string url, bool ComesFromLogOut = false)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(url, "POST"))
        {
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
                        UserPassManager.Instance.GetGroupDetailsForComingSoon();
                        PlayerPrefs.SetInt("FirstTime", 1);
                        PlayerPrefs.Save();

                        ConstantsHolder.userId = myObject1.data.user.id.ToString();
                    }
                }
            }
        }
    }

    IEnumerator LoginUserWithNewT(string url, string Jsondata, GameObject _loader = null, bool AutoLoginBool = false)
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
            if (request.error == null)
            {
                if (myObject1.success)
                {
                    PlayerPrefs.SetString("UserNameAndPassword", Jsondata);
                    if (_loader != null)
                        _loader.SetActive(false);

                    XanaliaUserTokenId = myObject1.data.xanaliaToken;
                    PlayerPrefs.SetString("TermsConditionAgreement", "Agree");
                    PlayerPrefs.SetInt("shownWelcome", 1);
                    PlayerPrefs.SetInt("FirstTime", 1);

                    ConstantsHolder.userId = myObject1.data.user.id.ToString();

                    if (!AutoLoginBool)
                    {
                        savePasswordList.instance.saveData(LoginEmailOrPhone.Text.Trim(), LoginPassword.Text.Trim());
                    }

                    //PlayerPrefs.SetInt("WalletLogin", 0); //  in Each case now we are login with Wallet
                    ConstantsGod.AUTH_TOKEN = myObject1.data.token;
                    PlayerPrefs.SetString("LoginTokenxanalia", myObject1.data.xanaliaToken);
                    //DynamicEventManager.deepLink?.Invoke("Login user here");

                    if (myObject1.data.isAdmin)
                    {
                        UserPassManager.Instance.testing = true;
                    }
                    //else  // Forces Enabled
                    //{
                    //    UserPassManager.Instance.testing = false;
                    //}
                    if (PlayerPrefs.GetString("LoginTokenxanalia") != "" && XanaliaBool)
                    {
                        WalletConnectDataClasses.NFTListMainNet NFTCreateJsonMain = new WalletConnectDataClasses.NFTListMainNet();

                        string xanaliaNetworkType = "mainnet";
                        if (APIBasepointManager.instance != null)
                        {
                            if (APIBasepointManager.instance.IsXanaLive)
                            {
                                xanaliaNetworkType = "mainnet";
                            }
                            else
                            {
                                xanaliaNetworkType = "testnet";
                            }
                        }


                        NFTCreateJsonMain = NFTCreateJsonMain.AssignNFTList(100, xanaliaNetworkType, "mycollection", 1);

                        var jsonObj = JsonUtility.ToJson(NFTCreateJsonMain);

                        StartCoroutine(XanaliaUserToken(ConstantsGod.API_BASEURL_XANALIA + ConstantsGod.userMy_Collection_Xanalia, jsonObj));
                        StartCoroutine(XanaliaNonCryptoNFTRole(ConstantsGod.API_BASEURL_XANALIA + ConstantsGod.getUserProfile_Xanalia));
                    }
                    else
                    {
                        if (PlayerPrefs.GetString("PremiumUserType") == "Access Pass" || PlayerPrefs.GetString("PremiumUserType") == "Extra NFT" || PlayerPrefs.GetString("PremiumUserType") == "astroboy")
                        {
                            UserPassManager.Instance.GetGroupDetails(PlayerPrefs.GetString("PremiumUserType"));
                        }
                        else
                        {
                            UserPassManager.Instance.GetGroupDetails("freeuser");
                        }
                    }

                    if (!string.IsNullOrEmpty(myObject1.data.user.walletAddress) && PlayerPrefs.HasKey("Equiped"))
                        LoadingHandler.Instance.nftLoadingScreen.SetActive(true);

                    PlayerPrefs.SetString("publicID", myObject1.data.user.walletAddress);

                    GetOwnedNFTsFromAPI();
                    UserPassManager.Instance.GetGroupDetailsForComingSoon();
                    SubmitSetDeviceToken();
                    LoggedInAsGuest = false;
                    getdatafromserver();
                    Debug.LogError(myObject1.data.token);
                    var parts = myObject1.data.token.Split('.');
                    if (parts.Length > 2)
                    {
                        var decode = parts[1];
                        var padLength = 4 - decode.Length % 4;
                        if (padLength < 4)
                        {
                            decode += new string('=', padLength);
                        }
                        var bytes = System.Convert.FromBase64String(decode);
                        var userInfo = System.Text.ASCIIEncoding.ASCII.GetString(bytes);
                        LoginClass L_LoginObject = new LoginClass();
                        Debug.LogError("User Info :- " + userInfo);
                        L_LoginObject = CheckResponceJsonOfLogin(userInfo);

                        PlayerPrefs.SetString("UserName", L_LoginObject.id);
                        PlayerPrefs.SetInt("IsLoggedIn", 1);
                        PlayerPrefs.SetInt("FristPresetSet", 1);
                        PlayerPrefs.SetString("PlayerName", myObject1.data.user.name);
                        PlayerPrefs.SetString("LoggedInMail", myObject1.data.user.email);
                        usernamePanal.SetActive(false);
                        usernamePanal.SetActive(false);
                        ConstantsHolder.xanaConstants.LoginasGustprofile = true;
                        CheckCameraMan();
                        PlayerPrefs.Save();
                        InventoryManager.instance.CheckWhenUserLogin();
                        if (!AutoLoginBool)
                        {
                            OpenUIPanal(7);

                            if (PlayerPrefs.GetString("LoginTokenxanalia") != "" && string.IsNullOrEmpty(myObject1.data.user.name))//rik
                            {
                                OpenUIPanal(5);
                                isSetXanaliyaUserName = true;
                                usernamePanal.transform.Find("Back-Btn (1)").gameObject.SetActive(false);
                            }
                        }
                        if (GameManager.Instance.UiManager != null)//rik
                        {
                            GameManager.Instance.UiManager._footerCan.transform.GetChild(0).GetComponent<HomeFooterHandler>().CheckLoginOrNotForFooterButton();
                        }
                    }


                    GameManager.Instance.FriendsHomeManager.GetComponent<FriendHomeManager>().SpawnFriends();

                }
            }
        }
        else
        {
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                //if (!ConnectionEstablished_popUp.activeInHierarchy)
                //{
                //    // errorTextLogin.GetComponent<Animator>().SetBool("playAnim", true);
                //    validationMessagePopUP.SetActive(true);
                //    errorTextLogin.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                //    // if (Application.systemLanguage == SystemLanguage.Japanese  )
                //    // {
                //    //     errorTextLogin.GetComponent<Text>().text = "接続状態が悪く繋がりません";
                //    // }
                //    // else
                //    // {
                //    //     errorTextLogin.GetComponent<Text>().text = request.error.ToUpper();
                //    // }
                //    errorHandler.ShowErrorMessage(ErrorType.Poor_Connection.ToString(), errorTextLogin.GetComponent<TextMeshProUGUI>());
                //    // StartCoroutine(WaitUntilAnimationFinished(errorTextLogin.GetComponent<Animator>()));
                //    if (_loader != null)
                //        _loader.SetActive(false);
                //}
            }
            else
            {
                if (request.error != null)
                {
                    if (!myObject1.success)
                    {
                        validationMessagePopUP.SetActive(true);
                        errorTextLogin.GetComponent<TextMeshProUGUI>().color = new Color(0.44f, 0.44f, 0.44f, 1f);
                        //print("Hey success false " + myObject1.msg);
                        if (myObject1.msg.Contains("You are already logged in another device"))
                        {
                            if (AutoLoginBool)
                            {
                                PlayerPrefs.DeleteAll();
                                yield return null;
                            }
                            else
                            {
                                PlayerPrefs.SetString("JSONdataforlogin", Jsondata);
                                LogoutfromOtherDevicePanel.SetActive(true);
                                MyClassOfLoginJson myObject = new MyClassOfLoginJson();
                                myObject = myObject.CreateFromJSON(Jsondata);
                                MyClassOfLogoutDevice logoutObj = new MyClassOfLogoutDevice();
                                string bodyJson2 = JsonUtility.ToJson(logoutObj.GetdataFromClass(myObject.email, myObject.phoneNumber, myObject.password));
                                PlayerPrefs.SetString("LogoutFromDeviceJSON", bodyJson2);
                            }
                        }

                        //if(myObject1.msg.Contains("Password is incorrect"))
                        //{
                        //    errorHandler.ShowErrorMessage(ErrorType.Wrong_Password, errorTextLogin.GetComponent<Text>());
                        //}

                        //else if(myObject1.msg.Contains("must be a valid email"))
                        //{
                        //    errorHandler.ShowErrorMessage(ErrorType.Please_enter_valid_email, errorTextLogin.GetComponent<Text>());
                        //}

                        //else
                        //{
                        //    errorHandler.ShowErrorMessage(ErrorType.User_Not_Valid, errorTextLogin.GetComponent<Text>());

                        //}

                        errorHandler.ShowErrorMessage(myObject1.msg, errorTextLogin.GetComponent<TextMeshProUGUI>());

                        if (_loader != null)
                            _loader.SetActive(false);
                    }
                }
            }
        }
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

    IEnumerator XanaliaUserToken(string url, string Jsondata)
    {
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(Jsondata);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        string _tokenis = "Bearer " + PlayerPrefs.GetString("LoginTokenxanalia");
        request.SetRequestHeader("Authorization", _tokenis);
        request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        nftlist = request.downloadHandler.text;
        File.WriteAllText((Application.persistentDataPath + "/NftData.txt"), request.downloadHandler.text);
    }
    IEnumerator XanaliaNonCryptoNFTRole(string url)
    {
        UnityWebRequest request = new UnityWebRequest(url, "GET");
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        string _tokenis = "Bearer " + PlayerPrefs.GetString("LoginTokenxanalia");
        request.SetRequestHeader("Authorization", _tokenis);
        request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        WalletConnectDataClasses.RootNonCryptoNFTRole myObject = new WalletConnectDataClasses.RootNonCryptoNFTRole();
        myObject = WalletConnectDataClasses.RootNonCryptoNFTRole.CreateFromJSON(request.downloadHandler.text);

        if (request.result != UnityWebRequest.Result.ConnectionError && request.result == UnityWebRequest.Result.Success)
        {
            if (request.error == null)
            {
                if (myObject.success && myObject.data.userNftRoleArr != null)
                {
                    int x = (int)NftRolePriority.guest;
                    string userNftRole = "free";
                    ConstantsGod.UserRoles = myObject.data.userNftRoleArr.ToList();
                    foreach (string s in myObject.data.userNftRoleArr)
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
            }
        }
        else
        {
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {

            }
            else
            {
                if (request.error != null)
                {
                    if (!myObject.success)
                    {
                        ////Debug.Log("Success false in  in set device token");
                    }
                }
            }
        }
    }

    #region Riken....... For Get UserPriorityRole For Other User
    public string GetOtherUserPriorityRole(List<string> userRoleList)
    {
        int x = (int)NftRolePriority.guest;
        string otherUserNftRole = "free";
        foreach (string s in userRoleList)
        {
            int rolePriority = ReturnNftRole(s);
            if (rolePriority <= x)
            {
                x = rolePriority;
                otherUserNftRole = s;
            }
        }
        return otherUserNftRole;
    }
    #endregion



    void getdatafromserver()
    {
        ServerSideUserDataHandler.Instance.GetDataFromServer();
    }
    public void LoginWithWallet()
    {
        Debug.LogError("login with wallet userregistration");
        PlayerPrefs.SetInt("IsLoggedIn", 1);
        PlayerPrefs.SetInt("FristPresetSet", 1);
        SubmitSetDeviceToken();
        LoggedInAsGuest = false;
        getdatafromserver();
        usernamePanal.SetActive(false);
        GetOwnedNFTsFromAPI();
        PlayerPrefs.Save();
        if (!ConstantsHolder.xanaConstants.isXanaPartyWorld)
        {
            StartCoroutine(GameManager.Instance.mainCharacter.GetComponent<CharacterOnScreenNameHandler>().IERequestGetUserDetails());
        }
        if (GameManager.Instance.UiManager != null)//rik
        {
            GameManager.Instance.UiManager._footerCan.transform.GetChild(0).GetComponent<HomeFooterHandler>().HomeSceneFooterSNSButtonIntrectableTrueFalse();
            GameManager.Instance.UiManager._footerCan.transform.GetChild(0).GetComponent<HomeFooterHandler>().GetComponent<HomeFooterHandler>().CheckLoginOrNotForFooterButton();
        }
    }

    [Serializable]
    public class MyClass
    {
        public MyClass myObject;
        public string success;
        public string data;
        public MyClass Load(string savedData)
        {
            myObject = new MyClass();

            myObject = JsonUtility.FromJson<MyClass>(savedData);
            return myObject;
        }
    }

    [Serializable]
    public class LoginClass : JsonObjectBase
    {
        public string id;
        public string iat;
        public int exp;
        public LoginClass Load(string savedData)
        {
            LoginClass LoginObject = new LoginClass();
            LoginObject = JsonUtility.FromJson<LoginClass>(savedData);
            return LoginObject;
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
    public class MyClassNewApiForStatusCode
    {
        public string statusCode;
        public string error;
        public string message;
        public MyClassNewApiForStatusCode Load(string jsonString)
        {
            return JsonUtility.FromJson<MyClassNewApiForStatusCode>(jsonString);
        }
    }

    [System.Serializable]
    public class UserLoginData
    {
        public bool success = false;


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

    [System.Serializable]
    public class JsonObjectBase
    {
    }

    [Serializable]
    public class MyClassOfPostingOTP : JsonObjectBase
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
    public class MyClassOfLoginJson : JsonObjectBase
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
    public class MyClassOfPostingPhoneOTP : JsonObjectBase
    {
        public string phoneNumber;
        public string otp;
        public MyClassOfPostingPhoneOTP GetdataFromClass(string phonenumber, string otp)
        {
            MyClassOfPostingPhoneOTP myObj = new MyClassOfPostingPhoneOTP();
            myObj.phoneNumber = phonenumber;
            myObj.otp = otp;
            return myObj;
        }
    }

    [Serializable]
    public class MyClassOfPhoneNumber : JsonObjectBase
    {
        public string phoneNumber;
        public MyClassOfPhoneNumber GetdataFromClass(string Cellnumber)
        {
            MyClassOfPhoneNumber myObj = new MyClassOfPhoneNumber();
            myObj.phoneNumber = Cellnumber;
            return myObj;
        }
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
    public class MyClassForChangePassword : JsonObjectBase
    {
        public string oldPassword;
        public string newPassword;

        public MyClassForChangePassword GetChangePassworddata(string oldPass, string NewPass)
        {
            MyClassForChangePassword myObj = new MyClassForChangePassword();
            myObj.oldPassword = oldPass;
            myObj.newPassword = NewPass;
            return myObj;
        }
    }

    LoginClass CheckResponceJsonOfLogin(string Localdata)
    {
        LoginClass myObject = new LoginClass();
        myObject = myObject.Load(Localdata);
        //print("user name in class" + (myObject.id));
        return myObject;
    }

    MyClass CheckResponceJson(string Localdata)
    {
        MyClass myObject = new MyClass();
        myObject = myObject.Load(Localdata);
        return myObject;
    }

    MyClassNewApi CheckResponceJsonNewApi(string Localdata)
    {
        MyClassNewApi myObject = new MyClassNewApi();
        myObject = myObject.Load(Localdata);
        //print("myObject " + myObject.data);
        return myObject;
    }

    IEnumerator WaitUntilAnimationFinished(Animator MyAnim)
    {

        yield return new WaitForSeconds(3f);
        MyAnim.SetBool("playAnim", false);
    }

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



#endregion