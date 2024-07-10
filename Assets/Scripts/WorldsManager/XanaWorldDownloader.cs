﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using YoutubeLight;

public class XanaWorldDownloader : MonoBehaviour
{

    //booleans 
    public static bool stopDownloading;
    public static bool downloadIsGoingOn;
    public static bool dataArranged;
    public static bool dataSorted;
    public static bool isSpawnDownloaded;
    public static bool isDefaultPriorityObjectDownloaded;
    public static bool isfailedObjectsDownloaded;
    public static bool isLowPriorityDownloaded;

    public Transform assetParent;
    public TMPro.TextMeshProUGUI assetDownloadingText;
    public TMPro.TextMeshProUGUI assetDownloadingTextPotrait;

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

    public static List<DownloadQueueData> downloadDataQueue = new List<DownloadQueueData>();
    public static List<DownloadQueueData> preLoadObjects = new List<DownloadQueueData>();
    public static List<DownloadQueueData> postLoadObjects = new List<DownloadQueueData>();
    public static List<DownloadQueueData> downloadFailed = new List<DownloadQueueData>();
    public static Dictionary<string, ObjectsInfo> xanaWorldDataDictionary = new Dictionary<string, ObjectsInfo>();
    public static Dictionary<string, GameObject> prefabObjectPool = new Dictionary<string, GameObject>();


    public static SceneData xanaSceneData = new SceneData();

    private string response;

    private float unloadDistance = 50;

    private static XanaWorldDownloader xanaWorldDownloader;

    private void Start()
    {
        xanaWorldDownloader = this;
    }

    void GetDataFromAPI(string worldID, bool islocal)
    {
        if (islocal)
        {
            response = worldID;
            xanaSceneData = JsonUtility.FromJson<SceneData>(worldID);
        }
        else
        {
            StartCoroutine(DownloadSceneData(worldID));
        }
    }

    IEnumerator DownloadSceneData(string worldID)
    {
        string _url = ConstantsGod.API_BASEURL + ConstantsGod.GETXANAOFFICIALWORLDBYID + worldID;
        using (UnityWebRequest www = UnityWebRequest.Get(_url))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            www.SendWebRequest();
            while (!www.isDone)
            {
                yield return null;
            }
            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                response = www.downloadHandler.text;
            }
            else
            {
                response = www.downloadHandler.text;
                xanaSceneData = JsonUtility.FromJson<SceneData>(www.downloadHandler.text);
            }
        }
    }

    private void OnEnable()
    {
        if (assetParent)
            assetParentStatic = assetParent;
        if (!ConstantsHolder.xanaConstants.isBuilderScene)
        {
            BuilderEventManager.XanaMapDataDownloaded += PostLoadingBuilderAssets;
            ScreenOrientationManager.switchOrientation += OnOrientationChange;
        }
    }

    private void OnDisable()
    {
        BuilderEventManager.XanaMapDataDownloaded -= PostLoadingBuilderAssets;
        ScreenOrientationManager.switchOrientation -= OnOrientationChange;
        ResetAll();
    }

    async void PostLoadingBuilderAssets(string worldData)
    {
        GetDataFromAPI(worldData, true);
        ArrangeData();
        while (!dataArranged)
        {
            await Task.Yield();
        }
        StartCoroutine(DownloadObjects(preLoadObjects, Priority.High));
        while (!isSpawnDownloaded)
        {
            await Task.Yield();
        }
        StartDownloadingAssets();
        if (totalAssetCount != downloadedTillNow)
            EnableDownloadingText();
    }

    void LoadAddressableSceneAfterDownload()
    {
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
    }

    public static void ArrangeData()
    {
        try
        {
            for (int i = 0; i < xanaSceneData.SceneObjects.Count; i++)
            {
                DownloadQueueData temp = new DownloadQueueData();
                temp.ItemID = xanaSceneData.SceneObjects[i].addressableKey;
                temp.DcitionaryKey = i.ToString();
                temp.Position = xanaSceneData.SceneObjects[i].position;
                temp.Rotation = xanaSceneData.SceneObjects[i].rotation;
                temp.Scale = xanaSceneData.SceneObjects[i].scale;

                if (!xanaWorldDataDictionary.ContainsKey(i.ToString()))
                {
                    xanaWorldDataDictionary.Add(i.ToString(), xanaSceneData.SceneObjects[i]);
                    if (xanaSceneData.SceneObjects[i].priority == Priority.High)
                    {
                        preLoadObjects.Add(temp);
                    }
                    if (xanaSceneData.SceneObjects[i].priority == Priority.Low)
                    {
                        postLoadObjects.Add(temp);
                    }
                    else
                    {
                        downloadDataQueue.Add(temp);
                        totalAssetCount++;
                    }
                }
            }
            dataArranged = true;
        }
        catch (Exception e)
        {
            Debug.LogError("An error occurred: " + e.Message);
        }

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
            ////Debug.LogError(item.Item1.ItemID+"---"+item.Item2);
            downloadDataQueue.Add(item.Item1);
            // //Debug.Log("Position: " + item.Item1 + ", Distance: " + item.Item2);
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
            ////Debug.Log("Position: " + item.Item1 + ", Distance: " + item.Item2);
        }
    }

    async void StartDownloadingAssets()
    {
        SortingQueueData(initialPlayerPos);
        while (!dataSorted)
        {
            await Task.Yield();
        }
        StartCoroutine(DownloadAssetsFromSortedList());
        while (!isDefaultPriorityObjectDownloaded)
        {
            await Task.Yield();
        }
        StartCoroutine(DownloadObjects(postLoadObjects, Priority.Low));
        while (!isLowPriorityDownloaded)
        {
            await Task.Yield();
        }
        StartCoroutine(DownloadFailedItem());
        while (!isfailedObjectsDownloaded)
        {
            await Task.Yield();
        }
        //StartCoroutine(CheckLongIntervalSorting());
        //StartCoroutine(CheckShortIntervalSorting());

    }

    IEnumerator DownloadAssetsFromSortedList()
    {
        while (downloadDataQueue.Count > 0 && !stopDownloading)
        {
            downloadIsGoingOn = true;
            string downloadKey = downloadDataQueue[0].ItemID;
            string dicKey = downloadDataQueue[0].DcitionaryKey;
            AsyncOperationHandle<GameObject> _async;
            //GameObject objectfromPool = GetObjectFromPool(downloadKey);
            //if (objectfromPool)
            //{
            //    InstantiateAsset(objectfromPool, xanaWorldDataDictionary[dicKey], true);
            //    yield break;
            //}
            //else
            _async = Addressables.LoadAssetAsync<GameObject>(downloadKey);
            while (!_async.IsDone)
            {
                yield return null;
            }
            if (_async.Status == AsyncOperationStatus.Succeeded)
            {
                InstantiateAsset(_async.Result, xanaWorldDataDictionary[dicKey], dicKey);
            }
            else
            {
                //Debug.LogError("Download Failed......");
            }
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.05f);
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

        if (totalAssetCount == downloadedTillNow)
            LoadingFlagUpdate(Priority.defaultPriority);
    }


    IEnumerator DownloadFailedItem()
    {
        while (downloadFailed.Count > 0)
        {
            string downloadKey = downloadFailed[0].ItemID;
            string dicKey = downloadFailed[0].DcitionaryKey;
            AsyncOperationHandle<GameObject> _async;
            //GameObject objectfromPool = GetObjectFromPool(downloadKey);
            //if (objectfromPool)
            //{
            //    InstantiateAsset(objectfromPool, xanaWorldDataDictionary[dicKey], true);
            //    yield break;
            //}
            //else
            _async = Addressables.LoadAssetAsync<GameObject>(downloadKey);

            while (!_async.IsDone)
            {
                yield return null;
            }
            if (_async.Status == AsyncOperationStatus.Succeeded)
            {
                InstantiateAsset(_async.Result, xanaWorldDataDictionary[dicKey], dicKey);
            }
            else
            {
                //Debug.LogError("Download Failed......");
            }
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.01f);
            downloadFailed.RemoveAt(0);
            DisplayDownloadedAssetText();
        }
        isfailedObjectsDownloaded = true;
        if (totalAssetCount == downloadedTillNow)
        {
            BuilderEventManager.AfterWorldOffcialWorldsInatantiated?.Invoke();
        }
    }

    public IEnumerator DownloadObjects(List<DownloadQueueData> downloadQueues, Priority objectLoadingPriority)
    {
        for (int i = 0; i < downloadQueues.Count; i++)
        {
            string downloadKey = downloadQueues[i].ItemID;
            string dicKey = downloadQueues[i].DcitionaryKey;
            AsyncOperationHandle<GameObject> _async;
            //GameObject objectfromPool = GetObjectFromPool(downloadKey);
            //if (objectfromPool)
            //{
            //    InstantiateAsset(objectfromPool, xanaWorldDataDictionary[dicKey], true);
            //    yield break;
            //}
            //else
            _async = Addressables.LoadAssetAsync<GameObject>(downloadKey);
            while (!_async.IsDone)
            {
                yield return null;
            }
            if (_async.Status == AsyncOperationStatus.Succeeded)
            {
                InstantiateAsset(_async.Result, xanaWorldDataDictionary[dicKey], dicKey);
            }
            else
            {
                //Debug.LogError(_async.Status);
            }
            yield return new WaitForSeconds(0.01f);
        }
        LoadingFlagUpdate(objectLoadingPriority);
    }

    void LoadingFlagUpdate(Priority flag)
    {
        switch (flag)
        {
            case Priority.defaultPriority:
                isDefaultPriorityObjectDownloaded = true;
                break;
            case Priority.High:
                isSpawnDownloaded = true;
                break;
            case Priority.Low:
                isLowPriorityDownloaded = true;
                break;
        }
    }

    void EnableDownloadingText()
    {
        assetDownloadingText.transform.parent.gameObject.SetActive(true);
        assetDownloadingTextPotrait.transform.parent.gameObject.SetActive(false);
    }

    void DisplayDownloadedAssetText()
    {
        ++downloadedTillNow;
        switch (GameManager.currentLanguage)
        {

            case "en":
                assetDownloadingText.text = "Currently Setting up the world... " + (downloadedTillNow) + "/" + (totalAssetCount);
                assetDownloadingTextPotrait.text = "Currently Setting up the world... " + (downloadedTillNow) + "/" + (totalAssetCount);
                if (downloadedTillNow == totalAssetCount)
                {
                    assetDownloadingText.text = "Loading Completed.... " + downloadedTillNow + "/" + (totalAssetCount);
                    assetDownloadingTextPotrait.text = "Loading Completed.... " + downloadedTillNow + "/" + (totalAssetCount);

                    assetDownloadingText.transform.parent.gameObject.SetActive(false);
                    assetDownloadingTextPotrait.transform.parent.gameObject.SetActive(false);
                }
                break;
            case "ja":
                assetDownloadingText.text = "現在ワールドを構築中です.... " + (downloadedTillNow) + "/" + (totalAssetCount);
                assetDownloadingTextPotrait.text = "現在ワールドを構築中です.... " + (downloadedTillNow) + "/" + (totalAssetCount);
                if (downloadedTillNow == totalAssetCount)
                {
                    assetDownloadingText.text = "読み込み完了.... " + downloadedTillNow + "/" + (totalAssetCount);
                    assetDownloadingTextPotrait.text = "読み込み完了.... " + downloadedTillNow + "/" + (totalAssetCount);
                    assetDownloadingText.transform.parent.gameObject.SetActive(false);
                    assetDownloadingTextPotrait.transform.parent.gameObject.SetActive(false);
                }
                break;
            default:
                assetDownloadingText.text = "Currently Setting up the world... " + (downloadedTillNow) + "/" + (totalAssetCount);
                assetDownloadingTextPotrait.text = "Currently Setting up the world... " + (downloadedTillNow) + "/" + (totalAssetCount);
                if (downloadedTillNow == totalAssetCount)
                {
                    assetDownloadingText.text = "Loading Completed.... " + downloadedTillNow + "/" + (totalAssetCount);
                    assetDownloadingTextPotrait.text = "Loading Completed.... " + downloadedTillNow + "/" + (totalAssetCount);
                    assetDownloadingText.transform.parent.gameObject.SetActive(false);
                    assetDownloadingTextPotrait.transform.parent.gameObject.SetActive(false);
                }
                break;
        }
    }

    void ResetDisplayDownloadText()
    {
        assetDownloadingText.text = string.Empty;
        assetDownloadingTextPotrait.text = string.Empty;
        assetDownloadingText.transform.parent.gameObject.SetActive(false);
        assetDownloadingTextPotrait.transform.parent.gameObject.SetActive(false);
    }


    GameObject GetObjectFromPool(string objectKey)
    {
        if (prefabObjectPool.ContainsKey(objectKey))
        {
            if (CheckAvailableForReuse(objectKey, prefabObjectPool[objectKey]))
                return prefabObjectPool[objectKey];
        }
        return null;
    }

    static void AddObjectInPool(string objectKey, GameObject prefabObject)
    {
        if (prefabObjectPool.ContainsKey(objectKey))
        {
            return;
        }
        else
        {
            prefabObjectPool.Add(objectKey, prefabObject);
        }
    }

    void RemoveItemFromPool(string objectKey)
    {
        Addressables.Release(objectKey);
        prefabObjectPool.Remove(objectKey);
    }

    bool CheckAvailableForReuse(string key, GameObject poolObject)
    {
        ObjectsInfo objectsInfo = new ObjectsInfo();
        if (xanaWorldDataDictionary.TryGetValue(key, out objectsInfo))
        {
            float distance = Vector3.Distance(poolObject.transform.position, objectsInfo.position);
            if (distance > unloadDistance)
                return true;
        }
        return false;
    }
    private static void InstantiateAsset(GameObject objectTobeInstantiate, ObjectsInfo _itemData, string downloadKey)
    {
        GameObject newObj = Instantiate(objectTobeInstantiate, _itemData.position, _itemData.rotation, assetParentStatic);
        newObj.transform.localScale = _itemData.scale;
        newObj.name = _itemData.name;
        newObj.SetActive(_itemData.isActive);
        ApplyLightmapData(_itemData.lightmapData, newObj);
        AddObjectInPool(downloadKey, newObj);
        AssignDomeId(newObj, _itemData);

       

    }

    private static void InstantiateAsset(GameObject ObjectFromPool, ObjectsInfo _itemData, bool alreadyInstantiated)
    {
        ObjectFromPool.transform.localScale = _itemData.scale;
        ObjectFromPool.name = _itemData.name;
        ObjectFromPool.SetActive(_itemData.isActive);
        ApplyLightmapData(_itemData.lightmapData, ObjectFromPool);
    }

    private static void ApplyLightmapData(LightmapData[] lightmapData, GameObject prefab)
    {
        try
        {
            Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].lightmapIndex = lightmapData[i].lightmapIndex;
                renderers[i].lightmapScaleOffset = lightmapData[i].lightmapScaleOffset;
            }
        }
        catch (Exception e)
        {
            //Debug.LogError("Error while applying lightmap data :- " + e.Message);
        }

    }

    static void AssignDomeId(GameObject DomeObject, ObjectsInfo _itemData)
    {
        if (_itemData.summitDomeInfo.domeIndex != 0)
        {
            DomeObject.GetComponentInChildren<OnTriggerSceneSwitch>().domeId = _itemData.summitDomeInfo.domeIndex;
            DomeObject.GetComponentInChildren<OnTriggerSceneSwitch>().textMeshPro.text = _itemData.summitDomeInfo.domeIndex.ToString();

        }

        #region DomeMiniMap

        // Cache the reference to xanaConstants for repeated use
        var xanaConstants = ConstantsHolder.xanaConstants;

        // Use StringComparison.Ordinal for case-sensitive comparison as it's faster for non-linguistic data
        if (xanaConstants.EnviornmentName.Equals("XANA Summit", StringComparison.Ordinal))
        {
            // Assuming newObj.transform doesn't change, consider caching GetComponentInChildren result if possible
            var sceneSwitchComponent = DomeObject.transform.GetComponentInChildren<OnTriggerSceneSwitch>();
            if (sceneSwitchComponent != null)
            {
                DomeMinimapDataHolder.OnInitDome?.Invoke(sceneSwitchComponent);
            }
        }
        #endregion
    }

    IEnumerator CheckForUnloading()
    {
        CheckingAgain:
        yield return new WaitForSecondsRealtime(timeshortSorting);
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
            BuilderEventManager.AfterWorldInstantiated?.Invoke();
            //CheckPlacementOfAllObjects();
        }
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
            BuilderEventManager.AfterWorldInstantiated?.Invoke();
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
            BuilderEventManager.AfterWorldInstantiated?.Invoke();
            //CheckPlacementOfAllObjects();
        }

    }




    //bool posChecking = true;
    //public void CheckPlacementOfAllObjects()
    //{
    //    if (posChecking)
    //    {
    //        foreach (Transform t in assetParent)
    //        {
    //            ItemData _itemData;
    //            builderDataDictionary.TryGetValue(t.name, out _itemData);
    //            t.transform.localPosition = _itemData.Position;
    //            t.transform.rotation = _itemData.Rotation;
    //        }
    //        posChecking = false;
    //    }
    //}


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

    public static void ResetAll()
    {
        stopDownloading = true;

        downloadDataQueue.Clear();
        xanaWorldDataDictionary.Clear();
        preLoadObjects.Clear();
        postLoadObjects.Clear();
        downloadFailed.Clear();
        xanaSceneData.SceneObjects.Clear();
        BuilderData.mapData = null;
        BuilderData.spawnPoint.Clear();
        downloadedTillNow = 0;
        totalAssetCount = 0;
        dataArranged = false;
        dataSorted = false;
        stopDownloading = false;
        isDefaultPriorityObjectDownloaded = false;
        isfailedObjectsDownloaded = false;
        isSpawnDownloaded = false;

        xanaWorldDownloader.ResetDisplayDownloadText();
        xanaWorldDownloader.StopAllCoroutines();
    }

}
[System.Serializable]
public class SceneData
{
    public List<ObjectsInfo> SceneObjects = new List<ObjectsInfo>();
}

[System.Serializable]
public class ObjectsInfo
{
    public string addressableKey;
    public string name;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public Bounds objectBound;
    public Priority priority;
    public string tagName;
    public int layerIndex;
    public bool isActive;
    public LightmapData[] lightmapData;
    public SummitDomeInfo summitDomeInfo;
}
[System.Serializable]
public class LightmapData
{
    public int lightmapIndex;
    public Vector4 lightmapScaleOffset;
    // Add more fields as needed to store relevant data
}

[System.Serializable]
public class SummitDomeInfo
{
    public int domeIndex;
}


public enum Priority
{
    defaultPriority = 0,
    High = 1,
    Low = 2
}