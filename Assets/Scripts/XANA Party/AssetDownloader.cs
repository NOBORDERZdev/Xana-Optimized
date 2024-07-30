using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class AssetDownloader : MonoBehaviour
{
    [SerializeField]
    TMP_Text assetTxt;

    [SerializeField]
    List<string> tags;
    async void Start()
    {
        var resourceLocator = await Addressables.InitializeAsync().Task;
        var allKeys = tags;//resourceLocator.Keys.ToList();
        var downloadSize = await Addressables.GetDownloadSizeAsync(tags).Task;
        print("downloadSize" + downloadSize);
        if (downloadSize > 0)
        {
            var downloadHandle = Addressables.DownloadDependenciesAsync(tags, Addressables.MergeMode.Union);
            StartCoroutine(ShowDownloadProgress(downloadHandle, downloadSize));
        }
        else
        {
            // Continue with your game logic
        }
    }

    IEnumerator ShowDownloadProgress(AsyncOperationHandle downloadHandle, long totalDownloadSize)
    {
        while (!downloadHandle.IsDone)
        {
            long currentDownloaded = (long)(downloadHandle.PercentComplete * totalDownloadSize);
            string progress = (currentDownloaded / totalDownloadSize).ToString();
            print("PROGRESS " + progress);
            assetTxt.text="PROGRESS : "+ progress;
            yield return null;
        }
        assetTxt.text = " : COMPLETED : ";
        // Download completed, continue with your game logic
    }
}
