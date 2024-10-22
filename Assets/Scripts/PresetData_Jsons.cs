using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PresetData_Jsons : MonoBehaviour
{
    public int avatarIndex = 0;
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
    AvatarController avatarController;
    CharcterBodyParts charcterBodyParts;

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
                AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "ChangecharacterOnCLickFromserver", AR_UndoRedo.ActionType.ChangeItem, Color.white, StoreManager.EnumClass.CategoryEnum.Presets);
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


        callScripts();
    }

    public void callScripts()
    {
        avatarController = GameManager.Instance.mainCharacter.GetComponent<AvatarController>();
        charcterBodyParts = CharcterBodyParts.instance;
    }


    public void callit()
    {
        clickname = "";
    }
    public void ChangecharacterOnCLickFromserver()
    {
        XanaConstants.xanaConstants.selectedAvatarNum = avatarIndex;
        callScripts();
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
        {
            return;
        }
        if (!IsStartUp_Canvas)   //for presets in avatar panel 
        {
            if (clickname != gameObject.name)
                clickname = gameObject.name;
            else
                return;
        }
        if (CharcterBodyParts.instance)
        {
            charcterBodyParts.DefaultTexture(false);
        }

        if (!IsStartUp_Canvas && !PremiumUsersDetails.Instance.CheckSpecificItem(PresetNameinServer))
        {
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
                AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "ChangecharacterOnCLickFromserver", AR_UndoRedo.ActionType.ChangeItem, Color.white, StoreManager.EnumClass.CategoryEnum.Presets);
            }

            if (XanaConstants.xanaConstants._lastClickedBtn && XanaConstants.xanaConstants._curretClickedBtn == XanaConstants.xanaConstants._lastClickedBtn
                && !IsStartUp_Canvas)
                return;

            //GameManager.Instance.isStoreAssetDownloading = true;
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

            //SavingCharacterDataClass _CharacterData2 = new SavingCharacterDataClass();
            //_CharacterData2 = JsonUtility.FromJson<SavingCharacterDataClass>(JsonDataPreset);
            //_CharacterData2.BodyFat = 0;
            //_CharacterData2.PresetValue = gameObject.name;
            //File.WriteAllText((Application.persistentDataPath + "/SavingReoPreset.json"), JsonUtility.ToJson(_CharacterData2));

            SavaCharacterProperties.instance.SaveItemList = JsonUtility.FromJson<SavingCharacterDataClass>(JsonDataPreset);
            SavaCharacterProperties.instance.SaveItemList.BodyFat = 0;
            SavaCharacterProperties.instance.SaveItemList.PresetValue = gameObject.name;
            File.WriteAllText((Application.persistentDataPath + "/SavingReoPreset.json"), JsonUtility.ToJson(SavaCharacterProperties.instance.SaveItemList));

            XanaConstants.xanaConstants.bodyNumber = 0;

            if (StoreManager.instance.StartPanel_PresetParentPanel.activeSelf)
            {
                Invoke("abcd", 1f);
                StoreManager.instance.StartPanel_PresetParentPanel.SetActive(false);
                UserRegisterationManager.instance.UsernameFieldAdvance.Clear();
                UserRegisterationManager.instance.usernamePanal.SetActive(true);
                if (PlayerPrefs.GetInt("iSignup") != 1) // if play as a guest
                {
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
            if (avatarController.wornEyewearable != null)
            {
                avatarController.UnStichItem("EyeWearable");
            }

            //if (_CharacterData.HairColor != null)
            if (SavaCharacterProperties.instance.SaveItemList.HairColor != null)
                XanaConstants.xanaConstants.isPresetHairColor = true;

            FetchPresetIds(SavaCharacterProperties.instance.SaveItemList);

            //avatarController.ApplyPreset(_CharacterData);
        //    GetSavedPreset();
        //    if (!presetAlreadySaved)
        //    {
        //        StoreManager.instance.SaveStoreBtn.GetComponent<Button>().interactable = true;
        //        SavedButtonClickedBlue();
        //    }
        //    else
        //    {
        //        StoreManager.instance.SaveStoreBtn.SetActive(true);
        //        StoreManager.instance.SaveStoreBtn.GetComponent<Button>().interactable = false;
        //        StoreManager.instance.SaveStoreBtn.GetComponent<Image>().color = Color.white;
        //        StoreManager.instance.GreyRibbonImage.SetActive(true);
        //        StoreManager.instance.WhiteRibbonImage.SetActive(false);
        //    }
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
    public void abcd()
    {  
        if (PlayerPrefs.GetInt("presetPanel") == 1)   // preset panel is enable so saving preset to account 
            PlayerPrefs.SetInt("presetPanel", 0);
        ItemDatabase.instance.GetComponent<SavaCharacterProperties>().SavePlayerProperties();
        avatarController.IntializeAvatar();
    }

    private GameObject dummyPant = null;
    private GameObject dummyShirt = null;
    private GameObject dummyHair = null;
    private GameObject dummyShoe = null;
    private void FetchPresetIds(SavingCharacterDataClass _CharacterData)
    {
        if (_CharacterData.myItemObj.Count > 0)
        {
            for (int i = 0; i < _CharacterData.myItemObj.Count; i++)
            {
                if (!string.IsNullOrEmpty(_CharacterData.myItemObj[i].ItemName))
                {
                    string type = _CharacterData.myItemObj[i].ItemType;
                    if (type.Contains("Legs"))
                    {
                        if (!_CharacterData.myItemObj[i].ItemName.Contains("md", System.StringComparison.CurrentCultureIgnoreCase))
                        {
                            avatarController.wornPantId = _CharacterData.myItemObj[i].ItemID;
                            if(dummyPant != null) Destroy(dummyPant);
                            dummyPant = new GameObject(_CharacterData.myItemObj[i].ItemName);
                            avatarController.wornPant = dummyPant;
                        }
                    }
                    else if (type.Contains("Chest"))
                    {
                        if (!_CharacterData.myItemObj[i].ItemName.Contains("md", System.StringComparison.CurrentCultureIgnoreCase))
                        {
                            avatarController.wornShirtId = _CharacterData.myItemObj[i].ItemID;
                            if (dummyShirt != null) Destroy(dummyShirt);
                            dummyShirt = new GameObject(_CharacterData.myItemObj[i].ItemName);
                            avatarController.wornShirt = dummyShirt;
                        }
                    }
                    else if (type.Contains("Feet"))
                    {
                        if (!_CharacterData.myItemObj[i].ItemName.Contains("md", System.StringComparison.CurrentCultureIgnoreCase))
                        {
                            avatarController.wornShoesId = _CharacterData.myItemObj[i].ItemID;
                            if (dummyShoe != null) Destroy(dummyShoe);
                            dummyShoe = new GameObject(_CharacterData.myItemObj[i].ItemName);
                            avatarController.wornShose = dummyShoe;
                        }
                    }
                    else if (type.Contains("Hair"))
                    {
                        if (!_CharacterData.myItemObj[i].ItemName.Contains("md", System.StringComparison.CurrentCultureIgnoreCase))
                        {
                            avatarController.wornHairId = _CharacterData.myItemObj[i].ItemID;
                            if (dummyHair != null) Destroy(dummyHair);
                            dummyHair = new GameObject(_CharacterData.myItemObj[i].ItemName);
                            avatarController.wornHair = dummyHair;
                        }
                    }
                }
            }
        }
    }

}
