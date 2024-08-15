using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SPAAIDresser : MonoBehaviour
{
    public SkinnedMeshRenderer _body;
    public List<SkinnedMeshRenderer> CurNPCCloths = new List<SkinnedMeshRenderer>();
    public Color DefaultSkinColor, DefaultHairColor;

    private GameObject wornShirt, wornPant, wornHair, wornShose;
    private string shirt_TextureName, Pent_TextureName, Shoes_TextureName, Skin_ColorName, Eye_ColorName, Hair_ColorName;
    private Stitcher stitcher;

    //[HideInInspector]
    public SavingCharacterDataClass AvatarJson;

    public TMPro.TextMeshProUGUI npcName;

    private void Start()
    {
        stitcher = new Stitcher();
        shirt_TextureName = "_Shirt_Mask";
        Pent_TextureName = "_Pant_Mask";
        Shoes_TextureName = "_Shoes_Mask";
        Hair_ColorName = "_Color";
        Eye_ColorName = "_Mask_Color";

        Custom_InitializeAvatar(AvatarJson);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>())
        {
            if (other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                UpdateClothingViewHandle(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>())
        {
            if (other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                UpdateClothingViewHandle(false);
            }
        }
    }

    void UpdateClothingViewHandle(bool _state)
    {
        if (CurNPCCloths.Count > 0)
        {
            foreach (SkinnedMeshRenderer smr in CurNPCCloths)
            {
                smr.updateWhenOffscreen = _state;
            }
        }
    }

    void Custom_InitializeAvatar(SavingCharacterDataClass _avatarClothData)
    {
        SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();
        _CharacterData = _avatarClothData;

        if (_CharacterData.myItemObj.Count > 0)
        {
            for (int i = 0; i < _CharacterData.myItemObj.Count; i++)
            {
                if (string.IsNullOrEmpty(_CharacterData.myItemObj[i].ItemName))
                    continue;
                DownloadAddressableWearableWearable(_CharacterData.myItemObj[i].ItemName, _CharacterData.myItemObj[i].ItemType);
            }
        }
    }

    private void DownloadAddressableWearableWearable(string key, string ObjectType)
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            try
            {
                AsyncOperationHandle loadObj;//= Addressables.LoadAssetAsync<GameObject>(key.ToLower());
                //bool flag = false;
                //loadObj = AddressableDownloader.Instance.MemoryManager.GetReferenceIfExist(key.ToLower(), ref flag);
                //if (!flag)
                    loadObj = Addressables.LoadAssetAsync<GameObject>(key.ToLower());
                loadObj.Completed += operationHandle =>
                {
                    OnLoadCompleted(operationHandle, ObjectType, key.ToLower());
                };
            }
            catch (System.Exception)
            {
                WearDefault(ObjectType); // wear default cloth
                throw new System.Exception("Error occur in loading addressable. Wear DefaultAvatar");
            }
        }
    }

    private void OnLoadCompleted(AsyncOperationHandle handle, string ObjectType, string key)
    {

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            AddressableDownloader.bundleAsyncOperationHandle.Add(handle);
            GameObject loadedObject = handle.Result as GameObject;
            if (loadedObject != null)
            {
                StichItem((GameObject)(object)loadedObject, ObjectType, this.gameObject, false);
            }
            else if (loadedObject.Equals(null) || loadedObject == null)
            {
                WearDefault(ObjectType);
                Debug.LogError("Loaded GameObject is null. Handle the error appropriately.");
            }

            AddressableDownloader.Instance.MemoryManager.AddToReferenceList(handle, key);
        }
        else if (handle.Status == AsyncOperationStatus.Failed)
        {
            WearDefault(ObjectType); // wear default cloth
            Debug.LogError("Failed to load addressable: " + handle.OperationException);
        }
    }

    private void WearDefault(string type)
    {
        switch (type)
        {
            case "Chest":
                StichItem(DefaultClothDatabase.instance.DefaultShirt, "Chest", this.gameObject, false);
                break;
            case "Legs":
                StichItem(DefaultClothDatabase.instance.DefaultPent, "Legs", this.gameObject, false);
                break;
            case "Feet":
                StichItem(DefaultClothDatabase.instance.DefaultShoes, "Feet", this.gameObject, false);
                break;
            case "Hair":
                StichItem(DefaultClothDatabase.instance.DefaultHair, "Hair", this.gameObject, false);
                break;
            default:
                break;
        }
    }

    private void StichItem(GameObject item, string type, GameObject applyOn, bool applyHairColor = true)
    {
        UnStichItem(type);
        if (item.GetComponent<EffectedParts>() && item.GetComponent<EffectedParts>().texture != null)
        {
            Texture tempTex = item.GetComponent<EffectedParts>().texture;
            ApplyMaskTexture(type, tempTex, this.gameObject);
        }

        if (item.GetComponent<EffectedParts>() && item.GetComponent<EffectedParts>().variation_Texture != null)
        {
            item.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial.SetTexture("_BaseMap", item.GetComponent<EffectedParts>().variation_Texture);
        }

        item = this.stitcher.Stitch(item, applyOn);
        CurNPCCloths.Add(item.GetComponentInChildren<SkinnedMeshRenderer>());
        if (type == "Hair")
        {
            StartCoroutine(ImplementColors(Color.black, SliderType.HairColor, applyOn));
        }
        item.layer = 11;
        switch (type)
        {
            case "Chest":
                wornShirt = item;
                break;
            case "Legs":
                wornPant = item;
                break;
            case "Hair":
                wornHair = item;
                break;
            case "Feet":
                wornShose = item;
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
        else
        {
            if (wornPant)
                wornPant.SetActive(true);
            if (wornHair)
                wornHair.SetActive(true);
        }
    }

    private void UnStichItem(string type)
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
        }
    }

    private void ApplyMaskTexture(string type, Texture texture, GameObject applyOn)
    {
        if (type.Contains("Chest") || type.Contains("Shirt") || type.Contains("arabic"))
        {
            TextureForShirt(texture);
        }
        else if (type.Contains("Legs") || type.Contains("pant") || type.Contains("Pants"))
        {
            TextureForPant(texture);
        }
        else if (type.Contains("Feet") || type.Contains("Shose", System.StringComparison.CurrentCultureIgnoreCase) || type.Contains("Plain_mask", System.StringComparison.CurrentCultureIgnoreCase))
        {
            TextureForShoes(texture);
        }
    }

    private void TextureForShirt(Texture texture)
    {
        _body.materials[0].SetTexture(shirt_TextureName, texture);
    }

    // Set texture For 
    private void TextureForPant(Texture texture)
    {
        _body.materials[0].SetTexture(Pent_TextureName, texture);
    }

    private void TextureForShoes(Texture texture)
    {
        _body.materials[0].SetTexture(Shoes_TextureName, texture);
    }

    public IEnumerator ImplementColors(Color _color, SliderType _objColor, GameObject applyOn)
    {
        yield return new WaitForSeconds(0f);

        switch (_objColor)
        {
            case SliderType.Skin:
                {
                    if (new Vector3(_color.r, _color.b, _color.g) != new Vector3(0.00f, 0.00f, 0.00f) /*!SkinColor.Compare(Color.black)*/)
                    {
                        applyOn.GetComponent<CharacterBodyParts>().head.materials[2].SetColor(Skin_ColorName, _color);
                        applyOn.GetComponent<CharacterBodyParts>().body.materials[0].SetColor(Skin_ColorName, _color);
                    }
                    else
                    {
                        applyOn.GetComponent<CharacterBodyParts>().head.materials[2].SetColor(Skin_ColorName, DefaultSkinColor);
                        applyOn.GetComponent<CharacterBodyParts>().body.materials[0].SetColor(Skin_ColorName, DefaultSkinColor);
                    }
                }
                break;

            case SliderType.HairColor:
                {
                    if (new Vector3(_color.r, _color.b, _color.g) != new Vector3(0.00f, 0.00f, 0.00f))
                    {
                        AvatarController ac = applyOn.GetComponent<AvatarController>();
                        if (ac.wornHair != null)
                        {
                            if (ac.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].name.Contains("_Band"))
                            {
                                ac.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor(Eye_ColorName, _color);
                            }
                            else if (ac.wornHair.GetComponent<SkinnedMeshRenderer>().materials.Length > 1)
                            {
                                if (ac.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].name.Contains("Cap") ||
                                   ac.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].name.Contains("Hat"))
                                    ac.wornHair.GetComponent<SkinnedMeshRenderer>().materials[1].SetColor(Hair_ColorName, _color);
                                else
                                    ac.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor(Hair_ColorName, _color);
                            }
                            else
                                ac.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor(Hair_ColorName, _color);
                        }
                    }
                }
                break;

            case SliderType.EyesColor:
                {
                    if (new Vector3(_color.r, _color.b, _color.g) != new Vector3(0.00f, 0.00f, 0.00f))
                    {
                        applyOn.GetComponent<CharacterBodyParts>().head.materials[0].SetColor(Eye_ColorName, _color);
                    }
                    else
                    {
                        applyOn.GetComponent<CharacterBodyParts>().head.materials[0].SetColor(Eye_ColorName, Color.white);
                    }
                }
                break;

            default:
                break;
        }
    }

}
