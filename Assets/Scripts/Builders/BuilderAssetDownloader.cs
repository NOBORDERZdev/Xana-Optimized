﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class BuilderAssetDownloader : MonoBehaviour
{
    public const string prefabPrefix = "pf";
    public const string prefabSuffix = "_XANA";

    public static bool proximityLoading = true;
    public static bool isPostLoading = true;

    //booleans 
    public static bool stopDownloading;
    public static bool downloadIsGoingOn;
    public static bool dataArranged;
    public static bool dataSorted;
    public static bool isSpawnDownloaded;
    bool isWorldInstantiated = true;

    public Transform assetParent;
    public TMPro.TextMeshProUGUI assetDownloadingText;
    public TMPro.TextMeshProUGUI assetDownloadingTextPotrait;
    //public TMPro.TextMeshProUGUI sortingText;
    public MeshCombiner meshCombiner;
    public static MeshCombiner meshCombinerRef;
    public static Vector3 initialPlayerPos;
    public static Vector3 currPlayerPosition;
    public static Transform assetParentStatic;
    private static int totalAssetCount;
    public static int downloadedTillNow = 0;

    [Header("Short Interval sorting element count")]
    public static int shortSortingCount = 100;
    public static int timeshortSorting = 10;
    [Header("Sorting full list again")]
    public static int timeFullSorting = 100;

    public static List<string> allAssetsList = new List<string>();
    public static List<DownloadQueueData> downloadDataQueue = new List<DownloadQueueData>();
    public static List<DownloadQueueData> downloadFailed = new List<DownloadQueueData>();
    public static Dictionary<string, ItemData> builderDataDictionary = new Dictionary<string, ItemData>();

    private void OnEnable()
    {
        assetParentStatic = assetParent;
        meshCombinerRef = meshCombiner;
        if (proximityLoading && isPostLoading)
        {
            BuilderEventManager.AfterPlayerInstantiated += StartDownloadingAssets;
            BuilderEventManager.AfterMapDataDownloaded += PostLoadingBuilderAssets;
            ScreenOrientationManager.switchOrientation += OnOrientationChange;
        }
    }

    private void OnDisable()
    {
        if (proximityLoading && isPostLoading)
        {
            BuilderEventManager.AfterPlayerInstantiated -= StartDownloadingAssets;
            BuilderEventManager.AfterMapDataDownloaded -= PostLoadingBuilderAssets;
            ScreenOrientationManager.switchOrientation -= OnOrientationChange;
        }
        ResetAll();
    }

    async void PostLoadingBuilderAssets()
    {
        ArrangeData();
        while (!dataArranged)
        {
            await Task.Yield();
        }
        StartCoroutine(DownloadSpawnPointsPreload());
        while (!isSpawnDownloaded)
        {
            await Task.Yield();
        }
        LoadAddressableSceneAfterDownload();
    }

    void LoadAddressableSceneAfterDownload()
    {
        if (SceneManager.sceneCount > 1 || ConstantsHolder.isFromXANASummit)
            return;
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
    }

    public static void ArrangeData()
    {
        for (int i = 0; i < BuilderData.mapData.data.json.otherItems.Count; i++)
        {
            DownloadQueueData temp = new DownloadQueueData();
            temp.ItemID = BuilderData.mapData.data.json.otherItems[i].ItemID;
            temp.DcitionaryKey = i.ToString();
            temp.Position = BuilderData.mapData.data.json.otherItems[i].Position;
            temp.Rotation = BuilderData.mapData.data.json.otherItems[i].Rotation;
            temp.Scale = BuilderData.mapData.data.json.otherItems[i].Scale;

            builderDataDictionary.Add(i.ToString(), BuilderData.mapData.data.json.otherItems[i]);
            if (BuilderData.mapData.data.json.otherItems[i].ItemID.Contains("SPW") || BuilderData.mapData.data.json.otherItems[i].spawnComponent)
            {
                //Debug.LogError(BuilderData.mapData.data.json.otherItems[i].Position);
                temp.IsActive = BuilderData.mapData.data.json.otherItems[i].spawnerComponentData.IsActive;
                BuilderData.preLoadspawnPoint.Add(temp);
            }
            else
            {
                downloadDataQueue.Add(temp);
                totalAssetCount++;
            }
        }
        dataArranged = true;
    }

    //Sorting data on start and after long Interval
    public static void SortingQueueData(Vector3 playerPos)
    {
        List<Tuple<DownloadQueueData, float>> distances = new List<Tuple<DownloadQueueData, float>>();
        foreach (DownloadQueueData position in downloadDataQueue)
        {
            float distance = Vector3.Distance(playerPos, position.Position);
            distances.Add(new Tuple<DownloadQueueData, float>(position, distance));
        }
        // Sort the list based on distances in ascending order
        distances.Sort((a, b) => a.Item2.CompareTo(b.Item2));

        downloadDataQueue.Clear();
        // Now, distances contains the vector positions sorted by proximity to the player position
        foreach (var item in distances)
        {
            downloadDataQueue.Add(item.Item1);
            // Debug.Log("Position: " + item.Item1 + ", Distance: " + item.Item2);
        }
        dataSorted = true;
    }


    static void SortingDataShortInterval(Vector3 playerPos)
    {
        List<Tuple<DownloadQueueData, float>> distances = new List<Tuple<DownloadQueueData, float>>();

        if (shortSortingCount > downloadDataQueue.Count)
            shortSortingCount = downloadDataQueue.Count;

        for (int i = 0; i < shortSortingCount; i++)
        {
            float distance = Vector3.Distance(playerPos, downloadDataQueue[i].Position);
            distances.Add(new Tuple<DownloadQueueData, float>(downloadDataQueue[i], distance));
        }

        // Sort the list based on distances in ascending order
        distances.Sort((a, b) => a.Item2.CompareTo(b.Item2));

        downloadDataQueue.RemoveRange(0, distances.Count);
        int x = 0;
        // Now, distances contains the vector positions sorted by proximity to the player position
        foreach (var item in distances)
        {
            downloadDataQueue.Insert(x, item.Item1);
            x++;
            //Debug.Log("Position: " + item.Item1 + ", Distance: " + item.Item2);
        }
    }

    async void StartDownloadingAssets()
    {
        if (ConstantsHolder.xanaConstants.isBuilderScene)
            BuilderEventManager.ApplySkyoxSettings?.Invoke();
        SortingQueueData(initialPlayerPos);
        while (!dataSorted)
        {
            await Task.Yield();
        }
        StartCoroutine(DownloadAssetsFromSortedList());
        StartCoroutine(CheckLongIntervalSorting());
        StartCoroutine(CheckShortIntervalSorting());

        //Remove spw count from Item count
        int objCount = BuilderData.mapData.data.json.otherItems.Count - BuilderData.preLoadspawnPoint.Count;

        if (objCount == 0)
        {
            assetDownloadingText.enabled = false;
            assetDownloadingTextPotrait.enabled = false;
            assetDownloadingText.transform.parent.gameObject.SetActive(false);
            assetDownloadingTextPotrait.transform.parent.gameObject.SetActive(false);
        }
    }

    IEnumerator DownloadAssetsFromSortedList()
    {
        while (downloadDataQueue.Count > 0 && !stopDownloading)
        {
            downloadIsGoingOn = true;
            string downloadKey = prefabPrefix + downloadDataQueue[0].ItemID + prefabSuffix;
            string dicKey = downloadDataQueue[0].DcitionaryKey;
            //AsyncOperationHandle<GameObject> _async = Addressables.LoadAssetAsync<GameObject>(downloadKey);
            //bool flag = false;
            //AsyncOperationHandle _async = AddressableDownloader.Instance.MemoryManager.GetReferenceIfExist(downloadKey, ref flag);
            //if (!flag)
            AsyncOperationHandle<GameObject> _async = Addressables.LoadAssetAsync<GameObject>(downloadKey);
            while (!_async.IsDone)
            {
                yield return null;
            }
            if (_async.Status == AsyncOperationStatus.Succeeded)
            {
                InstantiateAsset(_async.Result, builderDataDictionary[dicKey]);
                AddressableDownloader.Instance.MemoryManager.AddToReferenceList(_async, downloadKey);
            }
            else
            {
                Debug.LogError("Download Failed......");
            }
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(.01f);
            if (_async.Status == AsyncOperationStatus.Succeeded)
            {
                downloadDataQueue.RemoveAt(0);
                DisplayDownloadedAssetText();
            }
            else
            {
                downloadFailed.Add(downloadDataQueue[0]);
                downloadDataQueue.RemoveAt(0);
            }

            downloadIsGoingOn = false;
        }

        if (downloadDataQueue.Count <= 0)
        {
            StartCoroutine(DownloadFailedItem());
        }
    }


    IEnumerator DownloadFailedItem()
    {
        while (downloadFailed.Count > 0)
        {
            string downloadKey = prefabPrefix + downloadFailed[0].ItemID + prefabSuffix;
            string dicKey = downloadFailed[0].DcitionaryKey;
            //AsyncOperationHandle<GameObject> _async = Addressables.LoadAssetAsync<GameObject>(downloadKey);
            //bool flag = false;
            //AsyncOperationHandle _async = AddressableDownloader.Instance.MemoryManager.GetReferenceIfExist(downloadKey, ref flag);
            //if (!flag)
            AsyncOperationHandle<GameObject> _async = Addressables.LoadAssetAsync<GameObject>(downloadKey);
            while (!_async.IsDone)
            {
                yield return null;
            }
            if (_async.Status == AsyncOperationStatus.Succeeded)
            {
                InstantiateAsset(_async.Result, builderDataDictionary[dicKey]);
                //AddressableDownloader.Instance.MemoryManager.AddToReferenceList(_async, downloadKey);
            }
            else
            {
                Debug.LogError("Download Failed......");
            }
            yield return new WaitForEndOfFrame();
            if (_async.Status == AsyncOperationStatus.Succeeded)
            {
                downloadFailed.RemoveAt(0);
                DisplayDownloadedAssetText();
            }
            else
            {
                downloadFailed.RemoveAt(0);
                DisplayDownloadedAssetText();
            }
        }
    }


    public IEnumerator DownloadSpawnPointsPreload()
    {
        for (int i = 0; i < BuilderData.preLoadspawnPoint.Count; i++)
        {
            string downloadKey = prefabPrefix + BuilderData.preLoadspawnPoint[i].ItemID + prefabSuffix;
            string dicKey = BuilderData.preLoadspawnPoint[i].DcitionaryKey;
            //AsyncOperationHandle<GameObject> _async = Addressables.LoadAssetAsync<GameObject>(downloadKey);
            //bool flag = false;
            //AsyncOperationHandle _async = AddressableDownloader.Instance.MemoryManager.GetReferenceIfExist(downloadKey, ref flag);
            //if (!flag)
            AsyncOperationHandle<GameObject> _async = Addressables.LoadAssetAsync<GameObject>(downloadKey);
            while (!_async.IsDone)
            {
                yield return null;
            }
            if (_async.Status == AsyncOperationStatus.Succeeded)
            {
                InstantiateAsset(_async.Result, builderDataDictionary[dicKey]);
                AddressableDownloader.Instance.MemoryManager.AddToReferenceList(_async, downloadKey);
            }
            else
            {
                Debug.LogError(_async.Status);
            }
            yield return new WaitForEndOfFrame();
        }

        isSpawnDownloaded = true;
    }

    void DisplayDownloadedAssetText()
    {
        ++downloadedTillNow;
        int spawnPointCount = BuilderData.spawnPoint.Count;
        switch (GameManager.currentLanguage)
        {

            case "en":
                assetDownloadingText.text = "Currently Setting up the world... " + (downloadedTillNow + spawnPointCount) + "/" + (totalAssetCount + spawnPointCount);
                assetDownloadingTextPotrait.text = "Currently Setting up the world... " + (downloadedTillNow + spawnPointCount) + "/" + (totalAssetCount + spawnPointCount);
                if (downloadedTillNow == totalAssetCount)
                {
                    assetDownloadingText.text = "Loading Completed.... " + (downloadedTillNow + spawnPointCount) + "/" + (totalAssetCount + spawnPointCount);
                    assetDownloadingTextPotrait.text = "Loading Completed.... " + (downloadedTillNow + spawnPointCount) + "/" + (totalAssetCount + spawnPointCount);
                    assetDownloadingText.color = Color.green;
                    assetDownloadingTextPotrait.color = Color.green;
                    assetDownloadingText.enabled = false;
                    assetDownloadingTextPotrait.enabled = false;
                    assetDownloadingText.transform.parent.gameObject.SetActive(false);
                    assetDownloadingTextPotrait.transform.parent.gameObject.SetActive(false);
                }
                break;
            case "ja":
                assetDownloadingText.text = "現在ワールドを構築中です.... " + (downloadedTillNow + spawnPointCount) + "/" + (totalAssetCount + spawnPointCount);
                assetDownloadingTextPotrait.text = "現在ワールドを構築中です.... " + (downloadedTillNow + spawnPointCount) + "/" + (totalAssetCount + spawnPointCount);
                if (downloadedTillNow == totalAssetCount)
                {
                    assetDownloadingText.text = "読み込み完了.... " + (downloadedTillNow + spawnPointCount) + "/" + (totalAssetCount + spawnPointCount);
                    assetDownloadingTextPotrait.text = "読み込み完了.... " + (downloadedTillNow + spawnPointCount) + "/" + (totalAssetCount + spawnPointCount);
                    assetDownloadingText.color = Color.green;
                    assetDownloadingTextPotrait.color = Color.green;
                    assetDownloadingText.enabled = false;
                    assetDownloadingTextPotrait.enabled = false;
                    assetDownloadingText.transform.parent.gameObject.SetActive(false);
                    assetDownloadingTextPotrait.transform.parent.gameObject.SetActive(false);
                }
                break;
            default:
                assetDownloadingText.text = "Currently Setting up the world... " + (downloadedTillNow + spawnPointCount) + "/" + (totalAssetCount + spawnPointCount);
                assetDownloadingTextPotrait.text = "Currently Setting up the world... " + (downloadedTillNow + spawnPointCount) + "/" + (totalAssetCount + spawnPointCount);
                if (downloadedTillNow == totalAssetCount)
                {
                    assetDownloadingText.text = "Loading Completed.... " + (downloadedTillNow + spawnPointCount) + "/" + (totalAssetCount + spawnPointCount);
                    assetDownloadingTextPotrait.text = "Loading Completed.... " + (downloadedTillNow + spawnPointCount) + "/" + (totalAssetCount + spawnPointCount);
                    assetDownloadingText.color = Color.green;
                    assetDownloadingTextPotrait.color = Color.green;
                    assetDownloadingText.enabled = false;
                    assetDownloadingTextPotrait.enabled = false;
                    assetDownloadingText.transform.parent.gameObject.SetActive(false);
                    assetDownloadingTextPotrait.transform.parent.gameObject.SetActive(false);
                }
                break;
        }
    }


    private static void InstantiateAsset(GameObject objectTobeInstantiate, ItemData _itemData)
    {
        GameObject newObj = Instantiate(objectTobeInstantiate, _itemData.Position, _itemData.Rotation, assetParentStatic);
        //Rigidbody rb = null;
        //newObj.TryGetComponent(out rb);
        //if (rb == null)
        //    rb = newObj.AddComponent<Rigidbody>();
        //rb.isKinematic = true;
        newObj.SetActive(true);
        XanaItem xanaItem = newObj.GetComponent<XanaItem>();
        xanaItem.itemData = _itemData;
        newObj.transform.localScale = _itemData.Scale;
        if (_itemData.ItemID.Contains("SPW") || _itemData.spawnComponent)
        {
            SpawnPointData spawnPointData = new SpawnPointData();
            spawnPointData.spawnObject = newObj;
            spawnPointData.IsActive = _itemData.spawnerComponentData.IsActive;
            BuilderData.spawnPoint.Add(spawnPointData);
        }

        if (IsMultiplayerComponent(_itemData) && GamificationComponentData.instance.withMultiplayer)
        {
            newObj.SetActive(false);
            if (PhotonNetwork.IsMasterClient)
            {
                var multiplayerObject = PhotonNetwork.InstantiateRoomObject("MultiplayerComponent", _itemData.Position, _itemData.Rotation);
                MultiplayerComponentData multiplayerComponentData = new();
                multiplayerComponentData.RuntimeItemID = _itemData.RuntimeItemID;
                multiplayerComponentData.viewID = multiplayerObject.GetPhotonView().ViewID;
                GamificationComponentData.instance.SetMultiplayerComponentData(multiplayerComponentData);
            }
            return;
        }

        //if (!newObj.name.Contains("pfBLD1210015_XANA"))
        //    meshCombinerRef.HandleRendererEvent(xanaItem.itemGFXHandler._renderers, _itemData);

        //foreach (Transform childTransform in newObj.GetComponentsInChildren<Transform>())
        //{
        //    childTransform.tag = "Item";
        //}

        //Add game object into XanaItems List for Hirarchy
        //if (!GamificationComponentData.instance.xanaItems.Exists(x => x == xanaItem))
        GamificationComponentData.instance.xanaItems.Add(xanaItem);
        if (!_itemData.isVisible)
            newObj.SetActive(false);
    }

    static bool IsMultiplayerComponent(ItemData itemData)
    {
        if (itemData.rotatorComponentData.IsActive || itemData.addForceComponentData.isActive || itemData.toFroComponentData.IsActive || itemData.translateComponentData.IsActive || itemData.scalerComponentData.IsActive || itemData.rotateComponentData.IsActive)
        {
            return true;
        }
        else return false;
    }

    IEnumerator CheckShortIntervalSorting()
    {
    CheckingAgain:
        yield return new WaitForSecondsRealtime(timeshortSorting);
        stopDownloading = true;
        currPlayerPosition = GameplayEntityLoader.instance.mainController.transform.localPosition;
        yield return new WaitForEndOfFrame();
        while (downloadIsGoingOn)
        {
            yield return null;
        }
        SortingDataShortInterval(currPlayerPosition);
        yield return new WaitForEndOfFrame();
        stopDownloading = false;
        StartCoroutine(DownloadAssetsFromSortedList());
        if (downloadDataQueue.Count > 0)
            goto CheckingAgain;
        else
        {
            stopDownloading = true;
            if (isWorldInstantiated)
            {
                isWorldInstantiated = false;
                BuilderEventManager.AfterWorldInstantiated?.Invoke();
            }
            //CheckPlacementOfAllObjects();
        }
    }


    IEnumerator CheckLongIntervalSorting()
    {
    CheckingAgain:
        yield return new WaitForSecondsRealtime(timeFullSorting);
        StopCoroutine(CheckShortIntervalSorting());
        stopDownloading = true;
        currPlayerPosition = GameplayEntityLoader.instance.mainController.transform.localPosition;
        yield return new WaitForEndOfFrame();
        while (downloadIsGoingOn)
        {
            yield return null;
        }
        SortingQueueData(currPlayerPosition);
        yield return new WaitForEndOfFrame();
        stopDownloading = false;
        StartCoroutine(DownloadAssetsFromSortedList());
        if (downloadDataQueue.Count > 0)
        {
            StartCoroutine(CheckShortIntervalSorting());
            goto CheckingAgain;
        }
        else
        {
            stopDownloading = true;
            if (isWorldInstantiated)
            {
                isWorldInstantiated = false;
                BuilderEventManager.AfterWorldInstantiated?.Invoke();
            }
            //CheckPlacementOfAllObjects();
        }

    }

    bool posChecking = true;
    public void CheckPlacementOfAllObjects()
    {
        if (posChecking)
        {
            foreach (Transform t in assetParent)
            {
                ItemData _itemData;
                builderDataDictionary.TryGetValue(t.name, out _itemData);
                t.transform.localPosition = _itemData.Position;
                t.transform.rotation = _itemData.Rotation;
            }
            posChecking = false;
        }
    }


    void OnOrientationChange()
    {
        if (totalAssetCount != downloadedTillNow)
        {
            if (ScreenOrientationManager._instance.isPotrait)
            {
                assetDownloadingText.transform.parent.gameObject.SetActive(false);
                assetDownloadingTextPotrait.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                assetDownloadingText.transform.parent.gameObject.SetActive(true);
                assetDownloadingTextPotrait.transform.parent.gameObject.SetActive(false);
            }
        }
    }

    public void ResetAll()
    {
        stopDownloading = true;
        //foreach(Transform t in assetParent)
        //{
        //    Destroy(t.gameObject);
        //}

        downloadDataQueue.Clear();
        builderDataDictionary.Clear();
        BuilderData.mapData = null;
        BuilderData.spawnPoint.Clear();
        BuilderData.preLoadspawnPoint.Clear();
        downloadedTillNow = 0;
        totalAssetCount = 0;
        dataArranged = false;
        dataSorted = false;

        // BuilderEventManager.OnBuilderDataFetch?.Invoke(ConstantsHolder.xanaConstants.builderMapID, SetConstant.isLogin);
        stopDownloading = false;
    }


}
