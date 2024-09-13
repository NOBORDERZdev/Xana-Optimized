using RenderHeads.Media.AVProVideo;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Video;
using ZeelKheni.YoutubePlayer.Components;
using ZeelKheni.YoutubePlayer.Extensions;
using ZeelKheni.YoutubePlayer.Api;
using ZeelKheni.YoutubePlayer.Models;
using ZeelKheni.YoutubePlayer;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class AdvancedYoutubePlayer : MonoBehaviour
{
    public enum Quality
    {
        Standard = 18,
        UHD = 313,
        FULLHD = 137,
        HD = 136,
        HIGH = 135
    };


    public bool IsLive;
    public bool PlayOnAwake;
    [HideInInspector]
    public string UploadFeatureVideoID;

    public YoutubeInstance YoutubeInstance;

    [Tooltip("AddUrl For LiveSTreams")]
    public string VideoId;

    [Tooltip("require 2 video players except standard")]
    public Quality PreferedQuality;


    public VideoPlayer VideoPlayer;
    public VideoPlayer VideoPlayer1;
    public AudioSource audioSource;

    [Tooltip("Use AVPRO")]
    public MediaPlayer AVProVideoPlayer;

    public GameObject PreRecVideoScreen;
    public MeshFilter LiveVideoPlayerScreen;
    public Mesh AndroidLiveVideoMesh;
    public Mesh IOSLiveVideoMesh;

    string m_StartedPlayingVideoId;
    string m_PlayingVideoId;
    Action<string> HLSurlLoaded;

    public bool IsInternetDisconnected = false;

    private void OnEnable()
    {
        AvatarSpawnerOnDisconnect.OninternetDisconnect += OnInternetDisconnect;
        AvatarSpawnerOnDisconnect.OninternetConnected += OnInternetConnect;
        VideoPlayer.errorReceived += (hand, message) => { Debug.Log("Error in video.... "); PrepareVideoUrls(default, true); };
        SetSoundSettingReference();
        //if (IsLive)
        //{
        //    AVProVideoPlayer.gameObject.SetActive(true);
        //    VideoPlayer.gameObject.SetActive(false);
        //    AVProVideoPlayer.AutoStart = true;
        //    HLSurlLoaded += OnHlsLoaded;
        //}
        //else
        //{
        //    AVProVideoPlayer.gameObject.SetActive(false);
        //    VideoPlayer.gameObject.SetActive(true);
        //}

        //if (PlayOnAwake)
        //{
        //    await PlayVideoAsync();
        //}
    }
    private void OnDisable()
    {
        AvatarSpawnerOnDisconnect.OninternetDisconnect -= OnInternetDisconnect;
        AvatarSpawnerOnDisconnect.OninternetConnected -= OnInternetConnect;
    }

    public async void PlayVideo()
    {
        if (IsLive)
        {
            AVProVideoPlayer.gameObject.SetActive(true);
            VideoPlayer.gameObject.SetActive(false);
            AVProVideoPlayer.AutoStart = true;
            HLSurlLoaded += OnHlsLoaded;
        }
        else
        {
            AVProVideoPlayer.gameObject.SetActive(false);
            VideoPlayer.gameObject.SetActive(true);
        }

        if (PlayOnAwake)
        {
            await PlayVideoAsync();
        }
    }

    public async Task PlayVideoAsync(CancellationToken cancellationToken = default)
    {
        await PrepareVideoAsync(cancellationToken);
        if (!IsLive)
            VideoPlayer.Play();
        else
            AVProVideoPlayer.Play();
    }

    public void SetSoundSettingReference()
    {
        if (SoundSettings.soundManagerSettings == null)
        {
            return;
        }
        if (SceneManager.GetActiveScene().name == "Builder")
        {
            if (VideoPlayer != null)
            {
                if (VideoPlayer.GetComponent<AudioSource>())
                {
                    SoundSettings.soundManagerSettings.AddVideoSources(VideoPlayer.GetComponent<AudioSource>());
                }
            }
            if (AVProVideoPlayer != null)
            {
                SoundSettings.soundManagerSettings.AddLiveVideoSources(AVProVideoPlayer);
            }
        }
    }
    public async Task PrepareVideoAsync(CancellationToken cancellationToken = default)
    {
        // TODO: use destroyCancellationToken in 2022.3

        m_PlayingVideoId = VideoId;
        m_StartedPlayingVideoId = null;
        await PrepareVideoUrls(cancellationToken);
        /* if (!IsLive)
         {
             VideoPlayer.source = VideoSource.Url;
         }

         await TrySetVideoPlayerUrl();
         if (!IsLive)
             await VideoPlayer.PrepareAsync(cancellationToken);*/
    }

    private async Task PrepareVideoUrls(CancellationToken cancellationToken = default, bool skipPrevious = false)
    {
        if (!IsLive)
        {
            if (YoutubeInstance == null)
            {
                throw new InvalidOperationException("YoutubeInstance is not set");
            }

            var instanceUrl = await YoutubeInstance.GetInstanceUrl(cancellationToken);
            var videoInfo = await YoutubeApi.GetVideoInfo(instanceUrl, VideoId, cancellationToken, YoutubeInstance.YoutubeInstanceInfos, skipPrevious);
            var videoformat = GetCompatibleFormat(videoInfo, ((int)PreferedQuality).ToString());
            var audioformat = GetCompatibleFormat(videoInfo, "18");
            Debug.Log("<color=red>instance url " + instanceUrl + " our url " + videoformat.Url + "</color>");

            string url = GetProxiedUrl(videoformat.Url, instanceUrl);
            string audio = GetProxiedUrl(audioformat.Url, instanceUrl);

            if (VideoPlayer.url != url)
            {
                if (PreferedQuality != Quality.Standard)
                {
                    StartCoroutine(PlayVideoAndAudio(url, audio));
                }
                else
                {
                    Debug.Log($"Setting video url to {url}");
                    VideoPlayer.url = url;
                    VideoPlayer.Prepare();
                    BuilderEventManager.YoutubeVideoLoadedCallback?.Invoke(UploadFeatureVideoID);
                    VideoPlayer.Play();
                }
            }
        }
        else
        {
            var YoutubeHlsGetter = new FetchHtmlContent();
            StartCoroutine(YoutubeHlsGetter.GetHtmlContent(VideoId, HLSurlLoaded));
        }

    }


    private IEnumerator PlayVideoAndAudio(string video, string audio)
    {
        // Set the URLs
        VideoPlayer.url = video;
        VideoPlayer1.url = audio;
        // Prepare video
        VideoPlayer.Prepare();
        VideoPlayer1.Prepare();
        // Wait until the video is prepared
        while (!VideoPlayer.isPrepared || !VideoPlayer1.isPrepared)
        {
            yield return null;
        }

        // Download audio file
        /*using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(audio, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log(audio);
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = audioClip;
            }
        }*/

        BuilderEventManager.YoutubeVideoLoadedCallback?.Invoke(UploadFeatureVideoID);
        // Play both video and audio
        VideoPlayer.Play();
        VideoPlayer1.Play();

    }

    private string GetProxiedUrl(string url, string YoutubeUrl)
    {
        var uri = new Uri(url);
        var YoutubeUri = new Uri(YoutubeUrl);
        var builder = new UriBuilder(uri)
        {
            Host = YoutubeUri.Host,
            Scheme = YoutubeUri.Scheme,
            Port = YoutubeUri.Port
        };
        return builder.Uri.ToString();
    }

    private void OnHlsLoaded(string url)
    {
        if (AVProVideoPlayer.MediaPath.Path != url)
        {
            Debug.Log($"Setting video url to {url}");
            AVProVideoPlayer.OpenMedia(new MediaPath(url, MediaPathType.AbsolutePathOrURL), true);
        }
    }

    private FormatInfo GetCompatibleFormat(VideoInfo videoInfo, string Tag = "")
    {
        Debug.Log("Tag  " + Tag);
        var itag = Tag;
        if (string.IsNullOrEmpty(itag))
        {
            // 720p, the highest quality available with the video and audio combined
            itag = "22";
        }
        if (!IsLive)
        {
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
        else
        {
            return new FormatInfo() { Itag = itag, Url = videoInfo.hlsUrl };
        }
    }

    public void EnableVideoScreen(bool _isLiveVideo)
    {
        if (_isLiveVideo)
        {

#if UNITY_ANDROID
            LiveVideoPlayerScreen.mesh = AndroidLiveVideoMesh;
#elif UNITY_IOS
            LiveVideoPlayerScreen.mesh = IOSLiveVideoMesh;
#endif
            if (LiveVideoPlayerScreen != null)
            {
                LiveVideoPlayerScreen.gameObject.SetActive(_isLiveVideo);
            }
            if (PreRecVideoScreen != null)
            {
                PreRecVideoScreen.SetActive(!_isLiveVideo);
            }
        }
        else
        {
            VideoPlayer.playOnAwake = false;
            if (PreRecVideoScreen != null)
            {
                PreRecVideoScreen.SetActive(!_isLiveVideo);
            }
            if (LiveVideoPlayerScreen != null)
            {
                LiveVideoPlayerScreen.gameObject.SetActive(_isLiveVideo);
            }
        }
    }

    public void OnInternetDisconnect()
    {
        IsInternetDisconnected = true;
        //print("Internet Disconnected");
        if (VideoPlayer != null)
        {
            VideoPlayer.Stop();
        }
        if (AVProVideoPlayer != null)
        {
            AVProVideoPlayer.Stop();
        }
    }

    public void OnInternetConnect()
    {
        IsInternetDisconnected = false;
        //print("Internet connected again");
        if (VideoPlayer != null)
        {
            VideoPlayer.Play();
        }
        if (AVProVideoPlayer != null)
        {
            AVProVideoPlayer.Play();
        }
    }

    public static class QualityExtensions
    {
        public static string ToQualityString(Quality quality)
        {
            switch (quality)
            {
                case Quality.UHD:
                    return "0";
                case Quality.FULLHD:
                    return "137";
                case Quality.HD:
                    return "136";
                case Quality.HIGH:
                    return "137";
                default:
                    return "0";
            }
        }
    }
}
