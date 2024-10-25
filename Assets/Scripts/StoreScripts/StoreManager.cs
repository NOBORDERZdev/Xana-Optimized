using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;
using SuperStar.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DG.DemiLib;
//using HSVPicker;


public class StoreManager : MonoBehaviour
{
    [Header("Holds Api response")]
    public ResponseHolder apiResponseHolder;

    //public DownloadandRigClothes _DownloadRigClothes;
    public static StoreManager instance;
    [Header("Main Panels Store")]
    public GameObject StoreItemsPanel;
    //public GameObject CheckOutBuyItemPanel;
    public GameObject ShowSignUpPanel;
    //public GameObject LowCoinsPanel;
    //public GameObject ShopBuyCoinsPanel;
    public EnumClass.CategoryEnum CategoriesEnumVar;
    public Text textskin;

    public GameObject ItemsBtnPrefab;
    [FormerlySerializedAs("oval")] public GameObject GreyRibbonImage;
    public GameObject WhiteRibbonImage;
    public Color HighlightedColor;
    public Color NormalColor;
    [Header("Total Panels Cloths")]
    public GameObject MainPanelCloth;
    public GameObject[] ClothsPanel;
    public GameObject BtnsPanelCloth;
    public GameObject ClothBtnLine;
    public Text ClothBtnText;
    [Header("Total Panels Avatar")]
    public GameObject MainPanelAvatar;

    public GameObject[] AvatarPanel;
    public GameObject BtnsPanelAvatar;
    public GameObject AvatarBtnLine;
    public Text AvatarBtnText;

    [Header("Return Home Pop up")]
    public GameObject ReturnHomePopUp;
    [Header("Total Buying Btns")]
    public GameObject BuyStoreBtn;
    //open a panel for player name 
    public GameObject SaveStoreBtn;
    //save player data to server
    public GameObject saveButton;
    [Header("Total Texts money Display")]
    public Text BuyCountertxt;
    public Text TotalGameCoins;
    public List<ItemDetail> TotalBtnlist;
    public List<ItemDetail> CategorieslistHeads;
    public List<ItemDetail> CategorieslistFace;
    public List<ItemDetail> CategorieslistInner;
    public List<ItemDetail> CategorieslistOuter;
    public List<ItemDetail> CategorieslistAccesary;
    public List<ItemDetail> CategorieslistBottom;
    public List<ItemDetail> CategorieslistSocks;
    public List<ItemDetail> CategorieslistShoes;
    public List<ItemDetail> CategorieslistEyesColor;
    public List<ItemDetail> CategorieslistLipsColor;
    public List<ItemDetail> CategorieslistSkinToneColor;
    public List<ItemDetail> CategorieslistHairs;
    public List<ItemDetail> CategorieslistHairsColors;
    [Header("Categories Panel Cloths")]
    public Transform ParentOfBtnsForHeads;
    public Transform ParentOfBtnsForFace;
    public Transform ParentOfBtnsForInner;
    public Transform ParentOfBtnsForOuter;
    public Transform ParentOfBtnsForAccesary;
    public Transform ParentOfBtnsForBottom;
    public Transform ParentOfBtnsForSocks;
    public Transform ParentOfBtnsForShoes;
    [Header("Categories Panel Avatar")]
    public Transform ParentOfBtnsAvatarHairs;
    public Transform ParentOfBtnsAvatarFace;
    public Transform ParentOfBtnsAvatarEyeBrows;
    public Transform ParentOfBtnsAvatarEyeLashes;
    public Transform ParentOfBtnsAvatarEyes;
    public Transform ParentOfBtnsAvatarNose;
    public Transform ParentOfBtnsAvatarLips;
    public Transform ParentOfBtnsAvatarBody;
    public Transform ParentOfBtnsAvatarSkin;
    public Transform ParentOfBtnsAvatarMakeup;
    public Transform ParentOfBtnsAvatarAccessary;
    [Header("Categories Color Customizations")]
    public Transform ParentOfBtnsCustomHair;
    public Transform ParentOfBtnsCustomEyeBrows;
    public Transform ParentOfBtnsCustomFace;
    public Transform ParentOfBtnsCustomEyes;
    public Transform ParentOfBtnsCustomEyesPalette;
    public Transform ParentOfBtnsCustomLips;
    public Transform ParentOfBtnsCustomLipsPalette;
    public Transform ParentOfBtnsCustomSkin;

    private int headsDownlaodedCount, faceDownlaodedCount, innerDownlaodedCount, outerDownlaodedCount, accesaryDownlaodedCount, bottomDownlaodedCount, socksDownlaodedCount,
        shoesDownlaodedCount, hairDwonloadedCount, LipsColorDwonloadedCount, EyesColorDwonloadedCount, EyeBrowColorDwonloadedCount, HairColorDwonloadedCount, skinColorDwonloadedCount, eyeBrowDwonloadedCount,
        eyeBrowColorDwonloadedCount, eyeLashesDwonloadedCount, eyesDwonloadedCount, lipsDwonloadedCount;

    [Space(10f)]
    public GameObject colorCustomizationPrefabBtn;
    [Header("Color Customizations")]
    public bool colorMode = false;
    public GameObject colorBtn;
    public BodyColorCustomization bodyColorCustomization;
    //public CustomFakeStore fakeStore;
    public SliderColorPicker skinColorSlider;
    // Get Data FromJsonFiles
    [HideInInspector]
    public GetAllInfo JsonDataObj;

    // New APIS Integration //
    // APIS
    public string GetAllCategoriesAPI;
    public string GetAllSubCategoriesAPI;
    public string GetAllItems;

    private bool Clothdatabool;
    private int IndexofPanel;
    private int PreviousSelectionCount;


    // Containers
    GetAllInfoMainCategories ObjofMainCategory;
    string[] ArrayofMainCategories;
    public List<ItemsofSubCategories> SubCategoriesList;
    private bool CheckAPILoaded;
    List<TotaItemsClass> dataListOfItems = new List<TotaItemsClass>();

    public GameObject[] faceAvatarButton, eyeAvatarButton,
                  lipAvatarButton, eyeBrowsAvatarButton,
                   noseAvatarButton;

    public bool checkforSavebutton;  // its just for to check if item is downloaded or not
    public GameObject UndoBtn, RedoBtn, AvatarSaved, AvatarSavedGuest, AvatarUpdated;
    public GameObject Defaultreset, LastSavedreset, PanelResetDefault;
    // public GameObject ButtonFor_Preset;
    public GameObject StartPanel_PresetParentPanel, PresetArrayContent;
    // public GameObject backbutton_preset;
    public Transform contentList;

    public GameObject faceTapButton;
    public GameObject eyeBrowTapButton;
    public GameObject eyeTapButton;
    public GameObject noseTapButton;
    public GameObject lipTapButton;

    public Button hairColorButton;
    public Button eyeBrowsColorButton;

    public int panelIndex = 0;
    int buttonIndex = -1;
    public bool saveButtonPressed = false;
    [HideInInspector]
    public bool UndoClicked = false;
    [HideInInspector]
    public bool RedoClicked = false;

    private Image saveStoreBtnImage;
    public Button saveStoreBtnButton;
    public GameObject load;
    public GameObject loaderForItems;

    [Header("My Avatar Panel Prefab Refrence")]
    public Button myAvatarButton;
    //WaqasHamd
    //public ColorPicker skinColorPicker;
    public bool MultipleSave; // to enable/ disable multiple save 
    private GameObject childObject;
    public Button newAvatarPresetBtn;
    public CanvasScaler _CanvasScaler;
    public Action storeOpen;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        DisableColorPanels();
        checkforSavebutton = false;
    }
    [SerializeField]
    List<GridLayoutGroup> panelsLayoutGroups;

    void Start()
    {
        if (LoadPlayerAvatar.instance_loadplayer != null &&
    LoadPlayerAvatar.instance_loadplayer.saveButton != null)
        {
            load = LoadPlayerAvatar.instance_loadplayer.loader;
            saveButton = LoadPlayerAvatar.instance_loadplayer.saveButton.gameObject;
            saveStoreBtnImage = SaveStoreBtn.GetComponent<Image>();
            saveStoreBtnButton = SaveStoreBtn.GetComponent<Button>();
            CheckAPILoaded = false;
        }
        if (PlayerPrefs.GetInt("WalletLogin") != 1)
        {
            GetAllMainCategories();
            Clothdatabool = false;
            dataListOfItems = new List<TotaItemsClass>();
            PreviousSelectionCount = -1;
            StartCoroutine(WaitForInstance());
            if (AvatarSaved)
                AvatarSaved.SetActive(false);
            SetPresetValue();
        }
        if (XanaConstants.xanaConstants.screenType == XanaConstants.ScreenType.TabScreen)
        {
            for (int i = 0; i < panelsLayoutGroups.Count; i++)
            {
                panelsLayoutGroups[i].gameObject.AddComponent<AdjustGridLayoutCellSize>();
                panelsLayoutGroups[i].padding.left = 25;
                panelsLayoutGroups[i].padding.right = 25;
            }
            panelsLayoutGroups[panelsLayoutGroups.Count - 1].constraintCount = 4;
        }

        if (Defaultreset)
            Defaultreset.GetComponent<Button>().onClick.AddListener(delegate { Character_DefaultReset(true); });

        if (UndoBtn)
        {
            UndoBtn.GetComponent<Button>().onClick.AddListener(UndoStepBtn);
            UndoBtn.GetComponent<Button>().interactable = true;
        }
        if (RedoBtn)
        {
            RedoBtn.GetComponent<Button>().interactable = true;
            RedoBtn.GetComponent<Button>().onClick.AddListener(RedoStepBtn);
        }
        if (LastSavedreset)
        {
            LastSavedreset.GetComponent<Button>().onClick.AddListener(Character_ResettoLastSaved);
        }
    }
    public void skipAvatarSelection()
    {
        UserRegisterationManager.instance.usernamePanal.SetActive(true);
        _CanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
    }

    public void WalletLoggedinCall()
    {
        GetAllMainCategories();
        Clothdatabool = false;
        dataListOfItems = new List<TotaItemsClass>();
        PreviousSelectionCount = -1;
        CheckWhenUserLogin();
        if (AvatarSaved)
            AvatarSaved.SetActive(false);
        SetPresetValue();
    }
    // Using accessory panel as preset jsons
    void SetPresetValue()
    {
        //  prefabbutton_preset;
        ////  GameObject contentparent = ClothsPanel[4].GetComponent<ScrollRect>().content.gameObject;

        ////  for(int x=0;x<contentparent.transform.childCount;x++)
        //// {
        ////     contentparent.transform.GetChild(x).gameObject.name = "Preset" + (1+x).ToString();
        //    contentparent.transform.GetChild(x).gameObject.GetComponent<Image>().color = Color.black;
        //   contentparent.transform.GetChild(x).GetComponent<Button>().onClick.AddListener(ChangecharacterOnCLickFromserver);
        //// }

        //    GameObject button = (GameObject)Instantiate(ButtonFor_Preset);
        //     button.transform.parent = contentparent.transform;

        if (PlayerPrefs.GetString("PresetValue") != "")
            XanaConstants.xanaConstants.PresetValueString = PlayerPrefs.GetString("PresetValue");
    }


    public void CallDynamicLink()
    {
        StartCoroutine(waitAndDeeplink());
    }
    IEnumerator waitAndDeeplink()
    {
        yield return new WaitForSeconds(2);
        DynamicEventManager.deepLink?.Invoke("Come from store manager");
    }

    private void Update()
    {

        // Quick fix AKA ElFY
        //SaveBtn();

    }
    public void SaveBtn()
    {
        if (saveStoreBtnImage.color == Color.white)
            saveStoreBtnButton.interactable = false;
        else
            saveStoreBtnButton.interactable = true;
    }
    public void CheckWhenUserLogin()
    {
        saveStoreBtnButton.onClick.RemoveAllListeners();
        saveButton.GetComponent<Button>().onClick.RemoveAllListeners();
        newAvatarPresetBtn.onClick.RemoveAllListeners();
        if (PlayerPrefs.GetInt("IsLoggedIn") == 1)
        {
            if (MultipleSave)
            {
                if (AvatarSelfie.instance != null)
                    saveStoreBtnButton.onClick.AddListener(() => AvatarSelfie.instance.TakeScreenShootAndSaveData((IsSucess) => { }));
                if (LoadPlayerAvatar.instance_loadplayer != null)
                    saveStoreBtnButton.onClick.AddListener(() => LoadPlayerAvatar.instance_loadplayer.OpenPlayerNamePanel());

                if (AvatarSelfie.instance != null)
                    newAvatarPresetBtn.onClick.AddListener(() => AvatarSelfie.instance.TakeScreenShootAndSaveData((IsSucess) => { }));
                if (LoadPlayerAvatar.instance_loadplayer != null)
                    newAvatarPresetBtn.onClick.AddListener(() => LoadPlayerAvatar.instance_loadplayer.OpenPlayerNamePanel());

                saveButton.GetComponent<Button>().onClick.AddListener(OnSaveBtnClicked);

                ////saveStoreBtnButton.onClick.AddListener(() => AvatarSelfie.instance.TakeScreenShoot());
                //saveStoreBtnButton.onClick.AddListener(() => LoadPlayerAvatar.instance_loadplayer.OpenPlayerNamePanel());
                ////saveButton.GetComponent<Button>().onClick.AddListener(OnSaveBtnClicked);
                //newAvatarPresetBtn.onClick.AddListener(() => AvatarSelfie.instance.TakeScreenShootAndSaveData(false));
                ////newAvatarPresetBtn.onClick.AddListener(() => OnSaveBtnClicked());
                //// newAvatarPresetBtn.onClick.AddListener(()=> LoadPlayerAvatar.instance_loadplayer.ClosePlayerNamePanel());
                //LoadPlayerAvatar.instance_loadplayer.PlayerPanelSaveButton.onClick.AddListener(() => AvatarSelfie.instance.TakeScreenShootAndSaveData(true));
                ////  LoadPlayerAvatar.instance_loadplayer.PlayerPanelSaveButton.onClick.AddListener(OnSaveBtnClicked);
                ////newAvatarPresetBtn.onClick.AddListener(() => LoadPlayerAvatar.instance_loadplayer.OpenPlayerNamePanel());
            }
            else
            {
                saveStoreBtnButton.onClick.AddListener(OnSaveBtnClicked);
            }

        }
        else
        {
            saveStoreBtnButton.onClick.AddListener(OnSaveBtnClicked);
        }
    }


    IEnumerator WaitForInstance()
    {
        saveStoreBtnButton.onClick.RemoveAllListeners();
        saveButton.GetComponent<Button>().onClick.RemoveAllListeners();
        yield return new WaitForSeconds(.1f);
        //SaveStoreBtn.GetComponent<Button>().onClick.AddListener(OnSaveBtnClicked);
        if (PlayerPrefs.GetInt("IsLoggedIn") == 1 && MultipleSave)
        {
            if (AvatarSelfie.instance != null)
                saveStoreBtnButton.onClick.AddListener(() => AvatarSelfie.instance.TakeScreenShootAndSaveData((IsSucess) => { }));
            if (LoadPlayerAvatar.instance_loadplayer != null)
                saveStoreBtnButton.onClick.AddListener(() => LoadPlayerAvatar.instance_loadplayer.OpenPlayerNamePanel());
            saveButton.GetComponent<Button>().onClick.AddListener(OnSaveBtnClicked);
        }
        else
        {
            saveStoreBtnButton.onClick.AddListener(OnSaveBtnClicked);
        }

        //if (Defaultreset)
        //    Defaultreset.GetComponent<Button>().onClick.AddListener(delegate { Character_DefaultReset(true); });

        //if (UndoBtn)
        //{
        //    UndoBtn.GetComponent<Button>().onClick.AddListener(UndoStepBtn);
        //    UndoBtn.GetComponent<Button>().interactable = true;
        //}
        //if (RedoBtn)
        //{
        //    RedoBtn.GetComponent<Button>().interactable = true;
        //    RedoBtn.GetComponent<Button>().onClick.AddListener(RedoStepBtn);
        //}
        //if (LastSavedreset)
        //{
        //    LastSavedreset.GetComponent<Button>().onClick.AddListener(Character_ResettoLastSaved);
        //}
        // backbutton_preset.GetComponent<Button>().onClick.AddListener(BackTrackPreset);
    }
    void BackTrackPreset()
    {

        if (PlayerPrefs.GetInt("IsProcessComplete") == 0)

            UserRegisterationManager.instance.ShowWellComeCloseRetrack();
    }
    void Character_DefaultReset(bool clearData = true)
    {
        if (PlayerPrefs.GetInt("presetPanel") == 1)
            PlayerPrefs.SetInt("presetPanel", 0);

        PlayerPrefs.SetInt("Loaded", 0);
        //DefaultEnteriesforManican.instance.DefaultReset();

        ////Deleting Player data
        //LoadPlayerAvatar.instance_loadplayer.DeleteAvatarDataFromServer(ConstantsGod.AUTH_TOKEN, LoadPlayerAvatar.avatarId);
        ClearingLists(0);
        ClearingLists(1);
        ClearingLists(2);
        ClearingLists(7);
        if (clearData)
            ResetSaveFile();
        if (GameManager.Instance.mainCharacter.GetComponent<AvatarController>().wornEyewearable != null)
        {
            GameManager.Instance.mainCharacter.GetComponent<AvatarController>().UnStichItem("EyeWearable");
        }
        UndoSelection();
        XanaConstants.xanaConstants._lastClickedBtn = null;
        XanaConstants.xanaConstants._curretClickedBtn = null;
        if (GameManager.Instance) // reseting body type
        {
            GameManager.Instance.mainCharacter.GetComponent<AvatarController>().ResizeClothToBodyFat(GameManager.Instance.mainCharacter.gameObject, 0);
        }

        GameManager.Instance.mainCharacter.GetComponent<CharcterBodyParts>().DefaultTexture();
        GameManager.Instance.mainCharacter.GetComponent<AvatarController>().IntializeAvatar();

        //GameManager.Instance.mainCharacter.GetComponent<Equipment>().SaveDefaultValues();
        //GameManager.Instance.mainCharacter.GetComponent<Equipment>().UpdateStoreList();
        //Comented By Talha For Default cloth showing
        Default_LastSaved_PanelDisabler();
        //PlayerPrefs.SetString("PresetValue", "");
        XanaConstants.xanaConstants.PresetValueString = null;
        PresetData_Jsons.clickname = "";
        UpdateXanaConstants();
        //ItemDatabase.instance.GetComponent<SavaCharacterProperties>().SavePlayerProperties();
        UpdateStoreSelection(XanaConstants.xanaConstants.currentButtonIndex);
        //XanaConstants.xanaConstants._lastClickedBtn = null;

        if (EyesBlinking.instance)
            EyesBlinking.instance.StoreBlendShapeValues();          // Added by Ali Hamza
    }
    void Character_ResettoLastSaved()
    {
        UndoSelection();
        // PlayerPrefs.SetInt("Loaded", 0); 
        // apply last saved values from local

        if (PlayerPrefs.GetInt("presetPanel") == 1)
            PlayerPrefs.SetInt("presetPanel", 0);
        //GameManager.Instance.mainCharacter.GetComponent<AvatarController>().ResetForLastSaved();
        // DefaultEnteriesforManican.instance.ResetForPresets();
        //GameManager.Instance.mainCharacter.GetComponent<Equipment>().Start();
        GameManager.Instance.mainCharacter.GetComponent<CharcterBodyParts>().DefaultTexture();
        GameManager.Instance.mainCharacter.GetComponent<AvatarController>().IntializeAvatar();

        //On merging from Release getting this error
        //GameManager.Instance.mainCharacter.GetComponent<DefaultEnteriesforManican>().DefaultReset_HAck();
        //SavaCharacterProperties.instance.LoadMorphsfromFile();
        Default_LastSaved_PanelDisabler();


        GreyRibbonImage.SetActive(true);
       WhiteRibbonImage.SetActive(false);
        saveStoreBtnImage.color = Color.white;
        PresetData_Jsons test;
        if (FindObjectOfType<PresetData_Jsons>())
        {
            test = FindObjectOfType<PresetData_Jsons>();
            test.callit(); // = ""; // null;
        }// null;
         // DefaultEnteriesforManican.instance.LastSaved_Reset();
         // Default_LastSaved_PanelDisabler();
        XanaConstants.xanaConstants._lastClickedBtn = null;

        if (TempEnumVar == EnumClass.CategoryEnum.EyeBrowAvatar)
        {
            for (int i = 1; i < ParentOfBtnsAvatarEyeBrows.childCount; i++)
            {
                if (ParentOfBtnsAvatarEyeBrows.GetChild(i).GetComponent<ItemDetail>().id.ParseToInt() == SavaCharacterProperties.instance.characterController.eyeBrowId)
                    ParentOfBtnsAvatarEyeBrows.GetChild(i).GetComponent<Image>().enabled = true;
                else
                    ParentOfBtnsAvatarEyeBrows.GetChild(i).GetComponent<Image>().enabled = false;
            }
        }
        else if (TempEnumVar == EnumClass.CategoryEnum.EyeLashesAvatar)
        {
            for (int i = 0; i < ParentOfBtnsAvatarEyeLashes.childCount; i++)
            {
                if (ParentOfBtnsAvatarEyeLashes.GetChild(i).GetComponent<ItemDetail>().id.ParseToInt() == SavaCharacterProperties.instance.characterController.eyeLashesId)
                    ParentOfBtnsAvatarEyeLashes.GetChild(i).GetComponent<Image>().enabled = true;
                else
                    ParentOfBtnsAvatarEyeLashes.GetChild(i).GetComponent<Image>().enabled = false;
            }
        }

        UpdateXanaConstants();
        if (ParentOfBtnsCustomEyes.gameObject.activeInHierarchy || ParentOfBtnsCustomLips.gameObject.activeInHierarchy || ParentOfBtnsCustomSkin.gameObject.activeInHierarchy)
            UpdateColor(XanaConstants.xanaConstants.currentButtonIndex);
        else
            UpdateStoreSelection(XanaConstants.xanaConstants.currentButtonIndex);

        if (EyesBlinking.instance)
            EyesBlinking.instance.StoreBlendShapeValues();          // Added by Ali Hamza
    }




    public void DeletePreviousItems()
    {
        //Resources.UnloadUnusedAssets();
        //   Caching.ClearCache();

        if (ParentOfBtnsAvatarHairs.childCount >= 1) // hairs
        {
            print("~~~~~~~ ParentOfBtnsAvatarHairs" + ParentOfBtnsAvatarHairs.childCount);
            for (int i = ParentOfBtnsAvatarHairs.childCount - 1; i >= 1; i--)
            {
                AssetCache.Instance.RemoveFromMemory(ParentOfBtnsAvatarHairs.GetChild(i).GetComponent<ItemDetail>().iconLink, true);
                Destroy(ParentOfBtnsAvatarHairs.GetChild(i).gameObject);
                //Resources.UnloadUnusedAssets();
            }
        }
        // Eyebrow has Customization Icon Donot delect index 0
        if (ParentOfBtnsAvatarEyeBrows.childCount >= 2) // eyebrow
        {
            print("~~~~~~~ ParentOfBtnsAvatarEyeBrows" + ParentOfBtnsAvatarEyeBrows.childCount);
            for (int i = ParentOfBtnsAvatarEyeBrows.childCount - 1; i >= 2; i--)
            {
                AssetCache.Instance.RemoveFromMemory(ParentOfBtnsAvatarEyeBrows.GetChild(i).GetComponent<ItemDetail>().iconLink, true);
                Destroy(ParentOfBtnsAvatarEyeBrows.GetChild(i).gameObject);
                //Resources.UnloadUnusedAssets();
            }

        }
        if (ParentOfBtnsAvatarEyeLashes.childCount >= 1) // eyebrow
        {
            print("~~~~~~~ ParentOfBtnsAvatarEyeLashes" + ParentOfBtnsAvatarEyeLashes.childCount);
            for (int i = ParentOfBtnsAvatarEyeLashes.childCount - 1; i >= 0; i--)
            {
                AssetCache.Instance.RemoveFromMemory(ParentOfBtnsAvatarEyeLashes.GetChild(i).GetComponent<ItemDetail>().iconLink, true);
                Destroy(ParentOfBtnsAvatarEyeLashes.GetChild(i).gameObject);
                //Resources.UnloadUnusedAssets();
            }
        }
        //if (ParentOfBtnsAvatarEyes.childCount >= 1) // eyes
        //{
        //    print("~~~~~~~ ParentOfBtnsAvatarEyes" + ParentOfBtnsAvatarEyes.childCount);
        //    for (int i = ParentOfBtnsAvatarEyes.childCount - 1; i >= 0; i--)
        //    {
        //        Destroy(ParentOfBtnsAvatarEyes.GetChild(i).gameObject);
        //    }
        //}
        if (ParentOfBtnsAvatarSkin.childCount >= 1) // skin
        {
            print("~~~~~~~ ParentOfBtnsAvatarSkin" + ParentOfBtnsAvatarSkin.childCount);
            for (int i = ParentOfBtnsAvatarSkin.childCount - 1; i >= 0; i--)
            {
                Destroy(ParentOfBtnsAvatarSkin.GetChild(i).gameObject);
                //Resources.UnloadUnusedAssets();
            }
        }
        if (ParentOfBtnsForBottom.childCount >= 1) // bottom
        {
            print("~~~~~~~ ParentOfBtnsForBottom" + ParentOfBtnsForBottom.childCount);
            for (int i = ParentOfBtnsForBottom.childCount - 1; i >= 0; i--)
            {
                AssetCache.Instance.RemoveFromMemory(ParentOfBtnsForBottom.GetChild(i).GetComponent<ItemDetail>().iconLink, true);
                Destroy(ParentOfBtnsForBottom.GetChild(i).gameObject);
                //Resources.UnloadUnusedAssets();
            }
        }
        if (ParentOfBtnsForShoes.childCount >= 1) // Shoes
        {
            print("~~~~~~~ ParentOfBtnsForShoes" + ParentOfBtnsForShoes.childCount);
            for (int i = ParentOfBtnsForShoes.childCount - 1; i >= 0; i--)
            {
                AssetCache.Instance.RemoveFromMemory(ParentOfBtnsForShoes.GetChild(i).GetComponent<ItemDetail>().iconLink, true);
                Destroy(ParentOfBtnsForShoes.GetChild(i).gameObject);
                //Resources.UnloadUnusedAssets();
            }
        }
        if (ParentOfBtnsForOuter.childCount >= 1) // Outer
        {
            print("~~~~~~~ ParentOfBtnsForOuter" + ParentOfBtnsForOuter.childCount);
            for (int i = ParentOfBtnsForOuter.childCount - 1; i >= 0; i--)
            {
                AssetCache.Instance.RemoveFromMemory(ParentOfBtnsForOuter.GetChild(i).GetComponent<ItemDetail>().iconLink, true);
                Destroy(ParentOfBtnsForOuter.GetChild(i).gameObject);
                //Resources.UnloadUnusedAssets();
            }
        }

        headsDownlaodedCount = 0;
        faceDownlaodedCount = 0;
        innerDownlaodedCount = 0;
        outerDownlaodedCount = 0;
        accesaryDownlaodedCount = 0;
        bottomDownlaodedCount = 0;
        socksDownlaodedCount = 0;
        shoesDownlaodedCount = 0;
        hairDwonloadedCount = 0;
        LipsColorDwonloadedCount = 0;
        EyesColorDwonloadedCount = 0;
        EyeBrowColorDwonloadedCount = 0;
        HairColorDwonloadedCount = 0;
        skinColorDwonloadedCount = 0;
        eyeBrowDwonloadedCount = 0;
        eyeBrowColorDwonloadedCount = 0;
        eyeLashesDwonloadedCount = 0;
        eyesDwonloadedCount = 0;
        lipsDwonloadedCount = 0;
        if (LoadingHandler.Instance)
            LoadingHandler.Instance.storeLoadingScreen.SetActive(false);

    }

    void ClearingLists(int index)
    {
        //AssetBundle.UnloadAllAssetBundles(false);
        //Resources.UnloadUnusedAssets();

        ItemDatabase itemsData = ItemDatabase.instance;
        //Equipment equipment = GameManager.Instance.mainCharacter.GetComponent<Equipment>();
        itemsData.itemList[index].ItemName = "";
        //itemsData.itemList[index].ItemID =0;
        //itemsData.itemList[index].ItemLinkAndroid= "";
        //itemsData.itemList[index].ItemLinkIOS = "";

        //equipment.equippedItems[index].ItemName = "";
        //equipment.equippedItems[index].ItemID = 0;
        //equipment.equippedItems[index].ItemLinkAndroid = "";
        //equipment.equippedItems[index].ItemLinkIOS = "";
    }

    IEnumerator StoreSelection()
    {
        //AssetBundle.UnloadAllAssetBundles(false);
        //Resources.UnloadUnusedAssets();

        yield return new WaitForSeconds(0.5f);

        UpdateStoreSelection(XanaConstants.xanaConstants.currentButtonIndex);
    }
    void Default_LastSaved_PanelDisabler()
    {
        PanelResetDefault.SetActive(false);
    }

    public void OnSaveBtnClicked()
    {
        //AssetBundle.UnloadAllAssetBundles(false);
        //Resources.UnloadUnusedAssets();

        // print("ppp");
        if (ItemDatabase.instance.gameObject != null)
        {
            //  print("ppp+");
            if (PlayerPrefs.GetInt("presetPanel") == 1)
            {   // preset panel is enable so saving preset to account 
                PlayerPrefs.SetInt("presetPanel", 0);
            }
            //if (PresetData_Jsons.lastSelectedPreset)
            //{
            //    PlayerPrefs.SetString("PresetValue", PresetData_Jsons.lastSelectedPreset.name);
            //    XanaConstants.xanaConstants.PresetValueString = PlayerPrefs.GetString("PresetValue");
            //}
            PlayerPrefs.SetInt("Loaded", 1);
            if (PlayerPrefs.GetInt("IsLoggedIn") == 1)
            {
                if (MultipleSave)
                {
                    AvatarSaved.SetActive(true);
                }
                else
                {
                    AvatarSaved.SetActive(false);
                    AvatarSavedGuest.SetActive(true);
                }
            }
            else
            {
                if (!isSaveFromreturnHomePopUp)
                {
                    AvatarSavedGuest.SetActive(true);
                }
            }
            ItemDatabase.instance.GetComponent<SavaCharacterProperties>().SavePlayerProperties();
            //GameManager.Instance.mainCharacter.GetComponent<Equipment>().UpdateStoreList();
            saveButtonPressed = true;
            ResetMorphBooleanValues();

        }
    }
    /// <New APIS>
    IEnumerator WaitForAPICallCompleted(int m_GetIndex)
    {
        print("wait Until");
        yield return new WaitUntil(() => CheckAPILoaded == true);
        if (CheckAPILoaded)
        {
            print("wait Until completed");
            if (SubCategoriesList.Count > 0)
            {
                SubmitAllItemswithSpecificSubCategory(SubCategoriesList[m_GetIndex].id, false);
            }
        }
    }
    // **************************** Get Items by Sub categories ******************************//
    private string StringIndexofSubcategories(int _index)
    {
        var result = string.Join(",", _index.ToString());
        result = "[" + result + "]";
        return result;
    }
    [System.Serializable]
    public class ConvertSubCategoriesToJsonObj
    {
        public string subCategories;
        public int pageNumber;
        public int pageSize;
        public string order;
        public ConvertSubCategoriesToJsonObj CreateTOJSON(string jsonString, int _pageNumber, int _PageSize)
        {
            ConvertSubCategoriesToJsonObj myObj = new ConvertSubCategoriesToJsonObj();
            myObj.subCategories = jsonString;
            myObj.pageNumber = _pageNumber;
            myObj.pageSize = _PageSize;
            return myObj;
        }

        public ConvertSubCategoriesToJsonObj CreateTOJSON(string jsonString, int _pageNumber, int _PageSize, string _order)
        {
            ConvertSubCategoriesToJsonObj myObj = new ConvertSubCategoriesToJsonObj();
            myObj.subCategories = jsonString;
            myObj.pageNumber = _pageNumber;
            myObj.pageSize = _PageSize;
            myObj.order = _order;
            return myObj;
        }

    }
    public void SubmitAllItemswithSpecificSubCategory(int GetCategoryIndex, bool check)
    {
        //AssetBundle.UnloadAllAssetBundles(false);
        //Resources.UnloadUnusedAssets();

        bool Once;
        Once = check;
        if (PreviousSelectionCount != IndexofPanel)
        {
            PreviousSelectionCount = IndexofPanel;
            Once = true;
        }
        if (Once)
        {
            string result = StringIndexofSubcategories(GetCategoryIndex);
            ConvertSubCategoriesToJsonObj SubCatString = new ConvertSubCategoriesToJsonObj();
            //string bodyJson = JsonUtility.ToJson(SubCatString.CreateTOJSON(result, 1, 41, "asc"));
            string bodyJson = JsonUtility.ToJson(SubCatString.CreateTOJSON(result, 1, 200, "asc")); // Increase item Waqas Ahmad
            if (hitAllItemAPICorountine != null)
                StopCoroutine(hitAllItemAPICorountine);
            hitAllItemAPICorountine = StartCoroutine(HitALLItemsAPI(ConstantsGod.API_BASEURL + ConstantsGod.GETALLSTOREITEMS, bodyJson));
        }
    }
    public bool loadingItems = false;
    Coroutine itemLoading, hitAllItemAPICorountine;
    IEnumerator HitALLItemsAPI(string url, string Jsondata)
    {
        if (apiResponseHolder.CheckResponse(url + Jsondata))
        {
            GetItemInfoNewAPI JsonDataObj1 = new GetItemInfoNewAPI();
            JsonDataObj1 = JsonUtility.FromJson<GetItemInfoNewAPI>(apiResponseHolder.GetResponse(url + Jsondata));
            dataListOfItems.Clear();
            dataListOfItems = JsonDataObj1.data[0].items;
            PutDataInOurAPPNewAPI();
            yield break;
        }
        Debug.LogError("HitALLItemsAPI");
        if (LoadingHandler.Instance)
            LoadingHandler.Instance.storeLoadingScreen.SetActive(true);
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(Jsondata);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        if (UserRegisterationManager.instance.LoggedIn)
        {
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        }
        else
        {
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        }
        request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        GetItemInfoNewAPI JsonDataObj = new GetItemInfoNewAPI();
        JsonDataObj = JsonUtility.FromJson<GetItemInfoNewAPI>(request.downloadHandler.text);

        //AssetBundle.UnloadAllAssetBundles(false);
        //Resources.UnloadUnusedAssets();
        if (!request.isHttpError && !request.isNetworkError)
        {
            if (request.error == null)
            {
                if (JsonDataObj.success == true)
                {
                    dataListOfItems.Clear();

                    dataListOfItems = JsonDataObj.data[0].items;
                    PutDataInOurAPPNewAPI();
                    apiResponseHolder.AddReponse(url + Jsondata, request.downloadHandler.text);
                    if (LoadingHandler.Instance)
                        LoadingHandler.Instance.storeLoadingScreen.SetActive(false);
                }
            }
        }
        else
        {
            if (request.isNetworkError)
            {
                print("Network Error");
            }
            else
            {
                if (request.error != null)
                {
                    if (JsonDataObj.success == false)
                    {
                        print("Hey success false " + JsonDataObj.msg);
                    }
                }
            }
        }
        request.Dispose();
    }

    [System.Serializable]
    public class GetItemInfoNewAPI
    {
        public bool success;
        public List<DataList> data;
        public string msg;
    }
    [System.Serializable]
    public class DataList
    {
        public int id;
        public int categoryId;
        public string name;
        public string createdAt;
        public string updatedAt;
        public List<TotaItemsClass> items;
    }

    [System.Serializable]
    public class TotaItemsClass
    {
        public int id;
        public string assetLinkAndroid;
        public string assetLinkIos;
        public string assetLinkWindows;
        public string iconLink;
        public int categoryId;
        public int subCategoryId;
        public string name;
        public bool isPaid;
        public string price;
        public bool isPurchased;
        public bool isFavourite;
        public bool isOccupied;
        public bool isDeleted;
        public string createdBy;
        public string createdAt;
        public string updatedAt;
        public string[] itemTags;
    }
    // **************************** Get Items by Sub categories ENDSSSSS ******************************//

    // **************************************************************************************************//
    ////////////////////////// <MAIN Category Started Here> ///////////////////////////////////////////////
    public void GetAllMainCategories()
    {
        StartCoroutine(HitAllMainCategoriesAPI(ConstantsGod.API_BASEURL + ConstantsGod.GETALLSTOREITEMCATEGORY, ""));
    }
    IEnumerator HitAllMainCategoriesAPI(string url, string Jsondata)
    {
        while (ConstantsGod.AUTH_TOKEN.Equals("AUTH_TOKEN"))
        {
            yield return new WaitForSecondsRealtime(1f);
        }

        if (apiResponseHolder.CheckResponse(url + Jsondata))
        {
            string res = apiResponseHolder.GetResponse(url + Jsondata);
            ObjofMainCategory = GetAllDataNewAPI(res);
            SaveAllMainCategoriesToArray();
            yield break;
        }

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            request.SendWebRequest();
            while (!request.isDone)
            {
                yield return null;
            }
            ObjofMainCategory = GetAllDataNewAPI(request.downloadHandler.text);
            //Debug.LogError(request.downloadHandler.text);
            if (!request.isHttpError && !request.isNetworkError)
            {
                if (request.error == null)
                {
                    if (ObjofMainCategory.success == true)
                    {
                        SaveAllMainCategoriesToArray();
                        apiResponseHolder.AddReponse(url + Jsondata, request.downloadHandler.text);
                        //Debug.LogError(request.downloadHandler.text);
                    }
                }
            }
            else
            {
                if (request.isNetworkError)
                {
                    print("Network Error");
                }
                else
                {
                    if (request.error != null)
                    {
                        if (ObjofMainCategory.success == false)
                        {
                            print("Hey success false " + ObjofMainCategory.msg);
                        }
                    }
                }
            }
            request.Dispose();
        }
    }
    public void SaveAllMainCategoriesToArray()
    {
        List<string> purchaseItemsIDs = new List<string>();
        for (int i = 0; i < ObjofMainCategory.data.Count; i++)
        {
            purchaseItemsIDs.Add(ObjofMainCategory.data[i].id);
        }
        ArrayofMainCategories = purchaseItemsIDs.ToArray();
        //AssetBundle.UnloadAllAssetBundles(false);
        //Resources.UnloadUnusedAssets();

        GetAllSubCategories();
    }

    public GetAllInfoMainCategories GetAllDataNewAPI(string m_JsonData)
    {
        GetAllInfoMainCategories JsonDataObj = new GetAllInfoMainCategories();
        JsonDataObj = JsonUtility.FromJson<GetAllInfoMainCategories>(m_JsonData);
        return JsonDataObj;
    }
    [System.Serializable]
    public class GetAllInfoMainCategories
    {
        public bool success;
        public List<ItemsParentsNewAPI> data;
        public string msg;
    }
    [System.Serializable]
    public class ItemsParentsNewAPI
    {
        public string id;
        public string name;
        public string createdAt;
        public string updatedAt;
    }
    ////////////////////////// <MAIN Category ENDS here> ///////////////////////////////////////////////

    ///////////////////////// ************************** //////////////////////////////////////////////
    ////////////////////////// <SUB Category STARTS here> ///////////////////////////////////////////////
    private string AccessIndexOfSpecificCategory()
    {
        var result = string.Join(",", ArrayofMainCategories);
        result = "[" + result + "]";
        return result;
    }
    [System.Serializable]
    public class ConvertMainCat_Index_ToJson
    {
        public string categories;
        public ConvertMainCat_Index_ToJson CreateTOJSON(string jsonString)
        {
            ConvertMainCat_Index_ToJson myObj = new ConvertMainCat_Index_ToJson();
            myObj.categories = jsonString;
            return myObj;
        }
    }
    public void GetAllSubCategories()
    {
        string result = AccessIndexOfSpecificCategory();
        ConvertMainCat_Index_ToJson MainCatString = new ConvertMainCat_Index_ToJson();
        string bodyJson = JsonUtility.ToJson(MainCatString.CreateTOJSON(result));
        StartCoroutine(HitSUBCategoriesAPI(ConstantsGod.API_BASEURL + ConstantsGod.GETALLSTOREITEMSUBCATEGORY, bodyJson));
    }

    IEnumerator HitSUBCategoriesAPI(string url, string Jsondata)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(Jsondata);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        GetAllInfoSUBOFCategories JsonDataObj = new GetAllInfoSUBOFCategories();
        JsonDataObj = GetDataofSUBCategories(request.downloadHandler.text);
        //AssetBundle.UnloadAllAssetBundles(false);
        //Resources.UnloadUnusedAssets();
        //Debug.LogError("all sub categories :- " + request.downloadHandler.text);
        if (!request.isHttpError && !request.isNetworkError)
        {
            if (request.error == null)
            {
                if (JsonDataObj.success == true)
                {
                    SubCategoriesList = JsonDataObj.data;
                    CheckAPILoaded = true;
                }
            }
        }
        else
        {
            if (request.isNetworkError)
            {
                CheckAPILoaded = true;
            }
            else
            {
                if (request.error != null)
                {
                    if (JsonDataObj.success == false)
                    {
                        CheckAPILoaded = true;
                    }
                }
            }
        }
        request.Dispose();
    }
    public GetAllInfoSUBOFCategories GetDataofSUBCategories(string m_JsonData)
    {
        GetAllInfoSUBOFCategories JsonDataObj = new GetAllInfoSUBOFCategories();
        JsonDataObj = JsonUtility.FromJson<GetAllInfoSUBOFCategories>(m_JsonData);
        return JsonDataObj;
    }

    [System.Serializable]
    public class GetAllInfoSUBOFCategories
    {
        public bool success;
        public List<ItemsofSubCategories> data;
        public string msg;
    }
    [System.Serializable]
    public class ItemsofSubCategories
    {
        public int id;
        public int categoryId;
        public string name;
        public string createdAt;
        public string updatedAt;
    }
    //***************************** Get All Sub Categories END here **************************//

    /// </END APIS>

    public void GetDataofGuestUser()
    {
        BtnsPanelAvatar.GetComponent<SubBottons>().ClothBool = false;
        BtnsPanelAvatar.GetComponent<SubBottons>().AvatarBool = true;
        // When Comming form home then set last panel to -1
        PreviousSelectionCount = -1;
        XanaConstants.xanaConstants.currentButtonIndex = 0;
        BtnsPanelAvatar.GetComponent<SubBottons>().ClickBtnFtn(0);
        ////Debug.Log("Store hair data call====");
        SelectPanel(1);
        PlayerPrefs.SetInt("TotalCoins", 0);
        //UpdateUserCoins();
        if (!GameManager.Instance.OnceGuestBool)
        {
            RefreshDefault();
            GameManager.Instance.OnceGuestBool = true;
        }
        BuyCountertxt.text = "0";
    }
    public void GetDataAfterLogin()
    {
        // When Comming form home then set last panel to -1
        PreviousSelectionCount = -1;
        XanaConstants.xanaConstants.currentButtonIndex = 0;

        if (!GameManager.Instance.BottomAvatarButtonBool)
        {
            //BtnsPanelCloth.GetComponent<SubBottons>().ClickBtnFtn(0);
            //BtnsPanelAvatar.GetComponent<SubBottons>().ClickBtnFtn(0);
            SelectPanel(1);
        }
        else
        {
            //BtnsPanelCloth.GetComponent<SubBottons>().ClickBtnFtn(3);
            SelectPanel(0);
        }
        SubmitUserDetailAPI();
        if (!GameManager.Instance.OnceLoginBool)
        {
            RefreshDefault();
            GameManager.Instance.OnceLoginBool = true;
        }
        BuyCountertxt.text = "0";
    }
    public void SignUpAndLoginPanel(int TakeString)
    {
        //Debug.Log("<color=red> OpenPanelIndex:::" + TakeString + "</color>");
        switch (TakeString)
        {
            case 0:
                {
                    StoreItemsPanel.SetActive(false);
                    ShowSignUpPanel.SetActive(false);
                    GameManager.Instance.BGPlane.SetActive(false);
                    UIManager.Instance.HomePage.SetActive(true);
                    UserRegisterationManager.instance.OpenUIPanal(6);
                    break;
                }
            case 1:
                {
                    StoreItemsPanel.SetActive(false);
                    ShowSignUpPanel.SetActive(false);
                    GameManager.Instance.BGPlane.SetActive(false);
                    UIManager.Instance.HomePage.SetActive(true);
                    UserRegisterationManager.instance.OpenUIPanal(1);
                    break;
                }
            case 2:
                {
                    OpenMainPanel("StoreItemsPanel");
                    UndoSelection();
                    GetDataofGuestUser();
                    GameManager.Instance.ShadowPlane.GetComponent<Renderer>().material.SetColor("_Color", new Color(1f, 1f, 1f, 0.3137f));
                    break;
                }
            case 3:
                {
                    OpenMainPanel("StoreItemsPanel");

                    GetDataAfterLogin();
                    if (PlayerPrefs.GetInt("IsLoggedIn") == 1 && MultipleSave)
                    {
                        myAvatarButton.gameObject.SetActive(true);
                    }
                    GameManager.Instance.BottomAvatarButtonBool = false;
                    break;
                }
        }
    }
    public void OpenMainPanel(string TakePanel)
    {
        StoreItemsPanel.SetActive(false);
        //CheckOutBuyItemPanel.SetActive(false);
        ShowSignUpPanel.SetActive(false);
        //LowCoinsPanel.SetActive(false);
        //ShopBuyCoinsPanel.SetActive(false);
        switch (TakePanel)
        {
            case "StoreItemsPanel":
                {
                    StoreItemsPanel.SetActive(true);

                    break;
                }
            case "CheckOutBuyItemPanel":
                {
                    //CheckOutBuyItemPanel.SetActive(true);
                    break;
                }
            case "ShowSignUpPanel":
                {
                    StoreItemsPanel.SetActive(true);
                    ShowSignUpPanel.SetActive(true);
                    break;
                }
            case "LowCoinsPanel":
                {
                    //CheckOutBuyItemPanel.SetActive(true);
                    //LowCoinsPanel.SetActive(true);
                    break;
                }
            case "ShopBuyCoinsPanel":
                {
                    //ShopBuyCoinsPanel.SetActive(true);
                    break;
                }
        }

        Invoke("DelayAction", 0.2f);
    }

    void DelayAction()
    {
        if (storeOpen != null)                      // AR changes store open event
            storeOpen.Invoke();                     // call store open event
    }

    //private void OnDisable()
    //{
    //    Resources.UnloadUnusedAssets();
    //}

    //private void OnDestroy()
    //{
    //    Resources.UnloadUnusedAssets();
    //}
    public void BackToHomeFromCharCustomization()
    {

        // if user is getting back fromaccessory panel/preset panel
        if (PlayerPrefs.GetInt("presetPanel") == 1)
        {

            //  if (GameManager.Instance.UserStatus_)
            PlayerPrefs.SetInt("presetPanel", 0);  // was loggedin as account 
                                                   //  else
                                                   //     PlayerPrefs.SetInt("IsLoggedIn", 0);  // was as a guest
                                                   //SavaCharacterProperties.instance.LoadMorphsfromFile();

        }
        //PresetData_Jsons.lastSelectedPresetName = null;
        //XanaConstants.xanaConstants.PresetValueString = PlayerPrefs.GetString("PresetValue");

        GameManager.Instance.mainCharacter.GetComponent<Animator>().SetBool("Customization", false);


        GreyRibbonImage.SetActive(true);
        WhiteRibbonImage.SetActive(false);
        SaveStoreBtn.GetComponent<Image>().color = Color.white;
        GameManager.Instance.mainCharacter.GetComponent<AvatarController>().IntializeAvatar();
        saveButtonPressed = true;
        CharacterCustomizationUIManager.Instance.LoadMyClothCustomizationPanel();
        GameManager.Instance.ShadowPlane.GetComponent<Renderer>().material.SetColor("_Color", new Color(1f, 1f, 1f, 0.7843f));

        //SavaCharacterProperties.instance.LoadMorphsfromFile();
        if (ItemDatabase.instance != null)
        {
            ItemDatabase.instance.RevertSavedCloths();

            UpdateXanaConstants();
            SavaCharacterProperties.instance.AssignCustomSlidersData();
            SavaCharacterProperties.instance.AssignSavedPresets();
            GameManager.Instance.BlendShapeObj.DismissPoints();

            GameManager.Instance.BackFromStoreofCharacterCustom();
            MainPanelCloth.SetActive(false);
            StoreItemsPanel.SetActive(false);
            UndoSelection();
            //UndoRedo.undoRedo.undoRedoList.DestroyActionWithParameters(UndoRedo.undoRedo.undoRedoList);
            AR_UndoRedo.obj.DestroyList();
            DeletePreviousItems();

        }

        //Transform parentEyeBrowAvatar = ParentOfBtnsAvatarEyeBrows;                 // AH Working
        //if (parentEyeBrowAvatar.childCount > 1)
        //{
        //    for (int i = 1; i < parentEyeBrowAvatar.childCount; i++)
        //        parentEyeBrowAvatar.GetChild(i).GetComponent<Image>().enabled = false;
        //}
        //Transform parentEyelashesAvatar = ParentOfBtnsAvatarEyeLashes;                 // AH Working
        //if (parentEyelashesAvatar.childCount > 1)
        //{
        //    for (int i = 0; i < parentEyelashesAvatar.childCount; i++)
        //        parentEyelashesAvatar.GetChild(i).GetComponent<Image>().enabled = false;
        //}

        //DeletePreviousItems();

    }

    public void OnClickRetunHomePopUpBackButton()
    {
        ReturnHomePopUp.SetActive(false);
    }
    public void OnClickBackButton()
    {
        //GameManager.Instance.mainCharacter.GetComponent<FaceIK>().ikActive= true;
        // GameManager.Instance.ActorManager.IdlePlayerAvatorForMenu(false);
        //  GameManager.Instance.userAnimationPostFeature.GetComponent<UserPostFeature>().ActivatePostButtbleHome(true);

        eyeBrowsColorButton.gameObject.SetActive(false);
        hairColorButton.gameObject.SetActive(false);
        UIManager.Instance.ShowFooter(true);
        if (saveStoreBtnButton.interactable == true)
            ReturnHomePopUp.SetActive(true);
        else
            OnClickHomeButton();

    }

    public void OnClickHomeButton()
    {
        //  GameManager.Instance.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(false);

        isSaveFromreturnHomePopUp = false;
        ReturnHomePopUp.SetActive(false);
        AvatarUpdated.SetActive(false);
        BackToHomeFromCharCustomization();
    }
    public bool isSaveFromreturnHomePopUp;
    public void OnClickSaveAvatarButton()
    {
        isSaveFromreturnHomePopUp = true;
        ReturnHomePopUp.GetComponent<ReturnHomeLoader>().saveloader.SetActive(true);
        ReturnHomePopUp.GetComponent<ReturnHomeLoader>().saveButton.enabled = false;
        ReturnHomePopUp.GetComponent<ReturnHomeLoader>().closeButton.enabled = false;
        ReturnHomePopUp.GetComponent<ReturnHomeLoader>().homeButton.enabled = false;
        if (AvatarSelfie.instance != null) //this will take screenshot of character and automatically save avatar to server.
            AvatarSelfie.instance.TakeScreenShootAndSaveData((IsSucess) =>
            {
                if (IsSucess)
                    OnSaveBtnClicked();
                else
                    OnSaveBtnClicked(); //save avatar without thumbnail images this is for guest user.
                ReturnHomePopUp.GetComponent<ReturnHomeLoader>().saveloader.SetActive(false);
                ReturnHomePopUp.GetComponent<ReturnHomeLoader>().saveButton.enabled = true;
                ReturnHomePopUp.GetComponent<ReturnHomeLoader>().closeButton.enabled = true;
                ReturnHomePopUp.GetComponent<ReturnHomeLoader>().homeButton.enabled = true;
                ReturnHomePopUp.SetActive(false);
            });
        //commented invoke to shorten save flow as per nakamoto suggestion 
        //saveStoreBtnButton.onClick.Invoke();
        //LoadPlayerAvatar.instance_loadplayer.UpdateExistingUserData();

    }

    public void SelectPanel(int TakeIndex)
    {
        ////Debug.Log("<color=red> Panel Index:" + TakeIndex + "</color>");
        panelIndex = TakeIndex;

        //  StoreManager.instance.DeletePreviousItems();
        //Resources.UnloadUnusedAssets();

        if (TakeIndex == 0)
        {
            ////Debug.LogError("<color=red> Panel Index:" + TakeIndex + "</color>");
            //Resources.UnloadUnusedAssets();
            // CLoth
            buttonIndex = 3;
            XanaConstants.xanaConstants.currentButtonIndex = buttonIndex;
            MainPanelCloth.SetActive(true);
            MainPanelAvatar.SetActive(false);
            //OpenClothContainerPanel(0);
            BtnsPanelCloth.GetComponent<SubBottons>().ClickBtnFtn(3);
            ClothBtnLine.SetActive(true);
            AvatarBtnLine.SetActive(false);
            ClothBtnText.color = HighlightedColor;
            AvatarBtnText.color = NormalColor;
            UpdateStoreSelection(3);
        }
        else
        {
            ////Debug.LogError("<color=red> Panel Index:" + TakeIndex + "</color>");
            buttonIndex = 0;
            XanaConstants.xanaConstants.currentButtonIndex = buttonIndex;
            MainPanelCloth.SetActive(false);
            MainPanelAvatar.SetActive(true);
            //OpenAvatarContainerPanel(0);
            ////Debug.Log("Undo Redo Call the btn functionality");
            BtnsPanelAvatar.GetComponent<SubBottons>().ClickBtnFtn(0);
            ClothBtnLine.SetActive(false);
            AvatarBtnLine.SetActive(true);
            ClothBtnText.color = NormalColor;
            AvatarBtnText.color = HighlightedColor;
            UpdateStoreSelection(0);
        }

        if (PlayerPrefs.GetInt("presetPanel") == 1)
        {
            PlayerPrefs.SetInt("presetPanel", 0);  // was loggedin as account 

            GreyRibbonImage.SetActive(true);
            WhiteRibbonImage.SetActive(false);
            SaveStoreBtn.GetComponent<Image>().color = Color.white;

            SavaCharacterProperties.instance.LoadMorphsfromFile();
        }

        DisableColorPanels();
    }


    //public void UpdateUserCoins()
    //{
    //    string totalCoins = PlayerPrefs.GetInt("TotalCoins").ToString();
    //    double coins = Double.Parse(totalCoins);
    //    totalCoins = String.Format("{0:n0}", coins);
    //    //TotalGameCoins.text = totalCoins;
    //    CreditShopManager.instance.TotalCoins.text = totalCoins;
    //}
    // Update is called once per frame

    public void OpenClothContainerPanel(int m_GetIndex)
    {
        buttonIndex = m_GetIndex;
        Clothdatabool = true;
        IndexofPanel = m_GetIndex;
        for (int i = 0; i < ClothsPanel.Length; i++)
        {
            if (m_GetIndex != i)
                ClothsPanel[i].SetActive(false);
        }
        ClothsPanel[m_GetIndex].SetActive(true);
        if (m_GetIndex == 0 || m_GetIndex == 1 || m_GetIndex == 2 /*|| m_GetIndex == 3*/ || m_GetIndex == 4 /*|| m_GetIndex == 5*/ || m_GetIndex == 6 /*|| m_GetIndex == 9*/) //its a preset do nothing
        {
            // When Preset click than update the panel index
            PreviousSelectionCount = IndexofPanel;
            return;
        }
        if (CheckAPILoaded)
        {
            if (SubCategoriesList.Count > 0)
            {
                SubmitAllItemswithSpecificSubCategory(SubCategoriesList[m_GetIndex].id, false);
            }
        }
        else
        {
            StartCoroutine(WaitForAPICallCompleted(m_GetIndex));
        }
    }
    public void OpenAvatarContainerPanel(int m_GetIndex)
    {

        buttonIndex = m_GetIndex;
        Clothdatabool = false;
        IndexofPanel = m_GetIndex + 8; //16


        for (int i = 0; i < AvatarPanel.Length; i++)
        {
            AvatarPanel[i].SetActive(false);
        }
        AvatarPanel[m_GetIndex].SetActive(true);
        CheckColorProperty(m_GetIndex);
        if (m_GetIndex == 10 /*|| m_GetIndex == 8 EyeLashes*/|| m_GetIndex == 9) //its a preset do nothing
        {
            // When Preset click than update the panel index
            PreviousSelectionCount = IndexofPanel;
            return;
        }
        if (CheckAPILoaded)
        {
            if (SubCategoriesList.Count > 0)
            {
                // Debug.LogError("second time :- " + m_GetIndex);
                //print(SubCategoriesList[m_GetIndex + 8].id);
                SubmitAllItemswithSpecificSubCategory(SubCategoriesList[m_GetIndex + 8].id, false);
            }
        }
        else
        {
            // Debug.LogError("second time :- " + m_GetIndex);
            StartCoroutine(WaitForAPICallCompleted(m_GetIndex));
        }
    }

    public bool CheckColorPanelEnabled(int num)             // AR custom function check color panel enable or not
    {
        switch (num)
        {
            case 0:
                return ParentOfBtnsCustomHair.gameObject.activeSelf ? true : false;
                break;
            case 2:
                return ParentOfBtnsCustomEyeBrows.gameObject.activeSelf ? true : false;
                break;
            case 3:
                return ParentOfBtnsCustomEyesPalette.gameObject.activeSelf ? true : false;
                break;
            case 5:
                return ParentOfBtnsCustomLipsPalette.gameObject.activeSelf ? true : false;
                break;
            default:
                return false;
                break;
        }
    }

    public void OpenColorPanel(int index)
    {
        //Debug.Log("<color=blue> Open Color Panel Index: " + index + "</color>");
        if (index == 0)
        {
            ParentOfBtnsCustomHair.gameObject.SetActive(true);
            ParentOfBtnsAvatarHairs.gameObject.SetActive(false);
            SetContentOnScroll(AvatarPanel[0], (RectTransform)ParentOfBtnsCustomHair);
        }
        else if (index == 2)
        {
            ParentOfBtnsCustomEyeBrows.gameObject.SetActive(true);
            ParentOfBtnsAvatarEyeBrows.gameObject.SetActive(false);
            SetContentOnScroll(AvatarPanel[2], (RectTransform)ParentOfBtnsCustomEyeBrows);
        }
        else if (index == 3)
        {
            ParentOfBtnsCustomEyesPalette.gameObject.SetActive(true);
            ParentOfBtnsAvatarEyes.gameObject.SetActive(false);
            SetContentOnScroll(AvatarPanel[3], (RectTransform)ParentOfBtnsCustomEyesPalette);
        }
        else if (index == 5)
        {
            //Debug.Log("Open color palette");
            ParentOfBtnsCustomLipsPalette.gameObject.SetActive(true);
            ParentOfBtnsAvatarLips.gameObject.SetActive(false);
            SetContentOnScroll(AvatarPanel[5], (RectTransform)ParentOfBtnsCustomLipsPalette);
        }
    }

    bool tempBool;
    public bool CloseColorPanel(int index)
    {
        if (index == 0)
        {
            //if (ParentOfBtnsCustomHair.gameObject.activeInHierarchy)
            //    tempBool = true;
            ParentOfBtnsCustomHair.gameObject.SetActive(false);
            ParentOfBtnsAvatarHairs.gameObject.SetActive(true);
            SetContentOnScroll(AvatarPanel[0], (RectTransform)ParentOfBtnsAvatarHairs);
            return tempBool;
        }
        else if (index == 2)
        {
            //if (ParentOfBtnsCustomEyeBrows.gameObject.activeInHierarchy)
            //    tempBool = true;
            ParentOfBtnsCustomEyeBrows.gameObject.SetActive(false);
            ParentOfBtnsAvatarEyeBrows.gameObject.SetActive(true);
            SetContentOnScroll(AvatarPanel[2], (RectTransform)ParentOfBtnsAvatarEyeBrows);
            return tempBool;
        }
        else if (index == 3)
        {
            //if (ParentOfBtnsCustomEyesPalette.gameObject.activeInHierarchy)
            //    tempBool = true;
            ParentOfBtnsCustomEyesPalette.gameObject.SetActive(false);
            ParentOfBtnsCustomEyes.gameObject.SetActive(false);
            ParentOfBtnsAvatarEyes.gameObject.SetActive(true);
            SetContentOnScroll(AvatarPanel[3], (RectTransform)ParentOfBtnsAvatarEyes);
            return tempBool;
        }
        else if (index == 5)
        {
            //if (ParentOfBtnsCustomLipsPalette.gameObject.activeInHierarchy)
            //    tempBool = true;
            ParentOfBtnsCustomLipsPalette.gameObject.SetActive(false);
            ParentOfBtnsCustomLips.gameObject.SetActive(false);
            ParentOfBtnsAvatarLips.gameObject.SetActive(true);
            SetContentOnScroll(AvatarPanel[5], (RectTransform)ParentOfBtnsAvatarLips);
            return tempBool;
        }
        return tempBool;
    }
    void CheckColorProperty(int _index)
    {
        if (_index == 3 || _index == 5 /*|| _index == 8*/)
        {
            SwitchColorMode(_index);
            colorBtn.SetActive(true);
            colorBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            colorBtn.GetComponent<Button>().onClick.AddListener(() => OnColorButtonClicked(_index));

        }
        else if (_index == 0 || _index == 2)
        {
            SwitchColorMode(_index);
        }
        else
        {
            colorBtn.SetActive(false);
        }
    }

    void SwitchColorMode(int index)
    {
        //AssetBundle.UnloadAllAssetBundles(false);
        //Resources.UnloadUnusedAssets();

        ////Debug.Log("ColorBtn : " + index);
        ActivePanelCallStack.obj.UpdatePanelStatus(index, false);    // AR changes
        textskin.enabled = false;
        colorBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        colorBtn.GetComponent<Button>().onClick.AddListener(() => OnColorButtonClicked(index));

        switch (index)
        {
            case 0:
                ParentOfBtnsAvatarHairs.gameObject.SetActive(true);
                ParentOfBtnsCustomHair.gameObject.SetActive(false);

                SetContentOnScroll(AvatarPanel[0], (RectTransform)ParentOfBtnsAvatarHairs);
                break;
            case 1:
                ParentOfBtnsAvatarFace.gameObject.SetActive(true);
                ParentOfBtnsCustomFace.gameObject.SetActive(false);

                SetContentOnScroll(AvatarPanel[1], (RectTransform)ParentOfBtnsAvatarFace);
                break;
            case 2:
                ParentOfBtnsAvatarEyeBrows.gameObject.SetActive(true);
                ParentOfBtnsCustomEyeBrows.gameObject.SetActive(false);

                SetContentOnScroll(AvatarPanel[2], (RectTransform)ParentOfBtnsAvatarEyeBrows);
                break;
            case 3:
                ParentOfBtnsAvatarEyes.gameObject.SetActive(true);
                ParentOfBtnsCustomEyes.gameObject.SetActive(false);
                ParentOfBtnsCustomEyesPalette.gameObject.SetActive(false);
                SetContentOnScroll(AvatarPanel[3], (RectTransform)ParentOfBtnsAvatarEyes);
                break;
            case 5:
                ParentOfBtnsAvatarLips.gameObject.SetActive(true);
                ParentOfBtnsCustomLips.gameObject.SetActive(false);
                ParentOfBtnsCustomLipsPalette.gameObject.SetActive(false);
                SetContentOnScroll(AvatarPanel[5], (RectTransform)ParentOfBtnsAvatarLips);
                break;
            case 6:
                ParentOfBtnsCustomSkin.gameObject.SetActive(true);
                ParentOfBtnsAvatarSkin.gameObject.SetActive(false);

                SetContentOnScroll(AvatarPanel[8], (RectTransform)ParentOfBtnsCustomSkin);
                break;
        }
        //AssetBundle.UnloadAllAssetBundles(false);
        //Resources.UnloadUnusedAssets();

        UpdateStoreSelection(index);
    }
    public void OnColorButtonClicked(int _index)
    {
        //AssetBundle.UnloadAllAssetBundles(false);
        //Resources.UnloadUnusedAssets();

        colorBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        colorBtn.GetComponent<Button>().onClick.AddListener(() => SwitchColorMode(_index));

        switch (_index)
        {
            case 1:
                ParentOfBtnsAvatarFace.gameObject.SetActive(false);
                ParentOfBtnsCustomFace.gameObject.SetActive(true);

                SetContentOnScroll(AvatarPanel[1], (RectTransform)ParentOfBtnsCustomFace);
                break;
            case 3:
                ParentOfBtnsAvatarEyes.gameObject.SetActive(false);
                ParentOfBtnsCustomEyesPalette.gameObject.SetActive(false);
                ParentOfBtnsCustomEyes.gameObject.SetActive(true);

                UpdateColor(_index);

                SetContentOnScroll(AvatarPanel[3], (RectTransform)ParentOfBtnsCustomEyes);
                if (ParentOfBtnsCustomLips.transform.childCount == 0)
                {
                    SubmitAllItemswithSpecificSubCategory(SubCategoriesList[_index + 8].id, true);
                }

                break;
            case 5:
                ParentOfBtnsAvatarLips.gameObject.SetActive(false);
                ParentOfBtnsCustomLipsPalette.gameObject.SetActive(false);
                ParentOfBtnsCustomLips.gameObject.SetActive(true);

                UpdateColor(_index);

                SetContentOnScroll(AvatarPanel[5], (RectTransform)ParentOfBtnsCustomLips);
                if (ParentOfBtnsCustomLips.transform.childCount == 0)
                {
                    SubmitAllItemswithSpecificSubCategory(SubCategoriesList[_index + 8].id, true);
                }

                break;
            case 7:
                ParentOfBtnsAvatarSkin.gameObject.SetActive(false);
                ParentOfBtnsCustomSkin.gameObject.SetActive(true);


                UpdateColor(_index);

                SetContentOnScroll(AvatarPanel[7], (RectTransform)ParentOfBtnsAvatarSkin);
                break;
        }
    }

    // update color call when open color selection
    public void UpdateColor(int _index)
    {
        ////Debug.Log("<color=red>Update Color Index</color>" + _index);
        textskin.enabled = true;
        switch (_index)
        {
            case 0:
                if (XanaConstants.xanaConstants.hairColoPalette != "" && ParentOfBtnsCustomHair.transform.childCount != 0)
                {
                    for (int i = 0; i < ParentOfBtnsCustomHair.transform.childCount; i++)
                    {
                        childObject = ParentOfBtnsCustomHair.transform.GetChild(i).gameObject;
                        if (childObject.GetComponent<ItemDetail>().id == XanaConstants.xanaConstants.hairColoPalette)
                        {
                            //Debug.Log("ID = " + childObject.GetComponent<ItemDetail>().id);

                            childObject.GetComponent<Image>().enabled = true;
                            XanaConstants.xanaConstants._lastClickedBtn = childObject;
                            //Debug.Log("<color=red>StoreManager AssignLastClickedBtnHere</color>");
                            XanaConstants.xanaConstants.colorSelection[2] = childObject;

                            CheckForItemDetail(XanaConstants.xanaConstants.eyeColorPalette, 2);

                            break;
                        }
                    }
                }
                break;
            case 2:
                if (XanaConstants.xanaConstants.eyeBrowColorPaletteIndex != -1 && ParentOfBtnsCustomEyeBrows.transform.childCount != 0)
                {
                    for (int i = 0; i < ParentOfBtnsCustomEyeBrows.transform.childCount; i++)
                    {
                        childObject = ParentOfBtnsCustomEyeBrows.transform.GetChild(i).gameObject;
                        if (childObject.GetComponent<ItemDetail>().id == XanaConstants.xanaConstants.eyeBrowColorPaletteIndex.ToString())
                        {
                            childObject.GetComponent<Image>().enabled = true;
                            XanaConstants.xanaConstants._lastClickedBtn = childObject;
                            //Debug.Log("<color=red>StoreManager AssignLastClickedBtnHere</color>");
                            XanaConstants.xanaConstants.colorSelection[3] = childObject;

                            CheckForItemDetail(XanaConstants.xanaConstants.eyeColorPalette, 3);

                            break;
                        }
                    }
                }
                break;
            case 3:
                if (XanaConstants.xanaConstants.eyeColor != "")
                {
                    for (int i = 0; i < ParentOfBtnsCustomEyes.transform.childCount; i++)
                    {
                        childObject = ParentOfBtnsCustomEyes.transform.GetChild(i).gameObject;
                        if (childObject.GetComponent<ItemDetail>().id == XanaConstants.xanaConstants.eyeColor)
                        {
                            childObject.GetComponent<Image>().enabled = true;
                            XanaConstants.xanaConstants._lastClickedBtn = childObject;
                            //Debug.Log("<color=red>StoreManager AssignLastClickedBtnHere</color>");
                            XanaConstants.xanaConstants.colorSelection[0] = childObject;

                            CheckForItemDetail(XanaConstants.xanaConstants.eyeColor, 4);

                            break;
                        }
                    }
                }
                if (XanaConstants.xanaConstants.eyeColorPalette != "" && ParentOfBtnsCustomEyesPalette.transform.childCount != 0)
                {
                    for (int i = 0; i < ParentOfBtnsCustomEyesPalette.transform.childCount; i++)
                    {
                        childObject = ParentOfBtnsCustomEyesPalette.transform.GetChild(i).gameObject;
                        if (childObject.GetComponent<ItemDetail>().id == XanaConstants.xanaConstants.eyeColorPalette)
                        {
                            childObject.GetComponent<Image>().enabled = true;
                            XanaConstants.xanaConstants._lastClickedBtn = childObject;
                            //Debug.Log("<color=red>StoreManager AssignLastClickedBtnHere</color>");
                            XanaConstants.xanaConstants.colorSelection[4] = childObject;

                            CheckForItemDetail(XanaConstants.xanaConstants.eyeColorPalette, 4);

                            break;
                        }
                    }
                }
                break;

            case 5:
                if (XanaConstants.xanaConstants.lipColor != "")
                {
                    for (int i = 0; i < ParentOfBtnsCustomLips.transform.childCount; i++)
                    {
                        childObject = ParentOfBtnsCustomLips.transform.GetChild(i).gameObject;
                        if (childObject.GetComponent<ItemDetail>().id == XanaConstants.xanaConstants.lipColor)
                        {
                            childObject.GetComponent<Image>().enabled = true;
                            XanaConstants.xanaConstants._lastClickedBtn = childObject;
                            //Debug.Log("<color=red>StoreManager AssignLastClickedBtnHere</color>");
                            XanaConstants.xanaConstants.colorSelection[1] = childObject;

                            CheckForItemDetail(XanaConstants.xanaConstants.lipColor, 5);

                            break;
                        }
                    }
                }
                if (XanaConstants.xanaConstants.lipColorPalette != "" && ParentOfBtnsCustomLipsPalette.transform.childCount != 0)
                {
                    for (int i = 0; i < ParentOfBtnsCustomLipsPalette.transform.childCount; i++)
                    {
                        childObject = ParentOfBtnsCustomLipsPalette.transform.GetChild(i).gameObject;
                        if (childObject.GetComponent<ItemDetail>().id == XanaConstants.xanaConstants.lipColorPalette)
                        {
                            childObject.GetComponent<Image>().enabled = true;
                            XanaConstants.xanaConstants._lastClickedBtn = childObject;
                            //Debug.Log("<color=red>StoreManager AssignLastClickedBtnHere</color>");
                            XanaConstants.xanaConstants.colorSelection[5] = childObject;

                            CheckForItemDetail(XanaConstants.xanaConstants.lipColorPalette, 5);

                            break;
                        }
                    }
                }
                break;

            case 7:
                if (XanaConstants.xanaConstants.skinColor != "")
                {
                    for (int i = 0; i < ParentOfBtnsCustomSkin.transform.childCount; i++)
                    {
                        childObject = ParentOfBtnsCustomSkin.transform.GetChild(i).gameObject;
                        if (childObject.GetComponent<ItemDetail>().MyIndex.ToString() == XanaConstants.xanaConstants.skinColor)
                        {
                            childObject.GetComponent<Image>().enabled = true;
                            XanaConstants.xanaConstants._lastClickedBtn = childObject;
                            //Debug.Log("<color=red>StoreManager AssignLastClickedBtnHere</color>");
                            XanaConstants.xanaConstants.avatarStoreSelection[7] = childObject;

                            CheckForItemDetail(XanaConstants.xanaConstants.skinColor, 6);

                            break;
                        }
                    }
                }
                break;


        }
    }

    public void SetContentOnScroll(GameObject _scrollView, RectTransform _content)
    {
        _scrollView.GetComponent<ScrollRect>().content = _content;
    }

    public void ClearBuyItems()
    {
        for (int i = 0; i < TotalBtnlist.Count; i++)
        {
            TotalBtnlist[i].SelectedBool = false;
            TotalBtnlist[i].gameObject.GetComponent<Image>().color = TotalBtnlist[i].NormalColor;
        }
        TotalBtnlist.Clear();

    }
    public void GetSelectedBtn(int getIndex, EnumClass.CategoryEnum selectedEnum)
    {

        switch (selectedEnum)
        {
            case EnumClass.CategoryEnum.Head:
                {
                    //AssetBundle.UnloadAllAssetBundles(false);
                    // Resources.UnloadUnusedAssets();

                    for (int i = 0; i < CategorieslistHeads.Count; i++)
                    {
                        if (i == getIndex)
                        {
                            CategorieslistHeads[i].SelectedBool = true;
                            CategorieslistHeads[i].gameObject.GetComponent<Image>().color = CategorieslistHeads[i].HighlightedColor;

                            TotalBtnlist.Add(CategorieslistHeads[i]);
                        }
                        else
                        {
                            CategorieslistHeads[i].SelectedBool = false;
                            CategorieslistHeads[i].gameObject.GetComponent<Image>().color = CategorieslistHeads[i].NormalColor;
                            TotalBtnlist.Remove(CategorieslistHeads[i]);
                        }
                    }
                    break;
                }
            case EnumClass.CategoryEnum.Face:
                {
                    //AssetBundle.UnloadAllAssetBundles(false);
                    //Resources.UnloadUnusedAssets();

                    for (int i = 0; i < CategorieslistFace.Count; i++)
                    {
                        if (i == getIndex)
                        {
                            CategorieslistFace[i].SelectedBool = true;
                            CategorieslistFace[i].GetComponent<Image>().color = CategorieslistFace[i].HighlightedColor;

                            TotalBtnlist.Add(CategorieslistFace[i]);
                        }
                        else
                        {
                            CategorieslistFace[i].SelectedBool = false;
                            CategorieslistFace[i].GetComponent<Image>().color = CategorieslistFace[i].NormalColor;
                            TotalBtnlist.Remove(CategorieslistFace[i]);
                        }
                    }
                    break;
                }
            case EnumClass.CategoryEnum.Inner:
                {
                    //AssetBundle.UnloadAllAssetBundles(false);
                    //Resources.UnloadUnusedAssets();

                    for (int i = 0; i < CategorieslistInner.Count; i++)
                    {
                        if (i == getIndex)
                        {
                            CategorieslistInner[i].SelectedBool = true;
                            CategorieslistInner[i].GetComponent<Image>().color = CategorieslistInner[i].HighlightedColor;

                            TotalBtnlist.Add(CategorieslistInner[i]);

                        }
                        else
                        {
                            CategorieslistInner[i].SelectedBool = false;
                            CategorieslistInner[i].GetComponent<Image>().color = CategorieslistInner[i].NormalColor;
                            TotalBtnlist.Remove(CategorieslistInner[i]);
                        }
                    }
                    break;
                }
            case EnumClass.CategoryEnum.Outer:
                {
                    //AssetBundle.UnloadAllAssetBundles(false);
                    //Resources.UnloadUnusedAssets();

                    for (int i = 0; i < CategorieslistOuter.Count; i++)
                    {
                        if (i == getIndex)
                        {
                            CategorieslistOuter[i].SelectedBool = true;
                            CategorieslistOuter[i].GetComponent<Image>().color = CategorieslistOuter[i].HighlightedColor;
                            TotalBtnlist.Add(CategorieslistOuter[i]);
                        }
                        else
                        {
                            CategorieslistOuter[i].SelectedBool = false;
                            CategorieslistOuter[i].GetComponent<Image>().color = CategorieslistOuter[i].NormalColor;
                            TotalBtnlist.Remove(CategorieslistOuter[i]);
                        }
                    }
                    break;
                }

            case EnumClass.CategoryEnum.Accesary:
                {
                    //AssetBundle.UnloadAllAssetBundles(false);
                    //Resources.UnloadUnusedAssets();

                    for (int i = 0; i < CategorieslistAccesary.Count; i++)
                    {
                        if (i == getIndex)
                        {
                            CategorieslistAccesary[i].SelectedBool = true;
                            CategorieslistAccesary[i].GetComponent<Image>().color = CategorieslistAccesary[i].HighlightedColor;

                            TotalBtnlist.Add(CategorieslistAccesary[i]);
                        }
                        else
                        {
                            CategorieslistAccesary[i].SelectedBool = false;
                            CategorieslistAccesary[i].GetComponent<Image>().color = CategorieslistAccesary[i].NormalColor;
                            TotalBtnlist.Remove(CategorieslistAccesary[i]);
                        }
                    }
                    break;
                }

            case EnumClass.CategoryEnum.Bottom:
                {
                    //AssetBundle.UnloadAllAssetBundles(false);
                    //Resources.UnloadUnusedAssets();

                    for (int i = 0; i < CategorieslistBottom.Count; i++)
                    {
                        if (i == getIndex)
                        {
                            CategorieslistBottom[i].SelectedBool = true;
                            CategorieslistBottom[i].GetComponent<Image>().color = CategorieslistBottom[i].HighlightedColor;

                            TotalBtnlist.Add(CategorieslistBottom[i]);
                        }
                        else
                        {
                            CategorieslistBottom[i].SelectedBool = false;
                            CategorieslistBottom[i].GetComponent<Image>().color = CategorieslistBottom[i].NormalColor;
                            TotalBtnlist.Remove(CategorieslistBottom[i]);
                        }
                    }
                    break;
                }
            case EnumClass.CategoryEnum.Socks:
                {
                    //AssetBundle.UnloadAllAssetBundles(false);
                    //Resources.UnloadUnusedAssets();

                    for (int i = 0; i < CategorieslistSocks.Count; i++)
                    {
                        if (i == getIndex)
                        {
                            CategorieslistSocks[i].SelectedBool = true;
                            CategorieslistSocks[i].GetComponent<Image>().color = CategorieslistSocks[i].HighlightedColor;
                            TotalBtnlist.Add(CategorieslistSocks[i]);
                        }
                        else
                        {
                            CategorieslistSocks[i].SelectedBool = false;
                            CategorieslistSocks[i].GetComponent<Image>().color = CategorieslistSocks[i].NormalColor;
                            TotalBtnlist.Remove(CategorieslistSocks[i]);
                        }
                    }
                    break;
                }
            // CategorieslistShoes
            case EnumClass.CategoryEnum.Shoes:
                {
                    //AssetBundle.UnloadAllAssetBundles(false);
                    //Resources.UnloadUnusedAssets();

                    for (int i = 0; i < CategorieslistShoes.Count; i++)
                    {
                        if (i == getIndex)
                        {
                            CategorieslistShoes[i].SelectedBool = true;
                            CategorieslistShoes[i].GetComponent<Image>().color = CategorieslistShoes[i].HighlightedColor;
                            TotalBtnlist.Add(CategorieslistShoes[i]);
                        }
                        else
                        {
                            CategorieslistShoes[i].SelectedBool = false;
                            CategorieslistShoes[i].GetComponent<Image>().color = CategorieslistShoes[i].NormalColor;
                            TotalBtnlist.Remove(CategorieslistShoes[i]);
                        }
                    }
                    break;
                }
        }
        SelectBtnObjs();
    }

    public void SelectBtnObjs()
    {
        //AssetBundle.UnloadAllAssetBundles(false);
        //Resources.UnloadUnusedAssets();

        int TotalPrice = 0;
        for (int i = 0; i < TotalBtnlist.Count; i++)
        {
            if (TotalBtnlist[i].SelectedBool)
            {
                TotalPrice += int.Parse(TotalBtnlist[i].PriceTxt.text);
            }
        }
        BuyCountertxt.text = TotalBtnlist.Count.ToString();
    }
    //public void ClearParentFromObjs()
    //{
    //    if (BuyPanelParentOfBtns.childCount > 0)
    //    {
    //        for (int i = 0; i < BuyPanelParentOfBtns.childCount; i++)
    //        {
    //            Destroy(BuyPanelParentOfBtns.GetChild(i).gameObject);
    //        }
    //    }
    //}



    //public void GoToCheckOut()
    //{
    //    AssetBundle.UnloadAllAssetBundles(false);
    //    Resources.UnloadUnusedAssets();

    //    int Counter = 0;
    //    int TotalPrice = 0;
    //    TotalObjectsInBuyPanel.Clear();

    //    if (BuyPanelParentOfBtns.childCount > 0)
    //    {
    //        ClearParentFromObjs();
    //    }
    //    for (int i = 0; i < TotalBtnlist.Count; i++)
    //    {
    //        if (TotalBtnlist[i].SelectedBool)
    //        {
    //            Counter += 1;
    //            TotalPrice += int.Parse(TotalBtnlist[i].PriceTxt.text);
    //            print(Counter);
    //            TotalObjectsInBuyPanel.Add(TotalBtnlist[i].gameObject);
    //        }
    //    }
    //    if (Counter > 0)
    //    {
    //        if (!UserRegisterationManager.instance.LoggedIn)
    //        {
    //            OpenMainPanel("ShowSignUpPanel");
    //        }
    //        else
    //        {
    //            TotalItemPriceCheckOut = 0;
    //            TotalSelectedInBuyPanel.Clear();

    //            StoreItemsPanel.SetActive(false);
    //            CheckOutBuyItemPanel.SetActive(true);
    //            for (int i = 0; i < TotalObjectsInBuyPanel.Count; i++)
    //            {
    //                GameObject L_ItemBtnObj = Instantiate(BuyItemPrefab, BuyPanelParentOfBtns.transform);
    //                L_ItemBtnObj.GetComponent<ItemDetailBuyItem>().PriceTxt.text = TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>().PriceTxt.text;
    //                L_ItemBtnObj.GetComponent<ItemDetailBuyItem>().CategoryTxt.text = TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>().CategoriesEnumVar.ToString();

    //                // Add All Value from One Button to Buy Checkout Btn
    //                L_ItemBtnObj.GetComponent<ItemDetailBuyItem>().assetLinkAndroid = TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>().assetLinkAndroid;
    //                L_ItemBtnObj.GetComponent<ItemDetailBuyItem>().assetLinkIos = TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>().assetLinkIos;
    //                L_ItemBtnObj.GetComponent<ItemDetailBuyItem>().assetLinkWindows = TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>().assetLinkWindows;
    //                L_ItemBtnObj.GetComponent<ItemDetailBuyItem>().createdAt = TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>().createdAt;
    //                L_ItemBtnObj.GetComponent<ItemDetailBuyItem>().createdBy = TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>().createdBy;
    //                L_ItemBtnObj.GetComponent<ItemDetailBuyItem>().iconLink = TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>().iconLink;
    //                L_ItemBtnObj.GetComponent<ItemDetailBuyItem>().id = TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>().id;
    //                L_ItemBtnObj.GetComponent<ItemDetailBuyItem>().isFavourite = TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>().isFavourite;
    //                L_ItemBtnObj.GetComponent<ItemDetailBuyItem>().isOccupied = TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>().isOccupied;
    //                L_ItemBtnObj.GetComponent<ItemDetailBuyItem>().isPaid = TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>().isPaid;
    //                L_ItemBtnObj.GetComponent<ItemDetailBuyItem>().isPurchased = TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>().isPurchased;
    //                L_ItemBtnObj.GetComponent<ItemDetailBuyItem>().name = TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>().name;
    //                L_ItemBtnObj.GetComponent<ItemDetailBuyItem>().price = TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>().price;
    //                L_ItemBtnObj.GetComponent<ItemDetailBuyItem>().categoryId = TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>().categoryId;
    //                L_ItemBtnObj.GetComponent<ItemDetailBuyItem>().subCategory = TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>().subCategory;
    //                L_ItemBtnObj.GetComponent<ItemDetailBuyItem>().updatedAt = TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>().updatedAt;
    //                L_ItemBtnObj.GetComponent<ItemDetailBuyItem>().itemTags = TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>().itemTags;
    //                L_ItemBtnObj.GetComponent<ItemDetailBuyItem>().CategoriesEnumVar = TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>().CategoriesEnumVar;
    //                TotalSelectedInBuyPanel.Add(L_ItemBtnObj.gameObject);
    //                TotalItemPriceCheckOut += int.Parse(L_ItemBtnObj.GetComponent<ItemDetailBuyItem>().PriceTxt.text);
    //            }
    //            TotalPriceBuyPanelTxt.text = TotalItemPriceCheckOut.ToString();
    //            TotalItemsBuyPanelTxt.text = TotalObjectsInBuyPanel.Count.ToString();
    //        }
    //    }
    //}
    //public void BuyItems()
    //{
    //    if (PlayerPrefs.GetInt("TotalCoins") < TotalItemPriceCheckOut)
    //    {
    //        print("Price is less than Selected Items");
    //    }
    //    else if (PlayerPrefs.GetInt("TotalCoins") >= TotalItemPriceCheckOut)
    //    {
    //        if (TotalObjectsInBuyPanel.Count > 0)
    //        {
    //            for (int i = 0; i < TotalObjectsInBuyPanel.Count; i++)
    //            {
    //                EnumClass.CategoryEnum selectedEnum = TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>().CategoriesEnumVar;
    //                switch (selectedEnum)
    //                {
    //                    case EnumClass.CategoryEnum.Head:
    //                        {
    //                            if (CategorieslistHeads.Contains(TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>()))
    //                            {
    //                                int Getindex = CategorieslistHeads.IndexOf(TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>());
    //                                CategorieslistHeads[Getindex].price = "0";
    //                                CategorieslistHeads[Getindex].isPaid = "true";
    //                                CategorieslistHeads[Getindex].isPurchased = "true";
    //                            }
    //                            break;
    //                        }
    //                    case EnumClass.CategoryEnum.Face:
    //                        {
    //                            int Getindex = CategorieslistFace.IndexOf(TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>());
    //                            CategorieslistFace[Getindex].price = "0";
    //                            CategorieslistFace[Getindex].isPaid = "true";
    //                            CategorieslistFace[Getindex].isPurchased = "true";


    //                            break;
    //                        }
    //                    case EnumClass.CategoryEnum.Inner:
    //                        {
    //                            int Getindex = CategorieslistInner.IndexOf(TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>());
    //                            CategorieslistInner[Getindex].price = "0";
    //                            CategorieslistInner[Getindex].isPaid = "true";
    //                            CategorieslistInner[Getindex].isPurchased = "true";
    //                            break;
    //                        }
    //                    case EnumClass.CategoryEnum.Outer:
    //                        {
    //                            int Getindex = CategorieslistOuter.IndexOf(TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>());
    //                            CategorieslistOuter[Getindex].price = "0";
    //                            CategorieslistOuter[Getindex].isPaid = "true";
    //                            CategorieslistOuter[Getindex].isPurchased = "true";
    //                            break;
    //                        }
    //                    case EnumClass.CategoryEnum.Accesary:
    //                        {
    //                            int Getindex = CategorieslistAccesary.IndexOf(TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>());
    //                            CategorieslistAccesary[Getindex].price = "0";
    //                            CategorieslistAccesary[Getindex].isPaid = "true";
    //                            CategorieslistAccesary[Getindex].isPurchased = "true";
    //                            break;
    //                        }
    //                    case EnumClass.CategoryEnum.Bottom:
    //                        {
    //                            int Getindex = CategorieslistBottom.IndexOf(TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>());
    //                            CategorieslistBottom[Getindex].price = "0";
    //                            CategorieslistBottom[Getindex].isPaid = "true";
    //                            CategorieslistBottom[Getindex].isPurchased = "true";
    //                            break;
    //                        }
    //                    case EnumClass.CategoryEnum.Socks:
    //                        {
    //                            int Getindex = CategorieslistSocks.IndexOf(TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>());
    //                            CategorieslistSocks[Getindex].price = "0";
    //                            CategorieslistSocks[Getindex].isPaid = "true";
    //                            CategorieslistSocks[Getindex].isPurchased = "true";
    //                            break;
    //                        }
    //                    case EnumClass.CategoryEnum.Shoes:
    //                        {
    //                            int Getindex = CategorieslistShoes.IndexOf(TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>());
    //                            CategorieslistShoes[Getindex].price = "0";
    //                            CategorieslistShoes[Getindex].isPaid = "true";
    //                            CategorieslistShoes[Getindex].isPurchased = "true";
    //                            break;
    //                        }
    //                }
    //            }
    //            //PlayerPrefs.SetInt("TotalCoins", PlayerPrefs.GetInt("TotalCoins") - TotalItemPriceCheckOut);
    //        }
    //        UpdateItemsDetails();
    //        //CloseCheckOutPanel();
    //    }
    //}
    void UpdateItemsDetails()
    {
        //UpdateUserCoins();
        //for (int i = 0; i < TotalObjectsInBuyPanel.Count; i++)
        //{
        //    TotalObjectsInBuyPanel[i].GetComponent<ItemDetail>().DeSelectAfterBuying();
        //}
        //TotalObjectsInBuyPanel.Clear();

        // ShirtsSelection
        for (int i = 0; i < CategorieslistHeads.Count; i++)
        {
            CategorieslistHeads[i].UpdateValues();
        }
        // PantsSelection
        for (int i = 0; i < CategorieslistFace.Count; i++)
        {
            CategorieslistFace[i].UpdateValues();
        }
        // CategorieslistShoes
        for (int i = 0; i < CategorieslistInner.Count; i++)
        {
            CategorieslistInner[i].UpdateValues();
        }
        // CategorieslistHairs
        for (int i = 0; i < CategorieslistOuter.Count; i++)
        {
            CategorieslistOuter[i].UpdateValues();
        }
        // CategorieslistGlasses
        for (int i = 0; i < CategorieslistAccesary.Count; i++)
        {
            CategorieslistAccesary[i].UpdateValues();
        }
        // CategorieslistHats
        for (int i = 0; i < CategorieslistBottom.Count; i++)
        {
            CategorieslistBottom[i].UpdateValues();
        }
        //CategorieslistBags
        for (int i = 0; i < CategorieslistSocks.Count; i++)
        {
            CategorieslistSocks[i].UpdateValues();
        }
        //CategorieslistBags
        for (int i = 0; i < CategorieslistShoes.Count; i++)
        {
            CategorieslistShoes[i].UpdateValues();
        }
    }
    //public void RemoveItemsFromCheckOut(GameObject itemDetailsBtn)
    //{
    //    if (TotalSelectedInBuyPanel.Contains(itemDetailsBtn))
    //    {
    //        TotalSelectedInBuyPanel.Remove(itemDetailsBtn);
    //        UpdateCheckOutCash();
    //    }
    //    // TotalObjectsInBuyPanel
    //}
    //public void AddItemsFromCheckOut(GameObject itemDetailsBtn)
    //{
    //    if (!TotalSelectedInBuyPanel.Contains(itemDetailsBtn))
    //    {
    //        TotalSelectedInBuyPanel.Add(itemDetailsBtn);
    //        UpdateCheckOutCash();
    //    }
    //    // TotalObjectsInBuyPanel
    //}
    //public void BuyItemsbyPurchaseAPI()
    //{
    //    List<string> purchaseItemsIDs = new List<string>();
    //    if (PlayerPrefs.GetInt("TotalCoins") < TotalItemPriceCheckOut)
    //    {
    //        OpenMainPanel("LowCoinsPanel");
    //    }
    //    else if (PlayerPrefs.GetInt("TotalCoins") >= TotalItemPriceCheckOut)
    //    {
    //        if (TotalSelectedInBuyPanel.Count > 0)
    //        {
    //            for (int i = 0; i < TotalSelectedInBuyPanel.Count; i++)
    //            {
    //                purchaseItemsIDs.Add(TotalSelectedInBuyPanel[i].GetComponent<ItemDetailBuyItem>().id.ToString());
    //            }
    //        }
    //        ArrayofBuyItems = purchaseItemsIDs.ToArray();
    //        SubmitPurchaseAPI(ArrayofBuyItems);
    //    }
    //}
    //public void UpdateCheckOutCash()
    //{
    //    TotalItemPriceCheckOut = 0;
    //    if (TotalSelectedInBuyPanel.Count == 0)
    //    {
    //        TotalItemPriceCheckOut = 0;
    //        BuyBtnCheckOut.GetComponent<Button>().interactable = false;
    //    }
    //    else
    //    {
    //        BuyBtnCheckOut.GetComponent<Button>().interactable = true;
    //        for (int i = 0; i < TotalSelectedInBuyPanel.Count; i++)
    //        {
    //            TotalItemPriceCheckOut += int.Parse(TotalSelectedInBuyPanel[i].GetComponent<ItemDetailBuyItem>().PriceTxt.text);
    //        }
    //    }
    //    TotalPriceBuyPanelTxt.text = TotalItemPriceCheckOut.ToString();
    //    TotalItemsBuyPanelTxt.text = TotalSelectedInBuyPanel.Count.ToString();
    //}
    //public void CloseCheckOutPanel()
    //{
    //    CheckOutBuyItemPanel.SetActive(false);
    //}
    public class EnumClass : MonoBehaviour
    {
        public enum CategoryEnum
        {
            Head,
            Face,
            Inner,
            Outer,
            Accesary,
            Bottom,
            Socks,
            Shoes,
            HairAvatar,
            HairAvatarColor,
            LipsAvatar,
            LipsAvatarColor,
            EyesAvatar,
            EyesAvatarColor,
            SkinToneAvatar,
            Presets,
            EyeBrowAvatar,
            EyeBrowAvatarColor,
            EyeLashesAvatar,
            Nose,
            Body,
            Makeup,
            Avatar,
            Wearable,
            SliderColor,
            AvatarBtns,
            WearableBtns,
            None
        }
    }

    // Purchase Item Starts Here
    private void SubmitPurchaseAPI(string[] TakeArrayofBuyItems)
    {
        var result = string.Join(",", TakeArrayofBuyItems);
        result = "[" + result + "]";
        ClassforPurchaseAPI purchaseCLassObj = new ClassforPurchaseAPI();
        string bodyJson = JsonUtility.ToJson(purchaseCLassObj.CreateTOJSON(result)); ;
        StartCoroutine(HitPurchaseAPI(ConstantsGod.API_BASEURL + ConstantsGod.PurchasedAPI, bodyJson));
    }
    IEnumerator HitPurchaseAPI(string url, string Jsondata)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(Jsondata);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        request.SendWebRequest();
        while (request.isDone)
        {
            yield return null;
        }
        ClassforPurchaseDataExtract PurchaseDataExtractObj = new ClassforPurchaseDataExtract();
        PurchaseDataExtractObj = PurchaseDataExtractObj.CreateFromJSON(request.downloadHandler.text);
        if (!request.isHttpError && !request.isNetworkError)
        {
            if (request.error == null)
            {
                if (PurchaseDataExtractObj.success == true)
                {
                    RefreshDefault();
                    SubmitUserDetailAPI();
                    SubmitDefaultAPI();
                }
            }
        }
        else
        {
            if (request.isNetworkError)
            {
                print("Error Accured " + request.error.ToUpper());
            }
            else
            {
                if (request.error != null)
                {
                    if (PurchaseDataExtractObj.success == false)
                    {
                        print("Hey success false " + PurchaseDataExtractObj.msg);
                    }
                }
            }
        }
        request.Dispose();

    }
    void RefreshDefault()
    {
        CategorieslistHeads.Clear();
        CategorieslistFace.Clear();
        CategorieslistInner.Clear();
        CategorieslistOuter.Clear();
        CategorieslistAccesary.Clear();
        CategorieslistBottom.Clear();
        CategorieslistSocks.Clear();
        CategorieslistShoes.Clear();
        CategorieslistHairs.Clear();
        CategorieslistHairsColors.Clear();
        CategorieslistLipsColor.Clear();
        CategorieslistSkinToneColor.Clear();
        CategorieslistEyesColor.Clear();
        TotalBtnlist.Clear();

        BuyCountertxt.text = "0";
    }
    [System.Serializable]
    public class ClassforPurchaseAPI
    {
        public string ItemIds;
        public ClassforPurchaseAPI CreateTOJSON(string jsonString)
        {
            ClassforPurchaseAPI myObj = new ClassforPurchaseAPI();
            myObj.ItemIds = jsonString;
            return myObj;
        }
    }
    [System.Serializable]
    public class ClassforPurchaseDataExtract
    {
        public bool success;
        public string data;
        public string msg;
        public ClassforPurchaseDataExtract CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<ClassforPurchaseDataExtract>(jsonString);
        }
    }
    // Purchase Item End Here


    //  UserDetails Starts here ************************************************************************
    private string TestNetXenyTokenAPI = "https://backend.xanalia.com/sale-nft/get-xeny-tokens-by-user";
    private string MainNetXenyTokenAPI = ""; // Mainnet Api here
    public void SubmitUserDetailAPI()
    {
        //string localAPI = "";
        //if (!APIBaseUrlChange.instance.IsXanaLive)
        //{
        //    localAPI = TestNetXenyTokenAPI;
        //}
        //else
        //{
        //    // Mainnet Api
        //    localAPI = MainNetXenyTokenAPI;
        //}
        //StartCoroutine(XenyTokenUserAddrerss(localAPI));
    }
    private RequestedData requestData;

    IEnumerator XenyTokenUserAddrerss(string url)
    {

        requestData = new RequestedData();
        requestData.userAddress = PlayerPrefs.GetString("publicID");     //For Testing Xent coins address= "0xA4eFBae8755fE223eB4288B278BEb410F8c6e27E";
        string jsonData = JsonConvert.SerializeObject(requestData);
        // Convert the JSON data to a byte array
        byte[] postData = Encoding.UTF8.GetBytes(jsonData);
        UnityWebRequest request = UnityWebRequest.Post(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        //Debug.Log("hamara data v" + request.downloadHandler.text);

        if (!request.isHttpError && !request.isNetworkError)
        {
            if (request.error == null)
            {
                JObject json = JObject.Parse(request.downloadHandler.text);
                string token = json["userXenyTokens"].ToString();
                TotalGameCoins.text = token;
                print("xeny coins are = " + token);
            }
        }
        else
        {
            if (request.isNetworkError)
            {
                print("Error Occured " + request.error.ToUpper());
            }
        }
        request.Dispose();
    }
    // Submit GetUser Details        
    IEnumerator HitGetUserDetails(string url, string Jsondata)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);

            request.SendWebRequest();
            while (!request.isDone)
            {
                yield return null;
            }
            ClassforUserDetails myObjectOfUserDetail = new ClassforUserDetails();
            myObjectOfUserDetail = myObjectOfUserDetail.CreateFromJSON(request.downloadHandler.text);

            if (!request.isHttpError && !request.isNetworkError)
            {
                if (request.error == null)
                {
                    if (PlayerPrefs.GetInt("IsChanged") == 0)
                    {
                        PlayerPrefs.SetInt("IsChanged", 1);
                        UndoSelection();
                        StartCoroutine(CharacterChange());
                    }

                    if (myObjectOfUserDetail.success == true)
                    {
                        decimal CoinsInDecimal = decimal.Parse(myObjectOfUserDetail.data.coins);
                        int Coinsint = (int)CoinsInDecimal;
                        PlayerPrefs.SetInt("TotalCoins", Coinsint);
                        //UpdateUserCoins();
                    }
                }
            }
            else
            {
                if (request.isNetworkError)
                {
                    print(request.error.ToUpper());
                }
                else
                {
                    if (request.error != null)
                    {
                        if (myObjectOfUserDetail.success == false)
                        {
                            print("Hey success false " + myObjectOfUserDetail.msg);
                        }
                    }
                }
            }
            request.Dispose();
        }
    }

    [System.Serializable]
    public class ClassforUserDetails
    {
        public bool success;
        public JsondataOfUserDetails data;
        public string msg;
        public ClassforUserDetails CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<ClassforUserDetails>(jsonString);
        }
    }

    [System.Serializable]
    public class JsondataOfUserDetails
    {
        public int id;
        public string name;
        public string dob;
        public string phoneNumber;
        public string email;
        public string avatar;
        public int role;
        public string coins;
        public bool isVerified;
        public bool isRegister;
        public bool isDeleted;
        public string createdAt;
        public string updatedAt;
        public UserProfileForUserDetails userProfile;
        public static JsondataOfUserDetails CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<JsondataOfUserDetails>(jsonString);
        }
    }
    [System.Serializable]
    public class UserProfileForUserDetails
    {
        public string id;
        public string userId;
        public string gender;
        public string job;
        public string country;
        public string bio;
        public string isDeleted;
        public string createdAt;
        public string updatedAt;
        public static JsondataOfUserDetails CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<JsondataOfUserDetails>(jsonString);
        }
    }
    //  UserDetails End here -------------------------------------------------------------

    //********** Get Guest Issues ******

    public void SubmitDefaultAPIForGuest()
    {
        StartCoroutine(HitDefaultAPIforGuest(ConstantsGod.API_BASEURL + ConstantsGod.GetDefaultAPI, ""));
    }
    IEnumerator HitDefaultAPIforGuest(string url, string Jsondata)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            request.SendWebRequest();
            while (!request.isDone)
            {
                yield return null;
            }
            JsonDataObj = GetAllData(request.downloadHandler.text);
            if (!request.isHttpError && !request.isNetworkError)
            {
                if (request.error == null)
                {
                    //Debug.Log(request.downloadHandler.text);
                    if (JsonDataObj.success == true)
                    {
                        print("Success True All Default Data Fetched for Guest");
                    }
                }
            }
            else
            {
                if (request.isNetworkError)
                {
                    print("Network Error");
                }
                else
                {
                    if (request.error != null)
                    {
                        if (JsonDataObj.success == false)
                        {
                            print("Hey success false " + JsonDataObj.msg);
                        }
                    }
                }
            }
            request.Dispose();
        }
    }

    //  ******************************************** Get Default Starts Here ********************************************
    public void SubmitDefaultAPI()
    {
        StartCoroutine(HitDefaultAPI(ConstantsGod.API_BASEURL + ConstantsGod.GetDefaultAPI, ""));
    }
    IEnumerator HitDefaultAPI(string url, string Jsondata)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            request.SendWebRequest();
            while (!request.isDone)
            {
                yield return null;
            }
            JsonDataObj = GetAllData(request.downloadHandler.text);
            if (!request.isHttpError && !request.isNetworkError)
            {
                if (request.error == null)
                {
                    //Debug.Log(request.downloadHandler.text);
                    if (JsonDataObj.success == true)
                    {
                        print("Success True All Default Data Fetched");
                    }
                }
            }
            else
            {
                if (request.isNetworkError)
                {
                    print("Network Error");
                }
                else
                {
                    if (request.error != null)
                    {
                        if (JsonDataObj.success == false)
                        {
                            print("Hey success false " + JsonDataObj.msg);
                        }
                    }
                }
            }
            request.Dispose();
        }
    }

    public GetAllInfo GetAllData(string m_JsonData)
    {
        JsonDataObj = JsonUtility.FromJson<GetAllInfo>(m_JsonData);
        return JsonDataObj;
    }
    [System.Serializable]
    public class GetAllInfo
    {
        public bool success;
        public ItemClass data;
        public string msg;
    }
    [System.Serializable]
    public class ItemClass
    {
        public CategoryClass Items;
    }
    [System.Serializable]
    public class CategoryClass
    {
        public ToalClothItemsClass Cloth;
        public ToalAvatarItemsClass Avatar;
    }

    [System.Serializable]
    public class ToalClothItemsClass
    {
        public List<Head> Head;
        public List<Face> Face;
        public List<Inner> Inner;
        public List<Outer> Outer;
        public List<Accesary> Accesary;
        public List<Bottom> Bottom;
        public List<Socks> Socks;
        public List<Shoes> Shoes;
    }
    [System.Serializable]
    public class ToalAvatarItemsClass
    {
        public List<Hair> HairSelection;
        public List<FaceAvatar> FaceAvatarSelection;
        public List<EyeBrow> EyeBrowSelection;
        public List<Eyes> EyesSelection;
        public List<Nose> NoseSelection;
        public List<Lip> LipSelection;
        public List<Body> BodySelection;
        public List<Skin> SkinSelection;
    }
    // Cloth Customization

    // PantsSelection
    [System.Serializable]
    public class Head : ItemsParents
    {

    }
    //ShirtsSelection  
    [System.Serializable]
    public class Face : ItemsParents
    {

    }
    //HairsSelection 
    [System.Serializable]
    public class Inner : ItemsParents
    {

    }
    //ShoesSelection
    [System.Serializable]
    public class Outer : ItemsParents
    {


    }
    //Glasses
    [System.Serializable]
    public class Accesary : ItemsParents
    {
    }
    //Hats
    [System.Serializable]
    public class Bottom : ItemsParents
    {


    }
    //Bags
    [System.Serializable]
    public class Socks : ItemsParents
    {

    }
    //CategoriesDetails
    [System.Serializable]
    public class Shoes : ItemsParents
    {

    }

    // Avatar Customization

    // HairSelection
    [System.Serializable]
    public class Hair : ItemsParents
    {

    }
    // FaceAvatarSelection  
    [System.Serializable]
    public class FaceAvatar : ItemsParents
    {

    }
    // EyeBrowSelection 
    [System.Serializable]
    public class EyeBrow : ItemsParents
    {

    }
    //  EyesSelection
    [System.Serializable]
    public class Eyes : ItemsParents
    {


    }
    // NoseSelection
    [System.Serializable]
    public class Nose : ItemsParents
    {
    }
    // LipSelection
    [System.Serializable]
    public class Lip : ItemsParents
    {

    }
    //  BodySelection
    [System.Serializable]
    public class Body : ItemsParents
    {

    }
    // SkinSelection
    [System.Serializable]
    public class Skin : ItemsParents
    {

    }
    public class ItemsParents
    {
        public string assetLinkAndroid;
        public string assetLinkIos;
        public string assetLinkWindows;
        public string createdAt;
        public string createdBy;
        public string iconLink;
        public string id;
        public string isFavourite;
        public string isOccupied;
        public string isPaid;
        public string isPurchased;
        public string name;
        public string price;
        public string categoryId;
        public string subCategory;
        public string updatedAt;
        public string[] itemTags;
    }
    // ----------------------------------------- Get Default ENDS Here -----------------------------------------
    //  *************************************** Start Send Coins to Server ********************************************
    public void SubmitSendCoinstoServer(int getCoinsAfterInApp)
    {
        decimal CoinsInDecimal = Convert.ToDecimal(getCoinsAfterInApp);
        CoinsInDecimal = CoinsInDecimal + 0.00m;
        ClassofSendCoins sendcoinsObj = new ClassofSendCoins();
        string bodyJson = JsonUtility.ToJson(sendcoinsObj.CreateTOJSON(CoinsInDecimal.ToString()));
        StartCoroutine(HitSendCoinsAPI(ConstantsGod.API_BASEURL + ConstantsGod.SendCoinsAPI, bodyJson));
    }
    IEnumerator HitSendCoinsAPI(string url, string Jsondata)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(Jsondata);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);

        request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        ClassforSendCoinsExtraction SendCoinsDataExtractObj = new ClassforSendCoinsExtraction();
        SendCoinsDataExtractObj = SendCoinsDataExtractObj.CreateFromJSON(request.downloadHandler.text);
        if (!request.isHttpError && !request.isNetworkError)
        {
            if (request.error == null)
            {
                if (SendCoinsDataExtractObj.success == true)
                {
                    SubmitUserDetailAPI();
                }
            }
        }
        else
        {
            if (request.isNetworkError)
            {
                print("Error Accured " + request.error.ToUpper());
            }
            else
            {
                if (request.error != null)
                {
                    if (SendCoinsDataExtractObj.success == false)
                    {
                        //   print("Hey success false " + SendCoinsDataExtractObj.msg);
                    }
                }
            }
        }
        request.Dispose();
    }

    [System.Serializable]
    public class ClassofSendCoins
    {
        public string coins;
        public ClassofSendCoins CreateTOJSON(string jsonString)
        {
            ClassofSendCoins myObj = new ClassofSendCoins();
            myObj.coins = jsonString;
            return myObj;
        }
    }
    [System.Serializable]
    public class ClassforSendCoinsExtraction
    {
        public bool success;
        public string data;
        public string msg;
        public ClassforSendCoinsExtraction CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<ClassforSendCoinsExtraction>(jsonString);
        }
    }

    //  *************************************** End Coins to Server ********************************************
    public EnumClass.CategoryEnum TempEnumVar;

    public void PutDataInOurAPPNewAPI()
    {
        if (itemLoading != null)
            StopCoroutine(itemLoading);
        itemLoading = StartCoroutine(PutDataInOurAPPNewAPICoroutine());

    }
    public IEnumerator PutDataInOurAPPNewAPICoroutine()
    {
        if (!colorMode)
            yield return null;
        RefreshDefault();
        List<ItemDetail> TempitemDetail;
        TempitemDetail = new List<ItemDetail>();
        Transform TempSubcategoryParent = null;
        //    //Debug.Log("<color=red>Planel Index: " + IndexofPanel + "</color>");
        switch (IndexofPanel)
        {
            case 0:
                {
                    //TempSubcategoryParent = ParentOfBtnsForHeads;
                    //TempEnumVar = EnumClass.CategoryEnum.Head;
                    break;
                }
            case 1:
                {
                    //TempSubcategoryParent = ParentOfBtnsForFace;
                    //TempEnumVar = EnumClass.CategoryEnum.Face;

                    break;
                }
            case 2:
                {
                    //TempSubcategoryParent = ParentOfBtnsForInner;
                    //TempEnumVar = EnumClass.CategoryEnum.Inner;

                    break;
                }
            case 3:
                {
                    TempSubcategoryParent = ParentOfBtnsForOuter;
                    TempEnumVar = EnumClass.CategoryEnum.Outer;
                    break;
                }
            case 4:
                {
                    TempSubcategoryParent = ParentOfBtnsForAccesary;
                    TempEnumVar = EnumClass.CategoryEnum.Accesary;
                    break;
                }
            case 5:
                {
                    TempSubcategoryParent = ParentOfBtnsForBottom;
                    TempEnumVar = EnumClass.CategoryEnum.Bottom;
                    break;
                }
            case 6:
                {
                    //TempSubcategoryParent = ParentOfBtnsForSocks;
                    //TempEnumVar = EnumClass.CategoryEnum.Socks;
                    break;
                }
            case 7:
                {
                    TempSubcategoryParent = ParentOfBtnsForShoes;
                    TempEnumVar = EnumClass.CategoryEnum.Shoes;
                    break;
                }
            case 8:
                {
                    if (!colorMode)
                    {
                        // //Debug.Log("Hairs List 3-----" + ParentOfBtnsAvatarHairs.transform.childCount);
                        TempSubcategoryParent = ParentOfBtnsAvatarHairs;
                        TempEnumVar = EnumClass.CategoryEnum.HairAvatar;
                    }
                    else
                    {
                        TempSubcategoryParent = ParentOfBtnsCustomHair;
                        TempEnumVar = EnumClass.CategoryEnum.HairAvatarColor;
                    }
                    break;
                }

            case 10: // EyeBrow_Case - Added By WaqasAhmad
                {
                    if (!colorMode)
                    {
                        TempSubcategoryParent = ParentOfBtnsAvatarEyeBrows;
                        TempEnumVar = EnumClass.CategoryEnum.EyeBrowAvatar;

                        for (int i = 1; i < ParentOfBtnsAvatarEyeBrows.childCount; i++)
                        {
                            if (ParentOfBtnsAvatarEyeBrows.GetChild(i).GetComponent<ItemDetail>().id.ParseToInt() == SavaCharacterProperties.instance.characterController.eyeBrowId)
                                ParentOfBtnsAvatarEyeBrows.GetChild(i).GetComponent<Image>().enabled = true;
                            else
                                ParentOfBtnsAvatarEyeBrows.GetChild(i).GetComponent<Image>().enabled = false;
                        }
                    }
                    else
                    {
                        TempSubcategoryParent = ParentOfBtnsCustomEyeBrows;
                        TempEnumVar = EnumClass.CategoryEnum.EyeBrowAvatarColor;
                    }
                    break;
                }
            case 11:
                {
                    if (!colorMode)
                    {
                        TempSubcategoryParent = ParentOfBtnsCustomEyes;
                        TempEnumVar = EnumClass.CategoryEnum.EyesAvatar;
                    }
                    else
                    {
                        TempSubcategoryParent = ParentOfBtnsCustomEyesPalette;
                        TempEnumVar = EnumClass.CategoryEnum.EyesAvatarColor;
                    }
                    break;
                }
            case 13:
                {
                    if (!colorMode)
                    {
                        TempSubcategoryParent = ParentOfBtnsCustomLips;
                        TempEnumVar = EnumClass.CategoryEnum.LipsAvatar;
                    }
                    else
                    {
                        TempSubcategoryParent = ParentOfBtnsCustomLipsPalette;
                        TempEnumVar = EnumClass.CategoryEnum.LipsAvatarColor;
                    }
                    break;
                }
            case 15:
                {
                    TempSubcategoryParent = ParentOfBtnsCustomSkin;
                    TempEnumVar = EnumClass.CategoryEnum.SkinToneAvatar;
                    break;
                }
            case 16:
                {// EyeLashes

                    TempSubcategoryParent = ParentOfBtnsAvatarEyeLashes;
                    TempEnumVar = EnumClass.CategoryEnum.EyeLashesAvatar;

                    for (int i = 1; i < ParentOfBtnsAvatarEyeLashes.childCount; i++)
                    {
                        if (ParentOfBtnsAvatarEyeLashes.GetChild(i).GetComponent<ItemDetail>().id.ParseToInt() == SavaCharacterProperties.instance.characterController.eyeLashesId)
                            ParentOfBtnsAvatarEyeLashes.GetChild(i).GetComponent<Image>().enabled = true;
                        else
                            ParentOfBtnsAvatarEyeLashes.GetChild(i).GetComponent<Image>().enabled = false;
                    }
                    break;
                }
        }

        if (TempEnumVar == EnumClass.CategoryEnum.EyeBrowAvatar)
        {
            eyeBrowTapButton.SetActive(true);
            eyeBrowsColorButton.gameObject.SetActive(true);
        }
        else if (TempEnumVar == EnumClass.CategoryEnum.HairAvatar)
            hairColorButton.gameObject.SetActive(true);

        if (TempSubcategoryParent != null /*&& (TempSubcategoryParent.childCount <= 2)*/) // Child Count == 1 for eyeBrowsCustomization // Added by WaqasAhmad
        {
            // HeadSelection
            if (TempEnumVar == EnumClass.CategoryEnum.SkinToneAvatar)
            {
                int loopStart = GetDownloadedNumber(TempEnumVar);
                for (int i = loopStart; i < CharcterBodyParts.instance.skinColor.Count; i++)
                {
                    yield return new WaitForEndOfFrame();
                    GameObject L_ItemBtnObj = Instantiate(ItemsBtnPrefab, TempSubcategoryParent.transform);

                    L_ItemBtnObj.transform.parent = TempSubcategoryParent.transform;
                    L_ItemBtnObj.transform.localScale = new Vector3(1, 1, 1);
                    ItemDetail abc = L_ItemBtnObj.GetComponent<ItemDetail>();
                    abc.isFavourite = "False";
                    abc.isOccupied = "False";
                    abc.isPaid = "False";
                    abc.isPurchased = "true";
                    abc.name = CharcterBodyParts.instance.skinColor[i].ToString();
                    abc.price = "0";
                    abc.categoryId = "2";
                    abc.subCategory = "16";
                    abc.MyIndex = i;
                    abc.CategoriesEnumVar = TempEnumVar;
                    UpdateCategoryDownloadedInt(TempEnumVar);
                    TempitemDetail.Add(abc);
                    L_ItemBtnObj.SetActive(true);
                    if (abc.transform.parent.gameObject.activeSelf)
                    {
                        abc.StartRun();
                        abc.enableUpdate = true;
                    }
                    else
                    {
                        abc.enableUpdate = true;
                    }
                }
            }
            else if (TempEnumVar == EnumClass.CategoryEnum.HairAvatarColor)
            {
                int loopStart = GetDownloadedNumber(TempEnumVar);
                for (int i = loopStart; i < CharcterBodyParts.instance.hairColor.Count; i++)
                {
                    yield return new WaitForEndOfFrame();
                    GameObject L_ItemBtnObj = Instantiate(ItemsBtnPrefab, TempSubcategoryParent.transform);
                    L_ItemBtnObj.transform.parent = TempSubcategoryParent.transform;
                    L_ItemBtnObj.transform.localScale = new Vector3(1, 1, 1);
                    ItemDetail abc = L_ItemBtnObj.GetComponent<ItemDetail>();
                    abc.id = (i + 1).ToString();
                    abc.isFavourite = "False";
                    abc.isOccupied = "False";
                    abc.isPaid = "False";
                    abc.isPurchased = "true";
                    abc.name = CharcterBodyParts.instance.hairColor[i].ToString();
                    abc.price = "0";
                    abc.categoryId = "2";
                    abc.subCategory = "16";
                    abc.MyIndex = i;
                    abc.CategoriesEnumVar = TempEnumVar;
                    UpdateCategoryDownloadedInt(TempEnumVar);
                    TempitemDetail.Add(abc);
                    L_ItemBtnObj.SetActive(true);
                    if (abc.transform.parent.gameObject.activeSelf)
                    {
                        abc.StartRun();
                        abc.enableUpdate = true;
                    }
                    else
                    {
                        abc.enableUpdate = true;
                    }
                }
            }
            else if (TempEnumVar == EnumClass.CategoryEnum.EyeBrowAvatarColor)
            {
                int loopStart = GetDownloadedNumber(TempEnumVar);
                for (int i = loopStart; i < CharcterBodyParts.instance.eyeBrowsColor.Count; i++)
                {
                    yield return new WaitForEndOfFrame();
                    GameObject L_ItemBtnObj = Instantiate(ItemsBtnPrefab, TempSubcategoryParent.transform);
                    L_ItemBtnObj.transform.parent = TempSubcategoryParent.transform;
                    L_ItemBtnObj.transform.localScale = new Vector3(1, 1, 1);
                    ItemDetail abc = L_ItemBtnObj.GetComponent<ItemDetail>();
                    abc.id = (i + 1).ToString();
                    abc.isFavourite = "False";
                    abc.isOccupied = "False";
                    abc.isPaid = "False";
                    abc.isPurchased = "true";
                    abc.name = CharcterBodyParts.instance.eyeBrowsColor[i].ToString();
                    abc.price = "0";
                    abc.categoryId = "2";
                    abc.subCategory = "16";
                    abc.MyIndex = i;
                    abc.CategoriesEnumVar = TempEnumVar;
                    UpdateCategoryDownloadedInt(TempEnumVar);
                    TempitemDetail.Add(abc);
                    L_ItemBtnObj.SetActive(true);
                    if (abc.transform.parent.gameObject.activeSelf)
                    {

                        abc.StartRun();
                        abc.enableUpdate = true;
                    }
                    else
                    {
                        abc.enableUpdate = true;
                    }
                }
            }
            else if (TempEnumVar == EnumClass.CategoryEnum.EyesAvatarColor)
            {
                int loopStart = GetDownloadedNumber(TempEnumVar);
                for (int i = loopStart; i < CharcterBodyParts.instance.eyeColor.Count; i++)
                {
                    yield return new WaitForEndOfFrame();
                    GameObject L_ItemBtnObj = Instantiate(ItemsBtnPrefab, TempSubcategoryParent.transform);
                    L_ItemBtnObj.transform.parent = TempSubcategoryParent.transform;
                    L_ItemBtnObj.transform.localScale = new Vector3(1, 1, 1);
                    ItemDetail abc = L_ItemBtnObj.GetComponent<ItemDetail>();
                    abc.id = (i + 1).ToString();
                    abc.isFavourite = "False";
                    abc.isOccupied = "False";
                    abc.isPaid = "False";
                    abc.isPurchased = "true";
                    abc.name = CharcterBodyParts.instance.eyeColor[i].ToString();
                    abc.price = "0";
                    abc.categoryId = "2";
                    abc.subCategory = "16";
                    abc.MyIndex = i;
                    abc.CategoriesEnumVar = TempEnumVar;
                    UpdateCategoryDownloadedInt(TempEnumVar);
                    TempitemDetail.Add(abc);
                    L_ItemBtnObj.SetActive(true);
                    if (abc.transform.parent.gameObject.activeSelf)
                    {

                        abc.StartRun();
                        abc.enableUpdate = true;
                    }
                    else
                    {
                        abc.enableUpdate = true;
                    }
                }
            }
            else if (TempEnumVar == EnumClass.CategoryEnum.LipsAvatarColor)
            {
                int loopStart = GetDownloadedNumber(TempEnumVar);
                for (int i = loopStart; i < CharcterBodyParts.instance.lipColorPalette.Count; i++)
                {
                    yield return new WaitForEndOfFrame();
                    GameObject L_ItemBtnObj = Instantiate(ItemsBtnPrefab, TempSubcategoryParent.transform);
                    L_ItemBtnObj.transform.parent = TempSubcategoryParent.transform;
                    L_ItemBtnObj.transform.localScale = new Vector3(1, 1, 1);
                    ItemDetail abc = L_ItemBtnObj.GetComponent<ItemDetail>();
                    abc.id = (i + 1).ToString();
                    abc.isFavourite = "False";
                    abc.isOccupied = "False";
                    abc.isPaid = "False";
                    abc.isPurchased = "true";
                    abc.name = CharcterBodyParts.instance.lipColorPalette[i].ToString();
                    abc.price = "0";
                    abc.categoryId = "2";
                    abc.subCategory = "16";
                    abc.MyIndex = i;
                    abc.CategoriesEnumVar = TempEnumVar;
                    UpdateCategoryDownloadedInt(TempEnumVar);
                    TempitemDetail.Add(abc);
                    L_ItemBtnObj.SetActive(true);
                    if (abc.transform.parent.gameObject.activeSelf)
                    {

                        abc.StartRun();
                        abc.enableUpdate = true;
                    }
                    else
                    {
                        abc.enableUpdate = true;
                    }
                }
            }
            else
            {
                int loopStart = GetDownloadedNumber(TempEnumVar);
                for (int i = loopStart; i < dataListOfItems.Count; i++)
                {
                    yield return new WaitForEndOfFrame();
                    GameObject L_ItemBtnObj = Instantiate(ItemsBtnPrefab, TempSubcategoryParent.transform);
                    L_ItemBtnObj.transform.parent = TempSubcategoryParent.transform;
                    L_ItemBtnObj.transform.localScale = new Vector3(1, 1, 1);
                    ItemDetail abc = L_ItemBtnObj.GetComponent<ItemDetail>();
                    abc.iconLink = dataListOfItems[i].iconLink;
                    abc.id = dataListOfItems[i].id.ToString();
                    abc.isFavourite = dataListOfItems[i].isFavourite.ToString();
                    abc.isOccupied = dataListOfItems[i].isOccupied.ToString();
                    abc.isPaid = dataListOfItems[i].isPaid.ToString();
                    abc.isPurchased = dataListOfItems[i].isPurchased.ToString();
                    abc.name = dataListOfItems[i].name;
                    abc.price = dataListOfItems[i].price;
                    abc.categoryId = dataListOfItems[i].categoryId.ToString();
                    abc.subCategory = dataListOfItems[i].subCategoryId.ToString();
                    abc.itemTags = dataListOfItems[i].itemTags;
                    abc.MyIndex = i;
                    abc.CategoriesEnumVar = TempEnumVar;
                    UpdateCategoryDownloadedInt(TempEnumVar);
                    TempitemDetail.Add(abc);
                    L_ItemBtnObj.SetActive(true);
                    if (abc.transform.parent.gameObject.activeSelf)
                    {

                        abc.StartRun();
                        abc.enableUpdate = true;
                    }
                    else
                    {
                        abc.enableUpdate = true;
                    }
                }
            }

            if (TempitemDetail.Count > 0)
            {
                switch (TempEnumVar)
                {
                    case EnumClass.CategoryEnum.Head:
                        {
                            CategorieslistHeads = TempitemDetail;
                            break;
                        }
                    case EnumClass.CategoryEnum.Face:
                        {
                            CategorieslistFace = TempitemDetail;
                            break;
                        }
                    case EnumClass.CategoryEnum.Inner:
                        {
                            CategorieslistInner = TempitemDetail;
                            break;
                        }
                    case EnumClass.CategoryEnum.Outer:
                        {
                            CategorieslistOuter = TempitemDetail;
                            break;
                        }
                    case EnumClass.CategoryEnum.Accesary:
                        {
                            CategorieslistAccesary = TempitemDetail;
                            break;
                        }
                    case EnumClass.CategoryEnum.Bottom:
                        {
                            CategorieslistBottom = TempitemDetail;
                            break;
                        }
                    case EnumClass.CategoryEnum.Socks:
                        {
                            CategorieslistSocks = TempitemDetail;
                            break;
                        }
                    case EnumClass.CategoryEnum.Shoes:
                        {
                            CategorieslistShoes = TempitemDetail;
                            break;
                        }
                    case EnumClass.CategoryEnum.HairAvatar:
                        {
                            CategorieslistHairs = TempitemDetail;
                            break;
                        }
                    case EnumClass.CategoryEnum.HairAvatarColor:
                        {
                            CategorieslistHairsColors = TempitemDetail;
                            break;
                        }
                    case EnumClass.CategoryEnum.EyesAvatar:
                        {
                            CategorieslistEyesColor = TempitemDetail;
                            break;
                        }
                    case EnumClass.CategoryEnum.LipsAvatar:
                        {
                            CategorieslistLipsColor = TempitemDetail;
                            break;
                        }
                    case EnumClass.CategoryEnum.SkinToneAvatar:
                        {
                            CategorieslistSkinToneColor = TempitemDetail;
                            break;
                        }
                }
                UpdateColor(buttonIndex);
                if (buttonIndex != -1)
                {
                    //print("Panel Selected: " + panelIndex);
                    UpdateStoreSelection(buttonIndex);
                }
            }



        }

        //else
        //{
        //    UpdateStoreSelection(0);
        //}
    }

    int GetDownloadedNumber(EnumClass.CategoryEnum categoryEnum)
    {
        switch (TempEnumVar)
        {
            case EnumClass.CategoryEnum.Head:
                return headsDownlaodedCount;
            case EnumClass.CategoryEnum.Face:
                return faceDownlaodedCount;
            case EnumClass.CategoryEnum.Inner:
                return innerDownlaodedCount;
            case EnumClass.CategoryEnum.Outer:
                return outerDownlaodedCount;
            case EnumClass.CategoryEnum.Accesary:
                return accesaryDownlaodedCount;
            case EnumClass.CategoryEnum.Bottom:
                return bottomDownlaodedCount;
            case EnumClass.CategoryEnum.Socks:
                return socksDownlaodedCount;
            case EnumClass.CategoryEnum.Shoes:
                return shoesDownlaodedCount;
            case EnumClass.CategoryEnum.HairAvatar:
                return hairDwonloadedCount;
            case EnumClass.CategoryEnum.HairAvatarColor:
                return HairColorDwonloadedCount;
            case EnumClass.CategoryEnum.EyesAvatarColor:
                return EyesColorDwonloadedCount;
            case EnumClass.CategoryEnum.LipsAvatarColor:
                return LipsColorDwonloadedCount;
            case EnumClass.CategoryEnum.SkinToneAvatar:
                return skinColorDwonloadedCount;
            case EnumClass.CategoryEnum.EyeBrowAvatar:
                return eyeBrowDwonloadedCount;
            case EnumClass.CategoryEnum.EyeBrowAvatarColor:
                return eyeBrowColorDwonloadedCount;
            case EnumClass.CategoryEnum.EyeLashesAvatar:
                return eyeLashesDwonloadedCount;
            case EnumClass.CategoryEnum.EyesAvatar:
                return eyesDwonloadedCount;
            case EnumClass.CategoryEnum.LipsAvatar:
                return lipsDwonloadedCount;

        }
        return 0;
    }

    void UpdateCategoryDownloadedInt(EnumClass.CategoryEnum TempEnumVar)
    {
        switch (TempEnumVar)
        {
            case EnumClass.CategoryEnum.Head:
                {
                    headsDownlaodedCount++;
                    break;
                }
            case EnumClass.CategoryEnum.Face:
                {
                    faceDownlaodedCount++;
                    break;
                }
            case EnumClass.CategoryEnum.Inner:
                {
                    innerDownlaodedCount++;
                    break;
                }
            case EnumClass.CategoryEnum.Outer:
                {
                    outerDownlaodedCount++;
                    break;
                }
            case EnumClass.CategoryEnum.Accesary:
                {
                    accesaryDownlaodedCount++;
                    break;
                }
            case EnumClass.CategoryEnum.Bottom:
                {
                    bottomDownlaodedCount++;
                    break;
                }
            case EnumClass.CategoryEnum.Socks:
                {
                    socksDownlaodedCount++;
                    break;
                }
            case EnumClass.CategoryEnum.Shoes:
                {
                    shoesDownlaodedCount++;
                    break;
                }
            case EnumClass.CategoryEnum.HairAvatar:
                {
                    hairDwonloadedCount++;
                    break;
                }
            case EnumClass.CategoryEnum.HairAvatarColor:
                {
                    HairColorDwonloadedCount++;
                    break;
                }
            case EnumClass.CategoryEnum.EyesAvatarColor:
                {
                    EyesColorDwonloadedCount++;
                    break;
                }
            case EnumClass.CategoryEnum.LipsAvatarColor:
                {
                    LipsColorDwonloadedCount++;
                    break;
                }
            case EnumClass.CategoryEnum.SkinToneAvatar:
                {
                    skinColorDwonloadedCount++;
                    break;
                }
            case EnumClass.CategoryEnum.EyeBrowAvatar:
                {
                    eyeBrowDwonloadedCount++;
                    break;
                }
            case EnumClass.CategoryEnum.EyeBrowAvatarColor:
                {
                    eyeBrowColorDwonloadedCount++;
                    break;
                }
            case EnumClass.CategoryEnum.EyeLashesAvatar:
                {
                    eyeLashesDwonloadedCount++;
                    break;
                }
            case EnumClass.CategoryEnum.EyesAvatar:
                {
                    eyesDwonloadedCount++;
                    break;
                }
            case EnumClass.CategoryEnum.LipsAvatar:
                {
                    lipsDwonloadedCount++;
                    break;
                }

        }
    }

    public void LoadLocalItems()
    {

    }
    //UNDO REDO FUNCTIONALITY------------------

    public List<UndoRedoDataClass> UndoRedoList = new List<UndoRedoDataClass>();
    public int CurrentIndex = -1;
    [Serializable]
    public class UndoRedoDataClass
    {
        public Item ClothTex_Item = new Item();
    }

    IEnumerator CharacterChange()
    {
        yield return new WaitForSeconds(3.5f);

        UpdateStoreSelection(0);
    }
    private void OnApplicationQuit()
    {
        //PresetData_Jsons.lastSelectedPresetName = null;
    }

    // AR change start
    public void ForcellySetLastClickedBtnOfHair()
    {
        for (int i = 0; i < ParentOfBtnsAvatarHairs.transform.childCount; i++)
        {
            childObject = ParentOfBtnsAvatarHairs.transform.GetChild(i).gameObject;
            if (childObject.GetComponent<ItemDetail>().id == XanaConstants.xanaConstants.hair)
            {
                childObject.GetComponent<Image>().enabled = true;
                XanaConstants.xanaConstants._lastClickedBtn = childObject;
                //Debug.Log("<color=red>StoreManager AssignLastClickedBtnHere</color>");
                XanaConstants.xanaConstants.avatarStoreSelection[0] = childObject;

                CheckForItemDetail(XanaConstants.xanaConstants.hair, 3);
            }
        }
    }
    // AR changes end

    public void UpdateStoreSelection(int index)
    {
        ////Debug.Log("<color=blue>Update Store Selection: "+index+"</color>");
        switch (index)
        {
            case 0:
                GameManager.Instance.mainCharacter.GetComponent<FaceIK>().SetLookPos(1);
                if (XanaConstants.xanaConstants.hair != "")
                {
                    if (GameManager.Instance.mainCharacter.GetComponent<AvatarController>().wornHair.name == "MDhairs")
                    {
                        // //Debug.Log("Hairs list------"+ ParentOfBtnsAvatarHairs.transform.childCount);
                        ////Debug.Log("<color=blue>Store Selection if</color>");
                        for (int i = 0; i < ParentOfBtnsAvatarHairs.transform.childCount; i++)
                        {
                            childObject = ParentOfBtnsAvatarHairs.transform.GetChild(i).gameObject;
                            if (childObject.GetComponent<Image>().enabled)
                                childObject.GetComponent<Image>().enabled = false;
                        }



                    }
                    else if (!ParentOfBtnsCustomHair.gameObject.activeSelf)
                    {
                        // //Debug.Log("<color=blue>Store Selection else</color>");
                        ////Debug.Log("Hairs list 2------" + ParentOfBtnsAvatarHairs.transform.childCount);
                        for (int i = 0; i < ParentOfBtnsAvatarHairs.transform.childCount; i++)
                        {
                            childObject = ParentOfBtnsAvatarHairs.transform.GetChild(i).gameObject;
                            if (childObject.GetComponent<ItemDetail>().id == XanaConstants.xanaConstants.hair)
                            {
                                //  //Debug.Log("<color=blue>Enabled Selection</color>");
                                childObject.GetComponent<Image>().enabled = true;
                                XanaConstants.xanaConstants._lastClickedBtn = childObject;
                                // //Debug.Log("<color=red>StoreManager AssignLastClickedBtnHere</color>");
                                XanaConstants.xanaConstants.avatarStoreSelection[0] = childObject;

                                CheckForItemDetail(XanaConstants.xanaConstants.hair, 3);

                                break;
                            }
                        }
                    }



                }

                break;

            case 1:
                GameManager.Instance.mainCharacter.GetComponent<FaceIK>().SetLookPos(1);
                if (!XanaConstants.xanaConstants.isFaceMorphed)
                {
                    if (XanaConstants.xanaConstants.faceIndex != -1)
                    {
                        for (int i = 0; i < faceAvatarButton.Length; i++)
                        {
                            if (faceAvatarButton[i].GetComponent<AvatarBtn>().AvatarBtnId == XanaConstants.xanaConstants.faceIndex)
                            {
                                faceAvatarButton[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                                XanaConstants.xanaConstants._lastAvatarClickedBtn = faceAvatarButton[i];
                                XanaConstants.xanaConstants.avatarStoreSelection[1] = faceAvatarButton[i];

                                CheckForAvatarBtn(XanaConstants.xanaConstants.faceIndex, "face");
                                break;
                            }
                        }
                    }

                    //else
                    //{

                    //    int childNumber = ParentOfBtnsAvatarFace.transform.childCount - 1;

                    //    ParentOfBtnsAvatarFace.transform.GetChild(childNumber).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                    //    XanaConstants.xanaConstants._lastClickedBtn = ParentOfBtnsAvatarFace.transform.GetChild(childNumber).gameObject;
                    //    XanaConstants.xanaConstants.avatarStoreSelection[1] = ParentOfBtnsAvatarFace.transform.GetChild(childNumber).gameObject;

                    //    CheckForAvatarBtn(0, "face");
                    //}
                }

                else
                {
                    faceTapButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);

                    if (XanaConstants.xanaConstants._lastClickedBtn)
                    {
                        if (XanaConstants.xanaConstants._lastClickedBtn.name == faceTapButton.name)
                        {
                            XanaConstants.xanaConstants._lastClickedBtn = null;
                            XanaConstants.xanaConstants.avatarStoreSelection[1] = null;
                        }
                    }

                    saveButtonPressed = false;
                }

                break;

            case 2:
                GameManager.Instance.mainCharacter.GetComponent<FaceIK>().SetLookPos(1);
                if (!XanaConstants.xanaConstants.isEyebrowMorphed)
                {
                    if (XanaConstants.xanaConstants.eyeBrowIndex != -1)
                    {
                        for (int i = 0; i < eyeBrowsAvatarButton.Length; i++)
                        {
                            if (eyeBrowsAvatarButton[i].GetComponent<AvatarBtn>().AvatarBtnId == XanaConstants.xanaConstants.eyeBrowIndex)
                            {
                                eyeBrowsAvatarButton[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                                XanaConstants.xanaConstants._lastAvatarClickedBtn = eyeBrowsAvatarButton[i];
                                XanaConstants.xanaConstants.avatarStoreSelection[2] = eyeBrowsAvatarButton[i];

                                CheckForAvatarBtn(XanaConstants.xanaConstants.eyeBrowIndex, "eyeBrow");

                                break;
                            }
                        }
                        for (int i = 1; i < ParentOfBtnsAvatarEyeBrows.childCount; i++)
                        {
                            childObject = ParentOfBtnsAvatarEyeBrows.GetChild(i).gameObject;
                            if (childObject.GetComponent<ItemDetail>().id.ParseToInt() == SavaCharacterProperties.instance.characterController.eyeBrowId)
                            {
                                childObject.GetComponent<Image>().enabled = true;
                                XanaConstants.xanaConstants._lastClickedBtn = childObject;
                            }
                            else
                                childObject.GetComponent<Image>().enabled = false;
                        }

                        // Activate Eyebrow Customization Btn
                        if (ParentOfBtnsAvatarEyeBrows.childCount > 2)
                            eyeBrowTapButton.SetActive(true);
                    }

                    //else
                    //{

                    //    int childNumber = ParentOfBtnsAvatarEyeBrows.transform.childCount - 1;
                    //    print("child number is = " + childNumber);
                    //    ParentOfBtnsAvatarEyeBrows.transform.GetChild(childNumber).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                    //    XanaConstants.xanaConstants._lastClickedBtn = ParentOfBtnsAvatarEyeBrows.transform.GetChild(childNumber).gameObject;
                    //    XanaConstants.xanaConstants.avatarStoreSelection[2] = ParentOfBtnsAvatarEyeBrows.transform.GetChild(childNumber).gameObject;

                    //    CheckForAvatarBtn(0, "eyeBrow");
                    //}
                }

                else
                {
                    eyeBrowTapButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);

                    if (XanaConstants.xanaConstants._lastClickedBtn)
                    {
                        if (XanaConstants.xanaConstants._lastClickedBtn.name == eyeBrowTapButton.name)
                        {
                            XanaConstants.xanaConstants._lastClickedBtn = null;
                            XanaConstants.xanaConstants.avatarStoreSelection[2] = null;
                        }
                    }

                    saveButtonPressed = false;
                }

                break;

            case 3:
                GameManager.Instance.mainCharacter.GetComponent<FaceIK>().SetLookPos(1);
                if (panelIndex == 1)
                {
                    if (!XanaConstants.xanaConstants.isEyeMorphed)
                    {
                        if (XanaConstants.xanaConstants.eyeIndex != -1)
                        {
                            for (int i = 0; i < eyeAvatarButton.Length; i++)
                            {
                                if (eyeAvatarButton[i].GetComponent<AvatarBtn>().AvatarBtnId == XanaConstants.xanaConstants.eyeIndex)
                                {
                                    eyeAvatarButton[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                                    XanaConstants.xanaConstants._lastAvatarClickedBtn = eyeAvatarButton[i];
                                    XanaConstants.xanaConstants.avatarStoreSelection[3] = eyeAvatarButton[i];

                                    CheckForAvatarBtn(XanaConstants.xanaConstants.eyeIndex, "eye");

                                    break;
                                }
                            }
                        }

                        //else
                        //{

                        //    int childNumber = ParentOfBtnsAvatarEyes.transform.childCount - 1;

                        //    ParentOfBtnsAvatarEyes.transform.GetChild(childNumber).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                        //    XanaConstants.xanaConstants._lastClickedBtn = ParentOfBtnsAvatarEyes.transform.GetChild(childNumber).gameObject;
                        //    XanaConstants.xanaConstants.avatarStoreSelection[3] = ParentOfBtnsAvatarEyes.transform.GetChild(childNumber).gameObject;

                        //    CheckForAvatarBtn(0, "eye");
                        //}
                    }

                    else
                    {
                        eyeTapButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);

                        if (XanaConstants.xanaConstants._lastClickedBtn)
                        {
                            if (XanaConstants.xanaConstants._lastClickedBtn.name == eyeTapButton.name)
                            {
                                XanaConstants.xanaConstants._lastClickedBtn = null;
                                XanaConstants.xanaConstants.avatarStoreSelection[3] = null;
                            }
                        }

                        saveButtonPressed = false;
                    }
                }

                else
                {
                    if (XanaConstants.xanaConstants.shirt != "")
                    {
                        if (GameManager.Instance.mainCharacter.GetComponent<AvatarController>().wornShirt.name == "MDshirt")
                        {
                            for (int i = 0; i < ParentOfBtnsForOuter.transform.childCount; i++)
                            {
                                childObject = ParentOfBtnsForOuter.transform.GetChild(i).gameObject;
                                if (childObject.GetComponent<Image>().enabled)
                                    childObject.GetComponent<Image>().enabled = false;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < ParentOfBtnsForOuter.transform.childCount; i++)
                            {
                                childObject = ParentOfBtnsForOuter.transform.GetChild(i).gameObject;
                                if (childObject.GetComponent<ItemDetail>().id == XanaConstants.xanaConstants.shirt)
                                {
                                    childObject.GetComponent<Image>().enabled = true;
                                    XanaConstants.xanaConstants._lastClickedBtn = childObject;
                                    XanaConstants.xanaConstants.wearableStoreSelection[0] = childObject;

                                    CheckForItemDetail(XanaConstants.xanaConstants.shirt, 1);

                                    break;
                                }
                            }
                        }
                    }
                }

                break;

            case 4:
                GameManager.Instance.mainCharacter.GetComponent<FaceIK>().SetLookPos(1);
                if (panelIndex == 1)
                {
                    if (!XanaConstants.xanaConstants.isNoseMorphed)
                    {
                        if (XanaConstants.xanaConstants.noseIndex != -1)
                        {
                            for (int i = 0; i < noseAvatarButton.Length; i++)
                            {
                                if (noseAvatarButton[i].GetComponent<AvatarBtn>().AvatarBtnId == XanaConstants.xanaConstants.noseIndex)
                                {
                                    noseAvatarButton[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                                    XanaConstants.xanaConstants._lastAvatarClickedBtn = noseAvatarButton[i];
                                    XanaConstants.xanaConstants.avatarStoreSelection[4] = noseAvatarButton[i];

                                    CheckForAvatarBtn(XanaConstants.xanaConstants.noseIndex, "nose");

                                    break;
                                }
                            }
                        }

                        //else
                        //{

                        //    int childNumber = ParentOfBtnsAvatarNose.transform.childCount - 1;

                        //    ParentOfBtnsAvatarNose.transform.GetChild(childNumber).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                        //    XanaConstants.xanaConstants._lastClickedBtn = ParentOfBtnsAvatarNose.transform.GetChild(childNumber).gameObject;
                        //    XanaConstants.xanaConstants.avatarStoreSelection[4] = ParentOfBtnsAvatarNose.transform.GetChild(childNumber).gameObject;

                        //    CheckForAvatarBtn(0, "nose");
                        //}
                    }

                    else
                    {
                        noseTapButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);

                        if (XanaConstants.xanaConstants._lastClickedBtn)
                        {
                            if (XanaConstants.xanaConstants._lastClickedBtn.name == noseTapButton.name)
                            {
                                XanaConstants.xanaConstants._lastClickedBtn = null;
                                XanaConstants.xanaConstants.avatarStoreSelection[4] = null;
                            }
                        }

                        saveButtonPressed = false;
                    }
                }
                else
                {
                    if (XanaConstants.xanaConstants.PresetValueString != "")
                    {
                        for (int i = 0; i < ParentOfBtnsForAccesary.transform.childCount; i++)
                        {
                            childObject = ParentOfBtnsForAccesary.transform.GetChild(i).gameObject;
                            childObject.transform.GetChild(0).gameObject.SetActive(false);
                            if (childObject.name == XanaConstants.xanaConstants.PresetValueString)
                            {
                                XanaConstants.xanaConstants._lastClickedBtn = childObject;
                                childObject.transform.GetChild(0).gameObject.SetActive(true);
                                XanaConstants.xanaConstants.wearableStoreSelection[3] = childObject;
                            }
                        }
                    }
                }
                break;

            case 5:
                GameManager.Instance.mainCharacter.GetComponent<FaceIK>().SetLookPos(1);
                if (panelIndex == 1)
                {
                    if (!XanaConstants.xanaConstants.isLipMorphed)
                    {
                        if (XanaConstants.xanaConstants.lipIndex != -1)
                        {
                            for (int i = 0; i < lipAvatarButton.Length; i++)
                            {
                                if (lipAvatarButton[i].GetComponent<AvatarBtn>().AvatarBtnId == XanaConstants.xanaConstants.lipIndex)
                                {
                                    lipAvatarButton[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                                    XanaConstants.xanaConstants._lastAvatarClickedBtn = lipAvatarButton[i];
                                    XanaConstants.xanaConstants.avatarStoreSelection[5] = lipAvatarButton[i];

                                    CheckForAvatarBtn(XanaConstants.xanaConstants.lipIndex, "lip");

                                    break;
                                }
                            }
                        }

                        //else
                        //{

                        //    int childNumber = ParentOfBtnsAvatarLips.transform.childCount - 1;

                        //    ParentOfBtnsAvatarLips.transform.GetChild(childNumber).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                        //    XanaConstants.xanaConstants._lastClickedBtn = ParentOfBtnsAvatarLips.transform.GetChild(childNumber).gameObject;
                        //    XanaConstants.xanaConstants.avatarStoreSelection[5] = ParentOfBtnsAvatarLips.transform.GetChild(childNumber).gameObject;

                        //    CheckForAvatarBtn(0, "lip");
                        //}
                    }

                    else
                    {
                        lipTapButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);

                        if (XanaConstants.xanaConstants._lastClickedBtn)
                        {
                            if (XanaConstants.xanaConstants._lastClickedBtn.name == lipTapButton.name)
                            {
                                XanaConstants.xanaConstants._lastClickedBtn = null;
                                XanaConstants.xanaConstants.avatarStoreSelection[5] = null;
                            }
                        }

                        saveButtonPressed = false;
                    }
                }

                else
                {
                    if (XanaConstants.xanaConstants.pants != "")
                    {
                        ////Debug.Log(ParentOfBtnsForOuter.transform.childCount);
                        if (GameManager.Instance.mainCharacter.GetComponent<AvatarController>().wornPant.name == "MDpant")
                        {
                            for (int i = 0; i < ParentOfBtnsForBottom.transform.childCount; i++)
                            {
                                childObject = ParentOfBtnsForBottom.transform.GetChild(i).gameObject;
                                if (childObject.GetComponent<Image>().enabled)
                                    childObject.GetComponent<Image>().enabled = false;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < ParentOfBtnsForBottom.transform.childCount; i++)
                            {
                                childObject = ParentOfBtnsForBottom.transform.GetChild(i).gameObject;
                                if (childObject.GetComponent<ItemDetail>().id == XanaConstants.xanaConstants.pants)
                                {
                                    childObject.GetComponent<Image>().enabled = true;
                                    XanaConstants.xanaConstants._lastClickedBtn = childObject;
                                    XanaConstants.xanaConstants.wearableStoreSelection[1] = childObject;

                                    CheckForItemDetail(XanaConstants.xanaConstants.pants, 0);

                                    break;
                                }
                            }
                        }
                    }
                }

                break;

            case 6:
                GameManager.Instance.mainCharacter.GetComponent<FaceIK>().SetLookPos(2);
                if (XanaConstants.xanaConstants.bodyNumber != -1)
                {
                    for (int i = 0; i < ParentOfBtnsAvatarBody.transform.childCount; i++)
                    {
                        childObject = ParentOfBtnsAvatarBody.transform.GetChild(i).gameObject;
                        if (childObject.GetComponent<AvatarBtn>()._Bodyint == XanaConstants.xanaConstants.bodyNumber)
                        {
                            childObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                            XanaConstants.xanaConstants._lastAvatarClickedBtn = childObject;
                            XanaConstants.xanaConstants.avatarStoreSelection[6] = childObject;
                            break;
                        }
                    }
                }

                break;

            case 7:
                GameManager.Instance.mainCharacter.GetComponent<FaceIK>().SetLookPos(2);
                if (panelIndex == 0)
                {
                    if (XanaConstants.xanaConstants.shoes != "")
                    {
                        if (GameManager.Instance.mainCharacter.GetComponent<AvatarController>().wornShose.name == "MDshoes")
                        {
                            for (int i = 0; i < ParentOfBtnsForShoes.transform.childCount; i++)
                            {
                                childObject = ParentOfBtnsForShoes.transform.GetChild(i).gameObject;
                                if (childObject.GetComponent<Image>().enabled)
                                    childObject.GetComponent<Image>().enabled = false;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < ParentOfBtnsForShoes.transform.childCount; i++)
                            {
                                childObject = ParentOfBtnsForShoes.transform.GetChild(i).gameObject;
                                if (childObject.GetComponent<ItemDetail>().id == XanaConstants.xanaConstants.shoes)
                                {
                                    childObject.GetComponent<Image>().enabled = true;
                                    XanaConstants.xanaConstants._lastClickedBtn = childObject;
                                    XanaConstants.xanaConstants.wearableStoreSelection[2] = childObject;

                                    CheckForItemDetail(XanaConstants.xanaConstants.shoes, 2);

                                    break;
                                }
                            }
                        }
                    }
                }

                break;
            case 8:
                GameManager.Instance.mainCharacter.GetComponent<FaceIK>().SetLookPos(1);
                if (XanaConstants.xanaConstants.eyeLashesIndex != -1)
                {
                    for (int i = 0; i < ParentOfBtnsAvatarEyeLashes.transform.childCount; i++)
                    {
                        childObject = ParentOfBtnsAvatarEyeLashes.transform.GetChild(i).gameObject;
                        // Commented By Ahsan
                        //if (childObject.GetComponent<ItemDetail>().id.ParseToInt() == SavaCharacterProperties.instance.characterController.eyeLashesId)
                        //    childObject.GetComponent<Image>().enabled = true;
                        //else
                        //    childObject.GetComponent<Image>().enabled = false;

                        //if (childObject.GetComponent<AvatarBtn>().AvatarBtnId == XanaConstants.xanaConstants.eyeLashesIndex)
                        if (childObject.GetComponent<ItemDetail>().id == XanaConstants.xanaConstants.eyeLashesIndex.ToString())
                        {
                            childObject.GetComponent<Image>().enabled = true;
                            XanaConstants.xanaConstants._lastClickedBtn = childObject;
                            XanaConstants.xanaConstants.avatarStoreSelection[8] = childObject;
                            break;
                        }
                    }
                }
                break;
            case 9:
                GameManager.Instance.mainCharacter.GetComponent<FaceIK>().SetLookPos(1);
                if (XanaConstants.xanaConstants.makeupIndex != -1)
                {
                    for (int i = 0; i < ParentOfBtnsAvatarMakeup.transform.childCount; i++)
                    {
                        childObject = ParentOfBtnsAvatarMakeup.transform.GetChild(i).gameObject;
                        if (childObject.GetComponent<AvatarBtn>().AvatarBtnId == XanaConstants.xanaConstants.makeupIndex)
                        {
                            childObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                            XanaConstants.xanaConstants._lastAvatarClickedBtn = childObject;
                            XanaConstants.xanaConstants.avatarStoreSelection[9] = childObject;
                            break;
                        }
                    }
                }
                break;
            case 10:
                GameManager.Instance.mainCharacter.GetComponent<FaceIK>().SetLookPos(2);
                if (XanaConstants.xanaConstants.PresetValueString != "")
                {

                    for (int i = 0; i < ParentOfBtnsAvatarAccessary.transform.childCount; i++)
                    {
                        childObject = ParentOfBtnsAvatarAccessary.transform.GetChild(i).gameObject;
                        childObject.transform.GetChild(0).gameObject.SetActive(false);
                        if (childObject.name == XanaConstants.xanaConstants.PresetValueString)
                        {
                            XanaConstants.xanaConstants._lastClickedBtn = childObject;
                            childObject.transform.GetChild(0).gameObject.SetActive(true);
                            XanaConstants.xanaConstants.avatarStoreSelection[XanaConstants.xanaConstants.currentButtonIndex] = childObject;
                        }
                    }
                }
                break;
        }
    }
    public void UndoSelection()
    {
        for (int i = 0; i < XanaConstants.xanaConstants.avatarStoreSelection.Length; i++)
        {
            if (XanaConstants.xanaConstants.avatarStoreSelection[i])
            {
                if (XanaConstants.xanaConstants.avatarStoreSelection[i].GetComponent<ItemDetail>())
                {
                    XanaConstants.xanaConstants.avatarStoreSelection[i].GetComponent<Image>().enabled = false;
                }
                else if (XanaConstants.xanaConstants.avatarStoreSelection[i].GetComponent<PresetData_Jsons>())
                {
                    XanaConstants.xanaConstants.avatarStoreSelection[i].transform.GetChild(0).gameObject.SetActive(false);
                }
                else
                {
                    XanaConstants.xanaConstants.avatarStoreSelection[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
                }

                XanaConstants.xanaConstants.avatarStoreSelection[i] = null;
            }
        }

        for (int i = 0; i < XanaConstants.xanaConstants.wearableStoreSelection.Length; i++)
        {
            if (XanaConstants.xanaConstants.wearableStoreSelection[i])
            {
                if (XanaConstants.xanaConstants.wearableStoreSelection[i].GetComponent<ItemDetail>())
                {
                    XanaConstants.xanaConstants.wearableStoreSelection[i].GetComponent<Image>().enabled = false;
                }
                else if (XanaConstants.xanaConstants.wearableStoreSelection[i].GetComponent<PresetData_Jsons>())
                {
                    XanaConstants.xanaConstants.wearableStoreSelection[i].transform.GetChild(0).gameObject.SetActive(false);
                }
                else
                {
                    XanaConstants.xanaConstants.wearableStoreSelection[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
                }

                XanaConstants.xanaConstants.wearableStoreSelection[i] = null;
            }
        }

        for (int i = 0; i < XanaConstants.xanaConstants.colorSelection.Length; i++)
        {
            if (XanaConstants.xanaConstants.colorSelection[i])
            {
                if (XanaConstants.xanaConstants.colorSelection[i].GetComponent<ItemDetail>())
                {
                    XanaConstants.xanaConstants.colorSelection[i].GetComponent<Image>().enabled = false;
                }

                else
                {
                    XanaConstants.xanaConstants.colorSelection[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
                }

                XanaConstants.xanaConstants.colorSelection[i] = null;
            }
        }
    }

    public void ResetMorphBooleanValues()
    {
        XanaConstants.xanaConstants.isFaceMorphed = SavaCharacterProperties.instance.SaveItemList.faceMorphed;
        XanaConstants.xanaConstants.isEyebrowMorphed = SavaCharacterProperties.instance.SaveItemList.eyeBrowMorphed;
        XanaConstants.xanaConstants.isEyeMorphed = SavaCharacterProperties.instance.SaveItemList.eyeMorphed;
        XanaConstants.xanaConstants.isNoseMorphed = SavaCharacterProperties.instance.SaveItemList.noseMorphed;
        XanaConstants.xanaConstants.isLipMorphed = SavaCharacterProperties.instance.SaveItemList.lipMorphed;

        if (XanaConstants.xanaConstants._lastClickedBtn)
        {
            if (XanaConstants.xanaConstants._lastClickedBtn.GetComponent<AvatarBtn>())
                XanaConstants.xanaConstants._lastClickedBtn.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);

            XanaConstants.xanaConstants._lastClickedBtn = null;

            if (XanaConstants.xanaConstants.currentButtonIndex >= 0 && XanaConstants.xanaConstants.currentButtonIndex < XanaConstants.xanaConstants.avatarStoreSelection.Length)
                XanaConstants.xanaConstants.avatarStoreSelection[XanaConstants.xanaConstants.currentButtonIndex] = null;
        }

        if (ParentOfBtnsCustomEyes.gameObject.activeInHierarchy)
            OnColorButtonClicked(XanaConstants.xanaConstants.currentButtonIndex);
        else if (ParentOfBtnsCustomLips.gameObject.activeInHierarchy)
            OnColorButtonClicked(XanaConstants.xanaConstants.currentButtonIndex);
        else if (ParentOfBtnsCustomSkin.gameObject.activeInHierarchy)
            OnColorButtonClicked(XanaConstants.xanaConstants.currentButtonIndex);
        else
            UpdateStoreSelection(XanaConstants.xanaConstants.currentButtonIndex);

        ////Debug.Log("IsLoggedIn " + PlayerPrefs.GetInt("IsLoggedIn"));
        if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
        {
            if (isSaveFromreturnHomePopUp)
            {
                OnClickHomeButton();
            }
        }
    }

    public void DisableColorPanels()
    {
        ParentOfBtnsCustomEyes.gameObject.SetActive(false);
        ParentOfBtnsCustomLips.gameObject.SetActive(false);
        ParentOfBtnsCustomSkin.gameObject.SetActive(true);
    }


    // Enable Save Button if the character record is changed. 
    public void CheckForItemDetail(string currentId, int idIndex)
    {
        if (!saveButtonPressed)
        {
            if (File.Exists(GameManager.Instance.GetStringFolderPath()) && File.ReadAllText(GameManager.Instance.GetStringFolderPath()) != "")
            {
                SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();

                _CharacterData = _CharacterData.CreateFromJSON(File.ReadAllText(GameManager.Instance.GetStringFolderPath()));

                //if (_CharacterData.myItemObj.Count != 0)
                if (_CharacterData.myItemObj.Count > idIndex)
                {
                    if (currentId == _CharacterData.myItemObj[idIndex].ItemID.ToString())
                    {
                        ActivateSaveButton(false);
                    }

                    else
                    {
                        ActivateSaveButton(true);
                    }
                }
            }
        }

        else
        {
            saveButtonPressed = false;
        }
    }
    public void CheckForAvatarBtn(int currentId, string itemType)
    {
        if (!saveButtonPressed)
        {
            if (File.Exists(GameManager.Instance.GetStringFolderPath()) && File.ReadAllText(GameManager.Instance.GetStringFolderPath()) != "")
            {
                SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();

                _CharacterData = _CharacterData.CreateFromJSON(File.ReadAllText(GameManager.Instance.GetStringFolderPath()));

                bool itemIsSaved = false;

                switch (itemType)
                {
                    case "face":
                        if (currentId == _CharacterData.FaceValue)
                        {
                            itemIsSaved = true;
                        }
                        break;

                    case "eyeBrow":
                        if (currentId == _CharacterData.EyeBrowValue)
                        {
                            itemIsSaved = true;
                        }
                        break;

                    case "eye":
                        if (currentId == _CharacterData.EyeValue)
                        {
                            itemIsSaved = true;
                        }
                        break;

                    case "nose":
                        if (currentId == _CharacterData.NoseValue)
                        {
                            itemIsSaved = true;
                        }
                        break;

                    case "lip":
                        if (currentId == _CharacterData.LipsValue)
                        {
                            itemIsSaved = true;
                        }
                        break;
                }

                if (itemIsSaved)
                {
                    ActivateSaveButton(false);
                }

                else
                {
                    ActivateSaveButton(true);
                }
            }
        }

        else
        {
            saveButtonPressed = false;
        }
    }

    public void ResetSaveFile()
    {
        if (File.Exists(GameManager.Instance.GetStringFolderPath()) && File.ReadAllText(GameManager.Instance.GetStringFolderPath()) != "")
        {
            CharcterBodyParts bodyParts = GameManager.Instance.mainCharacter.GetComponent<CharcterBodyParts>();
            SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();
            _CharacterData = _CharacterData.CreateFromJSON(File.ReadAllText(GameManager.Instance.GetStringFolderPath()));
            _CharacterData.myItemObj.Clear();

            if (_CharacterData.FaceBlendsShapes != null)
                for (int i = 0; i < _CharacterData.FaceBlendsShapes.Length; i++)
                {
                    _CharacterData.FaceBlendsShapes[i] = 0;
                }
            _CharacterData.SavedBones.Clear();

            _CharacterData.eyeTextureName = "";
            _CharacterData.Skin = bodyParts.DefaultSkinColor;
            _CharacterData.LipColor = bodyParts.DefaultLipColor;
            _CharacterData.EyebrowColor = bodyParts.DefaultEyebrowColor;
            _CharacterData.EyebrowColor = Color.white;
            _CharacterData.HairColor = bodyParts.DefaultHairColor;
            _CharacterData.HairColorPaletteValue = 0;
            _CharacterData.MakeupValue = 0;
            _CharacterData.EyeLashesValue = 0;
            _CharacterData.FaceValue = 0;
            _CharacterData.EyeBrowValue = 0;
            _CharacterData.EyeBrowColorPaletteValue = 0;
            _CharacterData.EyeValue = 0;
            _CharacterData.EyesColorValue = 0;
            _CharacterData.EyesColorPaletteValue = 0;
            _CharacterData.LipsValue = 0;
            _CharacterData.LipsColorValue = 0;
            _CharacterData.LipsColorPaletteValue = 0;
            _CharacterData.NoseValue = 0;
            _CharacterData.BodyFat = 0;
            _CharacterData.SkinId = -1;
            _CharacterData.PresetValue = "";
            _CharacterData.makeupName = bodyParts.defaultMakeup.name;
            _CharacterData.eyeLashesName = bodyParts.defaultEyelashes.name;
            //_CharacterData.eyebrrowTexture = "";
            _CharacterData.eyebrrowTexture = bodyParts.defaultEyebrow.name;
            //_CharacterData.eyeBrowName = bodyParts.defaultEyebrow.name;

            string bodyJson = JsonUtility.ToJson(_CharacterData);
            File.WriteAllText(GameManager.Instance.GetStringFolderPath(), bodyJson);
            if (PlayerPrefs.GetInt("IsLoggedIn") == 1)
                ServerSIdeCharacterHandling.Instance.CreateUserOccupiedAsset(() =>
                {
                });
        }

        //print("in delete");
        //File.Delete(GameManager.Instance.GetStringFolderPath());
    }

    public void ActivateSaveButton(bool activate)
    {
        //if (!activate)
        //{
        //    SaveStoreBtn.SetActive(true);
        //    saveStoreBtnButton.interactable = false;
        //    saveStoreBtnImage.color = Color.white;
        //    GreyRibbonImage.SetActive(true);
        //    WhiteRibbonImage.SetActive(false);
        //}

        //else
        //{
        //    saveStoreBtnButton.interactable = true;
        //    SaveStoreBtn.SetActive(true);
        //    saveStoreBtnImage.color = new Color(0f, 0.5f, 1f, 0.8f);
        //    GreyRibbonImage.SetActive(false);
        //    WhiteRibbonImage.SetActive(true);
        //}
    }



    /// <summary>
    /// Undo Selection
    /// </summary>
    public void UndoStepBtn()
    {
        //print("undo call");
        UndoClicked = true;
        if (EyesBlinking.instance)
            EyesBlinking.instance.StoreBlendShapeValues();          // Added by Ali Hamza
    }

    /// <summary>
    /// Redo Selection
    /// </summary>
    public void RedoStepBtn()
    {
        //print("redo call");
        RedoClicked = true;
        if (EyesBlinking.instance)
            EyesBlinking.instance.StoreBlendShapeValues();          // Added by Ali Hamza
    }
    public void UpdateXanaConstants()
    {
        ////Debug.Log("<color=red> Update Xana Constant </color>");
        //if (SavaCharacterProperties.instance.SaveItemList.SavedBones.Count == 0)
        //{
        //    XanaConstants.xanaConstants.hair = SavaCharacterProperties.instance.characterController.wornHairId.ToString();
        //    XanaConstants.xanaConstants.hairColoPalette = SavaCharacterProperties.instance.characterController.hairColorPaletteId.ToString();
        //    XanaConstants.xanaConstants.shirt = SavaCharacterProperties.instance.characterController.wornShirtId.ToString();
        //    XanaConstants.xanaConstants.pants = SavaCharacterProperties.instance.characterController.wornPantId.ToString();
        //    XanaConstants.xanaConstants.shoes = SavaCharacterProperties.instance.characterController.wornShoesId.ToString();
        //    XanaConstants.xanaConstants.eyeWearable = SavaCharacterProperties.instance.characterController.wornEyewearableId.ToString();

        //    XanaConstants.xanaConstants.PresetValueString = SavaCharacterProperties.instance.characterController.presetValue;
        //    XanaConstants.xanaConstants.skinColor = SavaCharacterProperties.instance.characterController.skinId.ToString();
        //    XanaConstants.xanaConstants.faceIndex = SavaCharacterProperties.instance.characterController.faceId;
        //    XanaConstants.xanaConstants.eyeBrowIndex = SavaCharacterProperties.instance.characterController.eyeBrowId;
        //    XanaConstants.xanaConstants.eyeBrowColorPaletteIndex = SavaCharacterProperties.instance.characterController.eyeBrowColorPaletteId;
        //    XanaConstants.xanaConstants.eyeLashesIndex = SavaCharacterProperties.instance.characterController.eyeLashesId;
        //    XanaConstants.xanaConstants.eyeIndex = SavaCharacterProperties.instance.characterController.eyesId;
        //    XanaConstants.xanaConstants.eyeColor = SavaCharacterProperties.instance.characterController.eyesColorId.ToString();
        //    XanaConstants.xanaConstants.eyeColorPalette = SavaCharacterProperties.instance.characterController.eyesColorPaletteId.ToString();
        //    XanaConstants.xanaConstants.noseIndex = SavaCharacterProperties.instance.characterController.noseId;
        //    XanaConstants.xanaConstants.lipIndex = SavaCharacterProperties.instance.characterController.lipsId;
        //    XanaConstants.xanaConstants.lipColor = SavaCharacterProperties.instance.characterController.lipsColorId.ToString();
        //    XanaConstants.xanaConstants.lipColorPalette = SavaCharacterProperties.instance.characterController.lipsColorPaletteId.ToString();
        //    XanaConstants.xanaConstants.bodyNumber = SavaCharacterProperties.instance.characterController.bodyFat;
        //    XanaConstants.xanaConstants.makeupIndex = SavaCharacterProperties.instance.characterController.makeupId;
        //}
        //else
        //{
        XanaConstants.xanaConstants.hair = SavaCharacterProperties.instance.SaveItemList.myItemObj[2].ItemID.ToString();
        XanaConstants.xanaConstants.hairColoPalette = SavaCharacterProperties.instance.SaveItemList.HairColorPaletteValue.ToString();
        XanaConstants.xanaConstants.shirt = SavaCharacterProperties.instance.SaveItemList.myItemObj[1].ItemID.ToString();
        XanaConstants.xanaConstants.pants = SavaCharacterProperties.instance.SaveItemList.myItemObj[0].ItemID.ToString();
        XanaConstants.xanaConstants.shoes = SavaCharacterProperties.instance.SaveItemList.myItemObj[3].ItemID.ToString();
        XanaConstants.xanaConstants.eyeWearable = SavaCharacterProperties.instance.SaveItemList.EyeValue.ToString();

        XanaConstants.xanaConstants.PresetValueString = SavaCharacterProperties.instance.SaveItemList.PresetValue;
        XanaConstants.xanaConstants.skinColor = SavaCharacterProperties.instance.SaveItemList.SkinId.ToString();
        XanaConstants.xanaConstants.faceIndex = SavaCharacterProperties.instance.SaveItemList.FaceValue;
        XanaConstants.xanaConstants.eyeBrowIndex = SavaCharacterProperties.instance.SaveItemList.EyeBrowValue;
        XanaConstants.xanaConstants.eyeBrowColorPaletteIndex = SavaCharacterProperties.instance.SaveItemList.EyeBrowColorPaletteValue;
        XanaConstants.xanaConstants.eyeLashesIndex = SavaCharacterProperties.instance.SaveItemList.EyeLashesValue;
        XanaConstants.xanaConstants.eyeIndex = SavaCharacterProperties.instance.SaveItemList.EyeValue;
        XanaConstants.xanaConstants.eyeColor = SavaCharacterProperties.instance.SaveItemList.EyesColorValue.ToString();
        XanaConstants.xanaConstants.eyeColorPalette = SavaCharacterProperties.instance.SaveItemList.EyesColorPaletteValue.ToString();
        XanaConstants.xanaConstants.noseIndex = SavaCharacterProperties.instance.SaveItemList.NoseValue;
        XanaConstants.xanaConstants.lipIndex = SavaCharacterProperties.instance.SaveItemList.LipsValue;
        XanaConstants.xanaConstants.lipColor = SavaCharacterProperties.instance.SaveItemList.LipsColorValue.ToString();
        XanaConstants.xanaConstants.lipColorPalette = SavaCharacterProperties.instance.SaveItemList.LipsColorPaletteValue.ToString();
        XanaConstants.xanaConstants.bodyNumber = SavaCharacterProperties.instance.SaveItemList.BodyFat;
        XanaConstants.xanaConstants.makeupIndex = SavaCharacterProperties.instance.SaveItemList.MakeupValue;
        //}
    }
}
public class RequestedData
{
    public string userAddress;
}
public class XenyTokenData
{
    public double xenyToken;
    public string address;
}