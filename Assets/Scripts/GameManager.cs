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
    //public CharacterHandler maleAvatar;
    //public CharacterHandler femaleAvatar;
    public AvatarController avatarController;
    public CharacterBodyParts characterBodyParts;
    //public AvatarGender avatarGender;

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
    public bool isTabSwitched = false;
    [Header("Camera Work")]
    public GameObject faceMorphCam;
    public GameObject headCam;
    public GameObject bodyCam;
    public GameObject RequiredNFTPopUP;
    public GameObject ShadowPlane;
    public SaveCharacterProperties SaveCharacterProperties;
    public EquipUI EquipUiObj;
    public BlendShapeManager BlendShapeManager;
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
    public AdditiveScenesLoader additiveScenesManager;
    public HomeCameraController HomeCamera;
    public UIHandler UiManager;
    public HomeFooterHandler bottomTabManagerInstance;
    public WorldManager SpaceWorldManagerRef;
    internal string selectedPresetData="";
    private void Awake()
    {
        Debug.Log("GameManager Awake");
        if (Instance == null)
            Instance = this;
        PlayerPrefs.SetInt("presetPanel", 0);  // was loggedin as account 
        if (additiveScenesManager == null) // If Null then find object
        {
           additiveScenesManager = FindObjectOfType<AdditiveScenesLoader>();
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

            if (PlayerPrefs.HasKey("Equiped") || ConstantsHolder.xanaConstants.isNFTEquiped)
            {
                if (File.Exists(Application.persistentDataPath + ConstantsHolder.xanaConstants.NFTBoxerJson))
                {
                    ConstantsHolder.xanaConstants.clothJson = File.ReadAllText(Application.persistentDataPath + ConstantsHolder.xanaConstants.NFTBoxerJson);
                }
                return (Application.persistentDataPath + ConstantsHolder.xanaConstants.NFTBoxerJson);
            }
            else if (PlayerPrefs.GetInt("presetPanel") == 1)  // presetpanel enabled account)
            {
                if (File.Exists(Application.persistentDataPath + "/SavingReoPreset.json"))
                {
                    ConstantsHolder.xanaConstants.clothJson = File.ReadAllText(Application.persistentDataPath + "/SavingReoPreset.json");
                }
                return (Application.persistentDataPath + "/SavingReoPreset.json");
            }
            else
            {
                UserStatus_ = true;
                if (File.Exists(Application.persistentDataPath + "/logIn.json"))
                {
                    ConstantsHolder.xanaConstants.clothJson = File.ReadAllText(Application.persistentDataPath + "/logIn.json");
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
                    ConstantsHolder.xanaConstants.clothJson = File.ReadAllText(Application.persistentDataPath + "/SavingReoPreset.json");
                }
                return (Application.persistentDataPath + "/SavingReoPreset.json");
            }
            else
            {
                UserStatus_ = false;
                if (File.Exists(Application.persistentDataPath + "/loginAsGuestClass.json"))
                {
                    ConstantsHolder.xanaConstants.clothJson = File.ReadAllText(Application.persistentDataPath + "/loginAsGuestClass.json");
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
        SceneManager.UnloadSceneAsync("LoginSignupScene");
        print("Unload");
        SceneManager.LoadScene("LoginSignupScene", LoadSceneMode.Additive);
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
        SaveCharacterProperties = DefaultClothDatabase.instance.GetComponent<SaveCharacterProperties>();
         if (DefaultClothDatabase.instance != null)
        DefaultClothDatabase.instance.DownloadFromOtherWorld();
    }
    public void NotNowOfSignManager()
    {
      UiManager.LoginRegisterScreen.GetComponent<OnEnableDisable>().ClosePopUp();
       
        if (UiManager.HomePage.activeInHierarchy )
            UiManager.HomePage.SetActive(false);
        BGPlane.SetActive(true);

        if (WorldDescriptionPopupPreview.m_WorldIsClicked || WorldDescriptionPopupPreview.m_MuseumIsClicked || ConstantsHolder.loggedIn)
            UiManager.IsWorldClicked();

        else
        {
            if (!WorldBool && !BottomAvatarButtonBool)
                InventoryManager.instance.SignUpAndLoginPanel(2);
            else
            {
                InventoryManager.instance.SignUpAndLoginPanel(3);
            }
        }
    }
    public void AvatarMenuBtnPressed()
    {
        UiManager.AvaterButtonCustomPushed();
        AvatarCustomizationUIHandler.Instance.LoadMyClothCustomizationPanel();
        //Debug.Log("IsLoggedIn VALUEeeeeeeeee" + (PlayerPrefs.GetInt("IsLoggedIn")));
        if (ConstantsHolder.loggedIn) 
        {
            UiManager.HomePage.SetActive(false);
            InventoryManager.instance.SignUpAndLoginPanel(3);
            BGPlane.SetActive(true);
        }
        //else  // Disable Guest Sceniro
        //{
        //    //UserRegisterationManager.instance.checkbool_preser_start = true;
        //    //PlayerPrefs.SetInt("IsChanged", 0);  
        //    //UserRegisterationManager.instance.OpenUIPanal(17);

        //UserLoginSignupManager.instance.ShowWelcomeScreen();
        InventoryManager.instance.AvatarUpdated.SetActive(false);
        InventoryManager.instance.AvatarSaved.SetActive(false);
        InventoryManager.instance.AvatarSavedGuest.SetActive(false);
    }
    public void BottomAvatarBtnPressed()
    {
        UiManager.AvaterButtonCustomPushed();
        AvatarCustomizationUIHandler.Instance.LoadMyFaceCustomizationPanel();
        BottomAvatarButtonBool = true;
        if (ConstantsHolder.loggedIn || (PlayerPrefs.GetInt("IsLoggedIn") == 1))
        {
            UiManager.HomePage.SetActive(false);
            InventoryManager.instance.SignUpAndLoginPanel(3);
            BGPlane.SetActive(true);
        }
        else
        {
            //PlayerPrefs.SetInt("IsChanged", 0);
            //UserRegisterationManager.instance.OpenUIPanal(1);

            UserLoginSignupManager.instance.ShowWelcomeScreen();
        }
        InventoryManager.instance.AvatarSaved.SetActive(false);
        InventoryManager.instance.AvatarSavedGuest.SetActive(false);
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
            InventoryManager.instance.SignUpAndLoginPanel(3);
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
            SceneManager.LoadSceneAsync("Home");
        }
    }
    public void UpdatePlayerName(string newName)
    {
        mainCharacter.GetComponent<CharacterOnScreenNameHandler>().UpdateNameText(newName);
    }

    //public void ActivateAvatarByGender(string gender)
    //{
    //    switch (gender)
    //    {
    //        case "Male":
    //            maleAvatar.gameObject.SetActive(true);
    //            femaleAvatar.gameObject.SetActive(false);
    //            maleAvatar.UpdateAvatarRefrences();
    //            break;
    //        case "Female":
    //            maleAvatar.gameObject.SetActive(false);
    //            femaleAvatar.gameObject.SetActive(true);
    //            femaleAvatar.UpdateAvatarRefrences();
    //            break;
    //    }
    //}
}
