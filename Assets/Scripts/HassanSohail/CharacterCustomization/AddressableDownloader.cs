using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    //public int appliedItemsInPresets = 0;
    //public bool isTextureDownloaded = false;
    /// <summary>
    /// To Download Addressable object. with call back from coroutine
    /// </summary>
    /// <param name="name">tag or key of a addressable object</param>
    public IEnumerator DownloadAddressableObj(int itemId, string key, string type, AvatarController applyOn, Color mulitplayerHairColor, bool applyHairColor = true, bool callFromMultiplayer = false)
    {
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

            //if (!key.Contains("Fighter") && !key.Contains("FullCostume"))
            //{
            //    loadOp = Addressables.LoadAssetAsync<GameObject>(key.ToLower());
            //}
            //else
            //{
            //    loadOp = Addressables.LoadAssetAsync<GameObject>(key);
            //}

            while (!loadOp.IsDone)
                yield return loadOp;
            if (loadOp.Status == AsyncOperationStatus.Failed)
            {
                if (StoreManager.instance.loaderForItems && StoreManager.instance != null)
                    StoreManager.instance.loaderForItems.SetActive(false);

                GameManager.Instance.isStoreAssetDownloading = false;
                DisableLoadingPanel();
                yield break;
            }
            else if (loadOp.Status == AsyncOperationStatus.Succeeded)
            {
                loadOp.Result.name = key;
                if (PlayerPrefs.GetInt("presetPanel") != 1)
                {
                    if (callFromMultiplayer)
                    {
                        applyOn.StichItem(itemId, loadOp.Result, type, applyOn.gameObject,mulitplayerHairColor);
                    }
                    else
                    {
                        applyOn.StichItem(itemId, loadOp.Result, type, applyOn.gameObject, applyHairColor);
                    }
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
        //isTextureDownloaded = false;
        if (key != "" && Application.internetReachability != NetworkReachability.NotReachable)
        {
            if (nFTOjectType == CurrentTextureType.EyeLense || key.Contains("eyelense", StringComparison.CurrentCultureIgnoreCase))
            {
                key = key.ToLower();
                if (key == "eye_color_texture")
                {
                    // This Texture Store in Reference no need to download this texture
                    applyOn.GetComponent<CharcterBodyParts>().ApplyEyeLenTexture(applyOn.GetComponent<CharcterBodyParts>().Eye_Color_Texture, applyOn);
                    GameManager.Instance.isStoreAssetDownloading = false;
                    yield return null;
                }
                //else
                //{
                if (StoreManager.instance.loaderForItems && StoreManager.instance != null && PlayerPrefs.GetInt("presetPanel") != 1)
                    StoreManager.instance.loaderForItems.SetActive(true);
                AsyncOperationHandle<Texture> loadOp = Addressables.LoadAssetAsync<Texture>(key);

                while (!loadOp.IsDone)
                {
                    yield return loadOp;
                }
                if (loadOp.Status == AsyncOperationStatus.Failed)
                {
                    if (PlayerPrefs.GetInt("presetPanel") != 1)
                    {
                        if (StoreManager.instance.loaderForItems && StoreManager.instance != null)
                            StoreManager.instance.loaderForItems.SetActive(false);

                        GameManager.Instance.isStoreAssetDownloading = false;
                        DisableLoadingPanel();
                    }
                    yield break;
                }
                else if (loadOp.Status == AsyncOperationStatus.Succeeded)
                {
                    //if (key.Contains("eyebrow"))
                    //{
                    //    print("EyeBrows");
                    //    applyOn.GetComponent<CharcterBodyParts>().ApplyEyeBrowTexture(loadOp.Result, applyOn);
                    //}
                    //else if (key.Contains("eyelash"))
                    //{
                    //    print("Eyelashes");
                    //    applyOn.GetComponent<CharcterBodyParts>().ApplyEyeLashes(loadOp.Result, applyOn);
                    //}
                    //else
                    applyOn.GetComponent<CharcterBodyParts>().ApplyEyeLenTexture(loadOp.Result, applyOn);
                    if (StoreManager.instance.loaderForItems && StoreManager.instance != null && PlayerPrefs.GetInt("presetPanel") != 1)
                        StoreManager.instance.loaderForItems.SetActive(false);

                    GameManager.Instance.isStoreAssetDownloading = false;
                    //isTextureDownloaded = true;
                }
                //}
            }
            else if (nFTOjectType == CurrentTextureType.EyeBrows || key.Contains("brow", StringComparison.CurrentCultureIgnoreCase))
            {
                key = key.ToLower();
                if (StoreManager.instance.loaderForItems && StoreManager.instance != null)
                    StoreManager.instance.loaderForItems.SetActive(true);
                AsyncOperationHandle<Texture> loadOp = Addressables.LoadAssetAsync<Texture>(key);

                while (!loadOp.IsDone)
                {
                    yield return loadOp;
                }
                if (loadOp.Status == AsyncOperationStatus.Failed)
                {
                    if (StoreManager.instance.loaderForItems && StoreManager.instance != null)
                        StoreManager.instance.loaderForItems.SetActive(false);
                    GameManager.Instance.isStoreAssetDownloading = false;
                    yield break;
                }
                else if (loadOp.Status == AsyncOperationStatus.Succeeded)
                {
                    applyOn.GetComponent<CharcterBodyParts>().ApplyEyeBrow(loadOp.Result, applyOn);
                    if (StoreManager.instance.loaderForItems && StoreManager.instance != null)
                        StoreManager.instance.loaderForItems.SetActive(false);

                    GameManager.Instance.isStoreAssetDownloading = false;
                }
            }
            else if (nFTOjectType == CurrentTextureType.EyeLashes || key.Contains("lashes", StringComparison.CurrentCultureIgnoreCase))
            {
                key = key.ToLower();
                if (StoreManager.instance.loaderForItems && StoreManager.instance != null)
                    StoreManager.instance.loaderForItems.SetActive(true);
                AsyncOperationHandle<Texture> loadOp = Addressables.LoadAssetAsync<Texture>(key);

                while (!loadOp.IsDone)
                {
                    yield return loadOp;
                }
                if (loadOp.Status == AsyncOperationStatus.Failed)
                {
                    if (StoreManager.instance.loaderForItems && StoreManager.instance != null)
                        StoreManager.instance.loaderForItems.SetActive(false);

                    GameManager.Instance.isStoreAssetDownloading = false;
                    DisableLoadingPanel();
                    yield break;
                }
                else if (loadOp.Status == AsyncOperationStatus.Succeeded)
                {
                    applyOn.GetComponent<CharcterBodyParts>().ApplyEyeLashes(loadOp.Result, applyOn);
                    if (StoreManager.instance.loaderForItems && StoreManager.instance != null)
                        StoreManager.instance.loaderForItems.SetActive(false);

                    GameManager.Instance.isStoreAssetDownloading = false;
                }
            }
            else if (nFTOjectType == CurrentTextureType.Makeup || key.Contains("makeup", StringComparison.CurrentCultureIgnoreCase))
            {
                print("makeup download call");
                key = key.ToLower();
                if (StoreManager.instance.loaderForItems && StoreManager.instance != null)
                    StoreManager.instance.loaderForItems.SetActive(true);
                AsyncOperationHandle<Texture> loadOp = Addressables.LoadAssetAsync<Texture>(key);

                while (!loadOp.IsDone)
                {
                    yield return loadOp;
                }
                if (loadOp.Status == AsyncOperationStatus.Failed)
                {
                    if (StoreManager.instance.loaderForItems && StoreManager.instance != null)
                        StoreManager.instance.loaderForItems.SetActive(false);

                    GameManager.Instance.isStoreAssetDownloading = false;
                    DisableLoadingPanel();
                    yield break;
                }
                else if (loadOp.Status == AsyncOperationStatus.Succeeded)
                {
                    applyOn.GetComponent<CharcterBodyParts>().ApplyMakeup(loadOp.Result, applyOn);
                    if (StoreManager.instance.loaderForItems && StoreManager.instance != null)
                        StoreManager.instance.loaderForItems.SetActive(false);

                    GameManager.Instance.isStoreAssetDownloading = false;
                }
            }
            else if (nFTOjectType == CurrentTextureType.FaceTattoo || key.Contains("FaceTattoo", StringComparison.CurrentCultureIgnoreCase))
            {
                key = key.ToLower();
                AsyncOperationHandle<Texture> loadOp = Addressables.LoadAssetAsync<Texture>(key);
                while (!loadOp.IsDone)
                {
                    yield return null;
                }
                if (loadOp.Status == AsyncOperationStatus.Failed)
                {
                    applyOn.GetComponent<CharcterBodyParts>().RemoveTattoo(loadOp.Result, applyOn, CurrentTextureType.FaceTattoo);
                    yield break;
                }
                else if (loadOp.Status == AsyncOperationStatus.Succeeded)
                {
                    applyOn.GetComponent<CharcterBodyParts>().ApplyTattoo(loadOp.Result, applyOn, CurrentTextureType.FaceTattoo);
                }
            }
            else if (nFTOjectType == CurrentTextureType.ChestTattoo || key.Contains("ChestTattoo", StringComparison.CurrentCultureIgnoreCase))
            {
                key = key.ToLower();
                AsyncOperationHandle<Texture> loadOp = Addressables.LoadAssetAsync<Texture>(key);
                while (!loadOp.IsDone)
                {
                    yield return null;
                }
                if (loadOp.Status == AsyncOperationStatus.Failed)
                {
                    applyOn.GetComponent<CharcterBodyParts>().RemoveTattoo(loadOp.Result, applyOn, CurrentTextureType.ChestTattoo);
                    yield break;
                }
                else if (loadOp.Status == AsyncOperationStatus.Succeeded)
                {
                    applyOn.GetComponent<CharcterBodyParts>().ApplyTattoo(loadOp.Result, applyOn, CurrentTextureType.ChestTattoo);
                }
            }
            else if (nFTOjectType == CurrentTextureType.LegsTattoo || key.Contains("LegsTattoo", StringComparison.CurrentCultureIgnoreCase))
            {
                key = key.ToLower();
                AsyncOperationHandle<Texture> loadOp = Addressables.LoadAssetAsync<Texture>(key);
                while (!loadOp.IsDone)
                {
                    yield return null;
                }
                while (!loadOp.IsDone)
                {
                    yield return null;
                }
                if (loadOp.Status == AsyncOperationStatus.Failed)
                {
                    applyOn.GetComponent<CharcterBodyParts>().RemoveTattoo(loadOp.Result, applyOn, CurrentTextureType.LegsTattoo);
                    yield break;
                }
                else if (loadOp.Status == AsyncOperationStatus.Succeeded)
                {
                    applyOn.GetComponent<CharcterBodyParts>().ApplyTattoo(loadOp.Result, applyOn, CurrentTextureType.LegsTattoo);
                }
            }
            else if (nFTOjectType == CurrentTextureType.ArmTattoo || key.Contains("ArmTattoo", StringComparison.CurrentCultureIgnoreCase))
            {
                key = key.ToLower();
                AsyncOperationHandle<Texture> loadOp = Addressables.LoadAssetAsync<Texture>(key);
                while (!loadOp.IsDone)
                {
                    yield return null;
                }
                if (loadOp.Status == AsyncOperationStatus.Failed)
                {
                    applyOn.GetComponent<CharcterBodyParts>().RemoveTattoo(loadOp.Result, applyOn, CurrentTextureType.ArmTattoo);
                    yield break;
                }
                else if (loadOp.Status == AsyncOperationStatus.Succeeded)
                {
                    applyOn.GetComponent<CharcterBodyParts>().ApplyTattoo(loadOp.Result, applyOn, CurrentTextureType.ArmTattoo);
                }
            }
            else if (nFTOjectType == CurrentTextureType.Mustache || key.Contains("Mustache", StringComparison.CurrentCultureIgnoreCase))
            {
                key = key.ToLower();
                AsyncOperationHandle<Texture> loadOp = Addressables.LoadAssetAsync<Texture>(key);
                while (!loadOp.IsDone)
                {
                    yield return null;
                }
                if (loadOp.Status == AsyncOperationStatus.Failed)
                {
                    applyOn.GetComponent<CharcterBodyParts>().RemoveMustacheTexture(loadOp.Result, applyOn);
                    yield break;
                }
                else if (loadOp.Status == AsyncOperationStatus.Succeeded)
                {
                    applyOn.GetComponent<CharcterBodyParts>().ApplyMustacheTexture(loadOp.Result, applyOn);
                }
            }
            else if (nFTOjectType == CurrentTextureType.EyeLid || key.Contains("EyeLid", StringComparison.CurrentCultureIgnoreCase))
            {
                key = key.ToLower();
                AsyncOperationHandle<Texture> loadOp = Addressables.LoadAssetAsync<Texture>(key);
                while (!loadOp.IsDone)
                {
                    yield return null;
                }
                if (loadOp.Status == AsyncOperationStatus.Failed)
                {
                    applyOn.GetComponent<CharcterBodyParts>().RemoveEyeLidTexture(loadOp.Result, applyOn);
                    yield break;
                }
                else if (loadOp.Status == AsyncOperationStatus.Succeeded)
                {
                    applyOn.GetComponent<CharcterBodyParts>().ApplyEyeLidTexture(loadOp.Result, applyOn);
                }
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
