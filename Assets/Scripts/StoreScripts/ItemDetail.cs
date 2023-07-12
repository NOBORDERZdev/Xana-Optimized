using SuperStar.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static StoreManager;
using System;
public class ItemDetail : MonoBehaviour
{
    public Color NormalColor;
    public Color HighlightedColor;
    public string id;
    public string assetLinkAndroid;
    public string assetLinkIos;
    public string assetLinkWindows;
    public string iconLink;
    public string categoryId;
    public string subCategory;
    public string name;
    public string isPaid;
    public string price;
    public string isPurchased;
    public string isFavourite;
    public string isOccupied;
    public bool isDeleted;
    public string createdBy;
    public string createdAt;
    public string updatedAt;
    public string[] itemTags;
    public Image SelectImg;
    public Text PriceTxt;
    public int MyIndex;
    public bool SelectedBool;
    public Image _iconImg;
    private string _clothetype;
    private string DefaultTempString;
    [HideInInspector]
    public string _downloadableURL;
    public EnumClass.CategoryEnum CategoriesEnumVar;
    public Coroutine runningCoroutine;
    public bool enableUpdate = false;
    public bool completedCoroutine = false, runOnce = false, AddInUndoRedoOnce = false;
    public GameObject loadingSpriteImage;
    public bool firstTimeEnable = false;

    bool itemAlreadySaved = false;
    private bool isHairItem = false;
    int saveIndex = -1;
    //bool isAdded = true;
    private AddressableDownloader downloader;

    public void StartRun()
    {

        if (!runOnce)
        {

            runOnce = true;
            //if (runningCoroutine != null)
            //{
            //    StopCoroutine(runningCoroutine);
            //}

            if (CategoriesEnumVar == EnumClass.CategoryEnum.EyeBrowAvatar ||
                CategoriesEnumVar == EnumClass.CategoryEnum.EyeBrowAvatarColor ||
                CategoriesEnumVar == EnumClass.CategoryEnum.EyeLashesAvatar ||
                CategoriesEnumVar == EnumClass.CategoryEnum.LipsAvatar ||
                 CategoriesEnumVar == EnumClass.CategoryEnum.LipsAvatarColor ||
                CategoriesEnumVar == EnumClass.CategoryEnum.HairAvatarColor ||
                CategoriesEnumVar == EnumClass.CategoryEnum.EyesAvatar ||
                CategoriesEnumVar == EnumClass.CategoryEnum.EyesAvatarColor ||
                CategoriesEnumVar == EnumClass.CategoryEnum.SkinToneAvatar)
            {

                // Added By Waqas Ahmad
                // Eyebrow Icon resizing require
                // Eyebrow button method is itemBtnClicked
                //
                if (CategoriesEnumVar == EnumClass.CategoryEnum.EyeBrowAvatar || CategoriesEnumVar == EnumClass.CategoryEnum.EyeLashesAvatar)
                    this.gameObject.GetComponent<Button>().onClick.AddListener(ItemBtnClicked);
                //
                else
                    this.gameObject.GetComponent<Button>().onClick.AddListener(ColorBtnClicked);


                PriceTxt.gameObject.SetActive(false);
                _iconImg.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
                _iconImg.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);

            }
            else
            {
                this.gameObject.GetComponent<Button>().onClick.AddListener(ItemBtnClicked);
            }
            decimal PriceInDecimal = decimal.Parse(price);
            int priceint = (int)PriceInDecimal;
            PriceTxt.text = priceint.ToString();
            switch (CategoriesEnumVar)
            {
                case EnumClass.CategoryEnum.HairAvatar:
                    {
                        _clothetype = "Hair";
                        DefaultTempString = "/MDhairs.";
                    }
                    break;
                case EnumClass.CategoryEnum.Shoes:
                    {
                        _clothetype = "Feet";
                        DefaultTempString = "/MDshoes.";
                    }
                    break;
                case EnumClass.CategoryEnum.Outer:
                    {
                        _clothetype = "Chest";
                        DefaultTempString = "/MDshirt.";
                    }
                    break;
                case EnumClass.CategoryEnum.Bottom:
                    {
                        _clothetype = "Legs";
                        DefaultTempString = "/MDpant.";
                    }
                    break;
                case EnumClass.CategoryEnum.LipsAvatar:
                    {
                        _clothetype = "Lip";
                    }
                    break;
                case EnumClass.CategoryEnum.EyesAvatar:
                    {
                        _clothetype = "Eyes";
                    }
                    break;
                case EnumClass.CategoryEnum.SkinToneAvatar:
                    {
                        _clothetype = "Skin";
                    }
                    break;
            }
            isPurchased = "true";

            if (isPaid == "true")
            {
                PriceTxt.text = "Coming Soon";
                this.gameObject.GetComponent<Button>().interactable = false;
            }
        }
        if (CategoriesEnumVar == EnumClass.CategoryEnum.SkinToneAvatar)
        {
            _iconImg.sprite = CharcterBodyParts.instance.defaultPngForSkinIcon;
            _iconImg.color = CharcterBodyParts.instance.skinColor[MyIndex];
            loadingSpriteImage.SetActive(false);
            completedCoroutine = true;
            enableUpdate = false;
        }
        else if (CategoriesEnumVar == EnumClass.CategoryEnum.HairAvatarColor)
        {
            _iconImg.sprite = CharcterBodyParts.instance.defaultPngForSkinIcon;
            _iconImg.color = CharcterBodyParts.instance.hairColor[MyIndex];
            loadingSpriteImage.SetActive(false);
            completedCoroutine = true;
            enableUpdate = false;
        }
        else if (CategoriesEnumVar == EnumClass.CategoryEnum.EyeBrowAvatarColor)
        {
            _iconImg.sprite = CharcterBodyParts.instance.defaultPngForSkinIcon;
            _iconImg.color = CharcterBodyParts.instance.eyeBrowsColor[MyIndex];
            loadingSpriteImage.SetActive(false);
            completedCoroutine = true;
            enableUpdate = false;
        }
        else if (CategoriesEnumVar == EnumClass.CategoryEnum.EyesAvatarColor)
        {
            _iconImg.sprite = CharcterBodyParts.instance.defaultPngForSkinIcon;
            _iconImg.color = CharcterBodyParts.instance.eyeColor[MyIndex];
            loadingSpriteImage.SetActive(false);
            completedCoroutine = true;
            enableUpdate = false;
        }
        else if (CategoriesEnumVar == EnumClass.CategoryEnum.LipsAvatarColor)
        {
            _iconImg.sprite = CharcterBodyParts.instance.defaultPngForSkinIcon;
            _iconImg.color = CharcterBodyParts.instance.lipColorPalette[MyIndex];
            loadingSpriteImage.SetActive(false);
            completedCoroutine = true;
            enableUpdate = false;
        }
        else //if (iconLink != null && CategoriesEnumVar != EnumClass.CategoryEnum.SkinToneAvatar && CategoriesEnumVar != EnumClass.CategoryEnum.HairAvatarColor)
        {
            if (!completedCoroutine)
            {
                AssetCache.Instance.EnqueueOneResAndWait(iconLink, iconLink, (success) =>
                {
                    if (success)
                    {
                        AssetCache.Instance.LoadSpriteIntoImage(_iconImg, iconLink, changeAspectRatio: true);
                        // CheckAndSetResolutionOfImage(imgFeed.sprite);
                        //  isImageSuccessDownloadAndSave = true;
                        if (_iconImg != null)
                        {
                            // While Downloading User change the tab destroing objects make its null
                            _iconImg.gameObject.SetActive(true);
                            loadingSpriteImage.SetActive(false);
                        }
                        completedCoroutine = true;
                        enableUpdate = false;
                    }
                    else
                    {
                        //Debug.LogError("Download Failed");
                    }
                });


                //runningCoroutine = StartCoroutine(addsprite(_iconImg, iconLink));
            }
            else
            {
                _iconImg.gameObject.SetActive(true);
            }
        }

    }
    private void OnEnable()
    {
        downloader = AddressableDownloader.Instance;
        if (enableUpdate)
            StartRun();

        Invoke("Delay", 0.1f);    // AR changes
    }
    private void Start()
    {
        if (CategoriesEnumVar.Equals(EnumClass.CategoryEnum.HairAvatar) && this.id == XanaConstants.xanaConstants.hair)
        {
            isHairItem = true;
           // Debug.Log("IsStartItem=true");
        }
    }
    void Delay()
    {
        if (File.Exists(GameManager.Instance.GetStringFolderPath()) && File.ReadAllText(GameManager.Instance.GetStringFolderPath()) != "")
        {
            SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();
            _CharacterData = _CharacterData.CreateFromJSON(File.ReadAllText(GameManager.Instance.GetStringFolderPath()));

            string CurrentString = "";
            CurrentString = CategoriesEnumVar.ToString();
            switch (CurrentString)
            {
                case "HairAvatar":
                    {
                        //Debug.Log(this.id + " xanaConstantsHairs: " + XanaConstants.xanaConstants.hair);
                        //Debug.Log("wornHairId: " + SavaCharacterProperties.instance.characterController.wornHairId.ToString());
                        if ((isHairItem && this.id == XanaConstants.xanaConstants.hair) ||
                            (ActivePanelCallStack.obj.IsCallByBtn() && this.id == XanaConstants.xanaConstants.hair))
                        {
                            isHairItem = false;
                            if (!AR_UndoRedo.obj.addToList)
                                AR_UndoRedo.obj.addToList = true;
                            else
                            {
                                AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "ItemBtnClicked", AR_UndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.HairAvatar);
                                Debug.Log("<color=red> Set Default Hair </color>");
                            }
                        }
                        break;
                    }
                case "EyeBrowAvatar":
                    {
                        //Debug.Log(id.ParseToInt() + " EyeBrowValue: " + _CharacterData.EyeBrowValue);
                        if (ActivePanelCallStack.obj.IsCallByBtn() && id.ParseToInt() == _CharacterData.EyeBrowValue)
                        {
                            if (!AR_UndoRedo.obj.addToList)
                                AR_UndoRedo.obj.addToList = true;
                            else
                            {
                                AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "ItemBtnClicked", AR_UndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.EyeBrowAvatar);
                                Debug.Log("<color=red> Set Default EyeBrow </color>");
                            }
                        }
                        break;
                    }
                case "SkinToneAvatar":
                    {
                        //Debug.Log(MyIndex + " SkinValue: " + _CharacterData.SkinId);
                        if (ActivePanelCallStack.obj.IsCallByBtn() && MyIndex == _CharacterData.SkinId)
                        {
                            if (!AR_UndoRedo.obj.addToList)
                                AR_UndoRedo.obj.addToList = true;
                            else
                            {
                                AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "ColorBtnClicked", AR_UndoRedo.ActionType.ChangeColor, _iconImg.color, EnumClass.CategoryEnum.SkinToneAvatar);
                                Debug.Log("<color=red> Set Default Skin Color </color>");
                            }                          
                        }
                    }
                    break;
                case "HairAvatarColor":
                    {
                        //Debug.Log(id + " XanaHairColoPalette: " + XanaConstants.xanaConstants.hairColoPalette);
                        if (ActivePanelCallStack.obj.IsCallByBtn() && this.id == XanaConstants.xanaConstants.hairColoPalette)
                        {
                            if (!AR_UndoRedo.obj.addToList)
                                AR_UndoRedo.obj.addToList = true;
                            else
                            {
                                AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "ColorBtnClicked", AR_UndoRedo.ActionType.ChangeColor, _iconImg.color, EnumClass.CategoryEnum.HairAvatarColor);
                                Debug.Log("<color=red> Set Default HairColor:::: </color>");
                            }
                        }
                        break;
                    }
                case "EyeBrowAvatarColor":
                    {
                        Debug.Log("XanaEyeBrowColorPaletteIndex: " + XanaConstants.xanaConstants.eyeBrowColorPaletteIndex);
                        if (ActivePanelCallStack.obj.IsCallByBtn() && id.ParseToInt() == XanaConstants.xanaConstants.eyeBrowColorPaletteIndex)
                        {
                            if (!AR_UndoRedo.obj.addToList)
                                AR_UndoRedo.obj.addToList = true;
                            else
                            {
                                AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "ColorBtnClicked", AR_UndoRedo.ActionType.ChangeColor, _iconImg.color, EnumClass.CategoryEnum.EyeBrowAvatarColor);
                                Debug.Log("<color=red> Set Default EyeBrowColorPalette::::" + this.gameObject.name + " </color>");
                            }
                        }
                        break;
                    }
                case "EyesAvatarColor":
                    {
                        Debug.Log("XanaEyeColorPalette: " + XanaConstants.xanaConstants.eyeColorPalette);
                        if (ActivePanelCallStack.obj.IsCallByBtn() && this.id == XanaConstants.xanaConstants.eyeColorPalette)
                        {
                            if (!AR_UndoRedo.obj.addToList)
                                AR_UndoRedo.obj.addToList = true;
                            else
                            {
                                AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "ColorBtnClicked", AR_UndoRedo.ActionType.ChangeColor, _iconImg.color, EnumClass.CategoryEnum.EyesAvatarColor);
                                Debug.Log("<color=red> Set Default EyeColorPalette:::: </color>");
                            }
                        }
                        break;
                    }
                case "LipsAvatarColor":
                    {
                        Debug.Log("XanaLipColorPalette: " + XanaConstants.xanaConstants.lipColorPalette);
                        if (ActivePanelCallStack.obj.IsCallByBtn() && this.id == XanaConstants.xanaConstants.lipColorPalette)
                        {
                            if (!AR_UndoRedo.obj.addToList)
                                AR_UndoRedo.obj.addToList = true;
                            else
                            {
                                AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "ColorBtnClicked", AR_UndoRedo.ActionType.ChangeColor, _iconImg.color, EnumClass.CategoryEnum.LipsAvatarColor);
                                Debug.Log("<color=red> Set Default LipsColorPalette:::: </color>");
                            }
                        }
                        break;
                    }
                case "EyeLashesAvatar":
                    {
                        //Debug.Log(id.ParseToInt() + " EyeLashesValue: " + XanaConstants.xanaConstants.eyeLashesIndex);
                        if (ActivePanelCallStack.obj.IsCallByBtn() && id.ParseToInt() == XanaConstants.xanaConstants.eyeLashesIndex)
                        {
                            if (!AR_UndoRedo.obj.addToList)
                                AR_UndoRedo.obj.addToList = true;
                            else
                            {
                                AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "ItemBtnClicked", AR_UndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.EyeLashesAvatar);
                                Debug.Log("<color=red> Set Default EyeLashes </color>");
                            }
                        }
                        break;
                    }
                case "Outer":
                    {
                        //Debug.Log("Enter In outer: " + XanaConstants.xanaConstants.shirt);
                        if (id == XanaConstants.xanaConstants.shirt)  // ActivePanelCallStack.obj.IsCallByBtn() && 
                        {
                            if (!AR_UndoRedo.obj.addToList)
                                AR_UndoRedo.obj.addToList = true;
                            else
                            {
                                AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "ItemBtnClicked", AR_UndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.Outer);
                                Debug.Log("<color=red> Set Default Shirts </color>");
                            }
                        }
                        break;
                    }
                case "Shoes":
                    {
                        //Debug.Log("Enter In Shose: " + XanaConstants.xanaConstants.shoes);
                        if (id == XanaConstants.xanaConstants.shoes)  // ActivePanelCallStack.obj.IsCallByBtn() && 
                        {
                            if (!AR_UndoRedo.obj.addToList)
                                AR_UndoRedo.obj.addToList = true;
                            else
                            {
                                AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "ItemBtnClicked", AR_UndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.Shoes);
                                Debug.Log("<color=red> Set Default Shoes </color>");
                            }
                        }
                        break;
                    }
                case "Bottom":
                    {
                        //Debug.Log("Enter In Shose: " + XanaConstants.xanaConstants.pants);
                        if (id == XanaConstants.xanaConstants.pants)  // ActivePanelCallStack.obj.IsCallByBtn() && 
                        {
                            if (!AR_UndoRedo.obj.addToList)
                                AR_UndoRedo.obj.addToList = true;
                            else
                            {
                                AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "ItemBtnClicked", AR_UndoRedo.ActionType.ChangeItem, Color.white, EnumClass.CategoryEnum.Bottom);
                                Debug.Log("<color=red> Set Default Pents </color>");
                            }
                        }
                        break;
                    }
            }
        }
    }

    private void OnDisable()
    {
        if (this.name == "ColorButton")
            return;
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
        }

        //AssetCache.Instance.RemoveFromMemory(iconLink, true);       // AR changes
        //Destroy(this.gameObject);                                   // AR changes


        //this.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
    }

    public void UpdateValues()
    {
        decimal PriceInDecimal = decimal.Parse(price);
        int priceint = (int)PriceInDecimal;
        PriceTxt.text = priceint.ToString();
    }

    public IEnumerator addsprite(Image data, string rewview)
    {
        // WHEN IMAGES ARE IN PNG FORMAT
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(rewview))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError)
            {
                //Debug.Log(uwr.error);
            }
            else
            {
                Texture2D tex = ((DownloadHandlerTexture)uwr.downloadHandler).texture;
                // tex.Compress(true);
                if (data)   // Check if data is null or not
                    data.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
                _iconImg.gameObject.SetActive(true);
                loadingSpriteImage.SetActive(false);
                completedCoroutine = true;
                enableUpdate = false;
            }
            uwr.Dispose();
        }
        yield return null;

    }
    public void DeSelectBtns()
    {
        //  SelectImg.enabled = false;
        this.gameObject.GetComponent<Image>().color = NormalColor;
    }

    public void DeSelectAfterBuying()
    {
        SelectedBool = !SelectedBool;
        if (SelectedBool)
        {
            // SelectImg.enabled = true;
            this.gameObject.GetComponent<Image>().color = HighlightedColor;
        }
        else
        {
            // SelectImg.enabled = false;
            this.gameObject.GetComponent<Image>().color = NormalColor;
        }
        if (!SelectedBool)
        {
            StoreManager.instance.GetSelectedBtn(-1, CategoriesEnumVar);
        }
        else
        {
            StoreManager.instance.GetSelectedBtn(MyIndex, CategoriesEnumVar);
        }
    }
   
    public void ItemBtnClicked()
    {
        if (GameManager.Instance.isStoreAssetDownloading || GetComponent<Image>().enabled is true)
            return;

        string CurrentString = "";
        CurrentString = CategoriesEnumVar.ToString();

        switch (CurrentString)
        {
            case "Shoes":
                {
                    CurrentString = "Shoes";
                    break;
                }
            case "Outer":
                {
                    CurrentString = "Shirts/Outer";
                    break;
                }
            case "HairAvatar":
                {
                    CurrentString = "Hairs";
                    break;
                }
            // Added by Ahsan
            case "HairAvatarColor":
                {
                    CurrentString = "HairColor";
                    break;
                }
            case "Bottom":
                {
                    CurrentString = "Pents /Bottom";
                    break;
                }
            // Added By WaqasAhmad
            case "EyeBrowAvatar":
                {
                    CurrentString = "Eye Brow";
                    break;
                }
            // Added by Ahsan
            case "EyeBrowAvatarColor":
                {
                    CurrentString = "EyeBrowColor";
                    break;
                }
            // Added By WaqasAhmad
            case "EyeLashesAvatar":
                {
                    CurrentString = "Eye Lashes";
                    break;
                }
            case "EyesAvatarColor":
                {
                    CurrentString = "EyesColor";
                    break;
                }
            case "LipsAvatarColor":
                {
                    CurrentString = "LipsColor";
                    break;
                }
        }

        if (!PremiumUsersDetails.Instance.CheckSpecificItem(CurrentString))
        {
            PremiumUsersDetails.Instance.PremiumUserUI.SetActive(true);

            //print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            itemAlreadySaved = false;
            switch (CurrentString)
            {
                case "Shoes":
                    {
                        XanaConstants.xanaConstants.shoes = id;
                        XanaConstants.xanaConstants.wearableStoreSelection[XanaConstants.xanaConstants.currentButtonIndex] = gameObject;
                        saveIndex = 3;

                        break;
                    }
                case "Shirts/Outer":
                    {
                        XanaConstants.xanaConstants.shirt = id;
                        XanaConstants.xanaConstants.wearableStoreSelection[XanaConstants.xanaConstants.currentButtonIndex] = gameObject;
                        saveIndex = 1;
                        break;
                    }
                case "Hairs":
                    {
                        XanaConstants.xanaConstants.hair = id;
                        XanaConstants.xanaConstants.avatarStoreSelection[XanaConstants.xanaConstants.currentButtonIndex] = gameObject;
                        saveIndex = 2;
                        //Debug.Log("XanaConstants Hairs: " + XanaConstants.xanaConstants.hair);
                        //Debug.Log("wornHairId: " + SavaCharacterProperties.instance.characterController.wornHairId.ToString());
                        break;
                    }
                case "HairColor":
                    {
                        //if (!StoreManager.instance.CheckColorPanelEnabled(XanaConstants.xanaConstants.currentButtonIndex))
                        //{
                            Debug.Log("<color=blue> Open Hair Color Panel: </color>");
                            StoreManager.instance.OpenColorPanel(XanaConstants.xanaConstants.currentButtonIndex);
                            StoreManager.instance.colorMode = true;
                            StoreManager.instance.PutDataInOurAPPNewAPI();
                            StoreManager.instance.colorMode = false;
                        //}
                        //else   
                        //{
                        //    int enumNum = Array.IndexOf(Enum.GetValues(typeof(EnumClass.CategoryEnum)), EnumClass.CategoryEnum.HairAvatar);
                        //    AR_UndoRedo.obj.AvatarBtnPressedForcally(enumNum);
                        //}

                        // AR changes start
                        if (!AR_UndoRedo.obj.addToList)
                            AR_UndoRedo.obj.addToList = true;
                        else
                        {
                            AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "ItemBtnClicked", AR_UndoRedo.ActionType.ChangePanel, Color.white, EnumClass.CategoryEnum.HairAvatar);
                            Debug.Log("<color=red> Set Hair color btn into list:::: </color>");
                        }
                        // AR changes end
                        if (this.gameObject.name == "Color Button")
                            return;
                        break;
                    }
                case "EyesColor":
                    {
                        StoreManager.instance.OpenColorPanel(XanaConstants.xanaConstants.currentButtonIndex);
                        StoreManager.instance.colorMode = true;
                        StoreManager.instance.PutDataInOurAPPNewAPI();
                        StoreManager.instance.colorMode = false;

                        if (!AR_UndoRedo.obj.addToList)
                            AR_UndoRedo.obj.addToList = true;
                        else
                        {
                            AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "ItemBtnClicked", AR_UndoRedo.ActionType.ChangePanel, Color.white, EnumClass.CategoryEnum.EyesAvatar);
                            Debug.Log("<color=red> Set eye color btn into list:::: </color>");
                        }
                        if (this.gameObject.name == "Color Button")
                            return;
                        break;
                    }
                case "LipsColor":
                    {
                        StoreManager.instance.OpenColorPanel(XanaConstants.xanaConstants.currentButtonIndex);
                        StoreManager.instance.colorMode = true;
                        StoreManager.instance.PutDataInOurAPPNewAPI();
                        StoreManager.instance.colorMode = false;

                        if (!AR_UndoRedo.obj.addToList)
                            AR_UndoRedo.obj.addToList = true;
                        else
                        {
                            AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "ItemBtnClicked", AR_UndoRedo.ActionType.ChangePanel, Color.white, EnumClass.CategoryEnum.LipsAvatar);
                            Debug.Log("<color=red> Set lips color btn into list:::: </color>");
                        }

                        if (this.gameObject.name == "Color Button")
                            return;
                        break;
                    }
                case "EyeBrowColor":
                    {
                        StoreManager.instance.OpenColorPanel(XanaConstants.xanaConstants.currentButtonIndex);
                        StoreManager.instance.colorMode = true;
                        StoreManager.instance.PutDataInOurAPPNewAPI();
                        StoreManager.instance.colorMode = false;

                        if (!AR_UndoRedo.obj.addToList)
                            AR_UndoRedo.obj.addToList = true;
                        else
                        {
                            AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "ItemBtnClicked", AR_UndoRedo.ActionType.ChangePanel, Color.white, EnumClass.CategoryEnum.EyeBrowAvatar);
                            Debug.Log("<color=red> Set eyeBrow color btn into list:::: </color>");
                        }
                        if (this.gameObject.name == "Color Button")
                            return;
                        break;
                    }
                case "Eye Brow":
                    {
                        XanaConstants.xanaConstants.eyeBrowIndex = id.ParseToInt();
                        Debug.Log("Eye brow eyeBrowIndex: " + XanaConstants.xanaConstants.eyeBrowIndex);
                        SavaCharacterProperties.instance.characterController.eyeBrowId = id.ParseToInt();
                        Debug.Log("Eye brow ID: " + SavaCharacterProperties.instance.characterController.eyeBrowId);
                        //Commented By Ahsan
                        //Transform ParentAvatarofEyeBrows = StoreManager.instance.ParentOfBtnsAvatarEyeBrows;

                        //for (int i = 1; i < ParentAvatarofEyeBrows.childCount; i++)
                        //{
                        //    ParentAvatarofEyeBrows.GetChild(i).GetComponent<Image>().enabled = false;
                        //}

                        //ParentAvatarofEyeBrows.GetChild(MyIndex + 1).GetComponent<Image>().enabled = true;
                        break;
                    }
                case "Eye Lashes":
                    {
                        XanaConstants.xanaConstants.eyeLashesIndex = id.ParseToInt();
                        SavaCharacterProperties.instance.characterController.eyeLashesId = id.ParseToInt();
                        //Transform ParentAvatarofEyeLashes = StoreManager.instance.ParentOfBtnsAvatarEyeLashes;
                        //for (int i = 0; i < ParentAvatarofEyeLashes.childCount; i++)
                        //{
                        //    ParentAvatarofEyeLashes.GetChild(i).GetComponent<Image>().enabled = false;
                        //}
                        //ParentAvatarofEyeLashes.GetChild(MyIndex).GetComponent<Image>().enabled = true;
                    }
                    break;
                case "Pents /Bottom":
                    {
                        XanaConstants.xanaConstants.pants = id;
                        XanaConstants.xanaConstants.wearableStoreSelection[XanaConstants.xanaConstants.currentButtonIndex] = gameObject;
                        saveIndex = 0;

                        break;
                    }
            }
            XanaConstants.xanaConstants._curretClickedBtn = this.gameObject;

            Debug.Log("Undo Redo call in Purchase check: add to list= " + AR_UndoRedo.obj.addToList);
            if (!AR_UndoRedo.obj.addToList)
                AR_UndoRedo.obj.addToList = true;
            else
            {
                AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "ItemBtnClicked", AR_UndoRedo.ActionType.ChangeItem, Color.white, CategoriesEnumVar);
                Debug.Log("<color=red> Set On Btn clicked " + CurrentString + ":::: </color>");
            }

            if (XanaConstants.xanaConstants._lastClickedBtn && XanaConstants.xanaConstants._curretClickedBtn == XanaConstants.xanaConstants._lastClickedBtn)
                return;

            //Debug.Log("<color=red>" + XanaConstants.xanaConstants._curretClickedBtn.GetComponent<ItemDetail>().id + "</color>");
            XanaConstants.xanaConstants._curretClickedBtn.GetComponent<Image>().enabled = true;

            if (XanaConstants.xanaConstants._lastClickedBtn)
            {
                if (XanaConstants.xanaConstants._lastClickedBtn.GetComponent<ItemDetail>())
                    XanaConstants.xanaConstants._lastClickedBtn.GetComponent<Image>().enabled = false;
            }
            Debug.Log("<color=red>ItemDetail AssignLastClickedBtnHere</color>");
            XanaConstants.xanaConstants._lastClickedBtn = this.gameObject;

            if (!completedCoroutine)
                return;


            if (isPurchased == "true")
            {
                if (!GameManager.Instance.isStoreAssetDownloading)
                {
                    GameManager.Instance.isStoreAssetDownloading = true;
                    if (!name.Contains("md", System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (name.Contains("eyebrow"))
                            downloader.StartCoroutine(downloader.DownloadAddressableTexture(name, GameManager.Instance.mainCharacter, CurrentTextureType.EyeBrows));
                        else if (name.Contains("eyelash"))
                            downloader.StartCoroutine(downloader.DownloadAddressableTexture(name, GameManager.Instance.mainCharacter, CurrentTextureType.EyeLashes));
                        else
                            downloader.StartCoroutine(downloader.DownloadAddressableObj(int.Parse(id), name, _clothetype, GameManager.Instance.mainCharacter.GetComponent<AvatarController>(),Color.clear, false));
                    }
                    else
                    {
                        GameManager.Instance.mainCharacter.GetComponent<AvatarController>().WearDefaultItem(_clothetype, GameManager.Instance.mainCharacter);
                    }
                    //StoreManager.instance._DownloadRigClothes.NeedToDownloadOrNot(this, assetLinkAndroid, assetLinkIos, _clothetype, name.ToLower(), int.Parse(id));
                }
                this.GetComponent<Image>().enabled = true;

                if (StoreManager.instance.UndoBtn)
                    StoreManager.instance.UndoBtn.GetComponent<Button>().interactable = true;

                // to updated data in equipment and to change saveindex which is for save file
                switch (CategoriesEnumVar)
                {
                    case EnumClass.CategoryEnum.Head:
                        break;
                    case EnumClass.CategoryEnum.Face:
                        break;
                    case EnumClass.CategoryEnum.Inner:
                        break;
                    case EnumClass.CategoryEnum.Outer:
                        //CharacterController.instance.equipmentScript.SetItemIdName(int.Parse(id), name, assetLinkAndroid, assetLinkIos, 1);
                        XanaConstants.xanaConstants.shirt = id;
                        saveIndex = 1;
                        break;
                    case EnumClass.CategoryEnum.Accesary:
                        break;
                    case EnumClass.CategoryEnum.Bottom:
                        //CharacterController.instance.equipmentScript.SetItemIdName(int.Parse(id), name, assetLinkAndroid, assetLinkIos, 0);
                        XanaConstants.xanaConstants.pants = id;
                        saveIndex = 0;
                        break;
                    case EnumClass.CategoryEnum.Socks:
                        break;
                    case EnumClass.CategoryEnum.Shoes:
                        //CharacterController.instance.equipmentScript.SetItemIdName(int.Parse(id), name, assetLinkAndroid, assetLinkIos, 7);
                        XanaConstants.xanaConstants.shoes = id;
                        saveIndex = 3;
                        break;
                    case EnumClass.CategoryEnum.HairAvatar:
                        //CharacterController.instance.equipmentScript.SetItemIdName(int.Parse(id), name, assetLinkAndroid, assetLinkIos, 2);
                        XanaConstants.xanaConstants.hair = id;
                        saveIndex = 2;
                        break;
                    case EnumClass.CategoryEnum.LipsAvatar:
                        break;
                    case EnumClass.CategoryEnum.EyesAvatar:
                        break;
                    case EnumClass.CategoryEnum.SkinToneAvatar:
                        break;
                    case EnumClass.CategoryEnum.Presets:
                        break;
                    default:
                        break;
                }

                if (File.Exists(GameManager.Instance.GetStringFolderPath()) && File.ReadAllText(GameManager.Instance.GetStringFolderPath()) != "")
                {
                    SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();

                    _CharacterData = _CharacterData.CreateFromJSON(File.ReadAllText(GameManager.Instance.GetStringFolderPath()));
                    if (saveIndex >= 0 && _CharacterData.myItemObj.Count != 0)
                    {
                        if (id == _CharacterData.myItemObj[saveIndex].ItemID.ToString())
                        {
                            itemAlreadySaved = true;
                        }
                    }

                    if (CategoriesEnumVar == EnumClass.CategoryEnum.EyeBrowAvatar)
                    {
                        if (id.ParseToInt() == _CharacterData.EyeBrowValue)
                            itemAlreadySaved = true;
                    }
                    else if (CategoriesEnumVar == EnumClass.CategoryEnum.EyeLashesAvatar)
                    {
                        if (id.ParseToInt() == _CharacterData.EyeLashesValue)
                            itemAlreadySaved = true;
                    }
                }

                if (!itemAlreadySaved)
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
    }

    // open color pallete panel by press color btn
    public void ColorBtnClicked()
    {
        //SelectImg.enabled = true;
        //this.gameObject.GetComponent<Image>().color = HighlightedColor;
        //return;
        if (GetComponent<Image>().enabled is true)
            return;

        string CurrentString = "";

        if (CategoriesEnumVar.ToString() == "SkinToneAvatar")
        {
            CurrentString = "Skin";
        }
        else if (CategoriesEnumVar.ToString() == "EyesAvatar")
        {
            CurrentString = "eyes";
        }
        else if (CategoriesEnumVar.ToString() == "EyesAvatarColor")
        {
            CurrentString = "EyesColor";
        }
        else if (CategoriesEnumVar.ToString() == "LipsAvatar")
        {
            CurrentString = "Lip";
        }
        else if (CategoriesEnumVar.ToString() == "LipsAvatarColor")
        {
            CurrentString = "LipsColor";
        }
        else if (CategoriesEnumVar.ToString() == "HairAvatarColor")
        {
            CurrentString = "HairColor";
        }
        else if (CategoriesEnumVar.ToString() == "EyeBrowAvatarColor")
        {
            CurrentString = "EyeBrowColor";
        }

        Debug.Log("Current String is: " + CurrentString);

        if (!PremiumUsersDetails.Instance.CheckSpecificItem(CurrentString))
        {
            PremiumUsersDetails.Instance.PremiumUserUI.SetActive(true);

            //print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            itemAlreadySaved = false;

            if (CategoriesEnumVar.ToString() == "SkinToneAvatar")
            {
                //print("in skin tone ");
                XanaConstants.xanaConstants.skinColor = MyIndex.ToString();
                XanaConstants.xanaConstants.avatarStoreSelection[XanaConstants.xanaConstants.currentButtonIndex] = gameObject;

                saveIndex = 6;
            }
            else if (CategoriesEnumVar.ToString() == "EyesAvatar")
            {
                XanaConstants.xanaConstants.eyeColor = id;
                XanaConstants.xanaConstants.colorSelection[0] = gameObject;

                saveIndex = 4;
            }
            else if (CategoriesEnumVar.ToString() == "LipsAvatar")
            {
                XanaConstants.xanaConstants.lipColor = id;
                XanaConstants.xanaConstants.colorSelection[1] = gameObject;

                saveIndex = 5;
            }
            else if (CategoriesEnumVar.ToString() == "HairAvatarColor")
            {
                //Debug.Log("Assign ID to by default color selection: " + id);
                // Debug.Log("Before: " + XanaConstants.xanaConstants.hairColoPalette);

                XanaConstants.xanaConstants.hairColoPalette = id;
                XanaConstants.xanaConstants.colorSelection[2] = gameObject;
                //Debug.Log("After: " + XanaConstants.xanaConstants.hairColoPalette);
                //saveIndex = 4;
            }
            else if (CategoriesEnumVar.ToString() == "EyeBrowAvatarColor")
            {
                XanaConstants.xanaConstants.eyeBrowColorPaletteIndex = int.Parse(id);
                XanaConstants.xanaConstants.colorSelection[3] = gameObject;

                //saveIndex = 4;
            }
            else if (CategoriesEnumVar.ToString() == "EyesAvatarColor")
            {
                XanaConstants.xanaConstants.eyeColorPalette = id;
                XanaConstants.xanaConstants.colorSelection[4] = gameObject;

                //saveIndex = 4;
            }
            else if (CategoriesEnumVar.ToString() == "LipsAvatarColor")
            {
                XanaConstants.xanaConstants.lipColorPalette = id;
                XanaConstants.xanaConstants.colorSelection[5] = gameObject;

                //saveIndex = 4;
            }

            Debug.Log("Undo Redo call in Purchase check: add to list= " + AR_UndoRedo.obj.addToList);
            if (!AR_UndoRedo.obj.addToList)
                AR_UndoRedo.obj.addToList = true;
            else
            {
                AR_UndoRedo.obj.ActionWithParametersAdd(this.gameObject, -1, "ColorBtnClicked", AR_UndoRedo.ActionType.ChangeColor, _iconImg.color, CategoriesEnumVar);
                Debug.Log("<color=red> Set On color Btn clicked " + CurrentString + ":::: </color>");
            }

            //Debug.Log("Item Detail Btn Clicked Obj Name: " + this.gameObject);
            XanaConstants.xanaConstants._curretClickedBtn = this.gameObject;

            if (XanaConstants.xanaConstants._lastClickedBtn && XanaConstants.xanaConstants._curretClickedBtn == XanaConstants.xanaConstants._lastClickedBtn)
                return;

            XanaConstants.xanaConstants._curretClickedBtn.GetComponent<Image>().enabled = true;

            if (XanaConstants.xanaConstants._lastClickedBtn)
            {
                // Debug.Log("_lastClickedBtnId: " + XanaConstants.xanaConstants._lastClickedBtn.GetComponent<ItemDetail>().id);
                if (XanaConstants.xanaConstants._lastClickedBtn.GetComponent<ItemDetail>())
                {
                    XanaConstants.xanaConstants._lastClickedBtn.GetComponent<Image>().enabled = false;
                    //Debug.Log("Disabled Image Here");
                }
            }
            StoreManager.instance.ClearBuyItems();

            if (CategoriesEnumVar == EnumClass.CategoryEnum.SkinToneAvatar || CategoriesEnumVar == EnumClass.CategoryEnum.LipsAvatar)
            {
                applytexture(null, assetLinkIos);
            }
            else if (CategoriesEnumVar == EnumClass.CategoryEnum.EyesAvatar)
            {
                if (name != "")
                {
                    downloader.StartCoroutine(downloader.DownloadAddressableTexture(name, GameManager.Instance.mainCharacter, CurrentTextureType.EyeLense));
                    SavaCharacterProperties.instance.characterController.eyesColorId = int.Parse(id);
                }
                if (File.Exists(GameManager.Instance.GetStringFolderPath()) && File.ReadAllText(GameManager.Instance.GetStringFolderPath()) != "")
                {
                    SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();

                    _CharacterData = _CharacterData.CreateFromJSON(File.ReadAllText(GameManager.Instance.GetStringFolderPath()));
                    //if (_CharacterData.myItemObj.Count != 0)
                    if (id == _CharacterData.EyesColorValue.ToString())
                    {
                        itemAlreadySaved = true;
                    }
                }
                //StoreManager.instance._DownloadRigClothes.StartCoroutine(StoreManager.instance._DownloadRigClothes.DownloadAndApplyEyeLenTexture(name, GameManager.Instance.mainCharacter));
            }
            else if (CategoriesEnumVar.ToString() == "HairAvatarColor")
            {
                CharcterBodyParts.instance.ChangeHairColor(MyIndex);
                SavaCharacterProperties.instance.characterController.hairColorPaletteId = int.Parse(id);
            }
            else if (CategoriesEnumVar.ToString() == "EyeBrowAvatarColor")
            {
                CharcterBodyParts.instance.ChangeEyebrowColor(MyIndex);
                SavaCharacterProperties.instance.characterController.eyeBrowColorPaletteId = int.Parse(id);
            }
            else if (CategoriesEnumVar.ToString() == "EyesAvatarColor")
            {
                CharcterBodyParts.instance.ChangeEyeColor(MyIndex);
                SavaCharacterProperties.instance.characterController.eyesColorPaletteId = int.Parse(id);
            }
            else if (CategoriesEnumVar.ToString() == "LipsAvatarColor")
            {
                CharcterBodyParts.instance.ChangeLipColorForPalette(MyIndex);
                SavaCharacterProperties.instance.characterController.lipsColorPaletteId = int.Parse(id);
            }
            else if (!File.Exists(Application.persistentDataPath + "/" + name))
            {
                StoreManager.instance.load.SetActive(true);
                StartCoroutine(DownloadTextureFile(assetLinkIos));
            }
            else
            {
                applytexture(ReadTextureFromFile(Application.persistentDataPath + "/" + name), assetLinkIos);
            }

            if (!itemAlreadySaved)
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
            //Debug.Log("<color=red>ItemDetail AssignLastClickedBtnHere</color>");
            XanaConstants.xanaConstants._lastClickedBtn = this.gameObject;
        }
    }

    IEnumerator DownloadTextureFile(string TextureURL)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(TextureURL))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError)
            {
                //Debug.Log(uwr.error);
            }
            else
            {
                Texture2D tex = ((DownloadHandlerTexture)uwr.downloadHandler).texture;
                // tex.Compress(true);
                byte[] fileData = tex.EncodeToPNG();
                string filePath = Application.persistentDataPath + "/" + name;
                File.WriteAllBytes(filePath, fileData);
            }
            applytexture(ReadTextureFromFile(Application.persistentDataPath + "/" + name), assetLinkIos);
            uwr.Dispose();
        }
        yield return null;
    }

    public Texture2D ReadTextureFromFile(string path)
    {

        //print("Not downloading,available");
        byte[] _bytes;
        Texture2D mytexture;

        _bytes = File.ReadAllBytes(path);
        if (_bytes == null)
            return null;
        mytexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
        mytexture.LoadImage(_bytes);
        mytexture.Compress(true);
        return mytexture;
    }


    public void applytexture(Texture2D tex, string TextureURL)
    {
        //print("apply texture call");
        switch (_clothetype)
        {
            case "Lip":

                CharcterBodyParts.instance.ChangeLipColor(MyIndex);
                SavaCharacterProperties.instance.characterController.lipsColorId = int.Parse(id);

                if (File.Exists(GameManager.Instance.GetStringFolderPath()) && File.ReadAllText(GameManager.Instance.GetStringFolderPath()) != "")
                {
                    SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();

                    _CharacterData = _CharacterData.CreateFromJSON(File.ReadAllText(GameManager.Instance.GetStringFolderPath()));
                    //if (_CharacterData.myItemObj.Count != 0)
                    if (id == _CharacterData.LipsColorValue.ToString())
                    {
                        itemAlreadySaved = true;
                    }
                }
                break;

            case "Skin":
                // Waqas Ahmad
                CharcterBodyParts.instance.ChangeSkinColor(MyIndex);
                Debug.Log("Skin color slider");
                CharcterBodyParts.instance.ChangeSkinColorSlider(MyIndex);
                SavaCharacterProperties.instance.characterController.skinId = MyIndex;

                if (File.Exists(GameManager.Instance.GetStringFolderPath()) && File.ReadAllText(GameManager.Instance.GetStringFolderPath()) != "")
                {
                    SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();

                    _CharacterData = _CharacterData.CreateFromJSON(File.ReadAllText(GameManager.Instance.GetStringFolderPath()));
                    //if (_CharacterData.myItemObj.Count != 0)
                    if (MyIndex == _CharacterData.SkinId)
                    {
                        itemAlreadySaved = true;
                    }
                }
                break;
        }

        if (StoreManager.instance.UndoBtn)
            StoreManager.instance.UndoBtn.GetComponent<Button>().interactable = true;

        if (!itemAlreadySaved)
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
        StoreManager.instance.load.SetActive(false);
    }

    void SavedButtonClickedBlue()
    {
        StoreManager.instance.SaveStoreBtn.SetActive(true);
        StoreManager.instance.SaveStoreBtn.GetComponent<Image>().color = new Color(0f, 0.5f, 1f, 0.8f);
        StoreManager.instance.GreyRibbonImage.SetActive(false);
        StoreManager.instance.WhiteRibbonImage.SetActive(true);
    }
}
