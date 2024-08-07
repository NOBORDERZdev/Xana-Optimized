using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableDownloader : MonoBehaviour
{
    public List<Item> presetsItem;
    public int presetItemCount;
    public static AddressableDownloader Instance;
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
            //DownloadCatalogFile();
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
            XanaConstants.isAddressableCatalogDownload = true;
            isDownloading = false;
        }
    }
    IEnumerator CheckCatalogs()
    {
        yield return Addressables.InitializeAsync();
        XanaConstants.isAddressableCatalogDownload = true;

    }
    /// <summary>
    /// To Download Addressable object. with call back from coroutine
    /// </summary>
    /// <param name="name">tag or key of a addressable object</param>
    public IEnumerator DownloadAddressableObj(int itemId, string key, string type, AvatarController applyOn, Color mulitplayerHairColor, bool applyHairColor = true, bool callFromMultiplayer = false)
    {
        int _counter = 0;
        while (!XanaConstants.isAddressableCatalogDownload)
        {
            yield return new WaitForSeconds(1f);
        }

        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            if (key.Contains("gambeson")) // To remove gambeson from shirt names
            {
                string tempName = key.Replace("gambeson", "shirt");
                key = tempName;
            }
            if (StoreManager.instance.loaderForItems && StoreManager.instance != null)
            {
                StoreManager.instance.loaderForItems.SetActive(true);
            }
            while (true)
            {
                AsyncOperationHandle loadOp;

                bool flag = false;
                loadOp = MemoryManager.GetReferenceIfExist(key.ToLower(), ref flag);
                if (!flag)
                    loadOp = Addressables.LoadAssetAsync<GameObject>(key.ToLower());

                SwitchToShoesHirokoKoshinoNFT.Instance?.SwitchLightFor_HirokoKoshino(key.ToLower());
                yield return loadOp;
                if (loadOp.Status == AsyncOperationStatus.Failed)
                {
                    if (StoreManager.instance.loaderForItems && StoreManager.instance != null)
                        StoreManager.instance.loaderForItems.SetActive(false);
                    if (GameManager.Instance != null)
                        GameManager.Instance.isStoreAssetDownloading = false;
                    DisableLoadingPanel();
                    yield break;
                }
                else if (loadOp.Status == AsyncOperationStatus.Succeeded)
                {
                    if (loadOp.Result == null || loadOp.Result.Equals(null))  // Added by Ali Hamza to resolve avatar naked issue 
                    {
                        _counter++;
                        if (_counter < 5)
                        {
                            Addressables.ClearDependencyCacheAsync(key);
                            MemoryManager.RemoveAddressable(key);
                        }
                        else
                        {
                            applyOn.WearDefaultItem(type, applyOn.gameObject);
                            yield break;
                        }
                    }
                    else
                    {
                        //loadOp.Result. = key;
                        MemoryManager.AddToReferenceList(loadOp, key.ToLower());
                        if (PlayerPrefs.GetInt("presetPanel") != 1)
                        {
                            if (callFromMultiplayer)
                            {
                                applyOn.StichItem(itemId, loadOp.Result as GameObject, type, applyOn.gameObject, mulitplayerHairColor);
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

        if (StoreManager.instance.loaderForItems)
            StoreManager.instance.loaderForItems.SetActive(false);
        yield return null;
    }
    ///// <summary>
    ///// To Download Addressable Texture. with call back from coroutine
    ///// </summary>
    ///// <param name="name">tag or key of a addressable Texture</param>

    public IEnumerator DownloadAddressableTexture(string key, GameObject applyOn, CurrentTextureType nFTOjectType = 0)
    {
        int _counter = 0;
        while (!XanaConstants.isAddressableCatalogDownload)
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
                applyOn.GetComponent<CharcterBodyParts>().ApplyEyeLenTexture(applyOn.GetComponent<CharcterBodyParts>().Eye_Color_Texture, applyOn);
                GameManager.Instance.isStoreAssetDownloading = false;
                yield return null;
            }
            if (StoreManager.instance.loaderForItems && StoreManager.instance != null && PlayerPrefs.GetInt("presetPanel") != 1)
                StoreManager.instance.loaderForItems.SetActive(true);
            while (true)
            {
                AsyncOperationHandle loadOp;

                bool flag = false;

                loadOp = MemoryManager.GetReferenceIfExist(key, ref flag);
                if (!flag)
                    loadOp = Addressables.LoadAssetAsync<Texture>(key);

                while (!loadOp.IsDone)
                    yield return loadOp;

                if (loadOp.Status == AsyncOperationStatus.Failed)
                {
                    if (PlayerPrefs.GetInt("presetPanel") != 1)
                    {
                        if (StoreManager.instance.loaderForItems && StoreManager.instance != null)
                            StoreManager.instance.loaderForItems.SetActive(false);

                        GameManager.Instance.isStoreAssetDownloading = false;
                        DisableLoadingPanel();
                    }
                    applyOn.GetComponent<CharcterBodyParts>().SetTextureDefault(type, applyOn);
                    yield break;
                }
                else if (loadOp.Status == AsyncOperationStatus.Succeeded)
                {
                    if (loadOp.Result == null || loadOp.Result.Equals(null))   // Added by Ali Hamza to resolve avatar naked issue
                    {
                        _counter++;
                        if (_counter < 5)
                        {
                            Addressables.ClearDependencyCacheAsync(key);
                            MemoryManager.RemoveAddressable(key);
                        }
                        else
                        {
                            applyOn.GetComponent<CharcterBodyParts>().SetTextureDefault(type, applyOn);
                            yield break;
                        }
                    }
                    else
                    {
                        MemoryManager.AddToReferenceList(loadOp, key);
                        switch (type)
                        {
                            case CurrentTextureType.Null:
                                break;
                            case CurrentTextureType.FaceTattoo:
                                applyOn.GetComponent<CharcterBodyParts>().ApplyTattoo(loadOp.Result as Texture, applyOn, CurrentTextureType.FaceTattoo);
                                break;
                            case CurrentTextureType.ChestTattoo:
                                applyOn.GetComponent<CharcterBodyParts>().ApplyTattoo(loadOp.Result as Texture, applyOn, CurrentTextureType.ChestTattoo);
                                break;
                            case CurrentTextureType.LegsTattoo:
                                applyOn.GetComponent<CharcterBodyParts>().ApplyTattoo(loadOp.Result as Texture, applyOn, CurrentTextureType.LegsTattoo);
                                break;
                            case CurrentTextureType.ArmTattoo:
                                applyOn.GetComponent<CharcterBodyParts>().ApplyTattoo(loadOp.Result as Texture, applyOn, CurrentTextureType.ArmTattoo);
                                break;
                            case CurrentTextureType.Mustache:
                                applyOn.GetComponent<CharcterBodyParts>().ApplyMustacheTexture(loadOp.Result as Texture, applyOn);
                                break;
                            case CurrentTextureType.EyeLid:
                                applyOn.GetComponent<CharcterBodyParts>().ApplyEyeLidTexture(loadOp.Result as Texture, applyOn);
                                break;
                            case CurrentTextureType.EyeLense:
                                applyOn.GetComponent<CharcterBodyParts>().ApplyEyeLenTexture(loadOp.Result as Texture, applyOn);
                                break;
                            case CurrentTextureType.EyeLashes:
                                applyOn.GetComponent<CharcterBodyParts>().ApplyEyeLashes(loadOp.Result as Texture, applyOn);
                                break;
                            case CurrentTextureType.EyeBrows:
                                applyOn.GetComponent<CharcterBodyParts>().ApplyEyeBrow(loadOp.Result as Texture, applyOn);
                                break;
                            case CurrentTextureType.Skin:
                                break;
                            case CurrentTextureType.Lip:
                                break;
                            case CurrentTextureType.Makeup:
                                applyOn.GetComponent<CharcterBodyParts>().ApplyMakeup(loadOp.Result as Texture, applyOn);
                                break;
                            default:
                                break;
                        }
                        if (StoreManager.instance.loaderForItems && StoreManager.instance != null && PlayerPrefs.GetInt("presetPanel") != 1)
                            StoreManager.instance.loaderForItems.SetActive(false);
                        GameManager.Instance.isStoreAssetDownloading = false;
                        yield break;
                    }

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
            ItemDatabase.instance.GetComponent<SavaCharacterProperties>().SavePlayerProperties();
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
    Lip,
    Makeup
}