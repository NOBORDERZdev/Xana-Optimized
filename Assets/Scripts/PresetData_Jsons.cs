using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.UI.Extensions;
using WaheedDynamicScrollRect;

using static StoreManager;
public class PresetData_Jsons : MonoBehaviour
{

    public string JsonDataPreset;
    private string PresetNameinServer = "Presets";
    // Start is called before the first frame update
    public static string clickname;
    public bool IsStartUp_Canvas;   // if preset panel is appeared on start thn allow it to change 
    //bool isAddedInUndoRedo = false;
    bool presetAlreadySaved = false;
    //public static GameObject lastSelectedPreset=null;
    //public static string lastSelectedPresetName=null;
    [SerializeField] Texture eyeTex;
   // AvatarController avatarController;
   // CharcterBodyParts charcterBodyParts;

    private void OnEnable()
    {
        StartCoroutine(RegisterForUndoRedo());
    }

    IEnumerator RegisterForUndoRedo()
    {
        yield return new WaitForSeconds(0.05f);
        if (ActivePanelCallStack.obj.IsCallByBtn()) //&& this.transform.GetChild(0).gameObject.activeInHierarchy)
        {
            //isAddedInUndoRedo = true;
            //Debug.Log("<color=red> Enter In Presets </color>");
            if (!AR_UndoRedo.obj.addToList)
                AR_UndoRedo.obj.addToList = true;
            else
            {
                AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "ChangecharacterOnCLickFromserver", AR_UndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.Presets);
                //Debug.Log("<color=red> Set Default Preset</color>");
            }
        }
    }


    void Start()
    {
        if (gameObject.GetComponent<Button>() != null) 
        {
            gameObject.GetComponent<Button>().onClick.AddListener(ChangecharacterOnCLickFromserver);
        }


       // callScripts();
    }

    public void callScripts() 
    {
        //avatarController = GameManager.Instance.mainCharacter.GetComponent<AvatarController>();
        //charcterBodyParts = CharcterBodyParts.instance;
    }


    public void callit()
    {
        clickname = "";
    }
   public void ChangecharacterOnCLickFromserver()
    {
        //callScripts();
        if (StoreManager.instance.StartPanel_PresetParentPanel.activeInHierarchy)
        {
            //if (IsStartUp_Canvas && WaheedDynamicScrollRect.ScrollContent.instance != null)
            //{
               // JsonDataPreset = WaheedDynamicScrollRect.ScrollContent.instance.nameData;
                StoreManager.instance._CanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            //}
        }
        XanaConstants.xanaConstants.registerFirstTime = true;
        if (GameManager.Instance.isStoreAssetDownloading)
            return;

        if (!IsStartUp_Canvas)   //for presets in avatar panel 
        {
            if (clickname != gameObject.name)
                clickname = gameObject.name;
            else
                return;
        }
        //if (CharcterBodyParts.instance)
        //{
        //    charcterBodyParts.DefaultTexture(false);
        //}

        if (!IsStartUp_Canvas && !PremiumUsersDetails.Instance.CheckSpecificItem(PresetNameinServer))
        {
           // print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            //print("Horayyy you have Access");
            XanaConstants.xanaConstants.avatarStoreSelection[XanaConstants.xanaConstants.currentButtonIndex] = this.gameObject;
            XanaConstants.xanaConstants._curretClickedBtn = this.gameObject;

            if (!AR_UndoRedo.obj.addToList)
                AR_UndoRedo.obj.addToList = true;
            else
            {
                AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "ChangecharacterOnCLickFromserver", AR_UndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.Presets);
                Debug.Log("<color=red> Set Default Preset</color>");
            }

            if (XanaConstants.xanaConstants._lastClickedBtn && XanaConstants.xanaConstants._curretClickedBtn == XanaConstants.xanaConstants._lastClickedBtn
                && !IsStartUp_Canvas)
                return;

            GameManager.Instance.isStoreAssetDownloading = true;
            StoreManager.instance.UndoSelection();
           // if (!IsStartUp_Canvas)
                XanaConstants.xanaConstants._curretClickedBtn.transform.GetChild(0).gameObject.SetActive(true);
            if (XanaConstants.xanaConstants._lastClickedBtn && !IsStartUp_Canvas)
            {
                if (XanaConstants.xanaConstants._lastClickedBtn.GetComponent<PresetData_Jsons>())
                    XanaConstants.xanaConstants._lastClickedBtn.transform.GetChild(0).gameObject.SetActive(false);
            }
            
               
            XanaConstants.xanaConstants._lastClickedBtn = this.gameObject;
            XanaConstants.xanaConstants._lastClickedBtn = this.gameObject;
            XanaConstants.xanaConstants.PresetValueString = gameObject.name;
            PlayerPrefs.SetInt("presetPanel", 1);

            // Hack for latest update // keep all preset body fat to 0
            //change lipsto default
            SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();
            _CharacterData = JsonUtility.FromJson<SavingCharacterDataClass>(JsonDataPreset);  //(File.ReadAllText(GameManager.Instance.GetStringFolderPath()));

            //print(_CharacterData.BodyFat);
            _CharacterData.BodyFat = 0;
            _CharacterData.PresetValue = gameObject.name;

            XanaConstants.xanaConstants.bodyNumber = 0;
            File.WriteAllText((Application.persistentDataPath + "/SavingReoPreset.json"), JsonUtility.ToJson(_CharacterData));

            if (StoreManager.instance.StartPanel_PresetParentPanel.activeSelf)
            {
                Invoke("abcd", 5f);
                StoreManager.instance.StartPanel_PresetParentPanel.SetActive(false);
                UserRegisterationManager.instance.UsernameFieldAdvance.Clear();
                UserRegisterationManager.instance.usernamePanal.SetActive(true);
                if (PlayerPrefs.GetInt("iSignup") == 1)
                {
                   
                    // enable check so that it will know that index is comming from start of the game
                   // UserRegisterationManager.instance.checkbool_preser_start = false;
                    //UserRegisterationManager.instance.RegistrationCompletePanal.SetActive(true);
                    //UserRegisterationManager.instance.BlackScreen.SetActive(true);
                }
                else                // as a guest
                {
                    //Invoke("abcd", 5f);

                    //StoreManager.instance.StartPanel_PresetParentPanel.SetActive(false);
                    //UserRegisterationManager.instance.UsernameFieldAdvance.Clear();
                    //UserRegisterationManager.instance.usernamePanal.SetActive(true);
                    // enable check so that it will know that index is comming from start of the game
                    UserRegisterationManager.instance.checkbool_preser_start = false;
                }
            }
            else
            {
                if (this.gameObject.name != PlayerPrefs.GetString("PresetValue"))
                {
                    StoreManager.instance.SaveStoreBtn.GetComponent<Image>().color = new Color(0f, 0.5f, 1f, 0.8f);
                    StoreManager.instance.GreyRibbonImage.SetActive(false);
                    StoreManager.instance.WhiteRibbonImage.SetActive(true);
                }
               
                XanaConstants.xanaConstants._lastClickedBtn = this.gameObject;
            }
            //if (avatarController.wornEyewearable != null)
            //{
            //    avatarController.UnStichItem("EyeWearable");
            //}

            if (_CharacterData.HairColor != null)
                XanaConstants.xanaConstants.isPresetHairColor = true;

                //avatarController.ApplyPreset(_CharacterData);

            GetSavedPreset();
            if (!presetAlreadySaved)
            {
                StoreManager.instance.SaveStoreBtn.GetComponent<Button>().interactable = true;
                SavedButtonClickedBlue();
            }

            else
            {
                StoreManager.instance.SaveStoreBtn.SetActive(true);
                StoreManager.instance.SaveStoreBtn.GetComponent<Button>().interactable = false;
                StoreManager.instance.SaveStoreBtn.GetComponent<Image>().color = Color.white;
                StoreManager.instance.GreyRibbonImage.SetActive(true);
                StoreManager.instance.WhiteRibbonImage.SetActive(false);
            }
        }
    }
    void SavedButtonClickedBlue()
    {
        StoreManager.instance.SaveStoreBtn.SetActive(true);
        StoreManager.instance.SaveStoreBtn.GetComponent<Image>().color = new Color(0f, 0.5f, 1f, 0.8f);
        StoreManager.instance.GreyRibbonImage.SetActive(false);
        StoreManager.instance.WhiteRibbonImage.SetActive(true);
    }
    public void GetSavedPreset()
    {
        if (PlayerPrefs.GetInt("IsLoggedIn") == 1)  // logged in from account
        {
            if (File.Exists(Application.persistentDataPath + "/logIn.json") && File.ReadAllText(Application.persistentDataPath + "/logIn.json") != "")
            {
                SavingCharacterDataClass _CharacterData1 = new SavingCharacterDataClass();

                _CharacterData1 = _CharacterData1.CreateFromJSON(File.ReadAllText(Application.persistentDataPath + "/logIn.json"));
                if (this.gameObject.name == _CharacterData1.PresetValue)
                    presetAlreadySaved = true;
            }
        }
        else // Guest User
        {
            if (File.Exists(Application.persistentDataPath + "/loginAsGuestClass.json") && File.ReadAllText(Application.persistentDataPath + "/loginAsGuestClass.json") != "")
            {
                SavingCharacterDataClass _CharacterData1 = new SavingCharacterDataClass();

                _CharacterData1 = _CharacterData1.CreateFromJSON(File.ReadAllText(Application.persistentDataPath + "/loginAsGuestClass.json"));
                if (this.gameObject.name == _CharacterData1.PresetValue)
                    presetAlreadySaved = true;
            }
        }
    }
  public  void abcd()
    {
        //Debug.LogError("abcd Called");
        //UserRegisterationManager.instance.SignUpCompletedPresetApplied();
        //Debug.LogError("Coroutin Called " + PlayerPrefs.GetInt("presetPanel"));  
        if (PlayerPrefs.GetInt("presetPanel") == 1)   // preset panel is enable so saving preset to account 
            PlayerPrefs.SetInt("presetPanel", 0);
        ItemDatabase.instance.GetComponent<SavaCharacterProperties>().SavePlayerProperties();
        //avatarController.IntializeAvatar();
       // Debug.LogError("IntializeAvatar after");
    }  
}
