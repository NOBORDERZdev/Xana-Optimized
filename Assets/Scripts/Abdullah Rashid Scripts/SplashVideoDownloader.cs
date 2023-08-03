using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.Video;
using System;

public class SplashVideoDownloader : MonoBehaviour
{
    public static SplashVideoDownloader splashVideoDownloader;
    public string url;
    public string playbackDir;
    public string saveDir;
   
    private void Awake()
    {
        if (splashVideoDownloader == null)
            splashVideoDownloader = this;
    }
    public void Start()
    {
        url = "https://xana-prod-item.s3.ap-southeast-1.amazonaws.com/xanaprod/SplashVideo/BLACK+SCREEN+INTO+SPLASH.mp4";

        StartCoroutine(downloadAndPlayVideo(url, "SplashVideo.mp4", true));
    }


    //Downloads, Saves and Plays the Video
    IEnumerator downloadAndPlayVideo(string videoUrl, string saveFileName, bool overwriteVideo)
    {
        //Where to Save the Video
        saveDir = Path.Combine(Application.persistentDataPath, saveFileName);

        //Play back Directory
         playbackDir = saveDir;
#if UNITY_IPHONE
        playbackDir = "file://" + saveDir;
#endif

        bool downloadSuccess = false;
        byte[] vidData = null;

        /*Check if the video file exist before downloading it again. 
         Requires(using System.Linq)
        */
        string[] persistantData = Directory.GetFiles(Application.persistentDataPath);
        if (persistantData.Contains(playbackDir) && !overwriteVideo)
        {
            Debug.Log("Video already exist. Playing it now");
            //Play Video
           // playVideo(playbackDir);
            //EXIT
            yield break;
        }
        else if (persistantData.Contains(playbackDir) && overwriteVideo)
        {
            Debug.Log("Video already exist...");
            //yield return downloadData(videoUrl, (status, dowloadData) =>
            //{
            //    downloadSuccess = status;
            //    vidData = dowloadData;
            //});
        }
        else
        {
            Debug.Log("Video Does not exist. Downloading video");
            yield return downloadData(videoUrl, (status, dowloadData) =>
            {
                downloadSuccess = status;
                vidData = dowloadData;
            });
        }

        //Save then Play if there was no download error
        if (downloadSuccess)
        {
            //Save Video
            saveVideoFile(saveDir, vidData);

            //Play Video
           // playVideo(playbackDir);
        }
    }

    //Downloads the Video
    IEnumerator downloadData(string videoUrl, Action<bool, byte[]> result)
    {
        //Download Video
        UnityWebRequest webRequest = UnityWebRequest.Get(videoUrl);
        webRequest.Send();

        //Wait until download is done
        while (!webRequest.isDone)
        {
            Debug.Log("Downloading: " + webRequest.downloadProgress);
            yield return null;
        }

        //Exit if we encountered error
        if (webRequest.isNetworkError)
        {
            Debug.Log("Error while downloading Video: " + webRequest.error);
            yield break; //EXIT
        }

        Debug.Log("Video Downloaded");
        //Retrieve downloaded Data
        result(!webRequest.isNetworkError, webRequest.downloadHandler.data);
    }

    //Saves the video
    bool saveVideoFile(string saveDir, byte[] vidData)
    {
        try
        {
            FileStream stream = new FileStream(saveDir, FileMode.Create);
            stream.Write(vidData, 0, vidData.Length);
            stream.Close();
            Debug.Log("Video Downloaded to: " + saveDir.Replace("/", "\\"));
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("Error while saving Video File: " + e.Message);
        }
        return false;
    }

    //Plays the video
    void playVideo(string path)
    {
        Handheld.PlayFullScreenMovie(path, Color.black,
            FullScreenMovieControlMode.Full, FullScreenMovieScalingMode.AspectFill);
    }
}
