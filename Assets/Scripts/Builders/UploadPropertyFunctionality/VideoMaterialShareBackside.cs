using UnityEngine;
using UnityEngine.Video;

public class VideoMaterialShareBackside : MonoBehaviour
{
    public bool IsLive = false;

    [SerializeField]
    MeshRenderer _meshVideoPlayer;
    [SerializeField]
    MeshRenderer _meshBackside;

    [SerializeField]
    VideoPlayer _youtubeVideoPlayer;

   

    private void Awake()
    {
        if (IsLive)
        {
            _meshBackside.sharedMaterial = _meshVideoPlayer.sharedMaterial;
        }
    }
    void OnEnable()
    {
        if (_youtubeVideoPlayer) _youtubeVideoPlayer.started += VideoPlayerStarted;
    }

    private void OnDisable()
    {
        if (_youtubeVideoPlayer) _youtubeVideoPlayer.started -= VideoPlayerStarted;
    }

    private void VideoPlayerStarted(VideoPlayer source)
    {
        _meshBackside.material.SetTexture("_BaseMap", source.texture);
    }
}