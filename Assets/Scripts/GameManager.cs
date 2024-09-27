using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json;
using System;
using UnityEngine.Networking;
using System.Text;

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
    public BlendShapeImporter BlendShapeObj;
    public bool UserStatus_;   //if its true user is logged in else its as a guest
    public static string currentLanguage = "";
    public bool isStoreAssetDownloading = false;
    public Transform PostManager;
    public AvatarPathSystemManager avatarPathSystemManager;
    public ActorManager ActorManager;
    public MoodManager moodManager;
    public UserAnimationPostFeature userAnimationPostFeature;
    public Transform FriendsHomeManager;
    [Space(5)]
    public ClassAPIResponse rootObject;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        PlayerPrefs.SetInt("presetPanel", 0);  // was loggedin as account 
        StartCoroutine(GetClassCodeFromServer());
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
        Application.targetFrameRate = 60;
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
      UIManager.Instance.LoginRegisterScreen.GetComponent<OnEnableDisable>().ClosePopUp();

        if (XanaConstants.xanaConstants.EnviornmentName == "PMY ACADEMY" && !XanaConstants.xanaConstants.pmy_isTesting)
        {
            if (XanaConstants.xanaConstants.buttonClicked != null && !XanaConstants.xanaConstants.buttonClicked.GetComponent<WorldItemView>().worldItemPreview.enterClassCodePanel.activeInHierarchy)
            {
                XanaConstants.xanaConstants.buttonClicked.GetComponent<WorldItemView>().worldItemPreview.classCodeInputField.Text = "";
                XanaConstants.xanaConstants.buttonClicked.GetComponent<WorldItemView>().worldItemPreview.enterClassCodePanel.SetActive(true);
                return;
            }
        }

        if (UIManager.Instance.HomePage.activeInHierarchy )
            UIManager.Instance.HomePage.SetActive(false);
        BGPlane.SetActive(true);
        if (WorldItemPreviewTab.m_WorldIsClicked || WorldItemPreviewTab.m_MuseumIsClicked || UserRegisterationManager.instance.LoggedIn)
            UIManager.Instance.IsWorldClicked();
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
        UIManager.Instance.AvaterButtonCustomPushed();
        CharacterCustomizationUIManager.Instance.LoadMyClothCustomizationPanel();
        Debug.Log("IsLoggedIn VALUEeeeeeeeee" + (PlayerPrefs.GetInt("IsLoggedIn")));
        if (UserRegisterationManager.instance.LoggedIn ||  (PlayerPrefs.GetInt("IsLoggedIn") ==  1)) 
        {
            UIManager.Instance.HomePage.SetActive(false);
            StoreManager.instance.SignUpAndLoginPanel(3);
            BGPlane.SetActive(true);
        }
        else
        {
            UserRegisterationManager.instance.checkbool_preser_start = true;
             PlayerPrefs.SetInt("IsChanged", 0);  
            UserRegisterationManager.instance.OpenUIPanal(17);
        }
        StoreManager.instance.AvatarUpdated.SetActive(false);
        StoreManager.instance.AvatarSaved.SetActive(false);
        StoreManager.instance.AvatarSavedGuest.SetActive(false);
    }
    public void BottomAvatarBtnPressed()
    {
        UIManager.Instance.AvaterButtonCustomPushed();
        CharacterCustomizationUIManager.Instance.LoadMyFaceCustomizationPanel();
        BottomAvatarButtonBool = true;
        if (UserRegisterationManager.instance.LoggedIn || (PlayerPrefs.GetInt("IsLoggedIn") == 1))
        {
            UIManager.Instance.HomePage.SetActive(false);
            StoreManager.instance.SignUpAndLoginPanel(3);
            BGPlane.SetActive(true);
        }
        else
        {
            PlayerPrefs.SetInt("IsChanged", 0);
            UserRegisterationManager.instance.OpenUIPanal(1);
        }
        StoreManager.instance.AvatarSaved.SetActive(false);
        StoreManager.instance.AvatarSavedGuest.SetActive(false);
    }
    public void SignInSignUpCompleted()
    {
        if (WorldBool)
        {
            UIManager.Instance.HomePage.SetActive(true);
            BGPlane.SetActive(false);
        }
        else
        {
            UIManager.Instance.HomePage.SetActive(false);
            BGPlane.SetActive(true);
            StoreManager.instance.SignUpAndLoginPanel(3);
        }
    }
    public void BackFromStoreofCharacterCustom()
    {
        UIManager.Instance.HomePage.SetActive(true);
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

    IEnumerator GetClassCodeFromServer()
    {
        //string api = "https://api-test.xana.net/classCode/get-all-class-codes/1/20";

        yield return new WaitForSeconds(10f);
        string token = ConstantsGod.AUTH_TOKEN;

        StringBuilder api = new StringBuilder();
        api.Append(ConstantsGod.API_BASEURL + "/classCode/get-all-class-codes" + "/" +/* Page Size */ 1 + "/" +/* Record Size  */  350);
        //string api = "https://api-test.xana.net/classCode/get-all-class-codes" + "/" +/* Page Size */ 1 + "/" +/* Record Size  */  50;
       
        Debug.Log("<color=red> ClassCode -- API : " + api + "</color>");

        UnityWebRequest www;
        www = UnityWebRequest.Get(api.ToString());


        www.SetRequestHeader("Authorization", token);
        www.SendWebRequest();

        while (!www.isDone)
        {
            yield return null;
        }


        if (!www.isHttpError && !www.isNetworkError)
        {
            Debug.Log("<color=green> ClassCode -- OldMessages : " + www.downloadHandler.text + "</color>");
            string jsonString = www.downloadHandler.text;
            //ClassAPIResponse response = JsonUtility.FromJson<ClassAPIResponse>(www.downloadHandler.text);
            rootObject = new ClassAPIResponse();
            rootObject = JsonConvert.DeserializeObject<ClassAPIResponse>(jsonString);


            if (rootObject.success)
                CheckResponse(rootObject.data.rows);
        }
        else
            Debug.Log("<color=red> ClassCode -- NetWorkissue </color>");

        www.Dispose();
    }


    private void CheckResponse(List<ClassCode> response)
    {
        // Clear Old Data
        // Stop Duplicate Data
        XanaConstants.xanaConstants.pmy_ClassCode.Clear();
        XanaConstants.xanaConstants.pmy_ClassCode = new List<PMYAvailableClassCode>(response.Count);

        // Add New Data
        for (int i = 0; i < response.Count; i++)
        {
            XanaConstants.xanaConstants.pmy_ClassCode.Add(new PMYAvailableClassCode());
            XanaConstants.xanaConstants.pmy_ClassCode[i].id = (response[i].id);
            XanaConstants.xanaConstants.pmy_ClassCode[i].codeText = (response[i].codeText);
        }
    }
}

[Serializable]
public class ClassAPIResponse
{
    public bool success;
    public DataContain data;
    //public string msg;
}
[Serializable]
public class DataContain
{
    public int count;
    public List<ClassCode> rows;
}
[Serializable]
public class ClassCode
{
    public int id;
    public string loginId;
    public string subject;
    public string codeText;
    public bool isActive;
    public DateTime createdAt;
    public DateTime updatedAt;
}

[System.Serializable]
public class PMYAvailableClassCode
{
    public int id;
    public string codeText;
}

