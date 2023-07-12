using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class JjVideo : MonoBehaviour
{
    [SerializeField]
    VideoPlayer videoplayer;
    [SerializeField]
    string videoLink;
    // Start is called before the first frame update
    void OnEnable()
    {
        videoplayer.playOnAwake = false;
        videoplayer.errorReceived += ErrorOnVideo;
        SetPlayer(videoLink);
    }

    void SetPlayer(string link) {
        videoplayer.url = link;
        videoplayer.Play();
    }

    private void ErrorOnVideo(VideoPlayer source, string message)
    {
        SetPlayer(videoLink);
    }

    private void OnDisable()
    {
        videoplayer.errorReceived -= ErrorOnVideo;
    }
}
