using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;

public class SavaCharacterProperties : MonoBehaviour
{
    public static SavaCharacterProperties instance;
    public SavingCharacterDataClass SaveItemList = new SavingCharacterDataClass();
    public FilterBlendShapeSettings _sliderindexes;
    [HideInInspector]
    public static int NeedToShowSplash;


    //private Equipment equipment;
    private CharcterBodyParts charcterBodyParts;
    public AvatarController characterController;

    private void Awake()
    {
        instance = this;
        NeedToShowSplash = 1;
        SaveItemList = new SavingCharacterDataClass();
        charcterBodyParts = GameManager.Instance.mainCharacter.GetComponent<CharcterBodyParts>();
        characterController = GameManager.Instance.mainCharacter.GetComponent<AvatarController>();
    }
    public void Start()
    {
        StartLocal();

        SaveItemList.faceMorphed = false;
        SaveItemList.eyeBrowMorphed = false;
        SaveItemList.eyeMorphed = false;
        SaveItemList.noseMorphed = false;
        SaveItemList.lipMorphed = false;

        if (File.Exists(GameManager.Instance.GetStringFolderPath()) && File.ReadAllText(GameManager.Instance.GetStringFolderPath()) != "")
        {
            SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();
            _CharacterData = _CharacterData.CreateFromJSON(File.ReadAllText(GameManager.Instance.GetStringFolderPath()));

            SaveItemList.myItemObj = _CharacterData.myItemObj;
            SaveItemList.BodyFat = _CharacterData.BodyFat;
            SaveItemList.FaceBlendsShapes = _CharacterData.FaceBlendsShapes;
            SaveItemList.faceMorphed = _CharacterData.faceMorphed;
            SaveItemList.eyeBrowMorphed = _CharacterData.eyeBrowMorphed;
            SaveItemList.eyeMorphed = _CharacterData.eyeMorphed;
            SaveItemList.noseMorphed = _CharacterData.noseMorphed;
            SaveItemList.lipMorphed = _CharacterData.lipMorphed;
            SaveItemList.HairColorPaletteValue = _CharacterData.HairColorPaletteValue;
            SaveItemList.EyeValue = _CharacterData.EyeValue;

            SaveItemList.PresetValue = _CharacterData.PresetValue;
            SaveItemList.SkinId = _CharacterData.SkinId;
            SaveItemList.FaceValue = _CharacterData.FaceValue;
            SaveItemList.EyeBrowValue = _CharacterData.EyeBrowValue;
            SaveItemList.EyeBrowColorPaletteValue = _CharacterData.EyeBrowColorPaletteValue;
            SaveItemList.EyeLashesValue = _CharacterData.EyeLashesValue;
            SaveItemList.EyeValue = _CharacterData.EyeValue;
            SaveItemList.EyesColorValue = _CharacterData.EyesColorValue;
            SaveItemList.EyesColorPaletteValue = _CharacterData.EyesColorPaletteValue;
            SaveItemList.NoseValue = _CharacterData.NoseValue;
            SaveItemList.LipsValue = _CharacterData.LipsValue;
            SaveItemList.LipsColorValue = _CharacterData.LipsColorValue;
            SaveItemList.LipsColorPaletteValue = _CharacterData.LipsColorPaletteValue;
            SaveItemList.BodyFat = _CharacterData.BodyFat;
            SaveItemList.MakeupValue = _CharacterData.MakeupValue;
        }
        AssignCustomSlidersData();
    }

    public void AssignSavedPresets()
    {
        if (File.Exists(GameManager.Instance.GetStringFolderPath()) && File.ReadAllText(GameManager.Instance.GetStringFolderPath()) != "")
        {
            SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();
            _CharacterData = _CharacterData.CreateFromJSON(File.ReadAllText(GameManager.Instance.GetStringFolderPath()));

            for (int i = 0; i < GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount; i++)
            {
                if (_CharacterData.FaceBlendsShapes != null && i < _CharacterData.FaceBlendsShapes.Length)
                    GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(i, _CharacterData.FaceBlendsShapes[i]);
            }

            CharacterCustomizationManager.Instance.UpdateChBodyShape(_CharacterData.BodyFat);// Implementing Save Skin Color
            if (_CharacterData.Skin != null && _CharacterData.SssIntensity != null)
            {
                CharcterBodyParts.instance.ImplementSavedSkinColor(_CharacterData.Skin, _CharacterData.SssIntensity);
            }
            else
            {
                CharcterBodyParts.instance.ImplementSavedSkinColor(_CharacterData.Skin);
            }

            if (_CharacterData.SkinGerdientColor != null)
            {
                CharcterBodyParts.instance.ApplyGredientColor(_CharacterData.SkinGerdientColor, GameManager.Instance.mainCharacter);
            }
            else
            {
                CharcterBodyParts.instance.ApplyGredientDefault(GameManager.Instance.mainCharacter);
            }

        }
    }
    public void AssignSavedPresets_Hack()
    {
        if (File.Exists(GameManager.Instance.GetStringFolderPath()) && File.ReadAllText(GameManager.Instance.GetStringFolderPath()) != "")
        {
            SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();
            _CharacterData = _CharacterData.CreateFromJSON(File.ReadAllText(GameManager.Instance.GetStringFolderPath()));

            for (int i = 0; i < GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount; i++)
            {
                if (_CharacterData.FaceBlendsShapes != null && i < _CharacterData.FaceBlendsShapes.Length)
                    GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(i, _CharacterData.FaceBlendsShapes[i]);
            }
            CharacterCustomizationManager.Instance.UpdateChBodyShape(_CharacterData.BodyFat);
        }
    }

    public void AssignCustomSlidersData()
    {
        if (File.Exists(GameManager.Instance.GetStringFolderPath()) && File.ReadAllText(GameManager.Instance.GetStringFolderPath()) != "")
        {
            SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();
            _CharacterData = _CharacterData.CreateFromJSON(File.ReadAllText(GameManager.Instance.GetStringFolderPath()));

            for (int i = 0; i < GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount; i++)
            {
                if (_sliderindexes.ContainsIndex(i))
                {
                    if (_CharacterData.FaceBlendsShapes != null && i < _CharacterData.FaceBlendsShapes.Length)
                        GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(i, _CharacterData.FaceBlendsShapes[i]);
                }
            }
        }
    }
    public void AssignCustomsliderNewData()
    {
        if (File.Exists(GameManager.Instance.GetStringFolderPath()) && File.ReadAllText(GameManager.Instance.GetStringFolderPath()) != "")
        {
            SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();

            SkinnedMeshRenderer smr = GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>();

            for (int i = 0; i < smr.sharedMesh.blendShapeCount; i++)
            {
                if (_CharacterData.FaceBlendsShapes != null && i < _CharacterData.FaceBlendsShapes.Length)
                    _CharacterData.FaceBlendsShapes[i] = GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(i);
            }
            File.WriteAllText(GameManager.Instance.GetStringFolderPath(), JsonUtility.ToJson(_CharacterData));
            StoreManager.instance.OnSaveBtnClicked();
        }
    }

    public void SavePlayerPropertiesInClassObj()
    {
        SaveItemList.myItemObj.Clear();
        SaveItemList.id = LoadPlayerAvatar.avatarId;
        SaveItemList.name = LoadPlayerAvatar.avatarName;
        SaveItemList.thumbnail = LoadPlayerAvatar.avatarThumbnailUrl;
        StoreManager.instance.GreyRibbonImage.SetActive(true);
        StoreManager.instance.WhiteRibbonImage.SetActive(false);
        StoreManager.instance.SaveStoreBtn.GetComponent<Image>().color = Color.white;
        SaveItemList.myItemObj.Add(new Item(characterController.wornPantId, characterController.wornPant.name, "Legs"));
        SaveItemList.myItemObj.Add(new Item(characterController.wornShirtId, characterController.wornShirt.name, "Chest"));
        SaveItemList.myItemObj.Add(new Item(characterController.wornHairId, characterController.wornHair.name, "Hair"));
        SaveItemList.myItemObj.Add(new Item(characterController.wornShoesId, characterController.wornShose.name, "Feet"));
        if (characterController.wornEyewearable != null)
        {
            SaveItemList.myItemObj.Add(new Item(characterController.wornEyewearableId, characterController.wornEyewearable.name, "EyeWearable"));
        }
        SaveItemList.HairColorPaletteValue = characterController.hairColorPaletteId;
        SaveItemList.myItemObj.Add(new Item(0, "", "Glove"));
        SaveItemList.myItemObj.Add(new Item(0, "", "Chain"));
        SaveItemList.SkinId = characterController.skinId;
        SaveItemList.PresetValue = characterController.presetValue;
        SaveItemList.FaceValue = characterController.faceId;
        SaveItemList.EyeBrowValue = characterController.eyeBrowId;
        SaveItemList.EyeBrowColorPaletteValue = characterController.eyeBrowColorPaletteId;
        SaveItemList.EyeLashesValue = characterController.eyeLashesId;
        SaveItemList.EyeValue = characterController.eyesId;
        SaveItemList.EyesColorValue = characterController.eyesColorId;
        SaveItemList.EyesColorPaletteValue = characterController.eyesColorPaletteId;
        SaveItemList.NoseValue = characterController.noseId;
        SaveItemList.LipsValue = characterController.lipsId;
        SaveItemList.LipsColorValue = characterController.lipsColorId;
        SaveItemList.LipsColorPaletteValue = characterController.lipsColorPaletteId;
        SaveItemList.BodyFat = characterController.bodyFat;
        SaveItemList.MakeupValue = characterController.makeupId;
        for (int i = 0; i < GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount; i++)
        {
            if (i < SaveItemList.FaceBlendsShapes.Length)
                SaveItemList.FaceBlendsShapes[i] = GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(i);
        }
        SaveItemList.SavedBones.Clear();
        for (int i = 0; i < charcterBodyParts.BonesData.Count; i++)
        {
            Transform bone = charcterBodyParts.BonesData[i].Obj.transform;
            SaveItemList.SavedBones.Add(new BoneDataContainer(charcterBodyParts.BonesData[i].Name, bone.localPosition, bone.localEulerAngles, bone.localScale));
        }
        SaveItemList.faceMorphed = XanaConstants.xanaConstants.isFaceMorphed;
        SaveItemList.eyeBrowMorphed = XanaConstants.xanaConstants.isEyebrowMorphed;
        SaveItemList.eyeMorphed = XanaConstants.xanaConstants.isEyeMorphed;
        SaveItemList.noseMorphed = XanaConstants.xanaConstants.isNoseMorphed;
        SaveItemList.lipMorphed = XanaConstants.xanaConstants.isLipMorphed;
        if (File.Exists(GameManager.Instance.GetStringFolderPath()) && File.ReadAllText(GameManager.Instance.GetStringFolderPath()) != "")
        {
            SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();
            _CharacterData = _CharacterData.CreateFromJSON(File.ReadAllText(GameManager.Instance.GetStringFolderPath()));
            _CharacterData.id = SaveItemList.id;
            _CharacterData.name = SaveItemList.name;
            _CharacterData.thumbnail = SaveItemList.thumbnail;
            _CharacterData.myItemObj = SaveItemList.myItemObj;
            _CharacterData.PresetValue = SaveItemList.PresetValue;
            _CharacterData.HairColorPaletteValue = SaveItemList.HairColorPaletteValue;
            _CharacterData.BodyFat = SaveItemList.BodyFat;
            _CharacterData.FaceValue = SaveItemList.FaceValue;
            _CharacterData.EyeBrowValue = SaveItemList.EyeBrowValue;
            _CharacterData.EyeBrowColorPaletteValue = SaveItemList.EyeBrowColorPaletteValue;
            _CharacterData.EyeLashesValue = SaveItemList.EyeLashesValue;
            _CharacterData.EyeValue = SaveItemList.EyeValue;
            _CharacterData.EyesColorValue = SaveItemList.EyesColorValue;
            _CharacterData.EyesColorPaletteValue = SaveItemList.EyesColorPaletteValue;
            _CharacterData.NoseValue = SaveItemList.NoseValue;
            _CharacterData.LipsValue = SaveItemList.LipsValue;
            _CharacterData.LipsColorValue = SaveItemList.LipsColorValue;
            _CharacterData.LipsColorPaletteValue = SaveItemList.LipsColorPaletteValue;
            _CharacterData.MakeupValue = SaveItemList.MakeupValue;

            _CharacterData.FaceBlendsShapes = SaveItemList.FaceBlendsShapes;
            _CharacterData.faceMorphed = SaveItemList.faceMorphed;
            _CharacterData.eyeBrowMorphed = SaveItemList.eyeBrowMorphed;
            _CharacterData.eyeMorphed = SaveItemList.eyeMorphed;
            _CharacterData.noseMorphed = SaveItemList.noseMorphed;
            _CharacterData.lipMorphed = SaveItemList.lipMorphed;
            _CharacterData.SavedBones = SaveItemList.SavedBones;
            _CharacterData.Skin = charcterBodyParts.GetBodyColor();
            _CharacterData.SkinGerdientColor = charcterBodyParts.GetSkinGredientColor();
            _CharacterData.SkinId = SaveItemList.SkinId;

            // Added By WaqasAhmad
            _CharacterData.LipColor = charcterBodyParts.GetLipColor();
            _CharacterData.HairColor = charcterBodyParts.GetHairColor();
            _CharacterData.EyebrowColor = charcterBodyParts.GetEyebrowColor();
            _CharacterData.EyeColor = charcterBodyParts.GetEyeColor();
            //
            _CharacterData.eyeTextureName = GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().materials[0].GetTexture("_Main_Trexture").name;
            _CharacterData.eyebrrowTexture = GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().materials[1].GetTexture("_BaseMap").name;
            _CharacterData.eyeLashesName = GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().materials[3].GetTexture("_BaseMap").name;
            _CharacterData.makeupName = GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().materials[2].GetTexture("_Base_Texture").name;

            _CharacterData.FaceBlendsShapes = SaveItemList.FaceBlendsShapes;
            string bodyJson = JsonUtility.ToJson(_CharacterData);
            File.WriteAllText(GameManager.Instance.GetStringFolderPath(), bodyJson);
        }
        else  // IF NOT EXISTS THEN WRITE THE NEW FILE
        {
            //SavingCharacterDataClass SubCatString = new SavingCharacterDataClass();
            string bodyJson = JsonUtility.ToJson(SaveItemList, true);
            File.WriteAllText(GameManager.Instance.GetStringFolderPath(), bodyJson);
        }
    }
    public void SavePlayerProperties()
    {
        SavePlayerPropertiesInClassObj();
        if (PlayerPrefs.GetInt("IsLoggedIn") == 1)
            ServerSIdeCharacterHandling.Instance.CreateUserOccupiedAsset(() =>
            {
            });
    }

    public void CreateFileFortheFirstTime()
    {
        SavingCharacterDataClass SubCatString = new SavingCharacterDataClass();
        SubCatString.FaceBlendsShapes = new float[GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount];
        string bodyJson = JsonUtility.ToJson(SubCatString);
        File.WriteAllText(GameManager.Instance.GetStringFolderPath(), bodyJson);
    }

    public void SetDefaultData()
    {
        SavingCharacterDataClass SubCatString = new SavingCharacterDataClass();
        string bodyJson = JsonUtility.ToJson(SaveItemList);
        File.WriteAllText(GameManager.Instance.GetStringFolderPath(), bodyJson);
    }

    public void LoadMorphsfromFile()
    {
        Start();
        StartLocal();
    }
    //local file loading
    #region Local
    public void StartLocal()
    {
        SaveItemList.FaceBlendsShapes = new float[GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount];
        if (File.Exists(GameManager.Instance.GetStringFolderPath()) && File.ReadAllText(GameManager.Instance.GetStringFolderPath()) != "")
        {
            SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();
            _CharacterData = _CharacterData.CreateFromJSON(File.ReadAllText(GameManager.Instance.GetStringFolderPath()));

            SaveItemList.myItemObj = _CharacterData.myItemObj;
            SaveItemList.BodyFat = _CharacterData.BodyFat;
            SaveItemList.FaceBlendsShapes = _CharacterData.FaceBlendsShapes;

            float[] blendValues = new float[GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount];
            for (int i = 0; i < GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount; i++)
            {
                if (!_sliderindexes.ContainsIndex(i))
                {
                    blendValues[i] = GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(i);
                    GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(i, 0);
                }
            }

            SaveItemList.FaceBlendsShapes = blendValues;
            if (CharacterCustomizationManager.Instance)
            {
                CharacterCustomizationManager.Instance.UpdateChBodyShape(_CharacterData.BodyFat);
            }
            for (int i = 0; i < GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount; i++)
            {
                if (_sliderindexes.ContainsIndex(i))
                {
                    if (_CharacterData.FaceBlendsShapes != null && i < _CharacterData.FaceBlendsShapes.Length)
                        GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(i, _CharacterData.FaceBlendsShapes[i]);
                }
            }
        }
    }
    #endregion
}

[System.Serializable]
public class SavingCharacterDataClass
{
    public string id;
    public string name;
    public string thumbnail;
    public List<Item> myItemObj;

    public List<BoneDataContainer> SavedBones;
    public int SkinId;
    public Color Skin;
    public Color LipColor;
    public Color HairColor;
    public Color EyebrowColor;
    public Color EyeColor;
    public bool isSkinColorChanged;
    public bool isLipColorChanged;
    public int HairColorPaletteValue;
    public int BodyFat;
    public int FaceValue;
    public int NoseValue;
    public int EyeValue;
    public int EyesColorValue;
    public int EyesColorPaletteValue;
    public int EyeBrowValue;
    public int EyeBrowColorPaletteValue;
    public int EyeLashesValue;
    public int MakeupValue;
    public int LipsValue;
    public int LipsColorValue;
    public int LipsColorPaletteValue;
    public bool faceMorphed;
    public bool eyeBrowMorphed;
    public bool eyeMorphed;
    public bool noseMorphed;
    public bool lipMorphed;
    public string PresetValue;
    public string eyeTextureName;
    public string eyeLashesName;
    //public string eyeBrowName;
    public string makeupName;
    public float SssIntensity;
    public Color SkinGerdientColor;
    public string eyebrrowTexture;

    //nft avatar extra keys added.
    public string mustacheTextureName;
    public string faceTattooTextureName;
    public string chestTattooTextureName;
    public string legsTattooTextureName;
    public string armTattooTextureName;
    public string eyeLidTextureName;

    public float[] FaceBlendsShapes;

    public int stamina;
    public int speed;
    public string profile;
    public int defence;
    public int special_move;
    public int punch;
    public int kick;

    public SavingCharacterDataClass CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<SavingCharacterDataClass>(jsonString);
    }
    public SavingCharacterDataClass()
    {

    }
}

public class BoxerNFTDataClass
{
    public bool isNFTAquiped = false;
    public string id;
    //public List<BoneDataContainer> SavedBones;
    public string Gloves;
    public string Glasses;
    public string Full_Costumes;
    public string Chains;
    public string Hairs;
    public string Face_Tattoo;
    public string Forehead_Tattoo;
    public string Chest_Tattoo;
    public string Arm_Tattoo;
    public string Legs_Tattoo;
    public string Shoes;
    public string Mustache;
    public string Pants;
    public string Eyebrows;
    public string Lips;
    public string Heads;
    public string Eye_Shapes;
    public string Skin;
    public string Eye_Lense;
    public string Eyelid;
    public int stamina;
    public int speed;
    public string profile;
    public int defence;
    public int special_move;
    public int punch;
    public int kick;

    //public float[] FaceBlendsShapes;

    public BoxerNFTDataClass CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<BoxerNFTDataClass>(jsonString);
    }
}