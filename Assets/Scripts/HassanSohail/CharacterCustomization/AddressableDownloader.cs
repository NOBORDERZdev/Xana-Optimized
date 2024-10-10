﻿using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class AddressableDownloader : MonoBehaviour
{
    public List<Item> presetsItem;
    public int presetItemCount;
    public static AddressableDownloader Instance;
    public static List<AsyncOperationHandle> bundleAsyncOperationHandle = new List<AsyncOperationHandle>();
    public AddressableMemoryReleaser MemoryManager;
    private void Start()
    {
        presetItemCount = 0;
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            MemoryManager = GetComponent<AddressableMemoryReleaser>();
            DownloadCatalogFile();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    bool isDownloading = false;
    public void DownloadCatalogFile()
    {
        if (!isDownloading)
        {
            isDownloading = true;
#if UNITY_EDITOR
            string catalogFilePath = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetValueByName(UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.activeProfileId, "Remote.LoadPath");
            //Debug.LogError(catalogFilePath);
            Debug.Log("<color=red>" + catalogFilePath + "</color=red>");
            catalogFilePath = catalogFilePath.Replace("[BuildTarget]", UnityEditor.EditorUserBuildSettings.activeBuildTarget.ToString());
            catalogFilePath = catalogFilePath + "/XanaAddressableCatalog.json";
            AsyncOperationHandle DownloadingCatalog = Addressables.LoadContentCatalogAsync(catalogFilePath, true);
            DownloadingCatalog.Completed += OnCatalogDownload;
#else
              BuildScriptableObject buildScriptableObject = Resources.Load("BuildVersion/BuildVersion") as BuildScriptableObject;
                      AsyncOperationHandle DownloadingCatalog = Addressables.LoadContentCatalogAsync(buildScriptableObject.addressableCatalogFilePath,true);
                      DownloadingCatalog.Completed += OnCatalogDownload;
#endif
        }
    }
    void OnCatalogDownload(AsyncOperationHandle handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            StartCoroutine(CheckCatalogs());
        }
        else
        {
            ConstantsHolder.isAddressableCatalogDownload = true;
            isDownloading = false;
        }
    }
    IEnumerator CheckCatalogs()
    {
        yield return Addressables.InitializeAsync();
        ConstantsHolder.isAddressableCatalogDownload = true;

    }
    /// <summary>
    /// To Download Addressable object. with call back from coroutine
    /// </summary>
    /// <param name="name">tag or key of a addressable object</param>
    public IEnumerator DownloadAddressableObj(int itemId, string key, string type, string _gender, AvatarController applyOn, Color hairColor, bool applyHairColor = true, bool callFromMultiplayer = false)
    {
        int _counter = 0;
        while (!ConstantsHolder.isAddressableCatalogDownload)
        {
            Debug.Log("<color=red> Waiting for Addressable Catalog to download </color=red>");

            yield return new WaitForSeconds(1f);
        }

        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            if (key.Contains("gambeson")) // To remove gambeson from shirt names
            {
                string tempName = key.Replace("gambeson", "shirt");
                key = tempName;
            }
            if (InventoryManager.instance != null && InventoryManager.instance.loaderForItems)
            {
                InventoryManager.instance.loaderForItems.SetActive(true);
            }
            while (true)
            {
                AsyncOperationHandle loadOp;
                // Debug.LogError("key :- "+key.ToLower());
                //bool flag = false;
                //loadOp = MemoryManager.GetReferenceIfExist(key.ToLower(), ref flag);
                //if (!flag)
                    loadOp = Addressables.LoadAssetAsync<GameObject>(key.ToLower());

                SwitchToShoesHirokoKoshino.Instance?.SwitchLightFor_HirokoKoshino(key.ToLower());
                yield return loadOp;
                if (loadOp.Status == AsyncOperationStatus.Failed)
                {
                    Debug.Log("Fail To load");
                    if (InventoryManager.instance && InventoryManager.instance.loaderForItems && InventoryManager.instance != null)
                        InventoryManager.instance.loaderForItems.SetActive(false);
                    if (GameManager.Instance != null)
                        GameManager.Instance.isStoreAssetDownloading = false;
                    DisableLoadingPanel();
                    yield break;
                }
                else if (loadOp.Status == AsyncOperationStatus.Succeeded)
                {
                    if (loadOp.Result == null || loadOp.Result.Equals(null))  // Added by Ali Hamza to resolve avatar naked issue 
                    {
                        //_counter++;
                        //if (_counter < 5)
                        //{
                        //    Addressables.ClearDependencyCacheAsync(key);
                        //    MemoryManager.RemoveAddressable(key);
                        //}
                        //else
                        //{
                            AddressableDownloader.bundleAsyncOperationHandle.Add(loadOp);
                            applyOn.WearDefaultItem(type, applyOn.gameObject, _gender);
                            yield break;
                        //}
                    }
                    else
                    {
                        if (SceneManager.GetActiveScene().name != "Home")
                        {
                            applyOn.isWearOrNot = true;
                        }
                        //Debug.LogError(":Wear cloth");
                        //loadOp.Result. = key;
                        //MemoryManager.AddToReferenceList(loadOp, key.ToLower());
                        if (PlayerPrefs.GetInt("presetPanel") != 1)
                        {
                            //if (callFromMultiplayer)
                            //{
                            //    applyOn.StichItem(itemId, loadOp.Result as GameObject, type, applyOn.gameObject, mulitplayerHairColor);
                            //}
                            if(type == "Hair")
                            {
                                if (ConstantsHolder.xanaConstants.isStoreActive)
                                {
                                    GameObject downloadedHair = loadOp.Result as GameObject;
                                    Color hairDefaultColor = GetHairDefaultColorFromDownloadedHair(downloadedHair);
                                    applyOn.StichHairWithColor(itemId, downloadedHair, type, applyOn.gameObject, hairDefaultColor, callFromMultiplayer);
                                }
                                else 
                                    applyOn.StichHairWithColor(itemId, loadOp.Result as GameObject, type, applyOn.gameObject, hairColor,callFromMultiplayer);
                            }
                            else
                            {
                                applyOn.StichItem(itemId, loadOp.Result as GameObject, type, applyOn.gameObject, applyHairColor);
                            }
                            if (GameManager.Instance != null)
                                GameManager.Instance.isStoreAssetDownloading = false;
                            DisableLoadingPanel();
                        }
                        else
                        {
                            presetsItem.Add(new Item(itemId, loadOp.Result as GameObject, type));
                            if (presetsItem.Count >= presetItemCount)
                            {
                                StartCoroutine(ApplyPresetItems(applyOn));
                                yield return new WaitForSeconds(5);
                                if (GameManager.Instance.isStoreAssetDownloading)
                                {
                                    GameManager.Instance.isStoreAssetDownloading = false;
                                    DisableLoadingPanel();
                                }
                            }
                        }
                        yield break;
                    }
                }
            }
        }
    }

    public Color GetHairDefaultColorFromDownloadedHair(GameObject downloadedHair)
    {
        string Hair_ColorName = "_BaseColor";
        SkinnedMeshRenderer skinnedMeshRenderer = downloadedHair.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer.sharedMaterials.Length > 1) // In case Of Hat there is 2 material
        {
            if (skinnedMeshRenderer.sharedMaterials[0].name.Contains("Cap") || skinnedMeshRenderer.sharedMaterials[0].name.Contains("Hat") || skinnedMeshRenderer.sharedMaterials[0].name.Contains("Pins"))
                return skinnedMeshRenderer.sharedMaterials[1].GetColor(Hair_ColorName);
            else
                return skinnedMeshRenderer.sharedMaterials[0].GetColor(Hair_ColorName);
        }
        else
            return skinnedMeshRenderer.sharedMaterials[0].GetColor(Hair_ColorName);
    }
    void DisableLoadingPanel()
    {
        if (LoadingHandler.Instance != null)
        {
            LoadingHandler.Instance.presetCharacterLoading.SetActive(false);
        }
    }

    IEnumerator ApplyPresetItems(AvatarController applyOn)
    {
        for (int i = 0; i < presetsItem.Count; i++)
        {
            applyOn.StichItem(presetsItem[i].ItemID, presetsItem[i].ItemPrefab, presetsItem[i].ItemType, applyOn.gameObject, false);
        }
        presetsItem.Clear();
        AddressableDownloader.Instance.presetItemCount = 0;
        GameManager.Instance.isStoreAssetDownloading = false;
        DisableLoadingPanel();

        if (InventoryManager.instance.loaderForItems)
            InventoryManager.instance.loaderForItems.SetActive(false);
        yield return null;
    }
    ///// <summary>
    ///// To Download Addressable Texture. with call back from coroutine
    ///// </summary>
    ///// <param name="name">tag or key of a addressable Texture</param>

    public IEnumerator DownloadAddressableTexture(string key, GameObject applyOn, CurrentTextureType nFTOjectType = 0)
    {
        int _counter = 0;
        while (!ConstantsHolder.isAddressableCatalogDownload)
        {
            yield return new WaitForSeconds(1f);
        }

        CurrentTextureType type = 0;
        if (nFTOjectType == 0)
        {
            if (key.Contains("eyelense", StringComparison.CurrentCultureIgnoreCase))
            {
                type = CurrentTextureType.EyeLense;
            }
            else if (key.Contains("lashes", StringComparison.CurrentCultureIgnoreCase))
            {
                type = CurrentTextureType.EyeLashes;
            }
            else if (key.Contains("brow", StringComparison.CurrentCultureIgnoreCase))
            {
                type = CurrentTextureType.EyeBrows;
            }
            else if (key.Contains("makeup", StringComparison.CurrentCultureIgnoreCase))
            {
                type = CurrentTextureType.Makeup;
            }
            else if (key.Contains("FaceTattoo", StringComparison.CurrentCultureIgnoreCase))
            {
                type = CurrentTextureType.FaceTattoo;
            }
            else if (key.Contains("ChestTattoo", StringComparison.CurrentCultureIgnoreCase))
            {
                type = CurrentTextureType.ChestTattoo;
            }
            else if (key.Contains("LegsTattoo", StringComparison.CurrentCultureIgnoreCase))
            {
                type = CurrentTextureType.LegsTattoo;
            }
            else if (key.Contains("ArmTattoo", StringComparison.CurrentCultureIgnoreCase))
            {
                type = CurrentTextureType.ArmTattoo;
            }
            else if (key.Contains("Mustache", StringComparison.CurrentCultureIgnoreCase))
            {
                type = CurrentTextureType.Mustache;
            }
            else if (key.Contains("EyeLid", StringComparison.CurrentCultureIgnoreCase))
            {
                type = CurrentTextureType.EyeLid;
            }

        }
        else
        {
            type = nFTOjectType;
        }
        if (key != "" && Application.internetReachability != NetworkReachability.NotReachable)
        {

            key = key.ToLower();
            if (key == "eye_color_texture")
            {
                // This Texture Store in Reference no need to download this texture
                applyOn.GetComponent<CharacterBodyParts>().ApplyEyeLenTexture(applyOn.GetComponent<CharacterBodyParts>().Eye_Color_Texture, applyOn);
                GameManager.Instance.isStoreAssetDownloading = false;
                yield return null;
            }
            if (InventoryManager.instance != null && InventoryManager.instance.loaderForItems  && PlayerPrefs.GetInt("presetPanel") != 1)
                InventoryManager.instance.loaderForItems.SetActive(true);
            while (true)
            {
                AsyncOperationHandle loadOp;

                //bool flag = false;

                //loadOp = MemoryManager.GetReferenceIfExist(key, ref flag);
                //if (!flag)
                    loadOp = Addressables.LoadAssetAsync<Texture>(key);

                while (!loadOp.IsDone)
                    yield return loadOp;

                if (loadOp.Status == AsyncOperationStatus.Failed)
                {
                    if (PlayerPrefs.GetInt("presetPanel") != 1)
                    {
                        if (InventoryManager.instance.loaderForItems && InventoryManager.instance != null)
                            InventoryManager.instance.loaderForItems.SetActive(false);

                        GameManager.Instance.isStoreAssetDownloading = false;
                        DisableLoadingPanel();
                    }
                    applyOn.GetComponent<CharacterBodyParts>().SetTextureDefault(type, applyOn);
                    yield break;
                }
                else if (loadOp.Status == AsyncOperationStatus.Succeeded)
                {
                    if (loadOp.Result == null || loadOp.Result.Equals(null))   // Added by Ali Hamza to resolve avatar naked issue
                    {
                        //_counter++;
                        //if (_counter < 5)
                        //{
                        //    Addressables.ClearDependencyCacheAsync(key);
                        //    MemoryManager.RemoveAddressable(key);
                        //}
                        //else
                        //{
                        AddressableDownloader.bundleAsyncOperationHandle.Add(loadOp);
                        applyOn.GetComponent<CharacterBodyParts>().SetTextureDefault(type, applyOn);
                            yield break;
                        //}
                    }
                    else
                    {
                        //MemoryManager.AddToReferenceList(loadOp, key);
                        switch (type)
                        {
                            case CurrentTextureType.Null:
                                break;
                            case CurrentTextureType.FaceTattoo:
                                applyOn.GetComponent<CharacterBodyParts>().ApplyTattoo(loadOp.Result as Texture, applyOn, CurrentTextureType.FaceTattoo);
                                break;
                            case CurrentTextureType.ChestTattoo:
                                applyOn.GetComponent<CharacterBodyParts>().ApplyTattoo(loadOp.Result as Texture, applyOn, CurrentTextureType.ChestTattoo);
                                break;
                            case CurrentTextureType.LegsTattoo:
                                applyOn.GetComponent<CharacterBodyParts>().ApplyTattoo(loadOp.Result as Texture, applyOn, CurrentTextureType.LegsTattoo);
                                break;
                            case CurrentTextureType.ArmTattoo:
                                applyOn.GetComponent<CharacterBodyParts>().ApplyTattoo(loadOp.Result as Texture, applyOn, CurrentTextureType.ArmTattoo);
                                break;
                            case CurrentTextureType.Mustache:
                                applyOn.GetComponent<CharacterBodyParts>().ApplyMustacheTexture(loadOp.Result as Texture, applyOn);
                                break;
                            case CurrentTextureType.EyeLid:
                                applyOn.GetComponent<CharacterBodyParts>().ApplyEyeLidTexture(loadOp.Result as Texture, applyOn);
                                break;
                            case CurrentTextureType.EyeLense:
                                applyOn.GetComponent<CharacterBodyParts>().ApplyEyeLenTexture(loadOp.Result as Texture, applyOn);
                                break;
                            case CurrentTextureType.EyeLashes:
                                applyOn.GetComponent<CharacterBodyParts>().ApplyEyeLashes(loadOp.Result as Texture, applyOn);
                                break;
                            case CurrentTextureType.EyeBrows:
                                applyOn.GetComponent<CharacterBodyParts>().ApplyEyeBrow(loadOp.Result as Texture, applyOn);
                                break;
                            case CurrentTextureType.Skin:
                                break;
                            case CurrentTextureType.Lip:
                                break;
                            case CurrentTextureType.Makeup:
                                applyOn.GetComponent<CharacterBodyParts>().ApplyMakeup(loadOp.Result as Texture, applyOn);
                                break;
                            default:
                                break;
                        }
                        if (InventoryManager.instance != null && InventoryManager.instance.loaderForItems && PlayerPrefs.GetInt("presetPanel") != 1)
                            InventoryManager.instance.loaderForItems.SetActive(false);
                        GameManager.Instance.isStoreAssetDownloading = false;
                        yield break;
                    }

                }
            }
        }
    }
    public IEnumerator DownloadAddressableTextureByName(string groupName, string key, GameObject applyOn, CurrentTextureType nFTOjectType = 0)
    {
        if (groupName != "" && Application.internetReachability != NetworkReachability.NotReachable)
        {
            string address = $"{groupName}/{key}.png"; // Combine group name and key to form the address
            AsyncOperationHandle loadOp;

            //bool flag = false;
            //loadOp = MemoryManager.GetReferenceIfExist(address, ref flag);
            //if (!flag)
                loadOp = Addressables.LoadAssetAsync<Texture>(address);

            while (!loadOp.IsDone)
            {
                yield return null;
            }

            if (loadOp.Status == AsyncOperationStatus.Failed)
            {
                applyOn.GetComponent<CharacterBodyParts>().SetTextureDefault(nFTOjectType, applyOn);
                yield break;
            }
            else if (loadOp.Status == AsyncOperationStatus.Succeeded)
            {
                //MemoryManager.AddToReferenceList(loadOp, address);
                AddressableDownloader.bundleAsyncOperationHandle.Add(loadOp);
                switch (nFTOjectType)
                {
                    case CurrentTextureType.Skin:
                        applyOn.GetComponent<CharacterBodyParts>().ApplyBodyTexture(loadOp.Result as Texture, applyOn);
                        break;
                    case CurrentTextureType.Face:
                        applyOn.GetComponent<CharacterBodyParts>().ApplyFaceTexture(loadOp.Result as Texture, applyOn);
                        break;
                    default:
                        break;
                }
            }
        }
    }
    void SavePresetFristTime()
    {
        if (PlayerPrefs.GetInt("presetPanel") == 1 && PlayerPrefs.GetInt("FristPresetSet") == 0)
        {
            // preset panel is enable so saving preset to account 
            PlayerPrefs.SetInt("presetPanel", 0);
            PlayerPrefs.SetInt("FristPresetSet", 1);
            PlayerPrefs.Save();
            DefaultClothDatabase.instance.GetComponent<SaveCharacterProperties>().SavePlayerProperties();
        }
    }
}

public enum CurrentTextureType
{
    Null,
    FaceTattoo,
    ChestTattoo,
    LegsTattoo,
    ArmTattoo,
    Mustache,
    EyeLid,
    EyeLense,
    EyeLashes,
    EyeBrows,
    Skin,
    Face,
    Lip,
    Makeup
}