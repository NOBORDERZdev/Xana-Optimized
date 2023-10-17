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
    // Start is called before the first frame update
    void OnEnable()
    {
        //videoplayer.playOnAwake = false;
        //videoplayer.errorReceived += ErrorOnVideo;
        //videoplayer.frameReady += SetSound;
        if (XanaConstants.xanaConstants.EnviornmentName.Contains("XANA Lobby"))
        {
            Invoke(nameof(WaitPlay), 5);
        }
    }


    public void CheckForPlayValidPlayer()
    {
        if (isLiveVideo && liveVideoPlayer != null)
        {
            if (liveVideoPlayer)
                liveVideoPlayer.SetActive(true);
            if (preRecordedPlayer)
                preRecordedPlayer.SetActive(false);
            liveVideoPlayer.GetComponent<YoutubePlayerLivestream>()._livestreamUrl = videoLink;
            liveVideoPlayer.GetComponent<YoutubePlayerLivestream>().GetLivestreamUrl(videoLink);
            liveVideoPlayer.GetComponent<YoutubePlayerLivestream>().mPlayer.Play();
            awsVideoplayer.gameObject.SetActive(false);


        }
        else if (isPrerecoreded && preRecordedPlayer != null)
        {
            if (liveVideoPlayer)
                liveVideoPlayer.SetActive(false);
            if (preRecordedPlayer)
                preRecordedPlayer.SetActive(true);
            awsVideoplayer.gameObject.SetActive(false);
            liveVideoPlayer.GetComponent<YoutubeSimplified>().url = videoLink;
            liveVideoPlayer.GetComponent<YoutubeSimplified>().Play();
        }
        else if (isFromAws && awsVideoplayer.gameObject != null)
        {
           // Debug.LogError(videoLink);
            if (liveVideoPlayer)
                liveVideoPlayer.SetActive(false);
            if (preRecordedPlayer)
                preRecordedPlayer.SetActive(false);
            awsVideoplayer.SetActive(true);
            awsVideoplayer.GetComponent<VideoPlayer>().playOnAwake = true;
            awsVideoplayer.GetComponent<VideoPlayer>().isLooping = true;
            awsVideoplayer.GetComponent<VideoPlayer>().url = videoLink;
            awsVideoplayer.GetComponent<VideoPlayer>().Play();
        }
    }


    void WaitPlay()
    {
        SetPlayer(videoLink);
    }

    public void SetPlayer(string link)
    {
        awsVideoplayer.GetComponent<VideoPlayer>().url = link;
        awsVideoplayer.GetComponent<VideoPlayer>().Play();
    }

    void SetSound(VideoPlayer source, string message)
    {

    }

    private void ErrorOnVideo(VideoPlayer source, string message)
    {
        SetPlayer(videoLink);
    }

    private void OnDisable()
    {
        awsVideoplayer.GetComponent<VideoPlayer>().errorReceived -= ErrorOnVideo;
    }
}
