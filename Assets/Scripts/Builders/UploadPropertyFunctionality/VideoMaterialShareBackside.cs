using UnityEngine;
using UnityEngine.Video;

public class VideoMaterialShareBackside : MonoBehaviour
{
    [SerializeField]
    MeshRenderer MeshVideoPlayer;
    [SerializeField]
    MeshRenderer MeshBackside;

    [SerializeField]
    VideoPlayer YoutubeVideoPlayer; // Reference to the VideoPlayer

    public bool IsLive = false; // Flag to determine if sharing materials directly

    private void Awake()
    {
        if (IsLive)
        {
            // Share materials directly
            MeshBackside.sharedMaterial = MeshVideoPlayer.sharedMaterial;
        }
    }
    void OnEnable()
    {
        if (YoutubeVideoPlayer) YoutubeVideoPlayer.started += VideoPlayerStarted;
    }

    private void OnDisable()
    {
        if (YoutubeVideoPlayer) YoutubeVideoPlayer.started -= VideoPlayerStarted;
    }

    private void VideoPlayerStarted(VideoPlayer source)
    {
        // Set the texture from the video player as the base map for the backside material
        MeshBackside.material.SetTexture("_BaseMap", source.texture);
    }
}