using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Character")]
    public GameObject mainCharacter;
    public GameObject m_ChHead;
    [Header("Character Animator")]
    public Animator m_CharacterAnimator;
    RuntimeAnimatorController m_AnimControlller;
    public int defaultSelection; // for footer bottom Manager

    [Header("Camera's")]
    public Camera m_MainCamera;
    public Camera m_RenderTextureCamera;
    [Header("Objects During Flow")]
    public GameObject BGPlane;
    public bool WorldBool;
    public bool BottomAvatarButtonBool;
    public bool OnceGuestBool;
    public bool OnceLoginBool;
    [Header("Camera Work")]
    public GameObject faceMorphCam;
    public GameObject headCam;
    public GameObject bodyCam;
    public GameObject RequiredNFTPopUP;
    public GameObject ShadowPlane;
    public SavaCharacterProperties SaveCharacterProperties;
    public EquipUI EquipUiObj;
    public BlendShapeImporter BlendShapeImporter;
    public bool UserStatus_;   //if its true user is logged in else its as a guest
    public static string currentLanguage = "";
    public bool isStoreAssetDownloading = false;
    public Transform PostManager;

    //Script references
    public AvatarPathSystemManager avatarPathSystemManager;
    public ActorManager ActorManager;
    public MoodManager moodManager;
    public UserAnimationPostFeature userAnimationPostFeature;
    public Transform FriendsHomeManager;
    public AdditiveScenesManager additiveScenesManager;
    public HomeCameraController HomeCamera;
    public UIManager UiManager;
    public BottomTabManager bottomTabManagerInstance;
    internal string selectedPresetData="";
    private void Awake()
    {
        Debug.Log("GameManager Awake");
        if (Instance == null)
            Instance = this;
        PlayerPrefs.SetInt("presetPanel", 0);  // was loggedin as account 
        if (additiveScenesManager == null) // If Null then find object
        {
           additiveScenesManager = FindObjectOfType<AdditiveScenesManager>();
        }
    }
    public void HomeCameraInputHandler(bool flag)
    {
        HomeCamera.GetComponent<HomeCameraController>().InputFlag = flag;
    }
    public string GetStringFolderPath()
    {
        if (PlayerPrefs.GetInt("IsLoggedIn") == 1)  // loged from account)
        {
            if (menuAvatarFlowButton._instance)   // Disable Store Btn
                menuAvatarFlowButton._instance.StoreBtnController();

            if (PlayerPrefs.HasKey("Equiped") || XanaConstants.xanaConstants.isNFTEquiped)
            {
                if (File.Exists(Application.persistentDataPath + XanaConstants.xanaConstants.NFTBoxerJson))
                {
                    XanaConstants.xanaConstants.clothJson = File.ReadAllText(Application.persistentDataPath + XanaConstants.xanaConstants.NFTBoxerJson);
                }
                return (Application.persistentDataPath + XanaConstants.xanaConstants.NFTBoxerJson);
            }
            else if (PlayerPrefs.GetInt("presetPanel") == 1)  // presetpanel enabled account)
            {
                if (File.Exists(Application.persistentDataPath + "/SavingReoPreset.json"))
                {
                    XanaConstants.xanaConstants.clothJson = File.ReadAllText(Application.persistentDataPath + "/SavingReoPreset.json");
                }
                return (Application.persistentDataPath + "/SavingReoPreset.json");
            }
            else
            {
                UserStatus_ = true;
                if (File.Exists(Application.persistentDataPath + "/logIn.json"))
                {
                    XanaConstants.xanaConstants.clothJson = File.ReadAllText(Application.persistentDataPath + "/logIn.json");
                }
                return (Application.persistentDataPath + "/logIn.json");
            }
        }
        else
        {
            if (PlayerPrefs.GetInt("presetPanel") == 1)  // presetpanel enabled account)
            {
                if (File.Exists(Application.persistentDataPath + "/SavingReoPreset.json"))
                {
                    XanaConstants.xanaConstants.clothJson = File.ReadAllText(Application.persistentDataPath + "/SavingReoPreset.json");
                }
                return (Application.persistentDataPath + "/SavingReoPreset.json");
            }
            else
            {
                UserStatus_ = false;
                if (File.Exists(Application.persistentDataPath + "/loginAsGuestClass.json"))
                {
                    XanaConstants.xanaConstants.clothJson = File.ReadAllText(Application.persistentDataPath + "/loginAsGuestClass.json");
                }
                return (Application.persistentDataPath + "/loginAsGuestClass.json");
            }
        }
    }
    public void ComeFromWorld()
    {
       StartCoroutine(WaitForInstancefromWorld());
    }
    public IEnumerator HitReloadUnloadScene()
    {
        yield return new WaitForSeconds(.01f);
        SceneManager.UnloadSceneAsync("UserRegistration");
        print("Unload");
        SceneManager.LoadScene("UserRegistration", LoadSceneMode.Additive);
         yield return new WaitForSeconds(1f);
        print("wait");
        print("Loaded");
    }  
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
       // Application.targetFrameRate = 60;
        OnceGuestBool = false;
        OnceLoginBool = false;
    }
    IEnumerator WaitForInstancefromWorld()
    {
        yield return new WaitForSeconds(.05f);
        SaveCharacterProperties = ItemDatabase.instance.GetComponent<SavaCharacterProperties>();
         if (ItemDatabase.instance != null)
        ItemDatabase.instance.DownloadFromOtherWorld();
    }
    public void NotNowOfSignManager()
    {
      UiManager.LoginRegisterScreen.GetComponent<OnEnableDisable>().ClosePopUp();
       
        if (UiManager.HomePage.activeInHierarchy )
            UiManager.HomePage.SetActive(false);
        BGPlane.SetActive(true);

        if (WorldItemPreviewTab.m_WorldIsClicked || WorldItemPreviewTab.m_MuseumIsClicked || XanaConstants.loggedIn)
            UiManager.IsWorldClicked();

        else
        {
            if (!WorldBool && !BottomAvatarButtonBool)
                StoreManager.instance.SignUpAndLoginPanel(2);
            else
            {
                StoreManager.instance.SignUpAndLoginPanel(3);
            }
        }
    }
    public void AvatarMenuBtnPressed()
    {
        //UiManager.AvaterButtonCustomPushed();
        CharacterCustomizationUIManager.Instance.LoadMyClothCustomizationPanel();
        //Debug.Log("IsLoggedIn VALUEeeeeeeeee" + (PlayerPrefs.GetInt("IsLoggedIn")));
        if (XanaConstants.loggedIn) 
        {
            UiManager.HomePage.SetActive(false);
            StoreManager.instance.SignUpAndLoginPanel(3);
            BGPlane.SetActive(true);
        }
        //else  // Disable Guest Sceniro
        //{
        //    //UserRegisterationManager.instance.checkbool_preser_start = true;
        //    //PlayerPrefs.SetInt("IsChanged", 0);  
        //    //UserRegisterationManager.instance.OpenUIPanal(17);

        //    UserLoginSignupManager.instance.ShowWelcomeScreen();
        //}
        StoreManager.instance.AvatarUpdated.SetActive(false);
        StoreManager.instance.AvatarSaved.SetActive(false);
        StoreManager.instance.AvatarSavedGuest.SetActive(false);
    }
    public void BottomAvatarBtnPressed()
    {
        UiManager.AvaterButtonCustomPushed();
        CharacterCustomizationUIManager.Instance.LoadMyFaceCustomizationPanel();
        BottomAvatarButtonBool = true;
        if (XanaConstants.loggedIn || (PlayerPrefs.GetInt("IsLoggedIn") == 1))
        {
            UiManager.HomePage.SetActive(false);
            StoreManager.instance.SignUpAndLoginPanel(3);
            BGPlane.SetActive(true);
        }
        else
        {
            //PlayerPrefs.SetInt("IsChanged", 0);
            //UserRegisterationManager.instance.OpenUIPanal(1);

            UserLoginSignupManager.instance.ShowWelcomeScreen();
        }
        StoreManager.instance.AvatarSaved.SetActive(false);
        StoreManager.instance.AvatarSavedGuest.SetActive(false);
    }
    public void SignInSignUpCompleted()
    {
        if (WorldBool)
        {
            UiManager.HomePage.SetActive(true);
            BGPlane.SetActive(false);
        }
        else
        {
            UiManager.HomePage.SetActive(false);
            BGPlane.SetActive(true);
            StoreManager.instance.SignUpAndLoginPanel(3);
        }
    }
    public void BackFromStoreofCharacterCustom()
    {
        UiManager.HomePage.SetActive(true);
        BGPlane.SetActive(false);
    }
    public void ChangeCharacterAnimationState(bool l_State)
    {    
        m_CharacterAnimator.SetBool("Idle", l_State);
    }
    public void ResetCharacterAnimationController()
    {
        m_CharacterAnimator.runtimeAnimatorController = m_AnimControlller;
        mainCharacter.GetComponent<Animator>().runtimeAnimatorController = m_AnimControlller;
    }
    public void ReloadMainScene() 
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            SceneManager.LoadSceneAsync("Main");
        }
    }
    public void UpdatePlayerName(string newName)
    {
        mainCharacter.GetComponent<CharacterOnScreenNameHandler>().UpdateNameText(newName);
    }
}
