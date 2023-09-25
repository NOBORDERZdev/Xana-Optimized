using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.Exceptions;

public class AddressableDownloader : MonoBehaviour
{
    public List<Item> presetsItem;
    public int presetItemCount;
    public static AddressableDownloader Instance;
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
            DownloadCatalogFile();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    bool isDownloading=false;
    public void DownloadCatalogFile()
    {
        if(!isDownloading)
        {
            isDownloading = true;
#if UNITY_EDITOR
            string catalogFilePath = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetValueByName(UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.activeProfileId, "Remote.LoadPath");
            catalogFilePath = catalogFilePath.Replace("[BuildTarget]", UnityEditor.EditorUserBuildSettings.activeBuildTarget.ToString());
            catalogFilePath = catalogFilePath + "/XanaAddressableCatalog.json";
            AsyncOperationHandle DownloadingCatalog = Addressables.LoadContentCatalogAsync(catalogFilePath,true);
            DownloadingCatalog.Completed += OnCatalogDownload;
#else
BuildScriptableObject buildScriptableObject = Resources.Load("BuildVersion/BuildVersion") as BuildScriptableObject;
        AsyncOperationHandle DownloadingCatalog = Addressables.LoadContentCatalogAsync(buildScriptableObject.addressableCatalogFilePath,true);
            Debug.LogError(buildScriptableObject.addressableCatalogFilePath);
        DownloadingCatalog.Completed += OnCatalogDownload;
#endif

        }
    }


    void OnCatalogDownload(AsyncOperationHandle handle)
    {
        Debug.LogError("Status == "+handle.Status);
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            //XanaConstants.isAddressableCatalogDownload = true;
            XanaConstants.isAddressableCatalogDownload = true;
            //StartCoroutine(CheckCatalogs());
        }
        else
        {
            XanaConstants.isAddressableCatalogDownload = true;
            isDownloading = false;
        }
    }
    IEnumerator CheckCatalogs()
    {
        List<string> catalogsToUpdate = new List<string>();
        AsyncOperationHandle<List<string>> checkForUpdateHandle;
        checkForUpdateHandle = Addressables.CheckForCatalogUpdates();
        checkForUpdateHandle.Completed += op => { catalogsToUpdate.AddRange(op.Result); };
        yield return checkForUpdateHandle;

        if (catalogsToUpdate.Count > 0)
        {
            AsyncOperationHandle<List<IResourceLocator>> updateHandle = Addressables.UpdateCatalogs(catalogsToUpdate);
            yield return updateHandle;
            Addressables.Release(updateHandle);
        }
        Debug.LogError("catalogsToUpdate.Count == " + catalogsToUpdate.Count);
        Addressables.Release(checkForUpdateHandle);
        XanaConstants.isAddressableCatalogDownload = true;
    }
    //public int appliedItemsInPresets = 0;
    //public bool isTextureDownloaded = false;
    /// <summary>
    /// To Download Addressable object. with call back from coroutine
    /// </summary>
    /// <param name="name">tag or key of a addressable object</param>
    public IEnumerator DownloadAddressableObj(int itemId, string key, string type, AvatarController applyOn, Color mulitplayerHairColor, bool applyHairColor = true, bool callFromMultiplayer = false)
    {
        while (!XanaConstants.isAddressableCatalogDownload)
        {
            yield return new WaitForSeconds(0.5f);
        }

        // print("Addressable download object call for " + itemId + " key" +key+ " type" + type+ " applyOn" +  applyOn);
        Resources.UnloadUnusedAssets();
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            //if (!key.Contains("FullCostume") && !key.Contains("Fighter") && !key.Contains("eyebrow") && key.Contains("_")) // To get name after '_' only
            //{
            //    string tempName = key.Split('_').Last();
            //    key = tempName;
            //}
            if (key.Contains("gambeson")) // To remove gambeson from shirt names
            {
                string tempName = key.Replace("gambeson", "shirt");
                key = tempName;
            }
            if (StoreManager.instance.loaderForItems && StoreManager.instance != null)
            {
                StoreManager.instance.loaderForItems.SetActive(true);
            }
            AsyncOperationHandle<GameObject> loadOp;
            loadOp = Addressables.LoadAssetAsync<GameObject>(key.ToLower());

           SwitchToShoesHirokoKoshinoNFT.Instance?.SwitchLightFor_HirokoKoshino(key.ToLower());

            //if (!key.Contains("Fighter") && !key.Contains("FullCostume"))
            //{
            //    loadOp = Addressables.LoadAssetAsync<GameObject>(key.ToLower());
            //}
            //else
            //{
            //    loadOp = Addressables.LoadAssetAsync<GameObject>(key);
            //}
           // print("~~~~ "+ key.ToLower());
            yield return loadOp;
            //while (!loadOp.IsDone)
            //    yield return loadOp;
            Debug.LogError("~~~~~~LOG~~~~~~~~~~");
            Debug.LogError("~~~~~~LOG" + loadOp.Result);
            if (loadOp.Result== null)
            {
                AsyncOperationHandle < bool > checker = Addressables.ClearDependencyCacheAsync(key,false);
                yield return checker;
                Debug.LogError("~~~~~~LOG" + checker.Result);

                StartCoroutine (DownloadAddressableObj(itemId,  key, type,  applyOn,  mulitplayerHairColor,  applyHairColor,  callFromMultiplayer));
                // print("RECALLING DOWNLOAD " + itemId + key + type + applyOn+mulitplayerHairColor );
                // StartCoroutine(DownloadAddressableObj(itemId,key,type,applyOn,mulitplayerHairColor));
                //applyOn.WearDefaultItem(type,applyOn.gameObject);
                yield break;
            }

            if (loadOp.Status == AsyncOperationStatus.Failed)
            {
               // print("LOAD OP FAILED ");
                if (StoreManager.instance.loaderForItems && StoreManager.instance != null)
                    StoreManager.instance.loaderForItems.SetActive(false);
                if(GameManager.Instance != null)
                    GameManager.Instance.isStoreAssetDownloading = false;
                DisableLoadingPanel();
                yield break;
            }
            else if (loadOp.Status == AsyncOperationStatus.Succeeded)
            {
               //  print("loadOp.Result :" + loadOp.Result +" ~  " +loadOp.Result.name);
                loadOp.Result.name = key;
                if (PlayerPrefs.GetInt("presetPanel") != 1)
                {
                 //   print("~~~~~ not preset" + PlayerPrefs.GetInt("presetPanel"));
                    if (callFromMultiplayer)
                    {
                    //    print("CALL FROM MULTIPLAYER for : "+ applyOn.name);
                        applyOn.StichItem(itemId, loadOp.Result, type, applyOn.gameObject,mulitplayerHairColor);
                    }
                    else
                    {
                        applyOn.StichItem(itemId, loadOp.Result, type, applyOn.gameObject, applyHairColor);
                    }
                    if(GameManager.Instance != null)
                        GameManager.Instance.isStoreAssetDownloading = false;
                    DisableLoadingPanel();
                }
                else
                {
                    presetsItem.Add(new Item(itemId, loadOp.Result, type));
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
                //if(PlayerPrefs.GetInt("presetPanel") == 1)
                //appliedItemsInPresets++;
                //yield return loadOp.Result;
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
            applyOn.StichItem(presetsItem[i].ItemID, presetsItem[i].ItemPrefab, presetsItem[i].ItemType, applyOn.gameObject,false);
        }
        presetsItem.Clear();
        AddressableDownloader.Instance.presetItemCount = 0;
        //print(" ~~~ preset panel before "+ PlayerPrefs.GetInt("FristPresetSet"));
        // Invoke(nameof(SavePresetFristTime), 0.2f);
        GameManager.Instance.isStoreAssetDownloading = false;
        DisableLoadingPanel();

        if (StoreManager.instance.loaderForItems)
            StoreManager.instance.loaderForItems.SetActive(false);
        yield return null;
    }
    //public Texture DownloadAddressableTexture(string name)
    //{
    //    Texture temp = null;
    //    if (name != "" || name != null)
    //    {
    //        name = name.ToLower();
    //        yield return StartCoroutine(TextureCoroutine(name, (Texture callback) => {
    //            temp = callback;
    //        }));
    //      //  return temp;
    //    }
    //    else
    //    {
    //        return null;
    //    }
    //}

    ///// <summary>
    ///// To Download Addressable Texture. with call back from coroutine
    ///// </summary>
    ///// <param name="name">tag or key of a addressable Texture</param>

    public IEnumerator DownloadAddressableTexture(string key, GameObject applyOn, CurrentTextureType nFTOjectType = 0)
    {
        while (!XanaConstants.isAddressableCatalogDownload)
        {
            yield return new WaitForSeconds(0.5f);
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
            else if ( key.Contains("FaceTattoo", StringComparison.CurrentCultureIgnoreCase))
            {
                 type = CurrentTextureType.FaceTattoo;
            }
            else if ( key.Contains("ChestTattoo", StringComparison.CurrentCultureIgnoreCase))
            {
                  type = CurrentTextureType.ChestTattoo;
            }
            else if ( key.Contains("LegsTattoo", StringComparison.CurrentCultureIgnoreCase))
            {
                  type = CurrentTextureType.LegsTattoo;
            }
            else if (  key.Contains("ArmTattoo", StringComparison.CurrentCultureIgnoreCase))
            {
                 type = CurrentTextureType.ArmTattoo;
            }
            else if ( key.Contains("Mustache", StringComparison.CurrentCultureIgnoreCase))
            {
                 type = CurrentTextureType.Mustache;
            }
            else if (  key.Contains("EyeLid", StringComparison.CurrentCultureIgnoreCase))
            {
                 type = CurrentTextureType.EyeLid;
            }
            
        }
        else
        {
           type = nFTOjectType;
        }
        //isTextureDownloaded = false;
        if (key != "" && Application.internetReachability != NetworkReachability.NotReachable)
        {

            key = key.ToLower();
            if (key == "eye_color_texture")
            {
                // This Texture Store in Reference no need to download this texture
                applyOn.GetComponent<CharcterBodyParts>().ApplyEyeLenTexture(applyOn.GetComponent<CharcterBodyParts>().Eye_Color_Texture, applyOn);
                GameManager.Instance.isStoreAssetDownloading = false;
               // print("~~~~~~~ in eye color  texture if ");
                yield return null;
            }
            //else
            //{
           // print("!!!!!!!!!!!! texture downloading "+ key);
            if (StoreManager.instance.loaderForItems && StoreManager.instance != null && PlayerPrefs.GetInt("presetPanel") != 1)
                StoreManager.instance.loaderForItems.SetActive(true);
            AsyncOperationHandle<Texture> loadOp = Addressables.LoadAssetAsync<Texture>(key);
            yield return loadOp;
            if (loadOp.Result == null)
            {
                Debug.LogError("~~~~~~LOG~~~~~~~~~~");
                Debug.LogError("~~~~~~LOG" + loadOp.Result);
                Addressables.ClearDependencyCacheAsync(key);
               // Addressables.CleanBundleCache(key);
              //  Addressables.cl
                yield return new WaitForSeconds(2f);
                StartCoroutine(DownloadAddressableTexture(key, applyOn, nFTOjectType));
               // applyOn.GetComponent<CharcterBodyParts>().SetTextureDefault(type, applyOn);
                yield break;
            }
            if (loadOp.Status == AsyncOperationStatus.Failed)
            {
             //   print("~~~~~~~ TEXTURE FAIL ");
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
               // print("~~~~~~~ TEXTURE Succeded " + loadOp.Result);
                // Applying texture when successful downloaded.
                switch (type)
                {
                    case CurrentTextureType.Null:
                        break;
                    case CurrentTextureType.FaceTattoo:
                        applyOn.GetComponent<CharcterBodyParts>().ApplyTattoo(loadOp.Result, applyOn, CurrentTextureType.FaceTattoo);
                        break;
                    case CurrentTextureType.ChestTattoo:
                          applyOn.GetComponent<CharcterBodyParts>().ApplyTattoo(loadOp.Result, applyOn, CurrentTextureType.ChestTattoo);
                        break;
                    case CurrentTextureType.LegsTattoo:
                          applyOn.GetComponent<CharcterBodyParts>().ApplyTattoo(loadOp.Result, applyOn, CurrentTextureType.LegsTattoo);
                        break;
                    case CurrentTextureType.ArmTattoo:
                         applyOn.GetComponent<CharcterBodyParts>().ApplyTattoo(loadOp.Result, applyOn, CurrentTextureType.ArmTattoo);
                        break;
                    case CurrentTextureType.Mustache:
                        applyOn.GetComponent<CharcterBodyParts>().ApplyMustacheTexture(loadOp.Result, applyOn);
                        break;
                    case CurrentTextureType.EyeLid:
                        applyOn.GetComponent<CharcterBodyParts>().ApplyEyeLidTexture(loadOp.Result, applyOn);
                        break;
                    case CurrentTextureType.EyeLense:
                        applyOn.GetComponent<CharcterBodyParts>().ApplyEyeLenTexture(loadOp.Result, applyOn);
                        break;
                    case CurrentTextureType.EyeLashes:
                           applyOn.GetComponent<CharcterBodyParts>().ApplyEyeLashes(loadOp.Result, applyOn);
                        break;
                    case CurrentTextureType.EyeBrows:
                         applyOn.GetComponent<CharcterBodyParts>().ApplyEyeBrow(loadOp.Result, applyOn);
                        break;
                    case CurrentTextureType.Skin:
                        break;
                    case CurrentTextureType.Lip:
                        break;
                    case CurrentTextureType.Makeup:
                         applyOn.GetComponent<CharcterBodyParts>().ApplyMakeup(loadOp.Result, applyOn);
                        break;
                    default:
                        break;
                }
                if (StoreManager.instance.loaderForItems && StoreManager.instance != null && PlayerPrefs.GetInt("presetPanel") != 1)
                    StoreManager.instance.loaderForItems.SetActive(false);

                GameManager.Instance.isStoreAssetDownloading = false;
                //isTextureDownloaded = true;
            }
        }
    }

    void SavePresetFristTime()
    {
        if (PlayerPrefs.GetInt("presetPanel") == 1 && PlayerPrefs.GetInt("FristPresetSet") == 0)
        {   // preset panel is enable so saving preset to account 
            PlayerPrefs.SetInt("presetPanel", 0);
            PlayerPrefs.SetInt("FristPresetSet", 1);
            //print(" ~~~ preset panel after  " + PlayerPrefs.GetInt("FristPresetSet"));
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
