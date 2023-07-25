using System.Collections;
using System.Collections.Generic;
using LightShaft.Scripts;
using UnityEngine;
using UnityEngine.Video;

public class YoutubeSimplified : MonoBehaviour
{
    public YoutubePlayer player;

    public string url;
    public bool autoPlay = true;
    public bool fullscreen = true;
    public VideoPlayer videoPlayer;

    /*private void Awake()
    {
        videoPlayer = GetComponentInChildren<VideoPlayer>();
        player = GetComponentInChildren<YoutubePlayer>();
        player.videoPlayer = videoPlayer;
    }*/

    private void Start()
    {
        Metaverse.AvatarManager.OninternetDisconnect += OnInternetDisconnect;
        Metaverse.AvatarManager.OninternetConnected += OnInternetConnect;
    }

    public void Play()
    {
        if (!videoPlayer)
        {
            videoPlayer = GetComponentInChildren<VideoPlayer>();
        }
        videoPlayer.gameObject.SetActive(true);
        if (!player)
        {
            player = GetComponentInChildren<YoutubePlayer>();
            player.videoPlayer = videoPlayer;
        }
        if (fullscreen)
        {
            /*videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
            videoPlayer.aspectRatio = VideoAspectRatio.FitInside;
            videoPlayer.targetCamera = player.mainCamera;*/
            
        }
        player.autoPlayOnStart = autoPlay;
        player.videoQuality = YoutubePlayer.YoutubeVideoQuality.FULLHD;

        if(autoPlay)
            player.Play(url);
    }
    public void OnInternetDisconnect()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }
    }

    public void OnInternetConnect()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Play();
        }
    }
}
