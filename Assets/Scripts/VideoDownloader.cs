using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Networking;


public class VideoDownloader : MonoBehaviour
{
     string video_download_link;

    // Start is called before the first frame update
    void Start()
    {
        if (this.gameObject.name.Contains("TVAppare"))
        {
            video_download_link = "https://angeluim-metaverse.s3.ap-southeast-1.amazonaws.com/unitydata/environments/Idol+girl+Villa/Video+For+Idol+girl+Villa/Appare.mp4";
           StartCoroutine( VideoDownload());
        }
        else if (this.gameObject.name.Contains("Peel The Apple TV"))
        {
            video_download_link = "https://angeluim-metaverse.s3.ap-southeast-1.amazonaws.com/unitydata/environments/Idol+girl+Villa/Video+For+Idol+girl+Villa/Peeltheapple.mp4";
            StartCoroutine(VideoDownload());
        }

        else if (this.gameObject.name.Contains("TV PrettyAsh"))
        {
            video_download_link = "https://angeluim-metaverse.s3.ap-southeast-1.amazonaws.com/unitydata/environments/Idol+girl+Villa/Video+For+Idol+girl+Villa/Prettyash.mp4";
            StartCoroutine(VideoDownload());

        }
        else if (this.gameObject.name.Contains("TV Task Have Fun"))
        {
            video_download_link = "https://angeluim-metaverse.s3.ap-southeast-1.amazonaws.com/unitydata/environments/Idol+girl+Villa/Video+For+Idol+girl+Villa/Taskhavefun.mp4";
            StartCoroutine(VideoDownload());

        }
        else if (this.gameObject.name.Contains("TV UpUP Girl"))
        {
            video_download_link = "https://angeluim-metaverse.s3.ap-southeast-1.amazonaws.com/unitydata/environments/Idol+girl+Villa/Video+For+Idol+girl+Villa/%E3%82%A2%E3%83%83%E3%83%97%E3%82%A2%E3%83%83%E3%83%97%E3%82%AC%E3%83%BC%E3%83%AB%E3%82%BA(%E4%BB%AE).mp4";
            StartCoroutine(VideoDownload());

        }
        else if (this.gameObject.name.Contains("TV Lilnade"))
        {
            video_download_link = "https://angeluim-metaverse.s3.ap-southeast-1.amazonaws.com/unitydata/environments/Idol+girl+Villa/Video+For+Idol+girl+Villa/%E3%83%AA%E3%83%AB%E3%83%8D%E3%83%BC%E3%83%89.mp4";
            StartCoroutine(VideoDownload());

        }
        else if (this.gameObject.name.Contains("TV Osaka"))
        {
            video_download_link = "https://angeluim-metaverse.s3.ap-southeast-1.amazonaws.com/unitydata/environments/Idol+girl+Villa/Video+For+Idol+girl+Villa/%E5%A4%A7%E9%98%AA%E6%98%A5%E5%A4%8F%E7%A7%8B%E5%86%AC.mp4";
            StartCoroutine(VideoDownload());

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator VideoDownload()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(video_download_link))
        {
            yield return request.SendWebRequest();
            yield return new WaitForEndOfFrame();

            if (!request.isHttpError || !request.isNetworkError)
            {
                if(request.error == null)
                {
                    this.GetComponent<VideoPlayer>().url = video_download_link;
                }
            }
        }
    }
}
