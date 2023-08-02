using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class SplashVideoPlay : MonoBehaviour
{
    public string video_download_link;
    public VideoPlayer player;
  
    private void Start()
    {
       
#if UNITY_ANDROID
        video_download_link = SplashVideoDownloader.splashVideoDownloader.playbackDir;
#endif
#if UNITY_IPHONE
       video_download_link= "file://" + SplashVideoDownloader.splashVideoDownloader.playbackDir;
#endif
        player.url = video_download_link;
        player.loopPointReached += End;
      }

    void End(VideoPlayer VP) {
        this.gameObject.SetActive(false);
    }
}
