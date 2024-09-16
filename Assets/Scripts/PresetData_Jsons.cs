using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.UI.Extensions;
using WaheedDynamicScrollRect;

using static InventoryManager;
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
        if (StoreStackHandler.obj.IsCallByBtn()) //&& this.transform.GetChild(0).gameObject.activeInHierarchy)
        {
            //isAddedInUndoRedo = true;
            //Debug.Log("<color=red> Enter In Presets </color>");
            if (!StoreUndoRedo.obj.addToList)
                StoreUndoRedo.obj.addToList = true;
            else
            {
                StoreUndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "ChangecharacterOnCLickFromserver", StoreUndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.Presets);
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
        ConstantsHolder.xanaConstants.registerFirstTime = true;

        if (!IsStartUp_Canvas)   //for presets in avatar panel 
        {
            if (clickname != gameObject.name)
                clickname = gameObject.name;
            else
                return;
        }
        //else 
        //{ 
        //ConstantsHolder.xanaConstants.isFirstPanel = true;
        //}
        GameManager.Instance.characterBodyParts.DefaultTexture(false);

        if (!IsStartUp_Canvas && !UserPassManager.Instance.CheckSpecificItem(PresetNameinServer))
        {
            Debug.Log("Please Upgrade to Premium account");
            return;
        }
        else
        {
            ConstantsHolder.xanaConstants.avatarStoreSelection[ConstantsHolder.xanaConstants.currentButtonIndex] = this.gameObject;
            ConstantsHolder.xanaConstants._curretClickedBtn = this.gameObject;

            if (!StoreUndoRedo.obj.addToList)
                StoreUndoRedo.obj.addToList = true;
            else
            {
                StoreUndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "ChangecharacterOnCLickFromserver", StoreUndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.Presets);
                Debug.Log("<color=red> Set Default Preset</color>");
            }

            if (ConstantsHolder.xanaConstants._lastClickedBtn && ConstantsHolder.xanaConstants._curretClickedBtn == ConstantsHolder.xanaConstants._lastClickedBtn
                && !IsStartUp_Canvas)
            {
                Debug.Log("Same Button Clicked");
                return;
            }

            GameManager.Instance.isStoreAssetDownloading = true;
            InventoryManager.instance.UndoSelection();
            ConstantsHolder.xanaConstants._curretClickedBtn.transform.GetChild(0).gameObject.SetActive(true);
            if (ConstantsHolder.xanaConstants._lastClickedBtn && !IsStartUp_Canvas)
            {
                if (ConstantsHolder.xanaConstants._lastClickedBtn.GetComponent<PresetData_Jsons>())
                    ConstantsHolder.xanaConstants._lastClickedBtn.transform.GetChild(0).gameObject.SetActive(false);
            }

            ConstantsHolder.xanaConstants._lastClickedBtn = this.gameObject;
            ConstantsHolder.xanaConstants._lastClickedBtn = this.gameObject;
            ConstantsHolder.xanaConstants.PresetValueString = gameObject.name;
            PlayerPrefs.SetInt("presetPanel", 1);

            // Hack for latest update // keep all preset body fat to 0
            //change lipsto default

            SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();
            _CharacterData = JsonUtility.FromJson<SavingCharacterDataClass>(JsonDataPreset);  //(File.ReadAllText(GameManager.Instance.GetStringFolderPath()));        
            _CharacterData.BodyFat = 0;
            _CharacterData.PresetValue = gameObject.name;
            SaveCharacterProperties.instance.SaveItemList.gender = _CharacterData.gender;
            ConstantsHolder.xanaConstants.bodyNumber = 0;
            if (UGCManager.isSelfieTaken)
            {
                SaveUGCDataOnJson(_CharacterData);
            }
            else
            {
                _CharacterData.charactertypeAi = false;
                InventoryManager.instance.itemData.CharactertypeAi = false;
                UGCManager.isSelfieTaken = false;
            }

            // Set the position, rotation of the character 
            {
                string oldSelectedGender = CharacterHandler.instance.activePlayerGender.ToString();//== AvatarGender.Female ? "1" : "0";

                // Check Old and new Selected are not same
                if (oldSelectedGender != _CharacterData.gender) // 
                {
                    // Copy old avatar pos, rotation and implement to new avatar 
                    if(oldSelectedGender == "1")
                    {
                        // Old is Female
                        CharacterHandler.instance.maleAvatarData.avatar_parent.transform.localPosition = CharacterHandler.instance.femaleAvatarData.avatar_parent.transform.localPosition;
                        CharacterHandler.instance.maleAvatarData.avatar_parent.transform.localRotation = CharacterHandler.instance.femaleAvatarData.avatar_parent.transform.localRotation;
                    }
                    else
                    {
                        CharacterHandler.instance.femaleAvatarData.avatar_parent.transform.localPosition = CharacterHandler.instance.maleAvatarData.avatar_parent.transform.localPosition;
                        CharacterHandler.instance.femaleAvatarData.avatar_parent.transform.localRotation = CharacterHandler.instance.maleAvatarData.avatar_parent.transform.localRotation;
                    }
                    if(ConstantsHolder.xanaConstants.isStoreActive)
                        InventoryManager.instance.DeletePreviousItems();
                }
            }

            //Store selected preset data when signup
            GameManager.Instance.selectedPresetData = JsonUtility.ToJson(_CharacterData);

            //CharacterHandler.instance.ActivateAvatarByGender(_CharacterData.gender);
            //GameManager.Instance.mainCharacter.GetComponent<CharacterBodyParts>().SetAvatarByGender(_CharacterData.gender);


            if (InventoryManager.instance.StartPanel_PresetParentPanel.activeSelf || InventoryManager.instance.selfiePanel.activeSelf|| 
                InventoryManager.instance.StartPanel_PresetParentPanelSummit.activeSelf)
            {
                /*Invoke("abcd", 5f);*/
                InventoryManager.instance.StartPanel_PresetParentPanel.SetActive(false);
                InventoryManager.instance.StartPanel_PresetParentPanelSummit.SetActive(false);
                InventoryManager.instance.selfiePanel.SetActive(false);
                if (!GameManager.Instance.UiManager.isAvatarSelectionBtnClicked)
                {
                    UserLoginSignupManager.instance.OpenUserNamePanel();
                    if (UGCManager.isSelfieTaken) 
                    {
                        GameManager.Instance.ActorManager.IdlePlayerAvatorForMenu(true);
                        AvatarCustomizationManager.Instance.ResetCharacterRotation(180f);
                    }
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
                    UserLoginSignupManager.instance.SelectedPresetImage.gameObject.SetActive(false);
                    UserLoginSignupManager.instance.AiPresetImage.gameObject.SetActive(true);
                    UserLoginSignupManager.instance.SelectPresetImageforEditProfil.gameObject.SetActive(false);
                    UserLoginSignupManager.instance.AiPresetImageforEditProfil.gameObject.SetActive(true);
                }
                else
                {
                    //UserRegisterationManager.instance.renderImage.gameObject.SetActive(false);
                    UserLoginSignupManager.instance.SelectedPresetImage.gameObject.SetActive(true);
                    UserLoginSignupManager.instance.AiPresetImage.gameObject.SetActive(false);
                    UserLoginSignupManager.instance.SelectPresetImageforEditProfil.gameObject.SetActive(true);
                    UserLoginSignupManager.instance.AiPresetImageforEditProfil.gameObject.SetActive(false);
                }
            }
            else
            {
                if (this.gameObject.name != PlayerPrefs.GetString("PresetValue"))
                {
                    InventoryManager.instance.SaveStoreBtn.GetComponent<Image>().color = new Color(0f, 0.5f, 1f, 0.8f);
                    InventoryManager.instance.GreyRibbonImage.SetActive(false);
                    InventoryManager.instance.WhiteRibbonImage.SetActive(true);
                }

                ConstantsHolder.xanaConstants._lastClickedBtn = this.gameObject;
                InventoryManager.upateAssetOnGenderChanged?.Invoke();
            }
            if (GameManager.Instance.avatarController.wornEyeWearable != null)
            {
                GameManager.Instance.avatarController.UnStichItem("EyeWearable");
            }

            if (_CharacterData.HairColor != null)
                ConstantsHolder.xanaConstants.isPresetHairColor = true;
           
            ApplyPreset(_CharacterData);
            if (IsStartUp_Canvas)
            {
                File.WriteAllText((Application.persistentDataPath + "/loginAsGuestClass.json"), JsonUtility.ToJson(_CharacterData));
                File.WriteAllText((Application.persistentDataPath + "/logIn.json"), JsonUtility.ToJson(_CharacterData));
                GetSavedPreset();
                SavePresetOnServer(_CharacterData);
            }

            if (UGCManager.isSelfieTaken)
            {
                InventoryManager.instance.ApplyUGCValueOnCharacter(_CharacterData.gender);
                UGCManager.isSelfieTaken = false;
            }
            else
            {
                InventoryManager.instance.ApplyDefaultValueOnCharacter(_CharacterData.gender);
            }
           
            if (!presetAlreadySaved)
            {
                InventoryManager.instance.SaveStoreBtn.GetComponent<Button>().interactable = true;
                SavedButtonClickedBlue();
            }
            else
            {
                InventoryManager.instance.SaveStoreBtn.SetActive(true);
                InventoryManager.instance.SaveStoreBtn.GetComponent<Button>().interactable = false;
                InventoryManager.instance.SaveStoreBtn.GetComponent<Image>().color = Color.white;
                InventoryManager.instance.GreyRibbonImage.SetActive(true);
                InventoryManager.instance.WhiteRibbonImage.SetActive(false);
            }

            //if (UGCManager.isSelfieTaken)
            //{
            //    InventoryManager.instance.ApplyUGCValueOnCharacter();
            //}
        }

    }
    void SavedButtonClickedBlue()
    {
        InventoryManager.instance.SaveStoreBtn.SetActive(true);
        InventoryManager.instance.SaveStoreBtn.GetComponent<Image>().color = new Color(0f, 0.5f, 1f, 0.8f);
        InventoryManager.instance.GreyRibbonImage.SetActive(false);
        InventoryManager.instance.WhiteRibbonImage.SetActive(true);
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
    public void ApplyPreset(SavingCharacterDataClass _data)
    {
        //UserRegisterationManager.instance.SignUpCompletedPresetApplied();
        if (PlayerPrefs.GetInt("presetPanel") == 1)   // preset panel is enable so saving preset to account 
            PlayerPrefs.SetInt("presetPanel", 0);
        GameManager.Instance.avatarController.InitializeAvatar(false,_data);
    }

    void SavePresetOnServer(SavingCharacterDataClass savingCharacterDataClass)
    {
        if (PlayerPrefs.GetInt("IsLoggedIn") == 1)
        {
            File.WriteAllText((Application.persistentDataPath + "/logIn.json"), JsonUtility.ToJson(savingCharacterDataClass));
            ServerSideUserDataHandler.Instance.CreateUserOccupiedAsset(() =>
            {
            });
        }

    }
    void SaveUGCDataOnJson(SavingCharacterDataClass _CharacterData)
    {
        _CharacterData.ai_gender = InventoryManager.instance.itemData.gender;
        _CharacterData.charactertypeAi = InventoryManager.instance.itemData.CharactertypeAi;
        _CharacterData.hair_color = InventoryManager.instance.itemData.hair_color;
        _CharacterData.skin_color = InventoryManager.instance.itemData.skin_color;
        _CharacterData.lip_color = InventoryManager.instance.itemData.lips_color;
        _CharacterData.faceItemData = InventoryManager.instance.itemData.faceItemData;
        _CharacterData.noseItemData = InventoryManager.instance.itemData.noseItemData;
        _CharacterData.lipItemData = InventoryManager.instance.itemData.lipItemData;
        _CharacterData.hairItemData = InventoryManager.instance.itemData._hairItemData;
        _CharacterData.eyeItemData = InventoryManager.instance.itemData._eyeItemData;
        _CharacterData.eyeShapeItemData = InventoryManager.instance.itemData.eyeShapeItemData;
    }
}




public enum AvatarGender
{
    Male, Female
}
