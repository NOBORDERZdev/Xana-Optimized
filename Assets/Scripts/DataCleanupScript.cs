using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
public class DataCleanupScript : MonoBehaviour
{
    private void Start()
    {
#if !UNITY_ANDROID
        BuildScriptableObject buildScriptableObject =
        Resources.Load("BuildVersion/BuildVersion") as BuildScriptableObject;

        string completeVer = Application.version + "-" + buildScriptableObject.BuildNumber;

        Debug.Log("version :- "+completeVer);

        if (PlayerPrefs.HasKey("AppVer"))
        {
            if(PlayerPrefs.GetString("AppVer")!=completeVer)
            {
                ClearAddressableData();
                PlayerPrefs.SetString("AppVer", completeVer);
            }
        }
        else
        {
            ClearAddressableData();
            PlayerPrefs.SetString("AppVer",completeVer);
        }
#endif
    }

    public void ClearAddressableData()
    {
        // Unload all addressable assets and release their references
        Addressables.CleanBundleCache();
        Caching.ClearCache();
        
    }
}