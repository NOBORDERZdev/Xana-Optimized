using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using TMPro;

public class AssetDownloader : MonoBehaviour
{
    [SerializeField]
    GameObject LoadingScreen; // Reference to the loading screen UI
    [SerializeField]
    GameObject Splash; // Reference to the splash screen UI
    [SerializeField]
    List<string> labelReference; // List of labels for the assets to be downloaded
    [SerializeField] 
    TMP_Text downloadProgressText; // Text UI element to show download progress
    [SerializeField] 
    TMP_Text fullDownloadProgress; // Text UI element to show full download progress
    [SerializeField] 
    GameObject LoadingPopup; // Reference to the loading popup UI
    bool isLoading = false; // Flag to check if loading is in progress
    List<string> failedDownloads = new List<string>(); // List to keep track of failed downloads

    void Start(){
        // Check if download permission is granted
        if (PlayerPrefs.GetString("DownloadPermission", "false") == "false")
        {
            // Show splash screen and hide loading screen
            LoadingScreen.SetActive(false);
            Splash.SetActive(true);
            LoadingPopup.SetActive(false);
            StartCoroutine(DownloadAssets());
        }
        else
        {
            // Show loading screen and hide splash screen
            LoadingScreen.SetActive(true);
            Splash.SetActive(false);
            LoadingPopup.SetActive(false);
            MoveToScene();
        }
    }

    IEnumerator DownloadAssets()
    {
        // Get the download size of the assets
        AsyncOperationHandle<long> downloadSize = Addressables.GetDownloadSizeAsync(labelReference);
        while (!downloadSize.IsDone)
        {
            yield return null;
        }
        Debug.Log("Download Size : " + downloadSize.Result);

        if (downloadSize.IsDone)
        {
            if (downloadSize.Result > 0)
            {
                // Show the full download progress
                fullDownloadProgress.text = $"{(downloadSize.Result / (1024f * 1024f)).ToString("F2")} MB";
                if (PlayerPrefs.GetString("DownloadPermission", "false") == "false")
                {
                    LoadingPopup.SetActive(true);
                }
                LoadingScreen.SetActive(true);
                Splash.SetActive(false);
            }
            else
            {
                // No download needed, move to the next scene
                LoadingScreen.SetActive(false);
                Splash.SetActive(true);
                LoadingPopup.SetActive(false);
                MoveToScene();
                PlayerPrefs.GetString("DownloadPermission", "true") ;
                PlayerPrefs.Save();
            }
        }
        else
        {
            // Error in getting download size, move to the next scene
            LoadingScreen.SetActive(false);
            Splash.SetActive(true);
            LoadingPopup.SetActive(false);
            MoveToScene();
        }
    }

    public void StartDownload(){ 
        // Start the download process
        LoadingPopup.SetActive(false);
        StartCoroutine(DownloadCourtuine());
    }

    IEnumerator DownloadCourtuine()
    {
        // Start downloading assets with retry logic
        yield return StartCoroutine(DownloadWithRetry(labelReference));

        if (failedDownloads.Count > 0)
        {
            Debug.Log("Retrying failed downloads...");
            while (failedDownloads.Count > 0)
            {
                List<string> currentFailedDownloads = new List<string>(failedDownloads);
                failedDownloads.Clear();
                yield return StartCoroutine(DownloadWithRetry(currentFailedDownloads));
            }
        }

        if (failedDownloads.Count > 0)
        {
            Debug.LogError("Some assets failed to download after multiple attempts.");
        }
        else
        {
            // Save download permission and move to the next scene
            PlayerPrefs.SetString("DownloadPermission", "true");
            PlayerPrefs.Save();
            Invoke(nameof(MoveToScene), 0.5f);
        }
    }

    IEnumerator DownloadWithRetry(List<string> labels)
    {
        bool success = false;

        while (!success)
        {
            // Download dependencies for the given labels
            AsyncOperationHandle loader = Addressables.DownloadDependenciesAsync(labels, Addressables.MergeMode.None, false);
            while (!loader.IsDone)
            {
                yield return new WaitForEndOfFrame();
                string s = (loader.GetDownloadStatus().Percent * 100f).ToString("F0").PadLeft(2, '0');
                downloadProgressText.text = " " + s + "%";
                if (loader.GetDownloadStatus().TotalBytes > 0 && !isLoading)
                {
                    Debug.Log("Inside Load Asset");
                    isLoading = true;
                    Debug.Log("Size  " + loader.GetDownloadStatus().TotalBytes);
                }
            }

            if (loader.Status == AsyncOperationStatus.Succeeded)
            {
                success = true;
            }
            else
            {
                Debug.LogError("Download failed for some assets. Retrying...");
                yield return new WaitForSeconds(3); // Wait for 3 seconds before retrying
            }
        }

        if (!success)
        {
            Debug.LogError("Download failed after multiple attempts.");
            failedDownloads.AddRange(labels);
        }
    }

    void MoveToScene()
    {
        // Load the next scene
        SceneManager.LoadScene("Home");
    }

    public void CloseApp()
    {
        // Close the application
        Application.Quit();
    }
}
