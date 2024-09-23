using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UnityEngine.Video;
using RenderHeads.Media.AVProVideo;
using LightShaft.Scripts;

public class YoutubeStreamController : MonoBehaviour
{
    [SerializeField]
    public GameObject LiveStreamPlayer;
    [SerializeField]
    private VideoPlayer NormalPlayer;
    [SerializeField]
    private YoutubeAPIHandler APIHandler;
    private YoutubeStreamController Instance;
    public AudioSource videoPlayerAudioSource;
    public AudioSource mediaPlayerAudioSource;

    private string PrevURL;
    private bool IsOldURL = true;
    public static Action playPrercordedVideo;
    public AdvancedYoutubePlayer streamYoutubeVideo;
    // Start is called before the first frame update
    private void OnEnable()
    {
        PrevURL = "xyz";
        StartCoroutine(SetStreamContinous());
        playPrercordedVideo += PlayPrerecordedVideo;
       
    }

    private void OnDisable()
    {
        playPrercordedVideo -= PlayPrerecordedVideo;
    }

    public IEnumerator SetStreamContinous()
    {
        while (true)
        {
            StartCoroutine(SetStream());
            yield return new WaitForSeconds(5.0f);
        }
    }

    public void PlayPrerecordedVideo()
    {
        //YoutubeSimplified player = NormalPlayer.GetComponent<YoutubeSimplified>();
        NormalPlayer.url = APIHandler.Data.URL;
        NormalPlayer.Play();
    }

    private void Awake()
    {

        Instance = this;
        if (SoundController.Instance)
        {
            //if (NormalPlayer.GetComponent<YoutubeSimplified>().player.GetComponent<YoutubePlayer>().playInAVPRO)
            //{
            //    SoundController.Instance.videoPlayerSource = mediaPlayerAudioSource;
            //    SoundSettings.soundManagerSettings.videoSource = mediaPlayerAudioSource;
            //}
            //else
            //{
            //SoundController.Instance.videoPlayerSource = videoPlayerAudioSource;
            //SoundSettings.soundManagerSettings.videoSource = videoPlayerAudioSource;
            //}

            //SoundController.Instance.livePlayerSource = LiveStreamPlayer.GetComponent<MediaPlayer>();
            SoundSettings.soundManagerSettings.setNewSliderValues();
        }
    }

    private void Start()
    {
        //if (this.GetComponent<StreamYoutubeVideo>() != null)
        //{
        //    streamYoutubeVideo = this.GetComponent<StreamYoutubeVideo>();
        //}
        //if (videoPlayerAudioSource)
        //    videoPlayerAudioSource.gameObject.GetComponent<VideoPlayer>().targetMaterialRenderer.material.color = new Color32(57, 57, 57, 255);
        //if (NormalPlayer.GetComponent<YoutubeSimplified>().videoPlayer != null)
        //    NormalPlayer.GetComponent<YoutubeSimplified>().videoPlayer.targetMaterialRenderer.material.color = new Color32(57, 57, 57, 255);
        //if (NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer != null)
        //    NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.GetComponent<ApplyToMesh>().MeshRenderer.sharedMaterial.color = new Color32(57, 57, 57, 255);
        //#if UNITY_EDITOR && !UNITY_IOS
        //        if (!WorldItemView.m_EnvName.Contains("BreakingDown Arena") && !WorldItemView.m_EnvName.Contains("XANA FESTIVAL STAGE in Dubai.") && !WorldItemView.m_EnvName.Contains("DJ Event"))
        //        {
        //            Vector3 scale = NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale;
        //            scale.y *= -1;
        //            NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale = scale;
        //        }
        //#endif
        //#if UNITY_IOS
        //        if (WorldItemView.m_EnvName.Contains("DJ Event"))
        //        {
        //            Vector3 scale = NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale;
        //            scale.y *= -1;
        //            NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale = scale;
        //        }
        //        if (WorldItemView.m_EnvName.Contains("Xana Festival"))
        //        {
        //            Vector3 scale = NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale;
        //            scale.y *= -1;
        //            NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale = scale;
        //        }
        //        if (WorldItemView.m_EnvName.Contains("XANA Festival Stage"))
        //        {
        //            Vector3 scale = NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale;
        //            scale.y *= -1;
        //            NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale = scale;
        //        }
        //        if (WorldItemView.m_EnvName.Contains("NFTDuel Tournament") || WorldItemView.m_EnvName.Contains("XANA Lobby"))
        //        {
        //            Vector3 scale = NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale;
        //            scale.y *= -1;
        //            NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale = scale;
        //        }
        //#endif
    }

    private IEnumerator SetStream()
    {
        yield return StartCoroutine(APIHandler.GetStream());

        while (APIHandler.Data == null)
        {
            yield return null;
        }
        if (!APIHandler.Data.isPlaying) yield break;

        if (PrevURL.Equals(APIHandler.Data.URL)) yield break;

        if (!PrevURL.Equals(APIHandler.Data.URL) && APIHandler.Data.isPlaying)
        {
            PrevURL = APIHandler.Data.URL;
            SetUpStream();
        }
        else if (!APIHandler.Data.isPlaying)
        {
            streamYoutubeVideo.AVProVideoPlayer.enabled = false;
            LiveStreamPlayer.SetActive(false);
            NormalPlayer.gameObject.SetActive(true);
            if (gameObject.GetComponent<AvProLiveVideoSoundEnabler>())
            {
                gameObject.GetComponent<AvProLiveVideoSoundEnabler>().EnableLiveVideoSound(false);
            }
            //YoutubeSimplified player = NormalPlayer.GetComponent<YoutubeSimplified>();

            //LiveStreamPlayer.GetComponent<ApplyToMesh>().MeshRenderer.sharedMaterial.color = new Color32(57, 57, 57, 255);
            //if (NormalPlayer.GetComponent<YoutubeSimplified>().videoPlayer != null)
            //    NormalPlayer.GetComponent<YoutubeSimplified>().videoPlayer.targetMaterialRenderer.material.color = new Color32(57, 57, 57, 255);
            //if (NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer != null)
            //    NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.GetComponent<ApplyToMesh>().MeshRenderer.sharedMaterial.color = new Color32(57, 57, 57, 255);

            //player.OnInternetDisconnect();
            NormalPlayer.Stop();
        }
        else if (APIHandler.Data.isPlaying && APIHandler.Data.IsLive && !LiveStreamPlayer.GetComponent<MediaPlayer>().Info.HasVideo())
        {
            PrevURL = APIHandler.Data.URL;
            SetUpStream();
        }
        else if (APIHandler.Data.isPlaying && APIHandler.Data.IsLive && !LiveStreamPlayer.activeInHierarchy)
        {
            SetUpStream();
        }
        else if (APIHandler.Data.isPlaying && !APIHandler.Data.IsLive && !NormalPlayer.gameObject.activeInHierarchy)
        {
            SetUpStream();
        }
        else
        {
            if (APIHandler.Data.IsLive && LiveStreamPlayer.activeInHierarchy)
            {
                if (streamYoutubeVideo.AVProVideoPlayer.Control != null && !streamYoutubeVideo.AVProVideoPlayer.Control.IsPlaying())
                {
                    if (!streamYoutubeVideo.IsInternetDisconnected && APIHandler.Data.isPlaying.Equals(true))
                    {
                        //Debug.Log("Extra video play call worked for live video");
                        SetUpStream();
                    }
                }
            }
            //LiveStreamPlayer.GetComponent<ApplyToMesh>().MeshRenderer.sharedMaterial.color = new Color32(255, 255, 255, 255);
            //if (NormalPlayer.GetComponent<YoutubeSimplified>().videoPlayer != null)
            //    NormalPlayer.GetComponent<YoutubeSimplified>().videoPlayer.targetMaterialRenderer.material.color = new Color32(255, 255, 255, 255);
            //if (NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer != null)
            //    NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.GetComponent<ApplyToMesh>().MeshRenderer.sharedMaterial.color = new Color32(255, 255, 255, 255);
        }
    }

    private void SetUpStream()
    {
        if (APIHandler.Data.IsLive && APIHandler.Data.isPlaying)
        {
            SetVideoQuality(APIHandler.Data.quality);
            Debug.Log("Hardik changes check");
            streamYoutubeVideo.EnableVideoScreen(true);
            streamYoutubeVideo.AVProVideoPlayer.enabled = true;
            LiveStreamPlayer.SetActive(true);
            NormalPlayer.gameObject.SetActive(false);
            //if (gameObject.GetComponent<AvProDirectionalSound>())
            //{
            //    gameObject.GetComponent<AvProDirectionalSound>().ActivateDirectionalSoundIfNotYet();
            //}



            if (gameObject.GetComponent<AvProLiveVideoSoundEnabler>() && gameObject.GetComponent<AvProDirectionalSound>().PlayerTriggerCheck != null &&
                gameObject.GetComponent<AvProDirectionalSound>().PlayerTriggerCheck.IsPlayerTriggered)
            {
                gameObject.GetComponent<AvProLiveVideoSoundEnabler>().EnableLiveVideoSound(true);
            }

            //YoutubePlayerLivestream player = LiveStreamPlayer.GetComponent<YoutubePlayerLivestream>();
            //if (player)
            //{
            //    player.GetLivestreamUrl(APIHandler.Data.URL);
            //}
            //streamYoutubeVideo.StreamYtVideo(APIHandler.Data.URL, APIHandler.Data.IsLive);
            streamYoutubeVideo.VideoId = APIHandler.Data.URL;
            streamYoutubeVideo.IsLive = APIHandler.Data.IsLive;
            streamYoutubeVideo.PlayVideo();
            SetBGMAudioSound();
            if (!WorldItemView.m_EnvName.Contains("Xana Festival") || !WorldItemView.m_EnvName.Contains("NFTDuel Tournament"))
            {
                NormalPlayer.gameObject.SetActive(false);
            }

        }
        else
        {
            SetVideoQuality(APIHandler.Data.quality);
            //LiveStreamPlayer.GetComponent<ApplyToMesh>().MeshRenderer.sharedMaterial.color = new Color32(57, 57, 57, 255);
            streamYoutubeVideo.EnableVideoScreen(false);
            LiveStreamPlayer.SetActive(false);
            streamYoutubeVideo.AVProVideoPlayer.enabled = false;
            streamYoutubeVideo.VideoPlayer.enabled = true;
            NormalPlayer.gameObject.SetActive(true);

            //YoutubeSimplified player = NormalPlayer.GetComponent<YoutubeSimplified>();

            if (NormalPlayer && APIHandler.Data.isPlaying)
            {
                streamYoutubeVideo.VideoId = ExtractVideoIdFromUrl(APIHandler.Data.URL);
                streamYoutubeVideo.IsLive = APIHandler.Data.IsLive;
                streamYoutubeVideo.PlayVideo();
                SetBGMAudioSound();
                //NormalPlayer.url = APIHandler.Data.URL;
                //NormalPlayer.Play();
                //streamYoutubeVideo.StreamYtVideo(APIHandler.Data.URL, APIHandler.Data.IsLive);
            }
            else if (APIHandler.Data.isPlaying == false)
            {
                //player.OnInternetDisconnect();
                NormalPlayer.Stop();
            }

        }
    }
    
    
    public void SetBGMAudioSound()
    {
        if (gameObject.GetComponent<BGMVolumeControlOnTrigger>())
        {
            if (gameObject.GetComponent<BGMVolumeControlOnTrigger>().IsPlayerCollided)
            {
                gameObject.GetComponent<BGMVolumeControlOnTrigger>().SetBGMAudioOnTrigger(true);
            }
        }
    }

    public string ExtractVideoIdFromUrl(string url)
    {
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

        // If "v=" parameter is not found, handle accordingly (e.g., return null or an error message)
        return null;
    }

    public void SetVideoQuality(string _prefQuality)
    {
        switch (_prefQuality)
        {
            case "HIGH":
                streamYoutubeVideo.PreferedQuality = AdvancedYoutubePlayer.Quality.HIGH;
                break;
            case "HD":
                streamYoutubeVideo.PreferedQuality = AdvancedYoutubePlayer.Quality.HD;
                break;
            case "FULLHD":
                streamYoutubeVideo.PreferedQuality = AdvancedYoutubePlayer.Quality.FULLHD;
                break;
            case "UHD":
                streamYoutubeVideo.PreferedQuality = AdvancedYoutubePlayer.Quality.UHD;
                break;
            default:
                streamYoutubeVideo.PreferedQuality = AdvancedYoutubePlayer.Quality.Standard;
                break;
        }
    }

}
