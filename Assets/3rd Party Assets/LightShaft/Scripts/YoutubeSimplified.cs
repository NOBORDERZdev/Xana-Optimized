using System.Collections;
using System.Collections.Generic;
using LightShaft.Scripts;
using RenderHeads.Media.AVProVideo;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class YoutubeSimplified : MonoBehaviour
{
    public YoutubePlayer player;
    public MediaPlayer mPlayer;
    public string url;
    public bool autoPlay = true;
    public bool fullscreen = true;
    public VideoPlayer videoPlayer;
    public float x;
    public float y;
    public float z;
    [Space(5)]
    public bool isEnableBtn = true;

    /*private void Awake()
    {
        videoPlayer = GetComponentInChildren<VideoPlayer>();
        player = GetComponentInChildren<YoutubePlayer>();
        player.videoPlayer = videoPlayer;
    }*/

    private void Start()
    {
        AvatarSpawnerOnDisconnect.OninternetDisconnect += OnInternetDisconnect;
        AvatarSpawnerOnDisconnect.OninternetConnected += OnInternetConnect;
    }

    public void Play()
    {
        if (!videoPlayer)
        {
            videoPlayer = GetComponentInChildren<VideoPlayer>();
        }
        videoPlayer.gameObject.SetActive(true);
        if (mPlayer != null)
        {
            mPlayer.gameObject.SetActive(true);
        }
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
        //player.videoQuality = YoutubePlayer.YoutubeVideoQuality.FULLHD;

        if (autoPlay)
            player.Play(url);

        if (videoPlayer.gameObject.GetComponent<Button>() is null) return;
        videoPlayer.gameObject.GetComponent<Button>().enabled = isEnableBtn;
    }
    public void OnInternetDisconnect()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }
        if (mPlayer != null)
        {
            mPlayer.Stop();
        }
    }

    public void OnInternetConnect()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Play();
        }
        if (mPlayer != null)
        {
            mPlayer.Play();
        }
    }
}
