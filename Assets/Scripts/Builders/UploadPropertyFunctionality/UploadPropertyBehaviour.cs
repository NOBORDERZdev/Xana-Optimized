using UnityEngine;
using RenderHeads.Media.AVProVideo;
using UnityEngine.Video;

public class UploadPropertyBehaviour : MonoBehaviour
{
    public string id;
    public string url;
    public string localFileName;
    public bool liveStream;
    public bool addFrame;
    public bool isRepeat;
    //public YoutubePlayerLivestream youtubePlayerLivestream;
    //public YoutubePlayer youtubePlayer;
    public MediaPlayer mediaPlayer;
    public VideoPlayer videoPlayer;
    public MediaPlayer feedMediaPlayer;
    public DisplayImage displayImage;
    public GameObject loadingScreen;
    //[HideInInspector] public int index;
    public MediaTypeBuilder mediaType;
    public StreamYoutubeVideo streamYoutubeVideo;
    AudioSource videoAudioSource;

    private void OnEnable()
    {
        BuilderEventManager.YoutubeVideoLoadedCallback += TurnOffLoading;
        BuilderEventManager.BGMVolume += BGMVolume;
        feedMediaPlayer.Events.AddListener(HandleEvent);
        videoAudioSource = videoPlayer.GetComponent<AudioSource>();
        mediaPlayer.AudioVolume = SoundSettings.soundManagerSettings.totalVolumeSlider.value;
        videoAudioSource.volume = SoundSettings.soundManagerSettings.totalVolumeSlider.value;
    }

    private void OnDisable()
    {
        BuilderEventManager.YoutubeVideoLoadedCallback -= TurnOffLoading;
        BuilderEventManager.BGMVolume -= BGMVolume;
        feedMediaPlayer.Events.RemoveAllListeners();
    }


    private void Start()
    {
        switch (mediaType)
        {
            case MediaTypeBuilder.YouTube:
                {
                    //if (liveStream)
                    //    PlayLiveYoutubeVideo();
                    //else
                    //    PlayYoutubeVideo();
                    PlayYTvideo();
                    break;
                }
            case MediaTypeBuilder.LocalVideo:
                PlayUploadedVideo();
                break;
            case MediaTypeBuilder.LocalImage:
                DisplayUploadedImage();
                break;
        }
    }

    void BGMVolume(float value)
    {
        mediaPlayer.AudioVolume = value;
        videoAudioSource.volume = value;
    }

    void PlayYTvideo()
    {
        videoPlayer.gameObject.SetActive(!liveStream);
        mediaPlayer.gameObject.SetActive(liveStream);
        videoPlayer.isLooping = isRepeat;
        if (streamYoutubeVideo != null)
        {
            streamYoutubeVideo.id = id;
            streamYoutubeVideo.StreamYtVideo(url, liveStream);
        }
    }

    //public void PlayYoutubeVideo()
    //{
    //    ResetPlayer();
    //    youtubePlayer.gameObject.SetActive(true);
    //    youtubePlayerLivestream.gameObject.SetActive(false);
    //    youtubePlayer.videoPlayer.isLooping = isRepeat;
    //    youtubePlayer.uploadPropertyID = id;
    //    new Delayed.Action(() => youtubePlayer.Play(url), 1f);
    //}
    //void ResetPlayer()
    //{
    //    youtubePlayer.loadYoutubeUrlsOnly = false;
    //}
    void TurnOffLoading(string _id)
    {
        if (_id != id)
            return;
        if (loadingScreen) loadingScreen.SetActive(false);
    }

    void HandleEvent(MediaPlayer mp, MediaPlayerEvent.EventType eventType, ErrorCode code)
    {
        Debug.Log("MediaPlayer " + mp.Info + " generated event: " + eventType.ToString());
        if (eventType == MediaPlayerEvent.EventType.Error)
        {
            loadingScreen.SetActive(true);
        }
        else if (eventType == MediaPlayerEvent.EventType.FirstFrameReady)
        {
            if (loadingScreen) loadingScreen.SetActive(false);
            feedMediaPlayer.VideoPrepared?.Invoke(feedMediaPlayer);
        }
    }
    public void PlayUploadedVideo()
    {
        videoPlayer.gameObject.SetActive(false);
        feedMediaPlayer.gameObject.SetActive(true);
        feedMediaPlayer.OpenMedia(new MediaPath(url, MediaPathType.AbsolutePathOrURL), autoPlay: false);
        feedMediaPlayer.VideoPrepared += OnVideoPrepared;
        feedMediaPlayer.Play();
        feedMediaPlayer.Loop = isRepeat;
    }
    public void OnLeftClick()
    {
        Debug.LogError("I was Left Clicked");
    }
    //public void PlayLiveYoutubeVideo()
    //{
    //    youtubePlayer.gameObject.SetActive(false);
    //    new Delayed.Action(() =>
    //    {
    //        youtubePlayerLivestream.gameObject.SetActive(true);
    //        youtubePlayerLivestream._livestreamUrl = url;
    //        youtubePlayerLivestream.GetLivestreamUrl(url);
    //        youtubePlayerLivestream.mPlayer.Play();
    //        youtubePlayerLivestream.mPlayer.Loop = isRepeat;
    //    }, 1); // 1 second delay is given because on first time Live streaming was not working

    //}
    public void CheckMediaScreen(string _id, MediaTypeBuilder mediaType, string _url)
    {
        if (id == _id)
        {
            url = _url;
            switch (mediaType)
            {
                case MediaTypeBuilder.YouTube:
                    DisplayYoutubeVideo();
                    break;
                case MediaTypeBuilder.LocalVideo:
                    DisplayLocalVideo();
                    break;
                case MediaTypeBuilder.LocalImage:
                    DisplayUploadedImage();
                    break;
            }
        }
    }
    public void DisplayUploadedImage()
    {
        videoPlayer.gameObject.SetActive(false);
        feedMediaPlayer.gameObject.SetActive(false);
        displayImage.gameObject.SetActive(true);
        loadingScreen.SetActive(true);
        DisplayImage(id);
    }
    public void DisplayImage(string _id)
    {
        if (_id == id)
        {
            displayImage.ResetImgage();
            _ = displayImage.DownloadImageFromURL(url);
        }
    }
    public void DisplayYoutubeVideo()
    {
        if (liveStream)
            mediaPlayer.gameObject.SetActive(true);
        else
            videoPlayer.gameObject.SetActive(true);

        feedMediaPlayer.gameObject.SetActive(false);
        displayImage.gameObject.SetActive(false);
        loadingScreen.SetActive(true);
    }
    public void DisplayLocalVideo()
    {
        displayImage.gameObject.SetActive(false);
        videoPlayer.gameObject.SetActive(false);
        mediaPlayer.enabled = false;
        feedMediaPlayer.gameObject.SetActive(true);
        loadingScreen.SetActive(true);
        feedMediaPlayer.OpenMedia(new MediaPath(url, MediaPathType.AbsolutePathOrURL), autoPlay: false);

        feedMediaPlayer.VideoPrepared += OnVideoPrepared;
    }
    private void OnVideoPrepared(MediaPlayer mp)
    {
        //// Get the video width and height
        //float videoWidth = mp.Info.GetVideoWidth();
        //float videoHeight = mp.Info.GetVideoHeight();

        //// Calculate desired dimensions based on video's aspect ratio
        //float videoAspectRatio = videoWidth / videoHeight;
        //float quadAspectRatio = feedMediaPlayer.gameObject.transform.localScale.x / feedMediaPlayer.gameObject.transform.localScale.y;
        //float newScaleX, newScaleY;

        //if (videoAspectRatio >= quadAspectRatio)
        //{
        //    newScaleX = feedMediaPlayer.gameObject.transform.localScale.x;
        //    newScaleY = newScaleX / videoAspectRatio;
        //}
        //else
        //{
        //    newScaleY = feedMediaPlayer.gameObject.transform.localScale.y;
        //    newScaleX = newScaleY * videoAspectRatio;
        //}
        //if (gameObject.GetComponentInParent<BoxCollider>() != null)
        //    gameObject.GetComponentInParent<BoxCollider>().size = new Vector3(newScaleX, newScaleY, gameObject.GetComponentInParent<BoxCollider>().size.z);
        //// Update the scale of the feedMediaPlayer.gameObject
        //feedMediaPlayer.gameObject.transform.localScale = new Vector3(newScaleX, newScaleY, 1f);

        // Stop listening to the event to avoid multiple calls
        feedMediaPlayer.VideoPrepared -= OnVideoPrepared;

        // Start playing the video
        //feedMediaPlayer.Play();
    }
}