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
    GameObject LoadingScreen;
    [SerializeField]
    GameObject Splash;
    [SerializeField]
    List<string> labelReference;
    [SerializeField] 
    TMP_Text downloadProgressText;
    [SerializeField] 
    TMP_Text fullDownloadProgress;
    [SerializeField] 
    GameObject LoadingPopup;
    bool isLoading = false;

    void Start(){ 
        StartCoroutine(DownloadAssets());
    }

    IEnumerator DownloadAssets()
    {
        AsyncOperationHandle<long> downloadSize = Addressables.GetDownloadSizeAsync(labelReference);
        Debug.Log("Download Size : " + downloadSize.Result);
        while (!downloadSize.IsDone)
        {
            yield return null;
        }

        if (downloadSize.IsDone)
        {
            if (downloadSize.Result>0)
            {
                fullDownloadProgress.text = $"Downloading Data {(downloadSize.Result / 1024f) / 1024f} MB";
                if (PlayerPrefs.GetString("DownloadPermission", "false") == "false")
                {
                    LoadingPopup.SetActive(true);
                }
            }
            else
            {
                LoadingScreen.SetActive(false);
                Splash.SetActive(true);
                LoadingPopup.SetActive(false);
                MoveToScene();
            }
        }
        else
        {
            LoadingScreen.SetActive(false);
            Splash.SetActive(true);
            LoadingPopup.SetActive(false);
            MoveToScene();
        }

        
    }

    public void StartDownload(){ 
        LoadingPopup.SetActive(false);
        StartCoroutine(DownloadCourtuine());
    }

    IEnumerator DownloadCourtuine(){ 
        AsyncOperationHandle loader = Addressables.DownloadDependenciesAsync(labelReference,Addressables.MergeMode.None,false);
        while (!loader.IsDone)
        {
            yield return new WaitForEndOfFrame();
            string s = (loader.GetDownloadStatus().Percent * 100f).ToString("F0");
            downloadProgressText.text = /*"Please wait downloading assets... " +*/ s + "%";
            if(loader.GetDownloadStatus().TotalBytes > 0 && !isLoading)
            {
                Debug.Log("Inside Load Asset");
                isLoading = true;
                Debug.Log("Size  "+ loader.GetDownloadStatus().TotalBytes);
            }
        }
        PlayerPrefs.SetString("DownloadPermission", "true");
        Invoke(nameof(MoveToScene),0.5f);
    }


    void MoveToScene()
    {
        SceneManager.LoadScene("Home");
    }

    public void CloseApp()
    {
        Application.Quit();
    }
}