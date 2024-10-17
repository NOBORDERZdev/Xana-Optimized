using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class XanaWorldDownloader : MonoBehaviour
{

    //booleans 
    public static bool stopDownloading;
    public static bool downloadIsGoingOn;
    public static bool dataArranged;
    public static bool dataSorted;
    public static bool isSpawnDownloaded;
    public static bool isDefaultPriorityObjectDownloaded;
    public static bool isPostPriorityObjectDownloaded;
    public static bool isfailedObjectsDownloaded;

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


    public static SceneData xanaSceneData = new SceneData();

    public string response;
    // Start is called before the first frame update
    void Start()
    {

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
        if (!XanaConstants.xanaConstants.isBuilderScene)
        {
            BuilderEventManager.XanaMapDataDownloaded += PostLoadingBuilderAssets;
            ChangeOrientation_waqas.switchOrientation += OnOrientationChange;
        }

    }

    private void OnDisable()
    {

        BuilderEventManager.XanaMapDataDownloaded -= PostLoadingBuilderAssets;
        ChangeOrientation_waqas.switchOrientation -= OnOrientationChange;
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
        StartCoroutine(DownloadObjects(preLoadObjects,true));
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
            Debug.Log("<color=red>An error occurred: " + e.Message + "</color>");
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
            //Debug.LogError(item.Item1.ItemID+"---"+item.Item2);
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
        
        
        StartCoroutine(DownloadObjects(postLoadObjects, false));
        while (!isPostPriorityObjectDownloaded)
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
            //AsyncOperationHandle<GameObject> _async = Addressables.LoadAssetAsync<GameObject>(downloadKey);
            bool flag = false;
            AsyncOperationHandle _async = AddressableDownloader.Instance.MemoryManager.GetReferenceIfExist(downloadKey, ref flag);
            if (!flag)
            {
                try
                {
                    _async = Addressables.LoadAssetAsync<GameObject>(downloadKey);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error while downloading Addressable :- " + downloadKey);
                }
            }
            while (!_async.IsDone)
            {
                yield return null;
            }
            if (_async.Status == AsyncOperationStatus.Succeeded)
            {
                InstantiateAsset(_async.Result as GameObject, xanaWorldDataDictionary[dicKey]);
                AddressableDownloader.Instance.MemoryManager.AddToReferenceList(_async, downloadKey);
            }
            else
            {
                Debug.LogError("Download Failed......");
            }
            yield return new WaitForSeconds(0.01f);
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
        isDefaultPriorityObjectDownloaded = true;
    }


    IEnumerator DownloadFailedItem()
    {
        while (downloadFailed.Count > 0)
        {
            string downloadKey = downloadFailed[0].ItemID;
            string dicKey = downloadFailed[0].DcitionaryKey;
            //AsyncOperationHandle<GameObject> _async = Addressables.LoadAssetAsync<GameObject>(downloadKey);
            bool flag = false;
            AsyncOperationHandle _async = AddressableDownloader.Instance.MemoryManager.GetReferenceIfExist(downloadKey, ref flag);
            if (!flag)
            {
                try
                {
                    _async = Addressables.LoadAssetAsync<GameObject>(downloadKey);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error while downloading Addressable :- " + downloadKey);
                }
            }
            while (!_async.IsDone)
            {
                yield return null;
            }
            if (_async.Status == AsyncOperationStatus.Succeeded)
            {
                InstantiateAsset(_async.Result as GameObject, xanaWorldDataDictionary[dicKey]);
                AddressableDownloader.Instance.MemoryManager.AddToReferenceList(_async, downloadKey);
            }
            else
            {
                Debug.LogError("Download Failed......");
            }
            yield return new WaitForSeconds(0.1f);
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
        isfailedObjectsDownloaded = true;
        if (totalAssetCount == downloadedTillNow)
        {
            BuilderEventManager.AfterWorldOffcialWorldsInatantiated?.Invoke();
        }
    }


    public IEnumerator DownloadObjects(List<DownloadQueueData> downloadQueues, bool preLodaingObjects)
    {
        for (int i = 0; i < downloadQueues.Count; i++)
        {
            string downloadKey = downloadQueues[i].ItemID;
            string dicKey = downloadQueues[i].DcitionaryKey;
            //AsyncOperationHandle<GameObject> _async = Addressables.LoadAssetAsync<GameObject>(downloadKey);
            bool flag = false;
            AsyncOperationHandle _async = AddressableDownloader.Instance.MemoryManager.GetReferenceIfExist(downloadKey, ref flag);
            if (!flag)
            {
                _async = Addressables.LoadAssetAsync<GameObject>(downloadKey);
                Debug.LogError(downloadKey + "Obj Not exist in Memory Release pool");
            }
            else
            {
                Debug.LogError(downloadKey + "Obj exist in Memory Release pool");
            }
            while (!_async.IsDone)
            {
                yield return null;
            }
            if (_async.Status == AsyncOperationStatus.Succeeded)
            {
                InstantiateAsset(_async.Result as GameObject, xanaWorldDataDictionary[dicKey]);
                AddressableDownloader.Instance.MemoryManager.AddToReferenceList(_async, downloadKey);
            }
            else
            {
                Debug.LogError("Failed_PL_" + _async.Status);
            }
            yield return new WaitForSeconds(0.01f);
        }
        if (preLodaingObjects)
            isSpawnDownloaded = true;
        else
            isPostPriorityObjectDownloaded = true;
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
                    assetDownloadingText.color = Color.green;
                    assetDownloadingTextPotrait.color = Color.green;
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
                    assetDownloadingText.color = Color.green;
                    assetDownloadingTextPotrait.color = Color.green;
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
                    assetDownloadingText.color = Color.green;
                    assetDownloadingTextPotrait.color = Color.green;
                    assetDownloadingText.transform.parent.gameObject.SetActive(false);
                    assetDownloadingTextPotrait.transform.parent.gameObject.SetActive(false);
                }
                break;
        }
    }


    private static void InstantiateAsset(GameObject objectTobeInstantiate, ObjectsInfo _itemData)
    {
        GameObject newObj = Instantiate(objectTobeInstantiate, _itemData.position, _itemData.rotation, assetParentStatic);
        newObj.transform.localScale = _itemData.scale;
        newObj.name = _itemData.name;
        newObj.SetActive(_itemData.isActive);
        ApplyLightmapData(_itemData.lightmapData, newObj);
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
            Debug.Log("<color=red>Error while applying lightmap data :- " + e.Message + "</color>");
        }

    }

    IEnumerator CheckShortIntervalSorting()
    {
        CheckingAgain:
        yield return new WaitForSecondsRealtime(timeshortSorting);
        stopDownloading = true;
        currPlayerPosition = LoadFromFile.instance.mainController.transform.localPosition;
        yield return new WaitForSeconds(.5f);
        while (downloadIsGoingOn)
        {
            yield return null;
        }
        SortingDataShortInterval(currPlayerPosition);
        yield return new WaitForSeconds(.5f);
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
        currPlayerPosition = LoadFromFile.instance.mainController.transform.localPosition;
        yield return new WaitForSeconds(.5f);
        while (downloadIsGoingOn)
        {
            yield return null;
        }
        SortingQueueData(currPlayerPosition);
        yield return new WaitForSeconds(.5f);
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
            if (ChangeOrientation_waqas._instance.isPotrait)
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
        isDefaultPriorityObjectDownloaded=false;
        isfailedObjectsDownloaded = false;
        isSpawnDownloaded = false;
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
}
[System.Serializable]
public class LightmapData
{
    public int lightmapIndex;
    public Vector4 lightmapScaleOffset;
    // Add more fields as needed to store relevant data
}

public enum Priority
{
    defaultPriority = 0,
    High = 1,
    Low = 2
}