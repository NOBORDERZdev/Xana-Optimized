using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using System.IO;
using UnityEngine.ResourceManagement.AsyncOperations;


public class VersionChecker : MonoBehaviour
{
    private const string VersionKey = "AppVersion";
    private const string VersionCodeKey = "AppVersionCode";

    void Awake()
    {
        CheckAppVersion();
    }

    void CheckAppVersion()
    {

        string currentVersion = Application.version;
        int currentVersionCode = GetVersionCode();

        if (!PlayerPrefs.HasKey(VersionKey))
            PlayerPrefs.SetString(VersionKey, currentVersion);
        if (!PlayerPrefs.HasKey(VersionCodeKey))
            PlayerPrefs.SetInt(VersionCodeKey, currentVersionCode);

        string storedVersion = PlayerPrefs.GetString(VersionKey, "");
        int storedVersionCode = PlayerPrefs.GetInt(VersionCodeKey, -1);

        if ((storedVersion != currentVersion) || (storedVersionCode != currentVersionCode))
        {
            Addressables.CleanBundleCache();
            Caching.ClearCache();
            ClearAddressablesCache();
            PlayerPrefs.SetString(VersionKey, currentVersion);
            PlayerPrefs.SetInt(VersionCodeKey, currentVersionCode);
            //Handheld.Vibrate();
        }
        else
        {
            AddressableDownloader.Instance.DownloadCatalogFile();
        }
    }

    public void ClearAddressablesCache()
    {
        // Get the path to the addressables cache directory
        string cachePath = Path.Combine(Application.persistentDataPath, "com.unity.addressables");

        // Log the cache path for debugging
        Debug.Log($"Cache Path: {cachePath}");

        // Check if the directory exists
        if (Directory.Exists(cachePath))
        {
            try
            {
                // Delete the directory and all its contents
                Directory.Delete(cachePath, true);
                Debug.Log("Addressables cache directory deleted.");
            }
            catch (IOException e)
            {
                Debug.LogError($"Failed to delete cache directory: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("Addressables cache directory not found.");
        }

        // Clear Addressables cache using API
        ClearAddressablesCacheUsingAPI();
    }

    // Clear the Addressables cache using the API
    private void ClearAddressablesCacheUsingAPI()
    {
        Addressables.InitializeAsync().Completed += OnAddressablesInitialized;
    }

    private void OnAddressablesInitialized(AsyncOperationHandle<IResourceLocator> initHandle)
    {
        if (initHandle.Status == AsyncOperationStatus.Succeeded)
        {
            // Clear the cache for all keys (assuming "allAssets" is a valid label)
            string label = "PMY ACADEMY";
            AsyncOperationHandle handle = Addressables.ClearDependencyCacheAsync(label, true);
            handle.Completed += OnCacheCleared;
        }
        else
        {
            Debug.LogError("Failed to initialize Addressables.");
        }
    }

    private void OnCacheCleared(AsyncOperationHandle handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            //Debug.LogError("Addressables cache cleared via API.");
        }
        else
        {
           // Debug.LogError("Failed to clear Addressables cache via API.");
        }
        AddressableDownloader.Instance.DownloadCatalogFile();
    }


    int GetVersionCode()
    {
#if UNITY_EDITOR
        // Retrieve version code from Player Settings in Unity Editor
#if UNITY_ANDROID
        return PlayerSettings.Android.bundleVersionCode;
#elif UNITY_IOS
            return int.Parse(PlayerSettings.iOS.buildNumber);
#else
            return 1; // Default version code for other platforms
#endif
#elif UNITY_ANDROID
        using (var versionCodeClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            var context = versionCodeClass.GetStatic<AndroidJavaObject>("currentActivity");
            var packageManager = context.Call<AndroidJavaObject>("getPackageManager");
            var packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo", context.Call<string>("getPackageName"), 0);
            return packageInfo.Get<int>("versionCode");
        }
#elif UNITY_IOS
        // For iOS, you would retrieve this from the Info.plist file.
        return int.Parse(UnityEngine.iOS.Device.systemVersion.Replace(".", ""));
#else
        return 1; // Default version code for other platforms or during development.
#endif
    }

}
