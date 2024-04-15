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
    //AvatarController avatarController;
    //CharacterBodyParts charcterBodyParts;

    public AvatarGender avatarGender;

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
            gameObject.GetComponent<Button>().onClick.AddListener(ChangecharacterFromPresetPanel);
        }


        GetScriptRef();
    }

    public void GetScriptRef()
    {
        //avatarController = GameManager.Instance.mainCharacter.GetComponent<AvatarController>();
        //charcterBodyParts = GameManager.Instance.mainCharacter.GetComponent<CharacterBodyParts>();
    }


    public void callit()
    {
        clickname = "";
    }
    public void ChangecharacterFromPresetPanel()
    {
        GetScriptRef();
        XanaConstants.xanaConstants.registerFirstTime = true;

        if (!IsStartUp_Canvas)   //for presets in avatar panel 
        {
            if (clickname != gameObject.name)
                clickname = gameObject.name;
            else
                return;
        }
        GameManager.Instance.characterBodyParts.DefaultTexture(false);

        if (!IsStartUp_Canvas && !PremiumUsersDetails.Instance.CheckSpecificItem(PresetNameinServer))
        {
            Debug.Log("Please Upgrade to Premium account");
            return;
        }
        else
        {
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
            {
                Debug.Log("Same Button Clicked");
                return;
            }

            GameManager.Instance.isStoreAssetDownloading = true;
            StoreManager.instance.UndoSelection();
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
            _CharacterData.BodyFat = 0;
            _CharacterData.PresetValue = gameObject.name;
            SavaCharacterProperties.instance.SaveItemList.gender = _CharacterData.gender;
            XanaConstants.xanaConstants.bodyNumber = 0;
            if (UGCManager.isSelfieTaken)
            {
                SaveUGCDataOnJson(_CharacterData);
            }
            else
            {
                _CharacterData.charactertypeAi = false;
                StoreManager.instance.itemData.CharactertypeAi = false;
                UGCManager.isSelfieTaken = false;
            }

            File.WriteAllText((Application.persistentDataPath + "/loginAsGuestClass.json"), JsonUtility.ToJson(_CharacterData));
            File.WriteAllText((Application.persistentDataPath + "/logIn.json"), JsonUtility.ToJson(_CharacterData));

            //Store selected preset data when signup
            GameManager.Instance.selectedPresetData = JsonUtility.ToJson(_CharacterData);

            CharacterHandler.instance.ActivateAvatarByGender(_CharacterData.gender);
            //GameManager.Instance.mainCharacter.GetComponent<CharacterBodyParts>().SetAvatarByGender(_CharacterData.gender);


            if (StoreManager.instance.StartPanel_PresetParentPanel.activeSelf || StoreManager.instance.selfiePanel.activeSelf)
            {
                /*Invoke("abcd", 5f);*/
                StoreManager.instance.StartPanel_PresetParentPanel.SetActive(false);
                StoreManager.instance.selfiePanel.SetActive(false);
                if (!GameManager.Instance.UiManager.isAvatarSelectionBtnClicked)
                {
                    UserLoginSignupManager.instance.OpenUserNamePanel();
                }
                else
                {
                    GameManager.Instance.UiManager.isAvatarSelectionBtnClicked = false;
                    GameManager.Instance.m_RenderTextureCamera.gameObject.SetActive(false);
                    GameManager.Instance.ActorManager.IdlePlayerAvatorForMenu(false);
                }
                if (UGCManager.isSelfieTaken)
                {
                    //UserRegisterationManager.instance.renderImage.gameObject.SetActive(true);
                    UserLoginSignupManager.instance.selectedPresetImage.gameObject.SetActive(false);
                    UserLoginSignupManager.instance.aiPresetImage.gameObject.SetActive(true);
                }
                else
                {
                    //UserRegisterationManager.instance.renderImage.gameObject.SetActive(false);
                    UserLoginSignupManager.instance.selectedPresetImage.gameObject.SetActive(true);
                    UserLoginSignupManager.instance.aiPresetImage.gameObject.SetActive(false);
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
            if (GameManager.Instance.avatarController.wornEyeWearable != null)
            {
                GameManager.Instance.avatarController.UnStichItem("EyeWearable");
            }

            if (_CharacterData.HairColor != null)
                XanaConstants.xanaConstants.isPresetHairColor = true;
            GetSavedPreset();
            SavePresetOnServer(_CharacterData);
            ApplyPreset();

            if (UGCManager.isSelfieTaken)
            {
                StoreManager.instance.ApplyUGCValueOnCharacter(_CharacterData.gender);
                UGCManager.isSelfieTaken = false;
            }
            else
            {
                StoreManager.instance.ApplyDefaultValueOnCharacter(_CharacterData.gender);
            }
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

            //if (UGCManager.isSelfieTaken)
            //{
            //    StoreManager.instance.ApplyUGCValueOnCharacter();
            //}
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
    public void ApplyPreset()
    {
        //UserRegisterationManager.instance.SignUpCompletedPresetApplied();
        if (PlayerPrefs.GetInt("presetPanel") == 1)   // preset panel is enable so saving preset to account 
            PlayerPrefs.SetInt("presetPanel", 0);
        GameManager.Instance.avatarController.InitializeAvatar();
    }

    void SavePresetOnServer(SavingCharacterDataClass savingCharacterDataClass)
    {
        if (PlayerPrefs.GetInt("IsLoggedIn") == 1)
        {
            File.WriteAllText((Application.persistentDataPath + "/logIn.json"), JsonUtility.ToJson(savingCharacterDataClass));
            ServerSIdeCharacterHandling.Instance.CreateUserOccupiedAsset(() =>
            {
            });
        }

    }
    void SaveUGCDataOnJson(SavingCharacterDataClass _CharacterData)
    {
        _CharacterData.ai_gender = StoreManager.instance.itemData.gender;
        _CharacterData.charactertypeAi = StoreManager.instance.itemData.CharactertypeAi;
        _CharacterData.hair_color = StoreManager.instance.itemData.hair_color;
        _CharacterData.skin_color = StoreManager.instance.itemData.skin_color;
        _CharacterData.lip_color = StoreManager.instance.itemData.lips_color;
        _CharacterData.faceItemData = StoreManager.instance.itemData.faceItemData;
        _CharacterData.noseItemData = StoreManager.instance.itemData.noseItemData;
        _CharacterData.lipItemData = StoreManager.instance.itemData.lipItemData;
        _CharacterData.hairItemData = StoreManager.instance.itemData._hairItemData;
        _CharacterData.eyeItemData = StoreManager.instance.itemData._eyeItemData;
        _CharacterData.eyeShapeItemData = StoreManager.instance.itemData.eyeShapeItemData;
    }
}




public enum AvatarGender
{
    Male, Female
}
