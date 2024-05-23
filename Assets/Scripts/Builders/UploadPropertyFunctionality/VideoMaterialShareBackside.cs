using UnityEngine;
using UnityEngine.Video;

public class VideoMaterialShareBackside : MonoBehaviour
{
    [SerializeField]
    MeshRenderer _meshVideoPlayer;
    [SerializeField]
    MeshRenderer _meshBackside;

    [SerializeField]
    VideoPlayer _youtubeVideoPlayer; // Reference to the VideoPlayer

    public bool IsLive = false; // Flag to determine if sharing materials directly

    private void Awake()
    {
        if (IsLive)
        {
            // Share materials directly
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
        // Set the texture from the video player as the base map for the backside material
        _meshBackside.material.SetTexture("_BaseMap", source.texture);
    }
}