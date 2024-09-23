
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Photon.Voice.Unity;
using System.Linq;
using System.IO;
using Photon.Pun.Demo.PunBasics;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class RPCCallforBufferPlayers : MonoBehaviour, IPunInstantiateMagicCallback
{
    [HideInInspector]
    public AssetBundle bundle;
    public AssetBundleRequest newRequest;
    private string OtherPlayerId;
    public static List<string> bundle_Name = new List<string>();
    private bool ItemAlreadyExists = false;
    bool NeedtoDownload = true;
    bool NotNeedtoDownload = true;
    string ClothSlugName = "";
    [SerializeField]
    public static Dictionary<object, object> allPlayerIdData = new Dictionary<object, object>();
    object[] _mydatatosend = new object[3];
    private bool IsNFTCharacter;
    public string GetJsonFolderData()
    {
        if (PlayerPrefs.GetInt("IsLoggedIn") == 1)  // loged from account)
        {
            if (ConstantsHolder.xanaConstants.isNFTEquiped)
            {
                return File.ReadAllText(Application.persistentDataPath + ConstantsHolder.xanaConstants.NFTBoxerJson);
            }
            else
            {
                return File.ReadAllText(Application.persistentDataPath + "/logIn.json");
            }
        }
        else
        {
            return File.ReadAllText(Application.persistentDataPath + "/loginAsGuestClass.json");
        }
    }
    private void Start()
    {
        if (!this.GetComponent<PhotonView>().IsMine && !this.gameObject.GetComponent<Speaker>())
        {
            this.gameObject.AddComponent<Speaker>();
        }

        if (ConstantsHolder.isPenguin)
            return;

        if (ConstantsHolder.isFixedHumanoid)
        {
            _mydatatosend[0] = GetComponent<PhotonView>().ViewID as object;
            _mydatatosend[1] = XANASummitDataContainer.FixedAvatarJson as object;
            _mydatatosend[2] = ConstantsHolder.xanaConstants.isNFTEquiped;

            CallRpcInvoke();

            return;
        }

        if (this.GetComponent<PhotonView>().IsMine)
        {
            _mydatatosend[0] = GetComponent<PhotonView>().ViewID as object;
            _mydatatosend[1] = GetJsonFolderData() as object;
            _mydatatosend[2] = ConstantsHolder.xanaConstants.isNFTEquiped;
            Invoke(nameof(CallRpcInvoke), /*1.2f*/0f);
            //CallRpcInvoke();
        }
    }

    void CallRpcInvoke()
    {
        this.GetComponent<PhotonView>().RPC(nameof(CheckRpc), RpcTarget.AllBuffered, _mydatatosend as object);

    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        MutiplayerController.instance.playerobjects.Add(info.photonView.gameObject);
    }

    //Equipment otherEquip;

    [PunRPC]
    void CheckRpc(object[] Datasend)
    {
        AvatarController otherPlayer;
        string SendingPlayerID = Datasend[0].ToString();
        OtherPlayerId = Datasend[0].ToString();
        if (Datasend.Length > 2)
            IsNFTCharacter = (bool)Datasend[2];

        //for (int j = 0; j < MutiplayerController.instance.playerobjects.Count; j++)
        //{
        //if (MutiplayerController.instance.playerobjects[j] != null && MutiplayerController.instance.playerobjects[j].GetComponent<PhotonView>())
        //{
        if (GetComponent<PhotonView>().ViewID.ToString() == OtherPlayerId)
        {
            SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();
            _CharacterData = JsonUtility.FromJson<SavingCharacterDataClass>(Datasend[1].ToString());

            otherPlayer = gameObject.GetComponent<AvatarController>();
            CharacterBodyParts bodyparts = otherPlayer.GetComponent<CharacterBodyParts>();

            //otherPlayer._CharData = _CharacterData;
            if (IsNFTCharacter)
            {
                bodyparts.head.materials[2].SetInt("_Active", 0);
                bodyparts.body.materials[0].SetInt("_Active", 0);

                //extra blendshape added to character to build muscles on Character
                bodyparts.head.SetBlendShapeWeight(54, 100);
                bodyparts.body.SetBlendShapeWeight(0, 100);

                bodyparts.GetComponent<SwitchToBoxerAvatar>().OnNFTEquipShaderUpdate();

            }

            if (_CharacterData.myItemObj.Count != 0)
            {
                for (int i = 0; i < _CharacterData.myItemObj.Count; i++)
                {

                    if (!otherPlayer.GetComponent<PhotonView>().IsMine)
                    {
                        otherPlayer.GetComponent<AvatarController>().SetAvatarClothDefault(otherPlayer.gameObject, _CharacterData.gender);
                        //CharacterHandler.instance.ActivateAvatarByGender(_CharacterData.gender);
                        //bodyparts.SetAvatarByGender(_CharacterData.gender);


                        if (_CharacterData.avatarType == null || _CharacterData.avatarType == "OldAvatar")
                        {
                            float _rand = UnityEngine.Random.Range(0.1f, 2f);
                            string _gen = _rand <= 1 ? "Male" : "Female";
                            otherPlayer.SetAvatarClothDefault(otherPlayer.gameObject, _gen);
                        }
                        else
                        {
                            //Update Body fate
                            if (_CharacterData.myItemObj[i].ItemName != "")
                            {
                                string type = _CharacterData.myItemObj[i].ItemType;
                                if (type.Contains("Legs") || type.Contains("Chest") || type.Contains("Feet") || type.Contains("Hair") || type.Contains("EyeWearable") || type.Contains("Chain") || type.Contains("Glove"))
                                {
                                    if (type.Contains("Hair") && _CharacterData.hairItemData.Contains("No hair"))
                                    {
                                        if (otherPlayer.GetComponent<AvatarController>().wornHair)
                                            UnStichItem("Hair");
                                    }
                                    else
                                        WearAddreesable(_CharacterData.myItemObj[i].ItemType, _CharacterData.myItemObj[i].ItemName, otherPlayer.gameObject, _CharacterData.HairColor, _CharacterData.gender != null ? _CharacterData.gender : "Male");
                                }
                            }
                            else
                            {
                                if (otherPlayer)
                                {
                                    switch (_CharacterData.myItemObj[i].ItemType)
                                    {
                                        case "Legs":
                                            otherPlayer.WearDefaultItem("Legs", otherPlayer.gameObject, _CharacterData.gender != null ? _CharacterData.gender : "Male");
                                            break;
                                        case "Chest":
                                            otherPlayer.WearDefaultItem("Chest", otherPlayer.gameObject, _CharacterData.gender != null ? _CharacterData.gender : "Male");
                                            break;
                                        case "Feet":
                                            otherPlayer.WearDefaultItem("Feet", otherPlayer.gameObject, _CharacterData.gender != null ? _CharacterData.gender : "Male");
                                            break;
                                        case "Hair":
                                            otherPlayer.WearDefaultItem("Hair", otherPlayer.gameObject, _CharacterData.gender != null ? _CharacterData.gender : "Male");
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else // if player is all default cloths
            {
                _CharacterData.gender = GetComponent<CharacterBodyParts>().AvatarGender.ToString();
                otherPlayer.WearDefaultItem("Legs", otherPlayer.gameObject, _CharacterData.gender != null ? _CharacterData.gender : "Male");
                otherPlayer.WearDefaultItem("Chest", otherPlayer.gameObject, _CharacterData.gender != null ? _CharacterData.gender : "Male");
                otherPlayer.WearDefaultItem("Feet", otherPlayer.gameObject, _CharacterData.gender != null ? _CharacterData.gender : "Male");
                otherPlayer.WearDefaultItem("Hair", otherPlayer.gameObject, _CharacterData.gender != null ? _CharacterData.gender : "Male");
            }
            if (_CharacterData.charactertypeAi == true)
            {
                ApplyAIData(_CharacterData, bodyparts);
            }
            bodyparts.LoadBlendShapes(_CharacterData, otherPlayer.gameObject); // Load BlendShapes

            //if (_CharacterData.eyeTextureName != "" && _CharacterData.eyeTextureName != null)
            //{
            //    StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.eyeTextureName, otherPlayer.gameObject));
            //}
            //if (_CharacterData.eyebrrowTexture != "" && _CharacterData.eyebrrowTexture != null)
            //{
            //    StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.eyebrrowTexture, otherPlayer.gameObject));
            //}
            //if (_CharacterData.makeupName != "" && _CharacterData.makeupName != null)
            //{
            //    StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.makeupName, otherPlayer.gameObject));
            //}
            //if (_CharacterData.eyeLashesName != "" && _CharacterData.eyeLashesName != null)
            //{
            //    StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.eyeLashesName, otherPlayer.gameObject));
            //}

            ////if (_CharacterData.SkinGerdientColor != null && _CharacterData.SssIntensity != null)
            ////{
            ////    bodyparts.StartCoroutine(bodyparts.ImplementColors(_CharacterData.Skin, _CharacterData.LipColor, _CharacterData.SkinGerdientColor, this.gameObject));
            ////}
            ////else
            ////{
            ////    bodyparts.StartCoroutine(bodyparts.ImplementColors(_CharacterData.Skin, _CharacterData.LipColor, this.gameObject));
            ////}

            ////if (_CharacterData.Skin != null && _CharacterData.LipColor != null && _CharacterData.HairColor != null && _CharacterData.EyebrowColor != null && _CharacterData.EyeColor != null)
            ////{
            ////    //bodyparts.StartCoroutine(bodyparts.ImplementColors(_CharacterData.Skin, _CharacterData.LipColor, otherPlayer.gameObject));
            ////    bodyparts.StartCoroutine(bodyparts.ImplementColors(_CharacterData.Skin, _CharacterData.LipColor, _CharacterData.HairColor, _CharacterData.EyebrowColor, _CharacterData.EyeColor, otherPlayer.gameObject));
            ////}

            //// Seperate 
            //if (_CharacterData.Skin != null)
            //{
            //    bodyparts.StartCoroutine(bodyparts.ImplementColors(_CharacterData.Skin, SliderType.Skin, this.gameObject));
            //}
            //if (_CharacterData.EyeColor != null)
            //{
            //    bodyparts.StartCoroutine(bodyparts.ImplementColors(_CharacterData.EyeColor, SliderType.EyesColor, this.gameObject));
            //}
            //if (_CharacterData.LipColor != null)
            //{
            //    bodyparts.StartCoroutine(bodyparts.ImplementColors(_CharacterData.LipColor, SliderType.LipsColor, this.gameObject));
            //}
            //if (_CharacterData.HairColor != null)
            //{
            //    bodyparts.StartCoroutine(bodyparts.ImplementColors(_CharacterData.HairColor, SliderType.HairColor, this.gameObject));
            //}
            //if (_CharacterData.EyebrowColor != null)
            //{
            //    bodyparts.StartCoroutine(bodyparts.ImplementColors(_CharacterData.EyebrowColor, SliderType.EyeBrowColor, this.gameObject));
            //}

            //if (_CharacterData.SkinGerdientColor != null)
            //{
            //    bodyparts.ApplyGredientColor(_CharacterData.SkinGerdientColor, otherPlayer.gameObject);
            //}
            //else
            //{
            //    bodyparts.ApplyGredientDefault(otherPlayer.gameObject);
            //}
            //bodyparts.SetSssIntensity(0, otherPlayer.gameObject);
            //bodyparts.LoadBlendShapes(_CharacterData, bodyparts.gameObject);
            //otherPlayer.LoadBonesData(_CharacterData, otherPlayer.gameObject);


            #region Xana Avatar 1.0 //--> remove for xana avatar2.0
            //    if (_CharacterData.eyeTextureName != "" && _CharacterData.eyeTextureName != null)
            //{
            //    StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.eyeTextureName, otherPlayer.gameObject, CurrentTextureType.EyeLense));
            //}
            //if (_CharacterData.eyebrrowTexture != "" && _CharacterData.eyebrrowTexture != null)
            //{
            //    StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.eyebrrowTexture, otherPlayer.gameObject, CurrentTextureType.EyeBrows));
            //}
            //if (_CharacterData.eyeLashesName != "" && _CharacterData.eyeLashesName != null)
            //{
            //    StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.eyeLashesName, otherPlayer.gameObject, CurrentTextureType.EyeBrowPoints));
            //}
            ////if (_CharacterData.eyeBrowName != "" && _CharacterData.eyeBrowName != null)
            ////{
            ////    AddressableDownloader.Instance.StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.eyeBrowName, this.gameObject));
            ////}
            //if (_CharacterData.makeupName != "" && _CharacterData.makeupName != null)
            //{
            //    StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.makeupName, otherPlayer.gameObject, CurrentTextureType.Makeup));
            //}

            #region New texture are downloading for Boxer NFT
            //New texture are downloading for Boxer NFT 
            //if (!string.IsNullOrEmpty(_CharacterData.faceTattooTextureName) && _CharacterData.faceTattooTextureName != null)
            //    StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.faceTattooTextureName, otherPlayer.gameObject, CurrentTextureType.FaceTattoo));
            //else
            //    this.GetComponent<CharcterBodyParts>().RemoveTattoo(null, otherPlayer.gameObject, CurrentTextureType.FaceTattoo);

            //if (!string.IsNullOrEmpty(_CharacterData.chestTattooTextureName) && _CharacterData.chestTattooTextureName != null)
            //    StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.chestTattooTextureName, otherPlayer.gameObject, CurrentTextureType.ChestTattoo));
            //else
            //    this.GetComponent<CharcterBodyParts>().RemoveTattoo(null, otherPlayer.gameObject, CurrentTextureType.ChestTattoo);

            //if (!string.IsNullOrEmpty(_CharacterData.legsTattooTextureName) && _CharacterData.legsTattooTextureName != null)
            //    StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.legsTattooTextureName, otherPlayer.gameObject, CurrentTextureType.LegsTattoo));
            //else
            //    this.GetComponent<CharcterBodyParts>().RemoveTattoo(null, otherPlayer.gameObject, CurrentTextureType.LegsTattoo);

            //if (!string.IsNullOrEmpty(_CharacterData.armTattooTextureName) && _CharacterData.armTattooTextureName != null)
            //    AddressableDownloader.Instance.StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.armTattooTextureName, otherPlayer.gameObject, CurrentTextureType.ArmTattoo));
            //else
            //    this.GetComponent<CharcterBodyParts>().RemoveTattoo(null, otherPlayer.gameObject, CurrentTextureType.ArmTattoo);

            //if (!string.IsNullOrEmpty(_CharacterData.mustacheTextureName) && _CharacterData.mustacheTextureName != null)
            //    StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.mustacheTextureName, otherPlayer.gameObject, CurrentTextureType.Mustache));
            //else
            //    this.GetComponent<CharcterBodyParts>().RemoveMustacheTexture(null, otherPlayer.gameObject);


            //as eyelids are not looking good so for now we have removed it
            //if (!string.IsNullOrEmpty(_CharacterData.eyeLidTextureName) && _CharacterData.eyeLidTextureName != null)
            //    StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.eyeLidTextureName, otherPlayer.gameObject, CurrentTextureType.EyeLid));
            //else
            //    this.GetComponent<CharcterBodyParts>().RemoveEyeLidTexture(null, otherPlayer.gameObject);


            //if (_CharacterData.SkinGerdientColor != null && _CharacterData.SssIntensity != null)
            //{
            //    bodyParts.StartCoroutine(bodyParts.ImplementColors(_CharacterData.Skin, _CharacterData.LipColor, _CharacterData.SkinGerdientColor, this.gameObject));
            //}
            //else
            //{
            //    bodyParts.StartCoroutine(bodyParts.ImplementColors(_CharacterData.Skin, _CharacterData.LipColor, this.gameObject));
            //}

            //if (_CharacterData.Skin != null && _CharacterData.LipColor != null && _CharacterData.HairColor != null && _CharacterData.EyebrowColor != null && _CharacterData.EyeColor != null)
            //{
            //    //bodyParts.StartCoroutine(bodyParts.ImplementColors(_CharacterData.Skin, _CharacterData.LipColor, this.gameObject));
            //    bodyParts.StartCoroutine(bodyParts.ImplementColors(_CharacterData.Skin, _CharacterData.LipColor, _CharacterData.HairColor, _CharacterData.EyebrowColor, _CharacterData.EyeColor, this.gameObject));
            //}
            #endregion
            // Seperate 
            //    if (_CharacterData.Skin != null)
            //{
            //    bodyparts.StartCoroutine(bodyparts.ImplementColors(_CharacterData.Skin, SliderType.Skin, otherPlayer.gameObject));
            //    //if(IsNFTCharacter)
            //    //{
            //    //    LightPresetNFT _lightPresetNFT = GetComponent<AvatarController>().GetLightPresetValue(_CharacterData.Skin);
            //    //    GetComponent<SwitchToBoxerAvatar>().SwitchLight(_lightPresetNFT);
            //    //}

            //}
            //if (_CharacterData.EyeColor != null)
            //{
            //    bodyparts.StartCoroutine(bodyparts.ImplementColors(_CharacterData.EyeColor, SliderType.EyesColor, otherPlayer.gameObject));
            //}
            //if (_CharacterData.LipColor != null)
            //{
            //    bodyparts.StartCoroutine(bodyparts.ImplementColors(_CharacterData.LipColor, SliderType.LipsColor, otherPlayer.gameObject));
            //}
            ////if (_CharacterData.HairColor != null)
            ////{
            ////    bodyParts.StartCoroutine(bodyParts.ImplementColors(_CharacterData.HairColor, SliderType.HairColor, this.gameObject));
            ////}
            //if (_CharacterData.EyebrowColor != null)
            //{
            //    bodyparts.StartCoroutine(bodyparts.ImplementColors(_CharacterData.EyebrowColor, SliderType.EyeBrowColor, otherPlayer.gameObject));
            //}

            //if (_CharacterData.SkinGerdientColor != null)
            //{
            //    bodyparts.ApplyGredientColor(_CharacterData.SkinGerdientColor, otherPlayer.gameObject);
            //}
            //else
            //{
            //    bodyparts.ApplyGredientDefault(this.gameObject);
            //}

            //if (_CharacterData.SssIntensity != null)
            //{
            //    bodyParts.SetSssIntensity(_CharacterData.SssIntensity, this.gameObject);
            //}
            //else
            //{
            //    bodyParts.SetSssIntensity(bodyParts.defaultSssValue, this.gameObject);
            //}

            //bodyparts.SetSssIntensity(0, otherPlayer.gameObject);
            //bodyparts.LoadBlendShapes(_CharacterData, otherPlayer.gameObject);
            //otherPlayer.LoadBonesData(_CharacterData, otherPlayer.gameObject);

            #endregion
            StartCoroutine(otherPlayer.RPCMaskApply(otherPlayer.gameObject));

            if (otherPlayer.GetComponent<EyesBlinking>())                      // Added by Ali Hamza
            {
                otherPlayer.GetComponent<EyesBlinking>().StoreBlendShapeValues();
                StartCoroutine(otherPlayer.GetComponent<EyesBlinking>().BlinkingStartRoutine());
            }
        }
        //}
        //}
    }



    public void WearAddreesable(string itemtype, string itemName, GameObject applyOn, Color hairColor, string _gender)
    {
        //print("~~~~~~~~ itemtype "+ itemtype + "~~~ itemName " + itemName +"~~ applyOn " +applyOn.name + "~~~ _gender "+ _gender);
        if (!itemName.Contains("md", StringComparison.CurrentCultureIgnoreCase) &&
            !itemName.Contains("default", StringComparison.CurrentCultureIgnoreCase) &&
            !string.IsNullOrEmpty(itemName))
        {
            try
            {
                if (itemtype.Contains("Hair"))
                {
                    if (AddressableDownloader.Instance != null)
                    {
                        //  print("AddressableDownloader.Instance found for hair");
                        AddressableDownloader.Instance.StartCoroutine(AddressableDownloader.Instance.DownloadAddressableObj(-1, itemName, itemtype, _gender, applyOn.GetComponent<AvatarController>(), hairColor, true, true));
                    }
                    else
                    {
                        //  print("~!~!~!~!~ AddressableDownloader.Instance is NULL 1");
                    }
                }
                else
                {
                    if (AddressableDownloader.Instance != null)
                    {
                        //  print("AddressableDownloader.Instance found for other objects");
                        AddressableDownloader.Instance.StartCoroutine(AddressableDownloader.Instance.DownloadAddressableObj(-1, itemName, itemtype, _gender, applyOn.GetComponent<AvatarController>(), Color.clear));
                    }
                    else
                    {
                        //  print("~!~!~!~!~ AddressableDownloader.Instance is NULL 2");
                    }
                }
            }
            catch (Exception e)
            {
                // If Error occur in Downloading 
                // Then wear Default
                applyOn.GetComponent<AvatarController>().WearDefaultItem(itemtype, applyOn, _gender);
                // print("Exception : " + e);
            }
        }
        else
        {
            if (IsNFTCharacter)
            {
                if (itemtype.Contains("Chest"))
                {
                    if (applyOn.GetComponent<AvatarController>().wornShirt)
                    {
                        UnStichItem("Chest");
                        applyOn.GetComponent<CharacterBodyParts>().TextureForShirt(null);
                    }
                }
                else if (itemtype.Contains("Hair"))
                {
                    if (applyOn.GetComponent<AvatarController>().wornHair)
                        UnStichItem("Hair");
                }
                else if (itemtype.Contains("Legs"))
                {
                    if (applyOn.GetComponent<AvatarController>().wornPant)
                    {
                        UnStichItem("Legs");
                        applyOn.GetComponent<CharacterBodyParts>().TextureForPant(null);
                    }
                }
                else if (itemtype.Contains("Feet"))
                {
                    if (applyOn.GetComponent<AvatarController>().wornShoes)
                    {
                        UnStichItem("Feet");
                        applyOn.GetComponent<CharacterBodyParts>().TextureForShoes(null);
                    }

                }
                else if (itemtype.Contains("EyeWearable"))
                {
                    if (applyOn.GetComponent<AvatarController>().wornEyeWearable)
                        UnStichItem("EyeWearable");
                }
                else if (itemtype.Contains("Glove"))
                {
                    if (applyOn.GetComponent<AvatarController>().wornGloves)
                    {
                        UnStichItem("Glove");
                        applyOn.GetComponent<CharacterBodyParts>().TextureForGlove(null);
                    }

                }
                else if (itemtype.Contains("Chain"))
                {
                    if (applyOn.GetComponent<AvatarController>().wornChain)
                        UnStichItem("Chain");
                }

            }
            else
            {
                // Old Implementation
                // {
                //if (itemtype.Contains("Hair"))
                //{
                //    applyOn.GetComponent<AvatarController>().WearDefaultHair(applyOn, hairColor);
                //}
                //else
                //}
                    applyOn.GetComponent<AvatarController>().WearDefaultItem(itemtype, applyOn, _gender);
            }
        }
    }


    void ApplyAIData(SavingCharacterDataClass _CharacterData, CharacterBodyParts applyon)
    {
        applyon.head.SetBlendShapeWeight(_CharacterData.faceItemData, 100);
        applyon.head.SetBlendShapeWeight(_CharacterData.lipItemData, 100);
        applyon.head.SetBlendShapeWeight(_CharacterData.noseItemData, 100);
        applyon.head.SetBlendShapeWeight(_CharacterData.eyeShapeItemData, 100);
        //applyon.head.materials[2].SetColor("_BaseColor", _CharacterData.skin_color);
        //applyon.head.materials[2].SetColor("_Lips_Color", _CharacterData.lip_color);
        //applyon.body.materials[0].SetColor("_BaseColor", _CharacterData.hair_color);
        //if (_CharacterData.skin_color != null)
        //{
        //    StartCoroutine(applyon.ImplementColors(_CharacterData.skin_color, SliderType.Skin, this.gameObject));
        //}
        if (_CharacterData.lip_color != null)
        {
            applyon.head.materials[2].SetColor("_Lips_Color", _CharacterData.lip_color);
        }
        if (_CharacterData.eyeItemData != "" && _CharacterData.eyeItemData != null)
        {

            StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTexture(_CharacterData.eyeItemData, this.gameObject, CurrentTextureType.EyeLense));
        }
        if (_CharacterData.skin_color != "" && _CharacterData.Skin != null)
        {
            if (_CharacterData.ai_gender == "male")
            {
                StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTextureByName("Assets/Store Items Addressables/1k_Boy_Face_Texture", _CharacterData.skin_color, this.gameObject, CurrentTextureType.Face));
                StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTextureByName("Assets/Store Items Addressables/1k_Boy_Body_Texture", _CharacterData.skin_color, this.gameObject, CurrentTextureType.Skin));
            }
            else
            {
                StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTextureByName("Assets/Store Items Addressables/1k_Girl_Face_Textures", _CharacterData.skin_color, this.gameObject, CurrentTextureType.Face));
                StartCoroutine(AddressableDownloader.Instance.DownloadAddressableTextureByName("Assets/Store Items Addressables/1k_Girl_Body_Texture", _CharacterData.skin_color, this.gameObject, CurrentTextureType.Skin));
            }
        }
        if (_CharacterData.hairItemData != null)
        {
            if (_CharacterData.hairItemData.Equals("No hair"))
            {
                if (applyon.GetComponent<AvatarController>().wornHair)
                    UnStichItem("Hair");
            }
            else
                StartCoroutine(AddressableDownloader.Instance.DownloadAddressableObj(-1, _CharacterData.hairItemData, "Hair", _CharacterData.gender != null ? _CharacterData.gender : "Male", applyon.GetComponent<AvatarController>(), _CharacterData.hair_color, true, true));
        }
    }
    public void UnStichItem(string type)
    {
        switch (type)
        {
            case "Chest":
                Destroy(GetComponent<AvatarController>().wornShirt);
                break;
            case "Legs":
                Destroy(GetComponent<AvatarController>().wornPant);
                break;
            case "Hair":
                Destroy(GetComponent<AvatarController>().wornHair);
                break;
            case "Feet":
                Destroy(GetComponent<AvatarController>().wornShoes);
                break;
            case "EyeWearable":
                Destroy(GetComponent<AvatarController>().wornEyeWearable);
                break;
            case "Chain":
                Destroy(GetComponent<AvatarController>().wornChain);
                break;
            case "Glove":
                Destroy(GetComponent<AvatarController>().wornGloves);
                break;

        }
    }
}

