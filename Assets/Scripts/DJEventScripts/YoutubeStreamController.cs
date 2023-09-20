﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UnityEngine.Video;
using RenderHeads.Media.AVProVideo;

public class YoutubeStreamController : MonoBehaviour
{
    [SerializeField]
    public GameObject LiveStreamPlayer;
    [SerializeField]
    private GameObject NormalPlayer;
    [SerializeField]
    private YoutubeAPIHandler APIHandler;
    private YoutubeStreamController Instance;
    public AudioSource videoPlayerAudioSource;
    public AudioSource mediaPlayerAudioSource;

    private string PrevURL;
    private bool IsOldURL = true;
    public static Action playPrercordedVideo;
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
        YoutubeSimplified player = NormalPlayer.GetComponent<YoutubeSimplified>();
        player.url = APIHandler.Data.URL;
        player.Play();
    }

    private void Awake()
    {

        Instance = this;
        if (SoundManager.Instance)
        {
            if (videoPlayerAudioSource)
            {
                SoundManager.Instance.videoPlayerSource = videoPlayerAudioSource;
                SoundManagerSettings.soundManagerSettings.videoSource = videoPlayerAudioSource;
            }
            if (mediaPlayerAudioSource)
            {
                SoundManager.Instance.videoPlayerSource = mediaPlayerAudioSource;
                SoundManagerSettings.soundManagerSettings.videoSource = mediaPlayerAudioSource;
            }
            SoundManager.Instance.livePlayerSource = LiveStreamPlayer.GetComponent<MediaPlayer>();
            SoundManagerSettings.soundManagerSettings.setNewSliderValues();
        }
    }

    private void Start()
    {
        if (videoPlayerAudioSource)
            videoPlayerAudioSource.gameObject.GetComponent<VideoPlayer>().targetMaterialRenderer.material.color = new Color32(57, 57, 57, 255);
        if (NormalPlayer.GetComponent<YoutubeSimplified>().videoPlayer != null)
            NormalPlayer.GetComponent<YoutubeSimplified>().videoPlayer.targetMaterialRenderer.material.color = new Color32(57, 57, 57, 255);
        if (NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer != null)
            NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.GetComponent<ApplyToMesh>().MeshRenderer.sharedMaterial.color = new Color32(57, 57, 57, 255);
#if UNITY_EDITOR && !UNITY_IOS
        if (!FeedEventPrefab.m_EnvName.Contains("BreakingDown Arena") && !FeedEventPrefab.m_EnvName.Contains("XANA FESTIVAL STAGE in Dubai.") && !FeedEventPrefab.m_EnvName.Contains("DJ Event"))
        {
            Vector3 scale = NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale;
            scale.y *= -1;
            NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale = scale;
        }
#endif
#if UNITY_IOS
        if (FeedEventPrefab.m_EnvName.Contains("DJ Event"))
        {
            Vector3 scale = NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale;
            scale.y *= -1;
            NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale = scale;
        }
        if (FeedEventPrefab.m_EnvName.Contains("Xana Festival"))
        {
            Vector3 scale = NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale;
            scale.y *= -1;
            NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale = scale;
        }
        if (FeedEventPrefab.m_EnvName.Contains("XANA Festival Stage"))
        {
            Vector3 scale = NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale;
            scale.y *= -1;
            NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale = scale;
        }
        if (FeedEventPrefab.m_EnvName.Contains("NFTDuel Tournament") || FeedEventPrefab.m_EnvName.Contains("XANA Lobby"))
        {
            Vector3 scale = NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale;
            scale.y *= -1;
            NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale = scale;
        }
#endif
    }

    private IEnumerator SetStream()
    {
        yield return StartCoroutine(APIHandler.GetStream());

        while (APIHandler.Data == null)
        {
            yield return null;
        }

        if (!PrevURL.Equals(APIHandler.Data.URL) && APIHandler.Data.isPlaying)
        {
            PrevURL = APIHandler.Data.URL;
            SetUpStream();
        }
        else if (!APIHandler.Data.isPlaying)
        {
            LiveStreamPlayer.SetActive(false);
            NormalPlayer.SetActive(true);
            YoutubeSimplified player = NormalPlayer.GetComponent<YoutubeSimplified>();

            LiveStreamPlayer.GetComponent<ApplyToMesh>().MeshRenderer.sharedMaterial.color = new Color32(57, 57, 57, 255);
            if (NormalPlayer.GetComponent<YoutubeSimplified>().videoPlayer != null)
                NormalPlayer.GetComponent<YoutubeSimplified>().videoPlayer.targetMaterialRenderer.material.color = new Color32(57, 57, 57, 255);
            if (NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer != null)
                NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.GetComponent<ApplyToMesh>().MeshRenderer.sharedMaterial.color = new Color32(57, 57, 57, 255);

            player.OnInternetDisconnect();
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
        else if (APIHandler.Data.isPlaying && !APIHandler.Data.IsLive && !NormalPlayer.activeInHierarchy)
        {
            SetUpStream();
        }
        else
        {
            LiveStreamPlayer.GetComponent<ApplyToMesh>().MeshRenderer.sharedMaterial.color = new Color32(255, 255, 255, 255);
            if (NormalPlayer.GetComponent<YoutubeSimplified>().videoPlayer != null)
                NormalPlayer.GetComponent<YoutubeSimplified>().videoPlayer.targetMaterialRenderer.material.color = new Color32(255, 255, 255, 255);
            if (NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer != null)
                NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.GetComponent<ApplyToMesh>().MeshRenderer.sharedMaterial.color = new Color32(255, 255, 255, 255);
        }
    }

    private void SetUpStream()
    {
        if (APIHandler.Data.IsLive && APIHandler.Data.isPlaying)
        {
            Debug.Log("Hardik changes check");
            LiveStreamPlayer.SetActive(true);
            NormalPlayer.SetActive(false);



            YoutubePlayerLivestream player = LiveStreamPlayer.GetComponent<YoutubePlayerLivestream>();
            if (player)
            {
                player.GetLivestreamUrl(APIHandler.Data.URL);
            }

            if (!FeedEventPrefab.m_EnvName.Contains("Xana Festival") || !FeedEventPrefab.m_EnvName.Contains("NFTDuel Tournament"))
            {
                NormalPlayer.SetActive(false);
            }

        }
        else
        {

            LiveStreamPlayer.GetComponent<ApplyToMesh>().MeshRenderer.sharedMaterial.color = new Color32(57, 57, 57, 255);

            LiveStreamPlayer.SetActive(false);
            NormalPlayer.SetActive(true);

            YoutubeSimplified player = NormalPlayer.GetComponent<YoutubeSimplified>();

            if (player && APIHandler.Data.isPlaying)
            {
                player.url = APIHandler.Data.URL;
                player.Play();
            }
            else if (APIHandler.Data.isPlaying == false)
            {
                player.OnInternetDisconnect();
            }

        }
    }


}
