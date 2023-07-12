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
    private void Awake()
    {
        if (instance == null)
            instance = this;
        downloader = AddressableDownloader.Instance;
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
                    if (ActivePanelCallStack.obj.IsCallByBtn() && XanaConstants.xanaConstants.faceIndex == AvatarBtnId)   //!isAddedInUndoRedo && // check if image is selected
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
                    if (ActivePanelCallStack.obj.IsCallByBtn() && XanaConstants.xanaConstants.eyeBrowIndex == AvatarBtnId)   //!isAddedInUndoRedo && // check if image is selected
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
                    if (ActivePanelCallStack.obj.IsCallByBtn() && XanaConstants.xanaConstants.eyeLashesIndex == AvatarBtnId)   //!isAddedInUndoRedo && // check if image is selected
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
                    if (ActivePanelCallStack.obj.IsCallByBtn() && XanaConstants.xanaConstants.makeupIndex == AvatarBtnId)   //!isAddedInUndoRedo && // check if image is selected
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
                    if (ActivePanelCallStack.obj.IsCallByBtn() && XanaConstants.xanaConstants.eyeIndex == AvatarBtnId)   //!isAddedInUndoRedo && // check if image is selected
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
                    if (ActivePanelCallStack.obj.IsCallByBtn() && XanaConstants.xanaConstants.noseIndex == AvatarBtnId)   //!isAddedInUndoRedo && // check if image is selected
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
                    if (ActivePanelCallStack.obj.IsCallByBtn() && XanaConstants.xanaConstants.lipIndex == AvatarBtnId)   //!isAddedInUndoRedo && // check if image is selected
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
                    if (ActivePanelCallStack.obj.IsCallByBtn() && XanaConstants.xanaConstants.bodyNumber == _Bodyint)   //!isAddedInUndoRedo && // check if image is selected
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
                        XanaConstants.xanaConstants.faceIndex = AvatarBtnId;
                        XanaConstants.xanaConstants.isFaceMorphed = false;

                        if (isExist)
                        {
                            if (_CharacterData.FaceValue == XanaConstants.xanaConstants.faceIndex && !_CharacterData.faceMorphed)
                            {
                                itemAlreadySaved = true;
                            }
                        }

                        break;
                    }
                case "EyeBrow":
                    {
                        XanaConstants.xanaConstants.eyeBrowIndex = AvatarBtnId;
                        XanaConstants.xanaConstants.isEyebrowMorphed = false;

                        if (isExist)
                        {
                            if (_CharacterData.EyeBrowValue == XanaConstants.xanaConstants.eyeBrowIndex && !_CharacterData.eyeBrowMorphed)
                            {
                                itemAlreadySaved = true;
                            }
                        }

                        break;
                    }
                case "EyeLashes":
                    {
                        XanaConstants.xanaConstants.eyeLashesIndex = AvatarBtnId;

                        if (isExist)
                        {
                            if (_CharacterData.EyeLashesValue == XanaConstants.xanaConstants.eyeLashesIndex /*&& !_CharacterData.EyeLashesValue*/)
                            {
                                itemAlreadySaved = true;
                            }
                        }
                        break;
                    }
                case "Makeup":
                    {
                        print("makeup");
                        XanaConstants.xanaConstants.makeupIndex = AvatarBtnId;
                        if (isExist)
                        {
                            if (_CharacterData.MakeupValue == XanaConstants.xanaConstants.makeupIndex /*&& !_CharacterData.EyeLashesValue*/)
                            {
                                itemAlreadySaved = true;
                            }
                        }
                        break;
                    }
                case "Eyes":
                    {
                        XanaConstants.xanaConstants.eyeIndex = AvatarBtnId;
                        XanaConstants.xanaConstants.isEyeMorphed = false;

                        if (isExist)
                        {
                            if (_CharacterData.EyeValue == XanaConstants.xanaConstants.eyeIndex && !_CharacterData.eyeMorphed)
                            {
                                itemAlreadySaved = true;
                            }
                        }

                        break;
                    }
                case "Nose":
                    {
                        XanaConstants.xanaConstants.noseIndex = AvatarBtnId;
                        XanaConstants.xanaConstants.isNoseMorphed = false;

                        if (isExist)
                        {
                            if (_CharacterData.NoseValue == XanaConstants.xanaConstants.noseIndex && !_CharacterData.noseMorphed)
                            {
                                itemAlreadySaved = true;
                            }
                        }

                        break;
                    }
                case "Lips":
                    {
                        XanaConstants.xanaConstants.lipIndex = AvatarBtnId;
                        XanaConstants.xanaConstants.isLipMorphed = false;

                        if (isExist)
                        {
                            if (_CharacterData.LipsValue == XanaConstants.xanaConstants.lipIndex && !_CharacterData.lipMorphed)
                            {
                                itemAlreadySaved = true;
                            }
                        }

                        break;
                    }
                case "Body":
                    {
                        XanaConstants.xanaConstants.bodyNumber = _Bodyint;

                        if (isExist)
                        {
                            if (_CharacterData.BodyFat == XanaConstants.xanaConstants.bodyNumber)
                            {
                                if (XanaConstants.xanaConstants.PresetValueString == PlayerPrefs.GetString("PresetValue"))
                                    itemAlreadySaved = true;
                            }
                        }

                        break;
                    }
                case "FaceMorph":
                    {
                        XanaConstants.xanaConstants.isFaceMorphed = true;
                        break;
                    }
                case "EyesMorph":
                    {
                        XanaConstants.xanaConstants.isEyeMorphed = true;
                        break;
                    }
                case "EyeBrowMorph":
                    {
                        XanaConstants.xanaConstants.isEyebrowMorphed = true;
                        break;
                    }
                case "NoseMorph":
                    {
                        XanaConstants.xanaConstants.isNoseMorphed = true;
                        break;
                    }
                case "LipsMorph":
                    {
                        XanaConstants.xanaConstants.isLipMorphed = true;
                        break;
                    }
                case "HeadMorph":
                    {
                        break;
                    }
            }

            XanaConstants.xanaConstants._curretClickedBtn = this.gameObject;
            XanaConstants.xanaConstants.avatarStoreSelection[XanaConstants.xanaConstants.currentButtonIndex] = gameObject;

            if (XanaConstants.xanaConstants._lastClickedBtn && XanaConstants.xanaConstants._curretClickedBtn == XanaConstants.xanaConstants._lastClickedBtn)
                return;

            XanaConstants.xanaConstants._curretClickedBtn.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

            if (XanaConstants.xanaConstants._lastClickedBtn)
            {
                if (XanaConstants.xanaConstants._lastClickedBtn.GetComponent<AvatarBtn>())
                    XanaConstants.xanaConstants._lastClickedBtn.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            }
            //Debug.Log("<color=red>AvatarBtn AssignLastClickedBtnHere</color>");
            XanaConstants.xanaConstants._lastClickedBtn = this.gameObject;

            switch (isBtnString)
            {
                case "Face":
                    {
                        _BDCTrigger.CustomizationTriggerTwo();
                        SavaCharacterProperties.instance.characterController.faceId = AvatarBtnId;
                        break;
                    }
                case "EyeBrow":
                    {
                        _BDCTrigger.CustomizationTriggerTwo();
                        SavaCharacterProperties.instance.characterController.eyeBrowId = AvatarBtnId;

                        if (StoreManager.instance.UndoBtn)
                            StoreManager.instance.UndoBtn.GetComponent<Button>().interactable = true;

                        break;
                    }
                case "EyeLashes":
                    {
                        string lashesName = GetComponent<EyeLashBtn>().LashesName;
                        downloader.StartCoroutine(downloader.DownloadAddressableTexture(lashesName, GameManager.Instance.mainCharacter, CurrentTextureType.EyeLashes));
                        SavaCharacterProperties.instance.characterController.eyeLashesId = AvatarBtnId;
                        break;
                    }
                case "Makup":
                    {
                        string makeupName = GetComponent<EyeLashBtn>().LashesName;
                        downloader.StartCoroutine(downloader.DownloadAddressableTexture(makeupName, GameManager.Instance.mainCharacter, CurrentTextureType.Makeup));
                        SavaCharacterProperties.instance.characterController.makeupId = AvatarBtnId;
                        break;
                    }
                case "Makeup":
                    {
                        //print("~~~~~~~~~ makeup call ");
                        string lashesName = GetComponent<EyeLashBtn>().LashesName;
                        downloader.StartCoroutine(downloader.DownloadAddressableTexture(lashesName, GameManager.Instance.mainCharacter, CurrentTextureType.Makeup));
                        SavaCharacterProperties.instance.characterController.makeupId = AvatarBtnId;
                        break;
                    }
                case "Eyes":
                    {
                        _BDCTrigger.CustomizationTriggerTwo();
                        SavaCharacterProperties.instance.characterController.eyesId = AvatarBtnId;

                        if (StoreManager.instance.UndoBtn)
                            StoreManager.instance.UndoBtn.GetComponent<Button>().interactable = true;
                        break;
                    }
                case "Nose":
                    {
                        _BDCTrigger.CustomizationTriggerTwo();
                        SavaCharacterProperties.instance.characterController.noseId = AvatarBtnId;

                        if (StoreManager.instance.UndoBtn)
                            StoreManager.instance.UndoBtn.GetComponent<Button>().interactable = true;
                        break;
                    }
                case "Lips":
                    {
                        _BDCTrigger.CustomizationTriggerTwo();
                        SavaCharacterProperties.instance.characterController.lipsId = AvatarBtnId;

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

                        SavaCharacterProperties.instance.characterController.bodyFat = _Bodyint;

                        if (StoreManager.instance.UndoBtn)
                            StoreManager.instance.UndoBtn.GetComponent<Button>().interactable = true;
                        if (GameManager.Instance)
                        {
                            GameManager.Instance.mainCharacter.GetComponent<AvatarController>().ResizeClothToBodyFat(GameManager.Instance.mainCharacter.gameObject, XanaConstants.xanaConstants.bodyNumber);
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
                        UIManager.Instance._footerCan.SetActive(false);
                        GameManager.Instance.mainCharacter.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                        ChangeCameraForZoomFace.instance.ChangeCameraToIsometric();

                        //BlendShapeImporter.Instance.MorphTypeSelected("Head");
                        BlendShapeImporter.Instance.MorphTypeSelected("FaceMorph");

                        CharacterCustomizationUIManager.Instance.LoadCustomBlendShapePanel("Face");
                        BlendShapeImporter.Instance.TurnOnPoints("FaceMorph");
                        break;
                    }
                case "EyeBrowMorph":
                    {
                        CharacterCustomizationManager.Instance.OnFrontSide();
                        UIManager.Instance._footerCan.SetActive(false);
                        GameManager.Instance.mainCharacter.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                        ChangeCameraForZoomFace.instance.ChangeCameraToIsometric();
                        BlendShapeImporter.Instance.MorphTypeSelected("EyeBrow");
                        CharacterCustomizationUIManager.Instance.LoadCustomBlendShapePanel("Eyebrow");
                        BlendShapeImporter.Instance.TurnOnPoints("EyeBrowMorph");
                        break;
                    }
                case "EyesMorph":
                    {
                        EyesBlinking.instance.isBlinking = false;       // Added by Ali Hamza

                        CharacterCustomizationManager.Instance.OnFrontSide();
                        UIManager.Instance._footerCan.SetActive(false);
                        GameManager.Instance.mainCharacter.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                        ChangeCameraForZoomFace.instance.ChangeCameraToIsometric();
                        BlendShapeImporter.Instance.MorphTypeSelected("eye");
                        CharacterCustomizationUIManager.Instance.LoadCustomBlendShapePanel("Eyes");
                        BlendShapeImporter.Instance.TurnOnPoints("EyesMorph");
                        break;
                    }
                case "NoseMorph":
                    {
                        CharacterCustomizationManager.Instance.OnFrontSide();
                        UIManager.Instance._footerCan.SetActive(false);
                        GameManager.Instance.mainCharacter.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                        ChangeCameraForZoomFace.instance.ChangeCameraToIsometric();
                        BlendShapeImporter.Instance.MorphTypeSelected("Nose");
                        CharacterCustomizationUIManager.Instance.LoadCustomBlendShapePanel("Nose");
                        BlendShapeImporter.Instance.TurnOnPoints("NoseMorph");
                        break;
                    }
                case "LipsMorph":
                    {
                        CharacterCustomizationManager.Instance.OnFrontSide();
                        UIManager.Instance._footerCan.SetActive(false);
                        GameManager.Instance.mainCharacter.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                        ChangeCameraForZoomFace.instance.ChangeCameraToIsometric();
                        BlendShapeImporter.Instance.MorphTypeSelected("Lips");
                        CharacterCustomizationUIManager.Instance.LoadCustomBlendShapePanel("Lips");
                        BlendShapeImporter.Instance.TurnOnPoints("LipsMorph");
                        break;
                    }
                case "HeadMorph":
                    {
                        CharacterCustomizationManager.Instance.OnFrontSide();
                        UIManager.Instance._footerCan.SetActive(false);
                        GameManager.Instance.mainCharacter.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                        ChangeCameraForZoomFace.instance.ChangeCameraToIsometric();
                        BlendShapeImporter.Instance.MorphTypeSelected("Head");
                        CharacterCustomizationUIManager.Instance.LoadCustomBlendShapePanel("Head");
                        BlendShapeImporter.Instance.TurnOnPoints("HeadMorph");
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