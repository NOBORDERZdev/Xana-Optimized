using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static StoreManager;

public class AvatarBtn : MonoBehaviour
{
    public static AvatarBtn instance;
    public string isBtnString;
    public int AvatarBtnId;
    public int _Bodyint;
    public int hairIndex;
    public Image myImage;
    public Sprite myIcon;
    public Text PriceTxt;

    private BodyCustomizationTrigger _BDCTrigger;

    public GameObject LipsCustomizer, EyesCustomizer, NoseCustomizer, EyebrowsCustomizer, FaceCustomizer;

    bool itemAlreadySaved = false;
    bool isExist = false;
    bool isAdded = true;
    //bool isAddedInUndoRedo = false;       
    SavingCharacterDataClass _CharacterData;
    AddressableDownloader downloader;
    BlendShapeImporter shapeImporter;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        downloader = AddressableDownloader.Instance;
        shapeImporter = GameManager.Instance.BlendShapeImporter;
    }

    private void OnEnable()
    {
        Invoke("Delay", 0.1f);
    }
    void Delay()
    {
        switch (isBtnString)
        {
            case "Face":
                {
                    if (ActivePanelCallStack.obj.IsCallByBtn() && ConstantsHolder.xanaConstants.faceIndex == AvatarBtnId)   //!isAddedInUndoRedo && // check if image is selected
                    {

                        if (!AR_UndoRedo.obj.addToList)
                            AR_UndoRedo.obj.addToList = true;
                        else
                        {
                            AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", AR_UndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.Face);
                            Debug.Log("<color=red> Set Default Face morph</color>");
                        }
                    }
                }
                break;
            case "EyeBrow":
                {
                    if (ActivePanelCallStack.obj.IsCallByBtn() && ConstantsHolder.xanaConstants.eyeBrowIndex == AvatarBtnId)   //!isAddedInUndoRedo && // check if image is selected
                    {

                        if (!AR_UndoRedo.obj.addToList)
                            AR_UndoRedo.obj.addToList = true;
                        else
                        {
                            AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", AR_UndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.EyeBrowAvatar);
                            Debug.Log("<color=red> Set Default Eyebrow morph </color>");
                        }
                    }
                }
                break;
            case "EyeLashes":
                {
                    if (ActivePanelCallStack.obj.IsCallByBtn() && ConstantsHolder.xanaConstants.eyeLashesIndex == AvatarBtnId)   //!isAddedInUndoRedo && // check if image is selected
                    {

                        if (!AR_UndoRedo.obj.addToList)
                            AR_UndoRedo.obj.addToList = true;
                        else
                        {
                            AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", AR_UndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.EyeLashesAvatar);
                            Debug.Log("<color=red> Set Default EyeLashes morph </color>");
                        }
                    }
                }
                break;
            case "Makeup":
                {
                    if (ActivePanelCallStack.obj.IsCallByBtn() && ConstantsHolder.xanaConstants.makeupIndex == AvatarBtnId)   //!isAddedInUndoRedo && // check if image is selected
                    {

                        if (!AR_UndoRedo.obj.addToList)
                            AR_UndoRedo.obj.addToList = true;
                        else
                        {
                            AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", AR_UndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.Makeup);
                            Debug.Log("<color=red> Set Default Makeup </color>");
                        }
                    }
                }
                break;
            case "Eyes":
                {
                    if (ActivePanelCallStack.obj.IsCallByBtn() && ConstantsHolder.xanaConstants.eyeIndex == AvatarBtnId)   //!isAddedInUndoRedo && // check if image is selected
                    {

                        if (!AR_UndoRedo.obj.addToList)
                            AR_UndoRedo.obj.addToList = true;
                        else
                        {
                            AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", AR_UndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.EyesAvatar);
                            Debug.Log("<color=red> Set Default Eye </color>");
                        }
                    }
                }
                break;
            case "Nose":
                {
                    if (ActivePanelCallStack.obj.IsCallByBtn() && ConstantsHolder.xanaConstants.noseIndex == AvatarBtnId)   //!isAddedInUndoRedo && // check if image is selected
                    {

                        if (!AR_UndoRedo.obj.addToList)
                            AR_UndoRedo.obj.addToList = true;
                        else
                        {
                            AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", AR_UndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.Nose);
                            Debug.Log("<color=red> Set Default Nose </color>");
                        }
                    }
                }
                break;
            case "Lips":
                {
                    if (ActivePanelCallStack.obj.IsCallByBtn() && ConstantsHolder.xanaConstants.lipIndex == AvatarBtnId)   //!isAddedInUndoRedo && // check if image is selected
                    {

                        if (!AR_UndoRedo.obj.addToList)
                            AR_UndoRedo.obj.addToList = true;
                        else
                        {
                            AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", AR_UndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.LipsAvatar);
                            Debug.Log("<color=red> Set Default Lips </color>");
                        }
                    }
                }
                break;
            case "Body":
                {
                    if (ActivePanelCallStack.obj.IsCallByBtn() && ConstantsHolder.xanaConstants.bodyNumber == _Bodyint)   //!isAddedInUndoRedo && // check if image is selected
                    {

                        if (!AR_UndoRedo.obj.addToList)
                            AR_UndoRedo.obj.addToList = true;
                        else
                        {
                            AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", AR_UndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.Body);
                            Debug.Log("<color=red> Set Default Body </color>");
                        }
                    }
                }
                break;
        }

    }

    void Start()
    {
        if (gameObject.GetComponent<BodyCustomizationTrigger>())
        {
            _BDCTrigger = gameObject.GetComponent<BodyCustomizationTrigger>();
        }
        this.gameObject.GetComponent<Button>().onClick.AddListener(OnAvatarBtnClick);
        PriceTxt.enabled = false;

        _CharacterData = new SavingCharacterDataClass();
    }

    private void OnAvatarBtnClick()
    {
        //print("btn name is " + isBtnString);
        if (GetComponent<Image>().color.a is 1f) return;     // if selected is already then return
          
        itemAlreadySaved = false;
        isExist = false;

        if (File.Exists(GameManager.Instance.GetStringFolderPath()) && File.ReadAllText(GameManager.Instance.GetStringFolderPath()) != "")
        {
            _CharacterData = _CharacterData.CreateFromJSON(File.ReadAllText(GameManager.Instance.GetStringFolderPath()));
            isExist = true;
        }

        string CurrentString = "";

        //USe This switch for premium only
        switch (isBtnString)
        {
            case "Face":
                {
                    CurrentString = "Face";
                    // Undo Redo Functionailty
                    if (!AR_UndoRedo.obj.addToList)
                        AR_UndoRedo.obj.addToList = true;
                    else
                    {
                        AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", AR_UndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.Face);
                        Debug.Log("<color=red> Set Face morph btn into list </color>");
                    }
                    break;
                }
            case "HeadDefault":
                {
                    CurrentString = "Face";

                    break;
                }
            case "EyeBrow":
                {
                    CurrentString = "Eye Brow";

                    if (!AR_UndoRedo.obj.addToList)
                        AR_UndoRedo.obj.addToList = true;
                    else
                    {
                        AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", AR_UndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.EyeBrowAvatar);
                        Debug.Log("<color=red> Set Eyebrow morph btn into list </color>");
                    }
                    break;
                }
            case "EyeLashes":
                {
                    CurrentString = "EyeLashes";

                    if (!AR_UndoRedo.obj.addToList)
                        AR_UndoRedo.obj.addToList = true;
                    else
                    {
                        AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", AR_UndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.EyeLashesAvatar);
                        Debug.Log("<color=red> Set EyeLashes morph btn into list </color>");
                    }
                    break;
                }
            case "Makeup":
                {
                    CurrentString = "Makeup";

                    if (!AR_UndoRedo.obj.addToList)
                        AR_UndoRedo.obj.addToList = true;
                    else
                    {
                        AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", AR_UndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.Makeup);
                        Debug.Log("<color=red> Set Makeup morph btn into list </color>");
                    }
                    break;
                }
            case "Eyes":
                {
                    CurrentString = "Eyes";

                    if (!AR_UndoRedo.obj.addToList)
                        AR_UndoRedo.obj.addToList = true;
                    else
                    {
                        AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", AR_UndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.EyesAvatar);
                        Debug.Log("<color=red> Set Eye morph btn into list </color>");
                    }
                    break;
                }
            case "Nose":
                {
                    CurrentString = "Nose";
                    if (!AR_UndoRedo.obj.addToList)
                        AR_UndoRedo.obj.addToList = true;
                    else
                    {
                        AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", AR_UndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.Nose);
                        Debug.Log("<color=red> Set Nose morph btn into list </color>");
                    }
                    break;
                }
            case "Lips":
                {
                    CurrentString = "Lip";
                    if (!AR_UndoRedo.obj.addToList)
                        AR_UndoRedo.obj.addToList = true;
                    else
                    {
                        AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", AR_UndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.LipsAvatar);
                        Debug.Log("<color=red> Set lips morph btn into list </color>");
                    }
                    break;
                }
            case "LipsDefault":
                {
                    CurrentString = "Lip";
                    break;
                }
            case "Body":
                {
                    CurrentString = "Body";

                    if (!AR_UndoRedo.obj.addToList)
                        AR_UndoRedo.obj.addToList = true;
                    else
                    {
                        AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", AR_UndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.Body);
                        Debug.Log("<color=red> Set body morph btn into list </color>");
                    }
                    break;
                }
            case "FaceMorph":
                {
                    CurrentString = "Morphs";
                    break;
                }
            case "EyesMorph":
                {
                    CurrentString = "Morphs";
                    break;
                }
            case "EyeBrowMorph":
                {
                    CurrentString = "Morphs";
                    break;
                }
            case "NoseMorph":
                {
                    CurrentString = "Morphs";
                    break;
                }
            case "LipsMorph":
                {
                    CurrentString = "Morphs";
                    break;
                }
        }


        if ((!PremiumUsersDetails.Instance.CheckSpecificItem(CurrentString) && (CurrentString != "Makeup" && CurrentString != "EyeLashes")) && CurrentString != "")
        {
            //PremiumUsersDetails.Instance.PremiumUserUI.SetActive(true);
            //print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            //print("Horayyy you have Access");

            switch (isBtnString)
            {
                case "Face":
                    {
                        ConstantsHolder.xanaConstants.faceIndex = AvatarBtnId;
                        ConstantsHolder.xanaConstants.isFaceMorphed = false;

                        if (isExist)
                        {
                            if (_CharacterData.FaceValue == ConstantsHolder.xanaConstants.faceIndex && !_CharacterData.faceMorphed)
                            {
                                itemAlreadySaved = true;
                            }
                        }

                        break;
                    }
                case "EyeBrow":
                    {
                        ConstantsHolder.xanaConstants.eyeBrowIndex = AvatarBtnId;
                        ConstantsHolder.xanaConstants.isEyebrowMorphed = false;

                        if (isExist)
                        {
                            if (_CharacterData.EyeBrowValue == ConstantsHolder.xanaConstants.eyeBrowIndex && !_CharacterData.eyeBrowMorphed)
                            {
                                itemAlreadySaved = true;
                            }
                        }

                        break;
                    }
                case "EyeLashes":
                    {
                        ConstantsHolder.xanaConstants.eyeLashesIndex = AvatarBtnId;

                        if (isExist)
                        {
                            if (_CharacterData.EyeLashesValue == ConstantsHolder.xanaConstants.eyeLashesIndex /*&& !_CharacterData.EyeLashesValue*/)
                            {
                                itemAlreadySaved = true;
                            }
                        }
                        break;
                    }
                case "Makeup":
                    {
                        print("makeup");
                        ConstantsHolder.xanaConstants.makeupIndex = AvatarBtnId;
                        if (isExist)
                        {
                            if (_CharacterData.MakeupValue == ConstantsHolder.xanaConstants.makeupIndex /*&& !_CharacterData.EyeLashesValue*/)
                            {
                                itemAlreadySaved = true;
                            }
                        }
                        break;
                    }
                case "Eyes":
                    {
                        ConstantsHolder.xanaConstants.eyeIndex = AvatarBtnId;
                        ConstantsHolder.xanaConstants.isEyeMorphed = false;

                        if (isExist)
                        {
                            if (_CharacterData.EyeValue == ConstantsHolder.xanaConstants.eyeIndex && !_CharacterData.eyeMorphed)
                            {
                                itemAlreadySaved = true;
                            }
                        }

                        break;
                    }
                case "Nose":
                    {
                        ConstantsHolder.xanaConstants.noseIndex = AvatarBtnId;
                        ConstantsHolder.xanaConstants.isNoseMorphed = false;

                        if (isExist)
                        {
                            if (_CharacterData.NoseValue == ConstantsHolder.xanaConstants.noseIndex && !_CharacterData.noseMorphed)
                            {
                                itemAlreadySaved = true;
                            }
                        }

                        break;
                    }
                case "Lips":
                    {
                        ConstantsHolder.xanaConstants.lipIndex = AvatarBtnId;
                        ConstantsHolder.xanaConstants.isLipMorphed = false;

                        if (isExist)
                        {
                            if (_CharacterData.LipsValue == ConstantsHolder.xanaConstants.lipIndex && !_CharacterData.lipMorphed)
                            {
                                itemAlreadySaved = true;
                            }
                        }

                        break;
                    }
                case "Body":
                    {
                        ConstantsHolder.xanaConstants.bodyNumber = _Bodyint;

                        if (isExist)
                        {
                            if (_CharacterData.BodyFat == ConstantsHolder.xanaConstants.bodyNumber)
                            {
                                if (ConstantsHolder.xanaConstants.PresetValueString == PlayerPrefs.GetString("PresetValue"))
                                    itemAlreadySaved = true;
                            }
                        }

                        break;
                    }
                case "FaceMorph":
                    {
                        ConstantsHolder.xanaConstants.isFaceMorphed = true;
                        break;
                    }
                case "EyesMorph":
                    {
                        ConstantsHolder.xanaConstants.isEyeMorphed = true;
                        break;
                    }
                case "EyeBrowMorph":
                    {
                        ConstantsHolder.xanaConstants.isEyebrowMorphed = true;
                        break;
                    }
                case "NoseMorph":
                    {
                        ConstantsHolder.xanaConstants.isNoseMorphed = true;
                        break;
                    }
                case "LipsMorph":
                    {
                        ConstantsHolder.xanaConstants.isLipMorphed = true;
                        break;
                    }
                case "HeadMorph":
                    {
                        break;
                    }
            }

            if (CurrentString != "Morphs")
            {
                PatchForStore.isCustomizationPanelOpen = false;
                ConstantsHolder.xanaConstants._curretClickedBtn = this.gameObject;
                ConstantsHolder.xanaConstants.avatarStoreSelection[ConstantsHolder.xanaConstants.currentButtonIndex] = gameObject;

                if (ConstantsHolder.xanaConstants._lastAvatarClickedBtn && ConstantsHolder.xanaConstants._curretClickedBtn == ConstantsHolder.xanaConstants._lastAvatarClickedBtn)
                    return;

                ConstantsHolder.xanaConstants._curretClickedBtn.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

                if (ConstantsHolder.xanaConstants._lastAvatarClickedBtn)
                {
                    if (ConstantsHolder.xanaConstants._lastAvatarClickedBtn.GetComponent<AvatarBtn>())
                        ConstantsHolder.xanaConstants._lastAvatarClickedBtn.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
                }
            }
            else
            {
                PatchForStore.isCustomizationPanelOpen = true;
            }
            //Debug.Log("<color=red>AvatarBtn AssignLastClickedBtnHere</color>");
            ConstantsHolder.xanaConstants._lastAvatarClickedBtn = this.gameObject;

            switch (isBtnString)
            {
                case "Face":
                    {
                        _BDCTrigger.CustomizationTriggerTwo();
                        SaveCharacterProperties.instance.characterController.faceId = AvatarBtnId;
                        break;
                    }
                case "EyeBrow":
                    {
                        _BDCTrigger.CustomizationTriggerTwo();
                        SaveCharacterProperties.instance.characterController.eyeBrowId = AvatarBtnId;

                        if (StoreManager.instance.UndoBtn)
                            StoreManager.instance.UndoBtn.GetComponent<Button>().interactable = true;

                        break;
                    }
                case "EyeLashes":
                    {
                        string lashesName = GetComponent<EyeLashBtn>().LashesName;
                        downloader.StartCoroutine(downloader.DownloadAddressableTexture(lashesName, GameManager.Instance.mainCharacter, CurrentTextureType.EyeLashes));
                        SaveCharacterProperties.instance.characterController.eyeLashesId = AvatarBtnId;
                        break;
                    }
                case "Makup":
                    {
                        string makeupName = GetComponent<EyeLashBtn>().LashesName;
                        downloader.StartCoroutine(downloader.DownloadAddressableTexture(makeupName, GameManager.Instance.mainCharacter, CurrentTextureType.Makeup));
                        SaveCharacterProperties.instance.characterController.makeupId = AvatarBtnId;
                        break;
                    }
                case "Makeup":
                    {
                        //print("~~~~~~~~~ makeup call ");
                        string lashesName = GetComponent<EyeLashBtn>().LashesName;
                        downloader.StartCoroutine(downloader.DownloadAddressableTexture(lashesName, GameManager.Instance.mainCharacter, CurrentTextureType.Makeup));
                        SaveCharacterProperties.instance.characterController.makeupId = AvatarBtnId;
                        break;
                    }
                case "Eyes":
                    {
                        _BDCTrigger.CustomizationTriggerTwo();
                        SaveCharacterProperties.instance.characterController.eyesId = AvatarBtnId;

                        if (StoreManager.instance.UndoBtn)
                            StoreManager.instance.UndoBtn.GetComponent<Button>().interactable = true;
                        break;
                    }
                case "Nose":
                    {
                        _BDCTrigger.CustomizationTriggerTwo();
                        SaveCharacterProperties.instance.characterController.noseId = AvatarBtnId;

                        if (StoreManager.instance.UndoBtn)
                            StoreManager.instance.UndoBtn.GetComponent<Button>().interactable = true;
                        break;
                    }
                case "Lips":
                    {
                        _BDCTrigger.CustomizationTriggerTwo();
                        SaveCharacterProperties.instance.characterController.lipsId = AvatarBtnId;

                        if (StoreManager.instance.UndoBtn)
                            StoreManager.instance.UndoBtn.GetComponent<Button>().interactable = true;
                        break;
                    }
                case "Body":
                    {
                        StoreManager.instance.SaveStoreBtn.SetActive(true);
                        StoreManager.instance.SaveStoreBtn.GetComponent<Image>().color = new Color(0f, 0.5f, 1f, 0.8f);
                        StoreManager.instance.GreyRibbonImage.SetActive(false);
                        StoreManager.instance.WhiteRibbonImage.SetActive(true);
                        StoreManager.instance.ClearBuyItems();

                        SaveCharacterProperties.instance.characterController.bodyFat = _Bodyint;

                        if (StoreManager.instance.UndoBtn)
                            StoreManager.instance.UndoBtn.GetComponent<Button>().interactable = true;
                        if (GameManager.Instance)
                        {
                            GameManager.Instance.mainCharacter.GetComponent<AvatarController>().ResizeClothToBodyFat(GameManager.Instance.mainCharacter.gameObject, ConstantsHolder.xanaConstants.bodyNumber);
                        }
                        break;
                    }
                case "Skin":
                    {
                        break;
                    }
                case "FaceMorph":
                    {
                        CharacterCustomizationManager.Instance.OnFrontSide();
                        GameManager.Instance.UiManager._footerCan.SetActive(false);
                        GameManager.Instance.mainCharacter.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                        SetCameraPosForFaceCustomization.instance.ChangeCameraToIsometric();

                        //shapeImporter.MorphTypeSelected("Head");
                        shapeImporter.MorphTypeSelected("FaceMorph");

                        CharacterCustomizationUIManager.Instance.LoadCustomBlendShapePanel("Face");
                        shapeImporter.TurnOnPoints("FaceMorph");
                        break;
                    }
                case "EyeBrowMorph":
                    {
                        CharacterCustomizationManager.Instance.OnFrontSide();
                        GameManager.Instance.UiManager._footerCan.SetActive(false);
                        GameManager.Instance.mainCharacter.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                        SetCameraPosForFaceCustomization.instance.ChangeCameraToIsometric();
                        shapeImporter.MorphTypeSelected("EyeBrow");
                        CharacterCustomizationUIManager.Instance.LoadCustomBlendShapePanel("Eyebrow");
                        shapeImporter.TurnOnPoints("EyeBrowMorph");
                        break;
                    }
                case "EyesMorph":
                    {
                        EyesBlinking.instance.isBlinking = false;       // Added by Ali Hamza

                        CharacterCustomizationManager.Instance.OnFrontSide();
                        GameManager.Instance.UiManager._footerCan.SetActive(false);
                        GameManager.Instance.mainCharacter.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                        SetCameraPosForFaceCustomization.instance.ChangeCameraToIsometric();
                        shapeImporter.MorphTypeSelected("eye");
                        CharacterCustomizationUIManager.Instance.LoadCustomBlendShapePanel("Eyes");
                        shapeImporter.TurnOnPoints("EyesMorph");
                        break;
                    }
                case "NoseMorph":
                    {
                        CharacterCustomizationManager.Instance.OnFrontSide();
                        GameManager.Instance.UiManager._footerCan.SetActive(false);
                        GameManager.Instance.mainCharacter.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                        SetCameraPosForFaceCustomization.instance.ChangeCameraToIsometric();
                        shapeImporter.MorphTypeSelected("Nose");
                        CharacterCustomizationUIManager.Instance.LoadCustomBlendShapePanel("Nose");
                        shapeImporter.TurnOnPoints("NoseMorph");
                        break;
                    }
                case "LipsMorph":
                    {
                        CharacterCustomizationManager.Instance.OnFrontSide();
                        GameManager.Instance.UiManager._footerCan.SetActive(false);
                        GameManager.Instance.mainCharacter.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                        SetCameraPosForFaceCustomization.instance.ChangeCameraToIsometric();
                        shapeImporter.MorphTypeSelected("Lips");
                        CharacterCustomizationUIManager.Instance.LoadCustomBlendShapePanel("Lips");
                        shapeImporter.TurnOnPoints("LipsMorph");
                        break;
                    }
                case "HeadMorph":
                    {
                        CharacterCustomizationManager.Instance.OnFrontSide();
                        GameManager.Instance.UiManager._footerCan.SetActive(false);
                        GameManager.Instance.mainCharacter.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                        SetCameraPosForFaceCustomization.instance.ChangeCameraToIsometric();
                        shapeImporter.MorphTypeSelected("Head");
                        CharacterCustomizationUIManager.Instance.LoadCustomBlendShapePanel("Head");
                        shapeImporter.TurnOnPoints("HeadMorph");
                    }
                    break;
            }

            if (!itemAlreadySaved)
            {
                StoreManager.instance.SaveStoreBtn.GetComponent<Button>().interactable = true;
                StoreManager.instance.SaveStoreBtn.SetActive(true);
                StoreManager.instance.SaveStoreBtn.GetComponent<Image>().color = new Color(0f, 0.5f, 1f, 0.8f);
                StoreManager.instance.GreyRibbonImage.SetActive(false);
                StoreManager.instance.WhiteRibbonImage.SetActive(true);
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
}