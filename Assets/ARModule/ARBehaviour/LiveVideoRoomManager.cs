using DG.Tweening;
using NatCorder;
using NatCorder.Clocks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
//using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
//using UnityEngine.XR.ARFoundation;

public class LiveVideoRoomManager : MonoBehaviour
{
	public static LiveVideoRoomManager Instance;

    //public ARPoseDriver _aRPoseDriver;
    public AvatarScript _avatarScript; 

    public Image BackgroundImage;
	
	//public List<PostProcessProfile> filterVolumeProfile = new List<PostProcessProfile>();
	//public PostProcessVolume mainVolume;
    public Volume filterMainVolume; 
	public GameObject videoPlayer;
	public GameObject videoPlayScreen;
    public GameObject videoPlayerUIScreen;
	public GameObject VideoRawImage;
    public GameObject EmojiSelectionScreen;

    [Header("Image Selection Screen Reference")]
    public GameObject imageSelectionScreen;
    public GameObject imageSelectionUIScreen;
    public Image GalleryImage;

    public List<GameObject> imageSelectionUIObjList = new List<GameObject>();
    public List<GameObject> videoRecordingUIObjList = new List<GameObject>();

    public AudioSource audioSource;

    public  bool isRecoring = false;
    double videoTime;

    public int lastAvatarListCount;

    public bool IsVideoScreenImageScreenAvtive = false;

    private void Awake()
    {
        if (Instance == null)
        {
			Instance = this;
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "ARModuleRealityScene")
        {
            _avatarScript = ARFaceModuleManager.Instance.mainAvatar.GetComponent<AvatarScript>();
        }
        BackgroundImage.GetComponent<AspectRatioFitter>().aspectRatio = 0.57f;
    }

    private void Update()
    {
        if (isRecoring)
        {
            videoTime -= Time.deltaTime;
            if (videoTime <= 0)
            {
                isRecoring = false;
                OnSaveVideoTOGallary(videoPath);
            }
        }
    }

    public void OnStartVideoPlay(string url, bool isFromGallery, bool isPickVideo = false)
    {
        Debug.LogError("OnStartVideoPlay:" + url);
        // videoPlayer.GetComponent<VideoPlayer>().Prepare();

        //videoPlayer.GetComponent<VideoPlayer>().url = url;
        isPickVideoFromGellary = isFromGallery;
        VideoRawImage.SetActive(false);
        StartCoroutine(playVideo(url, isPickVideo));
        //videoPlayScreen.SetActive(true);
    }
    IEnumerator playVideo(string url, bool isPickVideo)
    {
        //Add VideoPlayer to the GameObject
        VideoPlayer currentVideoPlayer = videoPlayer.GetComponent<VideoPlayer>();

        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        currentVideoPlayer.targetTexture = renderTexture;

        //Assign the Texture from Video to RawImage to be displayed
        VideoRawImage.GetComponent<RawImage>().texture = renderTexture;

        //Disable Play on Awake for both Video and Audio
        currentVideoPlayer.playOnAwake = false;
        audioSource.playOnAwake = false;

        //We want to play from video clip not from url
        currentVideoPlayer.source = VideoSource.VideoClip;

        //Set Audio Output to AudioSource
        currentVideoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        //Assign the Audio from Video to AudioSource to be played
        currentVideoPlayer.EnableAudioTrack(0, true);
        currentVideoPlayer.SetTargetAudioSource(0, audioSource);

        //Set video To Play then prepare Audio to prevent Buffering
        currentVideoPlayer.url = url;
        currentVideoPlayer.Prepare();
        //Wait until video is prepared

        double videoLength = 0;
#if UNITY_EDITOR
        while (!currentVideoPlayer.isPrepared)
        {
            Debug.Log("Preparing Video");
            yield return null;
        }
        videoLength = currentVideoPlayer.length;
        Debug.Log("Done Preparing Video" + currentVideoPlayer.length);
#else
        videoLength = NativeGallery.VideoLength(url)/1000;
#endif
        if (videoLength > 15 && isPickVideo)
        {
            ARFaceModuleManager.Instance.ShowLoader(false);
            SNSWarningMessageManager.Instance.ShowWarningMessage("Please upload video upto 15 seconds");
        }
        else
        {
            if (isPickVideo)//if pick video button comes then disable this ui.......
            {
                ARFaceModuleManager.Instance.ShowLoader(false);
                if (ARFaceModuleManager.Instance.mainAvatar != null)
                {
                    ARFaceModuleManager.Instance.mainAvatar.gameObject.SetActive(false);
                }
                ARFaceModuleManager.Instance.DisableBottomMainPanel(false);
            }

            videoPath = url;//pick video path store.......        

            filterMainVolume.gameObject.SetActive(false);
            videoPlayScreen.SetActive(true);
            videoPlayerUIScreen.SetActive(true);
            GetLastAvatarListCount();

            //Play Video
            currentVideoPlayer.Play();
            yield return new WaitForSeconds(0.5f);
            VideoRawImage.SetActive(true);
            //Play Sound
            audioSource.Play();

            Debug.Log("Playing Video");
            while (currentVideoPlayer.isPlaying)
            {
                //Debug.LogWarning("Video Time: " + Mathf.FloorToInt((float)videoPlayer.GetComponent<VideoPlayer>().time));
                yield return null;
            }

            Debug.Log("Done Playing Video");
        }
    }

    public bool isPickVideoFromGellary = false;
    public void CloseVideoScreenBtnClick()
    {
        filterMainVolume.gameObject.SetActive(true);
        videoPlayer.GetComponent<VideoPlayer>().Stop();
        videoPlayScreen.SetActive(false);
        videoPlayerUIScreen.SetActive(false);

        if (File.Exists(videoPath) && !isPickVideoFromGellary)
        {
            File.Delete(videoPath);
        }
        ResetLastAvatarListCount();
        isPickVideoFromGellary = false;
    }

    public void CloseImageScreenBtnClick()
    {
        imageSelectionScreen.SetActive(false);
        imageSelectionUIScreen.SetActive(false);

        ResetLastAvatarListCount();
    }

    public void ImageSelectionAllUIDisable(bool isDisable)
    {
        for (int i = 0; i < imageSelectionUIObjList.Count; i++)
        {
            imageSelectionUIObjList[i].SetActive(isDisable);
        }
    }

    public void OnSaveImageSelectionBtnClick(int id)
    {
        ARFaceModuleManager.Instance.OnCaptureButtonClick(id);
    }

    public void OnClickSaveVideoButton()
    {
        RecordVideoBehaviour.instance.StartRecording();
        videoTime = videoPlayer.GetComponent<VideoPlayer>().length;
        Debug.LogError("videoTime" + videoTime); 
        isRecoring = true;        
    }

    public string videoPath;
    public void OnSaveVideoTOGallary(string path)
    {
        path = videoPath;
        if (!isRecoring)
        {
            OnStartVideoPlay(path, true);
            NativeGallery.SaveVideoToGallery(path, "XanaVideo", "Video");
            VideoRecordingAllUIDisable(true);
        }
    }

    public void OnClickSelectAvtarBtnOnVideoEditScreen()
    {
        ARFaceModuleManager.Instance.OnCharacterSelectionBtnClick();
    }

    public void OnEmojiSelectionBtnClick()
    {
        EmojiSelectionScreen.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -2000);
        EmojiSelectionScreen.SetActive(true);
        EmojiSelectionScreen.GetComponent<RectTransform>().DOAnchorPosY(0, 0.2f).SetEase(Ease.Linear);
    }

    public void OnScreenCloseBtnClick()
    {
        if (EmojiSelectionScreen.activeSelf)
        {
            EmojiSelectionScreen.GetComponent<RectTransform>().DOAnchorPosY(-2000, 0.2f).SetEase(Ease.Linear); 
            StartCoroutine(CloseOpenScreen(EmojiSelectionScreen));
        }       
    }

    public void VideoRecordingAllUIDisable(bool isDisable)
    {
        for (int i = 0; i < videoRecordingUIObjList.Count; i++)
        {
            //videoRecordingUIObjList[i].SetActive(isDisable);//temp cmnt
        }
    }
    IEnumerator CloseOpenScreen(GameObject openScreen)
    {
        yield return new WaitForSeconds(0.4f);
        openScreen.SetActive(false);
    }

    public void GetLastAvatarListCount()
    {
        IsVideoScreenImageScreenAvtive = true;

        //ARFaceModuleManager.Instance.DisableBottomMainPanel(false);
        if (ARFaceModuleManager.Instance.mainAvatar != null)
        {
            Debug.LogError("GetLastAvatarListCount..... main avatar false");
            ARFaceModuleManager.Instance.mainAvatar.SetActive(false);
        }
        if (ARFaceModuleManager.Instance.addAvtarItem.Count != 0)
        {
            for (int i = 0; i < ARFaceModuleManager.Instance.addAvtarItem.Count; i++)
            {
                ARFaceModuleManager.Instance.addAvtarItem[i].gameObject.SetActive(false);
            }
        }
        lastAvatarListCount = ARFaceModuleManager.Instance.addAvtarItem.Count;

        if (SceneManager.GetActiveScene().name == "ARModulePlanDetectionScene")
        {
            //_aRPoseDriver.enabled = false;
        }
        else if (SceneManager.GetActiveScene().name == "ARModuleRealityScene")
        {
            //ARFacePoseTrackingManager.Instance.isVideoOpen =true;
        }
    }

    public void ResetLastAvatarListCount()
    {
        IsVideoScreenImageScreenAvtive = false;

        ARFaceModuleManager.Instance.DisableBottomMainPanel(true);
               
        if (ARFaceModuleManager.Instance.addAvtarItem.Count != 0)
        {
            for (int i = 0; i < ARFaceModuleManager.Instance.addAvtarItem.Count; i++)
            {
                Debug.LogError("Condition:" + i + " :lastAvatarListCount:" + lastAvatarListCount);
                if (i < lastAvatarListCount)
                {
                    ARFaceModuleManager.Instance.addAvtarItem[i].gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogError("DeletePlayer:" + i);
                    GameObject crntObj = ARFaceModuleManager.Instance.addAvtarItem[i];
                    ARFaceModuleManager.Instance.addAvtarItem.Remove(crntObj);
                    Destroy(crntObj);
                    i--;
                }
            }
        }

        if (ARFaceModuleManager.Instance.videoEditCanvas.transform.childCount != 0)
        {
            for (int i = 0; i < ARFaceModuleManager.Instance.videoEditCanvas.transform.childCount; i++)
            {
                Destroy(ARFaceModuleManager.Instance.videoEditCanvas.transform.GetChild(i).gameObject);
            }
        }

        if (SceneManager.GetActiveScene().name == "ARModulePlanDetectionScene")
        {
            //_aRPoseDriver.enabled = true;
        }
        else if(SceneManager.GetActiveScene().name == "ARModuleRealityScene")
        {
            if (_avatarScript != null)
            {
                _avatarScript.SetDefaultAvatarHipsPos();
            }
            ARFaceModuleManager.Instance.SetDefaultAvatarPosition();
            //ARFacePoseTrackingManager.Instance.SetDefaultMoveTargetObjPos();
        }
        Debug.LogError("ResetLastAvatarListCount.......");
        if (ARFaceModuleManager.Instance.mainAvatar != null && SceneManager.GetActiveScene().name != "ARModulePlanDetectionScene")
        {
            ARFaceModuleManager.Instance.mainAvatar.gameObject.SetActive(true);
        }
    }
} 