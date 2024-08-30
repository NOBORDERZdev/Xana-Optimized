using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Threading.Tasks;

public class AvatarController : MonoBehaviour
{
    public delegate void CharacterLoaded();
    public static event CharacterLoaded characterLoaded;

    public Stitcher stitcher;
    private ItemDatabase itemDatabase;
    public bool staticPlayer;
    public bool isLoadStaticClothFromJson;
    public string staticClothJson;
    public string clothJson;
    public SkinnedMeshRenderer head;
    public SkinnedMeshRenderer Body;
    public GameObject wornHair, wornPant, wornShirt, wornShose, wornEyewearable, wornGloves, wornChain;
    [HideInInspector]
    public int wornHairId, hairColorPaletteId, wornPantId, wornShirtId, wornShoesId, wornEyewearableId, skinId, faceId, eyeBrowId, eyeBrowColorPaletteId, eyesId, eyesColorId, eyesColorPaletteId, noseId, lipsId, lipsColorId, lipsColorPaletteId, bodyFat, makeupId, eyeLashesId, wornGlovesId, wornChainId;
    [HideInInspector]
    public string presetValue;

    public List<Texture> masks = new List<Texture>();
    CharcterBodyParts bodyParts;
    public bool IsInit = false;

    //NFT avatar color codes
    public NFTColorCodes _nftAvatarColorCodes;
    public bool isVisibleOnCam = false;
    private void Awake()
    {
        bodyParts = this.GetComponent<CharcterBodyParts>();
        if (SceneManager.GetActiveScene().name == "ARModuleActionScene" || SceneManager.GetActiveScene().name == "ARModuleRoomScene" || SceneManager.GetActiveScene().name == "ARModuleRealityScene")
        {
            transform.localScale *= 2;
        }
        stitcher = new Stitcher();
    }

    public void OnEnable()
    {
        BoxerNFTEventManager.OnNFTequip += EquipNFT;
        BoxerNFTEventManager.OnNFTUnequip += UnequipNFT;

        itemDatabase = ItemDatabase.instance;

        if (IsInit)
        {
            SetAvatarClothDefault(this.gameObject);
        }

        string currScene = SceneManager.GetActiveScene().name;//Riken Add Condition for Set Default cloths on AR scene so.......
        if (!currScene.Contains("Main")) // call for worlds only
        {
            //Invoke(nameof(Custom_IntializeAvatar), 0.5f);
            Custom_IntializeAvatar();
            if (XanaConstants.xanaConstants.isNFTEquiped)
            {
                GetComponent<SwitchToBoxerAvatar>().OnNFTEquipShaderUpdate();
            }
        }
        else
        {
            //Debug.LogError("else main");
            if (XanaConstants.xanaConstants)
            {
                if (XanaConstants.xanaConstants.isNFTEquiped)
                {
                    EquipNFT();
                }
                if (XanaConstants.xanaConstants.isNFTEquiped)
                {
                    GetComponent<SwitchToBoxerAvatar>().OnNFTEquipShaderUpdate();
                }
            }
        }
    }

    private void OnDisable()
    {
        BoxerNFTEventManager.OnNFTequip -= EquipNFT;
        BoxerNFTEventManager.OnNFTUnequip -= UnequipNFT;
    }

    public void EquipNFT(bool canModifyFile = false)
    {
        // .Step to do After Equipt NFT
        // .Add Data into NFTBoxerJson 
        // .Disable Store BTN
        // .Make All Items Default [Eyebrow, EyeLashes, Bones, BlendShapes, Items Color[hair,eyebrow,eyes,lip,skin]]
        // .Reset BodyFat
        // .Read Data From Json & load its Properties

        XanaConstants.xanaConstants.isNFTEquiped = true;
        XanaConstants.xanaConstants.isHoldCharacterNFT = true;

        if (bodyParts != null)
        {
            ResetBonesDefault(bodyParts);
            bodyParts.DefaultBlendShapes(this.gameObject);

            bodyParts.DefaultTexture(false);
            ResizeClothToBodyFat(this.gameObject, 0);

            bodyParts.Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetInt("_Active", 0);
            bodyParts.Body_Bone.GetComponent<SkinnedMeshRenderer>().materials[0].SetInt("_Active", 0);

            //extra blendshape added to character to build muscles on Character
            bodyParts.Head.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(54, 100);
            bodyParts.Body_Bone.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, 100);

            BoxerNFTEventManager.OnNFTequipShaderUpdate?.Invoke();

            IntializeAvatar(canModifyFile);
        }

        if (EyesBlinking.instance)
        {
            EyesBlinking.instance.StoreBlendShapeValues();          // Added by Ali Hamza
        }
    }
    public void UnequipNFT()
    {
        XanaConstants.xanaConstants.isNFTEquiped = false;
        ResetBonesDefault(bodyParts);
        bodyParts.DefaultBlendShapes(this.gameObject);

        bodyParts.DefaultTexture(false);
        ResizeClothToBodyFat(this.gameObject, 0);

        if (wornEyewearable)
            UnStichItem("EyeWearable");

        if (wornChain)
            UnStichItem("Chain");

        if (wornGloves)
        {
            bodyParts.TextureForGlove(null);
            UnStichItem("Glove");
        }

        bodyParts.Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetInt("_Active", 1);
        bodyParts.Body_Bone.GetComponent<SkinnedMeshRenderer>().materials[0].SetInt("_Active", 1);

        BoxerNFTEventManager.OnNFTUnequipShaderUpdate?.Invoke();
        BoxerNFTEventManager.NFTLightUpdate?.Invoke(LightPresetNFT.DefaultSkin);
        IntializeAvatar();
    }

    public void SetAvatarClothDefault(GameObject applyOn)
    {
        IsInit = false;
        WearDefaultItem("Legs", applyOn.gameObject);
        WearDefaultItem("Chest", applyOn.gameObject);
        WearDefaultItem("Feet", applyOn.gameObject);
        WearDefaultItem("Hair", applyOn.gameObject);
        applyOn.GetComponent<CharcterBodyParts>().DefaultTexture();
    }
    private void SetItemIdsFromFile(SavingCharacterDataClass _CharacterData)
    {
        presetValue = _CharacterData.PresetValue;
        hairColorPaletteId = _CharacterData.HairColorPaletteValue;
        skinId = _CharacterData.SkinId;
        faceId = _CharacterData.FaceValue;
        eyeBrowId = _CharacterData.EyeBrowValue;
        eyeBrowColorPaletteId = _CharacterData.EyeBrowColorPaletteValue;
        eyesId = _CharacterData.EyeValue;
        eyesColorId = _CharacterData.EyesColorValue;
        eyesColorPaletteId = _CharacterData.EyesColorPaletteValue;
        noseId = _CharacterData.NoseValue;
        lipsId = _CharacterData.LipsValue;
        lipsColorId = _CharacterData.LipsColorValue;
        lipsColorPaletteId = _CharacterData.LipsColorPaletteValue;
        bodyFat = _CharacterData.BodyFat;
        eyeLashesId = _CharacterData.EyeLashesValue;
        makeupId = _CharacterData.MakeupValue;
    }
    /// <summary>
    /// To Inialize Character.
    ///  - Intilaze Store item 
    ///  - Intilaze Character customization (bones, morphes)
    /// </summary>
    /// 

    Color presetHairColor;

    public async void IntializeAvatar(bool canWriteFile = false)
    {
        Debug.Log("AVATAR Initializeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");
        while (!XanaConstants.isAddressableCatalogDownload)
        {
            await Task.Yield();
        }

        if (canWriteFile && /*XanaConstants.xanaConstants.isHoldCharacterNFT &&*/ XanaConstants.xanaConstants.isNFTEquiped)
        {
            BoxerNFTDataClass nftAttributes = new BoxerNFTDataClass();
            string _Path = Application.persistentDataPath + XanaConstants.xanaConstants.NFTBoxerJson;
            nftAttributes = nftAttributes.CreateFromJSON(File.ReadAllText(_Path));
            CreateOrUpdateBoxerFile(nftAttributes);
        }
        Custom_IntializeAvatar();
    }
    public SavingCharacterDataClass _PCharacterData = new SavingCharacterDataClass();
    void Custom_IntializeAvatar()
    {
        if (isLoadStaticClothFromJson)
        {
            SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();
            _CharacterData = new SavingCharacterDataClass();
            _CharacterData = _CharacterData.CreateFromJSON(staticClothJson);
            _PCharacterData = _CharacterData;
            clothJson = staticClothJson;
            if (_CharacterData.myItemObj.Count > 0)
            {
                for (int i = 0; i < _CharacterData.myItemObj.Count; i++)
                {
                    if (!string.IsNullOrEmpty(_CharacterData.myItemObj[i].ItemName))
                    {
                        string type = _CharacterData.myItemObj[i].ItemType;
                        if (type.Contains("Legs") || type.Contains("Chest") || type.Contains("Feet") || type.Contains("Hair") || type.Contains("EyeWearable") || type.Contains("Glove") || type.Contains("Chain"))
                        {
                            if (!_CharacterData.myItemObj[i].ItemName.Contains("md", System.StringComparison.CurrentCultureIgnoreCase))
                            {
                                StartCoroutine(AddressableDownloader.Instance.DownloadAddressableObj(_CharacterData.myItemObj[i].ItemID, _CharacterData.myItemObj[i].ItemName, type, this.gameObject.GetComponent<AvatarController>(), Color.clear));
                            }
                            else
                            {
                                if (XanaConstants.xanaConstants.isNFTEquiped)
                                {
                                    if (_CharacterData.myItemObj[i].ItemType.Contains("Chest"))
                                    {
                                        if (wornShirt)
                                        {
                                            UnStichItem("Chest");
                                            bodyParts.TextureForShirt(null);
                                        }
                                    }
                                    else if (_CharacterData.myItemObj[i].ItemType.Contains("Hair"))
                                    {
                                        if (wornHair)
                                            UnStichItem("Hair");
                                    }
                                    else if (_CharacterData.myItemObj[i].ItemType.Contains("Legs"))
                                    {
                                        if (wornPant)
                                        {
                                            UnStichItem("Legs");
                                            bodyParts.TextureForPant(null);
                                        }
                                    }
                                    else if (_CharacterData.myItemObj[i].ItemType.Contains("Feet"))
                                    {
                                        if (wornShose)
                                        {
                                            UnStichItem("Feet");
                                            bodyParts.TextureForShoes(null);
                                        }

                                    }
                                    else if (_CharacterData.myItemObj[i].ItemType.Contains("EyeWearable"))
                                    {
                                        if (wornEyewearable)
                                            UnStichItem("EyeWearable");
                                    }
                                    else if (_CharacterData.myItemObj[i].ItemType.Contains("Glove"))
                                    {
                                        if (wornGloves)
                                        {
                                            UnStichItem("Glove");
                                            bodyParts.TextureForGlove(null);
                                        }

                                    }
                                    else if (_CharacterData.myItemObj[i].ItemType.Contains("Chain"))
                                    {
                                        if (wornChain)
                                            UnStichItem("Chain");
                                    }

                                }
                                else
                                {
                                    WearDefaultItem(type, this.gameObject);
                                }
                            }
                        }
                        else
                        {
                            WearDefaultItem(_CharacterData.myItemObj[i].ItemType, this.gameObject);
                        }
                    }
                    else // wear the default item of that specific part.
                    {
                        if (XanaConstants.xanaConstants.isNFTEquiped && _CharacterData.myItemObj[i].ItemType.Contains("Chest"))
                        {
                            if (wornShirt)
                                UnStichItem("Chest");
                            bodyParts.TextureForShirt(null);
                        }
                        else
                        {
                            WearDefaultItem(_CharacterData.myItemObj[i].ItemType, this.gameObject);
                        }
                    }
                }
            }

            if (_CharacterData.eyeTextureName != "" && _CharacterData.eyeTextureName != null)
            {
                StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.eyeTextureName, this.gameObject, CurrentTextureType.EyeLense));
            }
            if (_CharacterData.eyebrrowTexture != "" && _CharacterData.eyebrrowTexture != null)
            {
                StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.eyebrrowTexture, this.gameObject, CurrentTextureType.EyeBrows));
            }
            if (_CharacterData.eyeLashesName != "" && _CharacterData.eyeLashesName != null)
            {
                StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.eyeLashesName, this.gameObject, CurrentTextureType.EyeLashes));
            }

            if (_CharacterData.makeupName != "" && _CharacterData.makeupName != null)
            {
                StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.makeupName, this.gameObject, CurrentTextureType.Makeup));
            }

            //New texture are downloading for Boxer NFT 
            if (!string.IsNullOrEmpty(_CharacterData.faceTattooTextureName) && _CharacterData.faceTattooTextureName != null)
                StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.faceTattooTextureName, this.gameObject, CurrentTextureType.FaceTattoo));
            else
                this.GetComponent<CharcterBodyParts>().RemoveTattoo(null, this.gameObject, CurrentTextureType.FaceTattoo);

            if (!string.IsNullOrEmpty(_CharacterData.chestTattooTextureName) && _CharacterData.chestTattooTextureName != null)
                StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.chestTattooTextureName, this.gameObject, CurrentTextureType.ChestTattoo));
            else
                this.GetComponent<CharcterBodyParts>().RemoveTattoo(null, this.gameObject, CurrentTextureType.ChestTattoo);

            if (!string.IsNullOrEmpty(_CharacterData.legsTattooTextureName) && _CharacterData.legsTattooTextureName != null)
                StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.legsTattooTextureName, this.gameObject, CurrentTextureType.LegsTattoo));
            else
                this.GetComponent<CharcterBodyParts>().RemoveTattoo(null, this.gameObject, CurrentTextureType.LegsTattoo);

            if (!string.IsNullOrEmpty(_CharacterData.armTattooTextureName) && _CharacterData.armTattooTextureName != null)
                StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.armTattooTextureName, this.gameObject, CurrentTextureType.ArmTattoo));
            else
                this.GetComponent<CharcterBodyParts>().RemoveTattoo(null, this.gameObject, CurrentTextureType.ArmTattoo);

            if (!string.IsNullOrEmpty(_CharacterData.mustacheTextureName) && _CharacterData.mustacheTextureName != null)
                StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.mustacheTextureName, this.gameObject, CurrentTextureType.Mustache));
            else
                this.GetComponent<CharcterBodyParts>().RemoveMustacheTexture(null, this.gameObject);

            // Seperate 
            if (_CharacterData.Skin != null)
            {
                StartCoroutine(bodyParts.ImplementColors(_CharacterData.Skin, SliderType.Skin, this.gameObject));
            }
            if (_CharacterData.EyeColor != null)
            {
                StartCoroutine(bodyParts.ImplementColors(_CharacterData.EyeColor, SliderType.EyesColor, this.gameObject));
            }
            if (_CharacterData.LipColor != null)
            {
                StartCoroutine(bodyParts.ImplementColors(_CharacterData.LipColor, SliderType.LipsColor, this.gameObject));
            }

            if (_CharacterData.EyebrowColor != null)
            {
                StartCoroutine(bodyParts.ImplementColors(_CharacterData.EyebrowColor, SliderType.EyeBrowColor, this.gameObject));
            }

            if (_CharacterData.SkinGerdientColor != null)
            {
                bodyParts.ApplyGredientColor(_CharacterData.SkinGerdientColor, this.gameObject);
            }
            else
            {
                bodyParts.ApplyGredientDefault(this.gameObject);
            }

            bodyParts.SetSssIntensity(0, this.gameObject);
            bodyParts.LoadBlendShapes(_CharacterData, this.gameObject);
            LoadBonesData(_CharacterData, this.gameObject);
            if (head != null && Body != null)
            {
                head.enabled = Body.enabled = true;
            }
            return;
        }
        if (File.Exists(GameManager.Instance.GetStringFolderPath()) && File.ReadAllText(GameManager.Instance.GetStringFolderPath()) != "") //Check if data exist
        {
            SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();
            _CharacterData = SavaCharacterProperties.instance.SaveItemList; // _CharacterData.CreateFromJSON(File.ReadAllText(GameManager.Instance.GetStringFolderPath()));
            _PCharacterData = _CharacterData;
            clothJson = File.ReadAllText(GameManager.Instance.GetStringFolderPath());
            if (SceneManager.GetActiveScene().name.Contains("Main")) // for store/ main menu
            {
                if (XanaConstants.xanaConstants.metaverseType == XanaConstants.MetaverseType.PMY)
                    return;

                if (_CharacterData.myItemObj.Count > 0)
                {
                    for (int i = 0; i < _CharacterData.myItemObj.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(_CharacterData.myItemObj[i].ItemName))
                        {
                            string type = _CharacterData.myItemObj[i].ItemType;
                            if (type.Contains("Legs") || type.Contains("Chest") || type.Contains("Feet") || type.Contains("Hair") || type.Contains("EyeWearable") || type.Contains("Glove") || type.Contains("Chain"))
                            {
                                //getHairColorFormFile = true;
                                if (!_CharacterData.myItemObj[i].ItemName.Contains("md", System.StringComparison.CurrentCultureIgnoreCase))
                                {
                                    StartCoroutine(AddressableDownloader.Instance.DownloadAddressableObj(_CharacterData.myItemObj[i].ItemID, _CharacterData.myItemObj[i].ItemName, type, this.gameObject.GetComponent<AvatarController>(), Color.clear));
                                }
                                else
                                {
                                    if (PlayerPrefs.HasKey("Equiped") || XanaConstants.xanaConstants.isNFTEquiped)
                                    {
                                        if (_CharacterData.myItemObj[i].ItemType.Contains("Chest"))
                                        {
                                            if (wornShirt)
                                            {
                                                UnStichItem("Chest");
                                                bodyParts.TextureForShirt(null);
                                            }
                                        }
                                        else if (_CharacterData.myItemObj[i].ItemType.Contains("Hair"))
                                        {
                                            if (wornHair)
                                                UnStichItem("Hair");
                                        }
                                        else if (_CharacterData.myItemObj[i].ItemType.Contains("Legs"))
                                        {
                                            // IF fullcostume[3 piece suit] than remove bottom
                                            if (wornPant)
                                            {
                                                UnStichItem("Legs");
                                                bodyParts.TextureForPant(null);
                                            }
                                        }
                                        else if (_CharacterData.myItemObj[i].ItemType.Contains("Feet"))
                                        {
                                            if (wornShose)
                                            {
                                                UnStichItem("Feet");
                                                bodyParts.TextureForShoes(null);
                                            }

                                        }
                                        else if (_CharacterData.myItemObj[i].ItemType.Contains("EyeWearable"))
                                        {
                                            if (wornEyewearable)
                                                UnStichItem("EyeWearable");
                                        }
                                        else if (_CharacterData.myItemObj[i].ItemType.Contains("Glove"))
                                        {
                                            if (wornGloves)
                                            {
                                                UnStichItem("Glove");
                                                bodyParts.TextureForGlove(null);
                                            }

                                        }
                                        else if (_CharacterData.myItemObj[i].ItemType.Contains("Chain"))
                                        {
                                            if (wornChain)
                                                UnStichItem("Chain");
                                        }

                                    }
                                    else
                                    {
                                        WearDefaultItem(type, this.gameObject);
                                    }
                                }
                            }
                            else
                            {
                                WearDefaultItem(_CharacterData.myItemObj[i].ItemType, this.gameObject);
                            }
                        }
                        else // wear the default item of that specific part.
                        {
                            if (XanaConstants.xanaConstants.isNFTEquiped && _CharacterData.myItemObj[i].ItemType.Contains("Chest"))
                            {
                                if (wornShirt)
                                    UnStichItem("Chest");
                                bodyParts.TextureForShirt(null);
                            }
                            else
                            {
                                WearDefaultItem(_CharacterData.myItemObj[i].ItemType, this.gameObject);
                            }
                        }
                    }
                }
                // Added By WaqasAhmad
                // When User Reset From Store 
                // _CharacterData file clear & no Data is available
                // Implemented Default Cloths
                else
                {
                    WearDefaultItem("Legs", this.gameObject);
                    WearDefaultItem("Chest", this.gameObject);
                    WearDefaultItem("Feet", this.gameObject);
                    WearDefaultItem("Hair", this.gameObject);

                    if (wornEyewearable)
                        UnStichItem("EyeWearable");
                    if (wornChain)
                        UnStichItem("Chain");
                    if (wornGloves)
                    {
                        UnStichItem("Glove");
                        bodyParts.TextureForGlove(null);
                    }
                }

                if (_CharacterData.eyeTextureName != "" && _CharacterData.eyeTextureName != null)
                {
                    StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.eyeTextureName, this.gameObject, CurrentTextureType.EyeLense));
                }

                if (_CharacterData.eyeLashesName != "" && _CharacterData.eyeLashesName != null)
                {
                    StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.eyeLashesName, this.gameObject, CurrentTextureType.EyeLashes));
                }
                if (_CharacterData.eyebrrowTexture != "" && _CharacterData.eyebrrowTexture != null)
                {
                    StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.eyebrrowTexture, this.gameObject, CurrentTextureType.EyeBrows));
                }

                if (_CharacterData.makeupName != "" && _CharacterData.makeupName != null)
                {
                    StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.makeupName, this.gameObject, CurrentTextureType.Makeup));
                }

                //New texture are downloading for Boxer NFT 
                if (!string.IsNullOrEmpty(_CharacterData.faceTattooTextureName) && _CharacterData.faceTattooTextureName != null)
                    StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.faceTattooTextureName, this.gameObject, CurrentTextureType.FaceTattoo));
                else
                    this.GetComponent<CharcterBodyParts>().RemoveTattoo(null, this.gameObject, CurrentTextureType.FaceTattoo);

                if (!string.IsNullOrEmpty(_CharacterData.chestTattooTextureName) && _CharacterData.chestTattooTextureName != null)
                    StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.chestTattooTextureName, this.gameObject, CurrentTextureType.ChestTattoo));
                else
                    this.GetComponent<CharcterBodyParts>().RemoveTattoo(null, this.gameObject, CurrentTextureType.ChestTattoo);

                if (!string.IsNullOrEmpty(_CharacterData.legsTattooTextureName) && _CharacterData.legsTattooTextureName != null)
                    StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.legsTattooTextureName, this.gameObject, CurrentTextureType.LegsTattoo));
                else
                    this.GetComponent<CharcterBodyParts>().RemoveTattoo(null, this.gameObject, CurrentTextureType.LegsTattoo);

                if (!string.IsNullOrEmpty(_CharacterData.armTattooTextureName) && _CharacterData.armTattooTextureName != null)
                    StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.armTattooTextureName, this.gameObject, CurrentTextureType.ArmTattoo));
                else
                    this.GetComponent<CharcterBodyParts>().RemoveTattoo(null, this.gameObject, CurrentTextureType.ArmTattoo);

                if (!string.IsNullOrEmpty(_CharacterData.mustacheTextureName) && _CharacterData.mustacheTextureName != null)
                    StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.mustacheTextureName, this.gameObject, CurrentTextureType.Mustache));
                else
                    this.GetComponent<CharcterBodyParts>().RemoveMustacheTexture(null, this.gameObject);

                LoadBonesData(_CharacterData, this.gameObject);

                // Seperate 
                if (_CharacterData.Skin != null)
                {
                    StartCoroutine(bodyParts.ImplementColors(_CharacterData.Skin, SliderType.Skin, this.gameObject));
                    if (XanaConstants.xanaConstants.isNFTEquiped)
                    {
                        BoxerNFTEventManager._lightPresetNFT = GetLightPresetValue(_CharacterData.Skin);
                        BoxerNFTEventManager.NFTLightUpdate?.Invoke(BoxerNFTEventManager._lightPresetNFT);
                    }
                }
                if (_CharacterData.EyeColor != null)
                {
                    StartCoroutine(bodyParts.ImplementColors(_CharacterData.EyeColor, SliderType.EyesColor, this.gameObject));
                }
                if (_CharacterData.LipColor != null)
                {
                    StartCoroutine(bodyParts.ImplementColors(_CharacterData.LipColor, SliderType.LipsColor, this.gameObject));
                }

                if (_CharacterData.EyebrowColor != null)
                {
                    Color tempColor = _CharacterData.EyebrowColor;
                    tempColor.a = 1;
                    _CharacterData.EyebrowColor = tempColor;
                    StartCoroutine(bodyParts.ImplementColors(_CharacterData.EyebrowColor, SliderType.EyeBrowColor, this.gameObject));
                }

                if (_CharacterData.SkinGerdientColor != null)
                {
                    bodyParts.ApplyGredientColor(_CharacterData.SkinGerdientColor, this.gameObject);
                }
                else
                {
                    bodyParts.ApplyGredientDefault(this.gameObject);
                }

                if (_CharacterData.SssIntensity != null)
                {
                    bodyParts.SetSssIntensity(_CharacterData.SssIntensity, this.gameObject);
                }
                else
                {
                    bodyParts.SetSssIntensity(bodyParts.defaultSssValue, this.gameObject);
                }
                SetItemIdsFromFile(_CharacterData);
                bodyParts.LoadBlendShapes(_CharacterData, this.gameObject);

            }
            else // wolrd scence 
            {
                if (GetComponent<PhotonView>() && GetComponent<PhotonView>().IsMine || staticPlayer) // self
                {
                    //SetAvatarClothDefault(this.gameObject);

                    if (_CharacterData.myItemObj.Count > 0)
                    {
                        for (int i = 0; i < _CharacterData.myItemObj.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(_CharacterData.myItemObj[i].ItemName))
                            {
                                string type = _CharacterData.myItemObj[i].ItemType;
                                if (type.Contains("Legs") || type.Contains("Chest") || type.Contains("Feet") || type.Contains("Hair") || type.Contains("EyeWearable") || type.Contains("Glove") || type.Contains("Chain"))
                                {
                                    if (!_CharacterData.myItemObj[i].ItemName.Contains("md", System.StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        StartCoroutine(AddressableDownloader.Instance.DownloadAddressableObj(_CharacterData.myItemObj[i].ItemID, _CharacterData.myItemObj[i].ItemName, type, this.gameObject.GetComponent<AvatarController>(), Color.clear));
                                    }
                                    else
                                    {
                                        if (XanaConstants.xanaConstants.isNFTEquiped)
                                        {
                                            if (_CharacterData.myItemObj[i].ItemType.Contains("Chest"))
                                            {
                                                if (wornShirt)
                                                {
                                                    UnStichItem("Chest");
                                                    bodyParts.TextureForShirt(null);
                                                }
                                            }
                                            else if (_CharacterData.myItemObj[i].ItemType.Contains("Hair"))
                                            {
                                                if (wornHair)
                                                    UnStichItem("Hair");
                                            }
                                            else if (_CharacterData.myItemObj[i].ItemType.Contains("Legs"))
                                            {
                                                if (wornPant)
                                                {
                                                    UnStichItem("Legs");
                                                    bodyParts.TextureForPant(null);
                                                }
                                            }
                                            else if (_CharacterData.myItemObj[i].ItemType.Contains("Feet"))
                                            {
                                                if (wornShose)
                                                {
                                                    UnStichItem("Feet");
                                                    bodyParts.TextureForShoes(null);
                                                }

                                            }
                                            else if (_CharacterData.myItemObj[i].ItemType.Contains("EyeWearable"))
                                            {
                                                if (wornEyewearable)
                                                    UnStichItem("EyeWearable");
                                            }
                                            else if (_CharacterData.myItemObj[i].ItemType.Contains("Glove"))
                                            {
                                                if (wornGloves)
                                                {
                                                    UnStichItem("Glove");
                                                    bodyParts.TextureForGlove(null);
                                                }

                                            }
                                            else if (_CharacterData.myItemObj[i].ItemType.Contains("Chain"))
                                            {
                                                if (wornChain)
                                                    UnStichItem("Chain");
                                            }

                                        }
                                        else
                                        {
                                            WearDefaultItem(type, this.gameObject);
                                        }
                                    }
                                }
                                else
                                {
                                    WearDefaultItem(_CharacterData.myItemObj[i].ItemType, this.gameObject);
                                }
                            }
                            else // wear the default item of that specific part.
                            {
                                if (XanaConstants.xanaConstants.isNFTEquiped && _CharacterData.myItemObj[i].ItemType.Contains("Chest"))
                                {
                                    if (wornShirt)
                                        UnStichItem("Chest");
                                    bodyParts.TextureForShirt(null);
                                }
                                else
                                {
                                    WearDefaultItem(_CharacterData.myItemObj[i].ItemType, this.gameObject);
                                }
                            }
                        }
                    }

                    if (_CharacterData.eyeTextureName != "" && _CharacterData.eyeTextureName != null)
                    {
                        StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.eyeTextureName, this.gameObject, CurrentTextureType.EyeLense));
                    }
                    if (_CharacterData.eyebrrowTexture != "" && _CharacterData.eyebrrowTexture != null)
                    {
                        StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.eyebrrowTexture, this.gameObject, CurrentTextureType.EyeBrows));
                    }
                    if (_CharacterData.eyeLashesName != "" && _CharacterData.eyeLashesName != null)
                    {
                        StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.eyeLashesName, this.gameObject, CurrentTextureType.EyeLashes));
                    }

                    if (_CharacterData.makeupName != "" && _CharacterData.makeupName != null)
                    {
                        StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.makeupName, this.gameObject, CurrentTextureType.Makeup));
                    }

                    //New texture are downloading for Boxer NFT 
                    if (!string.IsNullOrEmpty(_CharacterData.faceTattooTextureName) && _CharacterData.faceTattooTextureName != null)
                        StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.faceTattooTextureName, this.gameObject, CurrentTextureType.FaceTattoo));
                    else
                        this.GetComponent<CharcterBodyParts>().RemoveTattoo(null, this.gameObject, CurrentTextureType.FaceTattoo);

                    if (!string.IsNullOrEmpty(_CharacterData.chestTattooTextureName) && _CharacterData.chestTattooTextureName != null)
                        StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.chestTattooTextureName, this.gameObject, CurrentTextureType.ChestTattoo));
                    else
                        this.GetComponent<CharcterBodyParts>().RemoveTattoo(null, this.gameObject, CurrentTextureType.ChestTattoo);

                    if (!string.IsNullOrEmpty(_CharacterData.legsTattooTextureName) && _CharacterData.legsTattooTextureName != null)
                        StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.legsTattooTextureName, this.gameObject, CurrentTextureType.LegsTattoo));
                    else
                        this.GetComponent<CharcterBodyParts>().RemoveTattoo(null, this.gameObject, CurrentTextureType.LegsTattoo);

                    if (!string.IsNullOrEmpty(_CharacterData.armTattooTextureName) && _CharacterData.armTattooTextureName != null)
                        StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.armTattooTextureName, this.gameObject, CurrentTextureType.ArmTattoo));
                    else
                        this.GetComponent<CharcterBodyParts>().RemoveTattoo(null, this.gameObject, CurrentTextureType.ArmTattoo);

                    if (!string.IsNullOrEmpty(_CharacterData.mustacheTextureName) && _CharacterData.mustacheTextureName != null)
                        StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.mustacheTextureName, this.gameObject, CurrentTextureType.Mustache));
                    else
                        this.GetComponent<CharcterBodyParts>().RemoveMustacheTexture(null, this.gameObject);

                    // Seperate 
                    if (_CharacterData.Skin != null)
                    {
                        StartCoroutine(bodyParts.ImplementColors(_CharacterData.Skin, SliderType.Skin, this.gameObject));
                    }
                    if (_CharacterData.EyeColor != null)
                    {
                        StartCoroutine(bodyParts.ImplementColors(_CharacterData.EyeColor, SliderType.EyesColor, this.gameObject));
                    }
                    if (_CharacterData.LipColor != null)
                    {
                        StartCoroutine(bodyParts.ImplementColors(_CharacterData.LipColor, SliderType.LipsColor, this.gameObject));
                    }

                    if (_CharacterData.EyebrowColor != null)
                    {
                        StartCoroutine(bodyParts.ImplementColors(_CharacterData.EyebrowColor, SliderType.EyeBrowColor, this.gameObject));
                    }

                    if (_CharacterData.SkinGerdientColor != null)
                    {
                        bodyParts.ApplyGredientColor(_CharacterData.SkinGerdientColor, this.gameObject);
                    }
                    else
                    {
                        bodyParts.ApplyGredientDefault(this.gameObject);
                    }

                    bodyParts.SetSssIntensity(0, this.gameObject);
                    bodyParts.LoadBlendShapes(_CharacterData, this.gameObject);
                    LoadBonesData(_CharacterData, this.gameObject);

                }
            }
        }
        if (XanaConstants.xanaConstants.isNFTEquiped)
            LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
        if (head != null && Body != null)
        {
            head.enabled = Body.enabled = true;
        }
    }

    /// <summary>
    /// For Boxer NFT there is no modification in data
    /// We only need to save file each time
    /// No Detection Available for NFT changed so Each time update & Save
    /// </summary>
    void CreateOrUpdateBoxerFile(BoxerNFTDataClass _NFTData)
    {
        CharcterBodyParts charcterBodyParts = CharcterBodyParts.instance;
        SavingCharacterDataClass _CharacterData1 = new SavingCharacterDataClass();

        // Create Class New Object 
        // Setting Default Values into this Object
        {
            _CharacterData1.eyeTextureName = "";
            _CharacterData1.Skin = bodyParts.DefaultSkinColor;
            _CharacterData1.LipColor = bodyParts.DefaultLipColor;
            _CharacterData1.EyebrowColor = bodyParts.DefaultEyebrowColor;
            _CharacterData1.HairColor = bodyParts.DefaultHairColor;

            _CharacterData1.makeupName = bodyParts.defaultMakeup.name;
            _CharacterData1.eyeLashesName = bodyParts.defaultEyelashes.name;
            _CharacterData1.eyebrrowTexture = bodyParts.defaultEyebrow.name;

            _CharacterData1.id = LoadPlayerAvatar.avatarId;
            _CharacterData1.name = LoadPlayerAvatar.avatarName;
            _CharacterData1.thumbnail = LoadPlayerAvatar.avatarThumbnailUrl;
            _CharacterData1.SkinId = this.skinId;
            _CharacterData1.PresetValue = this.presetValue;
            _CharacterData1.FaceValue = this.faceId;
            _CharacterData1.EyeBrowValue = this.eyeBrowId;
            _CharacterData1.EyeLashesValue = this.eyeLashesId;
            _CharacterData1.EyeValue = this.eyesId;
            _CharacterData1.EyesColorValue = this.eyesColorId;
            _CharacterData1.NoseValue = this.noseId;
            _CharacterData1.LipsValue = this.lipsId;
            _CharacterData1.LipsColorValue = this.lipsColorId;
            _CharacterData1.BodyFat = this.bodyFat;
            _CharacterData1.MakeupValue = this.makeupId;
            _CharacterData1.faceMorphed = XanaConstants.xanaConstants.isFaceMorphed;
            _CharacterData1.eyeBrowMorphed = XanaConstants.xanaConstants.isEyebrowMorphed;
            _CharacterData1.eyeMorphed = XanaConstants.xanaConstants.isEyeMorphed;
            _CharacterData1.noseMorphed = XanaConstants.xanaConstants.isNoseMorphed;
            _CharacterData1.lipMorphed = XanaConstants.xanaConstants.isLipMorphed;
            _CharacterData1.punch = _NFTData.punch;
            _CharacterData1.special_move = _NFTData.special_move;
            _CharacterData1.kick = _NFTData.kick;
            _CharacterData1.profile = _NFTData.profile;
            _CharacterData1.speed = _NFTData.speed;
            _CharacterData1.stamina = _NFTData.stamina;
            _CharacterData1.defence = _NFTData.defence;


            _CharacterData1.SavedBones = new List<BoneDataContainer>();
            for (int i = 0; i < charcterBodyParts.BonesData.Count; i++)
            {
                Transform bone = charcterBodyParts.BonesData[i].Obj.transform;
                _CharacterData1.SavedBones.Add(new BoneDataContainer(charcterBodyParts.BonesData[i].Name, bone.localPosition, bone.localEulerAngles, bone.localScale));
            }

            int totaBlendShapes = GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount;
            _CharacterData1.FaceBlendsShapes = new float[totaBlendShapes];

            _CharacterData1.faceTattooTextureName = string.Empty;
            _CharacterData1.chestTattooTextureName = string.Empty;
            _CharacterData1.legsTattooTextureName = string.Empty;
            _CharacterData1.armTattooTextureName = string.Empty;
            _CharacterData1.mustacheTextureName = string.Empty;
            _CharacterData1.eyeLidTextureName = string.Empty;
        }

        int listCurrentIndex = 0;
        _CharacterData1.myItemObj = new List<Item>();

        // Cloths , Gloves , Chains , glasses
        {
            if (!string.IsNullOrEmpty(_NFTData.Pants))
            {
                string tempKey = GetUpdatedKey(_NFTData.Pants, "Pants_");

                _CharacterData1.myItemObj.Add(new Item(-1, tempKey, "Legs"));
                listCurrentIndex++;
            }
            else
            {
                _CharacterData1.myItemObj.Add(new Item(-1, "md", "Legs"));
            }

            if (!string.IsNullOrEmpty(_NFTData.Full_Costumes))
            {
                string tempKey = GetUpdatedKey(_NFTData.Full_Costumes, "Full_Costume_");

                _CharacterData1.myItemObj.Add(new Item(-1, tempKey, "Chest"));
                listCurrentIndex++;
            }
            else
            {
                _CharacterData1.myItemObj.Add(new Item(-1, "md", "Chest"));
                if (wornShirt)
                {
                    UnStichItem("Chest");
                    bodyParts.TextureForShirt(null);
                }
            }

            if (!string.IsNullOrEmpty(_NFTData.Hairs))
            {
                string tempKey = _NFTData.Hairs.Split('-')[0];
                tempKey = GetUpdatedKey(tempKey, "Hairs_");
                // need to split // its also has color code

                _CharacterData1.myItemObj.Add(new Item(-1, tempKey, "Hair"));
                listCurrentIndex++;
            }
            else
            {
                _CharacterData1.myItemObj.Add(new Item(-1, "md", "Hair"));
            }

            if (!string.IsNullOrEmpty(_NFTData.Shoes))
            {
                string tempKey = GetUpdatedKey(_NFTData.Shoes, "Shoes_");

                _CharacterData1.myItemObj.Add(new Item(-1, tempKey, "Feet"));
                listCurrentIndex++;
            }
            else
            {
                _CharacterData1.myItemObj.Add(new Item(-1, "md", "Feet"));
                if (wornShose)
                {
                    UnStichItem("Feet");
                    bodyParts.TextureForShoes(null);
                }
            }

            if (!string.IsNullOrEmpty(_NFTData.Glasses))
            {
                string tempKey = GetUpdatedKey(_NFTData.Glasses, "Glasses_");

                _CharacterData1.myItemObj.Add(new Item(-1, tempKey, "EyeWearable"));
                listCurrentIndex++;
            }
            else
            {
                _CharacterData1.myItemObj.Add(new Item(-1, "md", "EyeWearable"));
            }

            if (!string.IsNullOrEmpty(_NFTData.Gloves))
            {
                string tempKey = GetUpdatedKey(_NFTData.Gloves, "Gloves_");

                _CharacterData1.myItemObj.Add(new Item(-1, tempKey, "Glove"));
                listCurrentIndex++;
            }
            else
            {
                _CharacterData1.myItemObj.Add(new Item(-1, "md", "Glove"));
            }

            if (!string.IsNullOrEmpty(_NFTData.Chains))
            {
                string tempKey = GetUpdatedKey(_NFTData.Chains, "Chains_");

                _CharacterData1.myItemObj.Add(new Item(-1, tempKey, "Chain"));
                listCurrentIndex++;
            }
            else
            {
                _CharacterData1.myItemObj.Add(new Item(-1, "md", "Chain"));
            }
        }
        // Tattoo , Mustache , EyeLid
        {
            if (!string.IsNullOrEmpty(_NFTData.Face_Tattoo))
            {
                string tempKey = GetUpdatedKey(_NFTData.Face_Tattoo, "Face_Tattoo_");
                bodyParts.faceTattooColor = bodyParts.faceTattooColorDefault;
                _CharacterData1.faceTattooTextureName = tempKey;
            }
            else if (!string.IsNullOrEmpty(_NFTData.Forehead_Tattoo))
            {
                string tempKey = GetUpdatedKey(_NFTData.Forehead_Tattoo, "Forehead_Tattoo_");
                bodyParts.faceTattooColor = bodyParts.foreheafTattooColor;
                _CharacterData1.faceTattooTextureName = tempKey;
            }

            if (!string.IsNullOrEmpty(_NFTData.Chest_Tattoo))
            {
                string tempKey = GetUpdatedKey(_NFTData.Chest_Tattoo, "Chest_Tattoo_");
                _CharacterData1.chestTattooTextureName = tempKey;
            }

            if (!string.IsNullOrEmpty(_NFTData.Legs_Tattoo))
            {
                string tempKey = GetUpdatedKey(_NFTData.Legs_Tattoo, "Legs_Tattoo_");
                _CharacterData1.legsTattooTextureName = tempKey;
            }

            if (!string.IsNullOrEmpty(_NFTData.Arm_Tattoo))
            {
                string tempKey = GetUpdatedKey(_NFTData.Arm_Tattoo, "Arm_Tattoo_");
                _CharacterData1.armTattooTextureName = tempKey;
            }

            if (!string.IsNullOrEmpty(_NFTData.Mustache))
            {
                string tempKey = GetUpdatedKey(_NFTData.Mustache, "Mustache_");
                _CharacterData1.mustacheTextureName = tempKey;
            }

            if (!string.IsNullOrEmpty(_NFTData.Eyelid))
            {
                string tempKey = GetUpdatedKey(_NFTData.Eyelid, "Eyelid_");
                _CharacterData1.eyeLidTextureName = tempKey;
            }
        }

        // EyeShape, Lense, Skin, lips , Hairs , Eyebrows
        {
            if (!string.IsNullOrEmpty(_NFTData.Eye_Shapes))
            {
                string tempKey = _NFTData.Eye_Shapes.Replace(" - ", "_");
                tempKey = tempKey.Split('_')[0].Replace(" ", "");
                NFTBoxerEyeData.instance.SetNFTData(tempKey);

                // Add Values in current Object
                for (int i = 0; i < GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount; i++)
                {
                    _CharacterData1.FaceBlendsShapes[i] = GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(i);
                }

                _CharacterData1.SavedBones.Clear();
                for (int i = 0; i < charcterBodyParts.BonesData.Count; i++)
                {
                    Transform bone = charcterBodyParts.BonesData[i].Obj.transform;
                    _CharacterData1.SavedBones.Add(new BoneDataContainer(charcterBodyParts.BonesData[i].Name, bone.localPosition, bone.localEulerAngles, bone.localScale));
                }
            }

            if (!string.IsNullOrEmpty(_NFTData.Eye_Lense))
            {
                string tempKey = GetUpdatedKey(_NFTData.Eye_Lense, "Eye_Lense_");
                _CharacterData1.eyeTextureName = tempKey;
            }

            if (!string.IsNullOrEmpty(_NFTData.Eyebrows))
            {
                string temp = _NFTData.Eyebrows.Replace(" - ", "_").Split('_')[0];

                string tempKey = GetUpdatedKey(temp, "Eyebrows_");
                Debug.Log(tempKey);
                _CharacterData1.eyebrrowTexture = tempKey;
            }

            if (!string.IsNullOrEmpty(_NFTData.Skin))
            {
                string tempKey = GetColorCodeFromNFTKey(_NFTData.Skin);
                _CharacterData1.Skin = GetColorCode(tempKey);
            }

            if (!string.IsNullOrEmpty(_NFTData.Lips))
            {
                string tempKey = GetColorCodeFromNFTKey(_NFTData.Lips);
                _CharacterData1.LipColor = GetColorCode(tempKey);
            }

            if (!string.IsNullOrEmpty(_NFTData.Eyebrows))
            {
                string tempKey = GetColorCodeFromNFTKey(_NFTData.Eyebrows);
                _CharacterData1.EyebrowColor = GetColorCode(tempKey);
            }

            if (!string.IsNullOrEmpty(_NFTData.Hairs))
            {
                string tempKey = GetColorCodeFromNFTKey(_NFTData.Hairs);
                Debug.Log(tempKey);
                _CharacterData1.HairColor = GetColorCode(tempKey);
            }
        }

        string updatedBoxerData = JsonUtility.ToJson(_CharacterData1);
        File.WriteAllText((Application.persistentDataPath + XanaConstants.xanaConstants.NFTBoxerJson), updatedBoxerData);
    }

    public void WearDefaultItem(string type, GameObject applyOn)
    {
        switch (type)
        {
            case "Legs":
                StichItem(-1, ItemDatabase.instance.DefaultPent, type, applyOn);
                break;
            case "Chest":
                StichItem(-1, ItemDatabase.instance.DefaultShirt, type, applyOn);
                break;
            case "Feet":
                StichItem(-1, ItemDatabase.instance.DefaultShoes, type, applyOn);
                break;
            case "Hair":
                StichItem(-1, ItemDatabase.instance.DefaultHair, type, applyOn);
                break;
            default:
                break;
        }
    }
    public void WearDefaultHair(GameObject applyOn, Color hairColor)
    {
        StichItem(-1, ItemDatabase.instance.DefaultHair, "Hair", applyOn, hairColor);
    }


    public void ResetForLastSaved()
    {

        //body fats
        SavaCharacterProperties.instance.SaveItemList.BodyFat = 0;
        //body blends
        CharacterCustomizationManager.Instance.UpdateChBodyShape(0);

        ItemDatabase.instance.RevertSavedCloths();
    }

    public void LastSaved_Reset()
    {
        ItemDatabase.instance.RevertSavedCloths();
    }

    /// <summary>
    /// 1 - Update body according to fat
    /// 2 -Fit cloth according to the selected body type
    /// </summary>
    public void ResizeClothToBodyFat(GameObject applyOn, int bodyFat)
    {
        //Equipment equipment = applyOn.GetComponent<Equipment>();
        CharcterBodyParts bodyparts = applyOn.GetComponent<CharcterBodyParts>();

        float _size3 = 1f + ((float)bodyFat / 100f);

        Debug.Log("Resizing Body Parts & Cloths : " + bodyFat + "  :  " + _size3);

        if (bodyparts._scaleBodyParts.Count > 0)
        {
            for (int i = 0; i < bodyparts._scaleBodyParts.Count; i++)
            {
                bodyparts._scaleBodyParts[i].transform.localScale = new Vector3(_size3, 1, _size3);
            }
        }
    }


    /// <summary>
    /// To Load data from file
    /// </summary>
    /// <param name="_CharacterData"> player data save in file</param>
    /// <param name="applyOn">Object on which data is going to apply</param>
    public void LoadBonesData(SavingCharacterDataClass _CharacterData, GameObject applyOn)
    {
        CharcterBodyParts parts = applyOn.GetComponent<CharcterBodyParts>();
        if (applyOn != null)
        {
            if (_CharacterData != null)
            {
                List<BoneDataContainer> boneData = _CharacterData.SavedBones;
                if (boneData.Count > 0)
                {
                    for (int i = 0; i < boneData.Count; i++)
                    {
                        if (parts.BonesData.Count >= i && boneData[i] != null)
                        {
                            parts.BonesData[i].Obj.transform.localPosition = boneData[i].Pos;
                            parts.BonesData[i].Obj.transform.localScale = boneData[i].Scale;
                        }
                    }
                }
                else
                {
                    ResetBonesDefault(parts);
                }
            }
            else
            {
                if (parts.BonesData.Count > 0)
                {
                    ResetBonesDefault(parts);
                }
            }
        }
    }


    /// <summary>
    /// To reset bones to default pos and scale
    /// </summary>
    /// <param name="parts"> CharcterBodyParts </param>
    public void ResetBonesDefault(CharcterBodyParts parts)
    {
        if (parts.BonesData.Count > 0)
        {
            for (int i = 0; i < parts.BonesData.Count; i++)
            {
                if (parts.BonesData[i].Obj != null)
                {
                    parts.BonesData[i].Obj.transform.localPosition = parts.BonesData[i].Pos;
                    parts.BonesData[i].Obj.transform.localScale = parts.BonesData[i].Scale;
                    parts.BonesData[i].Obj.transform.localEulerAngles = parts.BonesData[i].Rotation;
                }
            }
        }
    }

    /// <summary>
    /// To stich item on player rig
    /// </summary>
    /// <param name="item">Cloth to wear</param>
    /// <param name="applyOn">Player that are going to wear the dress</param>
    public void StichItem(int itemId, GameObject item, string type, GameObject applyOn, bool applyHairColor = true)
    {
        CharcterBodyParts tempBodyParts = applyOn.gameObject.GetComponent<CharcterBodyParts>();
        UnStichItem(type);
        if (item.GetComponent<EffectedParts>() && item.GetComponent<EffectedParts>().texture != null)
        {
            Texture tempTex = item.GetComponent<EffectedParts>().texture;
            masks.Add(tempTex);
            tempBodyParts.ApplyMaskTexture(type, tempTex, this.gameObject);
        }

        if (item.GetComponent<EffectedParts>() && item.GetComponent<EffectedParts>().variation_Texture != null)
        {
            item.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial.SetTexture("_BaseMap", item.GetComponent<EffectedParts>().variation_Texture);
        }

        item = this.stitcher.Stitch(item, applyOn);
        if (type == "Hair")
        {
            if (applyHairColor /*&& _CharData.HairColor != null && getHairColorFormFile */)
            {
                SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();
                if (isLoadStaticClothFromJson)
                {
                    _CharacterData = _CharacterData.CreateFromJSON(staticClothJson);
                }
                else
                {
                    _CharacterData = _CharacterData.CreateFromJSON(File.ReadAllText(GameManager.Instance.GetStringFolderPath()));
                }
                StartCoroutine(tempBodyParts.ImplementColors(_CharacterData.HairColor, SliderType.HairColor, applyOn));
            }
            else if (type == "Hair" && XanaConstants.xanaConstants.isPresetHairColor && presetHairColor != null)
            {
                //getHairColorFormFile = false;
                StartCoroutine(tempBodyParts.ImplementColors(presetHairColor, SliderType.HairColor, applyOn));
                presetHairColor = Color.clear;
            }
        }

        if (SceneManager.GetActiveScene().name != "Main")
        {
            item.layer = 22;
        }
        else
        {

            if (XanaConstants.xanaConstants.isNFTEquiped)
            {
                if (PlayerPrefs.GetInt("IsNFTCollectionBreakingDown") == 1)
                {

                    if (type == "Hair")
                        item.layer = 25;
                    else
                        item.layer = 26;

                    SwitchToShoesHirokoKoshinoNFT.Instance.DisableAllLighting();
                }
                if (PlayerPrefs.GetInt("IsNFTCollectionBreakingDown") == 2)
                {
                    // HIROKO KOSHINO NFT 
                    SwitchToShoesHirokoKoshinoNFT.Instance.SwitchLightFor_HirokoKoshino(PlayerPrefs.GetString("HirokoLight"));
                    item.layer = 11;
                }
            }
            else
            {
                item.layer = 11;
            }
        }
        switch (type)
        {
            case "Chest":
                wornShirt = item;
                wornShirtId = itemId;
                wornShirt.GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen = true;
                break;
            case "Legs":
                wornPant = item;
                wornPantId = itemId;
                wornPant.GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen = true;
                break;
            case "Hair":
                wornHair = item;
                wornHairId = itemId;
                break;
            case "Feet":
                wornShose = item;
                wornShoesId = itemId;
                wornShose.GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen = true;
                break;
            case "EyeWearable":
                wornEyewearable = item;
                wornEyewearableId = itemId;
                break;
            case "Chain":
                wornChain = item;
                wornChainId = itemId;
                break;
            case "Glove":
                wornGloves = item;
                Material m = new Material(wornGloves.GetComponent<SkinnedMeshRenderer>().materials[0]);
                wornGloves.GetComponent<SkinnedMeshRenderer>().materials[0] = m;
                wornGlovesId = itemId;
                break;
        }
        if (item.name.Contains("arabic"))
        {
            // Disable Pant
            if (wornPant)
                wornPant.SetActive(false);

            // Disable Hair
            if (wornHair)
                wornHair.SetActive(false);
        }
        else if (wornShirt && (wornShirt.name.Contains("arabic") || wornShirt.name.Contains("Arabic")))
        {
            // Yes Arabic Wear , new pant or hair disable
            if (wornPant)
                wornPant.SetActive(false);

            if (wornHair)
                wornHair.SetActive(false);
        }
        else if (wornShirt && item.name.Contains("Full_Costume", System.StringComparison.CurrentCultureIgnoreCase))
        {
            if (wornPant)
                wornPant.SetActive(false);
            if (wornChain)
                wornChain.SetActive(false);

            if (gameObject.GetComponent<SwitchToBoxerAvatar>())
                gameObject.GetComponent<SwitchToBoxerAvatar>().OnFullCostumeWear();
        }
        else
        {
            if (wornPant)
                wornPant.SetActive(true);
            if (wornHair)
                wornHair.SetActive(true);
            if (wornChain)
                wornChain.SetActive(true);
        }
        if (PlayerPrefs.GetInt("presetPanel") != 1)
        {
            if (StoreManager.instance.loaderForItems && StoreManager.instance != null)
                StoreManager.instance.loaderForItems.SetActive(false);
        }
    }

    // For Multiplayer Hairs Only
    public void StichItem(int itemId, GameObject item, string type, GameObject applyOn, Color hairColor)
    {
        CharcterBodyParts tempBodyParts = applyOn.gameObject.GetComponent<CharcterBodyParts>();

        UnStichItem(type);
        if (item.GetComponent<EffectedParts>() && item.GetComponent<EffectedParts>().texture != null)
        {
            Texture tempTex = item.GetComponent<EffectedParts>().texture;
            masks.Add(tempTex);
            tempBodyParts.ApplyMaskTexture(type, tempTex, this.gameObject);
        }

        if (item.GetComponent<EffectedParts>() && item.GetComponent<EffectedParts>().variation_Texture != null)
        {
            item.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial.SetTexture("_BaseMap", item.GetComponent<EffectedParts>().variation_Texture);
        }

        item = this.stitcher.Stitch(item, applyOn);
        if (type == "Hair")
        {
            StartCoroutine(tempBodyParts.ImplementColors(hairColor, SliderType.HairColor, applyOn));
        }

        item.layer = 22;
        wornHair = item;
        wornHairId = itemId;
    }

    public void UnStichItem(string type)
    {
        switch (type)
        {
            case "Chest":
                Destroy(wornShirt);
                break;
            case "Legs":
                Destroy(wornPant);
                break;
            case "Hair":
                Destroy(wornHair);
                break;
            case "Feet":
                Destroy(wornShose);
                break;
            case "EyeWearable":
                Destroy(wornEyewearable);
                break;
            case "Chain":
                Destroy(wornChain);
                break;
            case "Glove":
                Destroy(wornGloves);
                break;
        }
    }

    public IEnumerator RPCMaskApply(GameObject applyOn)
    {
        yield return new WaitForSeconds(1);
        if (masks.Count > 0)
        {
            foreach (var mask in masks)
            {
                applyOn.gameObject.GetComponent<CharcterBodyParts>().ApplyMaskTexture(mask.name, mask, this.gameObject);
            }
        }
    }

    public void ApplyPreset(SavingCharacterDataClass _CharacterData)
    {
        presetHairColor = _CharacterData.HairColor;
        if (_CharacterData.myItemObj.Count > 0)
        {
            for (int i = 0; i < _CharacterData.myItemObj.Count; i++)
            {
                if (!string.IsNullOrEmpty(_CharacterData.myItemObj[i].ItemName))
                {
                    string type = _CharacterData.myItemObj[i].ItemType;
                    if (type.Contains("Legs") || type.Contains("Chest") || type.Contains("Feet") || type.Contains("Hair") || type.Contains("EyeWearable"))
                    {
                        if (!_CharacterData.myItemObj[i].ItemName.Contains("md", System.StringComparison.CurrentCultureIgnoreCase))
                        {
                            StartCoroutine(AddressableDownloader.Instance.DownloadAddressableObj(_CharacterData.myItemObj[i].ItemID, _CharacterData.myItemObj[i].ItemName, type, this.gameObject.GetComponent<AvatarController>(), Color.clear));
                        }
                        else
                        {
                            WearDefaultItem(type, this.gameObject);
                        }
                    }
                    else
                    {
                        WearDefaultItem(_CharacterData.myItemObj[i].ItemType, this.gameObject);
                    }
                }
                else // wear the default item of that specific part.
                {
                    WearDefaultItem(_CharacterData.myItemObj[i].ItemType, this.gameObject);
                }
            }
        }
        if (_CharacterData.eyeTextureName != "" && _CharacterData.eyeTextureName != null)
        {
            StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.eyeTextureName, this.gameObject, CurrentTextureType.EyeLense));
        }
        if (_CharacterData.eyebrrowTexture != "" && _CharacterData.eyebrrowTexture != null)
        {
            StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.eyebrrowTexture, this.gameObject, CurrentTextureType.EyeBrows));
        }
        if (_CharacterData.eyeLashesName != "" && _CharacterData.eyeLashesName != null)
        {
            StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.eyeLashesName, this.gameObject, CurrentTextureType.EyeLashes));
        }
        if (_CharacterData.makeupName != "" && _CharacterData.makeupName != null)
        {
            StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.makeupName, this.gameObject, CurrentTextureType.Makeup));
        }
        else
        {
            StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture("nomakeup", this.gameObject, CurrentTextureType.Makeup));
        }
        LoadBonesData(_CharacterData, this.gameObject);

        if (_CharacterData.Skin != null && _CharacterData.LipColor != null && _CharacterData.HairColor != null && _CharacterData.EyebrowColor != null && _CharacterData.EyeColor != null)
        {
            // Seperate 
            if (_CharacterData.Skin != null)
            {
                StartCoroutine(bodyParts.ImplementColors(_CharacterData.Skin, SliderType.Skin, this.gameObject));
            }
            if (_CharacterData.EyeColor != null)
            {
                StartCoroutine(bodyParts.ImplementColors(_CharacterData.EyeColor, SliderType.EyesColor, this.gameObject));
            }
            if (_CharacterData.LipColor != null)
            {
                StartCoroutine(bodyParts.ImplementColors(_CharacterData.LipColor, SliderType.LipsColor, this.gameObject));
            }

            if (_CharacterData.EyebrowColor != null)
            {
                StartCoroutine(bodyParts.ImplementColors(_CharacterData.EyebrowColor, SliderType.EyeBrowColor, this.gameObject));
            }
        }

        if (_CharacterData.SkinGerdientColor != null)
        {
            bodyParts.ApplyGredientColor(_CharacterData.SkinGerdientColor, this.gameObject);
        }
        else
        {
            bodyParts.ApplyGredientDefault(this.gameObject);
        }

        if (_CharacterData.SssIntensity != null)
        {
            bodyParts.SetSssIntensity(_CharacterData.SssIntensity, this.gameObject);
        }
        else
        {
            bodyParts.SetSssIntensity(bodyParts.defaultSssValue, this.gameObject);
        }

        SetItemIdsFromFile(_CharacterData);

        EyesBlinking.instance.isBlinking = false;
        bodyParts.LoadBlendShapes(_CharacterData, this.gameObject);
        bodyParts.ApplyPresiteGredient();

        EyesBlinking.instance.StoreBlendShapeValues();          // Added by Ali Hamza
    }
    string GetUpdatedKey(string Key, string prefixAdded)
    {
        string tempKey;
        if (Key.Contains(" - "))
            tempKey = Key.Replace(" - ", "_");
        else
            tempKey = Key;
        tempKey = tempKey.Replace(" ", "");
        tempKey = prefixAdded + tempKey;
        return tempKey;
    }

    string GetColorCodeFromNFTKey(string key)
    {
        string tempKey;
        if (key.Contains(" - "))
        {
            tempKey = key.Replace(" - ", "_");
            tempKey = tempKey.Split('_').Last();
        }
        else
            tempKey = key;

        tempKey = tempKey.Replace(" ", "");
        return tempKey;
    }


    public Color GetColorCode(string colorCode)
    {
        for (int i = 0; i < _nftAvatarColorCodes.colorCodes.Count; i++)
        {
            if (colorCode.ToLower() == _nftAvatarColorCodes.colorCodes[i].colorName.ToLower())
            {
                return _nftAvatarColorCodes.colorCodes[i].updatedColor;
            }
        }
        return Color.black;
    }

    public LightPresetNFT GetLightPresetValue(Color colorCode)
    {
        for (int i = 0; i < _nftAvatarColorCodes.colorCodes.Count; i++)
        {
            if (colorCode == _nftAvatarColorCodes.colorCodes[i].updatedColor)
            {
                return _nftAvatarColorCodes.colorCodes[i].LightPresetNFT;
            }
        }
        return LightPresetNFT.DefaultSkin;
    }
    void OnBecameInvisible()
    {
        isVisibleOnCam = false;
    }

    void OnBecameVisible()
    {
        isVisibleOnCam = true;
    }
}