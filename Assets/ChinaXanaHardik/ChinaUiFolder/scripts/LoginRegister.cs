using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginRegister : MonoBehaviour
{

    #region PhoneCodeRegisterUI
    public GameObject InputeFieldUI;
    public GameObject PrivacyCheckBox;
    // public GameObject UseragreeCheckBox;
    public GameObject RemberedCheckBox;
    public Toggle remmbered;
    #endregion

    #region LoginUI
    public GameObject InputeFieldUILogin2;
    public GameObject InputeFieldUIphoneLogin2;
    public GameObject PrivacyCheckBoxLogin;
    // public GameObject AgreementCheckBoxLogin;
    public GameObject RemberedCheckBoxLogin;
    public Button GetCodeBtn;
    public Toggle remmberedLogin;
    //  public TMPro.TextMeshProUGUI TimerValue;
    #endregion

    #region VerificationUI
    public GameObject InputeFieldUIVerificationName;
    public GameObject InputeFieldUIVerificationIDCard;
    #endregion

    #region ErrorPopup
    public GameObject ErrorPopup;
    public TMPro.TextMeshProUGUI ErrorTextMsg;
    #endregion

    #region SuccessPopup
    public GameObject SucessPopup;
    public TMPro.TextMeshProUGUI SucessTextMsg;
    #endregion

    #region PolicyPopup
    // public GameObject PrivacyPopup;
    #endregion

    #region RegisterUI

    public GameObject RegisterScreen;
    public TMPro.TMP_InputField PhoneInpute;
    public TMPro.TMP_InputField PasswordInpute;
    public TMPro.TMP_InputField ConfirmPasswordInpute;
    public InputField NicknameInpute;
    public TMPro.TMP_InputField verificationCodeInpute;
    public Button GetcodeRegisterBtn;
    //  public TMPro.TextMeshProUGUI RegisterTimerValue;
    #endregion

    public ChinaErrorHandler ErrorHandler;
    public static LoginRegister instance;

  //  public GameObject Login1;
    public GameObject Login2;
    public GameObject ChinaUiCanvas;
    public GameObject VerificationScreen;
    public GameObject PremiumObject;



    // public GameObject WelcomeScreen;
    public GameObject Loader;
    //public GameObject CharacterLoading;
    //public GameObject Avatar;
   


    float totalTime = 0f; //2 minutes
    float RegistertotalTime = 0f; //2 minutes
    private bool GetCodeBtnCall = false;
    public bool HomeClick = false;
    private bool verify = false;


    #region Xana App Enter Objects
   // public List<GameObject> XanaObjects = new List<GameObject>();
    public static bool ChinaUser = false;
    public static bool ARBack = false;
    public List<string>AlphaPassList=new List<string>();

    #endregion


    // Start is called before the first frame update
    void Start()
    {
        ChinaUser = true;
        Debug.Log("Token auto login===" + PlayerPrefs.GetInt(ChinaConstantGods.AUTH));
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString(ChinaConstantGods.AUTH)) && !PlayerPrefs.GetString(ChinaConstantGods.AUTH).Equals("token"))
        {
            Debug.Log("KYC Value check===" + PlayerPrefs.GetInt(ChinaConstantGods.KYCSTATUS));
            if (PlayerPrefs.GetInt(ChinaConstantGods.KYCSTATUS) == 1)
            {
                //   Login2.SetActive(false);
                //  Loader.SetActive(false);
                StartCoroutine(ChinaScene(ChinaConstantGods.API_BASEURL + ChinaConstantGods.ALPHAPASS));
                StartCoroutine(GetUserInfo(ChinaConstantGods.API_BASEURL + ChinaConstantGods.USERINFO, 1));
                StartCoroutine(WaitForScreen());
            }
        }

        else
        {
            if (!PlayerPrefs.GetString(ChinaConstantGods.REMMBERED).Equals("remmbered") && !string.IsNullOrEmpty(PlayerPrefs.GetString(ChinaConstantGods.REMMBERED))
            && !string.IsNullOrEmpty("0"))
            {
                remmbered.isOn = true;
                RemberedCheckBox.SetActive(true);
                InputeFieldUI.GetComponent<TMPro.TMP_InputField>().text = PlayerPrefs.GetString(ChinaConstantGods.REMMBERED);

            }

            if (!PlayerPrefs.GetString(ChinaConstantGods.REMMBEREDLOGIN).Equals("remmberedLogin") && !string.IsNullOrEmpty(PlayerPrefs.GetString(ChinaConstantGods.REMMBEREDLOGIN))
                && !string.IsNullOrEmpty("0"))
            {
                remmberedLogin.isOn = true;
                RemberedCheckBoxLogin.SetActive(true);
                InputeFieldUIphoneLogin2.GetComponent<TMPro.TMP_InputField>().text = PlayerPrefs.GetString(ChinaConstantGods.REMMBEREDLOGIN);
            }
        }

    }
    private void Awake()
    {
        ChinaUser = true;
        instance = this;
    }

    private void OnEnable()
    {
        ChinaUser = true;

    }
    #region XANAENTERAPI

    IEnumerator RegisterLoginXanaAPI(string url, string Jsondata)
    {
        Debug.Log("Url token==="+url);
        Loader.SetActive(true);
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
            Debug.Log("Get Register response" + request.downloadHandler.text);
        if (request.downloadHandler.text.ToString().Contains("false"))
        {
            Loader.SetActive(false);
            ErrorPopup.SetActive(true);
            Debug.Log("error msg" + request.downloadHandler);
            // ErrorTextMsg.text = "Xana Authentication Failed";
            ErrorTextMsg.text = "使用手机的用户已经存在";

        }
        else
        {
            try
            {
                XanaLoginRegisterForChina bean = Gods.DeserializeJSON<XanaLoginRegisterForChina>(request.downloadHandler.text);

                Debug.Log("login china ==="+ bean.success);
                if (bean.success)
                {

                    PlayerPrefs.SetString("TermsConditionAgreement", "Agree");
                    PlayerPrefs.SetInt("shownWelcome", 1);
                    PlayerPrefs.SetInt("firstTime", 1);

                 

                    //if (!AutoLoginBool)
                    //{
                    //    // savePasswordList.instance.saveData(LoginEmailNew.Text.Trim(), LoginPasswordShiftCode.GetText().Trim());
                    //    savePasswordList.instance.saveData(LoginEmailOrPhone.Text.Trim(), LoginPassword.Text.Trim());
                    //}

                    PlayerPrefs.SetInt("WalletLogin", 0);

                    PlayerPrefs.SetString("UserName", bean.data.user.id.ToString());
                    PlayerPrefs.SetInt("IsLoggedIn", 1);
                    PlayerPrefs.SetInt("firstTime", 1);
                    PlayerPrefs.SetInt("FristPresetSet", 1);

                    print("Alraeady Logged In " + PlayerPrefs.GetInt("IsLoggedIn"));
                    PlayerPrefs.SetString("PlayerName", bean.data.user.name);
                    PlayerPrefs.SetString("token", bean.data.token);
                    ConstantsGod.AUTH_TOKEN = PlayerPrefs.GetString("token"); 
                    Debug.Log("token user==" + ConstantsGod.AUTH_TOKEN);
                   // print("Welcome " + PlayerPrefs.GetString("UserName"));
                    //XanaConstants.xanaConstants.userId = bean.data.user.id.ToString();
                    //PlayerPrefs.SetString("LoggedInMail", bean.data.user.email);

                //    Loader.SetActive(false);
                   // PremiumObject.SetActive(true);


                    //if (AlphaPassList.Contains("xana_builder"))
                    //{
                   
                    SceneManager.LoadScene("Main");
                    //}
                    //else
                    //{
                    //    ErrorPopup.SetActive(true);
                    //    Loader.SetActive(false);
                    //  //  Alpha pass required for this feature
                    //    ErrorTextMsg.text = "此功能需要 Alpha 通行证";
                    //    Loader.SetActive(false);
                    //    Login2.SetActive(true);
                    //}
                }
                else
                {
                    ErrorPopup.SetActive(true);
                    Loader.SetActive(false);
                    ErrorTextMsg.text = "验证";
                    Loader.SetActive(false);
                    Login2.SetActive(true);

                }

            }
            catch (Exception e)
            {
               
                ErrorPopup.SetActive(true);
                Loader.SetActive(false);
                ErrorTextMsg.text = "验证";
                Loader.SetActive(false);
                Login2.SetActive(true);
            }
        }
               

    }
    #endregion


    IEnumerator WaitForScreen()
    {
       
        //WelcomeScreen.SetActive(true);
        if (PlayerPrefs.GetInt("IsLoggedIn") != 1)
        {
            StartCoroutine(ChinaScene(ChinaConstantGods.API_BASEURL + ChinaConstantGods.ALPHAPASS));
            //  WelcomeScreen.SetActive(true);
            RegisterXanaController controller1 = new RegisterXanaController();
            string bodyJson1 = JsonUtility.ToJson(controller1.GetUpdatedValue(PlayerPrefs.GetInt(ChinaConstantGods.USERID), PlayerPrefs.GetString(ChinaConstantGods.MOBILENUMBER)));
            Debug.Log("Json China Xana===" + bodyJson1);
            //  StartCoroutine(RegisterLoginXanaAPI(ServerManagerChina.Instance.CHINATOKENAPI, bodyJson1));
             StartCoroutine(RegisterLoginXanaAPI(ConstantsGod.API_BASEURL + "/auth/authenticate-cn-user", bodyJson1));
        }
        else
        {
            //Login2.SetActive(false);
            StartCoroutine(ChinaScene(ChinaConstantGods.API_BASEURL + ChinaConstantGods.ALPHAPASS));

            RegisterXanaController controller1 = new RegisterXanaController();
            string bodyJson1 = JsonUtility.ToJson(controller1.GetUpdatedValue(PlayerPrefs.GetInt(ChinaConstantGods.USERID), PlayerPrefs.GetString(ChinaConstantGods.MOBILENUMBER)));
            Debug.Log("Json China Xana===" + bodyJson1);
            //  StartCoroutine(RegisterLoginXanaAPI(ServerManagerChina.Instance.CHINATOKENAPI, bodyJson1));
            StartCoroutine(RegisterLoginXanaAPI(ConstantsGod.API_BASEURL + "/auth/authenticate-cn-user", bodyJson1));
        }
        yield return null;
    }

    #region validation
    public void ValidationCheck()
    {
        if (string.IsNullOrEmpty(InputeFieldUI.GetComponent<TMPro.TMP_InputField>().text))
        {
            ErrorPopup.SetActive(true);
            // ErrorTextMsg.text = "Please enter the inpute in both fields";
            ErrorTextMsg.text = "请在两个字段中输入输入";
            //ErrorHandler.ShowErrorMessage(ErrorType.Default_Message.ToString(), ErrorTextMsg);

        }
        else if (!PrivacyCheckBox.activeInHierarchy)
        {
            ErrorPopup.SetActive(true);
            ErrorTextMsg.text = "请检查隐私 / 用户协议";
            //ErrorTextMsg.text = "Please check the privacy/user agreement";
            //  ErrorHandler.ShowErrorMessage(ErrorType.Default_Message.ToString(), ErrorTextMsg);
        }
        else
        {
            if (RemberedCheckBox.activeInHierarchy)
            {
                PlayerPrefs.SetString(ChinaConstantGods.REMMBERED, InputeFieldUI.GetComponent<TMPro.TMP_InputField>().text.ToString());
                PhoneCodeController controller2 = new PhoneCodeController();
                string bodyJson2 = JsonUtility.ToJson(controller2.GetUpdatedValue("1", InputeFieldUI.GetComponent<TMPro.TMP_InputField>().text.ToString(), "60"));
                StartCoroutine(GetPhoneVerificationAPI(ChinaConstantGods.API_BASEURL + ChinaConstantGods.GETVERIFICATIONCODE, bodyJson2));

            }
            else
            {
                PlayerPrefs.SetString(ChinaConstantGods.REMMBERED, string.Empty);
                PhoneCodeController controller2 = new PhoneCodeController();
                string bodyJson2 = JsonUtility.ToJson(controller2.GetUpdatedValue("1", InputeFieldUI.GetComponent<TMPro.TMP_InputField>().text.ToString(), "60"));
                StartCoroutine(GetPhoneVerificationAPI(ChinaConstantGods.API_BASEURL + ChinaConstantGods.GETVERIFICATIONCODE, bodyJson2));
            }

        }
    }

    #region AllScene

    IEnumerator ChinaScene(string url)
    {
        Debug.Log("url----" + url);
        Loader.SetActive(true);

        var request = new UnityWebRequest(url, "GET");
        //byte[] bodyRaw = Encoding.UTF8.GetBytes(Jsondata);
        //request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        Debug.Log("Token====" + PlayerPrefs.GetString(ChinaConstantGods.AUTH));
        request.SetRequestHeader("Authorization", "bearer" + PlayerPrefs.GetString(ChinaConstantGods.AUTH));
        request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        if (request.result != UnityWebRequest.Result.Success)
        {
            Loader.SetActive(false);
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log("alpha pass reponse======="+ request.downloadHandler.text);
            AllNftScenes bean = Gods.DeserializeJSON<AllNftScenes>(request.downloadHandler.text);
            if (bean.code == 0)
            {
                if (bean.data.sceneList.Count > 0)
                {
                    AlphaPassList.Clear();

                    AlphaPassList.AddRange(bean.data.sceneList);
                }
               
            }
        }


    }
    #endregion


    public void RegisterValidationCheck()
    {

        if (string.IsNullOrEmpty(PhoneInpute.text.ToString()) || string.IsNullOrEmpty(PasswordInpute.text.ToString()) || string.IsNullOrEmpty(ConfirmPasswordInpute.text.ToString())
            || string.IsNullOrEmpty(NicknameInpute.text.ToString()) || string.IsNullOrEmpty(verificationCodeInpute.text.ToString()))
        {
            ErrorPopup.SetActive(true);
            //  ErrorTextMsg.text = "Please enter the inpute in both fields";
            ErrorTextMsg.text = "请在两个字段中输入输入";
            //ErrorHandler.ShowErrorMessage(ErrorType.Default_Message.ToString(), ErrorTextMsg);

        }
        else if (!PasswordInpute.text.ToString().Equals(ConfirmPasswordInpute.text.ToString()))
        {
            ErrorPopup.SetActive(true);
            //ErrorTextMsg.text = "Please Re-enter confirm password";
            ErrorTextMsg.text = "请重新输入确认密码";
        }

        else
        {
            Debug.Log("Phone Chnages===" + InputeFieldUI.GetComponent<TMPro.TMP_InputField>().text.ToString());

            RegisterController controller2 = new RegisterController();
            string bodyJson2 = JsonUtility.ToJson(controller2.GetUpdatedValue(PhoneInpute.text.ToString(), PasswordInpute.text.ToString(), verificationCodeInpute.text.ToString(), "ios", NicknameInpute.text.ToString()));
            StartCoroutine(RegisterAPI(ChinaConstantGods.API_BASEURL + ChinaConstantGods.APPREGISTERPOST, bodyJson2));
        }
    }

    public void Login2ValidationCheck()
    {
        Debug.Log("Value of inpute===="+InputeFieldUIphoneLogin2.GetComponent<TMPro.TMP_InputField>().text);
        if (string.IsNullOrEmpty(InputeFieldUIphoneLogin2.GetComponent<TMPro.TMP_InputField>().text) || string.IsNullOrEmpty(InputeFieldUILogin2.GetComponent<TMPro.TMP_InputField>().text))
        {
            ErrorPopup.SetActive(true);
            //ErrorTextMsg.text = "Please enter the inpute in both fields";
            ErrorTextMsg.text = "请填写手机号和登录验证码"; 
            //ErrorHandler.ShowErrorMessage(ErrorType.Default_Message.ToString(), ErrorTextMsg);
        }
        else if (!PrivacyCheckBoxLogin.activeInHierarchy)
        {
            ErrorPopup.SetActive(true);
            // ErrorTextMsg.text = "Please check the privacy/user agreement";
            ErrorTextMsg.text = "请检查并同意用户协议/隐私政策";
            //ErrorHandler.ShowErrorMessage(ErrorType.Default_Message.ToString(), ErrorTextMsg);
        }
        else
        {
           
            if (RemberedCheckBoxLogin.activeInHierarchy)
            {
                PlayerPrefs.SetString(ChinaConstantGods.REMMBEREDLOGIN, InputeFieldUIphoneLogin2.GetComponent<TMPro.TMP_InputField>().text.ToString());
                
                StartCoroutine(LoginAPI(ChinaConstantGods.API_BASEURL + ChinaConstantGods.LOGINPOST, InputeFieldUIphoneLogin2.GetComponent<TMPro.TMP_InputField>().text.ToString(),
                    InputeFieldUILogin2.GetComponent<TMPro.TMP_InputField>().text.ToString(), "2"));

            }
            else
            {
                PlayerPrefs.SetString(ChinaConstantGods.REMMBEREDLOGIN, string.Empty);
                StartCoroutine(LoginAPI(ChinaConstantGods.API_BASEURL + ChinaConstantGods.LOGINPOST, InputeFieldUIphoneLogin2.GetComponent<TMPro.TMP_InputField>().text.ToString(),
                     InputeFieldUILogin2.GetComponent<TMPro.TMP_InputField>().text.ToString(), "2"));
            }

        }
    }


    public void VerificationValidationCheck()
    {
        if (string.IsNullOrEmpty(InputeFieldUIVerificationName.GetComponent<InputField>().text) || string.IsNullOrEmpty(InputeFieldUIVerificationIDCard.GetComponent<TMPro.TMP_InputField>().text))
        {
            verify = true;
            VerificationScreen.SetActive(false);
            ErrorPopup.SetActive(true);
           // ErrorTextMsg.text = "Please enter the inpute in both fields";
            ErrorTextMsg.text = "请在两个字段中输入输入";
            //ErrorHandler.ShowErrorMessage(ErrorType.Default_Message.ToString(), ErrorTextMsg);
        }

        else
        {


            VerificationController controller1 = new VerificationController();
            string bodyJson1 = JsonUtility.ToJson(controller1.GetUpdatedValue(InputeFieldUIVerificationName.GetComponent<InputField>().text, InputeFieldUIVerificationIDCard.GetComponent<TMPro.TMP_InputField>().text));
            StartCoroutine(VerificationCodeAPI(ChinaConstantGods.API_BASEURL + ChinaConstantGods.VERIFICATIONPOST, bodyJson1));
        }
    }

    #endregion

    #region ClosePopup
    public void ClosePopup()
    {
        ErrorPopup.SetActive(false);

        if (ErrorTextMsg.text.Equals("Real name verification is\nnot performed") || ErrorTextMsg.text.Equals("未进行实名验证"))
        {
            VerificationScreen.SetActive(true);
        }

        if (verify)
        {
            VerificationScreen.SetActive(true);
        }

    }

    public void SuccessClosePopup()
    {
        SucessPopup.SetActive(false);
         if (Login2.activeInHierarchy)
        {
            Debug.Log("SucessTextMsg.text"+ SucessTextMsg.text);
            if (!SucessTextMsg.text.Equals("实名认证申请成功"))
            {
                if (PlayerPrefs.GetInt(ChinaConstantGods.KYCSTATUS) == 0 && !GetCodeBtnCall)
                {
                    totalTime = 0f;
                    VerificationScreen.SetActive(true);
                    InputeFieldUIVerificationName.GetComponent<InputField>().text = string.Empty;
                    InputeFieldUIVerificationIDCard.GetComponent<TMPro.TMP_InputField>().text = string.Empty;
                }
                else if (SucessTextMsg.text.Equals("Login") || SucessTextMsg.text.Equals("登录成功"))
                {

                    StartCoroutine(WaitForScreen());

                }

            }
            else if (SucessTextMsg.text.Equals("实名认证申请成功"))
            {
              
                StartCoroutine(WaitForScreen());
            }


        }
        else if (RegisterScreen.activeInHierarchy)
        {
            if (!GetCodeBtnCall)
            {
                Login2.SetActive(true);
                RegisterScreen.SetActive(false);
                GetCodeBtn.interactable = true;
            }
            else
            {
                GetCodeBtnCall = false;
            }

        }
    }
    public void VerificationClosePopup()
    {
        VerificationScreen.SetActive(false);
        InputeFieldUILogin2.GetComponent<TMPro.TMP_InputField>().textComponent.text = string.Empty;
        InputeFieldUIphoneLogin2.GetComponent<TMPro.TMP_InputField>().textComponent.text = string.Empty;
        totalTime = 0f;
    }

    #endregion


    #region RegisterAPI
    IEnumerator RegisterAPI(string url, string Jsondata)
    {
        Loader.SetActive(true);
        Debug.Log("Code value Register===" + Jsondata);
        Debug.Log("Code aya1===");

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
        Debug.Log("Get Register response" + request.downloadHandler.text);
        RegisterResponse bean = Gods.DeserializeJSON<RegisterResponse>(request.downloadHandler.text);

        if (bean.code == 0)
        {
            Loader.SetActive(false);
            GetCodeBtnCall = false;
            SucessPopup.SetActive(true);
            //SucessTextMsg.text = "Register Successfully";
            SucessTextMsg.text = "注册成功";
        }
        else
        {

            Loader.SetActive(false);
            ErrorPopup.SetActive(true);
            GetCodeBtnCall = false;
            ErrorTextMsg.text = bean.message;
        }

    }

    #endregion

    #region Logout
    public void Logout()
    {
        //PlayerPrefs.SetInt(ChinaConstantGods.KYCSTATUS, 255);
        //PlayerPrefs.DeleteKey(ChinaConstantGods.AUTH);
        Login2.SetActive(true);
    }

    #endregion


    #region LoginAPI

    IEnumerator LoginAPI(string url, string userValue, string codeValue, string loginTypeValue)
    {
        Debug.Log("Code value login==="+codeValue);
        Loader.SetActive(true);
        // print("Body " + Jsondata);
        WWWForm form = new WWWForm();
        form.AddField("username", userValue);
        form.AddField("code", codeValue);
        form.AddField("loginType", loginTypeValue);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {                                                                                                                                                                                           
                Loader.SetActive(false);
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Login response" + www.downloadHandler.text);
                LoginResponse bean = Gods.DeserializeJSON<LoginResponse>(www.downloadHandler.text);
                if (bean.code == 0)
                {
                    Loader.SetActive(false);
                    PlayerPrefs.SetString(ChinaConstantGods.AUTH, bean.data);
                    PlayerPrefs.Save();
                    StartCoroutine(GetUserInfo(ChinaConstantGods.API_BASEURL + ChinaConstantGods.USERINFO, 0));
                }
                else
                {
                    ChinaConstantGods.USERNOTFOUND = bean.message;
                    Loader.SetActive(false);
                    ErrorPopup.SetActive(true);
                    GetCodeBtnCall = false;
                    ErrorTextMsg.text = bean.message;
                }
            }
        }
    }


    public void BackToUI()
    {
        PrivacyCheckBox.SetActive(false);
        RemberedCheckBox.SetActive(false);
    }
                          

    #region CodeTimer


    private void Update()
    {

        if (Login2.activeInHierarchy)
        {
            if (totalTime > 0)
            {
                totalTime -= Time.deltaTime;
                GetCodeBtn.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "00:" + totalTime.ToString().Split('.')[0];
                if (GetCodeBtn.interactable)
                {
                    GetCodeBtn.interactable = false;

                }
            }
            else
            {
                GetCodeBtn.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "发送验证码";
                if (!GetCodeBtn.interactable)
                {
                    GetCodeBtn.interactable = true;
                }
            }
        }
        else if (RegisterScreen.activeInHierarchy)
        {
            if (RegistertotalTime > 0)
            {
                RegistertotalTime -= Time.deltaTime;
                GetcodeRegisterBtn.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "00:" + RegistertotalTime.ToString().Split('.')[0];
                if (GetcodeRegisterBtn.interactable)
                {
                    GetcodeRegisterBtn.interactable = false;
                }
            }
            else
            {
                GetcodeRegisterBtn.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "发送验证码";
                if (!GetcodeRegisterBtn.interactable)
                {
                    GetcodeRegisterBtn.interactable = true;
                }
            }
        }



        //if (PlayerPrefs.GetInt("IsLoggedIn") == 1 && Loader.activeInHierarchy)
        //{
        //    Loader.SetActive(false);
        //}
    }
    public void GetCodePhone()
    {

        if (string.IsNullOrEmpty(InputeFieldUIphoneLogin2.GetComponent<TMPro.TMP_InputField>().text))
        {
            ErrorPopup.SetActive(true);
            //    ErrorTextMsg.text = "Please enter the phone number";
            ErrorTextMsg.text = "请填写手机号和验证码";
            //ErrorHandler.ShowErrorMessage(ErrorType.Default_Message.ToString(), ErrorTextMsg);
        }


        else
        {

            if (RemberedCheckBoxLogin.activeInHierarchy)
            {
                PlayerPrefs.SetString(ChinaConstantGods.REMMBEREDLOGIN, InputeFieldUIphoneLogin2.GetComponent<TMPro.TMP_InputField>().text.ToString());
                PhoneCodeController controller2 = new PhoneCodeController();
                string bodyJson2 = JsonUtility.ToJson(controller2.GetUpdatedValue("2", InputeFieldUIphoneLogin2.GetComponent<TMPro.TMP_InputField>().text.ToString(), "60"));
                StartCoroutine(GetPhoneVerificationAPI(ChinaConstantGods.API_BASEURL + ChinaConstantGods.GETVERIFICATIONCODE, bodyJson2));
            }
            else
            {
                PlayerPrefs.SetString(ChinaConstantGods.REMMBEREDLOGIN, string.Empty);
                PhoneCodeController controller2 = new PhoneCodeController();
                string bodyJson2 = JsonUtility.ToJson(controller2.GetUpdatedValue("2", InputeFieldUIphoneLogin2.GetComponent<TMPro.TMP_InputField>().text.ToString(), "60"));
                StartCoroutine(GetPhoneVerificationAPI(ChinaConstantGods.API_BASEURL + ChinaConstantGods.GETVERIFICATIONCODE, bodyJson2));
            }

        }

    }

    public void GetCodeRegisterPhone()
    {

        if (string.IsNullOrEmpty(PhoneInpute.text))
        {
            ErrorPopup.SetActive(true);
            //ErrorTextMsg.text = "Please enter the phone number";
            ErrorTextMsg.text = "请输入电话号码";
            //ErrorHandler.ShowErrorMessage(ErrorType.Default_Message.ToString(), ErrorTextMsg);
        }


        else
        {
            PhoneCodeController controller2 = new PhoneCodeController();
            string bodyJson2 = JsonUtility.ToJson(controller2.GetUpdatedValue("1", PhoneInpute.text.ToString(), "60"));
            StartCoroutine(GetPhoneVerificationAPI(ChinaConstantGods.API_BASEURL + ChinaConstantGods.GETVERIFICATIONCODE, bodyJson2));
        }

    }




    #region PrivacyPolicyUrl
    public void PrivacyOpen()
    {
        Application.OpenURL("https://xanalia-online.oss-cn-shanghai.aliyuncs.com/DocFile/XANA%E9%9A%90%E7%A7%81%E6%94%BF%E7%AD%96.docx");
        // PrivacyPopup.SetActive(false);
    }
    #endregion


    #region UserAgreement
    public void AgreementOpen()
    {
        Application.OpenURL("https://xanalia-online.oss-cn-shanghai.aliyuncs.com/DocFile/XANA%E7%94%A8%E6%88%B7%E5%8D%8F%E8%AE%AE.docx");
        //  PrivacyPopup.SetActive(false);
    }
    #endregion

    #region UserInfo

    IEnumerator GetUserInfo(string url, int checkvalue)
    {
        Loader.SetActive(true);
        var request = new UnityWebRequest(url, "GET");
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        Debug.Log("Token====" + PlayerPrefs.GetString(ChinaConstantGods.AUTH));
        request.SetRequestHeader("Authorization", "bearer" + PlayerPrefs.GetString(ChinaConstantGods.AUTH));
        request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        if (request.result != UnityWebRequest.Result.Success)
        {
            Loader.SetActive(false);
            Debug.Log(request.error);
        }
        Debug.Log("userInfo======="+ request.downloadHandler.text);
        GetUserInfoDetails bean = Gods.DeserializeJSON<GetUserInfoDetails>(request.downloadHandler.text);

        if (bean.code == 0)
        {
            Loader.SetActive(false);

            if (bean.data.isPassKyc == 0)
            {
                SucessPopup.SetActive(true);
                //  SucessTextMsg.text = "Successfull!";
                SucessTextMsg.text = "成功的！";
                GetCodeBtnCall = false;
                PlayerPrefs.SetInt(ChinaConstantGods.USERID, bean.data.userId);
                PlayerPrefs.SetString(ChinaConstantGods.MOBILENUMBER, bean.data.mobile);

            }
            else
            {
                if (checkvalue == 0)
                {
                    SucessPopup.SetActive(true);
                    //SucessTextMsg.text = "Login";
                    SucessTextMsg.text = "登录成功";
                    PlayerPrefs.SetInt(ChinaConstantGods.USERID, bean.data.userId);
                    PlayerPrefs.SetString(ChinaConstantGods.MOBILENUMBER, bean.data.mobile);
                }
                else
                {
                    PlayerPrefs.SetInt(ChinaConstantGods.USERID, bean.data.userId);
                    PlayerPrefs.SetString(ChinaConstantGods.MOBILENUMBER, bean.data.mobile);
                    StartCoroutine(WaitForScreen());
                }

            }
            PlayerPrefs.SetInt(ChinaConstantGods.KYCSTATUS, bean.data.isPassKyc);
        }
        else
        {
            Loader.SetActive(false);
            ErrorPopup.SetActive(true);
            GetCodeBtnCall = false;
            ErrorTextMsg.text = "verification failed";

        }

        Debug.Log("Verification response" + request.downloadHandler.text);
    }

    #endregion
    public void UpdateLevelTimer(float totalSeconds)
    {
        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        int seconds = Mathf.RoundToInt(totalSeconds % 60f);

        string formatedSeconds = seconds.ToString();

        if (seconds == 60)
        {
            seconds = 0;
            minutes += 1;
        }


    }
    #endregion

    #endregion

    #region VerficationAPI
    IEnumerator VerificationCodeAPI(string url, string Jsondata)
    {
        Loader.SetActive(true);
        // print("Body " + Jsondata);
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(Jsondata);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        Debug.Log("Token====" + PlayerPrefs.GetString(ChinaConstantGods.AUTH));
        request.SetRequestHeader("Authorization", "bearer" + PlayerPrefs.GetString(ChinaConstantGods.AUTH));
        request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        if (request.result != UnityWebRequest.Result.Success)
        {
            Loader.SetActive(false);
            Debug.Log(request.error);
        }

        VerificationResponse bean = Gods.DeserializeJSON<VerificationResponse>(request.downloadHandler.text);

        if (bean.code == 0)
        {
            if (bean.data)
            {
              

                StartCoroutine(GetUserInfo(ChinaConstantGods.API_BASEURL + ChinaConstantGods.USERINFO, 0));
                verify = false;
                Loader.SetActive(false);
                VerificationScreen.SetActive(false);
                SucessPopup.SetActive(true);
                // SucessTextMsg.text = "The real name authentication\napplication is successfull";
                SucessTextMsg.text = "实名认证申请成功";

                
            }
            else
            {
                verify = true;
                Loader.SetActive(false);
                VerificationScreen.SetActive(false);
                ErrorPopup.SetActive(true);
                //  ErrorTextMsg.text = "Real name verification is\nnot performed";
                ErrorTextMsg.text = "未进行实名验证";
            }


        }
        else
        {
            verify = true;
            Loader.SetActive(false);
            VerificationScreen.SetActive(false);
            ErrorPopup.SetActive(true);
            // ErrorTextMsg.text = "Real name verification is\nnot performed";
            ErrorTextMsg.text = "未进行实名验证";
        }

        Debug.Log("Verification response" + request.downloadHandler.text);

    }
    #endregion

    #region GetVerifactionCodeAPI
    IEnumerator GetPhoneVerificationAPI(string url, string Jsondata)
    {
        Loader.SetActive(true);
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

        if (request.result != UnityWebRequest.Result.Success)
        {
            Loader.SetActive(false);
            Debug.Log(request.error);
        }
        PhoneCodeResponse bean = Gods.DeserializeJSON<PhoneCodeResponse>(request.downloadHandler.text.ToString().Trim());
        Debug.Log("Otp Validation==="+ request.downloadHandler.text.ToString().Trim());
        if (bean.code == 0)
        {
            Loader.SetActive(false);
            SucessPopup.SetActive(true);
            if (!string.IsNullOrEmpty(bean.message))
            {
                if ( RegisterScreen.activeInHierarchy)
                {
                    RegistertotalTime = 60f;
                    GetcodeRegisterBtn.interactable = false;
                    GetCodeBtnCall = true;
                    SucessTextMsg.text = bean.message;
                }
                else
                {
                    totalTime = 60f;
                    GetCodeBtn.interactable = false;
                    GetCodeBtnCall = true;
                    SucessTextMsg.text = bean.message;

                }

            }
            else
            {
                Loader.SetActive(false);
                if (RegisterScreen.activeInHierarchy)
                {
                    RegistertotalTime = 60f;
                    GetcodeRegisterBtn.interactable = false;
                    GetCodeBtnCall = true;
                    //SucessTextMsg.text = bean.message;
                }
                else
                {
                    totalTime = 60f;
                    GetCodeBtn.interactable = false;
                    GetCodeBtnCall = true;
                  //  SucessTextMsg.text = bean.message;

                }

                //  SucessTextMsg.text = "Code send successfully ";
                SucessTextMsg.text = "验证码发送成功 ";
            }
        }
        else
        {
            Loader.SetActive(false);
            ErrorPopup.SetActive(true);
            GetCodeBtnCall = false;
            ErrorTextMsg.text = bean.message;
        }
    }
    #endregion

    #region RegisterParam
    public class RegisterController : JsonObjectBase
    {
        public string mobile;
        public string loginPwd;
        public string code;
        public string regSource;
        public string nickName;
        public RegisterController GetUpdatedValue(string mob, string loginPass, string codeValue, string regSourceValue, string nickNameValue)
        {
            RegisterController myObj = new RegisterController();
            myObj.mobile = mob;
            myObj.loginPwd = loginPass;
            myObj.code = codeValue;
            myObj.regSource = regSourceValue;
            myObj.nickName = nickNameValue;
            return myObj;
        }
    }
    #endregion


    #region RegisterXanaParam
    public class RegisterXanaController : JsonObjectBase
    {
        public int userId;
        public string mobile;

        public RegisterXanaController GetUpdatedValue(int userIdChina, string mobileChina)
        {
            RegisterXanaController myObj = new RegisterXanaController();
            myObj.userId = userIdChina;
            myObj.mobile = mobileChina;
            return myObj;
        }
    }
    #endregion

    #region VerificationParam
    public class VerificationController : JsonObjectBase
    {
        public string realName;
        public string cardNo;
        public VerificationController GetUpdatedValue(string relaNameValue, string cardNoValue)
        {
            VerificationController myObj = new VerificationController();
            myObj.realName = relaNameValue;
            myObj.cardNo = cardNoValue;
            return myObj;
        }
    }
    #endregion

    #region PhoneCodeParam
    public class PhoneCodeController : JsonObjectBase
    {
        public string type;
        public string mobile;
        public string duration;
        public PhoneCodeController GetUpdatedValue(string typeValue, string mobvalue, string durationValue)
        {
            PhoneCodeController myObj = new PhoneCodeController();
            myObj.type = typeValue;
            myObj.mobile = mobvalue;
            myObj.duration = durationValue;
            return myObj;
        }
    }
    #endregion

    #region JsonObjectBaseClass
    [System.Serializable]
    public class JsonObjectBase
    {
    }
    #endregion


    #region LoginResponse
    public class LoginResponse
    {
        public int code { get; set; }
        public string data { get; set; }
        public string message { get; set; }
    }
    #endregion

    #region GetPhoneCodeResponse
    public class PhoneCodeResponse
    {
        public int code { get; set; }
        public string message { get; set; }
    }
    #endregion

    #region RegisterResponse
    public class RegisterResponse
    {
        public int code { get; set; }
        public string message { get; set; }
    }
    #endregion

    #region GetVerificationResponse
    public class VerificationResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public bool data { get; set; }
    }
    #endregion

    #region GetUserInfo

    public class UserData
    {
        public int userId { get; set; }
        public string nickName { get; set; }
        public string mobile { get; set; }
        public int userStatus { get; set; }
        public DateTime registerTime { get; set; }
        public DateTime updateTime { get; set; }
        public int isPassKyc { get; set; }
    }

    public class GetUserInfoDetails
    {
        public int code { get; set; }
        public string message { get; set; }
        public UserData data { get; set; }
    }
    #endregion

    #region RegisterApp


    #endregion

    #region XanaLoginRegister

    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public string coins { get; set; }
        public bool isVerified { get; set; }
    }

    public class Data
    {
        public string token { get; set; }
        public User user { get; set; }
    }

    public class XanaLoginRegisterForChina
    {
        public bool success { get; set; }
        public Data data { get; set; }
        public string msg { get; set; }
    }
    #endregion

    #region AllScene

    public class SceneData
    {
        public List<string> sceneList { get; set; }
    }

    public class AllNftScenes
    {
        public int code { get; set; }
        public string message { get; set; }
        public SceneData data { get; set; }
    }
    #endregion
}
