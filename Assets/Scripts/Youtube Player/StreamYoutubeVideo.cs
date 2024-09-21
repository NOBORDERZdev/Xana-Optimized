using Newtonsoft.Json;
using RenderHeads.Media.AVProVideo;
using SimpleJSON;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using UnityEngine.Events;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using ZeelKheni.YoutubePlayer.Models;
using ZeelKheni.YoutubePlayer.Api;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using System.Linq;
using ZeelKheni.YoutubePlayer.Extensions;
using static AdvancedYoutubePlayer;
using System;
using ZeelKheni.YoutubePlayer.Components;
using System.Collections.Generic;

public class StreamYoutubeVideo : MonoBehaviour
{
    public string streamAbleUrl;
    public string oldUrl;
    public GameObject LiveVideoUIRef;
    public SummitDomeNFTDataController SumitDomeNftCntrlrRef;
    public MediaPlayer mediaPlayer;
    public VideoPlayer videoPlayer;
    public UnityEvent liveVideoPlay;

    //Store ID for Builder Scene
    public string id;

    private void OnEnable()
    {
        AvatarSpawnerOnDisconnect.OninternetDisconnect += OnInternetDisconnect;
        videoPlayer.prepareCompleted += (vid) => { NFT_Holder_Manager.instance.videoReady(); };
        
    }

    private void OnDisable()
    {
        AvatarSpawnerOnDisconnect.OninternetDisconnect -= OnInternetDisconnect;
    }

    public void StreamYtVideo(string Url, bool isLive)
    {
        if (oldUrl != Url)
        {
            PrepareVideoUrls(Url, isLive);//StartCoroutine(GetStreamableUrl(Url, isLive));
        }
        else {
            PrepareVideoUrls(Url, isLive , default, true);
            /*if(isLive)
        {
            PlayLiveVideo();
        }
        else if (!isLive)
        {
            PlayPrerecordedVideo();*/
        }
    }
    public string ExtractVideoIdFromUrl(string url)
    {

        Uri uri = new Uri(url);
        if (!url.Contains("v="))
            return uri.Segments.Last();

        // Find the position of the "v=" parameter
        int startIndex = url.IndexOf("v=");

        if (startIndex != -1)
        {
            // Extract the substring after "v="
            startIndex += 2; // Move past "v="
            int endIndex = url.IndexOf('&', startIndex);
            if (endIndex == -1)
                endIndex = url.Length;

            // Get the video ID
            string videoId = url.Substring(startIndex, endIndex - startIndex);
            return videoId;
        }
        else if (url.Contains("youtu.be"))
        {
            // https://youtu.be/p4Sg5894rUg - To extract ID from Shortened URL like this:
            startIndex = url.LastIndexOf('/');
            if (startIndex != -1)
            {
                startIndex += 1;
                int endIndex = url.Length;

                string videoId = url[startIndex..endIndex]; // startIndex, endIndex - startIndex
                return videoId;
            }
        }

        // If "v=" parameter is not found, handle accordingly (e.g., return null or an error message)
        return null;
    }
    private async void PrepareVideoUrls(string Url, bool isLive, CancellationToken cancellationToken = default, bool skipPrevious = false)
    {oldUrl = Url;
        if (!isLive)
        {
            var VideoId = ExtractVideoIdFromUrl( Url);
            string video = await getvideoasync(VideoId);
           
            if (!video.IsNullOrEmpty())
            {
                videoPlayer.url = video;
                videoPlayer.Prepare();
               
                videoPlayer.Play();
                return;
            }
           

            var instanceUrl = await YoutubeInstance.Instance.GetInstanceUrl(cancellationToken);
            VideoInfo videoInfo = null;
            var lst = new List<YoutubeVideoInfo>(YoutubeInstance.Instance.YoutubeInstanceInfos);
            videoInfo = await YoutubeApi.GetVideoInfo(instanceUrl, VideoId, cancellationToken, lst, skipPrevious);

            if (videoInfo == null)
            {

                var youtube = new YoutubeClient();
                var videdo = await youtube.Videos.GetAsync("https://www.youtube.com/watch?v=" + VideoId);
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videdo.Id);
                var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();
                if (videoPlayer.url != streamInfo.Url)
                {



                    videoPlayer.url = streamInfo.Url;
                    videoPlayer.Prepare();
                    BuilderEventManager.YoutubeVideoLoadedCallback?.Invoke(id);
                    videoPlayer.Play();

                }
                setStreamableAsync(VideoId, streamInfo.Url);
            }
            else
            {

                var videoformat = GetCompatibleFormat(videoInfo);
           
               

                string url = videoformat.Url;//GetProxiedUrl(videoformat.Url, "https://invidious.xana.net/");
             



             
                        Debug.Log($"Setting video url to {url}");
                videoPlayer.url = url;
                videoPlayer.Prepare();
                BuilderEventManager.YoutubeVideoLoadedCallback?.Invoke(id);
                videoPlayer.Play();
                        setStreamableAsync(VideoId, url);
                 
            }
        }
        else
        {
            var YoutubeHlsGetter = new FetchHtmlContent();
            StartCoroutine(YoutubeHlsGetter.GetHtmlContent(Url, OnHlsLoaded));
        }
    }
    private void OnHlsLoaded(string url)
    {
        if (mediaPlayer.MediaPath.Path != url)
        {
            Debug.Log($"Setting video url to {url}");
            BuilderEventManager.YoutubeVideoLoadedCallback?.Invoke(id);
            mediaPlayer.OpenMedia(new MediaPath(url, MediaPathType.AbsolutePathOrURL), true);
        }
    }
    private FormatInfo GetCompatibleFormat(VideoInfo videoInfo, string Tag = "")
    {
        Debug.Log("Tag  " + Tag);
        var itag = Tag;
        if (string.IsNullOrEmpty(itag))
        {
            // 720p, the highest quality available with the video and audio combined
            itag = "18";
        }
      
            var format = videoInfo.FormatStreams.Find(f => f.Itag == itag);
            if (format != null)
            {
                return format;
            }

            // Maybe we're looking for an adaptive videoformat?
            format = videoInfo.AdaptiveFormats.Find(f => f.Itag == itag);
            if (format != null)
            {
                return format;
            }

            return videoInfo.FormatStreams.Last();
       
    }
    public async Task<string> getvideoasync(string videoId)
    {
        try
        {
            UnityEngine.Networking.UnityWebRequest request = UnityWebRequest.Get(ConstantsGod.API_BASEURL + ConstantsGod.YtStreamUrl + videoId);
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            await request.SendWebRequestAsync();
            if (request.error.IsNullOrEmpty())
            {
                var data = JsonConvert.DeserializeObject<YtStream>(request.downloadHandler.text, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                if (data.success)
                {
                    return data.data.url;
                }
            }
            return "";
        }
        catch (Exception e)
        {
            return "";
        }
    }


    public async void setStreamableAsync(string videoId, string Streamableurl)
    {
        WWWForm frm = new WWWForm();
        frm.AddField("videoId", videoId);
        frm.AddField("url", Streamableurl);
        UnityWebRequest request = UnityWebRequest.Post(ConstantsGod.API_BASEURL + ConstantsGod.YtSetStreamUrl, frm);
        request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        await request.SendWebRequestAsync();
        if (request.error.IsNullOrEmpty())
        {
            Debug.Log(request.downloadHandler.text);
        }
        Debug.Log(request.downloadHandler.text);
    }
    public IEnumerator GetStreamableUrl(string Url, bool isLive)
    {
        WWWForm form = new WWWForm();
        form.AddField("youtubeUrl", Url);
        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.GetStreamableYoutubeUrl), form))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);

            yield return www.SendWebRequest();
            while (!www.isDone)
            {
                yield return null;
            }
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("SteamError:" + www.error);
            }
            else
            {
                oldUrl = Url;
                string data = www.downloadHandler.text;
                GetYoutubeStreamableVideo getYoutubeStreamableVideo = JsonConvert.DeserializeObject<GetYoutubeStreamableVideo>(data);
                streamAbleUrl = getYoutubeStreamableVideo.data.downloadableUrl;
                if (isLive)
                {
                    PlayLiveVideo();
                }
                else
                {
                    PlayPrerecordedVideo();
                }
            }
            www.Dispose();
        }
    }


    private void PlayLiveVideo()
    {
        mediaPlayer.OpenMedia(MediaPathType.AbsolutePathOrURL, streamAbleUrl, true);
        mediaPlayer.Play();
        liveVideoPlay.Invoke();
        //BuilderEventManager.YoutubeVideoLoadedCallback?.Invoke(id);
    }

    private void PlayPrerecordedVideo()
    {
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = streamAbleUrl;

        if (ConstantsHolder.xanaConstants.isBuilderScene)
        {
            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += VideoPrepared;
        }
        else
            videoPlayer.Play();
    }

    void VideoPrepared(VideoPlayer vp)
    {
        vp.Play();
        BuilderEventManager.YoutubeVideoLoadedCallback?.Invoke(id);
    }

    public void OnInternetDisconnect()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }
        if (mediaPlayer != null)
        {
            mediaPlayer.Stop();
        }
    }

    public void OnInternetConnect()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Play();
        }
        if (mediaPlayer != null)
        {
            mediaPlayer.Play();
        }
    }

}
[System.Serializable]
public class GetYoutubeStreamableVideo
{
    public bool success;
    public YoutubeStreamData data;
    public string msg;
}
public class YoutubeStreamData
{
    public string downloadableUrl;
}