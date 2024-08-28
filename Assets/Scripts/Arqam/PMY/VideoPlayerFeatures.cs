using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using System.Security.Policy;

public class VideoPlayerFeatures : MonoBehaviour
{
    public GameObject VideoFeatures;
    public TextMeshProUGUI durationText;
    public Slider progressSlider;
    [Space(5)]
    public Button PauseUnpauseBtn;
    public GameObject PauseSprite;
    public GameObject ResumeSprite;

    private AdvancedYoutubePlayer _advanceYP;
    private VideoPlayer _videoPlayer;
    private bool _isPauseVideo = false;

    private void Start()
    {
        ResumeSprite.SetActive(false);
    }

    private void OnDisable()
    {
        if (_videoPlayer == null) return;

        _videoPlayer.prepareCompleted -= OnVideoPrepared;
        Destroy(_videoPlayer.gameObject.GetComponent<Button>());
        _videoPlayer.gameObject.GetComponent<RawImage>().raycastTarget = false;
        VideoFeatures.SetActive(false);
        progressSlider.value = 0f;
        durationText.text = "";
    }

    public void EnableVideoFeature()
    {
        VideoFeatures.SetActive(true);
        if (_advanceYP == null)
        {
            _advanceYP = GetComponent<AdvancedYoutubePlayer>();
            _videoPlayer = _advanceYP.VideoPlayer;
        }

        _videoPlayer.prepareCompleted += OnVideoPrepared;       
        _videoPlayer.gameObject.GetComponent<RawImage>().raycastTarget = true;
        _videoPlayer.gameObject.AddComponent<Button>().onClick.AddListener(PauseUnpauseVideo);
        PauseUnpauseBtn.onClick.AddListener(PauseUnpauseVideo);
    }

    public void PauseUnpauseVideo()
    {
        if (_isPauseVideo)
        {
            _isPauseVideo = false;
            _videoPlayer.Play();
            PauseSprite.SetActive(true);
            ResumeSprite.SetActive(false);
        }
        else if (!_isPauseVideo)
        {
            _isPauseVideo = true;
            _videoPlayer.Pause();
            PauseSprite.SetActive(false);
            ResumeSprite.SetActive(true);
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

        if (_videoPlayer != null && _videoPlayer.isPlaying)
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
