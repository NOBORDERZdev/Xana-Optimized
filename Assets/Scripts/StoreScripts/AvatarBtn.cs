using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static InventoryManager;

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
                    if (StoreStackHandler.obj.IsCallByBtn() && ConstantsHolder.xanaConstants.faceIndex == AvatarBtnId)   //!isAddedInUndoRedo && // check if image is selected
                    {

                        if (!StoreUndoRedo.obj.addToList)
                            StoreUndoRedo.obj.addToList = true;
                        else
                        {
                            StoreUndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", StoreUndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.Face);
                            Debug.Log("<color=red> Set Default Face morph</color>");
                        }
                    }
                }
                break;
            case "EyeBrow":
                {
                    if (StoreStackHandler.obj.IsCallByBtn() && ConstantsHolder.xanaConstants.eyeBrowIndex == AvatarBtnId)   //!isAddedInUndoRedo && // check if image is selected
                    {

                        if (!StoreUndoRedo.obj.addToList)
                            StoreUndoRedo.obj.addToList = true;
                        else
                        {
                            StoreUndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", StoreUndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.EyeBrowAvatar);
                            Debug.Log("<color=red> Set Default Eyebrow morph </color>");
                        }
                    }
                }
                break;
            case "EyeLashes":
                {
                    if (StoreStackHandler.obj.IsCallByBtn() && ConstantsHolder.xanaConstants.eyeLashesIndex == AvatarBtnId)   //!isAddedInUndoRedo && // check if image is selected
                    {

                        if (!StoreUndoRedo.obj.addToList)
                            StoreUndoRedo.obj.addToList = true;
                        else
                        {
                            StoreUndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", StoreUndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.EyeLashesAvatar);
                            Debug.Log("<color=red> Set Default EyeLashes morph </color>");
                        }
                    }
                }
                break;
            case "Makeup":
                {
                    if (StoreStackHandler.obj.IsCallByBtn() && ConstantsHolder.xanaConstants.makeupIndex == AvatarBtnId)   //!isAddedInUndoRedo && // check if image is selected
                    {

                        if (!StoreUndoRedo.obj.addToList)
                            StoreUndoRedo.obj.addToList = true;
                        else
                        {
                            StoreUndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", StoreUndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.Makeup);
                            Debug.Log("<color=red> Set Default Makeup </color>");
                        }
                    }
                }
                break;
            case "Eyes":
                {
                    if (StoreStackHandler.obj.IsCallByBtn() && ConstantsHolder.xanaConstants.eyeIndex == AvatarBtnId)   //!isAddedInUndoRedo && // check if image is selected
                    {

                        if (!StoreUndoRedo.obj.addToList)
                            StoreUndoRedo.obj.addToList = true;
                        else
                        {
                            StoreUndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", StoreUndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.EyesAvatar);
                            Debug.Log("<color=red> Set Default Eye </color>");
                        }
                    }
                }
                break;
            case "Nose":
                {
                    if (StoreStackHandler.obj.IsCallByBtn() && ConstantsHolder.xanaConstants.noseIndex == AvatarBtnId)   //!isAddedInUndoRedo && // check if image is selected
                    {

                        if (!StoreUndoRedo.obj.addToList)
                            StoreUndoRedo.obj.addToList = true;
                        else
                        {
                            StoreUndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", StoreUndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.Nose);
                            Debug.Log("<color=red> Set Default Nose </color>");
                        }
                    }
                }
                break;
            case "Lips":
                {
                    if (StoreStackHandler.obj.IsCallByBtn() && ConstantsHolder.xanaConstants.lipIndex == AvatarBtnId)   //!isAddedInUndoRedo && // check if image is selected
                    {

                        if (!StoreUndoRedo.obj.addToList)
                            StoreUndoRedo.obj.addToList = true;
                        else
                        {
                            StoreUndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", StoreUndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.LipsAvatar);
                            Debug.Log("<color=red> Set Default Lips </color>");
                        }
                    }
                }
                break;
            case "Body":
                {
                    if (StoreStackHandler.obj.IsCallByBtn() && ConstantsHolder.xanaConstants.bodyNumber == _Bodyint)   //!isAddedInUndoRedo && // check if image is selected
                    {

                        if (!StoreUndoRedo.obj.addToList)
                            StoreUndoRedo.obj.addToList = true;
                        else
                        {
                            StoreUndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", StoreUndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.Body);
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
                    if (!StoreUndoRedo.obj.addToList)
                        StoreUndoRedo.obj.addToList = true;
                    else
                    {
                        StoreUndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", StoreUndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.Face);
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

                    if (!StoreUndoRedo.obj.addToList)
                        StoreUndoRedo.obj.addToList = true;
                    else
                    {
                        StoreUndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", StoreUndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.EyeBrowAvatar);
                        Debug.Log("<color=red> Set Eyebrow morph btn into list </color>");
                    }
                    break;
                }
            case "EyeLashes":
                {
                    CurrentString = "EyeLashes";

                    if (!StoreUndoRedo.obj.addToList)
                        StoreUndoRedo.obj.addToList = true;
                    else
                    {
                        StoreUndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", StoreUndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.EyeLashesAvatar);
                        Debug.Log("<color=red> Set EyeLashes morph btn into list </color>");
                    }
                    break;
                }
            case "Makeup":
                {
                    CurrentString = "Makeup";

                    if (!StoreUndoRedo.obj.addToList)
                        StoreUndoRedo.obj.addToList = true;
                    else
                    {
                        StoreUndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", StoreUndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.Makeup);
                        Debug.Log("<color=red> Set Makeup morph btn into list </color>");
                    }
                    break;
                }
            case "Eyes":
                {
                    CurrentString = "Eyes";

                    if (!StoreUndoRedo.obj.addToList)
                        StoreUndoRedo.obj.addToList = true;
                    else
                    {
                        StoreUndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", StoreUndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.EyesAvatar);
                        Debug.Log("<color=red> Set Eye morph btn into list </color>");
                    }
                    break;
                }
            case "Nose":
                {
                    CurrentString = "Nose";
                    if (!StoreUndoRedo.obj.addToList)
                        StoreUndoRedo.obj.addToList = true;
                    else
                    {
                        StoreUndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", StoreUndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.Nose);
                        Debug.Log("<color=red> Set Nose morph btn into list </color>");
                    }
                    break;
                }
            case "Lips":
                {
                    CurrentString = "Lip";
                    if (!StoreUndoRedo.obj.addToList)
                        StoreUndoRedo.obj.addToList = true;
                    else
                    {
                        StoreUndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", StoreUndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.LipsAvatar);
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

                    if (!StoreUndoRedo.obj.addToList)
                        StoreUndoRedo.obj.addToList = true;
                    else
                    {
                        StoreUndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "OnAvatarBtnClick", StoreUndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.Body);
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


        if ((!UserPassManager.Instance.CheckSpecificItem(CurrentString) && (CurrentString != "Makeup" && CurrentString != "EyeLashes")) && CurrentString != "")
        {
            //UserPassManager.Instance.PremiumUserUI.SetActive(true);
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
                        SavaAvatarProperties.instance.characterController.faceId = AvatarBtnId;
                        break;
                    }
                case "EyeBrow":
                    {
                        _BDCTrigger.CustomizationTriggerTwo();
                        SavaAvatarProperties.instance.characterController.eyeBrowId = AvatarBtnId;

                        if (InventoryManager.instance.UndoBtn)
                            InventoryManager.instance.UndoBtn.GetComponent<Button>().interactable = true;

                        break;
                    }
                case "EyeLashes":
                    {
                        string lashesName = GetComponent<EyeLashBtn>().LashesName;
                        downloader.StartCoroutine(downloader.DownloadAddressableTexture(lashesName, GameManager.Instance.mainCharacter, CurrentTextureType.EyeLashes));
                        SavaAvatarProperties.instance.characterController.eyeLashesId = AvatarBtnId;
                        break;
                    }
                case "Makup":
                    {
                        string makeupName = GetComponent<EyeLashBtn>().LashesName;
                        downloader.StartCoroutine(downloader.DownloadAddressableTexture(makeupName, GameManager.Instance.mainCharacter, CurrentTextureType.Makeup));
                        SavaAvatarProperties.instance.characterController.makeupId = AvatarBtnId;
                        break;
                    }
                case "Makeup":
                    {
                        //print("~~~~~~~~~ makeup call ");
                        string lashesName = GetComponent<EyeLashBtn>().LashesName;
                        downloader.StartCoroutine(downloader.DownloadAddressableTexture(lashesName, GameManager.Instance.mainCharacter, CurrentTextureType.Makeup));
                        SavaAvatarProperties.instance.characterController.makeupId = AvatarBtnId;
                        break;
                    }
                case "Eyes":
                    {
                        _BDCTrigger.CustomizationTriggerTwo();
                        SavaAvatarProperties.instance.characterController.eyesId = AvatarBtnId;

                        if (InventoryManager.instance.UndoBtn)
                            InventoryManager.instance.UndoBtn.GetComponent<Button>().interactable = true;
                        break;
                    }
                case "Nose":
                    {
                        _BDCTrigger.CustomizationTriggerTwo();
                        SavaAvatarProperties.instance.characterController.noseId = AvatarBtnId;

                        if (InventoryManager.instance.UndoBtn)
                            InventoryManager.instance.UndoBtn.GetComponent<Button>().interactable = true;
                        break;
                    }
                case "Lips":
                    {
                        _BDCTrigger.CustomizationTriggerTwo();
                        SavaAvatarProperties.instance.characterController.lipsId = AvatarBtnId;

                        if (InventoryManager.instance.UndoBtn)
                            InventoryManager.instance.UndoBtn.GetComponent<Button>().interactable = true;
                        break;
                    }
                case "Body":
                    {
                        InventoryManager.instance.SaveStoreBtn.SetActive(true);
                        InventoryManager.instance.SaveStoreBtn.GetComponent<Image>().color = new Color(0f, 0.5f, 1f, 0.8f);
                        InventoryManager.instance.GreyRibbonImage.SetActive(false);
                        InventoryManager.instance.WhiteRibbonImage.SetActive(true);
                        InventoryManager.instance.ClearBuyItems();

                        SavaAvatarProperties.instance.characterController.bodyFat = _Bodyint;

                        if (InventoryManager.instance.UndoBtn)
                            InventoryManager.instance.UndoBtn.GetComponent<Button>().interactable = true;
                        if (GameManager.Instance)
                        {
                            GameManager.Instance.mainCharacter.GetComponent<AvatarSetupController>().ResizeClothToBodyFat(GameManager.Instance.mainCharacter.gameObject, ConstantsHolder.xanaConstants.bodyNumber);
                        }
                        break;
                    }
                case "Skin":
                    {
                        break;
                    }
                case "FaceMorph":
                    {
                        AvatarCustomizationManager.Instance.OnFrontSide();
                        UIHandler.Instance._footerCan.SetActive(false);
                        GameManager.Instance.mainCharacter.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                        SetCameraPosForFaceCustomization.instance.ChangeCameraToIsometric();

                        //BlendShapesManager.Instance.MorphTypeSelected("Head");
                        BlendShapesManager.Instance.MorphTypeSelected("FaceMorph");

                        AvatarCustomizationUIHandler.Instance.LoadCustomBlendShapePanel("Face");
                        BlendShapesManager.Instance.TurnOnPoints("FaceMorph");
                        break;
                    }
                case "EyeBrowMorph":
                    {
                        AvatarCustomizationManager.Instance.OnFrontSide();
                        UIHandler.Instance._footerCan.SetActive(false);
                        GameManager.Instance.mainCharacter.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                        SetCameraPosForFaceCustomization.instance.ChangeCameraToIsometric();
                        BlendShapesManager.Instance.MorphTypeSelected("EyeBrow");
                        AvatarCustomizationUIHandler.Instance.LoadCustomBlendShapePanel("Eyebrow");
                        BlendShapesManager.Instance.TurnOnPoints("EyeBrowMorph");
                        break;
                    }
                case "EyesMorph":
                    {
                        EyesBlinkController.instance.isBlinking = false;       // Added by Ali Hamza

                        AvatarCustomizationManager.Instance.OnFrontSide();
                        UIHandler.Instance._footerCan.SetActive(false);
                        GameManager.Instance.mainCharacter.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                        SetCameraPosForFaceCustomization.instance.ChangeCameraToIsometric();
                        BlendShapesManager.Instance.MorphTypeSelected("eye");
                        AvatarCustomizationUIHandler.Instance.LoadCustomBlendShapePanel("Eyes");
                        BlendShapesManager.Instance.TurnOnPoints("EyesMorph");
                        break;
                    }
                case "NoseMorph":
                    {
                        AvatarCustomizationManager.Instance.OnFrontSide();
                        UIHandler.Instance._footerCan.SetActive(false);
                        GameManager.Instance.mainCharacter.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                        SetCameraPosForFaceCustomization.instance.ChangeCameraToIsometric();
                        BlendShapesManager.Instance.MorphTypeSelected("Nose");
                        AvatarCustomizationUIHandler.Instance.LoadCustomBlendShapePanel("Nose");
                        BlendShapesManager.Instance.TurnOnPoints("NoseMorph");
                        break;
                    }
                case "LipsMorph":
                    {
                        AvatarCustomizationManager.Instance.OnFrontSide();
                        UIHandler.Instance._footerCan.SetActive(false);
                        GameManager.Instance.mainCharacter.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                        SetCameraPosForFaceCustomization.instance.ChangeCameraToIsometric();
                        BlendShapesManager.Instance.MorphTypeSelected("Lips");
                        AvatarCustomizationUIHandler.Instance.LoadCustomBlendShapePanel("Lips");
                        BlendShapesManager.Instance.TurnOnPoints("LipsMorph");
                        break;
                    }
                case "HeadMorph":
                    {
                        AvatarCustomizationManager.Instance.OnFrontSide();
                        UIHandler.Instance._footerCan.SetActive(false);
                        GameManager.Instance.mainCharacter.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                        SetCameraPosForFaceCustomization.instance.ChangeCameraToIsometric();
                        BlendShapesManager.Instance.MorphTypeSelected("Head");
                        AvatarCustomizationUIHandler.Instance.LoadCustomBlendShapePanel("Head");
                        BlendShapesManager.Instance.TurnOnPoints("HeadMorph");
                    }
                    break;
            }

            if (!itemAlreadySaved)
            {
                InventoryManager.instance.SaveStoreBtn.GetComponent<Button>().interactable = true;
                InventoryManager.instance.SaveStoreBtn.SetActive(true);
                InventoryManager.instance.SaveStoreBtn.GetComponent<Image>().color = new Color(0f, 0.5f, 1f, 0.8f);
                InventoryManager.instance.GreyRibbonImage.SetActive(false);
                InventoryManager.instance.WhiteRibbonImage.SetActive(true);
            }
            else
            {
                InventoryManager.instance.SaveStoreBtn.SetActive(true);
                InventoryManager.instance.SaveStoreBtn.GetComponent<Button>().interactable = false;
                InventoryManager.instance.SaveStoreBtn.GetComponent<Image>().color = Color.white;
                InventoryManager.instance.GreyRibbonImage.SetActive(true);
                InventoryManager.instance.WhiteRibbonImage.SetActive(false);
            }

        }
    }
}