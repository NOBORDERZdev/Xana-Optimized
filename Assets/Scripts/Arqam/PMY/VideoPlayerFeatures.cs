using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using Photon.Realtime;

public class VideoPlayerFeatures : MonoBehaviour
{
    public TextMeshProUGUI durationText;
    public Slider progressSlider;

    private VideoPlayer _videoPlayer;
    private bool _isPauseVideo = false;

    // Start is called before the first frame update
    void Awake()
    {
        progressSlider.gameObject.SetActive(false);
        _videoPlayer = GetComponent<VideoPlayer>();
        this.gameObject.AddComponent<Button>().onClick.AddListener(PauseUnpauseVideo);
        this.gameObject.GetComponent<RawImage>().raycastTarget = true;
    }

    private void OnEnable()
    {
        _videoPlayer.prepareCompleted += OnVideoPrepared;
    }
    private void OnDisable()
    {
        _videoPlayer.prepareCompleted -= OnVideoPrepared;
    }

    private void PauseUnpauseVideo()
    {
        if (_isPauseVideo)
        {
            _isPauseVideo = false;
            _videoPlayer.Play();
        }
        else if (!_isPauseVideo)
        {
            _isPauseVideo = true;
            _videoPlayer.Pause();
        }
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        // Update duration text when the video is prepared
        double videoDuration = _videoPlayer.length;
        durationText.text = FormatTime(videoDuration);
        progressSlider.maxValue = (float)videoDuration;
        progressSlider.gameObject.SetActive(true);
    }

    private string FormatTime(double time)
    {
        int minutes = Mathf.FloorToInt((float)time / 60F);
        int seconds = Mathf.FloorToInt((float)time - minutes * 60);
        return $"{minutes:00}:{seconds:00}";
    }

    // Update is called once per frame
    void Update()
    {
        if (_videoPlayer.isPlaying)
        {
            UpdateVideoProgress();
        }
    }

    private void UpdateVideoProgress()
    {
        double currentTime = _videoPlayer.time;
        durationText.text = $"{FormatTime(currentTime)} / {FormatTime(_videoPlayer.length)}";
        progressSlider.value = (float)currentTime;
    }

}
