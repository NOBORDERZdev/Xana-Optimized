using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class JjVideo : MonoBehaviour
{
    public GameObject awsVideoplayer;
    public GameObject liveVideoPlayer;
    public GameObject preRecordedPlayer;

    public string videoLink;

    public bool isLiveVideo;
    public bool isPrerecoreded;
    public bool isFromAws;

    MeshRenderer screenMesh;
    void OnEnable()
    {
        screenMesh = GetComponent<MeshRenderer>();
        if (ConstantsHolder.xanaConstants.EnviornmentName.Contains("XANA Lobby"))
        {
            Invoke(nameof(WaitPlay), 5);
        }
        else if (ConstantsHolder.xanaConstants.EnviornmentName.Contains("FIVE ELEMENTS"))
        {
            if (screenMesh)
                screenMesh.enabled = false;
        }
    }

    public void CheckForPlayValidPlayer()
    {
        if (isLiveVideo && liveVideoPlayer != null)
        {
            liveVideoPlayer?.SetActive(true);
            preRecordedPlayer?.SetActive(false);
            liveVideoPlayer.GetComponent<YoutubePlayerLivestream>()._livestreamUrl = videoLink;
            liveVideoPlayer.GetComponent<YoutubePlayerLivestream>().GetLivestreamUrl(videoLink);
            liveVideoPlayer.GetComponent<YoutubePlayerLivestream>().mPlayer.Play();
            awsVideoplayer.gameObject.SetActive(false);
        }
        else if (isPrerecoreded && preRecordedPlayer != null)
        {
            liveVideoPlayer?.SetActive(false);
            preRecordedPlayer?.SetActive(true);
            awsVideoplayer.gameObject.SetActive(false);
            liveVideoPlayer.GetComponent<YoutubeSimplified>().url = videoLink;
            liveVideoPlayer.GetComponent<YoutubeSimplified>().Play();
        }
        else if (isFromAws && awsVideoplayer != null)
        {
            liveVideoPlayer?.SetActive(false);
            preRecordedPlayer?.SetActive(false);
            awsVideoplayer.SetActive(true);
            VideoPlayer videoPlayer = awsVideoplayer.GetComponent<VideoPlayer>();
            videoPlayer.playOnAwake = true;
            videoPlayer.isLooping = true;
            videoPlayer.url = videoLink;
            videoPlayer.Play();
        }
    }

    void WaitPlay()
    {
        SetPlayer(videoLink);
    }

    void SetPlayer(string link)
    {
        VideoPlayer videoPlayer = awsVideoplayer.GetComponent<VideoPlayer>();
        videoPlayer.url = link;
        videoPlayer.Play();
    }

    void SetSound(VideoPlayer source, string message)
    {
        // Handle sound settings if needed
    }

    private void ErrorOnVideo(VideoPlayer source, string message)
    {
        SetPlayer(videoLink);
    }

    private void OnDisable()
    {
        if (awsVideoplayer)
            awsVideoplayer.GetComponent<VideoPlayer>().errorReceived -= ErrorOnVideo;
    }
}
